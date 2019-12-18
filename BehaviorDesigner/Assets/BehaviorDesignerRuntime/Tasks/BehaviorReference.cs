using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000042 RID: 66
	[TaskDescription("Behavior Reference allows you to run another behavior tree within the current behavior tree.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=53")]
	[TaskIcon("BehaviorTreeReferenceIcon.png")]
	public abstract class BehaviorReference : Action
	{
		// Token: 0x0600023A RID: 570 RVA: 0x00010D04 File Offset: 0x0000EF04
		public virtual ExternalBehavior[] GetExternalBehaviors()
		{
			return this.externalBehaviors;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00010D0C File Offset: 0x0000EF0C
		public override void OnReset()
		{
			this.externalBehaviors = null;
		}

		// Token: 0x0400010C RID: 268
		[Tooltip("External behavior array that this task should reference")]
		public ExternalBehavior[] externalBehaviors;

		// Token: 0x0400010D RID: 269
		[Tooltip("Any variables that should be set for the specific tree")]
		public SharedNamedVariable[] variables;

		// Token: 0x0400010E RID: 270
		[HideInInspector]
		public bool collapsed;
	}
}
