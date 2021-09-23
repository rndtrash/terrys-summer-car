using Sandbox;
using System;
using System.Collections.Generic;

namespace TSC
{
	[Library( "plug" ), AutoGenerate]
	public partial class Plug : Asset
	{
		public static IReadOnlyDictionary<string, Plug> All => _all;
		internal static Dictionary<string, Plug> _all = new();

		[Property]
		public string Title { get; set; }
		[Property]
		public string Type { get; set; }
		[Property, ResourceType( "vmdl" )]
		public string Model { get; set; }

		public Plug() : base()
		{
			Log.Info( "New plug!" );
		}

		protected override void PostLoad()
		{
			base.PostLoad();

			if ( !_all.ContainsKey( Name ) )
				_all.Add( Name, this );
		}

		public PlugEntity Spawn()
		{
			PlugEntity o;
			switch ( Type )
			{
				case "structural":
					o = new PlugEntity();
					break;
				default:
					throw new ArgumentException( $"WTF?? No constructor for {Type}???" );
			}

			o.PlugType = Type;
			o.SetModel( Model );
			o.SetupPhysicsFromModel( PhysicsMotionType.Static ); // TODO: Static?

			return o;
		}
	}
}
