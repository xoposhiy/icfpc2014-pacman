using Lib.Game;

namespace Lib.GMachine
{
	public class GMachineFactory : IGMachineFactory
	{
		private readonly GCmd[][] programs;

		public GMachineFactory([NotNull] params GCmd[][] programs)
		{
			this.programs = programs;
		}

		public IGMachine Create(int ghostIndex, [NotNull] IGhostInterruptService interruptService)
		{
			var program = programs[ghostIndex%programs.Length];
			return new GMachine(program, interruptService);
		}
	}
}