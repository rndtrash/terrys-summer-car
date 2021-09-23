using Sandbox;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace TSC
{
	public partial class TSCGame : Game
	{
		public TSCGame()
		{
			if ( IsServer )
			{
				_ = new TSCHudEntity();
			}
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new TSCPlayer();
			client.Pawn = player;

			player.Respawn();
		}

		[AdminCmd( "tsc_spawn" )]
		public static void tsc_spawn( string name = "" )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			if ( name == "" )
			{
				if ( Prop.All.Count > 0 )
					Log.Error( $"Available entities to spawn:\n- {string.Join( "\n- ", Prop.All.Keys )}" );
				return;
			}

			if ( Prop.All.ContainsKey( name ) )
			{
				var o = Prop.All[name].Spawn();

				var p = ConsoleSystem.Caller.Pawn;
				o.Position = p.Position + p.Rotation.Forward.Normal * 100.0f;
			}
			else
				Log.Warning( $"tsc_spawn: not found {name}." );
		}

		[ServerCmd( "tsc_stats" )]
		public static void tsc_stats()
		{
			Log.Info( $"Plugs: {Plug.All.Count}; Sockets: {Socket.All.Count}; Props: {Prop.All.Count}" );
		}
	}

}
