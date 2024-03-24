using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Drawing.Drawing2D; // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{

	private MyGame() : base(1366, 768, false, pPixelArt:true)
	{
		Menu menu = new Menu();
		AddChild(menu);
		
		
		// Level level = new Level();
		// AddChild(level);


		// --TEST PURPOSES--
		// test4 = new Canvas("Eggbox/eggbox1.png", false);
		// test4.scale = 2f;
		// test4.SetXY(0, 400);
		// AddChild(test4);
		// test = new Canvas("Test_middle.png", false);
		// test.scale = 2f;
		// test.SetXY(200, 400);
		// AddChild(test);
		// test2 = new Canvas("Test_high.png", false);
		// test2.SetXY(400, 400);
		// AddChild(test2);
		// test2.scale = 2f;
		// test3 = new Canvas("test3.png", false);
		// AddChild(test3);

		// -----------------

		// -- TO SEE WHAT ARE THE EGGS --
		// foreach (int i in eggs)
		// {
		// 	Console.WriteLine(i);	
		// }
		// -------------------------------
	}

	void Update()
	{
		
	}
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}