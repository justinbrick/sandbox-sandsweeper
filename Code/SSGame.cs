using Sandbox;
using Sandbox.UI;

namespace SandSweeper
{
	// The base game.
	public partial class SSGame : Game
	{

		public SSGame()
		{
			Log.Info( "Starting SandSweeper Game!" );
			if ( IsServer )
				new SSHud();
		}

		public override void ClientJoined( Client cl )
		{
			var pawn = new SSController();
			cl.Pawn = pawn;
			base.ClientJoined(cl);
		}
	}
}
