using System.Collections.Generic;
using Aware.Task.Model;
using Aware.Task.Web;
using Aware.Util;
using Aware.ECommerce.Enums;

namespace Aware.Task
{
    public interface ITaskManager
    {
        List<ITask> TaskList { get; }
        bool IsRunning { get; }
        void Initialize();
        void Start();
        void Stop();
        void RunImmediately(TaskType taskType, params string[] taskParams);
        void Refresh();
        TaskDefinition GetTask(int taskID);
    }
}