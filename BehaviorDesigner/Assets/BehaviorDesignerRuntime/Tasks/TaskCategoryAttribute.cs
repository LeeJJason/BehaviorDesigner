using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000034 RID: 52
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskCategoryAttribute : Attribute
	{
		// Token: 0x06000211 RID: 529 RVA: 0x0001007C File Offset: 0x0000E27C
		public TaskCategoryAttribute(string category)
		{
			this.mCategory = category;
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0001008C File Offset: 0x0000E28C
		public string Category
		{
			get
			{
				return this.mCategory;
			}
		}

		// Token: 0x040000D8 RID: 216
		public readonly string mCategory;
	}
}
