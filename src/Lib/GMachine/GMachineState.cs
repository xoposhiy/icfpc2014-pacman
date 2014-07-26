using System;

namespace Lib.GMachine
{
	public class GMachineState
	{
		public GMachineState()
		{
			Pc = 0;
			Registers = new byte[8];
			DataMemory = new byte[byte.MaxValue];
		}

		// todo !!! is PC readable by commands?
		public byte Pc { get; set; }

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
	}
}