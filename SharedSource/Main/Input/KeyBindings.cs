using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Framework.Services;

namespace Codigo
{
    /**
        * <summary>
        * The file containing the keys combinations is saved at: C:\Users\{myuser}\AppData\Local\IsolatedStorage
        * </summary>
       */
    [DataContract]
    class KeyBindings
    {
        /**
         * <summary>
         * The file is saved at: C:\Users\{myuser}\AppData\Local\IsolatedStorage
         * </summary>
        */
        private static string filePath = "Keys.xml";

        [DataMember]
        /**
        * <summary>
        * The key combinations for each available command
        * </summary>
       */
        public Dictionary<CommandEnum, List<KeyCombination>> bindings { get; private set; }
        

        public bool CommandPressed(CommandEnum cmd, KeyboardState keyboard,MouseState mouse)
        {
            return bindings[cmd].Exists((b) => b.IsKeyPressed(keyboard,mouse));
        }
        public bool CommandReleased(CommandEnum cmd, KeyboardState keyboard, MouseState mouse)
        {
            return bindings[cmd].TrueForAll((b) => b.IsKeyReleased(keyboard, mouse));
        }
        public bool CommandDown(CommandEnum cmd, KeyboardState currentKeyboard,KeyboardState oldKeyboard,MouseState currentMouse,MouseState oldMouse)
        {
            return bindings[cmd].Exists((b) => b.IsKeyReleased(oldKeyboard, oldMouse) && b.IsKeyPressed(currentKeyboard, currentMouse));
        }
        public bool CommandUp(CommandEnum cmd, KeyboardState currentKeyboard, KeyboardState oldKeyboard, MouseState currentMouse, MouseState oldMouse)
        {
            return bindings[cmd].Exists((b) => b.IsKeyPressed(oldKeyboard, oldMouse) && b.IsKeyReleased(currentKeyboard, currentMouse));
        }
        /**
         * <summary>
         * Reads a key list from the storage 
         * </summary>
         */
        public static KeyBindings Restore()
        {
            KeyBindings res = null;
            Storage storageService = WaveServices.Storage;
            storageService.SetKnownTypes(new[] { typeof(KeyBindings) });

            if (storageService.Exists(filePath))
            {
                try
                {
                    res = storageService.Read<KeyBindings>(filePath);
                }
                finally 
                {
                   
                }
            }
            return res;
        }
        /**
         * <summary>
         * Saves a key list in the storage
         * </summary>
         */
        public void Save()
        {
            Storage storageService = WaveServices.Storage;
            storageService.SetKnownTypes(new[] { typeof(KeyBindings) });
           
            if (!storageService.ExistsStorageFile(filePath))
            {
                storageService.CreateStorageFile(filePath).Close();
            }
            storageService.Write<KeyBindings>(filePath, this);
        }

