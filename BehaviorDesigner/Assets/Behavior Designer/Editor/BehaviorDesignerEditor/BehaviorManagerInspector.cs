using System;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200000C RID: 12
	[CustomEditor(typeof(BehaviorManager))]
	public class BehaviorManagerInspector : UnityEditor.Editor
    {
		// Token: 0x0600014A RID: 330 RVA: 0x0000B9CC File Offset: 0x00009BCC
		public override void OnInspectorGUI()
		{
			BehaviorManager behaviorManager = this.target as BehaviorManager;
			behaviorManager.UpdateInterval = (UpdateIntervalType)EditorGUILayout.EnumPopup("Update Interval", behaviorManager.UpdateInterval, new GUILayoutOption[0]);
			if (behaviorManager.UpdateInterval == UpdateIntervalType.SpecifySeconds)
			{
				EditorGUI.indentLevel++;
				behaviorManager.UpdateIntervalSeconds = EditorGUILayout.FloatField("Seconds", behaviorManager.UpdateIntervalSeconds, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			behaviorManager.ExecutionsPerTick = (BehaviorManager.ExecutionsPerTickType)EditorGUILayout.EnumPopup("Task Execution Type", behaviorManager.ExecutionsPerTick, new GUILayoutOption[0]);
			if (behaviorManager.ExecutionsPerTick == BehaviorManager.ExecutionsPerTickType.Count)
			{
				EditorGUI.indentLevel++;
				behaviorManager.MaxTaskExecutionsPerTick = EditorGUILayout.IntField("Max Execution Count", behaviorManager.MaxTaskExecutionsPerTick, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
		}
	}
}
