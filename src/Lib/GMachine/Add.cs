namespace Lib.GMachine
{
	public class Add : GCmd
	{
		public Add([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Add)
		{
			Dst = dst;
			Src = src;
		}

		[NotNull]
		public GArg Dst { get; set; }

		[NotNull]
		public GArg Src { get; set; }

		public override void Execute([NotNull] GMachineState state, IInterruptService interruptService)
		{
			var x = state.ReadValue(Dst);
			var y = state.ReadValue(Src);
			state.WriteValue(Dst, (byte)(x + y));
		}
	}
}