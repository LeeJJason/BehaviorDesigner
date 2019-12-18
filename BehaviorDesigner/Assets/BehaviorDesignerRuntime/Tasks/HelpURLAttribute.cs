using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000033 RID: 51
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class HelpURLAttribute : Attribute
	{
		// Token: 0x0600020F RID: 527 RVA: 0x00010064 File Offset: 0x0000E264
		public HelpURLAttribute(string url)
		{
			this.mURL = url;
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000210 RID: 528 RVA: 0x00010074 File Offset: 0x0000E274
		public string URL
		{
			get
			{
				return this.mURL;
			}
		}

		// Token: 0x040000D7 RID: 215
		private readonly string mURL;
	}
}
