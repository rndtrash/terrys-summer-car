using Sandbox;
using System.Collections.Generic;

namespace TSC
{
	public class TSCPropEntity : Sandbox.Prop
	{
		public List<SocketEntity> Sockets = new();
		public List<PlugEntity> Plugs = new();

		public TSCPropEntity() : base()
		{
			//
		}
	}
}
