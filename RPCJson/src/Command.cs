using System;
using System.Collections.Generic;

namespace RPCJson
{
	public class Command
	{
		public enum CommandType
		{
			GetProperty,
			SetProperty,
			InvokeMethod,
			GetField,
			SetField
		}

		public bool createthread = false;

		public int c;

		public int objectid;

		public int commandid;

		public string typename;

		public string name;

		public int nameindex;

		public string command;

		public Argument value;

		public List<Argument> arguments;

		public bool getproperty
		{
			get
			{
				return this.c == 0;
			}
		}

		public bool method
		{
			get
			{
				return this.c == 2;
			}
		}

		public bool setproperty
		{
			get
			{
				return this.c == 1;
			}
		}

		public bool getfield
		{
			get
			{
				return this.c == 3;
			}
		}

		public bool setfield
		{
			get
			{
				return this.c == 4;
			}
		}
	}
}
