## 行为树可触发事件类型优化
```CS
for (int i = 0; i < (int)EventTypes.None; i++)
{
    this.hasEvent[i] = this.TaskContainsMethod(((Behavior.EventTypes)i).ToString(), this.mBehaviorSource.RootTask);
    this.mBehaviorSource.RootTask);
}
```

### EventTypes 事件类型
```CS
public enum EventTypes
{
    // 碰撞进入
    OnCollisionEnter = 0,
    // 碰撞退出
    OnCollisionExit = 1,
    // 触发进入
    OnTriggerEnter = 2,
    // 触发退出
    OnTriggerExit = 3,
    // 2D 碰撞进入
    OnCollisionEnter2D = 4,
    // 2D 碰撞退出
    OnCollisionExit2D = 5,
    // 2D 触发进入
    OnTriggerEnter2D = 6,
    // 2D 触发退出
    OnTriggerExit2D = 7,
    //控制碰撞器碰撞 
    OnControllerColliderHit = 8,
    // LateUpdate
    OnLateUpdate = 9,
    // FixedUpdate
    OnFixedUpdate = 10,
    //缺省
    None = 11, 
}
```

### TaskContainsMethod 函数
```CS
private bool TaskContainsMethod(string methodName, Task task)
{
    if (task == null)
    {
        return false;
    }
    MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (method != null && method.DeclaringType.IsAssignableFrom(task.GetType()))
    {
        return true;
    }
    if (task is ParentTask)
    {
        ParentTask parentTask = task as ParentTask;
        if (parentTask.Children != null)
        {
            for (int i = 0; i < parentTask.Children.Count; i++)
            {
                if (this.TaskContainsMethod(methodName, parentTask.Children[i]))
                {
                    return true;
                }
            }
        }
    }
    return false;
}
```
### 事件触发
所有事件都是通过 `BehaviorManager` 获取行为树当前活动的节点执行的。
`LateUpdate`、`FixedUpdate`不需要对象触发事件就能调用。其他事件需要`BehaviorTree`对象本身触发事件后，通过`BehaviorManager`调用具体的节点。

`BehaviorTree` 中触发代码
```CS
public void OnCollisionEnter(Collision collision)
{
    if (this.hasEvent[0] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnCollisionEnter(collision, this);
    }
}

public void OnCollisionExit(Collision collision)
{
    if (this.hasEvent[1] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnCollisionExit(collision, this);
    }
}

public void OnTriggerEnter(Collider other)
{
    if (this.hasEvent[2] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnTriggerEnter(other, this);
    }
}

public void OnTriggerExit(Collider other)
{
    if (this.hasEvent[3] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnTriggerExit(other, this);
    }
}

public void OnCollisionEnter2D(Collision2D collision)
{
    if (this.hasEvent[4] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnCollisionEnter2D(collision, this);
    }
}

public void OnCollisionExit2D(Collision2D collision)
{
    if (this.hasEvent[5] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnCollisionExit2D(collision, this);
    }
}

public void OnTriggerEnter2D(Collider2D other)
{
    if (this.hasEvent[6] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnTriggerEnter2D(other, this);
    }
}

public void OnTriggerExit2D(Collider2D other)
{
    if (this.hasEvent[7] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnTriggerExit2D(other, this);
    }
}

public void OnControllerColliderHit(ControllerColliderHit hit)
{
    if (this.hasEvent[8] && BehaviorManager.instance != null)
    {
        BehaviorManager.instance.BehaviorOnControllerColliderHit(hit, this);
    }
}
```

`BehaviorManager` 中的触发代码：
```CS 
public void LateUpdate()
{
    for (int i = 0; i < this.behaviorTrees.Count; i++)
    {
        if (this.behaviorTrees[i].behavior.HasEvent[9])
        {
            for (int j = this.behaviorTrees[i].activeStack.Count - 1; j > -1; j--)
            {
                int index = this.behaviorTrees[i].activeStack[j].Peek();
                this.behaviorTrees[i].taskList[index].OnLateUpdate();
            }
        }
    }
}

public void FixedUpdate()
{
    for (int i = 0; i < this.behaviorTrees.Count; i++)
    {
        if (this.behaviorTrees[i].behavior.HasEvent[10])
        {
            for (int j = this.behaviorTrees[i].activeStack.Count - 1; j > -1; j--)
            {
                int index = this.behaviorTrees[i].activeStack[j].Peek();
                this.behaviorTrees[i].taskList[index].OnFixedUpdate();
            }
        }
    }
}

```


## 优化方式
* 由于项目中不使用这些事件类型，可直接全部取消
```CS
for (int i = 0; i < (int)EventTypes.None; i++)
{
    //this.hasEvent[i] = this.TaskContainsMethod(((Behavior.EventTypes)i).ToString(), this.mBehaviorSource.RootTask);
    this.hasEvent[i] = false;
}
this.initialized = true;
```
* 自己手动控制某些事件打开，避免节点遍历与反射。