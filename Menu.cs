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
        
        public Menu () : base()
        {
            soundtrack = new Sound("Music/menu.mp3");
            soundChannel = soundtrack.Play();
            hasStarted = false;
            button = new Button();
            button.width = game.width;
            button.height = game.height;
            AddChild(button);
        }

        private void Update()
        {
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