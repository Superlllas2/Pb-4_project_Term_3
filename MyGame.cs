using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Drawing.Drawing2D; // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
	//--LEGACY--
	// private LegacyPlot _legacyPlot;
	// private LegacyPlayer _legacyPlayer;
	//----------

	private Player player1;
	private Player player2;
	private EasyDraw leftSide;
	private EasyDraw rightSide;
	private HUD hud;
	private GUI gui;
	private Eggs eggs;

	private MyGame() : base(1300, 800, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		// -- TO SEE WHAT ARE THE EGGS --
		// foreach (int i in eggs)
		// {
		// 	Console.WriteLine(i);	
		// }
		// -------------------------------

		eggs = new Eggs();
		// AddChild(eggs);
		
		gui = new GUI();
		AddChild(gui);

		player1 = new Player(0, gui, eggs);
		player2 = new Player(1, gui, eggs);
		gui.SetScoreUpdateCallback(0, player1.UpdateScoreCallback);
		gui.SetScoreUpdateCallback(1, player2.UpdateScoreCallback);
		AddChild(player1);
		AddChild(player2);

		hud = new HUD(player1, player2, eggs.GetNumOnes());
		AddChild(hud);
	}
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}