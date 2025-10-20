using Axtox.IoT.Common.Animations.Settings;
using Axtox.IoT.Common.Animations.Settings.Exceptions;
using System;
using System.Threading;

namespace Axtox.IoT.Common.Animations
{
    public class BackgroundAnimator : IAnimator
    {
        private const ushort TicksPerMillisecond = 10000;

        private readonly AnimationSettings settings = new();
        private readonly Thread animatingThread;
        private readonly object animationResourcesLock = new();

        protected bool IsAnimating = false;
        protected IAnimatable CurrentAnimatableTarget;
        protected AnimatedValue CurrentTargetValue;

        public BackgroundAnimator()
        {
            animatingThread = new Thread(AnimateThread);
            animatingThread.Start();
        }

        public void Configure(AnimationSettingsBuilder settingsBuilderMethod)
        {
            ThrowIfDisposed();
            settingsBuilderMethod(settings
                ?? throw new ConfigureSettingsMissing($"Configuration is missing for the {nameof(BackgroundAnimator)}."));
        }

        private AutoResetEvent animationStartEvent = new(false);

        public void Animate(IAnimatable target, AnimatedValue toValue)
        {
            ThrowIfDisposed();
            lock (animationResourcesLock)
            {
                IsAnimating = true;
                CurrentAnimatableTarget = target ?? throw new ArgumentNullException(nameof(target));
                CurrentTargetValue = toValue;
                animationStartEvent.Set();
            }
        }

        public void Abort()
        {
            ThrowIfDisposed();
            lock (animationResourcesLock)
            {
                IsAnimating = false;
                CurrentAnimatableTarget = null;
            }
        }

        private void AnimateThread()
        {
            while (alive)
            {
                animationStartEvent.WaitOne();

                IAnimatable localTarget;
                AnimatedValue localToValue;
                float from, to;

                lock (animationResourcesLock)
                {
                    localTarget = CurrentAnimatableTarget;
                    localToValue = CurrentTargetValue;
                    from = localTarget.GetCurrentValue().Value;
                    to = localToValue.Value;
                }

                if (from == to)
                    Abort();

                if (!IsAnimating || CurrentAnimatableTarget == null)
                    continue;

                var easingFunction = EasingFunctions.GetEasingFunction(settings.EasingStyle);
                long startTimeInMilliseconds = DateTime.UtcNow.Ticks / TicksPerMillisecond;

                while (IsAnimating)
                {
                    lock (animationResourcesLock)
                        if (CurrentAnimatableTarget != localTarget || CurrentTargetValue != localToValue)
                            break;

                    long currentTimeInMilliseconds = DateTime.UtcNow.Ticks / TicksPerMillisecond;
                    long elapsedTimeInMilliseconds = currentTimeInMilliseconds - startTimeInMilliseconds;

                    if (from == to || elapsedTimeInMilliseconds >= settings.DurationInMilliseconds)
                    {
                        localTarget.SetAnimatedValue(localToValue);
                        break;
                    }

                    float currentStepTime = (float)elapsedTimeInMilliseconds / settings.DurationInMilliseconds;
                    float easedTime = easingFunction(currentStepTime);
                    float currentValue = from + (to - from) * easedTime;

                    localTarget.SetAnimatedValue(new AnimatedValue { Value = currentValue });
                    Thread.Sleep(settings.UpdateIntervalInMilliseconds);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException($"You are trying to acces already disposed object: {nameof(BackgroundAnimator)}");
        }

        #region Disposable Pattern

        private bool isDisposed;
        private bool alive = true;
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Abort();
                    alive = false;
                    animationStartEvent.Set();
                    animatingThread.Join();
                }

                isDisposed = true;
            }
        }

        ~BackgroundAnimator()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
