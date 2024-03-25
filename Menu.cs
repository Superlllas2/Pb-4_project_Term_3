using GXPEngine.Managers;

namespace GXPEngine
{
    public class Menu : GameObject
    {
        private Button button;
        private bool hasStarted;
        private Sound soundtrack;
        private SoundChannel soundChannel;
        private AnimationSprite test;
        // private Player player1;
        // private Player player2;

        public Menu() : base()
        {
            soundtrack = new Sound("Music/menu.mp3");
            soundChannel = soundtrack.Play();
            hasStarted = false;

            test = new AnimationSprite("DynamicAnimations/MainMenu1.png", 8, 1, -1, false, false);
            test.scale = 2f;
            AddChild(test);
            
            // player1 = new Player(0, gui, eggs, 0,0, "/dev/cu.usbmodem21401");
            // player2 = new Player(1, gui, eggs, 1,1, "/dev/cu.usbmodem21301");

            // button = new Button();
            // button.width = game.width;
            // button.height = game.height;
            // AddChild(button);
        }

        private void Update()
        {
            test.SetCycle(0, 8, 3);
            test.AnimateFixed();
            if (Input.GetMouseButtonDown(0))
            {
                if (button.HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    StartGame();
                    HideMenu();
                }
            }
        }

        private void HideMenu()
        {
            button.visible = false;
        }

        private void StartGame()
        {
            if (hasStarted == false)
            {
                soundChannel.Stop();
                Level level = new Level();
                AddChild(level);
                hasStarted = true;
            }
        }
    }
}