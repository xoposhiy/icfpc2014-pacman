using Lib.Game;
using Lib.LispLang;
using NUnit.Framework;

namespace Lib.AI
{
	public class GreedyLambdaMen_Lisp : Api
	{
		[Test]
		public void Main()
		{
			var macro = Compile(
				Def("GreedyStep", ArgNames("stateAndWorld"), 
					Cons(Call("World", "stateAndWorld"),
						Call("RecursiveFindGoal_2", 
							Call("World", "stateAndWorld"),
							Call("InitQueueAndVisited", 
								"world", 
								Call("lmLoc", "world"),
								Call("InitVisited",
									Call("mapHeight", Call("map", Call("World", "stateAndWorld"))),
									Call("mapWidth", Call("map", Call("World", "stateAndWorld")))))))),

				Def("RecursiveFindGoal", ArgNames("world", "queue", "visited"),
					If(Call("IsGoodCell", Car(Call("queue_peek", "queue")), Call("map", "world")),
						Cdr(Call("queue_peek", "queue")),
						Call("RecursiveFindGoal_2", 
							"world",
							Call("AddNeighbours",
								"queue", 
								Call("queue_dequeue", "queue"),
								"world",
								"visited")))),

				Def("RecursiveFindGoal_2", ArgNames("world", "queueAndVisited"),
					Call("RecursiveFindGoal",
						"world",
						Car("queueAndVisited"),
						Cdr("queueAndVisited"))),

				Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
					Call("fold",
						List(Cons(0, 0), "visited", "world"),
						"AddPointsIntoQueue",
						Call("NeighboursWithDirection", "lmPoint"))),

				Def("AddNeighbours", ArgNames("queue", "pointAndDirection", "world", "visited"),
					Call("fold",
						List("queue", "visited", "world"),
						"AddPointsIntoQueue",
						Call("Neighbours", Car("pointAndDirection"), Cdr("pointAndDirection")))),

				Def("AddPointsIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),	//TODO:ghost neighbours
					If(	Or(	Ceq(Call("getCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection")),
								1),
							Call("pointIsWall", 
								Car("pointAndDirection"), 
								Call("map", Get(2, "queueAndVisitedAndWorld"))),
							Call("any_activeGhostAtPoint",
								Call("ghStates", Get(2, "queueAndVisitedAndWorld"),
								Car("pointAndDirection")))),
						"queueAndVisitedAndWorld",
						List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
							Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
							Get(2, "queueAndVisitedAndWorld")))),

				Def("IsGoodCell", ArgNames("point", "map"),		//TODO: fruit time, fright ghosts
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
						Cons("value", Call("Repeat", Call("Sub", "max", 1), "value")), 
						Cons("value", 0))),

				Def("World", ArgNames("stateAndWorld"), Cdr("stateAndWorld")),
				Def("GetAIState", ArgNames("stateAndWorld"), Car("stateAndWorld")),

				LambdaMenLogic
				);
		}
	}
}