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
	
	public static int[] eggs = new int[12];
	private int numOnes;
	private int numZeros;

	private MyGame() : base(1300, 800, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		CreateArray();
		
		// -- TO SEE WHAT ARE THE EGGS --
		// foreach (int i in eggs)
		// {
		// 	Console.WriteLine(i);	
		// }
		// -------------------------------
		
		gui = new GUI();
		AddChild(gui);

		player1 = new Player(0, gui);
		player2 = new Player(1, gui);
		AddChild(player1);
		AddChild(player2);

		hud = new HUD(player1, player2, numOnes);
		AddChild(hud);
		
		//--LEGACY--
		// plot = new Plot();
		// AddChild(plot);
		//
		// player = new Player(plot);
		// AddChild(player);
		//----------
	}
	
	// Next method first puts all 1s into array, then 0s, and then shuffles it, so we get random placement of the
	// specific number of 1s and 0s :)
	void CreateArray()
	{
		numOnes = Utils.Random(5, 10);
		numZeros = eggs.Length - numOnes; 
		
		// Setting random number of 1s to the beginning array untill all 1s are assigned
		for (int i = 0; i < numOnes; i++)
		{
			eggs[i] = 1;
		}
		
		// Setting (array.Length - 1s amount) amount of 0 to the end of the array
		for (int i = numOnes; i < eggs.Length; i++)
		{
			eggs[i] = 0;
		}

		// We get the value from a random index and swap it with another value of a random index
		// We do it eggs.Length times
		for (int i = 0; i < eggs.Length; i++)
		{
			var randomIndex = Utils.Random(i, eggs.Length);
			var temp = eggs[i];
			eggs[i] = eggs[randomIndex];
			eggs[randomIndex] = temp;
		}
	}
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}