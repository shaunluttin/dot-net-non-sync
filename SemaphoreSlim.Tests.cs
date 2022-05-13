using System;
using System.Collections.Concurrent;
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
        /// Start (n) threads; ensure that only (m) access the resource at once.
        /// </summary>
        [Theory]
        [InlineData(10, 20)]
        public async void Todo(int expecteMaxActive, int totalThreads)
        {
            // Arrange
            var actualAccessCount = 0;

            var semSlim = new SemaphoreSlim(expecteMaxActive);

            // Act
            Enumerable.Range(0, totalThreads).Select((i) => new Thread(() =>
            {
                semSlim.Wait(); // wait for access

                Interlocked.Increment(ref actualAccessCount);

                semSlim.Release(); // release access

            }))
            .ToList()
            .ForEach((t) => t.Start());

            Thread.Sleep(1000);

            // Assert
            Assert.Equal(totalThreads, actualAccessCount);
        }
    }
}