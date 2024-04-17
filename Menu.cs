namespace GXPEngine
{
    public class Menu : GameObject
    {
        public Menu ()
        {
            Button button = new Button();
            button.width = game.width;
            button.height = game.height;
            AddChild(button);
        }
    }
}