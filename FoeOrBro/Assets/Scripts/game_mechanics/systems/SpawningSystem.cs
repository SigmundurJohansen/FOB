using Unity.Entities;
using Unity.Jobs;
using System.Runtime.InteropServices;

interface ITask
{
    void Execute();
}

struct Job : IJob
{
    public GCHandle _Task;

    public void Execute()
    {
        ITask task = (ITask)_Task.Target;
        task.Execute();
    }
}

public class SpawningSystem : ComponentSystem
{
    // Get instance of task to be executed
    //ITask task = GetTaskInstance();

    // Creates native reference of the task. Note: if there will be problems with
    // instance of GCHandle in future, it can be converted to/from IntPtr.
    //GCHandle taskHandle = GCHandle.Alloc(task);

    // Schedule jobs
    Job job = new Job()
    {
        //_Task = taskHandle,
    };

    protected override void OnUpdate()
    {
        
    }
}