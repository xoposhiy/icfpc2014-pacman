using System;
using Lib.Game;

namespace Lib.GMachine
{
	public class Mov : GCmd
	{
		public Mov([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Mov)
		{
			if (dst.Type == GArgType.Const)
				throw new InvalidOperationException(string.Format("Dst arg is constant: {0}", dst));
			Dst = dst;
			Src = src;
		}

		[NotNull]
		public GArg Dst { get; set; }

		[NotNull]
		public GArg Src { get; set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var value = state.ReadValue(Src);
			state.WriteValue(Dst, value);
		}

		protected override string ArgsToGhc()
		{
			return string.Format("{0}, {1}", Dst, Src);
		}
	}
}