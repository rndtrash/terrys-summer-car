using Sandbox;
using System;

namespace TSC
{
	public partial class SocketEntity : ModelEntity
	{
		public string SocketType;

		[Net]
		public BaseTrigger PlugTrigger { get; set; }

		public SocketEntity()
		{
		}

		// TODO: remove
		[Event.Tick.Server]
		public void TickServer()
		{
			DebugOverlay.Box( 0, PlugTrigger.Position - PlugTrigger.CollisionBounds.Maxs / 2, PlugTrigger.Position + PlugTrigger.CollisionBounds.Maxs / 2, Color.Red, false );
		}
	}
}
