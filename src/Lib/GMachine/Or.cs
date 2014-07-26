namespace Lib.GMachine
{
	public class Or : GCmd
	{
		public Or([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Or)
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
			var x = state.ReadValue(Dst);
			var y = state.ReadValue(Src);
			state.WriteValue(Dst, (byte)(x | y));
		}
	}
}