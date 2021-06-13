using Sandbox;

namespace TSC
{
    class TPoseAnimator : PawnAnimator
    {
        public override void Simulate()
        {
            Rotation = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );

            //
            // Look in the direction what the player's input is facing
            //

            SetLookAt( "aim_eyes", Pawn.EyePos + Input.Rotation.Forward * 200 );

            if ( Pawn.ActiveChild is BaseCarriable carry )
            {
                carry.SimulateAnimator( this );
            }
        }

        public override void OnEvent( string name )
        {
            base.OnEvent( name );
        }
    }
}
