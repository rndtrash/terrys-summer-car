using Sandbox;
using System;

namespace TSC
{
	public class TSCSocketFactory
	{
		public class TestTrigger : BaseTrigger
		{
			public override void StartTouch( Entity other )
			{
				Log.Info( $"Touch! {other}" );

				base.StartTouch( other );
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
			t.CollisionBounds = new BBox( TriggerZone.First, TriggerZone.Second );
			t.UsePhysicsCollision = true;

			t.Parent = o;
			o.PlugTrigger = t;
			Log.Info( $"{o.PlugTrigger == null}" );

			return o;
		}
	}
}
