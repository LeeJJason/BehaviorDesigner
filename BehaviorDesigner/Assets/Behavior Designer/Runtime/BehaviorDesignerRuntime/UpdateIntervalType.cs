using System;

namespace BehaviorDesigner.Runtime
{
	/// <summary>
    /// 行为树更新方式
    /// </summary>
	public enum UpdateIntervalType
	{
		/// <summary>
        /// 每帧
        /// </summary>
		EveryFrame,
		/// <summary>
        /// 指定时间
        /// </summary>
		SpecifySeconds,
		/// <summary>
        /// 自定义
        /// </summary>
		Manual
	}
}
