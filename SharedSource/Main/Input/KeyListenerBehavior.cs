using Codigo.GUI;
using Codigo.Scenes;
using System;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Codigo
{
    public class KeyListenerBehavior : SceneBehavior
    {
        private KeysScene scene;

        private Input input;

        public KeyListenerBehavior(KeysScene s)
        {
            this.scene = s;
            input = WaveServices.Input;
        }


        protected override void Update(TimeSpan gameTime)
        {

            if (scene.rewriting && input.KeyboardState.IsConnected)
            {
                DetectKeyboardInput();
            }
            if (MouseEnterBehavior.numHover == 0)
                if (scene.rewriting && input.MouseState.IsConnected)
                {
                    DetectMouseInput();
                }
        }

        private void DetectKeyboardInput()
        {

            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                if (input.KeyboardState.IsKeyPressed(k))
                {
                    scene.CreateKeyCombination(k);
                    break;
                }
            }

        }
        private void DetectMouseInput()
        {

            foreach (MouseEnum m in Enum.GetValues(typeof(MouseEnum)))
            {
                if (KeyCombination.IsKeyPressed(input.MouseState, m))
                {
                    scene.CreateMouseCombination(m);
                    break;
                }
            }

        }

        protected override void ResolveDependencies()
        {
           
        }
    }
}