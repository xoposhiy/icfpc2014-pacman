using System;
using Lib.LMachine;

namespace Lib.Game
{
	
	/// <summary>
	/// Interface of step-function of Lambda-Man 
	/// </summary>
	/// <returns>Current state of LM, Direction of move</returns>
	public delegate Tuple<LValue, Direction> LMStep(LValue ai, World world);
	public delegate Tuple<LValue, LMStep> LMMain(World initialWorld);

	public interface LambdaMan
	{
	   Tuple<LValue, LMStep> Main(World initialWorld);
	}


   
}
