using System;
using Lib.LMachine;
using Lib.LMachine.Parsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class Queue : Lisp
	{
		public SExpr Enqueue()
		{
			return Def("enqueue", ArgNames("queue", "value"),
				Cons(Cons("value", Car("queue")), Cdr("queue")));
		}

		public SExpr Transfer()
		{
			return Def("transfer", ArgNames("queue"), 
				If(Atom(Car("queue")),
					"queue",
					Call("transfer",
						Cons(
							Cdr(Car("queue")),
							Cons(Car(Car("queue")), Cdr("queue"))
						)
					)
				)
			);
		}

		public SExpr Dequeue()
		{
			return Def("dequeue", ArgNames("queue"),
				If(Atom(Cdr("queue")),
					Call("dequeue", Call("transfer", "queue")),
					Cons(
						Car(Cdr("queue")),
						Cons(
							Car("queue"),
							Cdr(Cdr("queue"))
						)
					)
				)
			);
		}

		[Test]
		public void Test_Enqueue_Empty()
		{
			var macro = Compile(
				Call("enqueue", Call("sampleQueue"), 3),
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
				Call("dequeue", Call("sampleQueue")),
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
				Call("dequeue", Call("sampleQueue")),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(3, 2, 1), List())),
				Transfer(), Dequeue());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("(1, (0, [2, 3]))", m.State.DataStack.Pop().ToString());
		}
	}
}