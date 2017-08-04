using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace Codigo
{
    public class KeysBehavior : Behavior
    {
        [RequiredComponent]
        private Camera2D cam;


        private KeyBindings keys;
        /**
         * <summary>Saved last update's mouse state</summary>
         */
        protected MouseState oldMouse;
        /**
        * <summary>Saved last update's keyboard state</summary>
        */
        protected KeyboardState oldKeyboard;

        /**<summary>
         * This frame's mouse state
         * </summary>
         */
        MouseState currentMouse;
        KeyboardState currentKeyboard;

        protected Dictionary<CommandEnum, bool> commandsExecuted;
        protected int zoom;

        public Vector2 currentMousePosition { get; private set; }

        //We save the current mouse and keyboard's states
        protected override void Initialize()
        {
            UpdateOrder = 0;
            currentMousePosition = Vector2.Zero;
            oldMouse = WaveServices.Input.MouseState;
            oldKeyboard = WaveServices.Input.KeyboardState;
            commandsExecuted = new Dictionary<CommandEnum, bool>();

            keys = KeyBindings.Restore();
           
            if(keys==null)
            {
                keys = new KeyBindings();
            }
            //At first, all enums will be false
            //Watch out with scene behaviors, if the are preupdate will be executed before this, causing an exception
            foreach (CommandEnum cmd in Enum.GetValues(typeof(CommandEnum)))
                commandsExecuted[cmd] = false;
        }

        //We calculate new pressed key states and then we save new keyboard and mouse's states
        protected override void Update(TimeSpan gameTime)
        {
            
            currentMouse = WaveServices.Input.MouseState;
            currentKeyboard = WaveServices.Input.KeyboardState;
            CalculateMousePosition();
            
            CommandByPressed(CommandEnum.CameraUp);
            CommandByPressed(CommandEnum.CameraRight);
            CommandByPressed(CommandEnum.CameraLeft);
            CommandByPressed(CommandEnum.CameraDown);
            CommandByDown(CommandEnum.CameraCenter);

            zoom = keys.WheelRotation(currentMouse) + keys.WheelRotation(currentKeyboard,currentMouse);

            CommandByDown(CommandEnum.StartPath);
            CommandByPressed(CommandEnum.AddTile);
            CommandByUp(CommandEnum.FinishPath);

            CommandByUp(CommandEnum.ActInTile);
            CommandByDown(CommandEnum.Select);

            CommandByDown(CommandEnum.Build);
            CommandByDown(CommandEnum.Train);
            CommandByDown(CommandEnum.Fight);
            CommandByDown(CommandEnum.Chop);
            CommandByDown(CommandEnum.Heal);

            CommandByDown(CommandEnum.Stop);
            CommandByDown(CommandEnum.Destroy);

            CommandByDown(CommandEnum.CottageOne);
            CommandByDown(CommandEnum.CottageTwo);
            CommandByDown(CommandEnum.CottageThree);
            CommandByDown(CommandEnum.CottageFour);
            CommandByDown(CommandEnum.CottageFive);
            CommandByDown(CommandEnum.CottageSix);

            CommandByDown(CommandEnum.StopCreation);
            CommandByDown(CommandEnum.Create);

            CommandByDown(CommandEnum.ToggleFog);

            CommandByDown(CommandEnum.Cottage);
            CommandByDown(CommandEnum.FakeTree);
            CommandByDown(CommandEnum.Candle);
            CommandByDown(CommandEnum.Bridge);
            CommandByDown(CommandEnum.Trap);

            CommandByDown(CommandEnum.Person);
            CommandByDown(CommandEnum.Water);
            CommandByDown(CommandEnum.Rock);
            CommandByDown(CommandEnum.Castle);
            CommandByDown(CommandEnum.Tree);


            oldMouse = currentMouse;
            oldKeyboard = currentKeyboard;
        }

        /**
         * <summary>
         * Calculates the position for the mouse this frame
         * </summary>
         */
        private void CalculateMousePosition()
        {
            Vector2 position = Vector2.Zero;
            Vector2 mouse = WaveServices.Input.MouseState.Position;
            cam.Unproject(ref mouse, out position);
            if (currentKeyboard.IsKeyPressed(Keys.A))
            {
                Trace.WriteLine(cam.UsedVirtualScreen.BottomEdge);
                Trace.WriteLine(cam.UsedVirtualScreen.LeftEdge);
                Trace.WriteLine(cam.UsedVirtualScreen.RightEdge);
                Trace.WriteLine(cam.UsedVirtualScreen.TopEdge);
                Trace.WriteLine(mouse);
                Trace.WriteLine(position);
            }
            currentMousePosition = position;
        }
        /**
         * <summary>
         * Updates whether the button for the command is pressed this frame
         * </summary>
         */
        private void CommandByPressed(CommandEnum cmd)
        { 
            commandsExecuted[cmd] = keys.CommandPressed(cmd, currentKeyboard,currentMouse);
        }
        /**
         * <summary>
         * Updates whether the button for the command is not pressed this frame
         * </summary>
         */
        private void CommandByRelease(CommandEnum cmd)
        {
            commandsExecuted[cmd] = keys.CommandReleased(cmd, currentKeyboard, currentMouse);
        }
        /**
         * <summary>
         * Updates whether the button for the command has been pressed down exactly this frame
         * </summary>
         */
        private void CommandByDown(CommandEnum cmd)
        {
            commandsExecuted[cmd] = keys.CommandDown(cmd, currentKeyboard,oldKeyboard, currentMouse, oldMouse);
        }
        /**
         * <summary>
         * Updates whether the button for the command has been released exactly this frame
         * </summary>
         */
        private void CommandByUp(CommandEnum cmd)
        {
            commandsExecuted[cmd] = keys.CommandUp(cmd, currentKeyboard,oldKeyboard,currentMouse,oldMouse);
        }
       
        public bool IsCommandExecuted(CommandEnum cmd)
        {
            return commandsExecuted[cmd];
        }

        public int Zoom()
        {
            return zoom;
        }
    }
}
