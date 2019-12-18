using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000049 RID: 73
	[Serializable]
	public class NamedVariable : GenericVariable
	{
		// Token: 0x0400011E RID: 286
		[SerializeField]
		public string name = string.Empty;
	}
}
