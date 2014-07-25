using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
	
	/// <summary>
	/// Interface of step-function of Lambda-Man
	/// </summary>
	/// <returns>Current state of LM, Direction of move</returns>
	public delegate Tuple<LValue, Direction> LMStep(LValue ai, World world);
	public delegate Tuple<LValue, LMStep> LMMain(World initialWorld);

	public interface ILambdaMan
	{
	   Tuple<LValue, LMStep> Main(World initialWorld);
	}


   
}
