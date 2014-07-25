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
    public delegate Tuple<LValue, Direction> LMStep(LValue currentAIState, World currentWorldState);

    public interface LambdaMan
    {
       Tuple<LValue, LMStep> Main(World initialWorldState);
    }


   
}
