using Lib.Game;
using Lib.Parsing.LParsing;
using NUnit.Framework;
using System;

namespace Lib.LispLang
{
	[TestFixture]
	class LocallyGreedyCarefulLambdaManOnList : Api
	{
		[Test]
		public void Test()
		{
			var macro = Api.Compile(

				loader,
				Def("main", ArgNames("world"),
					Cons(
						Call("initLMInternalState", Call("map", "world")),
						Fun("LMStep"))
						),

				Def("LMStep", ArgNames("lmSavedState", "world"),

					Cons(Call("lmLoc", "world"), Call("calcDirection", Args("lmSavedState", "world")))

					),

				worldApi,
				listApi,
				Math(),

				Def("getMapSize", ArgNames("lmstate"), Cdr("lmstate")),
				Def("lmSavedState.Loc", ArgNames("lmSaveState"), Car("lmSaveState")),


				Def("isCorrect", ArgNames("point", "map", "mapsize"),
					Or(IsGreater(0, Car("point")), IsGreater(Sub(Cdr("mapsize"), 1), Car("point")),
						IsGreater(0, Cdr("point")), IsGreater(Sub(Car("mapsize"), 1), Cdr("point")),
						Ceq(Call("getCell", Args("map", "point")), (int)MapCell.Wall))),


						
				Def("activeGhostAtPoint", ArgNames("ghost", "point"),
					And(
						Ceq(Call("ghVitality", Args("ghost")), (int)GhostVitality.Standard),
						Call("pEquals", Args(Call("ghLoc", Args("ghost")), "point")))),
				Def("frightGhostAtPoint", ArgNames("ghost", "point"),
					And(
						Ceq(Call("ghVitality", Args("ghost")), (int)GhostVitality.Fright),
						Call("pEquals", Args(Call("ghLoc", Args("ghost")), "point")))),

				DefAny1("activeGhostAtPoint"),
				DefAny1("frightGhostAtPoint"),

				
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
						Call("scoreOfGhosts", Args(Call("ghStates", args: "world"), "nextLoc")),
						If(Call("pEquals", Args("prevLoc", "nextLoc")), Sub(Sub(0, "depth"), 1), 0)))
				,



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

				Def("calcDirection", ArgNames("lmSavedState", "world"), Call("argmax", Call("scoreOfDirections", Call("lmSavedState.Loc", "lmSavedState"), Call("lmLoc", "world"), "lmSavedState", 3)))

		);
 
		



		
		Console.WriteLine(macro);
		var result = LParser.Parse(macro);
		}
	}
}
