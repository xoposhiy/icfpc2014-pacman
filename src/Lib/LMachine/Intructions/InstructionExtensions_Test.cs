using Lib.LMachine.Parsing;
using NUnit.Framework;

namespace Lib.LMachine.Intructions
{
	[TestFixture]
	public class InstructionExtensions_Test
	{
		[Test]
		public void ToGcc()
		{
			const string source = @"half = 21

LDC  half
LDF  doubler    ; load body
AP   1    ; call body with 1 variable in a new frame
RTN

doubler:
LD   0 0  ; var x    :body
LD   0 0  ; var x
ADD
RTN";
			Assert.That(LParser.Parse(source).Program.ToGcc(), Is.EqualTo(@"LDC 21
LDF 4
AP 1
RTN 
LD 0 0
LD 0 0
ADD 
RTN "));
		}
	}
}