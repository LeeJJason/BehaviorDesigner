using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000025 RID: 37
	public static class ObjectPool
	{
		// Token: 0x060001BC RID: 444 RVA: 0x0000FAF8 File Offset: 0x0000DCF8
		public static T Get<T>()
		{
			if (ObjectPool.poolDictionary.ContainsKey(typeof(T)))
			{
				Stack<T> stack = ObjectPool.poolDictionary[typeof(T)] as Stack<T>;
				if (stack.Count > 0)
				{
					return stack.Pop();
				}
			}
			return (T)((object)TaskUtility.CreateInstance(typeof(T)));
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000FB60 File Offset: 0x0000DD60
		public static void Return<T>(T obj)
		{
			if (obj == null)
			{
				return;
			}
			if (ObjectPool.poolDictionary.ContainsKey(typeof(T)))
			{
				Stack<T> stack = ObjectPool.poolDictionary[typeof(T)] as Stack<T>;
				stack.Push(obj);
			}
			else
			{
				Stack<T> stack2 = new Stack<T>();
				stack2.Push(obj);
				ObjectPool.poolDictionary.Add(typeof(T), stack2);
			}
		}

		// Token: 0x040000B4 RID: 180
		private static Dictionary<Type, object> poolDictionary = new Dictionary<Type, object>();
	}
}
