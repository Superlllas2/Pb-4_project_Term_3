using System;

namespace GXPEngine
{
    public class GUI : Canvas
    {
        private Sprite eggRack;
        private Sprite playerChoice1;
        private Sprite playerChoice2;
        private Sprite resultGoodSprite1;
        private Sprite resultGoodSprite2;
        private Sprite resultBadSprite1;
        private Sprite resultBadSprite2;
        private Player player;

        private float duration = 2.5f;
        private float elapsedTime;
        
        private Action<int> player1ScoreUpdateCallback;
        private Action<int> player2ScoreUpdateCallback;
        
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

            resultGoodSprite1 = new Sprite("successEgg.png", true, false);
            resultGoodSprite1.scale = 0.14f;
            resultGoodSprite2 = new Sprite("successEgg.png", true, false);
            resultGoodSprite1.scale = 0.14f;
            resultGoodSprite2.scale = 0.14f;
            resultGoodSprite1.SetXY(350, 530);
            resultGoodSprite2.SetXY(810, 530);
            resultGoodSprite1.visible = false;
            resultGoodSprite2.visible = false;
            resultBadSprite1 = new Sprite("failureEgg.png", true, false);
            resultBadSprite1.scale = 0.1f;
            resultBadSprite2 = new Sprite("failureEgg.png", true, false);
            resultBadSprite1.scale = 0.1f;
            resultBadSprite2.scale = 0.1f;
            resultBadSprite1.SetXY(380, 500);
            resultBadSprite2.SetXY(840, 500);
            resultBadSprite1.visible = false;
            resultBadSprite2.visible = false;
            AddChild(resultGoodSprite1);
            AddChild(resultGoodSprite2);
            AddChild(resultBadSprite1);
            AddChild(resultBadSprite2);
        }
        
        public void SetScoreUpdateCallback(int playerId, Action<int> callback) {
            if (playerId == 0) {
                player1ScoreUpdateCallback = callback;
            } else if (playerId == 1) {
                player2ScoreUpdateCallback = callback;
            }
        }
        
        public void ChangeChoice(int playerId, int key)
        {
            int x = (key == Key.D || key == Key.RIGHT) ? 840 : 400;

            if (playerId == 0) {
                playerChoice1.SetXY(x, 300);
            } else if (playerId == 1) {
                playerChoice2.SetXY(x, 400);
            }
        }

        // Makes decision Sprite disappear when clicked down button 
        public void DecisionMade(int playerId)
        {
            if (playerId == 0)
            {
                playerChoice1.visible = false;
            } else if (playerId == 1)
            {
                playerChoice2.visible = false;
            }
        }

        // Makes result sprites visible when choice is made
        public void ShowResult(int playerId, int result)
        {
            if (playerId == 0)
            {
                if (result == 0)
                {
                    resultGoodSprite1.visible = true;
                    Console.WriteLine("GUI ShowResult; player = 0; result = 0");

                    player1ScoreUpdateCallback?.Invoke(result);
                }
                else
                {
                    resultBadSprite1.visible = true;
                    Console.WriteLine("GUI ShowResult; player = 0; result = 1");

                    player1ScoreUpdateCallback?.Invoke(result);
                }
            }
            else
            {
                if (result == 0)
                {
                    resultGoodSprite2.visible = true;
                    Console.WriteLine("GUI ShowResult; player = 1; result = 0");

                    player2ScoreUpdateCallback?.Invoke(result);
                }
                else
                {
                    resultBadSprite2.visible = true;
                    Console.WriteLine("GUI ShowResult; player = 1; result = 1");

                    player2ScoreUpdateCallback?.Invoke(result);
                }
            }
        }
    }
}