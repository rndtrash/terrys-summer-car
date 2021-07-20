using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TSC
{
	public partial class TSCInventory : BaseInventory
	{
		public class InventoryCell
		{
			public Stack<BaseInventoryItem> Items;
			public Type Type;
		}

		public List<InventoryCell> Slots;

		public TSCInventory( Player player, int InventorySize = 4 * 4 ) : base( player )
		{
			Slots = new( InventorySize );
		}

		public override bool CanAdd( Entity entity )
		{
			return CanAdd( entity, out var _ );
		}

		protected bool CanAdd( Entity entity, out InventoryCell slot)
		{
			slot = null;
			if ( !entity.IsValid() || !base.CanAdd( entity ) || entity is not ItemEntity )
				return false;

			var item = entity as ItemEntity;

			foreach ( var i in Slots )
			{
				if ( i.Items.Count == 0 || (i.Type == item.Item.GetType() && i.Items.Count < i.Items.Peek().MaximumAmount ))
				{
					slot = i;
					return true;
				}
			}

			return false;
		}

		public override bool Add( Entity entity, bool makeActive = false )
		{
			Host.AssertServer();

			InventoryCell slot;
			if ( !entity.IsValid() || !CanAdd(entity, out slot) )
				return false;

			if ( slot == null )
				throw new Exception( "what the fuck" );

			//
			// Can't pickup if already owned
			//
			if ( entity.Owner != null )
				return false;

			//
			// Let the inventory reject the entity
			//
			if ( !CanAdd( entity ) )
				return false;

			//
			// Let the entity reject the inventory
			//
			if ( !entity.CanCarry( Owner ) )
				return false;

			//
			// Passed!
			//

			entity.Parent = Owner;

			//
			// Let the item do shit
			//
			entity.OnCarryStart( Owner );

			if ( makeActive )
			{
				SetActive( entity );
			}

			var item = (entity as ItemEntity).Item;
			if ( slot.Items.Count == 0 )
				slot.Type = item.GetType();
			slot.Items.Push( item );
			return true;
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x?.GetType() == t );
		}

		public override bool Drop( Entity ent )
		{
			throw new NotImplementedException();
		}

		public bool Drop( int index )
		{
			if ( !Host.IsServer || !HasItems(index) )
				return false;

			var slot = Slots[index];
			var item = slot.Items.Peek();
			if ( !item.CanDrop() )
				return false;

			item.Drop( Owner );
			return true;
		}

		public bool HasItems(int index)
		{
			return index < Slots.Count && Slots[index].Items.Count > 0;
		}
	}
}
