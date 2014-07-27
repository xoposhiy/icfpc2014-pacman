using System;

namespace Lib.GMachine
{
	public class GArg : IEquatable<GArg>
	{
		public bool Equals(GArg other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Type == other.Type && Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((GArg)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Type * 397) ^ Value.GetHashCode();
			}
		}

		[NotNull]
		public static GArg Const(byte value)
		{
			return new GArg(GArgType.Const, value);
		}

		[NotNull]
		public static GArg Data(byte value)
		{
			return new GArg(GArgType.Data, value);
		}

		[NotNull]
		public static GArg Reg(byte value)
		{
			if (value > 8) //8 - pc
				throw new InvalidOperationException(value.ToString());
			return new GArg(GArgType.Reg, value);
		}

		[NotNull]
		public static GArg IndirectReg(byte value)
		{
			if (value > 7)
				throw new InvalidOperationException("TODO");
			return new GArg(GArgType.IndirectReg, value);
		}

		private GArg(GArgType type, byte value)
		{
			Type = type;
			Value = value;
		}

		public GArgType Type { get; set; }

		public byte Value { get; set; }

		public override string ToString()
		{
			switch (Type)
			{
				case GArgType.Const:
					return Value.ToString();
				case GArgType.Data:
					return "[" + Value + "]";
				case GArgType.Reg:
					return regChars[Value];
				case GArgType.IndirectReg:
					return "[" + regChars[Value] + "]";
				default:
					throw new InvalidOperationException("TODO");
			}
		}

		private static readonly string[] regChars =
		{
			"A",
			"B",
			"C",
			"D",
			"E",
			"F",
			"G",
			"H"
		};
	}
}