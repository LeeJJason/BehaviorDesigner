using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000024 RID: 36
	public class TaskCopier : UnityEditor.Editor
    {
		// Token: 0x06000259 RID: 601 RVA: 0x000164C4 File Offset: 0x000146C4
		public static TaskSerializer CopySerialized(Task task)
		{
			TaskSerializer taskSerializer = new TaskSerializer();
			taskSerializer.offset = (task.NodeData.NodeDesigner as NodeDesigner).GetAbsolutePosition() + new Vector2(10f, 10f);
			taskSerializer.unityObjects = new List<Object>();
			taskSerializer.serialization = MiniJSON.Serialize(JSONSerialization.SerializeTask(task, false, ref taskSerializer.unityObjects));
			return taskSerializer;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0001652C File Offset: 0x0001472C
		public static Task PasteTask(BehaviorSource behaviorSource, TaskSerializer serializer)
		{
			Dictionary<int, Task> dictionary = new Dictionary<int, Task>();
			Task task = JSONDeserialization.DeserializeTask(behaviorSource, MiniJSON.Deserialize(serializer.serialization) as Dictionary<string, object>, ref dictionary, serializer.unityObjects);
			TaskCopier.CheckSharedVariables(behaviorSource, task);
			return task;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00016568 File Offset: 0x00014768
		private static void CheckSharedVariables(BehaviorSource behaviorSource, Task task)
		{
			if (task == null)
			{
				return;
			}
			TaskCopier.CheckSharedVariableFields(behaviorSource, task, task);
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						TaskCopier.CheckSharedVariables(behaviorSource, parentTask.Children[i]);
					}
				}
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000165D0 File Offset: 0x000147D0
		private static void CheckSharedVariableFields(BehaviorSource behaviorSource, Task task, object obj)
		{
			if (obj == null)
			{
				return;
			}
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType))
				{
					SharedVariable sharedVariable = allFields[i].GetValue(obj) as SharedVariable;
					if (sharedVariable.IsShared && !string.IsNullOrEmpty(sharedVariable.Name) && behaviorSource.GetVariable(sharedVariable.Name) == null)
					{
						behaviorSource.SetVariable(sharedVariable.Name, sharedVariable);
					}
					TaskCopier.CheckSharedVariableFields(behaviorSource, task, sharedVariable);
				}
				else if (allFields[i].FieldType.IsClass && !allFields[i].FieldType.Equals(typeof(Type)) && !typeof(Delegate).IsAssignableFrom(allFields[i].FieldType))
				{
					TaskCopier.CheckSharedVariableFields(behaviorSource, task, allFields[i].GetValue(obj));
				}
			}
		}
	}
}
