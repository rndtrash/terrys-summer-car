using Sandbox;
using Sandbox.Joints;
using System;
using System.Linq;

namespace TSC
{
	public partial class Hands : BaseCarriable
	{
		public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

		private PhysicsBody holdBody;
		private WeldJoint holdJoint;

		public PhysicsBody HeldBody { get; private set; }
		public Rotation HeldRot { get; private set; }
		public ModelEntity HeldEntity { get; private set; }
		public CollisionGroup HeldCollisionGroup { get; private set; }

		protected virtual float LinearFrequency => 10.0f;
		protected virtual float LinearDampingRatio => 1.0f;
		protected virtual float AngularFrequency => 10.0f;
		protected virtual float AngularDampingRatio => 1.0f;
		protected virtual float ThrowForce => 1000.0f;
		protected virtual float HoldDistance => 150.0f;
		protected virtual float AttachDistance => 300.0f;
		protected virtual float DropCooldown => 0.5f;
		protected virtual float BreakLinearForce => 2000.0f;

		private TimeSince timeSinceDrop;

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );

			CollisionGroup = CollisionGroup.Weapon;
			SetInteractsAs( CollisionLayer.Debris );
		}

		public override void Simulate( Client client )
		{
			if ( Owner is not Player owner ) return;

			if ( !IsServer )
				return;

			using ( Prediction.Off() )
			{
				var eyePos = owner.EyePos;
				var eyeRot = owner.EyeRot;
				var eyeDir = owner.EyeRot.Forward;

				if ( HeldBody.IsValid() && HeldBody.PhysicsGroup != null )
				{
					if ( holdJoint.IsValid && !holdJoint.IsActive )
					{
						GrabEnd();
					}
					else if ( Input.Pressed( InputButton.Attack1 ) )
					{
						if ( HeldBody.PhysicsGroup.BodyCount > 1 )
						{
							// Don't throw ragdolls as hard
							HeldBody.PhysicsGroup.ApplyImpulse( eyeDir * (ThrowForce * 0.5f), true );
							HeldBody.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * ThrowForce, true );
						}
						else
						{
							HeldBody.ApplyImpulse( eyeDir * (HeldBody.Mass * ThrowForce) );
							HeldBody.ApplyAngularImpulse( Vector3.Random * (HeldBody.Mass * ThrowForce) );
						}

						GrabEnd();
					}
					else if ( Input.Pressed( InputButton.Use ) )
					{
						timeSinceDrop = 0;

						GrabEnd();
					}
					else
					{
						GrabMove( eyePos, eyeDir, eyeRot );
					}

					return;
				}

				if ( timeSinceDrop < DropCooldown )
					return;

				var tr = Trace.Ray( eyePos, eyePos + eyeDir * AttachDistance )
					.UseHitboxes()
					.Ignore( owner, false )
					.Radius( 2.0f )
					.HitLayer( CollisionLayer.Debris )
					.Run();

				if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
					return;

				if ( tr.Entity.PhysicsGroup == null )
					return;

				var modelEnt = tr.Entity as ModelEntity;
				if ( !modelEnt.IsValid() )
					return;

				var body = tr.Body;

				if ( Input.Down( InputButton.Use ) )
				{
					var physicsGroup = tr.Entity.PhysicsGroup;

					if ( physicsGroup.BodyCount > 1 )
					{
						body = modelEnt.PhysicsBody;
						if ( !body.IsValid() )
							return;
					}

					if ( eyePos.Distance( body.Position ) <= AttachDistance )
					{
						GrabStart( modelEnt, body, eyePos + eyeDir * HoldDistance, eyeRot );
					}
				}
			}
		}

		private void Activate()
		{
			if ( !holdBody.IsValid() )
			{
				holdBody = new PhysicsBody
				{
					BodyType = PhysicsBodyType.Keyframed
				};
			}
		}

		private void Deactivate()
		{
			GrabEnd();

			holdBody?.Remove();
			holdBody = null;
		}

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );

			if ( IsServer )
			{
				Activate();
			}
		}

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			base.ActiveEnd( ent, dropped );

			if ( IsServer )
			{
				Deactivate();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if ( IsServer )
			{
				Deactivate();
			}
		}

		public override void OnCarryDrop( Entity dropper )
		{
		}

		private static bool IsBodyGrabbed( PhysicsBody body )
		{
			// There for sure is a better way to deal with this
			if ( All.OfType<Hands>().Any( x => x?.HeldBody?.PhysicsGroup == body?.PhysicsGroup ) ) return true;

			return false;
		}

		private void GrabStart( ModelEntity entity, PhysicsBody body, Vector3 grabPos, Rotation grabRot )
		{
			if ( !body.IsValid() )
				return;

			if ( body.PhysicsGroup == null )
				return;

			if ( IsBodyGrabbed( body ) )
				return;

			GrabEnd();

			HeldBody = body;
			HeldRot = grabRot.Inverse * HeldBody.Rotation;

			holdBody.Position = grabPos;
			holdBody.Rotation = HeldBody.Rotation;

			HeldBody.Wake();
			HeldBody.EnableAutoSleeping = false;

			holdJoint = PhysicsJoint.Weld
				.From( holdBody )
				.To( HeldBody, HeldBody.LocalMassCenter )
				.WithLinearSpring( LinearFrequency, LinearDampingRatio, 0.0f )
				.WithAngularSpring( AngularFrequency, AngularDampingRatio, 0.0f )
				.Breakable( HeldBody.Mass * BreakLinearForce, 0 )
				.Create();

			HeldEntity = entity;
			HeldCollisionGroup = HeldEntity.CollisionGroup;
			HeldEntity.CollisionGroup = CollisionGroup.Debris;

			var client = GetClientOwner();
			client?.Pvs.Add( HeldEntity );
		}

		private void GrabEnd()
		{
			if ( holdJoint.IsValid )
			{
				holdJoint.Remove();
			}

			if ( HeldBody.IsValid() )
			{
				HeldBody.EnableAutoSleeping = true;
			}

			if ( HeldEntity.IsValid() )
			{
				HeldEntity.CollisionGroup = HeldCollisionGroup;

				var client = GetClientOwner();
				client?.Pvs.Remove( HeldEntity );
			}

			HeldBody = null;
			HeldRot = Rotation.Identity;
			HeldEntity = null;
		}

		private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot )
		{
			if ( !HeldBody.IsValid() )
				return;

			holdBody.Position = startPos + dir * HoldDistance;
			holdBody.Rotation = rot * HeldRot;
		}
	}
}
