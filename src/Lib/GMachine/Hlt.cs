namespace Lib.GMachine
{
	public class Hlt : GCmd
	{
		public Hlt()
			: base(GCmdType.Hlt)
		{
		}

		public override void Execute([NotNull] GMachineState state)
		{
			state.Hlt = true;
		}
	}
}