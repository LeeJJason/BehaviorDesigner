using System;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200004A RID: 74
	[Serializable]
	public class SharedNamedVariable : SharedVariable<NamedVariable>
	{
		// Token: 0x0600025B RID: 603 RVA: 0x00011010 File Offset: 0x0000F210
		public SharedNamedVariable()
		{
			this.mValue = new NamedVariable();
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00011024 File Offset: 0x0000F224
		public static implicit operator SharedNamedVariable(NamedVariable value)
		{
			return new SharedNamedVariable
			{
				mValue = value
			};
		}
	}
}
