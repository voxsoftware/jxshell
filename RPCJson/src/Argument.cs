using System;

namespace RPCJson
{
	public class Argument
	{
		public string type;
		public object value;
		public DateTime date;



		public object getValue()
		{
			object result;
			if (this.type == "date")
			{
				result = this.date;
			}
			else
			{
				if (this.type == "object")
				{
					try
					{
						result = Objects.getbyid(Convert.ToInt32(this.value));
						return result;
					}
					catch (Exception e)
					{
						throw new ArgumentException("La propieda value del argumento no es válida", e);
					}
				}
				result = this.value;
			}
			return result;
		}

	}
}
