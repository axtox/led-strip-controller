namespace Axtox.IoT.Common.Animations.Settings
{
    /// <summary>
    /// Represents the configuration settings for animations performed by <see cref="IAnimator"/> implementations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="AnimationSettings"/> class encapsulates all configurable parameters that control
    /// animation behavior, including duration, update frequency, and easing style. These settings are
    /// applied via the <see cref="IAnimator.Configure"/> method using a builder pattern.
    /// </para>
    /// <para>
    /// Typical usage involves:
    /// <list type="number">
    /// <item>Creating an <see cref="IAnimator"/> instance (e.g., <see cref="BackgroundAnimator"/>)</item>
    /// <item>Calling <see cref="IAnimator.Configure"/> with a delegate that modifies the settings</item>
    /// <item>Starting animations with <see cref="IAnimator.Animate"/>, which will use the configured settings</item>
    /// </list>
    /// </para>
    /// <para>
    /// If not explicitly configured, default values provide a basic linear animation with a short duration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var animator = new BackgroundAnimator();
    /// animator.Configure(settings => {
    ///     settings.DurationInMilliseconds = 1000;
    ///     settings.EasingStyle = EasingStyle.EaseInOutQuad;
    ///     settings.UpdateIntervalInMilliseconds = 16; // ~60 FPS
    /// });
    /// 
    /// IAnimatable target = new LedDiode(/* parameters */);
    /// animator.Animate(target, new AnimatedValue { Value = 1.0f });
    /// </code>
    /// </example>
    public class AnimationSettings
    {
        /// <summary>
        /// Gets or sets the total duration of the animation in milliseconds.
        /// </summary>
        /// <value>
        /// The animation duration in milliseconds. Default is 100ms.
        /// </value>
        /// <remarks>
        /// <para>
        /// This value determines how long the animation takes to complete from start to finish.
        /// Longer durations create slower, more gradual animations, while shorter durations
        /// create faster transitions.
        /// </para>
        /// <para>
        /// The duration must be a positive value. Setting it to 0 will cause the animation
        /// to complete immediately with the target value.
        /// </para>
        /// </remarks>
        public ushort DurationInMilliseconds { get; set; } = 100;

        /// <summary>
        /// Gets or sets the update interval in milliseconds between animation frames.
        /// </summary>
        /// <value>
        /// The interval between animation updates in milliseconds. Default is 5ms (~200 FPS).
        /// </value>
        /// <remarks>
        /// <para>
        /// This value controls how frequently the animated value is recalculated and applied
        /// to the target object. Lower values result in smoother animations but consume more
        /// CPU resources due to more frequent updates.
        /// </para>
        /// <para>
        /// Common values:
        /// <list type="bullet">
        /// <item><description>5ms (~200 FPS) - Very smooth, higher CPU usage</description></item>
        /// <item><description>16ms (~60 FPS) - Smooth, balanced performance</description></item>
        /// <item><description>33ms (~30 FPS) - Acceptable for slower animations, lower CPU usage</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The actual update rate may vary based on system load and thread scheduling.
        /// </para>
        /// </remarks>
        public ushort UpdateIntervalInMilliseconds { get; set; } = 5;

        /// <summary>
        /// Gets or sets the easing function style applied to the animation.
        /// </summary>
        /// <value>
        /// The easing style to use. Default is <see cref="EasingStyle.Linear"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Easing functions control the rate of change during an animation, creating more
        /// natural and visually appealing motion. Different easing styles produce different
        /// acceleration and deceleration patterns.
        /// </para>
        /// <para>
        /// Available easing styles:
        /// <list type="bullet">
        /// <item><description><see cref="EasingStyle.Linear"/> - Constant speed throughout</description></item>
        /// <item><description><see cref="EasingStyle.EaseInQuad"/> - Slow start, accelerates toward end</description></item>
        /// <item><description><see cref="EasingStyle.EaseOutQuad"/> - Fast start, decelerates toward end</description></item>
        /// <item><description><see cref="EasingStyle.EaseInOutQuad"/> - Slow start and end, faster in middle</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public EasingStyle EasingStyle { get; set; } = EasingStyle.Linear;
    }

    /// <summary>
    /// Represents a delegate used to configure <see cref="AnimationSettings"/> via a builder pattern.
    /// </summary>
    /// <param name="settings">The <see cref="AnimationSettings"/> instance to configure.</param>
    /// <remarks>
    /// <para>
    /// This delegate is used by <see cref="IAnimator.Configure"/> to allow fluent configuration
    /// of animation parameters. The delegate receives a mutable settings object that can be
    /// modified to customize animation behavior.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// AnimationSettingsBuilder builder = settings => {
    ///     settings.DurationInMilliseconds = 500;
    ///     settings.EasingStyle = EasingStyle.EaseInOutQuad;
    /// };
    /// 
    /// animator.Configure(builder);
    /// </code>
    /// </example>
    public delegate void AnimationSettingsBuilder(AnimationSettings settings);
}
