using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000028 RID: 40
	[Serializable]
	public class TaskSerializationData
	{
		// Token: 0x040000BD RID: 189
		[SerializeField]
		public List<string> types = new List<string>();

		// Token: 0x040000BE RID: 190
		[SerializeField]
		public List<int> parentIndex = new List<int>();

		// Token: 0x040000BF RID: 191
		[SerializeField]
		public List<int> startIndex = new List<int>();

		// Token: 0x040000C0 RID: 192
		[SerializeField]
		public List<int> variableStartIndex = new List<int>();

		// Token: 0x040000C1 RID: 193
		[SerializeField]
		public string JSONSerialization;

		// Token: 0x040000C2 RID: 194
		[SerializeField]
		public FieldSerializationData fieldSerializationData = new FieldSerializationData();

		// Token: 0x040000C3 RID: 195
		[SerializeField]
		public string Version;
	}
}
