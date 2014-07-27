using Lib.Game;

namespace Lib.GMachine
{
	public class Dec : GCmd
	{
		public Dec([NotNull] GArg dst)
			: base(GCmdType.Dec)
		{
			ValidateDstArg(dst);
			Dst = dst;
		}

		[NotNull]
		public GArg Dst { get; set; }

		public override void Execute([NotNull] GMachineState state, IGhostInterruptService interruptService)
		{
			var value = state.ReadValue(Dst);
			state.WriteValue(Dst, --value);
		}

		protected override string ArgsToGhc()
		{
			return string.Format("{0}", Dst);
		}
	}
}