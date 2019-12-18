using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public abstract class ExternalBehavior : ScriptableObject, IBehavior
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000B4C8 File Offset: 0x000096C8
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000B4D0 File Offset: 0x000096D0
		public BehaviorSource BehaviorSource
		{
			get
			{
				return this.mBehaviorSource;
			}
			set
			{
				this.mBehaviorSource = value;
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000B4DC File Offset: 0x000096DC
		public BehaviorSource GetBehaviorSource()
		{
			return this.mBehaviorSource;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000B4E4 File Offset: 0x000096E4
		public void SetBehaviorSource(BehaviorSource behaviorSource)
		{
			this.mBehaviorSource = behaviorSource;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000B4F0 File Offset: 0x000096F0
		public UnityEngine.Object GetObject()
		{
			return this;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000B4F4 File Offset: 0x000096F4
		public string GetOwnerName()
		{
			return base.name;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000B4FC File Offset: 0x000096FC
		public SharedVariable GetVariable(string name)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			return this.mBehaviorSource.GetVariable(name);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000B518 File Offset: 0x00009718
		public void SetVariable(string name, SharedVariable item)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			this.mBehaviorSource.SetVariable(name, item);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000B538 File Offset: 0x00009738
		public void SetVariableValue(string name, object value)
		{
			SharedVariable variable = this.GetVariable(name);
			if (variable != null)
			{
				variable.SetValue(value);
				variable.ValueChanged();
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000B560 File Offset: 0x00009760
		public T FindTask<T>() where T : Task
		{
			return this.FindTask<T>(this.mBehaviorSource.RootTask);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000B574 File Offset: 0x00009774
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

		// Token: 0x0600012F RID: 303 RVA: 0x0000B604 File Offset: 0x00009804
		public List<T> FindTasks<T>() where T : Task
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			List<T> result = new List<T>();
			this.FindTasks<T>(this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000B63C File Offset: 0x0000983C
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

		// Token: 0x06000131 RID: 305 RVA: 0x0000B6B8 File Offset: 0x000098B8
		public Task FindTaskWithName(string taskName)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			return this.FindTaskWithName(taskName, this.mBehaviorSource.RootTask);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000B6E8 File Offset: 0x000098E8
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

		// Token: 0x06000133 RID: 307 RVA: 0x0000B75C File Offset: 0x0000995C
		public List<Task> FindTasksWithName(string taskName)
		{
			List<Task> result = new List<Task>();
			this.FindTasksWithName(taskName, this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000B784 File Offset: 0x00009984
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

		// Token: 0x04000083 RID: 131
		[SerializeField]
		private BehaviorSource mBehaviorSource;
	}
}
