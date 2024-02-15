using System;
using System.Drawing;

namespace GXPEngine
{
    public class Player : Canvas
    {
        private int score;
        private int playerId;
        private GUI gui;

        private static readonly int[,] playerControls = new int[2, 5]
        {
            { Key.W, Key.S, Key.A, Key.D, Key.SPACE }, // Player 1: WASD
            { Key.UP, Key.DOWN, Key.LEFT, Key.RIGHT, Key.ENTER } // Player 2: Arrow keys
        };


        public int GetScore()
        {
            return score;
        }
        
        public Player(int playerId, GUI gui) : base(1300, 800)
        {
            this.gui = gui;
            this.playerId = playerId;
            score = 0;
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
            
            if (Input.GetKey(leftKey))
            {
                gui.changeChoice(playerId, leftKey);
            }
            if (Input.GetKey(rightKey))
            {
                gui.changeChoice(playerId, rightKey);
            }

            if (Input.GetKey(downKey))
            {
                gui.decisionMade(playerId);
            }
        }
    }
}