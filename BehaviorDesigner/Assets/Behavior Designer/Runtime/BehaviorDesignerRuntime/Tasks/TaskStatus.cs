using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	/// <summary>
    /// 任务状态
    /// </summary>
	public enum TaskStatus
	{
		/// <summary>
        /// 无效
        /// </summary>
		Inactive,
		/// <summary>
        /// 失败
        /// </summary>
		Failure,
		/// <summary>
        /// 成功
        /// </summary>
		Success,
		/// <summary>
        /// 运行
        /// </summary>
		Running
	}
}
