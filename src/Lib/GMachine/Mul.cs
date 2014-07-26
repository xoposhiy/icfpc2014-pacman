using Lib.Game;

namespace Lib.GMachine
{
	public class Mul : GCmd
	{
		public Mul([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Mul)
		{
			Dst = dst;
			Src = src;
		}

		[NotNull]
		public GArg Dst { get; set; }

		[NotNull]
		public GArg Src { get; set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var x = state.ReadValue(Dst);
			var y = state.ReadValue(Src);
			state.WriteValue(Dst, (byte)(x * y));
		}
	}
}