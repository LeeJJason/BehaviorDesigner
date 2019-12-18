using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000019 RID: 25
	public class GlobalVariables : ScriptableObject, IVariableSource
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000B808 File Offset: 0x00009A08
		public static GlobalVariables Instance
		{
			get
			{
				if (GlobalVariables.instance == null)
				{
					GlobalVariables.instance = (Resources.Load("BehaviorDesignerGlobalVariables", typeof(GlobalVariables)) as GlobalVariables);
					if (GlobalVariables.instance != null)
					{
						GlobalVariables.instance.CheckForSerialization(false);
					}
				}
				return GlobalVariables.instance;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000139 RID: 313 RVA: 0x0000B864 File Offset: 0x00009A64
		// (set) Token: 0x0600013A RID: 314 RVA: 0x0000B86C File Offset: 0x00009A6C
		public List<SharedVariable> Variables
		{
			get
			{
				return this.mVariables;
			}
			set
			{
				this.mVariables = value;
				this.UpdateVariablesIndex();
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600013B RID: 315 RVA: 0x0000B87C File Offset: 0x00009A7C
		// (set) Token: 0x0600013C RID: 316 RVA: 0x0000B884 File Offset: 0x00009A84
		public VariableSerializationData VariableData
		{
			get
			{
				return this.mVariableData;
			}
			set
			{
				this.mVariableData = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600013D RID: 317 RVA: 0x0000B890 File Offset: 0x00009A90
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000B898 File Offset: 0x00009A98
		public string Version
		{
			get
			{
				return this.mVersion;
			}
			set
			{
				this.mVersion = value;
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		public void CheckForSerialization(bool force)
		{
			if (force || this.mVariables == null || (this.mVariables.Count > 0 && this.mVariables[0] == null))
			{
				if (this.VariableData != null && !string.IsNullOrEmpty(this.VariableData.JSONSerialization))
				{
					JSONDeserialization.Load(this.VariableData.JSONSerialization, this, this.mVersion);
				}
				else
				{
					BinaryDeserialization.Load(this, this.mVersion);
				}
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000B92C File Offset: 0x00009B2C
		public SharedVariable GetVariable(string name)
		{
			if (name == null)
			{
				return null;
			}
			this.CheckForSerialization(false);
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

		// Token: 0x06000141 RID: 321 RVA: 0x0000B9A0 File Offset: 0x00009BA0
		public List<SharedVariable> GetAllVariables()
		{
			this.CheckForSerialization(false);
			return this.mVariables;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		public void SetVariable(string name, SharedVariable sharedVariable)
		{
			this.CheckForSerialization(false);
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

		// Token: 0x06000143 RID: 323 RVA: 0x0000BA98 File Offset: 0x00009C98
		public void SetVariableValue(string name, object value)
		{
			SharedVariable variable = this.GetVariable(name);
			if (variable != null)
			{
				variable.SetValue(value);
				variable.ValueChanged();
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000BAC0 File Offset: 0x00009CC0
		public void UpdateVariableName(SharedVariable sharedVariable, string name)
		{
			this.CheckForSerialization(false);
			sharedVariable.Name = name;
			this.UpdateVariablesIndex();
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000BAD8 File Offset: 0x00009CD8
		public void SetAllVariables(List<SharedVariable> variables)
		{
			this.mVariables = variables;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000BAE4 File Offset: 0x00009CE4
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

		// Token: 0x04000084 RID: 132
		private static GlobalVariables instance;

		// Token: 0x04000085 RID: 133
		[SerializeField]
		private List<SharedVariable> mVariables;

		// Token: 0x04000086 RID: 134
		private Dictionary<string, int> mSharedVariableIndex;

		// Token: 0x04000087 RID: 135
		[SerializeField]
		private VariableSerializationData mVariableData;

		// Token: 0x04000088 RID: 136
		[SerializeField]
		private string mVersion;
	}
}
