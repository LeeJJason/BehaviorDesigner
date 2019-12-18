using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	public class VariableSerializationData
	{
		// Token: 0x040000C4 RID: 196
		[SerializeField]
		public List<int> variableStartIndex = new List<int>();

		// Token: 0x040000C5 RID: 197
		[SerializeField]
		public string JSONSerialization = string.Empty;

		// Token: 0x040000C6 RID: 198
		[SerializeField]
		public FieldSerializationData fieldSerializationData = new FieldSerializationData();
	}
}
