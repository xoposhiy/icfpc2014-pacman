using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Game
{

	public delegate Direction GStep(World world);
	public delegate GStep GMain(int ghostId, World initialWorld);

	public interface Ghost
	{
		GStep Main(int ghostId, World initialWorld);
	}
}
