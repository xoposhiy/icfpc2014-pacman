namespace Lib.GMachine
{
	public abstract class GCmd
	{
		protected GCmd(GCmdType type)
		{
			Type = type;
		}

		public GCmdType Type { get; private set; }

		public abstract void Execute([NotNull] GMachineState state, [NotNull] IInterruptService interruptService);
	}
}