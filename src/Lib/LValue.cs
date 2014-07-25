using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Lib
{
	public class LValue
	{
		public override string ToString()
		{
			if (Tag == LTag.Int)
				return Value.ToString(CultureInfo.InvariantCulture);
			if (Tag == LTag.Pair)
				return string.Format("({0}, {1})", Head, Tail);
			return "{" + Value + "}";
		}

		[NotNull]
		public static LValue FromInt(int value)
		{
			return new LValue(LTag.Int, value: value);
		}

		[NotNull]
		public static LValue FromClosure(uint address, [CanBeNull] Frame frame)
		{
			return new LValue(LTag.Closure, address: address, frame: frame);
		}

		[NotNull]
		public static LValue FromPair([NotNull] LValue head, [NotNull] LValue tail)
		{
			return new LValue(LTag.Pair, head: head, tail: tail);
		}

		public LValue(LTag tag, int value = 0, uint address = 0, [CanBeNull] LValue head = null, [CanBeNull] LValue tail = null, [CanBeNull] Frame frame = null)
		{
			Tag = tag;
			Value = value;
			Address = address;
			Head = head;
			Tail = tail;
			Frame = frame;
		}

		public readonly LTag Tag;
		public readonly int Value;
		public readonly uint Address;
		public readonly LValue Head;
		public readonly LValue Tail;

		[CanBeNull]
		public readonly Frame Frame;

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
			while (text[pos] == ' ' || text[pos] == ',')
				pos++;
			var tail = Parse(text, ref pos);
			if (text[pos] != ')')
				throw new Exception("syntax error. ) expected but found: " + text[pos] + " at pos " + pos);
			pos++;
			return FromPair(head, tail);
		}

		private static LValue ParseInt(string text, ref int pos)
		{
			var last = pos;
			while (last < text.Length && char.IsDigit(text[last]))
				last++;
			var value = FromInt(int.Parse(text.Substring(pos, last - pos)));
			pos = last;
			return value;
		}

		public int GetValue()
		{
			if (Tag != LTag.Int)
				throw new InvalidOperationException("TODO");
			return Value;
		}
	}

	public enum LTag
	{
		Int,
		Pair,
		Closure
	}
}