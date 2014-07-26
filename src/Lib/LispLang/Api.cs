using System.Linq;

namespace Lib.LispLang
{
	public class Api : Lisp
	{
		public static string CompileWithLibs(params SExpr[] main)
		{
			return Compile(loader.Concat(main).Concat(worldApi).Concat(listApi).Concat(queueApi).ToArray());
		}

		public static SExpr[] worldApi =
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
			Def("mapHeight", ArgNames("map"), Call("getListLength", "map")),
			Def("mapWidth", ArgNames("map"), Call("getListLength", Car("map"))),
			Def("initLMInternalState", ArgNames("map"), Cons(Cons(-1, -1), Cons(Call("mapHeight", "map"), Call("mapWidth", "map")))),
		};

		public static SExpr[] loader =
		{
			Cmd("LD 0 0"),
			Cmd("LD 0 1"),
			Cmd("LDF main"),
			Cmd("TAP 2"),
		};

		public static SExpr[] queueApi =
		{
			Queue.Enqueue(),
			Queue.Peek(),
			Queue.Dequeue(),
			Queue.Transfer(),
			Queue.IsEmpty()
		};

		public static SExpr[] listApi =
		{
			Def("get", ArgNames("list", "index"),
				If("index",
					Call("get", Cdr("list"), Sub("index", 1)),
					Car("list"))),
			Def("getListLength", ArgNames("aList"), If("aList", 0, Add(1, Call("getListLength", Cdr("aList"))))),
		};
	}
}