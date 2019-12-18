using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000009 RID: 9
	public class BehaviorDesignerWindow : EditorWindow
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00005A7C File Offset: 0x00003C7C
		public List<ErrorDetails> ErrorDetails
		{
			get
			{
				return this.mErrorDetails;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00005A84 File Offset: 0x00003C84
		public BehaviorSource ActiveBehaviorSource
		{
			get
			{
				return this.mActiveBehaviorSource;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00005A8C File Offset: 0x00003C8C
		public int ActiveBehaviorID
		{
			get
			{
				return this.mActiveBehaviorID;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00005A94 File Offset: 0x00003C94
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00005B1C File Offset: 0x00003D1C
		private DateTime LastUpdateCheck
		{
			get
			{
				try
				{
					if (this.mLastUpdateCheck != DateTime.MinValue)
					{
						return this.mLastUpdateCheck;
					}
					this.mLastUpdateCheck = DateTime.Parse(EditorPrefs.GetString("BehaviorDesignerLastUpdateCheck", "1/1/1971 00:00:01"), CultureInfo.InvariantCulture);
				}
				catch (Exception)
				{
					this.mLastUpdateCheck = DateTime.UtcNow;
				}
				return this.mLastUpdateCheck;
			}
			set
			{
				this.mLastUpdateCheck = value;
				EditorPrefs.SetString("BehaviorDesignerLastUpdateCheck", this.mLastUpdateCheck.ToString(CultureInfo.InvariantCulture));
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00005B40 File Offset: 0x00003D40
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00005B7C File Offset: 0x00003D7C
		public string LatestVersion
		{
			get
			{
				if (!string.IsNullOrEmpty(this.mLatestVersion))
				{
					return this.mLatestVersion;
				}
				this.mLatestVersion = EditorPrefs.GetString("BehaviorDesignerLatestVersion", "1.5.7".ToString());
				return this.mLatestVersion;
			}
			set
			{
				this.mLatestVersion = value;
				EditorPrefs.SetString("BehaviorDesignerLatestVersion", this.mLatestVersion);
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00005B98 File Offset: 0x00003D98
		[MenuItem("Tools/Behavior Designer/Editor", false, 0)]
		public static void ShowWindow()
		{
			BehaviorDesignerWindow window = EditorWindow.GetWindow<BehaviorDesignerWindow>(false, "Behavior Designer");
			window.wantsMouseMove = true;
			window.minSize = new Vector2(500f, 100f);
			window.Init();
			BehaviorDesignerPreferences.InitPrefernces();
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.ShowWelcomeScreen))
			{
				WelcomeScreen.ShowWindow();
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00005BE8 File Offset: 0x00003DE8
		public void OnEnable()
		{
			this.mIsPlaying = EditorApplication.isPlaying;
			this.mSizesInitialized = false;
			base.Repaint();
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectWindowChange));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlaymodeStateChange));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
			this.Init();
			this.SetBehaviorManager();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00005C7C File Offset: 0x00003E7C
		public void OnFocus()
		{
			BehaviorDesignerWindow.instance = this;
			base.wantsMouseMove = true;
			this.Init();
			if (!this.mLockActiveGameObject)
			{
				this.mActiveObject = Selection.activeObject;
			}
			this.ReloadPreviousBehavior();
			this.UpdateGraphStatus();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00005CC0 File Offset: 0x00003EC0
		public void OnSelectionChange()
		{
			if (!this.mLockActiveGameObject)
			{
				this.UpdateTree(false);
			}
			else
			{
				this.ReloadPreviousBehavior();
			}
			this.UpdateGraphStatus();
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00005CE8 File Offset: 0x00003EE8
		public void OnProjectWindowChange()
		{
			this.ReloadPreviousBehavior();
			this.ClearBreadcrumbMenu();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00005CF8 File Offset: 0x00003EF8
		private void ReloadPreviousBehavior()
		{
			if (this.mActiveObject != null)
			{
				if (this.mActiveObject as GameObject)
				{
					GameObject gameObject = this.mActiveObject as GameObject;
					int num = -1;
					Behavior[] components = gameObject.GetComponents<Behavior>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].GetInstanceID() == this.mActiveBehaviorID)
						{
							num = i;
							break;
						}
					}
					if (num != -1)
					{
						this.LoadBehavior(components[num].GetBehaviorSource(), true, false);
					}
					else if (components.Count<Behavior>() > 0)
					{
						this.LoadBehavior(components[0].GetBehaviorSource(), true, false);
					}
					else if (this.mGraphDesigner != null)
					{
						this.ClearGraph();
					}
				}
				else if (this.mActiveObject is ExternalBehavior)
				{
					ExternalBehavior externalBehavior = this.mActiveObject as ExternalBehavior;
					BehaviorSource behaviorSource = externalBehavior.BehaviorSource;
					if (externalBehavior.BehaviorSource.Owner == null)
					{
						externalBehavior.BehaviorSource.Owner = externalBehavior;
					}
					this.LoadBehavior(behaviorSource, true, false);
				}
				else if (this.mGraphDesigner != null)
				{
					this.mActiveObject = null;
					this.ClearGraph();
				}
			}
			else if (this.mGraphDesigner != null)
			{
				this.ClearGraph();
				base.Repaint();
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005E5C File Offset: 0x0000405C
		private void UpdateTree(bool firstLoad)
		{
			bool flag = firstLoad;
			if (Selection.activeObject != null)
			{
				bool loadPrevBehavior = false;
				if (!Selection.activeObject.Equals(this.mActiveObject))
				{
					this.mActiveObject = Selection.activeObject;
					flag = true;
				}
				BehaviorSource behaviorSource = null;
				GameObject gameObject = this.mActiveObject as GameObject;
				if (gameObject != null && gameObject.GetComponent<Behavior>() != null)
				{
					if (flag)
					{
						if (this.mActiveObject.Equals(this.mPrevActiveObject) && this.mActiveBehaviorID != -1)
						{
							loadPrevBehavior = true;
							int num = -1;
							Behavior[] components = (this.mActiveObject as GameObject).GetComponents<Behavior>();
							for (int i = 0; i < components.Length; i++)
							{
								if (components[i].GetInstanceID() == this.mActiveBehaviorID)
								{
									num = i;
									break;
								}
							}
							if (num != -1)
							{
								behaviorSource = gameObject.GetComponents<Behavior>()[num].GetBehaviorSource();
							}
							else if (components.Count<Behavior>() > 0)
							{
								behaviorSource = gameObject.GetComponents<Behavior>()[0].GetBehaviorSource();
							}
						}
						else
						{
							behaviorSource = gameObject.GetComponents<Behavior>()[0].GetBehaviorSource();
						}
					}
					else
					{
						Behavior[] components2 = gameObject.GetComponents<Behavior>();
						bool flag2 = false;
						if (this.mActiveBehaviorSource != null)
						{
							for (int j = 0; j < components2.Length; j++)
							{
								if (components2[j].Equals(this.mActiveBehaviorSource.Owner))
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							behaviorSource = gameObject.GetComponents<Behavior>()[0].GetBehaviorSource();
						}
						else
						{
							behaviorSource = this.mActiveBehaviorSource;
							loadPrevBehavior = true;
						}
					}
				}
				else if (this.mActiveObject is ExternalBehavior)
				{
					ExternalBehavior externalBehavior = this.mActiveObject as ExternalBehavior;
					if (externalBehavior.BehaviorSource.Owner == null)
					{
						externalBehavior.BehaviorSource.Owner = externalBehavior;
					}
					if (flag && this.mActiveObject.Equals(this.mPrevActiveObject))
					{
						loadPrevBehavior = true;
					}
					behaviorSource = externalBehavior.BehaviorSource;
				}
				else
				{
					this.mPrevActiveObject = null;
				}
				if (behaviorSource != null)
				{
					this.LoadBehavior(behaviorSource, loadPrevBehavior, false);
				}
				else if (behaviorSource == null)
				{
					this.ClearGraph();
				}
			}
			else
			{
				if (this.mActiveObject != null && this.mActiveBehaviorSource != null)
				{
					this.mPrevActiveObject = this.mActiveObject;
				}
				this.mActiveObject = null;
				this.ClearGraph();
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000060D0 File Offset: 0x000042D0
		private void Init()
		{
			if (this.mTaskList == null)
			{
				this.mTaskList = ScriptableObject.CreateInstance<TaskList>();
			}
			if (this.mVariableInspector == null)
			{
				this.mVariableInspector = ScriptableObject.CreateInstance<VariableInspector>();
			}
			if (this.mGraphDesigner == null)
			{
				this.mGraphDesigner = ScriptableObject.CreateInstance<GraphDesigner>();
			}
			if (this.mTaskInspector == null)
			{
				this.mTaskInspector = ScriptableObject.CreateInstance<TaskInspector>();
			}
			if (this.mGridMaterial == null)
			{
				this.mGridMaterial = new Material(Shader.Find("Hidden/Behavior Designer/Grid"));
				this.mGridMaterial.hideFlags = (HideFlags)61;
				this.mGridMaterial.shader.hideFlags = (HideFlags)61;
			}
			this.mTaskList.Init();
			FieldInspector.Init();
			this.ClearBreadcrumbMenu();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000061A8 File Offset: 0x000043A8
		public void UpdateGraphStatus()
		{
			if (this.mActiveObject == null || this.mGraphDesigner == null || (this.mActiveObject as GameObject == null && this.mActiveObject as ExternalBehavior == null))
			{
				this.mGraphStatus = "Select a GameObject";
			}
			else if (this.mActiveObject as GameObject != null && object.ReferenceEquals((this.mActiveObject as GameObject).GetComponent<Behavior>(), null))
			{
				this.mGraphStatus = "Right Click, Add a Behavior Tree Component";
			}
			else if (this.ViewOnlyMode() && this.mActiveBehaviorSource != null)
			{
				ExternalBehavior externalBehavior = (this.mActiveBehaviorSource.Owner.GetObject() as Behavior).ExternalBehavior;
				if (externalBehavior != null)
				{
					this.mGraphStatus = externalBehavior.BehaviorSource.ToString() + " (View Only Mode)";
				}
				else
				{
					this.mGraphStatus = this.mActiveBehaviorSource.ToString() + " (View Only Mode)";
				}
			}
			else if (!this.mGraphDesigner.HasEntryNode())
			{
				this.mGraphStatus = "Add a Task";
			}
			else if (this.IsReferencingTasks())
			{
				this.mGraphStatus = "Select tasks to reference (right click to exit)";
			}
			else if (this.mActiveBehaviorSource != null && this.mActiveBehaviorSource.Owner != null && this.mActiveBehaviorSource.Owner.GetObject() != null)
			{
				if (this.mExternalParent != null)
				{
					this.mGraphStatus = this.mExternalParent.ToString() + " (Editing External Behavior)";
				}
				else
				{
					this.mGraphStatus = this.mActiveBehaviorSource.ToString();
				}
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006380 File Offset: 0x00004580
		private void BuildBreadcrumbMenus(BehaviorDesignerWindow.BreadcrumbMenuType menuType)
		{
			Dictionary<BehaviorSource, string> dictionary = new Dictionary<BehaviorSource, string>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			HashSet<UnityEngine.Object> hashSet = new HashSet<UnityEngine.Object>();
			List<BehaviorSource> list = new List<BehaviorSource>();
			Behavior[] array = Resources.FindObjectsOfTypeAll(typeof(Behavior)) as Behavior[];
			for (int i = array.Length - 1; i > -1; i--)
			{
				BehaviorSource behaviorSource = array[i].GetBehaviorSource();
				if (behaviorSource.Owner == null)
				{
					behaviorSource.Owner = array[i];
				}
				list.Add(behaviorSource);
			}
			ExternalBehavior[] array2 = Resources.FindObjectsOfTypeAll(typeof(ExternalBehavior)) as ExternalBehavior[];
			for (int j = array2.Length - 1; j > -1; j--)
			{
				BehaviorSource behaviorSource2 = array2[j].GetBehaviorSource();
				if (behaviorSource2.Owner == null)
				{
					behaviorSource2.Owner = array2[j];
				}
				list.Add(behaviorSource2);
			}
			list.Sort(new AlphanumComparator<BehaviorSource>());
			int k = 0;
			while (k < list.Count)
			{
                UnityEngine.Object @object = list[k].Owner.GetObject();
				if (menuType != BehaviorDesignerWindow.BreadcrumbMenuType.Behavior)
				{
					goto IL_14E;
				}
				if (@object is Behavior)
				{
					if ((@object as Behavior).gameObject.Equals(this.mActiveObject))
					{
						goto IL_14E;
					}
				}
				else if ((@object as ExternalBehavior).Equals(this.mActiveObject))
				{
					goto IL_14E;
				}
				IL_29B:
				k++;
				continue;
				IL_14E:
				if (menuType == BehaviorDesignerWindow.BreadcrumbMenuType.GameObject && @object is Behavior)
				{
					if (hashSet.Contains((@object as Behavior).gameObject))
					{
						goto IL_29B;
					}
					hashSet.Add((@object as Behavior).gameObject);
				}
				string text = string.Empty;
				if (@object is Behavior)
				{
					switch (menuType)
					{
					case BehaviorDesignerWindow.BreadcrumbMenuType.GameObjectBehavior:
						text = list[k].ToString();
						break;
					case BehaviorDesignerWindow.BreadcrumbMenuType.GameObject:
						text = (@object as Behavior).gameObject.name;
						break;
					case BehaviorDesignerWindow.BreadcrumbMenuType.Behavior:
						text = list[k].behaviorName;
						break;
					}
					if (!AssetDatabase.GetAssetPath(@object).Equals(string.Empty))
					{
						text += " (prefab)";
					}
				}
				else
				{
					text = list[k].ToString() + " (external)";
				}
				int num = 0;
				if (dictionary2.TryGetValue(text, out num))
				{
					num = (dictionary2[text] = num + 1);
					text += string.Format(" ({0})", num + 1);
				}
				else
				{
					dictionary2.Add(text, 0);
				}
				dictionary.Add(list[k], text);
				goto IL_29B;
			}
			switch (menuType)
			{
			case BehaviorDesignerWindow.BreadcrumbMenuType.GameObjectBehavior:
				this.mBreadcrumbGameObjectBehaviorMenu = new GenericMenu();
				break;
			case BehaviorDesignerWindow.BreadcrumbMenuType.GameObject:
				this.mBreadcrumbGameObjectMenu = new GenericMenu();
				break;
			case BehaviorDesignerWindow.BreadcrumbMenuType.Behavior:
				this.mBreadcrumbBehaviorMenu = new GenericMenu();
				break;
			}
			foreach (KeyValuePair<BehaviorSource, string> keyValuePair in dictionary)
			{
				switch (menuType)
				{
				case BehaviorDesignerWindow.BreadcrumbMenuType.GameObjectBehavior:
					this.mBreadcrumbGameObjectBehaviorMenu.AddItem(new GUIContent(keyValuePair.Value), keyValuePair.Key.Equals(this.mActiveBehaviorSource), new GenericMenu.MenuFunction2(this.BehaviorSelectionCallback), keyValuePair.Key);
					break;
				case BehaviorDesignerWindow.BreadcrumbMenuType.GameObject:
				{
					bool flag;
					if (keyValuePair.Key.Owner.GetObject() is ExternalBehavior)
					{
						flag = (keyValuePair.Key.Owner.GetObject() as ExternalBehavior).GetObject().Equals(this.mActiveObject);
					}
					else
					{
						flag = (keyValuePair.Key.Owner.GetObject() as Behavior).gameObject.Equals(this.mActiveObject);
					}
					this.mBreadcrumbGameObjectMenu.AddItem(new GUIContent(keyValuePair.Value), flag, new GenericMenu.MenuFunction2(this.BehaviorSelectionCallback), keyValuePair.Key);
					break;
				}
				case BehaviorDesignerWindow.BreadcrumbMenuType.Behavior:
					this.mBreadcrumbBehaviorMenu.AddItem(new GUIContent(keyValuePair.Value), keyValuePair.Key.Equals(this.mActiveBehaviorSource), new GenericMenu.MenuFunction2(this.BehaviorSelectionCallback), keyValuePair.Key);
					break;
				}
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00006818 File Offset: 0x00004A18
		private void ClearBreadcrumbMenu()
		{
			this.mBreadcrumbGameObjectBehaviorMenu = null;
			this.mBreadcrumbGameObjectMenu = null;
			this.mBreadcrumbBehaviorMenu = null;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00006830 File Offset: 0x00004A30
		private void BuildRightClickMenu(NodeDesigner clickedNode)
		{
			if (this.mActiveObject == null)
			{
				return;
			}
			this.mRightClickMenu = new GenericMenu();
			this.mShowRightClickMenu = true;
			if (clickedNode == null && !EditorApplication.isPlaying && !this.ViewOnlyMode())
			{
				this.mTaskList.AddTasksToMenu(ref this.mRightClickMenu, null, "Add Task", new GenericMenu.MenuFunction2(this.AddTaskCallback));
				if (this.mCopiedTasks != null && this.mCopiedTasks.Count > 0)
				{
					this.mRightClickMenu.AddItem(new GUIContent("Paste Tasks"), false, new GenericMenu.MenuFunction(this.PasteNodes));
				}
				else
				{
					this.mRightClickMenu.AddDisabledItem(new GUIContent("Paste Tasks"));
				}
			}
			if (clickedNode != null && !clickedNode.IsEntryDisplay)
			{
				if (this.mGraphDesigner.SelectedNodes.Count == 1)
				{
					this.mRightClickMenu.AddItem(new GUIContent("Edit Script"), false, new GenericMenu.MenuFunction2(this.OpenInFileEditor), clickedNode);
					this.mRightClickMenu.AddItem(new GUIContent("Locate Script"), false, new GenericMenu.MenuFunction2(this.SelectInProject), clickedNode);
					if (!this.ViewOnlyMode())
					{
						this.mRightClickMenu.AddItem(new GUIContent((!clickedNode.Task.Disabled) ? "Disable" : "Enable"), false, new GenericMenu.MenuFunction2(this.ToggleEnableState), clickedNode);
						if (clickedNode.IsParent)
						{
							this.mRightClickMenu.AddItem(new GUIContent((!clickedNode.Task.NodeData.Collapsed) ? "Collapse" : "Expand"), false, new GenericMenu.MenuFunction2(this.ToggleCollapseState), clickedNode);
						}
						this.mRightClickMenu.AddItem(new GUIContent((!clickedNode.Task.NodeData.IsBreakpoint) ? "Set Breakpoint" : "Remove Breakpoint"), false, new GenericMenu.MenuFunction2(this.ToggleBreakpoint), clickedNode);
						this.mTaskList.AddTasksToMenu(ref this.mRightClickMenu, this.mGraphDesigner.SelectedNodes[0].Task.GetType(), "Replace", new GenericMenu.MenuFunction2(this.ReplaceTaskCallback));
					}
				}
				if (!EditorApplication.isPlaying && !this.ViewOnlyMode())
				{
					this.mRightClickMenu.AddItem(new GUIContent(string.Format("Copy Task{0}", (this.mGraphDesigner.SelectedNodes.Count <= 1) ? string.Empty : "s")), false, new GenericMenu.MenuFunction(this.CopyNodes));
					if (this.mCopiedTasks != null && this.mCopiedTasks.Count > 0)
					{
						this.mRightClickMenu.AddItem(new GUIContent(string.Format("Paste Task{0}", (this.mCopiedTasks.Count <= 1) ? string.Empty : "s")), false, new GenericMenu.MenuFunction(this.PasteNodes));
					}
					else
					{
						this.mRightClickMenu.AddDisabledItem(new GUIContent("Paste Tasks"));
					}
					this.mRightClickMenu.AddItem(new GUIContent(string.Format("Delete Task{0}", (this.mGraphDesigner.SelectedNodes.Count <= 1) ? string.Empty : "s")), false, new GenericMenu.MenuFunction(this.DeleteNodes));
				}
			}
			if (!EditorApplication.isPlaying && this.mActiveObject as GameObject != null)
			{
				if (clickedNode != null && !clickedNode.IsEntryDisplay)
				{
					this.mRightClickMenu.AddSeparator(string.Empty);
				}
				this.mRightClickMenu.AddItem(new GUIContent("Add Behavior Tree"), false, new GenericMenu.MenuFunction(this.AddBehavior));
				if (this.mActiveBehaviorSource != null)
				{
					this.mRightClickMenu.AddItem(new GUIContent("Remove Behavior Tree"), false, new GenericMenu.MenuFunction(this.RemoveBehavior));
					this.mRightClickMenu.AddItem(new GUIContent("Save As External Behavior Tree"), false, new GenericMenu.MenuFunction(this.SaveAsAsset));
				}
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00006C68 File Offset: 0x00004E68
		public void Update()
		{
			if (this.mTakingScreenshot)
			{
				base.Repaint();
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00006C7C File Offset: 0x00004E7C
		public void OnGUI()
		{
			this.mCurrentMousePosition = Event.current.mousePosition;
			this.SetupSizes();
			if (!this.mSizesInitialized)
			{
				this.mSizesInitialized = true;
				if (!this.mLockActiveGameObject || this.mActiveObject == null)
				{
					this.UpdateTree(true);
				}
				else
				{
					this.ReloadPreviousBehavior();
				}
			}
			if (this.Draw() && this.mGUITickCount > 1)
			{
				base.Repaint();
				this.mGUITickCount = 0;
			}
			this.HandleEvents();
			this.mGUITickCount++;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006D18 File Offset: 0x00004F18
		public void OnPlaymodeStateChange()
		{
			if (EditorApplication.isPlaying && !EditorApplication.isPaused)
			{
				if (this.mBehaviorManager == null)
				{
					this.SetBehaviorManager();
					if (this.mBehaviorManager == null)
					{
						return;
					}
				}
				if (this.mBehaviorManager.BreakpointTree != null && this.mEditorAtBreakpoint)
				{
					this.mEditorAtBreakpoint = false;
					this.mBehaviorManager.BreakpointTree = null;
				}
			}
			else if (EditorApplication.isPlaying && EditorApplication.isPaused)
			{
				if (this.mBehaviorManager != null && this.mBehaviorManager.BreakpointTree != null)
				{
					if (!this.mEditorAtBreakpoint)
					{
						this.mEditorAtBreakpoint = true;
						if (BehaviorDesignerPreferences.GetBool(BDPreferences.SelectOnBreakpoint) && !this.mLockActiveGameObject)
						{
							Selection.activeObject = this.mBehaviorManager.BreakpointTree;
							this.LoadBehavior(this.mBehaviorManager.BreakpointTree.GetBehaviorSource(), this.mActiveBehaviorSource == this.mBehaviorManager.BreakpointTree.GetBehaviorSource(), false);
						}
					}
					else
					{
						this.mEditorAtBreakpoint = false;
						this.mBehaviorManager.BreakpointTree = null;
					}
				}
			}
			else if (!EditorApplication.isPlaying)
			{
				this.mBehaviorManager = null;
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00006E6C File Offset: 0x0000506C
		private void SetBehaviorManager()
		{
			this.mBehaviorManager = BehaviorManager.instance;
			if (this.mBehaviorManager == null)
			{
				return;
			}
			BehaviorManager behaviorManager = this.mBehaviorManager;
			behaviorManager.OnTaskBreakpoint = (BehaviorManager.BehaviorManagerHandler)Delegate.Combine(behaviorManager.OnTaskBreakpoint, new BehaviorManager.BehaviorManagerHandler(this.OnTaskBreakpoint));
			this.mUpdateNodeTaskMap = true;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006EC4 File Offset: 0x000050C4
		public void OnTaskBreakpoint()
		{
			EditorApplication.isPaused = true;
			base.Repaint();
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006ED4 File Offset: 0x000050D4
		private void OnPreferenceChange(BDPreferences pref, object value)
		{
			switch (pref)
			{
			case BDPreferences.CompactMode:
				this.mGraphDesigner.GraphDirty();
				break;
			default:
				if (pref == BDPreferences.ShowSceneIcon || pref == BDPreferences.GizmosViewMode)
				{
					GizmoManager.UpdateAllGizmos();
				}
				break;
			case BDPreferences.BinarySerialization:
				this.SaveBehavior();
				break;
			case BDPreferences.ErrorChecking:
				this.CheckForErrors();
				break;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006F44 File Offset: 0x00005144
		public void OnInspectorUpdate()
		{
			if (this.mStepApplication)
			{
				EditorApplication.Step();
				this.mStepApplication = false;
			}
			if (EditorApplication.isPlaying && !EditorApplication.isPaused && this.mActiveBehaviorSource != null && this.mBehaviorManager != null)
			{
				if (this.mUpdateNodeTaskMap)
				{
					this.UpdateNodeTaskMap();
				}
				if (this.mBehaviorManager.BreakpointTree != null)
				{
					this.mBehaviorManager.BreakpointTree = null;
				}
				base.Repaint();
			}
			if (Application.isPlaying && this.mBehaviorManager == null)
			{
				this.SetBehaviorManager();
			}
			if (this.mBehaviorManager != null && this.mBehaviorManager.Dirty)
			{
				if (this.mActiveBehaviorSource != null)
				{
					this.LoadBehavior(this.mActiveBehaviorSource, true, false);
				}
				this.mBehaviorManager.Dirty = false;
			}
			if (!EditorApplication.isPlaying && this.mIsPlaying)
			{
				this.ReloadPreviousBehavior();
			}
			this.mIsPlaying = EditorApplication.isPlaying;
			this.UpdateGraphStatus();
			this.UpdateCheck();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000706C File Offset: 0x0000526C
		private void UpdateNodeTaskMap()
		{
			if (this.mUpdateNodeTaskMap && this.mBehaviorManager != null)
			{
				Behavior behavior = this.mActiveBehaviorSource.Owner as Behavior;
				List<Task> taskList = this.mBehaviorManager.GetTaskList(behavior);
				if (taskList != null)
				{
					this.mNodeDesignerTaskMap = new Dictionary<NodeDesigner, Task>();
					for (int i = 0; i < taskList.Count; i++)
					{
						NodeDesigner nodeDesigner = taskList[i].NodeData.NodeDesigner as NodeDesigner;
						if (nodeDesigner != null && !this.mNodeDesignerTaskMap.ContainsKey(nodeDesigner))
						{
							this.mNodeDesignerTaskMap.Add(nodeDesigner, taskList[i]);
						}
					}
					this.mUpdateNodeTaskMap = false;
				}
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000712C File Offset: 0x0000532C
		private bool Draw()
		{
			bool result = false;
			Color color = GUI.color;
			Color backgroundColor = GUI.backgroundColor;
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			this.DrawFileToolbar();
			this.DrawDebugToolbar();
			this.DrawPropertiesBox();
			if (this.DrawGraphArea())
			{
				result = true;
			}
			this.DrawPreferencesPane();
			if (this.mTakingScreenshot)
			{
				GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), BehaviorDesignerUtility.ScreenshotBackgroundTexture, 0, false);
			}
			GUI.color = color;
			GUI.backgroundColor = backgroundColor;
			return result;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000071C0 File Offset: 0x000053C0
		private void DrawFileToolbar()
		{
			GUILayout.BeginArea(this.mFileToolBarRect, EditorStyles.toolbar);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(BehaviorDesignerUtility.HistoryBackwardTexture, EditorStyles.toolbarButton, new GUILayoutOption[0]) && (this.mBehaviorSourceHistoryIndex > 0 || (this.mActiveBehaviorSource == null && this.mBehaviorSourceHistoryIndex == 0)))
			{
				BehaviorSource behaviorSource = null;
				if (this.mActiveBehaviorSource == null)
				{
					this.mBehaviorSourceHistoryIndex++;
				}
				while (behaviorSource == null && this.mBehaviorSourceHistory.Count > 0 && this.mBehaviorSourceHistoryIndex > 0)
				{
					this.mBehaviorSourceHistoryIndex--;
					behaviorSource = this.BehaviorSourceFromIBehaviorHistory(this.mBehaviorSourceHistory[this.mBehaviorSourceHistoryIndex] as IBehavior);
					if (behaviorSource == null || behaviorSource.Owner == null || behaviorSource.Owner.GetObject() == null)
					{
						this.mBehaviorSourceHistory.RemoveAt(this.mBehaviorSourceHistoryIndex);
						behaviorSource = null;
					}
				}
				if (behaviorSource != null)
				{
					this.LoadBehavior(behaviorSource, false);
				}
			}
			if (GUILayout.Button(BehaviorDesignerUtility.HistoryForwardTexture, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				BehaviorSource behaviorSource2 = null;
				if (this.mBehaviorSourceHistoryIndex < this.mBehaviorSourceHistory.Count - 1)
				{
					this.mBehaviorSourceHistoryIndex++;
					while (behaviorSource2 == null && this.mBehaviorSourceHistoryIndex < this.mBehaviorSourceHistory.Count && this.mBehaviorSourceHistoryIndex > 0)
					{
						behaviorSource2 = this.BehaviorSourceFromIBehaviorHistory(this.mBehaviorSourceHistory[this.mBehaviorSourceHistoryIndex] as IBehavior);
						if (behaviorSource2 == null || behaviorSource2.Owner == null || behaviorSource2.Owner.GetObject() == null)
						{
							this.mBehaviorSourceHistory.RemoveAt(this.mBehaviorSourceHistoryIndex);
							behaviorSource2 = null;
						}
					}
				}
				if (behaviorSource2 != null)
				{
					this.LoadBehavior(behaviorSource2, false);
				}
			}
			if (GUILayout.Button("...", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(22f)
			}))
			{
				this.BuildBreadcrumbMenus(BehaviorDesignerWindow.BreadcrumbMenuType.GameObjectBehavior);
				this.mBreadcrumbGameObjectBehaviorMenu.ShowAsContext();
			}
			string text = (!(this.mActiveObject as GameObject != null) && !(this.mActiveObject as ExternalBehavior != null)) ? "(None Selected)" : this.mActiveObject.name;
			if (GUILayout.Button(text, EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(140f)
			}))
			{
				this.BuildBreadcrumbMenus(BehaviorDesignerWindow.BreadcrumbMenuType.GameObject);
				this.mBreadcrumbGameObjectMenu.ShowAsContext();
			}
			string text2 = (this.mActiveBehaviorSource == null) ? "(None Selected)" : this.mActiveBehaviorSource.behaviorName;
			if (GUILayout.Button(text2, EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(140f)
			}) && this.mActiveBehaviorSource != null)
			{
				this.BuildBreadcrumbMenus(BehaviorDesignerWindow.BreadcrumbMenuType.Behavior);
				this.mBreadcrumbBehaviorMenu.ShowAsContext();
			}
			if (GUILayout.Button("Referenced Behaviors", EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(140f)
			}) && this.mActiveBehaviorSource != null)
			{
				List<BehaviorSource> list = this.mGraphDesigner.FindReferencedBehaviors();
				if (list.Count > 0)
				{
					list.Sort(new AlphanumComparator<BehaviorSource>());
					this.mReferencedBehaviorsMenu = new GenericMenu();
					for (int i = 0; i < list.Count; i++)
					{
						this.mReferencedBehaviorsMenu.AddItem(new GUIContent(list[i].ToString()), false, new GenericMenu.MenuFunction2(this.BehaviorSelectionCallback), list[i]);
					}
					this.mReferencedBehaviorsMenu.ShowAsContext();
				}
			}
			if (GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(22f)
			}))
			{
				if (this.mActiveBehaviorSource != null)
				{
					this.RemoveBehavior();
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to Remove Behavior Tree", "No behavior tree selected.", "OK");
				}
			}
			if (GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(22f)
			}))
			{
				if (this.mActiveObject != null)
				{
					this.AddBehavior();
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to Add Behavior Tree", "No GameObject is selected.", "OK");
				}
			}
			if (GUILayout.Button("Lock", (!this.mLockActiveGameObject) ? EditorStyles.toolbarButton : BehaviorDesignerUtility.ToolbarButtonSelectionGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(42f)
			}))
			{
				if (this.mActiveObject != null)
				{
					this.mLockActiveGameObject = !this.mLockActiveGameObject;
					if (!this.mLockActiveGameObject)
					{
						this.UpdateTree(false);
					}
				}
				else if (this.mLockActiveGameObject)
				{
					this.mLockActiveGameObject = false;
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to Lock GameObject", "No GameObject is selected.", "OK");
				}
			}
			GUI.enabled = (this.mActiveBehaviorSource == null || this.mExternalParent == null);
			if (GUILayout.Button("Export", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(46f)
			}))
			{
				if (this.mActiveBehaviorSource != null)
				{
					if (this.mActiveBehaviorSource.Owner.GetObject() as Behavior)
					{
						this.SaveAsAsset();
					}
					else
					{
						this.SaveAsPrefab();
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to Save Behavior Tree", "Select a behavior tree from within the scene.", "OK");
				}
			}
			GUI.enabled = true;
			if (GUILayout.Button("Take Screenshot", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(96f)
			}))
			{
				if (this.mActiveBehaviorSource != null)
				{
					this.TakeScreenshot();
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to Take Screenshot", "Select a behavior tree from within the scene.", "OK");
				}
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Preferences", (!this.mShowPrefPane) ? EditorStyles.toolbarButton : BehaviorDesignerUtility.ToolbarButtonSelectionGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			}))
			{
				this.mShowPrefPane = !this.mShowPrefPane;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007810 File Offset: 0x00005A10
		private void DrawDebugToolbar()
		{
			GUILayout.BeginArea(this.mDebugToolBarRect, EditorStyles.toolbar);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(BehaviorDesignerUtility.PlayTexture, (!EditorApplication.isPlaying) ? EditorStyles.toolbarButton : BehaviorDesignerUtility.ToolbarButtonSelectionGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			}))
			{
				EditorApplication.isPlaying = !EditorApplication.isPlaying;
			}
			if (GUILayout.Button(BehaviorDesignerUtility.PauseTexture, (!EditorApplication.isPaused) ? EditorStyles.toolbarButton : BehaviorDesignerUtility.ToolbarButtonSelectionGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			}))
			{
				EditorApplication.isPaused = !EditorApplication.isPaused;
			}
			if (GUILayout.Button(BehaviorDesignerUtility.StepTexture, EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			}) && EditorApplication.isPlaying)
			{
				this.mStepApplication = true;
			}
			if (this.mErrorDetails != null && this.mErrorDetails.Count > 0 && GUILayout.Button(new GUIContent(this.mErrorDetails.Count + " Error" + ((this.mErrorDetails.Count <= 1) ? string.Empty : "s"), BehaviorDesignerUtility.SmallErrorIconTexture), BehaviorDesignerUtility.ToolbarButtonLeftAlignGUIStyle, new GUILayoutOption[]
			{
				GUILayout.Width(85f)
			}))
			{
				ErrorWindow.ShowWindow();
			}
			GUILayout.FlexibleSpace();
			if ("1.5.7".ToString().CompareTo(this.LatestVersion) < 0)
			{
				GUILayout.Label("Behavior Designer " + this.LatestVersion + " is now available.", BehaviorDesignerUtility.ToolbarLabelGUIStyle, new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000079D8 File Offset: 0x00005BD8
		private void DrawPreferencesPane()
		{
			if (this.mShowPrefPane)
			{
				GUILayout.BeginArea(this.mPreferencesPaneRect, BehaviorDesignerUtility.PreferencesPaneGUIStyle);
				BehaviorDesignerPreferences.DrawPreferencesPane(new PreferenceChangeHandler(this.OnPreferenceChange));
				GUILayout.EndArea();
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007A0C File Offset: 0x00005C0C
		private void DrawPropertiesBox()
		{
			GUILayout.BeginArea(this.mPropertyToolbarRect, EditorStyles.toolbar);
			int num = this.mBehaviorToolbarSelection;
			this.mBehaviorToolbarSelection = GUILayout.Toolbar(this.mBehaviorToolbarSelection, this.mBehaviorToolbarStrings, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUILayout.EndArea();
			GUILayout.BeginArea(this.mPropertyBoxRect, BehaviorDesignerUtility.PropertyBoxGUIStyle);
			if (this.mBehaviorToolbarSelection == 0)
			{
				if (this.mActiveBehaviorSource != null)
				{
					GUILayout.Space(3f);
					BehaviorSource behaviorSource = (this.mExternalParent == null) ? this.mActiveBehaviorSource : this.mExternalParent;
					if (behaviorSource.Owner as Behavior != null)
					{
						bool flag = false;
						bool flag2 = false;
						if (BehaviorInspector.DrawInspectorGUI(behaviorSource.Owner as Behavior, new SerializedObject(behaviorSource.Owner as Behavior), false, ref flag, ref flag2, ref flag2))
						{
							BehaviorDesignerUtility.SetObjectDirty(behaviorSource.Owner.GetObject());
							if (flag)
							{
								this.LoadBehavior(behaviorSource, false, false);
							}
						}
					}
					else
					{
						bool flag3 = false;
						ExternalBehaviorInspector.DrawInspectorGUI(behaviorSource, false, ref flag3);
					}
				}
				else
				{
					GUILayout.Space(5f);
					GUILayout.Label("No behavior tree selected. Create a new behavior tree or select one from the hierarchy.", BehaviorDesignerUtility.LabelWrapGUIStyle, new GUILayoutOption[]
					{
						GUILayout.Width(285f)
					});
				}
			}
			else if (this.mBehaviorToolbarSelection == 1)
			{
				this.mTaskList.DrawTaskList(this, !this.ViewOnlyMode());
				if (num != 1)
				{
					this.mTaskList.FocusSearchField();
				}
			}
			else if (this.mBehaviorToolbarSelection == 2)
			{
				if (this.mActiveBehaviorSource != null)
				{
					BehaviorSource behaviorSource2 = (this.mExternalParent == null) ? this.mActiveBehaviorSource : this.mExternalParent;
					if (this.mVariableInspector.DrawVariables(behaviorSource2))
					{
						this.SaveBehavior();
					}
					if (num != 2)
					{
						this.mVariableInspector.FocusNameField();
					}
				}
				else
				{
					GUILayout.Space(5f);
					GUILayout.Label("No behavior tree selected. Create a new behavior tree or select one from the hierarchy.", BehaviorDesignerUtility.LabelWrapGUIStyle, new GUILayoutOption[]
					{
						GUILayout.Width(285f)
					});
				}
			}
			else if (this.mBehaviorToolbarSelection == 3)
			{
				if (this.mGraphDesigner.SelectedNodes.Count == 1 && !this.mGraphDesigner.SelectedNodes[0].IsEntryDisplay)
				{
					Task task = this.mGraphDesigner.SelectedNodes[0].Task;
					if (this.mNodeDesignerTaskMap != null && this.mNodeDesignerTaskMap.Count > 0)
					{
						NodeDesigner nodeDesigner = this.mGraphDesigner.SelectedNodes[0].Task.NodeData.NodeDesigner as NodeDesigner;
						if (nodeDesigner != null && this.mNodeDesignerTaskMap.ContainsKey(nodeDesigner))
						{
							task = this.mNodeDesignerTaskMap[nodeDesigner];
						}
					}
					if (this.mTaskInspector.DrawTaskInspector(this.mActiveBehaviorSource, this.mTaskList, task, !this.ViewOnlyMode()) && !Application.isPlaying)
					{
						this.SaveBehavior();
					}
				}
				else
				{
					GUILayout.Space(5f);
					if (this.mGraphDesigner.SelectedNodes.Count > 1)
					{
						GUILayout.Label("Only one task can be selected at a time to\n view its properties.", BehaviorDesignerUtility.LabelWrapGUIStyle, new GUILayoutOption[]
						{
							GUILayout.Width(285f)
						});
					}
					else
					{
						GUILayout.Label("Select a task from the tree to\nview its properties.", BehaviorDesignerUtility.LabelWrapGUIStyle, new GUILayoutOption[]
						{
							GUILayout.Width(285f)
						});
					}
				}
			}
			GUILayout.EndArea();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007D90 File Offset: 0x00005F90
		private bool DrawGraphArea()
		{
			if ((int)Event.current.type != 6 && !this.mTakingScreenshot)
			{
				Vector2 vector = GUI.BeginScrollView(new Rect(this.mGraphRect.x, this.mGraphRect.y, this.mGraphRect.width + 15f, this.mGraphRect.height + 15f), this.mGraphScrollPosition, new Rect(0f, 0f, this.mGraphScrollSize.x, this.mGraphScrollSize.y), true, true);
				if (vector != this.mGraphScrollPosition && (int)Event.current.type != 9 && (int)Event.current.type != 11)
				{
					this.mGraphOffset -= (vector - this.mGraphScrollPosition) / this.mGraphZoom;
					this.mGraphScrollPosition = vector;
					this.mGraphDesigner.GraphDirty();
				}
				GUI.EndScrollView();
			}
			GUI.Box(this.mGraphRect, string.Empty, BehaviorDesignerUtility.GraphBackgroundGUIStyle);
			this.DrawGrid();
			EditorZoomArea.Begin(this.mGraphRect, this.mGraphZoom);
			Vector2 mousePosition;
			if (!this.GetMousePositionInGraph(out mousePosition))
			{
				mousePosition = new Vector2(-1f, -1f);
			}
			bool result = false;
			if (this.mGraphDesigner != null && this.mGraphDesigner.DrawNodes(mousePosition, this.mGraphOffset))
			{
				result = true;
			}
			if (this.mTakingScreenshot && (int)Event.current.type == 7)
			{
				this.RenderScreenshotTile();
			}
			if (this.mIsSelecting)
			{
				GUI.Box(this.GetSelectionArea(), string.Empty, BehaviorDesignerUtility.SelectionGUIStyle);
			}
			EditorZoomArea.End();
			this.DrawGraphStatus();
			this.DrawSelectedTaskDescription();
			return result;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007F64 File Offset: 0x00006164
		private void DrawGrid()
		{
			if (!BehaviorDesignerPreferences.GetBool(BDPreferences.SnapToGrid))
			{
				return;
			}
			this.mGridMaterial.SetPass((!EditorGUIUtility.isProSkin) ? 1 : 0);
			GL.PushMatrix();
			GL.Begin(1);
			this.DrawGridLines(10f * this.mGraphZoom, new Vector2(this.mGraphOffset.x % 10f * this.mGraphZoom, this.mGraphOffset.y % 10f * this.mGraphZoom));
			GL.End();
			GL.PopMatrix();
			this.mGridMaterial.SetPass((!EditorGUIUtility.isProSkin) ? 3 : 2);
			GL.PushMatrix();
			GL.Begin(1);
			this.DrawGridLines(50f * this.mGraphZoom, new Vector2(this.mGraphOffset.x % 50f * this.mGraphZoom, this.mGraphOffset.y % 50f * this.mGraphZoom));
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00008070 File Offset: 0x00006270
		private void DrawGridLines(float gridSize, Vector2 offset)
		{
			float num = this.mGraphRect.x + offset.x;
			if (offset.x < 0f)
			{
				num += gridSize;
			}
			for (float num2 = num; num2 < this.mGraphRect.x + this.mGraphRect.width; num2 += gridSize)
			{
				this.DrawLine(new Vector2(num2, this.mGraphRect.y), new Vector2(num2, this.mGraphRect.y + this.mGraphRect.height));
			}
			float num3 = this.mGraphRect.y + offset.y;
			if (offset.y < 0f)
			{
				num3 += gridSize;
			}
			for (float num4 = num3; num4 < this.mGraphRect.y + this.mGraphRect.height; num4 += gridSize)
			{
				this.DrawLine(new Vector2(this.mGraphRect.x, num4), new Vector2(this.mGraphRect.x + this.mGraphRect.width, num4));
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008188 File Offset: 0x00006388
		private void DrawLine(Vector2 p1, Vector2 p2)
		{
			GL.Vertex(p1);
			GL.Vertex(p2);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000081A0 File Offset: 0x000063A0
		private void DrawGraphStatus()
		{
			if (!this.mGraphStatus.Equals(string.Empty))
			{
				GUI.Label(new Rect(this.mGraphRect.x + 5f, this.mGraphRect.y + 5f, this.mGraphRect.width, 30f), this.mGraphStatus, BehaviorDesignerUtility.GraphStatusGUIStyle);
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000820C File Offset: 0x0000640C
		private void DrawSelectedTaskDescription()
		{
			TaskDescriptionAttribute[] array;
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.ShowTaskDescription) && this.mGraphDesigner.SelectedNodes.Count == 1 && (array = (this.mGraphDesigner.SelectedNodes[0].Task.GetType().GetCustomAttributes(typeof(TaskDescriptionAttribute), false) as TaskDescriptionAttribute[])).Length > 0)
			{
				float num;
				float num2;
				BehaviorDesignerUtility.TaskCommentGUIStyle.CalcMinMaxWidth(new GUIContent(array[0].Description), out num, out num2);
				float num3 = Mathf.Min(400f, num2 + 20f);
				float num4 = Mathf.Min(300f, BehaviorDesignerUtility.TaskCommentGUIStyle.CalcHeight(new GUIContent(array[0].Description), num3)) + 3f;
				GUI.Box(new Rect(this.mGraphRect.x + 5f, this.mGraphRect.yMax - num4 - 5f, num3, num4), string.Empty, BehaviorDesignerUtility.TaskDescriptionGUIStyle);
				GUI.Box(new Rect(this.mGraphRect.x + 2f, this.mGraphRect.yMax - num4 - 5f, num3, num4), array[0].Description, BehaviorDesignerUtility.TaskCommentGUIStyle);
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00008348 File Offset: 0x00006548
		private void AddBehavior()
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}
			if (Selection.activeGameObject != null)
			{
				GameObject activeGameObject = Selection.activeGameObject;
				this.mActiveObject = Selection.activeObject;
				this.mGraphDesigner = ScriptableObject.CreateInstance<GraphDesigner>();
				Type type = Type.GetType("BehaviorDesigner.Runtime.BehaviorTree, Assembly-CSharp");
				if (type == null)
				{
					type = Type.GetType("BehaviorDesigner.Runtime.BehaviorTree, Assembly-CSharp-firstpass");
				}
				Behavior behavior = BehaviorUndo.AddComponent(activeGameObject, type) as Behavior;
				Behavior[] components = activeGameObject.GetComponents<Behavior>();
				HashSet<string> hashSet = new HashSet<string>();
				string text = string.Empty;
				for (int i = 0; i < components.Length; i++)
				{
					text = components[i].GetBehaviorSource().behaviorName;
					int num = 2;
					while (hashSet.Contains(text))
					{
						text = string.Format("{0} {1}", components[i].GetBehaviorSource().behaviorName, num);
						num++;
					}
					components[i].GetBehaviorSource().behaviorName = text;
					hashSet.Add(components[i].GetBehaviorSource().behaviorName);
				}
				this.LoadBehavior(behavior.GetBehaviorSource(), false);
				base.Repaint();
				if (BehaviorDesignerPreferences.GetBool(BDPreferences.AddGameGUIComponent))
				{
					type = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorGameGUI");
					BehaviorUndo.AddComponent(activeGameObject, type);
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00008488 File Offset: 0x00006688
		private void RemoveBehavior()
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}
			if (this.mActiveObject as GameObject != null && (this.mActiveBehaviorSource.EntryTask == null || (this.mActiveBehaviorSource.EntryTask != null && EditorUtility.DisplayDialog("Remove Behavior Tree", "Are you sure you want to remove this behavior tree?", "Yes", "No"))))
			{
				GameObject gameObject = this.mActiveObject as GameObject;
				int num = this.IndexForBehavior(this.mActiveBehaviorSource.Owner);
				BehaviorUndo.DestroyObject(this.mActiveBehaviorSource.Owner.GetObject(), true);
				num--;
				if (num == -1 && gameObject.GetComponents<Behavior>().Length > 0)
				{
					num = 0;
				}
				if (num > -1)
				{
					this.LoadBehavior(gameObject.GetComponents<Behavior>()[num].GetBehaviorSource(), true);
				}
				else
				{
					this.ClearGraph();
				}
				this.ClearBreadcrumbMenu();
				base.Repaint();
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00008578 File Offset: 0x00006778
		private int IndexForBehavior(IBehavior behavior)
		{
			if (behavior.GetObject() as Behavior)
			{
				Behavior[] components = (behavior.GetObject() as Behavior).gameObject.GetComponents<Behavior>();
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].Equals(behavior))
					{
						return i;
					}
				}
				return -1;
			}
			return 0;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000085D8 File Offset: 0x000067D8
		public NodeDesigner AddTask(Type type, bool useMousePosition)
		{
			if ((this.mActiveObject as GameObject == null && this.mActiveObject as ExternalBehavior == null) || EditorApplication.isPlaying)
			{
				return null;
			}
			Vector2 vector= new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom), 150f);
			if (useMousePosition)
			{
				this.GetMousePositionInGraph(out vector);
			}
			vector -= this.mGraphOffset;
			GameObject gameObject = this.mActiveObject as GameObject;
			if (gameObject != null && gameObject.GetComponent<Behavior>() == null)
			{
				this.AddBehavior();
			}
			BehaviorUndo.RegisterUndo("Add", this.mActiveBehaviorSource.Owner.GetObject());
			NodeDesigner result;
			if ((result = this.mGraphDesigner.AddNode(this.mActiveBehaviorSource, type, vector)) != null)
			{
				this.SaveBehavior();
				return result;
			}
			return null;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000086D0 File Offset: 0x000068D0
		public bool IsReferencingTasks()
		{
			return this.mTaskInspector.ActiveReferenceTask != null;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000086E4 File Offset: 0x000068E4
		public bool IsReferencingField(FieldInfo fieldInfo)
		{
			return fieldInfo.Equals(this.mTaskInspector.ActiveReferenceTaskFieldInfo);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000086F8 File Offset: 0x000068F8
		private void DisableReferenceTasks()
		{
			if (this.IsReferencingTasks())
			{
				this.ToggleReferenceTasks();
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000870C File Offset: 0x0000690C
		public void ToggleReferenceTasks()
		{
			this.ToggleReferenceTasks(null, null);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00008718 File Offset: 0x00006918
		public void ToggleReferenceTasks(Task task, FieldInfo fieldInfo)
		{
			bool flag = !this.IsReferencingTasks();
			this.mTaskInspector.SetActiveReferencedTasks((!flag) ? null : task, (!flag) ? null : fieldInfo);
			this.UpdateGraphStatus();
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000875C File Offset: 0x0000695C
		private void ReferenceTask(NodeDesigner nodeDesigner)
		{
			if (nodeDesigner != null && this.mTaskInspector.ReferenceTasks(nodeDesigner.Task))
			{
				this.SaveBehavior();
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00008794 File Offset: 0x00006994
		public void IdentifyNode(NodeDesigner nodeDesigner)
		{
			this.mGraphDesigner.IdentifyNode(nodeDesigner);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000087A4 File Offset: 0x000069A4
		private void TakeScreenshot()
		{
			this.mScreenshotPath = EditorUtility.SaveFilePanel("Save Screenshot", "Assets", this.mActiveBehaviorSource.behaviorName + "Screenshot.png", "png");
			if (this.mScreenshotPath.Length != 0 && Application.dataPath.Length < this.mScreenshotPath.Length)
			{
				this.mTakingScreenshot = true;
				this.mScreenshotGraphSize = this.mGraphDesigner.GraphSize(this.mGraphOffset);
				this.mGraphDesigner.GraphDirty();
				if (this.mScreenshotGraphSize.width == 0f || this.mScreenshotGraphSize.height == 0f)
				{
					this.mScreenshotGraphSize = new Rect(0f, 0f, 100f, 100f);
				}
				this.mScreenshotStartGraphZoom = this.mGraphZoom;
				this.mScreenshotStartGraphOffset = this.mGraphOffset;
				this.mGraphZoom = 1f;
				this.mGraphOffset.x = this.mGraphOffset.x - (this.mScreenshotGraphSize.xMin - 10f);
				this.mGraphOffset.y = this.mGraphOffset.y - (this.mScreenshotGraphSize.yMin - 10f);
				this.mScreenshotGraphOffset = this.mGraphOffset;
				this.mScreenshotGraphSize.Set(this.mScreenshotGraphSize.xMin - 9f, this.mScreenshotGraphSize.yMin, this.mScreenshotGraphSize.width + 18f, this.mScreenshotGraphSize.height + 18f);
				this.mScreenshotTexture = new Texture2D((int)this.mScreenshotGraphSize.width, (int)this.mScreenshotGraphSize.height, (TextureFormat)3, false);
				base.Repaint();
			}
			else if (Path.GetExtension(this.mScreenshotPath).Equals(".png"))
			{
				Debug.LogError("Error: Unable to save screenshot. The save location must be within the Asset directory.");
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00008994 File Offset: 0x00006B94
		private void RenderScreenshotTile()
		{
			float num = Mathf.Min(this.mGraphRect.width, this.mScreenshotGraphSize.width - (this.mGraphOffset.x - this.mScreenshotGraphOffset.x));
			float num2 = Mathf.Min(this.mGraphRect.height, this.mScreenshotGraphSize.height + (this.mGraphOffset.y - this.mScreenshotGraphOffset.y));
			Rect rect = new Rect(this.mGraphRect.x, 39f + this.mGraphRect.height - num2 - 7f, num, num2);
			this.mScreenshotTexture.ReadPixels(rect, -(int)(this.mGraphOffset.x - this.mScreenshotGraphOffset.x), (int)(this.mScreenshotGraphSize.height - num2 + (this.mGraphOffset.y - this.mScreenshotGraphOffset.y)));
			this.mScreenshotTexture.Apply(false);
			if (this.mScreenshotGraphSize.xMin + num - (this.mGraphOffset.x - this.mScreenshotGraphOffset.x) < this.mScreenshotGraphSize.xMax)
			{
				this.mGraphOffset.x = this.mGraphOffset.x - (num - 1f);
				this.mGraphDesigner.GraphDirty();
				base.Repaint();
			}
			else if (this.mScreenshotGraphSize.yMin + num2 - (this.mGraphOffset.y - this.mScreenshotGraphOffset.y) < this.mScreenshotGraphSize.yMax)
			{
				this.mGraphOffset.y = this.mGraphOffset.y - (num2 - 1f);
				this.mGraphOffset.x = this.mScreenshotGraphOffset.x;
				this.mGraphDesigner.GraphDirty();
				base.Repaint();
			}
			else
			{
				this.SaveScreenshot();
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008B74 File Offset: 0x00006D74
		private void SaveScreenshot()
		{
			byte[] bytes = this.mScreenshotTexture.EncodeToPNG();
			Object.DestroyImmediate(this.mScreenshotTexture, true);
			File.WriteAllBytes(this.mScreenshotPath, bytes);
			string text = string.Format("Assets/{0}", this.mScreenshotPath.Substring(Application.dataPath.Length + 1));
			AssetDatabase.ImportAsset(text);
			this.mTakingScreenshot = false;
			this.mGraphZoom = this.mScreenshotStartGraphZoom;
			this.mGraphOffset = this.mScreenshotStartGraphOffset;
			this.mGraphDesigner.GraphDirty();
			base.Repaint();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008C00 File Offset: 0x00006E00
		private void HandleEvents()
		{
			if (this.mTakingScreenshot)
			{
				return;
			}
			if ((int)Event.current.type != 1 && this.CheckForAutoScroll())
			{
				base.Repaint();
				return;
			}
			if ((int)Event.current.type == 7 || (int)Event.current.type == 8)
			{
				return;
			}
			switch ((int)Event.current.type)
			{
			case 0:
				if (Event.current.button == 0 && (int)Event.current.modifiers != 2)
				{
					Vector2 mousePosition;
					if (this.GetMousePositionInGraph(out mousePosition))
					{
						if (this.LeftMouseDown(Event.current.clickCount, mousePosition))
						{
							Event.current.Use();
						}
					}
					else if (this.GetMousePositionInPropertiesPane(out mousePosition) && this.mBehaviorToolbarSelection == 2 && this.mVariableInspector.LeftMouseDown(this.mActiveBehaviorSource, this.mActiveBehaviorSource, mousePosition))
					{
						Event.current.Use();
						base.Repaint();
					}
				}
				else if ((Event.current.button == 1 || ((int)Event.current.modifiers == 2 && Event.current.button == 0)) && this.RightMouseDown())
				{
					Event.current.Use();
				}
				break;
			case 1:
				if (Event.current.button == 0 && (int)Event.current.modifiers != 2)
				{
					if (this.LeftMouseRelease())
					{
						Event.current.Use();
					}
				}
				else if ((Event.current.button == 1 || ((int)Event.current.modifiers == 2 && Event.current.button == 0)) && this.mShowRightClickMenu)
				{
					this.mShowRightClickMenu = false;
					this.mRightClickMenu.ShowAsContext();
					Event.current.Use();
				}
				break;
			case 2:
				if (this.MouseMove())
				{
					Event.current.Use();
				}
				break;
			case 3:
				if (Event.current.button == 0)
				{
					if (this.LeftMouseDragged())
					{
						Event.current.Use();
					}
					else if ((int)Event.current.modifiers == 4 && this.MousePan())
					{
						Event.current.Use();
					}
				}
				else if (Event.current.button == 2 && this.MousePan())
				{
					Event.current.Use();
				}
				break;
			case 4:
				if ((int)Event.current.keyCode == 310 ||(int)Event.current.keyCode == 309)
				{
					this.mCommandDown = true;
				}
				break;
			case 5:
				if ((int)Event.current.keyCode == 127 ||(int)Event.current.keyCode == 8 || Event.current.commandName.Equals("Delete"))
				{
					if (this.PropertiesInspectorHasFocus() || EditorApplication.isPlaying)
					{
						return;
					}
					this.DeleteNodes();
					Event.current.Use();
				}
				else if ((int)Event.current.keyCode == 13 ||(int)Event.current.keyCode == 271)
				{
					if (this.mBehaviorToolbarSelection == 2 && this.mVariableInspector.HasFocus())
					{
						if (this.mVariableInspector.ClearFocus(true, this.mActiveBehaviorSource))
						{
							this.SaveBehavior();
						}
						base.Repaint();
					}
					else
					{
						this.DisableReferenceTasks();
					}
					Event.current.Use();
				}
				else if ((int)Event.current.keyCode == 27)
				{
					this.DisableReferenceTasks();
				}
				else if ((int)Event.current.keyCode == 310 ||(int)Event.current.keyCode == 309)
				{
					this.mCommandDown = false;
				}
				break;
			case 6:
				if (BehaviorDesignerPreferences.GetBool(BDPreferences.MouseWhellScrolls) && !this.mCommandDown)
				{
					this.MousePan();
				}
				else if (this.MouseZoom())
				{
					Event.current.Use();
				}
				break;
			case 13:
				if (EditorApplication.isPlaying)
				{
					return;
				}
				if (Event.current.commandName.Equals("Copy") || Event.current.commandName.Equals("Paste") || Event.current.commandName.Equals("Cut") || Event.current.commandName.Equals("SelectAll") || Event.current.commandName.Equals("Duplicate"))
				{
					if (this.PropertiesInspectorHasFocus() || EditorApplication.isPlaying || this.ViewOnlyMode())
					{
						return;
					}
					Event.current.Use();
				}
				break;
			case 14:
				if (this.PropertiesInspectorHasFocus() || EditorApplication.isPlaying || this.ViewOnlyMode())
				{
					return;
				}
				if (Event.current.commandName.Equals("Copy"))
				{
					this.CopyNodes();
					Event.current.Use();
				}
				else if (Event.current.commandName.Equals("Paste"))
				{
					this.PasteNodes();
					Event.current.Use();
				}
				else if (Event.current.commandName.Equals("Cut"))
				{
					this.CutNodes();
					Event.current.Use();
				}
				else if (Event.current.commandName.Equals("SelectAll"))
				{
					this.mGraphDesigner.SelectAll();
					Event.current.Use();
				}
				else if (Event.current.commandName.Equals("Duplicate"))
				{
					this.DuplicateNodes();
					Event.current.Use();
				}
				break;
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00009228 File Offset: 0x00007428
		private bool CheckForAutoScroll()
		{
			Vector2 vector;
			if (!this.GetMousePositionInGraph(out vector))
			{
				return false;
			}
			if (this.mGraphScrollRect.Contains(this.mCurrentMousePosition))
			{
				return false;
			}
			if (this.mIsDragging || this.mIsSelecting || this.mGraphDesigner.ActiveNodeConnection != null)
			{
				Vector2 zero = Vector2.zero;
				if (this.mCurrentMousePosition.y < this.mGraphScrollRect.yMin + 15f)
				{
					zero.y = 3f;
				}
				else if (this.mCurrentMousePosition.y > this.mGraphScrollRect.yMax - 15f)
				{
					zero.y = -3f;
				}
				if (this.mCurrentMousePosition.x < this.mGraphScrollRect.xMin + 15f)
				{
					zero.x = 3f;
				}
				else if (this.mCurrentMousePosition.x > this.mGraphScrollRect.xMax - 15f)
				{
					zero.x = -3f;
				}
				this.ScrollGraph(zero);
				if (this.mIsDragging)
				{
					this.mGraphDesigner.DragSelectedNodes(-zero / this.mGraphZoom,(int)Event.current.modifiers != 4);
				}
				if (this.mIsSelecting)
				{
					this.mSelectStartPosition += zero / this.mGraphZoom;
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000093B8 File Offset: 0x000075B8
		private bool MouseMove()
		{
			Vector2 point;
			if (!this.GetMousePositionInGraph(out point))
			{
				return false;
			}
			NodeDesigner nodeDesigner = this.mGraphDesigner.NodeAt(point, this.mGraphOffset);
			if (this.mGraphDesigner.HoverNode != null && ((nodeDesigner != null && !this.mGraphDesigner.HoverNode.Equals(nodeDesigner)) || !this.mGraphDesigner.HoverNode.HoverBarAreaContains(point, this.mGraphOffset)))
			{
				this.mGraphDesigner.ClearHover();
				base.Repaint();
			}
			if (nodeDesigner && !nodeDesigner.IsEntryDisplay && !this.ViewOnlyMode())
			{
				this.mGraphDesigner.Hover(nodeDesigner);
			}
			return this.mGraphDesigner.HoverNode != null;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000948C File Offset: 0x0000768C
		private bool LeftMouseDown(int clickCount, Vector2 mousePosition)
		{
			if (this.PropertiesInspectorHasFocus())
			{
				this.mTaskInspector.ClearFocus();
				this.mVariableInspector.ClearFocus(false, null);
				base.Repaint();
			}
			NodeDesigner nodeDesigner = this.mGraphDesigner.NodeAt(mousePosition, this.mGraphOffset);
			if ((int)Event.current.modifiers == 4)
			{
				this.mNodeClicked = this.mGraphDesigner.IsSelected(nodeDesigner);
				return false;
			}
			if (this.IsReferencingTasks())
			{
				if (nodeDesigner == null)
				{
					this.DisableReferenceTasks();
				}
				else
				{
					this.ReferenceTask(nodeDesigner);
				}
				return true;
			}
			if (nodeDesigner != null)
			{
				if (this.mGraphDesigner.HoverNode != null && !nodeDesigner.Equals(this.mGraphDesigner.HoverNode))
				{
					this.mGraphDesigner.ClearHover();
					this.mGraphDesigner.Hover(nodeDesigner);
				}
				NodeConnection nodeConnection;
				if (!this.ViewOnlyMode() && (nodeConnection = nodeDesigner.NodeConnectionRectContains(mousePosition, this.mGraphOffset)) != null)
				{
					if (this.mGraphDesigner.NodeCanOriginateConnection(nodeDesigner, nodeConnection))
					{
						this.mGraphDesigner.ActiveNodeConnection = nodeConnection;
					}
					return true;
				}
				if (nodeDesigner.Contains(mousePosition, this.mGraphOffset, false))
				{
					this.mKeepTasksSelected = false;
					if (this.mGraphDesigner.IsSelected(nodeDesigner))
					{
						if ((int)Event.current.modifiers == 2)
						{
							this.mKeepTasksSelected = true;
							this.mGraphDesigner.Deselect(nodeDesigner);
						}
						else if ((int)Event.current.modifiers == 1)
						{
							nodeDesigner.Task.NodeData.Collapsed = !nodeDesigner.Task.NodeData.Collapsed;
							this.mGraphDesigner.DeselectWithParent(nodeDesigner);
						}
						else if (clickCount == 2)
						{
							if (this.mBehaviorToolbarSelection != 3 && BehaviorDesignerPreferences.GetBool(BDPreferences.OpenInspectorOnTaskDoubleClick))
							{
								this.mBehaviorToolbarSelection = 3;
							}
							else if (nodeDesigner.Task is BehaviorReference)
							{
								BehaviorReference behaviorReference = nodeDesigner.Task as BehaviorReference;
								if (behaviorReference.GetExternalBehaviors() != null && behaviorReference.GetExternalBehaviors().Length > 0 && behaviorReference.GetExternalBehaviors()[0] != null)
								{
									if (this.mLockActiveGameObject)
									{
										this.LoadBehavior(behaviorReference.GetExternalBehaviors()[0].GetBehaviorSource(), false);
									}
									else
									{
										Selection.activeObject = behaviorReference.GetExternalBehaviors()[0];
									}
								}
							}
						}
					}
					else
					{
						if ((int)Event.current.modifiers != 1 &&(int)Event.current.modifiers != 2)
						{
							this.mGraphDesigner.ClearNodeSelection();
							this.mGraphDesigner.ClearConnectionSelection();
							if (BehaviorDesignerPreferences.GetBool(BDPreferences.OpenInspectorOnTaskSelection))
							{
								this.mBehaviorToolbarSelection = 3;
							}
						}
						else
						{
							this.mKeepTasksSelected = true;
						}
						this.mGraphDesigner.Select(nodeDesigner);
					}
					this.mNodeClicked = this.mGraphDesigner.IsSelected(nodeDesigner);
					return true;
				}
			}
			if (this.mGraphDesigner.HoverNode != null)
			{
				bool flag = false;
				if (this.mGraphDesigner.HoverNode.HoverBarButtonClick(mousePosition, this.mGraphOffset, ref flag))
				{
					this.SaveBehavior();
					if (flag && this.mGraphDesigner.HoverNode.Task.NodeData.Collapsed)
					{
						this.mGraphDesigner.DeselectWithParent(this.mGraphDesigner.HoverNode);
					}
					return true;
				}
			}
			List<NodeConnection> list = new List<NodeConnection>();
			this.mGraphDesigner.NodeConnectionsAt(mousePosition, this.mGraphOffset, ref list);
			if (list.Count > 0)
			{
				if ((int)Event.current.modifiers != 1 &&(int)Event.current.modifiers != 2)
				{
					this.mGraphDesigner.ClearNodeSelection();
					this.mGraphDesigner.ClearConnectionSelection();
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (this.mGraphDesigner.IsSelected(list[i]))
					{
						if ((int)Event.current.modifiers == 2)
						{
							this.mGraphDesigner.Deselect(list[i]);
						}
					}
					else
					{
						this.mGraphDesigner.Select(list[i]);
					}
				}
				return true;
			}
			if ((int)Event.current.modifiers != 1)
			{
				this.mGraphDesigner.ClearNodeSelection();
				this.mGraphDesigner.ClearConnectionSelection();
			}
			this.mSelectStartPosition = mousePosition;
			this.mIsSelecting = true;
			this.mIsDragging = false;
			this.mDragDelta = Vector2.zero;
			this.mNodeClicked = false;
			return true;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00009908 File Offset: 0x00007B08
		private bool LeftMouseDragged()
		{
			Vector2 vector;
			if (!this.GetMousePositionInGraph(out vector))
			{
				return false;
			}
			if ((int)Event.current.modifiers != 4)
			{
				if (this.IsReferencingTasks())
				{
					return true;
				}
				if (this.mIsSelecting)
				{
					this.mGraphDesigner.ClearNodeSelection();
					List<NodeDesigner> list = this.mGraphDesigner.NodesAt(this.GetSelectionArea(), this.mGraphOffset);
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							this.mGraphDesigner.Select(list[i]);
						}
					}
					return true;
				}
				if (this.mGraphDesigner.ActiveNodeConnection != null)
				{
					return true;
				}
			}
			if (this.mNodeClicked && !this.ViewOnlyMode())
			{
				Vector2 vector2 = Vector2.zero;
				if (BehaviorDesignerPreferences.GetBool(BDPreferences.SnapToGrid))
				{
					this.mDragDelta += Event.current.delta;
					if (Mathf.Abs(this.mDragDelta.x) > 10f)
					{
						float num = Mathf.Abs(this.mDragDelta.x) % 10f;
						vector2.x = (Mathf.Abs(this.mDragDelta.x) - num) * Mathf.Sign(this.mDragDelta.x);
						this.mDragDelta.x = num * Mathf.Sign(this.mDragDelta.x);
					}
					if (Mathf.Abs(this.mDragDelta.y) > 10f)
					{
						float num2 = Mathf.Abs(this.mDragDelta.y) % 10f;
						vector2.y = (Mathf.Abs(this.mDragDelta.y) - num2) * Mathf.Sign(this.mDragDelta.y);
						this.mDragDelta.y = num2 * Mathf.Sign(this.mDragDelta.y);
					}
				}
				else
				{
					vector2 = Event.current.delta;
				}
				bool flag = this.mGraphDesigner.DragSelectedNodes(vector2 / this.mGraphZoom,(int)Event.current.modifiers != 4);
				if (flag)
				{
					this.mKeepTasksSelected = true;
				}
				this.mIsDragging = true;
				return flag;
			}
			return false;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00009B44 File Offset: 0x00007D44
		private bool LeftMouseRelease()
		{
			this.mNodeClicked = false;
			if (this.IsReferencingTasks())
			{
				if (!this.mTaskInspector.IsActiveTaskArray() && !this.mTaskInspector.IsActiveTaskNull())
				{
					this.DisableReferenceTasks();
					base.Repaint();
				}
				Vector2 vector;
				if (!this.GetMousePositionInGraph(out vector))
				{
					this.mGraphDesigner.ActiveNodeConnection = null;
					return false;
				}
				return true;
			}
			else
			{
				if (this.mIsSelecting)
				{
					this.mIsSelecting = false;
					return true;
				}
				if (this.mIsDragging)
				{
					BehaviorUndo.RegisterUndo("Drag", this.mActiveBehaviorSource.Owner.GetObject());
					this.SaveBehavior();
					this.mIsDragging = false;
					this.mDragDelta = Vector3.zero;
					return true;
				}
				if (this.mGraphDesigner.ActiveNodeConnection != null)
				{
					Vector2 point;
					if (!this.GetMousePositionInGraph(out point))
					{
						this.mGraphDesigner.ActiveNodeConnection = null;
						return false;
					}
					NodeDesigner nodeDesigner = this.mGraphDesigner.NodeAt(point, this.mGraphOffset);
					if (nodeDesigner != null && !nodeDesigner.Equals(this.mGraphDesigner.ActiveNodeConnection.OriginatingNodeDesigner) && this.mGraphDesigner.NodeCanAcceptConnection(nodeDesigner, this.mGraphDesigner.ActiveNodeConnection))
					{
						this.mGraphDesigner.ConnectNodes(this.mActiveBehaviorSource, nodeDesigner);
						BehaviorUndo.RegisterUndo("Task Connection", this.mActiveBehaviorSource.Owner.GetObject());
						this.SaveBehavior();
					}
					else
					{
						this.mGraphDesigner.ActiveNodeConnection = null;
					}
					return true;
				}
				else
				{
					if ((int)Event.current.modifiers == 1 || this.mKeepTasksSelected)
					{
						return false;
					}
					Vector2 point2;
					if (!this.GetMousePositionInGraph(out point2))
					{
						return false;
					}
					NodeDesigner nodeDesigner2 = this.mGraphDesigner.NodeAt(point2, this.mGraphOffset);
					if (nodeDesigner2 != null && !this.mGraphDesigner.IsSelected(nodeDesigner2))
					{
						this.mGraphDesigner.DeselectAllExcept(nodeDesigner2);
					}
					return true;
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00009D40 File Offset: 0x00007F40
		private bool RightMouseDown()
		{
			if (this.IsReferencingTasks())
			{
				this.DisableReferenceTasks();
				return false;
			}
			Vector2 point;
			if (!this.GetMousePositionInGraph(out point))
			{
				return false;
			}
			NodeDesigner nodeDesigner = this.mGraphDesigner.NodeAt(point, this.mGraphOffset);
			if (nodeDesigner == null || !this.mGraphDesigner.IsSelected(nodeDesigner))
			{
				this.mGraphDesigner.ClearNodeSelection();
				this.mGraphDesigner.ClearConnectionSelection();
				if (nodeDesigner != null)
				{
					this.mGraphDesigner.Select(nodeDesigner);
				}
			}
			if (this.mGraphDesigner.HoverNode != null)
			{
				this.mGraphDesigner.ClearHover();
			}
			this.BuildRightClickMenu(nodeDesigner);
			return true;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00009DF8 File Offset: 0x00007FF8
		private bool MouseZoom()
		{
			Vector2 vector;
			if (!this.GetMousePositionInGraph(out vector))
			{
				return false;
			}
			float num = -Event.current.delta.y / 150f;
			this.mGraphZoom += num;
			this.mGraphZoom = Mathf.Clamp(this.mGraphZoom, 0.4f, 1f);
			Vector2 vector2;
			this.GetMousePositionInGraph(out vector2);
			this.mGraphOffset += vector2 - vector;
			this.mGraphScrollPosition += vector2 - vector;
			this.mGraphDesigner.GraphDirty();
			return true;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00009E9C File Offset: 0x0000809C
		private bool MousePan()
		{
			Vector2 vector;
			if (!this.GetMousePositionInGraph(out vector))
			{
				return false;
			}
			Vector2 vector2 = Event.current.delta;
			if ((int)Event.current.type == 6)
			{
				vector2 *= -1.5f;
				if ((int)Event.current.modifiers == 2)
				{
					vector2.x = vector2.y;
					vector2.y = 0f;
				}
			}
			this.ScrollGraph(vector2);
			return true;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00009F14 File Offset: 0x00008114
		private void ScrollGraph(Vector2 amount)
		{
			this.mGraphOffset += amount / this.mGraphZoom;
			this.mGraphScrollPosition -= amount;
			this.mGraphDesigner.GraphDirty();
			base.Repaint();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009F64 File Offset: 0x00008164
		private bool PropertiesInspectorHasFocus()
		{
			return this.mTaskInspector.HasFocus() || this.mVariableInspector.HasFocus();
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00009F84 File Offset: 0x00008184
		private void AddTaskCallback(object obj)
		{
			this.AddTask((Type)obj, true);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00009F94 File Offset: 0x00008194
		private void ReplaceTaskCallback(object obj)
		{
			Type type = (Type)obj;
			if (this.mGraphDesigner.SelectedNodes.Count != 1 || this.mGraphDesigner.SelectedNodes[0].Task.GetType().Equals(type))
			{
				return;
			}
			if (this.mGraphDesigner.ReplaceSelectedNode(this.mActiveBehaviorSource, type))
			{
				this.SaveBehavior();
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000A004 File Offset: 0x00008204
		private void BehaviorSelectionCallback(object obj)
		{
			BehaviorSource behaviorSource = obj as BehaviorSource;
			if (behaviorSource.Owner is Behavior)
			{
				this.mActiveObject = (behaviorSource.Owner as Behavior).gameObject;
			}
			else
			{
				this.mActiveObject = (behaviorSource.Owner as ExternalBehavior);
			}
			if (!this.mLockActiveGameObject)
			{
				Selection.activeObject = this.mActiveObject;
			}
			this.LoadBehavior(behaviorSource, false);
			this.UpdateGraphStatus();
			if (EditorApplication.isPaused)
			{
				this.mUpdateNodeTaskMap = true;
				this.UpdateNodeTaskMap();
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000A090 File Offset: 0x00008290
		private void ToggleEnableState(object obj)
		{
			NodeDesigner nodeDesigner = obj as NodeDesigner;
			nodeDesigner.ToggleEnableState();
			this.SaveBehavior();
			base.Repaint();
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000A0B8 File Offset: 0x000082B8
		private void ToggleCollapseState(object obj)
		{
			NodeDesigner nodeDesigner = obj as NodeDesigner;
			if (nodeDesigner.ToggleCollapseState())
			{
				this.mGraphDesigner.DeselectWithParent(nodeDesigner);
			}
			this.SaveBehavior();
			base.Repaint();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000A0F0 File Offset: 0x000082F0
		private void ToggleBreakpoint(object obj)
		{
			NodeDesigner nodeDesigner = obj as NodeDesigner;
			nodeDesigner.ToggleBreakpoint();
			this.SaveBehavior();
			base.Repaint();
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000A118 File Offset: 0x00008318
		private void OpenInFileEditor(object obj)
		{
			NodeDesigner nodeDesigner = obj as NodeDesigner;
			TaskInspector.OpenInFileEditor(nodeDesigner.Task);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000A138 File Offset: 0x00008338
		private void SelectInProject(object obj)
		{
			NodeDesigner nodeDesigner = obj as NodeDesigner;
			TaskInspector.SelectInProject(nodeDesigner.Task);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000A158 File Offset: 0x00008358
		private void CopyNodes()
		{
			this.mCopiedTasks = this.mGraphDesigner.Copy(this.mGraphOffset, this.mGraphZoom);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000A178 File Offset: 0x00008378
		private void PasteNodes()
		{
			if (this.mActiveObject == null || EditorApplication.isPlaying)
			{
				return;
			}
			GameObject gameObject = this.mActiveObject as GameObject;
			if (gameObject != null && gameObject.GetComponent<Behavior>() == null)
			{
				this.AddBehavior();
			}
			if (this.mCopiedTasks != null && this.mCopiedTasks.Count > 0)
			{
				BehaviorUndo.RegisterUndo("Paste", this.mActiveBehaviorSource.Owner.GetObject());
			}
			this.mGraphDesigner.Paste(this.mActiveBehaviorSource, new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom) - this.mGraphOffset.x, 150f - this.mGraphOffset.y), this.mCopiedTasks, this.mGraphOffset, this.mGraphZoom);
			this.SaveBehavior();
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000A270 File Offset: 0x00008470
		private void CutNodes()
		{
			this.mCopiedTasks = this.mGraphDesigner.Copy(this.mGraphOffset, this.mGraphZoom);
			if (this.mCopiedTasks != null && this.mCopiedTasks.Count > 0)
			{
				BehaviorUndo.RegisterUndo("Cut", this.mActiveBehaviorSource.Owner.GetObject());
			}
			this.mGraphDesigner.Delete(this.mActiveBehaviorSource);
			this.SaveBehavior();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000A2E8 File Offset: 0x000084E8
		private void DuplicateNodes()
		{
			List<TaskSerializer> list = this.mGraphDesigner.Copy(this.mGraphOffset, this.mGraphZoom);
			if (list != null && list.Count > 0)
			{
				BehaviorUndo.RegisterUndo("Duplicate", this.mActiveBehaviorSource.Owner.GetObject());
			}
			this.mGraphDesigner.Paste(this.mActiveBehaviorSource, new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom) - this.mGraphOffset.x, 150f - this.mGraphOffset.y), list, this.mGraphOffset, this.mGraphZoom);
			this.SaveBehavior();
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000A3A0 File Offset: 0x000085A0
		private void DeleteNodes()
		{
			if (this.ViewOnlyMode())
			{
				return;
			}
			this.mGraphDesigner.Delete(this.mActiveBehaviorSource);
			this.SaveBehavior();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000A3D4 File Offset: 0x000085D4
		public void RemoveSharedVariableReferences(SharedVariable sharedVariable)
		{
			if (this.mGraphDesigner.RemoveSharedVariableReferences(sharedVariable))
			{
				this.SaveBehavior();
				base.Repaint();
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000A3F4 File Offset: 0x000085F4
		private void OnUndoRedo()
		{
			if (this.mActiveBehaviorSource != null)
			{
				this.LoadBehavior(this.mActiveBehaviorSource, true, false);
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000A410 File Offset: 0x00008610
		private void SetupSizes()
		{
			if (this.mPrevScreenWidth == (float)Screen.width && this.mPrevScreenHeight == (float)Screen.height && this.mPropertiesPanelOnLeft == BehaviorDesignerPreferences.GetBool(BDPreferences.PropertiesPanelOnLeft))
			{
				return;
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.PropertiesPanelOnLeft))
			{
				this.mFileToolBarRect = new Rect(300f, 0f, (float)(Screen.width - 300), 18f);
				this.mPropertyToolbarRect = new Rect(0f, 0f, 300f, 18f);
				this.mPropertyBoxRect = new Rect(0f, this.mPropertyToolbarRect.height, 300f, (float)Screen.height - this.mPropertyToolbarRect.height - 21f);
				this.mGraphRect = new Rect(300f, 18f, (float)(Screen.width - 300 - 15), (float)(Screen.height - 36 - 21 - 15));
				this.mPreferencesPaneRect = new Rect(300f + this.mGraphRect.width - 290f, (float)(18 + ((!EditorGUIUtility.isProSkin) ? 2 : 1)), 290f, 368f);
			}
			else
			{
				this.mFileToolBarRect = new Rect(0f, 0f, (float)(Screen.width - 300), 18f);
				this.mPropertyToolbarRect = new Rect((float)(Screen.width - 300), 0f, 300f, 18f);
				this.mPropertyBoxRect = new Rect((float)(Screen.width - 300), this.mPropertyToolbarRect.height, 300f, (float)Screen.height - this.mPropertyToolbarRect.height - 21f);
				this.mGraphRect = new Rect(0f, 18f, (float)(Screen.width - 300 - 15), (float)(Screen.height - 36 - 21 - 15));
				this.mPreferencesPaneRect = new Rect(this.mGraphRect.width - 290f, (float)(18 + ((!EditorGUIUtility.isProSkin) ? 2 : 1)), 290f, 368f);
			}
			this.mDebugToolBarRect = new Rect(this.mGraphRect.x, (float)(Screen.height - 18 - 21), this.mGraphRect.width + 15f, 18f);
			this.mGraphScrollRect.Set(this.mGraphRect.xMin + 15f, this.mGraphRect.yMin + 15f, this.mGraphRect.width - 30f, this.mGraphRect.height - 30f);
			if (this.mGraphScrollPosition == new Vector2(-1f, -1f))
			{
				this.mGraphScrollPosition = (this.mGraphScrollSize - new Vector2(this.mGraphRect.width, this.mGraphRect.height)) / 2f - 2f * new Vector2(15f, 15f);
			}
			this.mPrevScreenWidth = (float)Screen.width;
			this.mPrevScreenHeight = (float)Screen.height;
			this.mPropertiesPanelOnLeft = BehaviorDesignerPreferences.GetBool(BDPreferences.PropertiesPanelOnLeft);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000A76C File Offset: 0x0000896C
		private bool GetMousePositionInGraph(out Vector2 mousePosition)
		{
			mousePosition = this.mCurrentMousePosition;
			if (!this.mGraphRect.Contains(mousePosition))
			{
				return false;
			}
			if (this.mShowPrefPane && this.mPreferencesPaneRect.Contains(mousePosition))
			{
				return false;
			}
			mousePosition -= new Vector2(this.mGraphRect.xMin, this.mGraphRect.yMin);
			mousePosition /= this.mGraphZoom;
			return true;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000A804 File Offset: 0x00008A04
		private bool GetMousePositionInPropertiesPane(out Vector2 mousePosition)
		{
			mousePosition = this.mCurrentMousePosition;
			if (!this.mPropertyBoxRect.Contains(mousePosition))
			{
				return false;
			}
			mousePosition.x -= this.mPropertyBoxRect.xMin;
			mousePosition.y -= this.mPropertyBoxRect.yMin;
			return true;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000A868 File Offset: 0x00008A68
		private Rect GetSelectionArea()
		{
			Vector2 vector;
			if (this.GetMousePositionInGraph(out vector))
			{
				float num = (this.mSelectStartPosition.x >= vector.x) ? vector.x : this.mSelectStartPosition.x;
				float num2 = (this.mSelectStartPosition.x <= vector.x) ? vector.x : this.mSelectStartPosition.x;
				float num3 = (this.mSelectStartPosition.y >= vector.y) ? vector.y : this.mSelectStartPosition.y;
				float num4 = (this.mSelectStartPosition.y <= vector.y) ? vector.y : this.mSelectStartPosition.y;
				this.mSelectionArea = new Rect(num, num3, num2 - num, num4 - num3);
			}
			return this.mSelectionArea;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000A95C File Offset: 0x00008B5C
		public bool ViewOnlyMode()
		{
			if (Application.isPlaying)
			{
				return false;
			}
			if (this.mActiveBehaviorSource == null || this.mActiveBehaviorSource.Owner == null || this.mActiveBehaviorSource.Owner.Equals(null))
			{
				return false;
			}
			Behavior behavior = this.mActiveBehaviorSource.Owner.GetObject() as Behavior;
			return behavior != null && !BehaviorDesignerPreferences.GetBool(BDPreferences.EditablePrefabInstances) && (int)PrefabUtility.GetPrefabType(this.mActiveBehaviorSource.Owner.GetObject()) == 3;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000A9F4 File Offset: 0x00008BF4
		private BehaviorSource BehaviorSourceFromIBehaviorHistory(IBehavior behavior)
		{
			if (behavior == null)
			{
				return null;
			}
			if (behavior.GetObject() is GameObject)
			{
				Behavior[] components = (behavior.GetObject() as GameObject).GetComponents<Behavior>();
				for (int i = 0; i < components.Count<Behavior>(); i++)
				{
					if (components[i].GetBehaviorSource().BehaviorID == behavior.GetBehaviorSource().BehaviorID)
					{
						return components[i].GetBehaviorSource();
					}
				}
				return null;
			}
			return behavior.GetBehaviorSource();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000AA74 File Offset: 0x00008C74
		public void SaveBehavior()
		{
			if (this.mActiveBehaviorSource == null || this.ViewOnlyMode() || Application.isPlaying)
			{
				return;
			}
			this.mGraphDesigner.Save(this.mActiveBehaviorSource);
			this.CheckForErrors();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000AABC File Offset: 0x00008CBC
		private void CheckForErrors()
		{
			if (this.mErrorDetails != null)
			{
				for (int i = 0; i < this.mErrorDetails.Count; i++)
				{
					if (this.mErrorDetails[i].NodeDesigner != null)
					{
						this.mErrorDetails[i].NodeDesigner.HasError = false;
					}
				}
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.ErrorChecking))
			{
				this.mErrorDetails = ErrorCheck.CheckForErrors(this.mActiveBehaviorSource);
				if (this.mErrorDetails != null)
				{
					for (int j = 0; j < this.mErrorDetails.Count; j++)
					{
						if (!(this.mErrorDetails[j].NodeDesigner == null))
						{
							this.mErrorDetails[j].NodeDesigner.HasError = true;
						}
					}
				}
			}
			else
			{
				this.mErrorDetails = null;
			}
			if (ErrorWindow.instance != null)
			{
				ErrorWindow.instance.ErrorDetails = this.mErrorDetails;
				ErrorWindow.instance.Repaint();
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		public bool ContainsError(Task task, int index)
		{
			if (this.mErrorDetails == null)
			{
				return false;
			}
			for (int i = 0; i < this.mErrorDetails.Count; i++)
			{
				if (task == null)
				{
					if (!(this.mErrorDetails[i].NodeDesigner != null))
					{
						if (this.mErrorDetails[i].Index == index)
						{
							return true;
						}
					}
				}
				else if (!(this.mErrorDetails[i].NodeDesigner == null))
				{
					if (this.mErrorDetails[i].NodeDesigner.Task == task && this.mErrorDetails[i].Index == index)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000ACAC File Offset: 0x00008EAC
		private bool UpdateCheck()
		{
			if (this.mUpdateCheckRequest != null && this.mUpdateCheckRequest.isDone)
			{
				if (!string.IsNullOrEmpty(this.mUpdateCheckRequest.error))
				{
					this.mUpdateCheckRequest = null;
					return false;
				}
				if (!"1.5.7".ToString().Equals(this.mUpdateCheckRequest.text))
				{
					this.LatestVersion = this.mUpdateCheckRequest.text;
				}
				this.mUpdateCheckRequest = null;
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.UpdateCheck) && DateTime.Compare(this.LastUpdateCheck.AddDays(1.0), DateTime.UtcNow) < 0)
			{
				string text = string.Format("http://www.opsive.com/assets/BehaviorDesigner/UpdateCheck.php?version={0}&unityversion={1}&devplatform={2}&targetplatform={3}", new object[]
				{
					"1.5.7",
					Application.unityVersion,
					Application.platform,
					EditorUserBuildSettings.activeBuildTarget
				});
				this.mUpdateCheckRequest = new WWW(text);
				this.LastUpdateCheck = DateTime.UtcNow;
			}
			return this.mUpdateCheckRequest != null;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000ADBC File Offset: 0x00008FBC
		private void SaveAsAsset()
		{
			if (this.mActiveBehaviorSource == null)
			{
				return;
			}
			string text = EditorUtility.SaveFilePanel("Save Behavior Tree", "Assets", this.mActiveBehaviorSource.behaviorName + ".asset", "asset");
			if (text.Length != 0 && Application.dataPath.Length < text.Length)
			{
				Type type = Type.GetType("BehaviorDesigner.Runtime.ExternalBehaviorTree, Assembly-CSharp");
				if (type == null)
				{
					type = Type.GetType("BehaviorDesigner.Runtime.ExternalBehaviorTree, Assembly-CSharp-firstpass");
				}
				if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
				{
					BinarySerialization.Save(this.mActiveBehaviorSource);
				}
				else
				{
					JSONSerialization.Save(this.mActiveBehaviorSource);
				}
				ExternalBehavior externalBehavior = ScriptableObject.CreateInstance(type) as ExternalBehavior;
				externalBehavior.SetBehaviorSource(new BehaviorSource(externalBehavior)
				{
					behaviorName = this.mActiveBehaviorSource.behaviorName,
					behaviorDescription = this.mActiveBehaviorSource.behaviorDescription,
					TaskData = this.mActiveBehaviorSource.TaskData
				});
				text = string.Format("Assets/{0}", text.Substring(Application.dataPath.Length + 1));
				AssetDatabase.DeleteAsset(text);
				AssetDatabase.CreateAsset(externalBehavior, text);
				AssetDatabase.ImportAsset(text);
				Selection.activeObject = externalBehavior;
			}
			else if (Path.GetExtension(text).Equals(".asset"))
			{
				Debug.LogError("Error: Unable to save external behavior tree. The save location must be within the Asset directory.");
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000AF0C File Offset: 0x0000910C
		private void SaveAsPrefab()
		{
			if (this.mActiveBehaviorSource == null)
			{
				return;
			}
			string text = EditorUtility.SaveFilePanel("Save Behavior Tree", "Assets", this.mActiveBehaviorSource.behaviorName + ".prefab", "prefab");
			if (text.Length != 0 && Application.dataPath.Length < text.Length)
			{
				GameObject gameObject = new GameObject();
				Type type = Type.GetType("BehaviorDesigner.Runtime.BehaviorTree, Assembly-CSharp");
				if (type == null)
				{
					type = Type.GetType("BehaviorDesigner.Runtime.BehaviorTree, Assembly-CSharp-firstpass");
				}
				Behavior behavior = gameObject.AddComponent(type) as Behavior;
				behavior.SetBehaviorSource(new BehaviorSource(behavior)
				{
					behaviorName = this.mActiveBehaviorSource.behaviorName,
					behaviorDescription = this.mActiveBehaviorSource.behaviorDescription,
					TaskData = this.mActiveBehaviorSource.TaskData
				});
				text = string.Format("Assets/{0}", text.Substring(Application.dataPath.Length + 1));
				AssetDatabase.DeleteAsset(text);
				GameObject activeObject = PrefabUtility.CreatePrefab(text, gameObject);
				Object.DestroyImmediate(gameObject, true);
				AssetDatabase.ImportAsset(text);
				Selection.activeObject = activeObject;
			}
			else if (Path.GetExtension(text).Equals(".prefab"))
			{
				Debug.LogError("Error: Unable to save prefab. The save location must be within the Asset directory.");
			}
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000B04C File Offset: 0x0000924C
		public void LoadBehavior(BehaviorSource behaviorSource, bool loadPrevBehavior)
		{
			this.LoadBehavior(behaviorSource, loadPrevBehavior, false);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B058 File Offset: 0x00009258
		public void LoadBehavior(BehaviorSource behaviorSource, bool loadPrevBehavior, bool inspectorLoad)
		{
			if (behaviorSource == null || object.ReferenceEquals(behaviorSource.Owner, null) || behaviorSource.Owner.Equals(null))
			{
				return;
			}
			if (inspectorLoad && !this.mSizesInitialized)
			{
				this.mActiveBehaviorID = behaviorSource.Owner.GetInstanceID();
				this.mPrevActiveObject = Selection.activeObject;
				this.mLoadedFromInspector = true;
				return;
			}
			if (!this.mSizesInitialized)
			{
				return;
			}
			if (!loadPrevBehavior)
			{
				this.DisableReferenceTasks();
				this.mVariableInspector.ResetSelectedVariableIndex();
			}
			this.mExternalParent = null;
			this.mActiveBehaviorSource = behaviorSource;
			if (behaviorSource.Owner is Behavior)
			{
				this.mActiveObject = (behaviorSource.Owner as Behavior).gameObject;
				ExternalBehavior externalBehavior = (behaviorSource.Owner as Behavior).ExternalBehavior;
				if (externalBehavior != null && !EditorApplication.isPlayingOrWillChangePlaymode)
				{
					this.mActiveBehaviorSource = externalBehavior.BehaviorSource;
					this.mActiveBehaviorSource.Owner = externalBehavior;
					this.mExternalParent = behaviorSource;
					behaviorSource.CheckForSerialization(true, null);
					if (VariableInspector.SyncVariables(behaviorSource, this.mActiveBehaviorSource.Variables))
					{
						if (BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
						{
							BinarySerialization.Save(behaviorSource);
						}
						else
						{
							JSONSerialization.Save(behaviorSource);
						}
					}
				}
			}
			else
			{
				this.mActiveObject = behaviorSource.Owner.GetObject();
			}
			this.mActiveBehaviorSource.BehaviorID = this.mActiveBehaviorSource.Owner.GetInstanceID();
			this.mActiveBehaviorID = this.mActiveBehaviorSource.BehaviorID;
			this.mPrevActiveObject = Selection.activeObject;
			if (this.mBehaviorSourceHistory.Count == 0 || this.mBehaviorSourceHistoryIndex >= this.mBehaviorSourceHistory.Count || this.mBehaviorSourceHistory[this.mBehaviorSourceHistoryIndex] == null || ((this.mBehaviorSourceHistory[this.mBehaviorSourceHistoryIndex] as IBehavior).GetBehaviorSource() != null && !this.mActiveBehaviorSource.BehaviorID.Equals((this.mBehaviorSourceHistory[this.mBehaviorSourceHistoryIndex] as IBehavior).GetBehaviorSource().BehaviorID)))
			{
				for (int i = this.mBehaviorSourceHistory.Count - 1; i > this.mBehaviorSourceHistoryIndex; i--)
				{
					this.mBehaviorSourceHistory.RemoveAt(i);
				}
				this.mBehaviorSourceHistory.Add(this.mActiveBehaviorSource.Owner.GetObject());
				this.mBehaviorSourceHistoryIndex++;
			}
			Vector2 vector = new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom), 150f);
			vector -= this.mGraphOffset;
			if (this.mGraphDesigner.Load(this.mActiveBehaviorSource, loadPrevBehavior && !this.mLoadedFromInspector, vector) && this.mGraphDesigner.HasEntryNode() && (!loadPrevBehavior || this.mLoadedFromInspector))
			{
				this.mGraphDesigner.SetStartOffset(Vector2.zero);
				this.mGraphOffset = new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom), 50f);
				this.mGraphScrollPosition = (this.mGraphScrollSize - new Vector2(this.mGraphRect.width, this.mGraphRect.height)) / 2f - 2f * new Vector2(15f, 15f);
				this.SaveBehavior();
			}
			this.mLoadedFromInspector = false;
			if (!this.mLockActiveGameObject)
			{
				Selection.activeObject = this.mActiveObject;
			}
			if (EditorApplication.isPlaying && this.mActiveBehaviorSource != null)
			{
				this.mRightClickMenu = null;
				this.mUpdateNodeTaskMap = true;
				this.UpdateNodeTaskMap();
			}
			this.CheckForErrors();
			this.UpdateGraphStatus();
			this.ClearBreadcrumbMenu();
			base.Repaint();
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000B448 File Offset: 0x00009648
		public void ClearGraph()
		{
			this.mGraphDesigner.Clear(true);
			this.mActiveBehaviorSource = null;
			this.CheckForErrors();
			this.UpdateGraphStatus();
			base.Repaint();
		}

		// Token: 0x040000A5 RID: 165
		[SerializeField]
		public static BehaviorDesignerWindow instance;

		// Token: 0x040000A6 RID: 166
		private Rect mGraphRect;

		// Token: 0x040000A7 RID: 167
		private Rect mGraphScrollRect;

		// Token: 0x040000A8 RID: 168
		private Rect mFileToolBarRect;

		// Token: 0x040000A9 RID: 169
		private Rect mDebugToolBarRect;

		// Token: 0x040000AA RID: 170
		private Rect mPropertyToolbarRect;

		// Token: 0x040000AB RID: 171
		private Rect mPropertyBoxRect;

		// Token: 0x040000AC RID: 172
		private Rect mPreferencesPaneRect;

		// Token: 0x040000AD RID: 173
		private Vector2 mGraphScrollSize = new Vector2(20000f, 20000f);

		// Token: 0x040000AE RID: 174
		private bool mSizesInitialized;

		// Token: 0x040000AF RID: 175
		private float mPrevScreenWidth = -1f;

		// Token: 0x040000B0 RID: 176
		private float mPrevScreenHeight = -1f;

		// Token: 0x040000B1 RID: 177
		private bool mPropertiesPanelOnLeft = true;

		// Token: 0x040000B2 RID: 178
		private Vector2 mCurrentMousePosition = Vector2.zero;

		// Token: 0x040000B3 RID: 179
		private Vector2 mGraphScrollPosition = new Vector2(-1f, -1f);

		// Token: 0x040000B4 RID: 180
		private Vector2 mGraphOffset = Vector2.zero;

		// Token: 0x040000B5 RID: 181
		private float mGraphZoom = 1f;

		// Token: 0x040000B6 RID: 182
		private int mBehaviorToolbarSelection = 1;

		// Token: 0x040000B7 RID: 183
		private string[] mBehaviorToolbarStrings = new string[]
		{
			"Behavior",
			"Tasks",
			"Variables",
			"Inspector"
		};

		// Token: 0x040000B8 RID: 184
		private string mGraphStatus = string.Empty;

		// Token: 0x040000B9 RID: 185
		private Material mGridMaterial;

		// Token: 0x040000BA RID: 186
		private int mGUITickCount;

		// Token: 0x040000BB RID: 187
		private Vector2 mSelectStartPosition = Vector2.zero;

		// Token: 0x040000BC RID: 188
		private Rect mSelectionArea;

		// Token: 0x040000BD RID: 189
		private bool mIsSelecting;

		// Token: 0x040000BE RID: 190
		private bool mIsDragging;

		// Token: 0x040000BF RID: 191
		private bool mKeepTasksSelected;

		// Token: 0x040000C0 RID: 192
		private bool mNodeClicked;

		// Token: 0x040000C1 RID: 193
		private Vector2 mDragDelta = Vector2.zero;

		// Token: 0x040000C2 RID: 194
		private bool mCommandDown;

		// Token: 0x040000C3 RID: 195
		private bool mUpdateNodeTaskMap;

		// Token: 0x040000C4 RID: 196
		private bool mStepApplication;

		// Token: 0x040000C5 RID: 197
		private Dictionary<NodeDesigner, Task> mNodeDesignerTaskMap;

		// Token: 0x040000C6 RID: 198
		private bool mEditorAtBreakpoint;

		// Token: 0x040000C7 RID: 199
		[SerializeField]
		private List<ErrorDetails> mErrorDetails;

		// Token: 0x040000C8 RID: 200
		private GenericMenu mRightClickMenu;

		// Token: 0x040000C9 RID: 201
		[SerializeField]
		private GenericMenu mBreadcrumbGameObjectBehaviorMenu;

		// Token: 0x040000CA RID: 202
		[SerializeField]
		private GenericMenu mBreadcrumbGameObjectMenu;

		// Token: 0x040000CB RID: 203
		[SerializeField]
		private GenericMenu mBreadcrumbBehaviorMenu;

		// Token: 0x040000CC RID: 204
		[SerializeField]
		private GenericMenu mReferencedBehaviorsMenu;

		// Token: 0x040000CD RID: 205
		private bool mShowRightClickMenu;

		// Token: 0x040000CE RID: 206
		private bool mShowPrefPane;

		// Token: 0x040000CF RID: 207
		[SerializeField]
		private GraphDesigner mGraphDesigner;

		// Token: 0x040000D0 RID: 208
		private TaskInspector mTaskInspector;

		// Token: 0x040000D1 RID: 209
		private TaskList mTaskList;

		// Token: 0x040000D2 RID: 210
		private VariableInspector mVariableInspector;

		// Token: 0x040000D3 RID: 211
		[SerializeField]
		private Object mActiveObject;

		// Token: 0x040000D4 RID: 212
		private Object mPrevActiveObject;

		// Token: 0x040000D5 RID: 213
		private BehaviorSource mActiveBehaviorSource;

		// Token: 0x040000D6 RID: 214
		private BehaviorSource mExternalParent;

		// Token: 0x040000D7 RID: 215
		private int mActiveBehaviorID = -1;

		// Token: 0x040000D8 RID: 216
		[SerializeField]
		private List<Object> mBehaviorSourceHistory = new List<Object>();

		// Token: 0x040000D9 RID: 217
		[SerializeField]
		private int mBehaviorSourceHistoryIndex = -1;

		// Token: 0x040000DA RID: 218
		private BehaviorManager mBehaviorManager;

		// Token: 0x040000DB RID: 219
		private bool mLockActiveGameObject;

		// Token: 0x040000DC RID: 220
		private bool mLoadedFromInspector;

		// Token: 0x040000DD RID: 221
		[SerializeField]
		private bool mIsPlaying;

		// Token: 0x040000DE RID: 222
		private WWW mUpdateCheckRequest;

		// Token: 0x040000DF RID: 223
		private DateTime mLastUpdateCheck = DateTime.MinValue;

		// Token: 0x040000E0 RID: 224
		private string mLatestVersion;

		// Token: 0x040000E1 RID: 225
		private bool mTakingScreenshot;

		// Token: 0x040000E2 RID: 226
		private float mScreenshotStartGraphZoom;

		// Token: 0x040000E3 RID: 227
		private Vector2 mScreenshotStartGraphOffset;

		// Token: 0x040000E4 RID: 228
		private Texture2D mScreenshotTexture;

		// Token: 0x040000E5 RID: 229
		private Rect mScreenshotGraphSize;

		// Token: 0x040000E6 RID: 230
		private Vector2 mScreenshotGraphOffset;

		// Token: 0x040000E7 RID: 231
		private string mScreenshotPath;

		// Token: 0x040000E8 RID: 232
		private List<TaskSerializer> mCopiedTasks;

		// Token: 0x0200000A RID: 10
		private enum BreadcrumbMenuType
		{
			// Token: 0x040000EA RID: 234
			GameObjectBehavior,
			// Token: 0x040000EB RID: 235
			GameObject,
			// Token: 0x040000EC RID: 236
			Behavior
		}
	}
}
