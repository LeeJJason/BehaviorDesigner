using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace BehaviorDesigner.Editor
{
	// Token: 0x02000023 RID: 35
	[Serializable]
	public class TaskSerializer
	{
		// Token: 0x04000178 RID: 376
		public string serialization;

		// Token: 0x04000179 RID: 377
		public Vector2 offset;

		// Token: 0x0400017A RID: 378
		public List<Object> unityObjects;

		// Token: 0x0400017B RID: 379
		public List<int> childrenIndex;
	}
}
