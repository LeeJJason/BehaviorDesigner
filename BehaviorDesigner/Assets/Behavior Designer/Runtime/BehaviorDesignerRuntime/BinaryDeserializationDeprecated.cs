using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

// Token: 0x02000015 RID: 21
public static class BinaryDeserializationDeprecated
{
	// Token: 0x06000104 RID: 260 RVA: 0x00009A48 File Offset: 0x00007C48
	public static void Load(BehaviorSource behaviorSource)
	{
		BinaryDeserializationDeprecated.Load(behaviorSource.TaskData, behaviorSource);
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00009A58 File Offset: 0x00007C58
	public static void Load(TaskSerializationData taskData, BehaviorSource behaviorSource)
	{
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
		BinaryDeserializationDeprecated.taskIDs = null;
		if (taskData.variableStartIndex != null)
		{
			List<SharedVariable> list = new List<SharedVariable>();
			Dictionary<string, int> dictionary = ObjectPool.Get<Dictionary<string, int>>();
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
					dictionary.Add(fieldSerializationData.typeName[j], fieldSerializationData.startIndex[j]);
				}
				SharedVariable sharedVariable = BinaryDeserializationDeprecated.BytesToSharedVariable(fieldSerializationData, dictionary, fieldSerializationData.byteDataArray, taskData.variableStartIndex[i], behaviorSource, false, string.Empty);
				if (sharedVariable != null)
				{
					list.Add(sharedVariable);
				}
			}
			ObjectPool.Return<Dictionary<string, int>>(dictionary);
			behaviorSource.Variables = list;
		}
		List<Task> list2 = new List<Task>();
		if (taskData.types != null)
		{
			for (int k = 0; k < taskData.types.Count; k++)
			{
				BinaryDeserializationDeprecated.LoadTask(taskData, fieldSerializationData, ref list2, ref behaviorSource);
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
		if (BinaryDeserializationDeprecated.taskIDs != null)
		{
			foreach (BinaryDeserializationDeprecated.ObjectFieldMap objectFieldMap in BinaryDeserializationDeprecated.taskIDs.Keys)
			{
				List<int> list3 = BinaryDeserializationDeprecated.taskIDs[objectFieldMap];
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

	// Token: 0x06000106 RID: 262 RVA: 0x00009F04 File Offset: 0x00008104
	public static void Load(GlobalVariables globalVariables)
	{
		if (globalVariables == null)
		{
			return;
		}
		globalVariables.Variables = null;
		FieldSerializationData fieldSerializationData;
		if (globalVariables.VariableData == null || (fieldSerializationData = globalVariables.VariableData.fieldSerializationData).byteData == null || fieldSerializationData.byteData.Count == 0)
		{
			return;
		}
		VariableSerializationData variableData = globalVariables.VariableData;
		fieldSerializationData.byteDataArray = fieldSerializationData.byteData.ToArray();
		if (variableData.variableStartIndex != null)
		{
			List<SharedVariable> list = new List<SharedVariable>();
			Dictionary<string, int> dictionary = ObjectPool.Get<Dictionary<string, int>>();
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
					dictionary.Add(fieldSerializationData.typeName[j], fieldSerializationData.startIndex[j]);
				}
				SharedVariable sharedVariable = BinaryDeserializationDeprecated.BytesToSharedVariable(fieldSerializationData, dictionary, fieldSerializationData.byteDataArray, variableData.variableStartIndex[i], globalVariables, false, string.Empty);
				if (sharedVariable != null)
				{
					list.Add(sharedVariable);
				}
			}
			ObjectPool.Return<Dictionary<string, int>>(dictionary);
			globalVariables.Variables = list;
		}
	}

	// Token: 0x06000107 RID: 263 RVA: 0x0000A06C File Offset: 0x0000826C
	private static void LoadTask(TaskSerializationData taskSerializationData, FieldSerializationData fieldSerializationData, ref List<Task> taskList, ref BehaviorSource behaviorSource)
	{
		int count = taskList.Count;
		Type type = TaskUtility.GetTypeWithinAssembly(taskSerializationData.types[count]);
		if (type == null)
		{
			bool flag = false;
			for (int i = 0; i < taskSerializationData.parentIndex.Count; i++)
			{
				if (count == taskSerializationData.parentIndex[i])
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
		task.Owner = (behaviorSource.Owner.GetObject() as Behavior);
		taskList.Add(task);
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
		Dictionary<string, int> dictionary = ObjectPool.Get<Dictionary<string, int>>();
		dictionary.Clear();
		for (int j = num; j < num2; j++)
		{
			if (!dictionary.ContainsKey(fieldSerializationData.typeName[j]))
			{
				dictionary.Add(fieldSerializationData.typeName[j], fieldSerializationData.startIndex[j]);
			}
		}
		task.ID = (int)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, dictionary, typeof(int), "ID", null, null, null);
		task.FriendlyName = (string)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, dictionary, typeof(string), "FriendlyName", null, null, null);
		task.IsInstant = (bool)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, dictionary, typeof(bool), "IsInstant", null, null, null);
		object obj;
		if ((obj = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, dictionary, typeof(bool), "Disabled", null, null, null)) != null)
		{
			task.Disabled = (bool)obj;
		}
		BinaryDeserializationDeprecated.LoadNodeData(fieldSerializationData, dictionary, taskList[count]);
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
		BinaryDeserializationDeprecated.LoadFields(fieldSerializationData, dictionary, taskList[count], string.Empty, behaviorSource);
		ObjectPool.Return<Dictionary<string, int>>(dictionary);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x0000A360 File Offset: 0x00008560
	private static void LoadNodeData(FieldSerializationData fieldSerializationData, Dictionary<string, int> fieldIndexMap, Task task)
	{
		NodeData nodeData = new NodeData();
		nodeData.Offset = (Vector2)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(Vector2), "NodeDataOffset", null, null, null);
		nodeData.Comment = (string)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), "NodeDataComment", null, null, null);
		nodeData.IsBreakpoint = (bool)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "NodeDataIsBreakpoint", null, null, null);
		nodeData.Collapsed = (bool)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), "NodeDataCollapsed", null, null, null);
		object obj = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(int), "NodeDataColorIndex", null, null, null);
		if (obj != null)
		{
			nodeData.ColorIndex = (int)obj;
		}
		obj = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(List<string>), "NodeDataWatchedFields", null, null, null);
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

	// Token: 0x06000109 RID: 265 RVA: 0x0000A4CC File Offset: 0x000086CC
	private static void LoadFields(FieldSerializationData fieldSerializationData, Dictionary<string, int> fieldIndexMap, object obj, string namePrefix, IVariableSource variableSource)
	{
		FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
		for (int i = 0; i < allFields.Length; i++)
		{
			if (!TaskUtility.HasAttribute(allFields[i], typeof(NonSerializedAttribute)) && ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || TaskUtility.HasAttribute(allFields[i], typeof(SerializeField))) && (!(obj is ParentTask) || !allFields[i].Name.Equals("children")))
			{
				object obj2 = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, allFields[i].FieldType, namePrefix + allFields[i].Name, variableSource, obj, allFields[i]);
				if (obj2 != null && !object.ReferenceEquals(obj2, null) && !obj2.Equals(null))
				{
					allFields[i].SetValue(obj, obj2);
				}
			}
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x0000A5B4 File Offset: 0x000087B4
	private static object LoadField(FieldSerializationData fieldSerializationData, Dictionary<string, int> fieldIndexMap, Type fieldType, string fieldName, IVariableSource variableSource, object obj = null, FieldInfo fieldInfo = null)
	{
		string text = fieldType.Name + fieldName;
		int num;
		if (fieldIndexMap.TryGetValue(text, out num))
		{
			object obj2 = null;
			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				int num2 = BinaryDeserializationDeprecated.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
				if (fieldType.IsArray)
				{
					Type elementType = fieldType.GetElementType();
					if (elementType == null)
					{
						return null;
					}
					Array array = Array.CreateInstance(elementType, num2);
					for (int i = 0; i < num2; i++)
					{
						object obj3 = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, elementType, text + i, variableSource, obj, fieldInfo);
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
					for (int j = 0; j < num2; j++)
					{
						object obj4 = BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, type2, text + j, variableSource, obj, fieldInfo);
						list.Add((!object.ReferenceEquals(obj4, null) && !obj4.Equals(null)) ? obj4 : null);
					}
					obj2 = list;
				}
			}
			else if (typeof(Task).IsAssignableFrom(fieldType))
			{
				if (fieldInfo != null && TaskUtility.HasAttribute(fieldInfo, typeof(InspectTaskAttribute)))
				{
					string text2 = BinaryDeserializationDeprecated.BytesToString(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num], BinaryDeserializationDeprecated.GetFieldSize(fieldSerializationData, num));
					if (!string.IsNullOrEmpty(text2))
					{
						Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(text2);
						if (typeWithinAssembly != null)
						{
							obj2 = TaskUtility.CreateInstance(typeWithinAssembly);
							BinaryDeserializationDeprecated.LoadFields(fieldSerializationData, fieldIndexMap, obj2, text, variableSource);
						}
					}
				}
				else
				{
					if (BinaryDeserializationDeprecated.taskIDs == null)
					{
						BinaryDeserializationDeprecated.taskIDs = new Dictionary<BinaryDeserializationDeprecated.ObjectFieldMap, List<int>>(new BinaryDeserializationDeprecated.ObjectFieldMapComparer());
					}
					int item = BinaryDeserializationDeprecated.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
					BinaryDeserializationDeprecated.ObjectFieldMap key = new BinaryDeserializationDeprecated.ObjectFieldMap(obj, fieldInfo);
					if (BinaryDeserializationDeprecated.taskIDs.ContainsKey(key))
					{
						BinaryDeserializationDeprecated.taskIDs[key].Add(item);
					}
					else
					{
						List<int> list2 = new List<int>();
						list2.Add(item);
						BinaryDeserializationDeprecated.taskIDs.Add(key, list2);
					}
				}
			}
			else if (typeof(SharedVariable).IsAssignableFrom(fieldType))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToSharedVariable(fieldSerializationData, fieldIndexMap, fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num], variableSource, true, text);
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
			{
				int index = BinaryDeserializationDeprecated.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
				obj2 = BinaryDeserializationDeprecated.IndexToUnityObject(index, fieldSerializationData);
			}
			else if (fieldType.Equals(typeof(int)) || fieldType.IsEnum)
			{
				obj2 = BinaryDeserializationDeprecated.BytesToInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(uint)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToUInt(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(float)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToFloat(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(double)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToDouble(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(long)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToLong(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(bool)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToBool(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(string)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToString(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num], BinaryDeserializationDeprecated.GetFieldSize(fieldSerializationData, num));
			}
			else if (fieldType.Equals(typeof(byte)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToByte(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Vector2)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToVector2(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Vector3)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToVector3(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Vector4)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToVector4(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Quaternion)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToQuaternion(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Color)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToColor(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Rect)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToRect(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(Matrix4x4)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToMatrix4x4(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(AnimationCurve)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToAnimationCurve(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.Equals(typeof(LayerMask)))
			{
				obj2 = BinaryDeserializationDeprecated.BytesToLayerMask(fieldSerializationData.byteDataArray, fieldSerializationData.dataPosition[num]);
			}
			else if (fieldType.IsClass || (fieldType.IsValueType && !fieldType.IsPrimitive))
			{
				obj2 = TaskUtility.CreateInstance(fieldType);
				BinaryDeserializationDeprecated.LoadFields(fieldSerializationData, fieldIndexMap, obj2, text, variableSource);
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
			return TaskUtility.CreateInstance(fieldType);
		}
		return null;
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000ACFC File Offset: 0x00008EFC
	private static int GetFieldSize(FieldSerializationData fieldSerializationData, int fieldIndex)
	{
		return ((fieldIndex + 1 >= fieldSerializationData.dataPosition.Count) ? fieldSerializationData.byteData.Count : fieldSerializationData.dataPosition[fieldIndex + 1]) - fieldSerializationData.dataPosition[fieldIndex];
	}

	// Token: 0x0600010C RID: 268 RVA: 0x0000AD48 File Offset: 0x00008F48
	private static int BytesToInt(byte[] bytes, int dataPosition)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes, dataPosition, 4);
		}
		return BitConverter.ToInt32(bytes, dataPosition);
	}

	// Token: 0x0600010D RID: 269 RVA: 0x0000AD64 File Offset: 0x00008F64
	private static uint BytesToUInt(byte[] bytes, int dataPosition)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes, dataPosition, 4);
		}
		return BitConverter.ToUInt32(bytes, dataPosition);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x0000AD80 File Offset: 0x00008F80
	private static float BytesToFloat(byte[] bytes, int dataPosition)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes, dataPosition, 4);
		}
		return BitConverter.ToSingle(bytes, dataPosition);
	}

	// Token: 0x0600010F RID: 271 RVA: 0x0000AD9C File Offset: 0x00008F9C
	private static double BytesToDouble(byte[] bytes, int dataPosition)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes, dataPosition, 8);
		}
		return BitConverter.ToDouble(bytes, dataPosition);
	}

	// Token: 0x06000110 RID: 272 RVA: 0x0000ADB8 File Offset: 0x00008FB8
	private static long BytesToLong(byte[] bytes, int dataPosition)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes, dataPosition, 8);
		}
		return BitConverter.ToInt64(bytes, dataPosition);
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000ADD4 File Offset: 0x00008FD4
	private static bool BytesToBool(byte[] bytes, int dataPosition)
	{
		return BitConverter.ToBoolean(bytes, dataPosition);
	}

	// Token: 0x06000112 RID: 274 RVA: 0x0000ADE0 File Offset: 0x00008FE0
	private static string BytesToString(byte[] bytes, int dataPosition, int dataSize)
	{
		if (dataSize == 0)
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(bytes, dataPosition, dataSize);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x0000ADFC File Offset: 0x00008FFC
	private static byte BytesToByte(byte[] bytes, int dataPosition)
	{
		return bytes[dataPosition];
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000AE04 File Offset: 0x00009004
	private static Color BytesToColor(byte[] bytes, int dataPosition)
	{
		Color black = Color.black;
		black.r = BitConverter.ToSingle(bytes, dataPosition);
		black.g = BitConverter.ToSingle(bytes, dataPosition + 4);
		black.b = BitConverter.ToSingle(bytes, dataPosition + 8);
		black.a = BitConverter.ToSingle(bytes, dataPosition + 12);
		return black;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x0000AE58 File Offset: 0x00009058
	private static Vector2 BytesToVector2(byte[] bytes, int dataPosition)
	{
		Vector2 zero = Vector2.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		return zero;
	}

	// Token: 0x06000116 RID: 278 RVA: 0x0000AE8C File Offset: 0x0000908C
	private static Vector3 BytesToVector3(byte[] bytes, int dataPosition)
	{
		Vector3 zero = Vector3.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		zero.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		return zero;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000AED0 File Offset: 0x000090D0
	private static Vector4 BytesToVector4(byte[] bytes, int dataPosition)
	{
		Vector4 zero = Vector4.zero;
		zero.x = BitConverter.ToSingle(bytes, dataPosition);
		zero.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		zero.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		zero.w = BitConverter.ToSingle(bytes, dataPosition + 12);
		return zero;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000AF24 File Offset: 0x00009124
	private static Quaternion BytesToQuaternion(byte[] bytes, int dataPosition)
	{
		Quaternion identity = Quaternion.identity;
		identity.x = BitConverter.ToSingle(bytes, dataPosition);
		identity.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		identity.z = BitConverter.ToSingle(bytes, dataPosition + 8);
		identity.w = BitConverter.ToSingle(bytes, dataPosition + 12);
		return identity;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000AF78 File Offset: 0x00009178
	private static Rect BytesToRect(byte[] bytes, int dataPosition)
	{
		Rect result = default(Rect);
		result.x = BitConverter.ToSingle(bytes, dataPosition);
		result.y = BitConverter.ToSingle(bytes, dataPosition + 4);
		result.width = BitConverter.ToSingle(bytes, dataPosition + 8);
		result.height = BitConverter.ToSingle(bytes, dataPosition + 12);
		return result;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000AFD0 File Offset: 0x000091D0
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

	// Token: 0x0600011B RID: 283 RVA: 0x0000B0F0 File Offset: 0x000092F0
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

	// Token: 0x0600011C RID: 284 RVA: 0x0000B1A8 File Offset: 0x000093A8
	private static LayerMask BytesToLayerMask(byte[] bytes, int dataPosition)
	{
		LayerMask result = default(LayerMask);
		result.value = BinaryDeserializationDeprecated.BytesToInt(bytes, dataPosition);
		return result;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000B1CC File Offset: 0x000093CC
	private static UnityEngine.Object IndexToUnityObject(int index, FieldSerializationData activeFieldSerializationData)
	{
		if (index < 0 || index >= activeFieldSerializationData.unityObjects.Count)
		{
			return null;
		}
		return activeFieldSerializationData.unityObjects[index];
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0000B200 File Offset: 0x00009400
	private static SharedVariable BytesToSharedVariable(FieldSerializationData fieldSerializationData, Dictionary<string, int> fieldIndexMap, byte[] bytes, int dataPosition, IVariableSource variableSource, bool fromField, string namePrefix)
	{
		SharedVariable sharedVariable = null;
		string text = (string)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), namePrefix + "Type", null, null, null);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string name = (string)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), namePrefix + "Name", null, null, null);
		bool flag = Convert.ToBoolean(BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), namePrefix + "IsShared", null, null, null));
		bool flag2 = Convert.ToBoolean(BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), namePrefix + "IsGlobal", null, null, null));
		if (flag && fromField)
		{
			if (!flag2)
			{
				sharedVariable = variableSource.GetVariable(name);
			}
			else
			{
				if (BinaryDeserializationDeprecated.globalVariables == null)
				{
					BinaryDeserializationDeprecated.globalVariables = GlobalVariables.Instance;
				}
				if (BinaryDeserializationDeprecated.globalVariables != null)
				{
					sharedVariable = BinaryDeserializationDeprecated.globalVariables.GetVariable(name);
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
			sharedVariable.NetworkSync = Convert.ToBoolean(BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(bool), namePrefix + "NetworkSync", null, null, null));
			if (!flag2)
			{
				sharedVariable.PropertyMapping = (string)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(string), namePrefix + "PropertyMapping", null, null, null);
				sharedVariable.PropertyMappingOwner = (GameObject)BinaryDeserializationDeprecated.LoadField(fieldSerializationData, fieldIndexMap, typeof(GameObject), namePrefix + "PropertyMappingOwner", null, null, null);
				sharedVariable.InitializePropertyMapping(variableSource as BehaviorSource);
			}
			if (!flag3)
			{
				sharedVariable.IsShared = true;
			}
			BinaryDeserializationDeprecated.LoadFields(fieldSerializationData, fieldIndexMap, sharedVariable, namePrefix, variableSource);
		}
		return sharedVariable;
	}

	// Token: 0x0400007F RID: 127
	private static GlobalVariables globalVariables;

	// Token: 0x04000080 RID: 128
	private static Dictionary<BinaryDeserializationDeprecated.ObjectFieldMap, List<int>> taskIDs;

	// Token: 0x02000016 RID: 22
	private class ObjectFieldMap
	{
		// Token: 0x0600011F RID: 287 RVA: 0x0000B410 File Offset: 0x00009610
		public ObjectFieldMap(object o, FieldInfo f)
		{
			this.obj = o;
			this.fieldInfo = f;
		}

		// Token: 0x04000081 RID: 129
		public object obj;

		// Token: 0x04000082 RID: 130
		public FieldInfo fieldInfo;
	}

	// Token: 0x02000017 RID: 23
	private class ObjectFieldMapComparer : IEqualityComparer<BinaryDeserializationDeprecated.ObjectFieldMap>
	{
		// Token: 0x06000121 RID: 289 RVA: 0x0000B430 File Offset: 0x00009630
		public bool Equals(BinaryDeserializationDeprecated.ObjectFieldMap a, BinaryDeserializationDeprecated.ObjectFieldMap b)
		{
			return !object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null) && a.obj.Equals(b.obj) && a.fieldInfo.Equals(b.fieldInfo);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000B484 File Offset: 0x00009684
		public int GetHashCode(BinaryDeserializationDeprecated.ObjectFieldMap a)
		{
			return (a == null) ? 0 : (a.obj.ToString().GetHashCode() + a.fieldInfo.ToString().GetHashCode());
		}
	}
}
