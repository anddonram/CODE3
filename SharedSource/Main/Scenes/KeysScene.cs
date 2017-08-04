using Codigo.GUI;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Common.Input;

namespace Codigo.Scenes
{
    public class KeysScene : Scene
    {
        private KeyBindings keys;

        CommandEnum current;
        public bool rewriting = false;
        private int lastClicked;

        private Type previousScene;

        private int rowNum = 10;
        private int offset = 0;

        private Button[] buttons;
        private TextBlock[] blocks;

        private bool restoreDefault;

        public KeysScene(Type previousScene, bool restoreDefault=false)
        {
            this.previousScene = previousScene;
            this.restoreDefault = restoreDefault;
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);
            MouseEnterBehavior.numHover = 0;
            Platform plat = WaveServices.Platform;
            this.VirtualScreenManager.Activate(plat.ScreenWidth, plat.ScreenHeight, StretchMode.Uniform, false);


            EntityManager.Add(new FixedCamera2D("cam") { BackgroundColor = Color.Brown });
            if (restoreDefault)
            {
                keys = new KeyBindings();
            }
            else
            {
                keys = KeyBindings.Restore();
                if (keys == null)
                {
                    keys = new KeyBindings();
                    keys.Save();
                }
            }
            Grid grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsBorder=true,
                Height = WaveServices.Platform.ScreenHeight * 0.8f,
                Width = WaveServices.Platform.ScreenWidth * 0.7f,
            };
            for (int i = 0; i < rowNum; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

            }
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            EntityManager.Add(grid);

            int row = 0;
            blocks = new TextBlock[rowNum];
            buttons = new Button[rowNum];

            foreach (CommandEnum cmd in keys.bindings.Keys)
            {
                Button b = new CustomButton() { Width = 200, Text = cmd.ToString(), number = row };
                b.Entity.AddComponent(new MouseEnterBehavior());
                
                TextBlock t = new TextBlock { Text = keys.bindings[cmd][0].ToString() };
                b.Click += WaitForKey;

                b.SetValue(GridControl.ColumnProperty, 0);
                b.SetValue(GridControl.RowProperty, row);

                t.SetValue(GridControl.ColumnProperty, 1);
                t.SetValue(GridControl.RowProperty, row);

                grid.Add(b);
                grid.Add(t);

                buttons[row] = b;
                blocks[row] = t;

                row++;
                if (row == rowNum)
                {
                    break;
                }
            }

            Button close = new Button()
            {
                Text = "Close",
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Bottom,
                HorizontalAlignment =HorizontalAlignment.Right
            };
            close.Entity.AddComponent(new MouseEnterBehavior());
            Button save = new Button()
            {
                Text = "Save",
                VerticalAlignment = VerticalAlignment.Bottom
            };
            save.Entity.AddComponent(new MouseEnterBehavior());
            close.Click += Close;
            save.Click += Save;
            EntityManager.Add(close);
            EntityManager.Add(save);

            Button restore = new Button()
            {
                Text = "Restore",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment=HorizontalAlignment.Center
            };
            restore.Entity.AddComponent(new MouseEnterBehavior());
            restore.Click += RestoreDefault;
            EntityManager.Add(restore);


            Button next = new Button()
            {
                Text = "Next",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            next.Entity.AddComponent(new MouseEnterBehavior());
            Button previous = new Button()
            {
                Text = "Previous",
                VerticalAlignment = VerticalAlignment.Center
            };
            previous.Entity.AddComponent(new MouseEnterBehavior());
            next.Click += Next;
            previous.Click += Previous;
            EntityManager.Add(next);
            EntityManager.Add(previous);

        }

       
        protected override void Start()
        {
            base.Start();
            WaveServices.Layout.PerformLayout(this);

            AddSceneBehavior(new KeyListenerBehavior(this),SceneBehavior.Order.PostUpdate);
        }
        public void CreateMouseCombination(MouseEnum m)
        {
            keys.OverrideCombination(current, m);
            blocks[lastClicked].Text = keys.bindings[current][0].ToString();
            Cancel();
        }

        public void CreateKeyCombination(Keys k)
        {
            keys.OverrideCombination(current, k);
            blocks[lastClicked].Text = keys.bindings[current][0].ToString();
            Cancel();
        }

        private void RestoreDefault(object sender, EventArgs e)
        {
            ScreenContext screenContext = new ScreenContext(new KeysScene(previousScene, true));
            WaveServices.ScreenContextManager.To(screenContext);
        }

        private void Previous(object sender, EventArgs e)
        {
            Cancel();
            if (offset > 0)
            {
                offset -= rowNum;

                int row = -offset;

                foreach (CommandEnum cmd in keys.bindings.Keys)
                {
                    if (row >= 0)
                    {
                        buttons[row].Text = cmd.ToString();
                        blocks[row].Text = keys.bindings[cmd][0].ToString();
                        buttons[row].IsVisible = true;
                        blocks[row].IsVisible = true;
                    }
                    row++;
                    if (row == rowNum)
                    {
                        break;
                    }
                }


            }
            WaveServices.Layout.PerformLayout(this);
        }

        private void Next(object sender, EventArgs e)
        {
            Cancel();
            if (offset+rowNum <keys.bindings.Count)
            {
                offset += rowNum;

                int row = -offset;

                foreach (CommandEnum cmd in keys.bindings.Keys)
                {
                    if (row >= 0)
                    {
                        buttons[row].Text = cmd.ToString();
                        blocks[row].Text = keys.bindings[cmd][0].ToString();
                        
                    }
                    row++;
                    if (row == rowNum)
                    {
                        break;
                    }
                }
                if (row > 0 && row < rowNum)
                {
                    for (int i = row; i < rowNum; i++)
                    {
                        buttons[i].IsVisible = false;
                        blocks[i].IsVisible = false;
                    }
                }
            }
            WaveServices.Layout.PerformLayout(this);
        }

        private void Save(object sender, EventArgs e)
        {
            Cancel();
            keys.Save();
            (sender as Button).BorderColor = Color.Green;
        }

        private void Close(object sender, EventArgs e)
        {
            Cancel();
            UIBehavior.Reload(previousScene);
        }

        private void WaitForKey(object sender, EventArgs e)
        {
            CustomButton b = sender as CustomButton;
            CommandEnum action;
            
            if (Enum.TryParse(b.Text, out action))
                if (!rewriting || current != action)
                {
                    Cancel();
                    lastClicked = b.number;
                    blocks[lastClicked].IsBorder = true;
                    blocks[lastClicked].BorderColor = Color.Blue;
                    buttons[lastClicked].BorderColor = Color.Blue;
                    rewriting = true;
                    current = action;
                }else
                {
                    Cancel();
                }

            WaveServices.Layout.PerformLayout(this);
        }
        private void Cancel()
        {
            if (lastClicked != -1)
            {
                buttons[lastClicked].BorderColor = Color.White;
                blocks[lastClicked].IsBorder = false;
            }
            lastClicked = -1;
            rewriting = false;
            WaveServices.Layout.PerformLayout(this);
        }
        
    }
}
