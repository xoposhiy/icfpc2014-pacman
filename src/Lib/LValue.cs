using System;
using JetBrains.Annotations;

namespace Lib
{
	public class LValue
	{
		[NotNull]
		public static LValue FromInt(int value)
		{
			return new LValue(LTag.Int, value: value);
		}

		[NotNull]
		public static LValue FromPair([NotNull] LValue head, [NotNull] LValue tail)
		{
			return new LValue(LTag.Pair, pair: new Pair(head, tail));
		}

		[NotNull]
		public static LValue FromClosure(uint address, [CanBeNull] Frame frame)
		{
			return new LValue(LTag.Closure, closure: new Closure(address, frame));
		}

		private LValue(LTag tag, int? value = null, [CanBeNull] Pair pair = null, [CanBeNull] Closure closure = null)
		{
			Tag = tag;
			Value = value;
			Pair = pair;
			Closure = closure;
		}

		public LTag Tag { get; private set; }

		public int? Value { get; private set; }

		[CanBeNull]
		public Pair Pair { get; private set; }

		[CanBeNull]
		public Closure Closure { get; private set; }

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
			if (!Value.HasValue)
				throw new InvalidOperationException("TODO");
			return Value.Value;
		}

		[NotNull]
		public Pair GetPair()
		{
			if (Tag != LTag.Pair)
				throw new InvalidOperationException("TODO");
			if (Pair == null)
				throw new InvalidOperationException("TODO");
			return Pair;
		}

		[NotNull]
		public Closure GetClosure()
		{
			if (Tag != LTag.Closure)
				throw new InvalidOperationException("TODO");
			if (Closure == null)
				throw new InvalidOperationException("TODO");
			return Closure;
		}

		public override string ToString()
		{
			switch (Tag)
			{
				case LTag.Int:
					return GetValue().ToString();
				case LTag.Pair:
					return GetPair().ToString();
				case LTag.Closure:
					return GetClosure().ToString();
				default:
					throw new InvalidOperationException(string.Format("Invalid Tag: {0}", Tag));
			}
		}
	}
}