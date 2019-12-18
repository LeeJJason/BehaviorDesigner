using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200001C RID: 28
	public class JSONDeserialization : UnityEngine.Object
	{
		// Token: 0x06000156 RID: 342 RVA: 0x0000BBB0 File Offset: 0x00009DB0
		public static void Load(TaskSerializationData taskData, BehaviorSource behaviorSource)
		{
			if (taskData != null && string.IsNullOrEmpty(taskData.Version))
			{
				JSONDeserializationDeprecated.Load(taskData, behaviorSource);
				return;
			}
			behaviorSource.EntryTask = null;
			behaviorSource.RootTask = null;
			behaviorSource.DetachedTasks = null;
			behaviorSource.Variables = null;
			Dictionary<string, object> dictionary;
			if (!JSONDeserialization.serializationCache.TryGetValue(taskData.JSONSerialization.GetHashCode(), out dictionary))
			{
				dictionary = (MiniJSON.Deserialize(taskData.JSONSerialization) as Dictionary<string, object>);
				JSONDeserialization.serializationCache.Add(taskData.JSONSerialization.GetHashCode(), dictionary);
			}
			if (dictionary == null)
			{
				Debug.Log("Failed to deserialize");
				return;
			}
			JSONDeserialization.taskIDs = new Dictionary<JSONDeserialization.TaskField, List<int>>();
			JSONDeserialization.updatedSerialization = (taskData.Version.CompareTo("1.5.7") >= 0);
			Dictionary<int, Task> dictionary2 = new Dictionary<int, Task>();
			JSONDeserialization.DeserializeVariables(behaviorSource, dictionary, taskData.fieldSerializationData.unityObjects);
			if (dictionary.ContainsKey("EntryTask"))
			{
				behaviorSource.EntryTask = JSONDeserialization.DeserializeTask(behaviorSource, dictionary["EntryTask"] as Dictionary<string, object>, ref dictionary2, taskData.fieldSerializationData.unityObjects);
			}
			if (dictionary.ContainsKey("RootTask"))
			{
				behaviorSource.RootTask = JSONDeserialization.DeserializeTask(behaviorSource, dictionary["RootTask"] as Dictionary<string, object>, ref dictionary2, taskData.fieldSerializationData.unityObjects);
			}
			if (dictionary.ContainsKey("DetachedTasks"))
			{
				List<Task> list = new List<Task>();
				foreach (object obj in (dictionary["DetachedTasks"] as IEnumerable))
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)obj;
					list.Add(JSONDeserialization.DeserializeTask(behaviorSource, dict, ref dictionary2, taskData.fieldSerializationData.unityObjects));
				}
				behaviorSource.DetachedTasks = list;
			}
			if (JSONDeserialization.taskIDs != null && JSONDeserialization.taskIDs.Count > 0)
			{
				foreach (JSONDeserialization.TaskField key in JSONDeserialization.taskIDs.Keys)
				{
					List<int> list2 = JSONDeserialization.taskIDs[key];
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
				JSONDeserialization.taskIDs = null;
			}
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		public static void Load(string serialization, GlobalVariables globalVariables, string version)
		{
			if (globalVariables == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(version))
			{
				JSONDeserializationDeprecated.Load(serialization, globalVariables);
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
			JSONDeserialization.updatedSerialization = (globalVariables.Version.CompareTo("1.5.7") >= 0);
			JSONDeserialization.DeserializeVariables(globalVariables, dictionary, globalVariables.VariableData.fieldSerializationData.unityObjects);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000C038 File Offset: 0x0000A238
		private static void DeserializeVariables(IVariableSource variableSource, Dictionary<string, object> dict, List<UnityEngine.Object> unityObjects)
		{
			object obj;
			if (dict.TryGetValue("Variables", out obj))
			{
				List<SharedVariable> list = new List<SharedVariable>();
				IList list2 = obj as IList;
				for (int i = 0; i < list2.Count; i++)
				{
					SharedVariable item = JSONDeserialization.DeserializeSharedVariable(list2[i] as Dictionary<string, object>, variableSource, true, unityObjects);
					list.Add(item);
				}
				variableSource.SetAllVariables(list);
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000C0A0 File Offset: 0x0000A2A0
		public static Task DeserializeTask(BehaviorSource behaviorSource, Dictionary<string, object> dict, ref Dictionary<int, Task> IDtoTask, List<UnityEngine.Object> unityObjects)
		{
			Task task = null;
			try
			{
				Type type = TaskUtility.GetTypeWithinAssembly(dict["Type"] as string);
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
				if (task is UnknownTask)
				{
					UnknownTask unknownTask = task as UnknownTask;
					unknownTask.JSONSerialization = MiniJSON.Serialize(dict);
				}
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
			task.NodeData = JSONDeserialization.DeserializeNodeData(dict["NodeData"] as Dictionary<string, object>, task);
			if (task.GetType().Equals(typeof(UnknownTask)) || task.GetType().Equals(typeof(UnknownParentTask)))
			{
				if (!task.FriendlyName.Contains("Unknown "))
				{
					task.FriendlyName = string.Format("Unknown {0}", task.FriendlyName);
				}
				task.NodeData.Comment = "Unknown Task. Right click and Replace to locate new task.";
			}
			JSONDeserialization.DeserializeObject(task, task, dict, behaviorSource, unityObjects);
			if (task is ParentTask && dict.TryGetValue("Children", out obj))
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask != null)
				{
					foreach (object obj2 in (obj as IEnumerable))
					{
						Dictionary<string, object> dict2 = (Dictionary<string, object>)obj2;
						Task child = JSONDeserialization.DeserializeTask(behaviorSource, dict2, ref IDtoTask, unityObjects);
						int index = (parentTask.Children != null) ? parentTask.Children.Count : 0;
						parentTask.AddChild(child, index);
					}
				}
			}
			return task;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000C33C File Offset: 0x0000A53C
		private static NodeData DeserializeNodeData(Dictionary<string, object> dict, Task task)
		{
			NodeData nodeData = new NodeData();
			object obj;
			if (dict.TryGetValue("Offset", out obj))
			{
				nodeData.Offset = JSONDeserialization.StringToVector2((string)obj);
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

		// Token: 0x0600015B RID: 347 RVA: 0x0000C490 File Offset: 0x0000A690
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
					if (JSONDeserialization.globalVariables == null)
					{
						JSONDeserialization.globalVariables = GlobalVariables.Instance;
					}
					if (JSONDeserialization.globalVariables != null)
					{
						sharedVariable = JSONDeserialization.globalVariables.GetVariable(obj as string);
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
						sharedVariable.PropertyMappingOwner = (JSONDeserialization.IndexToUnityObject(Convert.ToInt32(obj2), unityObjects) as GameObject);
					}
					sharedVariable.InitializePropertyMapping(variableSource as BehaviorSource);
				}
				if (!flag)
				{
					sharedVariable.IsShared = true;
				}
				JSONDeserialization.DeserializeObject(null, sharedVariable, dict, variableSource, unityObjects);
			}
			return sharedVariable;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000C670 File Offset: 0x0000A870
		private static void DeserializeObject(Task task, object obj, Dictionary<string, object> dict, IVariableSource variableSource, List<UnityEngine.Object> unityObjects)
		{
			if (dict == null)
			{
				return;
			}
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				string key = (!JSONDeserialization.updatedSerialization) ? (allFields[i].FieldType.Name.GetHashCode() + allFields[i].Name.GetHashCode()).ToString() : (allFields[i].FieldType.Name + allFields[i].Name);
				object obj2;
				if (dict.TryGetValue(key, out obj2))
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
								if (JSONDeserialization.taskIDs != null)
								{
									List<int> list2 = new List<int>();
									for (int j = 0; j < list.Count; j++)
									{
										list2.Add(Convert.ToInt32(list[j]));
									}
									JSONDeserialization.taskIDs.Add(new JSONDeserialization.TaskField(task, allFields[i]), list2);
								}
							}
							else if (allFields[i].FieldType.IsArray)
							{
								Array array = Array.CreateInstance(type, list.Count);
								for (int k = 0; k < list.Count; k++)
								{
									if (list[k] == null)
									{
										array.SetValue(null, k);
									}
									else
									{
										array.SetValue(JSONDeserialization.ValueToObject(task, type, list[k], variableSource, unityObjects), k);
									}
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
									if (list[l] == null)
									{
										list3.Add(null);
									}
									else
									{
										list3.Add(JSONDeserialization.ValueToObject(task, type, list[l], variableSource, unityObjects));
									}
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
								Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(dictionary["Type"] as string);
								if (typeWithinAssembly != null)
								{
									Task task2 = TaskUtility.CreateInstance(typeWithinAssembly) as Task;
									JSONDeserialization.DeserializeObject(task2, task2, dictionary, variableSource, unityObjects);
									allFields[i].SetValue(task, task2);
								}
							}
							else if (JSONDeserialization.taskIDs != null)
							{
								List<int> list4 = new List<int>();
								list4.Add(Convert.ToInt32(obj2));
								JSONDeserialization.taskIDs.Add(new JSONDeserialization.TaskField(task, allFields[i]), list4);
							}
						}
						else
						{
							allFields[i].SetValue(obj, JSONDeserialization.ValueToObject(task, fieldType, obj2, variableSource, unityObjects));
						}
					}
				}
				else if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType) && !allFields[i].FieldType.IsAbstract)
				{
					if (dict.TryGetValue((allFields[i].FieldType.Name.GetHashCode() + allFields[i].Name.GetHashCode()).ToString(), out obj2))
					{
						SharedVariable sharedVariable = TaskUtility.CreateInstance(allFields[i].FieldType) as SharedVariable;
						sharedVariable.SetValue(JSONDeserialization.ValueToObject(task, allFields[i].FieldType, obj2, variableSource, unityObjects));
						allFields[i].SetValue(obj, sharedVariable);
					}
					else
					{
						SharedVariable sharedVariable2 = TaskUtility.CreateInstance(allFields[i].FieldType) as SharedVariable;
						SharedVariable sharedVariable3 = allFields[i].GetValue(obj) as SharedVariable;
						if (sharedVariable3 != null)
						{
							sharedVariable2.SetValue(sharedVariable3.GetValue());
						}
						allFields[i].SetValue(obj, sharedVariable2);
					}
				}
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000CB28 File Offset: 0x0000AD28
		private static object ValueToObject(Task task, Type type, object obj, IVariableSource variableSource, List<UnityEngine.Object> unityObjects)
		{
			if (typeof(SharedVariable).IsAssignableFrom(type))
			{
				SharedVariable sharedVariable = JSONDeserialization.DeserializeSharedVariable(obj as Dictionary<string, object>, variableSource, false, unityObjects);
				if (sharedVariable == null)
				{
					sharedVariable = (TaskUtility.CreateInstance(type) as SharedVariable);
				}
				return sharedVariable;
			}
			if (type.Equals(typeof(UnityEngine.Object)) || type.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				return JSONDeserialization.IndexToUnityObject(Convert.ToInt32(obj), unityObjects);
			}
			if (!type.IsPrimitive)
			{
				if (!type.Equals(typeof(string)))
				{
					goto IL_B0;
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
			IL_B0:
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
				return JSONDeserialization.StringToVector2((string)obj);
			}
			if (type.Equals(typeof(Vector3)))
			{
				return JSONDeserialization.StringToVector3((string)obj);
			}
			if (type.Equals(typeof(Vector4)))
			{
				return JSONDeserialization.StringToVector4((string)obj);
			}
			if (type.Equals(typeof(Quaternion)))
			{
				return JSONDeserialization.StringToQuaternion((string)obj);
			}
			if (type.Equals(typeof(Matrix4x4)))
			{
				return JSONDeserialization.StringToMatrix4x4((string)obj);
			}
			if (type.Equals(typeof(Color)))
			{
				return JSONDeserialization.StringToColor((string)obj);
			}
			if (type.Equals(typeof(Rect)))
			{
				return JSONDeserialization.StringToRect((string)obj);
			}
			if (type.Equals(typeof(LayerMask)))
			{
				return JSONDeserialization.ValueToLayerMask(Convert.ToInt32(obj));
			}
			if (type.Equals(typeof(AnimationCurve)))
			{
				return JSONDeserialization.ValueToAnimationCurve((Dictionary<string, object>)obj);
			}
			object obj2 = TaskUtility.CreateInstance(type);
			JSONDeserialization.DeserializeObject(task, obj2, obj as Dictionary<string, object>, variableSource, unityObjects);
			return obj2;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000CDC0 File Offset: 0x0000AFC0
		private static Vector2 StringToVector2(string vector2String)
		{
			string[] array = vector2String.Substring(1, vector2String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000CE04 File Offset: 0x0000B004
		private static Vector3 StringToVector3(string vector3String)
		{
			string[] array = vector3String.Substring(1, vector3String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000CE50 File Offset: 0x0000B050
		private static Vector4 StringToVector4(string vector4String)
		{
			string[] array = vector4String.Substring(1, vector4String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000CEA4 File Offset: 0x0000B0A4
		private static Quaternion StringToQuaternion(string quaternionString)
		{
			string[] array = quaternionString.Substring(1, quaternionString.Length - 2).Split(new char[]
			{
				','
			});
			return new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000CEF8 File Offset: 0x0000B0F8
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

		// Token: 0x06000163 RID: 355 RVA: 0x0000D010 File Offset: 0x0000B210
		private static Color StringToColor(string colorString)
		{
			string[] array = colorString.Substring(5, colorString.Length - 6).Split(new char[]
			{
				','
			});
			return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000D064 File Offset: 0x0000B264
		private static Rect StringToRect(string rectString)
		{
			string[] array = rectString.Substring(1, rectString.Length - 2).Split(new char[]
			{
				','
			});
			return new Rect(float.Parse(array[0].Substring(2, array[0].Length - 2)), float.Parse(array[1].Substring(3, array[1].Length - 3)), float.Parse(array[2].Substring(7, array[2].Length - 7)), float.Parse(array[3].Substring(8, array[3].Length - 8)));
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000D0F8 File Offset: 0x0000B2F8
		private static LayerMask ValueToLayerMask(int value)
		{
			LayerMask result = default(LayerMask);
			result.value = value;
			return result;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000D118 File Offset: 0x0000B318
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

		// Token: 0x06000167 RID: 359 RVA: 0x0000D274 File Offset: 0x0000B474
		private static UnityEngine.Object IndexToUnityObject(int index, List<UnityEngine.Object> unityObjects)
		{
			if (index < 0 || index >= unityObjects.Count)
			{
				return null;
			}
			return unityObjects[index];
		}

		// Token: 0x04000089 RID: 137
		private static Dictionary<JSONDeserialization.TaskField, List<int>> taskIDs = null;

		// Token: 0x0400008A RID: 138
		private static GlobalVariables globalVariables = null;

		// Token: 0x0400008B RID: 139
		private static bool updatedSerialization;

		// Token: 0x0400008C RID: 140
		private static Dictionary<int, Dictionary<string, object>> serializationCache = new Dictionary<int, Dictionary<string, object>>();

		// Token: 0x0200001D RID: 29
		private struct TaskField
		{
			// Token: 0x06000168 RID: 360 RVA: 0x0000D294 File Offset: 0x0000B494
			public TaskField(Task t, FieldInfo f)
			{
				this.task = t;
				this.fieldInfo = f;
			}

			// Token: 0x0400008D RID: 141
			public Task task;

			// Token: 0x0400008E RID: 142
			public FieldInfo fieldInfo;
		}
	}
}
