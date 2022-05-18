using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace DotNetNonSync
{
    public class AutoResetEventTests
    {
        private readonly ITestOutputHelper _output;

        public AutoResetEventTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        public void SetWait_WhenTwoThreadsInteract_InterleavesThreads(int maxCalls)
        {
            // Arrange
            var callCount = -1;
            var callOrder = new string[maxCalls];

            var ping = new AutoResetEvent(true); // Ping goes first.
            var pong = new AutoResetEvent(false);

            // Act
            var pingThread = new Thread(() =>
            {
                ping.WaitOne();
                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = "Ping";
                    pong.Set();
                    ping.WaitOne();
                }

                pong.Set();
            });

            var pongThread = new Thread(() =>
            {
                pong.WaitOne();
                while(Interlocked.Increment(ref callCount) < maxCalls)
                {
                    callOrder[callCount] = "Pong";
                    ping.Set();
                    pong.WaitOne();
                }

                ping.Set();
            });

            // Start both threads 
            pongThread.Start();
            pingThread.Start();

            // Wait until both complete.
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