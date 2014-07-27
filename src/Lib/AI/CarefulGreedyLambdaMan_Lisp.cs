using System.IO;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class CarefulGreedyLambdaMan_Lisp : GreedyLambdaMan_Lisp
	{
		public static readonly string code;

		static CarefulGreedyLambdaMan_Lisp()
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
				Repeat,

				AddFirstPointIntoQueue,
				IsBadFirstCell
				);
			File.WriteAllText(KnownPlace.GccSamples + "ExtremelyGreedyLM" + ".mgcc", code);

			var gccCode = LParser.Parse(code).Program.ToGcc();
			File.WriteAllText(KnownPlace.GccSamples + "ExtremelyGreedyLM" + ".gcc", gccCode);
		}

		public static SExpr InitQueueAndVisited =
			Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
				Call("fold",
					List(Cons(0, 0), "visited", "world"),
					Fun("AddFirstPointIntoQueue"),
					Call("NeighboursWithDirection", "lmPoint")));

		public static SExpr AddFirstPointIntoQueue =
			Def("AddFirstPointIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),
				If(Call("IsBadFirstCell",
						Car("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
						Get(2, "queueAndVisitedAndWorld"))));

		public static SExpr IsBadFirstCell =
			Def("IsBadFirstCell", ArgNames("point", "world", "visited"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "point", World.Map("world")),
					Call("any_activeGhostAtPoint", World.GhStates("world"), "point"),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, -1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(1, 0))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, 1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(-1, 0)))
					));

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("point", "world", "visited"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "point", World.Map("world"))));
	}
}