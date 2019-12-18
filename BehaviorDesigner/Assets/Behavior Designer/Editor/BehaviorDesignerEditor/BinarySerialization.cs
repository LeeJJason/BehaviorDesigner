using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200000E RID: 14
	public class BinarySerialization
	{
		// Token: 0x06000150 RID: 336 RVA: 0x0000BAE4 File Offset: 0x00009CE4
		public static void Save(BehaviorSource behaviorSource)
		{
			BinarySerialization.fieldIndex = 0;
			BinarySerialization.taskSerializationData = new TaskSerializationData();
			BinarySerialization.fieldSerializationData = BinarySerialization.taskSerializationData.fieldSerializationData;
			if (behaviorSource.Variables != null)
			{
				for (int i = 0; i < behaviorSource.Variables.Count; i++)
				{
					BinarySerialization.taskSerializationData.variableStartIndex.Add(BinarySerialization.fieldSerializationData.startIndex.Count);
					BinarySerialization.SaveSharedVariable(behaviorSource.Variables[i], 0);
				}
			}
			if (!object.ReferenceEquals(behaviorSource.EntryTask, null))
			{
				BinarySerialization.SaveTask(behaviorSource.EntryTask, -1);
			}
			if (!object.ReferenceEquals(behaviorSource.RootTask, null))
			{
				BinarySerialization.SaveTask(behaviorSource.RootTask, 0);
			}
			if (behaviorSource.DetachedTasks != null)
			{
				for (int j = 0; j < behaviorSource.DetachedTasks.Count; j++)
				{
					BinarySerialization.SaveTask(behaviorSource.DetachedTasks[j], -1);
				}
			}
			BinarySerialization.taskSerializationData.Version = "1.5.7";
			behaviorSource.TaskData = BinarySerialization.taskSerializationData;
			if (behaviorSource.Owner != null && !behaviorSource.Owner.Equals(null))
			{
				BehaviorDesignerUtility.SetObjectDirty(behaviorSource.Owner.GetObject());
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000BC20 File Offset: 0x00009E20
		public static void Save(GlobalVariables globalVariables)
		{
			if (globalVariables == null)
			{
				return;
			}
			BinarySerialization.fieldIndex = 0;
			globalVariables.VariableData = new VariableSerializationData();
			if (globalVariables.Variables == null || globalVariables.Variables.Count == 0)
			{
				return;
			}
			BinarySerialization.fieldSerializationData = globalVariables.VariableData.fieldSerializationData;
			for (int i = 0; i < globalVariables.Variables.Count; i++)
			{
				globalVariables.VariableData.variableStartIndex.Add(BinarySerialization.fieldSerializationData.startIndex.Count);
				BinarySerialization.SaveSharedVariable(globalVariables.Variables[i], 0);
			}
			globalVariables.Version = "1.5.7";
			BehaviorDesignerUtility.SetObjectDirty(globalVariables);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000BCD8 File Offset: 0x00009ED8
		private static void SaveTask(Task task, int parentTaskIndex)
		{
			BinarySerialization.taskSerializationData.types.Add(task.GetType().ToString());
			BinarySerialization.taskSerializationData.parentIndex.Add(parentTaskIndex);
			BinarySerialization.taskSerializationData.startIndex.Add(BinarySerialization.fieldSerializationData.startIndex.Count);
			BinarySerialization.SaveField(typeof(int), "ID", 0, task.ID, null);
			BinarySerialization.SaveField(typeof(string), "FriendlyName", 0, task.FriendlyName, null);
			BinarySerialization.SaveField(typeof(bool), "IsInstant", 0, task.IsInstant, null);
			BinarySerialization.SaveField(typeof(bool), "Disabled", 0, task.Disabled, null);
			BinarySerialization.SaveNodeData(task.NodeData);
			BinarySerialization.SaveFields(task, 0);
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null && parentTask.Children.Count > 0)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						BinarySerialization.SaveTask(parentTask.Children[i], parentTask.ID);
					}
				}
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000BE20 File Offset: 0x0000A020
		private static void SaveNodeData(NodeData nodeData)
		{
			BinarySerialization.SaveField(typeof(Vector2), "NodeDataOffset", 0, nodeData.Offset, null);
			BinarySerialization.SaveField(typeof(string), "NodeDataComment", 0, nodeData.Comment, null);
			BinarySerialization.SaveField(typeof(bool), "NodeDataIsBreakpoint", 0, nodeData.IsBreakpoint, null);
			BinarySerialization.SaveField(typeof(bool), "NodeDataCollapsed", 0, nodeData.Collapsed, null);
			BinarySerialization.SaveField(typeof(int), "NodeDataColorIndex", 0, nodeData.ColorIndex, null);
			BinarySerialization.SaveField(typeof(List<string>), "NodeDataWatchedFields", 0, nodeData.WatchedFieldNames, null);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		private static void SaveSharedVariable(SharedVariable sharedVariable, int hashPrefix)
		{
			if (sharedVariable == null)
			{
				return;
			}
			BinarySerialization.SaveField(typeof(string), "Type", hashPrefix, sharedVariable.GetType().ToString(), null);
			BinarySerialization.SaveField(typeof(string), "Name", hashPrefix, sharedVariable.Name, null);
			if (sharedVariable.IsShared)
			{
				BinarySerialization.SaveField(typeof(bool), "IsShared", hashPrefix, sharedVariable.IsShared, null);
			}
			if (sharedVariable.IsGlobal)
			{
				BinarySerialization.SaveField(typeof(bool), "IsGlobal", hashPrefix, sharedVariable.IsGlobal, null);
			}
			if (sharedVariable.NetworkSync)
			{
				BinarySerialization.SaveField(typeof(bool), "NetworkSync", hashPrefix, sharedVariable.NetworkSync, null);
			}
			if (!string.IsNullOrEmpty(sharedVariable.PropertyMapping))
			{
				BinarySerialization.SaveField(typeof(string), "PropertyMapping", hashPrefix, sharedVariable.PropertyMapping, null);
				if (!object.Equals(sharedVariable.PropertyMappingOwner, null))
				{
					BinarySerialization.SaveField(typeof(GameObject), "PropertyMappingOwner", hashPrefix, sharedVariable.PropertyMappingOwner, null);
				}
			}
			BinarySerialization.SaveFields(sharedVariable, hashPrefix);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000C024 File Offset: 0x0000A224
		private static void SaveFields(object obj, int hashPrefix)
		{
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if (!BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(NonSerializedAttribute)) && ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(SerializeField))) && (!(obj is ParentTask) || !allFields[i].Name.Equals("children")))
				{
					object value = allFields[i].GetValue(obj);
					if (!object.ReferenceEquals(value, null))
					{
						BinarySerialization.SaveField(allFields[i].FieldType, allFields[i].Name, hashPrefix, value, allFields[i]);
					}
				}
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000C0F8 File Offset: 0x0000A2F8
		private static void SaveField(Type fieldType, string fieldName, int hashPrefix, object value, FieldInfo fieldInfo = null)
		{
			int num = hashPrefix + fieldType.Name.GetHashCode() + fieldName.GetHashCode();
			BinarySerialization.fieldSerializationData.fieldNameHash.Add(num);
			BinarySerialization.fieldSerializationData.startIndex.Add(BinarySerialization.fieldIndex);
			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				Type fieldType2;
				if (fieldType.IsArray)
				{
					fieldType2 = fieldType.GetElementType();
				}
				else
				{
					Type type = fieldType;
					while (!type.IsGenericType)
					{
						type = type.BaseType;
					}
					fieldType2 = type.GetGenericArguments()[0];
				}
				IList list = value as IList;
				if (list == null)
				{
					BinarySerialization.AddByteData(typeof(int), BinarySerialization.IntToBytes(0));
				}
				else
				{
					BinarySerialization.AddByteData(typeof(int), BinarySerialization.IntToBytes(list.Count));
					if (list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (object.ReferenceEquals(list[i], null))
							{
								BinarySerialization.AddByteData(fieldType2, BinarySerialization.IntToBytes(-1));
							}
							else
							{
								BinarySerialization.SaveField(fieldType2, i.ToString(), num / (i + 1), list[i], fieldInfo);
							}
						}
					}
				}
			}
			else if (typeof(Task).IsAssignableFrom(fieldType))
			{
				if (fieldInfo != null && BehaviorDesignerUtility.HasAttribute(fieldInfo, typeof(InspectTaskAttribute)))
				{
					BinarySerialization.AddByteData(fieldType, BinarySerialization.StringToBytes(value.GetType().ToString()));
					BinarySerialization.SaveFields(value, num);
				}
				else
				{
					BinarySerialization.AddByteData(fieldType, BinarySerialization.IntToBytes((value as Task).ID));
				}
			}
			else if (typeof(SharedVariable).IsAssignableFrom(fieldType))
			{
				BinarySerialization.SaveSharedVariable(value as SharedVariable, num);
			}
			else if (typeof(Object).IsAssignableFrom(fieldType))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.IntToBytes(BinarySerialization.fieldSerializationData.unityObjects.Count));
				BinarySerialization.fieldSerializationData.unityObjects.Add(value as Object);
			}
			else if (fieldType.Equals(typeof(int)) || fieldType.IsEnum)
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.IntToBytes((int)value));
			}
			else if (fieldType.Equals(typeof(short)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.Int16ToBytes((short)value));
			}
			else if (fieldType.Equals(typeof(uint)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.UIntToBytes((uint)value));
			}
			else if (fieldType.Equals(typeof(float)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.FloatToBytes((float)value));
			}
			else if (fieldType.Equals(typeof(double)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.DoubleToBytes((double)value));
			}
			else if (fieldType.Equals(typeof(long)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.LongToBytes((long)value));
			}
			else if (fieldType.Equals(typeof(bool)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.BoolToBytes((bool)value));
			}
			else if (fieldType.Equals(typeof(string)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.StringToBytes((string)value));
			}
			else if (fieldType.Equals(typeof(byte)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.ByteToBytes((byte)value));
			}
			else if (fieldType.Equals(typeof(Vector2)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.Vector2ToBytes((Vector2)value));
			}
			else if (fieldType.Equals(typeof(Vector3)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.Vector3ToBytes((Vector3)value));
			}
			else if (fieldType.Equals(typeof(Vector4)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.Vector4ToBytes((Vector4)value));
			}
			else if (fieldType.Equals(typeof(Quaternion)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.QuaternionToBytes((Quaternion)value));
			}
			else if (fieldType.Equals(typeof(Color)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.ColorToBytes((Color)value));
			}
			else if (fieldType.Equals(typeof(Rect)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.RectToBytes((Rect)value));
			}
			else if (fieldType.Equals(typeof(Matrix4x4)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.Matrix4x4ToBytes((Matrix4x4)value));
			}
			else if (fieldType.Equals(typeof(LayerMask)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.IntToBytes(((LayerMask)value).value));
			}
			else if (fieldType.Equals(typeof(AnimationCurve)))
			{
				BinarySerialization.AddByteData(fieldType, BinarySerialization.AnimationCurveToBytes((AnimationCurve)value));
			}
			else if (fieldType.IsClass || (fieldType.IsValueType && !fieldType.IsPrimitive))
			{
				if (object.ReferenceEquals(value, null))
				{
					value = Activator.CreateInstance(fieldType, true);
				}
				BinarySerialization.SaveFields(value, num);
			}
			else
			{
				Debug.LogError("Missing Serialization for " + fieldType);
			}
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000C684 File Offset: 0x0000A884
		private static byte[] IntToBytes(int value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000C68C File Offset: 0x0000A88C
		private static byte[] Int16ToBytes(short value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000C694 File Offset: 0x0000A894
		private static byte[] UIntToBytes(uint value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000C69C File Offset: 0x0000A89C
		private static byte[] FloatToBytes(float value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		private static byte[] DoubleToBytes(double value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000C6AC File Offset: 0x0000A8AC
		private static byte[] LongToBytes(long value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000C6B4 File Offset: 0x0000A8B4
		private static byte[] BoolToBytes(bool value)
		{
			return BitConverter.GetBytes(value);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000C6BC File Offset: 0x0000A8BC
		private static byte[] StringToBytes(string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			return Encoding.UTF8.GetBytes(str);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000C6D8 File Offset: 0x0000A8D8
		private static byte[] ByteToBytes(byte value)
		{
			return new byte[]
			{
				value
			};
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000C6F4 File Offset: 0x0000A8F4
		private static ICollection<byte> ColorToBytes(Color color)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(color.r));
			list.AddRange(BitConverter.GetBytes(color.g));
			list.AddRange(BitConverter.GetBytes(color.b));
			list.AddRange(BitConverter.GetBytes(color.a));
			return list;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000C750 File Offset: 0x0000A950
		private static ICollection<byte> Vector2ToBytes(Vector2 vector2)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector2.x));
			list.AddRange(BitConverter.GetBytes(vector2.y));
			return list;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000C788 File Offset: 0x0000A988
		private static ICollection<byte> Vector3ToBytes(Vector3 vector3)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector3.x));
			list.AddRange(BitConverter.GetBytes(vector3.y));
			list.AddRange(BitConverter.GetBytes(vector3.z));
			return list;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000C7D4 File Offset: 0x0000A9D4
		private static ICollection<byte> Vector4ToBytes(Vector4 vector4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector4.x));
			list.AddRange(BitConverter.GetBytes(vector4.y));
			list.AddRange(BitConverter.GetBytes(vector4.z));
			list.AddRange(BitConverter.GetBytes(vector4.w));
			return list;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000C830 File Offset: 0x0000AA30
		private static ICollection<byte> QuaternionToBytes(Quaternion quaternion)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(quaternion.x));
			list.AddRange(BitConverter.GetBytes(quaternion.y));
			list.AddRange(BitConverter.GetBytes(quaternion.z));
			list.AddRange(BitConverter.GetBytes(quaternion.w));
			return list;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000C88C File Offset: 0x0000AA8C
		private static ICollection<byte> RectToBytes(Rect rect)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(rect.x));
			list.AddRange(BitConverter.GetBytes(rect.y));
			list.AddRange(BitConverter.GetBytes(rect.width));
			list.AddRange(BitConverter.GetBytes(rect.height));
			return list;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000C8E8 File Offset: 0x0000AAE8
		private static ICollection<byte> Matrix4x4ToBytes(Matrix4x4 matrix4x4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(matrix4x4.m00));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m01));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m02));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m03));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m10));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m11));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m12));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m13));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m20));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m21));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m22));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m23));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m30));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m31));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m32));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m33));
			return list;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000CA1C File Offset: 0x0000AC1C
		private static ICollection<byte> AnimationCurveToBytes(AnimationCurve animationCurve)
		{
			List<byte> list = new List<byte>();
			Keyframe[] keys = animationCurve.keys;
			if (keys != null)
			{
				list.AddRange(BitConverter.GetBytes(keys.Length));
				for (int i = 0; i < keys.Length; i++)
				{
					list.AddRange(BitConverter.GetBytes(keys[i].time));
					list.AddRange(BitConverter.GetBytes(keys[i].value));
					list.AddRange(BitConverter.GetBytes(keys[i].inTangent));
					list.AddRange(BitConverter.GetBytes(keys[i].outTangent));
					list.AddRange(BitConverter.GetBytes(keys[i].tangentMode));
				}
			}
			else
			{
				list.AddRange(BitConverter.GetBytes(0));
			}
			list.AddRange(BitConverter.GetBytes((int)animationCurve.preWrapMode));
			list.AddRange(BitConverter.GetBytes((int)animationCurve.postWrapMode));
			return list;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000CB08 File Offset: 0x0000AD08
		private static void AddByteData(Type fieldType, ICollection<byte> bytes)
		{
			BinarySerialization.fieldSerializationData.dataPosition.Add(BinarySerialization.fieldSerializationData.byteData.Count);
			if (bytes != null)
			{
				BinarySerialization.fieldSerializationData.byteData.AddRange(bytes);
			}
			BinarySerialization.fieldIndex++;
		}

		// Token: 0x040000F3 RID: 243
		private static int fieldIndex;

		// Token: 0x040000F4 RID: 244
		private static TaskSerializationData taskSerializationData;

		// Token: 0x040000F5 RID: 245
		private static FieldSerializationData fieldSerializationData;
	}
}
