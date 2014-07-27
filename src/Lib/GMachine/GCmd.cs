using System;
using System.Xml.Serialization.Configuration;
using Lib.Game;

namespace Lib.GMachine
{
	public abstract class GCmd
	{
		protected GCmd(GCmdType type)
		{
			Type = type;
		}

		public GCmdType Type { get; private set; }

		public abstract void Execute([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService);

		protected static void ValidateDstArg([NotNull] GArg arg)
		{
			if (arg.Type == GArgType.Const)
				throw new InvalidOperationException(string.Format("Dst arg is constant: {0}", arg));
			if (arg.Type == GArgType.Reg && arg.Value == 8)
				throw new InvalidOperationException(string.Format("Dst arg is PC: {0}", arg));
		}

		[NotNull]
		public string ToGhc()
		{
			return string.Format("{0} {1}", Type.ToString().ToUpper(), ArgsToGhc());
		}

		[NotNull]
		protected abstract string ArgsToGhc();

		public override string ToString()
		{
			return ToGhc();
		}
	}
}