using System;
using System.Drawing;

namespace GXPEngine
{
    public class HUD : Canvas
    {

        private Font font = new Font(SystemFonts.DefaultFont.FontFamily, 24, FontStyle.Regular);
        private Brush textColor = Brushes.Beige;
        
        private string textTimer = "Timer: ";
        private string textFailureEggs = "Today we have this number of bad eggs: ";
        private SizeF textSize;

        private int totalTime = 50;

        private Player player1;
        private Player player2;

        private int numOnes;
        
        public HUD(Player player1, Player player2, int numOnes) : base(1366, 768)
        {
            this.numOnes = numOnes;
            this.player1 = player1;
            this.player2 = player2;
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
            graphics.DrawString(numOnes.ToString(), font, textColor, (game.width - GetTextSize(numOnes.ToString(), "width")) / 2, 100);
            graphics.DrawString(textTimer + (totalTime - Time.time/1000), font, textColor, 0, game.height - GetTextSize(textTimer, "height"));
            graphics.DrawString(player1.GetScore().ToString(), font, textColor, 0, 0);
            graphics.DrawString(player2.GetScore().ToString(), font, textColor, game.width - GetTextSize(player2.GetScore().ToString(), "width") , 0);
        }
    }
}