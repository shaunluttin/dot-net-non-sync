using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNetNonSync
{
    public class Task_ctor
    {

        [Fact]
        public void Task_WhenTokenSourceIsCancelled_TaskStatusIsCancelled()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            // Act
            var task = new Task(() => { }, tokenSource.Token);
            tokenSource.Cancel();

            // Assert
            Assert.Equal(TaskStatus.Canceled, task.Status);
        }

        [Fact]
        public async Task Task_WhenTokenSourceIsCancelled_AwaitThrowsTaskCancelledException()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var task = new Task(() => { }, tokenSource.Token);
            tokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await task);
        }

        [Fact]
        public void Task_WhenTokenSourceIsCancelled_StartThrowsInvalidOperationException()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var task = new Task(() => { }, tokenSource.Token);
            tokenSource.Cancel();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => task.Start());
        }
    }
}