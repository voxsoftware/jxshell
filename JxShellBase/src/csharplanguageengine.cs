using System;

namespace jxshell
{
	public class csharplanguageEngine : languageEngine
	{
		public override language create()
		{
			return new csharplanguage();
		}
	}
}
