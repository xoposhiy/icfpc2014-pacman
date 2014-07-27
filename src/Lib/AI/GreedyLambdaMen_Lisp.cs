using System.IO;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;
using NUnit.Framework;

namespace Lib.AI
{
	public class GreedyLambdaMen_Lisp : Api
	{
		public static readonly string code;

		static GreedyLambdaMen_Lisp()
		{
			code = CompileWithLibs(
				Def("main", ArgNames("world"),
					Cons(0, Fun("GreedyStep"))),

				Def("GreedyStep", ArgNames("state", "world"),
					Cons("state",
						Call("RecursiveFindGoal_2",
							"world",
							Call("InitQueueAndVisited", 
								"world", 
								Call("lmLoc", "world"),
								Call("InitVisited",
									Call("mapHeight", Call("map", "world")),
									Call("mapWidth", Call("map", "world"))))))),


				Def("RecursiveFindGoal", ArgNames("world", "queue", "visited"),
					If(Call("IsGoodCell", Car(Call("queue_peek", "queue")), Call("map", "world")),
						Cdr(Call("queue_peek", "queue")),
						Call("RecursiveFindGoal_2", 
							"world",
							Call("AddNeighbours",
								Call("queue_dequeue", "queue"), 
								Call("queue_peek", "queue"),
								"world",
								"visited")))),

				Def("RecursiveFindGoal_2", ArgNames("world", "queueAndVisited"),
					Call("RecursiveFindGoal",
						"world",
						Car("queueAndVisited"),
						Cdr("queueAndVisited"))),

				Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
					Call("CombineQueueAndVisited", 
						Call("InitQueueAndVisitedFold", "world", "lmPoint", "visited"))),

				Def("CombineQueueAndVisited", ArgNames("queueAndVisitedAndWorld"),
					Cons(Get(0, "queueAndVisitedAndWorld"), Get(1, "queueAndVisitedAndWorld"))),

				Def("InitQueueAndVisitedFold", ArgNames("world", "lmPoint", "visited"),
					Call("fold",
						List(Cons(0, 0), "visited", "world"),
						Fun("AddPointIntoQueue"),
						Call("NeighboursWithDirection", "lmPoint"))),

				Def("AddNeighbours", ArgNames("queue", "pointAndDirection", "world", "visited"),
					Call("fold",
						List("queue", "visited", "world"),
						Fun("AddPointIntoQueue"),
						Call("Neighbours", Car("pointAndDirection"), Cdr("pointAndDirection")))),

				Def("AddPointIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),	//TODO:ghost neighbours
					If(	Or(	Ceq(Call("getCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection")),
								1),
							Call("pointIsWall", 
								Car("pointAndDirection"), 
								Call("map", Get(2, "queueAndVisitedAndWorld"))),
							Call("any_activeGhostAtPoint",
								Call("ghStates", Get(2, "queueAndVisitedAndWorld")),
								Car("pointAndDirection"))),
						"queueAndVisitedAndWorld",
						List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
							Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
							Get(2, "queueAndVisitedAndWorld")))),

				Def("IsGoodCell", ArgNames("point", "map"),	//TODO: fruit time, fright ghosts
					Or(Call("pointIsPill", "point", "map"),
						Call("pointIsPowerPill", "point", "map"),
						Call("pointIsFruit", "point", "map"))),

				Def("NeighboursWithDirection", ArgNames("point"),
					List(Cons(Call("sum", "point", Cons(0, -1)), 0),
						Cons(Call("sum", "point", Cons(1, 0)), 1),
						Cons(Call("sum", "point", Cons(0, 1)), 2),
						Cons(Call("sum", "point", Cons(-1, 0)), 3))),

				Def("Neighbours", ArgNames("point", "direction"),
					List(Cons(Call("sum", "point", Cons(0, -1)), "direction"),
						Cons(Call("sum", "point", Cons(1, 0)), "direction"),
						Cons(Call("sum", "point", Cons(0, 1)), "direction"),
						Cons(Call("sum", "point", Cons(-1, 0)), "direction"))),

				Def("pointIsWall", ArgNames("point", "map"), 
					Ceq(Call("getCell", "map", "point"),
						(int)(MapCell.Wall))),

				Def("pointIsPill", ArgNames("point", "map"), 
					Ceq(Call("getCell", "map", "point"),
						(int)(MapCell.Pill))),

				Def("pointIsPowerPill", ArgNames("point", "map"), 
					Ceq(Call("getCell", "map", "point"),
						(int)(MapCell.PowerPill))),

				Def("pointIsFruit", ArgNames("point", "map"), 
					Ceq(Call("getCell", "map", "point"),
						(int)(MapCell.Fruit))),

				Def("InitVisited", ArgNames("mapHeight", "mapWidth"), 
					Call("Repeat", "mapHeight", Call("Repeat", "mapWidth", 0))),

				Def("Repeat", ArgNames("max", "value"),
					If("max", 
						Cons("value", Call("Repeat", Sub("max", 1), "value")), 
						0))
				);

			File.WriteAllText(KnownPlace.GccSamples + "GreedyLM" + ".mgcc", code);

			var gccCode = LParser.Parse(code).Program.ToGcc();
			File.WriteAllText(KnownPlace.GccSamples + "GreedyLM" + ".gcc", gccCode);
		}
	}
}