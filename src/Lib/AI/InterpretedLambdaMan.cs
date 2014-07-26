using System;
using Lib.Game;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class InterpretedLambdaMan :LambdaMan
	{
		private readonly ParseResult<Instruction> parsedProg;

		public InterpretedLambdaMan(string prog)
		{
			parsedProg = LParser.Parse(prog);
		}

		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			var m = new LMachineInterpreter(parsedProg.Program);
			m.State.DataStack.Push(initialWorld.ToLValue());
			m.State.DataStack.Push(42);
			m.State.DataStack.Push(LValue.FromClosure(0, null));
			new Tap(2).Execute(m.State);
			m.RunUntilStop();
			var res = m.State.DataStack.Pop().GetPair();
			var aiState = res.Head;
			var step = res.Tail;
			return Tuple.Create<LValue, LMStep>(aiState, (ai, world) => MakeStep(ai, world, step));
		}

		private Tuple<LValue, Direction> MakeStep(LValue ai, World world, LValue step)
		{
			var m = new LMachineInterpreter(parsedProg.Program);
			m.State.DataStack.Push(ai);
			m.State.DataStack.Push(world.ToLValue());
			m.State.DataStack.Push(step);
			new Tap(2).Execute(m.State);
			m.RunUntilStop();
			var res = m.State.DataStack.Pop().GetPair();
			return Tuple.Create(res.Head, (Direction)res.Tail.GetValue());
		}
	}
}