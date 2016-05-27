using System;
using RPCJson;
using System.Text;
using jxshell.dotnet4;
using System.Reflection;

namespace ClrForAll
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			if (jxshell.environment.windows)
				Console.OutputEncoding = Encoding.UTF8;

			Manager manager = new Manager ();
			manager.init ();
			manager.add (Assembly.GetExecutingAssembly());

			CommandLine.onRequest += (Command cmd) => {
				Executer.execute(cmd);
			};	
			CommandLine.read ();

		}
	}
}
