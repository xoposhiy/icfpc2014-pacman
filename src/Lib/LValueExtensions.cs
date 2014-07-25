using System;
using System.Collections.Generic;
using System.Linq;

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
			return list.AsEnumerable().Select(parseElement).ToList();
		}
		
		public static LValue[] AsTuple(this LValue tuple, int size)
		{
			var res = new LValue[size];
			for (int i = 0; i < size-1; i++)
			{
				res[i] = tuple.Head;
				tuple = tuple.Tail;
			}
			res[size-1] = tuple;
			return res;
		}

	}
}