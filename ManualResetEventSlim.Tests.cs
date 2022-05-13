using System.Threading;
using Xunit;

namespace DotNetNonSync
{
    public class ManualResetEventTests 
    {
        [Fact]
        public void Todo() 
        {
            // Waiting threads need to 'wait for the signal'.
            // Non-Singalled blocks waiting threads.
            // Signalled lets waiting thread to proceed.
            var mreSlim = new ManualResetEventSlim();

            // What needs disposal? What resources does an MSE Slim use?
            mreSlim.Dispose();

            // Q: What does it mean to be set?
            // A: To be 'set' means to be 'signalled'.
            var _0 = mreSlim.IsSet;

            // Q: What does it mean to be reset?
            // A: Reset sets the state to non-signalled.
            mreSlim.Reset();

            // What does it mean to be set?
            // A: Reset sets the state to signalled.
            mreSlim.Set();

            // Q: What does spin count mean?
            // A: This has something to do with dropping down to the kernal. Why would we want to
            // control when we drop down to the kernal? What happens before we drop down there?
            var _1 = mreSlim.SpinCount;

            // Q: What does Wait do?
            // A: Blocks the current thread until the MRE state becomes "signalled".
            mreSlim.Wait();

            // Q: What is a wait handle?
            // A: Encapsulates OS specific objects that await exlcusive access to shared resources.
            var _2 = mreSlim.WaitHandle;
        }
    }
}