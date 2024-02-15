using System;
using System.Runtime.InteropServices;

namespace GXPEngine
{
    public class GUI : Canvas
    {
        private Sprite eggRack;
        private Sprite playerChoice1;
        private Sprite playerChoice2;
        public GUI() : base(1300, 800)
        {
            eggRack = new Canvas("eggRack.png", false);
            AddChild(eggRack);
            eggRack.SetOrigin(eggRack.width/2, 0);
            eggRack.SetXY(650, 60);

            playerChoice1 = new Canvas("circle.png", false);
            playerChoice1.scale = 0.05f;
            playerChoice2 = new Canvas("circle.png", false);
            playerChoice2.scale = 0.05f;
            AddChild(playerChoice1);
            AddChild(playerChoice2);
            playerChoice1.SetXY(400, 300);
            playerChoice2.SetXY(840, 400);
        }

        public void changeChoice(int playerId, int key)
        {
            int x = (key == Key.D || key == Key.RIGHT) ? 840 : 400;

            if (playerId == 0) {
                playerChoice1.SetXY(x, 300);
            } else if (playerId == 1) {
                playerChoice2.SetXY(x, 400);
            }
        }

        public void decisionMade(int playerId)
        {
            if (playerId == 0)
            {
                playerChoice1.visible = false;
            } else if (playerId == 1)
            {
                playerChoice2.visible = false;
            }
        }
    }
}