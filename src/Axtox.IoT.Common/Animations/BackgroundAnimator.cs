using Axtox.IoT.Common.Animations.Settings;
using Axtox.IoT.Common.System.Logging;
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
        private readonly AutoResetEvent animationStartEvent = new(false);
        private readonly ILogger logger;

        protected bool IsAnimating = false;
        protected IAnimatable CurrentAnimatableTarget;
        protected AnimatedValue CurrentTargetValue;

        public AnimationSettings Settings => new()
        {
            DurationInMilliseconds = settings.DurationInMilliseconds,
            EasingStyle = settings.EasingStyle,
            UpdateIntervalInMilliseconds = settings.UpdateIntervalInMilliseconds
        };

        public BackgroundAnimator(ILogger logger)
        {
            animatingThread = new Thread(AnimateThread);
            animatingThread.Start();
            this.logger = logger;
        }

        public void Configure(AnimationSettingsBuilder settingsBuilderMethod)
        {
            ThrowIfDisposed();
            settingsBuilderMethod(settings);
        }

        public void Animate(IAnimatable target, AnimatedValue toValue)
        {
            ThrowIfDisposed();
            lock (animationResourcesLock)
            {
                if (IsAnimating)
                    logger.LogWarning($"Multiple animate calls detected -" +
                        $" changing the target or the target value on the fly is not fully supported.");

                IsAnimating = true;
                CurrentAnimatableTarget = target ?? throw new ArgumentNullException(nameof(target));
                CurrentTargetValue = toValue;
                animationStartEvent.Set();

                logger.LogInfo($"Started animation to value {toValue.Value}");
            }
        }

        public void Abort()
        {
            ThrowIfDisposed();
            lock (animationResourcesLock)
            {
                IsAnimating = false;
                CurrentAnimatableTarget = null;

                logger.LogInfo("Animation finished.");
            }
        }

        private void AnimateThread()
        {
            while (alive)
            {
                animationStartEvent.WaitOne();

                IAnimatable target;
                AnimatedValue toValue;
                float from, to;

                lock (animationResourcesLock)
                {
                    if (!alive)
                        return;

                    target = CurrentAnimatableTarget;
                    toValue = CurrentTargetValue;

                    if (!IsAnimating || CurrentAnimatableTarget == null)
                        continue;

                    from = target.GetCurrentValue().Value;
                    to = toValue.Value;
                }

                if (Math.Abs(from - to) <= 0.0001)
                    Abort();

                var easingFunction = EasingFunctions.GetEasingFunction(settings.EasingStyle);
                long startTimeInMilliseconds = DateTime.UtcNow.Ticks / TicksPerMillisecond;

                while (IsAnimating)
                {
                    lock (animationResourcesLock)
                        if (IsAnimating && CurrentAnimatableTarget != target)
                            break;

                    if (!IsAnimating)
                        break;

                    long currentTimeInMilliseconds = DateTime.UtcNow.Ticks / TicksPerMillisecond;
                    long elapsedTimeInMilliseconds = currentTimeInMilliseconds - startTimeInMilliseconds;

                    if (Math.Abs(from - to) <= 0.0001 || elapsedTimeInMilliseconds >= settings.DurationInMilliseconds)
                    {
                        target.SetAnimatedValue(toValue);
                        break;
                    }

                    float progress = (float)elapsedTimeInMilliseconds / settings.DurationInMilliseconds;
                    float easingFactor = easingFunction(progress);
                    float animatedValue = from + (to - from) * easingFactor;

                    target.SetAnimatedValue(new AnimatedValue { Value = animatedValue });
                    Thread.Sleep(settings.UpdateIntervalInMilliseconds);
                }
            }
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

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException($"You are trying to access already disposed object: {nameof(BackgroundAnimator)}");
        }
        #endregion
    }
}
