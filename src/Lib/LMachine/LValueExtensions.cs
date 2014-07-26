using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.LMachine
{
	public static class LValueExtensions
	{
		public static bool IsInt(this LValue v)
		{
			return v.Tag == LTag.Int;
		}

		public static IEnumerable<LValue> AsEnumerable(this LValue v)
		{
			while (v.Tag == LTag.Pair)
			{
				var pair = v.GetPair();
				yield return pair.Head;
				v = pair.Tail;
			}
			if (v.GetValue() != 0)
				throw new InvalidOperationException("TODO");
		}

		public static List<T> AsList<T>(this LValue list, Func<LValue, T> parseElement)
		{
			return list.AsEnumerable().Select(parseElement).ToList();
		}

		public static LValue[] AsTuple(this LValue tuple, int size)
		{
			var res = new LValue[size];
			for (var i = 0; i < size - 1; i++)
			{
				var pair = tuple.GetPair();
				res[i] = pair.Head;
				tuple = pair.Tail;
			}
			res[size - 1] = tuple;
			return res;
		}
	}
}