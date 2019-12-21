using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    /// <summary>
    /// BehaviorTree 的基类，行为树的真正对象
    /// </summary>
    [Serializable]
	public abstract class Behavior : MonoBehaviour, IBehavior
	{
        /// <summary>
        /// 初始化 BehaviorSource 对象
        /// </summary>
        public Behavior()
		{
			this.mBehaviorSource = new BehaviorSource(this);
		}

		/// <summary>
        /// 行为树启动后的回调
        /// </summary>
		public event Behavior.BehaviorHandler OnBehaviorStart;

		/// <summary>
        /// 行为树重新启动的回调
        /// </summary>
		public event Behavior.BehaviorHandler OnBehaviorRestart;

		/// <summary>
        /// 行为树结束回调
        /// </summary>
		public event Behavior.BehaviorHandler OnBehaviorEnd;

		/// <summary>
        /// 当对象激活时自动启动
        /// </summary>
		public bool StartWhenEnabled
		{
			get
			{
				return this.startWhenEnabled;
			}
			set
			{
				this.startWhenEnabled = value;
			}
		}

		/// <summary>
        /// disable 时为暂停
        /// </summary>
		public bool PauseWhenDisabled
		{
			get
			{
				return this.pauseWhenDisabled;
			}
			set
			{
				this.pauseWhenDisabled = value;
			}
		}

		/// <summary>
        /// 完成后重新开始
        /// </summary>
		public bool RestartWhenComplete
		{
			get
			{
				return this.restartWhenComplete;
			}
			set
			{
				this.restartWhenComplete = value;
			}
		}

		/// <summary>
        /// 任务变化时是否打印日志
        /// </summary>
		public bool LogTaskChanges
		{
			get
			{
				return this.logTaskChanges;
			}
			set
			{
				this.logTaskChanges = value;
			}
		}

		/// <summary>
        /// 组定义
        /// </summary>
		public int Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		/// <summary>
        /// 重新启动时充值变量
        /// </summary>
		public bool ResetValuesOnRestart
		{
			get
			{
				return this.resetValuesOnRestart;
			}
			set
			{
				this.resetValuesOnRestart = value;
			}
		}

		/// <summary>
        /// 行为树的资源文件对象
        /// </summary>
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

		/// <summary>
        /// 标记是否继承过变量
        /// </summary>
		public bool HasInheritedVariables
		{
			get
			{
				return this.hasInheritedVariables;
			}
			set
			{
				this.hasInheritedVariables = value;
			}
		}

		/// <summary>
        /// 名字
        /// </summary>
		public string BehaviorName
		{
			get
			{
				return this.mBehaviorSource.behaviorName;
			}
			set
			{
				this.mBehaviorSource.behaviorName = value;
			}
		}

		/// <summary>
        /// 描述
        /// </summary>
		public string BehaviorDescription
		{
			get
			{
				return this.mBehaviorSource.behaviorDescription;
			}
			set
			{
				this.mBehaviorSource.behaviorDescription = value;
			}
		}

        /// <summary>
        /// 返回 BehaviorSource
        /// </summary>
        /// <returns></returns>
        public BehaviorSource GetBehaviorSource()
		{
			return this.mBehaviorSource;
		}

		/// <summary>
        /// 设置
        /// </summary>
        /// <param name="behaviorSource"></param>
		public void SetBehaviorSource(BehaviorSource behaviorSource)
		{
			this.mBehaviorSource = behaviorSource;
		}

		/// <summary>
        /// 返回当前对象
        /// </summary>
        /// <returns></returns>
		public UnityEngine.Object GetObject()
		{
			return this;
		}

		/// <summary>
        /// 返回游戏对象
        /// </summary>
        /// <returns></returns>
		public string GetOwnerName()
		{
			return base.gameObject.name;
		}

		/// <summary>
        /// 获取当前的运行状态
        /// </summary>
		public TaskStatus ExecutionStatus
		{
			get
			{
				return this.executionStatus;
			}
			set
			{
				this.executionStatus = value;
			}
		}

		/// <summary>
        /// 具有哪些事件
        /// </summary>
		public bool[] HasEvent
		{
			get
			{
				return this.hasEvent;
			}
		}

		/// <summary>
        /// 启动时自动启动
        /// </summary>
		public void Start()
		{
			if (this.startWhenEnabled)
			{
				this.EnableBehavior();
			}
		}

        /// <summary>
        /// task 中是否包含 methodName 方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 启动行为树，创建 BehaviorManager 实例
        /// </summary>
        public void EnableBehavior()
		{
			Behavior.CreateBehaviorManager();
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.EnableBehavior(this);
			}
			if (!this.initialized)
			{
				for (int i = 0; i < (int)EventTypes.None; i++)
				{
                    //this.hasEvent[i] = this.TaskContainsMethod(((Behavior.EventTypes)i).ToString(), this.mBehaviorSource.RootTask);
                    this.hasEvent[i] = false;
                }
				this.initialized = true;
			}
		}

		/// <summary>
        /// 关闭行为树
        /// </summary>
		public void DisableBehavior()
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, this.pauseWhenDisabled);
				this.isPaused = this.pauseWhenDisabled;
			}
		}

		/// <summary>
        /// 暂停行为树  关闭行为树
        /// </summary>
        /// <param name="pause"></param>
		public void DisableBehavior(bool pause)
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, pause);
				this.isPaused = pause;
			}
		}

        /// <summary>
        /// Unity 对象OnEnable事件
        /// </summary>
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

        /// <summary>
        /// Unity 对象OnDisable事件
        /// </summary>
        public void OnDisable()
		{
			this.DisableBehavior();
		}

        /// <summary>
        /// Unity 对象OnDestroy事件
        /// </summary>
        public void OnDestroy()
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DestroyBehavior(this);
			}
		}

		/// <summary>
        /// 获取指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public SharedVariable GetVariable(string name)
		{
			this.CheckForSerialization();
			return this.mBehaviorSource.GetVariable(name);
		}

		/// <summary>
        /// 设置指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
		public void SetVariable(string name, SharedVariable item)
		{
			this.CheckForSerialization();
			this.mBehaviorSource.SetVariable(name, item);
		}

		/// <summary>
        /// 设置指定共享名字变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void SetVariableValue(string name, object value)
		{
			SharedVariable variable = this.GetVariable(name);
			if (variable != null)
			{
				variable.SetValue(value);
				variable.ValueChanged();
			}
			else
			{
				Debug.LogError("Error: No variable exists with name " + name);
			}
		}

		/// <summary>
        /// 返回所有的共享变量列表
        /// </summary>
        /// <returns></returns>
		public List<SharedVariable> GetAllVariables()
		{
			this.CheckForSerialization();
			return this.mBehaviorSource.GetAllVariables();
		}

		/// <summary>
        /// 反序列化资源
        /// </summary>
		public void CheckForSerialization()
		{
			if (this.externalBehavior != null)
			{
				List<SharedVariable> list = null;
				bool force = false;
				if (!this.hasInheritedVariables)
				{
					this.mBehaviorSource.CheckForSerialization(false, null);
					list = this.mBehaviorSource.GetAllVariables();
					this.hasInheritedVariables = true;
					force = true;
				}
				this.externalBehavior.BehaviorSource.Owner = this.ExternalBehavior;
				this.externalBehavior.BehaviorSource.CheckForSerialization(force, this.GetBehaviorSource());
				this.externalBehavior.BehaviorSource.EntryTask = this.mBehaviorSource.EntryTask;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != null)
						{
							this.mBehaviorSource.SetVariable(list[i].Name, list[i]);
						}
					}
				}
			}
			else
			{
				this.mBehaviorSource.CheckForSerialization(false, null);
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000026A8 File Offset: 0x000008A8
		public void OnCollisionEnter(Collision collision)
		{
			if (this.hasEvent[0] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnCollisionEnter(collision, this);
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000026D4 File Offset: 0x000008D4
		public void OnCollisionExit(Collision collision)
		{
			if (this.hasEvent[1] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnCollisionExit(collision, this);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002700 File Offset: 0x00000900
		public void OnTriggerEnter(Collider other)
		{
			if (this.hasEvent[2] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnTriggerEnter(other, this);
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000272C File Offset: 0x0000092C
		public void OnTriggerExit(Collider other)
		{
			if (this.hasEvent[3] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnTriggerExit(other, this);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002758 File Offset: 0x00000958
		public void OnCollisionEnter2D(Collision2D collision)
		{
			if (this.hasEvent[4] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnCollisionEnter2D(collision, this);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002784 File Offset: 0x00000984
		public void OnCollisionExit2D(Collision2D collision)
		{
			if (this.hasEvent[5] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnCollisionExit2D(collision, this);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000027B0 File Offset: 0x000009B0
		public void OnTriggerEnter2D(Collider2D other)
		{
			if (this.hasEvent[6] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnTriggerEnter2D(other, this);
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000027DC File Offset: 0x000009DC
		public void OnTriggerExit2D(Collider2D other)
		{
			if (this.hasEvent[7] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnTriggerExit2D(other, this);
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002808 File Offset: 0x00000A08
		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (this.hasEvent[8] && BehaviorManager.instance != null)
			{
				BehaviorManager.instance.BehaviorOnControllerColliderHit(hit, this);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002834 File Offset: 0x00000A34
		public void OnDrawGizmos()
		{
			this.DrawTaskGizmos(false);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002840 File Offset: 0x00000A40
		public void OnDrawGizmosSelected()
		{
			if (this.showBehaviorDesignerGizmo)
			{
				Gizmos.DrawIcon(base.transform.position, "Behavior Designer Scene Icon.png");
			}
			this.DrawTaskGizmos(true);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000286C File Offset: 0x00000A6C
		private void DrawTaskGizmos(bool selected)
		{

            if (this.gizmoViewMode == Behavior.GizmoViewMode.Never || (this.gizmoViewMode == Behavior.GizmoViewMode.Selected && !selected))
            {
                return;
            }
            if (this.gizmoViewMode == Behavior.GizmoViewMode.Running || this.gizmoViewMode == Behavior.GizmoViewMode.Always || (Application.isPlaying && this.ExecutionStatus == TaskStatus.Running) || !Application.isPlaying)
            {
                this.CheckForSerialization();
                this.DrawTaskGizmos(this.mBehaviorSource.RootTask);
                List<Task> detachedTasks = this.mBehaviorSource.DetachedTasks;
                if (detachedTasks != null)
                {
                    for (int i = 0; i < detachedTasks.Count; i++)
                    {
                        this.DrawTaskGizmos(detachedTasks[i]);
                    }
                }
            }
        }

		// Token: 0x0600003C RID: 60 RVA: 0x0000291C File Offset: 0x00000B1C
		private void DrawTaskGizmos(Task task)
		{
			if (task == null)
			{
				return;
			}
			if (this.gizmoViewMode == Behavior.GizmoViewMode.Running && !task.NodeData.IsReevaluating && (task.NodeData.IsReevaluating || task.NodeData.ExecutionStatus != TaskStatus.Running))
			{
				return;
			}
			task.OnDrawGizmos();
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						this.DrawTaskGizmos(parentTask.Children[i]);
					}
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000029C0 File Offset: 0x00000BC0
		public T FindTask<T>() where T : Task
		{
			return this.FindTask<T>(this.mBehaviorSource.RootTask);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000029D4 File Offset: 0x00000BD4
		private T FindTask<T>(Task task) where T : Task
		{
			if (task.GetType().Equals(typeof(T)))
			{
				return (T)((object)task);
			}
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					T result = (T)((object)null);
					if ((result = this.FindTask<T>(parentTask.Children[i])) != null)
					{
						return result;
					}
				}
			}
			return (T)((object)null);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002A64 File Offset: 0x00000C64
		public List<T> FindTasks<T>() where T : Task
		{
			this.CheckForSerialization();
			List<T> result = new List<T>();
			this.FindTasks<T>(this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002A94 File Offset: 0x00000C94
		private void FindTasks<T>(Task task, ref List<T> taskList) where T : Task
		{
			if (typeof(T).IsAssignableFrom(task.GetType()))
			{
				taskList.Add((T)((object)task));
			}
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					this.FindTasks<T>(parentTask.Children[i], ref taskList);
				}
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002B10 File Offset: 0x00000D10
		public Task FindTaskWithName(string taskName)
		{
			this.CheckForSerialization();
			return this.FindTaskWithName(taskName, this.mBehaviorSource.RootTask);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002B2C File Offset: 0x00000D2C
		private Task FindTaskWithName(string taskName, Task task)
		{
			if (task.FriendlyName.Equals(taskName))
			{
				return task;
			}
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					Task result;
					if ((result = this.FindTaskWithName(taskName, parentTask.Children[i])) != null)
					{
						return result;
					}
				}
			}
			return null;
		}

        /// <summary>
        /// 查找行为树中所有 FriendlyName 为 taskName 的任务节点
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public List<Task> FindTasksWithName(string taskName)
		{
			this.CheckForSerialization();
			List<Task> result = new List<Task>();
			this.FindTasksWithName(taskName, this.mBehaviorSource.RootTask, ref result);
			return result;
		}

        /// <summary>
        /// 查找 task 下 FriendlyName 为 taskName 的所有任务列表
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        /// <param name="taskList"></param>
        private void FindTasksWithName(string taskName, Task task, ref List<Task> taskList)
		{
			if (task.FriendlyName.Equals(taskName))
			{
				taskList.Add(task);
			}
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					this.FindTasksWithName(taskName, parentTask.Children[i], ref taskList);
				}
			}
		}

		/// <summary>
        /// 获取行为树中所有激活的任务列表
        /// </summary>
        /// <returns></returns>
		public List<Task> GetActiveTasks()
		{
			if (BehaviorManager.instance == null)
			{
				return null;
			}
			return BehaviorManager.instance.GetActiveTasks(this);
		}

        /// <summary>
        /// 通过 task 的 methodName 启动协程 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public Coroutine StartTaskCoroutine(Task task, string methodName)
		{
			MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
				return null;
			}
			if (this.activeTaskCoroutines == null)
			{
				this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
			}
			TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[0]), methodName);
			if (this.activeTaskCoroutines.ContainsKey(methodName))
			{
				List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
				list.Add(taskCoroutine);
				this.activeTaskCoroutines[methodName] = list;
			}
			else
			{
				List<TaskCoroutine> list2 = new List<TaskCoroutine>();
				list2.Add(taskCoroutine);
				this.activeTaskCoroutines.Add(methodName, list2);
			}
			return taskCoroutine.Coroutine;
		}

        /// <summary>
        /// 通过 task 的 methodName 启动协程 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Coroutine StartTaskCoroutine(Task task, string methodName, object value)
		{
			MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
				return null;
			}
			if (this.activeTaskCoroutines == null)
			{
				this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
			}
			TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[]
			{
				value
			}), methodName);
			if (this.activeTaskCoroutines.ContainsKey(methodName))
			{
				List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
				list.Add(taskCoroutine);
				this.activeTaskCoroutines[methodName] = list;
			}
			else
			{
				List<TaskCoroutine> list2 = new List<TaskCoroutine>();
				list2.Add(taskCoroutine);
				this.activeTaskCoroutines.Add(methodName, list2);
			}
			return taskCoroutine.Coroutine;
		}

        /// <summary>
        /// 停止名字为 methodName 的所有 TaskCoroutine
        /// </summary>
        /// <param name="methodName"></param>
        public void StopTaskCoroutine(string methodName)
		{
			if (!this.activeTaskCoroutines.ContainsKey(methodName))
			{
				return;
			}
			List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Stop();
			}
		}

        /// <summary>
        /// 停止所有的 TaskCoroutine
        /// </summary>
        public void StopAllTaskCoroutines()
		{
			base.StopAllCoroutines();
			foreach (KeyValuePair<string, List<TaskCoroutine>> keyValuePair in this.activeTaskCoroutines)
			{
				List<TaskCoroutine> value = keyValuePair.Value;
				for (int i = 0; i < value.Count; i++)
				{
					value[i].Stop();
				}
			}
		}

        /// <summary>
        /// 某个 TaskCoroutine 完成后通知清理
        /// </summary>
        /// <param name="taskCoroutine"></param>
        /// <param name="coroutineName"></param>
        public void TaskCoroutineEnded(TaskCoroutine taskCoroutine, string coroutineName)
		{
			if (this.activeTaskCoroutines.ContainsKey(coroutineName))
			{
				List<TaskCoroutine> list = this.activeTaskCoroutines[coroutineName];
				if (list.Count == 1)
				{
					this.activeTaskCoroutines.Remove(coroutineName);
				}
				else
				{
					list.Remove(taskCoroutine);
					this.activeTaskCoroutines[coroutineName] = list;
				}
			}
		}

		/// <summary>
        /// 启动事件
        /// </summary>
		public void OnBehaviorStarted()
		{
			if (this.OnBehaviorStart != null)
			{
				this.OnBehaviorStart();
			}
		}

		/// <summary>
        /// 重新启动事件
        /// </summary>
		public void OnBehaviorRestarted()
		{
			if (this.OnBehaviorRestart != null)
			{
				this.OnBehaviorRestart();
			}
		}

		/// <summary>
        /// 结束事件
        /// </summary>
		public void OnBehaviorEnded()
		{
			if (this.OnBehaviorEnd != null)
			{
				this.OnBehaviorEnd();
			}
		}

		/// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
		private void RegisterEvent(string name, Delegate handler)
		{
			if (this.eventTable == null)
			{
				this.eventTable = new Dictionary<Type, Dictionary<string, Delegate>>();
			}
			Dictionary<string, Delegate> dictionary;
			if (!this.eventTable.TryGetValue(handler.GetType(), out dictionary))
			{
				dictionary = new Dictionary<string, Delegate>();
				this.eventTable.Add(handler.GetType(), dictionary);
			}
			Delegate a;
			if (dictionary.TryGetValue(name, out a))
			{
				dictionary[name] = Delegate.Combine(a, handler);
			}
			else
			{
				dictionary.Add(name, handler);
			}
		}

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RegisterEvent(string name, System.Action handler)
		{
			this.RegisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RegisterEvent<T>(string name, Action<T> handler)
		{
			this.RegisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RegisterEvent<T, U>(string name, Action<T, U> handler)
		{
			this.RegisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			this.RegisterEvent(name, handler as Delegate);
		}

		/// <summary>
        /// 获取指定
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		private Delegate GetDelegate(string name, Type type)
		{
			Dictionary<string, Delegate> dictionary;
			Delegate result;
			if (this.eventTable != null && this.eventTable.TryGetValue(type, out dictionary) && dictionary.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		/// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="name"></param>
		public void SendEvent(string name)
		{
			System.Action action = this.GetDelegate(name, typeof(System.Action)) as System.Action;
			if (action != null)
			{
				action();
			}
		}

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        public void SendEvent<T>(string name, T arg1)
		{
			Action<T> action = this.GetDelegate(name, typeof(Action<T>)) as Action<T>;
			if (action != null)
			{
				action(arg1);
			}
		}

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void SendEvent<T, U>(string name, T arg1, U arg2)
		{
			Action<T, U> action = this.GetDelegate(name, typeof(Action<T, U>)) as Action<T, U>;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3)
		{
			Action<T, U, V> action = this.GetDelegate(name, typeof(Action<T, U, V>)) as Action<T, U, V>;
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}

		/// <summary>
        /// 取消事件注册
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
		private void UnregisterEvent(string name, Delegate handler)
		{
			if (this.eventTable == null)
			{
				return;
			}
			Dictionary<string, Delegate> dictionary;
			Delegate source;
			if (this.eventTable.TryGetValue(handler.GetType(), out dictionary) && dictionary.TryGetValue(name, out source))
			{
				dictionary[name] = Delegate.Remove(source, handler);
			}
		}

        /// <summary>
        /// 取消事件注册
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void UnregisterEvent(string name, System.Action handler)
		{
			this.UnregisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 取消事件注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void UnregisterEvent<T>(string name, Action<T> handler)
		{
			this.UnregisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 取消事件注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void UnregisterEvent<T, U>(string name, Action<T, U> handler)
		{
			this.UnregisterEvent(name, handler as Delegate);
		}

        /// <summary>
        /// 取消事件注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			this.UnregisterEvent(name, handler as Delegate);
		}

		/// <summary>
        /// 保存需要重置的值
        /// </summary>
		public void SaveResetValues()
		{
			if (this.defaultValues == null)
			{
				this.defaultValues = new List<Dictionary<string, object>>();
				this.defaultVariableValues = new Dictionary<string, object>();
				this.SaveValues();
			}
			else
			{
				this.ResetValues();
			}
		}

		/// <summary>
        /// 将需要重置的值缓存起来
        /// </summary>
		private void SaveValues()
		{
			List<SharedVariable> allVariables = this.mBehaviorSource.GetAllVariables();
			if (allVariables != null)
			{
				for (int i = 0; i < allVariables.Count; i++)
				{
					this.defaultVariableValues.Add(allVariables[i].Name, allVariables[i].GetValue());
				}
			}
			this.SaveValue(this.mBehaviorSource.RootTask);
		}

		/// <summary>
        /// 保存任务的默认值
        /// </summary>
        /// <param name="task"></param>
		private void SaveValue(Task task)
		{
			if (task == null)
			{
				return;
			}
			FieldInfo[] publicFields = TaskUtility.GetPublicFields(task.GetType());
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int i = 0;
			while (i < publicFields.Length)
			{
				object value = publicFields[i].GetValue(task);
				if (!(value is SharedVariable))
				{
					goto IL_5A;
				}
				SharedVariable sharedVariable = value as SharedVariable;
				if (!sharedVariable.IsGlobal && !sharedVariable.IsShared)
				{
					goto IL_5A;
				}
				IL_71:
				i++;
				continue;
				IL_5A:
				dictionary.Add(publicFields[i].Name, publicFields[i].GetValue(task));
				goto IL_71;
			}
			this.defaultValues.Add(dictionary);
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int j = 0; j < parentTask.Children.Count; j++)
					{
						this.SaveValue(parentTask.Children[j]);
					}
				}
			}
		}

		/// <summary>
        /// 重置行为树的默认值
        /// </summary>
		private void ResetValues()
		{
			foreach (KeyValuePair<string, object> keyValuePair in this.defaultVariableValues)
			{
				this.SetVariableValue(keyValuePair.Key, keyValuePair.Value);
			}
			int num = 0;
			this.ResetValue(this.mBehaviorSource.RootTask, ref num);
		}

		/// <summary>
        /// 重置任务节点的默认数据
        /// </summary>
        /// <param name="task"></param>
        /// <param name="index"></param>
		private void ResetValue(Task task, ref int index)
		{
			if (task == null || index >= this.defaultValues.Count)
			{
				return;
			}
			Dictionary<string, object> dictionary = this.defaultValues[index];
			index++;
			foreach (KeyValuePair<string, object> keyValuePair in dictionary)
			{
				FieldInfo field = task.GetType().GetField(keyValuePair.Key);
				if (field != null)
				{
					field.SetValue(task, keyValuePair.Value);
				}
			}
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						this.ResetValue(parentTask.Children[i], ref index);
					}
				}
			}
		}

		public override string ToString()
		{
			return this.mBehaviorSource.ToString();
		}

        /// <summary>
        /// 构造 BehaviorManager
        /// </summary> 
        /// <returns></returns>
        public static BehaviorManager CreateBehaviorManager()
		{
			if (BehaviorManager.instance == null && Application.isPlaying)
			{
				return new GameObject
				{
					name = "Behavior Manager"
				}.AddComponent<BehaviorManager>();
			}
			return null;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003514 File Offset: 0x00001714
		virtual public int GetInstanceID()
		{
			return base.GetInstanceID();
		}

		/// <summary>
        /// 标记为自动启动
        /// </summary>
		[SerializeField]
		private bool startWhenEnabled = true;

        /// <summary>
        /// disable 时为暂停
        /// </summary>
        [SerializeField]
		private bool pauseWhenDisabled;

		/// <summary>
        /// 结束后重新启动
        /// </summary>
		[SerializeField]
		private bool restartWhenComplete;

		/// <summary>
        /// 打印节点变化日志
        /// </summary>
		[SerializeField]
		private bool logTaskChanges;

		/// <summary>
        /// 组ID
        /// </summary>
		[SerializeField]
		private int group;

        /// <summary>
        /// Restart 时重置
        /// </summary>
        [SerializeField]
		private bool resetValuesOnRestart;

        /// <summary>
        /// 行为树的资源文件对象
        /// </summary>
        [SerializeField]
		private ExternalBehavior externalBehavior;

        /// <summary>
        /// 标记是否继承过变量
        /// </summary>
        private bool hasInheritedVariables;

		/// <summary>
        /// 行为树资源管理对象，Behavior 实例化时初始化
        /// </summary>
		[SerializeField]
		private BehaviorSource mBehaviorSource;

		/// <summary>
        /// 是否暂停
        /// </summary>
		private bool isPaused;

		/// <summary>
        /// 执行状态
        /// </summary>
		private TaskStatus executionStatus;

		/// <summary>
        /// 标记是否初始化，重新设置行为树资源时设置为false，启动完成后设置为true
        /// </summary>
		private bool initialized;

		/// <summary>
        /// 每个节点的字段的默认值缓存
        /// </summary>
		private List<Dictionary<string, object>> defaultValues;

		/// <summary>
        /// 共享变量的默认值缓存
        /// </summary>
		private Dictionary<string, object> defaultVariableValues;

		/// <summary>
        /// 标记包含事件类型
        /// </summary>
		private bool[] hasEvent = new bool[11];

        /// <summary>
        /// TaskCoroutine 的缓存容器
        /// </summary>
        private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines;

		/// <summary>
        /// 注册的事件集合
        /// </summary>
		private Dictionary<Type, Dictionary<string, Delegate>> eventTable;

        /// <summary>
        /// GizmoView 绘制模式
        /// </summary>
        [NonSerialized]
		public Behavior.GizmoViewMode gizmoViewMode;

		// Token: 0x04000013 RID: 19
		[NonSerialized]
		public bool showBehaviorDesignerGizmo = true;

		/// <summary>
        /// 事件类型枚举，与事件名相同
        /// </summary>
		public enum EventTypes
		{
			OnCollisionEnter,
			OnCollisionExit,
			OnTriggerEnter,
			OnTriggerExit,
			OnCollisionEnter2D,
			OnCollisionExit2D,
			OnTriggerEnter2D,
			OnTriggerExit2D,
			OnControllerColliderHit,
			OnLateUpdate,
			OnFixedUpdate,
			None
		}
        /// <summary>
        /// GizmoView 绘制模式
        /// </summary>
		public enum GizmoViewMode
		{
			Running,
			Always,
			Selected,
			Never
		}

		/// <summary>
        /// 行为树的事件委托类型
        /// </summary>
		public delegate void BehaviorHandler();
	}
}
