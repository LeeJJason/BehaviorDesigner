using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200002C RID: 44
	public class VariableInspector : ScriptableObject
	{
		// Token: 0x060002B0 RID: 688 RVA: 0x0001A740 File Offset: 0x00018940
		public void ResetSelectedVariableIndex()
		{
			this.mSelectedVariableIndex = -1;
			this.mVariableStartPosition = -1f;
			if (this.mVariablePosition != null)
			{
				this.mVariablePosition.Clear();
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0001A778 File Offset: 0x00018978
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0001A784 File Offset: 0x00018984
		public static List<Type> FindAllSharedVariableTypes(bool removeShared)
		{
			if (VariableInspector.sharedVariableTypes != null)
			{
				return VariableInspector.sharedVariableTypes;
			}
			VariableInspector.sharedVariableTypes = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					if (types[j].IsSubclassOf(typeof(SharedVariable)) && !types[j].IsAbstract)
					{
						VariableInspector.sharedVariableTypes.Add(types[j]);
					}
				}
			}
			VariableInspector.sharedVariableTypes.Sort(new AlphanumComparator<Type>());
			VariableInspector.sharedVariableStrings = new string[VariableInspector.sharedVariableTypes.Count];
			VariableInspector.sharedVariableTypesDict = new Dictionary<string, int>();
			for (int k = 0; k < VariableInspector.sharedVariableTypes.Count; k++)
			{
				string text = VariableInspector.sharedVariableTypes[k].Name;
				VariableInspector.sharedVariableTypesDict.Add(text, k);
				if (removeShared && text.Length > 6 && text.Substring(0, 6).Equals("Shared"))
				{
					text = text.Substring(6, text.Length - 6);
				}
				VariableInspector.sharedVariableStrings[k] = text;
			}
			return VariableInspector.sharedVariableTypes;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0001A8D4 File Offset: 0x00018AD4
		public bool ClearFocus(bool addVariable, BehaviorSource behaviorSource)
		{
			GUIUtility.keyboardControl = 0;
			bool result = false;
			if (addVariable && !string.IsNullOrEmpty(this.mVariableName) && VariableInspector.VariableNameValid(behaviorSource, this.mVariableName))
			{
				result = VariableInspector.AddVariable(behaviorSource, this.mVariableName, this.mVariableTypeIndex, false);
				this.mVariableName = string.Empty;
			}
			return result;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0001A930 File Offset: 0x00018B30
		public bool HasFocus()
		{
			return GUIUtility.keyboardControl != 0;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0001A940 File Offset: 0x00018B40
		public void FocusNameField()
		{
			this.mFocusNameField = true;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0001A94C File Offset: 0x00018B4C
		public bool LeftMouseDown(IVariableSource variableSource, BehaviorSource behaviorSource, Vector2 mousePosition)
		{
			return VariableInspector.LeftMouseDown(variableSource, behaviorSource, mousePosition, this.mVariablePosition, this.mVariableStartPosition, this.mScrollPosition, ref this.mSelectedVariableIndex, ref this.mSelectedVariableName, ref this.mSelectedVariableTypeIndex);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0001A988 File Offset: 0x00018B88
		public static bool LeftMouseDown(IVariableSource variableSource, BehaviorSource behaviorSource, Vector2 mousePosition, List<float> variablePosition, float variableStartPosition, Vector2 scrollPosition, ref int selectedVariableIndex, ref string selectedVariableName, ref int selectedVariableTypeIndex)
		{
			if (variablePosition != null && mousePosition.y > variableStartPosition && variableSource != null)
			{
				List<SharedVariable> allVariables;
				if (!Application.isPlaying && behaviorSource != null && behaviorSource.Owner is Behavior)
				{
					Behavior behavior = behaviorSource.Owner as Behavior;
					if (behavior.ExternalBehavior != null)
					{
						BehaviorSource behaviorSource2 = behavior.GetBehaviorSource();
						behaviorSource2.CheckForSerialization(true, null);
						allVariables = behaviorSource2.GetAllVariables();
						ExternalBehavior externalBehavior = behavior.ExternalBehavior;
						externalBehavior.BehaviorSource.Owner = externalBehavior;
						externalBehavior.BehaviorSource.CheckForSerialization(true, behaviorSource);
					}
					else
					{
						allVariables = variableSource.GetAllVariables();
					}
				}
				else
				{
					allVariables = variableSource.GetAllVariables();
				}
				if (allVariables == null || allVariables.Count != variablePosition.Count)
				{
					return false;
				}
				int i = 0;
				while (i < variablePosition.Count)
				{
					if (mousePosition.y < variablePosition[i] - scrollPosition.y)
					{
						if (i == selectedVariableIndex)
						{
							return false;
						}
						selectedVariableIndex = i;
						selectedVariableName = allVariables[i].Name;
						selectedVariableTypeIndex = VariableInspector.sharedVariableTypesDict[allVariables[i].GetType().Name];
						return true;
					}
					else
					{
						i++;
					}
				}
			}
			if (selectedVariableIndex != -1)
			{
				selectedVariableIndex = -1;
				return true;
			}
			return false;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0001AAE0 File Offset: 0x00018CE0
		public bool DrawVariables(BehaviorSource behaviorSource)
		{
			return VariableInspector.DrawVariables(behaviorSource, behaviorSource, ref this.mVariableName, ref this.mFocusNameField, ref this.mVariableTypeIndex, ref this.mScrollPosition, ref this.mVariablePosition, ref this.mVariableStartPosition, ref this.mSelectedVariableIndex, ref this.mSelectedVariableName, ref this.mSelectedVariableTypeIndex);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001AB2C File Offset: 0x00018D2C
		public static bool DrawVariables(IVariableSource variableSource, BehaviorSource behaviorSource, ref string variableName, ref bool focusNameField, ref int variableTypeIndex, ref Vector2 scrollPosition, ref List<float> variablePosition, ref float variableStartPosition, ref int selectedVariableIndex, ref string selectedVariableName, ref int selectedVariableTypeIndex)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[0]);
			bool flag = false;
			bool flag2 = false;
			List<SharedVariable> list = (variableSource == null) ? null : variableSource.GetAllVariables();
			if (VariableInspector.DrawHeader(variableSource, behaviorSource == null, ref variableStartPosition, ref variableName, ref focusNameField, ref variableTypeIndex, ref selectedVariableIndex, ref selectedVariableName, ref selectedVariableTypeIndex))
			{
				flag = true;
			}
			list = ((variableSource == null) ? null : variableSource.GetAllVariables());
			if (list != null && list.Count > 0)
			{
				GUI.enabled = !flag2;
				if (VariableInspector.DrawAllVariables(true, variableSource, ref list, true, ref variablePosition, ref selectedVariableIndex, ref selectedVariableName, ref selectedVariableTypeIndex, true, true))
				{
					flag = true;
				}
			}
			if (flag && variableSource != null)
			{
				variableSource.SetAllVariables(list);
			}
			GUI.enabled = true;
			GUILayout.EndScrollView();
			if (flag && !EditorApplication.isPlayingOrWillChangePlaymode && behaviorSource != null && behaviorSource.Owner is Behavior)
			{
				Behavior behavior = behaviorSource.Owner as Behavior;
				if (behavior.ExternalBehavior != null)
				{
					if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
					{
						BinarySerialization.Save(behaviorSource);
					}
					else
					{
						JSONSerialization.Save(behaviorSource);
					}
					BehaviorSource behaviorSource2 = behavior.ExternalBehavior.GetBehaviorSource();
					behaviorSource2.CheckForSerialization(true, null);
					VariableInspector.SyncVariables(behaviorSource2, list);
				}
			}
			return flag;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001AC70 File Offset: 0x00018E70
		public static bool SyncVariables(BehaviorSource localBehaviorSource, List<SharedVariable> variables)
		{
			List<SharedVariable> list = localBehaviorSource.GetAllVariables();
			if (variables != null)
			{
				bool result = false;
				if (list == null)
				{
					list = new List<SharedVariable>();
					localBehaviorSource.SetAllVariables(list);
					result = true;
				}
				for (int i = 0; i < variables.Count; i++)
				{
					if (list.Count - 1 < i)
					{
						SharedVariable sharedVariable = Activator.CreateInstance(variables[i].GetType()) as SharedVariable;
						sharedVariable.Name = variables[i].Name;
						sharedVariable.IsShared = true;
						sharedVariable.SetValue(variables[i].GetValue());
						list.Add(sharedVariable);
						result = true;
					}
					else if (list[i].Name != variables[i].Name || list[i].GetType() != variables[i].GetType())
					{
						SharedVariable sharedVariable2 = Activator.CreateInstance(variables[i].GetType()) as SharedVariable;
						sharedVariable2.Name = variables[i].Name;
						sharedVariable2.IsShared = true;
						sharedVariable2.SetValue(variables[i].GetValue());
						list[i] = sharedVariable2;
						result = true;
					}
				}
				for (int j = list.Count - 1; j > variables.Count - 1; j--)
				{
					list.RemoveAt(j);
					result = true;
				}
				return result;
			}
			if (list != null && list.Count > 0)
			{
				list.Clear();
				return true;
			}
			return false;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0001ADF4 File Offset: 0x00018FF4
		private static bool DrawHeader(IVariableSource variableSource, bool fromGlobalVariablesWindow, ref float variableStartPosition, ref string variableName, ref bool focusNameField, ref int variableTypeIndex, ref int selectedVariableIndex, ref string selectedVariableName, ref int selectedVariableTypeIndex)
		{
			if (VariableInspector.sharedVariableStrings == null)
			{
				VariableInspector.FindAllSharedVariableTypes(true);
			}
			EditorGUIUtility.labelWidth = 150f;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(4f);
			EditorGUILayout.LabelField("Name", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			GUI.SetNextControlName("Name");
			variableName = EditorGUILayout.TextField(variableName, new GUILayoutOption[]
			{
				GUILayout.Width(212f)
			});
			if (focusNameField)
			{
				GUI.FocusControl("Name");
				focusNameField = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(4f);
			GUILayout.Label("Type", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			variableTypeIndex = EditorGUILayout.Popup(variableTypeIndex, VariableInspector.sharedVariableStrings, EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(163f)
			});
			GUILayout.Space(8f);
			bool flag = false;
			bool flag2 = VariableInspector.VariableNameValid(variableSource, variableName);
			bool enabled = GUI.enabled;
			GUI.enabled = (flag2 && enabled);
			GUI.SetNextControlName("Add");
			if (GUILayout.Button("Add", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			}) && flag2)
			{
				if (fromGlobalVariablesWindow && variableSource == null)
				{
					GlobalVariables globalVariables = ScriptableObject.CreateInstance(typeof(GlobalVariables)) as GlobalVariables;
					string text = BehaviorDesignerUtility.GetEditorBaseDirectory(null).Substring(6, BehaviorDesignerUtility.GetEditorBaseDirectory(null).Length - 13);
					string str = text + "/Resources/BehaviorDesignerGlobalVariables.asset";
					if (!Directory.Exists(Application.dataPath + text + "/Resources"))
					{
						Directory.CreateDirectory(Application.dataPath + text + "/Resources");
					}
					if (!File.Exists(Application.dataPath + str))
					{
						AssetDatabase.CreateAsset(globalVariables, "Assets" + str);
						EditorUtility.DisplayDialog("Created Global Variables", "Behavior Designer Global Variables asset created:\n\nAssets" + text + "/Resources/BehaviorDesignerGlobalVariables.asset\n\nNote: Copy this file to transfer global variables between projects.", "OK");
					}
					variableSource = globalVariables;
				}
				flag = VariableInspector.AddVariable(variableSource, variableName, variableTypeIndex, fromGlobalVariablesWindow);
				if (flag)
				{
					selectedVariableIndex = variableSource.GetAllVariables().Count - 1;
					selectedVariableName = variableName;
					selectedVariableTypeIndex = variableTypeIndex;
					variableName = string.Empty;
				}
			}
			GUILayout.Space(6f);
			GUILayout.EndHorizontal();
			if (!fromGlobalVariablesWindow)
			{
				GUI.enabled = true;
				GUILayout.Space(3f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(5f);
				if (GUILayout.Button("Global Variables", EditorStyles.toolbarButton, new GUILayoutOption[]
				{
					GUILayout.Width(284f)
				}))
				{
					GlobalVariablesWindow.ShowWindow();
				}
				GUILayout.EndHorizontal();
			}
			BehaviorDesignerUtility.DrawContentSeperator(2);
			GUILayout.Space(4f);
			if (variableStartPosition == -1f && (int)Event.current.type == 7)
			{
				variableStartPosition = GUILayoutUtility.GetLastRect().yMax;
			}
			GUI.enabled = enabled;
			return flag;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0001B100 File Offset: 0x00019300
		private static bool AddVariable(IVariableSource variableSource, string variableName, int variableTypeIndex, bool fromGlobalVariablesWindow)
		{
			SharedVariable item = VariableInspector.CreateVariable(variableTypeIndex, variableName, fromGlobalVariablesWindow);
			List<SharedVariable> list = (variableSource == null) ? null : variableSource.GetAllVariables();
			if (list == null)
			{
				list = new List<SharedVariable>();
			}
			list.Add(item);
			variableSource.SetAllVariables(list);
			return true;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0001B144 File Offset: 0x00019344
		public static bool DrawAllVariables(bool showFooter, IVariableSource variableSource, ref List<SharedVariable> variables, bool canSelect, ref List<float> variablePosition, ref int selectedVariableIndex, ref string selectedVariableName, ref int selectedVariableTypeIndex, bool drawRemoveButton, bool drawLastSeparator)
		{
			if (variables == null)
			{
				return false;
			}
			bool result = false;
			if (canSelect && variablePosition == null)
			{
				variablePosition = new List<float>();
			}
			for (int i = 0; i < variables.Count; i++)
			{
				SharedVariable sharedVariable = variables[i];
				if (sharedVariable != null)
				{
					if (canSelect && selectedVariableIndex == i)
					{
						if (i == 0)
						{
							GUILayout.Space(2f);
						}
						bool flag = false;
						if (VariableInspector.DrawSelectedVariable(variableSource, ref variables, sharedVariable, ref selectedVariableIndex, ref selectedVariableName, ref selectedVariableTypeIndex, ref flag))
						{
							result = true;
						}
						if (flag)
						{
							if (BehaviorDesignerWindow.instance != null)
							{
								BehaviorDesignerWindow.instance.RemoveSharedVariableReferences(sharedVariable);
							}
							variables.RemoveAt(i);
							if (selectedVariableIndex == i)
							{
								selectedVariableIndex = -1;
							}
							else if (selectedVariableIndex > i)
							{
								selectedVariableIndex--;
							}
							result = true;
							break;
						}
					}
					else
					{
						GUILayout.BeginHorizontal();
						if (VariableInspector.DrawSharedVariable(variableSource, sharedVariable, false))
						{
							result = true;
						}
						if (drawRemoveButton && GUILayout.Button(BehaviorDesignerUtility.VariableDeleteButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
						{
							GUILayout.Width(19f)
						}) && EditorUtility.DisplayDialog("Delete Variable", "Are you sure you want to delete this variable?", "Yes", "No"))
						{
							if (BehaviorDesignerWindow.instance != null)
							{
								BehaviorDesignerWindow.instance.RemoveSharedVariableReferences(sharedVariable);
							}
							variables.RemoveAt(i);
							if (canSelect)
							{
								if (selectedVariableIndex == i)
								{
									selectedVariableIndex = -1;
								}
								else if (selectedVariableIndex > i)
								{
									selectedVariableIndex--;
								}
							}
							result = true;
							break;
						}
						if (BehaviorDesignerWindow.instance != null && BehaviorDesignerWindow.instance.ContainsError(null, i))
						{
							GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
							{
								GUILayout.Width(20f)
							});
						}
						GUILayout.Space(10f);
						GUILayout.EndHorizontal();
						if (i != variables.Count - 1 || drawLastSeparator)
						{
							BehaviorDesignerUtility.DrawContentSeperator(2, 7);
						}
					}
					GUILayout.Space(4f);
					if (canSelect && (int)Event.current.type == 7)
					{
						if (variablePosition.Count <= i)
						{
							variablePosition.Add(GUILayoutUtility.GetLastRect().yMax);
						}
						else
						{
							variablePosition[i] = GUILayoutUtility.GetLastRect().yMax;
						}
					}
				}
			}
			if (canSelect && variables.Count < variablePosition.Count)
			{
				for (int j = variablePosition.Count - 1; j >= variables.Count; j--)
				{
					variablePosition.RemoveAt(j);
				}
			}
			if (showFooter && variables.Count > 0)
			{
				GUI.enabled = true;
				GUILayout.Label("Select a variable to change its properties.", BehaviorDesignerUtility.LabelWrapGUIStyle, new GUILayoutOption[0]);
			}
			return result;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0001B430 File Offset: 0x00019630
		private static bool DrawSharedVariable(IVariableSource variableSource, SharedVariable sharedVariable, bool selected)
		{
			if (sharedVariable == null || sharedVariable.GetType().GetProperty("Value") == null)
			{
				return false;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool result = false;
			if (!string.IsNullOrEmpty(sharedVariable.PropertyMapping))
			{
				if (selected)
				{
					GUILayout.Label("Property", new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label(sharedVariable.Name, new GUILayoutOption[0]);
				}
				string[] array = sharedVariable.PropertyMapping.Split(new char[]
				{
					'.'
				});
				GUILayout.Label(array[array.Length - 1].Replace('/', '.'), new GUILayoutOption[0]);
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				FieldInspector.DrawFields(null, sharedVariable, new GUIContent(sharedVariable.Name));
				result = EditorGUI.EndChangeCheck();
			}
			if (!sharedVariable.IsGlobal && GUILayout.Button(BehaviorDesignerUtility.VariableMapButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(19f)
			}))
			{
				VariableInspector.ShowPropertyMappingMenu(variableSource as BehaviorSource, sharedVariable);
			}
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001B53C File Offset: 0x0001973C
		private static bool DrawSelectedVariable(IVariableSource variableSource, ref List<SharedVariable> variables, SharedVariable sharedVariable, ref int selectedVariableIndex, ref string selectedVariableName, ref int selectedVariableTypeIndex, ref bool deleted)
		{
			bool result = false;
			GUILayout.BeginVertical(BehaviorDesignerUtility.SelectedBackgroundGUIStyle, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Name", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			EditorGUI.BeginChangeCheck();
			selectedVariableName = GUILayout.TextField(selectedVariableName, new GUILayoutOption[]
			{
				GUILayout.Width(140f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				if (VariableInspector.VariableNameValid(variableSource, selectedVariableName))
				{
					variableSource.UpdateVariableName(sharedVariable, selectedVariableName);
				}
				result = true;
			}
			GUILayout.Space(10f);
			bool enabled = GUI.enabled;
			GUI.enabled = (enabled && selectedVariableIndex < variables.Count - 1);
			if (GUILayout.Button(BehaviorDesignerUtility.DownArrowButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(19f)
			}))
			{
				SharedVariable value = variables[selectedVariableIndex + 1];
				variables[selectedVariableIndex + 1] = variables[selectedVariableIndex];
				variables[selectedVariableIndex] = value;
				selectedVariableIndex++;
				result = true;
			}
			GUI.enabled = (enabled && (selectedVariableIndex < variables.Count - 1 || selectedVariableIndex != 0));
			GUILayout.Box(string.Empty, BehaviorDesignerUtility.ArrowSeparatorGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(1f),
				GUILayout.Height(18f)
			});
			GUI.enabled = (enabled && selectedVariableIndex != 0);
			if (GUILayout.Button(BehaviorDesignerUtility.UpArrowButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			}))
			{
				SharedVariable value2 = variables[selectedVariableIndex - 1];
				variables[selectedVariableIndex - 1] = variables[selectedVariableIndex];
				variables[selectedVariableIndex] = value2;
				selectedVariableIndex--;
				result = true;
			}
			GUI.enabled = enabled;
			if (GUILayout.Button(BehaviorDesignerUtility.VariableDeleteButtonTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(19f)
			}) && EditorUtility.DisplayDialog("Delete Variable", "Are you sure you want to delete this variable?", "Yes", "No"))
			{
				deleted = true;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Type", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			EditorGUI.BeginChangeCheck();
			selectedVariableTypeIndex = EditorGUILayout.Popup(selectedVariableTypeIndex, VariableInspector.sharedVariableStrings, EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			if (EditorGUI.EndChangeCheck() && VariableInspector.sharedVariableTypesDict[sharedVariable.GetType().Name] != selectedVariableTypeIndex)
			{
				if (BehaviorDesignerWindow.instance != null)
				{
					BehaviorDesignerWindow.instance.RemoveSharedVariableReferences(sharedVariable);
				}
				sharedVariable = VariableInspector.CreateVariable(selectedVariableTypeIndex, sharedVariable.Name, sharedVariable.IsGlobal);
				variables[selectedVariableIndex] = sharedVariable;
				result = true;
			}
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			GUILayout.Space(4f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = VariableInspector.CanNetworkSync(sharedVariable.GetType().GetProperty("Value").PropertyType);
			EditorGUI.BeginChangeCheck();
			sharedVariable.NetworkSync = EditorGUILayout.Toggle(new GUIContent("Network Sync", "Sync this variable over the network. Requires Unity 5.1 or greator. A NetworkIdentity must be attached to the behavior tree GameObject."), sharedVariable.NetworkSync, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				result = true;
			}
			GUILayout.EndHorizontal();
			GUI.enabled = enabled;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (VariableInspector.DrawSharedVariable(variableSource, sharedVariable, true))
			{
				result = true;
			}
			if (BehaviorDesignerWindow.instance != null && BehaviorDesignerWindow.instance.ContainsError(null, selectedVariableIndex))
			{
				GUILayout.Box(BehaviorDesignerUtility.ErrorIconTexture, BehaviorDesignerUtility.PlainTextureGUIStyle, new GUILayoutOption[]
				{
					GUILayout.Width(20f)
				});
			}
			GUILayout.EndHorizontal();
			BehaviorDesignerUtility.DrawContentSeperator(4, 7);
			GUILayout.EndVertical();
			GUILayout.Space(3f);
			return result;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0001B928 File Offset: 0x00019B28
		private static bool VariableNameValid(IVariableSource variableSource, string variableName)
		{
			return !variableName.Equals(string.Empty) && (variableSource == null || variableSource.GetVariable(variableName) == null);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0001B95C File Offset: 0x00019B5C
		private static SharedVariable CreateVariable(int index, string name, bool global)
		{
			SharedVariable sharedVariable = Activator.CreateInstance(VariableInspector.sharedVariableTypes[index]) as SharedVariable;
			sharedVariable.Name = name;
			sharedVariable.IsShared = true;
			sharedVariable.IsGlobal = global;
			return sharedVariable;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0001B998 File Offset: 0x00019B98
		private static bool CanNetworkSync(Type type)
		{
			return type == typeof(bool) || type == typeof(Color) || type == typeof(float) || type == typeof(GameObject) || type == typeof(int) || type == typeof(Quaternion) || type == typeof(Rect) || type == typeof(string) || type == typeof(Transform) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0001BA68 File Offset: 0x00019C68
		private static void ShowPropertyMappingMenu(BehaviorSource behaviorSource, SharedVariable sharedVariable)
		{
			VariableInspector.mPropertyMappingVariable = sharedVariable;
			VariableInspector.mPropertyMappingBehaviorSource = behaviorSource;
			VariableInspector.mPropertyMappingMenu = new GenericMenu();
			List<string> list = new List<string>();
			List<GameObject> list2 = new List<GameObject>();
			list.Add("None");
			list2.Add(null);
			int num = 0;
			if (behaviorSource.Owner.GetObject() is Behavior)
			{
				GameObject gameObject = (behaviorSource.Owner.GetObject() as Behavior).gameObject;
				int num2;
				if ((num2 = VariableInspector.AddPropertyName(sharedVariable, gameObject, ref list, ref list2, true)) != -1)
				{
					num = num2;
				}
				GameObject[] array;
				if (AssetDatabase.GetAssetPath(gameObject).Length == 0)
				{
					array = Object.FindObjectsOfType<GameObject>();
				}
				else
				{
					Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
					array = new GameObject[componentsInChildren.Length];
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						array[i] = componentsInChildren[i].gameObject;
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j].Equals(gameObject) && (num2 = VariableInspector.AddPropertyName(sharedVariable, array[j], ref list, ref list2, false)) != -1)
					{
						num = num2;
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				string[] array2 = list[k].Split(new char[]
				{
					'.'
				});
				if (list2[k] != null)
				{
					array2[array2.Length - 1] = VariableInspector.GetFullPath(list2[k].transform) + "/" + array2[array2.Length - 1];
				}
				VariableInspector.mPropertyMappingMenu.AddItem(new GUIContent(array2[array2.Length - 1]), k == num, new GenericMenu.MenuFunction2(VariableInspector.PropertySelected), new VariableInspector.SelectedPropertyMapping(list[k], list2[k]));
			}
			VariableInspector.mPropertyMappingMenu.ShowAsContext();
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0001BC4C File Offset: 0x00019E4C
		private static string GetFullPath(Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.name;
			}
			return VariableInspector.GetFullPath(transform.parent) + "/" + transform.name;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0001BC8C File Offset: 0x00019E8C
		private static int AddPropertyName(SharedVariable sharedVariable, GameObject gameObject, ref List<string> propertyNames, ref List<GameObject> propertyGameObjects, bool behaviorGameObject)
		{
			int result = -1;
			if (gameObject != null)
			{
				Component[] components = gameObject.GetComponents(typeof(Component));
				Type propertyType = sharedVariable.GetType().GetProperty("Value").PropertyType;
				for (int i = 0; i < components.Length; i++)
				{
					if (!(components[i] == null))
					{
						PropertyInfo[] properties = components[i].GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
						for (int j = 0; j < properties.Length; j++)
						{
							if (properties[j].PropertyType.Equals(propertyType) && !properties[j].IsSpecialName)
							{
								string text = components[i].GetType().FullName + "/" + properties[j].Name;
								if (text.Equals(sharedVariable.PropertyMapping) && (object.Equals(sharedVariable.PropertyMappingOwner, gameObject) || (object.Equals(sharedVariable.PropertyMappingOwner, null) && behaviorGameObject)))
								{
									result = propertyNames.Count;
								}
								propertyNames.Add(text);
								propertyGameObjects.Add(gameObject);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0001BDC4 File Offset: 0x00019FC4
		private static void PropertySelected(object selected)
		{
			VariableInspector.SelectedPropertyMapping selectedPropertyMapping = selected as VariableInspector.SelectedPropertyMapping;
			if (selectedPropertyMapping.Property.Equals("None"))
			{
				VariableInspector.mPropertyMappingVariable.PropertyMapping = string.Empty;
				VariableInspector.mPropertyMappingVariable.PropertyMappingOwner = null;
			}
			else
			{
				VariableInspector.mPropertyMappingVariable.PropertyMapping = selectedPropertyMapping.Property;
				VariableInspector.mPropertyMappingVariable.PropertyMappingOwner = selectedPropertyMapping.GameObject;
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
			{
				BinarySerialization.Save(VariableInspector.mPropertyMappingBehaviorSource);
			}
			else
			{
				JSONSerialization.Save(VariableInspector.mPropertyMappingBehaviorSource);
			}
		}

		// Token: 0x04000197 RID: 407
		private static string[] sharedVariableStrings;

		// Token: 0x04000198 RID: 408
		private static List<Type> sharedVariableTypes;

		// Token: 0x04000199 RID: 409
		private static Dictionary<string, int> sharedVariableTypesDict;

		// Token: 0x0400019A RID: 410
		private string mVariableName = string.Empty;

		// Token: 0x0400019B RID: 411
		private int mVariableTypeIndex;

		// Token: 0x0400019C RID: 412
		private Vector2 mScrollPosition = Vector2.zero;

		// Token: 0x0400019D RID: 413
		private bool mFocusNameField;

		// Token: 0x0400019E RID: 414
		[SerializeField]
		private float mVariableStartPosition = -1f;

		// Token: 0x0400019F RID: 415
		[SerializeField]
		private List<float> mVariablePosition;

		// Token: 0x040001A0 RID: 416
		[SerializeField]
		private int mSelectedVariableIndex = -1;

		// Token: 0x040001A1 RID: 417
		[SerializeField]
		private string mSelectedVariableName;

		// Token: 0x040001A2 RID: 418
		[SerializeField]
		private int mSelectedVariableTypeIndex;

		// Token: 0x040001A3 RID: 419
		private static SharedVariable mPropertyMappingVariable;

		// Token: 0x040001A4 RID: 420
		private static BehaviorSource mPropertyMappingBehaviorSource;

		// Token: 0x040001A5 RID: 421
		private static GenericMenu mPropertyMappingMenu;

		// Token: 0x0200002D RID: 45
		private class SelectedPropertyMapping
		{
			// Token: 0x060002C7 RID: 711 RVA: 0x0001BE54 File Offset: 0x0001A054
			public SelectedPropertyMapping(string property, GameObject gameObject)
			{
				this.mProperty = property;
				this.mGameObject = gameObject;
			}

			// Token: 0x17000084 RID: 132
			// (get) Token: 0x060002C8 RID: 712 RVA: 0x0001BE6C File Offset: 0x0001A06C
			public string Property
			{
				get
				{
					return this.mProperty;
				}
			}

			// Token: 0x17000085 RID: 133
			// (get) Token: 0x060002C9 RID: 713 RVA: 0x0001BE74 File Offset: 0x0001A074
			public GameObject GameObject
			{
				get
				{
					return this.mGameObject;
				}
			}

			// Token: 0x040001A6 RID: 422
			private string mProperty;

			// Token: 0x040001A7 RID: 423
			private GameObject mGameObject;
		}
	}
}
