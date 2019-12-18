using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public abstract class Behavior : MonoBehaviour, IBehavior
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000020EC File Offset: 0x000002EC
		public Behavior()
		{
			this.mBehaviorSource = new BehaviorSource(this);
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000002 RID: 2 RVA: 0x0000211C File Offset: 0x0000031C
		// (remove) Token: 0x06000003 RID: 3 RVA: 0x00002138 File Offset: 0x00000338
		public event Behavior.BehaviorHandler OnBehaviorStart;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000004 RID: 4 RVA: 0x00002154 File Offset: 0x00000354
		// (remove) Token: 0x06000005 RID: 5 RVA: 0x00002170 File Offset: 0x00000370
		public event Behavior.BehaviorHandler OnBehaviorRestart;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000006 RID: 6 RVA: 0x0000218C File Offset: 0x0000038C
		// (remove) Token: 0x06000007 RID: 7 RVA: 0x000021A8 File Offset: 0x000003A8
		public event Behavior.BehaviorHandler OnBehaviorEnd;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021C4 File Offset: 0x000003C4
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000021CC File Offset: 0x000003CC
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

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021D8 File Offset: 0x000003D8
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000021E0 File Offset: 0x000003E0
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000021EC File Offset: 0x000003EC
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000021F4 File Offset: 0x000003F4
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

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002200 File Offset: 0x00000400
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002208 File Offset: 0x00000408
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002214 File Offset: 0x00000414
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000221C File Offset: 0x0000041C
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002228 File Offset: 0x00000428
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002230 File Offset: 0x00000430
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000223C File Offset: 0x0000043C
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002244 File Offset: 0x00000444
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002298 File Offset: 0x00000498
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000022A0 File Offset: 0x000004A0
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000018 RID: 24 RVA: 0x000022AC File Offset: 0x000004AC
		// (set) Token: 0x06000019 RID: 25 RVA: 0x000022BC File Offset: 0x000004BC
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000022CC File Offset: 0x000004CC
		// (set) Token: 0x0600001B RID: 27 RVA: 0x000022DC File Offset: 0x000004DC
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

		// Token: 0x0600001C RID: 28 RVA: 0x000022EC File Offset: 0x000004EC
		public BehaviorSource GetBehaviorSource()
		{
			return this.mBehaviorSource;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000022F4 File Offset: 0x000004F4
		public void SetBehaviorSource(BehaviorSource behaviorSource)
		{
			this.mBehaviorSource = behaviorSource;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002300 File Offset: 0x00000500
		public UnityEngine.Object GetObject()
		{
			return this;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002304 File Offset: 0x00000504
		public string GetOwnerName()
		{
			return base.gameObject.name;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002314 File Offset: 0x00000514
		// (set) Token: 0x06000021 RID: 33 RVA: 0x0000231C File Offset: 0x0000051C
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002328 File Offset: 0x00000528
		public bool[] HasEvent
		{
			get
			{
				return this.hasEvent;
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002330 File Offset: 0x00000530
		public void Start()
		{
			if (this.startWhenEnabled)
			{
				this.EnableBehavior();
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002344 File Offset: 0x00000544
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

		// Token: 0x06000025 RID: 37 RVA: 0x000023DC File Offset: 0x000005DC
		public void EnableBehavior()
		{
			Behavior.CreateBehaviorManager();
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.EnableBehavior(this);
			}
			if (!this.initialized)
			{
				for (int i = 0; i < 11; i++)
				{
					this.hasEvent[i] = this.TaskContainsMethod(((Behavior.EventTypes)i).ToString(), this.mBehaviorSource.RootTask);
				}
				this.initialized = true;
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002454 File Offset: 0x00000654
		public void DisableBehavior()
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, this.pauseWhenDisabled);
				this.isPaused = this.pauseWhenDisabled;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002484 File Offset: 0x00000684
		public void DisableBehavior(bool pause)
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, pause);
				this.isPaused = pause;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000024AC File Offset: 0x000006AC
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

		// Token: 0x06000029 RID: 41 RVA: 0x00002508 File Offset: 0x00000708
		public void OnDisable()
		{
			this.DisableBehavior();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002510 File Offset: 0x00000710
		public void OnDestroy()
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DestroyBehavior(this);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002530 File Offset: 0x00000730
		public SharedVariable GetVariable(string name)
		{
			this.CheckForSerialization();
			return this.mBehaviorSource.GetVariable(name);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002544 File Offset: 0x00000744
		public void SetVariable(string name, SharedVariable item)
		{
			this.CheckForSerialization();
			this.mBehaviorSource.SetVariable(name, item);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000255C File Offset: 0x0000075C
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

		// Token: 0x0600002E RID: 46 RVA: 0x0000259C File Offset: 0x0000079C
		public List<SharedVariable> GetAllVariables()
		{
			this.CheckForSerialization();
			return this.mBehaviorSource.GetAllVariables();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000025B0 File Offset: 0x000007B0
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

		// Token: 0x06000043 RID: 67 RVA: 0x00002BA0 File Offset: 0x00000DA0
		public List<Task> FindTasksWithName(string taskName)
		{
			this.CheckForSerialization();
			List<Task> result = new List<Task>();
			this.FindTasksWithName(taskName, this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002BD0 File Offset: 0x00000DD0
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

		// Token: 0x06000045 RID: 69 RVA: 0x00002C40 File Offset: 0x00000E40
		public List<Task> GetActiveTasks()
		{
			if (BehaviorManager.instance == null)
			{
				return null;
			}
			return BehaviorManager.instance.GetActiveTasks(this);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002C60 File Offset: 0x00000E60
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

		// Token: 0x06000047 RID: 71 RVA: 0x00002D20 File Offset: 0x00000F20
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

		// Token: 0x06000048 RID: 72 RVA: 0x00002DE4 File Offset: 0x00000FE4
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

		// Token: 0x06000049 RID: 73 RVA: 0x00002E34 File Offset: 0x00001034
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

		// Token: 0x0600004A RID: 74 RVA: 0x00002EC4 File Offset: 0x000010C4
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

		// Token: 0x0600004B RID: 75 RVA: 0x00002F24 File Offset: 0x00001124
		public void OnBehaviorStarted()
		{
			if (this.OnBehaviorStart != null)
			{
				this.OnBehaviorStart();
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002F3C File Offset: 0x0000113C
		public void OnBehaviorRestarted()
		{
			if (this.OnBehaviorRestart != null)
			{
				this.OnBehaviorRestart();
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002F54 File Offset: 0x00001154
		public void OnBehaviorEnded()
		{
			if (this.OnBehaviorEnd != null)
			{
				this.OnBehaviorEnd();
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002F6C File Offset: 0x0000116C
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

		// Token: 0x0600004F RID: 79 RVA: 0x00002FE8 File Offset: 0x000011E8
		public void RegisterEvent(string name, System.Action handler)
		{
			this.RegisterEvent(name, handler);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002FF4 File Offset: 0x000011F4
		public void RegisterEvent<T>(string name, Action<T> handler)
		{
			this.RegisterEvent(name, handler);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003000 File Offset: 0x00001200
		public void RegisterEvent<T, U>(string name, Action<T, U> handler)
		{
			this.RegisterEvent(name, handler);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000300C File Offset: 0x0000120C
		public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			this.RegisterEvent(name, handler);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003018 File Offset: 0x00001218
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

		// Token: 0x06000054 RID: 84 RVA: 0x00003054 File Offset: 0x00001254
		public void SendEvent(string name)
		{
			System.Action action = this.GetDelegate(name, typeof(System.Action)) as System.Action;
			if (action != null)
			{
				action();
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003084 File Offset: 0x00001284
		public void SendEvent<T>(string name, T arg1)
		{
			Action<T> action = this.GetDelegate(name, typeof(Action<T>)) as Action<T>;
			if (action != null)
			{
				action(arg1);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000030B8 File Offset: 0x000012B8
		public void SendEvent<T, U>(string name, T arg1, U arg2)
		{
			Action<T, U> action = this.GetDelegate(name, typeof(Action<T, U>)) as Action<T, U>;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000030EC File Offset: 0x000012EC
		public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3)
		{
			Action<T, U, V> action = this.GetDelegate(name, typeof(Action<T, U, V>)) as Action<T, U, V>;
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003120 File Offset: 0x00001320
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

		// Token: 0x06000059 RID: 89 RVA: 0x00003170 File Offset: 0x00001370
		public void UnregisterEvent(string name, System.Action handler)
		{
			this.UnregisterEvent(name, handler);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000317C File Offset: 0x0000137C
		public void UnregisterEvent<T>(string name, Action<T> handler)
		{
			this.UnregisterEvent(name, handler);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003188 File Offset: 0x00001388
		public void UnregisterEvent<T, U>(string name, Action<T, U> handler)
		{
			this.UnregisterEvent(name, handler);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003194 File Offset: 0x00001394
		public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			this.UnregisterEvent(name, handler);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000031A0 File Offset: 0x000013A0
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

		// Token: 0x0600005E RID: 94 RVA: 0x000031E0 File Offset: 0x000013E0
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

		// Token: 0x0600005F RID: 95 RVA: 0x0000324C File Offset: 0x0000144C
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

		// Token: 0x06000060 RID: 96 RVA: 0x00003338 File Offset: 0x00001538
		private void ResetValues()
		{
			foreach (KeyValuePair<string, object> keyValuePair in this.defaultVariableValues)
			{
				this.SetVariableValue(keyValuePair.Key, keyValuePair.Value);
			}
			int num = 0;
			this.ResetValue(this.mBehaviorSource.RootTask, ref num);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000033C0 File Offset: 0x000015C0
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

		// Token: 0x06000062 RID: 98 RVA: 0x000034C4 File Offset: 0x000016C4
		public override string ToString()
		{
			return this.mBehaviorSource.ToString();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000034D4 File Offset: 0x000016D4
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

		// Token: 0x04000001 RID: 1
		[SerializeField]
		private bool startWhenEnabled = true;

		// Token: 0x04000002 RID: 2
		[SerializeField]
		private bool pauseWhenDisabled;

		// Token: 0x04000003 RID: 3
		[SerializeField]
		private bool restartWhenComplete;

		// Token: 0x04000004 RID: 4
		[SerializeField]
		private bool logTaskChanges;

		// Token: 0x04000005 RID: 5
		[SerializeField]
		private int group;

		// Token: 0x04000006 RID: 6
		[SerializeField]
		private bool resetValuesOnRestart;

		// Token: 0x04000007 RID: 7
		[SerializeField]
		private ExternalBehavior externalBehavior;

		// Token: 0x04000008 RID: 8
		private bool hasInheritedVariables;

		// Token: 0x04000009 RID: 9
		[SerializeField]
		private BehaviorSource mBehaviorSource;

		// Token: 0x0400000A RID: 10
		private bool isPaused;

		// Token: 0x0400000B RID: 11
		private TaskStatus executionStatus;

		// Token: 0x0400000C RID: 12
		private bool initialized;

		// Token: 0x0400000D RID: 13
		private List<Dictionary<string, object>> defaultValues;

		// Token: 0x0400000E RID: 14
		private Dictionary<string, object> defaultVariableValues;

		// Token: 0x0400000F RID: 15
		private bool[] hasEvent = new bool[11];

		// Token: 0x04000010 RID: 16
		private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines;

		// Token: 0x04000011 RID: 17
		private Dictionary<Type, Dictionary<string, Delegate>> eventTable;

		// Token: 0x04000012 RID: 18
		[NonSerialized]
		public Behavior.GizmoViewMode gizmoViewMode;

		// Token: 0x04000013 RID: 19
		[NonSerialized]
		public bool showBehaviorDesignerGizmo = true;

		// Token: 0x02000003 RID: 3
		public enum EventTypes
		{
			// Token: 0x04000018 RID: 24
			OnCollisionEnter,
			// Token: 0x04000019 RID: 25
			OnCollisionExit,
			// Token: 0x0400001A RID: 26
			OnTriggerEnter,
			// Token: 0x0400001B RID: 27
			OnTriggerExit,
			// Token: 0x0400001C RID: 28
			OnCollisionEnter2D,
			// Token: 0x0400001D RID: 29
			OnCollisionExit2D,
			// Token: 0x0400001E RID: 30
			OnTriggerEnter2D,
			// Token: 0x0400001F RID: 31
			OnTriggerExit2D,
			// Token: 0x04000020 RID: 32
			OnControllerColliderHit,
			// Token: 0x04000021 RID: 33
			OnLateUpdate,
			// Token: 0x04000022 RID: 34
			OnFixedUpdate,
			// Token: 0x04000023 RID: 35
			None
		}

		// Token: 0x02000004 RID: 4
		public enum GizmoViewMode
		{
			// Token: 0x04000025 RID: 37
			Running,
			// Token: 0x04000026 RID: 38
			Always,
			// Token: 0x04000027 RID: 39
			Selected,
			// Token: 0x04000028 RID: 40
			Never
		}

		// Token: 0x0200004D RID: 77
		// (Invoke) Token: 0x06000261 RID: 609
		public delegate void BehaviorHandler();
	}
}
