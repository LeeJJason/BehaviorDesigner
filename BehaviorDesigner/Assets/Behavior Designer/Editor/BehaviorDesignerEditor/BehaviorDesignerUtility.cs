using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000008 RID: 8
	public static class BehaviorDesignerUtility
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000031D4 File Offset: 0x000013D4
		public static GUIStyle GraphStatusGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.graphStatusGUIStyle == null)
				{
					BehaviorDesignerUtility.InitGraphStatusGUIStyle();
				}
				return BehaviorDesignerUtility.graphStatusGUIStyle;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000031EC File Offset: 0x000013EC
		public static GUIStyle TaskFoldoutGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskFoldoutGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskFoldoutGUIStyle();
				}
				return BehaviorDesignerUtility.taskFoldoutGUIStyle;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00003204 File Offset: 0x00001404
		public static GUIStyle TaskTitleGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskTitleGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskTitleGUIStyle();
				}
				return BehaviorDesignerUtility.taskTitleGUIStyle;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000321C File Offset: 0x0000141C
		public static GUIStyle GetTaskGUIStyle(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskGUIStyle[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskGUIStyle(colorIndex);
			}
			return BehaviorDesignerUtility.taskGUIStyle[colorIndex];
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003238 File Offset: 0x00001438
		public static GUIStyle GetTaskCompactGUIStyle(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskCompactGUIStyle[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskCompactGUIStyle(colorIndex);
			}
			return BehaviorDesignerUtility.taskCompactGUIStyle[colorIndex];
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003254 File Offset: 0x00001454
		public static GUIStyle GetTaskSelectedGUIStyle(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskSelectedGUIStyle[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskSelectedGUIStyle(colorIndex);
			}
			return BehaviorDesignerUtility.taskSelectedGUIStyle[colorIndex];
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003270 File Offset: 0x00001470
		public static GUIStyle GetTaskSelectedCompactGUIStyle(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskSelectedCompactGUIStyle[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskSelectedCompactGUIStyle(colorIndex);
			}
			return BehaviorDesignerUtility.taskSelectedCompactGUIStyle[colorIndex];
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000328C File Offset: 0x0000148C
		public static GUIStyle TaskRunningGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskRunningGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskRunningGUIStyle();
				}
				return BehaviorDesignerUtility.taskRunningGUIStyle;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002A RID: 42 RVA: 0x000032A4 File Offset: 0x000014A4
		public static GUIStyle TaskRunningCompactGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskRunningCompactGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskRunningCompactGUIStyle();
				}
				return BehaviorDesignerUtility.taskRunningCompactGUIStyle;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000032BC File Offset: 0x000014BC
		public static GUIStyle TaskRunningSelectedGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskRunningSelectedGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskRunningSelectedGUIStyle();
				}
				return BehaviorDesignerUtility.taskRunningSelectedGUIStyle;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000032D4 File Offset: 0x000014D4
		public static GUIStyle TaskRunningSelectedCompactGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskRunningSelectedCompactGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskRunningSelectedCompactGUIStyle();
				}
				return BehaviorDesignerUtility.taskRunningSelectedCompactGUIStyle;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000032EC File Offset: 0x000014EC
		public static GUIStyle TaskIdentifyGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskIdentifyGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskIdentifyGUIStyle();
				}
				return BehaviorDesignerUtility.taskIdentifyGUIStyle;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00003304 File Offset: 0x00001504
		public static GUIStyle TaskIdentifyCompactGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskIdentifyCompactGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskIdentifyCompactGUIStyle();
				}
				return BehaviorDesignerUtility.taskIdentifyCompactGUIStyle;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x0000331C File Offset: 0x0000151C
		public static GUIStyle TaskIdentifySelectedGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskIdentifySelectedGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskIdentifySelectedGUIStyle();
				}
				return BehaviorDesignerUtility.taskIdentifySelectedGUIStyle;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00003334 File Offset: 0x00001534
		public static GUIStyle TaskIdentifySelectedCompactGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskIdentifySelectedCompactGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskIdentifySelectedCompactGUIStyle();
				}
				return BehaviorDesignerUtility.taskIdentifySelectedCompactGUIStyle;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000031 RID: 49 RVA: 0x0000334C File Offset: 0x0000154C
		public static GUIStyle TaskHighlightGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskHighlightGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskHighlightGUIStyle();
				}
				return BehaviorDesignerUtility.taskHighlightGUIStyle;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00003364 File Offset: 0x00001564
		public static GUIStyle TaskHighlightCompactGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskHighlightCompactGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskHighlightCompactGUIStyle();
				}
				return BehaviorDesignerUtility.taskHighlightCompactGUIStyle;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000337C File Offset: 0x0000157C
		public static GUIStyle TaskCommentGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskCommentGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskCommentGUIStyle();
				}
				return BehaviorDesignerUtility.taskCommentGUIStyle;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00003394 File Offset: 0x00001594
		public static GUIStyle TaskCommentLeftAlignGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskCommentLeftAlignGUIStyle();
				}
				return BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000033AC File Offset: 0x000015AC
		public static GUIStyle TaskCommentRightAlignGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskCommentRightAlignGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskCommentRightAlignGUIStyle();
				}
				return BehaviorDesignerUtility.taskCommentRightAlignGUIStyle;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000036 RID: 54 RVA: 0x000033C4 File Offset: 0x000015C4
		public static GUIStyle TaskDescriptionGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskDescriptionGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskDescriptionGUIStyle();
				}
				return BehaviorDesignerUtility.taskDescriptionGUIStyle;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000033DC File Offset: 0x000015DC
		public static GUIStyle GraphBackgroundGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.graphBackgroundGUIStyle == null)
				{
					BehaviorDesignerUtility.InitGraphBackgroundGUIStyle();
				}
				return BehaviorDesignerUtility.graphBackgroundGUIStyle;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000033F4 File Offset: 0x000015F4
		public static GUIStyle SelectionGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.selectionGUIStyle == null)
				{
					BehaviorDesignerUtility.InitSelectionGUIStyle();
				}
				return BehaviorDesignerUtility.selectionGUIStyle;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000039 RID: 57 RVA: 0x0000340C File Offset: 0x0000160C
		public static GUIStyle SharedVariableToolbarPopup
		{
			get
			{
				if (BehaviorDesignerUtility.sharedVariableToolbarPopup == null)
				{
					BehaviorDesignerUtility.InitSharedVariableToolbarPopup();
				}
				return BehaviorDesignerUtility.sharedVariableToolbarPopup;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00003424 File Offset: 0x00001624
		public static GUIStyle LabelWrapGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.labelWrapGUIStyle == null)
				{
					BehaviorDesignerUtility.InitLabelWrapGUIStyle();
				}
				return BehaviorDesignerUtility.labelWrapGUIStyle;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000343C File Offset: 0x0000163C
		public static GUIStyle ToolbarButtonLeftAlignGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.tolbarButtonLeftAlignGUIStyle == null)
				{
					BehaviorDesignerUtility.InitToolbarButtonLeftAlignGUIStyle();
				}
				return BehaviorDesignerUtility.tolbarButtonLeftAlignGUIStyle;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00003454 File Offset: 0x00001654
		public static GUIStyle ToolbarLabelGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.toolbarLabelGUIStyle == null)
				{
					BehaviorDesignerUtility.InitToolbarLabelGUIStyle();
				}
				return BehaviorDesignerUtility.toolbarLabelGUIStyle;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600003D RID: 61 RVA: 0x0000346C File Offset: 0x0000166C
		public static GUIStyle TaskInspectorCommentGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskInspectorCommentGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskInspectorCommentGUIStyle();
				}
				return BehaviorDesignerUtility.taskInspectorCommentGUIStyle;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00003484 File Offset: 0x00001684
		public static GUIStyle TaskInspectorGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.taskInspectorGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTaskInspectorGUIStyle();
				}
				return BehaviorDesignerUtility.taskInspectorGUIStyle;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000349C File Offset: 0x0000169C
		public static GUIStyle ToolbarButtonSelectionGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.toolbarButtonSelectionGUIStyle == null)
				{
					BehaviorDesignerUtility.InitToolbarButtonSelectionGUIStyle();
				}
				return BehaviorDesignerUtility.toolbarButtonSelectionGUIStyle;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000034B4 File Offset: 0x000016B4
		public static GUIStyle PropertyBoxGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.propertyBoxGUIStyle == null)
				{
					BehaviorDesignerUtility.InitPropertyBoxGUIStyle();
				}
				return BehaviorDesignerUtility.propertyBoxGUIStyle;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000034CC File Offset: 0x000016CC
		public static GUIStyle PreferencesPaneGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.preferencesPaneGUIStyle == null)
				{
					BehaviorDesignerUtility.InitPreferencesPaneGUIStyle();
				}
				return BehaviorDesignerUtility.preferencesPaneGUIStyle;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000034E4 File Offset: 0x000016E4
		public static GUIStyle PlainButtonGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.plainButtonGUIStyle == null)
				{
					BehaviorDesignerUtility.InitPlainButtonGUIStyle();
				}
				return BehaviorDesignerUtility.plainButtonGUIStyle;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000034FC File Offset: 0x000016FC
		public static GUIStyle TransparentButtonGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.transparentButtonGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTransparentButtonGUIStyle();
				}
				return BehaviorDesignerUtility.transparentButtonGUIStyle;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003514 File Offset: 0x00001714
		public static GUIStyle TransparentButtonOffsetGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.transparentButtonOffsetGUIStyle == null)
				{
					BehaviorDesignerUtility.InitTransparentButtonOffsetGUIStyle();
				}
				return BehaviorDesignerUtility.transparentButtonOffsetGUIStyle;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000045 RID: 69 RVA: 0x0000352C File Offset: 0x0000172C
		public static GUIStyle ButtonGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.buttonGUIStyle == null)
				{
					BehaviorDesignerUtility.InitButtonGUIStyle();
				}
				return BehaviorDesignerUtility.buttonGUIStyle;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00003544 File Offset: 0x00001744
		public static GUIStyle PlainTextureGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.plainTextureGUIStyle == null)
				{
					BehaviorDesignerUtility.InitPlainTextureGUIStyle();
				}
				return BehaviorDesignerUtility.plainTextureGUIStyle;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000047 RID: 71 RVA: 0x0000355C File Offset: 0x0000175C
		public static GUIStyle ArrowSeparatorGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.arrowSeparatorGUIStyle == null)
				{
					BehaviorDesignerUtility.InitArrowSeparatorGUIStyle();
				}
				return BehaviorDesignerUtility.arrowSeparatorGUIStyle;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00003574 File Offset: 0x00001774
		public static GUIStyle SelectedBackgroundGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.selectedBackgroundGUIStyle == null)
				{
					BehaviorDesignerUtility.InitSelectedBackgroundGUIStyle();
				}
				return BehaviorDesignerUtility.selectedBackgroundGUIStyle;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000358C File Offset: 0x0000178C
		public static GUIStyle ErrorListDarkBackground
		{
			get
			{
				if (BehaviorDesignerUtility.errorListDarkBackground == null)
				{
					BehaviorDesignerUtility.InitErrorListDarkBackground();
				}
				return BehaviorDesignerUtility.errorListDarkBackground;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000035A4 File Offset: 0x000017A4
		public static GUIStyle ErrorListLightBackground
		{
			get
			{
				if (BehaviorDesignerUtility.errorListLightBackground == null)
				{
					BehaviorDesignerUtility.InitErrorListLightBackground();
				}
				return BehaviorDesignerUtility.errorListLightBackground;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000035BC File Offset: 0x000017BC
		public static GUIStyle WelcomeScreenIntroGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.welcomeScreenIntroGUIStyle == null)
				{
					BehaviorDesignerUtility.InitWelcomeScreenIntroGUIStyle();
				}
				return BehaviorDesignerUtility.welcomeScreenIntroGUIStyle;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000035D4 File Offset: 0x000017D4
		public static GUIStyle WelcomeScreenTextHeaderGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle == null)
				{
					BehaviorDesignerUtility.InitWelcomeScreenTextHeaderGUIStyle();
				}
				return BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000035EC File Offset: 0x000017EC
		public static GUIStyle WelcomeScreenTextDescriptionGUIStyle
		{
			get
			{
				if (BehaviorDesignerUtility.welcomeScreenTextDescriptionGUIStyle == null)
				{
					BehaviorDesignerUtility.InitWelcomeScreenTextDescriptionGUIStyle();
				}
				return BehaviorDesignerUtility.welcomeScreenTextDescriptionGUIStyle;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003604 File Offset: 0x00001804
		public static Texture2D GetTaskBorderTexture(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskBorderTexture[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskBorderTexture(colorIndex);
			}
			return BehaviorDesignerUtility.taskBorderTexture[colorIndex];
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00003628 File Offset: 0x00001828
		public static Texture2D TaskBorderRunningTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskBorderRunningTexture == null)
				{
					BehaviorDesignerUtility.InitTaskBorderRunningTexture();
				}
				return BehaviorDesignerUtility.taskBorderRunningTexture;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00003644 File Offset: 0x00001844
		public static Texture2D TaskBorderIdentifyTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskBorderIdentifyTexture == null)
				{
					BehaviorDesignerUtility.InitTaskBorderIdentifyTexture();
				}
				return BehaviorDesignerUtility.taskBorderIdentifyTexture;
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003660 File Offset: 0x00001860
		public static Texture2D GetTaskConnectionTopTexture(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskConnectionTopTexture[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskConnectionTopTexture(colorIndex);
			}
			return BehaviorDesignerUtility.taskConnectionTopTexture[colorIndex];
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003684 File Offset: 0x00001884
		public static Texture2D GetTaskConnectionBottomTexture(int colorIndex)
		{
			if (BehaviorDesignerUtility.taskConnectionBottomTexture[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitTaskConnectionBottomTexture(colorIndex);
			}
			return BehaviorDesignerUtility.taskConnectionBottomTexture[colorIndex];
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000036A8 File Offset: 0x000018A8
		public static Texture2D TaskConnectionRunningTopTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskConnectionRunningTopTexture == null)
				{
					BehaviorDesignerUtility.InitTaskConnectionRunningTopTexture();
				}
				return BehaviorDesignerUtility.taskConnectionRunningTopTexture;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000036C4 File Offset: 0x000018C4
		public static Texture2D TaskConnectionRunningBottomTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskConnectionRunningBottomTexture == null)
				{
					BehaviorDesignerUtility.InitTaskConnectionRunningBottomTexture();
				}
				return BehaviorDesignerUtility.taskConnectionRunningBottomTexture;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000036E0 File Offset: 0x000018E0
		public static Texture2D TaskConnectionIdentifyTopTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskConnectionIdentifyTopTexture == null)
				{
					BehaviorDesignerUtility.InitTaskConnectionIdentifyTopTexture();
				}
				return BehaviorDesignerUtility.taskConnectionIdentifyTopTexture;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000036FC File Offset: 0x000018FC
		public static Texture2D TaskConnectionIdentifyBottomTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskConnectionIdentifyBottomTexture == null)
				{
					BehaviorDesignerUtility.InitTaskConnectionIdentifyBottomTexture();
				}
				return BehaviorDesignerUtility.taskConnectionIdentifyBottomTexture;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003718 File Offset: 0x00001918
		public static Texture2D TaskConnectionCollapsedTexture
		{
			get
			{
				if (BehaviorDesignerUtility.taskConnectionCollapsedTexture == null)
				{
					BehaviorDesignerUtility.InitTaskConnectionCollapsedTexture();
				}
				return BehaviorDesignerUtility.taskConnectionCollapsedTexture;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003734 File Offset: 0x00001934
		public static Texture2D ContentSeparatorTexture
		{
			get
			{
				if (BehaviorDesignerUtility.contentSeparatorTexture == null)
				{
					BehaviorDesignerUtility.InitContentSeparatorTexture();
				}
				return BehaviorDesignerUtility.contentSeparatorTexture;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00003750 File Offset: 0x00001950
		public static Texture2D DocTexture
		{
			get
			{
				if (BehaviorDesignerUtility.docTexture == null)
				{
					BehaviorDesignerUtility.InitDocTexture();
				}
				return BehaviorDesignerUtility.docTexture;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600005A RID: 90 RVA: 0x0000376C File Offset: 0x0000196C
		public static Texture2D GearTexture
		{
			get
			{
				if (BehaviorDesignerUtility.gearTexture == null)
				{
					BehaviorDesignerUtility.InitGearTexture();
				}
				return BehaviorDesignerUtility.gearTexture;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003788 File Offset: 0x00001988
		public static Texture2D ColorSelectorTexture(int colorIndex)
		{
			if (BehaviorDesignerUtility.colorSelectorTexture[colorIndex] == null)
			{
				BehaviorDesignerUtility.InitColorSelectorTexture(colorIndex);
			}
			return BehaviorDesignerUtility.colorSelectorTexture[colorIndex];
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000037AC File Offset: 0x000019AC
		public static Texture2D VariableButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableButtonTexture == null)
				{
					BehaviorDesignerUtility.InitVariableButtonTexture();
				}
				return BehaviorDesignerUtility.variableButtonTexture;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000037C8 File Offset: 0x000019C8
		public static Texture2D VariableButtonSelectedTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableButtonSelectedTexture == null)
				{
					BehaviorDesignerUtility.InitVariableButtonSelectedTexture();
				}
				return BehaviorDesignerUtility.variableButtonSelectedTexture;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000037E4 File Offset: 0x000019E4
		public static Texture2D VariableWatchButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableWatchButtonTexture == null)
				{
					BehaviorDesignerUtility.InitVariableWatchButtonTexture();
				}
				return BehaviorDesignerUtility.variableWatchButtonTexture;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003800 File Offset: 0x00001A00
		public static Texture2D VariableWatchButtonSelectedTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableWatchButtonSelectedTexture == null)
				{
					BehaviorDesignerUtility.InitVariableWatchButtonSelectedTexture();
				}
				return BehaviorDesignerUtility.variableWatchButtonSelectedTexture;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000381C File Offset: 0x00001A1C
		public static Texture2D ReferencedTexture
		{
			get
			{
				if (BehaviorDesignerUtility.referencedTexture == null)
				{
					BehaviorDesignerUtility.InitReferencedTexture();
				}
				return BehaviorDesignerUtility.referencedTexture;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00003838 File Offset: 0x00001A38
		public static Texture2D ConditionalAbortSelfTexture
		{
			get
			{
				if (BehaviorDesignerUtility.conditionalAbortSelfTexture == null)
				{
					BehaviorDesignerUtility.InitConditionalAbortSelfTexture();
				}
				return BehaviorDesignerUtility.conditionalAbortSelfTexture;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003854 File Offset: 0x00001A54
		public static Texture2D ConditionalAbortLowerPriorityTexture
		{
			get
			{
				if (BehaviorDesignerUtility.conditionalAbortLowerPriorityTexture == null)
				{
					BehaviorDesignerUtility.InitConditionalAbortLowerPriorityTexture();
				}
				return BehaviorDesignerUtility.conditionalAbortLowerPriorityTexture;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00003870 File Offset: 0x00001A70
		public static Texture2D ConditionalAbortBothTexture
		{
			get
			{
				if (BehaviorDesignerUtility.conditionalAbortBothTexture == null)
				{
					BehaviorDesignerUtility.InitConditionalAbortBothTexture();
				}
				return BehaviorDesignerUtility.conditionalAbortBothTexture;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000388C File Offset: 0x00001A8C
		public static Texture2D DeleteButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.deleteButtonTexture == null)
				{
					BehaviorDesignerUtility.InitDeleteButtonTexture();
				}
				return BehaviorDesignerUtility.deleteButtonTexture;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000038A8 File Offset: 0x00001AA8
		public static Texture2D VariableDeleteButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableDeleteButtonTexture == null)
				{
					BehaviorDesignerUtility.InitVariableDeleteButtonTexture();
				}
				return BehaviorDesignerUtility.variableDeleteButtonTexture;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000038C4 File Offset: 0x00001AC4
		public static Texture2D DownArrowButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.downArrowButtonTexture == null)
				{
					BehaviorDesignerUtility.InitDownArrowButtonTexture();
				}
				return BehaviorDesignerUtility.downArrowButtonTexture;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000038E0 File Offset: 0x00001AE0
		public static Texture2D UpArrowButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.upArrowButtonTexture == null)
				{
					BehaviorDesignerUtility.InitUpArrowButtonTexture();
				}
				return BehaviorDesignerUtility.upArrowButtonTexture;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000038FC File Offset: 0x00001AFC
		public static Texture2D VariableMapButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.variableMapButtonTexture == null)
				{
					BehaviorDesignerUtility.InitVariableMapButtonTexture();
				}
				return BehaviorDesignerUtility.variableMapButtonTexture;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00003918 File Offset: 0x00001B18
		public static Texture2D IdentifyButtonTexture
		{
			get
			{
				if (BehaviorDesignerUtility.identifyButtonTexture == null)
				{
					BehaviorDesignerUtility.InitIdentifyButtonTexture();
				}
				return BehaviorDesignerUtility.identifyButtonTexture;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003934 File Offset: 0x00001B34
		public static Texture2D BreakpointTexture
		{
			get
			{
				if (BehaviorDesignerUtility.breakpointTexture == null)
				{
					BehaviorDesignerUtility.InitBreakpointTexture();
				}
				return BehaviorDesignerUtility.breakpointTexture;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00003950 File Offset: 0x00001B50
		public static Texture2D ErrorIconTexture
		{
			get
			{
				if (BehaviorDesignerUtility.errorIconTexture == null)
				{
					BehaviorDesignerUtility.InitErrorIconTexture();
				}
				return BehaviorDesignerUtility.errorIconTexture;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000396C File Offset: 0x00001B6C
		public static Texture2D SmallErrorIconTexture
		{
			get
			{
				if (BehaviorDesignerUtility.smallErrorIconTexture == null)
				{
					BehaviorDesignerUtility.InitSmallErrorIconTexture();
				}
				return BehaviorDesignerUtility.smallErrorIconTexture;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003988 File Offset: 0x00001B88
		public static Texture2D EnableTaskTexture
		{
			get
			{
				if (BehaviorDesignerUtility.enableTaskTexture == null)
				{
					BehaviorDesignerUtility.InitEnableTaskTexture();
				}
				return BehaviorDesignerUtility.enableTaskTexture;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000039A4 File Offset: 0x00001BA4
		public static Texture2D DisableTaskTexture
		{
			get
			{
				if (BehaviorDesignerUtility.disableTaskTexture == null)
				{
					BehaviorDesignerUtility.InitDisableTaskTexture();
				}
				return BehaviorDesignerUtility.disableTaskTexture;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000039C0 File Offset: 0x00001BC0
		public static Texture2D ExpandTaskTexture
		{
			get
			{
				if (BehaviorDesignerUtility.expandTaskTexture == null)
				{
					BehaviorDesignerUtility.InitExpandTaskTexture();
				}
				return BehaviorDesignerUtility.expandTaskTexture;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000039DC File Offset: 0x00001BDC
		public static Texture2D CollapseTaskTexture
		{
			get
			{
				if (BehaviorDesignerUtility.collapseTaskTexture == null)
				{
					BehaviorDesignerUtility.InitCollapseTaskTexture();
				}
				return BehaviorDesignerUtility.collapseTaskTexture;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000039F8 File Offset: 0x00001BF8
		public static Texture2D ExecutionSuccessTexture
		{
			get
			{
				if (BehaviorDesignerUtility.executionSuccessTexture == null)
				{
					BehaviorDesignerUtility.InitExecutionSuccessTexture();
				}
				return BehaviorDesignerUtility.executionSuccessTexture;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003A14 File Offset: 0x00001C14
		public static Texture2D ExecutionFailureTexture
		{
			get
			{
				if (BehaviorDesignerUtility.executionFailureTexture == null)
				{
					BehaviorDesignerUtility.InitExecutionFailureTexture();
				}
				return BehaviorDesignerUtility.executionFailureTexture;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00003A30 File Offset: 0x00001C30
		public static Texture2D ExecutionSuccessRepeatTexture
		{
			get
			{
				if (BehaviorDesignerUtility.executionSuccessRepeatTexture == null)
				{
					BehaviorDesignerUtility.InitExecutionSuccessRepeatTexture();
				}
				return BehaviorDesignerUtility.executionSuccessRepeatTexture;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003A4C File Offset: 0x00001C4C
		public static Texture2D ExecutionFailureRepeatTexture
		{
			get
			{
				if (BehaviorDesignerUtility.executionFailureRepeatTexture == null)
				{
					BehaviorDesignerUtility.InitExecutionFailureRepeatTexture();
				}
				return BehaviorDesignerUtility.executionFailureRepeatTexture;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00003A68 File Offset: 0x00001C68
		public static Texture2D HistoryBackwardTexture
		{
			get
			{
				if (BehaviorDesignerUtility.historyBackwardTexture == null)
				{
					BehaviorDesignerUtility.InitHistoryBackwardTexture();
				}
				return BehaviorDesignerUtility.historyBackwardTexture;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003A84 File Offset: 0x00001C84
		public static Texture2D HistoryForwardTexture
		{
			get
			{
				if (BehaviorDesignerUtility.historyForwardTexture == null)
				{
					BehaviorDesignerUtility.InitHistoryForwardTexture();
				}
				return BehaviorDesignerUtility.historyForwardTexture;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public static Texture2D PlayTexture
		{
			get
			{
				if (BehaviorDesignerUtility.playTexture == null)
				{
					BehaviorDesignerUtility.InitPlayTexture();
				}
				return BehaviorDesignerUtility.playTexture;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003ABC File Offset: 0x00001CBC
		public static Texture2D PauseTexture
		{
			get
			{
				if (BehaviorDesignerUtility.pauseTexture == null)
				{
					BehaviorDesignerUtility.InitPauseTexture();
				}
				return BehaviorDesignerUtility.pauseTexture;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00003AD8 File Offset: 0x00001CD8
		public static Texture2D StepTexture
		{
			get
			{
				if (BehaviorDesignerUtility.stepTexture == null)
				{
					BehaviorDesignerUtility.InitStepTexture();
				}
				return BehaviorDesignerUtility.stepTexture;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003AF4 File Offset: 0x00001CF4
		public static Texture2D ScreenshotBackgroundTexture
		{
			get
			{
				if (BehaviorDesignerUtility.screenshotBackgroundTexture == null)
				{
					BehaviorDesignerUtility.InitScreenshotBackgroundTexture();
				}
				return BehaviorDesignerUtility.screenshotBackgroundTexture;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00003B10 File Offset: 0x00001D10
		public static string SplitCamelCase(string s)
		{
			if (s.Equals(string.Empty))
			{
				return s;
			}
			if (BehaviorDesignerUtility.camelCaseSplit.ContainsKey(s))
			{
				return BehaviorDesignerUtility.camelCaseSplit[s];
			}
			string key = s;
			s = s.Replace("_uScript", "uScript");
			s = s.Replace("_PlayMaker", "PlayMaker");
			if (s.Length > 2 && s.Substring(0, 2).CompareTo("m_") == 0)
			{
				s = s.Substring(2, s.Length - 2);
			}
			s = BehaviorDesignerUtility.camelCaseRegex.Replace(s, " ");
			s = s.Replace("_", " ");
			s = s.Replace("u Script", " uScript");
			s = s.Replace("Play Maker", "PlayMaker");
			s = (char.ToUpper(s[0]) + s.Substring(1)).Trim();
			BehaviorDesignerUtility.camelCaseSplit.Add(key, s);
			return s;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003C20 File Offset: 0x00001E20
		public static bool HasAttribute(FieldInfo field, Type attributeType)
		{
			Dictionary<FieldInfo, bool> dictionary = null;
			if (BehaviorDesignerUtility.attributeFieldCache.ContainsKey(attributeType))
			{
				dictionary = BehaviorDesignerUtility.attributeFieldCache[attributeType];
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<FieldInfo, bool>();
			}
			if (dictionary.ContainsKey(field))
			{
				return dictionary[field];
			}
			bool flag = field.GetCustomAttributes(attributeType, false).Length > 0;
			dictionary.Add(field, flag);
			if (!BehaviorDesignerUtility.attributeFieldCache.ContainsKey(attributeType))
			{
				BehaviorDesignerUtility.attributeFieldCache.Add(attributeType, dictionary);
			}
			return flag;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003CA0 File Offset: 0x00001EA0
		public static List<Task> GetAllTasks(BehaviorSource behaviorSource)
		{
			List<Task> result = new List<Task>();
			if (behaviorSource.RootTask != null)
			{
				BehaviorDesignerUtility.GetAllTasks(behaviorSource.RootTask, ref result);
			}
			if (behaviorSource.DetachedTasks != null)
			{
				for (int i = 0; i < behaviorSource.DetachedTasks.Count; i++)
				{
					BehaviorDesignerUtility.GetAllTasks(behaviorSource.DetachedTasks[i], ref result);
				}
			}
			return result;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003D08 File Offset: 0x00001F08
		private static void GetAllTasks(Task task, ref List<Task> taskList)
		{
			taskList.Add(task);
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					BehaviorDesignerUtility.GetAllTasks(parentTask.Children[i], ref taskList);
				}
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003D64 File Offset: 0x00001F64
		public static bool AnyNullTasks(BehaviorSource behaviorSource)
		{
			if (behaviorSource.RootTask != null && BehaviorDesignerUtility.AnyNullTasks(behaviorSource.RootTask))
			{
				return true;
			}
			if (behaviorSource.DetachedTasks != null)
			{
				for (int i = 0; i < behaviorSource.DetachedTasks.Count; i++)
				{
					if (BehaviorDesignerUtility.AnyNullTasks(behaviorSource.DetachedTasks[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003DD0 File Offset: 0x00001FD0
		private static bool AnyNullTasks(Task task)
		{
			if (task == null)
			{
				return true;
			}
			ParentTask parentTask;
			if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					if (BehaviorDesignerUtility.AnyNullTasks(parentTask.Children[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003E34 File Offset: 0x00002034
		public static bool HasRootTask(string serialization)
		{
			if (string.IsNullOrEmpty(serialization))
			{
				return false;
			}
			Dictionary<string, object> dictionary = MiniJSON.Deserialize(serialization) as Dictionary<string, object>;
			return dictionary != null && dictionary.ContainsKey("RootTask");
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003E74 File Offset: 0x00002074
		public static string GetEditorBaseDirectory(Object obj = null)
		{
			string codeBase = Assembly.Load("BehaviorDesignerEditor").CodeBase;
			string text = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);
			return Path.GetDirectoryName(text.Substring(Application.dataPath.Length - 6));
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003EB4 File Offset: 0x000020B4
		public static Texture2D LoadTexture(string imageName, bool useSkinColor = true, Object obj = null)
		{
			if (BehaviorDesignerUtility.textureCache.ContainsKey(imageName))
			{
				return BehaviorDesignerUtility.textureCache[imageName];
			}
			Texture2D texture2D = null;
			string name = string.Format("{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
            Stream manifestResourceStream = Assembly.Load("BehaviorDesignerEditor").GetManifestResourceStream("BehaviorDesigner.Editor." + name);
            if (manifestResourceStream == null)
			{
				name = string.Format("BehaviorDesignerEditor.Resources.{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			}
			if (manifestResourceStream != null)
			{
				texture2D = new Texture2D(0, 0, (TextureFormat)4, false, true);
				texture2D.LoadImage(BehaviorDesignerUtility.ReadToEnd(manifestResourceStream));
				manifestResourceStream.Close();
			}
			texture2D.hideFlags = (HideFlags)61;
			BehaviorDesignerUtility.textureCache.Add(imageName, texture2D);
			return texture2D;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003FA0 File Offset: 0x000021A0
		private static Texture2D LoadTaskTexture(string imageName, bool useSkinColor = true, ScriptableObject obj = null)
		{
			if (BehaviorDesignerUtility.textureCache.ContainsKey(imageName))
			{
				return BehaviorDesignerUtility.textureCache[imageName];
			}
			Texture2D texture2D = null;
			string name = string.Format("{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
            Stream manifestResourceStream = Assembly.Load("BehaviorDesignerEditor").GetManifestResourceStream("BehaviorDesigner.Editor." + name);
            if (manifestResourceStream == null)
			{
				name = string.Format("BehaviorDesignerEditor.Resources.{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			}
			if (manifestResourceStream != null)
			{
				texture2D = new Texture2D(0, 0, (TextureFormat)4, false, true);
				texture2D.LoadImage(BehaviorDesignerUtility.ReadToEnd(manifestResourceStream));
				manifestResourceStream.Close();
			}
			if (texture2D == null)
			{
				Debug.Log(string.Format("{0}/Images/Task Backgrounds/{1}{2}", BehaviorDesignerUtility.GetEditorBaseDirectory(obj), (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName));
			}
			texture2D.hideFlags = (HideFlags)61;
			BehaviorDesignerUtility.textureCache.Add(imageName, texture2D);
			return texture2D;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000040D8 File Offset: 0x000022D8
		public static Texture2D LoadIcon(string iconName, ScriptableObject obj = null)
		{
			if (BehaviorDesignerUtility.iconCache.ContainsKey(iconName))
			{
				return BehaviorDesignerUtility.iconCache[iconName];
			}
			Texture2D texture2D = null;
			string name = iconName.Replace("{SkinColor}", (!EditorGUIUtility.isProSkin) ? "Light" : "Dark");
			Stream manifestResourceStream = Assembly.Load("BehaviorDesignerEditor").GetManifestResourceStream("BehaviorDesigner.Editor." + name);
			if (manifestResourceStream == null)
			{
				name = string.Format("BehaviorDesignerEditor.Resources.{0}", iconName.Replace("{SkinColor}", (!EditorGUIUtility.isProSkin) ? "Light" : "Dark"));
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			}
			if (manifestResourceStream != null)
			{
				texture2D = new Texture2D(0, 0, (TextureFormat)4, false, true);
				texture2D.LoadImage(BehaviorDesignerUtility.ReadToEnd(manifestResourceStream));
				manifestResourceStream.Close();
			}
			if (texture2D == null)
			{
				texture2D = (AssetDatabase.LoadAssetAtPath(iconName.Replace("{SkinColor}", (!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), typeof(Texture2D)) as Texture2D);
			}
			if (texture2D != null)
			{
				texture2D.hideFlags = (HideFlags)61;
			}
			BehaviorDesignerUtility.iconCache.Add(iconName, texture2D);
			return texture2D;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004200 File Offset: 0x00002400
		private static byte[] ReadToEnd(Stream stream)
		{
			byte[] array = new byte[16384];
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = stream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000427C File Offset: 0x0000247C
		public static void DrawContentSeperator(int yOffset)
		{
			BehaviorDesignerUtility.DrawContentSeperator(yOffset, 0);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004288 File Offset: 0x00002488
		public static void DrawContentSeperator(int yOffset, int widthExtension)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.x = -5f;
			lastRect.y += lastRect.height + (float)yOffset;
			lastRect.height = 2f;
			lastRect.width += (float)(10 + widthExtension);
			GUI.DrawTexture(lastRect, BehaviorDesignerUtility.ContentSeparatorTexture);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000042EC File Offset: 0x000024EC
		public static float RoundToNearest(float num, float baseNum)
		{
			return (float)((int)Math.Round((double)(num / baseNum), MidpointRounding.AwayFromZero)) * baseNum;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000042FC File Offset: 0x000024FC
		private static void InitGraphStatusGUIStyle()
		{
			BehaviorDesignerUtility.graphStatusGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.graphStatusGUIStyle.alignment = (TextAnchor)3;
			BehaviorDesignerUtility.graphStatusGUIStyle.fontSize = 20;
			BehaviorDesignerUtility.graphStatusGUIStyle.fontStyle = (FontStyle)1;
			if (EditorGUIUtility.isProSkin)
			{
				BehaviorDesignerUtility.graphStatusGUIStyle.normal.textColor = new Color(0.7058f, 0.7058f, 0.7058f);
			}
			else
			{
				BehaviorDesignerUtility.graphStatusGUIStyle.normal.textColor = new Color(0.8058f, 0.8058f, 0.8058f);
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004394 File Offset: 0x00002594
		private static void InitTaskFoldoutGUIStyle()
		{
			BehaviorDesignerUtility.taskFoldoutGUIStyle = new GUIStyle(EditorStyles.foldout);
			BehaviorDesignerUtility.taskFoldoutGUIStyle.alignment = (TextAnchor)3;
			BehaviorDesignerUtility.taskFoldoutGUIStyle.fontSize = 13;
			BehaviorDesignerUtility.taskFoldoutGUIStyle.fontStyle = (FontStyle)1;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000043C8 File Offset: 0x000025C8
		private static void InitTaskTitleGUIStyle()
		{
			BehaviorDesignerUtility.taskTitleGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.taskTitleGUIStyle.alignment = (TextAnchor)1;
			BehaviorDesignerUtility.taskTitleGUIStyle.fontSize = 12;
			BehaviorDesignerUtility.taskTitleGUIStyle.fontStyle = 0;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000440C File Offset: 0x0000260C
		private static void InitTaskGUIStyle(int colorIndex)
		{
			BehaviorDesignerUtility.taskGUIStyle[colorIndex] = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("Task" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null), new RectOffset(5, 3, 3, 5));
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000444C File Offset: 0x0000264C
		private static void InitTaskCompactGUIStyle(int colorIndex)
		{
			BehaviorDesignerUtility.taskCompactGUIStyle[colorIndex] = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskCompact" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null), new RectOffset(5, 4, 4, 5));
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000448C File Offset: 0x0000268C
		private static void InitTaskSelectedGUIStyle(int colorIndex)
		{
			BehaviorDesignerUtility.taskSelectedGUIStyle[colorIndex] = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskSelected" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000044CC File Offset: 0x000026CC
		private static void InitTaskSelectedCompactGUIStyle(int colorIndex)
		{
			BehaviorDesignerUtility.taskSelectedCompactGUIStyle[colorIndex] = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskSelectedCompact" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000450C File Offset: 0x0000270C
		private static string ColorIndexToColorString(int index)
		{
			if (index == 0)
			{
				return string.Empty;
			}
			if (index == 1)
			{
				return "Red";
			}
			if (index == 2)
			{
				return "Pink";
			}
			if (index == 3)
			{
				return "Brown";
			}
			if (index == 4)
			{
				return "RedOrange";
			}
			if (index == 5)
			{
				return "Turquoise";
			}
			if (index == 6)
			{
				return "Cyan";
			}
			if (index == 7)
			{
				return "Blue";
			}
			if (index == 8)
			{
				return "Purple";
			}
			return string.Empty;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004594 File Offset: 0x00002794
		private static void InitTaskRunningGUIStyle()
		{
			BehaviorDesignerUtility.taskRunningGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskRunning.png", true, null), new RectOffset(5, 3, 3, 5));
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000045B8 File Offset: 0x000027B8
		private static void InitTaskRunningCompactGUIStyle()
		{
			BehaviorDesignerUtility.taskRunningCompactGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskRunningCompact.png", true, null), new RectOffset(5, 4, 4, 5));
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000045DC File Offset: 0x000027DC
		private static void InitTaskRunningSelectedGUIStyle()
		{
			BehaviorDesignerUtility.taskRunningSelectedGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskRunningSelected.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004600 File Offset: 0x00002800
		private static void InitTaskRunningSelectedCompactGUIStyle()
		{
			BehaviorDesignerUtility.taskRunningSelectedCompactGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskRunningSelectedCompact.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004624 File Offset: 0x00002824
		private static void InitTaskIdentifyGUIStyle()
		{
			BehaviorDesignerUtility.taskIdentifyGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskIdentify.png", true, null), new RectOffset(5, 3, 3, 5));
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004648 File Offset: 0x00002848
		private static void InitTaskIdentifyCompactGUIStyle()
		{
			BehaviorDesignerUtility.taskIdentifyCompactGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskIdentifyCompact.png", true, null), new RectOffset(5, 4, 4, 5));
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000466C File Offset: 0x0000286C
		private static void InitTaskIdentifySelectedGUIStyle()
		{
			BehaviorDesignerUtility.taskIdentifySelectedGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskIdentifySelected.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004690 File Offset: 0x00002890
		private static void InitTaskIdentifySelectedCompactGUIStyle()
		{
			BehaviorDesignerUtility.taskIdentifySelectedCompactGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskIdentifySelectedCompact.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000046B4 File Offset: 0x000028B4
		private static void InitTaskHighlightGUIStyle()
		{
			BehaviorDesignerUtility.taskHighlightGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskHighlight.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000046D8 File Offset: 0x000028D8
		private static void InitTaskHighlightCompactGUIStyle()
		{
			BehaviorDesignerUtility.taskHighlightCompactGUIStyle = BehaviorDesignerUtility.InitTaskGUIStyle(BehaviorDesignerUtility.LoadTaskTexture("TaskHighlightCompact.png", true, null), new RectOffset(5, 4, 4, 4));
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000046FC File Offset: 0x000028FC
		private static GUIStyle InitTaskGUIStyle(Texture2D texture, RectOffset overflow)
		{
			return new GUIStyle(GUI.skin.box)
			{
				border = new RectOffset(10, 10, 10, 10),
				overflow = overflow,
				normal = 
				{
					background = texture,
                    textColor = Color.white
                },
				active = 
				{
					background = texture,
                    textColor = Color.white
                },
				hover = 
				{
					background = texture,
                    textColor = Color.white
                },
				focused = 
				{
					background = texture,
                    textColor = Color.white
                },
				stretchHeight = true,
				stretchWidth = true
			};
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000047B4 File Offset: 0x000029B4
		private static void InitTaskCommentGUIStyle()
		{
			BehaviorDesignerUtility.taskCommentGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.taskCommentGUIStyle.alignment = (TextAnchor)1;
			BehaviorDesignerUtility.taskCommentGUIStyle.fontSize = 12;
			BehaviorDesignerUtility.taskCommentGUIStyle.fontStyle = 0;
			BehaviorDesignerUtility.taskCommentGUIStyle.wordWrap = true;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004804 File Offset: 0x00002A04
		private static void InitTaskCommentLeftAlignGUIStyle()
		{
			BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle.alignment = 0;
			BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle.fontSize = 12;
			BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle.fontStyle = 0;
			BehaviorDesignerUtility.taskCommentLeftAlignGUIStyle.wordWrap = false;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004854 File Offset: 0x00002A54
		private static void InitTaskCommentRightAlignGUIStyle()
		{
			BehaviorDesignerUtility.taskCommentRightAlignGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.taskCommentRightAlignGUIStyle.alignment = (TextAnchor)2;
			BehaviorDesignerUtility.taskCommentRightAlignGUIStyle.fontSize = 12;
			BehaviorDesignerUtility.taskCommentRightAlignGUIStyle.fontStyle = 0;
			BehaviorDesignerUtility.taskCommentRightAlignGUIStyle.wordWrap = false;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000048A4 File Offset: 0x00002AA4
		private static void InitTaskDescriptionGUIStyle()
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false, true);
			if (EditorGUIUtility.isProSkin)
			{
				texture2D.SetPixel(1, 1, new Color(0.1647f, 0.1647f, 0.1647f));
			}
			else
			{
				texture2D.SetPixel(1, 1, new Color(0.75f, 0.75f, 0.75f));
			}
			texture2D.hideFlags = (HideFlags)61;
			texture2D.Apply();
			BehaviorDesignerUtility.taskDescriptionGUIStyle = new GUIStyle(GUI.skin.box);
			BehaviorDesignerUtility.taskDescriptionGUIStyle.normal.background = texture2D;
			BehaviorDesignerUtility.taskDescriptionGUIStyle.active.background = texture2D;
			BehaviorDesignerUtility.taskDescriptionGUIStyle.hover.background = texture2D;
			BehaviorDesignerUtility.taskDescriptionGUIStyle.focused.background = texture2D;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004968 File Offset: 0x00002B68
		private static void InitGraphBackgroundGUIStyle()
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false, true);
			if (EditorGUIUtility.isProSkin)
			{
				texture2D.SetPixel(1, 1, new Color(0.1647f, 0.1647f, 0.1647f));
			}
			else
			{
				texture2D.SetPixel(1, 1, new Color(0.3647f, 0.3647f, 0.3647f));
			}
			texture2D.hideFlags = (HideFlags)61;
			texture2D.Apply();
			BehaviorDesignerUtility.graphBackgroundGUIStyle = new GUIStyle(GUI.skin.box);
			BehaviorDesignerUtility.graphBackgroundGUIStyle.normal.background = texture2D;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.active.background = texture2D;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.hover.background = texture2D;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.focused.background = texture2D;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.normal.textColor = Color.white;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.active.textColor = Color.white;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.hover.textColor = Color.white;
			BehaviorDesignerUtility.graphBackgroundGUIStyle.focused.textColor = Color.white;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004A7C File Offset: 0x00002C7C
		private static void InitSelectionGUIStyle()
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false, true);
			Color color = (!EditorGUIUtility.isProSkin) ? new Color(0.243f, 0.5686f, 0.839f, 0.5f) : new Color(0.188f, 0.4588f, 0.6862f, 0.5f);
			texture2D.SetPixel(1, 1, color);
			texture2D.hideFlags = (HideFlags)61;
			texture2D.Apply();
			BehaviorDesignerUtility.selectionGUIStyle = new GUIStyle(GUI.skin.box);
			BehaviorDesignerUtility.selectionGUIStyle.normal.background = texture2D;
			BehaviorDesignerUtility.selectionGUIStyle.active.background = texture2D;
			BehaviorDesignerUtility.selectionGUIStyle.hover.background = texture2D;
			BehaviorDesignerUtility.selectionGUIStyle.focused.background = texture2D;
			BehaviorDesignerUtility.selectionGUIStyle.normal.textColor = Color.white;
			BehaviorDesignerUtility.selectionGUIStyle.active.textColor = Color.white;
			BehaviorDesignerUtility.selectionGUIStyle.hover.textColor = Color.white;
			BehaviorDesignerUtility.selectionGUIStyle.focused.textColor = Color.white;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004B94 File Offset: 0x00002D94
		private static void InitSharedVariableToolbarPopup()
		{
			BehaviorDesignerUtility.sharedVariableToolbarPopup = new GUIStyle(EditorStyles.toolbarPopup);
			BehaviorDesignerUtility.sharedVariableToolbarPopup.margin = new RectOffset(4, 4, 0, 0);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004BC4 File Offset: 0x00002DC4
		private static void InitLabelWrapGUIStyle()
		{
			BehaviorDesignerUtility.labelWrapGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.labelWrapGUIStyle.wordWrap = true;
			BehaviorDesignerUtility.labelWrapGUIStyle.alignment = (TextAnchor)4;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004BFC File Offset: 0x00002DFC
		private static void InitToolbarButtonLeftAlignGUIStyle()
		{
			BehaviorDesignerUtility.tolbarButtonLeftAlignGUIStyle = new GUIStyle(EditorStyles.toolbarButton);
			BehaviorDesignerUtility.tolbarButtonLeftAlignGUIStyle.alignment = (TextAnchor)3;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004C18 File Offset: 0x00002E18
		private static void InitToolbarLabelGUIStyle()
		{
			BehaviorDesignerUtility.toolbarLabelGUIStyle = new GUIStyle(EditorStyles.label);
			BehaviorDesignerUtility.toolbarLabelGUIStyle.normal.textColor = ((!EditorGUIUtility.isProSkin) ? new Color(0f, 0.5f, 0f) : new Color(0f, 0.7f, 0f));
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004C7C File Offset: 0x00002E7C
		private static void InitTaskInspectorCommentGUIStyle()
		{
			BehaviorDesignerUtility.taskInspectorCommentGUIStyle = new GUIStyle(GUI.skin.textArea);
			BehaviorDesignerUtility.taskInspectorCommentGUIStyle.wordWrap = true;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004CA0 File Offset: 0x00002EA0
		private static void InitTaskInspectorGUIStyle()
		{
			BehaviorDesignerUtility.taskInspectorGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.taskInspectorGUIStyle.alignment = (TextAnchor)3;
			BehaviorDesignerUtility.taskInspectorGUIStyle.fontSize = 11;
			BehaviorDesignerUtility.taskInspectorGUIStyle.fontStyle = (FontStyle)0;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004CE4 File Offset: 0x00002EE4
		private static void InitToolbarButtonSelectionGUIStyle()
		{
			BehaviorDesignerUtility.toolbarButtonSelectionGUIStyle = new GUIStyle(EditorStyles.toolbarButton);
			BehaviorDesignerUtility.toolbarButtonSelectionGUIStyle.normal.background = BehaviorDesignerUtility.toolbarButtonSelectionGUIStyle.active.background;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004D20 File Offset: 0x00002F20
		private static void InitPreferencesPaneGUIStyle()
		{
			BehaviorDesignerUtility.preferencesPaneGUIStyle = new GUIStyle(GUI.skin.box);
			BehaviorDesignerUtility.preferencesPaneGUIStyle.normal.background = EditorStyles.toolbarButton.normal.background;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004D60 File Offset: 0x00002F60
		private static void InitPropertyBoxGUIStyle()
		{
			BehaviorDesignerUtility.propertyBoxGUIStyle = new GUIStyle();
			BehaviorDesignerUtility.propertyBoxGUIStyle.padding = new RectOffset(2, 2, 0, 0);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004D80 File Offset: 0x00002F80
		private static void InitPlainButtonGUIStyle()
		{
			BehaviorDesignerUtility.plainButtonGUIStyle = new GUIStyle(GUI.skin.button);
			BehaviorDesignerUtility.plainButtonGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.plainButtonGUIStyle.margin = new RectOffset(0, 0, 2, 2);
			BehaviorDesignerUtility.plainButtonGUIStyle.padding = new RectOffset(0, 0, 1, 0);
			BehaviorDesignerUtility.plainButtonGUIStyle.normal.background = null;
			BehaviorDesignerUtility.plainButtonGUIStyle.active.background = null;
			BehaviorDesignerUtility.plainButtonGUIStyle.hover.background = null;
			BehaviorDesignerUtility.plainButtonGUIStyle.focused.background = null;
			BehaviorDesignerUtility.plainButtonGUIStyle.normal.textColor = Color.white;
			BehaviorDesignerUtility.plainButtonGUIStyle.active.textColor = Color.white;
			BehaviorDesignerUtility.plainButtonGUIStyle.hover.textColor = Color.white;
			BehaviorDesignerUtility.plainButtonGUIStyle.focused.textColor = Color.white;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004E6C File Offset: 0x0000306C
		private static void InitTransparentButtonGUIStyle()
		{
			BehaviorDesignerUtility.transparentButtonGUIStyle = new GUIStyle(GUI.skin.button);
			BehaviorDesignerUtility.transparentButtonGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.transparentButtonGUIStyle.margin = new RectOffset(4, 4, 2, 2);
			BehaviorDesignerUtility.transparentButtonGUIStyle.padding = new RectOffset(2, 2, 1, 0);
			BehaviorDesignerUtility.transparentButtonGUIStyle.normal.background = null;
			BehaviorDesignerUtility.transparentButtonGUIStyle.active.background = null;
			BehaviorDesignerUtility.transparentButtonGUIStyle.hover.background = null;
			BehaviorDesignerUtility.transparentButtonGUIStyle.focused.background = null;
			BehaviorDesignerUtility.transparentButtonGUIStyle.normal.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonGUIStyle.active.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonGUIStyle.hover.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonGUIStyle.focused.textColor = Color.white;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004F58 File Offset: 0x00003158
		private static void InitTransparentButtonOffsetGUIStyle()
		{
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle = new GUIStyle(GUI.skin.button);
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.margin = new RectOffset(4, 4, 4, 2);
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.padding = new RectOffset(2, 2, 1, 0);
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.normal.background = null;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.active.background = null;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.hover.background = null;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.focused.background = null;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.normal.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.active.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.hover.textColor = Color.white;
			BehaviorDesignerUtility.transparentButtonOffsetGUIStyle.focused.textColor = Color.white;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005044 File Offset: 0x00003244
		private static void InitButtonGUIStyle()
		{
			BehaviorDesignerUtility.buttonGUIStyle = new GUIStyle(GUI.skin.button);
			BehaviorDesignerUtility.buttonGUIStyle.margin = new RectOffset(0, 0, 2, 2);
			BehaviorDesignerUtility.buttonGUIStyle.padding = new RectOffset(0, 0, 1, 1);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000508C File Offset: 0x0000328C
		private static void InitPlainTextureGUIStyle()
		{
			BehaviorDesignerUtility.plainTextureGUIStyle = new GUIStyle();
			BehaviorDesignerUtility.plainTextureGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.plainTextureGUIStyle.margin = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.plainTextureGUIStyle.padding = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.plainTextureGUIStyle.normal.background = null;
			BehaviorDesignerUtility.plainTextureGUIStyle.active.background = null;
			BehaviorDesignerUtility.plainTextureGUIStyle.hover.background = null;
			BehaviorDesignerUtility.plainTextureGUIStyle.focused.background = null;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000511C File Offset: 0x0000331C
		private static void InitArrowSeparatorGUIStyle()
		{
			BehaviorDesignerUtility.arrowSeparatorGUIStyle = new GUIStyle();
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.margin = new RectOffset(0, 0, 3, 0);
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.padding = new RectOffset(0, 0, 0, 0);
			Texture2D background = BehaviorDesignerUtility.LoadTexture("ArrowSeparator.png", true, null);
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.normal.background = background;
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.active.background = background;
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.hover.background = background;
			BehaviorDesignerUtility.arrowSeparatorGUIStyle.focused.background = background;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000051BC File Offset: 0x000033BC
		private static void InitSelectedBackgroundGUIStyle()
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false, true);
			Color color = (!EditorGUIUtility.isProSkin) ? new Color(0.243f, 0.5686f, 0.839f, 0.5f) : new Color(0.188f, 0.4588f, 0.6862f, 0.5f);
			texture2D.SetPixel(1, 1, color);
			texture2D.hideFlags = (HideFlags)61;
			texture2D.Apply();
			BehaviorDesignerUtility.selectedBackgroundGUIStyle = new GUIStyle();
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.border = new RectOffset(0, 0, 0, 0);
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.margin = new RectOffset(0, 0, -2, 2);
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.normal.background = texture2D;
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.active.background = texture2D;
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.hover.background = texture2D;
			BehaviorDesignerUtility.selectedBackgroundGUIStyle.focused.background = texture2D;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000052A0 File Offset: 0x000034A0
		private static void InitErrorListDarkBackground()
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false, true);
			Color color = (!EditorGUIUtility.isProSkin) ? new Color(0.706f, 0.706f, 0.706f) : new Color(0.2f, 0.2f, 0.2f, 1f);
			texture2D.SetPixel(1, 1, color);
			texture2D.hideFlags = (HideFlags)61;
			texture2D.Apply();
			BehaviorDesignerUtility.errorListDarkBackground = new GUIStyle();
			BehaviorDesignerUtility.errorListDarkBackground.padding = new RectOffset(2, 0, 2, 0);
			BehaviorDesignerUtility.errorListDarkBackground.normal.background = texture2D;
			BehaviorDesignerUtility.errorListDarkBackground.active.background = texture2D;
			BehaviorDesignerUtility.errorListDarkBackground.hover.background = texture2D;
			BehaviorDesignerUtility.errorListDarkBackground.focused.background = texture2D;
			BehaviorDesignerUtility.errorListDarkBackground.normal.textColor = ((!EditorGUIUtility.isProSkin) ? new Color(0.206f, 0.206f, 0.206f) : new Color(0.706f, 0.706f, 0.706f));
			BehaviorDesignerUtility.errorListDarkBackground.alignment = (TextAnchor)0;
			BehaviorDesignerUtility.errorListDarkBackground.wordWrap = false;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000053C8 File Offset: 0x000035C8
		private static void InitErrorListLightBackground()
		{
			BehaviorDesignerUtility.errorListLightBackground = new GUIStyle();
			BehaviorDesignerUtility.errorListLightBackground.padding = new RectOffset(2, 0, 2, 0);
			BehaviorDesignerUtility.errorListLightBackground.normal.textColor = ((!EditorGUIUtility.isProSkin) ? new Color(0.106f, 0.106f, 0.106f) : new Color(0.706f, 0.706f, 0.706f));
			BehaviorDesignerUtility.errorListLightBackground.alignment = (TextAnchor)0;
			BehaviorDesignerUtility.errorListLightBackground.wordWrap = false;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005450 File Offset: 0x00003650
		private static void InitWelcomeScreenIntroGUIStyle()
		{
			BehaviorDesignerUtility.welcomeScreenIntroGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.welcomeScreenIntroGUIStyle.fontSize = 16;
			BehaviorDesignerUtility.welcomeScreenIntroGUIStyle.fontStyle = (FontStyle)1;
			BehaviorDesignerUtility.welcomeScreenIntroGUIStyle.normal.textColor = new Color(0.706f, 0.706f, 0.706f);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000054AC File Offset: 0x000036AC
		private static void InitWelcomeScreenTextHeaderGUIStyle()
		{
			BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle.alignment = (TextAnchor)3;
			BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle.fontSize = 14;
			BehaviorDesignerUtility.welcomeScreenTextHeaderGUIStyle.fontStyle = (FontStyle)1;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000054F0 File Offset: 0x000036F0
		private static void InitWelcomeScreenTextDescriptionGUIStyle()
		{
			BehaviorDesignerUtility.welcomeScreenTextDescriptionGUIStyle = new GUIStyle(GUI.skin.label);
			BehaviorDesignerUtility.welcomeScreenTextDescriptionGUIStyle.wordWrap = true;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00005514 File Offset: 0x00003714
		private static void InitTaskBorderTexture(int colorIndex)
		{
			BehaviorDesignerUtility.taskBorderTexture[colorIndex] = BehaviorDesignerUtility.LoadTaskTexture("TaskBorder" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000553C File Offset: 0x0000373C
		private static void InitTaskBorderRunningTexture()
		{
			BehaviorDesignerUtility.taskBorderRunningTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskBorderRunning.png", true, null);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005550 File Offset: 0x00003750
		private static void InitTaskBorderIdentifyTexture()
		{
			BehaviorDesignerUtility.taskBorderIdentifyTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskBorderIdentify.png", true, null);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005564 File Offset: 0x00003764
		private static void InitTaskConnectionTopTexture(int colorIndex)
		{
			BehaviorDesignerUtility.taskConnectionTopTexture[colorIndex] = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionTop" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000558C File Offset: 0x0000378C
		private static void InitTaskConnectionBottomTexture(int colorIndex)
		{
			BehaviorDesignerUtility.taskConnectionBottomTexture[colorIndex] = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionBottom" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000055B4 File Offset: 0x000037B4
		private static void InitTaskConnectionRunningTopTexture()
		{
			BehaviorDesignerUtility.taskConnectionRunningTopTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionRunningTop.png", true, null);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000055C8 File Offset: 0x000037C8
		private static void InitTaskConnectionRunningBottomTexture()
		{
			BehaviorDesignerUtility.taskConnectionRunningBottomTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionRunningBottom.png", true, null);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000055DC File Offset: 0x000037DC
		private static void InitTaskConnectionIdentifyTopTexture()
		{
			BehaviorDesignerUtility.taskConnectionIdentifyTopTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionIdentifyTop.png", true, null);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000055F0 File Offset: 0x000037F0
		private static void InitTaskConnectionIdentifyBottomTexture()
		{
			BehaviorDesignerUtility.taskConnectionIdentifyBottomTexture = BehaviorDesignerUtility.LoadTaskTexture("TaskConnectionIdentifyBottom.png", true, null);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005604 File Offset: 0x00003804
		private static void InitTaskConnectionCollapsedTexture()
		{
			BehaviorDesignerUtility.taskConnectionCollapsedTexture = BehaviorDesignerUtility.LoadTexture("TaskConnectionCollapsed.png", true, null);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005618 File Offset: 0x00003818
		private static void InitContentSeparatorTexture()
		{
			BehaviorDesignerUtility.contentSeparatorTexture = BehaviorDesignerUtility.LoadTexture("ContentSeparator.png", true, null);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000562C File Offset: 0x0000382C
		private static void InitDocTexture()
		{
			BehaviorDesignerUtility.docTexture = BehaviorDesignerUtility.LoadTexture("DocIcon.png", true, null);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005640 File Offset: 0x00003840
		private static void InitGearTexture()
		{
			BehaviorDesignerUtility.gearTexture = BehaviorDesignerUtility.LoadTexture("GearIcon.png", true, null);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005654 File Offset: 0x00003854
		private static void InitColorSelectorTexture(int colorIndex)
		{
			BehaviorDesignerUtility.colorSelectorTexture[colorIndex] = BehaviorDesignerUtility.LoadTexture("ColorSelector" + BehaviorDesignerUtility.ColorIndexToColorString(colorIndex) + ".png", true, null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000567C File Offset: 0x0000387C
		private static void InitVariableButtonTexture()
		{
			BehaviorDesignerUtility.variableButtonTexture = BehaviorDesignerUtility.LoadTexture("VariableButton.png", true, null);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00005690 File Offset: 0x00003890
		private static void InitVariableButtonSelectedTexture()
		{
			BehaviorDesignerUtility.variableButtonSelectedTexture = BehaviorDesignerUtility.LoadTexture("VariableButtonSelected.png", true, null);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000056A4 File Offset: 0x000038A4
		private static void InitVariableWatchButtonTexture()
		{
			BehaviorDesignerUtility.variableWatchButtonTexture = BehaviorDesignerUtility.LoadTexture("VariableWatchButton.png", true, null);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000056B8 File Offset: 0x000038B8
		private static void InitVariableWatchButtonSelectedTexture()
		{
			BehaviorDesignerUtility.variableWatchButtonSelectedTexture = BehaviorDesignerUtility.LoadTexture("VariableWatchButtonSelected.png", true, null);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000056CC File Offset: 0x000038CC
		private static void InitReferencedTexture()
		{
			BehaviorDesignerUtility.referencedTexture = BehaviorDesignerUtility.LoadTexture("LinkedIcon.png", true, null);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000056E0 File Offset: 0x000038E0
		private static void InitConditionalAbortSelfTexture()
		{
			BehaviorDesignerUtility.conditionalAbortSelfTexture = BehaviorDesignerUtility.LoadTexture("ConditionalAbortSelfIcon.png", true, null);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000056F4 File Offset: 0x000038F4
		private static void InitConditionalAbortLowerPriorityTexture()
		{
			BehaviorDesignerUtility.conditionalAbortLowerPriorityTexture = BehaviorDesignerUtility.LoadTexture("ConditionalAbortLowerPriorityIcon.png", true, null);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005708 File Offset: 0x00003908
		private static void InitConditionalAbortBothTexture()
		{
			BehaviorDesignerUtility.conditionalAbortBothTexture = BehaviorDesignerUtility.LoadTexture("ConditionalAbortBothIcon.png", true, null);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000571C File Offset: 0x0000391C
		private static void InitDeleteButtonTexture()
		{
			BehaviorDesignerUtility.deleteButtonTexture = BehaviorDesignerUtility.LoadTexture("DeleteButton.png", true, null);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005730 File Offset: 0x00003930
		private static void InitVariableDeleteButtonTexture()
		{
			BehaviorDesignerUtility.variableDeleteButtonTexture = BehaviorDesignerUtility.LoadTexture("VariableDeleteButton.png", true, null);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005744 File Offset: 0x00003944
		private static void InitDownArrowButtonTexture()
		{
			BehaviorDesignerUtility.downArrowButtonTexture = BehaviorDesignerUtility.LoadTexture("DownArrowButton.png", true, null);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00005758 File Offset: 0x00003958
		private static void InitUpArrowButtonTexture()
		{
			BehaviorDesignerUtility.upArrowButtonTexture = BehaviorDesignerUtility.LoadTexture("UpArrowButton.png", true, null);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000576C File Offset: 0x0000396C
		private static void InitVariableMapButtonTexture()
		{
			BehaviorDesignerUtility.variableMapButtonTexture = BehaviorDesignerUtility.LoadTexture("VariableMapButton.png", true, null);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005780 File Offset: 0x00003980
		private static void InitIdentifyButtonTexture()
		{
			BehaviorDesignerUtility.identifyButtonTexture = BehaviorDesignerUtility.LoadTexture("IdentifyButton.png", true, null);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005794 File Offset: 0x00003994
		private static void InitBreakpointTexture()
		{
			BehaviorDesignerUtility.breakpointTexture = BehaviorDesignerUtility.LoadTexture("BreakpointIcon.png", false, null);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000057A8 File Offset: 0x000039A8
		private static void InitErrorIconTexture()
		{
			BehaviorDesignerUtility.errorIconTexture = BehaviorDesignerUtility.LoadTexture("ErrorIcon.png", true, null);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000057BC File Offset: 0x000039BC
		private static void InitSmallErrorIconTexture()
		{
			BehaviorDesignerUtility.smallErrorIconTexture = BehaviorDesignerUtility.LoadTexture("SmallErrorIcon.png", true, null);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000057D0 File Offset: 0x000039D0
		private static void InitEnableTaskTexture()
		{
			BehaviorDesignerUtility.enableTaskTexture = BehaviorDesignerUtility.LoadTexture("TaskEnableIcon.png", false, null);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000057E4 File Offset: 0x000039E4
		private static void InitDisableTaskTexture()
		{
			BehaviorDesignerUtility.disableTaskTexture = BehaviorDesignerUtility.LoadTexture("TaskDisableIcon.png", false, null);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000057F8 File Offset: 0x000039F8
		private static void InitExpandTaskTexture()
		{
			BehaviorDesignerUtility.expandTaskTexture = BehaviorDesignerUtility.LoadTexture("TaskExpandIcon.png", false, null);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000580C File Offset: 0x00003A0C
		private static void InitCollapseTaskTexture()
		{
			BehaviorDesignerUtility.collapseTaskTexture = BehaviorDesignerUtility.LoadTexture("TaskCollapseIcon.png", false, null);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005820 File Offset: 0x00003A20
		private static void InitExecutionSuccessTexture()
		{
			BehaviorDesignerUtility.executionSuccessTexture = BehaviorDesignerUtility.LoadTexture("ExecutionSuccess.png", false, null);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005834 File Offset: 0x00003A34
		private static void InitExecutionFailureTexture()
		{
			BehaviorDesignerUtility.executionFailureTexture = BehaviorDesignerUtility.LoadTexture("ExecutionFailure.png", false, null);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00005848 File Offset: 0x00003A48
		private static void InitExecutionSuccessRepeatTexture()
		{
			BehaviorDesignerUtility.executionSuccessRepeatTexture = BehaviorDesignerUtility.LoadTexture("ExecutionSuccessRepeat.png", false, null);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000585C File Offset: 0x00003A5C
		private static void InitExecutionFailureRepeatTexture()
		{
			BehaviorDesignerUtility.executionFailureRepeatTexture = BehaviorDesignerUtility.LoadTexture("ExecutionFailureRepeat.png", false, null);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005870 File Offset: 0x00003A70
		private static void InitHistoryBackwardTexture()
		{
			BehaviorDesignerUtility.historyBackwardTexture = BehaviorDesignerUtility.LoadTexture("HistoryBackward.png", true, null);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005884 File Offset: 0x00003A84
		private static void InitHistoryForwardTexture()
		{
			BehaviorDesignerUtility.historyForwardTexture = BehaviorDesignerUtility.LoadTexture("HistoryForward.png", true, null);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005898 File Offset: 0x00003A98
		private static void InitPlayTexture()
		{
			BehaviorDesignerUtility.playTexture = BehaviorDesignerUtility.LoadTexture("Play.png", true, null);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000058AC File Offset: 0x00003AAC
		private static void InitPauseTexture()
		{
			BehaviorDesignerUtility.pauseTexture = BehaviorDesignerUtility.LoadTexture("Pause.png", true, null);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000058C0 File Offset: 0x00003AC0
		private static void InitStepTexture()
		{
			BehaviorDesignerUtility.stepTexture = BehaviorDesignerUtility.LoadTexture("Step.png", true, null);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000058D4 File Offset: 0x00003AD4
		private static void InitScreenshotBackgroundTexture()
		{
			BehaviorDesignerUtility.screenshotBackgroundTexture = new Texture2D(1, 1, (TextureFormat)3, false, true);
			if (EditorGUIUtility.isProSkin)
			{
				BehaviorDesignerUtility.screenshotBackgroundTexture.SetPixel(1, 1, new Color(0.1647f, 0.1647f, 0.1647f));
			}
			else
			{
				BehaviorDesignerUtility.screenshotBackgroundTexture.SetPixel(1, 1, new Color(0.3647f, 0.3647f, 0.3647f));
			}
			BehaviorDesignerUtility.screenshotBackgroundTexture.Apply();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000594C File Offset: 0x00003B4C
		public static void SetObjectDirty(UnityEngine.Object obj)
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}
			if (AssetDatabase.GetAssetPath(obj).Length == 0)
			{
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
			EditorUtility.SetDirty(obj);
		}

		// Token: 0x0400001F RID: 31
		public const string Version = "1.5.7";

		// Token: 0x04000020 RID: 32
		public const int ToolBarHeight = 18;

		// Token: 0x04000021 RID: 33
		public const int PropertyBoxWidth = 300;

		// Token: 0x04000022 RID: 34
		public const int ScrollBarSize = 15;

		// Token: 0x04000023 RID: 35
		public const int EditorWindowTabHeight = 21;

		// Token: 0x04000024 RID: 36
		public const int PreferencesPaneWidth = 290;

		// Token: 0x04000025 RID: 37
		public const int PreferencesPaneHeight = 368;

		// Token: 0x04000026 RID: 38
		public const float GraphZoomMax = 1f;

		// Token: 0x04000027 RID: 39
		public const float GraphZoomMin = 0.4f;

		// Token: 0x04000028 RID: 40
		public const float GraphZoomSensitivity = 150f;

		// Token: 0x04000029 RID: 41
		public const float GraphAutoScrollEdgeDistance = 15f;

		// Token: 0x0400002A RID: 42
		public const float GraphAutoScrollEdgeSpeed = 3f;

		// Token: 0x0400002B RID: 43
		public const int LineSelectionThreshold = 7;

		// Token: 0x0400002C RID: 44
		public const int TaskBackgroundShadowSize = 3;

		// Token: 0x0400002D RID: 45
		public const int TitleHeight = 20;

		// Token: 0x0400002E RID: 46
		public const int TitleCompactHeight = 28;

		// Token: 0x0400002F RID: 47
		public const int IconAreaHeight = 52;

		// Token: 0x04000030 RID: 48
		public const int IconSize = 44;

		// Token: 0x04000031 RID: 49
		public const int IconBorderSize = 46;

		// Token: 0x04000032 RID: 50
		public const int CompactAreaHeight = 22;

		// Token: 0x04000033 RID: 51
		public const int ConnectionWidth = 42;

		// Token: 0x04000034 RID: 52
		public const int TopConnectionHeight = 14;

		// Token: 0x04000035 RID: 53
		public const int BottomConnectionHeight = 16;

		// Token: 0x04000036 RID: 54
		public const int TaskConnectionCollapsedWidth = 26;

		// Token: 0x04000037 RID: 55
		public const int TaskConnectionCollapsedHeight = 6;

		// Token: 0x04000038 RID: 56
		public const int MinWidth = 100;

		// Token: 0x04000039 RID: 57
		public const int MaxWidth = 220;

		// Token: 0x0400003A RID: 58
		public const int MaxCommentHeight = 100;

		// Token: 0x0400003B RID: 59
		public const int TextPadding = 20;

		// Token: 0x0400003C RID: 60
		public const float NodeFadeDuration = 0.5f;

		// Token: 0x0400003D RID: 61
		public const int IdentifyUpdateFadeTime = 500;

		// Token: 0x0400003E RID: 62
		public const int MaxIdentifyUpdateCount = 2000;

		// Token: 0x0400003F RID: 63
		public const float InterruptTaskHighlightDuration = 0.75f;

		// Token: 0x04000040 RID: 64
		public const int TaskPropertiesLabelWidth = 150;

		// Token: 0x04000041 RID: 65
		public const int MaxTaskDescriptionBoxWidth = 400;

		// Token: 0x04000042 RID: 66
		public const int MaxTaskDescriptionBoxHeight = 300;

		// Token: 0x04000043 RID: 67
		public const int MinorGridTickSpacing = 10;

		// Token: 0x04000044 RID: 68
		public const int MajorGridTickSpacing = 50;

		// Token: 0x04000045 RID: 69
		public const int RepaintGUICount = 1;

		// Token: 0x04000046 RID: 70
		public const float UpdateCheckInterval = 1f;

		// Token: 0x04000047 RID: 71
		private static GUIStyle graphStatusGUIStyle = null;

		// Token: 0x04000048 RID: 72
		private static GUIStyle taskFoldoutGUIStyle = null;

		// Token: 0x04000049 RID: 73
		private static GUIStyle taskTitleGUIStyle = null;

		// Token: 0x0400004A RID: 74
		private static GUIStyle[] taskGUIStyle = new GUIStyle[9];

		// Token: 0x0400004B RID: 75
		private static GUIStyle[] taskCompactGUIStyle = new GUIStyle[9];

		// Token: 0x0400004C RID: 76
		private static GUIStyle[] taskSelectedGUIStyle = new GUIStyle[9];

		// Token: 0x0400004D RID: 77
		private static GUIStyle[] taskSelectedCompactGUIStyle = new GUIStyle[9];

		// Token: 0x0400004E RID: 78
		private static GUIStyle taskRunningGUIStyle = null;

		// Token: 0x0400004F RID: 79
		private static GUIStyle taskRunningCompactGUIStyle = null;

		// Token: 0x04000050 RID: 80
		private static GUIStyle taskRunningSelectedGUIStyle = null;

		// Token: 0x04000051 RID: 81
		private static GUIStyle taskRunningSelectedCompactGUIStyle = null;

		// Token: 0x04000052 RID: 82
		private static GUIStyle taskIdentifyGUIStyle = null;

		// Token: 0x04000053 RID: 83
		private static GUIStyle taskIdentifyCompactGUIStyle = null;

		// Token: 0x04000054 RID: 84
		private static GUIStyle taskIdentifySelectedGUIStyle = null;

		// Token: 0x04000055 RID: 85
		private static GUIStyle taskIdentifySelectedCompactGUIStyle = null;

		// Token: 0x04000056 RID: 86
		private static GUIStyle taskHighlightGUIStyle = null;

		// Token: 0x04000057 RID: 87
		private static GUIStyle taskHighlightCompactGUIStyle = null;

		// Token: 0x04000058 RID: 88
		private static GUIStyle taskCommentGUIStyle = null;

		// Token: 0x04000059 RID: 89
		private static GUIStyle taskCommentLeftAlignGUIStyle = null;

		// Token: 0x0400005A RID: 90
		private static GUIStyle taskCommentRightAlignGUIStyle = null;

		// Token: 0x0400005B RID: 91
		private static GUIStyle taskDescriptionGUIStyle = null;

		// Token: 0x0400005C RID: 92
		private static GUIStyle graphBackgroundGUIStyle = null;

		// Token: 0x0400005D RID: 93
		private static GUIStyle selectionGUIStyle = null;

		// Token: 0x0400005E RID: 94
		private static GUIStyle sharedVariableToolbarPopup = null;

		// Token: 0x0400005F RID: 95
		private static GUIStyle labelWrapGUIStyle = null;

		// Token: 0x04000060 RID: 96
		private static GUIStyle tolbarButtonLeftAlignGUIStyle = null;

		// Token: 0x04000061 RID: 97
		private static GUIStyle toolbarLabelGUIStyle = null;

		// Token: 0x04000062 RID: 98
		private static GUIStyle taskInspectorCommentGUIStyle = null;

		// Token: 0x04000063 RID: 99
		private static GUIStyle taskInspectorGUIStyle = null;

		// Token: 0x04000064 RID: 100
		private static GUIStyle toolbarButtonSelectionGUIStyle = null;

		// Token: 0x04000065 RID: 101
		private static GUIStyle propertyBoxGUIStyle = null;

		// Token: 0x04000066 RID: 102
		private static GUIStyle preferencesPaneGUIStyle = null;

		// Token: 0x04000067 RID: 103
		private static GUIStyle plainButtonGUIStyle = null;

		// Token: 0x04000068 RID: 104
		private static GUIStyle transparentButtonGUIStyle = null;

		// Token: 0x04000069 RID: 105
		private static GUIStyle transparentButtonOffsetGUIStyle = null;

		// Token: 0x0400006A RID: 106
		private static GUIStyle buttonGUIStyle = null;

		// Token: 0x0400006B RID: 107
		private static GUIStyle plainTextureGUIStyle = null;

		// Token: 0x0400006C RID: 108
		private static GUIStyle arrowSeparatorGUIStyle = null;

		// Token: 0x0400006D RID: 109
		private static GUIStyle selectedBackgroundGUIStyle = null;

		// Token: 0x0400006E RID: 110
		private static GUIStyle errorListDarkBackground = null;

		// Token: 0x0400006F RID: 111
		private static GUIStyle errorListLightBackground = null;

		// Token: 0x04000070 RID: 112
		private static GUIStyle welcomeScreenIntroGUIStyle = null;

		// Token: 0x04000071 RID: 113
		private static GUIStyle welcomeScreenTextHeaderGUIStyle = null;

		// Token: 0x04000072 RID: 114
		private static GUIStyle welcomeScreenTextDescriptionGUIStyle = null;

		// Token: 0x04000073 RID: 115
		private static Texture2D[] taskBorderTexture = new Texture2D[9];

		// Token: 0x04000074 RID: 116
		private static Texture2D taskBorderRunningTexture = null;

		// Token: 0x04000075 RID: 117
		private static Texture2D taskBorderIdentifyTexture = null;

		// Token: 0x04000076 RID: 118
		private static Texture2D[] taskConnectionTopTexture = new Texture2D[9];

		// Token: 0x04000077 RID: 119
		private static Texture2D[] taskConnectionBottomTexture = new Texture2D[9];

		// Token: 0x04000078 RID: 120
		private static Texture2D taskConnectionRunningTopTexture = null;

		// Token: 0x04000079 RID: 121
		private static Texture2D taskConnectionRunningBottomTexture = null;

		// Token: 0x0400007A RID: 122
		private static Texture2D taskConnectionIdentifyTopTexture = null;

		// Token: 0x0400007B RID: 123
		private static Texture2D taskConnectionIdentifyBottomTexture = null;

		// Token: 0x0400007C RID: 124
		private static Texture2D taskConnectionCollapsedTexture = null;

		// Token: 0x0400007D RID: 125
		private static Texture2D contentSeparatorTexture = null;

		// Token: 0x0400007E RID: 126
		private static Texture2D docTexture = null;

		// Token: 0x0400007F RID: 127
		private static Texture2D gearTexture = null;

		// Token: 0x04000080 RID: 128
		private static Texture2D[] colorSelectorTexture = new Texture2D[9];

		// Token: 0x04000081 RID: 129
		private static Texture2D variableButtonTexture = null;

		// Token: 0x04000082 RID: 130
		private static Texture2D variableButtonSelectedTexture = null;

		// Token: 0x04000083 RID: 131
		private static Texture2D variableWatchButtonTexture = null;

		// Token: 0x04000084 RID: 132
		private static Texture2D variableWatchButtonSelectedTexture = null;

		// Token: 0x04000085 RID: 133
		private static Texture2D referencedTexture = null;

		// Token: 0x04000086 RID: 134
		private static Texture2D conditionalAbortSelfTexture = null;

		// Token: 0x04000087 RID: 135
		private static Texture2D conditionalAbortLowerPriorityTexture = null;

		// Token: 0x04000088 RID: 136
		private static Texture2D conditionalAbortBothTexture = null;

		// Token: 0x04000089 RID: 137
		private static Texture2D deleteButtonTexture = null;

		// Token: 0x0400008A RID: 138
		private static Texture2D variableDeleteButtonTexture = null;

		// Token: 0x0400008B RID: 139
		private static Texture2D downArrowButtonTexture = null;

		// Token: 0x0400008C RID: 140
		private static Texture2D upArrowButtonTexture = null;

		// Token: 0x0400008D RID: 141
		private static Texture2D variableMapButtonTexture = null;

		// Token: 0x0400008E RID: 142
		private static Texture2D identifyButtonTexture = null;

		// Token: 0x0400008F RID: 143
		private static Texture2D breakpointTexture = null;

		// Token: 0x04000090 RID: 144
		private static Texture2D errorIconTexture = null;

		// Token: 0x04000091 RID: 145
		private static Texture2D smallErrorIconTexture = null;

		// Token: 0x04000092 RID: 146
		private static Texture2D enableTaskTexture = null;

		// Token: 0x04000093 RID: 147
		private static Texture2D disableTaskTexture = null;

		// Token: 0x04000094 RID: 148
		private static Texture2D expandTaskTexture = null;

		// Token: 0x04000095 RID: 149
		private static Texture2D collapseTaskTexture = null;

		// Token: 0x04000096 RID: 150
		private static Texture2D executionSuccessTexture = null;

		// Token: 0x04000097 RID: 151
		private static Texture2D executionFailureTexture = null;

		// Token: 0x04000098 RID: 152
		private static Texture2D executionSuccessRepeatTexture = null;

		// Token: 0x04000099 RID: 153
		private static Texture2D executionFailureRepeatTexture = null;

		// Token: 0x0400009A RID: 154
		public static Texture2D historyBackwardTexture = null;

		// Token: 0x0400009B RID: 155
		public static Texture2D historyForwardTexture = null;

		// Token: 0x0400009C RID: 156
		private static Texture2D playTexture = null;

		// Token: 0x0400009D RID: 157
		private static Texture2D pauseTexture = null;

		// Token: 0x0400009E RID: 158
		private static Texture2D stepTexture = null;

		// Token: 0x0400009F RID: 159
		private static Texture2D screenshotBackgroundTexture = null;

		// Token: 0x040000A0 RID: 160
		private static Regex camelCaseRegex = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

		// Token: 0x040000A1 RID: 161
		private static Dictionary<string, string> camelCaseSplit = new Dictionary<string, string>();

		// Token: 0x040000A2 RID: 162
		[NonSerialized]
		private static Dictionary<Type, Dictionary<FieldInfo, bool>> attributeFieldCache = new Dictionary<Type, Dictionary<FieldInfo, bool>>();

		// Token: 0x040000A3 RID: 163
		private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

		// Token: 0x040000A4 RID: 164
		private static Dictionary<string, Texture2D> iconCache = new Dictionary<string, Texture2D>();
	}
}
