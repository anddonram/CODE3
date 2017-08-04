#region Using Statements
using Codigo.Behaviors;
using Codigo.GUI;
using Codigo.Sound;
using Codigo.VictoryConditions;
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
using WaveEngine.TiledMap;
#endregion

namespace Codigo
{
    public class PlayableScene : Scene
    {
       
       
        protected override void CreateScene()
        {
            //Tips: dónde NO se pueden crear objetos y añadirlos al entityManager
            //En los Initialize de Component
            //En ResolveDependencies de Component

            //Dónde sí se puede:
            //En CreateScene de Scene
            //  Pero si llamas a métodos de otras clase, que sepas que su EntityManager es null en ese momento
            //  Es decir, EntityManager=null, pero Owner.Scene.EntityManager sí vale
            //En Start de Scene
            //En Update de cualquier cosa

            this.Load(WaveContent.Scenes.MyScene);
            Viewport v = WaveServices.GraphicsDevice.RenderState.Viewport;
            //Pone el formato de la pantalla virtual (no de la ventana, eso va en app.cs)
            this.VirtualScreenManager.Activate(v.Width, v.Height, StretchMode.Uniform);

            
            //los jugadores
            Entity p1 = new Entity();
            p1.Tag = "player";
            Player player1 = new Player();
            player1.playerColor = Color.Red;
            player1.playerName = "player1";
            EntityManager.Add(p1.AddComponent(player1));

            Entity p2 = new Entity();
            p2.Tag = "player";
            Player player2 = new Player();
            player2.playerColor = Color.Yellow;
            player2.playerName = "player2";
            EntityManager.Add(p2.AddComponent(player2));


            //la camara
            UIBehavior.ui = new UIBehavior();
            Entity camera = new Entity("camera2D").AddComponent(new Transform2D { Scale = new Vector2(0.28f, 0.28f) })
                .AddComponent(new Camera2D { BackgroundColor=Color.Brown})
                .AddComponent(new KeysBehavior())
                .AddComponent(new Camera2DBehavior())
                .AddComponent(UIBehavior.ui)
                .AddComponent(new VictoryBehavior())
                .AddComponent(new NoPeopleCondition())
                .AddComponent(new CastleCondition());

            EntityManager.Add(camera);
            RenderManager.SetActiveCamera2D(camera);
            
            // el mapa
            Map.map = new Map();
            FogOfWar.fog = new FogOfWar();
            Entity map = new Entity("map")
                .AddComponent(new Transform2D())//{ DrawOrder=1})//the map is rendered at the bottom
                .AddComponent(new TiledMap(WaveContent.Assets.Tiles.Map_12_20_tmx))
                .AddComponent(Map.map)
                .AddComponent(FogOfWar.fog);
            EntityManager.Add(map);
      
        }
        protected override void Start()
        {
            base.Start();
            //Find players and set them
            IEnumerable<object> objects=EntityManager.FindAllByTag("player");
            List<Player> players = new List<Player>();
            foreach (Entity ent in objects) {
                players.Add(ent.FindComponent<Player>());
            }
            //Set map
            Map.map.SetMap();
            FogOfWar.fog.InitializeFog(true);

            //Set ui
            UIBehavior ui=UIBehavior.ui;

            ui.SetPlayers(players.ToArray());
            ui.SetUI();
            PopulateGame pop = new PopulateGame();
            pop.SetWoods();
            for (int i = 0; i < players.Count; i++)
            {
                pop.SetPlayer(players[i], i,players.Count);
            }
           
            //Force ui move to its original place
            WaveServices.Layout.PerformLayout(this);

            //Place camera
            RenderManager.ActiveCamera2D.Owner.FindComponent<Camera2DBehavior>().SetDefaultValues();

            //this is for clearing the new changing visibility tiles,  D* purposes
            this.AddSceneBehavior(new FogClearBehavior(), SceneBehavior.Order.PostUpdate);


            //And this is for the sounds
            UIBehavior.ui.Owner.AddComponent(new SoundHandler());

            UIBehavior.ui.Owner.FindComponent<VictoryBehavior>().Enable();
        }

    }
}
