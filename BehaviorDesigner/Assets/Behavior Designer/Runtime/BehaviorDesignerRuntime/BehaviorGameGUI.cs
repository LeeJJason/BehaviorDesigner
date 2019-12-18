using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000005 RID: 5
	[AddComponentMenu("Behavior Designer/Behavior Game GUI")]
	public class BehaviorGameGUI : MonoBehaviour
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00003524 File Offset: 0x00001724
		public void Start()
		{
			this.mainCamera = Camera.main;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003534 File Offset: 0x00001734
		public void OnGUI()
		{
			if (this.behaviorManager == null)
			{
				this.behaviorManager = BehaviorManager.instance;
			}
			if (this.behaviorManager == null || this.mainCamera == null)
			{
				return;
			}
			List<BehaviorManager.BehaviorTree> behaviorTrees = this.behaviorManager.BehaviorTrees;
			for (int i = 0; i < behaviorTrees.Count; i++)
			{
				BehaviorManager.BehaviorTree behaviorTree = behaviorTrees[i];
				string text = string.Empty;
				for (int j = 0; j < behaviorTree.activeStack.Count; j++)
				{
					Stack<int> stack = behaviorTree.activeStack[j];
					if (stack.Count != 0)
					{
						Task task = behaviorTree.taskList[stack.Peek()];
						if (task is Tasks.Action)
						{
							text = text + behaviorTree.taskList[behaviorTree.activeStack[j].Peek()].FriendlyName + ((j >= behaviorTree.activeStack.Count - 1) ? string.Empty : "\n");
						}
					}
				}
				Transform transform = behaviorTree.behavior.transform;
				Vector3 vector = Camera.main.WorldToScreenPoint(transform.position);
				Vector2 vector2 = GUIUtility.ScreenToGUIPoint(vector);
				GUIContent guicontent = new GUIContent(text);
				Vector2 vector3 = GUI.skin.label.CalcSize(guicontent);
				vector3.x += 14f;
				vector3.y += 5f;
				GUI.Box(new Rect(vector2.x - vector3.x / 2f, (float)Screen.height - vector2.y + vector3.y / 2f, vector3.x, vector3.y), guicontent);
			}
		}

		// Token: 0x04000029 RID: 41
		private BehaviorManager behaviorManager;

		// Token: 0x0400002A RID: 42
		private Camera mainCamera;
	}
}
