using System;
using Lib.Game;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	[TestFixture]
	public class LocallyGreedyCarefulLambdaManOnList : Api
	{
		public static string code = CompileWithLibs(
				Def("main", ArgNames("world"),
					Cons(
						Call("initLMInternalState", Call("map", "world")),
						Fun("LMStep"))
						),

				Def("LMStep", ArgNames("lmSavedState", "world"),
				Cons(Cons(Call("lmLoc", "world"), Cdr("lmSavedState")), 
				Call("calcDirection", "lmSavedState", "world"))
					),


				Def("getMapSize", ArgNames("lmstate"), Cdr("lmstate")),
				Def("lmSavedState.Loc", ArgNames("lmSaveState"), Car("lmSaveState")),


				Def("isCorrect", ArgNames("point", "map", "mapsize"),
					Or(IsGreater(0, Car("point")), IsGreater(Sub(Cdr("mapsize"), 1), Car("point")),
						IsGreater(0, Cdr("point")), IsGreater(Sub(Car("mapsize"), 1), Cdr("point")),
						Ceq(Call("getCell", Args("map", "point")), (int)MapCell.Wall))),


				
				Def("scoreOfCell", ArgNames("cell"),
					If(Ceq("cell", (int)MapCell.Pill), 1,
						If(Ceq("cell", (int)MapCell.PowerPill), 5,
							If(Ceq("cell", (int)MapCell.Fruit), 3,
								0)))),

				Def("scoreOfGhosts", ArgNames("ghosts", "point"),
					Add(
						If(Call("any_activeGhostAtPoint", Args("ghosts", "point")), -100, 0),
						If(Call("any_frightGhostAtPoint", Args("ghosts", "point")), 5, 0))),


				Def("scoreOfPoint", ArgNames("prevLoc", "nextLoc", "world", "depth"),
					Add(
						Call("scoreOfCell", Args(Call("getCell", Args(Call("map", args: "world"), "nextLoc")))),
						Call("scoreOfGhosts", Args(Call("ghStates", "world"), "nextLoc")),
					If(Call("pEquals", Args("prevLoc", "nextLoc")), Sub(Sub(0, "depth"), 1), 0))),

				Def("scoreOfDirection", ArgNames("prevLoc", "currLoc", "direction", "world", "lmstate", "depth"),

					If(Call("isCorrect", Args(Call("sum", Args("currLoc", "direction")), Call("map", Args("world")), Call("getMapSize", Args("lmstate")))),
						Add(Mul(Add("depth", 1), Call("scoreOfPoint", Args("prevLoc", Call("sum", Args("currLoc", "direction")), "world", "depth"))),
							If(IsGreater("depth", 0), Call("max", Call("scoreOfDirections", Args("currLoc", Call("sum", Args("currLoc", "direction")), "world", "lmstate", Sub("depth", 1)))), 0)),
						-1000)),

				Def("scoreOfDirections", ArgNames("prevLoc", "currLoc", "world", "lmstate", "depth"),
					List(
						Call("scoreOfDirection", Args("prevLoc", "currLoc", Cons(-1, 0), "world", "lmstate", "depth")),
						Call("scoreOfDirection", Args("prevLoc", "currLoc", Cons(0, 1), "world", "lmstate", "depth")),
						Call("scoreOfDirection", Args("prevLoc", "currLoc", Cons(1, 0), "world", "lmstate", "depth")),
						Call("scoreOfDirection", Args("prevLoc", "currLoc", Cons(0, -1), "world", "lmstate", "depth")))),

			Def("calcDirection", ArgNames("lmSavedState", "world"),
				Call("argmax",
					Call("scoreOfDirections",
						Call("lmSavedState.Loc", "lmSavedState"),
						Call("lmLoc", "world"),
						"world",
						"lmSavedState", 
						2))),
						LambdaMenLogic

			);
		[Test]
		public void Test()
		{
			Console.WriteLine(code);
			var result = LParser.Parse(code).Program.ToGcc();
//			Console.WriteLine(result);
		}
	}
}
	