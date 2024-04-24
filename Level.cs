using System;
using System.Collections.Generic;

namespace GXPEngine
{
    // "/dev/cu.usbmodem21401"
    // "/dev/cu.usbmodem21301"

    public class Level : GameObject
    {
        private Canvas test;
        private Canvas test2;
        private Canvas test3;
        private Canvas test4;

        private Canvas background;

        private AnimationSprite fire;

        private Player player1;
        private Player player2;
        private HUD hud;
        private GUI gui;
        private Eggs eggs;
        private Sound gameSoundtrack;
        private SoundChannel gameSoundChannel;

        private List<Sprite> health1;
        private List<Sprite> health2;
        private int currentScore1;
        private int currentScore2;

        public Level()
        {
            health1 = new List<Sprite>();
            health1.Add(new Sprite("Health/health_0.png"));
            health1.Add(new Sprite("Health/health_1.png"));
            health1.Add(new Sprite("Health/health_2.png"));
            health1.Add(new Sprite("Health/health_3.png"));
            health1.Add(new Sprite("Health/health_4.png"));
            health1.Add(new Sprite("Health/health_5.png"));
            health1.Add(new Sprite("Health/health_6.png"));
            health1.Add(new Sprite("Health/health_7.png"));
            health1.Add(new Sprite("Health/health_8.png"));
            health1.Add(new Sprite("Health/health_9.png"));
            health1.Add(new Sprite("Health/health_10.png"));

            health2 = new List<Sprite>();
            health2.Add(new Sprite("Health/health_0.png"));
            health2.Add(new Sprite("Health/health_1.png"));
            health2.Add(new Sprite("Health/health_2.png"));
            health2.Add(new Sprite("Health/health_3.png"));
            health2.Add(new Sprite("Health/health_4.png"));
            health2.Add(new Sprite("Health/health_5.png"));
            health2.Add(new Sprite("Health/health_6.png"));
            health2.Add(new Sprite("Health/health_7.png"));
            health2.Add(new Sprite("Health/health_8.png"));
            health2.Add(new Sprite("Health/health_9.png"));
            health2.Add(new Sprite("Health/health_10.png"));

            gameSoundtrack = new Sound("Music/game.wav");
            gameSoundChannel = gameSoundtrack.Play();

            background = new Canvas("Background.png", false);
            background.scale = 2f;
            AddChild(background);

            fire = new AnimationSprite("StaticAnimations/fire.png", 8, 1, -1, true, false);
            fire.scale = 2f;
            AddChild(fire);

            eggs = new Eggs();

            gui = new GUI();
            AddChild(gui);

            player1 = new Player(0, gui, eggs, 0, 0, "/dev/cu.usbmodem21401");
            player2 = new Player(1, gui, eggs, 1, 1, "/dev/cu.usbmodem21301");
            gui.SetScoreUpdateCallback(player1.choice, player1.UpdateScoreCallback);
            gui.SetScoreUpdateCallback(player2.choice, player2.UpdateScoreCallback);
            AddChild(player1);
            AddChild(player2);

            foreach (Sprite sprite in health1)
            {
                sprite.visible = false;
                sprite.scale = 2f;
                sprite.SetXY(-540, -220);
                AddChild(sprite);
            }

            foreach (Sprite sprite in health2)
            {
                sprite.visible = false;
                sprite.scale = 2f;
                sprite.SetXY(550, -220);
                AddChild(sprite);
            }

            health1[5].visible = true;
            health2[5].visible = true;

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

            int newScore1 = player1.GetScore();
            int newScore2 = player2.GetScore();
            Console.WriteLine($"newScore: {newScore1}, oldScore: {currentScore1}");
            Console.WriteLine($"newScore: {newScore2}, oldScore: {currentScore2}");
            if (newScore1 != currentScore1)
            {
                Console.WriteLine("we get here P1");
                health1[currentScore1].visible = false;
                currentScore1 = newScore1;
                health1[currentScore1].visible = true;
            }

            if (newScore2 != currentScore2)
            {
                Console.WriteLine("we get here P2");
                health2[currentScore2].visible = false;
                currentScore2 = newScore2;
                health2[currentScore2].visible = true;
            }

            // TODO: increase the number below for a gameplay
            if (gui.currentEgg == 5)
            {
                gui.currentEgg++;
                var menu = new Menu(player1.GetScore(), player2.GetScore());
                Clean();
                // (parent as Menu).DoSomething();
                AddChild(menu);
            }
        }

        private void Clean()
        {
            List<GameObject> children = GetChildren();
            foreach (GameObject child in children)
            {
                child.Destroy();
            }


            GC.Collect();
        }
    }
}