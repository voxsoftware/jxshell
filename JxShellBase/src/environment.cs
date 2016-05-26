using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
//using System.Windows.Forms;

namespace jxshell
{
	public class environment
	{
		private static int uniqueId_ = 0;
		//public static bool linux = false;
		//public static bool windows = false;
		public static string[] argv = new string[0];
		public static string environmentPath = "";
		private static string exPath = "";
		public static string libraryPath = "";
		public static string appdataPath = "";
		public static string compilationPath = "";
		public static string applicationsPath = "";
		public static string languagePath = "";
		public static string globalAssemblyPath = "";
		public static string commonLibraryPath = "";
		public static string commonApplicationsPath = "";
		public static List<Assembly> assemblies = new List<Assembly>();
		//public static application application = null;
		private static Random r = new Random();

		private static string _lastPath = "";

		public static List<string> directories = new List<string>();

		public static string executablePath
		{
			get
			{
				return environment.exPath;
			}
		}

		public static OperatingSystem os{
			get{
				return Environment.OSVersion;
			}
		}


		public static bool windows {
			get {
				return os.Platform == PlatformID.Win32NT;
			}
		}


		public static bool linux{
			get{
				return os.Platform == PlatformID.Unix;
			}
		}

		public static bool unix{
			get{
				return linux||osx;
			}
		}

		public static bool osx{
			get{
				return os.Platform == PlatformID.MacOSX;
			}
		}




		public static string executableFile
		{
			get
			{
				return Assembly.GetExecutingAssembly().Location;
			}
		}




