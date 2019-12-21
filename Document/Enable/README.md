## Enable 启动优化
### 所有的启动点
#### ExternalBehavior 设置行为树资源
```CS
public ExternalBehavior ExternalBehavior
{
    get
    {
        return this.externalBehavior;
    }
    set
    {
        if (BehaviorManager.instance != null)
        {
            BehaviorManager.instance.DisableBehavior(this);
        }
        this.mBehaviorSource.HasSerialized = false;
        this.initialized = false;
        this.externalBehavior = value;
        if (this.startWhenEnabled)
        {
            this.EnableBehavior();
        }
    }
}
```
#### Unity 事件 OnEnable
```CS
public void OnEnable()
{
    if (BehaviorManager.instance != null && this.isPaused)
    {
        BehaviorManager.instance.EnableBehavior(this);
        this.isPaused = false;
    }
    else if (this.startWhenEnabled && this.initialized)
    {
        this.EnableBehavior();
    }
}
```
#### Unity 事件 Start
```CS
public void Start()
{
    if (this.startWhenEnabled)
    {
        this.EnableBehavior();
    }
}
```

### 行为树的停止
#### Behavior 代码
```CS
public void DisableBehavior()
{
    if (BehaviorManager.instance != null)
    {
        BehaviorManager.instance.DisableBehavior(this, this.pauseWhenDisabled);
        this.isPaused = this.pauseWhenDisabled;
    }
}

public void DisableBehavior(bool pause)
{
    if (BehaviorManager.instance != null)
    {
        BehaviorManager.instance.DisableBehavior(this, pause);
        this.isPaused = pause;
    }
}
```
#### BehaviorManager 代码
```CS 
public void DisableBehavior(Behavior behavior)
{
    this.DisableBehavior(behavior, false);
}

public void DisableBehavior(Behavior behavior, bool paused)
{
    if (!this.IsBehaviorEnabled(behavior))
    {
        if (!this.pausedBehaviorTrees.ContainsKey(behavior) || paused)
        {
            return;
        }
        this.EnableBehavior(behavior);
    }
    if (behavior.LogTaskChanges)
    {
        Debug.Log(string.Format("{0}: {1} {2}", this.RoundedTime(), (!paused) ? "Disabling" : "Pausing", behavior.ToString()));
    }
    if (paused)
    {
        BehaviorManager.BehaviorTree behaviorTree;
        if (!this.behaviorTreeMap.TryGetValue(behavior, out behaviorTree))
        {
            return;
        }
        if (!this.pausedBehaviorTrees.ContainsKey(behavior))
        {
            this.pausedBehaviorTrees.Add(behavior, behaviorTree);
            behavior.ExecutionStatus = TaskStatus.Inactive;
            for (int i = 0; i < behaviorTree.taskList.Count; i++)
            {
                behaviorTree.taskList[i].OnPause(true);
            }
            this.behaviorTrees.Remove(behaviorTree);
        }
    }
    else
    {
        this.DestroyBehavior(behavior);
    }
}
```
### 代码分析
1. 行为树在反复设置行为树资源时，如果再重新启动会走完整的行为树初始化流程，性能开销大。
2. 行为树能反复使用时，建议开启 `pauseWhenDisabled` ，重新启动时不会重新更新行为树节点。