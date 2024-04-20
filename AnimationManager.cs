using System;
using System.Collections.Generic;

namespace GXPEngine
{
    public class AnimationManager : GameObject
    {
        private GUI gui;
        
        // Animations
        private int animationFrame;
        private float animationTimer;
        private int frameCount;
        private AnimationSprite rightPanHimselfBad;
        private AnimationSprite rightPanOtherBad;
        private AnimationSprite leftPanHimselfBad;
        private AnimationSprite leftPanOtherBad;
        private AnimationSprite rightPanHimselfGood;
        private AnimationSprite rightPanOtherGood;
        private AnimationSprite leftPanHimselfGood;
        private AnimationSprite leftPanOtherGood;
        private Dictionary<string, AnimationSprite> animations = new Dictionary<string, AnimationSprite>();
        private Dictionary<string, bool> animationStates = new Dictionary<string, bool>();
        private Dictionary<string, float> animationTimers = new Dictionary<string, float>();
        private Dictionary<string, List<GameObject>> animationObjectMap = new Dictionary<string, List<GameObject>>();
        private const int ANIMATION_FRAME_DURATION = 20; // Duration for all animations
        
        public AnimationManager(GUI gui)
        {
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
        }
        
        void Update()
        {
            foreach (var animationName in animations.Keys)
            {
                UpdateAnimation(animationName);
            }
        }

        public void StartAnimation(string animationName)
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
        
        private void AddAnimation(string name, AnimationSprite animation)
        {
            animations[name] = animation;
            animationStates[name] = false;
            animationTimers[name] = 0f;
        }
    }
}