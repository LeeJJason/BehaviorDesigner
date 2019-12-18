using System;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000021 RID: 33
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class CustomObjectDrawer : Attribute
	{
		// Token: 0x06000250 RID: 592 RVA: 0x00016268 File Offset: 0x00014468
		public CustomObjectDrawer(Type type)
		{
			this.type = type;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000251 RID: 593 RVA: 0x00016278 File Offset: 0x00014478
		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x04000174 RID: 372
		private Type type;
	}
}
