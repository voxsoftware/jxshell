using System;
using System.Collections;
using System.Collections.Generic;

namespace RPCJson
{
	public class Objects
	{
		public static Dictionary<int, object> objects = new Dictionary<int, object>();

		public static Hashtable objectNames = new Hashtable();

		public static int id = 0;

		public static void FreeMany(object[] objs)
		{
			if (objs != null)
			{
				for (int i = 0; i < objs.Length; i++)
				{
					object obj = objs[i];
					if (obj != null)
					{
						int item = Convert.ToInt32(obj);
						Objects.remove(item);
					}
				}
			}
		}

		public static int save(object o)
		{
			object num = Objects.objectNames[o];
			int result;
			if (num != null)
			{
				result = (int)num;
			}
			else
			{
				Objects.id++;
				Objects.objectNames.Add(o, Objects.id);
				Objects.objects.Add(Objects.id, o);
				result = Objects.id;
			}
			return result;
		}

		public static void remove(int id)
		{
			object o;
			if (Objects.objects.TryGetValue(id, out o))
			{
				Objects.objects.Remove(id);
				Objects.objectNames.Remove(o);
				o = null;
			}
		}

		public static object getbyid(int id)
		{
			object o;
			object result;
			if (Objects.objects.TryGetValue(id, out o))
			{
				result = o;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int getidofobject(object o)
		{
			object i = Objects.objectNames[o];
			int result;
			if (i != null)
			{
				result = (int)i;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public static void clear()
		{
			Objects.objectNames.Clear();
			Objects.objects.Clear();
		}
	}
}
