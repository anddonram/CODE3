using Codigo.VictoryConditions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace Codigo.Behaviors
{
    public class VictoryBehavior : Behavior
    {
        private VictoryCondition[] conditions;
        private UIBehavior uibehavior;
        private bool finished;

        protected override void Initialize()
        {
            base.Initialize();
            //health = maxHealth;

            conditions = Owner.FindComponents<VictoryCondition>(isExactType:false);
            finished = false;
        }
       
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            uibehavior = Owner.FindComponent<UIBehavior>();
        }
        public void Enable()
        {
            foreach (VictoryCondition v in conditions)
            {
                v.active = true;
            }
        }
        protected override void Update(TimeSpan gameTime)
        {
            if (!finished)
                foreach (VictoryCondition v in conditions)
                {
                    if (v.active)
                    {
                        Player p = v.GetWinner();
                        if (p != null)
                        {
                            Trace.WriteLine("The winner is " + p.playerName);
                            Trace.WriteLine( v.GetName());
                            finished = true;
                            Finish(p.playerName, v.GetName());
                            break;
                        }
                    }
                }
        }

        private void Finish(string playerName, string v)
        {

            TextBlock t = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 100, Foreground = Color.Black, Margin = new Thickness(0, 0, 0, 0), Width = 400, Text = playerName + " wins", TextAlignment = TextAlignment.Center };
            Owner.Scene.EntityManager.Add(t);

            TextBlock t2 = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 100, Foreground = Color.Black, Margin = new Thickness(0, 0, 0, 50), Width = 400, Text = v, TextAlignment = TextAlignment.Center };
            Owner.Scene.EntityManager.Add(t2);
            WaveServices.Layout.PerformLayout();
            Owner.Scene.Pause();
            WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(3), () =>
            {
                UIBehavior.Reload(Owner.Scene.GetType());
            },false);

        }
    }
}
