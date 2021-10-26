using System;
using System.CodeDom.Compiler;
using Sandbox;
using Sandbox.UI;

namespace SandSweeper
{
	public class SSTile : Label
	{
		private static Random _rand = new();
		public bool IsMine;

		private State _state = State.Hidden;
		public State State
		{
			get => _state;
			set
			{
				var old = _state;
				_state = value;
				var instance = SSBoard.Instance;
				SetClass("marked", value == State.Marked);
				SetClass( "revealed", value == State.Revealed );
				if ( value == State.Revealed && old != State.Revealed )
				{
					if ( IsMine )
					{
						if ( !instance.Failed ) instance.Fail();
						return;
					}

					if ( instance.HasWon() ) SSHud.Instance.RootPanel.SetClass("win", true); //!instance.Failed && 
					var (x, y) = Position;
					var max = instance.BoardSize.Item1;


					var nearbyMines = GetNearbyMines();
					if ( nearbyMines == 0 )
					{
						for ( int i = -1; i < 2; ++i )
						{
							for ( int j = -1; j < 2; ++j )
							{
								var (xx, yy) = (x + i, y + j);
								if ( xx >= max || xx < 0 || yy >= max || yy < 0 ) continue;
								var tile = instance.Board[xx, yy];
								if ( tile == this || tile.State == State.Revealed ) continue;
								tile.State = State.Revealed;
							}
						}

						return;
					}

					Style.Content = nearbyMines.ToString();
					Style.FontColor = nearbyMines switch
					{
						1 => Color.Blue,
						2 => Color.Green,
						_ => Color.Red
					};
					Style.Dirty();
				}
			}
		}
		public (int, int) Position;
		public SSTile()
		{
			ChangeMineStatus();
		}

		public int GetNearbyMines()
		{
			var instance = SSBoard.Instance;
			var (x, y) = Position;
			var max = instance.BoardSize.Item1;
			int nearbyMines = 0;
			for ( int i = -1; i < 2; ++i )
			{
				for ( int j = -1; j < 2; ++j )
				{
					var (xx, yy) = (x + i, y + j);
					if ( xx >= max || xx < 0 || yy >= max || yy < 0 ) continue;
					var tile = instance.Board[xx, yy];
					if ( tile == this ) continue;
					if ( tile.IsMine ) ++nearbyMines;
				}
			}

			return nearbyMines;
		}

		// Change the status of the mine; NOTE: This might not need to be this way, I moved this into this function as a helper but I realized that resetting the board also takes into account the size of it.
		public void ChangeMineStatus()
		{
			IsMine = _rand.Next( 0, 6 ) == 3;
			SetClass( "mine", IsMine );
		}
		
		protected override void OnClick( MousePanelEvent e )
		{
			if ( State is State.Marked ) return;
			State = State.Revealed;
		}

		protected override void OnRightClick( MousePanelEvent e )
		{
			if ( State == State.Revealed ) return;
			State = (State == State.Hidden) ? State.Marked : State.Hidden;
			base.OnRightClick( e );
		}
	}

	public enum State : byte
	{
		Revealed,
		Hidden,
		Marked
	}
}
