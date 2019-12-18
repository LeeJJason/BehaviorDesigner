using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x02000026 RID: 38
	public abstract class ParentTask : Task
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001BF RID: 447 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x0000FBEC File Offset: 0x0000DDEC
		public List<Task> Children
		{
			get
			{
				return this.children;
			}
			private set
			{
				this.children = value;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000FBF8 File Offset: 0x0000DDF8
		public virtual int MaxChildren()
		{
			return int.MaxValue;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000FC00 File Offset: 0x0000DE00
		public virtual bool CanRunParallelChildren()
		{
			return false;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000FC04 File Offset: 0x0000DE04
		public virtual int CurrentChildIndex()
		{
			return 0;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000FC08 File Offset: 0x0000DE08
		public virtual bool CanExecute()
		{
			return true;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000FC0C File Offset: 0x0000DE0C
		public virtual TaskStatus Decorate(TaskStatus status)
		{
			return status;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000FC10 File Offset: 0x0000DE10
		public virtual bool CanReevaluate()
		{
			return false;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000FC14 File Offset: 0x0000DE14
		public virtual bool OnReevaluationStarted()
		{
			return false;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000FC18 File Offset: 0x0000DE18
		public virtual void OnReevaluationEnded(TaskStatus status)
		{
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000FC1C File Offset: 0x0000DE1C
		public virtual void OnChildExecuted(TaskStatus childStatus)
		{
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000FC20 File Offset: 0x0000DE20
		public virtual void OnChildExecuted(int childIndex, TaskStatus childStatus)
		{
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000FC24 File Offset: 0x0000DE24
		public virtual void OnChildStarted()
		{
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000FC28 File Offset: 0x0000DE28
		public virtual void OnChildStarted(int childIndex)
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000FC2C File Offset: 0x0000DE2C
		public virtual TaskStatus OverrideStatus(TaskStatus status)
		{
			return status;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000FC30 File Offset: 0x0000DE30
		public virtual TaskStatus OverrideStatus()
		{
			return TaskStatus.Running;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000FC34 File Offset: 0x0000DE34
		public virtual void OnConditionalAbort(int childIndex)
		{
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000FC38 File Offset: 0x0000DE38
		public override float GetUtility()
		{
			float num = 0f;
			if (this.children != null)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null && !this.children[i].Disabled)
					{
						num += this.children[i].GetUtility();
					}
				}
			}
			return num;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000FCB0 File Offset: 0x0000DEB0
		public override void OnDrawGizmos()
		{
			if (this.children != null)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null && !this.children[i].Disabled)
					{
						this.children[i].OnDrawGizmos();
					}
				}
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000FD1C File Offset: 0x0000DF1C
		public void AddChild(Task child, int index)
		{
			if (this.children == null)
			{
				this.children = new List<Task>();
			}
			this.children.Insert(index, child);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000FD44 File Offset: 0x0000DF44
		public void ReplaceAddChild(Task child, int index)
		{
			if (this.children != null && index < this.children.Count)
			{
				this.children[index] = child;
			}
			else
			{
				this.AddChild(child, index);
			}
		}

		// Token: 0x040000B5 RID: 181
		[SerializeField]
		protected List<Task> children;
	}
}
