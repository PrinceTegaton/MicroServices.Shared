using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroServices
{
    public static class TaskUtility
    {
        public static void FireAndForget(this Action action, TaskCreationOptions? taskCreationOptions = null, ILogger logger = null)
        {
            try
            {
                new Task(action, CancellationToken.None,
                    taskCreationOptions ?? TaskCreationOptions.LongRunning)
                    .Start();
            }
            catch (Exception ex)
            {
                // log errors
                logger?.LogError(ex, ex?.Message ?? "Background task execution error");
            }
        }
    }
}
