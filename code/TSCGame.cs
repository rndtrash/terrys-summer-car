
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace TSC
{
	public partial class TSCGame : Sandbox.Game
	{
		public static Dictionary<string, TSCProp> Props;
		public static string PropExtension = ".prop.json";

		public struct TSCPropJSON
		{
			public struct TypePos
			{
				public string type { get; set; }
				public Vector3 pos { get; set; }
			}


			public string type { get; set; }
			public List<TypePos> sockets { get; set; }
			public List<TypePos> plugs { get; set; }
		}

		public TSCGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );

				RecursivePropLookup();

				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
				new TSCHudEntity();
			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
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

		[ServerCmd("tsc_spawn")]
		public static void tsc_spawn( string name)
		{
			Assert.NotNull( ConsoleSystem.Caller );
			if ( Props.ContainsKey( name ) )
			{
				var o = Props[name].Spawn();
				o.Position = ConsoleSystem.Caller.Pawn.Position;
			}
			else
				Log.Warning( $"tsc_spawn: not found {name}. available: {String.Join( '/', Props.Keys )}" );
		}

		// TODO: please don't
		[ServerCmd("tsc_lookup")]
		public static void RecursivePropLookup(string root = "entities")
		{
			Log.Info( $"RecursivePropLookup looking..." );
			if (Props == null)
				Props = new Dictionary<string, TSCProp>();

			var result = FileSystem.Mounted.FindFile( root, "*" + PropExtension, true );
			foreach (var file in result)
			{
				var json = FileSystem.Mounted.ReadJson<TSCPropJSON>( $"{root}/{file}" );
				var filenoext = file.Remove( file.Length - PropExtension.Length );
				var tscprop = new TSCProp() {
					Type = json.type,
					Model = $"models/{filenoext}.vmdl"
				};

				//DebugOverlay.Line( Vector3.Up * 1000.0f, tscprop.Position, 1000.0f );
				//
				var fn = filenoext.Split( '/' ).Last();
				if ( Props.ContainsKey( fn ) )
					Props[fn] = tscprop;
				else
					Props.Add( fn, tscprop );
			}
		}
	}

}
