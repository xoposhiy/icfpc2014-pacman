using System;

namespace Lib.GMachine
{
	public class GMachineState
	{
		public GMachineState()
		{
			CyclesMade = 0;
			Registers = new byte[9];
			DataMemory = new byte[256];
		}

		public byte Pc
		{
			get { return Registers[8]; }
			set { Registers[8] = value; }
		}

		public int CyclesMade { get; set; }

		public bool Hlt { get; set; }

		[NotNull]
		public byte[] Registers { get; set; }

		[NotNull]
		public byte[] DataMemory { get; private set; }

		public byte ReadValue([NotNull] GArg srcArg)
		{
			switch (srcArg.Type)
			{
				case GArgType.Const:
					return srcArg.Value;
				case GArgType.Data:
					return DataMemory[srcArg.Value];
				case GArgType.Reg:
					return Registers[srcArg.Value];
				case GArgType.IndirectReg:
					return DataMemory[Registers[srcArg.Value]];
				default:
					throw new InvalidOperationException(string.Format("Invalid srcArg type: {0}", srcArg.Type));
			}
		}

		public void WriteValue([NotNull] GArg dstArg, byte value)
		{
			switch (dstArg.Type)
			{
				case GArgType.Data:
					DataMemory[dstArg.Value] = value;
					break;
				case GArgType.Reg:
					Registers[dstArg.Value] = value;
					break;
				case GArgType.IndirectReg:
					DataMemory[Registers[dstArg.Value]] = value;
					break;
				default:
					throw new InvalidOperationException(string.Format("Invalid dstArg type: {0}", dstArg.Type));
			}
		}

		[NotNull]
		public GMachineState Clone()
		{
			var clone = new GMachineState
			{
				CyclesMade = CyclesMade,
				Pc = Pc,
				Hlt = Hlt,
			};
			Array.Copy(Registers, clone.Registers, Registers.Length);
			Array.Copy(DataMemory, clone.DataMemory, DataMemory.Length);
			return clone;
		}
	}
}