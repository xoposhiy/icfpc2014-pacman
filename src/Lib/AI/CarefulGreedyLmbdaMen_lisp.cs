using System.IO;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class CarefulGreedyLambdaMen_Lisp : GreedyLambdaMen_Lisp
	{
		public static readonly string code;

		static CarefulGreedyLambdaMen_Lisp()
		{
			code = CompileWithLibs(
				main,
				GreedyStep,
				RecursiveFindGoal_2,
				RecursiveFindGoal,
				InitQueueAndVisited,
				AddNeighbours,
				AddPointIntoQueue,
				IsBadCell,
				IsGoodCell,
				NeighboursWithDirection,
				Neighbours,
				pointIsWall,
				pointIsPill,
				pointIsPowerPill,
				pointIsFruit,
				pointIsFruit_Time,
				InitVisited,
				Repeat
				);
			File.WriteAllText(KnownPlace.GccSamples + "ExtremelyGreedyLM" + ".mgcc", code);

			var gccCode = LParser.Parse(code).Program.ToGcc();
			File.WriteAllText(KnownPlace.GccSamples + "ExtremelyGreedyLM" + ".gcc", gccCode);
		}

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("point", "world", "visited"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "point", World.Map("world")),
					Call("any_activeGhostAtPoint", World.GhStates("world"), "point"),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, -1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(1, 0))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, 1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(-1, 0)))
					));

	}
}