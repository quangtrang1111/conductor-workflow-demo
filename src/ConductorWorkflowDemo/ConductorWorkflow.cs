using Conductor.Client;
using Conductor.Client.Extensions;
using Conductor.Client.Interfaces;
using Conductor.Client.Models;
using Conductor.Client.Worker;

namespace ConductorWorkflowDemo
{
    public static class ConductorWorkflow
    {
        public const string BASE_PATH = "http://localhost:8080/api";

        public static IServiceCollection AddConductorWorkflow(this IServiceCollection services)
        {
            services.AddConductorWorkflowTask<RandomNumberTask>();
            services.AddConductorWorkflowTask<DoubleNumberTask>();
            services.AddConductorWorkflowTask<SumNumberWorker>();

            var configuration = new Configuration() { BasePath = BASE_PATH };
            services.AddConductorWorker(configuration);
            services.AddHostedService<WorkflowTaskService>();

            return services;
        }
    }

    public class SimpleTaskBase : IWorkflowTask
    {
        public string TaskType { get; }
        public WorkflowTaskExecutorConfiguration WorkerSettings { get; }

        public SimpleTaskBase(string taskName)
        {
            TaskType = taskName;
            WorkerSettings = new WorkflowTaskExecutorConfiguration();
        }

        public virtual TaskResult Execute(Conductor.Client.Models.Task task)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomNumberTask : SimpleTaskBase
    {
        public RandomNumberTask(string taskName = "RandomNumberTask") : base(taskName) { }

        public override TaskResult Execute(Conductor.Client.Models.Task task)
        {
            var max = task.InputData["max"] as long? ?? 99;
            max = max > 99 ? 99 : max;
            int randomNumber = (new Random()).Next((int)max);

            var outputData = new Dictionary<string, object>
            {
                { "randomNumber", randomNumber }
            };

            var logs = new List<TaskExecLog> { new TaskExecLog { Log = $"This is log: {randomNumber}" } };

            return task.Completed(outputData, logs);
        }
    }

    public class DoubleNumberTask : SimpleTaskBase
    {
        public DoubleNumberTask(string taskName = "DoubleNumberTask") : base(taskName) { }

        public override TaskResult Execute(Conductor.Client.Models.Task task)
        {
            var number = task.InputData["number"] as long? ?? 0;

            var outputData = new Dictionary<string, object>
            {
                { "doubleNumber", number * number }
            };

            return task.Completed(outputData);
        }
    }

    public class SumNumberWorker : SimpleTaskBase
    {
        public SumNumberWorker(string taskName = "SumNumberTask") : base(taskName) { }

        public override TaskResult Execute(Conductor.Client.Models.Task task)
        {
            var number1 = task.InputData["number1"] as long? ?? 0;
            var number2 = task.InputData["number2"] as long? ?? 0;

            var outputData = new Dictionary<string, object>
            {
                { "sum", number1 + number2 }
            };

            return task.Completed(outputData);
        }
    }
}
