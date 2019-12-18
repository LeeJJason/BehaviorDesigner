using System;
using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x0200002B RID: 43
	public abstract class Task
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x0000FE80 File Offset: 0x0000E080
		public virtual void OnAwake()
		{
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000FE84 File Offset: 0x0000E084
		public virtual void OnStart()
		{
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000FE88 File Offset: 0x0000E088
		public virtual TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000FE8C File Offset: 0x0000E08C
		public virtual void OnLateUpdate()
		{
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000FE90 File Offset: 0x0000E090
		public virtual void OnFixedUpdate()
		{
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000FE94 File Offset: 0x0000E094
		public virtual void OnEnd()
		{
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000FE98 File Offset: 0x0000E098
		public virtual void OnPause(bool paused)
		{
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000FE9C File Offset: 0x0000E09C
		public virtual float GetPriority()
		{
			return 0f;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000FEA4 File Offset: 0x0000E0A4
		public virtual float GetUtility()
		{
			return 0f;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000FEAC File Offset: 0x0000E0AC
		public virtual void OnBehaviorRestart()
		{
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000FEB0 File Offset: 0x0000E0B0
		public virtual void OnBehaviorComplete()
		{
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000FEB4 File Offset: 0x0000E0B4
		public virtual void OnReset()
		{
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000FEB8 File Offset: 0x0000E0B8
		public virtual void OnDrawGizmos()
		{
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000FEBC File Offset: 0x0000E0BC
		protected void StartCoroutine(string methodName)
		{
			this.Owner.StartTaskCoroutine(this, methodName);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000FECC File Offset: 0x0000E0CC
		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.Owner.StartCoroutine(routine);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000FEDC File Offset: 0x0000E0DC
		protected Coroutine StartCoroutine(string methodName, object value)
		{
			return this.Owner.StartTaskCoroutine(this, methodName, value);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000FEEC File Offset: 0x0000E0EC
		protected void StopCoroutine(string methodName)
		{
			this.Owner.StopTaskCoroutine(methodName);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000FEFC File Offset: 0x0000E0FC
		protected void StopAllCoroutines()
		{
			this.Owner.StopAllTaskCoroutines();
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000FF0C File Offset: 0x0000E10C
		public virtual void OnCollisionEnter(Collision collision)
		{
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000FF10 File Offset: 0x0000E110
		public virtual void OnCollisionExit(Collision collision)
		{
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000FF14 File Offset: 0x0000E114
		public virtual void OnTriggerEnter(Collider other)
		{
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000FF18 File Offset: 0x0000E118
		public virtual void OnTriggerExit(Collider other)
		{
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000FF1C File Offset: 0x0000E11C
		public virtual void OnCollisionEnter2D(Collision2D collision)
		{
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000FF20 File Offset: 0x0000E120
		public virtual void OnCollisionExit2D(Collision2D collision)
		{
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000FF24 File Offset: 0x0000E124
		public virtual void OnTriggerEnter2D(Collider2D other)
		{
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000FF28 File Offset: 0x0000E128
		public virtual void OnTriggerExit2D(Collider2D other)
		{
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000FF2C File Offset: 0x0000E12C
		public virtual void OnControllerColliderHit(ControllerColliderHit hit)
		{
		}

		// Token: 0x17000040 RID: 64
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000FF30 File Offset: 0x0000E130
		public GameObject GameObject
		{
			set
			{
				this.gameObject = value;
			}
		}

		// Token: 0x17000041 RID: 65
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x0000FF3C File Offset: 0x0000E13C
		public Transform Transform
		{
			set
			{
				this.transform = value;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000FF48 File Offset: 0x0000E148
		protected T GetComponent<T>() where T : Component
		{
			return this.gameObject.GetComponent<T>();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000FF58 File Offset: 0x0000E158
		protected Component GetComponent(Type type)
		{
			return this.gameObject.GetComponent(type);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000FF68 File Offset: 0x0000E168
		protected GameObject GetDefaultGameObject(GameObject go)
		{
			if (go == null)
			{
				return this.gameObject;
			}
			return go;
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000FF80 File Offset: 0x0000E180
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000FF88 File Offset: 0x0000E188
		public NodeData NodeData
		{
			get
			{
				return this.nodeData;
			}
			set
			{
				this.nodeData = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000FF94 File Offset: 0x0000E194
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000FF9C File Offset: 0x0000E19C
		public Behavior Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0000FFA8 File Offset: 0x0000E1A8
		// (set) Token: 0x060001FD RID: 509 RVA: 0x0000FFB0 File Offset: 0x0000E1B0
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000FFBC File Offset: 0x0000E1BC
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
		public string FriendlyName
		{
			get
			{
				return this.friendlyName;
			}
			set
			{
				this.friendlyName = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000FFD0 File Offset: 0x0000E1D0
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0000FFD8 File Offset: 0x0000E1D8
		public bool IsInstant
		{
			get
			{
				return this.instant;
			}
			set
			{
				this.instant = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000FFEC File Offset: 0x0000E1EC
		public int ReferenceID
		{
			get
			{
				return this.referenceID;
			}
			set
			{
				this.referenceID = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
		// (set) Token: 0x06000205 RID: 517 RVA: 0x00010000 File Offset: 0x0000E200
		public bool Disabled
		{
			get
			{
				return this.disabled;
			}
			set
			{
				this.disabled = value;
			}
		}

		// Token: 0x040000CC RID: 204
		protected GameObject gameObject;

		// Token: 0x040000CD RID: 205
		protected Transform transform;

		// Token: 0x040000CE RID: 206
		[SerializeField]
		private NodeData nodeData;

		// Token: 0x040000CF RID: 207
		[SerializeField]
		private Behavior owner;

		// Token: 0x040000D0 RID: 208
		[SerializeField]
		private int id = -1;

		// Token: 0x040000D1 RID: 209
		[SerializeField]
		private string friendlyName = string.Empty;

		// Token: 0x040000D2 RID: 210
		[SerializeField]
		private bool instant = true;

		// Token: 0x040000D3 RID: 211
		private int referenceID = -1;

		// Token: 0x040000D4 RID: 212
		private bool disabled;
	}
}
