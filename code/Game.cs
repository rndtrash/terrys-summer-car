
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
		public static Dictionary<string, TSCSocketFactory> Sockets;
		public static Dictionary<string, TSCPlugFactory> Plugs;
		public static Dictionary<string, TSCPropFactory> Props;

		public static string SocketExtension = ".socket.json";
		public static string PlugExtension = ".plug.json";
		public static string PropExtension = ".prop.json";

		public struct TSCPropJSON
		{
			public string Type { get; set; }
			public List<TypePosRot> Sockets { get; set; }
			public List<TypePosRot> Plugs { get; set; }
		}

		public struct TSCSocketJSON
		{
			public string Type { get; set; }
			public Cube Trigger { get; set; }
		}

		public struct TSCPlugJSON
		{
			public string Type { get; set; }
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

		[AdminCmd( "tsc_spawn" )]
		public static void tsc_spawn( string name = "" )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			if ( name == "" )
			{
				Log.Error( $"Available entities to spawn:\n- {String.Join( "\n- ", Props.Keys )}" );
				return;
			}

			if ( Props.ContainsKey( name ) )
			{
				var o = Props[name].Spawn();

				var p = ConsoleSystem.Caller.Pawn;
				o.Position = p.Position + p.Rotation.Forward.Normal * 100.0f;
			}
			else
				Log.Warning( $"tsc_spawn: not found {name}." );
		}

		// TODO: please don't
		[AdminCmd( "tsc_lookup" )]
		public static void RecursivePropLookup( string root = "entities" )
		{
			Log.Info( $"RecursivePropLookup looking..." );

			IEnumerable<string> result;
			string path;

			#region Locate sockets
			if ( Sockets == null )
				Sockets = new Dictionary<string, TSCSocketFactory>();

			path = $"{root}/sockets";
			result = FileSystem.Mounted.FindFile( path, "*" + SocketExtension, true );
			foreach ( var file in result )
			{
				var json = FileSystem.Mounted.ReadJson<TSCSocketJSON>( $"{root}/sockets/{file}" );
				var filenoext = file.Remove( file.Length - SocketExtension.Length );
				var tscsocket = new TSCSocketFactory()
				{
					Type = json.Type,
					Model = $"models/sockets/{filenoext}.vmdl",
					TriggerZone = json.Trigger
				};
				var fn = filenoext.Split( '/' ).Last();
				if ( Sockets.ContainsKey( fn ) )
				{
					Sockets[fn] = null; // FIXME: trigger GC???????
					Sockets[fn] = tscsocket;
				}
				else
					Sockets.Add( fn, tscsocket );
			}
			#endregion

			#region Locate plugs
			if ( Plugs == null )
				Plugs = new Dictionary<string, TSCPlugFactory>();

			path = $"{root}/plugs";
			result = FileSystem.Mounted.FindFile( path, "*" + PlugExtension, true );
			foreach ( var file in result )
			{
				var json = FileSystem.Mounted.ReadJson<TSCPlugJSON>( $"{root}/plugs/{file}" );
				var filenoext = file.Remove( file.Length - PlugExtension.Length );
				var tscplug = new TSCPlugFactory()
				{
					Type = json.Type,
					Model = $"models/plugs/{filenoext}.vmdl"
				};
				var fn = filenoext.Split( '/' ).Last();
				if ( Plugs.ContainsKey( fn ) )
				{
					Plugs[fn] = null; // FIXME: trigger GC???????
					Plugs[fn] = tscplug;
				}
				else
					Plugs.Add( fn, tscplug );
			}
			#endregion

			#region Locate props
			if ( Props == null )
				Props = new Dictionary<string, TSCPropFactory>();

			path = $"{root}/props";
			result = FileSystem.Mounted.FindFile( path, "*" + PropExtension, true );
			foreach ( var file in result )
			{
				var json = FileSystem.Mounted.ReadJson<TSCPropJSON>( $"{root}/props/{file}" );
				var filenoext = file.Remove( file.Length - PropExtension.Length );

				List<TSCSocketFactory> prop_sockets = new();
				List<PosRot> prop_socketposrots = new();
				foreach ( var socket in json.Sockets )
				{
					if ( !Sockets.ContainsKey( socket.Type ) )
					{
						throw new ArgumentException( $"RecursivePropLookup: {filenoext}: invalid socket {socket.Type}" );
					}
					prop_sockets.Add( Sockets[socket.Type] );
					prop_socketposrots.Add( new PosRot( socket.Position, socket.Rotation ) );
				}

				List<TSCPlugFactory> prop_plugs = new();
				List<PosRot> prop_plugposrots = new();
				foreach ( var plug in json.Plugs )
				{
					if ( !Plugs.ContainsKey( plug.Type ) )
					{
						throw new ArgumentException( $"RecursivePropLookup: {filenoext}: invalid plug {plug.Type}" );
					}
					prop_plugs.Add( Plugs[plug.Type] );
					prop_plugposrots.Add( new PosRot( plug.Position, plug.Rotation ) );
				}

				var tscprop = new TSCPropFactory()
				{
					Type = json.Type,
					Model = $"models/props/{filenoext}.vmdl",
					SpawnSockets = prop_sockets,
					SocketPosRots = prop_socketposrots,
					SpawnPlugs = prop_plugs,
					PlugPosRots = prop_plugposrots
				};

				var fn = filenoext.Split( '/' ).Last();
				if ( Props.ContainsKey( fn ) )
				{
					Props[fn] = null; // FIXME: trigger GC???????
					Props[fn] = tscprop;
				}
				else
					Props.Add( fn, tscprop );
			}
			#endregion
		}
	}

}
