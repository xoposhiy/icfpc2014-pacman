using System;
using System.Collections.Generic;
using System.Linq;
using Lib.LMachine.Intructions;

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
			Def("ghLoc", ArgNames("ghState"), Car(Cdr("ghState"))),
			Def("ghVitality", ArgNames("ghState"), Car("ghState")),
			Def("ghDir", ArgNames("ghState"), Car(Cdr(Cdr("ghState")))),

			Def("sum", ArgNames("p1", "p2"),
				Cons(Add(X("p1"), X("p2")), Add(Y("p1"), Y("p2")))),
			Def("point", ArgNames("x", "y"), Cons("x", "y")),
			Def("pEquals", ArgNames("p1", "p2"), And(Ceq(X("p1"), X("p2")), Ceq(Y("p1"), Y("p2")))),
			Def("pdirections", ArgNames(), List(Cons(0, -1), Cons(1, 0), Cons(0, 1), Cons(-1, 0))),
			Def("getCell", ArgNames("map", "point"), Call("get", Call("get", "map", Cdr("point")), Car("point"))),
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

		public static readonly SExpr any =
			Def("any", ArgNames("list", "f"),
				If(Atom("list"),
					0,
					If(CallFunRef("f", Car("list")), // CallFunRef дает понять, что f надо искать в Env
						1,
						Call("any", Cdr("list"), "f")
						)));

		public static SExpr max = 
			Def("max", ArgNames("list"),
				Call("_max_iter", "list", int.MinValue),
				Return(),
				Def("_max_iter", ArgNames("list", "maxValue"),
					If(Atom("list"),
						"maxValue",
						Call("_max_iter", Cdr("list"), Max(Car("list"), "maxValue"))
						)
					)
				);

		public static SExpr min = 
			Def("min", ArgNames("list"),
				Call("_min_iter", "list", int.MaxValue),
				Return(),
				Def("_min_iter", ArgNames("list", "minValue"),
					If(Atom("list"),
						"minValue",
						Call("_min_iter", Cdr("list"), Max(Car("list"), "minValue"))
						)
					)
				);

		public static SExpr[] listApi =
		{
			Def("get", ArgNames("list", "index"),
				If("index",
					Call("get", Cdr("list"), Sub("index", 1)),
					Car("list"))),
			Def("getListLength", ArgNames("aList"), If("aList", 0, Add(1, Call("getListLength", Cdr("aList"))))),
			any,
//			max,
			min
		};

		public SExpr[] Math()
		{
			return new SExpr[]
			{
				Def("max", ArgNames("aList"),
					If(Cdr("aList"),
						If(IsGreater(Car("aList"), Call("max", Args(Cdr("aList")))), Car("aList"), Call("max", Args(Cdr("aList")))),
						Car("aList"))),

			};
		}

		/// <summary>
		/// name - имя объявляемой функции
		/// funcName - имя функции от нескольких аргументов,
		/// первый из которых - очередной элемент массива, а остальные - дополнительные параметры переданные в additionalParams
		/// возвращает булево значение
		/// </summary>
		public static SExpr DefAny1(String funcName)
		{
			return Def("any_" + funcName, ArgNames("aList", "arg1"),
				If(Atom("aList"), 0, If(Call(funcName, Args(Car("aList"), "arg1")), 1, Call("any_" + funcName, Args(Cdr("aList"), "arg1")))));

		}

	}
}