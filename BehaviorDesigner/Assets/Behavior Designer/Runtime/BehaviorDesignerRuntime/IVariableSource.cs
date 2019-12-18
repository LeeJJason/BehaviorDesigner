using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200001B RID: 27
	public interface IVariableSource
	{
		// Token: 0x0600014F RID: 335
		SharedVariable GetVariable(string name);

		// Token: 0x06000150 RID: 336
		List<SharedVariable> GetAllVariables();

		// Token: 0x06000151 RID: 337
		void SetVariable(string name, SharedVariable sharedVariable);

		// Token: 0x06000152 RID: 338
		void UpdateVariableName(SharedVariable sharedVariable, string name);

		// Token: 0x06000153 RID: 339
		void SetAllVariables(List<SharedVariable> variables);
	}
}
