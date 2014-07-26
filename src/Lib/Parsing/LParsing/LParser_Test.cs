using System;
using System.IO;
using System.Linq;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using NUnit.Framework;

namespace Lib.Parsing.LParsing
{
	[TestFixture]
	public class LParser_Test
	{
		private static void AssertInstructionEquals([NotNull] Instruction expected, [CanBeNull] Instruction actual)
		{
			Assert.IsNotNull(actual);
			Assert.That(actual.Type, Is.EqualTo(expected.Type));
			Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
			foreach (var property in expected.GetType().GetProperties())
				Assert.That(property.GetGetMethod().Invoke(actual, new object[0]), Is.EqualTo(property.GetGetMethod().Invoke(expected, new object[0])), property.Name);
		}

		[Test]
		public void ParseCommandWithIntParameter()
		{
			var parseResult = LParser.Parse("ldc 1");
			AssertInstructionEquals(new Ldc(1), parseResult.Program.Single());
			Assert.AreEqual(1, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseCommandWithAddressParameter()
		{
			var parseResult = LParser.Parse("ldf 10");
			AssertInstructionEquals(new Ldf(10), parseResult.Program.Single());
			Assert.AreEqual(1, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseManyLines()
		{
			var parseResult = LParser.Parse("ldc 1 \r\n ldc 2");
			AssertInstructionEquals(new Ldc(1), parseResult.Program[0]);
			AssertInstructionEquals(new Ldc(2), parseResult.Program[1]);
			Assert.AreEqual(1, parseResult.SourceLines[0]);
			Assert.AreEqual(2, parseResult.SourceLines[1]);
		}

		[Test]
		public void ParseConstants()
		{
			var parseResult = LParser.Parse("a = 1 \r\n ldc a");
			AssertInstructionEquals(new Ldc(1), parseResult.Program.Single());
			Assert.AreEqual(2, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseLabel()
		{
			var parseResult = LParser.Parse("ldf lalala \r\n  lalala: \r\n ldc 10");
			AssertInstructionEquals(new Ldf(1), parseResult.Program[0]);
			AssertInstructionEquals(new Ldc(10), parseResult.Program[1]);
			Assert.AreEqual(1, parseResult.SourceLines[0]);
			Assert.AreEqual(3, parseResult.SourceLines[1]);
		}

		[Test]
		public void ParseLabelWithInstraction()
		{
			var parseResult = LParser.Parse("ldf lalala \r\n  lalala: ldc 10");
			AssertInstructionEquals(new Ldf(1), parseResult.Program[0]);
			AssertInstructionEquals(new Ldc(10), parseResult.Program[1]);
			Assert.AreEqual(1, parseResult.SourceLines[0]);
			Assert.AreEqual(2, parseResult.SourceLines[1]);
		}

		[Test]
		public void ParseLabelWithInstractionAndComment()
		{
			var parseResult = LParser.Parse("ldf lalala \r\n  lalala: ldc 10; bububu");
			AssertInstructionEquals(new Ldf(1), parseResult.Program[0]);
			AssertInstructionEquals(new Ldc(10), parseResult.Program[1]);
			Assert.AreEqual(1, parseResult.SourceLines[0]);
			Assert.AreEqual(2, parseResult.SourceLines[1]);
		}

		[Test]
		public void ParseConstantDuplicates()
		{
			Assert.Throws<InvalidOperationException>(() => LParser.Parse("a = 1 \r\n a = 2"));
		}

		[Test]
		public void ParseCommentsAtEnd()
		{
			var parseResult = LParser.Parse("ldc 1 ;lalala");
			AssertInstructionEquals(new Ldc(1), parseResult.Program.Single());
			Assert.AreEqual(1, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseAndExecuteComplexProgram()
		{
			var source = File.ReadAllText(KnownPlace.GccSamples + "local.mgcc");
			var parsedProgram = LParser.Parse(source);
			var m = new LMachineInterpreter(parsedProgram.Program);
			while (!m.State.Stopped)
				m.Step();
			Assert.AreEqual(42, m.State.DataStack.Pop().GetValue());
		}
	}
}