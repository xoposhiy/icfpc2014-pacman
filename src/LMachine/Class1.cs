using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lib;

namespace LMachine
{
	public enum InstructionType
	{
		Ldc,
		Ld,
		Add,
		Sub,
		Div,
		Mul,
		Ceq,
		Cgt,
		Cgte,
		Atom,
		Cons,
		Car,
		Cdr,
		Sel,
		Join,
		Ldf,
		Ap,
		Rtn,
		Dum,
		Rap,
		Tap,
		Trap,
		Tsel,
		St,
	}

	public abstract class Instruction
	{
		protected Instruction(InstructionType type)
		{
			Type = type;
		}

		public InstructionType Type { get; private set; }

		public abstract void Execute([NotNull] LMachineState state);
	}

	public class Ldc : Instruction
	{
		public Ldc(int value)
			: base(InstructionType.Ldc)
		{
			Value = value;
		}

		public int Value { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			state.DataStack.Push(LValue.FromInt(Value));
			state.CurrentAddress++;
		}
	}

	public class Ld : Instruction
	{
		public Ld(uint frameIndex, uint valueIndex)
			: base(InstructionType.Ld)
		{
			FrameIndex = frameIndex;
			ValueIndex = valueIndex;
		}

		public uint FrameIndex { get; private set; }
		public uint ValueIndex { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var fp = state.CurrentFrame;
			for (var i = 0; i < FrameIndex; i++)
			{
				if (fp == null)
					throw new InvalidOperationException("TODO");
				fp = fp.Parent;
			}
			if (fp == null)
				throw new InvalidOperationException("TODO");

			if (fp.IsDum)
				throw new InvalidOperationException("TODO");

			var v = fp.GetValue(ValueIndex);
			state.DataStack.Push(v);

			state.CurrentAddress++;
		}
	}

	public class Add : Instruction
	{
		public Add()
			: base(InstructionType.Add)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = unchecked (x + y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Sub : Instruction
	{
		public Sub()
			: base(InstructionType.Sub)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = unchecked(x - y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Mul : Instruction
	{
		public Mul()
			: base(InstructionType.Mul)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = unchecked(x * y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Div : Instruction
	{
		public Div()
			: base(InstructionType.Div)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			if (y == 0)
				throw new InvalidOperationException("TODO");
			var z = unchecked(x / y); // todo !!! -3 / 2 = -2
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Ceq : Instruction
	{
		public Ceq()
			: base(InstructionType.Ceq)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = x == y ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Cgt : Instruction
	{
		public Cgt()
			: base(InstructionType.Cgt)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = x > y ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Cgte : Instruction
	{
		public Cgte()
			: base(InstructionType.Cgte)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = x >= y ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Atom : Instruction
	{
		public Atom()
			: base(InstructionType.Atom)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop();
			var z = x.Tag == LTag.Int ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}

	public class Cons : Instruction
	{
		public Cons()
			: base(InstructionType.Cons)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop();
			var x = state.DataStack.Pop();
			state.DataStack.Push(LValue.FromPair(x, y)); // todo !!! memmory heap managemnet / gc ?
			state.CurrentAddress++;
		}
	}

	public class Car : Instruction
	{
		public Car()
			: base(InstructionType.Car)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetPair();
			state.DataStack.Push(x.Head);
			state.CurrentAddress++;
		}
	}

	public class Cdr : Instruction
	{
		public Cdr()
			: base(InstructionType.Cdr)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetPair();
			state.DataStack.Push(x.Tail);
			state.CurrentAddress++;
		}
	}

	public class Sel : Instruction
	{
		public Sel()
			: base(InstructionType.Sel)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Join : Instruction
	{
		public Join()
			: base(InstructionType.Join)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Ldf : Instruction
	{
		public Ldf()
			: base(InstructionType.Ldf)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Ap : Instruction
	{
		public Ap()
			: base(InstructionType.Ap)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Rtn : Instruction
	{
		public Rtn()
			: base(InstructionType.Rtn)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Dum : Instruction
	{
		public Dum()
			: base(InstructionType.Dum)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Rap : Instruction
	{
		public Rap()
			: base(InstructionType.Rap)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Tsel : Instruction
	{
		public Tsel()
			: base(InstructionType.Tsel)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Tap : Instruction
	{
		public Tap()
			: base(InstructionType.Tap)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class Trap : Instruction
	{
		public Trap()
			: base(InstructionType.Trap)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public class St : Instruction
	{
		public St()
			: base(InstructionType.St)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{

		}
	}

	public enum CTag
	{
		Join,
		Ret,
		Frame,
	}

	public class ControlStackItem
	{
		public CTag Tag { get; private set; }
		public uint Address { get; private set; }

		[CanBeNull]
		public Frame Frame { get; private set; }
	}

	public class LStack<T> where T : class
	{
		private readonly Stack<T> stack;

		public LStack()
		{
			stack = new Stack<T>();
		}

		public void Push([NotNull] T value)
		{
			stack.Push(value);
		}

		[NotNull]
		public T Pop()
		{
			if (stack.Count == 0)
				throw new InvalidOperationException("TODO");
			return stack.Pop();
		}
	}

	public class LMachineState
	{
		public LMachineState()
		{
			DataStack = new LStack<LValue>();
			ControlStack = new LStack<ControlStackItem>();
		}

		public uint CurrentAddress { get; set; }

		[CanBeNull]
		public Frame CurrentFrame { get; set; }

		public bool Stopped { get; set; }

		[NotNull]
		public LStack<LValue> DataStack { get; private set; }

		[NotNull]
		public LStack<ControlStackItem> ControlStack { get; private set; }
	}

	public class LMachineInterpreter
	{
		public LMachineInterpreter([NotNull] Instruction[] program)
		{
			Program = program;
			State = new LMachineState();
		}

		[NotNull]
		public Instruction[] Program { get; private set; }

		[NotNull]
		public LMachineState State { get; private set; }

		public void Step()
		{
			if (State.CurrentAddress >= Program.Length)
				throw new InvalidOperationException("TODO");
			var instruction = Program[State.CurrentAddress];
			instruction.Execute(State);
		}
	}
}
