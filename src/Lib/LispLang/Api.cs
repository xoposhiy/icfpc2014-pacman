using System;
using System.Linq;
using Lib.Game;
using Lib.LMachine.Intructions;

namespace Lib.LispLang
{
	public class Api : Lisp
	{
		public static SExpr setCellXY =
			Def("setCellXY", ArgNames("map", "x", "y", "value"),
				Call("set", "map", "y", Call("set", Call("get", "map", "y"), "x", "value"))
			);

		public static SExpr setCell =
			Def("setCell", ArgNames("map", "point", "value"),
				Call("set", "map", Y("point"), Call("set", Call("get", "map", Y("point")), X("point"), "value")));

		public static SExpr[] worldApi =
		{
			Def("map", ArgNames("world"), Get(0, "world")),
			Def("lmState", ArgNames("world"), Get(1, "world")),
			Def("powerRemains", ArgNames("world"), Get(0, Get(1, "world"))),
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
			Def("getCell", ArgNames("map", "point"), Call("get", Call("get", "map", Y("point")), X("point"))),
			setCell,
			setCellXY,
			Def("mapHeight", ArgNames("map"), Call("getListLength", "map")),
			Def("mapWidth", ArgNames("map"), Call("getListLength", Car("map"))),
			Def("initLMInternalState", ArgNames("map"), 
				Cons(Cons(-1, -1), Cons(Call("mapHeight", "map"), Call("mapWidth", "map"))))
		};

		public static SExpr[] loader =
		{
			Cmd("LD 0 0"),
			Cmd("LD 0 1"),
			Cmd("LDF main"),
			Cmd("TAP 2")
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

		public static SExpr argmax =
			Def("argmax", ArgNames("list"),
				Call("_argmax_iter", "list", -1, int.MinValue, 0),
				Return(),
				Def("_argmax_iter", ArgNames("list", "maxIndex", "maxValue", "headIndex"),
					If(Atom("list"),
						"maxIndex",
						If(IsGreater(Car("list"), "maxValue"),
							Call("_argmax_iter", Cdr("list"), "headIndex", Car("list"), Add("headIndex", 1)),
							Call("_argmax_iter", Cdr("list"), "maxIndex", "maxValue", Add("headIndex", 1))
							)))
				);

		public static SExpr argmin =
			Def("argmin", ArgNames("list"),
				Call("_argmin_iter", "list", -1, int.MaxValue, 0),
				Return(),
				Def("_argmin_iter", ArgNames("list", "minIndex", "minValue", "headIndex"),
					If(Atom("list"),
						"minIndex",
						If(IsGreater("minValue", Car("list")),
							Call("_argmin_iter", Cdr("list"), "headIndex", Car("list"), Add("headIndex", 1)),
							Call("_argmin_iter", Cdr("list"), "minIndex", "minValue", Add("headIndex", 1))
							)))
				);

		public static SExpr min =
			Def("min", ArgNames("list"),
				Call("_min_iter", "list", int.MaxValue),
				Return(),
				Def("_min_iter", ArgNames("list", "minValue"),
					If(Atom("list"),
						"minValue",
						Call("_min_iter", Cdr("list"), Min(Car("list"), "minValue"))
						)
					)
				);

		public static SExpr get =
			Def("get", ArgNames("list", "index"),
			If("index",
				Call("get", Cdr("list"), Sub("index", 1)),
				Car("list")));

		public static SExpr set =
			Def("set", ArgNames("list", "index", "value"),
			If("index",
				Cons(Car("list"), Call("set", Cdr("list"), Sub("index", 1), "value")),
				Cons("value", Cdr("list"))));
		

		public static SExpr[] listApi =
		{
			get,
			set,
			Def("getListLength", ArgNames("aList"), If(Atom("aList"), 0, Add(1, Call("getListLength", Cdr("aList"))))),
			
			Def("fold", ArgNames("initElem", "func", "elemList"),
				If(Atom("elemList"),
					"initElem",
					Call("fold", 
						CallFunRef("func", "initElem", Get(0, "elemList")),
						"func",
						Cdr("elemList")))),
			any,
			max,
			min,
			argmax,
			argmin
		};


		public static string CompileWithLibs(params SExpr[] main)
		{
			return
				Compile(loader
					.Concat(main)
					.Concat(worldApi)
					.Concat(listApi)
					.Concat(queueApi)
					.Concat(LambdaMenLogic)
					.ToArray());
		}

		public static readonly SExpr[] LambdaMenLogic =
		{
			Def("activeGhostAtPoint", ArgNames("ghost", "point"),
				And(
					Ceq(Call("ghVitality", Args("ghost")), (int)GhostVitality.Standard),
					Call("pEquals", Args(Call("ghLoc", Args("ghost")), "point")))),

			Def("frightGhostAtPoint", ArgNames("ghost", "point"),
				And(
					Ceq(Call("ghVitality", Args("ghost")), (int)GhostVitality.Fright),
					Call("pEquals", Args(Call("ghLoc", Args("ghost")), "point")))),

			Def("ghostAtPoint", ArgNames("ghost", "point"),
				Call("pEquals", Args(Call("ghLoc", Args("ghost")), "point"))),

			DefAny1("activeGhostAtPoint"),
			DefAny1("frightGhostAtPoint"),
			DefAny1("ghostAtPoint"),
		};

		public SExpr[] Math()
		{
			return new[]
			{
				Def("max", ArgNames("aList"),
					If(Cdr("aList"),
						If(IsGreater(Car("aList"), Call("max", Args(Cdr("aList")))), Car("aList"), Call("max", Args(Cdr("aList")))),
						Car("aList")))
			};
		}

		
		/// НЕ УДАЛЯТЬ.
		/// name - имя объявляемой функции
		/// funcName - имя функции от нескольких аргументов,
		/// первый из которых - очередной элемент массива, а остальные - дополнительные параметры переданные в additionalParams
		/// возвращает булево значение
		/// </summary>
		public static SExpr DefAny1(String funcName)
		{
			return Def("any_" + funcName, ArgNames("aList", "arg1"),
				If(Atom("aList"),
					0,
					If(Call(funcName, Args(Car("aList"), "arg1")),
						1,
						Call("any_" + funcName, Args(Cdr("aList"), "arg1")))));
		}
	}
}