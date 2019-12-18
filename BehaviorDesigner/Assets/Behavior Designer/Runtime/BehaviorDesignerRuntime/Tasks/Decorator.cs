using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000045 RID: 69
	public class Decorator : ParentTask
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00010D30 File Offset: 0x0000EF30
		public override int MaxChildren()
		{
			return 1;
		}
	}
}
