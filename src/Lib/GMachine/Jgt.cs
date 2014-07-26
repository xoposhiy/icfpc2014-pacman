using Lib.Game;

namespace Lib.GMachine
{
	public class Jgt : GCmd
	{
		public Jgt([NotNull] GArg tArg, [NotNull] GArg x, [NotNull] GArg y)
			: base(GCmdType.Jgt)
		{
			TArg = tArg;
			X = x;
			Y = y;
		}

		[NotNull]
		public GArg TArg { get; set; }

		[NotNull]
		public GArg X { get; set; }

		[NotNull]
		public GArg Y { get; set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var x = state.ReadValue(X);
			var y = state.ReadValue(Y);
			if (x > y)
				state.Pc = TArg.GetConstValue();
		}
	}
}