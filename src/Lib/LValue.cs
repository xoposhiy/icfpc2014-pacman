﻿using System.Globalization;

namespace Lib
{
	public class LValue
	{
		public override string ToString()
		{
			if (Tag == LTag.Int) return Value.ToString(CultureInfo.InvariantCulture);
			if (Tag == LTag.Pair) return string.Format("({0}, {1})", Head, Tail);
			return "{" + Value + "}";
		}

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
}