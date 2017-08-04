using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace Codigo.GUI
{
    /**
     * <summary>
     * This class counts the number of GUI elements the mouse is on, so we can detect the GUI interface and not acting un tiles below that
     * </summary>
     */
    [DataContract]
    public class MouseEnterBehavior : Behavior
    {
        /**
         * <summary>
         * The number of elements the mouse is currently over.
         * Remember to reset this each time!!
         * </summary>
         */
        public static int numHover = 0;

        private bool lastHover = false;

        private Transform2D transform;

        protected override void Initialize()
        {
            // Mouse Over Button
            lastHover = false;
        }
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            transform = Owner.FindComponent<Transform2D>();
        }
        protected override void Update(TimeSpan gameTime)
        {
            MouseState mouse = WaveServices.Input.MouseState;
            if (mouse.IsConnected)
            {
                var pos = mouse.Position;

                bool hover = Owner.IsVisible && transform.Rectangle.Contains(pos) && pos != Vector2.Zero;
                // Mouse Over Button
                if (hover && !lastHover)
                {
                    // hover
                    numHover++;
                }
                else if (!hover && lastHover)
                {
                    // stop hovering
                    numHover--;
                }
                lastHover = hover;
            }
        }
    }
}
