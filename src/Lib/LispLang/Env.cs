using System;

namespace Lib.LispLang
{
	public class Env
	{
		public readonly string[] names;
		public readonly Env parent;

		public Env(string[] names, Env parent = null)
		{
			this.names = names;
			this.parent = parent;
		}

		public Env()
			: this(new string[0])
		{
		}

		public Env MakeChild(string[] args)
		{
			return new Env(args, this);
		}

		public Tuple<int, int> GetVariableAddr(string name, int depth = 0)
		{
			var ind = Array.IndexOf(names, name);
			if (ind >= 0)
				return Tuple.Create(depth, ind);
			if (parent != null)
				return parent.GetVariableAddr(name, depth + 1);
			throw new Exception(name);
		}
	}
}