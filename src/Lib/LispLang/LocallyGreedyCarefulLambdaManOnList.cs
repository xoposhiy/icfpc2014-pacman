using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.LispLang
{
	[TestFixture]
	class LocallyGreedyCarefulLambdaManOnList :Lisp
	{
		[Test]
		public void Test()
		{
			var macro = Compile(
				Def("LMStep", ArgNames("lmState", "world"), 
					Cons(0, 0)

				),
				
				Def("map", ArgNames("world"), Car("world")),
				Def("lmState", ArgNames("world"), Car(Cdr("world"))) ,
				Def("ghStates", ArgNames("world"), Car(Cdr(Cdr("world")))),
				Def("fruit", ArgNames("world"), Car(Cdr(Cdr(Cdr("world"))))),

				Def("lmLoc", ArgNames("world"), Car(Cdr(Call("lmState", "world")))),
				Def("lmDir", ArgNames("world"), Car(Cdr(Cdr(Call("lmState", "world"))))),

				Def("ghState", ArgNames("world", "ghInd"), 
					Call("get", 
						Call("ghStates", "world"), 
						"ghInd")),

				Def("sum", ArgNames("p1", "p2"), 
					Cons(Add(Car("p1"), Car("p2")), Add(Cdr("p1"), Cdr("p2")))),
				
				Def("point", ArgNames("x", "y"), Cons("x", "y")),
				Def("pdirections", ArgNames(), List( Cons(0, -1), Cons(1, 0), Cons(0, 1), Cons(-1, 0))),
				Def("getCell", ArgNames("map", "x", "y"), Call("get", Call("get", "map", "y"), "x"))
			);
			Console.WriteLine(macro);

				

		}
	}
}
