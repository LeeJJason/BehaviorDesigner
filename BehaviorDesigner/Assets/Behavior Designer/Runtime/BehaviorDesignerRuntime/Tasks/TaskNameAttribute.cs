using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000036 RID: 54
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskNameAttribute : Attribute
	{
		// Token: 0x06000215 RID: 533 RVA: 0x000100AC File Offset: 0x0000E2AC
		public TaskNameAttribute(string name)
		{
			this.mName = name;
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000216 RID: 534 RVA: 0x000100BC File Offset: 0x0000E2BC
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x040000DA RID: 218
		public readonly string mName;
	}
}
