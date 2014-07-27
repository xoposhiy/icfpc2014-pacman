using Lib.Game;

namespace Lib.AI
{
	public class RandomGhostFactory : IGMachineFactory
	{
		public IGMachine Create(int ghostIndex, IGhostInterruptService interruptService)
		{
			if (ghostIndex % 2 == 0) return new ChaseGhost(interruptService);
			return new RandomGhost(interruptService);
		}
	}
}