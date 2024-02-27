using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Drawing.Drawing2D; // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
	private Canvas test;
	private Canvas test2;
	
	private Player player1;
	private Player player2;
	private EasyDraw leftSide;
	private EasyDraw rightSide;
	private HUD hud;
	private GUI gui;
	private Eggs eggs;

	private MyGame() : base(1366, 768, false, pPixelArt:true) 
	{
		eggs = new Eggs();
		// AddChild(eggs);
		
		gui = new GUI();
		AddChild(gui);

		player1 = new Player(0, gui, eggs, 0,0);
		player2 = new Player(1, gui, eggs, 1,1);
		gui.SetScoreUpdateCallback(0, player1.UpdateScoreCallback);
		gui.SetScoreUpdateCallback(1, player2.UpdateScoreCallback);
		AddChild(player1);
		AddChild(player2);

		hud = new HUD(player1, player2, eggs.GetNumOnes());
		AddChild(hud);
		
		// --TEST PURPOSES--
		// test = new Canvas("_DSF5407.jpg", false);
		// test.scale = 2f;
		// AddChild(test);
		// test2 = new Canvas("test3.png", false);
		// test2.scale = 16f;
		// AddChild(test2);
		// -----------------

		// -- TO SEE WHAT ARE THE EGGS --
		// foreach (int i in eggs)
		// {
		// 	Console.WriteLine(i);	
		// }
		// -------------------------------
	}

	// void Update()
	// {
	// 	Console.WriteLine(player1.GetScore());
	// 	Console.WriteLine(player2.GetScore());
	// }
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}