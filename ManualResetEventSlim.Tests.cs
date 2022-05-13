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