using Axtox.IoT.Common.Animations.Settings;
using System;

namespace Axtox.IoT.Common.Animations
{
    /// <summary>
    /// Defines a contract for animating a value to a specified target.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IAnimator"/> interface provides a standardized way to perform smooth, 
    /// configurable animations on objects that implement <see cref="IAnimatable"/>. Implementers 
    /// of this interface should manage animation lifecycle, including starting, configuring, 
    /// and aborting animations.
    /// </para>
    /// <para>
    /// Typical usage involves:
    /// <list type="number">
    /// <item>Creating an instance of an <see cref="IAnimator"/> implementation (e.g., <see cref="BackgroundAnimator"/>)</item>
    /// <item>Configuring animation settings via <see cref="Configure"/> (duration, easing, update interval)</item>
    /// <item>Calling <see cref="Animate"/> to start the animation</item>
    /// <item>Optionally calling <see cref="Abort"/> to stop the animation early</item>
    /// <item>Disposing the animator when no longer needed</item>
    /// </list>
    /// </para>
    /// <para>
    /// Implementations should be thread-safe and handle proper resource cleanup through 
    /// the <see cref="IDisposable"/> pattern.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var animator = new BackgroundAnimator();
    /// animator.Configure(settings => {
    ///     settings.DurationInMilliseconds = 500;
    ///     settings.EasingStyle = EasingStyle.EaseInOutQuad;
    ///     settings.UpdateIntervalInMilliseconds = 10;
    /// });
    /// 
    /// IAnimatable ledBrightness = new LedDiode(/* parameters */);
    /// animator.AnimateTo(ledBrightness, new AnimatedValue { Value = 100.0f });
    /// </code>
    /// </example>
    public interface IAnimator : IDisposable
    {
        /// <summary>
        /// Gets the current animation settings for this animator.
        /// </summary>
        /// <value>
        /// An immutable copy of the <see cref="AnimationSettings"/> object containing the current
        /// animation configuration.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property returns a clone of the internal settings to prevent external modifications
        /// and ensure immutability. Changes to the returned object will not affect the animator's
        /// actual settings. To modify animation settings, use the <see cref="Configure"/> method.
        /// </para>
        /// <para>
        /// The returned settings reflect the values that will be used for the next animation started
        /// via <see cref="Animate"/>. If <see cref="Configure"/> has not been called, this will
        /// return the default settings.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// using var animator = new BackgroundAnimator();
        /// animator.Configure(s => s.DurationInMilliseconds = 1000);
        /// 
        /// var currentSettings = animator.Settings;
        /// // Modifying currentSettings will not affect the animator
        /// </code>
        /// </example>
        AnimationSettings Settings { get; }

        /// <summary>
        /// Configures the animation settings using a builder pattern.
        /// </summary>
        /// <param name="settingsBuilderMethod">A delegate that receives an <see cref="AnimationSettings"/> 
        /// instance to configure. Set properties such as <see cref="AnimationSettings.DurationInMilliseconds"/>, 
        /// <see cref="AnimationSettings.EasingStyle"/>, and <see cref="AnimationSettings.UpdateIntervalInMilliseconds"/>.</param>
        /// <exception cref="ConfigureSettingsMissing">Thrown when settings object is null or unavailable.</exception>
        /// <remarks>
        /// This method should be called before starting any animation to ensure desired animation 
        /// characteristics. If not called, default settings will be used.
        /// </remarks>
        void Configure(AnimationSettingsBuilder settingsBuilderMethod);

        /// <summary>
        /// Animates the specified target to the given value. Use <see cref="Configure"/> to set up animation
        /// settings, such as duration and easing style. If not set the default settings will be applied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The animation will transition the target's current state to the specified <paramref
        /// name="toValue"/>. Ensure that the <paramref name="target"/> is not null and is in a valid state to be
        /// animated.
        /// </para>
        /// <para>
        /// If an animation is already in progress, calling this method will replace the current animation 
        /// with a new one targeting the specified values. The previous animation will be interrupted.
        /// </para>
        /// <para>
        /// The animation runs on a background thread and does not block the calling thread.
        /// </para>
        /// </remarks>
        /// <param name="target">The object to be animated. Must implement the <see cref="IAnimatable"/> interface.</param>
        /// <param name="toValue">The target value to animate to. Represents the final state of the animation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is null.</exception>
        void Animate(IAnimatable target, AnimatedValue toValue);

        /// <summary>
        /// Aborts the currently running animation, if any.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method stops the animation immediately. The target will remain at its current intermediate 
        /// state rather than jumping to the final value.
        /// </para>
        /// <para>
        /// This method is thread-safe and can be called from any thread. If no animation is currently 
        /// running, this method has no effect.
        /// </para>
        /// </remarks>
        void Abort();
    }
}
