using System;

namespace GXPEngine
{
    public class LegacyPlayer : Canvas
    {
        private EasyDraw player;
        private bool isGrounded;

        //--Free fall physics--
        private float gravity = 9.81f;
        public float velocityY = 0;
        private float deltaTime;
        //---------------------
        
        private LegacyPlot _legacyPlot;
        
        public LegacyPlayer(LegacyPlot legacyPlot) : base(30, 60)
        {
            legacyPlot.SetPlayer(this);
            this._legacyPlot = legacyPlot;
            RespawnPlot();
            player = new EasyDraw(30, 60, true);
            AddChild(player);
            player.Rect(0, 0, 30, 60);
            player.Fill(255);
            
        }

        void Update()
        {
            // Free fall
            velocityY += gravity * Time.deltaTime/100;
            y += velocityY * Time.deltaTime/100;
            
            
            player.visible = true;
            
            // Check if it falls under the screen
            if (y > game.height)
            {
                RespawnPlot();
            }
        }
        
        void RespawnPlot()
        {
            // Set Up initial position
            x = Utils.Random(width, game.width-width);
            y = game.height - (int)(game.height * 0.9);
            velocityY = 0;
        }

        void OnCollision(GameObject other)
        {
            if (other is LegacyPlot) 
            {
                RespawnPlot();
            }
        }
    }
}