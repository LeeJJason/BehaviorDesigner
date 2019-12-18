using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000003 RID: 3
	public class AssetCreator : EditorWindow
	{
		// Token: 0x17000001 RID: 1
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002590 File Offset: 0x00000790
		private bool CSharp
		{
			set
			{
				this.m_CSharp = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000259C File Offset: 0x0000079C
		private AssetCreator.AssetClassType ClassType
		{
			set
			{
				this.m_classType = value;
				switch (this.m_classType)
				{
				case AssetCreator.AssetClassType.Action:
					this.m_AssetName = "NewAction";
					break;
				case AssetCreator.AssetClassType.Conditional:
					this.m_AssetName = "NewConditional";
					break;
				case AssetCreator.AssetClassType.SharedVariable:
					this.m_AssetName = "SharedNewVariable";
					break;
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002600 File Offset: 0x00000800
		public static void ShowWindow(AssetCreator.AssetClassType classType, bool cSharp)
		{
			AssetCreator window = EditorWindow.GetWindow<AssetCreator>(true, "Asset Name");
			EditorWindow editorWindow = window;
			Vector2 vector = new Vector2(300f, 55f);
			window.maxSize = vector;
			editorWindow.minSize = vector;
			window.ClassType = classType;
			window.CSharp = cSharp;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002648 File Offset: 0x00000848
		private void OnGUI()
		{
			this.m_AssetName = EditorGUILayout.TextField("Name", this.m_AssetName, new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("OK", new GUILayoutOption[0]))
			{
				AssetCreator.CreateScript(this.m_AssetName, this.m_classType, this.m_CSharp);
				base.Close();
			}
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				base.Close();
			}
			EditorGUILayout.EndHorizontal();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000026D0 File Offset: 0x000008D0
		public static void CreateAsset(Type type, string name)
		{
			ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);
			string text = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (text == string.Empty)
			{
				text = "Assets";
			}
			else if (Path.GetExtension(text) != string.Empty)
			{
				text = text.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), string.Empty);
			}
			string text2 = AssetDatabase.GenerateUniqueAssetPath(text + "/" + name + ".asset");
			AssetDatabase.CreateAsset(scriptableObject, text2);
			AssetDatabase.SaveAssets();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002760 File Offset: 0x00000960
		private static void CreateScript(string name, AssetCreator.AssetClassType classType, bool cSharp)
		{
			string text = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (text == string.Empty)
			{
				text = "Assets";
			}
			else if (Path.GetExtension(text) != string.Empty)
			{
				text = text.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), string.Empty);
			}
			string path = AssetDatabase.GenerateUniqueAssetPath(text + "/" + name + ((!cSharp) ? ".js" : ".cs"));
			StreamWriter streamWriter = new StreamWriter(path, false);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string value = string.Empty;
			switch (classType)
			{
			case AssetCreator.AssetClassType.Action:
				value = AssetCreator.ActionTaskContents(fileNameWithoutExtension, cSharp);
				break;
			case AssetCreator.AssetClassType.Conditional:
				value = AssetCreator.ConditionalTaskContents(fileNameWithoutExtension, cSharp);
				break;
			case AssetCreator.AssetClassType.SharedVariable:
				value = AssetCreator.SharedVariableContents(fileNameWithoutExtension);
				break;
			}
			streamWriter.Write(value);
			streamWriter.Close();
			AssetDatabase.Refresh();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002858 File Offset: 0x00000A58
		private static string ActionTaskContents(string name, bool cSharp)
		{
			if (cSharp)
			{
				return "using UnityEngine;\nusing BehaviorDesigner.Runtime;\nusing BehaviorDesigner.Runtime.Tasks;\n\npublic class " + name + " : Action\n{\n\tpublic override void OnStart()\n\t{\n\t\t\n\t}\n\n\tpublic override TaskStatus OnUpdate()\n\t{\n\t\treturn TaskStatus.Success;\n\t}\n}";
			}
			return "#pragma strict\n\nclass " + name + " extends BehaviorDesigner.Runtime.Tasks.Action\n{\n\tfunction OnStart()\n\t{\n\t\t\n\t}\n\n\tfunction OnUpdate()\n\t{\n\t\treturn BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;\n\t}\n}";
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002884 File Offset: 0x00000A84
		private static string ConditionalTaskContents(string name, bool cSharp)
		{
			if (cSharp)
			{
				return "using UnityEngine;\nusing BehaviorDesigner.Runtime;\nusing BehaviorDesigner.Runtime.Tasks;\n\npublic class " + name + " : Conditional\n{\n\tpublic override TaskStatus OnUpdate()\n\t{\n\t\treturn TaskStatus.Success;\n\t}\n}";
			}
			return "#pragma strict\n\nclass " + name + " extends BehaviorDesigner.Runtime.Tasks.Conditional\n{\n\tfunction OnUpdate()\n\t{\n\t\treturn BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;\n\t}\n}";
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000028B0 File Offset: 0x00000AB0
		private static string SharedVariableContents(string name)
		{
			string text = name.Remove(0, 6);
			return string.Concat(new string[]
			{
				"using UnityEngine;\nusing BehaviorDesigner.Runtime;\n\n[System.Serializable]\npublic class ",
				text,
				"\n{\n\n}\n\n[System.Serializable]\npublic class ",
				name,
				" : SharedVariable<",
				text,
				">\n{\n\tpublic override string ToString() { return mValue == null ? \"null\" : mValue.ToString(); }\n\tpublic static implicit operator ",
				name,
				"(",
				text,
				" value) { return new ",
				name,
				" { mValue = value }; }\n}"
			});
		}

		// Token: 0x04000001 RID: 1
		private bool m_CSharp = true;

		// Token: 0x04000002 RID: 2
		private AssetCreator.AssetClassType m_classType;

		// Token: 0x04000003 RID: 3
		private string m_AssetName;

		// Token: 0x02000004 RID: 4
		public enum AssetClassType
		{
			// Token: 0x04000005 RID: 5
			Action,
			// Token: 0x04000006 RID: 6
			Conditional,
			// Token: 0x04000007 RID: 7
			SharedVariable
		}
	}
}
