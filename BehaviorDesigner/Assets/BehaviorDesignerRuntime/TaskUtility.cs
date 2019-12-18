using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x0200003A RID: 58
	public class TaskUtility
	{
		// Token: 0x06000220 RID: 544 RVA: 0x00010188 File Offset: 0x0000E388
		public static object CreateInstance(Type t)
		{
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				t = Nullable.GetUnderlyingType(t);
			}
			return Activator.CreateInstance(t, true);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x000101C4 File Offset: 0x0000E3C4
		public static FieldInfo[] GetAllFields(Type t)
		{
			FieldInfo[] array = null;
			if (!TaskUtility.allFieldsLookup.TryGetValue(t, out array))
			{
				List<FieldInfo> list = ObjectPool.Get<List<FieldInfo>>();
				list.Clear();
				BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				TaskUtility.GetFields(t, ref list, (int)flags);
				array = list.ToArray();
				ObjectPool.Return<List<FieldInfo>>(list);
				TaskUtility.allFieldsLookup.Add(t, array);
			}
			return array;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00010218 File Offset: 0x0000E418
		public static FieldInfo[] GetPublicFields(Type t)
		{
			FieldInfo[] array = null;
			if (!TaskUtility.publicFieldsLookup.TryGetValue(t, out array))
			{
				List<FieldInfo> list = ObjectPool.Get<List<FieldInfo>>();
				list.Clear();
				BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
				TaskUtility.GetFields(t, ref list, (int)flags);
				array = list.ToArray();
				ObjectPool.Return<List<FieldInfo>>(list);
				TaskUtility.publicFieldsLookup.Add(t, array);
			}
			return array;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0001026C File Offset: 0x0000E46C
		private static void GetFields(Type t, ref List<FieldInfo> fieldList, int flags)
		{
			if (t == null || t.Equals(typeof(ParentTask)) || t.Equals(typeof(Task)) || t.Equals(typeof(SharedVariable)))
			{
				return;
			}
			FieldInfo[] fields = t.GetFields((BindingFlags)flags);
			for (int i = 0; i < fields.Length; i++)
			{
				fieldList.Add(fields[i]);
			}
			TaskUtility.GetFields(t.BaseType, ref fieldList, flags);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x000102F4 File Offset: 0x0000E4F4
		public static Type GetTypeWithinAssembly(string typeName)
		{
			Type type;
			if (TaskUtility.typeLookup.TryGetValue(typeName, out type))
			{
				return type;
			}
			type = Type.GetType(typeName);
			if (type == null)
			{
				if (TaskUtility.loadedAssemblies == null)
				{
					TaskUtility.loadedAssemblies = new List<string>();
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < assemblies.Length; i++)
					{
						TaskUtility.loadedAssemblies.Add(assemblies[i].FullName);
					}
				}
				for (int j = 0; j < TaskUtility.loadedAssemblies.Count; j++)
				{
					type = Type.GetType(typeName + "," + TaskUtility.loadedAssemblies[j]);
					if (type != null)
					{
						break;
					}
				}
			}
			if (type != null)
			{
				TaskUtility.typeLookup.Add(typeName, type);
			}
			return type;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x000103BC File Offset: 0x0000E5BC
		public static bool CompareType(Type t, string typeName)
		{
			Type type = Type.GetType(typeName + ", Assembly-CSharp");
			if (type == null)
			{
				type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
			}
			return t.Equals(type);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x000103F8 File Offset: 0x0000E5F8
		public static bool HasAttribute(FieldInfo field, Type attribute)
		{
			if (field == null)
			{
				return false;
			}
			Dictionary<Type, bool> dictionary;
			if (!TaskUtility.hasFieldLookup.TryGetValue(field, out dictionary))
			{
				dictionary = new Dictionary<Type, bool>();
				TaskUtility.hasFieldLookup.Add(field, dictionary);
			}
			bool flag;
			if (!dictionary.TryGetValue(attribute, out flag))
			{
				flag = (field.GetCustomAttributes(attribute, false).Length > 0);
				dictionary.Add(attribute, flag);
			}
			return flag;
		}

		// Token: 0x040000E1 RID: 225
		[NonSerialized]
		private static Dictionary<string, Type> typeLookup = new Dictionary<string, Type>();

		// Token: 0x040000E2 RID: 226
		private static List<string> loadedAssemblies = null;

		// Token: 0x040000E3 RID: 227
		private static Dictionary<Type, FieldInfo[]> allFieldsLookup = new Dictionary<Type, FieldInfo[]>();

		// Token: 0x040000E4 RID: 228
		private static Dictionary<Type, FieldInfo[]> publicFieldsLookup = new Dictionary<Type, FieldInfo[]>();

		// Token: 0x040000E5 RID: 229
		private static Dictionary<FieldInfo, Dictionary<Type, bool>> hasFieldLookup = new Dictionary<FieldInfo, Dictionary<Type, bool>>();
	}
}
