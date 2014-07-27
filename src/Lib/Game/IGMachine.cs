namespace Lib.Game
{
	public interface IGMachine
	{
		void Run();
		int GhostIndex { get; }
	}
}