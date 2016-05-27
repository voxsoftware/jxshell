using jxshell.dotnet4;
using System;
using System.Collections.Generic;
using System.Reflection;
using RPCJson;
namespace ClrForAll
{
	public class TypeLoader
	{
		public class TypeResponse
		{
			public List<object[]> staticFields;

			public List<object[]> staticProperties;

			public List<object[]> staticMethods;

			public List<object[]> fields;

			public List<object[]> properties;

			public List<object[]> methods;
		}

		public static object invokeMethod(Command cmd, ref object[] args, object obj)
		{
			MethodBase i = null;
			ConstructorInfo c = null;
			typeDescriptor t;
			if (null == obj)
			{
				Type ty = Manager.lastManager.getTypeOrGenericType(cmd.typename);
				t = typeDescriptor.loadFromType(ty, "", false);
			}
			else
			{
				t = typeDescriptor.loadFromType(obj.GetType(), "", false);
			}
			if (cmd.nameindex > 0)
			{
				i = t.methods[cmd.nameindex - 1].getMethodForParameters(ref args, null);
			}
			else if (obj != null)
			{
				i = t.getMethodForParameters(cmd.name, ref args);
			}
			else if (cmd.name == ".ctor")
			{
				c = t.constructor.getConstructorForParameters(ref args);
			}
			else
			{
				i = t.getStaticMethodForParameters(cmd.name, ref args);
			}
			object result;
			if (i == null)
			{
				result = c.Invoke(args);
			}
			else
			{
				result = i.Invoke(obj, args);
			}
			return result;
		}

		public static PropertyInfo property(Command cmd, ref object[] args, object obj)
		{
			typeDescriptor t;
			if (null == obj)
			{
				Type ty = Manager.lastManager.getTypeOrGenericType(cmd.typename);
				t = typeDescriptor.loadFromType(ty, "", false);
			}
			else
			{
				t = typeDescriptor.loadFromType(obj.GetType(), "", false);
			}
			PropertyInfo i;
			if (cmd.nameindex > 0)
			{
				i = t.properties[cmd.nameindex - 1].getPropertyForParameters(ref args);
			}
			else if (obj == null)
			{
				i = t.getStaticPropertyForParameters(cmd.name, ref args);
			}
			else
			{
				i = t.getPropertyForParameters(cmd.name, ref args);
			}
			return i;
		}

		public static FieldInfo field(Command cmd, object obj)
		{
			FieldInfo i = null;
			typeDescriptor t;
			if (null == obj)
			{
				Type ty = Manager.lastManager.getTypeOrGenericType(cmd.typename);
				t = typeDescriptor.loadFromType(ty, "", false);
			}
			else
			{
				t = typeDescriptor.loadFromType(obj.GetType(), "", false);
			}
			if (cmd.nameindex > 0)
			{
				i = t.fields[cmd.nameindex - 1].fieldInfo;
			}
			else
			{
				foreach (fieldDescriptor fieldi in t.fields)
				{
					if (fieldi.name == cmd.name)
					{
						i = fieldi.fieldInfo;
						if ((i.IsStatic && obj == null) || (!i.IsStatic && null != obj))
						{
							break;
						}
						i = null;
					}
				}
			}
			return i;
		}

		public static object getProperty(Command cmd, ref object[] args, object obj)
		{
			return TypeLoader.property(cmd, ref args, obj).GetValue(obj);
		}

		public static void setProperty(Command cmd, ref object[] args, object obj, object value)
		{
			TypeLoader.property(cmd, ref args, obj).SetValue(obj, value);
		}

		public static object getField(Command cmd, object obj)
		{
			return TypeLoader.field(cmd, obj).GetValue(obj);
		}

		public static void setField(Command cmd, object obj, object value)
		{
			TypeLoader.field(cmd, obj).SetValue(obj, value);
		}

		public static TypeLoader.TypeResponse loadMembers(typeDescriptor t)
		{
			TypeLoader.TypeResponse tr = new TypeLoader.TypeResponse();
			for (int i = 0; i < t.fields.Count; i++)
			{
				fieldDescriptor field = t.fields[i];
				bool isStatic = field.fieldInfo.IsStatic;
				List<object[]> fld;
				if (isStatic)
				{
					if (tr.staticFields == null)
					{
						tr.staticFields = new List<object[]>();
					}
					fld = tr.staticFields;
				}
				else
				{
					if (tr.fields == null)
					{
						tr.fields = new List<object[]>();
					}
					fld = tr.fields;
				}
				fld.Add(new object[]
					{
						field.name,
						i
					});
			}
			for (int i = 0; i < t.properties.Count; i++)
			{
				propertyDescriptor property = t.properties[i];
				List<PropertyInfo> pInfo = property.properties;
				bool p = false;
				for (int y = 0; y < pInfo.Count; y++)
				{
					PropertyInfo p2 = pInfo[y];
					if (null != p2)
					{
						MethodBase gm;
						if (p2.CanRead)
						{
							gm = p2.GetGetMethod();
						}
						else if (p2.CanWrite)
						{
							gm = p2.GetSetMethod();
						}
						else
						{
							gm = null;
						}
						if (gm != null)
						{
							bool isStatic = gm.IsStatic;
							List<object[]> fld;
							if (isStatic)
							{
								if (tr.staticProperties == null)
								{
									tr.staticProperties = new List<object[]>();
								}
								fld = tr.staticProperties;
							}
							else
							{
								if (tr.properties == null)
								{
									tr.properties = new List<object[]>();
								}
								fld = tr.properties;
							}
							fld.Add(new object[]
								{
									property.name,
									i
								});
							p = true;
						}
					}
					if (p)
					{
						break;
					}
				}
			}
			for (int i = 0; i < t.methods.Count; i++)
			{
				methodDescriptor method = t.methods[i];
				List<MethodBase> pInfo2 = method.baseMethods;
				for (int y = 0; y < pInfo2.Count; y++)
				{
					MethodBase p3 = pInfo2[y];
					bool isStatic = p3.IsStatic;
					List<object[]> fld;
					if (isStatic)
					{
						if (tr.staticMethods == null)
						{
							tr.staticMethods = new List<object[]>();
						}
						fld = tr.staticMethods;
					}
					else
					{
						if (tr.methods == null)
						{
							tr.methods = new List<object[]>();
						}
						fld = tr.methods;
					}
					fld.Add(new object[]
						{
							method.name,
							i
						});
					if (y == 0)
					{
						break;
					}
				}
			}
			return tr;
		}
	}
}
