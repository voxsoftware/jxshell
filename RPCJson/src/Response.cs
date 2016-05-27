using System;

namespace RPCJson
{
	public class Response
	{
		public bool isthis;

		public object nativevalue;

		public int objectid;

		public bool isnull;

		public int commandid;

		public string typename;

		public Exception error;

		public Command command;

		public bool response = true;

		public Response()
		{
		}

		public Response(Command cmd, object result, object comp)
		{
			this.commandid = cmd.commandid;
			if (result is byte[])
			{
				cmd.command = "get";
			}
			if (cmd.command == "get")
			{
				this.nativevalue = result;
			}
			else if (result == null)
			{
				this.isnull = true;
			}
			else if (comp != null && comp == result)
			{
				this.objectid = cmd.objectid;
				this.isthis = true;
			}
			else if (result is DateTime)
			{
				this.nativevalue = result;
			}
			else if (result is string)
			{
				this.nativevalue = result;
			}
			else if (result.GetType().IsPrimitive)
			{
				this.nativevalue = result;
			}
			else
			{
				int num = Objects.save(result);
				this.objectid = num;
				this.typename = result.GetType().FullName;
			}
		}
	}
}
