using Sandbox;

namespace SandSweeper
{
	// Do we even need this? Only time will tell.
	public partial class SSController : Entity
	{
		public override void Simulate( Client cl )
		{
			if ( Input.Pressed( InputButton.Run ) ) SSTile.RevealNear();
			base.Simulate( cl );
		}
	}
}
