namespace Lib.GMachine
{
	public class Sub : GCmd
	{
		public Sub([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Sub)
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
			state.WriteValue(Dst, (byte)(x - y));
		}
	}
}