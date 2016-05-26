using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class propertyDescriptor : memberDescriptor
	{
		public List<PropertyInfo> properties = new List<PropertyInfo>();

		public string name;

		public int maxParameterCount = 0;

		public int propertyOrder = 0;

		public PropertyInfo getPropertyForParameters(ref object[] parameters)
		{
			memberDescriptor.convertParameters(ref parameters);
			Type[] typeArray = Type.GetTypeArray(parameters);
			List<PropertyInfo> list = new List<PropertyInfo>(0);
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyInfo current in this.properties)
			{
				stringBuilder.Length = 0;
				ParameterInfo[] indexParameters = current.GetIndexParameters();
				if (indexParameters.Length == parameters.Length)
				{
					bool flag2 = true;
					bool flag3 = false;
					for (int i = 0; i < indexParameters.Length; i++)
					{
						ParameterInfo parameterInfo = indexParameters[i];
						if (parameters[i] != null)
						{
							if (!parameterInfo.ParameterType.IsAssignableFrom(parameters[i].GetType()))
							{
								stringBuilder.Append(",").Append(parameterInfo.ParameterType.ToString());
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
				string str = "";
				if (stringBuilder.Length > 0)
				{
					str = "Una de las sobrecargas admite estos tipos es: " + stringBuilder.ToString();
				}
				throw new Exception("No se puede determinar la mejor coincidencia para la ejecución de la propiedad. " + str);
			}
			return list[0];
		}
	}
}
