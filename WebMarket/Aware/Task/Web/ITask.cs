using Aware.ECommerce.Enums;
using Aware.Task.Model;
using Aware.Util;
using Aware.Util.Model;

namespace Aware.Task.Web
{
    public interface ITask
    {
        bool RunNow { get; set; }
        string StatusString { get; }
        TaskType Type { get; }
        TaskDefinition Definition { get; set; }
        event BaseTask.SetLastRun SetLastRunEvent;
        void Run();
        Result Execute();
        void SetDefinition(TaskDefinition definition);
        void SetStatus(TaskStatus status);
        void SetRunParams(params string[] runParams);
        bool IsValid();
        bool CanBeRun(bool runNow = false);
    }
}
