using System;
using System.Collections.Generic;

namespace Lib
{
	public static class LValueExtensions
	{

		public static bool IsInt(this LValue v)
		{
			return v.Tag == LTag.Int;
		}

		public static IEnumerable<LValue> AsEnumerable(this LValue v)
		{
			while (!v.IsInt() || v.Value != 0)
			{
				if (v.Tag != LTag.Pair) throw new Exception(v.ToString());
				yield return v.Head;
				v = v.Tail;
			}
		}

		public static List<T> AsList<T>(this LValue list, Func<LValue, T> parseElement)
		{
			throw new NotImplementedException();
		}

		public static MapCell[,] AsMap(this LValue map)
		{
			throw new NotImplementedException();
		}

		//... Other extension methods to interpret LValues

	}
}