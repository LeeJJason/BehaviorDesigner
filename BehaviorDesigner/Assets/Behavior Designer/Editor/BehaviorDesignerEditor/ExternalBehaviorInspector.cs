using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000015 RID: 21
	[CustomEditor(typeof(ExternalBehavior))]
	public class ExternalBehaviorInspector : UnityEditor.Editor
    {
		// Token: 0x06000185 RID: 389 RVA: 0x0000D694 File Offset: 0x0000B894
		public override void OnInspectorGUI()
		{
			ExternalBehavior externalBehavior = this.target as ExternalBehavior;
			if (externalBehavior == null)
			{
				return;
			}
			if (ExternalBehaviorInspector.DrawInspectorGUI(externalBehavior.BehaviorSource, true, ref this.mShowVariables))
			{
				BehaviorDesignerUtility.SetObjectDirty(externalBehavior);
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
		public void Reset()
		{
			ExternalBehavior externalBehavior = this.target as ExternalBehavior;
			if (externalBehavior == null)
			{
				return;
			}
			if (externalBehavior.BehaviorSource.Owner == null)
			{
				externalBehavior.BehaviorSource.Owner = externalBehavior;
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000D71C File Offset: 0x0000B91C
		public static bool DrawInspectorGUI(BehaviorSource behaviorSource, bool fromInspector, ref bool showVariables)
		{
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Behavior Name", new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			behaviorSource.behaviorName = EditorGUILayout.TextField(behaviorSource.behaviorName, new GUILayoutOption[0]);
			if (fromInspector && GUILayout.Button("Open", new GUILayoutOption[0]))
			{
				BehaviorDesignerWindow.ShowWindow();
				BehaviorDesignerWindow.instance.LoadBehavior(behaviorSource, false, true);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Behavior Description", new GUILayoutOption[0]);
			behaviorSource.behaviorDescription = EditorGUILayout.TextArea(behaviorSource.behaviorDescription, new GUILayoutOption[]
			{
				GUILayout.Height(48f)
			});
			if (fromInspector)
			{
				string text = "BehaviorDesigner.VariablesFoldout." + behaviorSource.GetHashCode();
				if (showVariables = EditorGUILayout.Foldout(EditorPrefs.GetBool(text, true), "Variables"))
				{
					EditorGUI.indentLevel++;
					List<SharedVariable> allVariables = behaviorSource.GetAllVariables();
					if (allVariables != null && VariableInspector.DrawAllVariables(false, behaviorSource, ref allVariables, false, ref ExternalBehaviorInspector.variablePosition, ref ExternalBehaviorInspector.selectedVariableIndex, ref ExternalBehaviorInspector.selectedVariableName, ref ExternalBehaviorInspector.selectedVariableTypeIndex, true, false))
					{
						if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
						{
							BinarySerialization.Save(behaviorSource);
						}
						else
						{
							JSONSerialization.Save(behaviorSource);
						}
					}
					EditorGUI.indentLevel--;
				}
				EditorPrefs.SetBool(text, showVariables);
			}
			return EditorGUI.EndChangeCheck();
		}

		// Token: 0x04000108 RID: 264
		private bool mShowVariables;

		// Token: 0x04000109 RID: 265
		private static List<float> variablePosition;

		// Token: 0x0400010A RID: 266
		private static int selectedVariableIndex = -1;

		// Token: 0x0400010B RID: 267
		private static string selectedVariableName;

		// Token: 0x0400010C RID: 268
		private static int selectedVariableTypeIndex;
	}
}
