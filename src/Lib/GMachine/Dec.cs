namespace Lib.GMachine
{
	public class Dec : GCmd
	{
		public Dec([NotNull] GArg dst)
			: base(GCmdType.Dec)
		{
			Dst = dst;
		}

		[NotNull]
		public GArg Dst { get; set; }

		public override void Execute([NotNull] GMachineState state)
		{
			var value = state.ReadValue(Dst);
			state.WriteValue(Dst, --value);
		}
	}
}