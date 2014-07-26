namespace Lib.GMachine
{
	public class Div : GCmd
	{
		public Div([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Div)
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
			state.WriteValue(Dst, (byte)(x / y));
		}
	}
}