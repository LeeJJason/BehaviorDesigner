using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace BehaviorDesigner.Editor
{
	// Token: 0x02000013 RID: 19
	public static class ErrorCheck
	{
		// Token: 0x06000179 RID: 377 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		public static List<ErrorDetails> CheckForErrors(BehaviorSource behaviorSource)
		{
			if (behaviorSource == null || behaviorSource.EntryTask == null)
			{
				return null;
			}
			List<ErrorDetails> result = null;
			bool flag = AssetDatabase.GetAssetPath(behaviorSource.Owner.GetObject()).Length > 0;
			ErrorCheck.CheckTaskForErrors(behaviorSource.EntryTask, flag, ref result);
			if (behaviorSource.RootTask == null)
			{
				ErrorCheck.AddError(ref result, ErrorDetails.ErrorType.MissingChildren, behaviorSource.EntryTask, -1, null);
			}
			if (behaviorSource.RootTask != null)
			{
				ErrorCheck.CheckTaskForErrors(behaviorSource.RootTask, flag, ref result);
			}
			if (!EditorApplication.isPlaying && flag && behaviorSource.Variables != null)
			{
				for (int i = 0; i < behaviorSource.Variables.Count; i++)
				{
					object value = behaviorSource.Variables[i].GetValue();
					if (value is Object && AssetDatabase.GetAssetPath(value as Object).Length == 0)
					{
						ErrorCheck.AddError(ref result, ErrorDetails.ErrorType.InvalidVariableReference, null, i, behaviorSource.Variables[i].Name);
					}
				}
			}
			return result;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000CFAC File Offset: 0x0000B1AC
		private static void CheckTaskForErrors(Task task, bool projectLevelBehavior, ref List<ErrorDetails> errorDetails)
		{
			if (task.Disabled)
			{
				return;
			}
			if (task is UnknownTask || task is UnknownParentTask)
			{
				ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.UnknownTask, task, -1, null);
			}
			if (task.GetType().GetCustomAttributes(typeof(SkipErrorCheckAttribute), false).Length == 0)
			{
				FieldInfo[] allFields = TaskUtility.GetAllFields(task.GetType());
				for (int i = 0; i < allFields.Length; i++)
				{
					ErrorCheck.CheckField(task, projectLevelBehavior, ref errorDetails, allFields[i], i, allFields[i].GetValue(task));
				}
			}
			if (task is ParentTask && task.NodeData.NodeDesigner != null && !(task.NodeData.NodeDesigner as NodeDesigner).IsEntryDisplay)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children == null || parentTask.Children.Count == 0)
				{
					ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.MissingChildren, task, -1, null);
				}
				else
				{
					for (int j = 0; j < parentTask.Children.Count; j++)
					{
						ErrorCheck.CheckTaskForErrors(parentTask.Children[j], projectLevelBehavior, ref errorDetails);
					}
				}
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000D0CC File Offset: 0x0000B2CC
		private static void CheckField(Task task, bool projectLevelBehavior, ref List<ErrorDetails> errorDetails, FieldInfo field, int index, object value)
		{
			if (value == null)
			{
				return;
			}
			if (TaskUtility.HasAttribute(field, typeof(RequiredFieldAttribute)) && !ErrorCheck.IsRequiredFieldValid(field.FieldType, value))
			{
				ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.RequiredField, task, index, field.Name);
			}
			if (typeof(SharedVariable).IsAssignableFrom(field.FieldType))
			{
				SharedVariable sharedVariable = value as SharedVariable;
				if (sharedVariable != null)
				{
					if (sharedVariable.IsShared && string.IsNullOrEmpty(sharedVariable.Name) && !TaskUtility.HasAttribute(field, typeof(SharedRequiredAttribute)))
					{
						ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.SharedVariable, task, index, field.Name);
					}
					object value2 = sharedVariable.GetValue();
					if (!EditorApplication.isPlaying && projectLevelBehavior && !sharedVariable.IsShared && value2 is Object && AssetDatabase.GetAssetPath(value2 as Object).Length <= 0)
					{
						ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.InvalidTaskReference, task, index, field.Name);
					}
				}
			}
			else if (value is Object)
			{
				bool flag = AssetDatabase.GetAssetPath(value as Object).Length > 0;
				if (!EditorApplication.isPlaying && !flag)
				{
					ErrorCheck.AddError(ref errorDetails, ErrorDetails.ErrorType.InvalidTaskReference, task, index, field.Name);
				}
			}
			else if (!typeof(Delegate).IsAssignableFrom(field.FieldType) && (field.FieldType.IsClass || (field.FieldType.IsValueType && !field.FieldType.IsPrimitive)))
			{
				FieldInfo[] allFields = TaskUtility.GetAllFields(field.FieldType);
				for (int i = 0; i < allFields.Length; i++)
				{
					ErrorCheck.CheckField(task, projectLevelBehavior, ref errorDetails, allFields[i], index, allFields[i].GetValue(value));
				}
			}
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000D2B0 File Offset: 0x0000B4B0
		private static void AddError(ref List<ErrorDetails> errorDetails, ErrorDetails.ErrorType type, Task task, int index, string fieldName)
		{
			if (errorDetails == null)
			{
				errorDetails = new List<ErrorDetails>();
			}
			errorDetails.Add(new ErrorDetails(type, task, index, fieldName));
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000D2D4 File Offset: 0x0000B4D4
		public static bool IsRequiredFieldValid(Type fieldType, object value)
		{
			if (value == null || value.Equals(null))
			{
				return false;
			}
			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				IList list = value as IList;
				if (list.Count == 0)
				{
					return false;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == null || list[i].Equals(null))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
