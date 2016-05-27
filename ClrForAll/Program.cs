using System;
using RPCJson;

namespace ClrForAll
{
	class MainClass
	{
		public static void Main (string[] args)
		{
				
			CommandLine.onRequest += (Command cmd) => {
				Executer.execute(cmd);
			};	
			CommandLine.read ();

		}
	}
}
