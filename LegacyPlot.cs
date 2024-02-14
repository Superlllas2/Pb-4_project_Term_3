using System;

namespace GXPEngine
{
    public class LegacyPlot : Canvas
    {
        private float plotSpeed = 5f;
        private EasyDraw plot;

        //--Basic parameters--
        private int plotWidth = 350;
        private int plotHeight = 40;
        //--------------------

        private LegacyPlayer _legacyPlayer;

        public void SetPlayer(LegacyPlayer legacyPlayer)
        {
            this._legacyPlayer = legacyPlayer;
        }
        
        public LegacyPlot() : base(10, 10)
        {
            RespawnPlot();
            plot = new EasyDraw(plotWidth, plotHeight, true);
            AddChild(plot);
            plot.Rect(0,0, plotWidth, plotHeight);
            plot.Fill(255,255,255);
        }

        void Update()
        {
            plot.visible = true;
            Controls();
        }

        void Controls()
        {
            // playerSpeed = 3f;
            if (Input.GetKey(Key.RIGHT))
            {
                x += plotSpeed;
            }

            if (Input.GetKey(Key.LEFT))
            {
                x -= plotSpeed;
            }
        }
        
        void RespawnPlot()
        {
            // Set Up initial position
            x = game.width/2;
            y = game.height - (int)(game.height * 0.2);
        }
        
    }
}