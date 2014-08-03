using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Game;

namespace Lib.GMachine
{
	public class GMachine2
	{
		public GMachineState state = new GMachineState();
		public IGhostInterruptService interrupts;
		private readonly Dictionary<string, Action<GArg[], byte[]>> instructions = new Dictionary<string, Action<GArg[], byte[]>>();

		public GMachine2(IGhostInterruptService interrupts)
		{
			this.interrupts = interrupts;
			RegisterOp("mov", ops => ops[1]);
			RegisterOp("add", ops => ops[0] + ops[1]);
			RegisterOp("add", ops => ops[0] - ops[1]);
			RegisterOp("and", ops => ops[0] & ops[1]);
			RegisterOp("or",  ops => ops[0] | ops[1]);
			RegisterOp("or",  ops => ops[0] ^ ops[1]);
			RegisterOp("dec", ops => --ops[0]);
			RegisterOp("inc", ops => ++ops[0]);
			RegisterOp("div", ops => ops[0]/ops[1]);
			RegisterOp("mul", ops => ops[0]*ops[1]);
			RegisterAction("hlt", ops => state.Hlt = true);
			RegisterAction("int", ops => Interrupt(ops[0]));
			RegisterAction("jeq", ops => state.Pc = ops[0] == ops[1] ? ops[3] : state.Pc);
			RegisterAction("jgt", ops => state.Pc = ops[0] > ops[1] ? ops[3] : state.Pc);
			RegisterAction("jlt", ops => state.Pc = ops[0] < ops[1] ? ops[3] : state.Pc);
		}

		private void RegisterOp(string name, Func<byte[], int> calc)
		{
			instructions.Add(name, (ops, vals) => state.WriteValue(ops[0], (byte)calc(vals)));
		}

		private void RegisterAction(string name, Action<byte[]> action)
		{
			instructions.Add(name, (ops, vals) => action(vals));
		}

		public void Run(string instructionName, GArg[] operands)
		{
			instructions[instructionName](operands, operands.Select(op => state.ReadValue(op)).ToArray());
		}
	
		private void Interrupt(byte intNo)
		{
			throw new NotImplementedException("TODO");
		}
	}
}