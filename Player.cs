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
        private int orderPlayer;
        private bool wasDownKeyPressed;
        
        private static bool player1KeyDown = false;
        private static bool player2KeyDown = false;
        
        // Timer
        private float coolDownTime = 4f;
        private float elapsedTime;
        private bool timerActive;

        // For the choice, 0 = left, 1 = right
        private int choice;

        private static readonly int[,] playerControls = new int[2, 5]
        {
            { Key.W, Key.S, Key.A, Key.D, Key.SPACE }, // Player 1: WASD
            { Key.UP, Key.DOWN, Key.LEFT, Key.RIGHT, Key.ENTER } // Player 2: Arrow keys
        };

        public Action<int> UpdateScoreCallback;

        private bool BothPlayersDecided()
        {
            return player1KeyDown && player2KeyDown;
        }
        
        private void UpdateScore(int result) {
            if (result == 0) {
                score =+ 10;
            }
            else
            {
                score =- 10;
            }
        }
        public int GetScore()
        {
            return score;
        }
        
        public Player(int playerId, GUI gui, Eggs eggs, int orderPlayer, int choice) : base(1366, 768)
        {
            this.eggs = eggs;
            this.gui = gui;
            this.playerId = playerId;
            this.orderPlayer = orderPlayer;
            this.choice = choice;
            
            score = 0;
            
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
            // Console.WriteLine(!BothPlayersDecided());
            // Console.WriteLine("Left player: " + player1KeyDown);
            // Console.WriteLine("Right player: " + player2KeyDown);
            
            if (Input.GetKey(leftKey))
            {
                if (!BothPlayersDecided()) { // Only allow changing choice if decisions are not locked
                    gui.ChangeChoice(playerId, leftKey);
                    choice = 0;
                }
            }
            if (Input.GetKey(rightKey))
            {
                if (!BothPlayersDecided()) { // Only allow changing choice if decisions are not locked
                    gui.ChangeChoice(playerId, rightKey);
                    choice = 1;
                }
            }

            // ----- Down ----- S -----
            // Check if both players have made their decisions or if cooldown is active
            if (!BothPlayersDecided() && !timerActive)
            {
                // Allow decision making if player hasn't already made a decision
                if (!(playerId == 0 ? player1KeyDown : player2KeyDown))
                {
                    // Decision making (left or right key)
                    if (Input.GetKey(leftKey))
                    {
                        gui.ChangeChoice(playerId, leftKey);
                        choice = 0;
                    }
                    if (Input.GetKey(rightKey))
                    {
                        gui.ChangeChoice(playerId, rightKey);
                        choice = 1;
                    }

                    // Lock decision if downKey is pressed and it's a new press
                    if (isDownKeyPressed && !wasDownKeyPressed)
                    {
                        if (playerId == 0)
                        {
                            player1KeyDown = true;
                            gui.ShowResult(playerId, eggs.GetResult(orderPlayer));
                            orderPlayer += 2;
                        }
                        else if (playerId == 1)
                        {
                            player2KeyDown = true;
                            gui.ShowResult(playerId, eggs.GetResult(orderPlayer));
                            orderPlayer += 2;
                        }
                        gui.DecisionMade(playerId);
                    }
                }
            }
            else
            {
                // Cooldown logic
                if (BothPlayersDecided())
                {
                    timerActive = true;
                    elapsedTime += Time.deltaTime;

                    if (elapsedTime >= coolDownTime)
                    {
                        // Reset after cooldown
                        player1KeyDown = false;
                        player2KeyDown = false;
                        timerActive = false;
                        elapsedTime = 0; // Reset timer
                    }
                }
            }

            wasDownKeyPressed = isDownKeyPressed;
        }
    }
}