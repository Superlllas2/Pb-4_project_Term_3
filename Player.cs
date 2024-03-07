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
        public int choice; // possibly redundant
        // private bool wasGyroLeftActionActive = false;

        // Controls
        private bool ControlsEnabled;

        private bool wasDownKeyPressed;

        private static bool player1Desided;
        private static bool player2Desided;
        private static bool player1KeyDown;
        private static bool player2KeyDown;

        // Animations
        private AnimationSprite rightPanAgree;
        private AnimationSprite rightPanDeny;
        private AnimationSprite badEgg;
        private int animationFrame = 0;
        private float animationTimer = 0f;
        private Dictionary<string, AnimationSprite> animations = new Dictionary<string, AnimationSprite>();
        private Dictionary<string, bool> animationStates = new Dictionary<string, bool>();
        private Dictionary<string, float> animationTimers = new Dictionary<string, float>();
        private Dictionary<string, List<GameObject>> animationObjectMap = new Dictionary<string, List<GameObject>>();
        private const int ANIMATION_FRAME_DURATION = 5; // Duration for all animations

        // Arduino controller
        private Gyroscope gyroscope;
        private bool wasGyroLeftActionActive = false;
        private bool wasGyroUpActionActive = false;
        private bool isGyroLeftActive = false;
        private bool isGyroUpActive = false;

        // Test
        private float _animationFrameCounter;
        private int frameCount;

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

        public Player(int playerId, GUI gui, Eggs eggs, int orderPlayer, int choice, string port)
        {
            this.eggs = eggs;
            this.gui = gui;
            this.playerId = playerId;
            this.orderPlayer = orderPlayer;
            this.choice = choice;
            score = 0;
            ControlsEnabled = true;


            // ----- Setup animation -----
            rightPanAgree = new AnimationSprite("DynamicAnimations/rightPanAgree.png", 7, 1, -1, false, false);
            rightPanAgree.scale = 2f;
            rightPanAgree.visible = false;
            AddAnimation("Player2Himself", rightPanAgree);
            animationObjectMap.Add("Player2Himself", new List<GameObject> { gui.rightPan });
            AddChild(rightPanAgree);

            rightPanDeny = new AnimationSprite("DynamicAnimations/rightPanDeny.png", 7, 1, -1, false, false);
            rightPanDeny.scale = 2f;
            rightPanDeny.visible = false;
            AddAnimation("Player2Other", rightPanDeny);
            animationObjectMap.Add("Player2Other", new List<GameObject> { gui.rightPan });
            AddChild(rightPanDeny);

            badEgg = new AnimationSprite("DynamicAnimations/badEgg.png", 12, 1, -1, false, false);
            badEgg.scale = 3f;
            badEgg.visible = false;
            AddAnimation("badEgg", badEgg);
            AddChild(badEgg);
            // ---------------------------
            // Handle exception of not connected controller
            try
            {
                gyroscope = new Gyroscope(port, 57600);
            }
            catch
            {
                Console.WriteLine($"Exception: player {playerId} controller is not connected or the port is wrong");
            }

            // The callback to communicate data received from the controller.
            // As Update method is not fast enough to receive,
            // I need to get results as soon as there it something to read
            // if (gyroscope != null)
            // {
            //     // gyroscope.OnDataReceived += HandleGyroData;
            // }

            // The callback to communicate with GUI
            UpdateScoreCallback = (result) => UpdateScore(result);
        }

        void Update()
        {
            gyroscope?.SerialPort_DataReceived();

            if (ControlsEnabled)
            {
                Controls();
            }

            if (BothPlayersDecided())
            {
                player1Desided = false;
                player2Desided = false;
            }


            foreach (var animationName in animations.Keys)
            {
                UpdateAnimation(animationName);
            }
        }

        /// <summary>
        /// Starts the specified animation.
        /// </summary>
        /// <param name="animationName">The name of the animation.</param>
        void StartAnimation(string animationName)
        {
            if (!animationStates[animationName] && animations.ContainsKey(animationName))
            {
                // Hide the specific objects related to this animation
                if (animationObjectMap.ContainsKey(animationName))
                {
                    foreach (var obj in animationObjectMap[animationName])
                    {
                        obj.visible = false;
                    }
                }

                animations[animationName].visible = true;
                animationStates[animationName] = true;
                animationFrame = 0;
                animationTimer = 0f;
                animations[animationName].SetCycle(0, animations[animationName].frameCount);
                animations[animationName].SetFrame(animationFrame);
            }
        }

        /// <summary>
        /// Updates a specific animation frame for the player
        /// </summary>
        /// <param name="animationName">The name of the animation to update</param>
        void UpdateAnimation(string animationName)
        {
            animationTimer += 1;
            if (animationStates[animationName])
            {
                if (animationTimer >= ANIMATION_FRAME_DURATION)
                {
                    animationTimer = 0f; // Reset the timer for the next frame
                    animations[animationName].NextFrame(); // Move to the next frame
                    frameCount += 1;
                    if (frameCount == animations[animationName].frameCount)
                    {
                        // Assuming animation loops back to 0
                        // Animation is complete
                        frameCount = 0;
                        animations[animationName].visible = false; // Hide the animation sprite
                        animationStates[animationName] = false;

                        // Show the specific objects related to this animation
                        if (animationObjectMap.ContainsKey(animationName))
                        {
                            foreach (var obj in animationObjectMap[animationName])
                            {
                                obj.visible = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is called to add a new animation
        /// </summary>
        /// <param name="name">The name of the animation</param>
        /// <param name="animation">The AnimationSprite object representing the animation</param>
        private void AddAnimation(string name, AnimationSprite animation)
        {
            animations[name] = animation;
            animationStates[name] = false;
            animationTimers[name] = 0f;
        }

        private void Controls()
        {
            isGyroLeftActive = IsGyroLeftActionActive();
            isGyroUpActive = IsGyroUpActionActive();

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
                        orderPlayer += 2;
                        gui.ChangeOtherPlayerScore(0, eggs.GetResult(orderPlayer));
                        StartAnimation("Player2Other");
                        player2Desided = true;
                        gui.eggArray[gui.currentEgg].visible = false;
                        gui.eggArray[gui.currentEgg++].visible = true;
                    }
                }
            }

            // ------ Up ------
            if ((isGyroUpActive && !wasGyroUpActionActive) || Input.GetKey(upKey))
            {
                if (playerId == 0 && !player1Desided)
                {
                    player1Desided = true;
                    ChangeScore(eggs.GetResult(orderPlayer));
                    orderPlayer += 2;
                }
                else if (playerId == 1 && !player2Desided)
                {
                    orderPlayer += 2;
                    ChangeScore(eggs.GetResult(orderPlayer));
                    StartAnimation("Player2Himself");
                    player2Desided = true;
                    gui.eggArray[gui.currentEgg].visible = false;
                    gui.eggArray[gui.currentEgg++].visible = true;
                }
            }

            wasGyroLeftActionActive = isGyroLeftActive;
            wasGyroUpActionActive = isGyroUpActive;
            // wasLeftKeyPressed = Input.GetKey(leftKey);

            // ------ Right ------
            if (Input.GetKey(rightKey))
            {
                if (!BothPlayersKeyDown())
                {
                    // Only allow changing choice if decisions are not locked
                    // gui.ChangeChoice(playerId, rightKey); LEGACY
                    choice = 1;
                    if (playerId == 0 && !player1Desided)
                    {
                        gui.ChangeOtherPlayerScore(1, eggs.GetResult(orderPlayer));
                        gui.eggArray[gui.currentEgg].visible = false;
                        gui.eggArray[gui.currentEgg++].visible = true;
                        orderPlayer += 2;
                    }
                    else if (playerId == 1)
                    {
                    }
                }
            }
            // ---------------------

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
            return gyroscope?.roll < -25;
        }

        private bool IsGyroUpActionActive()
        {
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