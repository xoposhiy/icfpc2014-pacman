using System;
using System.IO;
using Lib.Game;
using Lib.Parsing.GParsing;
using NUnit.Framework;

namespace Lib.GMachine
{
	[TestFixture]
	public class GMachine_Test
	{
		[Test]
		public void Test()
		{
			var x = byte.MaxValue;
			Console.Out.WriteLine(++x);
		}

		[Test]
		public void Run()
		{
			var code = @"
mov a, [10]
inc a
and a, 3
mov [10], a
int 0
hlt
";
			var map = MapUtils.Load(File.ReadAllText(@"..\..\..\..\mazes\maze1.txt"));
			var interrupts = new GameSim.GhostInterruptService(0, new World(map));
			var gm = new GMachine(GParser.Parse(code), interrupts, null);
			gm.Run();
			Assert.AreEqual(Direction.Right, interrupts.NewDirection);
			gm.Run();
			Assert.AreEqual(Direction.Down, interrupts.NewDirection);
			gm.Run();
			Assert.AreEqual(Direction.Left, interrupts.NewDirection);
			gm.Run();
			Assert.AreEqual(Direction.Up, interrupts.NewDirection);
			gm.Run();
			Assert.AreEqual(Direction.Right, interrupts.NewDirection);
		}
		[Test]
		public void TestFuncs()
		{
			var code = @"
r = 255;
mov a, 2
mov [r], pc
mov pc, double
int 8
hlt


double:
	add a, a
	add [r], 2
	mov pc, [r]
";

			var map = MapUtils.Load(File.ReadAllText(@"..\..\..\..\mazes\maze1.txt"));
			var interrupts = new GameSim.GhostInterruptService(0, new World(map));
			var gm = new GMachine(GParser.Parse(code), interrupts, null);
			gm.Run();
		}
	}
}