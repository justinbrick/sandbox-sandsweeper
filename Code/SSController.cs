using Sandbox;

namespace SandSweeper
{
	public partial class SSController : Entity
	{
		public override void Simulate( Client cl )
		{
			if ( Input.Pressed( InputButton.Run ) ) SSTile.RevealNear();
			base.Simulate( cl );
		}
	}
}
