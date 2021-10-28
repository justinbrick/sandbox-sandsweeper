using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SandSweeper
{
	/**
	 * TODO: Add a timer for when the game finishes that times how long it took for people to complete the level.
	 * TODO: Add a scoreboard for the fastest times.
	 */
	public class SSHud : HudEntity<RootPanel>
	{
		private static SSHud _instance;
		public static SSHud Instance
		{
			get => _instance;
			set => _instance ??= value;
		}
		private SSBoard _board;
		private Label _timerPanel;
		private TimeSince _startTime;
		private float _elapsedTime;
		
		public bool TimerStopped = false;

		public SSHud()
		{
			if ( !IsClient || _instance is not null ) return;
			_instance = this;
			Log.Info("Running SSHud"  );
			RootPanel.StyleSheet.Load("/UI/SSHud.scss");
			var topBar = RootPanel.Add.Panel("TopBar");
			_board = RootPanel.AddChild<SSBoard>("Board"); // Add the board to the game
			_board.BoardSize = (20,20);
			_board.Reset();
			var restartButton = topBar.Add.Button("", "restart_button");
			restartButton.AddEventListener("onclick", Reset); // We want to reset when this button gets clicked.
			_startTime = 0;
			_elapsedTime = 0;
			_timerPanel = topBar.AddChild<Label>("timer");
		}

		// On reset, we want to set everything back to the way it was beforehand.
		public void Reset()
		{
			RootPanel.SetClass(  "win", false );
			RootPanel.SetClass( "fail", false );
			_board.Reset();
			_startTime = 0;
			_elapsedTime = 0;
			TimerStopped = false;
		}

		[Event.Tick]
		public void Tick(  )
		{
			if ( IsServer ) return;
			if ( !TimerStopped ) _elapsedTime = _startTime;
			_timerPanel.Style.Content = $"{_elapsedTime:0.00}";
			_timerPanel.Style.Dirty();
		}
	}
}
