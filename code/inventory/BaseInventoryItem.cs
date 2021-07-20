using Sandbox;
using System;

namespace TSC
{
	public partial class BaseInventoryItem
	{
		public readonly string ID = "error";
		public readonly string Name = "BaseInventoryItem";
		public readonly float Weight = 1.0f;
		public readonly uint MaximumAmount = 20;

		public ItemEntity PreservedEntity = null;

		public virtual void Drop( Entity owner )
		{
			if ( PreservedEntity == null )
				Create();
			else
			{
				PreservedEntity.OnCarryDrop( owner );
				PreservedEntity = null;
			}
		}

		public virtual bool CanDrop()
		{
			return true;
		}

		protected virtual ItemEntity Create()
		{
			throw new NotImplementedException();
		}
	}
}
