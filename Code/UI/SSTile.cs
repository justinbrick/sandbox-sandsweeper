using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace SandSweeper
{
	public partial class SSTile : Label
	{
		private static Random _rand = new();
		public static SSTile HoveredTile = null;
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

					var nearby = NearbyTiles().ToArray();
					int mines = nearby.Count( tile => tile.IsMine );
					if ( mines == 0 )
					{
						foreach ( var tile in nearby ) 
							if ( tile.State is not State.Revealed ) tile.State = State.Revealed;
						return;
					}

					Style.Content = mines.ToString();
					Style.FontColor = mines switch
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
		
		/// <summary>
		/// Get the tiles near this one tile.
		/// </summary>
		/// <returns>Enumerable of SSTile</returns>
		public IEnumerable<SSTile> NearbyTiles()
		{
			var instance = SSBoard.Instance;
			var (x, y) = Position;
			var max = instance.BoardSize.Item1;
			for ( int i = -1; i < 2; ++i )
			{
				for ( int j = -1; j < 2; ++j )
				{
					var (xx, yy) = (x + i, y + j);
					if ( xx >= max || xx < 0 || yy >= max || yy < 0 ) continue;
					var tile = instance.Board[xx, yy];
					if ( tile == this ) continue;
					yield return tile;
				}
			}
		}

		// Change the status of the mine; NOTE: This might not need to be this way, I moved this into this function as a helper but I realized that resetting the board also takes into account the size of it.
		public void ChangeMineStatus()
		{
			IsMine = _rand.Next( 0, 6 ) == 3;
			SetClass( "mine", IsMine );
		}

		// You asked for this, now you get it, jerks.
		[ClientRpc]
		public static void RevealNear()
		{
			if ( HoveredTile is null ) return;
			Log.Info( "Revealing Nearby" );
			var nearby = HoveredTile.NearbyTiles().ToArray();
			var mines = nearby.Count( tile => tile.IsMine );
			var flags = nearby.Count( tile => tile.State is State.Marked );
			if ( mines != flags ) return;
			foreach ( var tile in nearby.Where( tile => tile.State is not State.Marked ) ) 
				tile.State = State.Revealed;
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

		protected override void OnMouseOver( MousePanelEvent e )
		{
			base.OnMouseOver( e );
			HoveredTile = this;
		}
	}

	public enum State : byte
	{
		Revealed,
		Hidden,
		Marked
	}
}
