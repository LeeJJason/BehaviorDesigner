using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200001A RID: 26
	public interface IBehavior
	{
		// Token: 0x06000147 RID: 327
		string GetOwnerName();

		// Token: 0x06000148 RID: 328
		int GetInstanceID();

		// Token: 0x06000149 RID: 329
		BehaviorSource GetBehaviorSource();

		// Token: 0x0600014A RID: 330
		void SetBehaviorSource(BehaviorSource behaviorSource);

		// Token: 0x0600014B RID: 331
		UnityEngine.Object GetObject();

		// Token: 0x0600014C RID: 332
		SharedVariable GetVariable(string name);

		// Token: 0x0600014D RID: 333
		void SetVariable(string name, SharedVariable item);

		// Token: 0x0600014E RID: 334
		void SetVariableValue(string name, object value);
	}
}
