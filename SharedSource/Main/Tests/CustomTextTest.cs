using Codigo.Behaviors;
using Codigo.GUI;
using Codigo.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace Codigo.Tests
{
    class CustomTextTest : Scene
    {
        protected override void CreateScene()
        {
       
            this.Load(WaveContent.Scenes.MyScene);
            Viewport v = WaveServices.GraphicsDevice.RenderState.Viewport;
            //Pone el formato de la pantalla virtual (no de la ventana, eso va en app.cs)
            this.VirtualScreenManager.Activate(v.Width, v.Height, StretchMode.Uniform);

            //la camara
            Entity camera = new Entity("camera2D").AddComponent(new Transform2D { Scale = new Vector2(0.28f, 0.28f) })
                .AddComponent(new Camera2D { BackgroundColor = Color.Brown })
                .AddComponent(new Camera2DBehavior())
           
                ;

            EntityManager.Add(camera);
            RenderManager.SetActiveCamera2D(camera);
            
            TextBox newBox = new TextBox();
            ToggleSwitch toggle = new ToggleSwitch
            {
                IsOn = true,
                OnText = "Can write. ',' for '.', '-' (subtract) to delete",
                OffText = "Cannot write. Switch!",
                Margin = new Thickness(0, 30, 0, 0)
            };
            CustomTextBox custom = new CustomTextBox();
            custom.SetTextBox(newBox, toggle);

            EntityManager.Add(newBox);
            EntityManager.Add(toggle);
            EntityManager.Add(new Entity().AddComponent(custom));

        }
    }
}
