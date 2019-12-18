using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using HelpURLAttribute = BehaviorDesigner.Runtime.Tasks.HelpURLAttribute;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000025 RID: 37
	[Serializable]
	public class TaskInspector : ScriptableObject
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600025E RID: 606 RVA: 0x000166E4 File Offset: 0x000148E4
		public Task ActiveReferenceTask
		{
			get
			{
				return this.activeReferenceTask;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600025F RID: 607 RVA: 0x000166EC File Offset: 0x000148EC
		public FieldInfo ActiveReferenceTaskFieldInfo
		{
			get
			{
				return this.activeReferenceTaskFieldInfo;
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x000166F4 File Offset: 0x000148F4
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00016700 File Offset: 0x00014900
		public void ClearFocus()
		{
			GUIUtility.keyboardControl = 0;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00016708 File Offset: 0x00014908
		public bool HasFocus()
		{
			return GUIUtility.keyboardControl != 0;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00016718 File Offset: 0x00014918
		public bool DrawTaskInspector(BehaviorSource behaviorSource, TaskList taskList, Task task, bool enabled)
		{
			if (task == null || (task.NodeData.NodeDesigner as NodeDesigner).IsEntryDisplay)
			{
				return false;
			}
			this.mScrollPosition = GUILayout.BeginScrollView(this.mScrollPosition, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			if (this.behaviorDesignerWindow == null)
			{
				this.behaviorDesignerWindow = BehaviorDesignerWindow.instance;
			}
			EditorGUIUtility.labelWidth = 150f;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Name", new GUILayoutOption[]
			{
				GUILayout.Width(90f)
			});
			task.FriendlyName = EditorGUILayout.TextField(task.FriendlyName, new GUILayoutOption[0]);
			if (GUILayout.Button(BehaviorDesignerUtility.DocTexture, BehaviorDesignerUtility.TransparentButtonGUIStyle, new GUILayoutOption[0]))
			{
				this.OpenHelpURL(task);
			}
			if (GUILayout.Button(BehaviorDesignerUtility.ColorSelectorTexture(task.NodeData.ColorIndex), BehaviorDesignerUtility.TransparentButtonOffsetGUIStyle, new GUILayoutOption[0]))
			{
				GenericMenu genericMenu = new GenericMenu();
				this.AddColorMenuItem(ref genericMenu, task, "Default", 0);
				this.AddColorMenuItem(ref genericMenu, task, "Red", 1);
				this.AddColorMenuItem(ref genericMenu, task, "Pink", 2);
				this.AddColorMenuItem(ref genericMenu, task, "Brown", 3);
				this.AddColorMenuItem(ref genericMenu, task, "Orange", 4);
				this.AddColorMenuItem(ref genericMenu, task, "Turquoise", 5);
				this.AddColorMenuItem(ref genericMenu, task, "Cyan", 6);
				this.AddColorMenuItem(ref genericMenu, task, "Blue", 7);
				this.AddColorMenuItem(ref genericMenu, task, "Purple", 8);
				genericMenu.ShowAsContext();
			}
			if (GUILayout.Button(BehaviorDesignerUtility.GearTexture, BehaviorDesignerUtility.TransparentButtonGUIStyle, new GUILayoutOption[0]))
			{
				GenericMenu genericMenu2 = new GenericMenu();
				genericMenu2.AddItem(new GUIContent("Edit Script"), false, new GenericMenu.MenuFunction2(TaskInspector.OpenInFileEditor), task);
				genericMenu2.AddItem(new GUIContent("Locate Script"), false, new GenericMenu.MenuFunction2(TaskInspector.SelectInProject), task);
				genericMenu2.AddItem(new GUIContent("Reset"), false, new GenericMenu.MenuFunction2(this.ResetTask), task);
				genericMenu2.ShowAsContext();
			}
			GUILayout.EndHorizontal();
			string text = BehaviorDesignerUtility.SplitCamelCase(task.GetType().Name.ToString());
			if (!task.FriendlyName.Equals(text))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type", new GUILayoutOption[]
				{
					GUILayout.Width(90f)
				});
				EditorGUILayout.LabelField(text, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(170f)
				});
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Instant", new GUILayoutOption[]
			{
				GUILayout.Width(90f)
			});
			task.IsInstant = EditorGUILayout.Toggle(task.IsInstant, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Comment", new GUILayoutOption[0]);
			task.NodeData.Comment = EditorGUILayout.TextArea(task.NodeData.Comment, BehaviorDesignerUtility.TaskInspectorCommentGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Height(48f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				GUI.changed = true;
			}
			BehaviorDesignerUtility.DrawContentSeperator(2);
			GUILayout.Space(6f);
			if (this.DrawTaskFields(behaviorSource, taskList, task, enabled))
			{
				BehaviorUndo.RegisterUndo("Inspector", behaviorSource.Owner.GetObject());
				GUI.changed = true;
			}
			GUI.enabled = true;
			GUILayout.EndScrollView();
			return GUI.changed;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x00016A7C File Offset: 0x00014C7C
		private bool DrawTaskFields(BehaviorSource behaviorSource, TaskList taskList, Task task, bool enabled)
		{
			if (task == null)
			{
				return false;
			}
			EditorGUI.BeginChangeCheck();
			FieldInspector.behaviorSource = behaviorSource;
			this.DrawObjectFields(behaviorSource, taskList, task, task, enabled, true);
			return EditorGUI.EndChangeCheck();
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00016AB8 File Offset: 0x00014CB8
		private void DrawObjectFields(BehaviorSource behaviorSource, TaskList taskList, Task task, object obj, bool enabled, bool drawWatch)
		{
			if (obj == null)
			{
				return;
			}
			List<Type> baseClasses = FieldInspector.GetBaseClasses(obj.GetType());
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			bool flag = this.IsReflectionTask(obj.GetType());
			for (int i = baseClasses.Count - 1; i > -1; i--)
			{
				FieldInfo[] fields = baseClasses[i].GetFields(bindingAttr);
				for (int j = 0; j < fields.Length; j++)
				{
					if (!BehaviorDesignerUtility.HasAttribute(fields[j], typeof(NonSerializedAttribute)) && !BehaviorDesignerUtility.HasAttribute(fields[j], typeof(HideInInspector)) && ((!fields[j].IsPrivate && !fields[j].IsFamily) || BehaviorDesignerUtility.HasAttribute(fields[j], typeof(SerializeField))) && (!(obj is ParentTask) || !fields[j].Name.Equals("children")) && (!flag || (!fields[j].FieldType.Equals(typeof(SharedVariable)) && !fields[j].FieldType.IsSubclassOf(typeof(SharedVariable))) || this.CanDrawReflectedField(obj, fields[j])))
					{
						string s = fields[j].Name;
						if (flag && (fields[j].FieldType.Equals(typeof(SharedVariable)) || fields[j].FieldType.IsSubclassOf(typeof(SharedVariable))))
						{
							s = this.InvokeParameterName(obj, fields[j]);
						}
						TooltipAttribute[] array;
						GUIContent guiContent;
						if ((array = (fields[j].GetCustomAttributes(typeof(TooltipAttribute), false) as TooltipAttribute[])).Length > 0)
						{
							guiContent = new GUIContent(BehaviorDesignerUtility.SplitCamelCase(s), array[0].Tooltip);
						}
						else
						{
							guiContent = new GUIContent(BehaviorDesignerUtility.SplitCamelCase(s));
						}
						object value = fields[j].GetValue(obj);
						Type fieldType = fields[j].FieldType;
						if (typeof(Task).IsAssignableFrom(fieldType) || (typeof(IList).IsAssignableFrom(fieldType) && (typeof(Task).IsAssignableFrom(fieldType.GetElementType()) || (fieldType.IsGenericType && typeof(Task).IsAssignableFrom(fieldType.GetGenericArguments()[0])))))
						{
							EditorGUI.BeginChangeCheck();
							this.DrawTaskValue(behaviorSource, taskList, fields[j], guiContent, task, value as Task, enabled);
							if (BehaviorDesignerWindow.instance.ContainsError(task, j))
							{
								GUILayout.Space(-3f);
								GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
								{
									GUILayout.Width(20f)
								});
							}
							if (EditorGUI.EndChangeCheck())
							{
								GUI.changed = true;
							}
						}
						else if (fieldType.Equals(typeof(SharedVariable)) || fieldType.IsSubclassOf(typeof(SharedVariable)))
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							EditorGUI.BeginChangeCheck();
							if (drawWatch)
							{
								this.DrawWatchedButton(task, fields[j]);
							}
							SharedVariable value2 = this.DrawSharedVariableValue(behaviorSource, fields[j], guiContent, task, value as SharedVariable, flag, enabled, drawWatch);
							if (BehaviorDesignerWindow.instance.ContainsError(task, j))
							{
								GUILayout.Space(-3f);
								GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
								{
									GUILayout.Width(20f)
								});
							}
							GUILayout.EndHorizontal();
							GUILayout.Space(4f);
							if (EditorGUI.EndChangeCheck())
							{
								fields[j].SetValue(obj, value2);
								GUI.changed = true;
							}
						}
						else
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							EditorGUI.BeginChangeCheck();
							if (drawWatch)
							{
								this.DrawWatchedButton(task, fields[j]);
							}
							object value3 = FieldInspector.DrawField(task, guiContent, fields[j], value);
							if (BehaviorDesignerWindow.instance.ContainsError(task, j))
							{
								GUILayout.Space(-3f);
								GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
								{
									GUILayout.Width(20f)
								});
							}
							if (EditorGUI.EndChangeCheck())
							{
								fields[j].SetValue(obj, value3);
								GUI.changed = true;
							}
							if (TaskUtility.HasAttribute(fields[j], typeof(RequiredFieldAttribute)) && !ErrorCheck.IsRequiredFieldValid(fieldType, value))
							{
								GUILayout.Space(-3f);
								GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
								{
									GUILayout.Width(20f)
								});
							}
							GUILayout.EndHorizontal();
							GUILayout.Space(4f);
						}
					}
				}
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00016F90 File Offset: 0x00015190
		private bool DrawWatchedButton(Task task, FieldInfo field)
		{
			GUILayout.Space(3f);
			bool flag = task.NodeData.ContainsWatchedField(field);
			if (GUILayout.Button((!flag) ? BehaviorDesignerUtility.VariableWatchButtonTexture : BehaviorDesignerUtility.VariableWatchButtonSelectedTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(15f)
			}))
			{
				if (flag)
				{
					task.NodeData.RemoveWatchedField(field);
				}
				else
				{
					task.NodeData.AddWatchedField(field);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00017014 File Offset: 0x00015214
		private void DrawTaskValue(BehaviorSource behaviorSource, TaskList taskList, FieldInfo field, GUIContent guiContent, Task parentTask, Task task, bool enabled)
		{
			if (BehaviorDesignerUtility.HasAttribute(field, typeof(InspectTaskAttribute)))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(guiContent, new GUILayoutOption[]
				{
					GUILayout.Width(144f)
				});
				if (GUILayout.Button((task == null) ? "Select" : BehaviorDesignerUtility.SplitCamelCase(task.GetType().Name.ToString()), EditorStyles.toolbarPopup, new GUILayoutOption[]
				{
					GUILayout.Width(134f)
				}))
				{
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("None"), task == null, new GenericMenu.MenuFunction2(this.InspectedTaskCallback), null);
					taskList.AddConditionalTasksToMenu(ref genericMenu, (task == null) ? null : task.GetType(), string.Empty, new GenericMenu.MenuFunction2(this.InspectedTaskCallback));
					genericMenu.ShowAsContext();
					this.mActiveMenuSelectionTask = parentTask;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(2f);
				this.DrawObjectFields(behaviorSource, taskList, task, task, enabled, false);
			}
			else
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.DrawWatchedButton(parentTask, field);
				GUILayout.Label(guiContent, BehaviorDesignerUtility.TaskInspectorGUIStyle, new GUILayoutOption[]
				{
					GUILayout.Width(165f)
				});
				bool flag = this.behaviorDesignerWindow.IsReferencingField(field);
				Color backgroundColor = GUI.backgroundColor;
				if (flag)
				{
					GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
				}
				if (GUILayout.Button((!flag) ? "Select" : "Done", EditorStyles.miniButtonMid, new GUILayoutOption[]
				{
					GUILayout.Width(80f)
				}))
				{
					if (this.behaviorDesignerWindow.IsReferencingTasks() && !flag)
					{
						this.behaviorDesignerWindow.ToggleReferenceTasks();
					}
					this.behaviorDesignerWindow.ToggleReferenceTasks(parentTask, field);
				}
				GUI.backgroundColor = backgroundColor;
				EditorGUILayout.EndHorizontal();
				if (typeof(IList).IsAssignableFrom(field.FieldType))
				{
					IList list = field.GetValue(parentTask) as IList;
					if (list == null || list.Count == 0)
					{
						GUILayout.Label("No Tasks Referenced", BehaviorDesignerUtility.TaskInspectorGUIStyle, new GUILayoutOption[0]);
					}
					else
					{
						for (int i = 0; i < list.Count; i++)
						{
							EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
							GUILayout.Label((list[i] as Task).NodeData.NodeDesigner.ToString(), BehaviorDesignerUtility.TaskInspectorGUIStyle, new GUILayoutOption[]
							{
								GUILayout.Width(232f)
							});
							if (GUILayout.Button(BehaviorDesignerUtility.DeleteButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
							{
								GUILayout.Width(14f)
							}))
							{
								this.ReferenceTasks(parentTask, ((list[i] as Task).NodeData.NodeDesigner as NodeDesigner).Task, field);
								GUI.changed = true;
							}
							GUILayout.Space(3f);
							if (GUILayout.Button(BehaviorDesignerUtility.IdentifyButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
							{
								GUILayout.Width(14f)
							}))
							{
								this.behaviorDesignerWindow.IdentifyNode((list[i] as Task).NodeData.NodeDesigner as NodeDesigner);
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				else
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					Task task2 = field.GetValue(parentTask) as Task;
					GUILayout.Label((task2 == null) ? "No Tasks Referenced" : task2.NodeData.NodeDesigner.ToString(), BehaviorDesignerUtility.TaskInspectorGUIStyle, new GUILayoutOption[]
					{
						GUILayout.Width(232f)
					});
					if (task2 != null)
					{
						if (GUILayout.Button(BehaviorDesignerUtility.DeleteButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
						{
							GUILayout.Width(14f)
						}))
						{
							this.ReferenceTasks(task, (task2.NodeData.NodeDesigner as NodeDesigner).Task, field);
							GUI.changed = true;
						}
						GUILayout.Space(3f);
						if (GUILayout.Button(BehaviorDesignerUtility.IdentifyButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
						{
							GUILayout.Width(14f)
						}))
						{
							this.behaviorDesignerWindow.IdentifyNode(task2.NodeData.NodeDesigner as NodeDesigner);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00017470 File Offset: 0x00015670
		private SharedVariable DrawSharedVariableValue(BehaviorSource behaviorSource, FieldInfo field, GUIContent guiContent, Task task, SharedVariable sharedVariable, bool isReflectionTask, bool enabled, bool drawWatch)
		{
			if (isReflectionTask)
			{
				if (!field.FieldType.Equals(typeof(SharedVariable)) && sharedVariable == null)
				{
					sharedVariable = (Activator.CreateInstance(field.FieldType) as SharedVariable);
					if (TaskUtility.HasAttribute(field, typeof(RequiredFieldAttribute)) || TaskUtility.HasAttribute(field, typeof(SharedRequiredAttribute)))
					{
						sharedVariable.IsShared = true;
					}
					GUI.changed = true;
				}
				bool drawComponentField;
				if (sharedVariable.IsShared)
				{
					GUILayout.Label(guiContent, new GUILayoutOption[]
					{
						GUILayout.Width(126f)
					});
					string[] array = null;
					int num = -1;
					int num2 = FieldInspector.GetVariablesOfType(sharedVariable.GetType().GetProperty("Value").PropertyType, sharedVariable.IsGlobal, sharedVariable.Name, behaviorSource, out array, ref num, false);
					Color backgroundColor = GUI.backgroundColor;
					if (num2 == 0 && !TaskUtility.HasAttribute(field, typeof(SharedRequiredAttribute)))
					{
						GUI.backgroundColor = Color.red;
					}
					int num3 = num2;
					num2 = EditorGUILayout.Popup(num2, array, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
					GUI.backgroundColor = backgroundColor;
					if (num2 != num3)
					{
						if (num2 == 0)
						{
							if (field.FieldType.Equals(typeof(SharedVariable)))
							{
								sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(sharedVariable.GetType().GetProperty("Value").PropertyType)) as SharedVariable);
							}
							else
							{
								sharedVariable = (Activator.CreateInstance(field.FieldType) as SharedVariable);
							}
							sharedVariable.IsShared = true;
						}
						else if (num != -1 && num2 >= num)
						{
							sharedVariable = GlobalVariables.Instance.GetVariable(array[num2].Substring(8, array[num2].Length - 8));
						}
						else
						{
							sharedVariable = behaviorSource.GetVariable(array[num2]);
						}
					}
					GUILayout.Space(8f);
				}
				else if ((drawComponentField = field.Name.Equals("componentName")) || field.Name.Equals("methodName") || field.Name.Equals("fieldName") || field.Name.Equals("propertyName"))
				{
					this.DrawReflectionField(task, guiContent, drawComponentField, field);
				}
				else
				{
					FieldInspector.DrawFields(task, sharedVariable, guiContent);
				}
				if (!TaskUtility.HasAttribute(field, typeof(RequiredFieldAttribute)) && !TaskUtility.HasAttribute(field, typeof(SharedRequiredAttribute)))
				{
					sharedVariable = FieldInspector.DrawSharedVariableToggleSharedButton(sharedVariable);
				}
				else if (!sharedVariable.IsShared)
				{
					sharedVariable.IsShared = true;
				}
			}
			else
			{
				sharedVariable = FieldInspector.DrawSharedVariable(null, guiContent, field, field.FieldType, sharedVariable);
			}
			GUILayout.Space(8f);
			return sharedVariable;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00017734 File Offset: 0x00015934
		private void InspectedTaskCallback(object obj)
		{
			if (this.mActiveMenuSelectionTask != null)
			{
				FieldInfo field = this.mActiveMenuSelectionTask.GetType().GetField("conditionalTask");
				if (obj == null)
				{
					field.SetValue(this.mActiveMenuSelectionTask, null);
				}
				else
				{
					Type type = (Type)obj;
					Task task = Activator.CreateInstance(type, true) as Task;
					field.SetValue(this.mActiveMenuSelectionTask, task);
					FieldInfo[] allFields = TaskUtility.GetAllFields(type);
					for (int i = 0; i < allFields.Length; i++)
					{
						if (allFields[i].FieldType.IsSubclassOf(typeof(SharedVariable)) && !BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(HideInInspector)) && !BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(NonSerializedAttribute)) && ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(SerializeField))))
						{
							SharedVariable sharedVariable = Activator.CreateInstance(allFields[i].FieldType) as SharedVariable;
							sharedVariable.IsShared = false;
							allFields[i].SetValue(task, sharedVariable);
						}
					}
				}
			}
			BehaviorDesignerWindow.instance.SaveBehavior();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00017874 File Offset: 0x00015A74
		public void SetActiveReferencedTasks(Task referenceTask, FieldInfo fieldInfo)
		{
			this.activeReferenceTask = referenceTask;
			this.activeReferenceTaskFieldInfo = fieldInfo;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00017884 File Offset: 0x00015A84
		public bool ReferenceTasks(Task referenceTask)
		{
			return this.ReferenceTasks(this.activeReferenceTask, referenceTask, this.activeReferenceTaskFieldInfo);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0001789C File Offset: 0x00015A9C
		private bool ReferenceTasks(Task sourceTask, Task referenceTask, FieldInfo sourceFieldInfo)
		{
			bool flag = false;
			bool showReferenceIcon = false;
			if (TaskInspector.ReferenceTasks(sourceTask, referenceTask, sourceFieldInfo, ref flag, ref showReferenceIcon, true, false))
			{
				(referenceTask.NodeData.NodeDesigner as NodeDesigner).ShowReferenceIcon = showReferenceIcon;
				if (flag)
				{
					this.PerformFullSync(this.activeReferenceTask);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000178EC File Offset: 0x00015AEC
		public static bool ReferenceTasks(Task sourceTask, Task referenceTask, FieldInfo sourceFieldInfo, ref bool fullSync, ref bool doReference, bool synchronize, bool unreferenceAll)
		{
			if (referenceTask == null || referenceTask.Equals(sourceTask) || (!typeof(IList).IsAssignableFrom(sourceFieldInfo.FieldType) && !referenceTask.GetType().IsAssignableFrom(sourceFieldInfo.FieldType)) || (typeof(IList).IsAssignableFrom(sourceFieldInfo.FieldType) && ((sourceFieldInfo.FieldType.IsGenericType && !referenceTask.GetType().IsAssignableFrom(sourceFieldInfo.FieldType.GetGenericArguments()[0])) || (!sourceFieldInfo.FieldType.IsGenericType && !referenceTask.GetType().IsAssignableFrom(sourceFieldInfo.FieldType.GetElementType())))))
			{
				return false;
			}
			if (synchronize && !TaskInspector.IsFieldLinked(sourceFieldInfo))
			{
				synchronize = false;
			}
			if (unreferenceAll)
			{
				sourceFieldInfo.SetValue(sourceTask, null);
				(sourceTask.NodeData.NodeDesigner as NodeDesigner).ShowReferenceIcon = false;
			}
			else
			{
				doReference = true;
				bool flag = false;
				if (typeof(IList).IsAssignableFrom(sourceFieldInfo.FieldType))
				{
					Task[] array = sourceFieldInfo.GetValue(sourceTask) as Task[];
					Type type;
					if (sourceFieldInfo.FieldType.IsArray)
					{
						type = sourceFieldInfo.FieldType.GetElementType();
					}
					else
					{
						Type type2 = sourceFieldInfo.FieldType;
						while (!type2.IsGenericType)
						{
							type2 = type2.BaseType;
						}
						type = type2.GetGenericArguments()[0];
					}
					IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
					{
						type
					})) as IList;
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (referenceTask.Equals(array[i]))
							{
								doReference = false;
							}
							else
							{
								list.Add(array[i]);
							}
						}
					}
					if (synchronize)
					{
						if (array != null && array.Length > 0)
						{
							for (int j = 0; j < array.Length; j++)
							{
								TaskInspector.ReferenceTasks(array[j], referenceTask, array[j].GetType().GetField(sourceFieldInfo.Name), ref flag, ref doReference, false, false);
								if (doReference)
								{
									TaskInspector.ReferenceTasks(referenceTask, array[j], referenceTask.GetType().GetField(sourceFieldInfo.Name), ref flag, ref doReference, false, false);
								}
							}
						}
						else if (doReference)
						{
							array = (referenceTask.GetType().GetField(sourceFieldInfo.Name).GetValue(referenceTask) as Task[]);
							if (array != null)
							{
								for (int k = 0; k < array.Length; k++)
								{
									list.Add(array[k]);
									(array[k].NodeData.NodeDesigner as NodeDesigner).ShowReferenceIcon = true;
									TaskInspector.ReferenceTasks(array[k], sourceTask, array[k].GetType().GetField(sourceFieldInfo.Name), ref doReference, ref flag, false, false);
								}
								doReference = true;
							}
						}
						TaskInspector.ReferenceTasks(referenceTask, sourceTask, referenceTask.GetType().GetField(sourceFieldInfo.Name), ref flag, ref doReference, false, !doReference);
					}
					if (doReference)
					{
						list.Add(referenceTask);
					}
					if (sourceFieldInfo.FieldType.IsArray)
					{
						Array array2 = Array.CreateInstance(sourceFieldInfo.FieldType.GetElementType(), list.Count);
						list.CopyTo(array2, 0);
						sourceFieldInfo.SetValue(sourceTask, array2);
					}
					else
					{
						sourceFieldInfo.SetValue(sourceTask, list);
					}
				}
				else
				{
					Task task = sourceFieldInfo.GetValue(sourceTask) as Task;
					doReference = !referenceTask.Equals(task);
					if (TaskInspector.IsFieldLinked(sourceFieldInfo) && task != null)
					{
						TaskInspector.ReferenceTasks(task, sourceTask, task.GetType().GetField(sourceFieldInfo.Name), ref flag, ref doReference, false, true);
					}
					if (synchronize)
					{
						TaskInspector.ReferenceTasks(referenceTask, sourceTask, referenceTask.GetType().GetField(sourceFieldInfo.Name), ref flag, ref doReference, false, !doReference);
					}
					sourceFieldInfo.SetValue(sourceTask, (!doReference) ? null : referenceTask);
				}
				if (synchronize)
				{
					(referenceTask.NodeData.NodeDesigner as NodeDesigner).ShowReferenceIcon = doReference;
				}
				fullSync = (doReference && synchronize);
			}
			return true;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00017D20 File Offset: 0x00015F20
		public bool IsActiveTaskArray()
		{
			return this.activeReferenceTaskFieldInfo.FieldType.IsArray;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00017D34 File Offset: 0x00015F34
		public bool IsActiveTaskNull()
		{
			return this.activeReferenceTaskFieldInfo.GetValue(this.activeReferenceTask) == null;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00017D4C File Offset: 0x00015F4C
		public static bool IsFieldLinked(FieldInfo field)
		{
			return BehaviorDesignerUtility.HasAttribute(field, typeof(LinkedTaskAttribute));
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00017D60 File Offset: 0x00015F60
		public static List<Task> GetReferencedTasks(Task task)
		{
			List<Task> list = new List<Task>();
			FieldInfo[] allFields = TaskUtility.GetAllFields(task.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(SerializeField)))
				{
					if (typeof(IList).IsAssignableFrom(allFields[i].FieldType) && (typeof(Task).IsAssignableFrom(allFields[i].FieldType.GetElementType()) || (allFields[i].FieldType.IsGenericType && typeof(Task).IsAssignableFrom(allFields[i].FieldType.GetGenericArguments()[0]))))
					{
						Task[] array = allFields[i].GetValue(task) as Task[];
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								list.Add(array[j]);
							}
						}
					}
					else if (allFields[i].FieldType.IsSubclassOf(typeof(Task)) && allFields[i].GetValue(task) != null)
					{
						list.Add(allFields[i].GetValue(task) as Task);
					}
				}
			}
			return (list.Count <= 0) ? null : list;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00017EC8 File Offset: 0x000160C8
		private void PerformFullSync(Task task)
		{
			List<Task> referencedTasks = TaskInspector.GetReferencedTasks(task);
			if (referencedTasks != null)
			{
				FieldInfo[] allFields = TaskUtility.GetAllFields(task.GetType());
				for (int i = 0; i < allFields.Length; i++)
				{
					if (!TaskInspector.IsFieldLinked(allFields[i]))
					{
						for (int j = 0; j < referencedTasks.Count; j++)
						{
							FieldInfo field;
							if ((field = referencedTasks[j].GetType().GetField(allFields[i].Name)) != null)
							{
								field.SetValue(referencedTasks[j], allFields[i].GetValue(task));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00017F64 File Offset: 0x00016164
		public static void OpenInFileEditor(object task)
		{
			MonoScript[] array = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].GetClass() != null && array[i].GetClass().Equals(task.GetType()))
				{
					AssetDatabase.OpenAsset(array[i]);
					break;
				}
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00017FDC File Offset: 0x000161DC
		public static void SelectInProject(object task)
		{
			MonoScript[] array = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].GetClass() != null && array[i].GetClass().Equals(task.GetType()))
				{
					Selection.activeObject = array[i];
					break;
				}
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00018054 File Offset: 0x00016254
		private void ResetTask(object task)
		{
			(task as Task).OnReset();
			List<Type> baseClasses = FieldInspector.GetBaseClasses(task.GetType());
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			for (int i = baseClasses.Count - 1; i > -1; i--)
			{
				FieldInfo[] fields = baseClasses[i].GetFields(bindingAttr);
				for (int j = 0; j < fields.Length; j++)
				{
					if (typeof(SharedVariable).IsAssignableFrom(fields[j].FieldType))
					{
						SharedVariable sharedVariable = fields[j].GetValue(task) as SharedVariable;
						if (TaskUtility.HasAttribute(fields[j], typeof(RequiredFieldAttribute)) && sharedVariable != null && !sharedVariable.IsShared)
						{
							sharedVariable.IsShared = true;
						}
					}
				}
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0001811C File Offset: 0x0001631C
		private void AddColorMenuItem(ref GenericMenu menu, Task task, string color, int index)
		{
			menu.AddItem(new GUIContent(color), task.NodeData.ColorIndex == index, new GenericMenu.MenuFunction2(this.SetTaskColor), new TaskInspector.TaskColor(task, index));
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0001815C File Offset: 0x0001635C
		private void SetTaskColor(object value)
		{
			TaskInspector.TaskColor taskColor = value as TaskInspector.TaskColor;
			if (taskColor.task.NodeData.ColorIndex != taskColor.colorIndex)
			{
				taskColor.task.NodeData.ColorIndex = taskColor.colorIndex;
				BehaviorDesignerWindow.instance.SaveBehavior();
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000181AC File Offset: 0x000163AC
		private void OpenHelpURL(Task task)
		{
			HelpURLAttribute[] array;
			if ((array = (task.GetType().GetCustomAttributes(typeof(HelpURLAttribute), false) as HelpURLAttribute[])).Length > 0)
			{
				Application.OpenURL(array[0].URL);
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000181F0 File Offset: 0x000163F0
		private bool IsReflectionTask(Type type)
		{
			return this.IsInvokeMethodTask(type) || this.IsFieldReflectionTask(type) || this.IsPropertyReflectionTask(type);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00018220 File Offset: 0x00016420
		private bool IsInvokeMethodTask(Type type)
		{
			return TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.InvokeMethod");
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00018230 File Offset: 0x00016430
		private bool IsFieldReflectionTask(Type type)
		{
			return TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.GetFieldValue") || TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.SetFieldValue") || TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.CompareFieldValue");
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0001826C File Offset: 0x0001646C
		private bool IsPropertyReflectionTask(Type type)
		{
			return TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.GetPropertyValue") || TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.SetPropertyValue") || TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.ComparePropertyValue");
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000182A8 File Offset: 0x000164A8
		private bool IsReflectionGetterTask(Type type)
		{
			return TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.GetFieldValue") || TaskUtility.CompareType(type, "BehaviorDesigner.Runtime.Tasks.GetPropertyValue");
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000182C8 File Offset: 0x000164C8
		private void DrawReflectionField(Task task, GUIContent guiContent, bool drawComponentField, FieldInfo field)
		{
			FieldInfo field2 = task.GetType().GetField("targetGameObject");
			SharedVariable sharedVariable = field2.GetValue(task) as SharedVariable;
			if (drawComponentField)
			{
				GUILayout.Label(guiContent, new GUILayoutOption[]
				{
					GUILayout.Width(146f)
				});
				SharedVariable sharedVariable2 = field.GetValue(task) as SharedVariable;
				string text = string.Empty;
				if (string.IsNullOrEmpty((string)sharedVariable2.GetValue()))
				{
					text = "Select";
				}
				else
				{
					string text2 = (string)sharedVariable2.GetValue();
					string[] array = text2.Split(new char[]
					{
						'.'
					});
					text = array[array.Length - 1];
				}
				if (GUILayout.Button(text, EditorStyles.toolbarPopup, new GUILayoutOption[]
				{
					GUILayout.Width(92f)
				}))
				{
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("None"), string.IsNullOrEmpty((string)sharedVariable2.GetValue()), new GenericMenu.MenuFunction2(this.ComponentSelectionCallback), null);
					GameObject gameObject = null;
					if (sharedVariable == null || (GameObject)sharedVariable.GetValue() == null)
					{
						if (task.Owner != null)
						{
							gameObject = task.Owner.gameObject;
						}
					}
					else
					{
						gameObject = (GameObject)sharedVariable.GetValue();
					}
					if (gameObject != null)
					{
						Component[] components = gameObject.GetComponents<Component>();
						for (int i = 0; i < components.Length; i++)
						{
							genericMenu.AddItem(new GUIContent(components[i].GetType().Name), components[i].GetType().FullName.Equals((string)sharedVariable2.GetValue()), new GenericMenu.MenuFunction2(this.ComponentSelectionCallback), components[i].GetType().FullName);
						}
						genericMenu.ShowAsContext();
						this.mActiveMenuSelectionTask = task;
					}
				}
			}
			else
			{
				GUILayout.Label(guiContent, new GUILayoutOption[]
				{
					GUILayout.Width(146f)
				});
				FieldInfo field3 = task.GetType().GetField("componentName");
				SharedVariable sharedVariable3 = field3.GetValue(task) as SharedVariable;
				SharedVariable sharedVariable4 = field.GetValue(task) as SharedVariable;
				string text3 = string.Empty;
				if (string.IsNullOrEmpty((string)sharedVariable3.GetValue()))
				{
					text3 = "Component Required";
				}
				else if (string.IsNullOrEmpty((string)sharedVariable4.GetValue()))
				{
					text3 = "Select";
				}
				else
				{
					text3 = (string)sharedVariable4.GetValue();
				}
				if (GUILayout.Button(text3, EditorStyles.toolbarPopup, new GUILayoutOption[]
				{
					GUILayout.Width(92f)
				}) && !string.IsNullOrEmpty((string)sharedVariable3.GetValue()))
				{
					GenericMenu genericMenu2 = new GenericMenu();
					genericMenu2.AddItem(new GUIContent("None"), string.IsNullOrEmpty((string)sharedVariable4.GetValue()), new GenericMenu.MenuFunction2(this.SecondaryReflectionSelectionCallback), null);
					GameObject gameObject2 = null;
					if (sharedVariable == null || (GameObject)sharedVariable.GetValue() == null)
					{
						if (task.Owner != null)
						{
							gameObject2 = task.Owner.gameObject;
						}
					}
					else
					{
						gameObject2 = (GameObject)sharedVariable.GetValue();
					}
					if (gameObject2 != null)
					{
						Component component = gameObject2.GetComponent(TaskUtility.GetTypeWithinAssembly((string)sharedVariable3.GetValue()));
						List<Type> sharedVariableTypes = VariableInspector.FindAllSharedVariableTypes(false);
						if (this.IsInvokeMethodTask(task.GetType()))
						{
							MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
							for (int j = 0; j < methods.Length; j++)
							{
								if (!methods[j].IsSpecialName && !methods[j].IsGenericMethod && methods[j].GetParameters().Length <= 4)
								{
									ParameterInfo[] parameters = methods[j].GetParameters();
									bool flag = true;
									for (int k = 0; k < parameters.Length; k++)
									{
										if (!this.SharedVariableTypeExists(sharedVariableTypes, parameters[k].ParameterType))
										{
											flag = false;
											break;
										}
									}
									if (flag && (methods[j].ReturnType.Equals(typeof(void)) || this.SharedVariableTypeExists(sharedVariableTypes, methods[j].ReturnType)))
									{
										genericMenu2.AddItem(new GUIContent(methods[j].Name), methods[j].Name.Equals((string)sharedVariable4.GetValue()), new GenericMenu.MenuFunction2(this.SecondaryReflectionSelectionCallback), methods[j]);
									}
								}
							}
						}
						else if (this.IsFieldReflectionTask(task.GetType()))
						{
							FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
							for (int l = 0; l < fields.Length; l++)
							{
								if (!fields[l].IsSpecialName)
								{
									if (this.SharedVariableTypeExists(sharedVariableTypes, fields[l].FieldType))
									{
										genericMenu2.AddItem(new GUIContent(fields[l].Name), fields[l].Name.Equals((string)sharedVariable4.GetValue()), new GenericMenu.MenuFunction2(this.SecondaryReflectionSelectionCallback), fields[l]);
									}
								}
							}
						}
						else
						{
							PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
							for (int m = 0; m < properties.Length; m++)
							{
								if (!properties[m].IsSpecialName)
								{
									if (this.SharedVariableTypeExists(sharedVariableTypes, properties[m].PropertyType))
									{
										genericMenu2.AddItem(new GUIContent(properties[m].Name), properties[m].Name.Equals((string)sharedVariable4.GetValue()), new GenericMenu.MenuFunction2(this.SecondaryReflectionSelectionCallback), properties[m]);
									}
								}
							}
						}
						genericMenu2.ShowAsContext();
						this.mActiveMenuSelectionTask = task;
					}
				}
			}
			GUILayout.Space(8f);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000188D8 File Offset: 0x00016AD8
		private void ComponentSelectionCallback(object obj)
		{
			if (this.mActiveMenuSelectionTask != null)
			{
				FieldInfo field = this.mActiveMenuSelectionTask.GetType().GetField("componentName");
				SharedVariable sharedVariable = Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")) as SharedVariable;
				if (obj == null)
				{
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
					sharedVariable = (Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")) as SharedVariable);
					FieldInfo field2;
					if (this.IsInvokeMethodTask(this.mActiveMenuSelectionTask.GetType()))
					{
						field2 = this.mActiveMenuSelectionTask.GetType().GetField("methodName");
						this.ClearInvokeVariablesTask();
					}
					else if (this.IsFieldReflectionTask(this.mActiveMenuSelectionTask.GetType()))
					{
						field2 = this.mActiveMenuSelectionTask.GetType().GetField("fieldName");
					}
					else
					{
						field2 = this.mActiveMenuSelectionTask.GetType().GetField("propertyName");
					}
					field2.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
				}
				else
				{
					string text = (string)obj;
					SharedVariable sharedVariable2 = field.GetValue(this.mActiveMenuSelectionTask) as SharedVariable;
					if (!text.Equals((string)sharedVariable2.GetValue()))
					{
						FieldInfo field3;
						FieldInfo field5;
						if (this.IsInvokeMethodTask(this.mActiveMenuSelectionTask.GetType()))
						{
							field3 = this.mActiveMenuSelectionTask.GetType().GetField("methodName");
							for (int i = 0; i < 4; i++)
							{
								FieldInfo field4 = this.mActiveMenuSelectionTask.GetType().GetField("parameter" + (i + 1));
								field4.SetValue(this.mActiveMenuSelectionTask, null);
							}
							field5 = this.mActiveMenuSelectionTask.GetType().GetField("storeResult");
						}
						else if (this.IsFieldReflectionTask(this.mActiveMenuSelectionTask.GetType()))
						{
							field3 = this.mActiveMenuSelectionTask.GetType().GetField("fieldName");
							field5 = this.mActiveMenuSelectionTask.GetType().GetField("fieldValue");
							if (field5 == null)
							{
								field5 = this.mActiveMenuSelectionTask.GetType().GetField("compareValue");
							}
						}
						else
						{
							field3 = this.mActiveMenuSelectionTask.GetType().GetField("propertyName");
							field5 = this.mActiveMenuSelectionTask.GetType().GetField("propertyValue");
							if (field5 == null)
							{
								field5 = this.mActiveMenuSelectionTask.GetType().GetField("compareValue");
							}
						}
						field3.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
						field5.SetValue(this.mActiveMenuSelectionTask, null);
					}
					sharedVariable = (Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")) as SharedVariable);
					sharedVariable.SetValue(text);
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
				}
			}
			BehaviorDesignerWindow.instance.SaveBehavior();
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00018BA0 File Offset: 0x00016DA0
		private void SecondaryReflectionSelectionCallback(object obj)
		{
			if (this.mActiveMenuSelectionTask != null)
			{
				SharedVariable sharedVariable = Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")) as SharedVariable;
				FieldInfo field;
				if (this.IsInvokeMethodTask(this.mActiveMenuSelectionTask.GetType()))
				{
					this.ClearInvokeVariablesTask();
					field = this.mActiveMenuSelectionTask.GetType().GetField("methodName");
				}
				else if (this.IsFieldReflectionTask(this.mActiveMenuSelectionTask.GetType()))
				{
					field = this.mActiveMenuSelectionTask.GetType().GetField("fieldName");
				}
				else
				{
					field = this.mActiveMenuSelectionTask.GetType().GetField("propertyName");
				}
				if (obj == null)
				{
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
				}
				else if (this.IsInvokeMethodTask(this.mActiveMenuSelectionTask.GetType()))
				{
					MethodInfo methodInfo = (MethodInfo)obj;
					sharedVariable.SetValue(methodInfo.Name);
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
					ParameterInfo[] parameters = methodInfo.GetParameters();
					for (int i = 0; i < 4; i++)
					{
						FieldInfo field2 = this.mActiveMenuSelectionTask.GetType().GetField("parameter" + (i + 1));
						if (i < parameters.Length)
						{
							sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(parameters[i].ParameterType)) as SharedVariable);
							field2.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
						}
						else
						{
							field2.SetValue(this.mActiveMenuSelectionTask, null);
						}
					}
					if (!methodInfo.ReturnType.Equals(typeof(void)))
					{
						FieldInfo field3 = this.mActiveMenuSelectionTask.GetType().GetField("storeResult");
						sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(methodInfo.ReturnType)) as SharedVariable);
						sharedVariable.IsShared = true;
						field3.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
					}
				}
				else if (this.IsFieldReflectionTask(this.mActiveMenuSelectionTask.GetType()))
				{
					FieldInfo fieldInfo = (FieldInfo)obj;
					sharedVariable.SetValue(fieldInfo.Name);
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
					FieldInfo field4 = this.mActiveMenuSelectionTask.GetType().GetField("fieldValue");
					if (field4 == null)
					{
						field4 = this.mActiveMenuSelectionTask.GetType().GetField("compareValue");
					}
					sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(fieldInfo.FieldType)) as SharedVariable);
					sharedVariable.IsShared = this.IsReflectionGetterTask(this.mActiveMenuSelectionTask.GetType());
					field4.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
				}
				else
				{
					PropertyInfo propertyInfo = (PropertyInfo)obj;
					sharedVariable.SetValue(propertyInfo.Name);
					field.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
					FieldInfo field5 = this.mActiveMenuSelectionTask.GetType().GetField("propertyValue");
					if (field5 == null)
					{
						field5 = this.mActiveMenuSelectionTask.GetType().GetField("compareValue");
					}
					sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(propertyInfo.PropertyType)) as SharedVariable);
					sharedVariable.IsShared = this.IsReflectionGetterTask(this.mActiveMenuSelectionTask.GetType());
					field5.SetValue(this.mActiveMenuSelectionTask, sharedVariable);
				}
			}
			BehaviorDesignerWindow.instance.SaveBehavior();
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00018ED0 File Offset: 0x000170D0
		private void ClearInvokeVariablesTask()
		{
			for (int i = 0; i < 4; i++)
			{
				FieldInfo field = this.mActiveMenuSelectionTask.GetType().GetField("parameter" + (i + 1));
				field.SetValue(this.mActiveMenuSelectionTask, null);
			}
			FieldInfo field2 = this.mActiveMenuSelectionTask.GetType().GetField("storeResult");
			field2.SetValue(this.mActiveMenuSelectionTask, null);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00018F44 File Offset: 0x00017144
		private bool CanDrawReflectedField(object task, FieldInfo field)
		{
			if (!field.Name.Contains("parameter") && !field.Name.Contains("storeResult") && !field.Name.Contains("fieldValue") && !field.Name.Contains("propertyValue") && !field.Name.Contains("compareValue"))
			{
				return true;
			}
			if (this.IsInvokeMethodTask(task.GetType()))
			{
				if (field.Name.Contains("parameter"))
				{
					FieldInfo field2 = task.GetType().GetField(field.Name);
					return field2.GetValue(task) != null;
				}
				MethodInfo invokeMethodInfo;
				return (invokeMethodInfo = this.GetInvokeMethodInfo(task)) != null && (!field.Name.Equals("storeResult") || !invokeMethodInfo.ReturnType.Equals(typeof(void)));
			}
			else
			{
				if (this.IsFieldReflectionTask(task.GetType()))
				{
					FieldInfo field3 = task.GetType().GetField("fieldName");
					SharedVariable sharedVariable = field3.GetValue(task) as SharedVariable;
					return sharedVariable != null && !string.IsNullOrEmpty((string)sharedVariable.GetValue());
				}
				FieldInfo field4 = task.GetType().GetField("propertyName");
				SharedVariable sharedVariable2 = field4.GetValue(task) as SharedVariable;
				return sharedVariable2 != null && !string.IsNullOrEmpty((string)sharedVariable2.GetValue());
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000190D4 File Offset: 0x000172D4
		private string InvokeParameterName(object task, FieldInfo field)
		{
			if (!field.Name.Contains("parameter"))
			{
				return field.Name;
			}
			MethodInfo invokeMethodInfo;
			if ((invokeMethodInfo = this.GetInvokeMethodInfo(task)) == null)
			{
				return field.Name;
			}
			ParameterInfo[] parameters = invokeMethodInfo.GetParameters();
			int num = int.Parse(field.Name.Substring(9)) - 1;
			if (num < parameters.Length)
			{
				return parameters[num].Name;
			}
			return field.Name;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00019148 File Offset: 0x00017348
		private MethodInfo GetInvokeMethodInfo(object task)
		{
			FieldInfo field = task.GetType().GetField("targetGameObject");
			SharedVariable sharedVariable = field.GetValue(task) as SharedVariable;
			GameObject gameObject = null;
			if (sharedVariable == null || (GameObject)sharedVariable.GetValue() == null)
			{
				if ((task as Task).Owner != null)
				{
					gameObject = (task as Task).Owner.gameObject;
				}
			}
			else
			{
				gameObject = (GameObject)sharedVariable.GetValue();
			}
			if (gameObject == null)
			{
				return null;
			}
			FieldInfo field2 = task.GetType().GetField("componentName");
			SharedVariable sharedVariable2 = field2.GetValue(task) as SharedVariable;
			if (sharedVariable2 == null || string.IsNullOrEmpty((string)sharedVariable2.GetValue()))
			{
				return null;
			}
			FieldInfo field3 = task.GetType().GetField("methodName");
			SharedVariable sharedVariable3 = field3.GetValue(task) as SharedVariable;
			if (sharedVariable3 == null || string.IsNullOrEmpty((string)sharedVariable3.GetValue()))
			{
				return null;
			}
			List<Type> list = new List<Type>();
			for (int i = 0; i < 4; i++)
			{
				FieldInfo field4 = task.GetType().GetField("parameter" + (i + 1));
				SharedVariable sharedVariable4;
				if ((sharedVariable4 = (field4.GetValue(task) as SharedVariable)) == null)
				{
					break;
				}
				list.Add(sharedVariable4.GetType().GetProperty("Value").PropertyType);
			}
			Component component = gameObject.GetComponent(TaskUtility.GetTypeWithinAssembly((string)sharedVariable2.GetValue()));
			return component.GetType().GetMethod((string)sharedVariable3.GetValue(), list.ToArray());
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0001930C File Offset: 0x0001750C
		private bool SharedVariableTypeExists(List<Type> sharedVariableTypes, Type type)
		{
			for (int i = 0; i < sharedVariableTypes.Count; i++)
			{
				if (FieldInspector.FriendlySharedVariableName(type).Equals(sharedVariableTypes[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400017C RID: 380
		private BehaviorDesignerWindow behaviorDesignerWindow;

		// Token: 0x0400017D RID: 381
		private Task activeReferenceTask;

		// Token: 0x0400017E RID: 382
		private FieldInfo activeReferenceTaskFieldInfo;

		// Token: 0x0400017F RID: 383
		private Task mActiveMenuSelectionTask;

		// Token: 0x04000180 RID: 384
		private Vector2 mScrollPosition = Vector2.zero;

		// Token: 0x02000026 RID: 38
		private class TaskColor
		{
			// Token: 0x06000286 RID: 646 RVA: 0x0001934C File Offset: 0x0001754C
			public TaskColor(Task task, int colorIndex)
			{
				this.task = task;
				this.colorIndex = colorIndex;
			}

			// Token: 0x04000181 RID: 385
			public Task task;

			// Token: 0x04000182 RID: 386
			public int colorIndex;
		}
	}
}
