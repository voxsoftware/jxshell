using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class ByteArrayConverter : JsonConverter
{
	public override bool CanRead
	{
		get
		{
			return true;
		}
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		return null;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, new Dictionary<string, string>
			{
				{
					"$$byte[]",
					Convert.ToBase64String((byte[])value)
				}
			});
	}

	public override bool CanConvert(Type t)
	{
		return typeof(byte[]).IsAssignableFrom(t);
	}
}
