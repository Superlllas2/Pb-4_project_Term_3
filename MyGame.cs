using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{

	private LegacyPlot _legacyPlot;
	private LegacyPlayer _legacyPlayer;

	private HUD hud;
	
	public MyGame() : base(1300, 800, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		hud = new HUD();
		AddChild(hud);


		//--LEGACY--
		// plot = new Plot();
		// AddChild(plot);
		//
		// player = new Player(plot);
		// AddChild(player);
		//----------
	}

	void Update() {
		
	}
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}