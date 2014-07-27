using System.IO;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

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
							Call("InitQueueAndVisited", 
								"world", 
								World.LmLoc("world"),
								Call("InitVisited",
									Call("mapHeight", World.Map("world")),
									Call("mapWidth", World.Map("world"))))))),


				Def("RecursiveFindGoal_2", ArgNames("queueAndVisitedAndWorld"),
					Call("RecursiveFindGoal",
						Get(0, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld"),
						Get(2, "queueAndVisitedAndWorld"))),

				Def("RecursiveFindGoal", ArgNames("queue", "visited", "world"),
					If(Call("IsGoodCell", Car(Call("queue_peek", "queue")), "world"),
						Cdr(Call("queue_peek", "queue")),
						Call("RecursiveFindGoal_2", 
							Call("AddNeighbours",
								Call("queue_dequeue", "queue"), 
								Call("queue_peek", "queue"),
								"world",
								"visited")))),

				Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
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
					If(	Or(	Ceq(World.GetCell(Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection")),
								1),
							Call("pointIsWall", 
								Car("pointAndDirection"), 
								World.Map(Get(2, "queueAndVisitedAndWorld"))),
							Call("any_activeGhostAtPoint",
								World.GhStates(Get(2, "queueAndVisitedAndWorld")),
								Car("pointAndDirection"))),
						"queueAndVisitedAndWorld",
						List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
							Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
							Get(2, "queueAndVisitedAndWorld")))),

				Def("IsGoodCell", ArgNames("point", "world"),	//TODO: fright ghosts
					Or(Call("pointIsPill", "point", World.Map("world")),
						Call("pointIsPowerPill", "point", World.Map("world")),
						Call("pointIsFruit_Time", "point", "world"))),

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
					Ceq(World.GetCell("map", "point"),
						(int)(MapCell.Wall))),

				Def("pointIsPill", ArgNames("point", "map"),
					Ceq(World.GetCell("map", "point"),
						(int)(MapCell.Pill))),

				Def("pointIsPowerPill", ArgNames("point", "map"),
					Ceq(World.GetCell("map", "point"),
						(int)(MapCell.PowerPill))),

				Def("pointIsFruit", ArgNames("point", "world"),
					Ceq(World.GetCell(World.Map("world"), "point"),
						(int)(MapCell.Fruit))),

				Def("pointIsFruit_Time", ArgNames("point", "world"),
					And(Ceq(World.GetCell(World.Map("world"), "point"),
							(int)(MapCell.Fruit)),
						Cgt(World.FruitExpired("world"),
							126))),

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