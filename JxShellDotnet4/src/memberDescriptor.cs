using System;
using System.Reflection;

namespace jxshell.dotnet4
{
	public class memberDescriptor
	{
		public static void convertParameters(ref object[] pars)
		{
			int num = -1;
			object[] array = pars;
			for (int i = 0; i < array.Length; i++)
			{
				object obj = array[i];
				if (!(obj is Missing))
				{
					num++;
					if (obj is wrapperBase)
					{
						pars[num] = ((wrapperBase)obj).wrappedObject;
					}
					else if (obj is DBNull)
					{
						pars[num] = null;
					}
				}
			}
			if (num + 1 != pars.Length)
			{
				Array.Resize<object>(ref pars, num + 1);
			}
		}
	}
}
