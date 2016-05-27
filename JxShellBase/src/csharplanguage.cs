using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace jxshell
{
	public class csharplanguage : language
	{
		//private library l = null;

		private CSharpCodeProvider cp;

		private CompilerParameters p = new CompilerParameters();

		public Assembly compiled = null;

		private string sourceDefault = "";

		private static Dictionary<string, int> compilations = new Dictionary<string, int>(0);

		private static Dictionary<string, Assembly> compileds = new Dictionary<string, Assembly>(0);

		public override string languageName
		{
			get
			{
				
				return "c#";
			}
		}

		public csharplanguage()
		{
			//Dictionary<string, string> dictionary = new Dictionary<string, string>();
			this.cp = new CSharpCodeProvider();
			this.p.GenerateInMemory = false;
			this.p.GenerateExecutable = false;
			FileStream fileStream = new FileStream(Path.GetDirectoryName(environment.executableFile) + "/jxshell.default.cs", FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			this.sourceDefault = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
		}

		/*
		public override void setContextLibrary(library lo)
		{
			this.l = lo;
		}*/

		public override Assembly getCompiledAssembly()
		{
			return this.compiled;
		}

		public override void runScript(string script)
		{
			this.p.GenerateInMemory = false;
			this.compileString(script, environment.getCompilationFile());
			Type type = this.compiled.GetType("program");
			MethodInfo method = type.GetMethod("main", new Type[0]);
			method.Invoke(null, new object[0]);
		}

		public void runScript(string script, bool inMemory)
		{
			this.p.GenerateInMemory = inMemory;
			this.compileString(script, (!inMemory) ? environment.getCompilationFile() : "");
			Type type = this.compiled.GetType("program");
			MethodInfo method = type.GetMethod("main", new Type[0]);
			method.Invoke(null, new object[0]);
		}

		public override void loadClass(string file)
		{
			int num = -1;
			try
			{
				num = csharplanguage.compilations[file];
			}
			catch (Exception)
			{
			}
			if (num == 0)
			{
				DateTime now = DateTime.Now;
				while ((DateTime.Now - now).TotalMilliseconds < 4000.0 && num == 0)
				{
					num = csharplanguage.compilations[file];
				}
				if (num == 0)
				{
					throw new TimeoutException("No se pudo compilar correctamente el archivo " + file + ". Se agoto el tiempo de espera para la sincronizacion de compilacion entre diferentes hilos.");
				}
			}
			if (num == 1)
			{
				Assembly assembly = csharplanguage.compileds[file];
				this.compiled = assembly;
			}
			else
			{
				csharplanguage.compilations[file] = 0;
				bool flag = true;
				string id = environment.uniqueId();
				string fileName = Path.GetFileName(file);
				string text = Path.GetDirectoryName(file) + "/__jxshell__cache";
				string path = text + "/" + fileName + ".cache";
				string text2 = "";
				environment.mkDir(text);
				if (File.Exists(path))
				{
					DateTime lastWriteTime = File.GetLastWriteTime(path);
					DateTime lastWriteTime2 = File.GetLastWriteTime(file);
					if (lastWriteTime2 <= lastWriteTime)
					{
						FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
						StreamReader streamReader = new StreamReader(fileStream);
						text2 = streamReader.ReadToEnd();
						streamReader.Close();
						fileStream.Close();
						if (text2 != "")
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					try
					{
						Assembly value = Assembly.LoadFile(text2);
						this.compiled = value;
						csharplanguage.compilations[file] = 1;
						csharplanguage.compileds[file] = value;
					}
					catch (Exception)
					{
						flag = true;
					}
				}
				if (flag)
				{
					text2 = environment.getCompilationFile(id);
					FileStream fileStream2 = new FileStream(file, FileMode.Open, FileAccess.Read);
					StreamReader streamReader2 = new StreamReader(fileStream2);
					string script = streamReader2.ReadToEnd();
					streamReader2.Close();
					fileStream2.Close();
					try
					{
						this.compileString(script, text2);
					}
					catch (Exception ex)
					{
						csharplanguage.compilations[file] = -1;
						throw new Exception("No se pudo realizar la compilacion de " + file + ". " + ex.ToString(), ex);
					}
					csharplanguage.compilations[file] = 1;
					csharplanguage.compileds[file] = this.compiled;
					fileStream2 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
					StreamWriter streamWriter = new StreamWriter(fileStream2);
					fileStream2.SetLength(0L);
					streamWriter.Write(text2);
					streamWriter.Close();
					fileStream2.Close();
				}
				try
				{
					//Type type = this.compiled.GetType("program");
					//PropertyInfo property = type.GetProperty("contextLibrary");
					//property.SetValue(null, this.l, new object[0]);
				}
				catch (Exception)
				{
				}
				try
				{
					Type type2 = this.compiled.GetType("program");
					MethodInfo method = type2.GetMethod("mainLibrary");
					method.Invoke(null, new object[0]);
				}
				catch (Exception)
				{
				}
			}
		}

		public override void runFile(string file)
		{
			this.loadClass(file);
			Type type = this.compiled.GetType("program");
			MethodInfo method = type.GetMethod("main", new Type[0]);
			method.Invoke(null, new object[0]);
		}

		public CompilerParameters getCompilerParameters()
		{
			return this.p;
		}

		public void compileString(string script, string file)
		{
			int num = environment.assemblies.Count;
			/*
			if (this.l != null)
			{
				num += this.l.assemblies.Count;
			}*/
			//int num2 = 0;
			string[] array = new string[num];
			for (int i = 0; i < environment.assemblies.Count; i++)
			{
				if (environment.assemblies[i] == null)
				{
					throw new Exception("No se pudo cargar uno o más ensamblados.");
				}
				array[i] = environment.assemblies[i].Location;
				//num2 = i + 1;
			}

			/*
			if (this.l != null)
			{
				for (int j = 0; j < this.l.assemblies.Count; j++)
				{
					array[num2 + j] = this.l.assemblies[j].Location;
				}
			}*/


			this.p.TreatWarningsAsErrors = false;
			if (file != "")
			{
				this.p.OutputAssembly = file;
			}
			this.p.ReferencedAssemblies.AddRange(array);
			string text = Path.GetTempPath() + environment.uniqueId() + ".cs";
			FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(script);
			streamWriter.Close();
			fileStream.Close();
			string text2 = Path.GetTempPath() + environment.uniqueId() + ".cs";
			fileStream = new FileStream(text2, FileMode.OpenOrCreate, FileAccess.Write);
			streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(this.sourceDefault);
			streamWriter.Close();
			fileStream.Close();
			object obj = this.cp;
			lock (obj)
			{
				CompilerResults compilerResults = this.cp.CompileAssemblyFromFile(this.p, new string[]
					{
						text,
						text2
					});
				if (compilerResults.Errors.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!compilerResults.Errors[0].IsWarning)
					{
						stringBuilder.Append(compilerResults.Errors[0].ErrorText);
					}
					for (int k = 1; k < compilerResults.Errors.Count; k++)
					{
						if (!compilerResults.Errors[k].IsWarning)
						{
							stringBuilder.AppendLine();
							stringBuilder.Append(compilerResults.Errors[k].ErrorText);
						}
					}
					if (stringBuilder.Length > 0)
					{
						throw new Exception(stringBuilder.ToString());
					}
				}
				this.compiled = compilerResults.CompiledAssembly;
			}
		}
	}
}
