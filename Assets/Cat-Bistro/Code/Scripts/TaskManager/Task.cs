using UnityEngine;

public abstract class Task : ScriptableObject
{
    public string taskName;
    public string taskDescription;

    public abstract void StartTask();
    public abstract bool TaskCompleted();

    public virtual void UpdateTask()
    {

    }

    public virtual void EndTask()
    {

    }
}