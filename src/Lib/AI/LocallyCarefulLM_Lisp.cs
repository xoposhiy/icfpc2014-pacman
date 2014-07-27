﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class LocallyCarefulLM_Lisp : Api
	{
		private static int depth;

		public string Code
		{
			get
			{
				var _code = code;
				File.WriteAllText(KnownPlace.GccSamples + "ksenia.z.mgcc", _code);
				var gcc = LParser.Parse(_code).Program.ToGcc();
				File.WriteAllText(KnownPlace.GccSamples + "ksenia.z.gcc", gcc);
				return _code;
			}
		}

		internal string code
		{
			get
			{
				return CompileWithLibs(
					Def("main", ArgNames("world"),
						Cons(
							LmSavedState.Create(World.Map("world")),
							Fun("LMStep"))

						),
					Def("LMStep", ArgNames("lmSavedState", "world"),
						Cons(LmSavedState.Create("lmSavedState", "world"),
							Call("calcDirection", "lmSavedState", "world"))
						),

					Def("calcDirection", ArgNames("lmSavedState", "world"),
						Call("argmax",
							DbgView(Score.ScoreOfDirections(
								LmSavedState.LmLoc("lmSavedState"),
								Api.World.LmLoc("world"),
								"world",
								depth)))
						),
						LmSavedState.Definitions,
						Score.Definitions);
				//Api.World.Definitions);
				//LmSavedState.Definitions);
				//Score.Definitions);

			}
		}


		private class LmSavedState : Api
		{

			private static Dictionary<string, SExpr> generated = new Dictionary<string, SExpr>();
			public static SExpr[] Definitions { get { return generated.Values.ToArray(); } }

			private static SExpr Create(SExpr currentLoc, SExpr sizeOfMap, SExpr visitedPoints)
			{
				return List(currentLoc, sizeOfMap, visitedPoints);
			}

			public static SExpr Create(SExpr map)
			{
				return LmSavedState.Create(Cons(-1, -1), Cons(Call("mapHeight", map), Call("mapWidth", map)), List());
			}

			public static SExpr Create(SExpr lmSavedState, SExpr world)
			{
				return LmSavedState.Create(
					World.LmLoc(world),
					LmSavedState.GetMapSize(lmSavedState),
					Cons(World.LmLoc(world), LmSavedState.GetVisitedPoints(lmSavedState)));
			}

			public static SExpr LmLoc(SExpr lmSavedState)
			{
				if (!generated.ContainsKey("LmLoc"))
					generated.Add("LmLoc", Def("lmSavedState.Loc", ArgNames("lmSavedState"), Get(0, "lmSavedState")));

				return Call("lmSavedState.Loc", lmSavedState);
			}

			public static SExpr GetMapSize(SExpr lmSavedState)
			{
				if (!generated.ContainsKey("GetMapSize"))
					generated.Add("GetMapSize", Def("getMapSize", ArgNames("lmstate"), Get(1, "lmstate")));
				return Call("getMapSize", lmSavedState);
			}

			public static SExpr GetVisitedPoints(SExpr lmSavedState)
			{
				if (!generated.ContainsKey("GetVisitedPoints"))
					generated.Add("GetVisitedPoints", Def("lmSavedState.VisitedPoints", ArgNames("lmstate"), Get(2, "lmstate")));
				return Call("lmSavedState.VisitedPoints", lmSavedState);
			}


		}

		private class Score : Api
		{
			public static SExpr[] Definitions
			{
				get
				{
					return new SExpr[]
					{

						Def("scoreOfCell", ArgNames("cell", "world"),
							If(Ceq("cell", (int)MapCell.Pill), 1,
								If(Ceq("cell", (int)MapCell.PowerPill), 5,
										If(And(Ceq("cell", (int)MapCell.Fruit), Cgt(World.FruitExpired("world"), 137*depth)), 30,
											0)))),

						Def("scoreOfGhosts", ArgNames("ghosts", "point", "powerRemaining"),
							Add(
								If(And(
									Cgt(137*depth, "powerRemaining"),
									Call("any_ghostAtPoint", Args("ghosts", "point"))),
									//Then
									-100,
									//Else
									0),
								If(Call("any_frightGhostAtPoint", Args("ghosts", "point")), 10, 0))),

						Def("scoreOfPoint", ArgNames("prevLoc", "nextLoc", "world", "depth"),
							Add(Add(
								ScoreOfCell("nextLoc", "world"),
								ScoreOfGhosts("nextLoc", "world")),
								If(Call("pEquals", Args("prevLoc", "nextLoc")), Sub(Sub(0, "depth"), 1), 0))
							),

						Def("scoreOfDirection", ArgNames("prevLoc", "currLoc", "nextLoc", "world", "depth"),
							If(World.IsCorrect("nextLoc", Api.World.Map("world")),
								//Then
								Add(Mul(Add("depth", 1),
									ScoreOfPoint("prevLoc", "nextLoc", "world", "depth")
									),
									If(
										Cgt("depth", 0),
										Call("max", ScoreOfDirections("currLoc", "nextLoc", "world", Sub("depth", 1))),
										0)),
								// Else
								-1000)),


					};
				}
			}



			public static SExpr ScoreOfCell(SExpr point, SExpr world)
			{
				var cell = World.GetCell(World.Map(world), point);
				return Call("scoreOfCell", cell, world);
			}

			public static SExpr ScoreOfGhosts(SExpr point, SExpr world)
			{
				return Call("scoreOfGhosts", Args(Api.World.GhStates(world), point, Api.World.LmVitality(world)));
			}

			public static SExpr ScoreOfPoint(SExpr prevLoc, SExpr nextLoc, SExpr world, SExpr depth)
			{
				return Call("scoreOfPoint", Args(prevLoc, nextLoc, world, depth));
			}

			public static SExpr ScoreOfDirection(SExpr prevLoc, SExpr currLoc, SExpr direction, SExpr world, SExpr depth)
			{
				return Call("scoreOfDirection", prevLoc, currLoc, Call("sum", currLoc, direction), world, depth);
			}

			public static SExpr ScoreOfDirections(SExpr prevLoc, SExpr currLoc, SExpr world, SExpr depth)
			{
				return List(
					ScoreOfDirection(prevLoc, currLoc, Cons(0, -1), world, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(1, 0), world, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(0, 1), world, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(-1, 0), world, depth)
					);
			}


			private class LmSavedState : Api
			{

				private static Dictionary<string, SExpr> generated = new Dictionary<string, SExpr>();

				public static SExpr[] Definitions
				{
					get { return generated.Values.ToArray(); }
				}


				public static SExpr Create(SExpr map)
				{
					return Create(Cons(-1, -1), Cons(Call("mapHeight", map), Call("mapWidth", map)));
				}

				public static SExpr Create(SExpr lmSavedState, SExpr world)
				{
					return Cons(
						World.LmLoc(world),
						GetMapSize(lmSavedState)
						);
				}

				public static SExpr LmLoc(SExpr lmSavedState)
				{
					if (!generated.ContainsKey("LmLoc"))
						generated.Add("LmLoc", Def("lmSavedState.Loc", ArgNames("lmSavedState"), Car("lmSavedState")));
					return Call("lmSavedState.Loc", lmSavedState);
				}

				public static SExpr GetMapSize(SExpr lmSavedState)
				{
					if (!generated.ContainsKey("GetMapSize"))
						generated.Add("GetMapSize", Def("getMapSize", ArgNames("lmstate"), Cdr("lmstate")));
					return Call("getMapSize", lmSavedState);
				}

			}


			private class GhostPredict : Api
			{



			}
		}
	}
}