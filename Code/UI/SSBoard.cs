using System.Linq;
using Sandbox;
using Sandbox.DataModel;
using Sandbox.UI;

namespace SandSweeper
{
	public class SSBoard : Panel
	{
		private static SSBoard _instance;
		public static SSBoard Instance
		{
			get => _instance;
			set => _instance ??= value;
		}
		public (int, int) BoardSize;
		public SSTile[,] Board;
		public bool Failed = false;
		
		public SSBoard()
		{
			if ( _instance is null ) Instance = this;
		}

		public void Reset()
		{
			Failed = false;
			var (x, y) = BoardSize;
			if (Board is not null)
				foreach ( var tile in Board )
					tile.Delete();
			Board = new SSTile[x, y];
			int i = 1;
			for ( int xx = 0; xx < x; ++xx )
			{
				for ( int yy = 0; yy < y; ++yy )
				{
					var tile = AddChild<SSTile>("Tile");
					tile.Position = (xx, yy);
					tile.Style.Height = Length.Percent(100.0f/x-float.Epsilon*2);
					tile.Style.Width = Length.Percent(100.0f/x-float.Epsilon*2);
					
					tile.Style.ZIndex = i;
					tile.Style.Dirty();
					Board[xx, yy] = tile;
					++i;
				}
			}
			
			Style.Dirty();
		}

		// This could be done more efficiently, but considering that this is a quick mockup made in S&box, I really don't care.
		public bool HasWon() => !Failed && Board.Cast<SSTile>().All( tile => tile.IsMine || tile.State == State.Revealed );

		public void Fail()
		{
			Failed = true;
			Log.Info("User failed game!"  );
			var instance = SSHud.Instance;
			instance.RootPanel.SetClass( "fail", true );
			instance.TimerStopped = true;
			foreach (var slot in Board) // Let's go ahead and reveal all the tiles now then.
			{
				slot.State = State.Revealed; 
			}
		}
	}
}
