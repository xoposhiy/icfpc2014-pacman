using Lib.Game;

namespace Lib.GMachine
{
	public class Inc : GCmd
	{
		public Inc([NotNull] GArg dst)
			: base(GCmdType.Inc)
		{
			Dst = dst;
		}

		[NotNull]
		public GArg Dst { get; set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var value = state.ReadValue(Dst);
			state.WriteValue(Dst, ++value);
		}
	}
}