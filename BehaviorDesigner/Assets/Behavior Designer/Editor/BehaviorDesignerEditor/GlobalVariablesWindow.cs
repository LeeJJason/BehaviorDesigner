using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000019 RID: 25
	public class GlobalVariablesWindow : EditorWindow
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x0000EEA8 File Offset: 0x0000D0A8
		[MenuItem("Tools/Behavior Designer/Global Variables", false, 1)]
		public static void ShowWindow()
		{
			GlobalVariablesWindow window = EditorWindow.GetWindow<GlobalVariablesWindow>(false, "Global Variables");
			window.minSize = new Vector2(300f, 410f);
			window.maxSize = new Vector2(300f, float.MaxValue);
			window.wantsMouseMove = true;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000EEF4 File Offset: 0x0000D0F4
		public void OnFocus()
		{
			GlobalVariablesWindow.instance = this;
			this.mVariableSource = GlobalVariables.Instance;
			if (this.mVariableSource != null)
			{
				this.mVariableSource.CheckForSerialization(!Application.isPlaying);
			}
			FieldInspector.Init();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000EF3C File Offset: 0x0000D13C
		public void OnGUI()
		{
			if (this.mVariableSource == null)
			{
				this.mVariableSource = GlobalVariables.Instance;
			}
			if (VariableInspector.DrawVariables(this.mVariableSource, null, ref this.mVariableName, ref this.mFocusNameField, ref this.mVariableTypeIndex, ref this.mScrollPosition, ref this.mVariablePosition, ref this.mVariableStartPosition, ref this.mSelectedVariableIndex, ref this.mSelectedVariableName, ref this.mSelectedVariableTypeIndex))
			{
				this.SerializeVariables();
			}
			if (Event.current.type == null && VariableInspector.LeftMouseDown(this.mVariableSource, null, Event.current.mousePosition, this.mVariablePosition, this.mVariableStartPosition, this.mScrollPosition, ref this.mSelectedVariableIndex, ref this.mSelectedVariableName, ref this.mSelectedVariableTypeIndex))
			{
				Event.current.Use();
				base.Repaint();
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000F010 File Offset: 0x0000D210
		private void SerializeVariables()
		{
			if (this.mVariableSource == null)
			{
				this.mVariableSource = GlobalVariables.Instance;
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
			{
				BinarySerialization.Save(this.mVariableSource);
			}
			else
			{
				JSONSerialization.Save(this.mVariableSource);
			}
		}

		// Token: 0x04000117 RID: 279
		private string mVariableName = string.Empty;

		// Token: 0x04000118 RID: 280
		private int mVariableTypeIndex;

		// Token: 0x04000119 RID: 281
		private Vector2 mScrollPosition = Vector2.zero;

		// Token: 0x0400011A RID: 282
		private bool mFocusNameField;

		// Token: 0x0400011B RID: 283
		[SerializeField]
		private float mVariableStartPosition = -1f;

		// Token: 0x0400011C RID: 284
		[SerializeField]
		private List<float> mVariablePosition;

		// Token: 0x0400011D RID: 285
		[SerializeField]
		private int mSelectedVariableIndex = -1;

		// Token: 0x0400011E RID: 286
		[SerializeField]
		private string mSelectedVariableName;

		// Token: 0x0400011F RID: 287
		[SerializeField]
		private int mSelectedVariableTypeIndex;

		// Token: 0x04000120 RID: 288
		private GlobalVariables mVariableSource;

		// Token: 0x04000121 RID: 289
		public static GlobalVariablesWindow instance;
	}
}