        public KeyBindings()
        {
            bindings = new Dictionary<CommandEnum, List<KeyCombination>>();
            //Camera keys
            AddSingleCombination(CommandEnum.CameraUp, Keys.Up);
            AddSingleCombination(CommandEnum.CameraDown, Keys.Down);
            AddSingleCombination(CommandEnum.CameraRight, Keys.Right);
            AddSingleCombination(CommandEnum.CameraLeft, Keys.Left);
            AddSingleCombination(
                 CommandEnum.CameraCenter,
                 Keys.Space
            );
            AddSingleCombination(
                CommandEnum.ZoomIn,
                Keys.Add
           );
            AddSingleCombination(
                CommandEnum.ZoomOut,
                Keys.Subtract
           );
            //Action keys
            AddSingleCombination(
                 CommandEnum.Build,
                 Keys.B
            );
            AddSingleCombination(
                CommandEnum.Train,
                Keys.T
           );
            AddSingleCombination(
                CommandEnum.Fight,
                Keys.F
           );
            AddSingleCombination(
                CommandEnum.Heal,
                Keys.H
           );
            AddSingleCombination(
                CommandEnum.Chop,
                Keys.C
           );

            AddSingleCombination(
                CommandEnum.Stop,
                Keys.S
           );
            AddSingleCombination(
                CommandEnum.Destroy,
                Keys.Delete
           );
            //Cottage keys
            AddSingleCombination(
                CommandEnum.CottageOne,
                Keys.Number1
           );
            AddSingleCombination(
                CommandEnum.CottageTwo,
                Keys.Number2
           );
            AddSingleCombination(
                CommandEnum.CottageThree,
                Keys.Number3
           );
            AddSingleCombination(
                CommandEnum.CottageFour,
                Keys.Number4
           );
            AddSingleCombination(
                CommandEnum.CottageFive,
                Keys.Number5
           );
            AddSingleCombination(
                CommandEnum.CottageSix,
                Keys.Number6
           );


            AddSingleCombination(CommandEnum.StartPath, MouseEnum.RightButton);
            AddSingleCombination(CommandEnum.AddTile, MouseEnum.RightButton);
            AddSingleCombination(CommandEnum.FinishPath, MouseEnum.RightButton);

            AddSingleCombination(CommandEnum.ActInTile, MouseEnum.RightButton);
            AddSingleCombination(CommandEnum.Select, MouseEnum.LeftButton);

            AddSingleCombination(CommandEnum.StopCreation, MouseEnum.RightButton);
            AddSingleCombination(CommandEnum.Create, MouseEnum.LeftButton);

            AddSingleCombination(CommandEnum.ToggleFog, Keys.P);

            AddSingleCombination(CommandEnum.Candle, Keys.Number1);
            AddSingleCombination(CommandEnum.Trap, Keys.Number2);
            AddSingleCombination(CommandEnum.FakeTree, Keys.Number3);
            AddSingleCombination(CommandEnum.Cottage, Keys.Number4);
            AddSingleCombination(CommandEnum.Bridge, Keys.Number5);
            AddSingleCombination(CommandEnum.Rock, Keys.Number6);
            AddSingleCombination(CommandEnum.Tree, Keys.Number7);
            AddSingleCombination(CommandEnum.Castle, Keys.Number8);
            AddSingleCombination(CommandEnum.Person, Keys.Number9);
            AddSingleCombination(CommandEnum.Water, Keys.Number0);
        }
        /**
         * <summary>
         * Overrides the all key combinations for an action
         * </summary>
         */
        public void OverrideCombination(CommandEnum current, Keys k)
        {
            if (bindings.ContainsKey(current))
            {
                bindings[current].Clear();
            }
            AddSingleCombination(current,k);
        }
        /**
       * <summary>
       * Overrides the all key combinations for an action
       * </summary>
       */
        public void OverrideCombination(CommandEnum current, MouseEnum k)
        {
            if (bindings.ContainsKey(current))
            {
                bindings[current].Clear();
            }
            AddSingleCombination(current, k);
        }

        /**<summary>
        * Adds a new key combination. Shortcut for single keys
        * </summary>
        */
        private void AddSingleCombination(CommandEnum cmd, MouseEnum mouse)
        {
            AddCombination(cmd, null,new MouseEnum[] { mouse });
        }
        /**<summary>
         * Adds a new key combination.Shortcut for single keys
         * </summary>
         */
        private void AddSingleCombination(CommandEnum cmd, Keys keys)
        {
            AddCombination(cmd, new Keys[] { keys }, null);  
        }
        /**<summary>
         * Adds a new key combination
         * </summary>
         */
        private void AddCombination(CommandEnum cmd, Keys[] keys, MouseEnum[] mouseEnum)
        {
            if (!bindings.ContainsKey(cmd))
                bindings[cmd] = new List<KeyCombination>();

            bindings[cmd].Add(new KeyCombination(keys, mouseEnum));
        }
        /**
         * <summary>
         * Handles mouse rotation
         * </summary>
         */
        public int WheelRotation(MouseState mouse)
        {
            return mouse.Wheel;
        }
        /**
         * <summary>
         * Simulates mouse rotation with key combinations
         * </summary>
         */
        public int WheelRotation(KeyboardState keyboard,MouseState mouse)
        {
            int res = 0;
            if (CommandPressed(CommandEnum.ZoomIn,keyboard,mouse))
            {
                res--;
            }
            if (CommandPressed(CommandEnum.ZoomOut, keyboard,mouse))
            {
                res++;
            }
            return res;
        }
    }
}
