namespace Lib.LispLang
{
	class Api : Lisp
	{
		public SExpr[] WorldApi()
		{
			return new[]
			{
				Def("map", ArgNames("world"), Get(0, "world")),
				Def("lmState", ArgNames("world"), Get(1, "world")),
				Def("ghStates", ArgNames("world"), Get(2, "world")),
				Def("fruit", ArgNames("world"), Get(3, "world")),

				Def("lmLoc", ArgNames("world"), Get(1, Call("lmState", "world"))),
				Def("lmDir", ArgNames("world"), Get(2, Call("lmState", "world"))),

				Def("ghState", ArgNames("world", "ghInd"),
					Call("get",
						Call("ghStates", "world"), "ghInd")),

				Def("sum", ArgNames("p1", "p2"),
					Cons(Add(X("p1"), X("p2")), Add(Y("p1"), Y("p2")))),

				Def("point", ArgNames("x", "y"), Cons("x", "y")),
				Def("pdirections", ArgNames(), List(Cons(0, -1), Cons(1, 0), Cons(0, 1), Cons(-1, 0))),

				Def("getCell", ArgNames("map", "x", "y"), Call("get", Call("get", "map", "y"), "x")),

				Get(),

				Queue.Enqueue(),
				Queue.Dequeue(),
				Queue.Transfer(),
				Queue.IsEmpty()

			};
		}

		public SExpr Get()
		{
			return 
				Def("get", ArgNames("list", "index"),
					If("index",
						Call("get", Cdr("list"), Sub("index", 1)),
						Car("list")));

		}
	}

	
}
