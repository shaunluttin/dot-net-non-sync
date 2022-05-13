using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNetNonSync
{
    public class Task_Run
    {
        // Queues the specified work to run on the thread pool 
        // and returns a Task object that represents that work.
        [Fact]
        public async Task Run_WhenPassedAnAction_RunsActionOnDifferentThead()
        {
            int[] threadIds = new int[2] { -1, -1 };

            threadIds[0] = Thread.CurrentThread.ManagedThreadId;

            // Act
            var queuedWork = Task.Run(() => {
                threadIds[1] = Thread.CurrentThread.ManagedThreadId;
            });

            await queuedWork;

            Assert.DoesNotContain(-1, threadIds);
            Assert.NotEqual(threadIds[0], threadIds[1]);
        }

        [Fact]
        public void Run_WhenItsActionThrows_Wait_PropogatesAnAggregateException()
        {
            Assert.Throws<AggregateException>(() =>
            {
                // Act
                Task.Run(() => throw new NotImplementedException()).Wait();
            });
        }

        [Fact]
        public async Task Run_WhenItsActionThrows_Await_UnwrapsAggregateException()
        {
            // Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => {
                await Task.Run(() => throw new NotImplementedException());
            });
        }

        [Fact]
        public void Run_WhenItsActionThrows_GetAwaiterGetResult_UnwrapsAggregateException()
        {
            // Assert
            Assert.Throws<NotImplementedException>(() =>
            {
                // Act
                Task.Run(() => throw new NotImplementedException()).GetAwaiter().GetResult();
            });
        }

        [Fact]
        public async Task Run_WhenCancelledBeforeStarted_ThrowsOnlyWhenAwaited()
        {
            var tokenSource = new CancellationTokenSource();

            tokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(async () => {
                var task = Task.Run(() => { }, tokenSource.Token);
                await task;
            });

            // Does not throw when not awaited.
            Task.Run(() => { }, tokenSource.Token);
        }
    }
}
