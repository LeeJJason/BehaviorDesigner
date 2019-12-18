using System;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000010 RID: 16
	public class EditorZoomArea
	{
		// Token: 0x06000170 RID: 368 RVA: 0x0000CD0C File Offset: 0x0000AF0C
		public static Rect Begin(Rect screenCoordsArea, float zoomScale)
		{
			GUI.EndGroup();
			Rect rect = screenCoordsArea.ScaleSizeBy(1f / zoomScale, screenCoordsArea.TopLeft());
			rect.y += 21f;
			GUI.BeginGroup(rect);
			EditorZoomArea._prevGuiMatrix = GUI.matrix;
			Matrix4x4 matrix4x = Matrix4x4.TRS(rect.TopLeft(), Quaternion.identity, Vector3.one);
			Vector3 one = Vector3.one;
			one.y = zoomScale;
			one.x = zoomScale;
			Matrix4x4 matrix4x2 = Matrix4x4.Scale(one);
			GUI.matrix = matrix4x * matrix4x2 * matrix4x.inverse * GUI.matrix;
			return rect;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
		public static void End()
		{
			GUI.matrix = EditorZoomArea._prevGuiMatrix;
			GUI.EndGroup();
			EditorZoomArea.groupRect.y = 21f;
			EditorZoomArea.groupRect.width = (float)Screen.width;
			EditorZoomArea.groupRect.height = (float)Screen.height;
			GUI.BeginGroup(EditorZoomArea.groupRect);
		}

		// Token: 0x040000F6 RID: 246
		private static Matrix4x4 _prevGuiMatrix;

		// Token: 0x040000F7 RID: 247
		private static Rect groupRect = default(Rect);
	}
}
