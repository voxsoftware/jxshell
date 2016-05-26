using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
//using System.Windows.Forms;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class typeDescriptor
	{
		private Dictionary<string, methodDescriptor> instanceMethods = new Dictionary<string, methodDescriptor>();

		private Dictionary<string, methodDescriptor> staticMethods = new Dictionary<string, methodDescriptor>();

		private Dictionary<string, propertyDescriptor> instanceProperties = new Dictionary<string, propertyDescriptor>();

		private Dictionary<string, propertyDescriptor> staticProperties = new Dictionary<string, propertyDescriptor>();

		private Dictionary<string, fieldDescriptor> instanceFields = new Dictionary<string, fieldDescriptor>();

		private Dictionary<string, fieldDescriptor> staticFields = new Dictionary<string, fieldDescriptor>();

		public static bool gencompile = true;

		public List<methodDescriptor> methods = new List<methodDescriptor>();

		public methodDescriptor constructor = null;

		public List<propertyDescriptor> properties = new List<propertyDescriptor>();

		public List<fieldDescriptor> fields = new List<fieldDescriptor>();

		internal static csharplanguage language;

		internal wrapperStatic compiledWrapper = null;

		internal static bool generateInMemory = true;

		internal Type type;

		internal string typeString = "";

		private bool compiled = false;

		private static Dictionary<Type, typeDescriptor> loadedTypes = new Dictionary<Type, typeDescriptor>();

		public typeDescriptor(Type t) : this(t, "", true)
		{
			this.typeString = typeDescriptor.getNameForType(t);
		}

		public typeDescriptor(Type t, string typeName, bool compile = true)
		{
			typeDescriptor.loadEvaluator();
			if (typeDescriptor.language == null)
			{
				typeDescriptor.language = (csharplanguage)jxshell.language.defaultLanguage.create();
			}
			this.typeString = typeName;
			MethodInfo[] array = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			MethodInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				MethodInfo methodInfo = array2[i];
				if (!methodInfo.Name.StartsWith("get_") && !methodInfo.Name.StartsWith("set_"))
				{
					methodDescriptor methodDescriptor = null;
					if (methodInfo.IsGenericMethod)
					{
						string text = "generic_" + methodInfo.Name;
						if (!this.instanceMethods.TryGetValue(text, out methodDescriptor))
						{
							methodDescriptor = new methodDescriptor();
							methodDescriptor.isGenericMethod = true;
							this.instanceMethods[text] = methodDescriptor;
							this.methods.Add(methodDescriptor);
							methodDescriptor.methodOrder = this.methods.Count - 1;
							methodDescriptor.name = text;
						}
					}
					else if (!this.instanceMethods.TryGetValue(methodInfo.Name, out methodDescriptor))
					{
						methodDescriptor = new methodDescriptor();
						this.instanceMethods[methodInfo.Name] = methodDescriptor;
						this.methods.Add(methodDescriptor);
						methodDescriptor.methodOrder = this.methods.Count - 1;
						methodDescriptor.name = methodInfo.Name;
					}
					methodDescriptor.baseMethods.Add(methodInfo);
					methodDescriptor.maxParameterCount = Math.Max(methodDescriptor.maxParameterCount, methodInfo.GetParameters().Length);
					if (methodInfo.IsGenericMethod)
					{
						methodDescriptor.genericParameterCount = Math.Max(methodDescriptor.genericParameterCount, methodInfo.GetGenericArguments().Length);
					}
				}
			}
			PropertyInfo[] array3 = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			PropertyInfo[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				PropertyInfo propertyInfo = array4[j];
				propertyDescriptor propertyDescriptor = null;
				if (!this.instanceProperties.TryGetValue(propertyInfo.Name, out propertyDescriptor))
				{
					propertyDescriptor = new propertyDescriptor();
					this.instanceProperties[propertyInfo.Name] = propertyDescriptor;
					this.properties.Add(propertyDescriptor);
					propertyDescriptor.propertyOrder = this.properties.Count - 1;
					propertyDescriptor.name = propertyInfo.Name;
				}
				propertyDescriptor.properties.Add(propertyInfo);
				propertyDescriptor.maxParameterCount = Math.Max(propertyDescriptor.maxParameterCount, propertyInfo.GetIndexParameters().Length);
			}
			FieldInfo[] array5 = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			FieldInfo[] array6 = array5;
			for (int k = 0; k < array6.Length; k++)
			{
				FieldInfo fieldInfo = array6[k];
				fieldDescriptor fieldDescriptor = null;
				if (!this.instanceFields.TryGetValue(fieldInfo.Name, out fieldDescriptor))
				{
					fieldDescriptor = new fieldDescriptor();
					this.instanceFields[fieldInfo.Name] = fieldDescriptor;
					this.fields.Add(fieldDescriptor);
					fieldDescriptor.fieldOrder = this.fields.Count - 1;
					fieldDescriptor.name = fieldInfo.Name;
				}
				fieldDescriptor.fieldInfo = fieldInfo;
			}
			ConstructorInfo[] constructors = t.GetConstructors();
			ConstructorInfo[] array7 = constructors;
			for (int l = 0; l < array7.Length; l++)
			{
				ConstructorInfo constructorInfo = array7[l];
				methodDescriptor methodDescriptor2;
				if (this.constructor == null)
				{
					methodDescriptor2 = new methodDescriptor();
					this.constructor = methodDescriptor2;
					this.methods.Add(methodDescriptor2);
					methodDescriptor2.name = "construct";
					methodDescriptor2.methodOrder = this.methods.Count - 1;
				}
				else
				{
					methodDescriptor2 = this.constructor;
				}
				methodDescriptor2.baseMethods.Add(constructorInfo);
				methodDescriptor2.maxParameterCount = Math.Max(methodDescriptor2.maxParameterCount, constructorInfo.GetParameters().Length);
			}
			array = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			MethodInfo[] array8 = array;
			for (int m = 0; m < array8.Length; m++)
			{
				MethodInfo methodInfo2 = array8[m];
				if (!methodInfo2.Name.StartsWith("get_") && !methodInfo2.Name.StartsWith("set_"))
				{
					methodDescriptor methodDescriptor3 = null;
					if (methodInfo2.IsGenericMethod)
					{
						string text2 = "generic_" + methodInfo2.Name;
						if (!this.instanceMethods.TryGetValue(text2, out methodDescriptor3))
						{
							methodDescriptor3 = new methodDescriptor();
							this.staticMethods[text2] = methodDescriptor3;
							methodDescriptor3.isGenericMethod = true;
							this.methods.Add(methodDescriptor3);
							methodDescriptor3.methodOrder = this.methods.Count - 1;
							methodDescriptor3.name = text2;
						}
					}
					else if (!this.staticMethods.TryGetValue(methodInfo2.Name, out methodDescriptor3))
					{
						methodDescriptor3 = new methodDescriptor();
						this.staticMethods[methodInfo2.Name] = methodDescriptor3;
						this.methods.Add(methodDescriptor3);
						methodDescriptor3.name = methodInfo2.Name;
						methodDescriptor3.methodOrder = this.methods.Count - 1;
					}
					methodDescriptor3.baseMethods.Add(methodInfo2);
					methodDescriptor3.maxParameterCount = Math.Max(methodDescriptor3.maxParameterCount, methodInfo2.GetParameters().Length);
					if (methodInfo2.IsGenericMethod)
					{
						methodDescriptor3.genericParameterCount = Math.Max(methodDescriptor3.genericParameterCount, methodInfo2.GetGenericArguments().Length);
					}
				}
			}
			array3 = t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			PropertyInfo[] array9 = array3;
			for (int n = 0; n < array9.Length; n++)
			{
				PropertyInfo propertyInfo2 = array9[n];
				propertyDescriptor propertyDescriptor2 = null;
				if (!this.staticProperties.TryGetValue(propertyInfo2.Name, out propertyDescriptor2))
				{
					propertyDescriptor2 = new propertyDescriptor();
					this.staticProperties[propertyInfo2.Name] = propertyDescriptor2;
					this.properties.Add(propertyDescriptor2);
					propertyDescriptor2.propertyOrder = this.properties.Count - 1;
					propertyDescriptor2.name = propertyInfo2.Name;
				}
				propertyDescriptor2.properties.Add(propertyInfo2);
				propertyDescriptor2.maxParameterCount = Math.Max(propertyDescriptor2.maxParameterCount, propertyInfo2.GetIndexParameters().Length);
			}
			array5 = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			FieldInfo[] array10 = array5;
			for (int num = 0; num < array10.Length; num++)
			{
				FieldInfo fieldInfo2 = array10[num];
				fieldDescriptor fieldDescriptor2 = null;
				if (!this.instanceFields.TryGetValue(fieldInfo2.Name, out fieldDescriptor2))
				{
					fieldDescriptor2 = new fieldDescriptor();
					this.staticFields[fieldInfo2.Name] = fieldDescriptor2;
					this.fields.Add(fieldDescriptor2);
					fieldDescriptor2.fieldOrder = this.fields.Count - 1;
					fieldDescriptor2.name = fieldInfo2.Name;
				}
				fieldDescriptor2.fieldInfo = fieldInfo2;
			}
			this.type = t;
			typeDescriptor.loadedTypes[t] = this;
			if (compile && typeDescriptor.gencompile)
			{
				this.compile();
			}
		}

		public static string getNameForType(Type t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			if (!t.IsGenericType)
			{
				result = t.ToString().Replace("+", ".").Replace("&", "");
			}
			else
			{
				string text = t.ToString();
				int length = text.IndexOf("`");
				text = text.Substring(0, length);
				stringBuilder.Append(text);
				stringBuilder.Append("<");
				Type[] genericArguments = t.GetGenericArguments();
				bool flag = false;
				Type[] array = genericArguments;
				for (int i = 0; i < array.Length; i++)
				{
					Type t2 = array[i];
					if (!flag)
					{
						flag = true;
					}
					else
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(typeDescriptor.getNameForType(t2));
				}
				stringBuilder.Append(">");
				result = stringBuilder.ToString();
			}
			return result;
		}

		internal static void loadEvaluator()
		{
		}

		public static typeDescriptor loadFromType(Type t)
		{
			typeDescriptor typeDescriptor;
			typeDescriptor result;
			if (!typeDescriptor.loadedTypes.TryGetValue(t, out typeDescriptor))
			{
				result = new typeDescriptor(t);
			}
			else
			{
				result = typeDescriptor;
			}
			return result;
		}

		public static typeDescriptor loadFromType(Type t, string typeName, bool compile = true)
		{
			typeDescriptor typeDescriptor;
			typeDescriptor result;
			if (!typeDescriptor.loadedTypes.TryGetValue(t, out typeDescriptor))
			{
				result = new typeDescriptor(t, typeName, compile);
			}
			else
			{
				result = typeDescriptor;
			}
			return result;
		}

		public MethodBase getMethodForParameters(string method, ref object[] parameters)
		{
			methodDescriptor methodDescriptor = this.instanceMethods[method];
			return methodDescriptor.getMethodForParameters(ref parameters, null);
		}

		public MethodBase getStaticMethodForParameters(string method, ref object[] parameters)
		{
			methodDescriptor methodDescriptor = this.staticMethods[method];
			return methodDescriptor.getMethodForParameters(ref parameters, null);
		}

		public PropertyInfo getPropertyForParameters(string property, ref object[] parameters)
		{
			propertyDescriptor propertyDescriptor = this.instanceProperties[property];
			return propertyDescriptor.getPropertyForParameters(ref parameters);
		}

		public PropertyInfo getStaticPropertyForParameters(string property, ref object[] parameters)
		{
			propertyDescriptor propertyDescriptor = this.staticProperties[property];
			return propertyDescriptor.getPropertyForParameters(ref parameters);
		}

		public object invokeMethod(object o, string method, params object[] args)
		{
			MethodBase methodForParameters = this.getMethodForParameters(method, ref args);
			return methodForParameters.Invoke(o, args);
		}

		public void invokeMethod(out object o, object ox, string method, params object[] args)
		{
			MethodBase methodForParameters = this.getMethodForParameters(method, ref args);
			o = methodForParameters.Invoke(ox, args);
		}

		public object getProperty(object o, string property, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			return propertyForParameters.GetValue(o, args);
		}

		public void getProperty(out object o, object ox, string property, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			o = propertyForParameters.GetValue(ox, args);
		}

		public void setProperty(object o, string property, object value, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			propertyForParameters.SetValue(o, value, args);
		}

		public object getStaticProperty(object o, string property, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			return staticPropertyForParameters.GetValue(o, args);
		}

		public void getStaticProperty(out object o, object ox, string property, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			o = staticPropertyForParameters.GetValue(ox, args);
		}

		public void setStaticProperty(object o, string property, object value, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			staticPropertyForParameters.SetValue(o, value, args);
		}

		public object invokeStaticMethod(object o, string method, params object[] args)
		{
			MethodBase staticMethodForParameters = this.getStaticMethodForParameters(method, ref args);
			return staticMethodForParameters.Invoke(o, args);
		}

		public void invokeStaticMethod(out object o, object ox, string method, params object[] args)
		{
			MethodBase staticMethodForParameters = this.getStaticMethodForParameters(method, ref args);
			o = staticMethodForParameters.Invoke(ox, args);
		}

		public static void addUsingsStatements(StringBuilder sb)
		{
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Runtime.InteropServices;");
			sb.AppendLine("using System.Reflection;");
			sb.AppendLine("using jxshell.dotnet4;");
		}

		public void precompile(StringBuilder sb, ref string staticClass, ref string instanceClass)
		{
			sb.AppendLine("namespace jxshell.dotnet4{");
			string text = "_" + environment.uniqueId();
			string text2 = "_" + environment.uniqueId();
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(text2).Append(" : ");
			if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("delegateWrapperStatic{");
			}
			else
			{
				sb.Append("wrapperStatic{");
			}
			sb.AppendLine();
			sb.Append("public ").Append(text2).Append("(Type t, typeDescriptor td):base(t,td){");
			if (this.type.IsEnum)
			{
				sb.Append("__initEnum();");
			}
			sb.Append("}");
			sb.AppendLine();
			sb.Append("public override ").Append("wrapper").Append(" getWrapper(object o){return new ").Append(text).Append("(o,typeD);}");
			sb.AppendLine();
			if (!this.type.IsEnum)
			{
				sb.AppendLine("/* FIELDS */");
				foreach (KeyValuePair<string, fieldDescriptor> current in this.staticFields)
				{
					sb.Append("public object ").Append(current.Value.name).Append("{");
					sb.Append("get{");
					sb.Append("var fielD = typeD.fields[").Append(current.Value.fieldOrder).Append("];");
					sb.Append("return fielD.getValue(null);");
					sb.Append("}");
					sb.AppendLine();
					sb.Append("set{");
					sb.Append("var fielD = typeD.fields[").Append(current.Value.fieldOrder).Append("];");
					sb.Append("fielD.setValue(value, null);");
					sb.Append("}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("/* MÉTODOS */");
			foreach (KeyValuePair<string, methodDescriptor> current2 in this.staticMethods)
			{
				sb.Append("public object ").Append(current2.Value.name).Append("(");
				StringBuilder stringBuilder = new StringBuilder();
				bool isGenericMethod = current2.Value.isGenericMethod;
				if (isGenericMethod)
				{
					stringBuilder.Append("System.Collections.Generic.List<System.Type> genericArguments=new System.Collections.Generic.List<System.Type>(1);");
					for (int i = 0; i < current2.Value.genericParameterCount; i++)
					{
						if (i > 0)
						{
							sb.Append(",");
						}
						stringBuilder.Append("if(type").Append(i).Append("!=null){if(type").Append(i);
						stringBuilder.Append(" is wrapper){genericArguments.Add((System.Type)(((wrapper)type").Append(i).Append(").wrappedObject));}else{");
						stringBuilder.Append("genericArguments.Add(Manager.lastManager.getTypeOrGenericType(type").Append(i).Append(".ToString()));}").Append("}");
						sb.Append("[Optional] object ").Append("type").Append(i);
						stringBuilder.AppendLine();
					}
				}
				stringBuilder.Append("object[] args = {");
				for (int j = 0; j < current2.Value.maxParameterCount; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(",");
					}
					if (j > 0 || isGenericMethod)
					{
						sb.Append(",");
					}
					stringBuilder.Append("a").Append(j);
					sb.Append("[Optional] object ").Append("a").Append(j);
				}
				sb.Append("){");
				stringBuilder.Append("}");
				sb.AppendLine();
				sb.Append(stringBuilder).Append(";");
				sb.AppendLine();
				sb.Append("var m = typeD.methods[").Append(current2.Value.methodOrder).Append("];");
				sb.AppendLine();
				if (isGenericMethod)
				{
					sb.Append("var method = m.getGenericMethodForParameters(genericArguments.ToArray(), ref args);");
				}
				else
				{
					sb.Append("var method = m.getMethodForParameters(ref args);");
				}
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("return __process(method.Invoke(null,args));");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> current3 in this.staticProperties)
			{
				sb.Append("public object ");
				if (current3.Value.maxParameterCount > 0)
				{
					sb.Append("this[");
				}
				else
				{
					sb.Append(current3.Value.name);
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				if (current3.Value.maxParameterCount > 0)
				{
					stringBuilder2.Append("object[] args = {");
					for (int k = 0; k < current3.Value.maxParameterCount; k++)
					{
						if (k > 0)
						{
							stringBuilder2.Append(",");
							sb.Append(",");
						}
						stringBuilder2.Append("a").Append(k);
						sb.Append("[Optional] object ").Append("a").Append(k);
					}
					sb.Append("]");
					stringBuilder2.Append("};");
				}
				else
				{
					stringBuilder2.Append("object[] args = {};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder2);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(current3.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("return __process(method.GetValue(null,args));");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("set{");
				sb.Append(stringBuilder2);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(current3.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.AppendLine("if(value is wrapper){value=((wrapper)value).wrappedObject;}");
				sb.Append("method.SetValue(null,value,args);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("}");
			}
			if (this.type.IsEnum)
			{
				string[] names = Enum.GetNames(this.type);
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("Type ty = typeof(").Append(typeDescriptor.getNameForType(this.type)).Append(");");
				stringBuilder3.Append("Array values = global::System.Enum.GetValues(ty);");
				stringBuilder3.AppendLine();
				int num = 0;
				string[] array = names;
				for (int l = 0; l < array.Length; l++)
				{
					string value = array[l];
					sb.Append("public ").Append(text).Append(" ").Append(value).Append("= new ").Append(text).Append("();").AppendLine();
					stringBuilder3.Append(value).Append(".wrappedObject = values.GetValue(").Append(num).Append(");");
					stringBuilder3.AppendLine();
					stringBuilder3.Append(value).Append(".wrappedType = ty;");
					stringBuilder3.AppendLine();
					stringBuilder3.Append(value).Append(".typeD = typeD;");
					stringBuilder3.AppendLine();
					num++;
				}
				sb.Append("public void __initEnum(){").AppendLine().Append(stringBuilder3.ToString()).AppendLine().Append("}");
			}
			methodDescriptor methodDescriptor = this.constructor;
			if (methodDescriptor != null)
			{
				if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
				{
					sb.Append("public override delegateWrapper getThisWrapper(){");
					sb.Append("var o = new ").Append(text).Append("();return o;");
					sb.Append("}");
				}
				else
				{
					sb.Append("public object ").Append(methodDescriptor.name).Append("(");
					StringBuilder stringBuilder4 = new StringBuilder();
					stringBuilder4.Append("object[] args = {");
					for (int m = 0; m < methodDescriptor.maxParameterCount; m++)
					{
						if (m > 0)
						{
							stringBuilder4.Append(",");
							sb.Append(",");
						}
						stringBuilder4.Append("a").Append(m);
						sb.Append("[Optional] object ").Append("a").Append(m);
					}
					sb.Append("){");
					stringBuilder4.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder4).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(methodDescriptor.methodOrder).Append("];");
					sb.AppendLine();
					sb.Append("var method = m.getConstructorForParameters(ref args);");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("return getWrapper(method.Invoke(args));");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("}");
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(text).Append(" : ");
			if (this.type.IsEnum)
			{
				sb.Append("enumW");
			}
			else if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("delegateW");
			}
			else
			{
				sb.Append("w");
			}
			sb.Append("rapper{").AppendLine();
			sb.Append("public ").Append(text).Append("(object o, typeDescriptor td):base(o,td){}");
			sb.Append("public ").Append(text).Append("():base(){}");
			sb.AppendLine();
			if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
			}
			sb.AppendLine();
			sb.AppendLine("/* FIELDS */");
			foreach (KeyValuePair<string, fieldDescriptor> current4 in this.instanceFields)
			{
				sb.Append("public object ").Append(current4.Value.name).Append("{");
				sb.Append("get{");
				sb.Append("var fielD = typeD.fields[").Append(current4.Value.fieldOrder).Append("];");
				sb.Append("return fielD.getValue(wrappedObject);");
				sb.Append("}");
				sb.AppendLine();
				sb.Append("set{");
				sb.Append("var fielD = typeD.fields[").Append(current4.Value.fieldOrder).Append("];");
				sb.Append("fielD.setValue(value, wrappedObject);");
				sb.Append("}");
				sb.AppendLine();
				sb.AppendLine("}");
			}
			sb.AppendLine("/* MÉTODOS */");
			foreach (KeyValuePair<string, methodDescriptor> current5 in this.instanceMethods)
			{
				if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType) && current5.Value.name == "Invoke")
				{
					sb.Append("public object call(");
					StringBuilder stringBuilder5 = new StringBuilder();
					StringBuilder stringBuilder6 = new StringBuilder();
					stringBuilder5.Append("");
					for (int n = 0; n < current5.Value.maxParameterCount; n++)
					{
						if (n > 0)
						{
							stringBuilder5.Append(",");
							stringBuilder6.Append(",");
							sb.Append(",");
						}
						stringBuilder6.Append("wrapper.getFromObject(a").Append(n).Append(")");
						stringBuilder5.Append("a").Append(n);
						sb.Append("[Optional] object ").Append("a").Append(n);
					}
					sb.Append("){");
					sb.AppendLine();
					int maxParameterCount = current5.Value.maxParameterCount;
					sb.Append("invokerparam").Append(maxParameterCount).Append(" invoker = new invokerparam").Append(maxParameterCount).Append("(__internalMethod);");
					sb.Append("object o =null;");
					MethodInfo methodInfo = (MethodInfo)current5.Value.baseMethods[0];
					if (methodInfo.ReturnType == typeof(void))
					{
						sb.Append("invoker.invokeasVoid");
					}
					else
					{
						sb.Append("o=invoker.invoke");
					}
					sb.Append("(__internalTarget");
					if (current5.Value.maxParameterCount > 0)
					{
						sb.Append(",");
					}
					sb.Append(stringBuilder5.ToString()).Append(");");
					sb.AppendLine();
					sb.AppendLine("return o;}");
					sb.Append("public ");
					if (methodInfo.ReturnType == typeof(void))
					{
						sb.Append("void ");
					}
					else
					{
						sb.Append(typeDescriptor.getNameForType(methodInfo.ReturnType));
					}
					sb.Append(" __internalInvoke(");
					ParameterInfo[] parameters = methodInfo.GetParameters();
					for (int num2 = 0; num2 < parameters.Length; num2++)
					{
						ParameterInfo parameterInfo = parameters[num2];
						if (num2 > 0)
						{
							sb.Append(",");
						}
						string value2;
						if (parameterInfo.ParameterType.IsPointer)
						{
							value2 = "object";
						}
						else
						{
							value2 = typeDescriptor.getNameForType(parameterInfo.ParameterType);
						}
						sb.Append(value2).Append(" a").Append(num2);
					}
					sb.Append("){");
					sb.AppendLine();
					if (methodInfo.ReturnType == typeof(void))
					{
						sb.Append("call(").Append(stringBuilder6.ToString()).Append(");");
					}
					else
					{
						sb.Append("object o= ");
						sb.Append("call(").Append(stringBuilder6.ToString()).Append(");");
						sb.AppendLine();
						sb.Append("if(o is wrapper){o = ((wrapper)o).wrappedObject;}");
						sb.AppendLine();
						sb.Append("return (").Append(typeDescriptor.getNameForType(methodInfo.ReturnType)).Append(")o;");
						sb.AppendLine();
					}
					sb.Append("}\n");
				}
				else
				{
					sb.Append("public object ").Append(current5.Value.name).Append("(");
					StringBuilder stringBuilder7 = new StringBuilder();
					bool isGenericMethod2 = current5.Value.isGenericMethod;
					if (isGenericMethod2)
					{
						stringBuilder7.Append("System.Collections.Generic.List<System.Type> genericArguments=new System.Collections.Generic.List<System.Type>(1);");
						for (int num3 = 0; num3 < current5.Value.genericParameterCount; num3++)
						{
							if (num3 > 0)
							{
								sb.Append(",");
							}
							stringBuilder7.Append("if(type").Append(num3).Append("!=null){if(type").Append(num3);
							stringBuilder7.Append(" is wrapper){genericArguments.Add((System.Type)(((wrapper)type").Append(num3).Append(").wrappedObject));}else{");
							stringBuilder7.Append("genericArguments.Add(Manager.lastManager.getTypeOrGenericType(type").Append(num3).Append(".ToString()));}").Append("}");
							sb.Append("[Optional] object ").Append("type").Append(num3);
							stringBuilder7.AppendLine();
						}
					}
					stringBuilder7.Append("object[] args = {");
					for (int num4 = 0; num4 < current5.Value.maxParameterCount; num4++)
					{
						if (num4 > 0)
						{
							stringBuilder7.Append(",");
						}
						if (num4 > 0 || isGenericMethod2)
						{
							sb.Append(",");
						}
						stringBuilder7.Append("a").Append(num4);
						sb.Append("[Optional] object ").Append("a").Append(num4);
					}
					sb.Append("){");
					stringBuilder7.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder7).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(current5.Value.methodOrder).Append("];");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("var method = m.getMethodForParameters(ref args);");
					sb.AppendLine();
					sb.Append("return __process(method.Invoke(wrappedObject,args));");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> current6 in this.instanceProperties)
			{
				sb.Append("public object ");
				if (current6.Value.maxParameterCount > 0)
				{
					sb.Append("this[");
				}
				else
				{
					sb.Append(current6.Value.name);
				}
				StringBuilder stringBuilder8 = new StringBuilder();
				if (current6.Value.maxParameterCount > 0)
				{
					stringBuilder8.Append("object[] args = {");
					for (int num5 = 0; num5 < current6.Value.maxParameterCount; num5++)
					{
						if (num5 > 0)
						{
							stringBuilder8.Append(",");
							sb.Append(",");
						}
						stringBuilder8.Append("a").Append(num5);
						sb.Append("[Optional] object ").Append("a").Append(num5);
					}
					sb.Append("]");
					stringBuilder8.Append("};");
				}
				else
				{
					stringBuilder8.Append("object[] args = {};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder8);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(current6.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.Append("object o= method.GetValue(wrappedObject,args);");
				sb.AppendLine("return __process(o);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("set{");
				sb.Append(stringBuilder8);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(current6.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.AppendLine("if(value is wrapper){value=((wrapper)value).wrappedObject;}");
				sb.Append("method.SetValue(wrappedObject,value,args);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("}");
			}
			sb.AppendLine("}}");
			staticClass = text2;
			instanceClass = text;
		}

		public bool isCompiled()
		{
			return this.compiled;
		}

		public wrapperStatic compile()
		{
			wrapperStatic result;
			if (this.compiled)
			{
				result = this.compiledWrapper;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				string str = "";
				string text = "";
				typeDescriptor.addUsingsStatements(stringBuilder);
				this.precompile(stringBuilder, ref str, ref text);
				stringBuilder.AppendLine("class program{public static void main(){}}");
				try
				{
					csharplanguage csharplanguage = typeDescriptor.language;
					//Clipboard.SetText(stringBuilder.ToString());
					csharplanguage.runScript(stringBuilder.ToString(), typeDescriptor.generateInMemory);
					Type type = csharplanguage.getCompiledAssembly().GetType("jxshell.dotnet4." + str);
					ConstructorInfo constructorInfo = type.GetConstructor(new Type[]
						{
							typeof(Type),
							typeof(typeDescriptor)
						});
					this.compiledWrapper = (wrapperStatic)constructorInfo.Invoke(new object[]
						{
							this.type,
							this
						});
					this.compiled = true;
				}
				catch (Exception ex)
				{
					throw new Exception("No se puede obtener un wrapper para el tipo " + this.type.ToString() + ". " + ex.Message, ex);
				}
				result = this.compiledWrapper;
			}
			return result;
		}

		public void setCompiledWrapper(wrapperStatic ww)
		{
			this.compiled = true;
			this.compiledWrapper = ww;
		}
	}
}
