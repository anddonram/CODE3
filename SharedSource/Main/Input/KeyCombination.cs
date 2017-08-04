using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;

namespace Codigo
{
    /**
     * <summary>
     * Represents a combination of keys and mouse actions that are need to execute a command in the game
     * </summary>
     */
     [DataContract]
    public class KeyCombination
    {
        /**
         * <summary>
         * The list of keys to be pressed in conjunction with the mouse
         * </summary>
         */
         [DataMember]
        public List<Keys> keys { get; private set; }
        /**
         * <summary>
         * The list of mouse buttons to be pressed in conjunction with the keyboard
         * </summary>
         */
         [DataMember]
        public List<MouseEnum> mouseEnum { get; private set; }

        public KeyCombination(Keys[] keys, MouseEnum[] mouse)
        {
            if (keys != null)
            {
                this.keys = new List<Keys>(keys);
            }
            else
            {
                this.keys = new List<Keys>();
            }
            if (mouse != null)
            {
                this.mouseEnum = new List<MouseEnum>(mouse);
            }
            else
            {
                this.mouseEnum = new List<MouseEnum>();
            }
        }

        public bool IsKeyPressed(KeyboardState keyboard, MouseState mouse)
        {
            return keys.TrueForAll((key) => keyboard.IsKeyPressed(key))
                && mouseEnum.TrueForAll(m => IsKeyPressed(mouse, m));
        }
        public bool IsKeyReleased(KeyboardState keyboard, MouseState mouse)
        {
            return keys.Exists((key) => keyboard.IsKeyReleased(key))
                || mouseEnum.Exists(m => !IsKeyPressed(mouse, m));
        }
        /**
         * <summary>
         * Auxiliar method to know pressed mouse buttons
         * </summary>
         */
        public static bool IsKeyPressed(MouseState mouse, MouseEnum type)
        {
            bool res = false;
            switch (type)
            {
                case MouseEnum.LeftButton:
                    res = mouse.LeftButton == ButtonState.Pressed;
                    break;
                case MouseEnum.RightButton:
                    res = mouse.RightButton == ButtonState.Pressed;
                    break;
                case MouseEnum.MiddleButton:
                    res = mouse.MiddleButton == ButtonState.Pressed;
                    break;
                case MouseEnum.Wheel:
                    res = mouse.Wheel != 0;
                    break;
                default:
                    break;
            }
            return res;
        }
        public override string ToString()
        {
            string s;

            if (keys.Count == 0 && mouseEnum.Count == 0)
            {
                s = "None";
            }
            else
            {
                StringBuilder build = new StringBuilder();
                if (keys.Count > 0)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        build.Append(keys[i].ToString());
                        if (i < keys.Count - 1)
                            build.Append(" + ");
                    }
                    if (mouseEnum.Count > 0)
                    {
                        build.Append(" + ");
                    }
                }

                if (mouseEnum.Count > 0)
                {
                    for (int i = 0; i < mouseEnum.Count; i++)
                    {
                        build.Append(mouseEnum[i].ToString());
                        if (i < mouseEnum.Count - 1)
                            build.Append(" + ");
                    }
                }
                s = build.ToString();
            }

            return s;
        }
    }
}
