using System;
using UnityEditor;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000005 RID: 5
	public class AssetCreationMenus
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002930 File Offset: 0x00000B30
		[MenuItem("Assets/Create/Behavior Designer/C# Action Task")]
		public static void CreateCSharpActionTask()
		{
			AssetCreator.ShowWindow(AssetCreator.AssetClassType.Action, true);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000293C File Offset: 0x00000B3C
		[MenuItem("Assets/Create/Behavior Designer/C# Conditional Task")]
		public static void CreateCSharpConditionalTask()
		{
			AssetCreator.ShowWindow(AssetCreator.AssetClassType.Conditional, true);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002948 File Offset: 0x00000B48
		[MenuItem("Assets/Create/Behavior Designer/Unityscript Action Task")]
		public static void CreateUnityscriptActionTask()
		{
			AssetCreator.ShowWindow(AssetCreator.AssetClassType.Action, false);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002954 File Offset: 0x00000B54
		[MenuItem("Assets/Create/Behavior Designer/Unityscript Conditional Task")]
		public static void CreateUnityscriptConditionalTask()
		{
			AssetCreator.ShowWindow(AssetCreator.AssetClassType.Conditional, false);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002960 File Offset: 0x00000B60
		[MenuItem("Assets/Create/Behavior Designer/Shared Variable")]
		public static void CreateSharedVariable()
		{
			AssetCreator.ShowWindow(AssetCreator.AssetClassType.SharedVariable, true);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000296C File Offset: 0x00000B6C
		[MenuItem("Assets/Create/Behavior Designer/External Behavior Tree")]
		public static void CreateExternalBehaviorTree()
		{
			Type type = Type.GetType("BehaviorDesigner.Runtime.ExternalBehaviorTree, Assembly-CSharp");
			if (type == null)
			{
				type = Type.GetType("BehaviorDesigner.Runtime.ExternalBehaviorTree, Assembly-CSharp-firstpass");
			}
			AssetCreator.CreateAsset(type, "NewExternalBehavior");
		}
	}
}
