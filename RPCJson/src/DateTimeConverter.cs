using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class DateTimeConverter : JsonConverter
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
		Dictionary<string, int> d = new Dictionary<string, int>();
		int unixTimestamp = (int)((DateTime)value).ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		d.Add("$$datetime", unixTimestamp);
		serializer.Serialize(writer, d);
	}

	public override bool CanConvert(Type t)
	{
		return typeof(DateTime).IsAssignableFrom(t);
	}
}
