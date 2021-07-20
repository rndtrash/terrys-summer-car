using Sandbox;

namespace TSC
{
	public partial class ItemEntity : BaseCarriable
	{
		public BaseInventoryItem Item = new();

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/error.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}

		public override void OnCarryStart( Entity carrier )
		{
			if ( IsClient ) return;
			
			base.OnCarryStart( carrier );
			Item.PreservedEntity = this;
		}
	}
}
