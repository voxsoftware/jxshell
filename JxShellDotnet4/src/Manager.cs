using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.dotnet4
{
	[ComVisible(true), Guid("9173A427-2F3B-405D-9B0F-23C7B7048113"), ProgId("jxshell.dotnet4")]
	public class Manager : IDisposable
	{
		public static Manager lastManager = null;

		//private license license = null;

		public ResolveEventHandler assemblyResolve;

		private Dictionary<string, Assembly> fileAssemblies = new Dictionary<string, Assembly>();

		private static bool environmentInit = false;

		public List<Assembly> assemblies = new List<Assembly>();

		public Dictionary<string, Type> loadedTypes = new Dictionary<string, Type>();

		/*public license getLicense()
		{
			return this.license;
		}*/

		public object getBytesFromString(string s)
		{
			return wrapper.getFromObject(Encoding.ASCII.GetBytes(s));
		}

		private byte[] getBytesFromString(string s, bool xprivate)
		{
			return Encoding.ASCII.GetBytes(s);
		}


		/*
		public void prueba()
		{
			string path = "C:\\Users\\James\\Documents\\jxshell.dotnet4\\Samples\\utf8\\textoenutf8.txt";
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
			MessageBox.Show(text);
		}
		*/
		public string getTypeString(wrapperBase typex)
		{
			return typeDescriptor.getNameForType((Type)typex.wrappedObject);
		}

		public object getUTF8WrappedString(object s)
		{
			byte[] bytes;
			if (s is wrapper)
			{
				bytes = (byte[])((wrapper)s).wrappedObject;
			}
			else
			{
				bytes = this.getBytesFromString(s.ToString(), true);
			}
			string @string = Encoding.UTF8.GetString(bytes);
			return wrapper.createWrapper(@string, typeDescriptor.loadFromType(typeof(string)));
		}

		public void loadManyTypes(string types)
		{
			string[] array = types.Split(new string[]
				{
					"-"
				}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();
			typeDescriptor.addUsingsStatements(stringBuilder);
			Dictionary<Type, type_1> dictionary = new Dictionary<Type, type_1>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string typeName = array2[i];
				Type typeOrGenericType = this.getTypeOrGenericType(typeName);
				typeDescriptor typeDescriptor = new typeDescriptor(typeOrGenericType, typeName, false);
				if (!typeDescriptor.isCompiled())
				{
					string staticClass = "";
					string instanceClass = "";
					typeDescriptor.precompile(stringBuilder, ref staticClass, ref instanceClass);
					dictionary[typeOrGenericType] = new type_1
					{
						td = typeDescriptor,
						staticClass = staticClass,
						instanceClass = instanceClass,
						t = typeOrGenericType
					};
				}
			}
			stringBuilder.AppendLine("class program{public static void main(){}}");
			csharplanguage csharplanguage = (csharplanguage)language.defaultLanguage.create();
			if (dictionary.Count > 0)
			{
				csharplanguage.runScript(stringBuilder.ToString(), typeDescriptor.generateInMemory);
			}
			foreach (KeyValuePair<Type, type_1> current in dictionary)
			{
				type_1 value = current.Value;
				Type type = csharplanguage.getCompiledAssembly().GetType("jxshell.dotnet4." + value.staticClass);
				ConstructorInfo constructor = type.GetConstructor(new Type[]
					{
						typeof(Type),
						typeof(typeDescriptor)
					});
				value.td.setCompiledWrapper((wrapperStatic)constructor.Invoke(new object[]
					{
						value.t,
						value.td
					}));
			}
		}

		public void loadManyTypes(Type[] types)
		{
			StringBuilder stringBuilder = new StringBuilder();
			typeDescriptor.addUsingsStatements(stringBuilder);
			Dictionary<Type, type_1> dictionary = new Dictionary<Type, type_1>();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (!type.IsGenericType && type.IsPublic)
				{
					typeDescriptor typeDescriptor = new typeDescriptor(type, typeDescriptor.getNameForType(type), false);
					if (!typeDescriptor.isCompiled())
					{
						string staticClass = "";
						string instanceClass = "";
						typeDescriptor.precompile(stringBuilder, ref staticClass, ref instanceClass);
						dictionary[type] = new type_1
						{
							td = typeDescriptor,
							staticClass = staticClass,
							instanceClass = instanceClass,
							t = type
						};
					}
				}
			}
			stringBuilder.AppendLine("class program{public static void main(){}}");
			csharplanguage csharplanguage = (csharplanguage)language.defaultLanguage.create();
			if (dictionary.Count > 0)
			{
				csharplanguage.runScript(stringBuilder.ToString(), typeDescriptor.generateInMemory);
			}
			foreach (KeyValuePair<Type, type_1> current in dictionary)
			{
				type_1 value = current.Value;
				Type type2 = csharplanguage.getCompiledAssembly().GetType("jxshell.dotnet4." + value.staticClass);
				ConstructorInfo constructor = type2.GetConstructor(new Type[]
					{
						typeof(Type),
						typeof(typeDescriptor)
					});
				value.td.setCompiledWrapper((wrapperStatic)constructor.Invoke(new object[]
					{
						value.t,
						value.td
					}));
			}
		}

		public void registerVFPClassToDotnet(string code, string vfpclassname, string netclassName)
		{
		}

		public wrapper getObjectAsType(object o, object xt)
		{
			Type type;
			if (xt is wrapper)
			{
				wrapper wrapper = (wrapper)xt;
				type = (Type)wrapper.wrappedObject;
			}
			else
			{
				type = this.getTypeOrGenericType(xt.ToString());
			}
			object o2 = Convert.ChangeType(o, type);
			typeDescriptor td = typeDescriptor.loadFromType(type);
			return wrapper.createWrapper(o2, td);
		}

		public void Dispose()
		{
			/*
			if (this.license != null)
			{
				this.license.stopValidate();
			}
			*/
		}

		public void init()
		{
			//this.license = new license();
			Manager.lastManager = this;
			if (!Manager.environmentInit)
			{
				ResolveEventHandler value = delegate(object sender, ResolveEventArgs args)
				{
					Assembly result = null;
					if (this.fileAssemblies.TryGetValue(args.Name, out result))
					{
					}
					return result;
				};
				AppDomain.CurrentDomain.AssemblyResolve += value;
				environment.initEnvironment();
				Manager.environmentInit = true;
			}
			this.loadAssembly(typeof(Console).Assembly);
			this.loadAssembly(typeof(WebClient).Assembly);
			this.loadAssembly(typeof(typeDescriptor).Assembly);
			//this.loadAssembly(typeof(Form).Assembly);
		}

		public void loadAssembly(string name)
		{
			this.add(Assembly.Load(name));
		}

		public void loadAssemblyPartialName(string name)
		{
			this.add(Assembly.LoadWithPartialName(name));
		}

		public void loadAssembly(Assembly a)
		{
			this.add(a);
		}

		public void loadAssemblyFile(string file)
		{
			string item = environment.addBs(Path.GetDirectoryName(file));
			if (environment.directories.IndexOf(item) < 0)
			{
				environment.directories.Add(item);
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			try
			{
				Assembly assembly = Assembly.LoadFile(file);
				this.fileAssemblies[assembly.FullName] = assembly;
				this.fileAssemblies[fileNameWithoutExtension] = assembly;
				this.add(assembly);
			}
			catch (ReflectionTypeLoadException ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Se encontró los siguientes errores al cargar el ensamblado:");
				Exception[] loaderExceptions = ex.LoaderExceptions;
				for (int i = 0; i < loaderExceptions.Length; i++)
				{
					Exception ex2 = loaderExceptions[i];
					stringBuilder.AppendLine(ex2.ToString());
				}
				throw new Exception(stringBuilder.ToString());
			}
		}

		public void add(Assembly a)
		{
			//this.validLicense();
			if (this.assemblies.IndexOf(a) < 0)
			{
				this.assemblies.Add(a);
				environment.loadAssembly(a, true);
				Type[] types = a.GetTypes();
				Type[] array = types;
				for (int i = 0; i < array.Length; i++)
				{
					Type type = array[i];
					if (type.IsGenericType)
					{
						string text = type.ToString();
						string text2 = text.Replace("+", ".");
						int length = text.IndexOf("[");
						this.loadedTypes[text.Substring(0, length)] = type;
						if (text2 != text)
						{
							this.loadedTypes[text2.Substring(0, length)] = type;
						}
					}
					else
					{
						string text3 = type.ToString();
						string text4 = text3.Replace("+", ".");
						this.loadedTypes[text3] = type;
						if (text4 != text3)
						{
							this.loadedTypes[text4] = type;
						}
					}
				}
			}
		}

		public void setThreadedLibraryFile(string file)
		{
			//com.libraryFile = file;
		}

		/*
		public object createThread(string code, object target, string method, object arg = null)
		{
			object arg2 = null;
			if (!(arg is Missing))
			{
				arg2 = arg;
			}
			return thread.create(code, target, method, arg2);
		}
		*/

		/*
		public void validLicense()
		{
			if (!this.license.isValidLicense())
			{
				throw new Exception("Su período de prueba ha caducado. Debe conseguir una licencia para seguir utilizando el programa.");
			}
		}
		*/

		public wrapperStatic getStaticWrapper(string typeName)
		{
			//this.validLicense();
			Type typeOrGenericType = this.getTypeOrGenericType(typeName);
			return wrapperStatic.loadFromType(typeOrGenericType);
		}

		public object getTypeFromString(string name)
		{
			//this.validLicense();
			return wrapper.getFromObject(this.getTypeOrGenericType(name));
		}

		public wrapper getTypeFromObject(object o)
		{
			wrapper result;
			if (o == null)
			{
				result = (wrapper)wrapper.getFromObject(typeof(DBNull));
			}
			else
			{
				if (o is wrapper)
				{
					o = ((wrapper)o).wrappedObject;
				}
				result = (wrapper)wrapper.getFromObject(o.GetType());
			}
			return result;
		}

		public wrapper getDefaultFor(object type)
		{
			//this.validLicense();
			Type type2;
			if (type is wrapper)
			{
				type2 = (Type)((wrapper)type).wrappedObject;
			}
			else
			{
				type2 = this.getTypeOrGenericType(type.ToString());
			}
			object o = null;
			if (type2.IsValueType)
			{
				o = Activator.CreateInstance(type2);
			}
			return wrapper.createWrapper(o, typeDescriptor.loadFromType(type2));
		}

		public Type getTypeOrGenericType(string typeName)
		{
			string[] array = typeName.Split(new char[]
				{
					'<'
				});
			Type result;
			if (array.Length > 1)
			{
				if (array[1].IndexOf(">") < 0)
				{
					throw new Exception("El nombre del tipo no es válido.");
				}
				int length = array[1].LastIndexOf('>');
				string text = array[1].Substring(0, length);
				string[] array2 = text.Split(new char[]
					{
						','
					});
				List<Type> list = new List<Type>();
				string[] array3 = array2;
				for (int i = 0; i < array3.Length; i++)
				{
					string text2 = array3[i];
					string typeName2 = text2.Trim();
					list.Add(this.getTypeOrGenericType(typeName2));
				}
				Type[] array4 = list.ToArray();
				Type typeOrGenericType = this.getTypeOrGenericType(array[0] + "`" + array4.Length.ToString());
				Type type = typeOrGenericType.MakeGenericType(array4);
				typeDescriptor.loadFromType(type, typeName, true);
				result = type;
			}
			else if (typeName.IndexOf("[") > 0)
			{
				int num = typeName.IndexOf('[');
				string typeName3 = typeName.Substring(0, num);
				string text3 = typeName.Substring(num);
				Type type2 = this.getTypeOrGenericType(typeName3);
				char c = text3[0];
				int j = 0;
				int num2 = 1;
				while (j < text3.Length)
				{
					while (c != ']')
					{
						if (c == ',')
						{
							num2++;
						}
						j++;
						c = text3[j];
					}
					j++;
					type2 = type2.MakeArrayType(num2);
					num2 = 1;
				}
				result = type2;
			}
			else
			{
				Type type3;
				if (!this.loadedTypes.TryGetValue(typeName, out type3))
				{
					throw new Exception("El tipo especificado no se encontró. Revise si debe cargar un ensamblado.");
				}
				result = type3;
			}
			return result;
		}

		public typeDescriptor loadType(string typeName)
		{
			Type typeOrGenericType = this.getTypeOrGenericType(typeName);
			return typeDescriptor.loadFromType(typeOrGenericType);
		}
	}
}
