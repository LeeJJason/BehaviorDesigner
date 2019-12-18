using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000044 RID: 68
	public abstract class Composite : ParentTask
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00010D20 File Offset: 0x0000EF20
		public AbortType AbortType
		{
			get
			{
				return this.abortType;
			}
		}

		// Token: 0x04000114 RID: 276
		[Tooltip("Specifies the type of conditional abort. More information is located at http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=89.")]
		[SerializeField]
		protected AbortType abortType;
	}
}
