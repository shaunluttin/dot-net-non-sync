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
        [InlineData(4)]
        [InlineData(5)]
        public void SetResetWait_WhenTwoThreadsInteract_InterleavesThreads(int maxCalls)
        {
            // Arrange
            var callCount = -1;
            var callOrder = new string[maxCalls];

            var ping = new ManualResetEventSlim(true);
            var pong = new ManualResetEventSlim(false);

            // Act
            var pingThread = new Thread(() =>
            {
                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = $"{callCount} Ping: {pong.IsSet} | {ping.IsSet}";

                    pong.Set(); 
                    ping.Reset();
                    ping.Wait();
                }

                pong.Set();
            });

            var pongThread = new Thread(() =>
            {
                pong.Wait();

                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = $"{callCount} Pong: {pong.IsSet} | {ping.IsSet}";

                    ping.Set();
                    pong.Reset();
                    pong.Wait();
                }

                ping.Set();
            });

            pongThread.Start();
            pingThread.Start();

            // Wait until all the threads complete.
            foreach (var thread in new [] {pingThread, pongThread}) 
            {
                thread.Join();
            }

            // Assert
            _output.WriteLine(string.Join(System.Environment.NewLine, callOrder));

            Assert.DoesNotContain(callOrder, item => item == null);

            for (var i = 0; i < callOrder.Length; ++i)
            {
                if (i % 2 == 0) {
                    Assert.Contains("Ping", callOrder[i]);
                } else {
                    Assert.Contains("Pong", callOrder[i]);
                }
            }
        }
    }
}