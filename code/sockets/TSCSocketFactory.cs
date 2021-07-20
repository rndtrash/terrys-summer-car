using Sandbox;
using System;

namespace TSC
{
	public class TSCSocketFactory
	{
		public class TestTrigger : BaseTrigger
		{
			public override void Touch( Entity other )
			{
				Log.Info( $"Touch! {other.EntityName}" );

				base.Touch( other );
			}
		}

		public string Type;
		public string Model;
		public Cube TriggerZone;

		public SocketEntity Spawn()
		{
			SocketEntity o;
			switch ( Type )
			{
				case "structural":
					o = new SocketEntity();
					break;
				default:
					throw new ArgumentException( $"WTF?? No constructor for {Type}???" );
			}

			o.SocketType = Type;
			o.SetModel( Model );
			o.SetupPhysicsFromModel( PhysicsMotionType.Static ); // TODO: Static?

			var t = new TestTrigger();
			t.Tags.Add( "socket" );
			t.SetupPhysicsFromAABB( PhysicsMotionType.Static, TriggerZone.First, TriggerZone.Second );

			t.Parent = o;
			o.PlugTrigger = t;

			return o;
		}
	}
}
