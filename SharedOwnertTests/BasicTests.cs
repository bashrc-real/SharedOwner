using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

using Timer = System.Timers.Timer;

namespace SharedOwnertTests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void SharedOwnerTestMultiThreaded()
        {
            
            var signal1 = new AutoResetEvent(false);
            var signal2 = new AutoResetEvent(false);
            var sharedTimer = SharedOwner.SharedHandle<System.Timers.Timer>.MakeSharedHandle(() => new System.Timers.Timer());
            Assert.IsTrue(sharedTimer.ReferenceCount == 1);
            var tsk1 = Task.Run(() =>
            {
                using (var timerHandle = sharedTimer.GetHandle())
                {
                    Assert.IsTrue(signal1.WaitOne(5000));
                    Assert.AreEqual(sharedTimer.ReferenceCount, 3);
                    ((Timer)timerHandle).Start();
                }
                signal2.Set();
            });
            var tsk2 = Task.Run(() =>
            {
                using (var timerHandle = sharedTimer.GetHandle())
                {
                    signal1.Set();
                    Assert.IsTrue(signal2.WaitOne(5000));
                    Assert.IsTrue(sharedTimer.ReferenceCount == 2);
                    ((Timer)timerHandle).Stop();
                }
            });

            tsk2.Wait();
            Assert.IsTrue(sharedTimer.ReferenceCount == 1);
            var ob = sharedTimer.GetHandle();
        }

    }
}
