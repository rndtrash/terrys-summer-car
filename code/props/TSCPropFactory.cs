using Sandbox;
using System;
using System.Collections.Generic;

namespace TSC
{
	public class TSCPropFactory
	{
		public string Type;
		public string Model;
		public List<TSCSocketFactory> SpawnSockets = new();
		public List<TSCPlugFactory> SpawnPlugs = new();
		public List<PosRot> SocketPosRots = new();
		public List<PosRot> PlugPosRots = new();

		public TSCPropFactory()
		{
		}

		public TSCPropEntity Spawn()
		{
			TSCPropEntity o;
			switch ( Type )
			{
				case "generic":
					o = new TSCPropEntity();
					break;
				default:
					throw new ArgumentException( $"WTF?? No constructor for {Type}???" );
			}

			o.SetModel( Model );
			o.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

			for ( var i = 0; i < SpawnSockets.Count; i++ )
			{
				var socket = SpawnSockets[i];
				var socketposrot = SocketPosRots[i];
				SocketEntity so = socket.Spawn();
				Log.Info( $"socket {socketposrot.Position}" );
				so.Position = socketposrot.Position;
				so.Parent = o;
				Log.Info( $"socket {so.Position}" );
				so.Rotation = Rotation.From( socketposrot.Rotation.x, socketposrot.Rotation.y, socketposrot.Rotation.z );
				o.Sockets.Add( so );
			}

			for ( var i = 0; i < SpawnPlugs.Count; i++ )
			{
				var plug = SpawnPlugs[i];
				var plugposrot = PlugPosRots[i];
				PlugEntity po = plug.Spawn();
				Log.Info( $"plug {plugposrot.Position}" );
				po.Position = plugposrot.Position;
				po.Parent = o;
				Log.Info( $"plug {po.Position}" );
				po.Rotation = Rotation.From( plugposrot.Rotation.x, plugposrot.Rotation.y, plugposrot.Rotation.z );
				o.Plugs.Add( po );
			}

			return o;
		}
	}
}
