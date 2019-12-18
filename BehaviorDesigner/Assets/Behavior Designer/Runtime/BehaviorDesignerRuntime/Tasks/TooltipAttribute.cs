using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000031 RID: 49
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class TooltipAttribute : Attribute
	{
		// Token: 0x0600020B RID: 523 RVA: 0x00010034 File Offset: 0x0000E234
		public TooltipAttribute(string tooltip)
		{
			this.mTooltip = tooltip;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00010044 File Offset: 0x0000E244
		public string Tooltip
		{
			get
			{
				return this.mTooltip;
			}
		}

		// Token: 0x040000D5 RID: 213
		public readonly string mTooltip;
	}
}
