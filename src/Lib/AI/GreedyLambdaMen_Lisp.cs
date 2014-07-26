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
				Def("getListLength", ArgNames("aList"), If("aList", 0, Add(1, Call("getListLength", Cdr("aList"))))),
				Def("initLMInternalState", ArgNames("map"), Cons(Cons(-1, -1), Cons(Call("getListLength", "map"), Call("getListLength", Car("map")))))
				);
		}
	}
}