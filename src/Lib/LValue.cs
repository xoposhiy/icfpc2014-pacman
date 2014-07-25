using System;
using System.Globalization;

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

		public static LValue Parse(string text)
		{
			var pos = 0;
			return Parse(text, ref pos);
		}

		public static LValue Parse(string text, ref int pos)
		{
			return text[pos] == '(' 
				? ParsePair(text, ref pos) 
				: ParseInt(text, ref pos);
		}

		private static LValue ParsePair(string text, ref int pos)
		{
			pos++;
			var head = Parse(text, ref pos);
			while (text[pos] == ' ' || text[pos] == ',') pos++;
			var tail = Parse(text, ref pos);
			if (text[pos] != ')') throw new Exception("syntax error. ) expected but found: " + text[pos]+ " at pos " + pos);
			pos++;
			return FromPair(head, tail);
		}

		private static LValue ParseInt(string text, ref int pos)
		{
			var last = pos;
			while (last < text.Length && char.IsDigit(text[last])) last++;
			LValue value = FromInt(int.Parse(text.Substring(pos, last - pos)));
			pos = last;
			return value;
		}
	}

	public enum LTag
	{
		Int,
		Pair,
		Closure
	}
}