using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000035 RID: 53
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskDescriptionAttribute : Attribute
	{
		// Token: 0x06000213 RID: 531 RVA: 0x00010094 File Offset: 0x0000E294
		public TaskDescriptionAttribute(string description)
		{
			this.mDescription = description;
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000214 RID: 532 RVA: 0x000100A4 File Offset: 0x0000E2A4
		public string Description
		{
			get
			{
				return this.mDescription;
			}
		}

		// Token: 0x040000D9 RID: 217
		public readonly string mDescription;
	}
}
