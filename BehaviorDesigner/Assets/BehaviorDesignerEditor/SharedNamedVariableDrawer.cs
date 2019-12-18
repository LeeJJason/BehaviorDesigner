using System;
using System.Collections.Generic;
using BehaviorDesigner.Editor;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

// Token: 0x02000032 RID: 50
[CustomObjectDrawer(typeof(NamedVariable))]
public class SharedNamedVariableDrawer : ObjectDrawer
{
	// Token: 0x060002DA RID: 730 RVA: 0x0001D118 File Offset: 0x0001B318
	public override void OnGUI(GUIContent label)
	{
		NamedVariable namedVariable = this.value as NamedVariable;
		EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
		if (FieldInspector.DrawFoldout(namedVariable.GetHashCode(), label))
		{
			EditorGUI.indentLevel++;
			if (SharedNamedVariableDrawer.variableNames == null)
			{
				List<Type> list = VariableInspector.FindAllSharedVariableTypes(true);
				SharedNamedVariableDrawer.variableNames = new string[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					SharedNamedVariableDrawer.variableNames[i] = list[i].Name.Remove(0, 6);
				}
			}
			int num = 0;
			string value = namedVariable.type.Remove(0, 6);
			for (int j = 0; j < SharedNamedVariableDrawer.variableNames.Length; j++)
			{
				if (SharedNamedVariableDrawer.variableNames[j].Equals(value))
				{
					num = j;
					break;
				}
			}
			namedVariable.name = EditorGUILayout.TextField("Name", namedVariable.name, new GUILayoutOption[0]);
			int num2 = EditorGUILayout.Popup("Type", num, SharedNamedVariableDrawer.variableNames, BehaviorDesignerUtility.SharedVariableToolbarPopup, new GUILayoutOption[0]);
			Type type = VariableInspector.FindAllSharedVariableTypes(true)[num2];
			if (num2 != num)
			{
				num = num2;
				namedVariable.value = (Activator.CreateInstance(type) as SharedVariable);
			}
			GUILayout.Space(3f);
			namedVariable.type = "Shared" + SharedNamedVariableDrawer.variableNames[num];
			namedVariable.value = FieldInspector.DrawSharedVariable(null, new GUIContent("Value"), null, type, namedVariable.value);
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.EndVertical();
	}

	// Token: 0x040001D5 RID: 469
	private static string[] variableNames;
}
