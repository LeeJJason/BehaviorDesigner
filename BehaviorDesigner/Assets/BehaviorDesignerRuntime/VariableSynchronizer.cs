using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200003D RID: 61
	[AddComponentMenu("Behavior Designer/Variable Synchronizer")]
	public class VariableSynchronizer : MonoBehaviour
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600022A RID: 554 RVA: 0x000104B4 File Offset: 0x0000E6B4
		// (set) Token: 0x0600022B RID: 555 RVA: 0x000104BC File Offset: 0x0000E6BC
		public UpdateIntervalType UpdateInterval
		{
			get
			{
				return this.updateInterval;
			}
			set
			{
				this.updateInterval = value;
				this.UpdateIntervalChanged();
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600022C RID: 556 RVA: 0x000104CC File Offset: 0x0000E6CC
		// (set) Token: 0x0600022D RID: 557 RVA: 0x000104D4 File Offset: 0x0000E6D4
		public float UpdateIntervalSeconds
		{
			get
			{
				return this.updateIntervalSeconds;
			}
			set
			{
				this.updateIntervalSeconds = value;
				this.UpdateIntervalChanged();
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600022E RID: 558 RVA: 0x000104E4 File Offset: 0x0000E6E4
		// (set) Token: 0x0600022F RID: 559 RVA: 0x000104EC File Offset: 0x0000E6EC
		public List<VariableSynchronizer.SynchronizedVariable> SynchronizedVariables
		{
			get
			{
				return this.synchronizedVariables;
			}
			set
			{
				this.synchronizedVariables = value;
				base.enabled = true;
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x000104FC File Offset: 0x0000E6FC
		private void UpdateIntervalChanged()
		{
			base.StopCoroutine("CoroutineUpdate");
			if (this.updateInterval == UpdateIntervalType.EveryFrame)
			{
				base.enabled = true;
			}
			else if (this.updateInterval == UpdateIntervalType.SpecifySeconds)
			{
				if (Application.isPlaying)
				{
					this.updateWait = new WaitForSeconds(this.updateIntervalSeconds);
					base.StartCoroutine("CoroutineUpdate");
				}
				base.enabled = false;
			}
			else
			{
				base.enabled = false;
			}
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00010574 File Offset: 0x0000E774
		public void Awake()
		{
			for (int i = this.synchronizedVariables.Count - 1; i > -1; i--)
			{
				VariableSynchronizer.SynchronizedVariable synchronizedVariable = this.synchronizedVariables[i];
				if (synchronizedVariable.global)
				{
					synchronizedVariable.sharedVariable = GlobalVariables.Instance.GetVariable(synchronizedVariable.variableName);
				}
				else
				{
					synchronizedVariable.sharedVariable = synchronizedVariable.behavior.GetVariable(synchronizedVariable.variableName);
				}
				string text = string.Empty;
				if (synchronizedVariable.sharedVariable == null)
				{
					text = "the SharedVariable can't be found";
				}
				else
				{
					switch (synchronizedVariable.synchronizationType)
					{
					case VariableSynchronizer.SynchronizationType.BehaviorDesigner:
					{
						Behavior behavior = synchronizedVariable.targetComponent as Behavior;
						if (behavior == null)
						{
							text = "the target component is not of type Behavior Tree";
						}
						else
						{
							if (synchronizedVariable.targetGlobal)
							{
								synchronizedVariable.targetSharedVariable = GlobalVariables.Instance.GetVariable(synchronizedVariable.targetName);
							}
							else
							{
								synchronizedVariable.targetSharedVariable = behavior.GetVariable(synchronizedVariable.targetName);
							}
							if (synchronizedVariable.targetSharedVariable == null)
							{
								text = "the target SharedVariable cannot be found";
							}
						}
						break;
					}
					case VariableSynchronizer.SynchronizationType.Property:
					{
						PropertyInfo property = synchronizedVariable.targetComponent.GetType().GetProperty(synchronizedVariable.targetName);
						if (property == null)
						{
							text = "the property " + synchronizedVariable.targetName + " doesn't exist";
						}
						else if (synchronizedVariable.setVariable)
						{
							MethodInfo getMethod = property.GetGetMethod();
							if (getMethod == null)
							{
								text = "the property has no get method";
							}
							else
							{
								synchronizedVariable.getDelegate = VariableSynchronizer.CreateGetDelegate(synchronizedVariable.targetComponent, getMethod);
							}
						}
						else
						{
							MethodInfo setMethod = property.GetSetMethod();
							if (setMethod == null)
							{
								text = "the property has no set method";
							}
							else
							{
								synchronizedVariable.setDelegate = VariableSynchronizer.CreateSetDelegate(synchronizedVariable.targetComponent, setMethod);
							}
						}
						break;
					}
					case VariableSynchronizer.SynchronizationType.Animator:
						synchronizedVariable.animator = (synchronizedVariable.targetComponent as Animator);
						if (synchronizedVariable.animator == null)
						{
							text = "the component is not of type Animator";
						}
						else
						{
							synchronizedVariable.targetID = Animator.StringToHash(synchronizedVariable.targetName);
							Type propertyType = synchronizedVariable.sharedVariable.GetType().GetProperty("Value").PropertyType;
							if (propertyType.Equals(typeof(bool)))
							{
								synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Bool;
							}
							else if (propertyType.Equals(typeof(float)))
							{
								synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Float;
							}
							else if (propertyType.Equals(typeof(int)))
							{
								synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Integer;
							}
							else
							{
								text = "there is no animator parameter type that can synchronize with " + propertyType;
							}
						}
						break;
					case VariableSynchronizer.SynchronizationType.PlayMaker:
					{
						Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_PlayMaker");
						if (typeWithinAssembly != null)
						{
							MethodInfo method = typeWithinAssembly.GetMethod("Start");
							if (method != null)
							{
								int num = (int)method.Invoke(null, new object[]
								{
									synchronizedVariable
								});
								if (num == 1)
								{
									text = "the PlayMaker NamedVariable cannot be found";
								}
								else if (num == 2)
								{
									text = "the Behavior Designer SharedVariable is not the same type as the PlayMaker NamedVariable";
								}
								else
								{
									MethodInfo method2 = typeWithinAssembly.GetMethod("Tick");
									if (method2 != null)
									{
										synchronizedVariable.thirdPartyTick = (Action<VariableSynchronizer.SynchronizedVariable>)Delegate.CreateDelegate(typeof(Action<VariableSynchronizer.SynchronizedVariable>), method2);
									}
								}
							}
						}
						else
						{
							text = "has the PlayMaker classes been imported?";
						}
						break;
					}
					case VariableSynchronizer.SynchronizationType.uFrame:
					{
						Type typeWithinAssembly2 = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_uFrame");
						if (typeWithinAssembly2 != null)
						{
							MethodInfo method3 = typeWithinAssembly2.GetMethod("Start");
							if (method3 != null)
							{
								int num2 = (int)method3.Invoke(null, new object[]
								{
									synchronizedVariable
								});
								if (num2 == 1)
								{
									text = "the uFrame property cannot be found";
								}
								else if (num2 == 2)
								{
									text = "the Behavior Designer SharedVariable is not the same type as the uFrame property";
								}
								else
								{
									MethodInfo method4 = typeWithinAssembly2.GetMethod("Tick");
									if (method4 != null)
									{
										synchronizedVariable.thirdPartyTick = (Action<VariableSynchronizer.SynchronizedVariable>)Delegate.CreateDelegate(typeof(Action<VariableSynchronizer.SynchronizedVariable>), method4);
									}
								}
							}
						}
						else
						{
							text = "has the uFrame classes been imported?";
						}
						break;
					}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					Debug.LogError(string.Format("Unable to synchronize {0}: {1}", synchronizedVariable.sharedVariable.Name, text));
					this.synchronizedVariables.RemoveAt(i);
				}
			}
			if (this.synchronizedVariables.Count == 0)
			{
				base.enabled = false;
				return;
			}
			this.UpdateIntervalChanged();
		}

		// Token: 0x06000232 RID: 562 RVA: 0x000109BC File Offset: 0x0000EBBC
		public void Update()
		{
			this.Tick();
		}

		// Token: 0x06000233 RID: 563 RVA: 0x000109C4 File Offset: 0x0000EBC4
		private IEnumerator CoroutineUpdate()
		{
			for (;;)
			{
				this.Tick();
				yield return this.updateWait;
			}
			yield break;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000109E0 File Offset: 0x0000EBE0
		public void Tick()
		{
			for (int i = 0; i < this.synchronizedVariables.Count; i++)
			{
				VariableSynchronizer.SynchronizedVariable synchronizedVariable = this.synchronizedVariables[i];
				switch (synchronizedVariable.synchronizationType)
				{
				case VariableSynchronizer.SynchronizationType.BehaviorDesigner:
					if (synchronizedVariable.setVariable)
					{
						synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.targetSharedVariable.GetValue());
					}
					else
					{
						synchronizedVariable.targetSharedVariable.SetValue(synchronizedVariable.sharedVariable.GetValue());
					}
					break;
				case VariableSynchronizer.SynchronizationType.Property:
					if (synchronizedVariable.setVariable)
					{
						synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.getDelegate());
					}
					else
					{
						synchronizedVariable.setDelegate(synchronizedVariable.sharedVariable.GetValue());
					}
					break;
				case VariableSynchronizer.SynchronizationType.Animator:
					if (synchronizedVariable.setVariable)
					{
						switch (synchronizedVariable.animatorParameterType)
						{
						case VariableSynchronizer.AnimatorParameterType.Bool:
							synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetBool(synchronizedVariable.targetID));
							break;
						case VariableSynchronizer.AnimatorParameterType.Float:
							synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetFloat(synchronizedVariable.targetID));
							break;
						case VariableSynchronizer.AnimatorParameterType.Integer:
							synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetInteger(synchronizedVariable.targetID));
							break;
						}
					}
					else
					{
						switch (synchronizedVariable.animatorParameterType)
						{
						case VariableSynchronizer.AnimatorParameterType.Bool:
							synchronizedVariable.animator.SetBool(synchronizedVariable.targetID, (bool)synchronizedVariable.sharedVariable.GetValue());
							break;
						case VariableSynchronizer.AnimatorParameterType.Float:
							synchronizedVariable.animator.SetFloat(synchronizedVariable.targetID, (float)synchronizedVariable.sharedVariable.GetValue());
							break;
						case VariableSynchronizer.AnimatorParameterType.Integer:
							synchronizedVariable.animator.SetInteger(synchronizedVariable.targetID, (int)synchronizedVariable.sharedVariable.GetValue());
							break;
						}
					}
					break;
				case VariableSynchronizer.SynchronizationType.PlayMaker:
				case VariableSynchronizer.SynchronizationType.uFrame:
					synchronizedVariable.thirdPartyTick(synchronizedVariable);
					break;
				}
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00010C04 File Offset: 0x0000EE04
		private static Func<object> CreateGetDelegate(object instance, MethodInfo method)
		{
			ConstantExpression instance2 = Expression.Constant(instance);
			MethodCallExpression expression = Expression.Call(instance2, method);
			return Expression.Lambda<Func<object>>(Expression.TypeAs(expression, typeof(object)), new ParameterExpression[0]).Compile();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00010C40 File Offset: 0x0000EE40
		private static Action<object> CreateSetDelegate(object instance, MethodInfo method)
		{
			ConstantExpression instance2 = Expression.Constant(instance);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "p");
			UnaryExpression unaryExpression = Expression.Convert(parameterExpression, method.GetParameters()[0].ParameterType);
			MethodCallExpression body = Expression.Call(instance2, method, new Expression[]
			{
				unaryExpression
			});
			return Expression.Lambda<Action<object>>(body, new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
		}

		// Token: 0x040000EC RID: 236
		[SerializeField]
		private UpdateIntervalType updateInterval;

		// Token: 0x040000ED RID: 237
		[SerializeField]
		private float updateIntervalSeconds;

		// Token: 0x040000EE RID: 238
		private WaitForSeconds updateWait;

		// Token: 0x040000EF RID: 239
		[SerializeField]
		private List<VariableSynchronizer.SynchronizedVariable> synchronizedVariables = new List<VariableSynchronizer.SynchronizedVariable>();

		// Token: 0x0200003E RID: 62
		public enum SynchronizationType
		{
			// Token: 0x040000F1 RID: 241
			BehaviorDesigner,
			// Token: 0x040000F2 RID: 242
			Property,
			// Token: 0x040000F3 RID: 243
			Animator,
			// Token: 0x040000F4 RID: 244
			PlayMaker,
			// Token: 0x040000F5 RID: 245
			uFrame
		}

		// Token: 0x0200003F RID: 63
		public enum AnimatorParameterType
		{
			// Token: 0x040000F7 RID: 247
			Bool,
			// Token: 0x040000F8 RID: 248
			Float,
			// Token: 0x040000F9 RID: 249
			Integer
		}

		// Token: 0x02000040 RID: 64
		[Serializable]
		public class SynchronizedVariable
		{
			// Token: 0x06000237 RID: 567 RVA: 0x00010CA4 File Offset: 0x0000EEA4
			public SynchronizedVariable(VariableSynchronizer.SynchronizationType synchronizationType, bool setVariable, Behavior behavior, string variableName, bool global, Component targetComponent, string targetName, bool targetGlobal)
			{
				this.synchronizationType = synchronizationType;
				this.setVariable = setVariable;
				this.behavior = behavior;
				this.variableName = variableName;
				this.global = global;
				this.targetComponent = targetComponent;
				this.targetName = targetName;
				this.targetGlobal = targetGlobal;
			}

			// Token: 0x040000FA RID: 250
			public VariableSynchronizer.SynchronizationType synchronizationType;

			// Token: 0x040000FB RID: 251
			public bool setVariable;

			// Token: 0x040000FC RID: 252
			public Behavior behavior;

			// Token: 0x040000FD RID: 253
			public string variableName;

			// Token: 0x040000FE RID: 254
			public bool global;

			// Token: 0x040000FF RID: 255
			public Component targetComponent;

			// Token: 0x04000100 RID: 256
			public string targetName;

			// Token: 0x04000101 RID: 257
			public bool targetGlobal;

			// Token: 0x04000102 RID: 258
			public SharedVariable targetSharedVariable;

			// Token: 0x04000103 RID: 259
			public Action<object> setDelegate;

			// Token: 0x04000104 RID: 260
			public Func<object> getDelegate;

			// Token: 0x04000105 RID: 261
			public Animator animator;

			// Token: 0x04000106 RID: 262
			public VariableSynchronizer.AnimatorParameterType animatorParameterType;

			// Token: 0x04000107 RID: 263
			public int targetID;

			// Token: 0x04000108 RID: 264
			public Action<VariableSynchronizer.SynchronizedVariable> thirdPartyTick;

			// Token: 0x04000109 RID: 265
			public Enum variableType;

			// Token: 0x0400010A RID: 266
			public object thirdPartyVariable;

			// Token: 0x0400010B RID: 267
			public SharedVariable sharedVariable;
		}
	}
}
