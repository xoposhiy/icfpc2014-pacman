using System;
using Lib.Game;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class InterpretedLambdaMan : LambdaMan
	{
		private readonly ParseResult<Instruction> programParseResult;
		private LMachineInterpreter interpreter;
		private readonly Action<InterpretedLambdaMan> runUntilStopMain;
		private readonly Action<InterpretedLambdaMan> runUntilStopStep;

		public InterpretedLambdaMan(string prog, Action<InterpretedLambdaMan> runUntilStopMain = null, Action<InterpretedLambdaMan> runUntilStopStep = null)
		{
			programParseResult = LParser.Parse(prog);
			this.runUntilStopMain = runUntilStopMain ?? (x => interpreter.RunUntilStop());
			this.runUntilStopStep = runUntilStopStep ?? (x => interpreter.RunUntilStop());
		}

		public LMachineInterpreter Interpreter
		{
			get { return interpreter; }
		}

		public ParseResult<Instruction> ProgramParseResult
		{
			get { return programParseResult; }
		}

		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			interpreter = new LMachineInterpreter(programParseResult.Program, LValue.FromClosure(0, null), initialWorld.ToLValue(), 42);
			runUntilStopMain(this);
			var res = interpreter.State.DataStack.Pop().GetPair();
			var aiState = res.Head;
			var step = res.Tail;
			return Tuple.Create<LValue, LMStep>(aiState, (ai, world) => MakeStep(ai, world, step));
		}

		private Tuple<LValue, Direction> MakeStep(LValue ai, World world, LValue step)
		{
			interpreter = new LMachineInterpreter(programParseResult.Program, step, ai, world.ToLValue());
			runUntilStopStep(this);
			var res = interpreter.State.DataStack.Pop().GetPair();
			return Tuple.Create(res.Head, (Direction)res.Tail.GetValue());
		}
	}
}