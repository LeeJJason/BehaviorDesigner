using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

// Token: 0x02000012 RID: 18
public static class BinaryDeserialization
{
	// Token: 0x060000E4 RID: 228 RVA: 0x00007E30 File Offset: 0x00006030
	public static void Load(BehaviorSource behaviorSource)
	{
		BinaryDeserialization.Load(behaviorSource.TaskData, behaviorSource);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00007E40 File Offset: 0x00006040
	public static void Load(TaskSerializationData taskData, BehaviorSource behaviorSource)
	{
		if (taskData != null && string.IsNullOrEmpty(taskData.Version))
		{
			BinaryDeserializationDeprecated.Load(taskData, behaviorSource);
			return;
		}
		behaviorSource.EntryTask = null;
		behaviorSource.RootTask = null;
		behaviorSource.DetachedTasks = null;
		behaviorSource.Variables = null;
		FieldSerializationData fieldSerializationData;
		if (taskData == null || (fieldSerializationData = taskData.fieldSerializationData).byteData == null || fieldSerializationData.byteData.Count == 0)
		{
			return;
		}
		fieldSerializationData.byteDataArray = fieldSerializationData.byteData.ToArray();
		BinaryDeserialization.taskIDs = null;
		BinaryDeserialization.updatedSerialization = (taskData.Version.CompareTo("1.5.7") >= 0);
		if (taskData.variableStartIndex != null)
		{
			List<SharedVariable> list = new List<SharedVariable>();
			Dictionary<int, int> dictionary = ObjectPool.Get<Dictionary<int, int>>();
			for (int i = 0; i < taskData.variableStartIndex.Count; i++)
			{
				int num = taskData.variableStartIndex[i];
				int num2;
				if (i + 1 < taskData.variableStartIndex.Count)
				{
					num2 = taskData.variableStartIndex[i + 1];
				}
				else if (taskData.startIndex != null && taskData.startIndex.Count > 0)
				{
					num2 = taskData.startIndex[0];
				}
				else
				{
					num2 = fieldSerializationData.startIndex.Count;
				}
				dictionary.Clear();
				for (int j = num; j < num2; j++)
				{
					dictionary.Add(fieldSerializationData.fieldNameHash[j], fieldSerializationData.startIndex[j]);
				}
				SharedVariable sharedVariable = BinaryDeserialization.BytesToSharedVariable(fieldSerializationData, dictionary, fieldSerializationData.byteDataArray, taskData.variableStartIndex[i], behaviorSource, false, 0);
				if (sharedVariable != null)
				{
					list.Add(sharedVariable);
				}
			}
			ObjectPool.Return<Dictionary<int, int>>(dictionary);
			behaviorSource.Variables = list;
		}
		List<Task> list2 = new List<Task>();
		if (taskData.types != null)
		{
			for (int k = 0; k < taskData.types.Count; k++)
			{
				BinaryDeserialization.LoadTask(taskData, fieldSerializationData, ref list2, ref behaviorSource);
			}
		}
		if (taskData.parentIndex.Count != list2.Count)
		{
			Debug.LogError("Deserialization Error: parent index count does not match task list count");
			return;
		}
		for (int l = 0; l < taskData.parentIndex.Count; l++)
		{
			if (taskData.parentIndex[l] == -1)
			{
				if (behaviorSource.EntryTask == null)
				{
					behaviorSource.EntryTask = list2[l];
				}
				else
				{
					if (behaviorSource.DetachedTasks == null)
					{
						behaviorSource.DetachedTasks = new List<Task>();
					}
					behaviorSource.DetachedTasks.Add(list2[l]);
				}
			}
			else if (taskData.parentIndex[l] == 0)
			{
				behaviorSource.RootTask = list2[l];
			}
			else if (taskData.parentIndex[l] != -1)
			{
				ParentTask parentTask = list2[taskData.parentIndex[l]] as ParentTask;
				if (parentTask != null)
				{
					int index = (parentTask.Children != null) ? parentTask.Children.Count : 0;
					parentTask.AddChild(list2[l], index);
				}
			}
		}
		if (BinaryDeserialization.taskIDs != null)
		{
			foreach (BinaryDeserialization.ObjectFieldMap objectFieldMap in BinaryDeserialization.taskIDs.Keys)
			{
				List<int> list3 = BinaryDeserialization.taskIDs[objectFieldMap];
				Type fieldType = objectFieldMap.fieldInfo.FieldType;
				if (typeof(IList).IsAssignableFrom(fieldType))
				{
					if (fieldType.IsArray)
					{
						Type elementType = fieldType.GetElementType();
						Array array = Array.CreateInstance(elementType, list3.Count);
						for (int m = 0; m < array.Length; m++)
						{
							array.SetValue(list2[list3[m]], m);
						}
						objectFieldMap.fieldInfo.SetValue(objectFieldMap.obj, array);
					}
					else
					{
						Type type = fieldType.GetGenericArguments()[0];
						IList list4 = TaskUtility.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
						{
							type
						})) as IList;
						for (int n = 0; n < list3.Count; n++)
						{
							list4.Add(list2[list3[n]]);
						}
						objectFieldMap.fieldInfo.SetValue(objectFieldMap.obj, list4);
					}
				}
				else
				{
					objectFieldMap.fieldInfo.SetValue(objectFieldMap.obj, list2[list3[0]]);
				}
			}
		}
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00008320 File Offset: 0x00006520
	public static void Load(GlobalVariables globalVariables, string version)
	{
		if (globalVariables == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(version))
		{
			BinaryDeserializationDeprecated.Load(globalVariables);
			return;
		}
		globalVariables.Variables = null;
		FieldSerializationData fieldSerializationData;
		if (globalVariables.VariableData == null || (fieldSerializationData = globalVariables.VariableData.fieldSerializationData).byteData == null || fieldSerializationData.byteData.Count == 0)
		{
			return;
		}
		if (fieldSerializationData.typeName.Count > 0)
		{
			BinaryDeserializationDeprecated.Load(globalVariables);
			return;
		}
		VariableSerializationData variableData = globalVariables.VariableData;
		fieldSerializationData.byteDataArray = fieldSerializationData.byteData.ToArray();
		BinaryDeserialization.updatedSerialization = (globalVariables.Version.CompareTo("1.5.7") >= 0);
		if (variableData.variableStartIndex != null)
		{
			List<SharedVariable> list = new List<SharedVariable>();
			Dictionary<int, int> dictionary = ObjectPool.Get<Dictionary<int, int>>();
			for (int i = 0; i < variableData.variableStartIndex.Count; i++)
			{
				int num = variableData.variableStartIndex[i];
				int num2;
				if (i + 1 < variableData.variableStartIndex.Count)
				{
					num2 = variableData.variableStartIndex[i + 1];
				}
				else
				{
					num2 = fieldSerializationData.startIndex.Count;
				}
				dictionary.Clear();
				for (int j = num; j < num2; j++)
				{
					dictionary.Add(fieldSerializationData.fieldNameHash[j], fieldSerializationData.startIndex[j]);
				}
				SharedVariable sharedVariable = BinaryDeserialization.BytesToSharedVariable(fieldSerializationData, dictionary, fieldSerializationData.byteDataArray, variableData.variableStartIndex[i], globalVariables, false, 0);
				if (sharedVariable != null)
				{
					list.Add(sharedVariable);
				}
			}
			ObjectPool.Return<Dictionary<int, int>>(dictionary);
			globalVariables.Variables = list;
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x000084C8 File Offset: 0x000066C8
	public static void LoadTask(TaskSerializationData taskSerializationData, FieldSerializationData fieldSerializationData, ref List<Task> taskList, ref BehaviorSource behaviorSource)
	{
		int count = taskList.Count;
		int num = taskSerializationData.startIndex[count];
		int num2;
		if (count + 1 < taskSerializationData.startIndex.Count)
		{
			num2 = taskSerializationData.startIndex[count + 1];
		}
		else
		{
			num2 = fieldSerializationData.startIndex.Count;
		}
		Dictionary<int, int> dictionary = ObjectPool.Get<Dictionary<int, int>>();
		dictionary.Clear();
		for (int i = num; i < num2; i++)
		{
			if (!dictionary.ContainsKey(fieldSerializationData.fieldNameHash[i]))
			{
				dictionary.Add(fieldSerializationData.fieldNameHash[i], fieldSerializationData.startIndex[i]);
			}
		}
		Type type = TaskUtility.GetTypeWithinAssembly(taskSerializationData.types[count]);
		if (type == null)
		{
			bool flag = false;
			for (int j = 0; j < taskSerializationData.parentIndex.Count; j++)
			{
				if (count == taskSerializationData.parentIndex[j])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				type = typeof(UnknownParentTask);
			}
			else
			{
				type = typeof(UnknownTask);
			}
		}
		Task task = TaskUtility.CreateInstance(type) as Task;
		if (task is UnknownTask)
		{
			UnknownTask unknownTask = task as UnknownTask;
			for (int k = num; k < num2; k++)
			{
				unknownTask.fieldNameHash.Add(fieldSerializationData.fieldNameHash[k]);
				unknownTask.startIndex.Add(fieldSerializationData.startIndex[k] - fieldSerializationData.startIndex[num]);
			}
			for (int l = fieldSerializationData.startIndex[num]; l <= fieldSerializationData.startIndex[num2 - 1]; l++)
			{
				unknownTask.dataPosition.Add(fieldSerializationData.dataPosition[l] - fieldSerializationData.dataPosition[fieldSerializationData.startIndex[num]]);
			}
			if (count + 1 < taskSerializationData.startIndex.Count && taskSerializationData.startIndex[count + 1] < fieldSerializationData.dataPosition.Count)
			{
				num2 = fieldSerializationData.dataPosition[taskSerializationData.startIndex[count + 1]];
			}
			else
			{
				num2 = fieldSerializationData.byteData.Count;
			}
			for (int m = fieldSerializationData.dataPosition[fieldSerializationData.startIndex[num]]; m < num2; m++)
			{
				unknownTask.byteData.Add(fieldSerializationData.byteData[m]);
			}
			unknownTask.unityObjects = fieldSerializationData.unityObjects;
		}
		task.Owner = (behaviorSource.Owner.GetObject() as Behavior);
		taskList.Add(task);
		task.ID = (int)BinaryDeserialization.LoadField(fieldSerializationData, dictionary, typeof(int), "ID", 0, null, null, null);
		task.FriendlyName = (string)BinaryDeserialization.LoadField(fieldSerializationData, dictionary, typeof(string), "FriendlyName", 0, null, null, null);
		task.IsInstant = (bool)BinaryDeserialization.LoadField(fieldSerializationData, dictionary, typeof(bool), "IsInstant", 0, null, null, null);
		object obj;
		if ((obj = BinaryDeserialization.LoadField(fieldSerializationData, dictionary, typeof(bool), "Disabled", 0, null, null, null)) != null)
		{
			task.Disabled = (bool)obj;
		}
		BinaryDeserialization.LoadNodeData(fieldSerializationData, dictionary, taskList[count]);
		if (task.GetType().Equals(typeof(UnknownTask)) || task.GetType().Equals(typeof(UnknownParentTask)))
		{
			if (!task.FriendlyName.Contains("Unknown "))
			{
				task.FriendlyName = string.Format("Unknown {0}", task.FriendlyName);
			}
			task.NodeData.Comment = "Unknown Task. Right click and Replace to locate new task.";
		}
		BinaryDeserialization.LoadFields(fieldSerializationData, dictionary, taskList[count], 0, behaviorSource);
		ObjectPool.Return<Dictionary<int, int>>(dictionary);
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x000088E0 File Offset: 0x00006AE0
	private static void LoadNodeData(FieldSerializationData fieldSerializationData, Dictionary<int, int> fieldIndexMap, Task task)
	{
		NodeData nodeData = new NodeData();
		nodeData.Offset = (Vector2)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(Vector2), "NodeDataOffset", 0, null, null, null);
		nodeData.Comment = (string)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), "NodeDataComment", 0, null, null, null);
		nodeData.IsBreakpoint = (bool)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "NodeDataIsBreakpoint", 0, null, null, null);
		nodeData.Collapsed = (bool)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "NodeDataCollapsed", 0, null, null, null);
		object obj = BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(int), "NodeDataColorIndex", 0, null, null, null);
		if (obj != null)
		{
			nodeData.ColorIndex = (int)obj;
		}
		obj = BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(List<string>), "NodeDataWatchedFields", 0, null, null, null);
		if (obj != null)
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
		task.NodeData = nodeData;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00008A54 File Offset: 0x00006C54
	private static void LoadFields(FieldSerializationData fieldSerializationData, Dictionary<int, int> fieldIndexMap, object obj, int hashPrefix, IVariableSource variableSource)
	{
		FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
		for (int i = 0; i < allFields.Length; i++)
		{
			if (!TaskUtility.HasAttribute(allFields[i], typeof(NonSerializedAttribute)) && ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || TaskUtility.HasAttribute(allFields[i], typeof(SerializeField))) && (!(obj is ParentTask) || !allFields[i].Name.Equals("children")))
			{
				object obj2 = BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, allFields[i].FieldType, allFields[i].Name, hashPrefix, variableSource, obj, allFields[i]);
				if (obj2 != null && !object.ReferenceEquals(obj2, null) && !obj2.Equals(null))
				{
					allFields[i].SetValue(obj, obj2);
				}
			}
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00008B38 File Offset: 0x00006D38
	private static object LoadField(FieldSerializationData fieldSerializationData, Dictionary<int, int> fieldIndexMap, Type fieldType, string fieldName, int hashPrefix, IVariableSource variableSource, object obj = null, FieldInfo fieldInfo = null)
	{
		int num = hashPrefix + fieldType.Name.GetHashCode() + fieldName.GetHashCode();
		int num2;
		if (fieldIndexMap.TryGetValue(num, out num2))
		{
			object obj2 = null;
			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				int num3 = BinaryDeserialization.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
				if (fieldType.IsArray)
				{
					Type elementType = fieldType.GetElementType();
					if (elementType == null)
					{
						return null;
					}
					Array array = Array.CreateInstance(elementType, num3);
					for (int i = 0; i < num3; i++)
					{
						object obj3 = BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, elementType, i.ToString(), num / ((!BinaryDeserialization.updatedSerialization) ? 1 : (i + 1)), variableSource, obj, fieldInfo);
						array.SetValue((!object.ReferenceEquals(obj3, null) && !obj3.Equals(null)) ? obj3 : null, i);
					}
					obj2 = array;
				}
				else
				{
					Type type = fieldType;
					while (!type.IsGenericType)
					{
						type = type.BaseType;
					}
					Type type2 = type.GetGenericArguments()[0];
					IList list;
					if (fieldType.IsGenericType)
					{
						list = (TaskUtility.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
						{
							type2
						})) as IList);
					}
					else
					{
						list = (TaskUtility.CreateInstance(fieldType) as IList);
					}
					for (int j = 0; j < num3; j++)
					{
						object obj4 = BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, type2, j.ToString(), num / ((!BinaryDeserialization.updatedSerialization) ? 1 : (j + 1)), variableSource, obj, fieldInfo);
						list.Add((!object.ReferenceEquals(obj4, null) && !obj4.Equals(null)) ? obj4 : null);
					}
					obj2 = list;
				}
			}
			else if (typeof(Task).IsAssignableFrom(fieldType))
			{
				if (fieldInfo != null && TaskUtility.HasAttribute(fieldInfo, typeof(InspectTaskAttribute)))
				{
					string text = BinaryDeserialization.BytesToString(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2], BinaryDeserialization.GetFieldSize(fieldSerializationData, num2));
					if (!string.IsNullOrEmpty(text))
					{
						Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(text);
						if (typeWithinAssembly != null)
						{
							obj2 = TaskUtility.CreateInstance(typeWithinAssembly);
							BinaryDeserialization.LoadFields(fieldSerializationData, fieldIndexMap, obj2, num, variableSource);
						}
					}
				}
				else
				{
					if (BinaryDeserialization.taskIDs == null)
					{
						BinaryDeserialization.taskIDs = new Dictionary<BinaryDeserialization.ObjectFieldMap, List<int>>(new BinaryDeserialization.ObjectFieldMapComparer());
					}
					int item = BinaryDeserialization.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
					BinaryDeserialization.ObjectFieldMap key = new BinaryDeserialization.ObjectFieldMap(obj, fieldInfo);
					if (BinaryDeserialization.taskIDs.ContainsKey(key))
					{
						BinaryDeserialization.taskIDs[key].Add(item);
					}
					else
					{
						List<int> list2 = new List<int>();
						list2.Add(item);
						BinaryDeserialization.taskIDs.Add(key, list2);
					}
				}
			}
			else if (typeof(SharedVariable).IsAssignableFrom(fieldType))
			{
				obj2 = BinaryDeserialization.BytesToSharedVariable(fieldSerializationData, fieldIndexMap, fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2], variableSource, true, num);
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
			{
				int index = BinaryDeserialization.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
				obj2 = BinaryDeserialization.IndexToUnityObject(index, fieldSerializationData);
			}
			else if (fieldType.Equals(typeof(int)) || fieldType.IsEnum)
			{
				obj2 = BinaryDeserialization.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(uint)))
			{
				obj2 = BinaryDeserialization.BytesToUInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(float)))
			{
				obj2 = BinaryDeserialization.BytesToFloat(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(double)))
			{
				obj2 = BinaryDeserialization.BytesToDouble(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(long)))
			{
				obj2 = BinaryDeserialization.BytesToLong(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(bool)))
			{
				obj2 = BinaryDeserialization.BytesToBool(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(string)))
			{
				obj2 = BinaryDeserialization.BytesToString(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2], BinaryDeserialization.GetFieldSize(fieldSerializationData, num2));
			}
			else if (fieldType.Equals(typeof(byte)))
			{
				obj2 = BinaryDeserialization.BytesToByte(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Vector2)))
			{
				obj2 = BinaryDeserialization.BytesToVector2(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Vector3)))
			{
				obj2 = BinaryDeserialization.BytesToVector3(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Vector4)))
			{
				obj2 = BinaryDeserialization.BytesToVector4(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Quaternion)))
			{
				obj2 = BinaryDeserialization.BytesToQuaternion(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Color)))
			{
				obj2 = BinaryDeserialization.BytesToColor(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Rect)))
			{
				obj2 = BinaryDeserialization.BytesToRect(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(Matrix4x4)))
			{
				obj2 = BinaryDeserialization.BytesToMatrix4x4(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(AnimationCurve)))
			{
				obj2 = BinaryDeserialization.BytesToAnimationCurve(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.Equals(typeof(LayerMask)))
			{
				obj2 = BinaryDeserialization.BytesToLayerMask(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num2]);
			}
			else if (fieldType.IsClass || (fieldType.IsValueType && !fieldType.IsPrimitive))
			{
				obj2 = TaskUtility.CreateInstance(fieldType);
				BinaryDeserialization.LoadFields(fieldSerializationData, fieldIndexMap, obj2, num, variableSource);
				return obj2;
			}
			return obj2;
		}
		if (fieldType.IsAbstract)
		{
			return null;
		}
		if (typeof(SharedVariable).IsAssignableFrom(fieldType))
		{
			SharedVariable sharedVariable = TaskUtility.CreateInstance(fieldType) as SharedVariable;
			SharedVariable sharedVariable2 = fieldInfo.GetValue(obj) as SharedVariable;
			if (sharedVariable2 != null)
			{
				sharedVariable.SetValue(sharedVariable2.GetValue());
			}
			return sharedVariable;
		}
		return null;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x000092F4 File Offset: 0x000074F4
	private static int GetFieldSize(FieldSerializationData fieldSerializationData, int fieldIndex)
	{
		return ((fieldIndex + 1 >= fieldSerializationData.dataPosition.Count) ? fieldSerializationData.byteData.Count : fieldSerializationData.dataPosition[fieldIndex + 1]) - fieldSerializationData.dataPosition[fieldIndex];
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00009340 File Offset: 0x00007540
	private static int BytesToInt(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToInt32(bytes, dataPosition);
	}

	// Token: 0x060000ED RID: 237 RVA: 0x0000934C File Offset: 0x0000754C
	private static uint BytesToUInt(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToUInt32(bytes, dataPosition);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00009358 File Offset: 0x00007558
	private static float BytesToFloat(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToSingle(bytes, dataPosition);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00009364 File Offset: 0x00007564
	private static double BytesToDouble(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToDouble(bytes, dataPosition);
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00009370 File Offset: 0x00007570
	private static long BytesToLong(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToInt64(bytes, dataPosition);
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x0000937C File Offset: 0x0000757C
	private static bool BytesToBool(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToBoolean(bytes, dataPosition);
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00009388 File Offset: 0x00007588
	private static string BytesToString(byte[] bytes, int dataPosition, int dataSize)
	{
		if (dataSize == 0)
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(bytes, dataPosition, dataSize);
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000093A4 File Offset: 0x000075A4
	private static byte BytesToByte(byte[] bytes, int dataPosition)
	{
		return bytes[dataPosition];
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000093AC File Offset: 0x000075AC
	private static Color BytesToColor(byte[] bytes, int dataPosition)
	{
		Color black = Color.black;
		black.r = BitConverter.ToSingle(bytes, dataPosition);
		black.g = BitConverter.ToSingle(bytes, dataPosition + 4);
		black.b = BitConverter.ToSingle(bytes, dataPosition + 8);
		black.a = BitConverter.ToSingle(bytes, dataPosition + 12);
		return black;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00009400 File Offset: 0x00007600
	private static Vector2 BytesToVector2(byte[] bytes, int dataPosition)
	{
		Vector2 zero = Vector2.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		return zero;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00009434 File Offset: 0x00007634
	private static Vector3 BytesToVector3(byte[] bytes, int dataPosition)
	{
		Vector3 zero = Vector3.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		zero.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		return zero;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00009478 File Offset: 0x00007678
	private static Vector4 BytesToVector4(byte[] bytes, int dataPosition)
	{
		Vector4 zero = Vector4.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		zero.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		zero.w = BitConverter.ToSingle(bytes, dataPosition + 12);
		return zero;
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000094CC File Offset: 0x000076CC
	private static Quaternion BytesToQuaternion(byte[] bytes, int dataPosition)
	{
		Quaternion identity = Quaternion.identity;
		identity.x = BitConverter.ToSingle(bytes, dataPosition);
		identity.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		identity.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		identity.w = BitConverter.ToSingle(bytes, dataPosition + 12);
		return identity;
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00009520 File Offset: 0x00007720
	private static Rect BytesToRect(byte[] bytes, int dataPosition)
	{
		Rect result = default(Rect);
		result.x = BitConverter.ToSingle(bytes, dataPosition);
		result.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		result.width = BitConverter.ToSingle(bytes, dataPosition + 8);
		result.height = BitConverter.ToSingle(bytes, dataPosition + 12);
		return result;
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00009578 File Offset: 0x00007778
	private static Matrix4x4 BytesToMatrix4x4(byte[] bytes, int dataPosition)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = BitConverter.ToSingle(bytes, dataPosition);
		identity.m01 = BitConverter.ToSingle(bytes, dataPosition + 4);
		identity.m02 = BitConverter.ToSingle(bytes, dataPosition + 8);
		identity.m03 = BitConverter.ToSingle(bytes, dataPosition + 12);
		identity.m10 = BitConverter.ToSingle(bytes, dataPosition + 16);
		identity.m11 = BitConverter.ToSingle(bytes, dataPosition + 20);
		identity.m12 = BitConverter.ToSingle(bytes, dataPosition + 24);
		identity.m13 = BitConverter.ToSingle(bytes, dataPosition + 28);
		identity.m20 = BitConverter.ToSingle(bytes, dataPosition + 32);
		identity.m21 = BitConverter.ToSingle(bytes, dataPosition + 36);
		identity.m22 = BitConverter.ToSingle(bytes, dataPosition + 40);
		identity.m23 = BitConverter.ToSingle(bytes, dataPosition + 44);
		identity.m30 = BitConverter.ToSingle(bytes, dataPosition + 48);
		identity.m31 = BitConverter.ToSingle(bytes, dataPosition + 52);
		identity.m32 = BitConverter.ToSingle(bytes, dataPosition + 56);
		identity.m33 = BitConverter.ToSingle(bytes, dataPosition + 60);
		return identity;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00009698 File Offset: 0x00007898
	private static AnimationCurve BytesToAnimationCurve(byte[] bytes, int dataPosition)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		int num = BitConverter.ToInt32(bytes, dataPosition);
		for (int i = 0; i < num; i++)
		{
			Keyframe keyframe = default(Keyframe);
			keyframe.time = BitConverter.ToSingle(bytes, dataPosition + 4);
			keyframe.value = BitConverter.ToSingle(bytes, dataPosition + 8);
			keyframe.inTangent = BitConverter.ToSingle(bytes, dataPosition + 12);
			keyframe.outTangent = BitConverter.ToSingle(bytes, dataPosition + 16);
			keyframe.tangentMode = BitConverter.ToInt32(bytes, dataPosition + 20);
			animationCurve.AddKey(keyframe);
			dataPosition += 20;
		}
		animationCurve.preWrapMode = (WrapMode)BitConverter.ToInt32(bytes, dataPosition + 4);
		animationCurve.postWrapMode = (WrapMode)BitConverter.ToInt32(bytes, dataPosition + 8);
		return animationCurve;
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00009750 File Offset: 0x00007950
	private static LayerMask BytesToLayerMask(byte[] bytes, int dataPosition)
	{
		LayerMask result = default(LayerMask);
		result.value = BinaryDeserialization.BytesToInt(bytes, dataPosition);
		return result;
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00009774 File Offset: 0x00007974
	private static UnityEngine.Object IndexToUnityObject(int index, FieldSerializationData activeFieldSerializationData)
	{
		if (index < 0 || index >= activeFieldSerializationData.unityObjects.Count)
		{
			return null;
		}
		return activeFieldSerializationData.unityObjects[index];
	}

	// Token: 0x060000FE RID: 254 RVA: 0x000097A8 File Offset: 0x000079A8
	private static SharedVariable BytesToSharedVariable(FieldSerializationData fieldSerializationData, Dictionary<int, int> fieldIndexMap, byte[] bytes, int dataPosition, IVariableSource variableSource, bool fromField, int hashPrefix)
	{
		SharedVariable sharedVariable = null;
		string text = (string)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), "Type", hashPrefix, null, null, null);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string name = (string)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), "Name", hashPrefix, null, null, null);
		bool flag = Convert.ToBoolean(BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "IsShared", hashPrefix, null, null, null));
		bool flag2 = Convert.ToBoolean(BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "IsGlobal", hashPrefix, null, null, null));
		if (flag && fromField)
		{
			if (!flag2)
			{
				sharedVariable = variableSource.GetVariable(name);
			}
			else
			{
				if (BinaryDeserialization.globalVariables == null)
				{
					BinaryDeserialization.globalVariables = GlobalVariables.Instance;
				}
				if (BinaryDeserialization.globalVariables != null)
				{
					sharedVariable = BinaryDeserialization.globalVariables.GetVariable(name);
				}
			}
		}
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(text);
		if (typeWithinAssembly == null)
		{
			return null;
		}
		bool flag3 = true;
		if (sharedVariable == null || !(flag3 = sharedVariable.GetType().Equals(typeWithinAssembly)))
		{
			sharedVariable = (TaskUtility.CreateInstance(typeWithinAssembly) as SharedVariable);
			sharedVariable.Name = name;
			sharedVariable.IsShared = flag;
			sharedVariable.IsGlobal = flag2;
			sharedVariable.NetworkSync = Convert.ToBoolean(BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "NetworkSync", hashPrefix, null, null, null));
			if (!flag2)
			{
				sharedVariable.PropertyMapping = (string)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), "PropertyMapping", hashPrefix, null, null, null);
				sharedVariable.PropertyMappingOwner = (GameObject)BinaryDeserialization.LoadField(fieldSerializationData, fieldIndexMap, typeof(GameObject), "PropertyMappingOwner", hashPrefix, null, null, null);
				sharedVariable.InitializePropertyMapping(variableSource as BehaviorSource);
			}
			if (!flag3)
			{
				sharedVariable.IsShared = true;
			}
			BinaryDeserialization.LoadFields(fieldSerializationData, fieldIndexMap, sharedVariable, hashPrefix, variableSource);
		}
		return sharedVariable;
	}

	// Token: 0x0400007A RID: 122
	private static GlobalVariables globalVariables;

	// Token: 0x0400007B RID: 123
	private static Dictionary<BinaryDeserialization.ObjectFieldMap, List<int>> taskIDs;

	// Token: 0x0400007C RID: 124
	private static bool updatedSerialization;

	// Token: 0x02000013 RID: 19
	private class ObjectFieldMap
	{
		// Token: 0x060000FF RID: 255 RVA: 0x00009994 File Offset: 0x00007B94
		public ObjectFieldMap(object o, FieldInfo f)
		{
			this.obj = o;
			this.fieldInfo = f;
		}

		// Token: 0x0400007D RID: 125
		public object obj;

		// Token: 0x0400007E RID: 126
		public FieldInfo fieldInfo;
	}

	// Token: 0x02000014 RID: 20
	private class ObjectFieldMapComparer : IEqualityComparer<BinaryDeserialization.ObjectFieldMap>
	{
		// Token: 0x06000101 RID: 257 RVA: 0x000099B4 File Offset: 0x00007BB4
		public bool Equals(BinaryDeserialization.ObjectFieldMap a, BinaryDeserialization.ObjectFieldMap b)
		{
			return !object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null) && a.obj.Equals(b.obj) && a.fieldInfo.Equals(b.fieldInfo);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00009A08 File Offset: 0x00007C08
		public int GetHashCode(BinaryDeserialization.ObjectFieldMap a)
		{
			return (a == null) ? 0 : (a.obj.ToString().GetHashCode() + a.fieldInfo.ToString().GetHashCode());
		}
	}
}
