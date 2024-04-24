using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using GXPEngine.Managers;

namespace GXPEngine
{
    public class Menu : GameObject
    {
        private Button button;
        private bool hasStarted;
        private Sound soundtrack;
        private SoundChannel soundChannel;
        private AnimationSprite test;

        private bool isPlayer1;
        private bool isDraw;

        private Gyroscope p1;
        private Gyroscope p2;
        private bool isPlaying;

        // Menu 1
        private List<Sprite> menuSprites;
        private int currentPictureIndex;
        private bool buttonPressed;
        private float timeSinceLastButtonPress;
        private float debounceDelay = 0.4f;
        private bool wasTopButtonPressed;
        private bool wasAnyButtonPressed;

        // Menu 2
        private Sprite win1;
        private Sprite win2;
        private Sprite draw;
        private bool isMenu2;

        public Menu()
        {
            soundtrack = new Sound("Music/menu.mp3");
            soundChannel = soundtrack.Play();
            hasStarted = false;

            test = new AnimationSprite("DynamicAnimations/MainMenu1.png", 8, 1, -1, false, false);
            test.scale = 2f;
            AddChild(test);

            menuSprites = new List<Sprite>();
            menuSprites.Add(new Sprite("endingScreen1Winner.png"));
            menuSprites.Add(new Sprite("endingScreen2Winner.png"));
            menuSprites.Add(new Sprite("endingScreenDraw.png"));

            foreach (Sprite sprite in menuSprites)
            {
                sprite.visible = false;
                sprite.scale = 2f;
                AddChild(sprite);
            }

            p1 = new Gyroscope("/dev/cu.usbmodem21401", 57600);
            p1.Button1 += DoSomethingOnButton1;
            p1.Button2 += DoSomethingOnButton2;
            AddChild(p1);

            isPlaying = true;
            isMenu2 = false;
        }

        public Menu(int scoreP1, int scoreP2)
        {
            buttonPressed = false;
            isPlaying = true;
            isMenu2 = true;
            hasStarted = false;

            p2 = new Gyroscope("/dev/cu.usbmodem21301", 57600);
            p2.Button1 += DoSomethingOnButton1;
            p2.Button2 += DoSomethingOnButton2;
            AddChild(p2);

            if (scoreP1 == scoreP2)
            {
                draw = new Sprite("endingScreenDraw.png");
                draw.scale = 2f;
                AddChild(draw);
            }
            else if (scoreP1 > scoreP2)
            {
                win1 = new Sprite("endingScreen1Winner.png");
                win1.scale = 2f;
                AddChild(win1);
            }
            else
            {
                win2 = new Sprite("endingScreen2Winner.png");
                win2.scale = 2f;
                AddChild(win2);
            }
        }

        void Update()
        {
            if (buttonPressed)
            {
                timeSinceLastButtonPress += Time.deltaTime / 1000f;

                if (timeSinceLastButtonPress >= debounceDelay)
                {
                    buttonPressed = false;
                    timeSinceLastButtonPress = 0;
                }
            }

            if (timeSinceLastButtonPress >= debounceDelay)
            {
                buttonPressed = false;
            }

            if (!isMenu2)
            {
                test.SetCycle(0, 8, 3);
                test.AnimateFixed();
            }


            if (isPlaying)
            {
                if (!isMenu2)
                {
                    p1.SerialPort_DataReceived();
                }
                else
                {
                    p2.SerialPort_DataReceived();
                }
            }
        }

        void PictureGoNext()
        {
            currentPictureIndex++;
            if (currentPictureIndex < menuSprites.Count)
            {
                menuSprites[currentPictureIndex].visible = true;
            }
        }

        void PictureGoPrevious()
        {
            if (currentPictureIndex > 0)
            {
                menuSprites[currentPictureIndex].visible = false;
                currentPictureIndex--;
            }
        }

        void DoSomethingOnButton1()
        {
            Console.WriteLine("hello b1");
            if (!buttonPressed)
            {
                wasAnyButtonPressed = true;
                buttonPressed = true;
                if (!isMenu2)
                {
                    if (!wasTopButtonPressed)
                    {
                        test.LateDestroy();
                        menuSprites[0].visible = true;
                        wasTopButtonPressed = true;
                    }
                    else
                    {
                        if (currentPictureIndex < menuSprites.Count - 1)
                        {
                            PictureGoNext();
                        }
                        else
                        {
                            StartGame();
                        }
                    }
                }
                else
                {
                    StartGame();
                }
            }
        }

        void DoSomethingOnButton2()
        {
            Console.WriteLine("hello b2");
            if (!buttonPressed)
            {
                if (!isMenu2)
                {
                    buttonPressed = true;
                
                    if (!wasAnyButtonPressed)
                    {
                        Game.main.LateDestroy();
                    }

                    if (currentPictureIndex > 0)
                    {
                        PictureGoPrevious();
                    }
                }
                else
                {
                    StartGame();
                }
            }
        }

        private void StartGame()
        {
            if (hasStarted == false)
            {
                if (!isMenu2)
                {
                    Clean();
                    soundChannel.Stop();
                    Level level = new Level();
                    AddChild(level);
                    hasStarted = true;
                }
                else
                {
                    Console.WriteLine("we get to StartGame else");
                    Clean();
                    Level level = new Level();
                    AddChild(level);
                    hasStarted = true;
                }
            }
        }


        public void DoSomething() {}
        private void Clean()
        {
            if (!isMenu2)
            {
                p1.Button1 -= DoSomethingOnButton1;
                p1.Button2 -= DoSomethingOnButton2;
                List<GameObject> children = GetChildren();
                isPlaying = false;
                foreach (GameObject child in children)
                {
                    child.Destroy();
                }
            }
            else
            {
                Console.WriteLine("we get to Clean");
                p2.Button1 -= DoSomethingOnButton1;
                p2.Button2 -= DoSomethingOnButton2;
                List<GameObject> children = GetChildren();
                isPlaying = false;
                foreach (GameObject child in children)
                {
                    child.Destroy();
                }
            }
        }
    }
}