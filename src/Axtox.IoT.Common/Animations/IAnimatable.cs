namespace Axtox.IoT.Common.Animations
{
    /// <summary>
    /// Defines a contract for objects that can be animated by an <see cref="IAnimator"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IAnimatable"/> interface allows objects to participate in smooth animations
    /// by providing methods to retrieve their current state and update it during animation frames.
    /// Implementers should manage their internal state and ensure thread-safe access when being
    /// animated from background threads.
    /// </para>
    /// <para>
    /// Typical usage involves:
    /// <list type="number">
    /// <item>Implementing <see cref="GetCurrentValue"/> to return the object's current animated state</item>
    /// <item>Implementing <see cref="SetAnimatedValue"/> to apply intermediate animation values</item>
    /// <item>Passing the object to an <see cref="IAnimator"/> instance (e.g., <see cref="BackgroundAnimator"/>)</item>
    /// </list>
    /// </para>
    /// <para>
    /// The animation system uses these methods to smoothly transition from the current value
    /// to a target value over a specified duration with configurable easing functions.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class LedDiode : IAnimatable
    /// {
    ///     private PwmChannel _pwm;
    ///     
    ///     public AnimatedValue GetCurrentValue() 
    ///         => new() { Value = (float)_pwm.DutyCycle };
    ///     
    ///     public void SetAnimatedValue(AnimatedValue value) 
    ///         => _pwm.DutyCycle = value.Value;
    /// }
    /// 
    /// // Usage:
    /// using var animator = new BackgroundAnimator();
    /// animator.Configure(s => s.DurationInMilliseconds = 500);
    /// IAnimatable led = new LedDiode();
    /// animator.Animate(led, new AnimatedValue { Value = 1.0f });
    /// </code>
    /// </example>
    public interface IAnimatable
    {
        /// <summary>
        /// Gets the current animated value of the object.
        /// </summary>
        /// <returns>An <see cref="AnimatedValue"/> representing the object's current state.</returns>
        /// <remarks>
        /// <para>
        /// This method is called by the animator at the start of an animation to determine
        /// the starting point for interpolation. It should return the actual current state
        /// of the object being animated.
        /// </para>
        /// <para>
        /// Implementers should ensure this method is thread-safe as it may be called from
        /// animation background threads.
        /// </para>
        /// </remarks>
        AnimatedValue GetCurrentValue();

        /// <summary>
        /// Sets the animated value during an animation frame.
        /// </summary>
        /// <param name="value">The interpolated value to apply, calculated by the animator
        /// based on elapsed time and easing function.</param>
        /// <remarks>
        /// <para>
        /// This method is called repeatedly during an animation to apply intermediate values
        /// between the starting value and target value. The animator handles all interpolation
        /// and easing calculations.
        /// </para>
        /// <para>
        /// Implementers should apply the value to their internal state immediately and ensure
        /// thread-safe access to shared resources, as this method is called from background threads.
        /// </para>
        /// </remarks>
        void SetAnimatedValue(AnimatedValue value);
    }
}
