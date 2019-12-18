using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000008 RID: 8
	[AddComponentMenu("Behavior Designer/Behavior Manager")]
	public class BehaviorManager : MonoBehaviour
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000037C0 File Offset: 0x000019C0
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000037C8 File Offset: 0x000019C8
		public UpdateIntervalType UpdateInterval
		{
			get
			{
				return this.updateInterval;
			}
			set
			{
				this.updateInterval = value;
				this.UpdateIntervalChanged();
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000037D8 File Offset: 0x000019D8
		// (set) Token: 0x0600006F RID: 111 RVA: 0x000037E0 File Offset: 0x000019E0
		public float UpdateIntervalSeconds
		{
			get
			{
				return this.updateIntervalSeconds;
			}
			set
			{
				this.updateIntervalSeconds = value;
				this.UpdateIntervalChanged();
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000037F0 File Offset: 0x000019F0
		// (set) Token: 0x06000071 RID: 113 RVA: 0x000037F8 File Offset: 0x000019F8
		public BehaviorManager.ExecutionsPerTickType ExecutionsPerTick
		{
			get
			{
				return this.executionsPerTick;
			}
			set
			{
				this.executionsPerTick = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003804 File Offset: 0x00001A04
		// (set) Token: 0x06000073 RID: 115 RVA: 0x0000380C File Offset: 0x00001A0C
		public int MaxTaskExecutionsPerTick
		{
			get
			{
				return this.maxTaskExecutionsPerTick;
			}
			set
			{
				this.maxTaskExecutionsPerTick = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00003818 File Offset: 0x00001A18
		public BehaviorManager.BehaviorManagerHandler OnEnableBehavior
		{
			set
			{
				this.onEnableBehavior = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00003824 File Offset: 0x00001A24
		// (set) Token: 0x06000076 RID: 118 RVA: 0x0000382C File Offset: 0x00001A2C
		public BehaviorManager.BehaviorManagerHandler OnTaskBreakpoint
		{
			get
			{
				return this.onTaskBreakpoint;
			}
			set
			{
				this.onTaskBreakpoint = (BehaviorManager.BehaviorManagerHandler)Delegate.Combine(this.onTaskBreakpoint, value);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00003848 File Offset: 0x00001A48
		public List<BehaviorManager.BehaviorTree> BehaviorTrees
		{
			get
			{
				return this.behaviorTrees;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003850 File Offset: 0x00001A50
		private static MethodInfo PlayMakerStopMethod
		{
			get
			{
				if (BehaviorManager.playMakerStopMethod == null)
				{
					BehaviorManager.playMakerStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_PlayMaker").GetMethod("StopPlayMaker");
				}
				return BehaviorManager.playMakerStopMethod;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00003888 File Offset: 0x00001A88
		private static MethodInfo UScriptStopMethod
		{
			get
			{
				if (BehaviorManager.uScriptStopMethod == null)
				{
					BehaviorManager.uScriptStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_uScript").GetMethod("StopuScript");
				}
				return BehaviorManager.uScriptStopMethod;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000038C0 File Offset: 0x00001AC0
		private static MethodInfo DialogueSystemStopMethod
		{
			get
			{
				if (BehaviorManager.dialogueSystemStopMethod == null)
				{
					BehaviorManager.dialogueSystemStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_DialogueSystem").GetMethod("StopDialogueSystem");
				}
				return BehaviorManager.dialogueSystemStopMethod;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000038F8 File Offset: 0x00001AF8
		private static MethodInfo USequencerStopMethod
		{
			get
			{
				if (BehaviorManager.uSequencerStopMethod == null)
				{
					BehaviorManager.uSequencerStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_uSequencer").GetMethod("StopuSequencer");
				}
				return BehaviorManager.uSequencerStopMethod;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003930 File Offset: 0x00001B30
		private static MethodInfo ICodeStopMethod
		{
			get
			{
				if (BehaviorManager.iCodeStopMethod == null)
				{
					BehaviorManager.iCodeStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_ICode").GetMethod("StopICode");
				}
				return BehaviorManager.iCodeStopMethod;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003968 File Offset: 0x00001B68
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00003970 File Offset: 0x00001B70
		public Behavior BreakpointTree
		{
			get
			{
				return this.breakpointTree;
			}
			set
			{
				this.breakpointTree = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007F RID: 127 RVA: 0x0000397C File Offset: 0x00001B7C
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00003984 File Offset: 0x00001B84
		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003990 File Offset: 0x00001B90
		public void Awake()
		{
			BehaviorManager.instance = this;
			this.UpdateIntervalChanged();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000039A0 File Offset: 0x00001BA0
		private void UpdateIntervalChanged()
		{
			base.StopCoroutine("CoroutineUpdate");
			if (this.updateInterval == UpdateIntervalType.EveryFrame)
			{
				base.enabled = true;
			}
			else if (this.updateInterval == UpdateIntervalType.SpecifySeconds)
			{
				if (Application.isPlaying)
				{
					this.updateWait = new WaitForSeconds(this.updateIntervalSeconds);
					base.StartCoroutine("CoroutineUpdate");
				}
				base.enabled = false;
			}
			else
			{
				base.enabled = false;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003A18 File Offset: 0x00001C18
		public void OnDestroy()
		{
			for (int i = this.behaviorTrees.Count - 1; i > -1; i--)
			{
				this.DisableBehavior(this.behaviorTrees[i].behavior);
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003A5C File Offset: 0x00001C5C
		public void OnApplicationQuit()
		{
			for (int i = this.behaviorTrees.Count - 1; i > -1; i--)
			{
				this.DisableBehavior(this.behaviorTrees[i].behavior);
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public void EnableBehavior(Behavior behavior)
		{
			if (this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree;
			if (this.pausedBehaviorTrees.TryGetValue(behavior, out behaviorTree))
			{
				this.behaviorTrees.Add(behaviorTree);
				this.pausedBehaviorTrees.Remove(behavior);
				behavior.ExecutionStatus = TaskStatus.Running;
				for (int i = 0; i < behaviorTree.taskList.Count; i++)
				{
					behaviorTree.taskList[i].OnPause(false);
				}
				return;
			}
			BehaviorManager.TaskAddData taskAddData = ObjectPool.Get<BehaviorManager.TaskAddData>();
			taskAddData.Initialize();
			behavior.CheckForSerialization();
			Task rootTask = behavior.GetBehaviorSource().RootTask;
			if (rootTask == null)
			{
				Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains no root task. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
				return;
			}
			behaviorTree = ObjectPool.Get<BehaviorManager.BehaviorTree>();
			behaviorTree.Initialize(behavior);
			behaviorTree.parentIndex.Add(-1);
			behaviorTree.relativeChildIndex.Add(-1);
			behaviorTree.parentCompositeIndex.Add(-1);
			bool flag = behavior.ExternalBehavior != null;
			int num = this.AddToTaskList(behaviorTree, rootTask, ref flag, taskAddData);
			if (num < 0)
			{
				behaviorTree = null;
				int num2 = num;
				switch (num2 + 6)
				{
				case 0:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a root task which is disabled. This behavior will be disabled.", new object[]
					{
						behavior.GetBehaviorSource().behaviorName,
						behavior.gameObject.name,
						taskAddData.errorTaskName,
						taskAddData.errorTask
					}));
					break;
				case 1:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a Behavior Tree Reference task ({2} (index {3})) that which has an element with a null value in the externalBehaviors array. This behavior will be disabled.", new object[]
					{
						behavior.GetBehaviorSource().behaviorName,
						behavior.gameObject.name,
						taskAddData.errorTaskName,
						taskAddData.errorTask
					}));
					break;
				case 2:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains multiple external behavior trees at the root task or as a child of a parent task which cannot contain so many children (such as a decorator task). This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
					break;
				case 3:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a null task (referenced from parent task {2} (index {3})). This behavior will be disabled.", new object[]
					{
						behavior.GetBehaviorSource().behaviorName,
						behavior.gameObject.name,
						taskAddData.errorTaskName,
						taskAddData.errorTask
					}));
					break;
				case 4:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" cannot find the referenced external task. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
					break;
				case 5:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a parent task ({2} (index {3})) with no children. This behavior will be disabled.", new object[]
					{
						behavior.GetBehaviorSource().behaviorName,
						behavior.gameObject.name,
						taskAddData.errorTaskName,
						taskAddData.errorTask
					}));
					break;
				}
				return;
			}
			this.dirty = true;
			if (behavior.ExternalBehavior != null)
			{
				behavior.GetBehaviorSource().EntryTask = behavior.ExternalBehavior.BehaviorSource.EntryTask;
			}
			behavior.GetBehaviorSource().RootTask = behaviorTree.taskList[0];
			if (behavior.ResetValuesOnRestart)
			{
				behavior.SaveResetValues();
			}
			Stack<int> stack = ObjectPool.Get<Stack<int>>();
			stack.Clear();
			behaviorTree.activeStack.Add(stack);
			behaviorTree.interruptionIndex.Add(-1);
			behaviorTree.nonInstantTaskStatus.Add(TaskStatus.Inactive);
			if (behaviorTree.behavior.LogTaskChanges)
			{
				for (int j = 0; j < behaviorTree.taskList.Count; j++)
				{
					Debug.Log(string.Format("{0}: Task {1} ({2}, index {3}) {4}", new object[]
					{
						this.RoundedTime(),
						behaviorTree.taskList[j].FriendlyName,
						behaviorTree.taskList[j].GetType(),
						j,
						behaviorTree.taskList[j].GetHashCode()
					}));
				}
			}
			for (int k = 0; k < behaviorTree.taskList.Count; k++)
			{
				behaviorTree.taskList[k].OnAwake();
			}
			this.behaviorTrees.Add(behaviorTree);
			this.behaviorTreeMap.Add(behavior, behaviorTree);
			if (this.onEnableBehavior != null)
			{
				this.onEnableBehavior();
			}
			if (!behaviorTree.taskList[0].Disabled)
			{
				behaviorTree.behavior.OnBehaviorStarted();
				behavior.ExecutionStatus = TaskStatus.Running;
				this.PushTask(behaviorTree, 0, 0);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003F30 File Offset: 0x00002130
		private int AddToTaskList(BehaviorManager.BehaviorTree behaviorTree, Task task, ref bool hasExternalBehavior, BehaviorManager.TaskAddData data)
		{
			if (task == null)
			{
				return -3;
			}
			task.GameObject = behaviorTree.behavior.gameObject;
			task.Transform = behaviorTree.behavior.transform;
			task.Owner = behaviorTree.behavior;
			if (task is BehaviorReference)
			{
				BehaviorReference behaviorReference = task as BehaviorReference;
				if (behaviorReference == null)
				{
					return -2;
				}
				ExternalBehavior[] externalBehaviors;
				if ((externalBehaviors = behaviorReference.GetExternalBehaviors()) == null)
				{
					return -2;
				}
				BehaviorSource[] array = new BehaviorSource[externalBehaviors.Length];
				for (int i = 0; i < externalBehaviors.Length; i++)
				{
					if (externalBehaviors[i] == null)
					{
						data.errorTask = behaviorTree.taskList.Count;
						data.errorTaskName = (string.IsNullOrEmpty(task.FriendlyName) ? task.GetType().ToString() : task.FriendlyName);
						return -5;
					}
					array[i] = externalBehaviors[i].BehaviorSource;
					array[i].Owner = externalBehaviors[i];
				}
				if (array == null)
				{
					return -2;
				}
				ParentTask parentTask = data.parentTask;
				int parentIndex = data.parentIndex;
				int compositeParentIndex = data.compositeParentIndex;
				data.offset = task.NodeData.Offset;
				data.depth++;
				for (int j = 0; j < array.Length; j++)
				{
					BehaviorSource behaviorSource = ObjectPool.Get<BehaviorSource>();
					behaviorSource.Initialize(array[j].Owner);
					array[j].CheckForSerialization(true, behaviorSource);
					Task rootTask = behaviorSource.RootTask;
					if (rootTask == null)
					{
						ObjectPool.Return<BehaviorSource>(behaviorSource);
						return -2;
					}
					if (rootTask is ParentTask)
					{
						rootTask.NodeData.Collapsed = (task as BehaviorReference).collapsed;
					}
					rootTask.Disabled = task.Disabled;
					if (behaviorReference.variables != null)
					{
						for (int k = 0; k < behaviorReference.variables.Length; k++)
						{
							if (data.overrideFields == null)
							{
								data.overrideFields = ObjectPool.Get<Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue>>();
								data.overrideFields.Clear();
							}
							if (!data.overrideFields.ContainsKey(behaviorReference.variables[k].Value.name))
							{
								BehaviorManager.TaskAddData.OverrideFieldValue overrideFieldValue = ObjectPool.Get<BehaviorManager.TaskAddData.OverrideFieldValue>();
								overrideFieldValue.Initialize(behaviorReference.variables[k].Value, data.depth);
								data.overrideFields.Add(behaviorReference.variables[k].Value.name, overrideFieldValue);
							}
						}
					}
					if (behaviorSource.Variables != null)
					{
						for (int l = 0; l < behaviorSource.Variables.Count; l++)
						{
							SharedVariable sharedVariable;
							if ((sharedVariable = behaviorTree.behavior.GetVariable(behaviorSource.Variables[l].Name)) == null)
							{
								sharedVariable = behaviorSource.Variables[l];
								behaviorTree.behavior.SetVariable(sharedVariable.Name, sharedVariable);
							}
							else
							{
								behaviorSource.Variables[l].SetValue(sharedVariable.GetValue());
							}
							if (data.overrideFields == null)
							{
								data.overrideFields = ObjectPool.Get<Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue>>();
								data.overrideFields.Clear();
							}
							if (!data.overrideFields.ContainsKey(sharedVariable.Name))
							{
								BehaviorManager.TaskAddData.OverrideFieldValue overrideFieldValue2 = ObjectPool.Get<BehaviorManager.TaskAddData.OverrideFieldValue>();
								overrideFieldValue2.Initialize(sharedVariable, data.depth);
								data.overrideFields.Add(sharedVariable.Name, overrideFieldValue2);
							}
						}
					}
					ObjectPool.Return<BehaviorSource>(behaviorSource);
					if (j > 0)
					{
						data.parentTask = parentTask;
						data.parentIndex = parentIndex;
						data.compositeParentIndex = compositeParentIndex;
						if (data.parentTask == null || j >= data.parentTask.MaxChildren())
						{
							return -4;
						}
						behaviorTree.parentIndex.Add(data.parentIndex);
						behaviorTree.relativeChildIndex.Add(data.parentTask.Children.Count);
						behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
						behaviorTree.childrenIndex[data.parentIndex].Add(behaviorTree.taskList.Count);
						data.parentTask.AddChild(rootTask, data.parentTask.Children.Count);
					}
					hasExternalBehavior = true;
					bool fromExternalTask = data.fromExternalTask;
					data.fromExternalTask = true;
					int result;
					if ((result = this.AddToTaskList(behaviorTree, rootTask, ref hasExternalBehavior, data)) < 0)
					{
						return result;
					}
					data.fromExternalTask = fromExternalTask;
				}
				if (data.overrideFields != null)
				{
					Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue> dictionary = ObjectPool.Get<Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue>>();
					dictionary.Clear();
					foreach (KeyValuePair<string, BehaviorManager.TaskAddData.OverrideFieldValue> keyValuePair in data.overrideFields)
					{
						if (keyValuePair.Value.Depth != data.depth)
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					ObjectPool.Return<Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue>>(data.overrideFields);
					data.overrideFields = dictionary;
				}
				data.depth--;
			}
			else
			{
				if (behaviorTree.taskList.Count == 0 && task.Disabled)
				{
					return -6;
				}
				task.ReferenceID = behaviorTree.taskList.Count;
				behaviorTree.taskList.Add(task);
				if (data.overrideFields != null)
				{
					this.OverrideFields(behaviorTree, data, task);
				}
				if (data.fromExternalTask)
				{
					if (data.parentTask == null)
					{
						task.NodeData.Offset = behaviorTree.behavior.GetBehaviorSource().RootTask.NodeData.Offset;
					}
					else
					{
						int index = behaviorTree.relativeChildIndex[behaviorTree.relativeChildIndex.Count - 1];
						data.parentTask.ReplaceAddChild(task, index);
						if (data.offset != Vector2.zero)
						{
							task.NodeData.Offset = data.offset;
							data.offset = Vector2.zero;
						}
					}
				}
				if (task is ParentTask)
				{
					ParentTask parentTask2 = task as ParentTask;
					if (parentTask2.Children == null || parentTask2.Children.Count == 0)
					{
						data.errorTask = behaviorTree.taskList.Count - 1;
						data.errorTaskName = (string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].GetType().ToString() : behaviorTree.taskList[data.errorTask].FriendlyName);
						return -1;
					}
					int num = behaviorTree.taskList.Count - 1;
					List<int> list = ObjectPool.Get<List<int>>();
					list.Clear();
					behaviorTree.childrenIndex.Add(list);
					list = ObjectPool.Get<List<int>>();
					list.Clear();
					behaviorTree.childConditionalIndex.Add(list);
					int count = parentTask2.Children.Count;
					for (int m = 0; m < count; m++)
					{
						behaviorTree.parentIndex.Add(num);
						behaviorTree.relativeChildIndex.Add(m);
						behaviorTree.childrenIndex[num].Add(behaviorTree.taskList.Count);
						data.parentTask = (task as ParentTask);
						data.parentIndex = num;
						if (task is Composite)
						{
							data.compositeParentIndex = num;
						}
						behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
						int num2;
						if ((num2 = this.AddToTaskList(behaviorTree, parentTask2.Children[m], ref hasExternalBehavior, data)) < 0)
						{
							if (num2 == -3)
							{
								data.errorTask = num;
								data.errorTaskName = (string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].GetType().ToString() : behaviorTree.taskList[data.errorTask].FriendlyName);
							}
							return num2;
						}
					}
				}
				else
				{
					behaviorTree.childrenIndex.Add(null);
					behaviorTree.childConditionalIndex.Add(null);
					if (task is Conditional)
					{
						int num3 = behaviorTree.taskList.Count - 1;
						int num4 = behaviorTree.parentCompositeIndex[num3];
						if (num4 != -1)
						{
							behaviorTree.childConditionalIndex[num4].Add(num3);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000047F4 File Offset: 0x000029F4
		private void OverrideFields(BehaviorManager.BehaviorTree behaviorTree, BehaviorManager.TaskAddData data, object obj)
		{
			if (obj == null || object.Equals(obj, null))
			{
				return;
			}
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				object value = allFields[i].GetValue(obj);
				if (value != null)
				{
					Type fieldType;
					if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType))
					{
						SharedVariable sharedVariable = this.OverrideSharedVariable(behaviorTree, data, allFields[i].FieldType, value as SharedVariable);
						if (sharedVariable != null)
						{
							allFields[i].SetValue(obj, sharedVariable);
						}
					}
					else if (typeof(IList).IsAssignableFrom(allFields[i].FieldType) && (typeof(SharedVariable).IsAssignableFrom(fieldType = allFields[i].FieldType.GetElementType()) || (allFields[i].FieldType.IsGenericType && typeof(SharedVariable).IsAssignableFrom(fieldType = allFields[i].FieldType.GetGenericArguments()[0]))))
					{
						IList<SharedVariable> list = value as IList<SharedVariable>;
						if (list != null)
						{
							for (int j = 0; j < list.Count; j++)
							{
								SharedVariable sharedVariable2 = this.OverrideSharedVariable(behaviorTree, data, fieldType, list[j]);
								if (sharedVariable2 != null)
								{
									list[j] = sharedVariable2;
								}
							}
						}
					}
					if (allFields[i].FieldType.IsClass && !allFields[i].FieldType.Equals(typeof(Type)) && !typeof(Delegate).IsAssignableFrom(allFields[i].FieldType) && !data.overiddenFields.Contains(value))
					{
						data.overiddenFields.Add(value);
						this.OverrideFields(behaviorTree, data, value);
						data.overiddenFields.Remove(value);
					}
				}
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000049DC File Offset: 0x00002BDC
		private SharedVariable OverrideSharedVariable(BehaviorManager.BehaviorTree behaviorTree, BehaviorManager.TaskAddData data, Type fieldType, SharedVariable sharedVariable)
		{
			SharedVariable sharedVariable2 = sharedVariable;
			if (sharedVariable is SharedGenericVariable)
			{
				sharedVariable = ((sharedVariable as SharedGenericVariable).GetValue() as GenericVariable).value;
			}
			else if (sharedVariable is SharedNamedVariable)
			{
				sharedVariable = ((sharedVariable as SharedNamedVariable).GetValue() as NamedVariable).value;
			}
			if (sharedVariable == null)
			{
				return null;
			}
			BehaviorManager.TaskAddData.OverrideFieldValue overrideFieldValue;
			if (!string.IsNullOrEmpty(sharedVariable.Name) && data.overrideFields.TryGetValue(sharedVariable.Name, out overrideFieldValue))
			{
				SharedVariable sharedVariable3 = null;
				if (overrideFieldValue.Value is SharedVariable)
				{
					sharedVariable3 = (overrideFieldValue.Value as SharedVariable);
				}
				else if (overrideFieldValue.Value is NamedVariable)
				{
					sharedVariable3 = (overrideFieldValue.Value as NamedVariable).value;
					if (sharedVariable3.IsShared)
					{
						sharedVariable3 = behaviorTree.behavior.GetVariable(sharedVariable3.Name);
					}
				}
				else if (overrideFieldValue.Value is GenericVariable)
				{
					sharedVariable3 = (overrideFieldValue.Value as GenericVariable).value;
					if (sharedVariable3.IsShared)
					{
						sharedVariable3 = behaviorTree.behavior.GetVariable(sharedVariable3.Name);
					}
				}
				if (sharedVariable2 is SharedNamedVariable || sharedVariable2 is SharedGenericVariable)
				{
					if (fieldType.Equals(typeof(SharedVariable)) || sharedVariable3.GetType().Equals(sharedVariable.GetType()))
					{
						if (sharedVariable2 is SharedNamedVariable)
						{
							(sharedVariable2 as SharedNamedVariable).Value.value = sharedVariable3;
						}
						else if (sharedVariable2 is SharedGenericVariable)
						{
							(sharedVariable2 as SharedGenericVariable).Value.value = sharedVariable3;
						}
					}
				}
				else if (sharedVariable3 is SharedVariable)
				{
					return sharedVariable3;
				}
			}
			return null;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004BA4 File Offset: 0x00002DA4
		public void DisableBehavior(Behavior behavior)
		{
			this.DisableBehavior(behavior, false);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004BB0 File Offset: 0x00002DB0
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

		// Token: 0x0600008B RID: 139 RVA: 0x00004CB4 File Offset: 0x00002EB4
		public void DestroyBehavior(Behavior behavior)
		{
			BehaviorManager.BehaviorTree behaviorTree;
			if (!this.behaviorTreeMap.TryGetValue(behavior, out behaviorTree))
			{
				return;
			}
			if (this.pausedBehaviorTrees.ContainsKey(behavior))
			{
				this.pausedBehaviorTrees.Remove(behavior);
				for (int i = 0; i < behaviorTree.taskList.Count; i++)
				{
					behaviorTree.taskList[i].OnPause(false);
				}
				behavior.ExecutionStatus = TaskStatus.Running;
			}
			TaskStatus executionStatus = TaskStatus.Success;
			for (int j = behaviorTree.activeStack.Count - 1; j > -1; j--)
			{
				while (behaviorTree.activeStack[j].Count > 0)
				{
					int count = behaviorTree.activeStack[j].Count;
					this.PopTask(behaviorTree, behaviorTree.activeStack[j].Peek(), j, ref executionStatus, true, false);
					if (count == 1)
					{
						break;
					}
				}
			}
			this.RemoveChildConditionalReevaluate(behaviorTree, -1);
			for (int k = 0; k < behaviorTree.taskList.Count; k++)
			{
				behaviorTree.taskList[k].OnBehaviorComplete();
			}
			this.behaviorTreeMap.Remove(behavior);
			this.behaviorTrees.Remove(behaviorTree);
			ObjectPool.Return<BehaviorManager.BehaviorTree>(behaviorTree);
			behavior.ExecutionStatus = executionStatus;
			behavior.OnBehaviorEnded();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004E08 File Offset: 0x00003008
		public void RestartBehavior(Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			TaskStatus taskStatus = TaskStatus.Success;
			for (int i = behaviorTree.activeStack.Count - 1; i > -1; i--)
			{
				while (behaviorTree.activeStack[i].Count > 0)
				{
					int count = behaviorTree.activeStack[i].Count;
					this.PopTask(behaviorTree, behaviorTree.activeStack[i].Peek(), i, ref taskStatus, true, false);
					if (count == 1)
					{
						break;
					}
				}
			}
			this.Restart(behaviorTree);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004EB0 File Offset: 0x000030B0
		public bool IsBehaviorEnabled(Behavior behavior)
		{
			return this.behaviorTreeMap != null && this.behaviorTreeMap.Count > 0 && behavior != null && behavior.ExecutionStatus == TaskStatus.Running;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004EF4 File Offset: 0x000030F4
		public void Update()
		{
			this.Tick();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004EFC File Offset: 0x000030FC
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

		// Token: 0x06000090 RID: 144 RVA: 0x00004FA8 File Offset: 0x000031A8
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

		// Token: 0x06000091 RID: 145 RVA: 0x00005054 File Offset: 0x00003254
		private IEnumerator CoroutineUpdate()
		{
			for (;;)
			{
				this.Tick();
				yield return this.updateWait;
			}
			yield break;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005070 File Offset: 0x00003270
		public void Tick()
		{
			for (int i = 0; i < this.behaviorTrees.Count; i++)
			{
				this.Tick(this.behaviorTrees[i]);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000050AC File Offset: 0x000032AC
		public void Tick(Behavior behavior)
		{
			if (behavior == null || !this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			this.Tick(this.behaviorTreeMap[behavior]);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000050DC File Offset: 0x000032DC
		private void Tick(BehaviorManager.BehaviorTree behaviorTree)
		{
			behaviorTree.executionCount = 0;
			this.ReevaluateParentTasks(behaviorTree);
			this.ReevaluateConditionalTasks(behaviorTree);
			for (int i = behaviorTree.activeStack.Count - 1; i > -1; i--)
			{
				TaskStatus taskStatus = TaskStatus.Inactive;
				int num;
				if (i < behaviorTree.interruptionIndex.Count && (num = behaviorTree.interruptionIndex[i]) != -1)
				{
					behaviorTree.interruptionIndex[i] = -1;
					while (behaviorTree.activeStack[i].Peek() != num)
					{
						int count = behaviorTree.activeStack[i].Count;
						this.PopTask(behaviorTree, behaviorTree.activeStack[i].Peek(), i, ref taskStatus, true);
						if (count == 1)
						{
							break;
						}
					}
					if (i < behaviorTree.activeStack.Count && behaviorTree.activeStack[i].Count > 0 && behaviorTree.taskList[num] == behaviorTree.taskList[behaviorTree.activeStack[i].Peek()])
					{
						if (behaviorTree.taskList[num] is ParentTask)
						{
							taskStatus = (behaviorTree.taskList[num] as ParentTask).OverrideStatus();
						}
						this.PopTask(behaviorTree, num, i, ref taskStatus, true);
					}
				}
				int num2 = -1;
				while (taskStatus != TaskStatus.Running && i < behaviorTree.activeStack.Count && behaviorTree.activeStack[i].Count > 0)
				{
					int num3 = behaviorTree.activeStack[i].Peek();
					if ((i < behaviorTree.activeStack.Count && behaviorTree.activeStack[i].Count > 0 && num2 == behaviorTree.activeStack[i].Peek()) || !this.IsBehaviorEnabled(behaviorTree.behavior))
					{
						break;
					}
					num2 = num3;
					taskStatus = this.RunTask(behaviorTree, num3, i, taskStatus);
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000052E4 File Offset: 0x000034E4
		private void ReevaluateConditionalTasks(BehaviorManager.BehaviorTree behaviorTree)
		{
			for (int i = behaviorTree.conditionalReevaluate.Count - 1; i > -1; i--)
			{
				if (behaviorTree.conditionalReevaluate[i].compositeIndex != -1)
				{
					int index = behaviorTree.conditionalReevaluate[i].index;
					TaskStatus taskStatus = behaviorTree.taskList[index].OnUpdate();
					if (taskStatus != behaviorTree.conditionalReevaluate[i].taskStatus)
					{
						if (behaviorTree.behavior.LogTaskChanges)
						{
							int num = behaviorTree.parentCompositeIndex[index];
							MonoBehaviour.print(string.Format("{0}: {1}: Conditional abort with task {2} ({3}, index {4}) because of conditional task {5} ({6}, index {7}) with status {8}", new object[]
							{
								this.RoundedTime(),
								behaviorTree.behavior.ToString(),
								behaviorTree.taskList[num].FriendlyName,
								behaviorTree.taskList[num].GetType(),
								num,
								behaviorTree.taskList[index].FriendlyName,
								behaviorTree.taskList[index].GetType(),
								index,
								taskStatus
							}));
						}
						int compositeIndex = behaviorTree.conditionalReevaluate[i].compositeIndex;
						for (int j = behaviorTree.activeStack.Count - 1; j > -1; j--)
						{
							if (behaviorTree.activeStack[j].Count > 0)
							{
								int num2 = behaviorTree.activeStack[j].Peek();
								int num3 = this.FindLCA(behaviorTree, index, num2);
								if (this.IsChild(behaviorTree, num3, compositeIndex))
								{
									int count = behaviorTree.activeStack.Count;
									while (num2 != -1 && num2 != num3 && behaviorTree.activeStack.Count == count)
									{
										TaskStatus taskStatus2 = TaskStatus.Failure;
										this.PopTask(behaviorTree, num2, j, ref taskStatus2, false);
										num2 = behaviorTree.parentIndex[num2];
									}
								}
							}
						}
						for (int k = behaviorTree.conditionalReevaluate.Count - 1; k > i - 1; k--)
						{
							BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate = behaviorTree.conditionalReevaluate[k];
							if (this.FindLCA(behaviorTree, compositeIndex, conditionalReevaluate.index) == compositeIndex)
							{
								behaviorTree.taskList[behaviorTree.conditionalReevaluate[k].index].NodeData.IsReevaluating = false;
								ObjectPool.Return<BehaviorManager.BehaviorTree.ConditionalReevaluate>(behaviorTree.conditionalReevaluate[k]);
								behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[k].index);
								behaviorTree.conditionalReevaluate.RemoveAt(k);
							}
						}
						Composite composite = behaviorTree.taskList[behaviorTree.parentCompositeIndex[index]] as Composite;
						for (int l = i - 1; l > -1; l--)
						{
							BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate2 = behaviorTree.conditionalReevaluate[l];
							if (composite.AbortType == AbortType.LowerPriority && behaviorTree.parentCompositeIndex[conditionalReevaluate2.index] == behaviorTree.parentCompositeIndex[index])
							{
								behaviorTree.taskList[behaviorTree.conditionalReevaluate[l].index].NodeData.IsReevaluating = false;
								behaviorTree.conditionalReevaluate[l].compositeIndex = -1;
							}
							else if (behaviorTree.parentCompositeIndex[conditionalReevaluate2.index] == behaviorTree.parentCompositeIndex[index])
							{
								for (int m = 0; m < behaviorTree.childrenIndex[compositeIndex].Count; m++)
								{
									if (this.IsParentTask(behaviorTree, behaviorTree.childrenIndex[compositeIndex][m], conditionalReevaluate2.index))
									{
										int num4 = behaviorTree.childrenIndex[compositeIndex][m];
										while (!(behaviorTree.taskList[num4] is Composite))
										{
											if (behaviorTree.childrenIndex[num4] == null)
											{
												break;
											}
											num4 = behaviorTree.childrenIndex[num4][0];
										}
										if (behaviorTree.taskList[num4] is Composite)
										{
											conditionalReevaluate2.compositeIndex = num4;
										}
										break;
									}
								}
							}
						}
						this.conditionalParentIndexes.Clear();
						for (int num5 = behaviorTree.parentIndex[index]; num5 != compositeIndex; num5 = behaviorTree.parentIndex[num5])
						{
							this.conditionalParentIndexes.Add(num5);
						}
						if (this.conditionalParentIndexes.Count == 0)
						{
							this.conditionalParentIndexes.Add(behaviorTree.parentIndex[index]);
						}
						ParentTask parentTask = behaviorTree.taskList[compositeIndex] as ParentTask;
						parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[this.conditionalParentIndexes[this.conditionalParentIndexes.Count - 1]]);
						for (int n = this.conditionalParentIndexes.Count - 1; n > -1; n--)
						{
							parentTask = (behaviorTree.taskList[this.conditionalParentIndexes[n]] as ParentTask);
							if (n == 0)
							{
								parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[index]);
							}
							else
							{
								parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[this.conditionalParentIndexes[n - 1]]);
							}
						}
						behaviorTree.taskList[index].NodeData.InterruptTime = Time.realtimeSinceStartup;
					}
				}
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005898 File Offset: 0x00003A98
		private void ReevaluateParentTasks(BehaviorManager.BehaviorTree behaviorTree)
		{
			for (int i = behaviorTree.parentReevaluate.Count - 1; i > -1; i--)
			{
				int num = behaviorTree.parentReevaluate[i];
				if (behaviorTree.taskList[num] is Decorator)
				{
					if (behaviorTree.taskList[num].OnUpdate() == TaskStatus.Failure)
					{
						this.Interrupt(behaviorTree.behavior, behaviorTree.taskList[num]);
					}
				}
				else if (behaviorTree.taskList[num] is Composite)
				{
					ParentTask parentTask = behaviorTree.taskList[num] as ParentTask;
					if (parentTask.OnReevaluationStarted())
					{
						int num2 = 0;
						TaskStatus status = this.RunParentTask(behaviorTree, num, ref num2, TaskStatus.Inactive);
						parentTask.OnReevaluationEnded(status);
					}
				}
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005964 File Offset: 0x00003B64
		private TaskStatus RunTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex, int stackIndex, TaskStatus previousStatus)
		{
			Task task = behaviorTree.taskList[taskIndex];
			if (task == null)
			{
				return previousStatus;
			}
			if (task.Disabled)
			{
				if (behaviorTree.behavior.LogTaskChanges)
				{
					MonoBehaviour.print(string.Format("{0}: {1}: Skip task {2} ({3}, index {4}) at stack index {5} (task disabled)", new object[]
					{
						this.RoundedTime(),
						behaviorTree.behavior.ToString(),
						behaviorTree.taskList[taskIndex].FriendlyName,
						behaviorTree.taskList[taskIndex].GetType(),
						taskIndex,
						stackIndex
					}));
				}
				if (behaviorTree.parentIndex[taskIndex] != -1)
				{
					ParentTask parentTask = behaviorTree.taskList[behaviorTree.parentIndex[taskIndex]] as ParentTask;
					if (!parentTask.CanRunParallelChildren())
					{
						parentTask.OnChildExecuted(TaskStatus.Inactive);
					}
					else
					{
						parentTask.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], TaskStatus.Inactive);
						this.RemoveStack(behaviorTree, stackIndex);
					}
				}
				return previousStatus;
			}
			TaskStatus taskStatus = previousStatus;
			if (!task.IsInstant && (behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Failure || behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Success))
			{
				taskStatus = behaviorTree.nonInstantTaskStatus[stackIndex];
				this.PopTask(behaviorTree, taskIndex, stackIndex, ref taskStatus, true);
				return taskStatus;
			}
			this.PushTask(behaviorTree, taskIndex, stackIndex);
			if (this.breakpointTree != null)
			{
				return TaskStatus.Running;
			}
			if (task is ParentTask)
			{
				ParentTask parentTask2 = task as ParentTask;
				taskStatus = this.RunParentTask(behaviorTree, taskIndex, ref stackIndex, taskStatus);
				taskStatus = parentTask2.OverrideStatus(taskStatus);
			}
			else
			{
				taskStatus = task.OnUpdate();
			}
			if (taskStatus != TaskStatus.Running)
			{
				if (task.IsInstant)
				{
					this.PopTask(behaviorTree, taskIndex, stackIndex, ref taskStatus, true);
				}
				else
				{
					behaviorTree.nonInstantTaskStatus[stackIndex] = taskStatus;
				}
			}
			return taskStatus;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005B40 File Offset: 0x00003D40
		private TaskStatus RunParentTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex, ref int stackIndex, TaskStatus status)
		{
			ParentTask parentTask = behaviorTree.taskList[taskIndex] as ParentTask;
			if (!parentTask.CanRunParallelChildren() || parentTask.OverrideStatus(TaskStatus.Running) != TaskStatus.Running)
			{
				TaskStatus taskStatus = TaskStatus.Inactive;
				int num = stackIndex;
				int num2 = -1;
				while (parentTask.CanExecute() && (taskStatus != TaskStatus.Running || parentTask.CanRunParallelChildren()) && this.IsBehaviorEnabled(behaviorTree.behavior))
				{
					List<int> list = behaviorTree.childrenIndex[taskIndex];
					int num3 = parentTask.CurrentChildIndex();
					if ((this.executionsPerTick == BehaviorManager.ExecutionsPerTickType.NoDuplicates && num3 == num2) || (this.executionsPerTick == BehaviorManager.ExecutionsPerTickType.Count && behaviorTree.executionCount >= this.maxTaskExecutionsPerTick))
					{
						if (this.executionsPerTick == BehaviorManager.ExecutionsPerTickType.Count)
						{
							Debug.LogWarning(string.Format("{0}: {1}: More than the specified number of task executions per tick ({2}) have executed, returning early.", this.RoundedTime(), behaviorTree.behavior.ToString(), this.maxTaskExecutionsPerTick));
						}
						status = TaskStatus.Running;
						break;
					}
					num2 = num3;
					if (parentTask.CanRunParallelChildren())
					{
						behaviorTree.activeStack.Add(ObjectPool.Get<Stack<int>>());
						behaviorTree.interruptionIndex.Add(-1);
						behaviorTree.nonInstantTaskStatus.Add(TaskStatus.Inactive);
						stackIndex = behaviorTree.activeStack.Count - 1;
						parentTask.OnChildStarted(num3);
					}
					else
					{
						parentTask.OnChildStarted();
					}
					taskStatus = (status = this.RunTask(behaviorTree, list[num3], stackIndex, status));
				}
				stackIndex = num;
			}
			return status;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005CB0 File Offset: 0x00003EB0
		private void PushTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex, int stackIndex)
		{
			if (!this.IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count)
			{
				return;
			}
			Stack<int> stack = behaviorTree.activeStack[stackIndex];
			if (stack.Count == 0 || stack.Peek() != taskIndex)
			{
				stack.Push(taskIndex);
				behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Running;
				behaviorTree.executionCount++;
				Task task = behaviorTree.taskList[taskIndex];
				task.NodeData.PushTime = Time.realtimeSinceStartup;
				task.NodeData.ExecutionStatus = TaskStatus.Running;
				if (task.NodeData.IsBreakpoint)
				{
					this.breakpointTree = behaviorTree.behavior;
					if (this.onTaskBreakpoint != null)
					{
						this.onTaskBreakpoint();
					}
				}
				if (behaviorTree.behavior.LogTaskChanges)
				{
					MonoBehaviour.print(string.Format("{0}: {1}: Push task {2} ({3}, index {4}) at stack index {5}", new object[]
					{
						this.RoundedTime(),
						behaviorTree.behavior.ToString(),
						task.FriendlyName,
						task.GetType(),
						taskIndex,
						stackIndex
					}));
				}
				task.OnStart();
				if (task is ParentTask)
				{
					ParentTask parentTask = task as ParentTask;
					if (parentTask.CanReevaluate())
					{
						behaviorTree.parentReevaluate.Add(taskIndex);
					}
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005E18 File Offset: 0x00004018
		private void PopTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex, int stackIndex, ref TaskStatus status, bool popChildren)
		{
			this.PopTask(behaviorTree, taskIndex, stackIndex, ref status, popChildren, true);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005E28 File Offset: 0x00004028
		private void PopTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex, int stackIndex, ref TaskStatus status, bool popChildren, bool notifyOnEmptyStack)
		{
			if (!this.IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count || behaviorTree.activeStack[stackIndex].Count == 0 || taskIndex != behaviorTree.activeStack[stackIndex].Peek())
			{
				return;
			}
			behaviorTree.activeStack[stackIndex].Pop();
			behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Inactive;
			this.StopThirdPartyTask(behaviorTree, taskIndex);
			Task task = behaviorTree.taskList[taskIndex];
			task.OnEnd();
			int num = behaviorTree.parentIndex[taskIndex];
			task.NodeData.PushTime = -1f;
			task.NodeData.PopTime = Time.realtimeSinceStartup;
			task.NodeData.ExecutionStatus = status;
			if (behaviorTree.behavior.LogTaskChanges)
			{
				MonoBehaviour.print(string.Format("{0}: {1}: Pop task {2} ({3}, index {4}) at stack index {5} with status {6}", new object[]
				{
					this.RoundedTime(),
					behaviorTree.behavior.ToString(),
					task.FriendlyName,
					task.GetType(),
					taskIndex,
					stackIndex,
					status
				}));
			}
			if (num != -1)
			{
				if (task is Conditional)
				{
					int num2 = behaviorTree.parentCompositeIndex[taskIndex];
					if (num2 != -1)
					{
						Composite composite = behaviorTree.taskList[num2] as Composite;
						if (composite.AbortType != AbortType.None)
						{
							BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate;
							if (behaviorTree.conditionalReevaluateMap.TryGetValue(taskIndex, out conditionalReevaluate))
							{
								conditionalReevaluate.compositeIndex = ((composite.AbortType == AbortType.LowerPriority) ? -1 : num2);
								conditionalReevaluate.taskStatus = status;
								task.NodeData.IsReevaluating = (composite.AbortType != AbortType.LowerPriority);
							}
							else
							{
								BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate2 = ObjectPool.Get<BehaviorManager.BehaviorTree.ConditionalReevaluate>();
								conditionalReevaluate2.Initialize(taskIndex, status, stackIndex, (composite.AbortType == AbortType.LowerPriority) ? -1 : num2);
								behaviorTree.conditionalReevaluate.Add(conditionalReevaluate2);
								behaviorTree.conditionalReevaluateMap.Add(taskIndex, conditionalReevaluate2);
								task.NodeData.IsReevaluating = (composite.AbortType == AbortType.Self || composite.AbortType == AbortType.Both);
							}
						}
					}
				}
				ParentTask parentTask = behaviorTree.taskList[num] as ParentTask;
				if (!parentTask.CanRunParallelChildren())
				{
					parentTask.OnChildExecuted(status);
					status = parentTask.Decorate(status);
				}
				else
				{
					parentTask.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], status);
				}
			}
			if (task is ParentTask)
			{
				ParentTask parentTask2 = task as ParentTask;
				if (parentTask2.CanReevaluate())
				{
					for (int i = behaviorTree.parentReevaluate.Count - 1; i > -1; i--)
					{
						if (behaviorTree.parentReevaluate[i] == taskIndex)
						{
							behaviorTree.parentReevaluate.RemoveAt(i);
							break;
						}
					}
				}
				if (parentTask2 is Composite)
				{
					Composite composite2 = parentTask2 as Composite;
					if (composite2.AbortType == AbortType.Self || composite2.AbortType == AbortType.None || behaviorTree.activeStack[stackIndex].Count == 0)
					{
						this.RemoveChildConditionalReevaluate(behaviorTree, taskIndex);
					}
					else if (composite2.AbortType == AbortType.LowerPriority || composite2.AbortType == AbortType.Both)
					{
						for (int j = 0; j < behaviorTree.childConditionalIndex[taskIndex].Count; j++)
						{
							int num3 = behaviorTree.childConditionalIndex[taskIndex][j];
							BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate3;
							if (behaviorTree.conditionalReevaluateMap.TryGetValue(num3, out conditionalReevaluate3))
							{
								int num4 = behaviorTree.parentCompositeIndex[taskIndex];
								if (num4 != -1)
								{
									if (!(behaviorTree.taskList[num4] as ParentTask).CanRunParallelChildren())
									{
										conditionalReevaluate3.compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
										behaviorTree.taskList[num3].NodeData.IsReevaluating = true;
									}
									else
									{
										for (int k = behaviorTree.conditionalReevaluate.Count - 1; k > j - 1; k--)
										{
											BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate4 = behaviorTree.conditionalReevaluate[k];
											if (this.FindLCA(behaviorTree, num4, conditionalReevaluate4.index) == num4)
											{
												behaviorTree.taskList[behaviorTree.conditionalReevaluate[k].index].NodeData.IsReevaluating = false;
												ObjectPool.Return<BehaviorManager.BehaviorTree.ConditionalReevaluate>(behaviorTree.conditionalReevaluate[k]);
												behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[k].index);
												behaviorTree.conditionalReevaluate.RemoveAt(k);
											}
										}
									}
								}
							}
						}
						for (int l = 0; l < behaviorTree.conditionalReevaluate.Count; l++)
						{
							if (behaviorTree.conditionalReevaluate[l].compositeIndex == taskIndex)
							{
								behaviorTree.conditionalReevaluate[l].compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
							}
						}
					}
				}
			}
			if (popChildren)
			{
				for (int m = behaviorTree.activeStack.Count - 1; m > stackIndex; m--)
				{
					if (behaviorTree.activeStack[m].Count > 0 && this.IsParentTask(behaviorTree, taskIndex, behaviorTree.activeStack[m].Peek()))
					{
						TaskStatus taskStatus = TaskStatus.Failure;
						for (int n = behaviorTree.activeStack[m].Count; n > 0; n--)
						{
							this.PopTask(behaviorTree, behaviorTree.activeStack[m].Peek(), m, ref taskStatus, false, notifyOnEmptyStack);
						}
					}
				}
			}
			if (behaviorTree.activeStack[stackIndex].Count == 0)
			{
				if (stackIndex == 0)
				{
					if (notifyOnEmptyStack)
					{
						if (behaviorTree.behavior.RestartWhenComplete)
						{
							this.Restart(behaviorTree);
						}
						else
						{
							Behavior behavior = behaviorTree.behavior;
							this.DisableBehavior(behaviorTree.behavior);
							behavior.ExecutionStatus = status;
						}
					}
					status = TaskStatus.Inactive;
				}
				else
				{
					this.RemoveStack(behaviorTree, stackIndex);
					status = TaskStatus.Running;
				}
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006460 File Offset: 0x00004660
		private void RemoveChildConditionalReevaluate(BehaviorManager.BehaviorTree behaviorTree, int compositeIndex)
		{
			for (int i = behaviorTree.conditionalReevaluate.Count - 1; i > -1; i--)
			{
				if (behaviorTree.conditionalReevaluate[i].compositeIndex == compositeIndex)
				{
					ObjectPool.Return<BehaviorManager.BehaviorTree.ConditionalReevaluate>(behaviorTree.conditionalReevaluate[i]);
					int index = behaviorTree.conditionalReevaluate[i].index;
					behaviorTree.conditionalReevaluateMap.Remove(index);
					behaviorTree.conditionalReevaluate.RemoveAt(i);
					behaviorTree.taskList[index].NodeData.IsReevaluating = false;
				}
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000064F8 File Offset: 0x000046F8
		private void Restart(BehaviorManager.BehaviorTree behaviorTree)
		{
			if (behaviorTree.behavior.LogTaskChanges)
			{
				Debug.Log(string.Format("{0}: Restarting {1}", this.RoundedTime(), behaviorTree.behavior.ToString()));
			}
			this.RemoveChildConditionalReevaluate(behaviorTree, -1);
			if (behaviorTree.behavior.ResetValuesOnRestart)
			{
				behaviorTree.behavior.SaveResetValues();
			}
			for (int i = 0; i < behaviorTree.taskList.Count; i++)
			{
				behaviorTree.taskList[i].OnBehaviorRestart();
			}
			behaviorTree.behavior.OnBehaviorRestarted();
			this.PushTask(behaviorTree, 0, 0);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000065A0 File Offset: 0x000047A0
		private bool IsParentTask(BehaviorManager.BehaviorTree behaviorTree, int possibleParent, int possibleChild)
		{
			int num2;
			for (int num = possibleChild; num != -1; num = num2)
			{
				num2 = behaviorTree.parentIndex[num];
				if (num2 == possibleParent)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000065D8 File Offset: 0x000047D8
		public void Interrupt(Behavior behavior, Task task)
		{
			this.Interrupt(behavior, task, task);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000065E4 File Offset: 0x000047E4
		public void Interrupt(Behavior behavior, Task task, Task interruptionTask)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			int num = -1;
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.taskList.Count; i++)
			{
				if (behaviorTree.taskList[i].ReferenceID == task.ReferenceID)
				{
					num = i;
					break;
				}
			}
			if (num > -1)
			{
				for (int j = 0; j < behaviorTree.activeStack.Count; j++)
				{
					if (behaviorTree.activeStack[j].Count > 0)
					{
						for (int num2 = behaviorTree.activeStack[j].Peek(); num2 != -1; num2 = behaviorTree.parentIndex[num2])
						{
							if (num2 == num)
							{
								behaviorTree.interruptionIndex[j] = num;
								if (behavior.LogTaskChanges)
								{
									Debug.Log(string.Format("{0}: {1}: Interrupt task {2} ({3}) with index {4} at stack index {5}", new object[]
									{
										this.RoundedTime(),
										behaviorTree.behavior.ToString(),
										task.FriendlyName,
										task.GetType().ToString(),
										num,
										j
									}));
								}
								interruptionTask.NodeData.InterruptTime = Time.realtimeSinceStartup;
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006748 File Offset: 0x00004948
		public void StopThirdPartyTask(BehaviorManager.BehaviorTree behaviorTree, int taskIndex)
		{
			this.thirdPartyTaskCompare.Task = behaviorTree.taskList[taskIndex];
			object key;
			if (this.taskObjectMap.TryGetValue(this.thirdPartyTaskCompare, out key))
			{
				BehaviorManager.ThirdPartyObjectType thirdPartyObjectType = this.objectTaskMap[key].ThirdPartyObjectType;
				if (BehaviorManager.invokeParameters == null)
				{
					BehaviorManager.invokeParameters = new object[1];
				}
				BehaviorManager.invokeParameters[0] = behaviorTree.taskList[taskIndex];
				switch (thirdPartyObjectType)
				{
				case BehaviorManager.ThirdPartyObjectType.PlayMaker:
					BehaviorManager.PlayMakerStopMethod.Invoke(null, BehaviorManager.invokeParameters);
					break;
				case BehaviorManager.ThirdPartyObjectType.uScript:
					BehaviorManager.UScriptStopMethod.Invoke(null, BehaviorManager.invokeParameters);
					break;
				case BehaviorManager.ThirdPartyObjectType.DialogueSystem:
					BehaviorManager.DialogueSystemStopMethod.Invoke(null, BehaviorManager.invokeParameters);
					break;
				case BehaviorManager.ThirdPartyObjectType.uSequencer:
					BehaviorManager.USequencerStopMethod.Invoke(null, BehaviorManager.invokeParameters);
					break;
				case BehaviorManager.ThirdPartyObjectType.ICode:
					BehaviorManager.ICodeStopMethod.Invoke(null, BehaviorManager.invokeParameters);
					break;
				}
				this.RemoveActiveThirdPartyTask(behaviorTree.taskList[taskIndex]);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006860 File Offset: 0x00004A60
		public void RemoveActiveThirdPartyTask(Task task)
		{
			this.thirdPartyTaskCompare.Task = task;
			object obj;
			if (this.taskObjectMap.TryGetValue(this.thirdPartyTaskCompare, out obj))
			{
				ObjectPool.Return<object>(obj);
				this.taskObjectMap.Remove(this.thirdPartyTaskCompare);
				this.objectTaskMap.Remove(obj);
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000068B8 File Offset: 0x00004AB8
		private void RemoveStack(BehaviorManager.BehaviorTree behaviorTree, int stackIndex)
		{
			Stack<int> stack = behaviorTree.activeStack[stackIndex];
			stack.Clear();
			ObjectPool.Return<Stack<int>>(stack);
			behaviorTree.activeStack.RemoveAt(stackIndex);
			behaviorTree.interruptionIndex.RemoveAt(stackIndex);
			behaviorTree.nonInstantTaskStatus.RemoveAt(stackIndex);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006904 File Offset: 0x00004B04
		private int FindLCA(BehaviorManager.BehaviorTree behaviorTree, int taskIndex1, int taskIndex2)
		{
			HashSet<int> hashSet = ObjectPool.Get<HashSet<int>>();
			hashSet.Clear();
			int num;
			for (num = taskIndex1; num != -1; num = behaviorTree.parentIndex[num])
			{
				hashSet.Add(num);
			}
			num = taskIndex2;
			while (!hashSet.Contains(num))
			{
				num = behaviorTree.parentIndex[num];
			}
			return num;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006964 File Offset: 0x00004B64
		private bool IsChild(BehaviorManager.BehaviorTree behaviorTree, int taskIndex1, int taskIndex2)
		{
			for (int num = taskIndex1; num != -1; num = behaviorTree.parentIndex[num])
			{
				if (num == taskIndex2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006998 File Offset: 0x00004B98
		public List<Task> GetActiveTasks(Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return null;
			}
			List<Task> list = new List<Task>();
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				Task task = behaviorTree.taskList[behaviorTree.activeStack[i].Peek()];
				if (task is Tasks.Action)
				{
					list.Add(task);
				}
			}
			return list;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006A18 File Offset: 0x00004C18
		public void BehaviorOnCollisionEnter(Collision collision, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnCollisionEnter(collision);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnCollisionEnter(collision);
					}
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006B48 File Offset: 0x00004D48
		public void BehaviorOnCollisionExit(Collision collision, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnCollisionExit(collision);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnCollisionExit(collision);
					}
				}
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006C78 File Offset: 0x00004E78
		public void BehaviorOnTriggerEnter(Collider other, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnTriggerEnter(other);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnTriggerEnter(other);
					}
				}
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006DA8 File Offset: 0x00004FA8
		public void BehaviorOnTriggerExit(Collider other, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnTriggerExit(other);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnTriggerExit(other);
					}
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006ED8 File Offset: 0x000050D8
		public void BehaviorOnCollisionEnter2D(Collision2D collision, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnCollisionEnter2D(collision);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnCollisionEnter2D(collision);
					}
				}
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007008 File Offset: 0x00005208
		public void BehaviorOnCollisionExit2D(Collision2D collision, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnCollisionExit2D(collision);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnCollisionExit2D(collision);
					}
				}
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00007138 File Offset: 0x00005338
		public void BehaviorOnTriggerEnter2D(Collider2D other, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnTriggerEnter2D(other);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnTriggerEnter2D(other);
					}
				}
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00007268 File Offset: 0x00005468
		public void BehaviorOnTriggerExit2D(Collider2D other, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnTriggerExit2D(other);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnTriggerExit2D(other);
					}
				}
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00007398 File Offset: 0x00005598
		public void BehaviorOnControllerColliderHit(ControllerColliderHit hit, Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					for (int num = behaviorTree.activeStack[i].Peek(); num != -1; num = behaviorTree.parentIndex[num])
					{
						if (behaviorTree.taskList[num].Disabled)
						{
							break;
						}
						behaviorTree.taskList[num].OnControllerColliderHit(hit);
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].Disabled)
				{
					if (behaviorTree.conditionalReevaluate[j].compositeIndex != -1)
					{
						behaviorTree.taskList[num].OnControllerColliderHit(hit);
					}
				}
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000074C8 File Offset: 0x000056C8
		public bool MapObjectToTask(object objectKey, Task task, BehaviorManager.ThirdPartyObjectType objectType)
		{
			if (this.objectTaskMap.ContainsKey(objectKey))
			{
				string arg = string.Empty;
				switch (objectType)
				{
				case BehaviorManager.ThirdPartyObjectType.PlayMaker:
					arg = "PlayMaker FSM";
					break;
				case BehaviorManager.ThirdPartyObjectType.uScript:
					arg = "uScript Graph";
					break;
				case BehaviorManager.ThirdPartyObjectType.DialogueSystem:
					arg = "Dialogue System";
					break;
				case BehaviorManager.ThirdPartyObjectType.uSequencer:
					arg = "uSequencer sequence";
					break;
				case BehaviorManager.ThirdPartyObjectType.ICode:
					arg = "ICode state machine";
					break;
				}
				Debug.LogError(string.Format("Only one behavior can be mapped to the same instance of the {0}.", arg));
				return false;
			}
			BehaviorManager.ThirdPartyTask thirdPartyTask = ObjectPool.Get<BehaviorManager.ThirdPartyTask>();
			thirdPartyTask.Initialize(task, objectType);
			this.objectTaskMap.Add(objectKey, thirdPartyTask);
			this.taskObjectMap.Add(thirdPartyTask, objectKey);
			return true;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007580 File Offset: 0x00005780
		public Task TaskForObject(object objectKey)
		{
			BehaviorManager.ThirdPartyTask thirdPartyTask;
			if (!this.objectTaskMap.TryGetValue(objectKey, out thirdPartyTask))
			{
				return null;
			}
			return thirdPartyTask.Task;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000075A8 File Offset: 0x000057A8
		private decimal RoundedTime()
		{
			return Math.Round((decimal)Time.time, 5, MidpointRounding.AwayFromZero);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000075BC File Offset: 0x000057BC
		public List<Task> GetTaskList(Behavior behavior)
		{
			if (!this.IsBehaviorEnabled(behavior))
			{
				return null;
			}
			BehaviorManager.BehaviorTree behaviorTree = this.behaviorTreeMap[behavior];
			return behaviorTree.taskList;
		}

		// Token: 0x0400002F RID: 47
		public static BehaviorManager instance;

		// Token: 0x04000030 RID: 48
		[SerializeField]
		private UpdateIntervalType updateInterval;

		// Token: 0x04000031 RID: 49
		[SerializeField]
		private float updateIntervalSeconds;

		// Token: 0x04000032 RID: 50
		[SerializeField]
		private BehaviorManager.ExecutionsPerTickType executionsPerTick;

		// Token: 0x04000033 RID: 51
		[SerializeField]
		private int maxTaskExecutionsPerTick = 100;

		// Token: 0x04000034 RID: 52
		private WaitForSeconds updateWait;

		// Token: 0x04000035 RID: 53
		public BehaviorManager.BehaviorManagerHandler onEnableBehavior;

		// Token: 0x04000036 RID: 54
		public BehaviorManager.BehaviorManagerHandler onTaskBreakpoint;

		// Token: 0x04000037 RID: 55
		private List<BehaviorManager.BehaviorTree> behaviorTrees = new List<BehaviorManager.BehaviorTree>();

		// Token: 0x04000038 RID: 56
		private Dictionary<Behavior, BehaviorManager.BehaviorTree> pausedBehaviorTrees = new Dictionary<Behavior, BehaviorManager.BehaviorTree>();

		// Token: 0x04000039 RID: 57
		private Dictionary<Behavior, BehaviorManager.BehaviorTree> behaviorTreeMap = new Dictionary<Behavior, BehaviorManager.BehaviorTree>();

		// Token: 0x0400003A RID: 58
		private List<int> conditionalParentIndexes = new List<int>();

		// Token: 0x0400003B RID: 59
		private Dictionary<object, BehaviorManager.ThirdPartyTask> objectTaskMap = new Dictionary<object, BehaviorManager.ThirdPartyTask>();

		// Token: 0x0400003C RID: 60
		private Dictionary<BehaviorManager.ThirdPartyTask, object> taskObjectMap = new Dictionary<BehaviorManager.ThirdPartyTask, object>(new BehaviorManager.ThirdPartyTaskComparer());

		// Token: 0x0400003D RID: 61
		private BehaviorManager.ThirdPartyTask thirdPartyTaskCompare = new BehaviorManager.ThirdPartyTask();

		// Token: 0x0400003E RID: 62
		private static MethodInfo playMakerStopMethod;

		// Token: 0x0400003F RID: 63
		private static MethodInfo uScriptStopMethod;

		// Token: 0x04000040 RID: 64
		private static MethodInfo dialogueSystemStopMethod;

		// Token: 0x04000041 RID: 65
		private static MethodInfo uSequencerStopMethod;

		// Token: 0x04000042 RID: 66
		private static MethodInfo iCodeStopMethod;

		// Token: 0x04000043 RID: 67
		private static object[] invokeParameters;

		// Token: 0x04000044 RID: 68
		private Behavior breakpointTree;

		// Token: 0x04000045 RID: 69
		private bool dirty;

		// Token: 0x02000009 RID: 9
		public enum ExecutionsPerTickType
		{
			// Token: 0x04000047 RID: 71
			NoDuplicates,
			// Token: 0x04000048 RID: 72
			Count
		}

		// Token: 0x0200000A RID: 10
		public class BehaviorTree
		{
			// Token: 0x060000B5 RID: 181 RVA: 0x00007684 File Offset: 0x00005884
			public void Initialize(Behavior b)
			{
				this.behavior = b;
				for (int i = this.childrenIndex.Count - 1; i > -1; i--)
				{
					ObjectPool.Return<List<int>>(this.childrenIndex[i]);
				}
				for (int j = this.activeStack.Count - 1; j > -1; j--)
				{
					ObjectPool.Return<Stack<int>>(this.activeStack[j]);
				}
				for (int k = this.childConditionalIndex.Count - 1; k > -1; k--)
				{
					ObjectPool.Return<List<int>>(this.childConditionalIndex[k]);
				}
				this.taskList.Clear();
				this.parentIndex.Clear();
				this.childrenIndex.Clear();
				this.relativeChildIndex.Clear();
				this.activeStack.Clear();
				this.nonInstantTaskStatus.Clear();
				this.interruptionIndex.Clear();
				this.conditionalReevaluate.Clear();
				this.conditionalReevaluateMap.Clear();
				this.parentReevaluate.Clear();
				this.parentCompositeIndex.Clear();
				this.childConditionalIndex.Clear();
			}

			// Token: 0x04000049 RID: 73
			public List<Task> taskList = new List<Task>(5);

			// Token: 0x0400004A RID: 74
			public List<int> parentIndex = new List<int>(5);

			// Token: 0x0400004B RID: 75
			public List<List<int>> childrenIndex = new List<List<int>>(10);

			// Token: 0x0400004C RID: 76
			public List<int> relativeChildIndex = new List<int>(10);

			// Token: 0x0400004D RID: 77
			public List<Stack<int>> activeStack = new List<Stack<int>>(10);

			// Token: 0x0400004E RID: 78
			public List<TaskStatus> nonInstantTaskStatus = new List<TaskStatus>(10);

			// Token: 0x0400004F RID: 79
			public List<int> interruptionIndex = new List<int>(10);

			// Token: 0x04000050 RID: 80
			public List<BehaviorManager.BehaviorTree.ConditionalReevaluate> conditionalReevaluate = new List<BehaviorManager.BehaviorTree.ConditionalReevaluate>(10);

			// Token: 0x04000051 RID: 81
			public Dictionary<int, BehaviorManager.BehaviorTree.ConditionalReevaluate> conditionalReevaluateMap = new Dictionary<int, BehaviorManager.BehaviorTree.ConditionalReevaluate>(10);

			// Token: 0x04000052 RID: 82
			public List<int> parentReevaluate = new List<int>(5);

			// Token: 0x04000053 RID: 83
			public List<int> parentCompositeIndex = new List<int>(5);

			// Token: 0x04000054 RID: 84
			public List<List<int>> childConditionalIndex = new List<List<int>>(5);

			// Token: 0x04000055 RID: 85
			public int executionCount;

			// Token: 0x04000056 RID: 86
			public Behavior behavior;

			// Token: 0x0200000B RID: 11
			public class ConditionalReevaluate
			{
				// Token: 0x060000B7 RID: 183 RVA: 0x000077C4 File Offset: 0x000059C4
				public void Initialize(int i, TaskStatus status, int stack, int composite)
				{
					this.index = i;
					this.taskStatus = status;
					this.stackIndex = stack;
					this.compositeIndex = composite;
				}

				// Token: 0x04000057 RID: 87
				public int index;

				// Token: 0x04000058 RID: 88
				public TaskStatus taskStatus;

				// Token: 0x04000059 RID: 89
				public int compositeIndex = -1;

				// Token: 0x0400005A RID: 90
				public int stackIndex = -1;
			}
		}

		// Token: 0x0200000C RID: 12
		public enum ThirdPartyObjectType
		{
			// Token: 0x0400005C RID: 92
			PlayMaker,
			// Token: 0x0400005D RID: 93
			uScript,
			// Token: 0x0400005E RID: 94
			DialogueSystem,
			// Token: 0x0400005F RID: 95
			uSequencer,
			// Token: 0x04000060 RID: 96
			ICode
		}

		// Token: 0x0200000D RID: 13
		public class ThirdPartyTask
		{
			// Token: 0x1700001B RID: 27
			// (get) Token: 0x060000B9 RID: 185 RVA: 0x000077EC File Offset: 0x000059EC
			// (set) Token: 0x060000BA RID: 186 RVA: 0x000077F4 File Offset: 0x000059F4
			public Task Task
			{
				get
				{
					return this.task;
				}
				set
				{
					this.task = value;
				}
			}

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x060000BB RID: 187 RVA: 0x00007800 File Offset: 0x00005A00
			public BehaviorManager.ThirdPartyObjectType ThirdPartyObjectType
			{
				get
				{
					return this.thirdPartyObjectType;
				}
			}

			// Token: 0x060000BC RID: 188 RVA: 0x00007808 File Offset: 0x00005A08
			public void Initialize(Task t, BehaviorManager.ThirdPartyObjectType objectType)
			{
				this.task = t;
				this.thirdPartyObjectType = objectType;
			}

			// Token: 0x04000061 RID: 97
			private Task task;

			// Token: 0x04000062 RID: 98
			private BehaviorManager.ThirdPartyObjectType thirdPartyObjectType;
		}

		// Token: 0x0200000E RID: 14
		public class ThirdPartyTaskComparer : IEqualityComparer<BehaviorManager.ThirdPartyTask>
		{
			// Token: 0x060000BE RID: 190 RVA: 0x00007820 File Offset: 0x00005A20
			public bool Equals(BehaviorManager.ThirdPartyTask a, BehaviorManager.ThirdPartyTask b)
			{
				return !object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null) && a.Task.Equals(b.Task);
			}

			// Token: 0x060000BF RID: 191 RVA: 0x0000785C File Offset: 0x00005A5C
			public int GetHashCode(BehaviorManager.ThirdPartyTask obj)
			{
				return (obj == null) ? 0 : obj.Task.GetHashCode();
			}
		}

		// Token: 0x0200000F RID: 15
		public class TaskAddData
		{
			// Token: 0x060000C1 RID: 193 RVA: 0x000078AC File Offset: 0x00005AAC
			public void Initialize()
			{
				if (this.overrideFields != null)
				{
					foreach (KeyValuePair<string, BehaviorManager.TaskAddData.OverrideFieldValue> obj in this.overrideFields)
					{
						ObjectPool.Return<KeyValuePair<string, BehaviorManager.TaskAddData.OverrideFieldValue>>(obj);
					}
				}
				ObjectPool.Return<Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue>>(this.overrideFields);
				this.fromExternalTask = false;
				this.parentTask = null;
				this.parentIndex = -1;
				this.depth = 0;
				this.compositeParentIndex = -1;
				this.overrideFields = null;
			}

			// Token: 0x04000063 RID: 99
			public bool fromExternalTask;

			// Token: 0x04000064 RID: 100
			public ParentTask parentTask;

			// Token: 0x04000065 RID: 101
			public int parentIndex = -1;

			// Token: 0x04000066 RID: 102
			public int depth;

			// Token: 0x04000067 RID: 103
			public int compositeParentIndex = -1;

			// Token: 0x04000068 RID: 104
			public Vector2 offset;

			// Token: 0x04000069 RID: 105
			public Dictionary<string, BehaviorManager.TaskAddData.OverrideFieldValue> overrideFields;

			// Token: 0x0400006A RID: 106
			public HashSet<object> overiddenFields = new HashSet<object>();

			// Token: 0x0400006B RID: 107
			public int errorTask = -1;

			// Token: 0x0400006C RID: 108
			public string errorTaskName = string.Empty;

			// Token: 0x02000010 RID: 16
			public class OverrideFieldValue
			{
				// Token: 0x1700001D RID: 29
				// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000795C File Offset: 0x00005B5C
				public object Value
				{
					get
					{
						return this.value;
					}
				}

				// Token: 0x1700001E RID: 30
				// (get) Token: 0x060000C4 RID: 196 RVA: 0x00007964 File Offset: 0x00005B64
				public int Depth
				{
					get
					{
						return this.depth;
					}
				}

				// Token: 0x060000C5 RID: 197 RVA: 0x0000796C File Offset: 0x00005B6C
				public void Initialize(object v, int d)
				{
					this.value = v;
					this.depth = d;
				}

				// Token: 0x0400006D RID: 109
				private object value;

				// Token: 0x0400006E RID: 110
				private int depth;
			}
		}

		// Token: 0x0200004E RID: 78
		// (Invoke) Token: 0x06000265 RID: 613
		public delegate void BehaviorManagerHandler();
	}
}
