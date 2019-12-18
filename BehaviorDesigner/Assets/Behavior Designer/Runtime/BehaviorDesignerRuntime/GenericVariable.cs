using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200004B RID: 75
	[Serializable]
	public class GenericVariable
	{
		// Token: 0x0600025D RID: 605 RVA: 0x00011040 File Offset: 0x0000F240
		public GenericVariable()
		{
			this.value = (Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")) as SharedVariable);
		}

		// Token: 0x0400011F RID: 287
		[SerializeField]
		public string type = "SharedString";

		// Token: 0x04000120 RID: 288
		[SerializeField]
		public SharedVariable value;
	}
}
