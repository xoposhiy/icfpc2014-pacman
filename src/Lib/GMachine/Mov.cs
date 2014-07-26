namespace Lib.GMachine
{
	public class Mov : GCmd
	{
		public Mov([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Mov)
		{
			Dst = dst;
			Src = src;
		}

		[NotNull]
		public GArg Dst { get; set; }

		[NotNull]
		public GArg Src { get; set; }

		public override void Execute([NotNull] GMachineState state)
		{
			var value = state.ReadValue(Src);
			state.WriteValue(Dst, value);
		}
	}
}