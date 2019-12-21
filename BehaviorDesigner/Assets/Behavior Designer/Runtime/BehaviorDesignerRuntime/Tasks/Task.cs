using System;
using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	// Token: 0x0200002B RID: 43
	public abstract class Task
	{
		/// <summary>
        /// 加载时
        /// </summary>
		public virtual void OnAwake()
		{
		}

		/// <summary>
        /// 启动时
        /// </summary>
		public virtual void OnStart()
		{
		}

		/// <summary>
        /// 常规更新，返回运行状态
        /// </summary>
        /// <returns></returns>
		public virtual TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}

        /// <summary>
        /// 系统事件 OnLateUpdate
        /// </summary>
        public virtual void OnLateUpdate()
		{
		}

        /// <summary>
        /// 系统事件 OnFixedUpdate
        /// </summary>
        public virtual void OnFixedUpdate()
		{
		}

		/// <summary>
        /// 运行完成
        /// </summary>
		public virtual void OnEnd()
		{
		}

		/// <summary>
        /// 暂停事件
        /// </summary>
        /// <param name="paused">是否为暂停</param>
		public virtual void OnPause(bool paused)
		{
		}

		/// <summary>
        /// 优先级
        /// </summary>
        /// <returns></returns>
		public virtual float GetPriority()
		{
			return 0f;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000FEA4 File Offset: 0x0000E0A4
		public virtual float GetUtility()
		{
			return 0f;
		}

		/// <summary>
        /// 行为树重新运行事件
        /// </summary>
		public virtual void OnBehaviorRestart()
		{
		}

		/// <summary>
        /// 行为树运行完成事件
        /// </summary>
		public virtual void OnBehaviorComplete()
		{
		}

		/// <summary>
        /// 行为树重置事件
        /// </summary>
		public virtual void OnReset()
		{
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000FEB8 File Offset: 0x0000E0B8
		public virtual void OnDrawGizmos()
		{
		}

		/// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="methodName"></param>
		protected void StartCoroutine(string methodName)
		{
			this.Owner.StartTaskCoroutine(this, methodName);
		}

		/// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.Owner.StartCoroutine(routine);
		}

		/// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		protected Coroutine StartCoroutine(string methodName, object value)
		{
			return this.Owner.StartTaskCoroutine(this, methodName, value);
		}

		/// <summary>
        /// 停止指定协程
        /// </summary>
        /// <param name="methodName"></param>
		protected void StopCoroutine(string methodName)
		{
			this.Owner.StopTaskCoroutine(methodName);
		}

		/// <summary>
        /// 停止所有协程
        /// </summary>
		protected void StopAllCoroutines()
		{
			this.Owner.StopAllTaskCoroutines();
		}

		/// <summary>
        /// 碰撞进入事件
        /// </summary>
        /// <param name="collision"></param>
		public virtual void OnCollisionEnter(Collision collision)
		{
		}

		/// <summary>
        /// 碰撞退出事件
        /// </summary>
        /// <param name="collision"></param>
		public virtual void OnCollisionExit(Collision collision)
		{
		}

		/// <summary>
        /// 触发器进入事件
        /// </summary>
        /// <param name="other"></param>
		public virtual void OnTriggerEnter(Collider other)
		{
		}

		/// <summary>
        /// 触发器退出事件
        /// </summary>
        /// <param name="other"></param>
		public virtual void OnTriggerExit(Collider other)
		{
		}

		/// <summary>
        /// 2D 碰撞进入事件
        /// </summary>
        /// <param name="collision"></param>
		public virtual void OnCollisionEnter2D(Collision2D collision)
		{
		}

		/// <summary>
        /// 2D 碰撞退出事件
        /// </summary>
        /// <param name="collision"></param>
		public virtual void OnCollisionExit2D(Collision2D collision)
		{
		}

		/// <summary>
        /// 2D 触发器进入事件
        /// </summary>
        /// <param name="other"></param>
		public virtual void OnTriggerEnter2D(Collider2D other)
		{
		}

		/// <summary>
        /// 2D 触发器退出事件
        /// </summary>
        /// <param name="other"></param>
		public virtual void OnTriggerExit2D(Collider2D other)
		{
		}

		/// <summary>
        /// 控制碰撞事件
        /// </summary>
        /// <param name="hit"></param>
		public virtual void OnControllerColliderHit(ControllerColliderHit hit)
		{
		}

        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject GameObject
		{
			set
			{
				this.gameObject = value;
			}
		}

        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform
		{
			set
			{
				this.transform = value;
			}
		}

		/// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		protected T GetComponent<T>() where T : Component
		{
			return this.gameObject.GetComponent<T>();
		}

		/// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		protected Component GetComponent(Type type)
		{
			return this.gameObject.GetComponent(type);
		}

        /// <summary>
        /// 获取 GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        protected GameObject GetDefaultGameObject(GameObject go)
		{
			if (go == null)
			{
				return this.gameObject;
			}
			return go;
		}

		/// <summary>
        /// 节点数据
        /// </summary>
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

		/// <summary>
        /// 所属行为树
        /// </summary>
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

		/// <summary>
        /// ID
        /// </summary>
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

        /// <summary>
        /// FriendlyName
        /// </summary>
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

        /// <summary>
        /// IsInstant
        /// </summary>
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

        /// <summary>
        /// ReferenceID
        /// </summary>
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

        /// <summary>
        /// Disabled
        /// </summary>
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

        /// <summary>
        /// GameObject
        /// </summary>
        protected GameObject gameObject;

        /// <summary>
        /// Transform
        /// </summary>
        protected Transform transform;

		
		[SerializeField]
		private NodeData nodeData;

		
		[SerializeField]
		private Behavior owner;

		
		[SerializeField]
		private int id = -1;

		
		[SerializeField]
		private string friendlyName = string.Empty;

		
		[SerializeField]
		private bool instant = true;

		
		private int referenceID = -1;

		
		private bool disabled;
	}
}
