using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000011 RID: 17
	[Serializable]
	public class BehaviorSource : IVariableSource
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x0000797C File Offset: 0x00005B7C
		public BehaviorSource()
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000079A4 File Offset: 0x00005BA4
		public BehaviorSource(IBehavior owner)
		{
			this.Initialize(owner);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x000079DC File Offset: 0x00005BDC
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x000079E4 File Offset: 0x00005BE4
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000079F0 File Offset: 0x00005BF0
		// (set) Token: 0x060000CB RID: 203 RVA: 0x000079F8 File Offset: 0x00005BF8
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00007A04 File Offset: 0x00005C04
		// (set) Token: 0x060000CD RID: 205 RVA: 0x00007A0C File Offset: 0x00005C0C
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00007A18 File Offset: 0x00005C18
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00007A20 File Offset: 0x00005C20
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00007A2C File Offset: 0x00005C2C
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00007A40 File Offset: 0x00005C40
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00007A50 File Offset: 0x00005C50
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00007A58 File Offset: 0x00005C58
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00007A64 File Offset: 0x00005C64
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00007A6C File Offset: 0x00005C6C
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00007A78 File Offset: 0x00005C78
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00007A80 File Offset: 0x00005C80
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

		// Token: 0x060000D8 RID: 216 RVA: 0x00007A8C File Offset: 0x00005C8C
		public void Initialize(IBehavior owner)
		{
			this.mOwner = owner;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007A98 File Offset: 0x00005C98
		public void Save(Task entryTask, Task rootTask, List<Task> detachedTasks)
		{
			this.mEntryTask = entryTask;
			this.mRootTask = rootTask;
			this.mDetachedTasks = detachedTasks;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007AB0 File Offset: 0x00005CB0
		public void Load(out Task entryTask, out Task rootTask, out List<Task> detachedTasks)
		{
			entryTask = this.mEntryTask;
			rootTask = this.mRootTask;
			detachedTasks = this.mDetachedTasks;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007ACC File Offset: 0x00005CCC
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

		// Token: 0x060000DC RID: 220 RVA: 0x00007B70 File Offset: 0x00005D70
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

		// Token: 0x060000DD RID: 221 RVA: 0x00007BE0 File Offset: 0x00005DE0
		public List<SharedVariable> GetAllVariables()
		{
			this.CheckForSerialization(false, null);
			return this.mVariables;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007BF4 File Offset: 0x00005DF4
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

		// Token: 0x060000DF RID: 223 RVA: 0x00007D08 File Offset: 0x00005F08
		public void UpdateVariableName(SharedVariable sharedVariable, string name)
		{
			this.CheckForSerialization(false, null);
			sharedVariable.Name = name;
			this.UpdateVariablesIndex();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00007D20 File Offset: 0x00005F20
		public void SetAllVariables(List<SharedVariable> variables)
		{
			this.mVariables = variables;
			this.UpdateVariablesIndex();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007D30 File Offset: 0x00005F30
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

		// Token: 0x0400006F RID: 111
		public string behaviorName = "Behavior";

		// Token: 0x04000070 RID: 112
		public string behaviorDescription = string.Empty;

		// Token: 0x04000071 RID: 113
		private int behaviorID = -1;

		// Token: 0x04000072 RID: 114
		private Task mEntryTask;

		// Token: 0x04000073 RID: 115
		private Task mRootTask;

		// Token: 0x04000074 RID: 116
		private List<Task> mDetachedTasks;

		// Token: 0x04000075 RID: 117
		private List<SharedVariable> mVariables;

		// Token: 0x04000076 RID: 118
		private Dictionary<string, int> mSharedVariableIndex;

		// Token: 0x04000077 RID: 119
		[NonSerialized]
		private bool mHasSerialized;

		// Token: 0x04000078 RID: 120
		[SerializeField]
		private TaskSerializationData mTaskData;

		// Token: 0x04000079 RID: 121
		[SerializeField]
		private IBehavior mOwner;
	}
}
