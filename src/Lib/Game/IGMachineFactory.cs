namespace Lib.Game
{
	public interface IGMachineFactory
	{
		[NotNull]
		IGMachine Create(int ghostIndex, [NotNull] IGhostInterruptService interruptService);
	}
}