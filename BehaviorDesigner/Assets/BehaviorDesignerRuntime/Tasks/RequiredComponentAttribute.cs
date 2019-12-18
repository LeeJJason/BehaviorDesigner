using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000037 RID: 55
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RequiredComponentAttribute : Attribute
	{
		// Token: 0x06000217 RID: 535 RVA: 0x000100C4 File Offset: 0x0000E2C4
		public RequiredComponentAttribute(Type componentType)
		{
			this.mComponentType = componentType;
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000218 RID: 536 RVA: 0x000100D4 File Offset: 0x0000E2D4
		public Type ComponentType
		{
			get
			{
				return this.mComponentType;
			}
		}

		// Token: 0x040000DB RID: 219
		public readonly Type mComponentType;
	}
}
