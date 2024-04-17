using System;
using GXPEngine;
using System.Drawing;

public class MyGameNew : Game
{	

	static void Main() {
		new MyGameNew().Start();
	}

	Ball _ball;

	EasyDraw _text;

	public MyGameNew () : base(800, 600, false,false)
	{
		_ball = new Ball (30, new Vec2 (width / 2, height / 2));
		AddChild (_ball);

		_text = new EasyDraw (200,25);
		_text.TextAlign (CenterMode.Min, CenterMode.Min);
		AddChild (_text);
	}

	void Update () {
		_ball.Step ();

		_text.Clear (Color.Transparent);
		_text.Text("Velocity: "+_ball.velocity, 0, 0);
	}
}

