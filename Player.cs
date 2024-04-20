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

        // Controls
        private bool ControlsEnabled;

        private bool wasDownKeyPressed;

        private static bool player1Desided;
        private static bool player2Desided;
        private static bool player1KeyDown;
        private static bool player2KeyDown;

        // Animations
        private AnimationSprite rightPanHimselfBad;
        private AnimationSprite rightPanOtherBad;
        private AnimationSprite leftPanHimselfBad;
        private AnimationSprite leftPanOtherBad;
        private AnimationSprite rightPanHimselfGood;
        private AnimationSprite rightPanOtherGood;
        private AnimationSprite leftPanHimselfGood;
        private AnimationSprite leftPanOtherGood;
        private AnimationSprite badEgg;
        private int animationFrame;
        private float animationTimer;
        private Dictionary<string, AnimationSprite> animations = new Dictionary<string, AnimationSprite>();
        private Dictionary<string, bool> animationStates = new Dictionary<string, bool>();
        private Dictionary<string, float> animationTimers = new Dictionary<string, float>();
        private Dictionary<string, List<GameObject>> animationObjectMap = new Dictionary<string, List<GameObject>>();
        private const int ANIMATION_FRAME_DURATION = 20; // Duration for all animations

        // Arduino controller
        private Gyroscope gyroscope;
        private bool wasGyroLeftActionActive;
        private bool wasGyroUpActionActive;
        private bool wasGyroRightActionActive;
        private bool isGyroLeftActive;
        private bool isGyroUpActive;
        private bool isGyroRightActive;

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

            

            // ----- Animation setup -----
            rightPanHimselfBad =
                new AnimationSprite("DynamicAnimations/rightPanHimselfBad.png", 19, 1, -1, false, false);
            rightPanHimselfBad.scale = 2f;
            rightPanHimselfBad.visible = false;
            AddAnimation("Player2Himself", rightPanHimselfBad);
            animationObjectMap.Add("Player2Himself", new List<GameObject> { gui.rightPan });
            AddChild(rightPanHimselfBad);

            rightPanOtherBad = new AnimationSprite("DynamicAnimations/rightPanOtherBad.png", 19, 1, -1, false, false);
            rightPanOtherBad.scale = 2f;
            rightPanOtherBad.visible = false;
            AddAnimation("Player2Other", rightPanOtherBad);
            animationObjectMap.Add("Player2Other", new List<GameObject> { gui.rightPan });
            AddChild(rightPanOtherBad);

            leftPanHimselfBad =
                new AnimationSprite("DynamicAnimations/leftPanHimselfBad.png", 19, 1, -1, false, false);
            leftPanHimselfBad.scale = 2f;
            leftPanHimselfBad.visible = false;
            AddAnimation("Player1Himself", leftPanHimselfBad);
            animationObjectMap.Add("Player1Himself", new List<GameObject> { gui.leftPan });
            AddChild(leftPanHimselfBad);

            leftPanOtherBad = new AnimationSprite("DynamicAnimations/leftPanOtherBad.png", 19, 1, -1, false, false);
            leftPanOtherBad.scale = 2f;
            leftPanOtherBad.visible = false;
            AddAnimation("Player1Other", leftPanOtherBad);
            animationObjectMap.Add("Player1Other", new List<GameObject> { gui.leftPan });
            AddChild(leftPanOtherBad);

            rightPanHimselfGood =
                new AnimationSprite("DynamicAnimations/rightPanHimselfGood.png", 11, 1, -1, false, false);
            rightPanHimselfGood.scale = 2f;
            rightPanHimselfGood.visible = false;
            AddAnimation("Player2HimselfGood", rightPanHimselfGood);
            animationObjectMap.Add("Player2HimselfGood", new List<GameObject> { gui.rightPan });
            AddChild(rightPanHimselfGood);

            rightPanOtherGood =
                new AnimationSprite("DynamicAnimations/rightPanOtherGood.png", 11, 1, -1, false, false);
            rightPanOtherGood.scale = 2f;
            rightPanOtherGood.visible = false;
            AddAnimation("Player2OtherGood", rightPanOtherGood);
            animationObjectMap.Add("Player2OtherGood", new List<GameObject> { gui.rightPan });
            AddChild(rightPanOtherGood);

            leftPanHimselfGood =
                new AnimationSprite("DynamicAnimations/leftPanHimselfGood.png", 11, 1, -1, false, false);
            leftPanHimselfGood.scale = 2f;
            leftPanHimselfGood.visible = false;
            AddAnimation("Player1HimselfGood", leftPanHimselfGood);
            animationObjectMap.Add("Player1HimselfGood", new List<GameObject> { gui.leftPan });
            AddChild(leftPanHimselfGood);

            leftPanOtherGood = new AnimationSprite("DynamicAnimations/leftPanOtherGood.png", 11, 1, -1, false, false);
            leftPanOtherGood.scale = 2f;
            leftPanOtherGood.visible = false;
            AddAnimation("Player1OtherGood", leftPanOtherGood);
            animationObjectMap.Add("Player1OtherGood", new List<GameObject> { gui.leftPan });
            AddChild(leftPanOtherGood);
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
                        gui.ChangeOtherPlayerScore(0, eggs.GetResult(orderPlayer));
                        if (eggs.GetResult(orderPlayer) == 0)
                        {
                            StartAnimation("Player2OtherGood");
                        }
                        else
                        {
                            StartAnimation("Player2Other");
                        }

                        player2Desided = true;
                        gui.eggArray[gui.currentEgg].visible = false;
                        gui.eggArray[gui.currentEgg++].visible = true;
                        orderPlayer += 2;
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
                    gui.eggArray[gui.currentEgg++].visible = true;
                    gui.eggArray[gui.currentEgg].visible = false;
                    if (eggs.GetResult(orderPlayer) == 0)
                    {
                        StartAnimation("Player1HimselfGood");
                    }
                    else
                    {
                        StartAnimation("Player1Himself");
                    }

                    orderPlayer += 2;
                }
                else if (playerId == 1 && !player2Desided)
                {
                    ChangeScore(eggs.GetResult(orderPlayer));
                    player2Desided = true;
                    gui.eggArray[gui.currentEgg++].visible = true;
                    gui.eggArray[gui.currentEgg].visible = false;
                    if (eggs.GetResult(orderPlayer) == 0)
                    {
                        StartAnimation("Player2HimselfGood");
                    }
                    else
                    {
                        StartAnimation("Player2Himself");
                    }

                    orderPlayer += 2;
                }
            }

            // wasLeftKeyPressed = Input.GetKey(leftKey);

            // ------ Right ------
            // TODO: playerDecided doesnt correctly work here
            // TODO: animation of a bad and good outcomes are reversed
            if ((isGyroRightActive && !wasGyroRightActionActive) || Input.GetKey(rightKey))
            {
                if (!BothPlayersKeyDown())
                {
                    // Only allow changing choice if decisions are not locked
                    // gui.ChangeChoice(playerId, rightKey); LEGACY
                    choice = 1;
                    if (playerId == 0 && !player1Desided)
                    {
                        player1Desided = true;
                        gui.ChangeOtherPlayerScore(1, eggs.GetResult(orderPlayer));
                        gui.eggArray[gui.currentEgg++].visible = true;
                        gui.eggArray[gui.currentEgg].visible = false;
                        
                        if (eggs.GetResult(orderPlayer) == 0)
                        {
                            Console.WriteLine("We get here");
                            StartAnimation("Player1OtherGood");
                        }
                        else
                        {
                            StartAnimation("Player1Other");
                        }
                        orderPlayer += 2;
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