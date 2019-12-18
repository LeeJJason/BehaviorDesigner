using System;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200001E RID: 30
	[Serializable]
	public class NodeConnection : ScriptableObject
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0001313C File Offset: 0x0001133C
		// (set) Token: 0x060001FD RID: 509 RVA: 0x00013144 File Offset: 0x00011344
		public NodeDesigner OriginatingNodeDesigner
		{
			get
			{
				return this.originatingNodeDesigner;
			}
			set
			{
				this.originatingNodeDesigner = value;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00013150 File Offset: 0x00011350
		// (set) Token: 0x060001FF RID: 511 RVA: 0x00013158 File Offset: 0x00011358
		public NodeDesigner DestinationNodeDesigner
		{
			get
			{
				return this.destinationNodeDesigner;
			}
			set
			{
				this.destinationNodeDesigner = value;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00013164 File Offset: 0x00011364
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0001316C File Offset: 0x0001136C
		public NodeConnectionType NodeConnectionType
		{
			get
			{
				return this.nodeConnectionType;
			}
			set
			{
				this.nodeConnectionType = value;
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00013178 File Offset: 0x00011378
		public void select()
		{
			this.selected = true;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00013184 File Offset: 0x00011384
		public void deselect()
		{
			this.selected = false;
		}

		// Token: 0x1700006A RID: 106
		// (set) Token: 0x06000204 RID: 516 RVA: 0x00013190 File Offset: 0x00011390
		public float HorizontalHeight
		{
			set
			{
				this.horizontalHeight = value;
				this.horizontalDirty = true;
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000131A0 File Offset: 0x000113A0
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000131AC File Offset: 0x000113AC
		public void LoadConnection(NodeDesigner nodeDesigner, NodeConnectionType nodeConnectionType)
		{
			this.originatingNodeDesigner = nodeDesigner;
			this.nodeConnectionType = nodeConnectionType;
			this.selected = false;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x000131C4 File Offset: 0x000113C4
		public void DrawConnection(Vector2 offset, bool disabled)
		{
			this.DrawConnection(this.OriginatingNodeDesigner.GetConnectionPosition(offset, NodeConnectionType.Outgoing), this.DestinationNodeDesigner.GetConnectionPosition(offset, NodeConnectionType.Incoming), disabled);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x000131F4 File Offset: 0x000113F4
		public void DrawConnection(Vector2 source, Vector2 destination, bool disabled)
		{
			Color color = (!disabled) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
			bool flag = this.destinationNodeDesigner != null && this.destinationNodeDesigner.Task != null && this.destinationNodeDesigner.Task.NodeData.PushTime != -1f && this.destinationNodeDesigner.Task.NodeData.PushTime >= this.destinationNodeDesigner.Task.NodeData.PopTime;
			float num = (!BehaviorDesignerPreferences.GetBool(BDPreferences.FadeNodes)) ? 0.01f : 0.5f;
			if (this.selected)
			{
				if (disabled)
				{
					if (EditorGUIUtility.isProSkin)
					{
						color = this.selectedDisabledProColor;
					}
					else
					{
						color = this.selectedDisabledStandardColor;
					}
				}
				else if (EditorGUIUtility.isProSkin)
				{
					color = this.selectedEnabledProColor;
				}
				else
				{
					color = this.selectedEnabledStandardColor;
				}
			}
			else if (flag)
			{
				if (EditorGUIUtility.isProSkin)
				{
					color = this.taskRunningProColor;
				}
				else
				{
					color = this.taskRunningStandardColor;
				}
			}
			else if (num != 0f && this.destinationNodeDesigner != null && this.destinationNodeDesigner.Task != null && this.destinationNodeDesigner.Task.NodeData.PopTime != -1f && this.destinationNodeDesigner.Task.NodeData.PopTime <= Time.realtimeSinceStartup && Time.realtimeSinceStartup - this.destinationNodeDesigner.Task.NodeData.PopTime < num)
			{
				float num2 = 1f - (Time.realtimeSinceStartup - this.destinationNodeDesigner.Task.NodeData.PopTime) / num;
				Color white = Color.white;
				if (EditorGUIUtility.isProSkin)
				{
					white = this.taskRunningProColor;
				}
				else
				{
					white = this.taskRunningStandardColor;
				}
				color = Color.Lerp(Color.white, white, num2);
			}
			Handles.color = color;
			if (this.horizontalDirty)
			{
				this.startHorizontalBreak = new Vector2(source.x, this.horizontalHeight);
				this.endHorizontalBreak = new Vector2(destination.x, this.horizontalHeight);
				this.horizontalDirty = false;
			}
			this.linePoints[0] = source;
			this.linePoints[1] = this.startHorizontalBreak;
			this.linePoints[2] = this.endHorizontalBreak;
			this.linePoints[3] = destination;
			Handles.DrawPolyLine(this.linePoints);
			for (int i = 0; i < this.linePoints.Length; i++)
			{
				Vector3[] array = this.linePoints;
				int num3 = i;
				array[num3].x = array[num3].x + 1f;
				Vector3[] array2 = this.linePoints;
				int num4 = i;
				array2[num4].y = array2[num4].y + 1f;
			}
			Handles.DrawPolyLine(this.linePoints);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00013534 File Offset: 0x00011734
		public bool Contains(Vector2 point, Vector2 offset)
		{
			Vector2 center = this.originatingNodeDesigner.OutgoingConnectionRect(offset).center;
			Vector2 vector = new Vector2(center.x, this.horizontalHeight);
			float num = Mathf.Abs(point.x - center.x);
			if (num < 7f && ((point.y >= center.y && point.y <= vector.y) || (point.y <= center.y && point.y >= vector.y)))
			{
				return true;
			}
			Rect rect = this.destinationNodeDesigner.IncomingConnectionRect(offset);
			Vector2 vector2 = new Vector2(rect.center.x, rect.y);
			Vector2 vector3 = new Vector2(vector2.x, this.horizontalHeight);
			num = Mathf.Abs(point.y - this.horizontalHeight);
			if (num < 7f && ((point.x <= center.x && point.x >= vector3.x) || (point.x >= center.x && point.x <= vector3.x)))
			{
				return true;
			}
			num = Mathf.Abs(point.x - vector2.x);
			return num < 7f && ((point.y >= vector2.y && point.y <= vector3.y) || (point.y <= vector2.y && point.y >= vector3.y));
		}

		// Token: 0x04000134 RID: 308
		[SerializeField]
		private NodeDesigner originatingNodeDesigner;

		// Token: 0x04000135 RID: 309
		[SerializeField]
		private NodeDesigner destinationNodeDesigner;

		// Token: 0x04000136 RID: 310
		[SerializeField]
		private NodeConnectionType nodeConnectionType;

		// Token: 0x04000137 RID: 311
		[SerializeField]
		private bool selected;

		// Token: 0x04000138 RID: 312
		[SerializeField]
		private float horizontalHeight;

		// Token: 0x04000139 RID: 313
		private readonly Color selectedDisabledProColor = new Color(0.1316f, 0.3212f, 0.4803f);

		// Token: 0x0400013A RID: 314
		private readonly Color selectedDisabledStandardColor = new Color(0.1701f, 0.3982f, 0.5873f);

		// Token: 0x0400013B RID: 315
		private readonly Color selectedEnabledProColor = new Color(0.188f, 0.4588f, 0.6862f);

		// Token: 0x0400013C RID: 316
		private readonly Color selectedEnabledStandardColor = new Color(0.243f, 0.5686f, 0.839f);

		// Token: 0x0400013D RID: 317
		private readonly Color taskRunningProColor = new Color(0f, 0.698f, 0.4f);

		// Token: 0x0400013E RID: 318
		private readonly Color taskRunningStandardColor = new Color(0f, 1f, 0.2784f);

		// Token: 0x0400013F RID: 319
		private bool horizontalDirty = true;

		// Token: 0x04000140 RID: 320
		private Vector2 startHorizontalBreak;

		// Token: 0x04000141 RID: 321
		private Vector2 endHorizontalBreak;

		// Token: 0x04000142 RID: 322
		private Vector3[] linePoints = new Vector3[4];
	}
}
