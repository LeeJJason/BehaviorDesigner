using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class TaskList : ScriptableObject
	{
		// Token: 0x06000288 RID: 648 RVA: 0x00019384 File Offset: 0x00017584
		public void OnEnable()
		{
			base.hideFlags = (HideFlags)61;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00019390 File Offset: 0x00017590
		public void Init()
		{
			this.mCategoryList = new List<TaskList.CategoryList>();
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					if (!types[j].Equals(typeof(BehaviorReference)) && !types[j].IsAbstract)
					{
						if (types[j].IsSubclassOf(typeof(Action)) || types[j].IsSubclassOf(typeof(Composite)) || types[j].IsSubclassOf(typeof(Conditional)) || types[j].IsSubclassOf(typeof(Decorator)))
						{
							list.Add(types[j]);
						}
					}
				}
			}
			list.Sort(new AlphanumComparator<Type>());
			Dictionary<string, TaskList.CategoryList> dictionary = new Dictionary<string, TaskList.CategoryList>();
			string text = string.Empty;
			int id = 0;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].IsSubclassOf(typeof(Action)))
				{
					text = "Actions";
				}
				else if (list[k].IsSubclassOf(typeof(Composite)))
				{
					text = "Composites";
				}
				else if (list[k].IsSubclassOf(typeof(Conditional)))
				{
					text = "Conditionals";
				}
				else
				{
					text = "Decorators";
				}
				TaskCategoryAttribute[] array;
				if ((array = (list[k].GetCustomAttributes(typeof(TaskCategoryAttribute), false) as TaskCategoryAttribute[])).Length > 0)
				{
					text = text + "/" + array[0].Category;
				}
				string text2 = string.Empty;
				string[] array2 = text.Split(new char[]
				{
					'/'
				});
				TaskList.CategoryList categoryList = null;
				TaskList.CategoryList categoryList2;
				for (int l = 0; l < array2.Length; l++)
				{
					if (l > 0)
					{
						text2 += "/";
					}
					text2 += array2[l];
					if (!dictionary.ContainsKey(text2))
					{
						categoryList2 = new TaskList.CategoryList(array2[l], text2, this.PreviouslyExpanded(id), id++);
						if (categoryList == null)
						{
							this.mCategoryList.Add(categoryList2);
						}
						else
						{
							categoryList.addSubcategory(categoryList2);
						}
						dictionary.Add(text2, categoryList2);
					}
					else
					{
						categoryList2 = dictionary[text2];
					}
					categoryList = categoryList2;
				}
				categoryList2 = dictionary[text2];
				categoryList2.addTask(list[k]);
			}
			this.Search(BehaviorDesignerUtility.SplitCamelCase(this.mSearchString).ToLower().Replace(" ", string.Empty), this.mCategoryList);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001968C File Offset: 0x0001788C
		public void AddTasksToMenu(ref GenericMenu genericMenu, Type selectedTaskType, string parentName, GenericMenu.MenuFunction2 menuFunction)
		{
			this.AddCategoryTasksToMenu(ref genericMenu, this.mCategoryList, selectedTaskType, parentName, menuFunction);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000196A0 File Offset: 0x000178A0
		public void AddConditionalTasksToMenu(ref GenericMenu genericMenu, Type selectedTaskType, string parentName, GenericMenu.MenuFunction2 menuFunction)
		{
			if (this.mCategoryList[2].Tasks != null)
			{
				for (int i = 0; i < this.mCategoryList[2].Tasks.Count; i++)
				{
					if (parentName.Equals(string.Empty))
					{
						genericMenu.AddItem(new GUIContent(string.Format("{0}/{1}", this.mCategoryList[2].Fullpath, this.mCategoryList[2].Tasks[i].Name.ToString())), this.mCategoryList[2].Tasks[i].Type.Equals(selectedTaskType), menuFunction, this.mCategoryList[2].Tasks[i].Type);
					}
					else
					{
						genericMenu.AddItem(new GUIContent(string.Format("{0}/{1}/{2}", parentName, this.mCategoryList[22].Fullpath, this.mCategoryList[2].Tasks[i].Name.ToString())), this.mCategoryList[2].Tasks[i].Type.Equals(selectedTaskType), menuFunction, this.mCategoryList[2].Tasks[i].Type);
					}
				}
			}
			this.AddCategoryTasksToMenu(ref genericMenu, this.mCategoryList[2].Subcategories, selectedTaskType, parentName, menuFunction);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00019830 File Offset: 0x00017A30
		private void AddCategoryTasksToMenu(ref GenericMenu genericMenu, List<TaskList.CategoryList> categoryList, Type selectedTaskType, string parentName, GenericMenu.MenuFunction2 menuFunction)
		{
			for (int i = 0; i < categoryList.Count; i++)
			{
				if (categoryList[i].Subcategories != null)
				{
					this.AddCategoryTasksToMenu(ref genericMenu, categoryList[i].Subcategories, selectedTaskType, parentName, menuFunction);
				}
				if (categoryList[i].Tasks != null)
				{
					for (int j = 0; j < categoryList[i].Tasks.Count; j++)
					{
						if (parentName.Equals(string.Empty))
						{
							genericMenu.AddItem(new GUIContent(string.Format("{0}/{1}", categoryList[i].Fullpath, categoryList[i].Tasks[j].Name.ToString())), categoryList[i].Tasks[j].Type.Equals(selectedTaskType), menuFunction, categoryList[i].Tasks[j].Type);
						}
						else
						{
							genericMenu.AddItem(new GUIContent(string.Format("{0}/{1}/{2}", parentName, categoryList[i].Fullpath, categoryList[i].Tasks[j].Name.ToString())), categoryList[i].Tasks[j].Type.Equals(selectedTaskType), menuFunction, categoryList[i].Tasks[j].Type);
						}
					}
				}
			}
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000199B0 File Offset: 0x00017BB0
		public void FocusSearchField()
		{
			this.mFocusSearch = true;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000199BC File Offset: 0x00017BBC
		public void DrawTaskList(BehaviorDesignerWindow window, bool enabled)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.SetNextControlName("Search");
			string value = GUILayout.TextField(this.mSearchString, GUI.skin.FindStyle("ToolbarSeachTextField"), new GUILayoutOption[0]);
			if (this.mFocusSearch)
			{
				GUI.FocusControl("Search");
				this.mFocusSearch = false;
			}
			if (!this.mSearchString.Equals(value))
			{
				this.mSearchString = value;
				this.Search(BehaviorDesignerUtility.SplitCamelCase(this.mSearchString).ToLower().Replace(" ", string.Empty), this.mCategoryList);
			}
			if (GUILayout.Button(string.Empty, (!this.mSearchString.Equals(string.Empty)) ? GUI.skin.FindStyle("ToolbarSeachCancelButton") : GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty"), new GUILayoutOption[0]))
			{
				this.mSearchString = string.Empty;
				this.Search(string.Empty, this.mCategoryList);
				GUI.FocusControl(null);
			}
			GUILayout.EndHorizontal();
			BehaviorDesignerUtility.DrawContentSeperator(2);
			GUILayout.Space(4f);
			this.mScrollPosition = GUILayout.BeginScrollView(this.mScrollPosition, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			if (this.mCategoryList.Count > 1)
			{
				this.DrawCategory(window, this.mCategoryList[1]);
			}
			if (this.mCategoryList.Count > 3)
			{
				this.DrawCategory(window, this.mCategoryList[3]);
			}
			if (this.mCategoryList.Count > 0)
			{
				this.DrawCategory(window, this.mCategoryList[0]);
			}
			if (this.mCategoryList.Count > 2)
			{
				this.DrawCategory(window, this.mCategoryList[2]);
			}
			GUI.enabled = true;
			GUILayout.EndScrollView();
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00019B9C File Offset: 0x00017D9C
		private void DrawCategory(BehaviorDesignerWindow window, TaskList.CategoryList category)
		{
			if (category.Visible)
			{
				category.Expanded = EditorGUILayout.Foldout(category.Expanded, category.Name, BehaviorDesignerUtility.TaskFoldoutGUIStyle);
				this.SetExpanded(category.ID, category.Expanded);
				if (category.Expanded)
				{
					EditorGUI.indentLevel++;
					if (category.Tasks != null)
					{
						for (int i = 0; i < category.Tasks.Count; i++)
						{
							if (category.Tasks[i].Visible)
							{
								GUILayout.BeginHorizontal(new GUILayoutOption[0]);
								GUILayout.Space((float)(EditorGUI.indentLevel * 16));
								TaskNameAttribute[] array;
								string name;
								if ((array = (category.Tasks[i].Type.GetCustomAttributes(typeof(TaskNameAttribute), false) as TaskNameAttribute[])).Length > 0)
								{
									name = array[0].Name;
								}
								else
								{
									name = category.Tasks[i].Name;
								}
								if (GUILayout.Button(name, EditorStyles.toolbarButton, new GUILayoutOption[]
								{
									GUILayout.MaxWidth((float)(300 - EditorGUI.indentLevel * 16 - 24))
								}))
								{
									window.AddTask(category.Tasks[i].Type, false);
								}
								GUILayout.Space(3f);
								GUILayout.EndHorizontal();
							}
						}
					}
					if (category.Subcategories != null)
					{
						this.DrawCategoryTaskList(window, category.Subcategories);
					}
					EditorGUI.indentLevel--;
				}
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00019D20 File Offset: 0x00017F20
		private void DrawCategoryTaskList(BehaviorDesignerWindow window, List<TaskList.CategoryList> categoryList)
		{
			for (int i = 0; i < categoryList.Count; i++)
			{
				this.DrawCategory(window, categoryList[i]);
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00019D54 File Offset: 0x00017F54
		private bool Search(string searchString, List<TaskList.CategoryList> categoryList)
		{
			bool result = searchString.Equals(string.Empty);
			for (int i = 0; i < categoryList.Count; i++)
			{
				bool flag = false;
				categoryList[i].Visible = false;
				if (categoryList[i].Subcategories != null && this.Search(searchString, categoryList[i].Subcategories))
				{
					categoryList[i].Visible = true;
					result = true;
				}
				if (BehaviorDesignerUtility.SplitCamelCase(categoryList[i].Name).ToLower().Replace(" ", string.Empty).Contains(searchString))
				{
					result = true;
					flag = true;
					categoryList[i].Visible = true;
					if (categoryList[i].Subcategories != null)
					{
						this.MarkVisible(categoryList[i].Subcategories);
					}
				}
				if (categoryList[i].Tasks != null)
				{
					for (int j = 0; j < categoryList[i].Tasks.Count; j++)
					{
						categoryList[i].Tasks[j].Visible = searchString.Equals(string.Empty);
						if (flag || categoryList[i].Tasks[j].Name.ToLower().Replace(" ", string.Empty).Contains(searchString))
						{
							categoryList[i].Tasks[j].Visible = true;
							result = true;
							categoryList[i].Visible = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00019EE8 File Offset: 0x000180E8
		private void MarkVisible(List<TaskList.CategoryList> categoryList)
		{
			for (int i = 0; i < categoryList.Count; i++)
			{
				categoryList[i].Visible = true;
				if (categoryList[i].Subcategories != null)
				{
					this.MarkVisible(categoryList[i].Subcategories);
				}
				if (categoryList[i].Tasks != null)
				{
					for (int j = 0; j < categoryList[i].Tasks.Count; j++)
					{
						categoryList[i].Tasks[j].Visible = true;
					}
				}
			}
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00019F88 File Offset: 0x00018188
		private bool PreviouslyExpanded(int id)
		{
			return EditorPrefs.GetBool("BehaviorDesignerTaskList" + id, true);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00019FA0 File Offset: 0x000181A0
		private void SetExpanded(int id, bool visible)
		{
			EditorPrefs.SetBool("BehaviorDesignerTaskList" + id, visible);
		}

		// Token: 0x04000183 RID: 387
		private List<TaskList.CategoryList> mCategoryList;

		// Token: 0x04000184 RID: 388
		private Vector2 mScrollPosition = Vector2.zero;

		// Token: 0x04000185 RID: 389
		private string mSearchString = string.Empty;

		// Token: 0x04000186 RID: 390
		private bool mFocusSearch;

		// Token: 0x02000028 RID: 40
		public enum TaskTypes
		{
			// Token: 0x04000188 RID: 392
			Action,
			// Token: 0x04000189 RID: 393
			Composite,
			// Token: 0x0400018A RID: 394
			Conditional,
			// Token: 0x0400018B RID: 395
			Decorator,
			// Token: 0x0400018C RID: 396
			Last
		}

		// Token: 0x02000029 RID: 41
		private class SearchableType
		{
			// Token: 0x06000295 RID: 661 RVA: 0x00019FB8 File Offset: 0x000181B8
			public SearchableType(Type type)
			{
				this.mType = type;
				this.mName = BehaviorDesignerUtility.SplitCamelCase(this.mType.Name);
			}

			// Token: 0x1700007A RID: 122
			// (get) Token: 0x06000296 RID: 662 RVA: 0x00019FF0 File Offset: 0x000181F0
			public Type Type
			{
				get
				{
					return this.mType;
				}
			}

			// Token: 0x1700007B RID: 123
			// (get) Token: 0x06000297 RID: 663 RVA: 0x00019FF8 File Offset: 0x000181F8
			// (set) Token: 0x06000298 RID: 664 RVA: 0x0001A000 File Offset: 0x00018200
			public bool Visible
			{
				get
				{
					return this.mVisible;
				}
				set
				{
					this.mVisible = value;
				}
			}

			// Token: 0x1700007C RID: 124
			// (get) Token: 0x06000299 RID: 665 RVA: 0x0001A00C File Offset: 0x0001820C
			public string Name
			{
				get
				{
					return this.mName;
				}
			}

			// Token: 0x0400018D RID: 397
			private Type mType;

			// Token: 0x0400018E RID: 398
			private bool mVisible = true;

			// Token: 0x0400018F RID: 399
			private string mName;
		}

		// Token: 0x0200002A RID: 42
		private class CategoryList
		{
			// Token: 0x0600029A RID: 666 RVA: 0x0001A014 File Offset: 0x00018214
			public CategoryList(string name, string fullpath, bool expanded, int id)
			{
				this.mName = name;
				this.mFullpath = fullpath;
				this.mExpanded = expanded;
				this.mID = id;
			}

			// Token: 0x1700007D RID: 125
			// (get) Token: 0x0600029B RID: 667 RVA: 0x0001A068 File Offset: 0x00018268
			public string Name
			{
				get
				{
					return this.mName;
				}
			}

			// Token: 0x1700007E RID: 126
			// (get) Token: 0x0600029C RID: 668 RVA: 0x0001A070 File Offset: 0x00018270
			public string Fullpath
			{
				get
				{
					return this.mFullpath;
				}
			}

			// Token: 0x1700007F RID: 127
			// (get) Token: 0x0600029D RID: 669 RVA: 0x0001A078 File Offset: 0x00018278
			public List<TaskList.CategoryList> Subcategories
			{
				get
				{
					return this.mSubcategories;
				}
			}

			// Token: 0x17000080 RID: 128
			// (get) Token: 0x0600029E RID: 670 RVA: 0x0001A080 File Offset: 0x00018280
			public List<TaskList.SearchableType> Tasks
			{
				get
				{
					return this.mTasks;
				}
			}

			// Token: 0x17000081 RID: 129
			// (get) Token: 0x0600029F RID: 671 RVA: 0x0001A088 File Offset: 0x00018288
			// (set) Token: 0x060002A0 RID: 672 RVA: 0x0001A090 File Offset: 0x00018290
			public bool Expanded
			{
				get
				{
					return this.mExpanded;
				}
				set
				{
					this.mExpanded = value;
				}
			}

			// Token: 0x17000082 RID: 130
			// (get) Token: 0x060002A1 RID: 673 RVA: 0x0001A09C File Offset: 0x0001829C
			// (set) Token: 0x060002A2 RID: 674 RVA: 0x0001A0A4 File Offset: 0x000182A4
			public bool Visible
			{
				get
				{
					return this.mVisible;
				}
				set
				{
					this.mVisible = value;
				}
			}

			// Token: 0x17000083 RID: 131
			// (get) Token: 0x060002A3 RID: 675 RVA: 0x0001A0B0 File Offset: 0x000182B0
			public int ID
			{
				get
				{
					return this.mID;
				}
			}

			// Token: 0x060002A4 RID: 676 RVA: 0x0001A0B8 File Offset: 0x000182B8
			public void addSubcategory(TaskList.CategoryList category)
			{
				if (this.mSubcategories == null)
				{
					this.mSubcategories = new List<TaskList.CategoryList>();
				}
				this.mSubcategories.Add(category);
			}

			// Token: 0x060002A5 RID: 677 RVA: 0x0001A0E8 File Offset: 0x000182E8
			public void addTask(Type taskType)
			{
				if (this.mTasks == null)
				{
					this.mTasks = new List<TaskList.SearchableType>();
				}
				this.mTasks.Add(new TaskList.SearchableType(taskType));
			}

			// Token: 0x04000190 RID: 400
			private string mName = string.Empty;

			// Token: 0x04000191 RID: 401
			private string mFullpath = string.Empty;

			// Token: 0x04000192 RID: 402
			private List<TaskList.CategoryList> mSubcategories;

			// Token: 0x04000193 RID: 403
			private List<TaskList.SearchableType> mTasks;

			// Token: 0x04000194 RID: 404
			private bool mExpanded = true;

			// Token: 0x04000195 RID: 405
			private bool mVisible = true;

			// Token: 0x04000196 RID: 406
			private int mID;
		}
	}
}
