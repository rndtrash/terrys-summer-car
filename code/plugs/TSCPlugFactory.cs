using Sandbox;
using System;

namespace TSC
{
	public class TSCPlugFactory
	{
		public string Type;
		public string Model;

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
