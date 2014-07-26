using System;

namespace Lib.GMachine
{
	public class GArg
	{
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
			if (value > 7)
				throw new InvalidOperationException("TODO");
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

		public byte GetConstValue()
		{
			if (Type != GArgType.Const)
				throw new InvalidOperationException(string.Format("Invalid arg type: {0}", Type));
			return Value;
		}
	}
}