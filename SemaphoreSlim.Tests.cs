using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace DotNetNonSync
{
    public class SemaphoreSlimTests
    {
        private readonly ITestOutputHelper _output;

        public SemaphoreSlimTests(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// Start (n) threads; ensure that only (m) threads access the resource at once.
        /// </summary>
        [Theory]
        [InlineData(5, 15)]
        public void WaitRelease_WithThreadPressureAndMaxThreads_NeverExceedsMax(int totalThreads, int expectedMaxAccess)
        {
            // Arrange
            const int sharedAccessDuration = 200;

            var sharedAccessCount = 0;
            var sharedAccessCounts = new int[totalThreads];
            var remainingAccessCounts = new int[totalThreads];

            var semSlim = new SemaphoreSlim(expectedMaxAccess);

            // Act
            var threads = Enumerable
                .Range(0, totalThreads)
                .Select((index) =>
                {
                    var thread = new Thread(() =>
                    {
                        semSlim.Wait();

                        sharedAccessCounts[index] = Interlocked.Increment(ref sharedAccessCount);
                        remainingAccessCounts[index] = semSlim.CurrentCount;
                        Thread.Sleep(sharedAccessDuration);

                        semSlim.Release();

                        Interlocked.Decrement(ref sharedAccessCount);
                    });

                    thread.Start();

                    return thread;
                })
                // Start all the threads.
                .ToList();

            // Wait until all the threads complete.
            foreach (var thread in threads) 
            {
                thread.Join();
            }

            // Assert
            Assert.All(
                sharedAccessCounts, 
                item => Assert.InRange(item, 0, expectedMaxAccess)
            );

            Assert.All(
                sharedAccessCounts.Zip(remainingAccessCounts), 
                item => Assert.Equal(expectedMaxAccess, item.First + item.Second)
            );
        }
    }
}