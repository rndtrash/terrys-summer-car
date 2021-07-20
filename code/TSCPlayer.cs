using Sandbox;

namespace TSC
{
	public partial class TSCPlayer : Player
	{

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			// T POSE
			Animator = new TPoseAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Inventory = new TSCInventory(this);

			var itemEntity = new ItemEntity();

			base.Respawn();

			itemEntity.Position = Position + Rotation.Forward * 25.0f;
			Inventory.Add( itemEntity );
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			SimulateActiveChild( cl, ActiveChild );
		}
	}
}
