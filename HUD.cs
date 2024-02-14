using System;
using System.Drawing;

namespace GXPEngine
{
    public class HUD : Canvas
    {

        private Font font = new Font(SystemFonts.DefaultFont.FontFamily, 24, FontStyle.Regular);
        private Brush textColor = Brushes.Beige;
        
        private string textTimer = "Timer: ";
        // TODO: Make score dynamic and not a static value
        private string textScore1 = "5";
        private string textScore2 = "3";
        private SizeF textSize;

        private int totalTime = 50;

        // private float
        
        public HUD() : base(1300, 800)
        {
            Console.WriteLine(textSize.Width);
        }
        
        // Dynamically changes depending on text, so in case if text changes, it still looks good
        float GetTextSize(String text, String side)
        {
            textSize = graphics.MeasureString(text, font);
            return side == "height" ? textSize.Height : textSize.Width;
        }
        
        void Update()
        {
            graphics.Clear(Color.Empty);
            graphics.DrawString(textTimer + (totalTime - Time.time/1000), font, textColor, 0, game.height - GetTextSize(textTimer, "height"));
            graphics.DrawString(textScore1, font, textColor, 0, 0);
            graphics.DrawString(textScore2, font, textColor, game.width - GetTextSize(textScore2, "width"), 0);
        }
    }
}