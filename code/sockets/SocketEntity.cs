using Sandbox;
using System;

namespace TSC
{
	public partial class SocketEntity : ModelEntity
	{
		public string SocketType;

		[Net]
		public BaseTrigger PlugTrigger { get; set; }

		public SocketEntity()
		{
		}
	}
}
