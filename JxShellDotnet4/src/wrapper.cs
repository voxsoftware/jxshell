using System;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class wrapper : wrapperBase
	{
		public wrapper()
		{
		}

		public wrapper(object o)
		{
			if (o == null)
			{
				throw new Exception("No se puede crear un objeto contenedor a partir de null");
			}
			this.wrappedObject = o;
			this.wrappedType = o.GetType();
			this.typeD = typeDescriptor.loadFromType(o.GetType());
		}

		public wrapper(object o, typeDescriptor td)
		{
			this.wrappedObject = o;
			this.wrappedType = o.GetType();
			this.typeD = td;
		}

		public static wrapper createWrapper(object o)
		{
			typeDescriptor typeDescriptor = typeDescriptor.loadFromType(o.GetType());
			return typeDescriptor.compile().getWrapper(o);
		}

		public static wrapper createWrapper(object o, typeDescriptor td)
		{
			return td.compile().getWrapper(o);
		}

		public override object __invokeMethod(string method, params object[] args)
		{
			object obj = this.typeD.invokeMethod(this.wrappedObject, method, args);
			object result;
			if (obj != null && obj.Equals(this.wrappedObject))
			{
				result = this;
			}
			else
			{
				result = wrapper.getFromObject(obj);
			}
			return result;
		}

		public override object __getProperty(string property, params object[] args)
		{
			object property2 = this.typeD.getProperty(this.wrappedObject, property, args);
			object result;
			if (property2 != null && property2.Equals(this.wrappedObject))
			{
				result = this;
			}
			else
			{
				result = wrapper.getFromObject(property2);
			}
			return result;
		}

		public override void __setProperty(string property, string value, params object[] args)
		{
			this.typeD.setProperty(this.wrappedObject, property, value, args);
		}

		public static object getFromObject(object o)
		{
			object result;
			if (o is wrapper)
			{
				result = o;
			}
			else if (o == null || o is DBNull)
			{
				result = null;
			}
			else if (o is long)
			{
				result = (double)((long)o);
			}
			else
			{
				Type type = o.GetType();
				if (type.IsPrimitive || o is string)
				{
					result = o;
				}
				else
				{
					result = wrapper.createWrapper(o);
				}
			}
			return result;
		}

		public object __process(object o)
		{
			object result;
			if (o != null && o.GetType() == this.wrappedType && o.Equals(this.wrappedObject))
			{
				result = this;
			}
			else
			{
				result = wrapper.getFromObject(o);
			}
			return result;
		}

		public override string ToString()
		{
			return this.wrappedObject.ToString();
		}

		public override bool Equals(object obj)
		{
			return this.wrappedObject.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.wrappedObject.GetHashCode();
		}
	}
}
