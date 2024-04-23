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
        
        // Menu
        private List<Sprite> menuSprites;
        private int currentPictureIndex;
        private bool buttonPressed;
        private float timeSinceLastButtonPress = 0;
        private float debounceDelay = 0.5f;
        private bool wasTopButtonPressed;
        private bool wasAnyButtonPressed;

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
                AddChild(sprite);
            }

            p1 = new Gyroscope("/dev/cu.usbmodem21301", 57600);
            p1.Button1 += DoSomethingOnButton1;
            p1.Button2 += DoSomethingOnButton2;
        }

        public Menu(bool isPlayer1, bool isDraw)
        {
            this.isPlayer1 = isPlayer1;
            this.isDraw = isDraw;
            if (isDraw)
            {
                
            }
            else
            {
                if (isPlayer1)
                {
                    // code for player 1
                }
                else
                {
                    // code for player 2
                }
            }
        }

        void Update()
        {
            if (buttonPressed) {
                timeSinceLastButtonPress += Time.deltaTime/1000f;
                Console.WriteLine(timeSinceLastButtonPress);

                // Reset button pressed flag after the debounce delay has passed and no new press has occurred
                if (timeSinceLastButtonPress >= debounceDelay) {
                    buttonPressed = false;
                    timeSinceLastButtonPress = 0; // Reset the timer after the action is allowed
                }
            }

            if (timeSinceLastButtonPress >= debounceDelay)
            {
                buttonPressed = false;
            }
            
            test.SetCycle(0, 8, 3);
            test.AnimateFixed();

            p1.SerialPort_DataReceived();
        }

        void PictureListing()
        {
            if (currentPictureIndex < menuSprites.Count)
            {
                menuSprites[currentPictureIndex].visible = true;
            }
        }

        void DoSomethingOnButton1()
        {
            if (!buttonPressed)
            {
                wasAnyButtonPressed = true;
                buttonPressed = true;
                if (!wasTopButtonPressed)
                {
                    test.LateDestroy();
                    menuSprites[0].visible = true;
                    wasTopButtonPressed = true;
                }
                else
                {
                    if (currentPictureIndex <= menuSprites.Count)
                    {
                        PictureListing();
                        currentPictureIndex++;
                    }
                    else
                    {
                        StartGame();
                    }
                }
            }
        }

        void DoSomethingOnButton2()
        {
            if (!buttonPressed)
            {
                if (!wasAnyButtonPressed)
                {
                    Game.main.LateDestroy();
                }
                
                buttonPressed = true;
                
                if (currentPictureIndex >= 0)
                {
                    menuSprites[currentPictureIndex].visible = false;
                    PictureListing();
                    currentPictureIndex--;
                }
            }
        }

        private void StartGame()
        {
            if (hasStarted == false)
            {
                RemoveAll();
                soundChannel.Stop();
                Level level = new Level();
                AddChild(level);
                hasStarted = true;
            }
        }

        private void RemoveAll()
        {
            List<GameObject> children = GetChildren();
            foreach (GameObject child in children)
            {
                child.Destroy();
            }
        }
    }
}