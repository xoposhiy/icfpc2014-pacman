using System;
using Lib.Game;

namespace Lib.AI
{
	public class GhostFactory : IGMachineFactory
	{
		private readonly Action<GMachine.GMachine> runUntilStopStep;
		private readonly Ghost[] gDefs;

		public GhostFactory([NotNull] params Ghost[] gDefs)
			: this(null, gDefs)
		{
		}

		public GhostFactory(Action<GMachine.GMachine> runUntilStopStep,  [NotNull] params Ghost[] gDefs)
		{
			if (gDefs.Length > 4)
				throw new InvalidOperationException("TODO");
			this.runUntilStopStep = runUntilStopStep;
			this.gDefs = gDefs;
		}

		[NotNull]
		public IGMachine Create(int ghostIndex, [NotNull] IGhostInterruptService interruptService)
		{
			var gDef = gDefs[ghostIndex % gDefs.Length];
			if (gDef.ParseResult != null)
				return new GMachine.GMachine(gDef.ParseResult, interruptService, runUntilStopStep);
			if (gDef.GhostType != null)
				return (IGMachine)Activator.CreateInstance(gDef.GhostType, interruptService);
			throw new InvalidOperationException("TODO");
		}
	}
}