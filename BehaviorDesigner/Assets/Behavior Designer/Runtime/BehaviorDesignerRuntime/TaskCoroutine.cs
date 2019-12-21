using System;
using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	/// <summary>
    /// 任务协程
    /// </summary>
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

		/// <summary>
        /// 启动的协程对象
        /// </summary>
		public Coroutine Coroutine
		{
			get
			{
				return this.mCoroutine;
			}
		}

		/// <summary>
        /// 停止
        /// </summary>
		public void Stop()
		{
			this.mStop = true;
		}

		/// <summary>
        /// 协程运行函数
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 协程函数返回的 IEnumerator
        /// </summary>
        private IEnumerator mCoroutineEnumerator;

        /// <summary>
        /// 启动的协程对象
        /// </summary>
        private Coroutine mCoroutine;

		/// <summary>
        /// 行为树
        /// </summary>
		private Behavior mParent;

		/// <summary>
        /// 协程名字
        /// </summary>
		private string mCoroutineName;

        /// <summary>
        /// 是否停止
        /// </summary>
        private bool mStop;
	}
}
