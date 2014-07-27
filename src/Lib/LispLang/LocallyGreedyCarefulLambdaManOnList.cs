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
			Def("isCorrect", ArgNames("point", "map"),
				Not(
					Ceq(Call("getCell", "map", "point"), (int)MapCell.Wall))),
			Def("scoreOfCell", ArgNames("cell"),
				If(Ceq("cell", (int)MapCell.Pill), 1,
					If(Ceq("cell", (int)MapCell.PowerPill), 5,
						If(Ceq("cell", (int)MapCell.Fruit), 30,
							0)))),
			Def("scoreOfGhosts", ArgNames("ghosts", "point", "powerRemaining"),
				Add(
					If(And(
						IsGreater(0, "powerRemaining"),
						Call("any_ghostAtPoint", Args("ghosts", "point"))),
						//Then
						-100,
						//Else
						0),
					If(Call("any_frightGhostAtPoint", Args("ghosts", "point")), 5, 0))),
			Def("scoreOfPoint", ArgNames("prevLoc", "nextLoc", "world", "depth"),
				Add(Add(
					Call("scoreOfCell", Args(Call("getCell", Args(Call("map", args: "world"), "nextLoc")))),
					Call("scoreOfGhosts", Call("ghStates", "world"), "nextLoc", Call("powerRemains", "world"))),
					If(Call("pEquals", Args("prevLoc", "nextLoc")), Sub(Sub(0, "depth"), 1), 0)
					)),
			Def("scoreOfDirection", ArgNames("prevLoc", "currLoc", "direction", "world", "lmstate", "depth"),
				If(Call("isCorrect",
					Call("sum", "currLoc", "direction"),
					Call("map", "world")),
					//Then
					Add(Mul(Add("depth", 1),
						Call("scoreOfPoint", "prevLoc", Call("sum", Args("currLoc", "direction")), "world", "depth")
						),
						If(
							IsGreater("depth", 0),
							Call("max",
								Call("scoreOfDirections",
									Args("currLoc", Call("sum", Args("currLoc", "direction")), "world", "lmstate",
										Sub("depth", 1)))),
							0)),
					// Else
					-1000)),
			Def("scoreOfDirections", ArgNames("prevLoc", "currLoc", "world", "lmstate", "depth"),
				List(
					Call("scoreOfDirection", "prevLoc", "currLoc", Cons(0, -1), "world", "lmstate", "depth"),
					Call("scoreOfDirection", "prevLoc", "currLoc", Cons(1, 0), "world", "lmstate", "depth"),
					Call("scoreOfDirection", "prevLoc", "currLoc", Cons(0, 1), "world", "lmstate", "depth"),
					Call("scoreOfDirection", "prevLoc", "currLoc", Cons(-1, 0), "world", "lmstate", "depth")
					)),
			Def("calcDirection", ArgNames("lmSavedState", "world"),
				Call("argmax",
					DbgView(Call("scoreOfDirections",
						Call("lmSavedState.Loc", "lmSavedState"),
						Call("lmLoc", "world"),
						"world",
						"lmSavedState",
						2)))
				));

		[Test]
		public void Test()
		{
			//Console.WriteLine(code);
			string result = LParser.Parse(code).Program.ToGcc();
			Console.WriteLine(result);
		}
	}
}