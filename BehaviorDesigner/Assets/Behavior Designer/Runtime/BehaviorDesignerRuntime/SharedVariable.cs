using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000046 RID: 70
	public abstract class SharedVariable
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00010D3C File Offset: 0x0000EF3C
		// (set) Token: 0x06000242 RID: 578 RVA: 0x00010D44 File Offset: 0x0000EF44
		public bool IsShared
		{
			get
			{
				return this.mIsShared;
			}
			set
			{
				this.mIsShared = value;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00010D50 File Offset: 0x0000EF50
		// (set) Token: 0x06000244 RID: 580 RVA: 0x00010D58 File Offset: 0x0000EF58
		public bool IsGlobal
		{
			get
			{
				return this.mIsGlobal;
			}
			set
			{
				this.mIsGlobal = value;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000245 RID: 581 RVA: 0x00010D64 File Offset: 0x0000EF64
		// (set) Token: 0x06000246 RID: 582 RVA: 0x00010D6C File Offset: 0x0000EF6C
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00010D78 File Offset: 0x0000EF78
		// (set) Token: 0x06000248 RID: 584 RVA: 0x00010D80 File Offset: 0x0000EF80
		public string PropertyMapping
		{
			get
			{
				return this.mPropertyMapping;
			}
			set
			{
				this.mPropertyMapping = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00010D8C File Offset: 0x0000EF8C
		// (set) Token: 0x0600024A RID: 586 RVA: 0x00010D94 File Offset: 0x0000EF94
		public GameObject PropertyMappingOwner
		{
			get
			{
				return this.mPropertyMappingOwner;
			}
			set
			{
				this.mPropertyMappingOwner = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600024B RID: 587 RVA: 0x00010DA0 File Offset: 0x0000EFA0
		// (set) Token: 0x0600024C RID: 588 RVA: 0x00010DA8 File Offset: 0x0000EFA8
		public bool NetworkSync
		{
			get
			{
				return this.mNetworkSync;
			}
			set
			{
				this.mNetworkSync = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600024D RID: 589 RVA: 0x00010DB4 File Offset: 0x0000EFB4
		public bool IsNone
		{
			get
			{
				return this.mIsShared && string.IsNullOrEmpty(this.mName);
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00010DD0 File Offset: 0x0000EFD0
		public void ValueChanged()
		{
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00010DD4 File Offset: 0x0000EFD4
		public virtual void InitializePropertyMapping(BehaviorSource behaviorSource)
		{
		}

		// Token: 0x06000250 RID: 592
		public abstract object GetValue();

		// Token: 0x06000251 RID: 593
		public abstract void SetValue(object value);

		// Token: 0x04000115 RID: 277
		[SerializeField]
		private bool mIsShared;

		// Token: 0x04000116 RID: 278
		[SerializeField]
		private bool mIsGlobal;

		// Token: 0x04000117 RID: 279
		[SerializeField]
		private string mName;

		// Token: 0x04000118 RID: 280
		[SerializeField]
		private string mPropertyMapping;

		// Token: 0x04000119 RID: 281
		[SerializeField]
		private GameObject mPropertyMappingOwner;

		// Token: 0x0400011A RID: 282
		[SerializeField]
		private bool mNetworkSync;
	}
}
