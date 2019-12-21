using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	/// <summary>
    /// 行为树的抽象接口
    /// </summary>
	public interface IBehavior
	{
		/// <summary>
        /// 获取所属者名称
        /// </summary>
        /// <returns></returns>
		string GetOwnerName();

		/// <summary>
        /// 获得实例ID
        /// </summary>
        /// <returns></returns>
		int GetInstanceID();

        /// <summary>
        /// 返回 BehaviorSource
        /// </summary>
        /// <returns></returns>
        BehaviorSource GetBehaviorSource();

        /// <summary>
        /// 设置 BehaviorSource
        /// </summary>
        /// <param name="behaviorSource"></param>
        void SetBehaviorSource(BehaviorSource behaviorSource);

		/// <summary>
        /// 获得资源对象
        /// </summary>
        /// <returns></returns>
		UnityEngine.Object GetObject();

		/// <summary>
        /// 通过共享名，获得共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		SharedVariable GetVariable(string name);

		/// <summary>
        /// 通过名字添加共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
		void SetVariable(string name, SharedVariable item);

		/// <summary>
        /// 通过名字设置共享变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		void SetVariableValue(string name, object value);
	}
}
