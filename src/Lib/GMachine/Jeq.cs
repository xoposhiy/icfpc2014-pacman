using Lib.Game;

namespace Lib.GMachine
{
	public class Jeq : GCmd
	{
		public Jeq(byte targetAddress, [NotNull] GArg x, [NotNull] GArg y)
			: base(GCmdType.Jeq)
		{
			TargetAddress = targetAddress;
			X = x;
			Y = y;
		}

		public byte TargetAddress { get; private set; }

		[NotNull]
		public GArg X { get; private set; }

		[NotNull]
		public GArg Y { get; private set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var x = state.ReadValue(X);
			var y = state.ReadValue(Y);
			if (x == y)
				state.Pc = TargetAddress;
		}
	}
}