using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using System.Text.Json;

namespace RPCJson
{
	public class Parser
	{
		internal static JsonSerializer js;

		//internal static JsonParser jp;

		static Parser()
		{
			Parser.js = new JsonSerializer();
			Parser.js.Converters.Add(new ByteArrayConverter());
			Parser.js.Converters.Add(new DateTimeConverter());
		}

		public static Command parse_3(string cmd)
		{
			return JsonConvert.DeserializeObject<Command>(cmd);
		}

		public static Command parse_3(TextReader tr)
		{
			return Parser.js.Deserialize<Command>(new JsonTextReader(tr));
		}

		public static Command parse(string cmd)
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(cmd));
			Command c = new Command();
			int currentobject = 0;
			string currentname = "";
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.StartObject)
				{
					if (currentname == "value")
					{
						c.value = Parser.parseArgument(reader);
					}
					else if (currentobject == 0)
					{
						currentobject++;
					}
					else
					{
						Parser.parseDictionary(reader);
					}
				}
				else if (reader.TokenType == JsonToken.PropertyName)
				{
					currentname = reader.Value.ToString();
				}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					if (currentname == "arguments")
					{
						c.arguments = Parser.parseListArgument(reader);
					}
					else
					{
						Parser.parseArray(reader);
					}
				}
				else
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						break;
					}
					object val = reader.Value;
					if (currentname == "c")
					{
						c.c = Convert.ToInt32(val);
					}
					else if (currentname == "createthread")
					{
						c.createthread = Convert.ToBoolean(val);
					}
					else if (currentname == "command")
					{
						c.command = val.ToString();
					}
					else if (currentname == "commandid")
					{
						c.commandid = Convert.ToInt32(val);
					}
					else if (currentname == "objectid")
					{
						c.objectid = Convert.ToInt32(val);
					}
					else if (currentname == "nameindex")
					{
						c.nameindex = Convert.ToInt32(val);
					}
					else if (currentname == "typename")
					{
						c.typename = val.ToString();
					}
					else if (currentname == "name")
					{
						c.name = val.ToString();
					}
					else if (currentname == "value")
					{
						c.value = new Argument();
						c.value.value = val;
					}
				}
			}
			return c;
		}

		public static List<Argument> parseListArgument(JsonTextReader reader)
		{
			List<Argument> args = new List<Argument>();
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.StartArray)
				{
					//object[] result = 
					Parser.parseArray(reader);
					args.Add(null);
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					Argument result2 = Parser.parseArgument(reader);
					args.Add(result2);
				}
				else if (reader.TokenType == JsonToken.EndArray)
				{
					break;
				}
			}
			return args;
		}

		public static Argument parseArgument(JsonTextReader reader)
		{
			Argument A = new Argument();
			string currentname = "";
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					currentname = reader.Value.ToString();
					if (currentname == "date")
					{
						A.date = Parser.parseAsDateTime(reader);
					}
					else if (currentname == "byte[]")
					{
						string val = reader.ReadAsString();
						A.value = Convert.FromBase64String(val);
					}
				}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					object[] result = Parser.parseArray(reader);
					if (currentname == "value")
					{
						A.value = result;
					}
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					Dictionary<string, object> result2 = Parser.parseDictionary(reader);
					if (currentname == "value")
					{
						A.value = result2;
					}
				}
				else
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						break;
					}
					object val2 = reader.Value;
					if (currentname == "type")
					{
						A.type = val2.ToString();
					}
					else if (currentname == "value")
					{
						A.value = val2;
					}
				}
			}
			return A;
		}

		public static object[] parseArray(JsonTextReader reader)
		{
			List<object> i = new List<object>();
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.StartArray)
				{
					object[] result = Parser.parseArray(reader);
					i.Add(result);
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					Dictionary<string, object> result2 = Parser.parseDictionary(reader);
					i.Add(result2);
				}
				else
				{
					if (reader.TokenType == JsonToken.EndArray)
					{
						break;
					}
					i.Add(reader.Value);
				}
			}
			return i.ToArray();
		}

		public static object parseValue(JsonTextReader reader)
		{
			object result;
			if (!reader.Read())
			{
				result = null;
			}
			else
			{
				result = reader.Value;
			}
			return result;
		}

		public static DateTime parseAsDateTime(JsonTextReader reader)
		{
			return reader.ReadAsDateTime().Value;
		}

		public static Dictionary<string, object> parseDictionary(JsonTextReader reader)
		{
			Dictionary<string, object> i = new Dictionary<string, object>();
			string currentname = "";
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					currentname = reader.Value.ToString();
				}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					object[] result = Parser.parseArray(reader);
					i.Add(currentname, result);
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					Dictionary<string, object> result2 = Parser.parseDictionary(reader);
					i.Add(currentname, result2);
				}
				else
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						break;
					}
					i.Add(currentname, reader.Value);
				}
			}
			return i;
		}

		public static void parse_3_2(Command cmd, Argument arg, JsonTextReader jr)
		{
			string prop = "";
			while (jr.Read())
			{
				if (jr.TokenType == JsonToken.PropertyName)
				{
					prop = jr.Value.ToString();
				}
				else if (jr.TokenType == JsonToken.EndObject)
				{
					break;
				}
				if (prop == "type")
				{
					arg.type = jr.ReadAsString();
					prop = "";
				}
				else if (prop == "value")
				{
					jr.Read();
					arg.value = jr.Value;
					prop = "";
				}
				else if (prop == "date")
				{
					arg.date = jr.ReadAsDateTime().Value;
					prop = "";
				}
			}
		}

		public static void parse_3_3(Command cmd, JsonTextReader jr)
		{
			cmd.arguments = new List<Argument>();
			while (jr.Read())
			{
				if (jr.TokenType == JsonToken.StartObject)
				{
					Argument arg = new Argument();
					cmd.arguments.Add(arg);
					Parser.parse_3_2(cmd, arg, jr);
				}
				else if (jr.TokenType == JsonToken.EndArray)
				{
					break;
				}
			}
		}

		/*
		public static Command parse_2(string cmd)
		{
			return Parser.jp.Parse<Command>(cmd);
		}

		public static Command parse_2(TextReader tr)
		{
			return Parser.jp.Parse<Command>(tr);
		}
		*/
		public static Command parse2(Stream m)
		{
			m.Position = 0L;
			BinaryReader br = new BinaryReader(m);
			Command cmd = new Command();
			cmd.objectid = br.ReadInt32();
			int length = br.ReadInt32();
			byte[] o = br.ReadBytes(length);
			cmd.name = Encoding.UTF8.GetString(o);
			cmd.arguments = new List<Argument>();
			int count = br.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				Argument arg = new Argument();
				length = br.ReadInt32();
				o = br.ReadBytes(length);
				arg.type = Encoding.UTF8.GetString(o);
				if (arg.type == "string")
				{
					length = br.ReadInt32();
					o = br.ReadBytes(length);
					arg.value = Encoding.UTF8.GetString(o);
				}
				cmd.arguments.Add(arg);
			}
			return cmd;
		}
	}
}
