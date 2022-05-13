using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace DotNetNonSync
{
    public class ManualResetEventSlimTests
    {
        private readonly ITestOutputHelper _output;

        public ManualResetEventSlimTests(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <remarks>
        /// Q: What does Wait() do?
        /// A: Wait() causes the current thread to sleep until signalled.
        ///
        /// Q: What does it mean to be signalled?
        /// A: Signalled lets a waiting thread proceed.
        ///
        /// Q: What does it mean to be set?
        /// A: To be set means to be signalled.
        ///
        /// Q: What does it mean to be reset?
        /// A: To be reset means NOT to be signalled.
        ///
        /// Q: What does spin count mean?
        /// A: This determines when the .NET runtime might drop down to kernal processes.
        ///
        /// Q: What is a WaitHandle?
        /// A: The WaitHandle encapsulates OS specific objects that control access to shared resources.
        /// </remarks>
        [Theory]
        [InlineData(11)]
        [InlineData(12)]
        public void SetResetWait_WhenTwoThreadsInteract_InterleavesThreads(int maxCalls)
        {
            // Arrange
            var callCount = -1;
            var callOrder = new string[maxCalls];

            var mreSlimLettuce = new ManualResetEventSlim();
            var mreSlimTomato = new ManualResetEventSlim();

            // Act
            new Thread(() =>
            {
                mreSlimTomato.Set();
                mreSlimLettuce.Wait();

                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = $"{callCount} Lettuce: {mreSlimLettuce.IsSet} | {mreSlimTomato.IsSet}";
                    mreSlimTomato.Set();
                    mreSlimLettuce.Reset();
                    mreSlimLettuce.Wait();
                }
            })
            .Start();

            mreSlimTomato.Wait();

            while(Interlocked.Increment(ref callCount) < maxCalls)
            {
                callOrder[callCount] = $"{callCount} Tomato: {mreSlimLettuce.IsSet} | {mreSlimTomato.IsSet}";

                mreSlimLettuce.Set();

                if (callCount + 1 < maxCalls)
                {
                    mreSlimTomato.Reset();
                    mreSlimTomato.Wait();
                }
            }

            // Assert
            Assert.DoesNotContain(callOrder, item => item == null);

            for (var i = 0; i < callOrder.Length; ++i)
            {
                if (i % 2 == 0) {
                    Assert.Contains("Tomato", callOrder[i]);
                } else {
                    Assert.Contains("Lettuce", callOrder[i]);
                }
            }
        }
    }
}