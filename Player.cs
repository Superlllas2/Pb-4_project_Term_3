using System;
using System.Drawing;

namespace GXPEngine
{
    public class Player : Canvas
    {
        private int score;
        private int playerId;
        private GUI gui;
        private Eggs eggs;
        private int orderPlayer1;
        private int orderPlayer2;
        private bool wasDownKeyPressed = false;

        private static readonly int[,] playerControls = new int[2, 5]
        {
            { Key.W, Key.S, Key.A, Key.D, Key.SPACE }, // Player 1: WASD
            { Key.UP, Key.DOWN, Key.LEFT, Key.RIGHT, Key.ENTER } // Player 2: Arrow keys
        };

        public Action<int> UpdateScoreCallback;

        public int GetScore()
        {
            return score;
        }
        
        public void UpdateScore(int result) {
            if (result == 0) {
                score++;
            }
            else
            {
                score--;
            }
        }
        
        public Player(int playerId, GUI gui, Eggs eggs) : base(1300, 800)
        {
            this.eggs = eggs;
            this.gui = gui;
            this.playerId = playerId;
            score = 0;
            orderPlayer1 = 0;
            orderPlayer2 = 1;
            
            UpdateScoreCallback = (result) => UpdateScore(result);
        }
        
        

        void Update()
        {
            Controls();
        }

        void Controls()
        {
            // Get the control set based on playerId
            var upKey = playerControls[playerId, 0];
            var downKey = playerControls[playerId, 1];
            var leftKey = playerControls[playerId, 2];
            var rightKey = playerControls[playerId, 3];
            
            bool isDownKeyPressed = Input.GetKey(downKey);
            
            if (Input.GetKey(leftKey))
            {
                gui.ChangeChoice(playerId, leftKey);
            }
            if (Input.GetKey(rightKey))
            {
                gui.ChangeChoice(playerId, rightKey);
            }

            if (Input.GetKey(downKey))
            {
                if (isDownKeyPressed && !wasDownKeyPressed)
                {
                    gui.DecisionMade(playerId);
                    gui.ShowResult(playerId, eggs.GetResult(orderPlayer1));
                    orderPlayer1 += 2;
                    orderPlayer2 += 2;
                }
            }
            wasDownKeyPressed = isDownKeyPressed;
        }
    }
}