namespace Lib.Game
{
	public interface IGhostInterruptService
	{
		void SetNewDirectionForThisGhost(Direction newDirection);

		[NotNull]
		Point GetLamdbaManCurrentLocation();

		byte GetThisGhostIndex();

		[CanBeNull]
		GhostState TryGetGhostState(byte ghostIndex);

		MapCell GetMapState(byte x, byte y);

		void DebugTrace(byte pc, [NotNull] byte[] registers);
	}
}