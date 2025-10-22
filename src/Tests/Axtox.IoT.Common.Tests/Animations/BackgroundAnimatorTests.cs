using Axtox.IoT.Common.Animations;
using nanoFramework.TestFramework;
using System;
using System.Threading;

namespace Axtox.IoT.Common.Tests.Animations
{
    [TestClass]
    public class BackgroundAnimatorTests
    {
        #region Test Helpers

        private class StubAnimatable : IAnimatable
        {
            private readonly object _lock = new();
            private float _currentValue;

            public float CurrentValue
            {
                get { lock (_lock) return _currentValue; }
                set { lock (_lock) _currentValue = value; }
            }

            public int GetCurrentValueCallCount { get; private set; }
            public int SetAnimatedValueCallCount { get; private set; }

            public AnimatedValue GetCurrentValue()
            {
                GetCurrentValueCallCount++;
                lock (_lock)
                    return new AnimatedValue { Value = _currentValue };
            }

            public void SetAnimatedValue(AnimatedValue value)
            {
                SetAnimatedValueCallCount++;
                lock (_lock)
                    _currentValue = value.Value;
            }
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateInstance_WhenCalled()
        {
            // Arrange & Act
            using var animator = new BackgroundAnimator();

            // Assert
            Assert.IsNotNull(animator);
        }

        #endregion

        #region Configure Tests

        [TestMethod]
        public void Configure_ShouldApplySettings_WhenValidSettingsProvided()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var settingsApplied = false;

            // Act
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 500;
                settings.UpdateIntervalInMilliseconds = 10;
                settings.EasingStyle = EasingStyle.EaseInOutQuad;
                settingsApplied = true;
            });

