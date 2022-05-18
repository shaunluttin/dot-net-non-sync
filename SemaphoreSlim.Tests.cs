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
        [InlineData(1, 10)]
        [InlineData(2, 10)]
        [InlineData(4, 10)]
        [InlineData(8, 10)]
        [InlineData(16, 10)]
        [InlineData(32, 10)]
        [InlineData(64, 10)]
        [InlineData(128, 10)]
        public void WaitRelease_WithThreadPressureAndMaxThreads_ReachesButNeverExceedsMax(int totalThreads, int maxAccessCount)
        {
            // Arrange
            var sharedAccessCount = 0;
            var sharedAccessCounts = new int[totalThreads];

            var semSlim = new SemaphoreSlim(maxAccessCount);

            // Act
            var threads = Enumerable
                .Range(0, totalThreads)
                .Select((index) =>
                {
                    var thread = new Thread(() =>
                    {
                        semSlim.Wait();
                        sharedAccessCounts[index] = Interlocked.Increment(ref sharedAccessCount);

                        // Sleep for long enough to cause the semaphore to wait.
                        Thread.Sleep(maxAccessCount * 5);

                        Interlocked.Decrement(ref sharedAccessCount);
                        semSlim.Release();

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
                item => Assert.InRange(item, 1, maxAccessCount)
            );

            if(maxAccessCount <= totalThreads) {
                Assert.Contains(maxAccessCount, sharedAccessCounts);
            } else {
                Assert.Contains(totalThreads, sharedAccessCounts);
            } 
        }
    }
}