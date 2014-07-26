using System.Linq;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using NUnit.Framework;

namespace Lib.AI
{
	public class GreedyLambdaMen_Lisp : Lisp
	{
		[Test]
		public void Main()
		{
			var macro = Compile(
				Def("World", ArgNames("stateAndWorld"), Cdr("stateAndWorld")),
				Def("GetAIState", ArgNames("stateAndWorld"), Car("stateAndWorld")),

				Def("Repeat", ArgNames("max", "value"),
					If("max", 
						Cons("value", Call("Repeat", Call("Sub", "max", 1), "value")), 
						Cons("value", 0))),

				Def("InitVisited", 
					ArgNames("mapHeight", "mapWidth"), 
					Call("Repeat", "mapHeight", Call("Repeat", "mapWidth", 0))),

				Def("RecursiveFindGoal", 
					ArgNames("map", "queue", "visited"),
					If(Call("IsGoodCell", Call("queue_peek", "queue")),		//"IsGoodCell"
						Cdr(Call("queue_peek", "queue")),
						Call("RecursiveFindGoal_2", 
							"map",
							Call("AddNeighbours",							//"AddNeighbours"
								"queue", 
								Call("queue_dequeue", "queue"),
								"map",
								"visited")))),

				Def("RecursiveFindGoal_2",
					ArgNames("map", "queueAndVisited"),
					Call("RecursiveFindGoal",
						"map",
						Car("queueAndVisited"),
						Cdr("queueAndVisited"))),

				Def("GreedyStep",
					ArgNames("stateAndWorld"), 
					Cons(Call("World", "stateAndWorld"),
						Call("RecursiveFindGoal", 
							Call("map", Call("World", "stateAndWorld")), 
							Cons(0, 0),							// Empty queue
							Call("InitVisited",					// bool copy of maze
								Call("mapHeight", Call("map", Call("World", "stateAndWorld"))), 
								Call("mapWidth", Call("map", Call("World", "stateAndWorld")))))))
				);
		}
	}
}