		public static string locateInGlobalPath(string file)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			string[] array = Directory.GetDirectories(environment.globalAssemblyPath);
			string text = string.Format(environment.globalAssemblyPath + "{0}.dll", fileNameWithoutExtension);
			if (!File.Exists(text))
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string path = array2[i];
					text = string.Format(environment.addBs(path) + "{0}.dll", fileNameWithoutExtension);
					if (File.Exists(text))
					{
						break;
					}
					text = "";
				}
			}
			return text;
		}


		/*
		public static library loadLibrary(string name)
		{
			bool flag = true;
			string text = "";
			try
			{
				text = environment.addBs(name) + "library.jap";
				if (!File.Exists(text))
				{
					try
					{
						text = environment.addBs(Path.GetDirectoryName(environment.getLastPath())) + environment.addBs(name) + "library.jap";
					}
					catch (Exception)
					{
					}
					if (!File.Exists(text))
					{
						text = environment.libraryPath + environment.addBs(name) + "library.jap";
						if (!File.Exists(text))
						{
							text = environment.commonLibraryPath + environment.addBs(name) + "library.jap";
							if (!File.Exists(text))
							{
								flag = false;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			if (!flag)
			{
				throw new Exception("La libreria especificada no se encontro.");
			}
			library library = environment.createLibraryApplicationFrom(text);
			library.compile();
			return library;
		}

		public static void loadSpecialLibrary(string name)
		{
			bool flag = true;
			string path = "";
			try
			{
				path = environment.addBs(name) + "library.jap";
				if (!File.Exists(path))
				{
					try
					{
						path = environment.addBs(Path.GetDirectoryName(environment.getLastPath())) + environment.addBs(name) + "library.jap";
					}
					catch (Exception)
					{
					}
					if (!File.Exists(path))
					{
						path = environment.libraryPath + environment.addBs(name) + "library.jap";
						if (!File.Exists(path))
						{
							path = environment.commonLibraryPath + environment.addBs(name) + "library.jap";
							if (!File.Exists(path))
							{
								flag = false;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			if (!flag)
			{
				throw new Exception("La libreria especificada no se encontro.");
			}
		}
		*/


		public static string uniqueId()
		{
			environment.uniqueId_++;
			if (environment.uniqueId_ > 1000)
			{
				environment.uniqueId_ = 0;
			}
			return string.Concat(new string[]
				{
					"_",
					DateTime.Now.Ticks.GetHashCode().ToString("x"),
					environment.r.Next().ToString(),
					"_",
					environment.uniqueId_.ToString()
				});
		}

		public static string getCompilationFile()
		{
			return environment.compilationPath + environment.uniqueId() + ".$$.dll";
		}

		public static string getCompilationFile(string id)
		{
			return environment.compilationPath + id + ".$$.dll";
		}

		public static string getDirectoryPathForUri(Uri u)
		{
			string result;
			if (u.IsFile)
			{
				result = environment.addBs(Path.GetDirectoryName(u.LocalPath));
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (u.Scheme != "file")
				{
					stringBuilder.Append(u.Scheme).Append(":/");
				}
				else
				{
					stringBuilder.Append(u.Scheme).Append("://");
				}
				stringBuilder.Append(u.Authority);
				string value = string.Join("", u.Segments, 0, u.Segments.Length - 1);
				stringBuilder.Append(value);
				result = environment.addBs(stringBuilder.ToString());
			}
			return result;
		}

		public static void initEnvironment()
		{
			environment.exPath = environment.addBs(Path.GetDirectoryName(environment.executableFile));
			PropertyInfo property = typeof(TimeZoneInfo).GetProperty("TimeZoneDirectory", BindingFlags.Static | BindingFlags.NonPublic);
			if (property != null)
			{
				string value = Path.Combine(environment.exPath, "zoneinfo");
				property.SetValue(null, value, new object[0]);
			}
			if (environment.windows)
			{
				environment.environmentPath = environment.addBs(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))) + "70\\";
			}
			else
			{
				environment.environmentPath = Environment.GetEnvironmentVariable("home") + "/70/";
			}
			environment.commonLibraryPath = environment.addBs(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)) + "70/library/";
			environment.commonApplicationsPath = environment.addBs(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)) + "70/applications/";
			environment.libraryPath = environment.environmentPath + "library/";
			environment.appdataPath = environment.environmentPath + "appdata/";
			environment.compilationPath = environment.environmentPath + "compilation/";
			environment.applicationsPath = environment.environmentPath + "applications/";
			environment.languagePath = environment.libraryPath + "lang/";
			environment.globalAssemblyPath = environment.environmentPath + "global/";
			environment.mkDir(environment.environmentPath);
			environment.mkDir(environment.libraryPath);
			environment.mkDir(environment.appdataPath);
			environment.mkDir(environment.compilationPath);
			environment.mkDir(environment.applicationsPath);
			environment.mkDir(environment.globalAssemblyPath);
			try
			{
				environment.mkDir(environment.commonLibraryPath);
			}
			catch (Exception)
			{
			}
			environment.loadAssembly(Assembly.GetExecutingAssembly(), true);
			environment.loadAssembly(Assembly.GetAssembly(typeof(Microsoft.CSharp.RuntimeBinder.Binder)), true);
			environment.loadAssemblyPartialName("System.Core");
			environment.directories.Add(environment.appdataPath);
			ResolveEventHandler value2 = delegate(object s, ResolveEventArgs e)
			{
				Assembly result;
				//if (e.Name == "System.Windows.Forms")
				//{
				//	result = typeof(Form).Assembly;
				//}
				//else
				if (e.Name == "System")
				{
					result = typeof(WebRequest).Assembly;
				}
				else
				{
					string text = new AssemblyName(e.Name).Name;
					if (text.ToLower().EndsWith(".resources"))
					{
						text = text.Substring(0, text.Length - ".resources".Length);
						result = Assembly.GetExecutingAssembly();
					}
					else
					{
						if (environment.unix)
						{
							Assembly assembly = null;
							try
							{
								assembly = Assembly.Load(text);
							}
							catch
							{
							}
							if (assembly != null)
							{
								result = assembly;
								return result;
							}
						}
						string text2 = text;
						string text3 = string.Format(environment.exPath + "{0}.dll", text2);
						if (!File.Exists(text3))
						{
							/*
							if (environment.application != null)
							{
								text3 = string.Format(environment.application.dir + "{0}.dll", text2);
							}*/
							if (true)
							{
								text3 = string.Format(environment.compilationPath + "{0}.dll", text2);
								if (!File.Exists(text3))
								{
									text3 = string.Format(environment.libraryPath + "{0}.dll", text2);
									if (!File.Exists(text3))
									{
										text3 = string.Format(environment.appdataPath + "{0}.dll", text2);
										if (!File.Exists(text3))
										{
											text3 = environment.locateInGlobalPath(text2);
											if (!File.Exists(text3))
											{
												bool flag = true;
												int num = 0;
												while (!File.Exists(text3) && flag)
												{
													if (environment.directories.Count == num)
													{
														flag = false;
													}
													else
													{
														text3 = string.Format(environment.addBs(environment.directories[num]) + "{0}.dll", text2);
														num++;
													}
												}
											}
										}
									}
								}
							}
						}
						result = Assembly.LoadFrom(text3);
					}
				}
				return result;
			};
			AppDomain.CurrentDomain.AssemblyResolve += value2;
		}

		public static void mkDir(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static void loadAssembly(string name)
		{
			Assembly a = Assembly.Load(name);
			environment.loadAssembly(a, true);
		}

		public static void loadAssemblyPartialName(string name)
		{
			Assembly a = Assembly.LoadWithPartialName(name);
			environment.loadAssembly(a, true);
		}

		public static TimeZoneInfo getTimeZoneInfo(string zone)
		{
			return TimeZoneInfo.FindSystemTimeZoneById(zone);
		}

		public static void loadAssemblyFromFile(string file)
		{
			try
			{
				Assembly a = Assembly.LoadFile(file);
				environment.loadAssembly(a, true);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al cargar el archivo '" + file + "'. Detalle del error: " + ex.Message);
			}
		}

		public static void loadAssembly(Assembly a, bool include = true)
		{
			if (environment.assemblies.IndexOf(a) < 0)
			{
				environment.assemblies.Add(a);
			}
		}

		public static string getLastPath()
		{
			return environment._lastPath;
		}

		public static string addBs(string path)
		{
			path = ((!path.EndsWith("/") && !path.EndsWith("\\")) ? (path + Path.AltDirectorySeparatorChar.ToString()) : path);
			return path;
		}


		/*
		public static application createApplicationFrom(string file)
		{
			Uri uri = new Uri(file);
			application result;
			if (uri.IsFile)
			{
				FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read);
				result = environment.createApplicationFrom(f, file, true, false);
			}
			else
			{
				WebRequest webRequest = WebRequest.Create(file);
				webRequest.Timeout = 10000;
				Stream responseStream = webRequest.GetResponse().GetResponseStream();
				result = environment.createApplicationFrom(responseStream, file, true, false);
			}
			return result;
		}

		public static library createLibraryApplicationFrom(string file)
		{
			Uri uri = new Uri(file);
			library result;
			if (uri.IsFile)
			{
				FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read);
				application application = environment.createApplicationFrom(f, file, false, false);
				application.type = librarytype.library;
				result = application;
			}
			else
			{
				WebRequest webRequest = WebRequest.Create(file);
				webRequest.Timeout = 10000;
				Stream responseStream = webRequest.GetResponse().GetResponseStream();
				application application2 = environment.createApplicationFrom(responseStream, file, false, false);
				application2.type = librarytype.library;
				result = application2;
			}
			return result;
		}

		public static application createApplicationFrom(Stream f, string file, bool isapp, bool withoutRun = false)
		{
			StreamReader streamReader = new StreamReader(f);
			string value = streamReader.ReadToEnd();
			streamReader.Close();
			f.Close();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)JsonConvert.DeserializeObject(value, typeof(Dictionary<string, object>));
			application application = new application();
			if (isapp && !withoutRun)
			{
				environment.application = application;
			}
			library library = null;
			languageEngine languageEngine = null;
			application.file = file;
			application.uri = new Uri(file);
			application.languagename = "c#";
			string[] array = new string[0];
			string[] array2 = new string[0];
			string[] array3 = new string[0];
			string[] array4 = new string[0];
			foreach (KeyValuePair<string, object> current in dictionary)
			{
				string key = current.Key;
				switch (key)
				{
				case "assemblygroup":
					application.assemblyGroup = (bool)current.Value;
					break;
				case "name":
					application.name = (string)current.Value;
					break;
				case "languageReference":
					{
						string str = (string)current.Value;
						string file2 = environment.languagePath + str + ".jlang";
						library library2 = environment.createLibraryApplicationFrom(file2);
						library = library2;
						library2.compile();
						break;
					}
				case "language":
					application.languagename = (string)current.Value;
					try
					{
						languageEngine = language.languages[application.languagename];
					}
					catch (Exception)
					{
					}
					if (languageEngine == null)
					{
						string file3 = environment.languagePath + application.languagename + ".jlang";
						library library3 = environment.createLibraryApplicationFrom(file3);
						library3.compile();
						Type type = library3.compiledLibrary.GetType("program");
						languageEngine = (languageEngine)type.GetMethod("create").Invoke(null, new object[0]);
					}
					break;
				case "mainfile":
					{
						string mainfile = (string)current.Value;
						application.mainfile = mainfile;
						break;
					}
				case "assemblydirectories":
					{
						JArray jArray = (JArray)current.Value;
						array4 = jArray.ToObject<string[]>();
						break;
					}
				case "classFiles":
					array = (string[])current.Value;
					break;
				case "references":
					{
						JObject jObject = (JObject)current.Value;
						if (jObject["file"] != null)
						{
							array3 = jObject["file"].ToObject<string[]>();
						}
						if (jObject["byName"] != null)
						{
							string[] array5 = jObject["byName"].ToObject<string[]>();
							for (int i = 0; i < array5.Length; i++)
							{
								application.addAssemblyReferenceByName(array5[i]);
							}
						}
						if (jObject["byPartialName"] != null)
						{
							string[] array6 = jObject["byPartialName"].ToObject<string[]>();
							for (int j = 0; j < array6.Length; j++)
							{
							}
						}
						if (jObject["libraries"] != null)
						{
							array2 = jObject["libraries"].ToObject<string[]>();
						}
						if (jObject["classFiles"] != null)
						{
							array = jObject["classFiles"].ToObject<string[]>();
						}
						break;
					}
				}
			}
			if (languageEngine == null)
			{
				languageEngine = language.languages[application.languagename];
			}
			application.language = languageEngine;
			for (int k = 0; k < array4.Length; k++)
			{
				string fullFile = application.getFullFile(array4[k]);
				array4[k] = fullFile;
			}
			environment.directories.AddRange(array4);
			if (library != null)
			{
				application.libraries.Add(library);
				foreach (Assembly current2 in library.assemblies)
				{
					application.addAssembly(current2);
				}
				application.addAssembly(library.compiledLibrary);
			}
			for (int l = 0; l < array2.Length; l++)
			{
				application.addLibraryReference(array2[l]);
			}
			for (int m = 0; m < array3.Length; m++)
			{
				application.addAssemblyReferenceByFile(array3[m]);
			}
			for (int n = 0; n < array.Length; n++)
			{
				application.addClassReference(array[n]);
			}
			return application;
		}*/
	}

}
