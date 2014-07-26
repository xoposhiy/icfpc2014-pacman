namespace Lib.GMachine
{
	public class Xor : GCmd
	{
		public Xor([NotNull] GArg dst, [NotNull] GArg src)
			: base(GCmdType.Xor)
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
			state.WriteValue(Dst, (byte)(x ^ y));
		}
	}
}