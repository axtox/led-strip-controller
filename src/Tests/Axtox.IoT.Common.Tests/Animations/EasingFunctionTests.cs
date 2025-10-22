using Axtox.IoT.Common.Animations;
using nanoFramework.TestFramework;
using System;

namespace Axtox.IoT.Common.Tests.Animations
{
    [TestClass]
    public class EasingFunctionTests
    {
        [TestMethod]
        public void Linear_WithValidValues_ReturnsCorrectResults()
        {
            Assert.AreEqual(0f, EasingFunctions.Linear(0f));
            Assert.AreEqual(0.5f, EasingFunctions.Linear(0.5f));
            Assert.AreEqual(1f, EasingFunctions.Linear(1f));
        }

        [TestMethod]
        public void Linear_WithInvalidValue_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.Linear(-0.1f));
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.Linear(1.1f));
        }

        [TestMethod]
        public void EaseInQuad_WithValidValues_ReturnsCorrectResults()
        {
            Assert.AreEqual(0f, EasingFunctions.EaseInQuad(0f));
            Assert.AreEqual(0.25f, EasingFunctions.EaseInQuad(0.5f));
            Assert.AreEqual(1f, EasingFunctions.EaseInQuad(1f));
        }

        [TestMethod]
        public void EaseInQuad_WithInvalidValue_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseInQuad(-0.1f));
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseInQuad(1.1f));
        }

        [TestMethod]
        public void EaseOutQuad_WithValidValues_ReturnsCorrectResults()
        {
            Assert.AreEqual(0f, EasingFunctions.EaseOutQuad(0f));
            Assert.AreEqual(0.75f, EasingFunctions.EaseOutQuad(0.5f));
            Assert.AreEqual(1f, EasingFunctions.EaseOutQuad(1f));
        }

        [TestMethod]
        public void EaseOutQuad_WithInvalidValue_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseOutQuad(-0.1f));
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseOutQuad(1.1f));
        }

        [TestMethod]
        public void EaseInOutQuad_WithValidValues_ReturnsCorrectResults()
        {
            Assert.AreEqual(0f, EasingFunctions.EaseInOutQuad(0f));
            Assert.AreEqual(0.5f, EasingFunctions.EaseInOutQuad(0.5f));
            Assert.AreEqual(1f, EasingFunctions.EaseInOutQuad(1f));

            // Test first half (ease in)
            Assert.AreEqual(0.125f, EasingFunctions.EaseInOutQuad(0.25f));

            // Test second half (ease out)
            Assert.AreEqual(0.875f, EasingFunctions.EaseInOutQuad(0.75f));
        }

        [TestMethod]
        public void EaseInOutQuad_WithInvalidValue_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseInOutQuad(-0.1f));
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => EasingFunctions.EaseInOutQuad(1.1f));
        }

        [TestMethod]
        public void GetEasingFunction_ReturnsCorrectFunction()
        {
            var linearFunc = EasingFunctions.GetEasingFunction(EasingStyle.Linear);
            Assert.AreEqual(0.5f, linearFunc(0.5f));

            var easeInFunc = EasingFunctions.GetEasingFunction(EasingStyle.EaseInQuad);
            Assert.AreEqual(0.25f, easeInFunc(0.5f));

            var easeOutFunc = EasingFunctions.GetEasingFunction(EasingStyle.EaseOutQuad);
            Assert.AreEqual(0.75f, easeOutFunc(0.5f));

            var easeInOutFunc = EasingFunctions.GetEasingFunction(EasingStyle.EaseInOutQuad);
            Assert.AreEqual(0.5f, easeInOutFunc(0.5f));
        }

        [TestMethod]
        public void AllEasingFunctions_StartAtZero_EndAtOne()
        {
            Assert.AreEqual(0f, EasingFunctions.Linear(0f));
            Assert.AreEqual(1f, EasingFunctions.Linear(1f));

            Assert.AreEqual(0f, EasingFunctions.EaseInQuad(0f));
            Assert.AreEqual(1f, EasingFunctions.EaseInQuad(1f));

            Assert.AreEqual(0f, EasingFunctions.EaseOutQuad(0f));
            Assert.AreEqual(1f, EasingFunctions.EaseOutQuad(1f));

            Assert.AreEqual(0f, EasingFunctions.EaseInOutQuad(0f));
            Assert.AreEqual(1f, EasingFunctions.EaseInOutQuad(1f));
        }
    }
}
