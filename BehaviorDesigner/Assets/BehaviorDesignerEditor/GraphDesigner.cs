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
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class GraphDesigner : ScriptableObject
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000F0A0 File Offset: 0x0000D2A0
		public NodeDesigner RootNode
		{
			get
			{
				return this.mRootNode;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000F0A8 File Offset: 0x0000D2A8
		public List<NodeDesigner> DetachedNodes
		{
			get
			{
				return this.mDetachedNodes;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000F0B0 File Offset: 0x0000D2B0
		public List<NodeDesigner> SelectedNodes
		{
			get
			{
				return this.mSelectedNodes;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000F0B8 File Offset: 0x0000D2B8
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000F0C0 File Offset: 0x0000D2C0
		public NodeDesigner HoverNode
		{
			get
			{
				return this.mHoverNode;
			}
			set
			{
				this.mHoverNode = value;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000F0CC File Offset: 0x0000D2CC
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000F0D4 File Offset: 0x0000D2D4
		public NodeConnection ActiveNodeConnection
		{
			get
			{
				return this.mActiveNodeConnection;
			}
			set
			{
				this.mActiveNodeConnection = value;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000F0E0 File Offset: 0x0000D2E0
		public List<NodeConnection> SelectedNodeConnections
		{
			get
			{
				return this.mSelectedNodeConnections;
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000F0E8 File Offset: 0x0000D2E8
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
		public NodeDesigner AddNode(BehaviorSource behaviorSource, Type type, Vector2 position)
		{
			Task task = Activator.CreateInstance(type, true) as Task;
			if (task == null)
			{
				EditorUtility.DisplayDialog("Unable to Add Task", string.Format("Unable to create task of type {0}. Is the class name the same as the file name?", type), "OK");
				return null;
			}
			return this.AddNode(behaviorSource, task, position);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000F13C File Offset: 0x0000D33C
		private NodeDesigner AddNode(BehaviorSource behaviorSource, Task task, Vector2 position)
		{
			if (this.mEntryNode == null)
			{
				Task task2 = Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.Tasks.EntryTask")) as Task;
				this.mEntryNode = ScriptableObject.CreateInstance<NodeDesigner>();
				this.mEntryNode.LoadNode(task2, behaviorSource, new Vector2(position.x, position.y - 120f), ref this.mNextTaskID);
				this.mEntryNode.MakeEntryDisplay();
			}
			NodeDesigner nodeDesigner = ScriptableObject.CreateInstance<NodeDesigner>();
			nodeDesigner.LoadNode(task, behaviorSource, position, ref this.mNextTaskID);
			TaskNameAttribute[] array;
			if ((array = (task.GetType().GetCustomAttributes(typeof(TaskNameAttribute), false) as TaskNameAttribute[])).Length > 0)
			{
				task.FriendlyName = array[0].Name;
			}
			if (this.mEntryNode.OutgoingNodeConnections.Count == 0)
			{
				this.mActiveNodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
				this.mActiveNodeConnection.LoadConnection(this.mEntryNode, NodeConnectionType.Outgoing);
				this.ConnectNodes(behaviorSource, nodeDesigner);
			}
			else
			{
				this.mDetachedNodes.Add(nodeDesigner);
			}
			return nodeDesigner;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000F248 File Offset: 0x0000D448
		public NodeDesigner NodeAt(Vector2 point, Vector2 offset)
		{
			if (this.mEntryNode == null)
			{
				return null;
			}
			for (int i = 0; i < this.mSelectedNodes.Count; i++)
			{
				if (this.mSelectedNodes[i].Contains(point, offset, false))
				{
					return this.mSelectedNodes[i];
				}
			}
			NodeDesigner result;
			for (int j = this.mDetachedNodes.Count - 1; j > -1; j--)
			{
				if (this.mDetachedNodes[j] != null && (result = this.NodeChildrenAt(this.mDetachedNodes[j], point, offset)) != null)
				{
					return result;
				}
			}
			if (this.mRootNode != null && (result = this.NodeChildrenAt(this.mRootNode, point, offset)) != null)
			{
				return result;
			}
			if (this.mEntryNode.Contains(point, offset, true))
			{
				return this.mEntryNode;
			}
			return null;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000F350 File Offset: 0x0000D550
		private NodeDesigner NodeChildrenAt(NodeDesigner nodeDesigner, Vector2 point, Vector2 offset)
		{
			if (nodeDesigner.Contains(point, offset, true))
			{
				return nodeDesigner;
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (!parentTask.NodeData.Collapsed && parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						NodeDesigner result;
						if (parentTask.Children[i] != null && (result = this.NodeChildrenAt(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, point, offset)) != null)
						{
							return result;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000F404 File Offset: 0x0000D604
		public List<NodeDesigner> NodesAt(Rect rect, Vector2 offset)
		{
			List<NodeDesigner> list = new List<NodeDesigner>();
			if (this.mRootNode != null)
			{
				this.NodesChildrenAt(this.mRootNode, rect, offset, ref list);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.NodesChildrenAt(this.mDetachedNodes[i], rect, offset, ref list);
			}
			return (list.Count <= 0) ? null : list;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000F480 File Offset: 0x0000D680
		private void NodesChildrenAt(NodeDesigner nodeDesigner, Rect rect, Vector2 offset, ref List<NodeDesigner> nodes)
		{
			if (nodeDesigner.Intersects(rect, offset))
			{
				nodes.Add(nodeDesigner);
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (!parentTask.NodeData.Collapsed && parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i] != null)
						{
							this.NodesChildrenAt(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, rect, offset, ref nodes);
						}
					}
				}
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000F528 File Offset: 0x0000D728
		public bool IsSelected(NodeDesigner nodeDesigner)
		{
			return this.mSelectedNodes.Contains(nodeDesigner);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000F538 File Offset: 0x0000D738
		public bool IsParentSelected(NodeDesigner nodeDesigner)
		{
			return nodeDesigner.ParentNodeDesigner != null && (this.IsSelected(nodeDesigner.ParentNodeDesigner) || this.IsParentSelected(nodeDesigner.ParentNodeDesigner));
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000F578 File Offset: 0x0000D778
		public void Select(NodeDesigner nodeDesigner)
		{
			this.Select(nodeDesigner, true);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000F584 File Offset: 0x0000D784
		public void Select(NodeDesigner nodeDesigner, bool addHash)
		{
			if (this.mSelectedNodes.Count == 1)
			{
				this.IndicateReferencedTasks(this.mSelectedNodes[0].Task, false);
			}
			this.mSelectedNodes.Add(nodeDesigner);
			if (addHash)
			{
				this.mNodeSelectedID.Add(nodeDesigner.Task.ID);
			}
			nodeDesigner.Select();
			if (this.mSelectedNodes.Count == 1)
			{
				this.IndicateReferencedTasks(this.mSelectedNodes[0].Task, true);
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000F614 File Offset: 0x0000D814
		public void Deselect(NodeDesigner nodeDesigner)
		{
			this.mSelectedNodes.Remove(nodeDesigner);
			this.mNodeSelectedID.Remove(nodeDesigner.Task.ID);
			nodeDesigner.Deselect();
			this.IndicateReferencedTasks(nodeDesigner.Task, false);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000F658 File Offset: 0x0000D858
		public void DeselectAllExcept(NodeDesigner nodeDesigner)
		{
			for (int i = this.mSelectedNodes.Count - 1; i >= 0; i--)
			{
				if (!this.mSelectedNodes[i].Equals(nodeDesigner))
				{
					this.mSelectedNodes[i].Deselect();
					this.mSelectedNodes.RemoveAt(i);
					this.mNodeSelectedID.RemoveAt(i);
				}
			}
			this.IndicateReferencedTasks(nodeDesigner.Task, false);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000F6D0 File Offset: 0x0000D8D0
		public void ClearNodeSelection()
		{
			if (this.mSelectedNodes.Count == 1)
			{
				this.IndicateReferencedTasks(this.mSelectedNodes[0].Task, false);
			}
			for (int i = 0; i < this.mSelectedNodes.Count; i++)
			{
				this.mSelectedNodes[i].Deselect();
			}
			this.mSelectedNodes.Clear();
			this.mNodeSelectedID.Clear();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000F74C File Offset: 0x0000D94C
		public void DeselectWithParent(NodeDesigner nodeDesigner)
		{
			for (int i = this.mSelectedNodes.Count - 1; i >= 0; i--)
			{
				if (this.mSelectedNodes[i].HasParent(nodeDesigner))
				{
					this.Deselect(this.mSelectedNodes[i]);
				}
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000F7A0 File Offset: 0x0000D9A0
		public bool ReplaceSelectedNode(BehaviorSource behaviorSource, Type taskType)
		{
			BehaviorUndo.RegisterUndo("Replace", behaviorSource.Owner.GetObject());
			Vector2 absolutePosition = this.SelectedNodes[0].GetAbsolutePosition();
			NodeDesigner parentNodeDesigner = this.SelectedNodes[0].ParentNodeDesigner;
			List<Task> list = (!this.SelectedNodes[0].IsParent) ? null : (this.SelectedNodes[0].Task as ParentTask).Children;
			UnknownTask unknownTask = this.SelectedNodes[0].Task as UnknownTask;
			this.RemoveNode(this.SelectedNodes[0]);
			this.mSelectedNodes.Clear();
			TaskReferences.CheckReferences(behaviorSource);
			NodeDesigner nodeDesigner = null;
			if (unknownTask != null)
			{
				Task task = null;
				if (!string.IsNullOrEmpty(unknownTask.JSONSerialization))
				{
					Dictionary<int, Task> dictionary = new Dictionary<int, Task>();
					Dictionary<string, object> dictionary2 = MiniJSON.Deserialize(unknownTask.JSONSerialization) as Dictionary<string, object>;
					if (dictionary2.ContainsKey("Type"))
					{
						dictionary2["Type"] = taskType.ToString();
					}
					task = JSONDeserialization.DeserializeTask(behaviorSource, dictionary2, ref dictionary, null);
				}
				else
				{
					TaskSerializationData taskSerializationData = new TaskSerializationData();
					taskSerializationData.types.Add(taskType.ToString());
					taskSerializationData.startIndex.Add(0);
					FieldSerializationData fieldSerializationData = new FieldSerializationData();
					fieldSerializationData.fieldNameHash = unknownTask.fieldNameHash;
					fieldSerializationData.startIndex = unknownTask.startIndex;
					fieldSerializationData.dataPosition = unknownTask.dataPosition;
					fieldSerializationData.unityObjects = unknownTask.unityObjects;
					fieldSerializationData.byteDataArray = unknownTask.byteData.ToArray();
					List<Task> list2 = new List<Task>();
					BinaryDeserialization.LoadTask(taskSerializationData, fieldSerializationData, ref list2, ref behaviorSource);
					if (list2.Count > 0)
					{
						task = list2[0];
					}
				}
				if (task != null)
				{
					nodeDesigner = this.AddNode(behaviorSource, task, absolutePosition);
				}
			}
			else
			{
				nodeDesigner = this.AddNode(behaviorSource, taskType, absolutePosition);
			}
			if (nodeDesigner == null)
			{
				return false;
			}
			if (parentNodeDesigner != null)
			{
				this.ActiveNodeConnection = parentNodeDesigner.CreateNodeConnection(false);
				this.ConnectNodes(behaviorSource, nodeDesigner);
			}
			if (nodeDesigner.IsParent && list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.ActiveNodeConnection = nodeDesigner.CreateNodeConnection(false);
					this.ConnectNodes(behaviorSource, list[i].NodeData.NodeDesigner as NodeDesigner);
					if (i >= (nodeDesigner.Task as ParentTask).MaxChildren())
					{
						break;
					}
				}
			}
			this.Select(nodeDesigner);
			return true;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000FA34 File Offset: 0x0000DC34
		public void Hover(NodeDesigner nodeDesigner)
		{
			if (!nodeDesigner.ShowHoverBar)
			{
				nodeDesigner.ShowHoverBar = true;
				this.HoverNode = nodeDesigner;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000FA50 File Offset: 0x0000DC50
		public void ClearHover()
		{
			if (this.HoverNode)
			{
				this.HoverNode.ShowHoverBar = false;
				this.HoverNode = null;
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000FA80 File Offset: 0x0000DC80
		private void IndicateReferencedTasks(Task task, bool indicate)
		{
			List<Task> referencedTasks = TaskInspector.GetReferencedTasks(task);
			if (referencedTasks != null && referencedTasks.Count > 0)
			{
				for (int i = 0; i < referencedTasks.Count; i++)
				{
					if (referencedTasks[i] != null && referencedTasks[i].NodeData != null)
					{
						NodeDesigner nodeDesigner = referencedTasks[i].NodeData.NodeDesigner as NodeDesigner;
						if (nodeDesigner != null)
						{
							nodeDesigner.ShowReferenceIcon = indicate;
						}
					}
				}
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000FB08 File Offset: 0x0000DD08
		public bool DragSelectedNodes(Vector2 delta, bool dragChildren)
		{
			if (this.mSelectedNodes.Count == 0)
			{
				return false;
			}
			bool flag = this.mSelectedNodes.Count == 1;
			for (int i = 0; i < this.mSelectedNodes.Count; i++)
			{
				this.DragNode(this.mSelectedNodes[i], delta, dragChildren);
			}
			if (flag && dragChildren && this.mSelectedNodes[0].IsEntryDisplay && this.mRootNode != null)
			{
				this.DragNode(this.mRootNode, delta, dragChildren);
			}
			return true;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000FBA8 File Offset: 0x0000DDA8
		private void DragNode(NodeDesigner nodeDesigner, Vector2 delta, bool dragChildren)
		{
			if (this.IsParentSelected(nodeDesigner) && dragChildren)
			{
				return;
			}
			nodeDesigner.ChangeOffset(delta);
			if (nodeDesigner.ParentNodeDesigner != null)
			{
				int num = nodeDesigner.ParentNodeDesigner.ChildIndexForTask(nodeDesigner.Task);
				if (num != -1)
				{
					int index = num - 1;
					bool flag = false;
					NodeDesigner nodeDesigner2 = nodeDesigner.ParentNodeDesigner.NodeDesignerForChildIndex(index);
					if (nodeDesigner2 != null && nodeDesigner.Task.NodeData.Offset.x < nodeDesigner2.Task.NodeData.Offset.x)
					{
						nodeDesigner.ParentNodeDesigner.MoveChildNode(num, true);
						flag = true;
					}
					if (!flag)
					{
						index = num + 1;
						nodeDesigner2 = nodeDesigner.ParentNodeDesigner.NodeDesignerForChildIndex(index);
						if (nodeDesigner2 != null && nodeDesigner.Task.NodeData.Offset.x > nodeDesigner2.Task.NodeData.Offset.x)
						{
							nodeDesigner.ParentNodeDesigner.MoveChildNode(num, false);
						}
					}
				}
			}
			if (nodeDesigner.IsParent && !dragChildren)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						NodeDesigner nodeDesigner3 = parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner;
						nodeDesigner3.ChangeOffset(-delta);
					}
				}
			}
			this.MarkNodeDirty(nodeDesigner);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000FD44 File Offset: 0x0000DF44
		public bool DrawNodes(Vector2 mousePosition, Vector2 offset)
		{
			if (this.mEntryNode == null)
			{
				return false;
			}
			this.mEntryNode.DrawNodeConnection(offset, false);
			if (this.mRootNode != null)
			{
				this.DrawNodeConnectionChildren(this.mRootNode, offset, this.mRootNode.Task.Disabled);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.DrawNodeConnectionChildren(this.mDetachedNodes[i], offset, this.mDetachedNodes[i].Task.Disabled);
			}
			for (int j = 0; j < this.mSelectedNodeConnections.Count; j++)
			{
				this.mSelectedNodeConnections[j].DrawConnection(offset, this.mSelectedNodeConnections[j].OriginatingNodeDesigner.IsDisabled());
			}
			if (mousePosition != new Vector2(-1f, -1f) && this.mActiveNodeConnection != null)
			{
				this.mActiveNodeConnection.HorizontalHeight = (this.mActiveNodeConnection.OriginatingNodeDesigner.GetConnectionPosition(offset, this.mActiveNodeConnection.NodeConnectionType).y + mousePosition.y) / 2f;
				this.mActiveNodeConnection.DrawConnection(this.mActiveNodeConnection.OriginatingNodeDesigner.GetConnectionPosition(offset, this.mActiveNodeConnection.NodeConnectionType), mousePosition, this.mActiveNodeConnection.NodeConnectionType == NodeConnectionType.Outgoing && this.mActiveNodeConnection.OriginatingNodeDesigner.IsDisabled());
			}
			this.mEntryNode.DrawNode(offset, false, false);
			bool result = false;
			if (this.mRootNode != null && this.DrawNodeChildren(this.mRootNode, offset, this.mRootNode.Task.Disabled))
			{
				result = true;
			}
			for (int k = 0; k < this.mDetachedNodes.Count; k++)
			{
				if (this.DrawNodeChildren(this.mDetachedNodes[k], offset, this.mDetachedNodes[k].Task.Disabled))
				{
					result = true;
				}
			}
			for (int l = 0; l < this.mSelectedNodes.Count; l++)
			{
				if (this.mSelectedNodes[l].DrawNode(offset, true, this.mSelectedNodes[l].IsDisabled()))
				{
					result = true;
				}
			}
			if (this.mRootNode != null)
			{
				this.DrawNodeCommentChildren(this.mRootNode, offset);
			}
			for (int m = 0; m < this.mDetachedNodes.Count; m++)
			{
				this.DrawNodeCommentChildren(this.mDetachedNodes[m], offset);
			}
			return result;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00010014 File Offset: 0x0000E214
		private bool DrawNodeChildren(NodeDesigner nodeDesigner, Vector2 offset, bool disabledNode)
		{
			if (nodeDesigner == null)
			{
				return false;
			}
			bool result = false;
			if (nodeDesigner.DrawNode(offset, false, disabledNode))
			{
				result = true;
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (!parentTask.NodeData.Collapsed && parentTask.Children != null)
				{
					for (int i = parentTask.Children.Count - 1; i > -1; i--)
					{
						if (parentTask.Children[i] != null && this.DrawNodeChildren(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, offset, parentTask.Disabled || disabledNode))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000100DC File Offset: 0x0000E2DC
		private void DrawNodeConnectionChildren(NodeDesigner nodeDesigner, Vector2 offset, bool disabledNode)
		{
			if (nodeDesigner == null)
			{
				return;
			}
			if (!nodeDesigner.Task.NodeData.Collapsed)
			{
				nodeDesigner.DrawNodeConnection(offset, nodeDesigner.Task.Disabled || disabledNode);
				if (nodeDesigner.IsParent)
				{
					ParentTask parentTask = nodeDesigner.Task as ParentTask;
					if (parentTask.Children != null)
					{
						for (int i = 0; i < parentTask.Children.Count; i++)
						{
							if (parentTask.Children[i] != null)
							{
								this.DrawNodeConnectionChildren(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, offset, parentTask.Disabled || disabledNode);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x000101A8 File Offset: 0x0000E3A8
		private void DrawNodeCommentChildren(NodeDesigner nodeDesigner, Vector2 offset)
		{
			if (nodeDesigner == null)
			{
				return;
			}
			nodeDesigner.DrawNodeComment(offset);
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (!parentTask.NodeData.Collapsed && parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i] != null)
						{
							this.DrawNodeCommentChildren(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, offset);
						}
					}
				}
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0001024C File Offset: 0x0000E44C
		private void RemoveNode(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner.IsEntryDisplay)
			{
				return;
			}
			if (nodeDesigner.IsParent)
			{
				for (int i = 0; i < nodeDesigner.OutgoingNodeConnections.Count; i++)
				{
					NodeDesigner destinationNodeDesigner = nodeDesigner.OutgoingNodeConnections[i].DestinationNodeDesigner;
					this.mDetachedNodes.Add(destinationNodeDesigner);
					destinationNodeDesigner.Task.NodeData.Offset = destinationNodeDesigner.GetAbsolutePosition();
					destinationNodeDesigner.ParentNodeDesigner = null;
				}
			}
			if (nodeDesigner.ParentNodeDesigner != null)
			{
				nodeDesigner.ParentNodeDesigner.RemoveChildNode(nodeDesigner);
			}
			if (this.mRootNode != null && this.mRootNode.Equals(nodeDesigner))
			{
				this.mEntryNode.RemoveChildNode(nodeDesigner);
				this.mRootNode = null;
			}
			if (this.mRootNode != null)
			{
				this.RemoveReferencedTasks(this.mRootNode, nodeDesigner.Task);
			}
			if (this.mDetachedNodes != null)
			{
				for (int j = 0; j < this.mDetachedNodes.Count; j++)
				{
					this.RemoveReferencedTasks(this.mDetachedNodes[j], nodeDesigner.Task);
				}
			}
			this.mDetachedNodes.Remove(nodeDesigner);
			BehaviorUndo.DestroyObject(nodeDesigner, false);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00010390 File Offset: 0x0000E590
		private void RemoveReferencedTasks(NodeDesigner nodeDesigner, Task task)
		{
			bool flag = false;
			bool flag2 = false;
			FieldInfo[] allFields = TaskUtility.GetAllFields(nodeDesigner.Task.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if ((!allFields[i].IsPrivate && !allFields[i].IsFamily) || BehaviorDesignerUtility.HasAttribute(allFields[i], typeof(SerializeField)))
				{
					if (typeof(IList).IsAssignableFrom(allFields[i].FieldType))
					{
						if (typeof(Task).IsAssignableFrom(allFields[i].FieldType.GetElementType()) || (allFields[i].FieldType.IsGenericType && typeof(Task).IsAssignableFrom(allFields[i].FieldType.GetGenericArguments()[0])))
						{
							Task[] array = allFields[i].GetValue(nodeDesigner.Task) as Task[];
							if (array != null)
							{
								for (int j = array.Length - 1; j > -1; j--)
								{
									if (array[i] != null && (nodeDesigner.Task.Equals(task) || array[i].Equals(task)))
									{
										TaskInspector.ReferenceTasks(nodeDesigner.Task, task, allFields[i], ref flag, ref flag2, false, false);
									}
								}
							}
						}
					}
					else if (typeof(Task).IsAssignableFrom(allFields[i].FieldType))
					{
						Task task2 = allFields[i].GetValue(nodeDesigner.Task) as Task;
						if (task2 != null && (nodeDesigner.Task.Equals(task) || task2.Equals(task)))
						{
							TaskInspector.ReferenceTasks(nodeDesigner.Task, task, allFields[i], ref flag, ref flag2, false, false);
						}
					}
				}
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int k = 0; k < parentTask.Children.Count; k++)
					{
						if (parentTask.Children[k] != null)
						{
							this.RemoveReferencedTasks(parentTask.Children[k].NodeData.NodeDesigner as NodeDesigner, task);
						}
					}
				}
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x000105D4 File Offset: 0x0000E7D4
		public bool NodeCanOriginateConnection(NodeDesigner nodeDesigner, NodeConnection connection)
		{
			return !nodeDesigner.IsEntryDisplay || (nodeDesigner.IsEntryDisplay && connection.NodeConnectionType == NodeConnectionType.Outgoing);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00010608 File Offset: 0x0000E808
		public bool NodeCanAcceptConnection(NodeDesigner nodeDesigner, NodeConnection connection)
		{
			if ((!nodeDesigner.IsEntryDisplay || connection.NodeConnectionType != NodeConnectionType.Incoming) && (nodeDesigner.IsEntryDisplay || (!nodeDesigner.IsParent && (nodeDesigner.IsParent || connection.NodeConnectionType != NodeConnectionType.Outgoing))))
			{
				return false;
			}
			if (nodeDesigner.IsEntryDisplay || connection.OriginatingNodeDesigner.IsEntryDisplay)
			{
				return true;
			}
			HashSet<NodeDesigner> hashSet = new HashSet<NodeDesigner>();
			NodeDesigner nodeDesigner2 = (connection.NodeConnectionType != NodeConnectionType.Outgoing) ? connection.OriginatingNodeDesigner : nodeDesigner;
			NodeDesigner item = (connection.NodeConnectionType != NodeConnectionType.Outgoing) ? nodeDesigner : connection.OriginatingNodeDesigner;
			return !this.CycleExists(nodeDesigner2, ref hashSet) && !hashSet.Contains(item);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x000106E0 File Offset: 0x0000E8E0
		private bool CycleExists(NodeDesigner nodeDesigner, ref HashSet<NodeDesigner> set)
		{
			if (set.Contains(nodeDesigner))
			{
				return true;
			}
			set.Add(nodeDesigner);
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (this.CycleExists(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, ref set))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00010770 File Offset: 0x0000E970
		public void ConnectNodes(BehaviorSource behaviorSource, NodeDesigner nodeDesigner)
		{
			NodeConnection nodeConnection = this.mActiveNodeConnection;
			this.mActiveNodeConnection = null;
			if (nodeConnection != null && !nodeConnection.OriginatingNodeDesigner.Equals(nodeDesigner))
			{
				NodeDesigner originatingNodeDesigner = nodeConnection.OriginatingNodeDesigner;
				if (nodeConnection.NodeConnectionType == NodeConnectionType.Outgoing)
				{
					this.RemoveParentConnection(nodeDesigner);
					this.CheckForLastConnectionRemoval(originatingNodeDesigner);
					originatingNodeDesigner.AddChildNode(nodeDesigner, nodeConnection, true, false);
				}
				else
				{
					this.RemoveParentConnection(originatingNodeDesigner);
					this.CheckForLastConnectionRemoval(nodeDesigner);
					nodeDesigner.AddChildNode(originatingNodeDesigner, nodeConnection, true, false);
				}
				if (nodeConnection.OriginatingNodeDesigner.IsEntryDisplay)
				{
					this.mRootNode = nodeConnection.DestinationNodeDesigner;
				}
				this.mDetachedNodes.Remove(nodeConnection.DestinationNodeDesigner);
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00010820 File Offset: 0x0000EA20
		private void RemoveParentConnection(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner.ParentNodeDesigner != null)
			{
				NodeDesigner parentNodeDesigner = nodeDesigner.ParentNodeDesigner;
				NodeConnection nodeConnection = null;
				for (int i = 0; i < parentNodeDesigner.OutgoingNodeConnections.Count; i++)
				{
					if (parentNodeDesigner.OutgoingNodeConnections[i].DestinationNodeDesigner.Equals(nodeDesigner))
					{
						nodeConnection = parentNodeDesigner.OutgoingNodeConnections[i];
						break;
					}
				}
				if (nodeConnection != null)
				{
					this.RemoveConnection(nodeConnection);
				}
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x000108A4 File Offset: 0x0000EAA4
		private void CheckForLastConnectionRemoval(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner.IsEntryDisplay)
			{
				if (nodeDesigner.OutgoingNodeConnections.Count == 1)
				{
					this.RemoveConnection(nodeDesigner.OutgoingNodeConnections[0]);
				}
			}
			else
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null && parentTask.Children.Count + 1 > parentTask.MaxChildren())
				{
					NodeConnection nodeConnection = null;
					for (int i = 0; i < nodeDesigner.OutgoingNodeConnections.Count; i++)
					{
						if (nodeDesigner.OutgoingNodeConnections[i].DestinationNodeDesigner.Equals(parentTask.Children[parentTask.Children.Count - 1].NodeData.NodeDesigner as NodeDesigner))
						{
							nodeConnection = nodeDesigner.OutgoingNodeConnections[i];
							break;
						}
					}
					if (nodeConnection != null)
					{
						this.RemoveConnection(nodeConnection);
					}
				}
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00010998 File Offset: 0x0000EB98
		public void NodeConnectionsAt(Vector2 point, Vector2 offset, ref List<NodeConnection> nodeConnections)
		{
			if (this.mEntryNode == null)
			{
				return;
			}
			this.NodeChildrenConnectionsAt(this.mEntryNode, point, offset, ref nodeConnections);
			if (this.mRootNode != null)
			{
				this.NodeChildrenConnectionsAt(this.mRootNode, point, offset, ref nodeConnections);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.NodeChildrenConnectionsAt(this.mDetachedNodes[i], point, offset, ref nodeConnections);
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00010A18 File Offset: 0x0000EC18
		private void NodeChildrenConnectionsAt(NodeDesigner nodeDesigner, Vector2 point, Vector2 offset, ref List<NodeConnection> nodeConnections)
		{
			if (nodeDesigner.Task.NodeData.Collapsed)
			{
				return;
			}
			nodeDesigner.ConnectionContains(point, offset, ref nodeConnections);
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask != null && parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i] != null)
						{
							this.NodeChildrenConnectionsAt(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, point, offset, ref nodeConnections);
						}
					}
				}
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00010AC0 File Offset: 0x0000ECC0
		public void RemoveConnection(NodeConnection nodeConnection)
		{
			nodeConnection.DestinationNodeDesigner.Task.NodeData.Offset = nodeConnection.DestinationNodeDesigner.GetAbsolutePosition();
			this.mDetachedNodes.Add(nodeConnection.DestinationNodeDesigner);
			nodeConnection.OriginatingNodeDesigner.RemoveChildNode(nodeConnection.DestinationNodeDesigner);
			if (nodeConnection.OriginatingNodeDesigner.IsEntryDisplay)
			{
				this.mRootNode = null;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00010B28 File Offset: 0x0000ED28
		public bool IsSelected(NodeConnection nodeConnection)
		{
			for (int i = 0; i < this.mSelectedNodeConnections.Count; i++)
			{
				if (this.mSelectedNodeConnections[i].Equals(nodeConnection))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00010B6C File Offset: 0x0000ED6C
		public void Select(NodeConnection nodeConnection)
		{
			this.mSelectedNodeConnections.Add(nodeConnection);
			nodeConnection.select();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00010B80 File Offset: 0x0000ED80
		public void Deselect(NodeConnection nodeConnection)
		{
			this.mSelectedNodeConnections.Remove(nodeConnection);
			nodeConnection.deselect();
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00010B98 File Offset: 0x0000ED98
		public void ClearConnectionSelection()
		{
			for (int i = 0; i < this.mSelectedNodeConnections.Count; i++)
			{
				this.mSelectedNodeConnections[i].deselect();
			}
			this.mSelectedNodeConnections.Clear();
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00010BE0 File Offset: 0x0000EDE0
		public void GraphDirty()
		{
			if (this.mEntryNode == null)
			{
				return;
			}
			this.mEntryNode.MarkDirty();
			if (this.mRootNode != null)
			{
				this.MarkNodeDirty(this.mRootNode);
			}
			for (int i = this.mDetachedNodes.Count - 1; i > -1; i--)
			{
				this.MarkNodeDirty(this.mDetachedNodes[i]);
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00010C58 File Offset: 0x0000EE58
		private void MarkNodeDirty(NodeDesigner nodeDesigner)
		{
			nodeDesigner.MarkDirty();
			if (nodeDesigner.IsEntryDisplay)
			{
				if (nodeDesigner.OutgoingNodeConnections.Count > 0 && nodeDesigner.OutgoingNodeConnections[0].DestinationNodeDesigner != null)
				{
					this.MarkNodeDirty(nodeDesigner.OutgoingNodeConnections[0].DestinationNodeDesigner);
				}
			}
			else if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i] != null)
						{
							this.MarkNodeDirty(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner);
						}
					}
				}
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00010D30 File Offset: 0x0000EF30
		public List<BehaviorSource> FindReferencedBehaviors()
		{
			List<BehaviorSource> result = new List<BehaviorSource>();
			if (this.mRootNode != null)
			{
				this.FindReferencedBehaviors(this.mRootNode, ref result);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.FindReferencedBehaviors(this.mDetachedNodes[i], ref result);
			}
			return result;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00010D94 File Offset: 0x0000EF94
		public void FindReferencedBehaviors(NodeDesigner nodeDesigner, ref List<BehaviorSource> behaviors)
		{
			FieldInfo[] publicFields = TaskUtility.GetPublicFields(nodeDesigner.Task.GetType());
			for (int i = 0; i < publicFields.Length; i++)
			{
				Type fieldType = publicFields[i].FieldType;
				if (typeof(IList).IsAssignableFrom(fieldType))
				{
					Type type = fieldType;
					if (fieldType.IsGenericType)
					{
						while (!type.IsGenericType)
						{
							type = type.BaseType;
						}
						type = fieldType.GetGenericArguments()[0];
					}
					else
					{
						type = fieldType.GetElementType();
					}
					if (type != null)
					{
						if (typeof(ExternalBehavior).IsAssignableFrom(type) || typeof(Behavior).IsAssignableFrom(type))
						{
							IList list = publicFields[i].GetValue(nodeDesigner.Task) as IList;
							if (list != null)
							{
								for (int j = 0; j < list.Count; j++)
								{
									if (list[j] != null)
									{
										BehaviorSource behaviorSource;
										if (list[j] is ExternalBehavior)
										{
											behaviorSource = (list[j] as ExternalBehavior).BehaviorSource;
											if (behaviorSource.Owner == null)
											{
												behaviorSource.Owner = (list[j] as ExternalBehavior);
											}
										}
										else
										{
											behaviorSource = (list[j] as Behavior).GetBehaviorSource();
											if (behaviorSource.Owner == null)
											{
												behaviorSource.Owner = (list[j] as Behavior);
											}
										}
										behaviors.Add(behaviorSource);
									}
								}
							}
						}
						else if (typeof(Behavior).IsAssignableFrom(type))
						{
						}
					}
				}
				else if (typeof(ExternalBehavior).IsAssignableFrom(fieldType) || typeof(Behavior).IsAssignableFrom(fieldType))
				{
					object value = publicFields[i].GetValue(nodeDesigner.Task);
					if (value != null)
					{
						BehaviorSource behaviorSource2;
						if (value is ExternalBehavior)
						{
							behaviorSource2 = (value as ExternalBehavior).BehaviorSource;
							if (behaviorSource2.Owner == null)
							{
								behaviorSource2.Owner = (value as ExternalBehavior);
							}
							behaviors.Add(behaviorSource2);
						}
						else
						{
							behaviorSource2 = (value as Behavior).GetBehaviorSource();
							if (behaviorSource2.Owner == null)
							{
								behaviorSource2.Owner = (value as Behavior);
							}
						}
						behaviors.Add(behaviorSource2);
					}
				}
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int k = 0; k < parentTask.Children.Count; k++)
					{
						if (parentTask.Children[k] != null)
						{
							this.FindReferencedBehaviors(parentTask.Children[k].NodeData.NodeDesigner as NodeDesigner, ref behaviors);
						}
					}
				}
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0001107C File Offset: 0x0000F27C
		public void SelectAll()
		{
			for (int i = this.mSelectedNodes.Count - 1; i > -1; i--)
			{
				this.Deselect(this.mSelectedNodes[i]);
			}
			if (this.mRootNode != null)
			{
				this.SelectAll(this.mRootNode);
			}
			for (int j = this.mDetachedNodes.Count - 1; j > -1; j--)
			{
				this.SelectAll(this.mDetachedNodes[j]);
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00011108 File Offset: 0x0000F308
		private void SelectAll(NodeDesigner nodeDesigner)
		{
			this.Select(nodeDesigner);
			if (nodeDesigner.Task.GetType().IsSubclassOf(typeof(ParentTask)))
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						this.SelectAll(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner);
					}
				}
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00011190 File Offset: 0x0000F390
		public void IdentifyNode(NodeDesigner nodeDesigner)
		{
			nodeDesigner.IdentifyNode();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00011198 File Offset: 0x0000F398
		public List<TaskSerializer> Copy(Vector2 graphOffset, float graphZoom)
		{
			List<TaskSerializer> list = new List<TaskSerializer>();
			for (int i = 0; i < this.mSelectedNodes.Count; i++)
			{
				TaskSerializer taskSerializer;
				if ((taskSerializer = TaskCopier.CopySerialized(this.mSelectedNodes[i].Task)) != null)
				{
					if (this.mSelectedNodes[i].IsParent)
					{
						ParentTask parentTask = this.mSelectedNodes[i].Task as ParentTask;
						if (parentTask.Children != null)
						{
							List<int> list2 = new List<int>();
							for (int j = 0; j < parentTask.Children.Count; j++)
							{
								int item;
								if ((item = this.mSelectedNodes.IndexOf(parentTask.Children[j].NodeData.NodeDesigner as NodeDesigner)) != -1)
								{
									list2.Add(item);
								}
							}
							taskSerializer.childrenIndex = list2;
						}
					}
					taskSerializer.offset = (taskSerializer.offset + graphOffset) * graphZoom;
					list.Add(taskSerializer);
				}
			}
			return (list.Count <= 0) ? null : list;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x000112BC File Offset: 0x0000F4BC
		public bool Paste(BehaviorSource behaviorSource, Vector3 position, List<TaskSerializer> copiedTasks, Vector2 graphOffset, float graphZoom)
		{
			if (copiedTasks == null || copiedTasks.Count == 0)
			{
				return false;
			}
			this.ClearNodeSelection();
			this.ClearConnectionSelection();
			this.RemapIDs();
			List<NodeDesigner> list = new List<NodeDesigner>();
			for (int i = 0; i < copiedTasks.Count; i++)
			{
				TaskSerializer taskSerializer = copiedTasks[i];
				Task task = TaskCopier.PasteTask(behaviorSource, taskSerializer);
				NodeDesigner nodeDesigner = ScriptableObject.CreateInstance<NodeDesigner>();
				nodeDesigner.LoadTask(task, (behaviorSource.Owner == null) ? null : (behaviorSource.Owner.GetObject() as Behavior), ref this.mNextTaskID);
				nodeDesigner.Task.NodeData.Offset = taskSerializer.offset / graphZoom - graphOffset;
				list.Add(nodeDesigner);
				this.mDetachedNodes.Add(nodeDesigner);
				this.Select(nodeDesigner);
			}
			for (int j = 0; j < copiedTasks.Count; j++)
			{
				TaskSerializer taskSerializer2 = copiedTasks[j];
				if (taskSerializer2.childrenIndex != null)
				{
					for (int k = 0; k < taskSerializer2.childrenIndex.Count; k++)
					{
						NodeDesigner nodeDesigner2 = list[j];
						NodeConnection nodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
						nodeConnection.LoadConnection(nodeDesigner2, NodeConnectionType.Outgoing);
						nodeDesigner2.AddChildNode(list[taskSerializer2.childrenIndex[k]], nodeConnection, true, false);
						this.mDetachedNodes.Remove(list[taskSerializer2.childrenIndex[k]]);
					}
				}
			}
			if (this.mEntryNode == null)
			{
				Task task2 = Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.Tasks.EntryTask")) as Task;
				this.mEntryNode = ScriptableObject.CreateInstance<NodeDesigner>();
				this.mEntryNode.LoadNode(task2, behaviorSource, new Vector2(position.x, position.y - 120f), ref this.mNextTaskID);
				this.mEntryNode.MakeEntryDisplay();
				if (this.mDetachedNodes.Count > 0)
				{
					this.mActiveNodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
					this.mActiveNodeConnection.LoadConnection(this.mEntryNode, NodeConnectionType.Outgoing);
					this.ConnectNodes(behaviorSource, this.mDetachedNodes[0]);
				}
			}
			this.Save(behaviorSource);
			return true;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x000114F4 File Offset: 0x0000F6F4
		public bool Delete(BehaviorSource behaviorSource)
		{
			bool flag = false;
			if (this.mSelectedNodeConnections != null)
			{
				for (int i = 0; i < this.mSelectedNodeConnections.Count; i++)
				{
					this.RemoveConnection(this.mSelectedNodeConnections[i]);
				}
				this.mSelectedNodeConnections.Clear();
				flag = true;
			}
			if (this.mSelectedNodes != null)
			{
				for (int j = 0; j < this.mSelectedNodes.Count; j++)
				{
					this.RemoveNode(this.mSelectedNodes[j]);
				}
				this.mSelectedNodes.Clear();
				flag = true;
			}
			if (flag)
			{
				BehaviorUndo.RegisterUndo("Delete", behaviorSource.Owner.GetObject());
				TaskReferences.CheckReferences(behaviorSource);
				this.Save(behaviorSource);
			}
			return flag;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x000115B8 File Offset: 0x0000F7B8
		public bool RemoveSharedVariableReferences(SharedVariable sharedVariable)
		{
			if (this.mEntryNode == null)
			{
				return false;
			}
			bool result = false;
			if (this.mRootNode != null && this.RemoveSharedVariableReference(this.mRootNode, sharedVariable))
			{
				result = true;
			}
			if (this.mDetachedNodes != null)
			{
				for (int i = 0; i < this.mDetachedNodes.Count; i++)
				{
					if (this.RemoveSharedVariableReference(this.mDetachedNodes[i], sharedVariable))
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00011644 File Offset: 0x0000F844
		private bool RemoveSharedVariableReference(NodeDesigner nodeDesigner, SharedVariable sharedVariable)
		{
			bool result = false;
			FieldInfo[] allFields = TaskUtility.GetAllFields(nodeDesigner.Task.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				if (typeof(SharedVariable).IsAssignableFrom(allFields[i].FieldType))
				{
					SharedVariable sharedVariable2 = allFields[i].GetValue(nodeDesigner.Task) as SharedVariable;
					if (sharedVariable2 != null && !string.IsNullOrEmpty(sharedVariable2.Name) && sharedVariable2.IsGlobal == sharedVariable.IsGlobal && sharedVariable2.Name.Equals(sharedVariable.Name))
					{
						if (!allFields[i].FieldType.IsAbstract)
						{
							sharedVariable2 = (Activator.CreateInstance(allFields[i].FieldType) as SharedVariable);
							sharedVariable2.IsShared = true;
							allFields[i].SetValue(nodeDesigner.Task, sharedVariable2);
						}
						result = true;
					}
				}
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int j = 0; j < parentTask.Children.Count; j++)
					{
						if (parentTask.Children[j] != null && this.RemoveSharedVariableReference(parentTask.Children[j].NodeData.NodeDesigner as NodeDesigner, sharedVariable))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x000117A8 File Offset: 0x0000F9A8
		private void RemapIDs()
		{
			if (this.mEntryNode == null)
			{
				return;
			}
			this.mNextTaskID = 0;
			this.mEntryNode.SetID(ref this.mNextTaskID);
			if (this.mRootNode != null)
			{
				this.mRootNode.SetID(ref this.mNextTaskID);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.mDetachedNodes[i].SetID(ref this.mNextTaskID);
			}
			this.mNodeSelectedID.Clear();
			for (int j = 0; j < this.mSelectedNodes.Count; j++)
			{
				this.mNodeSelectedID.Add(this.mSelectedNodes[j].Task.ID);
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0001187C File Offset: 0x0000FA7C
		public Rect GraphSize(Vector3 offset)
		{
			if (this.mEntryNode == null)
			{
				return default(Rect);
			}
			Rect result = default(Rect);
			result.xMin = float.MaxValue;
			result.xMax = float.MinValue;
			result.yMin = float.MaxValue;
			result.yMax = float.MinValue;
			this.GetNodeMinMax(offset, this.mEntryNode, ref result);
			if (this.mRootNode != null)
			{
				this.GetNodeMinMax(offset, this.mRootNode, ref result);
			}
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.GetNodeMinMax(offset, this.mDetachedNodes[i], ref result);
			}
			return result;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0001194C File Offset: 0x0000FB4C
		private void GetNodeMinMax(Vector2 offset, NodeDesigner nodeDesigner, ref Rect minMaxRect)
		{
			Rect rect = nodeDesigner.Rectangle(offset, true, true);
			if (rect.xMin < minMaxRect.xMin)
			{
				minMaxRect.xMin = rect.xMin;
			}
			if (rect.yMin < minMaxRect.yMin)
			{
				minMaxRect.yMin = rect.yMin;
			}
			if (rect.xMax > minMaxRect.xMax)
			{
				minMaxRect.xMax = rect.xMax;
			}
			if (rect.yMax > minMaxRect.yMax)
			{
				minMaxRect.yMax = rect.yMax;
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						this.GetNodeMinMax(offset, parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, ref minMaxRect);
					}
				}
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00011A40 File Offset: 0x0000FC40
		public void Save(BehaviorSource behaviorSource)
		{
			if (object.ReferenceEquals(behaviorSource.Owner.GetObject(), null))
			{
				return;
			}
			this.RemapIDs();
			List<Task> list = new List<Task>();
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				list.Add(this.mDetachedNodes[i].Task);
			}
			behaviorSource.Save((!(this.mEntryNode != null)) ? null : this.mEntryNode.Task, (!(this.mRootNode != null)) ? null : this.mRootNode.Task, list);
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
			{
				BinarySerialization.Save(behaviorSource);
			}
			else
			{
				JSONSerialization.Save(behaviorSource);
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00011B0C File Offset: 0x0000FD0C
		public bool Load(BehaviorSource behaviorSource, bool loadPrevBehavior, Vector2 nodePosition)
		{
			if (behaviorSource == null)
			{
				this.Clear(false);
				return false;
			}
			this.DestroyNodeDesigners();
			if (behaviorSource.Owner != null && behaviorSource.Owner is Behavior && (behaviorSource.Owner as Behavior).ExternalBehavior != null)
			{
				List<SharedVariable> list = null;
				bool force = !Application.isPlaying;
				if (Application.isPlaying && !(behaviorSource.Owner as Behavior).HasInheritedVariables)
				{
					behaviorSource.CheckForSerialization(true, null);
					list = behaviorSource.GetAllVariables();
					(behaviorSource.Owner as Behavior).HasInheritedVariables = true;
					force = true;
				}
				ExternalBehavior externalBehavior = (behaviorSource.Owner as Behavior).ExternalBehavior;
				externalBehavior.BehaviorSource.Owner = externalBehavior;
				externalBehavior.BehaviorSource.CheckForSerialization(force, behaviorSource);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						behaviorSource.SetVariable(list[i].Name, list[i]);
					}
				}
			}
			else
			{
				behaviorSource.CheckForSerialization(!Application.isPlaying, null);
			}
			if (behaviorSource.EntryTask == null && behaviorSource.RootTask == null && behaviorSource.DetachedTasks == null)
			{
				this.Clear(false);
				return false;
			}
			if (loadPrevBehavior)
			{
				this.mSelectedNodes.Clear();
				this.mSelectedNodeConnections.Clear();
				if (this.mPrevNodeSelectedID != null)
				{
					for (int j = 0; j < this.mPrevNodeSelectedID.Length; j++)
					{
						this.mNodeSelectedID.Add(this.mPrevNodeSelectedID[j]);
					}
					this.mPrevNodeSelectedID = null;
				}
			}
			else
			{
				this.Clear(false);
			}
			this.mNextTaskID = 0;
			this.mEntryNode = null;
			this.mRootNode = null;
			this.mDetachedNodes.Clear();
			Task task;
			Task task2;
			List<Task> list2;
			behaviorSource.Load(out task, out task2, out list2);
			if (BehaviorDesignerUtility.AnyNullTasks(behaviorSource) || (behaviorSource.TaskData != null && BehaviorDesignerUtility.HasRootTask(behaviorSource.TaskData.JSONSerialization) && behaviorSource.RootTask == null))
			{
				behaviorSource.CheckForSerialization(true, null);
				behaviorSource.Load(out task, out task2, out list2);
			}
			if (task == null)
			{
				if (task2 != null || (list2 != null && list2.Count > 0))
				{
					task = (behaviorSource.EntryTask = (Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.Tasks.EntryTask"), true) as Task));
					this.mEntryNode = ScriptableObject.CreateInstance<NodeDesigner>();
					if (task2 != null)
					{
						this.mEntryNode.LoadNode(task, behaviorSource, new Vector2(task2.NodeData.Offset.x, task2.NodeData.Offset.y - 120f), ref this.mNextTaskID);
					}
					else
					{
						this.mEntryNode.LoadNode(task, behaviorSource, new Vector2(nodePosition.x, nodePosition.y - 120f), ref this.mNextTaskID);
					}
					this.mEntryNode.MakeEntryDisplay();
				}
			}
			else
			{
				this.mEntryNode = ScriptableObject.CreateInstance<NodeDesigner>();
				this.mEntryNode.LoadTask(task, (behaviorSource.Owner == null) ? null : (behaviorSource.Owner.GetObject() as Behavior), ref this.mNextTaskID);
				this.mEntryNode.MakeEntryDisplay();
			}
			if (task2 != null)
			{
				this.mRootNode = ScriptableObject.CreateInstance<NodeDesigner>();
				this.mRootNode.LoadTask(task2, (behaviorSource.Owner == null) ? null : (behaviorSource.Owner.GetObject() as Behavior), ref this.mNextTaskID);
				NodeConnection nodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
				nodeConnection.LoadConnection(this.mEntryNode, NodeConnectionType.Fixed);
				this.mEntryNode.AddChildNode(this.mRootNode, nodeConnection, false, false);
				this.LoadNodeSelection(this.mRootNode);
				if (this.mEntryNode.OutgoingNodeConnections.Count == 0)
				{
					this.mActiveNodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
					this.mActiveNodeConnection.LoadConnection(this.mEntryNode, NodeConnectionType.Outgoing);
					this.ConnectNodes(behaviorSource, this.mRootNode);
				}
			}
			if (list2 != null)
			{
				for (int k = 0; k < list2.Count; k++)
				{
					if (list2[k] != null)
					{
						NodeDesigner nodeDesigner = ScriptableObject.CreateInstance<NodeDesigner>();
						nodeDesigner.LoadTask(list2[k], (behaviorSource.Owner == null) ? null : (behaviorSource.Owner.GetObject() as Behavior), ref this.mNextTaskID);
						this.mDetachedNodes.Add(nodeDesigner);
						this.LoadNodeSelection(nodeDesigner);
					}
				}
			}
			return true;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00011FA0 File Offset: 0x000101A0
		public bool HasEntryNode()
		{
			return this.mEntryNode != null && this.mEntryNode.Task != null;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00011FC8 File Offset: 0x000101C8
		public Vector2 EntryNodePosition()
		{
			return this.mEntryNode.GetAbsolutePosition();
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00011FD8 File Offset: 0x000101D8
		public void SetStartOffset(Vector2 offset)
		{
			Vector2 vector = offset - this.mEntryNode.Task.NodeData.Offset;
			this.mEntryNode.Task.NodeData.Offset = offset;
			for (int i = 0; i < this.mDetachedNodes.Count; i++)
			{
				this.mDetachedNodes[i].Task.NodeData.Offset += vector;
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0001205C File Offset: 0x0001025C
		private void LoadNodeSelection(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner == null)
			{
				return;
			}
			if (this.mNodeSelectedID != null && this.mNodeSelectedID.Contains(nodeDesigner.Task.ID))
			{
				this.Select(nodeDesigner, false);
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (parentTask.Children[i] != null && parentTask.Children[i].NodeData != null)
						{
							this.LoadNodeSelection(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner);
						}
					}
				}
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001212C File Offset: 0x0001032C
		public void Clear(bool saveSelectedNodes)
		{
			if (saveSelectedNodes)
			{
				this.mPrevNodeSelectedID = this.mNodeSelectedID.ToArray();
			}
			else
			{
				this.mPrevNodeSelectedID = null;
			}
			this.mNodeSelectedID.Clear();
			this.mSelectedNodes.Clear();
			this.mSelectedNodeConnections.Clear();
			this.DestroyNodeDesigners();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00012184 File Offset: 0x00010384
		public void DestroyNodeDesigners()
		{
			if (this.mEntryNode != null)
			{
				this.Clear(this.mEntryNode);
			}
			if (this.mRootNode != null)
			{
				this.Clear(this.mRootNode);
			}
			for (int i = this.mDetachedNodes.Count - 1; i > -1; i--)
			{
				this.Clear(this.mDetachedNodes[i]);
			}
			this.mEntryNode = null;
			this.mRootNode = null;
			this.mDetachedNodes = new List<NodeDesigner>();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00012214 File Offset: 0x00010414
		private void Clear(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner == null)
			{
				return;
			}
			if (nodeDesigner.IsParent)
			{
				ParentTask parentTask = nodeDesigner.Task as ParentTask;
				if (parentTask != null && parentTask.Children != null)
				{
					for (int i = parentTask.Children.Count - 1; i > -1; i--)
					{
						if (parentTask.Children[i] != null)
						{
							this.Clear(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner);
						}
					}
				}
			}
			nodeDesigner.DestroyConnections();
			Object.DestroyImmediate(nodeDesigner, true);
		}

		// Token: 0x04000122 RID: 290
		private NodeDesigner mEntryNode;

		// Token: 0x04000123 RID: 291
		private NodeDesigner mRootNode;

		// Token: 0x04000124 RID: 292
		private List<NodeDesigner> mDetachedNodes = new List<NodeDesigner>();

		// Token: 0x04000125 RID: 293
		[SerializeField]
		private List<NodeDesigner> mSelectedNodes = new List<NodeDesigner>();

		// Token: 0x04000126 RID: 294
		private NodeDesigner mHoverNode;

		// Token: 0x04000127 RID: 295
		private NodeConnection mActiveNodeConnection;

		// Token: 0x04000128 RID: 296
		[SerializeField]
		private List<NodeConnection> mSelectedNodeConnections = new List<NodeConnection>();

		// Token: 0x04000129 RID: 297
		[SerializeField]
		private int mNextTaskID;

		// Token: 0x0400012A RID: 298
		private List<int> mNodeSelectedID = new List<int>();

		// Token: 0x0400012B RID: 299
		[SerializeField]
		private int[] mPrevNodeSelectedID;
	}
}
