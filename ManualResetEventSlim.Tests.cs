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

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        public void SetResetWait_WhenTwoThreadsInteract_InterleavesThreads(int maxCalls)
        {
            // Arrange
            var callCount = -1;
            var callOrder = new string[maxCalls];

            var ping = new ManualResetEventSlim(true); // Ping goes first.
            var pong = new ManualResetEventSlim(false);

            // Act
            var pingThread = new Thread(() =>
            {
                ping.Wait();
                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = $"Ping";
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
                    callOrder[callCount] = "Pong";
                    ping.Set();
                    pong.Reset();
                    pong.Wait(); 
                }

                ping.Set();
            });

            pongThread.Start();
            pingThread.Start();

            // Wait until all the threads complete.
            pingThread.Join();
            pongThread.Join();

            // Assert
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