            // Assert
            Assert.IsTrue(settingsApplied);
        }

        [TestMethod]
        public void Configure_ShouldThrowObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var animator = new BackgroundAnimator();
            animator.Dispose();

            // Act & Assert
            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Configure(settings => { });
            });
        }

        #endregion

        #region Animate Tests

        [TestMethod]
        public void Animate_ShouldAnimateToTargetValue_WhenValidTargetProvided()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(200);

            // Assert
            Assert.AreEqual(1.0f, stub.CurrentValue);
            Assert.IsTrue(stub.GetCurrentValueCallCount > 0);
            Assert.IsTrue(stub.SetAnimatedValueCallCount > 0);
        }

        [TestMethod]
        public void Animate_ShouldCallSetAnimatedValueMultipleTimes_DuringAnimation()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(200);

            // Assert
            // 100 ms duration with 10 ms interval should result in approximately 10 updates
            // plus one final set to the target value
            Assert.AreEqual(100 / 10 + 1, stub.SetAnimatedValueCallCount);
        }

        [TestMethod]
        public void Animate_ShouldThrowArgumentNullException_WhenTargetIsNull()
        {
            // Arrange
            using var animator = new BackgroundAnimator();

            // Act & Assert
            Assert.ThrowsException(typeof(ArgumentNullException), () =>
            {
                animator.Animate(null, new AnimatedValue { Value = 1.0f });
            });
        }

        [TestMethod]
        public void Animate_ShouldThrowObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var animator = new BackgroundAnimator();
            var stub = new StubAnimatable();
            animator.Dispose();

            // Act & Assert
            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            });
        }

        [TestMethod]
        public void Animate_ShouldCompleteImmediately_WhenFromAndToValuesAreEqual()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.5f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 0.5f });
            Thread.Sleep(50);

            // Assert
            Assert.AreEqual(0.5f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldInterruptPreviousAnimation_WhenNewAnimationStarted()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 200;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(50);
            animator.Animate(stub, new AnimatedValue { Value = 0.0f });
            Thread.Sleep(250);

            // Assert
            Assert.AreEqual(0.0f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldUseDefaultSettings_WhenNotConfigured()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(200);

            // Assert
            Assert.IsTrue(stub.SetAnimatedValueCallCount > 0);
            Assert.AreEqual(1.0f, stub.CurrentValue);
        }

        #endregion

        #region Abort Tests

        [TestMethod]
        public void Abort_ShouldStopAnimation_WhenAnimationIsRunning()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 500;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(50);
            var valueBeforeAbort = stub.CurrentValue;
            animator.Abort();
            Thread.Sleep(100);
            var valueAfterAbort = stub.CurrentValue;

            // Assert
            Assert.IsTrue(valueBeforeAbort > 0.0f);
            Assert.IsTrue(valueBeforeAbort < 1.0f);
            Assert.AreEqual(valueBeforeAbort, valueAfterAbort);
        }

        [TestMethod]
        public void Abort_ShouldDoNothing_WhenNoAnimationIsRunning()
        {
            // Arrange
            using var animator = new BackgroundAnimator();

            // Act & Assert (should not throw)
            animator.Abort();
        }

        [TestMethod]
        public void Abort_ShouldThrowObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var animator = new BackgroundAnimator();
            animator.Dispose();

            // Act & Assert
            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Abort();
            });
        }

        [TestMethod]
        public void Abort_ShouldLeaveTargetAtIntermediateValue_WhenCalledDuringAnimation()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 500;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(100);
            animator.Abort();

            // Assert
            Assert.IsTrue(stub.CurrentValue > 0.0f && stub.CurrentValue < 1.0f);
        }

        #endregion

        #region Dispose Tests

        [TestMethod]
        public void Dispose_ShouldStopAnimation_WhenAnimationIsRunning()
        {
            // Arrange
            var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 500;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(50);
            animator.Dispose();
            Thread.Sleep(100);
            var finalValue = stub.CurrentValue;

            // Assert
            Assert.IsTrue(finalValue < 1.0f);
        }

        [TestMethod]
        public void Dispose_ShouldAllowMultipleCalls_WithoutException()
        {
            // Arrange
            var animator = new BackgroundAnimator();

            // Act & Assert (should not throw)
            animator.Dispose();
            animator.Dispose();
        }

        [TestMethod]
        public void Dispose_ShouldPreventFurtherOperations_AfterDisposal()
        {
            // Arrange
            var animator = new BackgroundAnimator();
            var stub = new StubAnimatable();
            animator.Dispose();

            // Act & Assert
            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Configure(s => { });
            });

            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            });

            Assert.ThrowsException(typeof(ObjectDisposedException), () =>
            {
                animator.Abort();
            });
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Animate_ShouldHandleVeryShortDuration_Correctly()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 1;
                settings.UpdateIntervalInMilliseconds = 1;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(50);

            // Assert
            Assert.AreEqual(1.0f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldHandleNegativeToPositiveTransition_Correctly()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = -1.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(200);

            // Assert
            Assert.AreEqual(1.0f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldBeThreadSafe_WhenCalledFromMultipleThreads()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
            });

            // Act
            var thread1 = new Thread(() => animator.Animate(stub, new AnimatedValue { Value = 1.0f }));
            var thread2 = new Thread(() => animator.Animate(stub, new AnimatedValue { Value = 0.5f }));

            thread1.Start();
            Thread.Sleep(10);
            thread2.Start();

            thread1.Join();
            thread2.Join();
            Thread.Sleep(200);

            // Assert - should complete without exception
            Assert.IsTrue(stub.SetAnimatedValueCallCount > 0);
            Assert.AreEqual(0.5f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldHandleRapidAbortAndRestart_Correctly()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub = new StubAnimatable { CurrentValue = 0.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 200;
                settings.UpdateIntervalInMilliseconds = 10;
            });

            // Act
            animator.Animate(stub, new AnimatedValue { Value = 1.0f });
            Thread.Sleep(20);
            animator.Abort();
            animator.Animate(stub, new AnimatedValue { Value = 0.0f });
            Thread.Sleep(250);

            // Assert
            Assert.AreEqual(0.0f, stub.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldHandleDifferentAnimatables_Correctly()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub1 = new StubAnimatable { CurrentValue = 0.0f };
            var stub2 = new StubAnimatable { CurrentValue = 10.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
                settings.UpdateIntervalInMilliseconds = 10;
            });
            // Act
            animator.Animate(stub1, new AnimatedValue { Value = 5.0f });
            Thread.Sleep(150);
            animator.Animate(stub2, new AnimatedValue { Value = 0.0f });
            Thread.Sleep(150);
            // Assert
            Assert.AreEqual(5.0f, stub1.CurrentValue);
            Assert.AreEqual(0.0f, stub2.CurrentValue);
        }

        [TestMethod]
        public void Animate_ShouldHandleConcurrentAnimatables_Correctly()
        {
            // Arrange
            using var animator = new BackgroundAnimator();
            var stub1 = new StubAnimatable { CurrentValue = 0.0f };
            var stub2 = new StubAnimatable { CurrentValue = 10.0f };
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 100;
                settings.UpdateIntervalInMilliseconds = 10;
            });
            // Act
            animator.Animate(stub1, new AnimatedValue { Value = 5.0f });
            Thread.Sleep(50);
            animator.Animate(stub2, new AnimatedValue { Value = 0.0f });
            Thread.Sleep(150);
            // Assert
            Assert.IsTrue(stub1.SetAnimatedValueCallCount > 0 && stub1.CurrentValue < 5.0f);
            Assert.AreEqual(0.0f, stub2.CurrentValue);
        }

        #endregion
    }
}
