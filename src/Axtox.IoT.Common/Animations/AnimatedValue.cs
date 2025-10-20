namespace Axtox.IoT.Common.Animations
{
    /// <summary>
    /// Represents a normalized animation value used by the animation system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="AnimatedValue"/> struct encapsulates a single floating-point value
    /// that represents the state of an animatable property. This value is used by
    /// <see cref="IAnimator"/> implementations (such as <see cref="BackgroundAnimator"/>)
    /// to communicate interpolated animation states to <see cref="IAnimatable"/> objects.
    /// </para>
    /// <para>
    /// The <see cref="Value"/> property is typically normalized to a range of 0.0 to 1.0,
    /// where 0.0 represents the minimum state and 1.0 represents the maximum state.
    /// However, implementers of <see cref="IAnimatable"/> may use any appropriate range
    /// based on their specific use case (e.g., 0-100 for percentage-based values, or
    /// 0-255 for byte-range values).
    /// </para>
    /// <para>
    /// This struct is immutable and uses the <c>init</c> accessor, ensuring that values
    /// are set only during initialization.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Creating an AnimatedValue with normalized value
    /// var targetValue = new AnimatedValue { Value = 1.0f };
    /// 
    /// // Using with animator
    /// using var animator = new BackgroundAnimator();
    /// animator.Configure(s => s.DurationInMilliseconds = 500);
    /// animator.Animate(ledDiode, new AnimatedValue { Value = 0.8f });
    /// </code>
    /// </example>
    public readonly struct AnimatedValue
    {
        /// <summary>
        /// Gets the animation value.
        /// </summary>
        /// <value>
        /// A floating-point value typically in the range of 0.0 to 1.0, where:
        /// <list type="bullet">
        /// <item><description>0.0 represents the minimum state (e.g., off, fully transparent, minimum brightness)</description></item>
        /// <item><description>1.0 represents the maximum state (e.g., on, fully opaque, maximum brightness)</description></item>
        /// </list>
        /// </value>
        /// <remarks>
        /// <para>
        /// While the typical range is 0.0 to 1.0, implementations of <see cref="IAnimatable"/>
        /// may use different ranges based on their specific requirements. The animator does not
        /// enforce these constraints, allowing flexibility in value interpretation.
        /// </para>
        /// <para>
        /// During animation, this value is calculated by the animator using the configured
        /// easing function and is passed to <see cref="IAnimatable.SetAnimatedValue"/> for
        /// each animation frame.
        /// </para>
        /// </remarks>
        public float Value { get; init; }

        /// <summary>
        /// Implicitly converts a <see cref="float"/> value to an <see cref="AnimatedValue"/>.
        /// </summary>
        /// <param name="value">The float value to convert.</param>
        /// <returns>An <see cref="AnimatedValue"/> with the specified value.</returns>
        /// <example>
        /// <code>
        /// AnimatedValue animValue = 0.5f; // Implicit conversion from float
        /// </code>
        /// </example>
        public static implicit operator AnimatedValue(float value) => new() { Value = value };

        /// <summary>
        /// Implicitly converts an <see cref="AnimatedValue"/> to a <see cref="float"/>.
        /// </summary>
        /// <param name="animatedValue">The <see cref="AnimatedValue"/> to convert.</param>
        /// <returns>The <see cref="Value"/> of the <see cref="AnimatedValue"/>.</returns>
        /// <example>
        /// <code>
        /// float value = new AnimatedValue { Value = 0.5f }; // Implicit conversion to float
        /// </code>
        /// </example>
        public static implicit operator float(AnimatedValue animatedValue) => animatedValue.Value;
    }
}
