using Lib.Game;

namespace Lib.GMachine
{
	public interface IInterruptService
	{
		void SetNewDirectionForThisGhost(Direction newDirection);

		[NotNull]
		Point GetLamdbaManCurrentLocation();

		byte GetThisGhostIndex();

		[CanBeNull]
		GhostState TryGetGhostState(byte ghostIndex);

		MapCell GetMapState(byte x, byte y);

		void DebugTrace(byte pc, byte[] registers);
	}
}