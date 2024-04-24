using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace GXPEngine
{
    public class Player : GameObject
    {
        // Player
        private int score;
        private int playerId;
        private GUI gui;
        private Eggs eggs;
        private int orderPlayer;
        public int choice;

        // Controls
        private bool wasDownKeyPressed;
        private static bool player1Desided;
        private static bool player2Desided;
        private static bool player1KeyDown;
        private static bool player2KeyDown;
        
        // Animation
        private AnimationManager animationManager;

        // Arduino controller
        private Gyroscope gyroscope;
        private bool wasGyroLeftActionActive;
        private bool wasGyroUpActionActive;
        private bool wasGyroRightActionActive;
        private bool isGyroLeftActive;
        private bool isGyroUpActive;
        private bool isGyroRightActive;

        // Timer
        private float coolDownTime = 4f;
        private float elapsedTime;
        private bool timerActive;


        private static readonly int[,] playerControls = new int[2, 5]
        {
            { Key.W, Key.S, Key.A, Key.D, Key.SPACE }, // Player 1: WASD
            { Key.UP, Key.DOWN, Key.LEFT, Key.RIGHT, Key.ENTER } // Player 2: Arrow keys
        };

        public Action<int> UpdateScoreCallback;

        ~Player()
        {
            Console.WriteLine("Destroyed!");
        }
        
        public Player(int playerId, GUI gui, Eggs eggs, int orderPlayer, int choice, string port)
        {
            counter++;
            this.eggs = eggs;
            this.gui = gui;
            this.playerId = playerId;
            this.orderPlayer = orderPlayer;
            this.choice = choice;
            score = 0;
            animationManager = new AnimationManager(gui);
            AddChild(animationManager);
            
            // Handle exception of not connected controller
            try
            {
                gyroscope = new Gyroscope(port, 57600);
                // AddChild(gyroscope);
            }
            catch
            {
                Console.WriteLine($"Exception: player {playerId} controller is not connected or the port is wrong");
            }
            
            // The callback to communicate with GUI
            UpdateScoreCallback = (result) => UpdateScore(result);
        }
        
        public static int counter = 0;

        void Update()
        {
            Console.WriteLine("update id:" + counter);
            gyroscope?.SerialPort_DataReceived();
            
            GameControls();

            if (BothPlayersDecided())
            {
                player1Desided = false;
                player2Desided = false;
            }
        }

        private void GameControls()
        {
            isGyroLeftActive = IsGyroLeftActionActive();
            isGyroUpActive = IsGyroUpActionActive();
            isGyroRightActive = IsGyroRightActionActive();

            // Get the control set based on playerId
            var upKey = playerControls[playerId, 0];
            var downKey = playerControls[playerId, 1];
            var leftKey = playerControls[playerId, 2];
            var rightKey = playerControls[playerId, 3];

            bool isDownKeyPressed = Input.GetKey(downKey);
            // Console.WriteLine(!BothPlayersDecided());
            // Console.WriteLine("Left player: " + player1KeyDown);
            // Console.WriteLine("Right player: " + player2KeyDown);


            // ------ Left ------
            if ((isGyroLeftActive && !wasGyroLeftActionActive) || Input.GetKey(leftKey))
            {
                if (!BothPlayersKeyDown())
                {
                    // Only allow changing choice if decisions are not locked
                    // gui.ChangeChoice(playerId, leftKey); LEGACY
                    choice = 0;
                    if (playerId == 0)
                    {
                    }
                    else if (playerId == 1 && !player2Desided)
                    {
                        if (gui.currentEgg != 12)
                        {
                            gui.ChangeOtherPlayerScore(0, eggs.GetResult(orderPlayer));
                            if (eggs.GetResult(orderPlayer) == 0)
                            {
                                animationManager.StartAnimation("Player2OtherGood");
                            }
                            else
                            {
                                animationManager.StartAnimation("Player2Other");
                            }

                            player2Desided = true;
                            gui.eggArray[1 + gui.currentEgg++].visible = true;
                            // gui.eggArray[gui.currentEgg].visible = false;
                            orderPlayer += 2;
                        }
                        else
                        {
                            gui.currentEgg++;
                            Console.WriteLine("No more eggs left");
                        }
                    }
                }
            }

            // ------ Up ------
            if ((isGyroUpActive && !wasGyroUpActionActive) || Input.GetKey(upKey))
            {
                if (playerId == 0 && !player1Desided)
                {
                    if (gui.currentEgg != 12)
                    {
                        player1Desided = true;
                        ChangeScore(eggs.GetResult(orderPlayer));
                        gui.eggArray[1 + gui.currentEgg++].visible = true;
                        // gui.eggArray[gui.currentEgg].visible = false;
                        if (eggs.GetResult(orderPlayer) == 0)
                        {
                            animationManager.StartAnimation("Player1HimselfGood");
                        }
                        else
                        {
                            animationManager.StartAnimation("Player1Himself");
                        }
                        orderPlayer += 2; 
                    }
                    else
                    {
                        gui.currentEgg++;
                        Console.WriteLine("No more eggs left");
                    }
                }
                else if (playerId == 1 && !player2Desided)
                {
                    if (gui.currentEgg != 12)
                    {
                        ChangeScore(eggs.GetResult(orderPlayer));
                        player2Desided = true;
                        gui.eggArray[1 + gui.currentEgg++].visible = true;
                        // gui.eggArray[gui.currentEgg].visible = false;
                        if (eggs.GetResult(orderPlayer) == 0)
                        {
                            animationManager.StartAnimation("Player2HimselfGood");
                        }
                        else
                        {
                            animationManager.StartAnimation("Player2Himself");
                        }
                        orderPlayer += 2;   
                    }
                    else
                    {
                        gui.currentEgg++;
                        Console.WriteLine("No more eggs left");
                    }
                }
            }

            // wasLeftKeyPressed = Input.GetKey(leftKey);

            // ------ Right ------
            if ((isGyroRightActive && !wasGyroRightActionActive) || Input.GetKey(rightKey))
            {
                if (!BothPlayersKeyDown())
                {
                    // Only allow changing choice if decisions are not locked
                    // gui.ChangeChoice(playerId, rightKey); LEGACY
                    choice = 1;
                    if (playerId == 0 && !player1Desided)
                    {
                        if (gui.currentEgg != 12)
                        {
                            player1Desided = true;
                            gui.ChangeOtherPlayerScore(1, eggs.GetResult(orderPlayer));
                            gui.eggArray[1 + gui.currentEgg++].visible = true;
                            // gui.eggArray[gui.currentEgg].visible = false;
                        
                            if (eggs.GetResult(orderPlayer) == 0)
                            {
                                animationManager.StartAnimation("Player1OtherGood");
                            }
                            else
                            {
                                animationManager.StartAnimation("Player1Other");
                            }
                            orderPlayer += 2;
                        }
                        else
                        {
                            gui.currentEgg++;
                            Console.WriteLine("No more eggs left");
                        }
                    }
                    else if (playerId == 1)
                    {
                    }
                }
            }

            wasGyroLeftActionActive = isGyroLeftActive;
            wasGyroUpActionActive = isGyroUpActive;
            wasGyroRightActionActive = isGyroRightActive;

            // ----- Down ----- S -----
            // Check if both players have made their decisions or if cooldown is active
            if (!BothPlayersKeyDown() && !timerActive)
            {
                // Allow decision making if a player hasn't already made a decision
                if (!(playerId == 0 ? player1KeyDown : player2KeyDown))
                {
                    // Decision making (left or right key)
                    if (Input.GetKey(leftKey))
                    {
                        choice = 0;
                    }

                    if (Input.GetKey(rightKey))
                    {
                        choice = 1;
                    }

                    // Lock decision if downKey is pressed and it's a new press
                    if (isDownKeyPressed && !wasDownKeyPressed)
                    {
                        if (playerId == 0)
                        {
                            player1KeyDown = true;
                            gui.ShowResult(choice, eggs.GetResult(orderPlayer));
                            orderPlayer += 2;
                        }
                        else if (playerId == 1)
                        {
                            player2KeyDown = true;
                            gui.ShowResult(choice, eggs.GetResult(orderPlayer));
                            orderPlayer += 2;
                        }

                        gui.DecisionMade(playerId);
                    }
                }
            }
            else
            {
                // Cooldown logic
                if (BothPlayersKeyDown())
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
        
        private bool BothPlayersKeyDown()
        {
            return player1KeyDown && player2KeyDown;
        }

        private void UpdateScore(int result)
        {
            score += (result == 0) ? 10 : -10;
        }

        public int GetScore()
        {
            return score;
        }

        private bool IsGyroLeftActionActive()
        {
            if (gyroscope?.roll < -25)
            {
                // debug
            }
            return gyroscope?.roll < -25;
        }

        private bool IsGyroRightActionActive()
        {
            if (gyroscope?.roll > 25)
            {
                // debug
            }
            return gyroscope?.roll > 25;
        }

        private bool IsGyroUpActionActive()
        {
            if (gyroscope?.pitch < -15)
            {
                // debug
            }
            return gyroscope?.pitch < -15;
        }

        private void ChangeScore(int i)
        {
            // Egg = 0 is good | Egg = 1 is bad
            Console.WriteLine($"Changing score for Player {playerId}. Result: {i}");
            score += (i == 0) ? 10 : -10;
        }

        private bool BothPlayersDecided()
        {
            return player1Desided && player2Desided;
        }
    }
}