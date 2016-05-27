using jxshell;
using jxshell.dotnet4;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using RPCJson;

namespace ClrForAll
{
	public class Executer
	{
		public static void execute(Command cmd)
		{
			if (cmd.createthread)
			{
				Thread t = new Thread(new ParameterizedThreadStart(Executer.execute2));
				t.SetApartmentState(ApartmentState.STA);
				t.Start(cmd);
			}
			else
			{
				try
				{
					Executer._execute(cmd);
				}
				catch (Exception e)
				{
					CommandLine.write(new Response
						{
							error = e,
							command = cmd,
							commandid = cmd.commandid
						});
				}
			}
		}

		public static void execute2(object cmd)
		{
			Command c = cmd as Command;
			c.createthread = false;
			Executer.execute(c);
		}

		internal static void _execute(Command cmd)
		{
			try
			{
				object[] args;
				if (cmd.arguments != null)
				{
					args = new object[cmd.arguments.Count];
					for (int i = 0; i < cmd.arguments.Count; i++)
					{
						args[i] = cmd.arguments[i].getValue();
					}
				}
				else
				{
					args = new object[0];
				}
				bool emp = false;
				if (cmd.command == "assembly.add")
				{
					for (int i = 0; i < cmd.arguments.Count; i++)
					{
						Manager.lastManager.loadAssemblyPartialName(cmd.arguments[i].getValue().ToString());
					}
					emp = true;
				}
				else if (cmd.command == "assembly.addfile")
				{
					for (int i = 0; i < cmd.arguments.Count; i++)
					{
						Manager.lastManager.loadAssemblyFile(cmd.arguments[i].getValue().ToString());
					}
					emp = true;
				}
				else if (cmd.command == "remove")
				{
					if (cmd.objectid > 0)
					{
						Objects.remove(cmd.objectid);
					}
					emp = true;
				}
				else if (cmd.command == "compile")
				{
					string code = cmd.arguments[0].getValue().ToString();
					csharplanguage lang = (csharplanguage)language.languages["c#"].create();
					lang.compileString(code, "");
					Assembly a = lang.getCompiledAssembly();
					if (a != null)
					{
						Manager.lastManager.loadAssembly(a);
					}
					emp = true;
				}
				else if (cmd.command == "loadmembers")
				{
					//string code = cmd.typename;
					Type ty = Manager.lastManager.getTypeOrGenericType(cmd.typename);
					typeDescriptor t2 = typeDescriptor.loadFromType(ty, "", false);
					TypeLoader.TypeResponse tr = TypeLoader.loadMembers(t2);
					cmd.command = "get";
					Response r = new Response(cmd, tr, null);
					CommandLine.write(r);
				}
				else if (cmd.objectid > 0)
				{
					object obj = Objects.getbyid(cmd.objectid);
					if (obj == null)
					{
						throw new NullReferenceException("El objeto ha sido liberado de memoria o el id no es válido");
					}
					object result = null;
					if (cmd.getproperty)
					{
						result = TypeLoader.getProperty(cmd, ref args, obj);
					}
					else if (cmd.getfield)
					{
						result = TypeLoader.getField(cmd, obj);
					}
					else if (cmd.method)
					{
						result = TypeLoader.invokeMethod(cmd, ref args, obj);
					}
					else if (cmd.setproperty)
					{
						TypeLoader.setProperty(cmd, ref args, obj, cmd.value.getValue());
					}
					else if (cmd.setfield)
					{
						TypeLoader.setField(cmd, obj, cmd.value.getValue());
					}
					Response r = new Response(cmd, result, null);
					CommandLine.write(r);
				}
				else
				{
					object result = null;
					if (cmd.getproperty)
					{
						result = TypeLoader.getProperty(cmd, ref args, null);
					}
					else if (cmd.getfield)
					{
						result = TypeLoader.getField(cmd, null);
					}
					else if (cmd.method)
					{
						result = TypeLoader.invokeMethod(cmd, ref args, null);
					}
					else if (cmd.setproperty)
					{
						TypeLoader.setProperty(cmd, ref args, null, cmd.value.getValue());
					}
					else if (cmd.setfield)
					{
						TypeLoader.setField(cmd, null, cmd.value.getValue());
					}
					Response r = new Response(cmd, result, null);
					CommandLine.write(r);
				}
				if (emp)
				{
					CommandLine.write(new Response
						{
							isnull = true,
							commandid = cmd.commandid
						});
				}
			}
			catch (Exception e)
			{
				if (e.InnerException != null)
				{
					throw e.InnerException;
				}
				if (e is KeyNotFoundException)
				{
					throw new MissingMemberException("El método o propiedad no existe");
				}
				throw;
			}
		}
	}
}
