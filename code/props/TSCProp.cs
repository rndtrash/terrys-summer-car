using Sandbox;
using System;
using System.Collections.Generic;

namespace TSC
{
	public class TSCProp
	{
		public string Type;
		public string Model;

		public TSCProp()
		{
		}

		public Prop Spawn()
		{
			Prop o;
			switch (Type)
			{
				case "generic":
					o = new Prop();
					break;
				default:
					throw new ArgumentException( $"WTF?? No constructor for {Type}???" );
			}

			o.SetModel( Model );
			o.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

			return o;
		}
	}
}
