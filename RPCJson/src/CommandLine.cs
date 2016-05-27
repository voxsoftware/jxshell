using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RPCJson
{
	public class CommandLine
	{

		public delegate void Request (Command cmd);
		public static event Request onRequest;
		private static bool _read; 



		public static void stop(){
			_read = false;
		}

		public static void read()
		{
			_read = true;
			Stream input = Console.OpenStandardInput();
			byte[] buffer = new byte[8192];
			MemoryStream ms = new MemoryStream();
			while (true)
			{

				if(!_read){
					return; 
				}
				int length;
				if (input.CanRead && (length = input.Read(buffer, 0, buffer.Length)) > 0)
				{

					if(!_read){
						return; 
					}
					ms.Write(buffer, 0, length);
					ms.Position -= 1L;
					if (ms.ReadByte() == 10)
					{
						ms.Position = 0L;
						try
						{
							string s = Encoding.UTF8.GetString(ms.ToArray());
							ms.SetLength(0L);
							string[] strs = s.Split(new char[]
								{
									'\n'
								});
							string[] array = strs;
							for (int i = 0; i < array.Length; i++)
							{
								string s2 = array[i];
								if (s2.Trim().Length > 0)
								{
									try
									{
										Command cmd = Parser.parse(s2.ToString());
										if(onRequest!=null){
											onRequest(cmd);
										}
									}
									catch (ThreadAbortException e)
									{
										Console.WriteLine(e.ToString());
									}
									catch (Exception e2)
									{
										Console.WriteLine(e2.ToString());
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
				}
			}
		}

		public static void write(Response r)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter t = new StringWriter(sb);
			Parser.js.Serialize(t, r);
			Console.WriteLine(sb.ToString());
		}
	}
}
