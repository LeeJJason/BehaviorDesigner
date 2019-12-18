using System;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000011 RID: 17
	[Serializable]
	public class ErrorDetails
	{
		// Token: 0x06000172 RID: 370 RVA: 0x0000CE0C File Offset: 0x0000B00C
		public ErrorDetails(ErrorDetails.ErrorType type, Task task, int index, string fieldName)
		{
			this.mType = type;
			if (task != null)
			{
				this.mNodeDesigner = (task.NodeData.NodeDesigner as NodeDesigner);
				this.mTaskFriendlyName = task.FriendlyName;
				this.mTaskType = task.GetType().ToString();
			}
			this.mIndex = index;
			this.mFieldName = fieldName;
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000173 RID: 371 RVA: 0x0000CE78 File Offset: 0x0000B078
		public ErrorDetails.ErrorType Type
		{
			get
			{
				return this.mType;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000174 RID: 372 RVA: 0x0000CE80 File Offset: 0x0000B080
		public NodeDesigner NodeDesigner
		{
			get
			{
				return this.mNodeDesigner;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000CE88 File Offset: 0x0000B088
		public string TaskFriendlyName
		{
			get
			{
				return this.mTaskFriendlyName;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000CE90 File Offset: 0x0000B090
		public string TaskType
		{
			get
			{
				return this.mTaskType;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000CE98 File Offset: 0x0000B098
		public int Index
		{
			get
			{
				return this.mIndex;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000CEA0 File Offset: 0x0000B0A0
		public string FieldName
		{
			get
			{
				return this.mFieldName;
			}
		}

		// Token: 0x040000F8 RID: 248
		[SerializeField]
		private ErrorDetails.ErrorType mType;

		// Token: 0x040000F9 RID: 249
		[SerializeField]
		private NodeDesigner mNodeDesigner;

		// Token: 0x040000FA RID: 250
		[SerializeField]
		private string mTaskFriendlyName;

		// Token: 0x040000FB RID: 251
		[SerializeField]
		private string mTaskType;

		// Token: 0x040000FC RID: 252
		[SerializeField]
		private int mIndex = -1;

		// Token: 0x040000FD RID: 253
		[SerializeField]
		private string mFieldName;

		// Token: 0x02000012 RID: 18
		public enum ErrorType
		{
			// Token: 0x040000FF RID: 255
			RequiredField,
			// Token: 0x04000100 RID: 256
			SharedVariable,
			// Token: 0x04000101 RID: 257
			MissingChildren,
			// Token: 0x04000102 RID: 258
			UnknownTask,
			// Token: 0x04000103 RID: 259
			InvalidTaskReference,
			// Token: 0x04000104 RID: 260
			InvalidVariableReference
		}
	}
}
