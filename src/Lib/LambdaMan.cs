using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    
    delegate Tuple<LValue, Direction> LMStep(LValue currentAIState, World currentWorldState);

    class LambdaMan
    {
        public Tuple<LValue, LMStep> Main(World initialWorldState)
        {
            throw new NotImplementedException();
        }
    }
}
