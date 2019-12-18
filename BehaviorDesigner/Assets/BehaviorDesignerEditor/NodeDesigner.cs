using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200001F RID: 31
	[Serializable]
	public class NodeDesigner : ScriptableObject
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00013768 File Offset: 0x00011968
		// (set) Token: 0x0600020C RID: 524 RVA: 0x00013770 File Offset: 0x00011970
		public Task Task
		{
			get
			{
				return this.mTask;
			}
			set
			{
				this.mTask = value;
				this.Init();
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00013780 File Offset: 0x00011980
		public void Select()
		{
			if (!this.isEntryDisplay)
			{
				this.mSelected = true;
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00013794 File Offset: 0x00011994
		public void Deselect()
		{
			this.mSelected = false;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x000137A0 File Offset: 0x000119A0
		public void MarkDirty()
		{
			this.mConnectionIsDirty = true;
			this.mRectIsDirty = true;
			this.mIncomingRectIsDirty = true;
			this.mOutgoingRectIsDirty = true;
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000210 RID: 528 RVA: 0x000137C0 File Offset: 0x000119C0
		public bool IsParent
		{
			get
			{
				return this.isParent;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000211 RID: 529 RVA: 0x000137C8 File Offset: 0x000119C8
		public bool IsEntryDisplay
		{
			get
			{
				return this.isEntryDisplay;
			}
		}

		// Token: 0x1700006E RID: 110
		// (set) Token: 0x06000212 RID: 530 RVA: 0x000137D0 File Offset: 0x000119D0
		public bool ShowReferenceIcon
		{
			set
			{
				this.showReferenceIcon = value;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000213 RID: 531 RVA: 0x000137DC File Offset: 0x000119DC
		// (set) Token: 0x06000214 RID: 532 RVA: 0x000137E4 File Offset: 0x000119E4
		public bool ShowHoverBar
		{
			get
			{
				return this.showHoverBar;
			}
			set
			{
				this.showHoverBar = value;
			}
		}

		// Token: 0x17000070 RID: 112
		// (set) Token: 0x06000215 RID: 533 RVA: 0x000137F0 File Offset: 0x000119F0
		public bool HasError
		{
			set
			{
				this.hasError = value;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000216 RID: 534 RVA: 0x000137FC File Offset: 0x000119FC
		// (set) Token: 0x06000217 RID: 535 RVA: 0x00013804 File Offset: 0x00011A04
		public NodeDesigner ParentNodeDesigner
		{
			get
			{
				return this.parentNodeDesigner;
			}
			set
			{
				this.parentNodeDesigner = value;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000218 RID: 536 RVA: 0x00013810 File Offset: 0x00011A10
		public List<NodeConnection> OutgoingNodeConnections
		{
			get
			{
				return this.outgoingNodeConnections;
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00013818 File Offset: 0x00011A18
		public Rect IncomingConnectionRect(Vector2 offset)
		{
			if (!this.mIncomingRectIsDirty)
			{
				return this.mIncomingRectangle;
			}
			Rect rect = this.Rectangle(offset, false, false);
			this.mIncomingRectangle = new Rect(rect.x + (rect.width - 42f) / 2f, rect.y - 14f, 42f, 14f);
			this.mIncomingRectIsDirty = false;
			return this.mIncomingRectangle;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0001388C File Offset: 0x00011A8C
		public Rect OutgoingConnectionRect(Vector2 offset)
		{
			if (!this.mOutgoingRectIsDirty)
			{
				return this.mOutgoingRectangle;
			}
			Rect rect = this.Rectangle(offset, false, false);
			this.mOutgoingRectangle = new Rect(rect.x + (rect.width - 42f) / 2f, rect.yMax, 42f, 16f);
			this.mOutgoingRectIsDirty = false;
			return this.mOutgoingRectangle;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x000138FC File Offset: 0x00011AFC
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00013908 File Offset: 0x00011B08
		public void LoadTask(Task task, Behavior owner, ref int id)
		{
			if (task == null)
			{
				return;
			}
			this.mTask = task;
			this.mTask.Owner = owner;
			this.mTask.ID = id++;
			this.mTask.NodeData.NodeDesigner = this;
			this.mTask.NodeData.InitWatchedFields(this.mTask);
			if (!this.mTask.NodeData.FriendlyName.Equals(string.Empty))
			{
				this.mTask.FriendlyName = this.mTask.NodeData.FriendlyName;
				this.mTask.NodeData.FriendlyName = string.Empty;
			}
			this.LoadTaskIcon();
			this.Init();
			RequiredComponentAttribute[] array;
			if (this.mTask.Owner != null && (array = (this.mTask.GetType().GetCustomAttributes(typeof(RequiredComponentAttribute), true) as RequiredComponentAttribute[])).Length > 0)
			{
				Type componentType = array[0].ComponentType;
				if (typeof(Component).IsAssignableFrom(componentType) && this.mTask.Owner.gameObject.GetComponent(componentType) == null)
				{
					this.mTask.Owner.gameObject.AddComponent(componentType);
				}
			}
			List<Type> baseClasses = FieldInspector.GetBaseClasses(this.mTask.GetType());
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			for (int i = baseClasses.Count - 1; i > -1; i--)
			{
				FieldInfo[] fields = baseClasses[i].GetFields(bindingAttr);
				for (int j = 0; j < fields.Length; j++)
				{
					if (typeof(SharedVariable).IsAssignableFrom(fields[j].FieldType) && !fields[j].FieldType.IsAbstract)
					{
						SharedVariable sharedVariable = fields[j].GetValue(this.mTask) as SharedVariable;
						if (sharedVariable == null)
						{
							sharedVariable = (Activator.CreateInstance(fields[j].FieldType) as SharedVariable);
						}
						if (TaskUtility.HasAttribute(fields[j], typeof(RequiredFieldAttribute)) || TaskUtility.HasAttribute(fields[j], typeof(SharedRequiredAttribute)))
						{
							sharedVariable.IsShared = true;
						}
						fields[j].SetValue(this.mTask, sharedVariable);
					}
				}
			}
			if (this.isParent)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				if (parentTask.Children != null)
				{
					for (int k = 0; k < parentTask.Children.Count; k++)
					{
						NodeDesigner nodeDesigner = ScriptableObject.CreateInstance<NodeDesigner>();
						nodeDesigner.LoadTask(parentTask.Children[k], owner, ref id);
						NodeConnection nodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
						nodeConnection.LoadConnection(this, NodeConnectionType.Fixed);
						this.AddChildNode(nodeDesigner, nodeConnection, true, true, k);
					}
				}
				this.mConnectionIsDirty = true;
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00013BF4 File Offset: 0x00011DF4
		public void LoadNode(Task task, BehaviorSource behaviorSource, Vector2 offset, ref int id)
		{
			this.mTask = task;
			this.mTask.Owner = (behaviorSource.Owner as Behavior);
			this.mTask.ID = id++;
			this.mTask.NodeData = new NodeData();
			this.mTask.NodeData.Offset = offset;
			this.mTask.NodeData.NodeDesigner = this;
			this.LoadTaskIcon();
			this.Init();
			this.mTask.FriendlyName = this.taskName;
			RequiredComponentAttribute[] array;
			if (this.mTask.Owner != null && (array = (this.mTask.GetType().GetCustomAttributes(typeof(RequiredComponentAttribute), true) as RequiredComponentAttribute[])).Length > 0)
			{
				Type componentType = array[0].ComponentType;
				if (typeof(Component).IsAssignableFrom(componentType) && this.mTask.Owner.gameObject.GetComponent(componentType) == null)
				{
					this.mTask.Owner.gameObject.AddComponent(componentType);
				}
			}
			List<Type> baseClasses = FieldInspector.GetBaseClasses(this.mTask.GetType());
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			for (int i = baseClasses.Count - 1; i > -1; i--)
			{
				FieldInfo[] fields = baseClasses[i].GetFields(bindingAttr);
				for (int j = 0; j < fields.Length; j++)
				{
					if (typeof(SharedVariable).IsAssignableFrom(fields[j].FieldType) && !fields[j].FieldType.IsAbstract)
					{
						SharedVariable sharedVariable = fields[j].GetValue(this.mTask) as SharedVariable;
						if (sharedVariable == null)
						{
							sharedVariable = (Activator.CreateInstance(fields[j].FieldType) as SharedVariable);
						}
						if (TaskUtility.HasAttribute(fields[j], typeof(RequiredFieldAttribute)) || TaskUtility.HasAttribute(fields[j], typeof(SharedRequiredAttribute)))
						{
							sharedVariable.IsShared = true;
						}
						fields[j].SetValue(this.mTask, sharedVariable);
					}
				}
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00013E28 File Offset: 0x00012028
		private void LoadTaskIcon()
		{
			this.mTask.NodeData.Icon = null;
			TaskIconAttribute[] array;
			if ((array = (this.mTask.GetType().GetCustomAttributes(typeof(TaskIconAttribute), false) as TaskIconAttribute[])).Length > 0)
			{
				this.mTask.NodeData.Icon = BehaviorDesignerUtility.LoadIcon(array[0].IconPath, null);
			}
			if (this.mTask.NodeData.Icon == null)
			{
				string iconName = string.Empty;
				if (this.mTask.GetType().IsSubclassOf(typeof(Action)))
				{
					iconName = "{SkinColor}ActionIcon.png";
				}
				else if (this.mTask.GetType().IsSubclassOf(typeof(Conditional)))
				{
					iconName = "{SkinColor}ConditionalIcon.png";
				}
				else if (this.mTask.GetType().IsSubclassOf(typeof(Composite)))
				{
					iconName = "{SkinColor}CompositeIcon.png";
				}
				else if (this.mTask.GetType().IsSubclassOf(typeof(Decorator)))
				{
					iconName = "{SkinColor}DecoratorIcon.png";
				}
				else
				{
					iconName = "{SkinColor}EntryIcon.png";
				}
				this.mTask.NodeData.Icon = BehaviorDesignerUtility.LoadIcon(iconName, null);
			}
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00013F78 File Offset: 0x00012178
		private void Init()
		{
			this.taskName = BehaviorDesignerUtility.SplitCamelCase(this.mTask.GetType().Name.ToString());
			this.isParent = this.mTask.GetType().IsSubclassOf(typeof(ParentTask));
			if (this.isParent)
			{
				this.outgoingNodeConnections = new List<NodeConnection>();
			}
			this.mRectIsDirty = (this.mCacheIsDirty = true);
			this.mIncomingRectIsDirty = true;
			this.mOutgoingRectIsDirty = true;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00013FFC File Offset: 0x000121FC
		public void MakeEntryDisplay()
		{
			this.isEntryDisplay = (this.isParent = true);
			this.mTask.FriendlyName = (this.taskName = "Entry");
			this.outgoingNodeConnections = new List<NodeConnection>();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00014040 File Offset: 0x00012240
		public Vector2 GetAbsolutePosition()
		{
			Vector2 vector = this.mTask.NodeData.Offset;
			if (this.parentNodeDesigner != null)
			{
				vector += this.parentNodeDesigner.GetAbsolutePosition();
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.SnapToGrid))
			{
				vector.Set(BehaviorDesignerUtility.RoundToNearest(vector.x, 10f), BehaviorDesignerUtility.RoundToNearest(vector.y, 10f));
			}
			return vector;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000140B8 File Offset: 0x000122B8
		public Rect Rectangle(Vector2 offset, bool includeConnections, bool includeComments)
		{
			Rect result = this.Rectangle(offset);
			if (includeConnections)
			{
				if (!this.isEntryDisplay)
				{
					result.yMin -= 14f;
				}
				if (this.isParent)
				{
					result.yMax += 16f;
				}
			}
			if (includeComments && this.mTask != null)
			{
				if (this.mTask.NodeData.WatchedFields != null && this.mTask.NodeData.WatchedFields.Count > 0 && result.xMax < this.watchedFieldRect.xMax)
				{
					result.xMax = this.watchedFieldRect.xMax;
				}
				if (!this.mTask.NodeData.Comment.Equals(string.Empty))
				{
					if (result.xMax < this.commentRect.xMax)
					{
						result.xMax = this.commentRect.xMax;
					}
					if (result.yMax < this.commentRect.yMax)
					{
						result.yMax = this.commentRect.yMax;
					}
				}
			}
			return result;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x000141EC File Offset: 0x000123EC
		private Rect Rectangle(Vector2 offset)
		{
			if (!this.mRectIsDirty)
			{
				return this.mRectangle;
			}
			this.mCacheIsDirty = true;
			if (this.mTask == null)
			{
				return default(Rect);
			}
			float num = BehaviorDesignerUtility.TaskTitleGUIStyle.CalcSize(new GUIContent(this.ToString())).x + 20f;
			if (!this.isParent)
			{
				float num2;
				float num3;
				BehaviorDesignerUtility.TaskCommentGUIStyle.CalcMinMaxWidth(new GUIContent(this.mTask.NodeData.Comment), out num2, out num3);
				num3 += 20f;
				num = ((num <= num3) ? num3 : num);
			}
			num = Mathf.Min(220f, Mathf.Max(100f, num));
			Vector2 absolutePosition = this.GetAbsolutePosition();
			float num4 = (float)(20 + ((!BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode)) ? 52 : 22));
			this.mRectangle = new Rect(absolutePosition.x + offset.x - num / 2f, absolutePosition.y + offset.y, num, num4);
			this.mRectIsDirty = false;
			return this.mRectangle;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0001430C File Offset: 0x0001250C
		private void UpdateCache(Rect nodeRect)
		{
			if (this.mCacheIsDirty)
			{
				this.nodeCollapsedTextureRect = new Rect(nodeRect.x + (nodeRect.width - 26f) / 2f + 1f, nodeRect.yMax + 2f, 26f, 6f);
				this.iconTextureRect = new Rect(nodeRect.x + (nodeRect.width - 44f) / 2f, nodeRect.y + 4f + 2f, 44f, 44f);
				this.titleRect = new Rect(nodeRect.x, nodeRect.yMax - (float)((!BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode)) ? 20 : 28) - 1f, nodeRect.width, 20f);
				this.breakpointTextureRect = new Rect(nodeRect.xMax - 16f, nodeRect.y + 3f, 14f, 14f);
				this.errorTextureRect = new Rect(nodeRect.xMax - 12f, nodeRect.y - 8f, 20f, 20f);
				this.referenceTextureRect = new Rect(nodeRect.x + 2f, nodeRect.y + 3f, 14f, 14f);
				this.conditionalAbortTextureRect = new Rect(nodeRect.x + 3f, nodeRect.y + 3f, 16f, 16f);
				this.conditionalAbortLowerPriorityTextureRect = new Rect(nodeRect.x + 3f, nodeRect.y, 16f, 16f);
				this.disabledButtonTextureRect = new Rect(nodeRect.x - 1f, nodeRect.y - 17f, 14f, 14f);
				this.collapseButtonTextureRect = new Rect(nodeRect.x + 15f, nodeRect.y - 17f, 14f, 14f);
				this.incomingConnectionTextureRect = new Rect(nodeRect.x + (nodeRect.width - 42f) / 2f, nodeRect.y - 14f - 3f + 3f, 42f, 17f);
				this.outgoingConnectionTextureRect = new Rect(nodeRect.x + (nodeRect.width - 42f) / 2f, nodeRect.yMax - 3f, 42f, 19f);
				this.successReevaluatingExecutionStatusTextureRect = new Rect(nodeRect.xMax - 37f, nodeRect.yMax - 38f, 35f, 36f);
				this.successExecutionStatusTextureRect = new Rect(nodeRect.xMax - 37f, nodeRect.yMax - 33f, 35f, 31f);
				this.failureExecutionStatusTextureRect = new Rect(nodeRect.xMax - 37f, nodeRect.yMax - 38f, 35f, 36f);
				this.iconBorderTextureRect = new Rect(nodeRect.x + (nodeRect.width - 46f) / 2f, nodeRect.y + 3f + 2f, 46f, 46f);
				this.CalculateNodeCommentRect(nodeRect);
				this.mCacheIsDirty = false;
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00014694 File Offset: 0x00012894
		private void CalculateNodeCommentRect(Rect nodeRect)
		{
			bool flag = false;
			if (this.mTask.NodeData.WatchedFields != null && this.mTask.NodeData.WatchedFields.Count > 0)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				for (int i = 0; i < this.mTask.NodeData.WatchedFields.Count; i++)
				{
					FieldInfo fieldInfo = this.mTask.NodeData.WatchedFields[i];
					text = text + BehaviorDesignerUtility.SplitCamelCase(fieldInfo.Name) + ": \n";
					text2 = text2 + ((fieldInfo.GetValue(this.mTask) == null) ? "null" : fieldInfo.GetValue(this.mTask).ToString()) + "\n";
				}
				float num;
				float num2;
				BehaviorDesignerUtility.TaskCommentGUIStyle.CalcMinMaxWidth(new GUIContent(text), out num, out num2);
				float num3;
				BehaviorDesignerUtility.TaskCommentGUIStyle.CalcMinMaxWidth(new GUIContent(text2), out num, out num3);
				float num4 = num2;
				float num5 = num3;
				float num6 = Mathf.Min(220f, num2 + num3 + 20f);
				if (num6 == 220f)
				{
					num4 = num2 / (num2 + num3) * 220f;
					num5 = num3 / (num2 + num3) * 220f;
				}
				this.watchedFieldRect = new Rect(nodeRect.xMax + 4f, nodeRect.y, num6 + 8f, nodeRect.height);
				this.watchedFieldNamesRect = new Rect(nodeRect.xMax + 6f, nodeRect.y + 4f, num4, nodeRect.height - 8f);
				this.watchedFieldValuesRect = new Rect(nodeRect.xMax + 6f + num4, nodeRect.y + 4f, num5, nodeRect.height - 8f);
				flag = true;
			}
			if (!this.mTask.NodeData.Comment.Equals(string.Empty))
			{
				if (this.isParent)
				{
					float num7;
					float num8;
					BehaviorDesignerUtility.TaskCommentGUIStyle.CalcMinMaxWidth(new GUIContent(this.mTask.NodeData.Comment), out num7, out num8);
					float num9 = Mathf.Min(220f, num8 + 20f);
					if (flag)
					{
						this.commentRect = new Rect(nodeRect.xMin - 12f - num9, nodeRect.y, num9 + 8f, nodeRect.height);
						this.commentLabelRect = new Rect(nodeRect.xMin - 6f - num9, nodeRect.y + 4f, num9, nodeRect.height - 8f);
					}
					else
					{
						this.commentRect = new Rect(nodeRect.xMax + 4f, nodeRect.y, num9 + 8f, nodeRect.height);
						this.commentLabelRect = new Rect(nodeRect.xMax + 6f, nodeRect.y + 4f, num9, nodeRect.height - 8f);
					}
				}
				else
				{
					float num10 = Mathf.Min(100f, BehaviorDesignerUtility.TaskCommentGUIStyle.CalcHeight(new GUIContent(this.mTask.NodeData.Comment), nodeRect.width - 4f));
					this.commentRect = new Rect(nodeRect.x, nodeRect.yMax + 4f, nodeRect.width, num10 + 4f);
					this.commentLabelRect = new Rect(nodeRect.x, nodeRect.yMax + 4f, nodeRect.width - 4f, num10);
				}
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00014A4C File Offset: 0x00012C4C
		public bool DrawNode(Vector2 offset, bool drawSelected, bool disabled)
		{
			if (drawSelected != this.mSelected)
			{
				return false;
			}
			if (this.ToString().Length != this.prevFriendlyNameLength)
			{
				this.prevFriendlyNameLength = this.ToString().Length;
				this.mRectIsDirty = true;
			}
			Rect rect = this.Rectangle(offset, false, false);
			this.UpdateCache(rect);
			bool flag = (this.mTask.NodeData.PushTime != -1f && this.mTask.NodeData.PushTime >= this.mTask.NodeData.PopTime) || (this.isEntryDisplay && this.outgoingNodeConnections.Count > 0 && this.outgoingNodeConnections[0].DestinationNodeDesigner.Task.NodeData.PushTime != -1f);
			bool flag2 = this.mIdentifyUpdateCount != -1;
			bool result = this.prevRunningState != flag;
			float num = (!BehaviorDesignerPreferences.GetBool(BDPreferences.FadeNodes)) ? 0.01f : 0.5f;
			float num2 = 0f;
			if (flag2)
			{
				if (2000 - this.mIdentifyUpdateCount < 500)
				{
					num2 = (float)(2000 - this.mIdentifyUpdateCount) / 500f;
				}
				else
				{
					num2 = 1f;
				}
				if (this.mIdentifyUpdateCount != -1)
				{
					this.mIdentifyUpdateCount++;
					if (this.mIdentifyUpdateCount > 2000)
					{
						this.mIdentifyUpdateCount = -1;
					}
				}
				result = true;
			}
			else if (flag)
			{
				num2 = 1f;
			}
			else if ((this.mTask.NodeData.PopTime != -1f && num != 0f && this.mTask.NodeData.PopTime <= Time.realtimeSinceStartup && Time.realtimeSinceStartup - this.mTask.NodeData.PopTime < num) || (this.isEntryDisplay && this.outgoingNodeConnections.Count > 0 && this.outgoingNodeConnections[0].DestinationNodeDesigner.Task.NodeData.PopTime != -1f && this.outgoingNodeConnections[0].DestinationNodeDesigner.Task.NodeData.PopTime <= Time.realtimeSinceStartup && Time.realtimeSinceStartup - this.outgoingNodeConnections[0].DestinationNodeDesigner.Task.NodeData.PopTime < num))
			{
				if (this.isEntryDisplay)
				{
					num2 = 1f - (Time.realtimeSinceStartup - this.outgoingNodeConnections[0].DestinationNodeDesigner.Task.NodeData.PopTime) / num;
				}
				else
				{
					num2 = 1f - (Time.realtimeSinceStartup - this.mTask.NodeData.PopTime) / num;
				}
				result = true;
			}
			if (!this.isEntryDisplay && !this.prevRunningState && this.parentNodeDesigner != null)
			{
				this.parentNodeDesigner.BringConnectionToFront(this);
			}
			this.prevRunningState = flag;
			if (num2 != 1f)
			{
				GUI.color = ((!disabled && !this.mTask.Disabled) ? Color.white : this.grayColor);
				GUIStyle backgroundGUIStyle;
				if (BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode))
				{
					backgroundGUIStyle = ((!this.mSelected) ? BehaviorDesignerUtility.GetTaskCompactGUIStyle(this.mTask.NodeData.ColorIndex) : BehaviorDesignerUtility.GetTaskSelectedCompactGUIStyle(this.mTask.NodeData.ColorIndex));
				}
				else
				{
					backgroundGUIStyle = ((!this.mSelected) ? BehaviorDesignerUtility.GetTaskGUIStyle(this.mTask.NodeData.ColorIndex) : BehaviorDesignerUtility.GetTaskSelectedGUIStyle(this.mTask.NodeData.ColorIndex));
				}
				this.DrawNodeTexture(rect, BehaviorDesignerUtility.GetTaskConnectionTopTexture(this.mTask.NodeData.ColorIndex), BehaviorDesignerUtility.GetTaskConnectionBottomTexture(this.mTask.NodeData.ColorIndex), backgroundGUIStyle, BehaviorDesignerUtility.GetTaskBorderTexture(this.mTask.NodeData.ColorIndex));
			}
			if (num2 > 0f)
			{
				GUIStyle backgroundGUIStyle2;
				Texture2D iconBorderTexture;
				if (flag2)
				{
					if (BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode))
					{
						if (this.mSelected)
						{
							backgroundGUIStyle2 = BehaviorDesignerUtility.TaskIdentifySelectedCompactGUIStyle;
						}
						else
						{
							backgroundGUIStyle2 = BehaviorDesignerUtility.TaskIdentifyCompactGUIStyle;
						}
					}
					else if (this.mSelected)
					{
						backgroundGUIStyle2 = BehaviorDesignerUtility.TaskIdentifySelectedGUIStyle;
					}
					else
					{
						backgroundGUIStyle2 = BehaviorDesignerUtility.TaskIdentifyGUIStyle;
					}
					iconBorderTexture = BehaviorDesignerUtility.TaskBorderIdentifyTexture;
				}
				else
				{
					if (BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode))
					{
						if (this.mSelected)
						{
							backgroundGUIStyle2 = BehaviorDesignerUtility.TaskRunningSelectedCompactGUIStyle;
						}
						else
						{
							backgroundGUIStyle2 = BehaviorDesignerUtility.TaskRunningCompactGUIStyle;
						}
					}
					else if (this.mSelected)
					{
						backgroundGUIStyle2 = BehaviorDesignerUtility.TaskRunningSelectedGUIStyle;
					}
					else
					{
						backgroundGUIStyle2 = BehaviorDesignerUtility.TaskRunningGUIStyle;
					}
					iconBorderTexture = BehaviorDesignerUtility.TaskBorderRunningTexture;
				}
				Color color = (!disabled && !this.mTask.Disabled) ? Color.white : this.grayColor;
				color.a = num2;
				GUI.color = color;
				Texture2D connectionTopTexture = null;
				Texture2D connectionBottomTexture = null;
				if (!this.isEntryDisplay)
				{
					if (flag2)
					{
						connectionTopTexture = BehaviorDesignerUtility.TaskConnectionIdentifyTopTexture;
					}
					else
					{
						connectionTopTexture = BehaviorDesignerUtility.TaskConnectionRunningTopTexture;
					}
				}
				if (this.isParent)
				{
					if (flag2)
					{
						connectionBottomTexture = BehaviorDesignerUtility.TaskConnectionIdentifyBottomTexture;
					}
					else
					{
						connectionBottomTexture = BehaviorDesignerUtility.TaskConnectionRunningBottomTexture;
					}
				}
				this.DrawNodeTexture(rect, connectionTopTexture, connectionBottomTexture, backgroundGUIStyle2, iconBorderTexture);
				GUI.color = Color.white;
			}
			if (this.mTask.NodeData.Collapsed)
			{
				GUI.DrawTexture(this.nodeCollapsedTextureRect, BehaviorDesignerUtility.TaskConnectionCollapsedTexture);
			}
			if (!BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode))
			{
				GUI.DrawTexture(this.iconTextureRect, this.mTask.NodeData.Icon);
			}
			if (this.mTask.NodeData.InterruptTime != -1f && Time.realtimeSinceStartup - this.mTask.NodeData.InterruptTime < 0.75f + num)
			{
				float a;
				if (Time.realtimeSinceStartup - this.mTask.NodeData.InterruptTime < 0.75f)
				{
					a = 1f;
				}
				else
				{
					a = 1f - (Time.realtimeSinceStartup - (this.mTask.NodeData.InterruptTime + 0.75f)) / num;
				}
				Color white = Color.white;
				white.a = a;
				GUI.color = white;
				GUI.Label(rect, string.Empty, BehaviorDesignerUtility.TaskHighlightGUIStyle);
				GUI.color = Color.white;
			}
			GUI.Label(this.titleRect, this.ToString(), BehaviorDesignerUtility.TaskTitleGUIStyle);
			if (this.mTask.NodeData.IsBreakpoint)
			{
				GUI.DrawTexture(this.breakpointTextureRect, BehaviorDesignerUtility.BreakpointTexture);
			}
			if (this.showReferenceIcon)
			{
				GUI.DrawTexture(this.referenceTextureRect, BehaviorDesignerUtility.ReferencedTexture);
			}
			if (this.hasError)
			{
				GUI.DrawTexture(this.errorTextureRect, BehaviorDesignerUtility.ErrorIconTexture);
			}
			if (this.mTask is Composite && (this.mTask as Composite).AbortType != AbortType.None)
			{
				switch ((this.mTask as Composite).AbortType)
				{
				case AbortType.Self:
					GUI.DrawTexture(this.conditionalAbortTextureRect, BehaviorDesignerUtility.ConditionalAbortSelfTexture);
					break;
				case AbortType.LowerPriority:
					GUI.DrawTexture(this.conditionalAbortLowerPriorityTextureRect, BehaviorDesignerUtility.ConditionalAbortLowerPriorityTexture);
					break;
				case AbortType.Both:
					GUI.DrawTexture(this.conditionalAbortTextureRect, BehaviorDesignerUtility.ConditionalAbortBothTexture);
					break;
				}
			}
			GUI.color = Color.white;
			if (this.showHoverBar)
			{
				GUI.DrawTexture(this.disabledButtonTextureRect, (!this.mTask.Disabled) ? BehaviorDesignerUtility.DisableTaskTexture : BehaviorDesignerUtility.EnableTaskTexture, (ScaleMode)2);
				if (this.isParent || this.mTask is BehaviorReference)
				{
					bool collapsed = this.mTask.NodeData.Collapsed;
					if (this.mTask is BehaviorReference)
					{
						collapsed = (this.mTask as BehaviorReference).collapsed;
					}
					GUI.DrawTexture(this.collapseButtonTextureRect, (!collapsed) ? BehaviorDesignerUtility.CollapseTaskTexture : BehaviorDesignerUtility.ExpandTaskTexture, (ScaleMode)2);
				}
			}
			return result;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000152C0 File Offset: 0x000134C0
		private void DrawNodeTexture(Rect nodeRect, Texture2D connectionTopTexture, Texture2D connectionBottomTexture, GUIStyle backgroundGUIStyle, Texture2D iconBorderTexture)
		{
			if (!this.isEntryDisplay)
			{
				GUI.DrawTexture(this.incomingConnectionTextureRect, connectionTopTexture, (ScaleMode)2);
			}
			if (this.isParent)
			{
				GUI.DrawTexture(this.outgoingConnectionTextureRect, connectionBottomTexture, (ScaleMode)2);
			}
			GUI.Label(nodeRect, string.Empty, backgroundGUIStyle);
			if (this.mTask.NodeData.ExecutionStatus == TaskStatus.Success)
			{
				if (this.mTask.NodeData.IsReevaluating)
				{
					GUI.DrawTexture(this.successReevaluatingExecutionStatusTextureRect, BehaviorDesignerUtility.ExecutionSuccessRepeatTexture);
				}
				else
				{
					GUI.DrawTexture(this.successExecutionStatusTextureRect, BehaviorDesignerUtility.ExecutionSuccessTexture);
				}
			}
			else if (this.mTask.NodeData.ExecutionStatus == TaskStatus.Failure)
			{
				GUI.DrawTexture(this.failureExecutionStatusTextureRect, (!this.mTask.NodeData.IsReevaluating) ? BehaviorDesignerUtility.ExecutionFailureTexture : BehaviorDesignerUtility.ExecutionFailureRepeatTexture);
			}
			if (!BehaviorDesignerPreferences.GetBool(BDPreferences.CompactMode))
			{
				GUI.DrawTexture(this.iconBorderTextureRect, iconBorderTexture);
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x000153C0 File Offset: 0x000135C0
		public void DrawNodeConnection(Vector2 offset, bool disabled)
		{
			if (this.mConnectionIsDirty)
			{
				this.DetermineConnectionHorizontalHeight(this.Rectangle(offset, false, false), offset);
				this.mConnectionIsDirty = false;
			}
			if (this.isParent)
			{
				for (int i = 0; i < this.outgoingNodeConnections.Count; i++)
				{
					this.outgoingNodeConnections[i].DrawConnection(offset, disabled);
				}
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0001542C File Offset: 0x0001362C
		public void DrawNodeComment(Vector2 offset)
		{
			if (this.mTask.NodeData.Comment.Length != this.prevCommentLength)
			{
				this.prevCommentLength = this.mTask.NodeData.Comment.Length;
				this.mRectIsDirty = true;
			}
			if (this.mTask.NodeData.WatchedFields != null && this.mTask.NodeData.WatchedFields.Count > 0)
			{
				if (this.mTask.NodeData.WatchedFields.Count != this.prevWatchedFieldsLength.Count)
				{
					this.mRectIsDirty = true;
					this.prevWatchedFieldsLength.Clear();
					for (int i = 0; i < this.mTask.NodeData.WatchedFields.Count; i++)
					{
						this.prevWatchedFieldsLength.Add(this.mTask.NodeData.WatchedFields[i].GetValue(this.mTask).ToString().Length);
					}
				}
				else
				{
					for (int j = 0; j < this.mTask.NodeData.WatchedFields.Count; j++)
					{
						int length = this.mTask.NodeData.WatchedFields[j].GetValue(this.mTask).ToString().Length;
						if (length != this.prevWatchedFieldsLength[j])
						{
							this.mRectIsDirty = true;
							break;
						}
					}
				}
			}
			if (this.mTask.NodeData.Comment.Equals(string.Empty) && (this.mTask.NodeData.WatchedFields == null || this.mTask.NodeData.WatchedFields.Count == 0))
			{
				return;
			}
			if (this.mTask.NodeData.WatchedFields != null && this.mTask.NodeData.WatchedFields.Count > 0)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				for (int k = 0; k < this.mTask.NodeData.WatchedFields.Count; k++)
				{
					FieldInfo fieldInfo = this.mTask.NodeData.WatchedFields[k];
					text = text + BehaviorDesignerUtility.SplitCamelCase(fieldInfo.Name) + ": \n";
					text2 = text2 + ((fieldInfo.GetValue(this.mTask) == null) ? "null" : fieldInfo.GetValue(this.mTask).ToString()) + "\n";
				}
				GUI.Box(this.watchedFieldRect, string.Empty, BehaviorDesignerUtility.TaskDescriptionGUIStyle);
				GUI.Label(this.watchedFieldNamesRect, text, BehaviorDesignerUtility.TaskCommentRightAlignGUIStyle);
				GUI.Label(this.watchedFieldValuesRect, text2, BehaviorDesignerUtility.TaskCommentLeftAlignGUIStyle);
			}
			if (!this.mTask.NodeData.Comment.Equals(string.Empty))
			{
				GUI.Box(this.commentRect, string.Empty, BehaviorDesignerUtility.TaskDescriptionGUIStyle);
				GUI.Label(this.commentLabelRect, this.mTask.NodeData.Comment, BehaviorDesignerUtility.TaskCommentGUIStyle);
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00015764 File Offset: 0x00013964
		public bool Contains(Vector2 point, Vector2 offset, bool includeConnections)
		{
			return this.Rectangle(offset, includeConnections, false).Contains(point);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00015784 File Offset: 0x00013984
		public NodeConnection NodeConnectionRectContains(Vector2 point, Vector2 offset)
		{
			bool incomingNodeConnection;
			if ((incomingNodeConnection = this.IncomingConnectionRect(offset).Contains(point)) || (this.isParent && this.OutgoingConnectionRect(offset).Contains(point)))
			{
				return this.CreateNodeConnection(incomingNodeConnection);
			}
			return null;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000157D4 File Offset: 0x000139D4
		public NodeConnection CreateNodeConnection(bool incomingNodeConnection)
		{
			NodeConnection nodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
			nodeConnection.LoadConnection(this, (!incomingNodeConnection) ? NodeConnectionType.Outgoing : NodeConnectionType.Incoming);
			return nodeConnection;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x000157FC File Offset: 0x000139FC
		public void ConnectionContains(Vector2 point, Vector2 offset, ref List<NodeConnection> nodeConnections)
		{
			if (this.outgoingNodeConnections == null || this.isEntryDisplay)
			{
				return;
			}
			for (int i = 0; i < this.outgoingNodeConnections.Count; i++)
			{
				if (this.outgoingNodeConnections[i].Contains(point, offset))
				{
					nodeConnections.Add(this.outgoingNodeConnections[i]);
				}
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00015868 File Offset: 0x00013A68
		private void DetermineConnectionHorizontalHeight(Rect nodeRect, Vector2 offset)
		{
			if (this.isParent)
			{
				float num = float.MaxValue;
				float num2 = num;
				for (int i = 0; i < this.outgoingNodeConnections.Count; i++)
				{
					Rect rect = this.outgoingNodeConnections[i].DestinationNodeDesigner.Rectangle(offset, false, false);
					if (rect.y < num)
					{
						num = rect.y;
						num2 = rect.y;
					}
				}
				num = num * 0.75f + nodeRect.yMax * 0.25f;
				if (num < nodeRect.yMax + 15f)
				{
					num = nodeRect.yMax + 15f;
				}
				else if (num > num2 - 15f)
				{
					num = num2 - 15f;
				}
				for (int j = 0; j < this.outgoingNodeConnections.Count; j++)
				{
					this.outgoingNodeConnections[j].HorizontalHeight = num;
				}
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00015960 File Offset: 0x00013B60
		public Vector2 GetConnectionPosition(Vector2 offset, NodeConnectionType connectionType)
		{
			Vector2 result;
			if (connectionType == NodeConnectionType.Incoming)
			{
				Rect rect = this.IncomingConnectionRect(offset);
				result = new Vector2(rect.center.x, rect.y + 7f);
			}
			else
			{
				Rect rect2 = this.OutgoingConnectionRect(offset);
				result = new Vector2(rect2.center.x, rect2.yMax - 8f);
			}
			return result;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x000159D0 File Offset: 0x00013BD0
		public bool HoverBarAreaContains(Vector2 point, Vector2 offset)
		{
			Rect rect = this.Rectangle(offset, false, false);
			rect.y -= 24f;
			return rect.Contains(point);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00015A04 File Offset: 0x00013C04
		public bool HoverBarButtonClick(Vector2 point, Vector2 offset, ref bool collapsedButtonClicked)
		{
			Rect rect = this.Rectangle(offset, false, false);
			Rect rect2 = new Rect(rect.x - 1f, rect.y - 17f, 14f, 14f);
			Rect rect3 = rect2;
			bool flag = false;
			if (rect2.Contains(point))
			{
				this.mTask.Disabled = !this.mTask.Disabled;
				flag = true;
			}
			if (!flag && (this.isParent || this.mTask is BehaviorReference))
			{
				Rect rect4 = new Rect(rect.x + 15f, rect.y - 17f, 14f, 14f);
				rect3.xMax = rect4.xMax;
				if (rect4.Contains(point))
				{
					if (this.mTask is BehaviorReference)
					{
						(this.mTask as BehaviorReference).collapsed = !(this.mTask as BehaviorReference).collapsed;
					}
					else
					{
						this.mTask.NodeData.Collapsed = !this.mTask.NodeData.Collapsed;
					}
					collapsedButtonClicked = true;
					flag = true;
				}
			}
			if (!flag && rect3.Contains(point))
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00015B50 File Offset: 0x00013D50
		public bool Intersects(Rect rect, Vector2 offset)
		{
			Rect rect2 = this.Rectangle(offset, false, false);
			return rect2.xMin < rect.xMax && rect2.xMax > rect.xMin && rect2.yMin < rect.yMax && rect2.yMax > rect.yMin;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00015BB4 File Offset: 0x00013DB4
		public void ChangeOffset(Vector2 delta)
		{
			Vector2 vector = this.mTask.NodeData.Offset;
			vector += delta;
			this.mTask.NodeData.Offset = vector;
			this.MarkDirty();
			if (this.parentNodeDesigner != null)
			{
				this.parentNodeDesigner.MarkDirty();
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00015C10 File Offset: 0x00013E10
		public void AddChildNode(NodeDesigner childNodeDesigner, NodeConnection nodeConnection, bool adjustOffset, bool replaceNode)
		{
			this.AddChildNode(childNodeDesigner, nodeConnection, adjustOffset, replaceNode, -1);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00015C20 File Offset: 0x00013E20
		public void AddChildNode(NodeDesigner childNodeDesigner, NodeConnection nodeConnection, bool adjustOffset, bool replaceNode, int replaceNodeIndex)
		{
			if (replaceNode)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				parentTask.Children[replaceNodeIndex] = childNodeDesigner.Task;
			}
			else
			{
				if (!this.isEntryDisplay)
				{
					ParentTask parentTask2 = this.mTask as ParentTask;
					int i = 0;
					if (parentTask2.Children != null)
					{
						for (i = 0; i < parentTask2.Children.Count; i++)
						{
							if (childNodeDesigner.GetAbsolutePosition().x < (parentTask2.Children[i].NodeData.NodeDesigner as NodeDesigner).GetAbsolutePosition().x)
							{
								break;
							}
						}
					}
					parentTask2.AddChild(childNodeDesigner.Task, i);
				}
				if (adjustOffset)
				{
					childNodeDesigner.Task.NodeData.Offset -= this.GetAbsolutePosition();
				}
			}
			childNodeDesigner.ParentNodeDesigner = this;
			nodeConnection.DestinationNodeDesigner = childNodeDesigner;
			nodeConnection.NodeConnectionType = NodeConnectionType.Fixed;
			if (!nodeConnection.OriginatingNodeDesigner.Equals(this))
			{
				nodeConnection.OriginatingNodeDesigner = this;
			}
			this.outgoingNodeConnections.Add(nodeConnection);
			this.mConnectionIsDirty = true;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00015D50 File Offset: 0x00013F50
		public void RemoveChildNode(NodeDesigner childNodeDesigner)
		{
			if (!this.isEntryDisplay)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				parentTask.Children.Remove(childNodeDesigner.Task);
			}
			for (int i = 0; i < this.outgoingNodeConnections.Count; i++)
			{
				NodeConnection nodeConnection = this.outgoingNodeConnections[i];
				if (nodeConnection.DestinationNodeDesigner.Equals(childNodeDesigner) || nodeConnection.OriginatingNodeDesigner.Equals(childNodeDesigner))
				{
					this.outgoingNodeConnections.RemoveAt(i);
					break;
				}
			}
			childNodeDesigner.ParentNodeDesigner = null;
			this.mConnectionIsDirty = true;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00015DF0 File Offset: 0x00013FF0
		public void SetID(ref int id)
		{
			this.mTask.ID = id++;
			if (this.isParent)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner).SetID(ref id);
					}
				}
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00015E70 File Offset: 0x00014070
		public int ChildIndexForTask(Task childTask)
		{
			if (this.isParent)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i].Equals(childTask))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00015ED8 File Offset: 0x000140D8
		public NodeDesigner NodeDesignerForChildIndex(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (this.isParent)
			{
				ParentTask parentTask = this.mTask as ParentTask;
				if (parentTask.Children != null)
				{
					if (index >= parentTask.Children.Count || parentTask.Children[index] == null)
					{
						return null;
					}
					return parentTask.Children[index].NodeData.NodeDesigner as NodeDesigner;
				}
			}
			return null;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00015F54 File Offset: 0x00014154
		public void MoveChildNode(int index, bool decreaseIndex)
		{
			int index2 = index + ((!decreaseIndex) ? 1 : -1);
			ParentTask parentTask = this.mTask as ParentTask;
			Task value = parentTask.Children[index];
			parentTask.Children[index] = parentTask.Children[index2];
			parentTask.Children[index2] = value;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00015FB0 File Offset: 0x000141B0
		private void BringConnectionToFront(NodeDesigner nodeDesigner)
		{
			for (int i = 0; i < this.outgoingNodeConnections.Count; i++)
			{
				if (this.outgoingNodeConnections[i].DestinationNodeDesigner.Equals(nodeDesigner))
				{
					NodeConnection value = this.outgoingNodeConnections[i];
					this.outgoingNodeConnections[i] = this.outgoingNodeConnections[this.outgoingNodeConnections.Count - 1];
					this.outgoingNodeConnections[this.outgoingNodeConnections.Count - 1] = value;
					break;
				}
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00016044 File Offset: 0x00014244
		public void ToggleBreakpoint()
		{
			this.mTask.NodeData.IsBreakpoint = !this.Task.NodeData.IsBreakpoint;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00016074 File Offset: 0x00014274
		public void ToggleEnableState()
		{
			this.mTask.Disabled = !this.Task.Disabled;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00016090 File Offset: 0x00014290
		public bool IsDisabled()
		{
			return this.mTask.Disabled || (this.parentNodeDesigner != null && this.parentNodeDesigner.IsDisabled());
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000160D0 File Offset: 0x000142D0
		public bool ToggleCollapseState()
		{
			this.mTask.NodeData.Collapsed = !this.Task.NodeData.Collapsed;
			return this.mTask.NodeData.Collapsed;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00016110 File Offset: 0x00014310
		public void IdentifyNode()
		{
			this.mIdentifyUpdateCount = 0;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0001611C File Offset: 0x0001431C
		public bool HasParent(NodeDesigner nodeDesigner)
		{
			return !(this.parentNodeDesigner == null) && (this.parentNodeDesigner.Equals(nodeDesigner) || this.parentNodeDesigner.HasParent(nodeDesigner));
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0001615C File Offset: 0x0001435C
		public void DestroyConnections()
		{
			if (this.outgoingNodeConnections != null)
			{
				for (int i = this.outgoingNodeConnections.Count - 1; i > -1; i--)
				{
					UnityEngine.Object.DestroyImmediate(this.outgoingNodeConnections[i], true);
				}
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x000161A4 File Offset: 0x000143A4
		public override bool Equals(object obj)
		{
			return object.ReferenceEquals(this, obj);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000161B0 File Offset: 0x000143B0
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000161B8 File Offset: 0x000143B8
		public override string ToString()
		{
			return (this.mTask != null) ? ((!this.mTask.FriendlyName.Equals(string.Empty)) ? this.mTask.FriendlyName : this.taskName) : string.Empty;
		}

		// Token: 0x04000143 RID: 323
		[SerializeField]
		private Task mTask;

		// Token: 0x04000144 RID: 324
		[SerializeField]
		private bool mSelected;

		// Token: 0x04000145 RID: 325
		private int mIdentifyUpdateCount = -1;

		// Token: 0x04000146 RID: 326
		[SerializeField]
		private bool mConnectionIsDirty;

		// Token: 0x04000147 RID: 327
		private bool mRectIsDirty = true;

		// Token: 0x04000148 RID: 328
		private bool mIncomingRectIsDirty = true;

		// Token: 0x04000149 RID: 329
		private bool mOutgoingRectIsDirty = true;

		// Token: 0x0400014A RID: 330
		[SerializeField]
		private bool isParent;

		// Token: 0x0400014B RID: 331
		[SerializeField]
		private bool isEntryDisplay;

		// Token: 0x0400014C RID: 332
		[SerializeField]
		private bool showReferenceIcon;

		// Token: 0x0400014D RID: 333
		private bool showHoverBar;

		// Token: 0x0400014E RID: 334
		private bool hasError;

		// Token: 0x0400014F RID: 335
		[SerializeField]
		private string taskName = string.Empty;

		// Token: 0x04000150 RID: 336
		private Rect mRectangle;

		// Token: 0x04000151 RID: 337
		private Rect mOutgoingRectangle;

		// Token: 0x04000152 RID: 338
		private Rect mIncomingRectangle;

		// Token: 0x04000153 RID: 339
		private bool prevRunningState;

		// Token: 0x04000154 RID: 340
		private int prevCommentLength = -1;

		// Token: 0x04000155 RID: 341
		private List<int> prevWatchedFieldsLength = new List<int>();

		// Token: 0x04000156 RID: 342
		private int prevFriendlyNameLength = -1;

		// Token: 0x04000157 RID: 343
		[SerializeField]
		private NodeDesigner parentNodeDesigner;

		// Token: 0x04000158 RID: 344
		[SerializeField]
		private List<NodeConnection> outgoingNodeConnections;

		// Token: 0x04000159 RID: 345
		private bool mCacheIsDirty = true;

		// Token: 0x0400015A RID: 346
		private readonly Color grayColor = new Color(0.7f, 0.7f, 0.7f);

		// Token: 0x0400015B RID: 347
		private Rect nodeCollapsedTextureRect;

		// Token: 0x0400015C RID: 348
		private Rect iconTextureRect;

		// Token: 0x0400015D RID: 349
		private Rect titleRect;

		// Token: 0x0400015E RID: 350
		private Rect breakpointTextureRect;

		// Token: 0x0400015F RID: 351
		private Rect errorTextureRect;

		// Token: 0x04000160 RID: 352
		private Rect referenceTextureRect;

		// Token: 0x04000161 RID: 353
		private Rect conditionalAbortTextureRect;

		// Token: 0x04000162 RID: 354
		private Rect conditionalAbortLowerPriorityTextureRect;

		// Token: 0x04000163 RID: 355
		private Rect disabledButtonTextureRect;

		// Token: 0x04000164 RID: 356
		private Rect collapseButtonTextureRect;

		// Token: 0x04000165 RID: 357
		private Rect incomingConnectionTextureRect;

		// Token: 0x04000166 RID: 358
		private Rect outgoingConnectionTextureRect;

		// Token: 0x04000167 RID: 359
		private Rect successReevaluatingExecutionStatusTextureRect;

		// Token: 0x04000168 RID: 360
		private Rect successExecutionStatusTextureRect;

		// Token: 0x04000169 RID: 361
		private Rect failureExecutionStatusTextureRect;

		// Token: 0x0400016A RID: 362
		private Rect iconBorderTextureRect;

		// Token: 0x0400016B RID: 363
		private Rect watchedFieldRect;

		// Token: 0x0400016C RID: 364
		private Rect watchedFieldNamesRect;

		// Token: 0x0400016D RID: 365
		private Rect watchedFieldValuesRect;

		// Token: 0x0400016E RID: 366
		private Rect commentRect;

		// Token: 0x0400016F RID: 367
		private Rect commentLabelRect;
	}
}
