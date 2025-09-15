using System.Threading;
using System.Threading.Tasks;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   2/22/2021 10:06:26 PM
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:
    /// Changelog:        
    /// </summary>
    public static class TaskExtensions
    {
        public static async Task<T> WaitOrCancel<T>(this Task<T> task, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await Task.WhenAny(task, token.WhenCanceled());
            token.ThrowIfCancellationRequested();

            return await task;
        }
        public static Task WhenCanceled(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}