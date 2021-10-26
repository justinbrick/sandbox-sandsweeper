using Sandbox;
using Sandbox.UI;

namespace SandSweeper
{
	// The base game.
	public class SSGame : GameBase
	{

		public SSGame()
		{
			Log.Info( "Starting SandSweeper Game!" );
			if ( IsServer )
				new SSHud();
		}
		
		public override void Shutdown()
		{
			
		}

		public override void ClientJoined( Client cl )
		{
			var pawn = new SSController();
			cl.Pawn = pawn;
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			
		}

		public override bool CanHearPlayerVoice( Client source, Client dest )
		{
			return false;
		}

		public override void PostLevelLoaded()
		{
			
		}

		public override CameraSetup BuildCamera( CameraSetup camSetup )
		{
			return camSetup;
		}

		public override void OnVoicePlayed( ulong steamId, float level )
		{
			
		}
	}
}
