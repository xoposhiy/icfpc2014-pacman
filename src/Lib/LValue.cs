using System;
using System.Collections.Generic;

namespace Lib
{
	public class LValue
	{
		public static LValue FromInt(int value)
		{
			return new LValue(LTag.Int, value);
		}
		public static LValue FromClosure(int address)
		{
			return new LValue(LTag.Closure, address);
		}
		public static LValue FromPair(LValue head, LValue tail)
		{
			return new LValue(LTag.Pair, 0, head, tail);
		}

		public LValue(LTag tag, int value, LValue head = null, LValue tail = null)
		{
			Tag = tag;
			Value = value;
			Head = head;
			Tail = tail;
		}

		public readonly LTag Tag;
		public readonly int Value;
		public readonly LValue Head;
		public readonly LValue Tail;

	}

	public enum LTag
	{
		Int,
		Pair,
		Closure
	}


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

		public static char[,] AsMap(this LValue map)
		{
			throw new NotImplementedException();
		}

		//... Other extension methods to interpret LValues

	}

}