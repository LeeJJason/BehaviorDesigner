using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000022 RID: 34
	internal static class ObjectDrawerUtility
	{
		// Token: 0x06000253 RID: 595 RVA: 0x0001629C File Offset: 0x0001449C
		private static void BuildObjectDrawers()
		{
			if (ObjectDrawerUtility.mapBuilt)
			{
				return;
			}
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly != null)
				{
					try
					{
						foreach (Type type in assembly.GetExportedTypes())
						{
							CustomObjectDrawer[] array;
							if (typeof(ObjectDrawer).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && (array = (type.GetCustomAttributes(typeof(CustomObjectDrawer), false) as CustomObjectDrawer[])).Length > 0)
							{
								ObjectDrawerUtility.objectDrawerTypeMap.Add(array[0].Type, type);
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
			ObjectDrawerUtility.mapBuilt = true;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0001639C File Offset: 0x0001459C
		private static bool ObjectDrawerForType(Type type, ref ObjectDrawer objectDrawer, ref Type objectDrawerType, int hash)
		{
			ObjectDrawerUtility.BuildObjectDrawers();
			if (!ObjectDrawerUtility.objectDrawerTypeMap.ContainsKey(type))
			{
				return false;
			}
			objectDrawerType = ObjectDrawerUtility.objectDrawerTypeMap[type];
			if (ObjectDrawerUtility.objectDrawerMap.ContainsKey(hash))
			{
				objectDrawer = ObjectDrawerUtility.objectDrawerMap[hash];
			}
			return true;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000163EC File Offset: 0x000145EC
		public static ObjectDrawer GetObjectDrawer(Task task, FieldInfo field)
		{
			ObjectDrawer objectDrawer = null;
			Type type = null;
			if (!ObjectDrawerUtility.ObjectDrawerForType(field.FieldType, ref objectDrawer, ref type, field.GetHashCode()))
			{
				return null;
			}
			if (objectDrawer != null)
			{
				return objectDrawer;
			}
			objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
			objectDrawer.FieldInfo = field;
			objectDrawer.Task = task;
			ObjectDrawerUtility.objectDrawerMap.Add(field.GetHashCode(), objectDrawer);
			return objectDrawer;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00016450 File Offset: 0x00014650
		public static ObjectDrawer GetObjectDrawer(Task task, ObjectDrawerAttribute attribute)
		{
			ObjectDrawer objectDrawer = null;
			Type type = null;
			if (!ObjectDrawerUtility.ObjectDrawerForType(attribute.GetType(), ref objectDrawer, ref type, attribute.GetHashCode()))
			{
				return null;
			}
			if (objectDrawer != null)
			{
				return objectDrawer;
			}
			objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
			objectDrawer.Attribute = attribute;
			objectDrawer.Task = task;
			ObjectDrawerUtility.objectDrawerMap.Add(attribute.GetHashCode(), objectDrawer);
			return objectDrawer;
		}

		// Token: 0x04000175 RID: 373
		private static Dictionary<Type, Type> objectDrawerTypeMap = new Dictionary<Type, Type>();

		// Token: 0x04000176 RID: 374
		private static Dictionary<int, ObjectDrawer> objectDrawerMap = new Dictionary<int, ObjectDrawer>();

		// Token: 0x04000177 RID: 375
		private static bool mapBuilt = false;
	}
}
