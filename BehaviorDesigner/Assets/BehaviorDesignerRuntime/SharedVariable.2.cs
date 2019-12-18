using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000047 RID: 71
	public abstract class SharedVariable<T> : SharedVariable
	{
		// Token: 0x06000253 RID: 595 RVA: 0x00010DE0 File Offset: 0x0000EFE0
		public override void InitializePropertyMapping(BehaviorSource behaviorSource)
		{
			if (!Application.isPlaying || !(behaviorSource.Owner.GetObject() is Behavior))
			{
				return;
			}
			if (!string.IsNullOrEmpty(base.PropertyMapping))
			{
				string[] array = base.PropertyMapping.Split(new char[]
				{
					'/'
				});
				GameObject gameObject;
				if (!object.Equals(base.PropertyMappingOwner, null))
				{
					gameObject = base.PropertyMappingOwner;
				}
				else
				{
					gameObject = (behaviorSource.Owner.GetObject() as Behavior).gameObject;
				}
				Component component = gameObject.GetComponent(TaskUtility.GetTypeWithinAssembly(array[0]));
				Type type = component.GetType();
				PropertyInfo property = type.GetProperty(array[1]);
				if (property != null)
				{
					MethodInfo methodInfo = property.GetGetMethod();
					if (methodInfo != null)
					{
						this.mGetter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), component, methodInfo);
					}
					methodInfo = property.GetSetMethod();
					if (methodInfo != null)
					{
						this.mSetter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), component, methodInfo);
					}
				}
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000254 RID: 596 RVA: 0x00010EEC File Offset: 0x0000F0EC
		// (set) Token: 0x06000255 RID: 597 RVA: 0x00010F10 File Offset: 0x0000F110
		public T Value
		{
			get
			{
				return (this.mGetter == null) ? this.mValue : this.mGetter();
			}
			set
			{
				bool flag = base.NetworkSync && !this.mValue.Equals(value);
				if (this.mSetter != null)
				{
					this.mSetter(value);
				}
				else
				{
					this.mValue = value;
				}
				if (flag)
				{
					base.ValueChanged();
				}
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00010F78 File Offset: 0x0000F178
		public override object GetValue()
		{
			return this.Value;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00010F88 File Offset: 0x0000F188
		public override void SetValue(object value)
		{
			if (this.mSetter != null)
			{
				this.mSetter((T)((object)value));
			}
			else
			{
				this.mValue = (T)((object)value);
			}
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00010FB8 File Offset: 0x0000F1B8
		public override string ToString()
		{
			string result;
			if (this.Value == null)
			{
				result = "(null)";
			}
			else
			{
				T value = this.Value;
				result = value.ToString();
			}
			return result;
		}

		// Token: 0x0400011B RID: 283
		private Func<T> mGetter;

		// Token: 0x0400011C RID: 284
		private Action<T> mSetter;

		// Token: 0x0400011D RID: 285
		[SerializeField]
		protected T mValue;
	}
}
