using System;
using Lib.Game;
using Lib.LMachine;
using Lib.LMachine.Parsing;

namespace Lib.AI
{
	public class InterpretedLambdaMan :ILambdaMan
	{
		private LParseResult parsedProg;

		public InterpretedLambdaMan(string prog)
		{
			parsedProg = LParser.Parse(prog);
		}

		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			throw new NotImplementedException();
		}
	}
}
