using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class FieldSerializationData
	{
		// Token: 0x040000B6 RID: 182
		[SerializeField]
		public List<string> typeName = new List<string>();

		// Token: 0x040000B7 RID: 183
		[SerializeField]
		public List<int> fieldNameHash = new List<int>();

		// Token: 0x040000B8 RID: 184
		[SerializeField]
		public List<int> startIndex = new List<int>();

		// Token: 0x040000B9 RID: 185
		[SerializeField]
		public List<int> dataPosition = new List<int>();

		// Token: 0x040000BA RID: 186
		[SerializeField]
		public List<UnityEngine.Object> unityObjects = new List<UnityEngine.Object>();

		// Token: 0x040000BB RID: 187
		[SerializeField]
		public List<byte> byteData = new List<byte>();

		// Token: 0x040000BC RID: 188
		public byte[] byteDataArray;
	}
}
