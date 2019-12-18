using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200002E RID: 46
	[CustomEditor(typeof(VariableSynchronizer))]
	public class VariableSynchronizerInspector : UnityEditor.Editor
    {
		// Token: 0x060002CB RID: 715 RVA: 0x0001BE90 File Offset: 0x0001A090
		public override void OnInspectorGUI()
		{
			VariableSynchronizer variableSynchronizer = this.target as VariableSynchronizer;
			if (variableSynchronizer == null)
			{
				return;
			}
			GUILayout.Space(5f);
			variableSynchronizer.UpdateInterval = (UpdateIntervalType)EditorGUILayout.EnumPopup("Update Interval", variableSynchronizer.UpdateInterval, new GUILayoutOption[0]);
			if (variableSynchronizer.UpdateInterval == UpdateIntervalType.SpecifySeconds)
			{
				variableSynchronizer.UpdateIntervalSeconds = EditorGUILayout.FloatField("Seconds", variableSynchronizer.UpdateIntervalSeconds, new GUILayoutOption[0]);
			}
			GUILayout.Space(5f);
			GUI.enabled = !Application.isPlaying;
			this.DrawSharedVariableSynchronizer(this.sharedVariableSynchronizer, null);
			if (string.IsNullOrEmpty(this.sharedVariableSynchronizer.targetName))
			{
				this.DrawSynchronizedVariables(variableSynchronizer);
				return;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Direction", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(146f)
			});
			if (GUILayout.Button(BehaviorDesignerUtility.LoadTexture((!this.setVariable) ? "RightArrowButton.png" : "LeftArrowButton.png", true, this), BehaviorDesignerUtility.ButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(22f)
			}))
			{
				this.setVariable = !this.setVariable;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			this.synchronizationType = (VariableSynchronizer.SynchronizationType)EditorGUILayout.EnumPopup("Type", this.synchronizationType, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.targetSynchronizer = new VariableSynchronizerInspector.Synchronizer();
			}
			if (this.targetSynchronizer == null)
			{
				this.targetSynchronizer = new VariableSynchronizerInspector.Synchronizer();
			}
			if (this.sharedVariableValueType == null && !string.IsNullOrEmpty(this.sharedVariableValueTypeName))
			{
				this.sharedVariableValueType = TaskUtility.GetTypeWithinAssembly(this.sharedVariableValueTypeName);
			}
			switch (this.synchronizationType)
			{
			case VariableSynchronizer.SynchronizationType.BehaviorDesigner:
				this.DrawSharedVariableSynchronizer(this.targetSynchronizer, this.sharedVariableValueType);
				break;
			case VariableSynchronizer.SynchronizationType.Property:
				this.DrawPropertySynchronizer(this.targetSynchronizer, this.sharedVariableValueType);
				break;
			case VariableSynchronizer.SynchronizationType.Animator:
				this.DrawAnimatorSynchronizer(this.targetSynchronizer);
				break;
			case VariableSynchronizer.SynchronizationType.PlayMaker:
				this.DrawPlayMakerSynchronizer(this.targetSynchronizer, this.sharedVariableValueType);
				break;
			case VariableSynchronizer.SynchronizationType.uFrame:
				this.DrawuFrameSynchronizer(this.targetSynchronizer, this.sharedVariableValueType);
				break;
			}
			if (string.IsNullOrEmpty(this.targetSynchronizer.targetName))
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button("Add", new GUILayoutOption[0]))
			{
				VariableSynchronizer.SynchronizedVariable item = new VariableSynchronizer.SynchronizedVariable(this.synchronizationType, this.setVariable, this.sharedVariableSynchronizer.component as Behavior, this.sharedVariableSynchronizer.targetName, this.sharedVariableSynchronizer.global, this.targetSynchronizer.component, this.targetSynchronizer.targetName, this.targetSynchronizer.global);
				variableSynchronizer.SynchronizedVariables.Add(item);
				BehaviorDesignerUtility.SetObjectDirty(variableSynchronizer);
				this.sharedVariableSynchronizer = new VariableSynchronizerInspector.Synchronizer();
				this.targetSynchronizer = new VariableSynchronizerInspector.Synchronizer();
			}
			GUI.enabled = true;
			this.DrawSynchronizedVariables(variableSynchronizer);
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0001C1A4 File Offset: 0x0001A3A4
		public static void DrawComponentSelector(VariableSynchronizerInspector.Synchronizer synchronizer, Type componentType, VariableSynchronizerInspector.ComponentListType listType)
		{
			bool flag = false;
			EditorGUI.BeginChangeCheck();
			synchronizer.gameObject = (EditorGUILayout.ObjectField("GameObject", synchronizer.gameObject, typeof(GameObject), true, new GUILayoutOption[0]) as GameObject);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			if (synchronizer.gameObject == null)
			{
				GUI.enabled = false;
			}
			switch (listType)
			{
			case VariableSynchronizerInspector.ComponentListType.Instant:
				if (flag)
				{
					if (synchronizer.gameObject != null)
					{
						synchronizer.component = synchronizer.gameObject.GetComponent(componentType);
					}
					else
					{
						synchronizer.component = null;
					}
				}
				break;
			case VariableSynchronizerInspector.ComponentListType.Popup:
			{
				int num = 0;
				List<string> list = new List<string>();
				Component[] array = null;
				list.Add("None");
				if (synchronizer.gameObject != null)
				{
					array = synchronizer.gameObject.GetComponents(componentType);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].Equals(synchronizer.component))
						{
							num = list.Count;
						}
						string text = BehaviorDesignerUtility.SplitCamelCase(array[i].GetType().Name);
						int num2 = 0;
						for (int j = 0; j < list.Count; j++)
						{
							if (list[i].Equals(text))
							{
								num2++;
							}
						}
						if (num2 > 0)
						{
							text = text + " " + num2;
						}
						list.Add(text);
					}
				}
				EditorGUI.BeginChangeCheck();
				num = EditorGUILayout.Popup("Component", num, list.ToArray(), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (num != 0)
					{
						synchronizer.component = array[num - 1];
					}
					else
					{
						synchronizer.component = null;
					}
				}
				break;
			}
			case VariableSynchronizerInspector.ComponentListType.BehaviorDesignerGroup:
				if (synchronizer.gameObject != null)
				{
					Behavior[] components = synchronizer.gameObject.GetComponents<Behavior>();
					if (components != null && components.Length > 1)
					{
						synchronizer.componentGroup = EditorGUILayout.IntField("Behavior Tree Group", synchronizer.componentGroup, new GUILayoutOption[0]);
					}
					synchronizer.component = VariableSynchronizerInspector.GetBehaviorWithGroup(components, synchronizer.componentGroup);
				}
				break;
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0001C3E4 File Offset: 0x0001A5E4
		private bool DrawSharedVariableSynchronizer(VariableSynchronizerInspector.Synchronizer synchronizer, Type valueType)
		{
			VariableSynchronizerInspector.DrawComponentSelector(synchronizer, typeof(Behavior), VariableSynchronizerInspector.ComponentListType.BehaviorDesignerGroup);
			int num = 0;
			int num2 = -1;
			string[] array = null;
			if (synchronizer.component != null)
			{
				Behavior behavior = synchronizer.component as Behavior;
				num = FieldInspector.GetVariablesOfType(valueType, synchronizer.global, synchronizer.targetName, behavior.GetBehaviorSource(), out array, ref num2, valueType == null);
			}
			else
			{
				array = new string[]
				{
					"None"
				};
			}
			EditorGUI.BeginChangeCheck();
			num = EditorGUILayout.Popup("Shared Variable", num, array, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (num != 0)
				{
					if (num2 != -1 && num >= num2)
					{
						synchronizer.targetName = array[num].Substring(8, array[num].Length - 8);
						synchronizer.global = true;
					}
					else
					{
						synchronizer.targetName = array[num];
						synchronizer.global = false;
					}
					if (valueType == null)
					{
						SharedVariable variable;
						if (synchronizer.global)
						{
							variable = GlobalVariables.Instance.GetVariable(synchronizer.targetName);
						}
						else
						{
							Behavior behavior2 = synchronizer.component as Behavior;
							variable = behavior2.GetVariable(array[num]);
						}
						this.sharedVariableValueTypeName = variable.GetType().GetProperty("Value").PropertyType.FullName;
						this.sharedVariableValueType = null;
					}
				}
				else
				{
					synchronizer.targetName = null;
				}
			}
			if (string.IsNullOrEmpty(synchronizer.targetName))
			{
				GUI.enabled = false;
			}
			return GUI.enabled;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0001C560 File Offset: 0x0001A760
		private static Behavior GetBehaviorWithGroup(Behavior[] behaviors, int group)
		{
			if (behaviors == null || behaviors.Length == 0)
			{
				return null;
			}
			if (behaviors.Length == 1)
			{
				return behaviors[0];
			}
			for (int i = 0; i < behaviors.Length; i++)
			{
				if (behaviors[i].Group == group)
				{
					return behaviors[i];
				}
			}
			return behaviors[0];
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0001C5B4 File Offset: 0x0001A7B4
		private void DrawPropertySynchronizer(VariableSynchronizerInspector.Synchronizer synchronizer, Type valueType)
		{
			VariableSynchronizerInspector.DrawComponentSelector(synchronizer, typeof(Component), VariableSynchronizerInspector.ComponentListType.Popup);
			int num = 0;
			List<string> list = new List<string>();
			list.Add("None");
			if (synchronizer.component != null)
			{
				PropertyInfo[] properties = synchronizer.component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].PropertyType.Equals(valueType) && !properties[i].IsSpecialName)
					{
						if (properties[i].Name.Equals(synchronizer.targetName))
						{
							num = list.Count;
						}
						list.Add(properties[i].Name);
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			num = EditorGUILayout.Popup("Property", num, list.ToArray(), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (num != 0)
				{
					synchronizer.targetName = list[num];
				}
				else
				{
					synchronizer.targetName = string.Empty;
				}
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001C6BC File Offset: 0x0001A8BC
		private void DrawAnimatorSynchronizer(VariableSynchronizerInspector.Synchronizer synchronizer)
		{
			VariableSynchronizerInspector.DrawComponentSelector(synchronizer, typeof(Animator), VariableSynchronizerInspector.ComponentListType.Instant);
			synchronizer.targetName = EditorGUILayout.TextField("Parameter Name", synchronizer.targetName, new GUILayoutOption[0]);
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001C6EC File Offset: 0x0001A8EC
		private void DrawPlayMakerSynchronizer(VariableSynchronizerInspector.Synchronizer synchronizer, Type valueType)
		{
			if (this.playMakerSynchronizationType == null)
			{
				this.playMakerSynchronizationType = Type.GetType("BehaviorDesigner.Editor.VariableSynchronizerInspector_PlayMaker, Assembly-CSharp-Editor");
				if (this.playMakerSynchronizationType == null)
				{
					EditorGUILayout.LabelField("Unable to find PlayMaker inspector task.", new GUILayoutOption[0]);
					return;
				}
			}
			if (this.thirdPartySynchronizer == null)
			{
				MethodInfo method = this.playMakerSynchronizationType.GetMethod("DrawPlayMakerSynchronizer");
				if (method != null)
				{
					this.thirdPartySynchronizer = (Action<VariableSynchronizerInspector.Synchronizer, Type>)Delegate.CreateDelegate(typeof(Action<VariableSynchronizerInspector.Synchronizer, Type>), method);
				}
			}
			this.thirdPartySynchronizer(synchronizer, valueType);
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001C77C File Offset: 0x0001A97C
		private void DrawuFrameSynchronizer(VariableSynchronizerInspector.Synchronizer synchronizer, Type valueType)
		{
			if (this.uFrameSynchronizationType == null)
			{
				this.uFrameSynchronizationType = Type.GetType("BehaviorDesigner.Editor.VariableSynchronizerInspector_uFrame, Assembly-CSharp-Editor");
				if (this.uFrameSynchronizationType == null)
				{
					EditorGUILayout.LabelField("Unable to find uFrame inspector task.", new GUILayoutOption[0]);
					return;
				}
			}
			if (this.thirdPartySynchronizer == null)
			{
				MethodInfo method = this.uFrameSynchronizationType.GetMethod("DrawSynchronizer");
				if (method != null)
				{
					this.thirdPartySynchronizer = (Action<VariableSynchronizerInspector.Synchronizer, Type>)Delegate.CreateDelegate(typeof(Action<VariableSynchronizerInspector.Synchronizer, Type>), method);
				}
			}
			this.thirdPartySynchronizer(synchronizer, valueType);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0001C80C File Offset: 0x0001AA0C
		private void DrawSynchronizedVariables(VariableSynchronizer variableSynchronizer)
		{
			GUI.enabled = true;
			if (variableSynchronizer.SynchronizedVariables == null || variableSynchronizer.SynchronizedVariables.Count == 0)
			{
				return;
			}
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.x = -5f;
			lastRect.y += lastRect.height + 1f;
			lastRect.height = 2f;
			lastRect.width += 20f;
			GUI.DrawTexture(lastRect, BehaviorDesignerUtility.LoadTexture("ContentSeparator.png", true, this));
			GUILayout.Space(6f);
			for (int i = 0; i < variableSynchronizer.SynchronizedVariables.Count; i++)
			{
				VariableSynchronizer.SynchronizedVariable synchronizedVariable = variableSynchronizer.SynchronizedVariables[i];
				if (synchronizedVariable.global)
				{
					if (GlobalVariables.Instance.GetVariable(synchronizedVariable.variableName) == null)
					{
						variableSynchronizer.SynchronizedVariables.RemoveAt(i);
						break;
					}
				}
				else if (synchronizedVariable.behavior.GetVariable(synchronizedVariable.variableName) == null)
				{
					variableSynchronizer.SynchronizedVariables.RemoveAt(i);
					break;
				}
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.LabelField(synchronizedVariable.variableName, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(120f)
				});
				if (GUILayout.Button(BehaviorDesignerUtility.LoadTexture((!synchronizedVariable.setVariable) ? "RightArrowButton.png" : "LeftArrowButton.png", true, this), BehaviorDesignerUtility.ButtonGUIStyle, new GUILayoutOption[]
				{
					GUILayout.Width(22f)
				}) && !Application.isPlaying)
				{
					synchronizedVariable.setVariable = !synchronizedVariable.setVariable;
				}
				EditorGUILayout.LabelField(string.Format("{0} ({1})", synchronizedVariable.targetName, synchronizedVariable.synchronizationType.ToString()), new GUILayoutOption[]
				{
					GUILayout.MinWidth(120f)
				});
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(BehaviorDesignerUtility.LoadTexture("DeleteButton.png", true, this), BehaviorDesignerUtility.ButtonGUIStyle, new GUILayoutOption[]
				{
					GUILayout.Width(22f)
				}))
				{
					variableSynchronizer.SynchronizedVariables.RemoveAt(i);
					EditorGUILayout.EndHorizontal();
					break;
				}
				GUILayout.Space(2f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(2f);
			}
			GUILayout.Space(4f);
		}

		// Token: 0x040001A8 RID: 424
		[SerializeField]
		private VariableSynchronizerInspector.Synchronizer sharedVariableSynchronizer = new VariableSynchronizerInspector.Synchronizer();

		// Token: 0x040001A9 RID: 425
		[SerializeField]
		private string sharedVariableValueTypeName;

		// Token: 0x040001AA RID: 426
		private Type sharedVariableValueType;

		// Token: 0x040001AB RID: 427
		[SerializeField]
		private VariableSynchronizer.SynchronizationType synchronizationType;

		// Token: 0x040001AC RID: 428
		[SerializeField]
		private bool setVariable;

		// Token: 0x040001AD RID: 429
		[SerializeField]
		private VariableSynchronizerInspector.Synchronizer targetSynchronizer;

		// Token: 0x040001AE RID: 430
		private Action<VariableSynchronizerInspector.Synchronizer, Type> thirdPartySynchronizer;

		// Token: 0x040001AF RID: 431
		private Type playMakerSynchronizationType;

		// Token: 0x040001B0 RID: 432
		private Type uFrameSynchronizationType;

		// Token: 0x0200002F RID: 47
		public enum ComponentListType
		{
			// Token: 0x040001B2 RID: 434
			Instant,
			// Token: 0x040001B3 RID: 435
			Popup,
			// Token: 0x040001B4 RID: 436
			BehaviorDesignerGroup,
			// Token: 0x040001B5 RID: 437
			None
		}

		// Token: 0x02000030 RID: 48
		[Serializable]
		public class Synchronizer
		{
			// Token: 0x040001B6 RID: 438
			public GameObject gameObject;

			// Token: 0x040001B7 RID: 439
			public Component component;

			// Token: 0x040001B8 RID: 440
			public string targetName;

			// Token: 0x040001B9 RID: 441
			public bool global;

			// Token: 0x040001BA RID: 442
			public int componentGroup;

			// Token: 0x040001BB RID: 443
			public string componentName;
		}
	}
}
