using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000014 RID: 20
	public class ErrorWindow : EditorWindow
	{
		// Token: 0x17000060 RID: 96
		// (set) Token: 0x0600017F RID: 383 RVA: 0x0000D360 File Offset: 0x0000B560
		public List<ErrorDetails> ErrorDetails
		{
			set
			{
				this.mErrorDetails = value;
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000D36C File Offset: 0x0000B56C
		[MenuItem("Tools/Behavior Designer/Error List", false, 2)]
		public static void ShowWindow()
		{
			ErrorWindow window = EditorWindow.GetWindow<ErrorWindow>(false, "Error List");
			window.minSize = new Vector2(400f, 200f);
			window.wantsMouseMove = true;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000D3A4 File Offset: 0x0000B5A4
		public void OnFocus()
		{
			ErrorWindow.instance = this;
			if (BehaviorDesignerWindow.instance != null)
			{
				this.mErrorDetails = BehaviorDesignerWindow.instance.ErrorDetails;
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000D3D8 File Offset: 0x0000B5D8
		public void OnGUI()
		{
			this.mScrollPosition = EditorGUILayout.BeginScrollView(this.mScrollPosition, new GUILayoutOption[0]);
			if (this.mErrorDetails != null && this.mErrorDetails.Count > 0)
			{
				for (int i = 0; i < this.mErrorDetails.Count; i++)
				{
					ErrorDetails errorDetails = this.mErrorDetails[i];
					if (errorDetails != null && (errorDetails.Type == BehaviorDesigner.Editor.ErrorDetails.ErrorType.InvalidVariableReference || (!(errorDetails.NodeDesigner == null) && errorDetails.NodeDesigner.Task != null)))
					{
						string text = string.Empty;
						switch (errorDetails.Type)
						{
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.RequiredField:
							text = string.Format("The task {0} ({1}, index {2}) requires a value for the field {3}.", new object[]
							{
								errorDetails.TaskFriendlyName,
								errorDetails.TaskType,
								errorDetails.NodeDesigner.Task.ID,
								BehaviorDesignerUtility.SplitCamelCase(errorDetails.FieldName)
							});
							break;
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.SharedVariable:
							text = string.Format("The task {0} ({1}, index {2}) has a Shared Variable field ({3}) that is marked as shared but is not referencing a Shared Variable.", new object[]
							{
								errorDetails.TaskFriendlyName,
								errorDetails.TaskType,
								errorDetails.NodeDesigner.Task.ID,
								BehaviorDesignerUtility.SplitCamelCase(errorDetails.FieldName)
							});
							break;
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.MissingChildren:
							text = string.Format("The {0} task ({1}, index {2}) is a parent task which does not have any children", errorDetails.TaskFriendlyName, errorDetails.TaskType, errorDetails.NodeDesigner.Task.ID);
							break;
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.UnknownTask:
							text = string.Format("The task at index {0} is unknown. Has a task been renamed or deleted?", errorDetails.NodeDesigner.Task.ID);
							break;
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.InvalidTaskReference:
							text = string.Format("The task {0} ({1}, index {2}) has a field ({3}) which is referencing an object within the scene. Behavior tree variables at the project level cannot reference objects within a scene.", new object[]
							{
								errorDetails.TaskFriendlyName,
								errorDetails.TaskType,
								errorDetails.NodeDesigner.Task.ID,
								BehaviorDesignerUtility.SplitCamelCase(errorDetails.FieldName)
							});
							break;
						case BehaviorDesigner.Editor.ErrorDetails.ErrorType.InvalidVariableReference:
							text = string.Format("The variable {0} is referencing an object within the scene. Behavior tree variables at the project level cannot reference objects within a scene.", errorDetails.FieldName);
							break;
						}
						EditorGUILayout.LabelField(text, (i % 2 != 0) ? BehaviorDesignerUtility.ErrorListDarkBackground : BehaviorDesignerUtility.ErrorListLightBackground, new GUILayoutOption[]
						{
							GUILayout.Height(30f),
							GUILayout.Width((float)(Screen.width - 7))
						});
					}
				}
			}
			else if (!BehaviorDesignerPreferences.GetBool(BDPreferences.ErrorChecking))
			{
				EditorGUILayout.LabelField("Enable realtime error checking from the preferences to view the errors.", BehaviorDesignerUtility.ErrorListLightBackground, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.LabelField("The behavior tree has no errors.", BehaviorDesignerUtility.ErrorListLightBackground, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndScrollView();
		}

		// Token: 0x04000105 RID: 261
		private List<ErrorDetails> mErrorDetails;

		// Token: 0x04000106 RID: 262
		private Vector2 mScrollPosition;

		// Token: 0x04000107 RID: 263
		public static ErrorWindow instance;
	}
}
