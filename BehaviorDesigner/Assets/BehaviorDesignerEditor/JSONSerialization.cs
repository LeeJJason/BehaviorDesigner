using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
namespace BehaviorDesigner.Editor
{
	// Token: 0x0200001C RID: 28
	public class JSONSerialization : Object
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x000123A8 File Offset: 0x000105A8
		public static void Save(BehaviorSource behaviorSource)
		{
			behaviorSource.CheckForSerialization(false, null);
			JSONSerialization.taskSerializationData = new TaskSerializationData();
			JSONSerialization.fieldSerializationData = JSONSerialization.taskSerializationData.fieldSerializationData;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (behaviorSource.EntryTask != null)
			{
				dictionary.Add("EntryTask", JSONSerialization.SerializeTask(behaviorSource.EntryTask, true, ref JSONSerialization.fieldSerializationData.unityObjects));
			}
			if (behaviorSource.RootTask != null)
			{
				dictionary.Add("RootTask", JSONSerialization.SerializeTask(behaviorSource.RootTask, true, ref JSONSerialization.fieldSerializationData.unityObjects));
			}
			if (behaviorSource.DetachedTasks != null && behaviorSource.DetachedTasks.Count > 0)
			{
				Dictionary<string, object>[] array = new Dictionary<string, object>[behaviorSource.DetachedTasks.Count];
				for (int i = 0; i < behaviorSource.DetachedTasks.Count; i++)
				{
					array[i] = JSONSerialization.SerializeTask(behaviorSource.DetachedTasks[i], true, ref JSONSerialization.fieldSerializationData.unityObjects);
				}
				dictionary.Add("DetachedTasks", array);
			}
			if (behaviorSource.Variables != null && behaviorSource.Variables.Count > 0)
			{
				dictionary.Add("Variables", JSONSerialization.SerializeVariables(behaviorSource.Variables, ref JSONSerialization.fieldSerializationData.unityObjects));
			}
			JSONSerialization.taskSerializationData.Version = "1.5.7";
			JSONSerialization.taskSerializationData.JSONSerialization = MiniJSON.Serialize(dictionary);
			behaviorSource.TaskData = JSONSerialization.taskSerializationData;
			if (behaviorSource.Owner != null && !behaviorSource.Owner.Equals(null))
			{
				BehaviorDesignerUtility.SetObjectDirty(behaviorSource.Owner.GetObject());
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0001253C File Offset: 0x0001073C
		public static void Save(GlobalVariables variables)
		{
			if (variables == null)
			{
				return;
			}
			JSONSerialization.variableSerializationData = new VariableSerializationData();
			JSONSerialization.fieldSerializationData = JSONSerialization.variableSerializationData.fieldSerializationData;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Variables", JSONSerialization.SerializeVariables(variables.Variables, ref JSONSerialization.fieldSerializationData.unityObjects));
			JSONSerialization.variableSerializationData.JSONSerialization = MiniJSON.Serialize(dictionary);
			variables.VariableData = JSONSerialization.variableSerializationData;
			variables.Version = "1.5.7";
			BehaviorDesignerUtility.SetObjectDirty(variables);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x000125C4 File Offset: 0x000107C4
		private static Dictionary<string, object>[] SerializeVariables(List<SharedVariable> variables, ref List<Object> unityObjects)
		{
			Dictionary<string, object>[] array = new Dictionary<string, object>[variables.Count];
			for (int i = 0; i < variables.Count; i++)
			{
				array[i] = JSONSerialization.SerializeVariable(variables[i], ref unityObjects);
			}
			return array;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00012608 File Offset: 0x00010808
		public static Dictionary<string, object> SerializeTask(Task task, bool serializeChildren, ref List<Object> unityObjects)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Type", task.GetType());
			dictionary.Add("NodeData", JSONSerialization.SerializeNodeData(task.NodeData));
			dictionary.Add("ID", task.ID);
			dictionary.Add("Name", task.FriendlyName);
			dictionary.Add("Instant", task.IsInstant);
			if (task.Disabled)
			{
				dictionary.Add("Disabled", task.Disabled);
			}
			JSONSerialization.SerializeFields(task, ref dictionary, ref unityObjects);
			if (serializeChildren && task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null && parentTask.Children.Count > 0)
				{
					Dictionary<string, object>[] array = new Dictionary<string, object>[parentTask.Children.Count];
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						array[i] = JSONSerialization.SerializeTask(parentTask.Children[i], serializeChildren, ref unityObjects);
					}
					dictionary.Add("Children", array);
				}
			}
			return dictionary;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00012730 File Offset: 0x00010930
		private static Dictionary<string, object> SerializeNodeData(NodeData nodeData)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Offset", nodeData.Offset);
			if (nodeData.Comment.Length > 0)
			{
				dictionary.Add("Comment", nodeData.Comment);
			}
			if (nodeData.IsBreakpoint)
			{
				dictionary.Add("IsBreakpoint", nodeData.IsBreakpoint);
			}
			if (nodeData.Collapsed)
			{
				dictionary.Add("Collapsed", nodeData.Collapsed);
			}
			if (nodeData.ColorIndex != 0)
			{
				dictionary.Add("ColorIndex", nodeData.ColorIndex);
			}
			if (nodeData.WatchedFieldNames != null && nodeData.WatchedFieldNames.Count > 0)
			{
				dictionary.Add("WatchedFields", nodeData.WatchedFieldNames);
			}
			return dictionary;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0001280C File Offset: 0x00010A0C
		private static Dictionary<string, object> SerializeVariable(SharedVariable sharedVariable, ref List<Object> unityObjects)
		{
			if (sharedVariable == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Type", sharedVariable.GetType());
			dictionary.Add("Name", sharedVariable.Name);
			if (sharedVariable.IsShared)
			{
				dictionary.Add("IsShared", sharedVariable.IsShared);
			}
			if (sharedVariable.IsGlobal)
			{
				dictionary.Add("IsGlobal", sharedVariable.IsGlobal);
			}
			if (sharedVariable.NetworkSync)
			{
				dictionary.Add("NetworkSync", sharedVariable.NetworkSync);
			}
			if (!string.IsNullOrEmpty(sharedVariable.PropertyMapping))
			{
				dictionary.Add("PropertyMapping", sharedVariable.PropertyMapping);
				if (!object.Equals(sharedVariable.PropertyMappingOwner, null))
				{
					dictionary.Add("PropertyMappingOwner", unityObjects.Count);
					unityObjects.Add(sharedVariable.PropertyMappingOwner);
				}
			}
			JSONSerialization.SerializeFields(sharedVariable, ref dictionary, ref unityObjects);
			return dictionary;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0001290C File Offset: 0x00010B0C
		private static void SerializeFields(object obj, ref Dictionary<string, object> dict, ref List<Object> unityObjects)
		{
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if (!BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(NonSerializedAttribute)) && ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(SerializeField))) && (!(obj is ParentTask) || !allFields[i].Name.Equals("children")))
				{
					if (allFields[i].GetValue(obj) != null)
					{
						string key = (allFields[i].FieldType.Name + allFields[i].Name).ToString();
						if (typeof(IList).IsAssignableFrom(allFields[i].FieldType))
						{
							IList list = allFields[i].GetValue(obj) as IList;
							if (list != null)
							{
								List<object> list2 = new List<object>();
								for (int j = 0; j < list.Count; j++)
								{
									if (list[j] == null)
									{
										list2.Add(null);
									}
									else
									{
										Type type = list[j].GetType();
										if (list[j] is Task)
										{
											Task task = list[j] as Task;
											list2.Add(task.ID);
										}
										else if (list[j] is SharedVariable)
										{
											list2.Add(JSONSerialization.SerializeVariable(list[j] as SharedVariable, ref unityObjects));
										}
										else if (list[j] is Object)
										{
											Object @object = list[j] as Object;
											if (!object.ReferenceEquals(@object, null) && @object != null)
											{
												list2.Add(unityObjects.Count);
												unityObjects.Add(@object);
											}
										}
										else if (type.Equals(typeof(LayerMask)))
										{
											list2.Add(((LayerMask)list[j]).value);
										}
										else if (type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(Vector2)) || type.Equals(typeof(Vector3)) || type.Equals(typeof(Vector4)) || type.Equals(typeof(Quaternion)) || type.Equals(typeof(Matrix4x4)) || type.Equals(typeof(Color)) || type.Equals(typeof(Rect)))
										{
											list2.Add(list[j]);
										}
										else
										{
											Dictionary<string, object> item = new Dictionary<string, object>();
											JSONSerialization.SerializeFields(list[j], ref item, ref unityObjects);
											list2.Add(item);
										}
									}
								}
								if (list2 != null)
								{
									dict.Add(key, list2);
								}
							}
						}
						else if (typeof(Task).IsAssignableFrom(allFields[i].FieldType))
						{
							Task task2 = allFields[i].GetValue(obj) as Task;
							if (task2 != null)
							{
								if (BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(InspectTaskAttribute)))
								{
									Dictionary<string, object> dictionary = new Dictionary<string, object>();
									dictionary.Add("Type", task2.GetType());
									JSONSerialization.SerializeFields(task2, ref dictionary, ref unityObjects);
									dict.Add(key, dictionary);
								}
								else
								{
									dict.Add(key, task2.ID);
								}
							}
						}
						else if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType))
						{
							if (!dict.ContainsKey(key))
							{
								dict.Add(key, JSONSerialization.SerializeVariable(allFields[i].GetValue(obj) as SharedVariable, ref unityObjects));
							}
						}
						else if (typeof(Object).IsAssignableFrom(allFields[i].FieldType))
						{
							Object object2 = allFields[i].GetValue(obj) as Object;
							if (!object.ReferenceEquals(object2, null) && object2 != null)
							{
								dict.Add(key, unityObjects.Count);
								unityObjects.Add(object2);
							}
						}
						else if (allFields[i].FieldType.Equals(typeof(LayerMask)))
						{
							dict.Add(key, ((LayerMask)allFields[i].GetValue(obj)).value);
						}
						else if (allFields[i].FieldType.IsPrimitive || allFields[i].FieldType.IsEnum || allFields[i].FieldType.Equals(typeof(string)) || allFields[i].FieldType.Equals(typeof(Vector2)) || allFields[i].FieldType.Equals(typeof(Vector3)) || allFields[i].FieldType.Equals(typeof(Vector4)) || allFields[i].FieldType.Equals(typeof(Quaternion)) || allFields[i].FieldType.Equals(typeof(Matrix4x4)) || allFields[i].FieldType.Equals(typeof(Color)) || allFields[i].FieldType.Equals(typeof(Rect)))
						{
							dict.Add(key, allFields[i].GetValue(obj));
						}
						else if (allFields[i].FieldType.Equals(typeof(AnimationCurve)))
						{
							AnimationCurve animationCurve = allFields[i].GetValue(obj) as AnimationCurve;
							Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
							if (animationCurve.keys != null)
							{
								Keyframe[] keys = animationCurve.keys;
								List<List<object>> list3 = new List<List<object>>();
								for (int k = 0; k < keys.Length; k++)
								{
									list3.Add(new List<object>
									{
										keys[k].time,
										keys[k].value,
										keys[k].inTangent,
										keys[k].outTangent,
										keys[k].tangentMode
									});
								}
								dictionary2.Add("Keys", list3);
							}
							dictionary2.Add("PreWrapMode", animationCurve.preWrapMode);
							dictionary2.Add("PostWrapMode", animationCurve.postWrapMode);
							dict.Add(key, dictionary2);
						}
						else
						{
							Dictionary<string, object> value = new Dictionary<string, object>();
							JSONSerialization.SerializeFields(allFields[i].GetValue(obj), ref value, ref unityObjects);
							dict.Add(key, value);
						}
					}
				}
			}
		}

		// Token: 0x0400012D RID: 301
		private static TaskSerializationData taskSerializationData;

		// Token: 0x0400012E RID: 302
		private static FieldSerializationData fieldSerializationData;

		// Token: 0x0400012F RID: 303
		private static VariableSerializationData variableSerializationData;
	}
}
