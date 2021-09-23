using Sandbox;
using System;
using System.Collections.Generic;

namespace TSC
{
	[Library( "prop" ), AutoGenerate]
	public partial class Prop : Asset
	{
		public static IReadOnlyDictionary<string, Prop> All => _all;
		internal static Dictionary<string, Prop> _all = new();

		[Property]
		public string Title { get; set; }
		[Property]
		public string Description { get; set; }
		[Property]
		public string Type { get; set; }
		[Property, ResourceType( "vmdl" )]
		public string Model { get; set; }
		[Property]
		public string[] PlugAttachmentPoints { get; set; } // Format: name;type
		[Property]
		public string[] SocketAttachmentPoints { get; set; } // Format: name;type

		protected override void PostLoad()
		{
			base.PostLoad();

			if ( !_all.ContainsKey( Name ) )
				_all.Add( Name, this );
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

			foreach ( var socketAttachmentPoint in SocketAttachmentPoints )
			{
				var split = socketAttachmentPoint.Split( ";" );
				if ( split.Length != 2 )
					throw new ArgumentException( "Invalid socket attachment point format, should be \"attpointname;socketname\"" );

				var att = o.GetAttachment( split[0] );
				if ( !att.HasValue )
					throw new ArgumentException( $"Invalid attachment point: {split[0]}" );

				Socket socket;
				try
				{
					socket = Socket.All[split[1]];
				}
				catch ( KeyNotFoundException )
				{
					throw new ArgumentException( $"Invalid socket: {split[1]}" );
				}

				SocketEntity so = socket.Spawn();
				so.Position = att.Value.Position;
				so.Parent = o;
				so.Rotation = att.Value.Rotation;
				o.Sockets.Add( so );
			}

			foreach ( var plugAttachmentPoint in PlugAttachmentPoints )
			{
				var split = plugAttachmentPoint.Split( ";" );
				if ( split.Length != 2 )
					throw new ArgumentException( "Invalid plug attachment point format, should be \"attpointname;plugname\"" );

				var att = o.GetAttachment( split[0] );
				if ( !att.HasValue )
					throw new ArgumentException( $"Invalid attachment point: {split[0]}" );

				Plug plug;
				try
				{
					plug = Plug.All[split[1]];
				}
				catch ( KeyNotFoundException )
				{
					throw new ArgumentException( $"Invalid plug: {split[1]}" );
				}

				PlugEntity po = plug.Spawn();
				po.Position = att.Value.Position;
				po.Parent = o;
				po.Rotation = att.Value.Rotation;
				o.Plugs.Add( po );
			}

			return o;
		}
	}
}
