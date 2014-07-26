using Lib.Game;

namespace Lib.AI
{
	public class RandomGhostFactory : IGMachineFactory
	{
		public IGMachine Create(int ghostIndex, IGhostInterruptService interruptService)
		{
			return new RandomGhost(interruptService);
		}
	}
}