using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200001E RID: 30
	public class JSONDeserializationDeprecated : UnityEngine.Object
	{
		// Token: 0x0600016B RID: 363 RVA: 0x0000D2C4 File Offset: 0x0000B4C4
		public static void Load(TaskSerializationData taskData, BehaviorSource behaviorSource)
		{
			behaviorSource.EntryTask = null;
			behaviorSource.RootTask = null;
			behaviorSource.DetachedTasks = null;
			behaviorSource.Variables = null;
			Dictionary<string, object> dictionary;
			if (!JSONDeserializationDeprecated.serializationCache.TryGetValue(taskData.JSONSerialization.GetHashCode(), out dictionary))
			{
				dictionary = (MiniJSON.Deserialize(taskData.JSONSerialization) as Dictionary<string, object>);
				JSONDeserializationDeprecated.serializationCache.Add(taskData.JSONSerialization.GetHashCode(), dictionary);
			}
			if (dictionary == null)
			{
				Debug.Log("Failed to deserialize");
				return;
			}
			JSONDeserializationDeprecated.taskIDs = new Dictionary<JSONDeserializationDeprecated.TaskField, List<int>>();
			Dictionary<int, Task> dictionary2 = new Dictionary<int, Task>();
			JSONDeserializationDeprecated.DeserializeVariables(behaviorSource, dictionary, taskData.fieldSerializationData.unityObjects);
			if (dictionary.ContainsKey("EntryTask"))
			{
				behaviorSource.EntryTask = JSONDeserializationDeprecated.DeserializeTask(behaviorSource, dictionary["EntryTask"] as Dictionary<string, object>, ref dictionary2, taskData.fieldSerializationData.unityObjects);
			}
			if (dictionary.ContainsKey("RootTask"))
			{
				behaviorSource.RootTask = JSONDeserializationDeprecated.DeserializeTask(behaviorSource, dictionary["RootTask"] as Dictionary<string, object>, ref dictionary2, taskData.fieldSerializationData.unityObjects);
			}
			if (dictionary.ContainsKey("DetachedTasks"))
			{
				List<Task> list = new List<Task>();
				foreach (object obj in (dictionary["DetachedTasks"] as IEnumerable))
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)obj;
					list.Add(JSONDeserializationDeprecated.DeserializeTask(behaviorSource, dict, ref dictionary2, taskData.fieldSerializationData.unityObjects));
				}
				behaviorSource.DetachedTasks = list;
			}
			if (JSONDeserializationDeprecated.taskIDs != null && JSONDeserializationDeprecated.taskIDs.Count > 0)
			{
				foreach (JSONDeserializationDeprecated.TaskField key in JSONDeserializationDeprecated.taskIDs.Keys)
				{
					List<int> list2 = JSONDeserializationDeprecated.taskIDs[key];
					Type fieldType = key.fieldInfo.FieldType;
					if (key.fieldInfo.FieldType.IsArray)
					{
						int num = 0;
						for (int i = 0; i < list2.Count; i++)
						{
							Task task = dictionary2[list2[i]];
							if (task.GetType().Equals(fieldType.GetElementType()) || task.GetType().IsSubclassOf(fieldType.GetElementType()))
							{
								num++;
							}
						}
						Array array = Array.CreateInstance(fieldType.GetElementType(), num);
						int num2 = 0;
						for (int j = 0; j < list2.Count; j++)
						{
							Task task2 = dictionary2[list2[j]];
							if (task2.GetType().Equals(fieldType.GetElementType()) || task2.GetType().IsSubclassOf(fieldType.GetElementType()))
							{
								array.SetValue(task2, num2);
								num2++;
							}
						}
						key.fieldInfo.SetValue(key.task, array);
					}
					else
					{
						Task task3 = dictionary2[list2[0]];
						if (task3.GetType().Equals(key.fieldInfo.FieldType) || task3.GetType().IsSubclassOf(key.fieldInfo.FieldType))
						{
							key.fieldInfo.SetValue(key.task, task3);
						}
					}
				}
				JSONDeserializationDeprecated.taskIDs = null;
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000D680 File Offset: 0x0000B880
		public static void Load(string serialization, GlobalVariables globalVariables)
		{
			if (globalVariables == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = MiniJSON.Deserialize(serialization) as Dictionary<string, object>;
			if (dictionary == null)
			{
				Debug.Log("Failed to deserialize");
				return;
			}
			if (globalVariables.VariableData == null)
			{
				globalVariables.VariableData = new VariableSerializationData();
			}
			JSONDeserializationDeprecated.DeserializeVariables(globalVariables, dictionary, globalVariables.VariableData.fieldSerializationData.unityObjects);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000D6E4 File Offset: 0x0000B8E4
		private static void DeserializeVariables(IVariableSource variableSource, Dictionary<string, object> dict, List<UnityEngine.Object> unityObjects)
		{
			object obj;
			if (dict.TryGetValue("Variables", out obj))
			{
				List<SharedVariable> list = new List<SharedVariable>();
				IList list2 = obj as IList;
				for (int i = 0; i < list2.Count; i++)
				{
					SharedVariable item = JSONDeserializationDeprecated.DeserializeSharedVariable(list2[i] as Dictionary<string, object>, variableSource, true, unityObjects);
					list.Add(item);
				}
				variableSource.SetAllVariables(list);
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000D74C File Offset: 0x0000B94C
		public static Task DeserializeTask(BehaviorSource behaviorSource, Dictionary<string, object> dict, ref Dictionary<int, Task> IDtoTask, List<UnityEngine.Object> unityObjects)
		{
			Task task = null;
			try
			{
				Type type = TaskUtility.GetTypeWithinAssembly(dict["ObjectType"] as string);
				if (type == null)
				{
					if (dict.ContainsKey("Children"))
					{
						type = typeof(UnknownParentTask);
					}
					else
					{
						type = typeof(UnknownTask);
					}
				}
				task = (TaskUtility.CreateInstance(type) as Task);
			}
			catch (Exception)
			{
			}
			if (task == null)
			{
				return null;
			}
			task.Owner = (behaviorSource.Owner.GetObject() as Behavior);
			task.ID = Convert.ToInt32(dict["ID"]);
			object obj;
			if (dict.TryGetValue("Name", out obj))
			{
				task.FriendlyName = (string)obj;
			}
			if (dict.TryGetValue("Instant", out obj))
			{
				task.IsInstant = Convert.ToBoolean(obj);
			}
			if (dict.TryGetValue("Disabled", out obj))
			{
				task.Disabled = Convert.ToBoolean(obj);
			}
			IDtoTask.Add(task.ID, task);
			task.NodeData = JSONDeserializationDeprecated.DeserializeNodeData(dict["NodeData"] as Dictionary<string, object>, task);
			if (task.GetType().Equals(typeof(UnknownTask)) || task.GetType().Equals(typeof(UnknownParentTask)))
			{
				if (!task.FriendlyName.Contains("Unknown "))
				{
					task.FriendlyName = string.Format("Unknown {0}", task.FriendlyName);
				}
				if (!task.NodeData.Comment.Contains("Loaded from an unknown type. Was a task renamed or deleted?"))
				{
					task.NodeData.Comment = string.Format("Loaded from an unknown type. Was a task renamed or deleted?{0}", (!task.NodeData.Comment.Equals(string.Empty)) ? string.Format("\0{0}", task.NodeData.Comment) : string.Empty);
				}
			}
			JSONDeserializationDeprecated.DeserializeObject(task, task, dict, behaviorSource, unityObjects);
			if (task is ParentTask && dict.TryGetValue("Children", out obj))
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask != null)
				{
					foreach (object obj2 in (obj as IEnumerable))
					{
						Dictionary<string, object> dict2 = (Dictionary<string, object>)obj2;
						Task child = JSONDeserializationDeprecated.DeserializeTask(behaviorSource, dict2, ref IDtoTask, unityObjects);
						int index = (parentTask.Children != null) ? parentTask.Children.Count : 0;
						parentTask.AddChild(child, index);
					}
				}
			}
			return task;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000DA1C File Offset: 0x0000BC1C
		private static NodeData DeserializeNodeData(Dictionary<string, object> dict, Task task)
		{
			NodeData nodeData = new NodeData();
			object obj;
			if (dict.TryGetValue("Offset", out obj))
			{
				nodeData.Offset = JSONDeserializationDeprecated.StringToVector2((string)obj);
			}
			if (dict.TryGetValue("FriendlyName", out obj))
			{
				task.FriendlyName = (string)obj;
			}
			if (dict.TryGetValue("Comment", out obj))
			{
				nodeData.Comment = (string)obj;
			}
			if (dict.TryGetValue("IsBreakpoint", out obj))
			{
				nodeData.IsBreakpoint = Convert.ToBoolean(obj);
			}
			if (dict.TryGetValue("Collapsed", out obj))
			{
				nodeData.Collapsed = Convert.ToBoolean(obj);
			}
			if (dict.TryGetValue("ColorIndex", out obj))
			{
				nodeData.ColorIndex = Convert.ToInt32(obj);
			}
			if (dict.TryGetValue("WatchedFields", out obj))
			{
				nodeData.WatchedFieldNames = new List<string>();
				nodeData.WatchedFields = new List<FieldInfo>();
				IList list = obj as IList;
				for (int i = 0; i < list.Count; i++)
				{
					FieldInfo field = task.GetType().GetField((string)list[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						nodeData.WatchedFieldNames.Add(field.Name);
						nodeData.WatchedFields.Add(field);
					}
				}
			}
			return nodeData;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000DB70 File Offset: 0x0000BD70
		private static SharedVariable DeserializeSharedVariable(Dictionary<string, object> dict, IVariableSource variableSource, bool fromSource, List<UnityEngine.Object> unityObjects)
		{
			if (dict == null)
			{
				return null;
			}
			SharedVariable sharedVariable = null;
			object obj;
			if (!fromSource && variableSource != null && dict.TryGetValue("Name", out obj))
			{
				object value;
				dict.TryGetValue("IsGlobal", out value);
				if (!dict.TryGetValue("IsGlobal", out value) || !Convert.ToBoolean(value))
				{
					sharedVariable = variableSource.GetVariable(obj as string);
				}
				else
				{
					if (JSONDeserializationDeprecated.globalVariables == null)
					{
						JSONDeserializationDeprecated.globalVariables = GlobalVariables.Instance;
					}
					if (JSONDeserializationDeprecated.globalVariables != null)
					{
						sharedVariable = JSONDeserializationDeprecated.globalVariables.GetVariable(obj as string);
					}
				}
			}
			Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(dict["Type"] as string);
			if (typeWithinAssembly == null)
			{
				return null;
			}
			bool flag = true;
			if (sharedVariable == null || !(flag = sharedVariable.GetType().Equals(typeWithinAssembly)))
			{
				sharedVariable = (TaskUtility.CreateInstance(typeWithinAssembly) as SharedVariable);
				sharedVariable.Name = (dict["Name"] as string);
				object obj2;
				if (dict.TryGetValue("IsShared", out obj2))
				{
					sharedVariable.IsShared = Convert.ToBoolean(obj2);
				}
				if (dict.TryGetValue("IsGlobal", out obj2))
				{
					sharedVariable.IsGlobal = Convert.ToBoolean(obj2);
				}
				if (dict.TryGetValue("NetworkSync", out obj2))
				{
					sharedVariable.NetworkSync = Convert.ToBoolean(obj2);
				}
				if (!sharedVariable.IsGlobal && dict.TryGetValue("PropertyMapping", out obj2))
				{
					sharedVariable.PropertyMapping = (obj2 as string);
					if (dict.TryGetValue("PropertyMappingOwner", out obj2))
					{
						sharedVariable.PropertyMappingOwner = (JSONDeserializationDeprecated.IndexToUnityObject(Convert.ToInt32(obj2), unityObjects) as GameObject);
					}
					sharedVariable.InitializePropertyMapping(variableSource as BehaviorSource);
				}
				if (!flag)
				{
					sharedVariable.IsShared = true;
				}
				JSONDeserializationDeprecated.DeserializeObject(null, sharedVariable, dict, variableSource, unityObjects);
			}
			return sharedVariable;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000DD50 File Offset: 0x0000BF50
		private static void DeserializeObject(Task task, object obj, Dictionary<string, object> dict, IVariableSource variableSource, List<UnityEngine.Object> unityObjects)
		{
			if (dict == null)
			{
				return;
			}
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				object obj2;
				if (dict.TryGetValue(allFields[i].FieldType + "," + allFields[i].Name, out obj2) || dict.TryGetValue(allFields[i].Name, out obj2))
				{
					if (typeof(IList).IsAssignableFrom(allFields[i].FieldType))
					{
						IList list = obj2 as IList;
						if (list != null)
						{
							Type type;
							if (allFields[i].FieldType.IsArray)
							{
								type = allFields[i].FieldType.GetElementType();
							}
							else
							{
								Type type2 = allFields[i].FieldType;
								while (!type2.IsGenericType)
								{
									type2 = type2.BaseType;
								}
								type = type2.GetGenericArguments()[0];
							}
							bool flag = type.Equals(typeof(Task)) || type.IsSubclassOf(typeof(Task));
							if (flag)
							{
								if (JSONDeserializationDeprecated.taskIDs != null)
								{
									List<int> list2 = new List<int>();
									for (int j = 0; j < list.Count; j++)
									{
										list2.Add(Convert.ToInt32(list[j]));
									}
									JSONDeserializationDeprecated.taskIDs.Add(new JSONDeserializationDeprecated.TaskField(task, allFields[i]), list2);
								}
							}
							else if (allFields[i].FieldType.IsArray)
							{
								Array array = Array.CreateInstance(type, list.Count);
								for (int k = 0; k < list.Count; k++)
								{
									array.SetValue(JSONDeserializationDeprecated.ValueToObject(task, type, list[k], variableSource, unityObjects), k);
								}
								allFields[i].SetValue(obj, array);
							}
							else
							{
								IList list3;
								if (allFields[i].FieldType.IsGenericType)
								{
									list3 = (TaskUtility.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
									{
										type
									})) as IList);
								}
								else
								{
									list3 = (TaskUtility.CreateInstance(allFields[i].FieldType) as IList);
								}
								for (int l = 0; l < list.Count; l++)
								{
									list3.Add(JSONDeserializationDeprecated.ValueToObject(task, type, list[l], variableSource, unityObjects));
								}
								allFields[i].SetValue(obj, list3);
							}
						}
					}
					else
					{
						Type fieldType = allFields[i].FieldType;
						if (fieldType.Equals(typeof(Task)) || fieldType.IsSubclassOf(typeof(Task)))
						{
							if (TaskUtility.HasAttribute(allFields[i], typeof(InspectTaskAttribute)))
							{
								Dictionary<string, object> dictionary = obj2 as Dictionary<string, object>;
								Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(dictionary["ObjectType"] as string);
								if (typeWithinAssembly != null)
								{
									Task task2 = TaskUtility.CreateInstance(typeWithinAssembly) as Task;
									JSONDeserializationDeprecated.DeserializeObject(task2, task2, dictionary, variableSource, unityObjects);
									allFields[i].SetValue(task, task2);
								}
							}
							else if (JSONDeserializationDeprecated.taskIDs != null)
							{
								List<int> list4 = new List<int>();
								list4.Add(Convert.ToInt32(obj2));
								JSONDeserializationDeprecated.taskIDs.Add(new JSONDeserializationDeprecated.TaskField(task, allFields[i]), list4);
							}
						}
						else
						{
							allFields[i].SetValue(obj, JSONDeserializationDeprecated.ValueToObject(task, fieldType, obj2, variableSource, unityObjects));
						}
					}
				}
				else if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType) && !allFields[i].FieldType.IsAbstract)
				{
					if (dict.TryGetValue(allFields[i].FieldType + "," + allFields[i].Name, out obj2))
					{
						SharedVariable sharedVariable = TaskUtility.CreateInstance(allFields[i].FieldType) as SharedVariable;
						sharedVariable.SetValue(JSONDeserializationDeprecated.ValueToObject(task, allFields[i].FieldType, obj2, variableSource, unityObjects));
						allFields[i].SetValue(obj, sharedVariable);
					}
					else
					{
						SharedVariable value = TaskUtility.CreateInstance(allFields[i].FieldType) as SharedVariable;
						allFields[i].SetValue(obj, value);
					}
				}
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000E170 File Offset: 0x0000C370
		private static object ValueToObject(Task task, Type type, object obj, IVariableSource variableSource, List<UnityEngine.Object> unityObjects)
		{
			if (type.Equals(typeof(SharedVariable)) || type.IsSubclassOf(typeof(SharedVariable)))
			{
				SharedVariable sharedVariable = JSONDeserializationDeprecated.DeserializeSharedVariable(obj as Dictionary<string, object>, variableSource, false, unityObjects);
				if (sharedVariable == null)
				{
					sharedVariable = (TaskUtility.CreateInstance(type) as SharedVariable);
				}
				return sharedVariable;
			}
			if (type.Equals(typeof(UnityEngine.Object)) || type.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				return JSONDeserializationDeprecated.IndexToUnityObject(Convert.ToInt32(obj), unityObjects);
			}
			if (!type.IsPrimitive)
			{
				if (!type.Equals(typeof(string)))
				{
					goto IL_C5;
				}
			}
			try
			{
				return Convert.ChangeType(obj, type);
			}
			catch (Exception)
			{
				return null;
			}
			IL_C5:
			if (type.IsSubclassOf(typeof(Enum)))
			{
				try
				{
					return Enum.Parse(type, (string)obj);
				}
				catch (Exception)
				{
					return null;
				}
			}
			if (type.Equals(typeof(Vector2)))
			{
				return JSONDeserializationDeprecated.StringToVector2((string)obj);
			}
			if (type.Equals(typeof(Vector3)))
			{
				return JSONDeserializationDeprecated.StringToVector3((string)obj);
			}
			if (type.Equals(typeof(Vector4)))
			{
				return JSONDeserializationDeprecated.StringToVector4((string)obj);
			}
			if (type.Equals(typeof(Quaternion)))
			{
				return JSONDeserializationDeprecated.StringToQuaternion((string)obj);
			}
			if (type.Equals(typeof(Matrix4x4)))
			{
				return JSONDeserializationDeprecated.StringToMatrix4x4((string)obj);
			}
			if (type.Equals(typeof(Color)))
			{
				return JSONDeserializationDeprecated.StringToColor((string)obj);
			}
			if (type.Equals(typeof(Rect)))
			{
				return JSONDeserializationDeprecated.StringToRect((string)obj);
			}
			if (type.Equals(typeof(LayerMask)))
			{
				return JSONDeserializationDeprecated.ValueToLayerMask(Convert.ToInt32(obj));
			}
			if (type.Equals(typeof(AnimationCurve)))
			{
				return JSONDeserializationDeprecated.ValueToAnimationCurve((Dictionary<string, object>)obj);
			}
			object obj2 = TaskUtility.CreateInstance(type);
			JSONDeserializationDeprecated.DeserializeObject(task, obj2, obj as Dictionary<string, object>, variableSource, unityObjects);
			return obj2;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000E41C File Offset: 0x0000C61C
		private static Vector2 StringToVector2(string vector2String)
		{
			string[] array = vector2String.Substring(1, vector2String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000E460 File Offset: 0x0000C660
		private static Vector3 StringToVector3(string vector3String)
		{
			string[] array = vector3String.Substring(1, vector3String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000E4AC File Offset: 0x0000C6AC
		private static Vector4 StringToVector4(string vector4String)
		{
			string[] array = vector4String.Substring(1, vector4String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000E500 File Offset: 0x0000C700
		private static Quaternion StringToQuaternion(string quaternionString)
		{
			string[] array = quaternionString.Substring(1, quaternionString.Length - 2).Split(new char[]
			{
				','
			});
			return new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000E554 File Offset: 0x0000C754
		private static Matrix4x4 StringToMatrix4x4(string matrixString)
		{
			string[] array = matrixString.Split(null);
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = float.Parse(array[0]);
			result.m01 = float.Parse(array[1]);
			result.m02 = float.Parse(array[2]);
			result.m03 = float.Parse(array[3]);
			result.m10 = float.Parse(array[4]);
			result.m11 = float.Parse(array[5]);
			result.m12 = float.Parse(array[6]);
			result.m13 = float.Parse(array[7]);
			result.m20 = float.Parse(array[8]);
			result.m21 = float.Parse(array[9]);
			result.m22 = float.Parse(array[10]);
			result.m23 = float.Parse(array[11]);
			result.m30 = float.Parse(array[12]);
			result.m31 = float.Parse(array[13]);
			result.m32 = float.Parse(array[14]);
			result.m33 = float.Parse(array[15]);
			return result;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000E66C File Offset: 0x0000C86C
		private static Color StringToColor(string colorString)
		{
			string[] array = colorString.Substring(5, colorString.Length - 6).Split(new char[]
			{
				','
			});
			return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000E6C0 File Offset: 0x0000C8C0
		private static Rect StringToRect(string rectString)
		{
			string[] array = rectString.Substring(1, rectString.Length - 2).Split(new char[]
			{
				','
			});
			return new Rect(float.Parse(array[0].Substring(2, array[0].Length - 2)), float.Parse(array[1].Substring(3, array[1].Length - 3)), float.Parse(array[2].Substring(7, array[2].Length - 7)), float.Parse(array[3].Substring(8, array[3].Length - 8)));
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000E754 File Offset: 0x0000C954
		private static LayerMask ValueToLayerMask(int value)
		{
			LayerMask result = default(LayerMask);
			result.value = value;
			return result;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000E774 File Offset: 0x0000C974
		private static AnimationCurve ValueToAnimationCurve(Dictionary<string, object> value)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			object obj;
			if (value.TryGetValue("Keys", out obj))
			{
				List<object> list = obj as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					List<object> list2 = list[i] as List<object>;
					Keyframe keyframe = new Keyframe((float)Convert.ChangeType(list2[0], typeof(float)), (float)Convert.ChangeType(list2[1], typeof(float)), (float)Convert.ChangeType(list2[2], typeof(float)), (float)Convert.ChangeType(list2[3], typeof(float)));
					keyframe.tangentMode = (int)Convert.ChangeType(list2[4], typeof(int));
					animationCurve.AddKey(keyframe);
				}
			}
			if (value.TryGetValue("PreWrapMode", out obj))
			{
				animationCurve.preWrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), (string)obj);
			}
			if (value.TryGetValue("PostWrapMode", out obj))
			{
				animationCurve.postWrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), (string)obj);
			}
			return animationCurve;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000E8D0 File Offset: 0x0000CAD0
		private static UnityEngine.Object IndexToUnityObject(int index, List<UnityEngine.Object> unityObjects)
		{
			if (index < 0 || index >= unityObjects.Count)
			{
				return null;
			}
			return unityObjects[index];
		}

		// Token: 0x0400008F RID: 143
		private static Dictionary<JSONDeserializationDeprecated.TaskField, List<int>> taskIDs = null;

		// Token: 0x04000090 RID: 144
		private static GlobalVariables globalVariables = null;

		// Token: 0x04000091 RID: 145
		private static Dictionary<int, Dictionary<string, object>> serializationCache = new Dictionary<int, Dictionary<string, object>>();

		// Token: 0x0200001F RID: 31
		private struct TaskField
		{
			// Token: 0x0600017D RID: 381 RVA: 0x0000E8F0 File Offset: 0x0000CAF0
			public TaskField(Task t, FieldInfo f)
			{
				this.task = t;
				this.fieldInfo = f;
			}

			// Token: 0x04000092 RID: 146
			public Task task;

			// Token: 0x04000093 RID: 147
			public FieldInfo fieldInfo;
		}
	}
}
