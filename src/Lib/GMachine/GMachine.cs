using System;
using Lib.Game;

namespace Lib.GMachine
{
	public class GMachine : IGMachine
	{
		private readonly IGhostInterruptService interruptService;

		public GMachine([NotNull] GCmd[] program, [NotNull] IGhostInterruptService interruptService)
		{
			this.interruptService = interruptService;
			Program = program;
			State = new GMachineState();
		}

		[NotNull]
		public GCmd[] Program { get; private set; }

		[NotNull]
		public GMachineState State { get; private set; }

		public void Run()
		{
			var cycles = 0;
			State.Pc = 0;
			State.Hlt = false;
			while (!State.Hlt && cycles++ < 1024)
			{
				var pc = State.Pc;
				var cmd = Program[pc];
				try
				{
					cmd.Execute(State, interruptService);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("GHC instruction failed: {0}", e);
					break;
				}
				if (State.Pc == pc)
					State.Pc++;
			}
		}
	}
}