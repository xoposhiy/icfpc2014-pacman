using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.LispLang
{
	[TestFixture]
	class LocallyGreedyCarefulLambdaManOnList : Lisp
	{
		[Test]
		public void Test()
		{
			var macro = Compile(
				Def("LMStep", ArgNames("lmSavedState", "world"),
					Cons(0, 0)

				),

				Def("getmap", ArgNames("world"), Car("world")),
				Def("getlmState", ArgNames("world"), Car(Cdr("world"))),
				Def("getghStates", ArgNames("world"), Car(Cdr(Cdr("world")))),
				Def("getghState", ArgNames("world", "ghInd"), Call("get", Args(Call("getghStates", Args("world")), "ghInd"))),
				Def("getfruit", ArgNames("world"), Car(Cdr(Cdr(Cdr("world"))))),

				Def("lmLoc", ArgNames("world"), Car(Cdr(Call("lmState", "world")))),
				Def("lmDir", ArgNames("world"), Car(Cdr(Cdr(Call("lmState", "world"))))),

				Def("ghState", ArgNames("world", "ghInd"),
					Call("get",
						Call("ghStates", "world"),
						"ghInd")),

				Def("sum", ArgNames("p1", "p2"),
					Cons(Add(Car("p1"), Car("p2")), Add(Cdr("p1"), Cdr("p2")))),

				Def("point", ArgNames("x", "y"), Cons("x", "y")),
				Def("pdirections", ArgNames(), List(Cons(0, -1), Cons(1, 0), Cons(0, 1), Cons(-1, 0))),
				Def("getCell", ArgNames("map", "x", "y"), Call("get", Call("get", "map", "y"), "x")),

				Def("getMapSize", ArgNames("lmstate"), Cdr("lmstate")),


				Def("isCorrect", ArgNames("point", "map", "mapsize"), Or(IsGreater(0, Car("point")), IsGreater(Sub(Cdr("mapsize"), 1), Car("point")), IsGreater(0, Cdr("point")), IsGreater(Sub(Car("mapsize"), 1), Cdr("point"))))
			);
			Console.WriteLine(macro);



		}
	}
}
