using Sandbox;
using System;

namespace TSC
{
	public class TSCSocketFactory
	{
		public string Type;
		public string Model;

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

			return o;
		}
	}
}
