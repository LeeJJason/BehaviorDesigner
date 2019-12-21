using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	/// <summary>
    /// 行为树资源类，用于管理解析后的行为树资源对象
    /// </summary>
	[Serializable]
	public class BehaviorSource : IVariableSource
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x0000797C File Offset: 0x00005B7C
		public BehaviorSource()
		{
		}

        /// <summary>
        /// 为owner构建 BehaviorSource
        /// </summary>
        /// <param name="owner"></param>
        public BehaviorSource(IBehavior owner)
		{
			this.Initialize(owner);
		}

		/// <summary>
        /// 定义自己的ID标识
        /// </summary>
		public int BehaviorID
		{
			get
			{
				return this.behaviorID;
			}
			set
			{
				this.behaviorID = value;
			}
		}

		/// <summary>
        /// 行为树的入口节点
        /// </summary>
		public Task EntryTask
		{
			get
			{
				return this.mEntryTask;
			}
			set
			{
				this.mEntryTask = value;
			}
		}

		/// <summary>
        /// 入口节点的第一个节点
        /// </summary>
		public Task RootTask
		{
			get
			{
				return this.mRootTask;
			}
			set
			{
				this.mRootTask = value;
			}
		}

		/// <summary>
        /// 分离的节点列表
        /// </summary>
		public List<Task> DetachedTasks
		{
			get
			{
				return this.mDetachedTasks;
			}
			set
			{
				this.mDetachedTasks = value;
			}
		}

		/// <summary>
        /// 共享的变量列表
        /// </summary>
		public List<SharedVariable> Variables
		{
			get
			{
				this.CheckForSerialization(false, null);
				return this.mVariables;
			}
			set
			{
				this.mVariables = value;
				this.UpdateVariablesIndex();
			}
		}

		/// <summary>
        /// 用于标记当前的行为树资源是否解析
        /// </summary>
		public bool HasSerialized
		{
			get
			{
				return this.mHasSerialized;
			}
			set
			{
				this.mHasSerialized = value;
			}
		}

		/// <summary>
        /// 节点序列化信息
        /// </summary>
		public TaskSerializationData TaskData
		{
			get
			{
				return this.mTaskData;
			}
			set
			{
				this.mTaskData = value;
			}
		}

		/// <summary>
        /// 所属者
        /// </summary>
		public IBehavior Owner
		{
			get
			{
				return this.mOwner;
			}
			set
			{
				this.mOwner = value;
			}
		}

		/// <summary>
        /// 初始化所熟者
        /// </summary>
        /// <param name="owner"></param>
		public void Initialize(IBehavior owner)
		{
			this.mOwner = owner;
		}

		/// <summary>
        /// 设置相关信息
        /// </summary>
        /// <param name="entryTask"></param>
        /// <param name="rootTask"></param>
        /// <param name="detachedTasks"></param>
		public void Save(Task entryTask, Task rootTask, List<Task> detachedTasks)
		{
			this.mEntryTask = entryTask;
			this.mRootTask = rootTask;
			this.mDetachedTasks = detachedTasks;
		}

		/// <summary>
        /// 加载相关的信息
        /// </summary>
        /// <param name="entryTask"></param>
        /// <param name="rootTask"></param>
        /// <param name="detachedTasks"></param>
		public void Load(out Task entryTask, out Task rootTask, out List<Task> detachedTasks)
		{
			entryTask = this.mEntryTask;
			rootTask = this.mRootTask;
			detachedTasks = this.mDetachedTasks;
		}

		/// <summary>
        /// 通过序列化信息解析数据
        /// </summary>
        /// <param name="force">强制解析</param>
        /// <param name="behaviorSource"></param>
        /// <returns></returns>
		public bool CheckForSerialization(bool force, BehaviorSource behaviorSource = null)
		{
			bool flag = (behaviorSource == null) ? this.HasSerialized : behaviorSource.HasSerialized;
			if (!flag || force)
			{
				if (behaviorSource != null)
				{
					behaviorSource.HasSerialized = true;
				}
				else
				{
					this.HasSerialized = true;
				}
				if (this.mTaskData != null && !string.IsNullOrEmpty(this.mTaskData.JSONSerialization))
				{
					JSONDeserialization.Load(this.mTaskData, (behaviorSource != null) ? behaviorSource : this);
				}
				else
				{
					BinaryDeserialization.Load(this.mTaskData, (behaviorSource != null) ? behaviorSource : this);
				}
				return true;
			}
			return false;
		}

		/// <summary>
        /// 通过共享变量名获得共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public SharedVariable GetVariable(string name)
		{
			if (name == null)
			{
				return null;
			}
			if (this.mVariables != null)
			{
				if (this.mSharedVariableIndex == null || this.mSharedVariableIndex.Count != this.mVariables.Count)
				{
					this.UpdateVariablesIndex();
				}
				int index;
				if (this.mSharedVariableIndex.TryGetValue(name, out index))
				{
					return this.mVariables[index];
				}
			}
			return null;
		}

		/// <summary>
        /// 获取所有的共享变量列表
        /// </summary>
        /// <returns></returns>
		public List<SharedVariable> GetAllVariables()
		{
			this.CheckForSerialization(false, null);
			return this.mVariables;
		}

		/// <summary>
        /// 设置指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sharedVariable"></param>
		public void SetVariable(string name, SharedVariable sharedVariable)
		{
			if (this.mVariables == null)
			{
				this.mVariables = new List<SharedVariable>();
			}
			else if (this.mSharedVariableIndex == null)
			{
				this.UpdateVariablesIndex();
			}
			sharedVariable.Name = name;
			int index;
			if (this.mSharedVariableIndex != null && this.mSharedVariableIndex.TryGetValue(name, out index))
			{
				SharedVariable sharedVariable2 = this.mVariables[index];
				if (!sharedVariable2.GetType().Equals(typeof(SharedVariable)) && !sharedVariable2.GetType().Equals(sharedVariable.GetType()))
				{
					Debug.LogError(string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", name, sharedVariable2.GetType(), sharedVariable.GetType()));
				}
				else if (!string.IsNullOrEmpty(sharedVariable.PropertyMapping))
				{
					sharedVariable2.PropertyMappingOwner = sharedVariable.PropertyMappingOwner;
					sharedVariable2.PropertyMapping = sharedVariable.PropertyMapping;
					sharedVariable2.InitializePropertyMapping(this);
				}
				else
				{
					sharedVariable2.SetValue(sharedVariable.GetValue());
				}
			}
			else
			{
				this.mVariables.Add(sharedVariable);
				this.UpdateVariablesIndex();
			}
		}

		/// <summary>
        /// 更新共享变量的名字
        /// </summary>
        /// <param name="sharedVariable"></param>
        /// <param name="name"></param>
		public void UpdateVariableName(SharedVariable sharedVariable, string name)
		{
			this.CheckForSerialization(false, null);
			sharedVariable.Name = name;
			this.UpdateVariablesIndex();
		}

		/// <summary>
        /// 更新所有的共享变量列表
        /// </summary>
        /// <param name="variables"></param>
		public void SetAllVariables(List<SharedVariable> variables)
		{
			this.mVariables = variables;
			this.UpdateVariablesIndex();
		}

        /// <summary>
        /// 通过 共享变量列表 更新 共享变量名索引表
        /// </summary>
        private void UpdateVariablesIndex()
		{
			if (this.mVariables == null)
			{
				if (this.mSharedVariableIndex != null)
				{
					this.mSharedVariableIndex = null;
				}
				return;
			}
			if (this.mSharedVariableIndex == null)
			{
				this.mSharedVariableIndex = new Dictionary<string, int>(this.mVariables.Count);
			}
			else
			{
				this.mSharedVariableIndex.Clear();
			}
			for (int i = 0; i < this.mVariables.Count; i++)
			{
				if (this.mVariables[i] != null)
				{
					this.mSharedVariableIndex.Add(this.mVariables[i].Name, i);
				}
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00007DDC File Offset: 0x00005FDC
		public override string ToString()
		{
			if (this.mOwner == null || this.mOwner.GetObject() == null)
			{
				return this.behaviorName;
			}
			return string.Format("{0} - {1}", this.Owner.GetOwnerName(), this.behaviorName);
		}

        /// <summary>
        /// 行为树名
        /// </summary>
        public string behaviorName = "Behavior";

		/// <summary>
        /// 行为树描述
        /// </summary>
		public string behaviorDescription = string.Empty;

		/// <summary>
        /// 定义ID
        /// </summary>
		private int behaviorID = -1;

		/// <summary>
        /// 行为树的入口节点
        /// </summary>
		private Task mEntryTask;

		/// <summary>
        /// 入口节点的子节点
        /// </summary>
		private Task mRootTask;

		/// <summary>
        /// 分离的节点
        /// </summary>
		private List<Task> mDetachedTasks;

		/// <summary>
        /// 共享的变量
        /// </summary>
		private List<SharedVariable> mVariables;

		/// <summary>
        /// 共享变量的名字与列表索引的对应
        /// </summary>
		private Dictionary<string, int> mSharedVariableIndex;

		/// <summary>
        /// 是否解析
        /// </summary>
		[NonSerialized]
		private bool mHasSerialized;

		/// <summary>
        /// 序列化的节点信息
        /// </summary>
		[SerializeField]
		private TaskSerializationData mTaskData;

		/// <summary>
        /// 所属者
        /// </summary>
		[SerializeField]
		private IBehavior mOwner;
	}
}
