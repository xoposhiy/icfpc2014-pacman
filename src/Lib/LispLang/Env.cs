namespace Lib.LispLang
{
	public class Env
	{
		public string[] names;

		public Env(string[] names)
		{
			this.names = names;
		}

		public Env()
			: this(new string[0])
		{
		}
	}
}