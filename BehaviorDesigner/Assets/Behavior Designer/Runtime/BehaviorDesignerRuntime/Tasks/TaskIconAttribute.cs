using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000032 RID: 50
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskIconAttribute : Attribute
	{
		// Token: 0x0600020D RID: 525 RVA: 0x0001004C File Offset: 0x0000E24C
		public TaskIconAttribute(string iconPath)
		{
			this.mIconPath = iconPath;
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0001005C File Offset: 0x0000E25C
		public string IconPath
		{
			get
			{
				return this.mIconPath;
			}
		}

		// Token: 0x040000D6 RID: 214
		public readonly string mIconPath;
	}
}
