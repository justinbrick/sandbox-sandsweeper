using System.Linq;
using Sandbox;
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
		public Panel[] Rows;
		public SSTile[,] Board;
		public bool Failed = false;
		
		public SSBoard()
		{
			if ( _instance is null ) Instance = this;
		}

		public void Reset()
		{
			Failed = false;
			var (width, height) = BoardSize;
			if (Board is not null)
				foreach ( var tile in Board )
					tile.Delete();
			Board = new SSTile[width, height];
			Rows = new Panel[width];

			// For the entire board's height, add our rows.
			for (int i = 0; i < width; ++i)
			{
				var row = Rows[i] = AddChild<Panel>("row");
				for (int j = 0; j < height; ++j)
				{
					var tile = row.AddChild<SSTile>("Tile");
					tile.Position = (i, j);
					Board[i, j] = tile;
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
