using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace TSC
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class TSCHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public TSCHudEntity()
		{
			if ( IsClient )
			{
				//RootPanel.SetTemplate( "/ui/tschud.html" );
				RootPanel.StyleSheet.Load( "/ui/tschud.scss" );

				RootPanel.AddChild<Health>();
				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<KillFeed>();
				RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
				RootPanel.AddChild<NameTags>();

				RootPanel.AddChild<InventoryPanel>();
			}
		}
	}

}
