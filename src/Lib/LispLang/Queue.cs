using System;
using Lib.LMachine;
using Lib.LMachine.Parsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class Queue : Lisp
	{
		public static SExpr Enqueue()
		{
			return Def("queue_enqueue", ArgNames("q", "value"),
				Cons(Cons("value", Car("q")), Cdr("q")));
		}

		public static SExpr Transfer()
		{
			return Def("queue_transfer", ArgNames("q"), 
				If(Atom(Car("q")),
					"q",
					Call("queue_transfer",
						Cons(
							Cdr(Car("q")),
							Cons(Car(Car("q")), Cdr("q"))
						)
					)
				)
			);
		}

		public static SExpr Peek()
		{
			return Def("queue_peek", ArgNames("q"),
				If(Atom(Cdr("q")),
					Call("queue_peek", Call("queue_transfer", "q")),
					Car(Cdr("q"))
				)
			);
			
		}

		public static SExpr Dequeue()
		{
			return Def("queue_dequeue", ArgNames("q"),
				If(Atom(Cdr("q")),
					Call("queue_dequeue", Call("queue_transfer", "q")),
					Cons(
						Car(Cdr("q")),
						Cons(
							Car("q"),
							Cdr(Cdr("q"))
						)
					)
				)
			);
		}

		public static SExpr IsEmpty()
		{
			return Def("queue_isempty", ArgNames("q"),
				And(Atom(Car("q")), Atom(Cdr("q")))
			);
		}

		[Test]
		public void Test_Enqueue_Empty()
		{
			var macro = Compile(
				Call("queue_enqueue", Call("sampleQueue"), 3),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(), List())),
				Enqueue());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("([3], 0)", m.State.DataStack.Pop().ToString());
		}

		[Test]
		public void Test_DequeueSimple()
		{
			var macro = Compile(
				Call("queue_dequeue", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(1, 2, 3), List(4, 5, 6))),
				Transfer(), Dequeue());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("(4, ([1, 2, 3], [5, 6]))", m.State.DataStack.Pop().ToString());
		}
		
		[Test]
		public void Test_DequeueEmptyTail()
		{
			var macro = Compile(
				Call("queue_dequeue", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(3, 2, 1), List())),
				Transfer(), Dequeue());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("[1, 0, 2, 3]", m.State.DataStack.Pop().ToString());
		}
		
		[Test]
		public void Test_IsEmpty_ForEmpty()
		{
			var macro = Compile(
				Call("queue_isempty", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(), List())),
				IsEmpty());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("1", m.State.DataStack.Pop().ToString());
		}
		
		[Test]
		public void Test_IsEmpty_ForNotEmpty_Head()
		{
			var macro = Compile(
				Call("queue_isempty", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(1), List())),
				IsEmpty());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("0", m.State.DataStack.Pop().ToString());
		}

		[Test]
		public void Test_IsEmpty_ForNotEmpty_Tail()
		{
			var macro = Compile(
				Call("queue_isempty", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(), List(2))),
				IsEmpty());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("0", m.State.DataStack.Pop().ToString());
		}
	}
}