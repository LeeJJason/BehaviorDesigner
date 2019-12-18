using System;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000020 RID: 32
	public class ObjectDrawer
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00016214 File Offset: 0x00014414
		// (set) Token: 0x06000248 RID: 584 RVA: 0x0001621C File Offset: 0x0001441C
		public FieldInfo FieldInfo
		{
			get
			{
				return this.fieldInfo;
			}
			set
			{
				this.fieldInfo = value;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00016228 File Offset: 0x00014428
		// (set) Token: 0x0600024A RID: 586 RVA: 0x00016230 File Offset: 0x00014430
		public ObjectDrawerAttribute Attribute
		{
			get
			{
				return this.attribute;
			}
			set
			{
				this.attribute = value;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0001623C File Offset: 0x0001443C
		// (set) Token: 0x0600024C RID: 588 RVA: 0x00016244 File Offset: 0x00014444
		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600024D RID: 589 RVA: 0x00016250 File Offset: 0x00014450
		// (set) Token: 0x0600024E RID: 590 RVA: 0x00016258 File Offset: 0x00014458
		public Task Task
		{
			get
			{
				return this.task;
			}
			set
			{
				this.task = value;
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00016264 File Offset: 0x00014464
		public virtual void OnGUI(GUIContent label)
		{
		}

		// Token: 0x04000170 RID: 368
		protected FieldInfo fieldInfo;

		// Token: 0x04000171 RID: 369
		protected ObjectDrawerAttribute attribute;

		// Token: 0x04000172 RID: 370
		protected object value;

		// Token: 0x04000173 RID: 371
		protected Task task;
	}
}
