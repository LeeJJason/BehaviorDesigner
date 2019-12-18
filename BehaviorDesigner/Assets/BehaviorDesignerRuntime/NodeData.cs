using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000024 RID: 36
	[Serializable]
	public class NodeData
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000F780 File Offset: 0x0000D980
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000F788 File Offset: 0x0000D988
		public object NodeDesigner
		{
			get
			{
				return this.nodeDesigner;
			}
			set
			{
				this.nodeDesigner = value;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000F794 File Offset: 0x0000D994
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000F79C File Offset: 0x0000D99C
		public Vector2 Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000F7A8 File Offset: 0x0000D9A8
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000F7B0 File Offset: 0x0000D9B0
		public string FriendlyName
		{
			get
			{
				return this.friendlyName;
			}
			set
			{
				this.friendlyName = value;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000F7BC File Offset: 0x0000D9BC
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000F7C4 File Offset: 0x0000D9C4
		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000F7D0 File Offset: 0x0000D9D0
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000F7D8 File Offset: 0x0000D9D8
		public bool IsBreakpoint
		{
			get
			{
				return this.isBreakpoint;
			}
			set
			{
				this.isBreakpoint = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000F7E4 File Offset: 0x0000D9E4
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000F7EC File Offset: 0x0000D9EC
		public Texture Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000F7F8 File Offset: 0x0000D9F8
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000F800 File Offset: 0x0000DA00
		public bool Collapsed
		{
			get
			{
				return this.collapsed;
			}
			set
			{
				this.collapsed = value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000F80C File Offset: 0x0000DA0C
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x0000F814 File Offset: 0x0000DA14
		public int ColorIndex
		{
			get
			{
				return this.colorIndex;
			}
			set
			{
				this.colorIndex = value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000F820 File Offset: 0x0000DA20
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000F828 File Offset: 0x0000DA28
		public List<string> WatchedFieldNames
		{
			get
			{
				return this.watchedFieldNames;
			}
			set
			{
				this.watchedFieldNames = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000F834 File Offset: 0x0000DA34
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000F83C File Offset: 0x0000DA3C
		public List<FieldInfo> WatchedFields
		{
			get
			{
				return this.watchedFields;
			}
			set
			{
				this.watchedFields = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000F848 File Offset: 0x0000DA48
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000F850 File Offset: 0x0000DA50
		public float PushTime
		{
			get
			{
				return this.pushTime;
			}
			set
			{
				this.pushTime = value;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000F85C File Offset: 0x0000DA5C
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000F864 File Offset: 0x0000DA64
		public float PopTime
		{
			get
			{
				return this.popTime;
			}
			set
			{
				this.popTime = value;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000F870 File Offset: 0x0000DA70
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000F878 File Offset: 0x0000DA78
		public float InterruptTime
		{
			get
			{
				return this.interruptTime;
			}
			set
			{
				this.interruptTime = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000F884 File Offset: 0x0000DA84
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000F88C File Offset: 0x0000DA8C
		public bool IsReevaluating
		{
			get
			{
				return this.isReevaluating;
			}
			set
			{
				this.isReevaluating = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x0000F898 File Offset: 0x0000DA98
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000F8A0 File Offset: 0x0000DAA0
		public TaskStatus ExecutionStatus
		{
			get
			{
				return this.executionStatus;
			}
			set
			{
				this.executionStatus = value;
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000F8AC File Offset: 0x0000DAAC
		public void InitWatchedFields(Task task)
		{
			if (this.watchedFieldNames != null && this.watchedFieldNames.Count > 0)
			{
				this.watchedFields = new List<FieldInfo>();
				for (int i = 0; i < this.watchedFieldNames.Count; i++)
				{
					FieldInfo field = task.GetType().GetField(this.watchedFieldNames[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						this.watchedFields.Add(field);
					}
				}
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000F928 File Offset: 0x0000DB28
		public void CopyFrom(NodeData nodeData, Task task)
		{
			this.nodeDesigner = nodeData.NodeDesigner;
			this.offset = nodeData.Offset;
			this.comment = nodeData.Comment;
			this.isBreakpoint = nodeData.IsBreakpoint;
			this.collapsed = nodeData.Collapsed;
			if (nodeData.WatchedFields != null && nodeData.WatchedFields.Count > 0)
			{
				this.watchedFields = new List<FieldInfo>();
				this.watchedFieldNames = new List<string>();
				for (int i = 0; i < nodeData.watchedFields.Count; i++)
				{
					FieldInfo field = task.GetType().GetField(nodeData.WatchedFields[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						this.watchedFields.Add(field);
						this.watchedFieldNames.Add(field.Name);
					}
				}
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000FA04 File Offset: 0x0000DC04
		public bool ContainsWatchedField(FieldInfo field)
		{
			return this.watchedFields != null && this.watchedFields.Contains(field);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000FA20 File Offset: 0x0000DC20
		public void AddWatchedField(FieldInfo field)
		{
			if (this.watchedFields == null)
			{
				this.watchedFields = new List<FieldInfo>();
				this.watchedFieldNames = new List<string>();
			}
			this.watchedFields.Add(field);
			this.watchedFieldNames.Add(field.Name);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000FA6C File Offset: 0x0000DC6C
		public void RemoveWatchedField(FieldInfo field)
		{
			if (this.watchedFields != null)
			{
				this.watchedFields.Remove(field);
				this.watchedFieldNames.Remove(field.Name);
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000FAA4 File Offset: 0x0000DCA4
		private static Vector2 StringToVector2(string vector2String)
		{
			string[] array = vector2String.Substring(1, vector2String.Length - 2).Split(new char[]
			{
				','
			});
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]));
		}

		// Token: 0x040000A5 RID: 165
		[SerializeField]
		private object nodeDesigner;

		// Token: 0x040000A6 RID: 166
		[SerializeField]
		private Vector2 offset;

		// Token: 0x040000A7 RID: 167
		[SerializeField]
		private string friendlyName = string.Empty;

		// Token: 0x040000A8 RID: 168
		[SerializeField]
		private string comment = string.Empty;

		// Token: 0x040000A9 RID: 169
		[SerializeField]
		private bool isBreakpoint;

		// Token: 0x040000AA RID: 170
		[SerializeField]
		private Texture icon;

		// Token: 0x040000AB RID: 171
		[SerializeField]
		private bool collapsed;

		// Token: 0x040000AC RID: 172
		[SerializeField]
		private int colorIndex;

		// Token: 0x040000AD RID: 173
		[SerializeField]
		private List<string> watchedFieldNames;

		// Token: 0x040000AE RID: 174
		private List<FieldInfo> watchedFields;

		// Token: 0x040000AF RID: 175
		private float pushTime = -1f;

		// Token: 0x040000B0 RID: 176
		private float popTime = -1f;

		// Token: 0x040000B1 RID: 177
		private float interruptTime = -1f;

		// Token: 0x040000B2 RID: 178
		private bool isReevaluating;

		// Token: 0x040000B3 RID: 179
		private TaskStatus executionStatus;
	}
}
