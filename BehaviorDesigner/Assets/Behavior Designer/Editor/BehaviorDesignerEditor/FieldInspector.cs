using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000016 RID: 22
	public static class FieldInspector
	{
		// Token: 0x06000189 RID: 393 RVA: 0x0000D8BC File Offset: 0x0000BABC
		public static void Init()
		{
			FieldInspector.InitLayers();
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000D8C4 File Offset: 0x0000BAC4
		private static bool FoldOut(int hash)
		{
			if (FieldInspector.foldoutDictionary.ContainsKey(hash))
			{
				return FieldInspector.foldoutDictionary[hash];
			}
			FieldInspector.foldoutDictionary.Add(hash, BehaviorDesignerPreferences.GetBool(BDPreferences.FoldoutFields));
			return true;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000D900 File Offset: 0x0000BB00
		private static void SetFoldOut(int hash, bool value)
		{
			if (FieldInspector.foldoutDictionary.ContainsKey(hash))
			{
				FieldInspector.foldoutDictionary[hash] = value;
				return;
			}
			FieldInspector.foldoutDictionary.Add(hash, value);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000D92C File Offset: 0x0000BB2C
		public static bool DrawFoldout(int hash, GUIContent guiContent)
		{
			bool flag = FieldInspector.FoldOut(hash);
			bool flag2 = EditorGUILayout.Foldout(flag, guiContent);
			if (flag2 != flag)
			{
				FieldInspector.SetFoldOut(hash, flag2);
			}
			return flag2;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000D958 File Offset: 0x0000BB58
		public static object DrawFields(Task task, object obj)
		{
			return FieldInspector.DrawFields(task, obj, null);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000D964 File Offset: 0x0000BB64
		public static object DrawFields(Task task, object obj, GUIContent guiContent)
		{
			if (obj == null)
			{
				return null;
			}
			List<Type> baseClasses = FieldInspector.GetBaseClasses(obj.GetType());
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			for (int i = baseClasses.Count - 1; i > -1; i--)
			{
				FieldInfo[] fields = baseClasses[i].GetFields(bindingAttr);
				for (int j = 0; j < fields.Length; j++)
				{
					if (!BehaviorDesignerUtility.HasAttribute(fields[j], typeof(NonSerializedAttribute)) && !BehaviorDesignerUtility.HasAttribute(fields[j], typeof(HideInInspector)) && ((!fields[j].IsPrivate && !fields[j].IsFamily) || BehaviorDesignerUtility.HasAttribute(fields[j], typeof(SerializeField))) && (!(obj is ParentTask) || !fields[j].Name.Equals("children")))
					{
						if (guiContent == null)
						{
							string name = fields[j].Name;
							TooltipAttribute[] array;
							if ((array = (fields[j].GetCustomAttributes(typeof(TooltipAttribute), false) as TooltipAttribute[])).Length > 0)
							{
								guiContent = new GUIContent(BehaviorDesignerUtility.SplitCamelCase(name), array[0].Tooltip);
							}
							else
							{
								guiContent = new GUIContent(BehaviorDesignerUtility.SplitCamelCase(name));
							}
						}
						EditorGUI.BeginChangeCheck();
						object value = FieldInspector.DrawField(task, guiContent, fields[j], fields[j].GetValue(obj));
						if (EditorGUI.EndChangeCheck())
						{
							fields[j].SetValue(obj, value);
							GUI.changed = true;
						}
						guiContent = null;
					}
				}
			}
			return obj;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000DAF4 File Offset: 0x0000BCF4
		public static List<Type> GetBaseClasses(Type t)
		{
			List<Type> list = new List<Type>();
			while (t != null && !t.Equals(typeof(ParentTask)) && !t.Equals(typeof(Task)) && !t.Equals(typeof(SharedVariable)))
			{
				list.Add(t);
				t = t.BaseType;
			}
			return list;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000DB64 File Offset: 0x0000BD64
		public static object DrawField(Task task, GUIContent guiContent, FieldInfo field, object value)
		{
			if (field.FieldType.IsAbstract)
			{
				EditorGUILayout.LabelField(guiContent, new GUILayoutOption[0]);
				return null;
			}
			ObjectDrawer objectDrawer;
			if ((objectDrawer = ObjectDrawerUtility.GetObjectDrawer(task, field)) != null)
			{
				if (value == null && !field.FieldType.IsAbstract)
				{
					value = Activator.CreateInstance(field.FieldType, true);
				}
				objectDrawer.Value = value;
				objectDrawer.OnGUI(guiContent);
				if (objectDrawer.Value != value)
				{
					value = objectDrawer.Value;
					GUI.changed = true;
				}
				return value;
			}
			ObjectDrawerAttribute[] array;
			if ((array = (field.GetCustomAttributes(typeof(ObjectDrawerAttribute), true) as ObjectDrawerAttribute[])).Length > 0 && (objectDrawer = ObjectDrawerUtility.GetObjectDrawer(task, array[0])) != null)
			{
				if (value == null)
				{
					value = Activator.CreateInstance(field.FieldType, true);
				}
				objectDrawer.Value = value;
				objectDrawer.OnGUI(guiContent);
				if (objectDrawer.Value != value)
				{
					value = objectDrawer.Value;
					GUI.changed = true;
				}
				return value;
			}
			return FieldInspector.DrawField(task, guiContent, field, field.FieldType, value);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000DC6C File Offset: 0x0000BE6C
		private static object DrawField(Task task, GUIContent guiContent, FieldInfo fieldInfo, Type fieldType, object value)
		{
			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				return FieldInspector.DrawArrayField(task, guiContent, fieldInfo, fieldType, value);
			}
			return FieldInspector.DrawSingleField(task, guiContent, fieldInfo, fieldType, value);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000DCA8 File Offset: 0x0000BEA8
		private static object DrawArrayField(Task task, GUIContent guiContent, FieldInfo fieldInfo, Type fieldType, object value)
		{
			Type type;
			if (fieldType.IsArray)
			{
				type = fieldType.GetElementType();
			}
			else
			{
				Type type2 = fieldType;
				while (!type2.IsGenericType)
				{
					type2 = type2.BaseType;
				}
				type = type2.GetGenericArguments()[0];
			}
			IList list;
			if (value == null)
			{
				if (fieldType.IsGenericType || fieldType.IsArray)
				{
					list = (Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
					{
						type
					}), true) as IList);
				}
				else
				{
					list = (Activator.CreateInstance(fieldType, true) as IList);
				}
				if (fieldType.IsArray)
				{
					Array array = Array.CreateInstance(type, list.Count);
					list.CopyTo(array, 0);
					list = array;
				}
				GUI.changed = true;
			}
			else
			{
				list = (IList)value;
			}
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			if (FieldInspector.DrawFoldout(guiContent.text.GetHashCode(), guiContent))
			{
				EditorGUI.indentLevel++;
				bool flag = guiContent.text.GetHashCode() == FieldInspector.editingFieldHash;
				int num = (!flag) ? list.Count : FieldInspector.savedArraySize;
				int num2 = EditorGUILayout.IntField("Size", num, new GUILayoutOption[0]);
				if (flag && FieldInspector.editingArray && (GUIUtility.keyboardControl != FieldInspector.currentKeyboardControl ||(int)Event.current.keyCode == 13))
				{
					if (num2 != list.Count)
					{
						Array array2 = Array.CreateInstance(type, num2);
						int num3 = -1;
						for (int i = 0; i < num2; i++)
						{
							if (i < list.Count)
							{
								num3 = i;
							}
							if (num3 == -1)
							{
								break;
							}
							object value2 = list[num3];
							if (i >= list.Count && !typeof(Object).IsAssignableFrom(type))
							{
								value2 = Activator.CreateInstance(list[num3].GetType(), true);
							}
							array2.SetValue(value2, i);
						}
						if (fieldType.IsArray)
						{
							list = array2;
						}
						else
						{
							if (fieldType.IsGenericType)
							{
								list = (Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
								{
									type
								}), true) as IList);
							}
							else
							{
								list = (Activator.CreateInstance(fieldType, true) as IList);
							}
							for (int j = 0; j < array2.Length; j++)
							{
								list.Add(array2.GetValue(j));
							}
						}
					}
					FieldInspector.editingArray = false;
					FieldInspector.savedArraySize = -1;
					FieldInspector.editingFieldHash = -1;
					GUI.changed = true;
				}
				else if (num2 != num)
				{
					if (!FieldInspector.editingArray)
					{
						FieldInspector.currentKeyboardControl = GUIUtility.keyboardControl;
						FieldInspector.editingArray = true;
						FieldInspector.editingFieldHash = guiContent.text.GetHashCode();
					}
					FieldInspector.savedArraySize = num2;
				}
				for (int k = 0; k < list.Count; k++)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					guiContent.text = "Element " + k;
					list[k] = FieldInspector.DrawField(task, guiContent, fieldInfo, type, list[k]);
					GUILayout.Space(6f);
					GUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndVertical();
			return list;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000E004 File Offset: 0x0000C204
		private static object DrawSingleField(Task task, GUIContent guiContent, FieldInfo fieldInfo, Type fieldType, object value)
		{
			if (fieldType.Equals(typeof(int)))
			{
				return EditorGUILayout.IntField(guiContent, (int)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(float)))
			{
				return EditorGUILayout.FloatField(guiContent, (float)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(double)))
			{
				return EditorGUILayout.FloatField(guiContent, Convert.ToSingle((double)value), new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(long)))
			{
				return (long)EditorGUILayout.IntField(guiContent, Convert.ToInt32((long)value), new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(bool)))
			{
				return EditorGUILayout.Toggle(guiContent, (bool)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(string)))
			{
				return EditorGUILayout.TextField(guiContent, (string)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(byte)))
			{
				return Convert.ToByte(EditorGUILayout.IntField(guiContent, Convert.ToInt32(value), new GUILayoutOption[0]));
			}
			if (fieldType.Equals(typeof(Vector2)))
			{
				return EditorGUILayout.Vector2Field(guiContent.text, (Vector2)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(Vector3)))
			{
				return EditorGUILayout.Vector3Field(guiContent.text, (Vector3)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(Vector4)))
			{
				return EditorGUILayout.Vector4Field(guiContent.text, (Vector4)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(Quaternion)))
			{
				Quaternion quaternion = (Quaternion)value;
				Vector4 vector = Vector4.zero;
				vector.Set(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
				vector = EditorGUILayout.Vector4Field(guiContent.text, vector, new GUILayoutOption[0]);
				quaternion.Set(vector.x, vector.y, vector.z, vector.w);
				return quaternion;
			}
			if (fieldType.Equals(typeof(Color)))
			{
				return EditorGUILayout.ColorField(guiContent, (Color)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(Rect)))
			{
				return EditorGUILayout.RectField(guiContent, (Rect)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(Matrix4x4)))
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				if (FieldInspector.DrawFoldout(guiContent.text.GetHashCode(), guiContent))
				{
					EditorGUI.indentLevel++;
					Matrix4x4 matrix4x = (Matrix4x4)value;
					for (int i = 0; i < 4; i++)
					{
						for (int j = 0; j < 4; j++)
						{
							EditorGUI.BeginChangeCheck();
							matrix4x[i, j] = EditorGUILayout.FloatField("E" + i.ToString() + j.ToString(), matrix4x[i, j], new GUILayoutOption[0]);
							if (EditorGUI.EndChangeCheck())
							{
								GUI.changed = true;
							}
						}
					}
					value = matrix4x;
					EditorGUI.indentLevel--;
				}
				GUILayout.EndVertical();
				return value;
			}
			if (fieldType.Equals(typeof(AnimationCurve)))
			{
				if (value == null)
				{
					value = new AnimationCurve();
				}
				return EditorGUILayout.CurveField(guiContent, (AnimationCurve)value, new GUILayoutOption[0]);
			}
			if (fieldType.Equals(typeof(LayerMask)))
			{
				return FieldInspector.DrawLayerMask(guiContent, (LayerMask)value);
			}
			if (typeof(SharedVariable).IsAssignableFrom(fieldType))
			{
				return FieldInspector.DrawSharedVariable(task, guiContent, fieldInfo, fieldType, value as SharedVariable);
			}
			if (typeof(Object).IsAssignableFrom(fieldType))
			{
				return EditorGUILayout.ObjectField(guiContent, (Object)value, fieldType, true, new GUILayoutOption[0]);
			}
			if (fieldType.IsEnum)
			{
				return EditorGUILayout.EnumPopup(guiContent, (Enum)value, new GUILayoutOption[0]);
			}
			if (!fieldType.IsClass && (!fieldType.IsValueType || fieldType.IsPrimitive))
			{
				EditorGUILayout.LabelField("Unsupported Type: " + fieldType, new GUILayoutOption[0]);
				return null;
			}
			if (typeof(Delegate).IsAssignableFrom(fieldType))
			{
				return null;
			}
			int hashCode = guiContent.text.GetHashCode();
			if (FieldInspector.drawnObjects.Contains(hashCode))
			{
				return null;
			}
			FieldInspector.drawnObjects.Add(hashCode);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (value == null)
			{
				if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					fieldType = Nullable.GetUnderlyingType(fieldType);
				}
				value = Activator.CreateInstance(fieldType, true);
			}
			if (FieldInspector.DrawFoldout(hashCode, guiContent))
			{
				EditorGUI.indentLevel++;
				value = FieldInspector.DrawFields(task, value);
				EditorGUI.indentLevel--;
			}
			GUILayout.EndVertical();
			FieldInspector.drawnObjects.Remove(hashCode);
			return value;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000E584 File Offset: 0x0000C784
		public static SharedVariable DrawSharedVariable(Task task, GUIContent guiContent, FieldInfo fieldInfo, Type fieldType, SharedVariable sharedVariable)
		{
			if (!fieldType.Equals(typeof(SharedVariable)) && sharedVariable == null)
			{
				sharedVariable = (Activator.CreateInstance(fieldType, true) as SharedVariable);
				if (TaskUtility.HasAttribute(fieldInfo, typeof(RequiredFieldAttribute)) || TaskUtility.HasAttribute(fieldInfo, typeof(SharedRequiredAttribute)))
				{
					sharedVariable.IsShared = true;
				}
				GUI.changed = true;
			}
			if (sharedVariable == null || sharedVariable.IsShared)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				string[] array = null;
				int num = -1;
				int num2 = FieldInspector.GetVariablesOfType((sharedVariable == null) ? null : sharedVariable.GetType().GetProperty("Value").PropertyType, sharedVariable != null && sharedVariable.IsGlobal, (sharedVariable == null) ? string.Empty : sharedVariable.Name, FieldInspector.behaviorSource, out array, ref num, fieldType.Equals(typeof(SharedVariable)));
				Color backgroundColor = GUI.backgroundColor;
				if (num2 == 0 && !TaskUtility.HasAttribute(fieldInfo, typeof(SharedRequiredAttribute)))
				{
					GUI.backgroundColor = Color.red;
				}
				int num3 = num2;
				num2 = EditorGUILayout.Popup(guiContent.text, num2, array, BehaviorDesignerUtility.SharedVariableToolbarPopup, new GUILayoutOption[0]);
				GUI.backgroundColor = backgroundColor;
				if (num2 != num3)
				{
					if (num2 == 0)
					{
						if (fieldType.Equals(typeof(SharedVariable)))
						{
							sharedVariable = null;
						}
						else
						{
							sharedVariable = (Activator.CreateInstance(fieldType, true) as SharedVariable);
							sharedVariable.IsShared = true;
						}
					}
					else if (num != -1 && num2 >= num)
					{
						sharedVariable = GlobalVariables.Instance.GetVariable(array[num2].Substring(8, array[num2].Length - 8));
					}
					else
					{
						sharedVariable = FieldInspector.behaviorSource.GetVariable(array[num2]);
					}
					GUI.changed = true;
				}
				if (!fieldType.Equals(typeof(SharedVariable)) && !TaskUtility.HasAttribute(fieldInfo, typeof(RequiredFieldAttribute)) && !TaskUtility.HasAttribute(fieldInfo, typeof(SharedRequiredAttribute)))
				{
					sharedVariable = FieldInspector.DrawSharedVariableToggleSharedButton(sharedVariable);
					GUILayout.Space(-3f);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			else
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				ObjectDrawerAttribute[] array2;
				ObjectDrawer objectDrawer;
				if (fieldInfo != null && (array2 = (fieldInfo.GetCustomAttributes(typeof(ObjectDrawerAttribute), true) as ObjectDrawerAttribute[])).Length > 0 && (objectDrawer = ObjectDrawerUtility.GetObjectDrawer(task, array2[0])) != null)
				{
					objectDrawer.Value = sharedVariable;
					objectDrawer.OnGUI(guiContent);
				}
				else
				{
					FieldInspector.DrawFields(task, sharedVariable, guiContent);
				}
				if (!TaskUtility.HasAttribute(fieldInfo, typeof(RequiredFieldAttribute)) && !TaskUtility.HasAttribute(fieldInfo, typeof(SharedRequiredAttribute)))
				{
					sharedVariable = FieldInspector.DrawSharedVariableToggleSharedButton(sharedVariable);
				}
				GUILayout.EndHorizontal();
			}
			return sharedVariable;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000E868 File Offset: 0x0000CA68
		public static int GetVariablesOfType(Type valueType, bool isGlobal, string name, BehaviorSource behaviorSource, out string[] names, ref int globalStartIndex, bool getAll)
		{
			if (behaviorSource == null)
			{
				names = new string[0];
				return 0;
			}
			List<SharedVariable> variables = behaviorSource.Variables;
			int result = 0;
			List<string> list = new List<string>();
			list.Add("None");
			if (variables != null)
			{
				for (int i = 0; i < variables.Count; i++)
				{
					if (variables[i] != null)
					{
						Type propertyType = variables[i].GetType().GetProperty("Value").PropertyType;
						if (valueType == null || getAll || valueType.IsAssignableFrom(propertyType))
						{
							list.Add(variables[i].Name);
							if (!isGlobal && variables[i].Name.Equals(name))
							{
								result = list.Count - 1;
							}
						}
					}
				}
			}
			GlobalVariables instance;
			if ((instance = GlobalVariables.Instance) != null)
			{
				globalStartIndex = list.Count;
				variables = instance.Variables;
				if (variables != null)
				{
					for (int j = 0; j < variables.Count; j++)
					{
						if (variables[j] != null)
						{
							Type propertyType2 = variables[j].GetType().GetProperty("Value").PropertyType;
							if (valueType == null || getAll || propertyType2.Equals(valueType))
							{
								list.Add("Globals/" + variables[j].Name);
								if (isGlobal && variables[j].Name.Equals(name))
								{
									result = list.Count - 1;
								}
							}
						}
					}
				}
			}
			names = list.ToArray();
			return result;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000EA20 File Offset: 0x0000CC20
		internal static SharedVariable DrawSharedVariableToggleSharedButton(SharedVariable sharedVariable)
		{
			if (sharedVariable == null)
			{
				return null;
			}
			if (GUILayout.Button((!sharedVariable.IsShared) ? BehaviorDesignerUtility.VariableButtonTexture : BehaviorDesignerUtility.VariableButtonSelectedTexture, BehaviorDesignerUtility.PlainButtonGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(15f)
			}))
			{
				bool isShared = !sharedVariable.IsShared;
				if (sharedVariable.GetType().Equals(typeof(SharedVariable)))
				{
					sharedVariable = (Activator.CreateInstance(FieldInspector.FriendlySharedVariableName(sharedVariable.GetType().GetProperty("Value").PropertyType), true) as SharedVariable);
				}
				else
				{
					sharedVariable = (Activator.CreateInstance(sharedVariable.GetType(), true) as SharedVariable);
				}
				sharedVariable.IsShared = isShared;
			}
			return sharedVariable;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000EADC File Offset: 0x0000CCDC
		internal static Type FriendlySharedVariableName(Type type)
		{
			if (type.Equals(typeof(bool)))
			{
				return TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedBool");
			}
			if (type.Equals(typeof(int)))
			{
				return TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedInt");
			}
			if (type.Equals(typeof(float)))
			{
				return TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedFloat");
			}
			if (type.Equals(typeof(string)))
			{
				return TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString");
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.Shared" + type.Name);
				if (typeWithinAssembly != null)
				{
					return typeWithinAssembly;
				}
			}
			else
			{
				Type typeWithinAssembly2 = TaskUtility.GetTypeWithinAssembly("Shared" + type.Name);
				if (typeWithinAssembly2 != null)
				{
					return typeWithinAssembly2;
				}
			}
			return type;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000EBC0 File Offset: 0x0000CDC0
		private static LayerMask DrawLayerMask(GUIContent guiContent, LayerMask layerMask)
		{
			if (FieldInspector.layerNames == null)
			{
				FieldInspector.InitLayers();
			}
			int num = 0;
			for (int i = 0; i < FieldInspector.layerNames.Length; i++)
			{
				if ((layerMask.value & FieldInspector.maskValues[i]) == FieldInspector.maskValues[i])
				{
					num |= 1 << i;
				}
			}
			int num2 = EditorGUILayout.MaskField(guiContent, num, FieldInspector.layerNames, new GUILayoutOption[0]);
			if (num2 != num)
			{
				num = 0;
				for (int j = 0; j < FieldInspector.layerNames.Length; j++)
				{
					if ((num2 & 1 << j) != 0)
					{
						num |= FieldInspector.maskValues[j];
					}
				}
				layerMask.value = num;
			}
			return layerMask;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000EC74 File Offset: 0x0000CE74
		private static void InitLayers()
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < 32; i++)
			{
				string text = LayerMask.LayerToName(i);
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
					list2.Add(1 << i);
				}
			}
			FieldInspector.layerNames = list.ToArray();
			FieldInspector.maskValues = list2.ToArray();
		}

		// Token: 0x0400010D RID: 269
		private static int currentKeyboardControl = -1;

		// Token: 0x0400010E RID: 270
		private static bool editingArray = false;

		// Token: 0x0400010F RID: 271
		private static int savedArraySize = -1;

		// Token: 0x04000110 RID: 272
		private static int editingFieldHash;

		// Token: 0x04000111 RID: 273
		public static BehaviorSource behaviorSource;

		// Token: 0x04000112 RID: 274
		private static Dictionary<int, bool> foldoutDictionary = new Dictionary<int, bool>();

		// Token: 0x04000113 RID: 275
		private static HashSet<int> drawnObjects = new HashSet<int>();

		// Token: 0x04000114 RID: 276
		private static string[] layerNames;

		// Token: 0x04000115 RID: 277
		private static int[] maskValues;
	}
}
