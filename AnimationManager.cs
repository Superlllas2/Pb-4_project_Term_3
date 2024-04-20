using System.Collections.Generic;

namespace GXPEngine
{
    public class AnimationManager
    {
        Dictionary<string, AnimationSprite> animations = new Dictionary<string, AnimationSprite>();
        Dictionary<string, bool> animationPlayed = new Dictionary<string, bool>();

        public void AddAnimation(string key, AnimationSprite animation)
        {
            animations[key] = animation;
            animationPlayed[key] = false; // Initially, animations haven't been played
        }

        public void PlayAnimation(string key, byte animationDelay)
        {
            if (!animationPlayed[key])
            {
                AnimationSprite animation = animations[key];
                animation.visible = true;
                animation.SetCycle(0, animation.frameCount, animationDelay, false);
                animationPlayed[key] = true; // Mark as played
            }
        }

        public void ResetAnimation(string key)
        {
            animationPlayed[key] = false;
            animations[key].visible = false; // Optionally hide the sprite
            animations[key].SetFrame(0); // Reset to the first frame for next playback
        }

        // public void ResetAllAnimations()
        // {
        //     foreach (var key in animationPlayed.Keys.ToList())
        //     {
        //         ResetAnimation(key);
        //     }
        // }
    }
}