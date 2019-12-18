using System;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200004C RID: 76
	[Serializable]
	public class SharedGenericVariable : SharedVariable<GenericVariable>
	{
		// Token: 0x0600025E RID: 606 RVA: 0x00011070 File Offset: 0x0000F270
		public SharedGenericVariable()
		{
			this.mValue = new GenericVariable();
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00011084 File Offset: 0x0000F284
		public static implicit operator SharedGenericVariable(GenericVariable value)
		{
			return new SharedGenericVariable
			{
				mValue = value
			};
		}
	}
}
