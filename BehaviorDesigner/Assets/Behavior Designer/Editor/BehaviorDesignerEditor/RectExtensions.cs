using System;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200000F RID: 15
	public static class RectExtensions
	{
		// Token: 0x06000169 RID: 361 RVA: 0x0000CB58 File Offset: 0x0000AD58
		public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000CB70 File Offset: 0x0000AD70
		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000CB80 File Offset: 0x0000AD80
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000CC20 File Offset: 0x0000AE20
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000CC30 File Offset: 0x0000AE30
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
	}
}
