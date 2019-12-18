using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200000D RID: 13
	public class BehaviorUndo
	{
		// Token: 0x0600014C RID: 332 RVA: 0x0000BABC File Offset: 0x00009CBC
		public static void RegisterUndo(string undoName, Object undoObject)
		{
			Undo.RecordObject(undoObject, undoName);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000BAC8 File Offset: 0x00009CC8
		public static Component AddComponent(GameObject undoObject, Type type)
		{
			return Undo.AddComponent(undoObject, type);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000BAD4 File Offset: 0x00009CD4
		public static void DestroyObject(Object undoObject, bool registerScene)
		{
			Undo.DestroyObjectImmediate(undoObject);
		}
	}
}
