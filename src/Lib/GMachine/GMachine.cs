using System;
using Lib.Game;
using Lib.Parsing;

namespace Lib.GMachine
{
	public class GMachine : IGMachine
	{
		private readonly IGhostInterruptService interruptService;
		private readonly Action<GMachine> runUntilStopStep;
		private GMachineState lastInitialState;

		public GMachine([NotNull] ParseResult<GCmd> parseResult, [NotNull] IGhostInterruptService interruptService, Action<GMachine> runUntilStopStep)
		{
			this.interruptService = interruptService;
			this.runUntilStopStep = runUntilStopStep ?? (machine => machine.RunToEnd());
			ParseResult = parseResult;
			Program = parseResult.Program;
			State = new GMachineState();
			GhostIndex = interruptService.GetThisGhostIndex();
		}

		public int GhostIndex { get; private set; }

		[NotNull]
		public ParseResult<GCmd> ParseResult { get; private set; }

		[NotNull]
		public GCmd[] Program { get; private set; }

		[NotNull]
		public GMachineState State { get; private set; }

		public void Run()
		{
			Init();
			runUntilStopStep(this);
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
				throw new GException(this, e);
			}
			if (State.Pc == pc)
				State.Pc++;
			State.CyclesMade++;
			return true;
		}
	}
}