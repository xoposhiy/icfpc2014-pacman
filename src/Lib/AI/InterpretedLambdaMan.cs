using System;
using System.CodeDom;
using Lib.Game;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.LMachine.Parsing;

namespace Lib.AI
{
	public class InterpretedLambdaMan :ILambdaMan
	{
		private readonly LParseResult parsedProg;
		private LValue step;

		public InterpretedLambdaMan(string prog)
		{
			parsedProg = LParser.Parse(prog);
		}

		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			var m = new LMachineInterpreter(parsedProg.Program);
			m.State.DataStack.Push(initialWorld.ToLValue());
			m.State.DataStack.Push(LValue.FromInt(42)); //undocumented arg
			m.RunUntilStop();
			var res = m.State.DataStack.Pop();
			var pair = res.GetPair();
			this.step = res.GetPair().Tail;
			return Tuple.Create<LValue, LMStep>(pair.Head, MakeStep);
		}

		private Tuple<LValue, Direction> MakeStep(LValue ai, World world)
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
