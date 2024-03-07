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
        
        // Static sprites
        public Sprite rightEgg;
        public Sprite leftEgg;
        
        public Sprite leftPan;
        public Sprite rightPan;
        
        private Player player;
        
        private float resultSpriteVisibilityDuration = 4f;
        private float elapsedTimeSinceResultShown;
        private bool resultSpriteTimerActive;
        
        public Sprite[] eggArray = new Sprite[13];
        public int currentEgg;
        
        private Action<int> player1ScoreUpdateCallback;
        private Action<int> player2ScoreUpdateCallback;
        
        public GUI() : base(1366, 768)
        {
            for (int i = 0; i < 13; i++) {
                string imagePath = $"Eggbox/EggBox{i}.png";
                eggArray[i] = new Sprite(imagePath);
                eggArray[i].scale = 2f;
                eggArray[i].visible = false;
                AddChild(eggArray[i]);
            }
            
            eggArray[0].visible = true;
            
            
            leftPan = new Canvas("panLeftStatic.png", false);
            leftPan.scale = 2f;
            AddChild(leftPan);

            rightPan = new Canvas("panRightStatic.png", false);
            rightPan.scale = 2f;
            AddChild(rightPan);
            
            rightEgg = new Sprite("eggRightPlate.png", false, false);
            rightEgg.scale = 2f;
            rightEgg.visible = false;
            AddChild(rightPan);

            leftEgg = new Sprite("eggLeftPlate.png", false, false);
            leftEgg.scale = 2f;
            leftEgg.visible = false;
            AddChild(leftEgg);

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

        public void ChangeEggBox()
        {
            
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

        /// <summary>
        /// Changes the score of the other player.
        /// </summary>
        /// <param name="playerId">The ID of the player whose score will be updated.</param>
        /// <param name="result">The new score for the player.</param>
        public void ChangeOtherPlayerScore(int playerId, int result)
        {
            if (playerId == 0)
            {
                player1ScoreUpdateCallback?.Invoke(result);
            }
            else
            {
                player2ScoreUpdateCallback?.Invoke(result);
            }
        }

        // Makes result sprites visible when choice is made
        public void ShowResult(int choice, int result)
        {
            elapsedTimeSinceResultShown = 0f;
            resultSpriteTimerActive = true;
            if (choice == 0)
            {
                if (result == 0)
                {
                    resultGoodSprite1.visible = true;
                    player1ScoreUpdateCallback?.Invoke(result);
                }
                else
                {
                    resultBadSprite1.visible = true;
                    player1ScoreUpdateCallback?.Invoke(result);
                }
            }
            else
            {
                if (result == 0)
                {
                    resultGoodSprite2.visible = true;
                    player2ScoreUpdateCallback?.Invoke(result);
                }
                else
                {
                    resultBadSprite2.visible = true;
                    player2ScoreUpdateCallback?.Invoke(result);
                }
            }
        }
        
        void Update()
        {
            if (resultSpriteTimerActive)
            {
                elapsedTimeSinceResultShown += Time.deltaTime;
                if (elapsedTimeSinceResultShown/1000 >= resultSpriteVisibilityDuration)
                {
                    // Hide the result sprites
                    resultGoodSprite1.visible = false;
                    resultBadSprite1.visible = false;
                    resultGoodSprite2.visible = false;
                    resultBadSprite2.visible = false;

                    // Stop the timer
                    resultSpriteTimerActive = false;
                    
                    // Show choice sprites
                    playerChoice1.visible = true;
                    playerChoice2.visible = true;
                }
            }
        }
    }
}