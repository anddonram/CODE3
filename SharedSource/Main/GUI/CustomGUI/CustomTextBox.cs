using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Codigo.GUI
{
    public class CustomTextBox:Behavior
    {    /// <summary>
         /// The input service
         /// </summary>
        private Input inputService;
        /// <summary>
        /// The text control
        /// </summary>
        private TextBox textControl;
        /// <summary>
        /// Two part of text
        /// </summary>
        private string textBeforeCursor= string.Empty, textAfterCursor= string.Empty;

        private bool IsFocus=true;
        /// <summary>
        /// The before keyboard state
        /// </summary>
        private KeyboardState beforeKeyboardState;
        /// <summary>
        /// The alt-case actived
        /// </summary>
        private bool altcase;

        /// <summary>
        /// The uppercase actived
        /// </summary>
        private bool uppercase;
        protected override void Initialize()
        {
            base.Initialize();

            this.inputService = WaveServices.Input;

        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.inputService.KeyboardState.IsConnected  && this.IsFocus)
            {

                if (textControl.Text != textBeforeCursor + textAfterCursor)
                {
                    textControl.Text = textBeforeCursor + textAfterCursor;
                }
                if (this.inputService.KeyboardState.Number1 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number1 != ButtonState.Pressed)
                {
                    this.AppendCharacter('1');
                }
                else if (this.inputService.KeyboardState.Number2 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number2 != ButtonState.Pressed)
                {
                    this.AppendCharacter('2');
                }
                else if (this.inputService.KeyboardState.Number3 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number3 != ButtonState.Pressed)
                {
                    this.AppendCharacter('3');
                }
                else if (this.inputService.KeyboardState.Number4 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number4 != ButtonState.Pressed)
                {
                    this.AppendCharacter('4');
                }
                else if (this.inputService.KeyboardState.Number5 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number5 != ButtonState.Pressed)
                {
                    this.AppendCharacter('5');
                }
                else if (this.inputService.KeyboardState.Number6 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number6 != ButtonState.Pressed)
                {
                    this.AppendCharacter('6');
                }
                else if (this.inputService.KeyboardState.Number7 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number7 != ButtonState.Pressed)
                {
                    this.AppendCharacter('7');
                }
                else if (this.inputService.KeyboardState.Number8 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number8 != ButtonState.Pressed)
                {
                    this.AppendCharacter('8');
                }
                else if (this.inputService.KeyboardState.Number9 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number9 != ButtonState.Pressed)
                {
                    this.AppendCharacter('9');
                }
                else if (this.inputService.KeyboardState.Number0 == ButtonState.Pressed &&
                    this.beforeKeyboardState.Number0 != ButtonState.Pressed)
                {
                    this.AppendCharacter('0');
                }
                else if (this.inputService.KeyboardState.A == ButtonState.Pressed &&
                    this.beforeKeyboardState.A != ButtonState.Pressed)
                {
                    this.AppendCharacter('a');
                }
                else if (this.inputService.KeyboardState.B == ButtonState.Pressed &&
                    this.beforeKeyboardState.B != ButtonState.Pressed)
                {
                    this.AppendCharacter('b');
                }
                else if (this.inputService.KeyboardState.C == ButtonState.Pressed &&
                    this.beforeKeyboardState.C != ButtonState.Pressed)
                {
                    this.AppendCharacter('c');
                }
                else if (this.inputService.KeyboardState.D == ButtonState.Pressed &&
                    this.beforeKeyboardState.D != ButtonState.Pressed)
                {
                    this.AppendCharacter('d');
                }
                else if (this.inputService.KeyboardState.E == ButtonState.Pressed &&
                    this.beforeKeyboardState.E != ButtonState.Pressed)
                {
                    this.AppendCharacter('e');
                }
                else if (this.inputService.KeyboardState.F == ButtonState.Pressed &&
                    this.beforeKeyboardState.F != ButtonState.Pressed)
                {
                    this.AppendCharacter('f');
                }
                else if (this.inputService.KeyboardState.G == ButtonState.Pressed &&
                    this.beforeKeyboardState.G != ButtonState.Pressed)
                {
                    this.AppendCharacter('g');
                }
                else if (this.inputService.KeyboardState.H == ButtonState.Pressed &&
                    this.beforeKeyboardState.H != ButtonState.Pressed)
                {
                    this.AppendCharacter('h');
                }
                else if (this.inputService.KeyboardState.I == ButtonState.Pressed &&
                    this.beforeKeyboardState.I != ButtonState.Pressed)
                {
                    this.AppendCharacter('i');
                }
                else if (this.inputService.KeyboardState.Grave == ButtonState.Pressed &&
                    this.beforeKeyboardState.Grave != ButtonState.Pressed)
                {
                    this.AppendCharacter('º');
                }
                else if (this.inputService.KeyboardState.J == ButtonState.Pressed &&
                   this.beforeKeyboardState.J != ButtonState.Pressed)
                {
                    this.AppendCharacter('j');
                }
                else if (this.inputService.KeyboardState.K == ButtonState.Pressed &&
                   this.beforeKeyboardState.K != ButtonState.Pressed)
                {
                    this.AppendCharacter('k');
                }
                else if (this.inputService.KeyboardState.L == ButtonState.Pressed &&
                   this.beforeKeyboardState.L != ButtonState.Pressed)
                {
                    this.AppendCharacter('l');
                }
                else if (this.inputService.KeyboardState.M == ButtonState.Pressed &&
                   this.beforeKeyboardState.M != ButtonState.Pressed)
                {
                    this.AppendCharacter('m');
                }
                else if (this.inputService.KeyboardState.N == ButtonState.Pressed &&
                   this.beforeKeyboardState.N != ButtonState.Pressed)
                {
                    this.AppendCharacter('n');
                }
                else if (this.inputService.KeyboardState.O == ButtonState.Pressed &&
                   this.beforeKeyboardState.O != ButtonState.Pressed)
                {
                    this.AppendCharacter('o');
                }
                else if (this.inputService.KeyboardState.P == ButtonState.Pressed &&
                   this.beforeKeyboardState.P != ButtonState.Pressed)
                {
                    this.AppendCharacter('p');
                }
                else if (this.inputService.KeyboardState.Q == ButtonState.Pressed &&
                   this.beforeKeyboardState.Q != ButtonState.Pressed)
                {
                    this.AppendCharacter('q');
                }
                else if (this.inputService.KeyboardState.R == ButtonState.Pressed &&
                   this.beforeKeyboardState.R != ButtonState.Pressed)
                {
                    this.AppendCharacter('r');
                }
                else if (this.inputService.KeyboardState.S == ButtonState.Pressed &&
                   this.beforeKeyboardState.S != ButtonState.Pressed)
                {
                    this.AppendCharacter('s');
                }
                else if (this.inputService.KeyboardState.T == ButtonState.Pressed &&
                   this.beforeKeyboardState.T != ButtonState.Pressed)
                {
                    this.AppendCharacter('t');
                }
                else if (this.inputService.KeyboardState.U == ButtonState.Pressed &&
                   this.beforeKeyboardState.U != ButtonState.Pressed)
                {
                    this.AppendCharacter('u');
                }
                else if (this.inputService.KeyboardState.V == ButtonState.Pressed &&
                   this.beforeKeyboardState.V != ButtonState.Pressed)
                {
                    this.AppendCharacter('v');
                }
                else if (this.inputService.KeyboardState.W == ButtonState.Pressed &&
                   this.beforeKeyboardState.W != ButtonState.Pressed)
                {
                    this.AppendCharacter('w');
                }
                else if (this.inputService.KeyboardState.X == ButtonState.Pressed &&
                   this.beforeKeyboardState.X != ButtonState.Pressed)
                {
                    this.AppendCharacter('x');
                }
                else if (this.inputService.KeyboardState.Y == ButtonState.Pressed &&
                   this.beforeKeyboardState.Y != ButtonState.Pressed)
                {
                    this.AppendCharacter('y');
                }
                else if (this.inputService.KeyboardState.Z == ButtonState.Pressed &&
                   this.beforeKeyboardState.Z != ButtonState.Pressed)
                {
                    this.AppendCharacter('z');
                }
                else if (this.inputService.KeyboardState.Space == ButtonState.Pressed &&
                   this.beforeKeyboardState.Space != ButtonState.Pressed)
                {
                    this.AppendCharacter(' ');
                }
                else if (this.inputService.KeyboardState.Comma == ButtonState.Pressed &&
                 this.beforeKeyboardState.Comma != ButtonState.Pressed)
                {
                    this.AppendCharacter('.');
                }
                if (this.inputService.KeyboardState.Subtract == ButtonState.Pressed &&
                    this.beforeKeyboardState.Subtract != ButtonState.Pressed)
                {
                    this.RemoveBackCharacter();
                }

         

                // Supr Key
                if (this.inputService.KeyboardState.Delete == ButtonState.Pressed && this.beforeKeyboardState.Delete != ButtonState.Pressed)
                {
                    this.RemoveFrontCharacter();
                }

                // Special keys
                if ((this.inputService.KeyboardState.LeftShift == ButtonState.Pressed && this.beforeKeyboardState.LeftShift != ButtonState.Pressed) ||
                     (this.inputService.KeyboardState.RightShift == ButtonState.Pressed && this.beforeKeyboardState.RightShift != ButtonState.Pressed) ||
                     (this.inputService.KeyboardState.CapitalLock == ButtonState.Pressed && this.beforeKeyboardState.CapitalLock != ButtonState.Pressed && !this.uppercase))
                {
                    this.uppercase = true;
                }
                else if ((this.inputService.KeyboardState.LeftShift == ButtonState.Release && this.beforeKeyboardState.LeftShift != ButtonState.Release) ||
                     (this.inputService.KeyboardState.RightShift == ButtonState.Release && this.beforeKeyboardState.RightShift != ButtonState.Release) ||
                     (this.inputService.KeyboardState.CapitalLock == ButtonState.Pressed && this.beforeKeyboardState.CapitalLock != ButtonState.Pressed && this.uppercase))
                {
                    this.uppercase = false;
                }

                // Combinate Alt
                if (this.inputService.KeyboardState.RightAlt == ButtonState.Pressed && this.beforeKeyboardState.RightAlt != ButtonState.Pressed)
                {
                    this.altcase = true;
                }
                else if (this.inputService.KeyboardState.RightAlt == ButtonState.Release)
                {
                    this.altcase = false;
                }
            }else if (!textControl.IsReadOnly)
            {
                textControl.IsReadOnly = true;
            }
       
            this.beforeKeyboardState = this.inputService.KeyboardState;
        }
        
        internal void SetTextBox(TextBox newBox,ToggleSwitch toggle)
        {
            this.textControl = newBox;            
            textControl.Text = string.Empty;
            toggle.Toggled +=ToggleFocus;
        }

        private void ToggleFocus(object sender, EventArgs e)
        {
            IsFocus = (sender as ToggleSwitch).IsOn;
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="c">The c.</param>
        private void AppendCharacter(char c)
        {
     
            char character = c;
            if (this.uppercase)
            {
                if (c == 'º')
                {
                    character = 'ª';
                }
                else if (c == '1')
                {
                    character = '!';
                }
                else if (c == '2')
                {
                    character = '"';
                }
                else if (c == '3')
                {
                    character = '·';
                }
                else if (c == '4')
                {
                    character = '$';
                }
                else if (c == '5')
                {
                    character = '%';
                }
                else if (c == '6')
                {
                    character = '&';
                }
                else if (c == '7')
                {
                    character = '/';
                }
                else if (c == '8')
                {
                    character = '(';
                }
                else if (c == '9')
                {
                    character = ')';
                }
                else if (c == '0')
                {
                    character = '=';
                }
                else
                {
                    character = char.ToUpper(c);
                }
            }

            if (this.altcase)
            {
                if (c == 'º')
                {
                    character = '\\';
                }
                else if (c == '1')
                {
                    character = '|';
                }
                else if (c == '2')
                {
                    character = '@';
                }
                else if (c == '3')
                {
                    character = '#';
                }
                else if (c == '4')
                {
                    character = '~';
                }
                else if (c == '5')
                {
                    character = '€';
                }
                else if (c == '6')
                {
                    character = '¬';
                }
            }

            this.textControl.Text = this.textBeforeCursor + character + this.textAfterCursor;
            this.textBeforeCursor += character;
        }
        /// <summary>
        /// Removes the character.
        /// </summary>
        private void RemoveBackCharacter()
        {
            string text = this.textBeforeCursor;

            if (text.Length < 2)
            {
                this.textBeforeCursor = string.Empty;
            }
            else if (text.Length > 4)
            {
                string fourLast = text.Substring(text.Length - 4);
                if (fourLast.Equals(" /n "))
                {
                    this.textBeforeCursor = text.Substring(0, text.Length - 4);
                }
                else
                {
                    this.textBeforeCursor = text.Substring(0, text.Length - 1);
                }
            }
            else
            {
                this.textBeforeCursor = text.Substring(0, text.Length - 1);
            }

            this.textControl.Text = this.textBeforeCursor + this.textAfterCursor;
        }
        /// <summary>
        /// Removes the front character.
        /// </summary>
        private void RemoveFrontCharacter()
        {
            if (this.textAfterCursor.Length > 0)
            {
                this.textAfterCursor = this.textAfterCursor.Substring(1);
                this.textControl.Text = this.textBeforeCursor + this.textAfterCursor;
            }
        }
    }
}
