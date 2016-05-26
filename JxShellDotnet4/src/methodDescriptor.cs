using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class methodDescriptor : memberDescriptor
	{
		public List<MethodBase> baseMethods = new List<MethodBase>();

		public string name;

		public int maxParameterCount = 0;

		public int methodOrder = 0;

		public bool isGenericMethod = false;

		public int genericParameterCount = 0;

		public ConstructorInfo getConstructorForParameters(ref object[] parameters)
		{
			memberDescriptor.convertParameters(ref parameters);
			List<MethodBase> list = new List<MethodBase>(0);
			bool flag = false;
			foreach (MethodBase current in this.baseMethods)
			{
				ParameterInfo[] parameters2 = current.GetParameters();
				if (parameters2.Length == parameters.Length)
				{
					bool flag2 = true;
					bool flag3 = false;
					for (int i = 0; i < parameters2.Length; i++)
					{
						ParameterInfo parameterInfo = parameters2[i];
						if (parameters[i] != null)
						{
							if (!parameterInfo.ParameterType.IsAssignableFrom(parameters[i].GetType()))
							{
								flag2 = false;
							}
						}
						else
						{
							flag3 = true;
						}
					}
					if (flag2)
					{
						list.Add(current);
						flag = (flag || (flag3 && flag2));
					}
				}
			}
			if ((flag && list.Count > 0) || list.Count == 0)
			{
				throw new Exception("No se puede determinar la mejor coincidencia para la ejecución del método.");
			}
			return (ConstructorInfo)list[0];
		}

		public MethodBase getGenericMethodForParameters(Type[] arguments, ref object[] parameters)
		{
			return this.getMethodForParameters(ref parameters, arguments);
		}

		public MethodBase getMethodForParameters(ref object[] parameters, Type[] arguments = null)
		{
			memberDescriptor.convertParameters(ref parameters);
			List<MethodBase> list = new List<MethodBase>(0);
			bool flag = false;
			for (int i = 0; i < this.baseMethods.Count; i++)
			{
				MethodBase methodBase = this.baseMethods[i];
				bool flag2 = true;
				if (arguments != null)
				{
					if (methodBase.GetGenericArguments().Length != arguments.Length)
					{
						flag2 = false;
					}
					else
					{
						MethodInfo methodInfo = (MethodInfo)methodBase;
						methodBase = methodInfo.MakeGenericMethod(arguments);
					}
				}
				if (flag2)
				{
					ParameterInfo[] parameters2 = methodBase.GetParameters();
					if (parameters2.Length == parameters.Length)
					{
						bool flag3 = true;
						bool flag4 = false;
						for (int j = 0; j < parameters2.Length; j++)
						{
							ParameterInfo parameterInfo = parameters2[j];
							if (parameters[j] != null)
							{
								Type type = parameters[j].GetType();
								if (!parameterInfo.ParameterType.IsAssignableFrom(type))
								{
									flag3 = false;
								}
							}
							else
							{
								flag4 = true;
							}
						}
						if (flag3)
						{
							list.Add(methodBase);
							i = 99999;
							flag = (flag || (flag4 && flag3));
						}
					}
				}
			}
			if ((flag && list.Count > 1) || list.Count == 0)
			{
				throw new Exception("No se puede determinar la mejor coincidencia para la ejecución del método.");
			}
			return list[0];
		}
	}
}
