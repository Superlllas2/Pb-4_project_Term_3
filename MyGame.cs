using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Drawing.Drawing2D; // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
	private Canvas test;
	private Canvas test2;
	private Canvas test3;
	private Canvas test4;

	private Canvas background;

	private AnimationSprite fire;
	
	private Player player1;
	private Player player2;
	private EasyDraw leftSide;
	private EasyDraw rightSide;
	private HUD hud;
	private GUI gui;
	private Eggs eggs;
	private Sound soundtrack;
	private SoundChannel soundChannel;

	private MyGame() : base(1366, 768, false, pPixelArt:true)
	{
		soundtrack = new Sound("assets/soundtrack.mp3");
		soundChannel = soundtrack.Play();
		
		Menu menu = new Menu();
		AddChild(menu);
		
		background = new Canvas("Background.png", false);
		background.scale = 2f;
		AddChild(background);

		fire = new AnimationSprite("StaticAnimations/fire.png", 8, 1, -1, true, false);
		fire.scale = 2f;
		AddChild(fire);
		
		eggs = new Eggs();
		// AddChild(eggs);
		
		gui = new GUI();
		AddChild(gui);
		// "/dev/cu.usbmodem21401"
		// "/dev/cu.usbmodem21301"
		player1 = new Player(0, gui, eggs, 0,0, "/dev/cu.usbmodem21401");
		player2 = new Player(1, gui, eggs, 1,1, "/dev/cu.usbmodem21301");
		gui.SetScoreUpdateCallback(player1.choice, player1.UpdateScoreCallback);
		gui.SetScoreUpdateCallback(player2.choice, player2.UpdateScoreCallback);
		AddChild(player1);
		AddChild(player2);

		hud = new HUD(player1, player2, eggs.GetNumOnes());
		AddChild(hud);
		
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
		fire.Animate(10 * Time.deltaTime / 1000f);
		
	}
	
	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}