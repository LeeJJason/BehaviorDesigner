using System;
using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000039 RID: 57
	public class TaskCoroutine
	{
		// Token: 0x0600021A RID: 538 RVA: 0x000100E4 File Offset: 0x0000E2E4
		public TaskCoroutine(Behavior parent, IEnumerator coroutine, string coroutineName)
		{
			this.mParent = parent;
			this.mCoroutineEnumerator = coroutine;
			this.mCoroutineName = coroutineName;
			this.mCoroutine = parent.StartCoroutine(this.RunCoroutine());
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00010114 File Offset: 0x0000E314
		public Coroutine Coroutine
		{
			get
			{
				return this.mCoroutine;
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0001011C File Offset: 0x0000E31C
		public void Stop()
		{
			this.mStop = true;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00010128 File Offset: 0x0000E328
		public IEnumerator RunCoroutine()
		{
			while (!this.mStop)
			{
				if (this.mCoroutineEnumerator == null || !this.mCoroutineEnumerator.MoveNext())
				{
					break;
				}
				yield return this.mCoroutineEnumerator.Current;
			}
			this.mParent.TaskCoroutineEnded(this, this.mCoroutineName);
			yield break;
		}

		// Token: 0x040000DC RID: 220
		private IEnumerator mCoroutineEnumerator;

		// Token: 0x040000DD RID: 221
		private Coroutine mCoroutine;

		// Token: 0x040000DE RID: 222
		private Behavior mParent;

		// Token: 0x040000DF RID: 223
		private string mCoroutineName;

		// Token: 0x040000E0 RID: 224
		private bool mStop;
	}
}
