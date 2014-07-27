using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Lib.AI;
using Lib.Game;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Lib.LispLang
{
	[TestFixture]
	public class LocallyGreedyCarefulLM_Lisp : Api
	{
		public static int depth = 4;


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
							LmSavedState.Create(Call("map", "world")),
							Fun("LMStep"))
						//Call("LMStep", LmSavedState.Create(Call("map", "world")), "world")
						),
					Def("LMStep", ArgNames("lmSavedState", "world"),
						Cons(LmSavedState.Create("lmSavedState", "world"),
							Call("calcDirection", "lmSavedState", "world"))
						),

					Def("calcDirection", ArgNames("lmSavedState", "world"),
						Call("argmax",
							Score.ScoreOfDirections(
								LmSavedState.LmLoc("lmSavedState"),
								World.LmLoc("world"),
								"world",
								"lmSavedState",
								CashedCalcs.Create("lmSavedState", "world"),
								depth))
						),

					LmSavedState.Definitions,
					Score.Definitions,
					GhostPredict.Definitions
					);
			}
		}

		public static string CodeDebug
		{
			get
			{
				var _code = codeDebug;
				File.WriteAllText(KnownPlace.GccSamples + "ksenia.z.mgcc", _code);
				var gcc = LParser.Parse(_code).Program.ToGcc();
				File.WriteAllText(KnownPlace.GccSamples + "ksenia.z.gcc", gcc);
				return _code;
			}
		}

		internal static string codeDebug
		{
			get
			{
				var main = new[] {
				
					Def("main", ArgNames("world"),
						Cons(
							LmSavedState.Create(Call("map", "world")),
							Fun("LMStep"))
						//Call("LMStep", LmSavedState.Create(Call("map", "world")), "world")
						),
					Def("LMStep", ArgNames("lmSavedState", "world"),
						Cons(LmSavedState.Create("lmSavedState", "world"),
							Call("calcDirection", "lmSavedState", "world"))
						),

					Def("calcDirection", ArgNames("lmSavedState", "world"),
						Call("argmax",
							Score.ScoreOfDirections(
								LmSavedState.LmLoc("lmSavedState"),
								World.LmLoc("world"),
								"world",
								"lmSavedState",
								CashedCalcs.Create("lmSavedState", "world"),
								depth))
						),
					
					LmSavedState.Definitions,
					Score.Definitions,
					GhostPredict.Definitions
					};

				var world = new Game.World(MapUtils.LoadFromKnownLocation("world-classic.txt"));
				var loader = new[]
				{
					Call("runstep", World.Create(world)),
					Return(),
					Def("runstep", ArgNames("world"),
						Call("LMStep",  Car(Call("main", "world")), "world")),

				};
				return Compile(loader
					.Concat(main)
					.Concat(worldApi)
					.Concat(listApi)
					.Concat(queueApi)
					.Concat(LambdaMenLogic)
					.Concat(World.Definitions)
					.ToArray());

			}
		}

		private class Score : Api
		{
			public static SExpr[] Definitions {get{
					return new SExpr[]
					{
						
						Def("scoreOfCell", ArgNames("cell", "world"),
						If(Ceq("cell", (int)MapCell.Pill), 1,
							If(Ceq("cell", (int)MapCell.Empty), -1,
								If(Ceq("cell", (int)MapCell.PowerPill), 5,
									If(And(Ceq("cell", (int)MapCell.Fruit), Cgt(World.FruitExpired("world"), 127)), 30,
										0))))),

						Def("scoreOfGhosts", ArgNames("point", "ghostLocs", "world"),
							Add(
								If(And(
									Cgt(137*depth, World.LmVitality("world")),
									GhostPredict.AnyGhostAroundPoint("point", "ghostLocs")),
									//Then
									-100,
									//Else
									0),
								If(Call("any_frightGhostAtPoint", Args(World.GhStates("world"), "point")), 10, 0))),

					Def("scoreOfPoint", ArgNames("prevLoc", "nextLoc", "visitedPoints", "world", "cashed", "depth"),
							Add(Add(Add(
								ScoreOfCell("nextLoc", "world"),
								ScoreOfGhosts("nextLoc", CashedCalcs.PredictedGhostLocs("cashed"), "world")),
								If(Call("pEquals", Args("prevLoc", "nextLoc")), Sub(Sub(0, "depth"), 1), 0)),
								Sub(0, World.GetCell("visitedPoints", "nextLoc")))
							),

						Def("scoreOfDirection", ArgNames("prevLoc", "currLoc", "nextLoc", "world", "lmstate", "cashed", "depth"),
							If(World.IsCorrect("nextLoc", Api.World.Map("world")),
								//Then
								Add(Mul(Add("depth", 1),
									ScoreOfPoint("prevLoc", "nextLoc", "lmstate", "world", "cashed", "depth")
									),
									If(
										Cgt("depth", 0),
										Call("max", ScoreOfDirections("currLoc", "nextLoc", "world", "lmstate", "cashed", Sub("depth", 1))),
										0)),
								// Else
								-1000))


					};}}

			

			public static SExpr ScoreOfCell(SExpr point, SExpr world)
			{
				var cell = World.GetCell(World.Map(world), point);
				return Call("scoreOfCell", cell, world);
			}

			public static SExpr ScoreOfGhosts(SExpr point, SExpr ghostLocs, SExpr world)
			{
				return Call("scoreOfGhosts", point, ghostLocs, world);
			}

			public static SExpr ScoreOfPoint(SExpr prevLoc, SExpr nextLoc, SExpr lmSavedState, SExpr world, SExpr cashed, SExpr depth)
			{
				return Call("scoreOfPoint", Args(prevLoc, nextLoc, LmSavedState.GetVisitedPoints(lmSavedState), world, cashed, depth));
			}

			public static SExpr ScoreOfDirection(SExpr prevLoc, SExpr currLoc, SExpr direction, SExpr world, SExpr lmstate, SExpr cashed, SExpr depth)
			{
				return Call("scoreOfDirection", prevLoc, currLoc, Call("sum", currLoc, direction), world, lmstate, cashed, depth);
			}

			public static SExpr ScoreOfDirections(SExpr prevLoc, SExpr currLoc, SExpr world, SExpr lmstate, SExpr cashed, SExpr depth)
			{
				return List(
					ScoreOfDirection(prevLoc, currLoc, Cons(0, -1), world, lmstate, cashed, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(1, 0), world, lmstate, cashed, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(0, 1), world, lmstate, cashed, depth),
					ScoreOfDirection(prevLoc, currLoc, Cons(-1, 0), world, lmstate, cashed, depth)
					);
			}

		}


		private class LmSavedState : Api
		{

			private static Dictionary<string, SExpr> generated = new Dictionary<string, SExpr>()
			{
				{ "init", GreedyLambdaMan_Lisp.InitVisited },
				{ "repeat", GreedyLambdaMan_Lisp.Repeat }

			};
			public static SExpr[] Definitions {get { return generated.Values.ToArray(); }}

			private static SExpr Create(SExpr currentLoc, SExpr sizeOfMap, SExpr visitedPoints)
			{
				return List(currentLoc, sizeOfMap, visitedPoints);
			}

			public static SExpr Create(SExpr map)
			{
				return LmSavedState.Create(Cons(-1, -1), Cons(Call("mapHeight", map), Call("mapWidth", map)), 
					Call("InitVisited", Call("mapHeight", World.Map("world")),Call("mapWidth", World.Map("world"))));
			}

			public static SExpr Create(SExpr lmSavedState, SExpr world)
			{
				var loc = World.LmLoc(world);
				var visitedPoints = LmSavedState.GetVisitedPoints(lmSavedState);
				return LmSavedState.Create(
					loc,
					LmSavedState.GetMapSize(lmSavedState),
					Call("setCell", visitedPoints, loc, Add(World.GetCell(visitedPoints, loc), 1)));
			}

			public static SExpr LmLoc(SExpr lmSavedState)
			{
				if (!generated.ContainsKey("LmLoc"))
					generated.Add("LmLoc", Def("lmSavedState.Loc", ArgNames("lmSavedState"), Get(0,"lmSavedState")));

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

		private class GhostPredict : Api
		{
			public static SExpr[] Definitions = new SExpr[]
				{
					Def("duplicateGhostLocs", ArgNames("ghostLocs"), 
						If(Atom("ghostLocs"), List(), 
							Cons(Car("ghostLocs"),
								Cons( Call("sum", Car("ghostLocs"), Cons(-1, 0)), 
									Cons( Call("sum", Car("ghostLocs"), Cons(0, 1)), 
										Cons( Call("sum", Car("ghostLocs"), Cons(1, 0)),
											Cons( Call("sum", Car("ghostLocs"), Cons(0, -1)), Call("duplicateGhostLocs", Cdr("ghostLocs"))))))))),

					Def("getCorrectPoints", ArgNames("pList", "map"), 
						If(Atom("pList"), 
							List(),
							If( World.IsCorrect(Car("pList"), "map"), 
								Cons(Car("pList"), Call("getCorrectPoints", Cdr("pList"), "map")),
								Call("getCorrectPoints", Cdr("pList"), "map")))),

					Def("selectGhostLocs", ArgNames("ghosts"), 
						If(Atom("ghosts"), 
							List(),
							Cons(World.GhLoc(Car("ghosts")), Call("selectGhostLocs", Cdr("ghosts"))))),

					DefAny1("pEquals"),

				};


			public static SExpr GetCorrectGhostLocAfter(SExpr world)
			{

				var ghLocs = Call("selectGhostLocs", World.GhStates("world"));
				var extGhLocs = Call("duplicateGhostLocs", ghLocs);
				var correct = Call("getCorrectPoints", extGhLocs, World.Map("world"));
				return correct;

			}

			public static SExpr AnyGhostAroundPoint(SExpr point, SExpr ghostLocs)
			{
				/*
				var ghLocs = Call("selectGhostLocs", World.GhStates("world"));
				var extGhLocs = Call("duplicateGhostLocs", ghLocs);
				var correct = Call("getCorrectPoints", extGhLocs, World.Map("world"));
				 * */
				return Call("any_pEquals", ghostLocs, point);
			}


		}



		private class CashedCalcs
		{
			public static SExpr Create(SExpr lmSavedState, SExpr world)
			{
				return Cons(GhostPredict.GetCorrectGhostLocAfter(world), 0);
			}

			public static SExpr PredictedGhostLocs(SExpr cached)
			{
				return Car(cached);
			}

		}

		[Test]
		public void Test()
		{
			Console.WriteLine(code);
			string result = LParser.Parse(code).Program.ToGcc();
			Console.WriteLine(result);

		}
	}
}