using System;

namespace Axtox.IoT.Common.Animations
{
    /// <summary>
    /// Defines the easing style for animations
    /// </summary>
    public enum EasingStyle
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad
    }

    /// <summary>
    /// Represents a function that applies an easing transformation to a value.
    /// </summary>
    /// <remarks>Easing functions are commonly used in animations to create smooth transitions by modifying
    /// the rate of change of a value over time. The specific transformation applied depends on the implementation of
    /// the delegate.</remarks>
    /// <param name="value">A value between 0 and 1, where 0 typically represents the start of the transition and 1 represents the end.</param>
    /// <returns>A transformed value, typically between 0 and 1, that represents the eased result of the input value.</returns>
    public delegate float EasingFunction(float value);

    /// <summary>
    /// Provides easing functions for smooth animations.
    /// All easing functions accept a normalized time value (0.0 to 1.0) representing animation progress,
    /// and return a normalized eased value (0.0 to 1.0) with the applied easing curve.
    /// </summary>
    public static class EasingFunctions
    {
        /// <summary>
        /// Linear interpolation (no easing).
        /// </summary>
        /// <param name="progress">Normalized progress value between 0.0 and 1.0, where 0.0 is the start and 1.0 is the end of the animation.</param>
        /// <returns>The same value (linear progression).</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not between 0.0 and 1.0.</exception>
        public static float Linear(float progress)
        {
            if (progress < 0f || progress > 1f)
                throw new ArgumentOutOfRangeException(nameof(progress), "Value must be between 0.0 and 1.0.");

            return progress;
        }

        /// <summary>
        /// Quadratic easing in (slow start, fast end).
        /// Creates an accelerating curve using the formula: value².
        /// </summary>
        /// <param name="progress">Normalized progress value between 0.0 and 1.0, where 0.0 is the start and 1.0 is the end of the animation.</param>
        /// <returns>Eased value between 0.0 and 1.0 with slow start and fast end.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not between 0.0 and 1.0.</exception>
        public static float EaseInQuad(float progress)
        {
            if (progress < 0f || progress > 1f)
                throw new ArgumentOutOfRangeException(nameof(progress), "Value must be between 0.0 and 1.0.");

            return progress * progress;
        }

        /// <summary>
        /// Quadratic easing out (fast start, slow end).
        /// Creates a decelerating curve using the formula: value × (2 - value).
        /// </summary>
        /// <param name="progress">Normalized progress value between 0.0 and 1.0, where 0.0 is the start and 1.0 is the end of the animation.</param>
        /// <returns>Eased value between 0.0 and 1.0 with fast start and slow end.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not between 0.0 and 1.0.</exception>
        public static float EaseOutQuad(float progress)
        {
            if (progress < 0f || progress > 1f)
                throw new ArgumentOutOfRangeException(nameof(progress), "Value must be between 0.0 and 1.0.");

            return progress * (2 - progress);
        }

        /// <summary>
        /// Quadratic easing in and out (slow start and end, fast middle).
        /// Combines ease-in for the first half (0.0 to 0.5) and ease-out for the second half (0.5 to 1.0).
        /// First half formula: 2 × value²
        /// Second half formula: -1 + (4 - 2 × value) × value
        /// </summary>
        /// <param name="progress">Normalized progress value between 0.0 and 1.0, where 0.0 is the start and 1.0 is the end of the animation.</param>
        /// <returns>Eased value between 0.0 and 1.0 with slow start and end, fast middle.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not between 0.0 and 1.0.</exception>
        public static float EaseInOutQuad(float progress)
        {
            if (progress < 0f || progress > 1f)
                throw new ArgumentOutOfRangeException(nameof(progress), "Value must be between 0.0 and 1.0.");

            return progress < 0.5f ? 2 * progress * progress : -1 + (4 - 2 * progress) * progress;
        }

        /// <summary>
        /// Gets the easing function delegate for the specified easing style.
        /// </summary>
        /// <param name="style">The easing style to retrieve.</param>
        /// <returns>A delegate to the corresponding easing function.</returns>
        public static EasingFunction GetEasingFunction(EasingStyle style)
        {
            return style switch
            {
                EasingStyle.Linear => Linear,
                EasingStyle.EaseInQuad => EaseInQuad,
                EasingStyle.EaseOutQuad => EaseOutQuad,
                EasingStyle.EaseInOutQuad => EaseInOutQuad,
                _ => Linear
            };
        }
    }
}
