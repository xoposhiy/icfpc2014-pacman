using System;
using Lib.Game;

namespace Lib.GMachine
{
	public class Int : GCmd
	{
		public Int(int intArg)
			: base(GCmdType.Int)
		{
			I = intArg;
		}

		public int I { get; private set; }

		public override void Execute([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			switch (I)
			{
				case 0:
					Int0(state, interruptService);
					break;
				case 1:
					Int1(state, interruptService);
					break;
				case 2:
					Int2(state, interruptService);
					break;
				case 3:
					Int3(state, interruptService);
					break;
				case 4:
					Int4(state, interruptService);
					break;
				case 5:
					Int5(state, interruptService);
					break;
				case 6:
					Int6(state, interruptService);
					break;
				case 7:
					Int7(state, interruptService);
					break;
				case 8:
					Int8(state, interruptService);
					break;
				default:
					throw new InvalidOperationException(string.Format("Invalid interrupt: {0}", I));
			}
		}

		private static void Int0([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			var newDirection = state.Registers[0] > 3 ? (Direction?)null : (Direction)state.Registers[0];
			if (newDirection.HasValue)
				interruptService.SetNewDirectionForThisGhost(newDirection.Value);
		}

		private static void Int1([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			var l = interruptService.GetLamdbaManCurrentLocation();
			state.Registers[0] = (byte)l.X;
			state.Registers[1] = (byte)l.Y;
		}

		private static void Int2([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			// nb! there is no second lambda man
		}

		private static void Int3([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			state.Registers[0] = interruptService.GetThisGhostIndex();
		}

		private static void Int4([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			var ghostState = interruptService.TryGetGhostState(state.Registers[0]);
			if (ghostState != null)
			{
				state.Registers[0] = (byte)ghostState.initialLocation.X;
				state.Registers[1] = (byte)ghostState.initialLocation.Y;
			}
		}

		private static void Int5([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			var ghostState = interruptService.TryGetGhostState(state.Registers[0]);
			if (ghostState != null)
			{
				state.Registers[0] = (byte)ghostState.location.X;
				state.Registers[1] = (byte)ghostState.location.Y;
			}
		}

		private static void Int6([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			var ghostState = interruptService.TryGetGhostState(state.Registers[0]);
			if (ghostState != null)
			{
				state.Registers[0] = (byte)ghostState.vitality;
				state.Registers[1] = (byte)ghostState.direction;
			}
		}

		private static void Int7([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			state.Registers[0] = (byte)interruptService.GetMapState(state.Registers[0], state.Registers[1]);
		}

		private static void Int8([NotNull] GMachineState state, [NotNull] IGhostInterruptService interruptService)
		{
			interruptService.DebugTrace(state.Pc, state.Registers);
		}
	}
}