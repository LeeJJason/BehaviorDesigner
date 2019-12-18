using System;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000031 RID: 49
	public class WelcomeScreen : EditorWindow
	{
		// Token: 0x060002D6 RID: 726 RVA: 0x0001CCBC File Offset: 0x0001AEBC
		[MenuItem("Tools/Behavior Designer/Welcome Screen", false, 3)]
		public static void ShowWindow()
		{
			WelcomeScreen window = EditorWindow.GetWindow<WelcomeScreen>(true, "Welcome to Behavior Designer");
			EditorWindow editorWindow = window;
			Vector2 vector = new Vector2(340f, 410f);
			window.maxSize = vector;
			editorWindow.minSize = vector;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001CCF4 File Offset: 0x0001AEF4
		public void OnEnable()
		{
			this.m_WelcomeScreenImage = BehaviorDesignerUtility.LoadTexture("WelcomeScreenHeader.png", false, this);
			this.m_SamplesImage = BehaviorDesignerUtility.LoadIcon("WelcomeScreenSamplesIcon.png", this);
			this.m_DocImage = BehaviorDesignerUtility.LoadIcon("WelcomeScreenDocumentationIcon.png", this);
			this.m_VideoImage = BehaviorDesignerUtility.LoadIcon("WelcomeScreenVideosIcon.png", this);
			this.m_ForumImage = BehaviorDesignerUtility.LoadIcon("WelcomeScreenForumIcon.png", this);
			this.m_ContactImage = BehaviorDesignerUtility.LoadIcon("WelcomeScreenContactIcon.png", this);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001CD68 File Offset: 0x0001AF68
		public void OnGUI()
		{
			GUI.DrawTexture(this.m_WelcomeScreenImageRect, this.m_WelcomeScreenImage);
			GUI.Label(this.m_WelcomeIntroRect, "Welcome To Behavior Designer", BehaviorDesignerUtility.WelcomeScreenIntroGUIStyle);
			GUI.DrawTexture(this.m_SamplesImageRect, this.m_SamplesImage);
			GUI.Label(this.m_SamplesHeaderRect, "Samples", BehaviorDesignerUtility.WelcomeScreenTextHeaderGUIStyle);
			GUI.Label(this.m_SamplesDescriptionRect, "Download sample projects to get a feel for Behavior Designer.", BehaviorDesignerUtility.WelcomeScreenTextDescriptionGUIStyle);
			GUI.DrawTexture(this.m_DocImageRect, this.m_DocImage);
			GUI.Label(this.m_DocHeaderRect, "Documentation", BehaviorDesignerUtility.WelcomeScreenTextHeaderGUIStyle);
			GUI.Label(this.m_DocDescriptionRect, "Browser our extensive online documentation.", BehaviorDesignerUtility.WelcomeScreenTextDescriptionGUIStyle);
			GUI.DrawTexture(this.m_VideoImageRect, this.m_VideoImage);
			GUI.Label(this.m_VideoHeaderRect, "Videos", BehaviorDesignerUtility.WelcomeScreenTextHeaderGUIStyle);
			GUI.Label(this.m_VideoDescriptionRect, "Watch our tutorial videos which cover a wide variety of topics.", BehaviorDesignerUtility.WelcomeScreenTextDescriptionGUIStyle);
			GUI.DrawTexture(this.m_ForumImageRect, this.m_ForumImage);
			GUI.Label(this.m_ForumHeaderRect, "Forums", BehaviorDesignerUtility.WelcomeScreenTextHeaderGUIStyle);
			GUI.Label(this.m_ForumDescriptionRect, "Join the forums!", BehaviorDesignerUtility.WelcomeScreenTextDescriptionGUIStyle);
			GUI.DrawTexture(this.m_ContactImageRect, this.m_ContactImage);
			GUI.Label(this.m_ContactHeaderRect, "Contact", BehaviorDesignerUtility.WelcomeScreenTextHeaderGUIStyle);
			GUI.Label(this.m_ContactDescriptionRect, "We are here to help.", BehaviorDesignerUtility.WelcomeScreenTextDescriptionGUIStyle);
			GUI.Label(this.m_VersionRect, "Version 1.5.7");
			bool flag = GUI.Toggle(this.m_ToggleButtonRect, BehaviorDesignerPreferences.GetBool(BDPreferences.ShowWelcomeScreen), "Show at Startup");
			if (flag != BehaviorDesignerPreferences.GetBool(BDPreferences.ShowWelcomeScreen))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ShowWelcomeScreen, flag);
			}
			EditorGUIUtility.AddCursorRect(this.m_SamplesImageRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_SamplesHeaderRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_SamplesDescriptionRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_DocImageRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_DocHeaderRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_DocDescriptionRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_VideoImageRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_VideoHeaderRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_VideoDescriptionRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ForumImageRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ForumHeaderRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ForumDescriptionRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ContactImageRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ContactHeaderRect, (MouseCursor)4);
			EditorGUIUtility.AddCursorRect(this.m_ContactDescriptionRect, (MouseCursor)4);
			if ((int)Event.current.type == 1)
			{
				Vector2 mousePosition = Event.current.mousePosition;
				if (this.m_SamplesImageRect.Contains(mousePosition) || this.m_SamplesHeaderRect.Contains(mousePosition) || this.m_SamplesDescriptionRect.Contains(mousePosition))
				{
					Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/samples.php");
				}
				else if (this.m_DocImageRect.Contains(mousePosition) || this.m_DocHeaderRect.Contains(mousePosition) || this.m_DocDescriptionRect.Contains(mousePosition))
				{
					Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php");
				}
				else if (this.m_VideoImageRect.Contains(mousePosition) || this.m_VideoHeaderRect.Contains(mousePosition) || this.m_VideoDescriptionRect.Contains(mousePosition))
				{
					Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/videos.php");
				}
				else if (this.m_ForumImageRect.Contains(mousePosition) || this.m_ForumHeaderRect.Contains(mousePosition) || this.m_ForumDescriptionRect.Contains(mousePosition))
				{
					Application.OpenURL("http://www.opsive.com/forum");
				}
				else if (this.m_ContactImageRect.Contains(mousePosition) || this.m_ContactHeaderRect.Contains(mousePosition) || this.m_ContactDescriptionRect.Contains(mousePosition))
				{
					Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=12");
				}
			}
		}

		// Token: 0x040001BC RID: 444
		private Texture m_WelcomeScreenImage;

		// Token: 0x040001BD RID: 445
		private Texture m_SamplesImage;

		// Token: 0x040001BE RID: 446
		private Texture m_DocImage;

		// Token: 0x040001BF RID: 447
		private Texture m_VideoImage;

		// Token: 0x040001C0 RID: 448
		private Texture m_ForumImage;

		// Token: 0x040001C1 RID: 449
		private Texture m_ContactImage;

		// Token: 0x040001C2 RID: 450
		private Rect m_WelcomeScreenImageRect = new Rect(0f, 0f, 340f, 44f);

		// Token: 0x040001C3 RID: 451
		private Rect m_WelcomeIntroRect = new Rect(46f, 12f, 306f, 40f);

		// Token: 0x040001C4 RID: 452
		private Rect m_SamplesImageRect = new Rect(15f, 58f, 50f, 50f);

		// Token: 0x040001C5 RID: 453
		private Rect m_DocImageRect = new Rect(15f, 124f, 53f, 50f);

		// Token: 0x040001C6 RID: 454
		private Rect m_VideoImageRect = new Rect(15f, 190f, 50f, 50f);

		// Token: 0x040001C7 RID: 455
		private Rect m_ForumImageRect = new Rect(15f, 256f, 50f, 50f);

		// Token: 0x040001C8 RID: 456
		private Rect m_ContactImageRect = new Rect(15f, 322f, 50f, 50f);

		// Token: 0x040001C9 RID: 457
		private Rect m_VersionRect = new Rect(5f, 385f, 125f, 20f);

		// Token: 0x040001CA RID: 458
		private Rect m_ToggleButtonRect = new Rect(220f, 385f, 125f, 20f);

		// Token: 0x040001CB RID: 459
		private Rect m_SamplesHeaderRect = new Rect(70f, 57f, 250f, 20f);

		// Token: 0x040001CC RID: 460
		private Rect m_DocHeaderRect = new Rect(70f, 123f, 250f, 20f);

		// Token: 0x040001CD RID: 461
		private Rect m_VideoHeaderRect = new Rect(70f, 189f, 250f, 20f);

		// Token: 0x040001CE RID: 462
		private Rect m_ForumHeaderRect = new Rect(70f, 258f, 250f, 20f);

		// Token: 0x040001CF RID: 463
		private Rect m_ContactHeaderRect = new Rect(70f, 324f, 250f, 20f);

		// Token: 0x040001D0 RID: 464
		private Rect m_SamplesDescriptionRect = new Rect(70f, 77f, 250f, 30f);

		// Token: 0x040001D1 RID: 465
		private Rect m_DocDescriptionRect = new Rect(70f, 143f, 250f, 30f);

		// Token: 0x040001D2 RID: 466
		private Rect m_VideoDescriptionRect = new Rect(70f, 209f, 250f, 30f);

		// Token: 0x040001D3 RID: 467
		private Rect m_ForumDescriptionRect = new Rect(70f, 278f, 250f, 30f);

		// Token: 0x040001D4 RID: 468
		private Rect m_ContactDescriptionRect = new Rect(70f, 344f, 250f, 30f);
	}
}
