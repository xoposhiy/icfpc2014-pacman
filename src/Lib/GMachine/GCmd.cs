using Lib.Game;

namespace Lib.GMachine
{
	public abstract class GCmd
	{
		protected GCmd(GCmdType type)
		{
			Type = type;
		}

		public GCmdType Type { get; private set; }

		public abstract void Execute([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService);

		[NotNull]
		public string ToGhc()
		{
			return string.Format("{0} {1}", Type.ToString().ToUpper(), ArgsToGhc());
		}

		[NotNull]
		protected abstract string ArgsToGhc();

		public override string ToString()
		{
			return ToGhc();
		}
	}
}