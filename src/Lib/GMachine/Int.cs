using System;

namespace Lib.GMachine
{
	public class Int : GCmd
	{
		public Int([NotNull] GArg intArg)
			: base(GCmdType.Int)
		{
			I = intArg;
		}

		public GArg I { get; private set; }

		public override void Execute([NotNull] GMachineState state)
		{
			if (I.GetConstValue() > 7)
				throw new InvalidOperationException(string.Format("Invalid interrupt: {0}", I));
		}
	}
}