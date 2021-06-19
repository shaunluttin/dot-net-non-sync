using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNetNonSync
{
    public class TaskRun
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
            try { 
                // Act
                Task.Run(() => throw new Exception()).Wait();
            } catch (Exception ex) {
                // Assert
                Assert.IsType<AggregateException>(ex);
            }
        }

        [Fact]
        public async Task Run_WhenItsActionThrows_Await_UnwrapsAggregateException()
        {
            try {
                // Act
                await Task.Run(() => throw new NotImplementedException());
            } catch (Exception ex) {
                // Assert
                Assert.IsType<NotImplementedException>(ex);
            }
        }

        [Fact]
        public void Run_WhenItsActionThrows_GetAwaiterGetResult_UnwrapsAggregateException()
        {
            try {
                // Act
                Task.Run(() => throw new NotImplementedException()).GetAwaiter().GetResult();
            } catch (Exception ex) {
                // Assert
                Assert.IsType<NotImplementedException>(ex);
            }
        }
    }
}
