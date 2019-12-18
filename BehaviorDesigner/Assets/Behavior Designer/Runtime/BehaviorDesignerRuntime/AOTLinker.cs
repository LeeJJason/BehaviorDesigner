using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class AOTLinker : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x00003724 File Offset: 0x00001924
	public void Linker()
	{
		BehaviorManager.BehaviorTree behaviorTree = new BehaviorManager.BehaviorTree();
		BehaviorManager.BehaviorTree.ConditionalReevaluate conditionalReevaluate = new BehaviorManager.BehaviorTree.ConditionalReevaluate();
		BehaviorManager.TaskAddData taskAddData = new BehaviorManager.TaskAddData();
		BehaviorManager.TaskAddData.OverrideFieldValue overrideFieldValue = new BehaviorManager.TaskAddData.OverrideFieldValue();
	}
}
