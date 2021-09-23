using Sandbox;
using System;
using System.Collections.Generic;

namespace TSC
{
	[Library( "socket" ), AutoGenerate]
	public partial class Socket : Asset
	{
		public static IReadOnlyDictionary<string, Socket> All => _all;
		internal static Dictionary<string, Socket> _all = new();

		[Property]
		public string Title { get; set; }
		[Property]
		public string Type { get; set; }
		[Property, ResourceType( "vmdl" )]
		public string Model { get; set; }
		[Property]
		public Vector3 TriggerZoneMins { get; set; }
		[Property]
		public Vector3 TriggerZoneMaxs { get; set; }

		public class TestTrigger : BaseTrigger
		{
			public override void Touch( Entity other )
			{
				Log.Info( $"Touch! {other.EngineEntityName}" );

				base.Touch( other );
			}
		}

		protected override void PostLoad()
		{
			base.PostLoad();

			if ( !_all.ContainsKey( Name ) )
				_all.Add( Name, this );
		}

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
			t.SetupPhysicsFromAABB( PhysicsMotionType.Static, TriggerZoneMins, TriggerZoneMaxs );

			t.Parent = o;
			o.PlugTrigger = t;

			return o;
		}
	}
}
