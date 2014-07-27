using System;
using Lib.Game;

namespace Lib.GMachine
{
	public class GMachine : IGMachine
	{
		private readonly IGhostInterruptService interruptService;
		private GMachineState lastInitialState;

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
			Init();
			RunToEnd();
		}

		public void Init()
		{
			State.Pc = 0;
			State.CyclesMade = 0;
			State.Hlt = false;
			lastInitialState = State.Clone();
		}

		public void ResetState()
		{
			State = lastInitialState.Clone();
		}

		public void RunToEnd()
		{
			while (Step())
			{
			}
		}

		public void StepBack()
		{
			var cyclesMade = State.CyclesMade;
			ResetState();
			for (var i = 0; i < cyclesMade - 1; i++)
				Step();
		}

		public bool Step()
		{
			if (State.Hlt || State.CyclesMade >= 1024)
				return false;
			var pc = State.Pc;
			try
			{
				var cmd = Program[pc];
				cmd.Execute(State, interruptService);
			}
			catch (Exception e)
			{
				throw new GException(e);
			}
			if (State.Pc == pc)
				State.Pc++;
			State.CyclesMade++;
			return true;
		}
	}
}