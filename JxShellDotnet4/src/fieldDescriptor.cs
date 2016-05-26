using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class fieldDescriptor : memberDescriptor
	{
		public FieldInfo fieldInfo;

		public string name;

		public int fieldOrder = 0;

		public object getValue(object o = null)
		{
			object value = this.fieldInfo.GetValue(o);
			return wrapper.getFromObject(value);
		}

		public void setValue(object value, object o = null)
		{
			if (value is wrapper)
			{
				value = ((wrapper)value).wrappedObject;
			}
			this.fieldInfo.SetValue(o, value);
		}
	}
}
