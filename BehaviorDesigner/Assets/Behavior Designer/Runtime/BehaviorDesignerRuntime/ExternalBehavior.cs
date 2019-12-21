using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    /// <summary>
    /// 行为树资源 ExternalBehaviorTree 的基类
    /// </summary>
    [Serializable]
	public abstract class ExternalBehavior : ScriptableObject, IBehavior
	{
        /// <summary>
        /// BehaviorSource
        /// </summary>
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

        /// <summary>
        /// 返回 BehaviorSource
        /// </summary>
        /// <returns></returns>
        public BehaviorSource GetBehaviorSource()
		{
			return this.mBehaviorSource;
		}

        /// <summary>
        /// 设置 BehaviorSource
        /// </summary>
        /// <param name="behaviorSource"></param>
        public void SetBehaviorSource(BehaviorSource behaviorSource)
		{
			this.mBehaviorSource = behaviorSource;
		}

		/// <summary>
        /// 返回资源对象
        /// </summary>
        /// <returns></returns>
		public UnityEngine.Object GetObject()
		{
			return this;
		}

		/// <summary>
        /// 返回名字
        /// </summary>
        /// <returns></returns>
		public string GetOwnerName()
		{
			return base.name;
		}

		/// <summary>
        /// 获得指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public SharedVariable GetVariable(string name)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			return this.mBehaviorSource.GetVariable(name);
		}

		/// <summary>
        /// 添加指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
		public void SetVariable(string name, SharedVariable item)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			this.mBehaviorSource.SetVariable(name, item);
		}

		/// <summary>
        /// 设置指定名字共享变量的值
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
		}

		/// <summary>
        /// 获取指定类型的节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T FindTask<T>() where T : Task
		{
			return this.FindTask<T>(this.mBehaviorSource.RootTask);
		}

		/// <summary>
        /// 获取指定节点及以下的指定类型节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
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

		/// <summary>
        /// 获取指定类型的节点列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public List<T> FindTasks<T>() where T : Task
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			List<T> result = new List<T>();
			this.FindTasks<T>(this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		/// <summary>
        /// 获取指定节点及以下的指定类型的节点列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="taskList"></param>
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

		/// <summary>
        /// 查找指定名字的节点
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
		public Task FindTaskWithName(string taskName)
		{
			this.mBehaviorSource.CheckForSerialization(false, null);
			return this.FindTaskWithName(taskName, this.mBehaviorSource.RootTask);
		}

		/// <summary>
        /// 查找指定节点及以下的的指定名字的节点
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
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
        /// 查找指定名字的节点列表
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
		public List<Task> FindTasksWithName(string taskName)
		{
			List<Task> result = new List<Task>();
			this.FindTasksWithName(taskName, this.mBehaviorSource.RootTask, ref result);
			return result;
		}

		/// <summary>
        /// 查找指定节点及以下的指定名字的节点列表
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

		// Token: 0x04000083 RID: 131
		[SerializeField]
		private BehaviorSource mBehaviorSource;
	}
}
