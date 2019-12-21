using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
	/// <summary>
    /// 变量控制接口
    /// </summary>
	public interface IVariableSource
	{
		/// <summary>
        /// 获取指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		SharedVariable GetVariable(string name);

		/// <summary>
        /// 获取所有的共享变量列表
        /// </summary>
        /// <returns></returns>
		List<SharedVariable> GetAllVariables();

		/// <summary>
        /// 添加指定名字的共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sharedVariable"></param>
		void SetVariable(string name, SharedVariable sharedVariable);

		/// <summary>
        /// 更新指定共享变量的名字
        /// </summary>
        /// <param name="sharedVariable"></param>
        /// <param name="name"></param>
		void UpdateVariableName(SharedVariable sharedVariable, string name);

		/// <summary>
        /// 设置共享变量的列表
        /// </summary>
        /// <param name="variables"></param>
		void SetAllVariables(List<SharedVariable> variables);
	}
}
