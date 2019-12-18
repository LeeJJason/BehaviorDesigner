using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x0200003C RID: 60
	public class UnknownTask : Task
	{
		// Token: 0x040000E6 RID: 230
		[HideInInspector]
		public string JSONSerialization;

		// Token: 0x040000E7 RID: 231
		[HideInInspector]
		public List<int> fieldNameHash = new List<int>();

		// Token: 0x040000E8 RID: 232
		[HideInInspector]
		public List<int> startIndex = new List<int>();

		// Token: 0x040000E9 RID: 233
		[HideInInspector]
		public List<int> dataPosition = new List<int>();

		// Token: 0x040000EA RID: 234
		[HideInInspector]
		public List<UnityEngine.Object> unityObjects = new List<UnityEngine.Object>();

		// Token: 0x040000EB RID: 235
		[HideInInspector]
		public List<byte> byteData = new List<byte>();
	}
}
