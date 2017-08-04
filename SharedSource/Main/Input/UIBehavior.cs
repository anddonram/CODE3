using System;
using System.Collections.Generic;
using WaveEngine.Framework;
using WaveEngine.Components.UI;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Math;
using WaveEngine.TiledMap;
using WaveEngine.Framework.UI;
using Codigo.Behaviors;
using System.Diagnostics;
using Codigo.Components;
using WaveEngine.Components.Animation;
using WaveEngine.Common.Graphics;
using Codigo.GUI;
using System.Threading;
using Codigo.Scenes;

namespace Codigo
{
    public class UIBehavior : Behavior
    {
        /**
         * <summary>
         * A static instance everyone can access. Populate in CreateScene or things will go bad!!
         * </summary>
         */
        public static UIBehavior ui;
        /**
         * <summary>
         * 4 the players
         * </summary>
         */
        private Player[] players;
        /**
         * <summary>
         * which player is currently controlling this instance
         * </summary>
         */
        public Player activePlayer { get; private set; }
        [RequiredComponent]
        private Camera2D cam;
        [RequiredComponent]
        private KeysBehavior keysBehavior;

        private FogOfWar fog;
        private Map map;


        /**
         * <summary>
         * holds the name of the WO we will create
         * </summary>
         */
        string creating = null;

        /** <summary>
        * UI interface text for different purposes: name, coordinates, health, wood, etc.
        * </summary>
        */
        private TextBlock coordinateText, woNameText, healthText,woodText;
        /**
         * <summary>
         * Shows the person skills if selected
         * </summary>
         */
        private TextBlock buildSkill, fightSkill, chopSkill, healSkill;
        /**
         * <summary>
         * All the buttons for creating WOs that can be used by the player
         * </summary>
         */
        public Dictionary<CommandEnum, RadioButton> buildingCreationButtons;
        /**
         * <summary>
         * All the buttons for creating WOs that cannot be created by the player
         * </summary>
         */
        public Dictionary<CommandEnum, RadioButton> woCreationButtons;

        /**
         * <summary>
         * These blocks of text help you make an unit exit a cottage
         * </summary>
         */
        private TextBlock[] cottageList;

        /**
         * <summary>
         * This slider allows runtime modification of the WO speed.
         * </summary>
         */
        private Slider speedSlider;
        /**
         * <summary>
         * This slider allows runtime modification of the scene speed.
         * </summary>
         */
        private Slider gameSpeedSlider;
        /**
         * <summary>
         * This button stops the selected wo action, if possible
         * </summary>
         */
        private Button stopButton;

        private Button attackButton;
        /**
         * <summary>
         * The action button list, for selecting actions
         * </summary>
         */
        public List<Button> buttons = new List<Button>();
        /**
         * <summary>
         * The action handlers, for synchronizing the buttons with the corresponding actions
         * </summary>
         */
        public List<ActionHandler> actions = new List<ActionHandler>();
        /**
         * <summary>
         * This controls whether on the second right click, we are gonna move there
         * </summary>
         */
        public bool optionsAlreadyShown = false;

        /**<summary>
         * This switcher changes the ui between debug interface and game interface
         * </summary>
         */
        private ToggleSwitch2 debugSwitch;
   
        /**<summary>
         * This switcher changes the ui to show or hide all creation buttons
         * </summary>
         */
        private ToggleSwitch2 creationSwitch;
        /**<summary>
         * This switcher changes the ui to show or hide all player change buttons
         * </summary>
         */
        private ToggleSwitch2 playerSwitch;

        /**
         * <summary>
         * Restarts the scene
         * </summary>
         */
        Button reloadButton;
        /**
         * <summary>
         * Changes the scene type
         * </summary>
         */
        Button changeSceneButton;
        /**
         * <summary>
         * Exit the application button
         * </summary>
         */
        Button exitButton;
        /**
         * <summary>
         * Change the keys button
         * </summary>
         */
        Button keysButton;
        /**
         * <summary>
         * Opens the menu
         * </summary>
         */
        ToggleSwitch2 menuButton;

        /**
         * <summary>
         * When any of these are pressed, the active player will change accordingly
         * </summary>
         */
        public CustomRadioButton[] playerButtons;
        /**
         * <summary>
         * When this button is pressed, the current active player will be null
         * </summary>
         */
        private CustomRadioButton clearPlayerButton;
        /**
         * <summary>
         * Variable for toggling the fog
         * </summary>
          */
        bool fogOn = true;

        public Player[] GetPlayers()
        {
            return players;
        }

        public void SetPlayers(Player[] playersList)
        {
            players = playersList;
            playerButtons = new CustomRadioButton[players.Length];
           
            for (int i=0;i<players.Length;i++)
            {           
                players[i].SetResources();
                playerButtons[i] = new CustomRadioButton { Foreground = Color.Black, number = i, GroupName = "Players", Text = players[i].playerName, Margin = new Thickness(0, 0, 0, 40 * (players.Length-i+1)),VerticalAlignment=VerticalAlignment.Bottom };
                Owner.Scene.EntityManager.Add(playerButtons[i]);
                playerButtons[i].Checked +=ChangePlayer;
                playerButtons[i].Entity.AddComponent(new MouseEnterBehavior());
            }
            activePlayer = players[0];
            playerButtons[0].IsChecked = true;
            clearPlayerButton = new CustomRadioButton { Foreground = Color.Black, number = -1, GroupName = "Players", Text = "No player", Margin = new Thickness(0, 0 , 0, 40), VerticalAlignment = VerticalAlignment.Bottom };
            clearPlayerButton.Checked += ClearPlayer;
            clearPlayerButton.Entity.AddComponent(new MouseEnterBehavior());
            Owner.Scene.EntityManager.Add(clearPlayerButton);
        }
     
        private void ClearPlayer(object sender, EventArgs e)
        {
            ClearPlayer();
        }
        public void ClearPlayer()
        {
            if (activePlayer != null)
            {
                activePlayer.active = false;
                activePlayer = null;
                if (fog != null)
                    fog.SetFogForPlayer(null);
            }
        }

        private void ChangePlayer(object sender, EventArgs e)
        {    
            ChangePlayer((sender as CustomRadioButton).number);
        }
        public void ChangePlayer(int i)
        {
            if(activePlayer!=null)
                activePlayer.active = false;
            ClearActionButtons();
            activePlayer = players[i];
            activePlayer.active = true;
            if (fog != null)
                fog.SetFogForPlayer(activePlayer);
        }
        public void SetUI()
        {
            buildingCreationButtons = new Dictionary<CommandEnum, RadioButton>();
            woCreationButtons = new Dictionary<CommandEnum, RadioButton>();

            CommandEnum[] commands = WorldObjectData.buildings;
            for (int i = 0; i < commands.Length; i++)
            {
               buildingCreationButtons.Add(commands[i], CreateCreationButton(commands[i], i + 1));
            }

            CommandEnum[] commands2 = WorldObjectData.notBuildings;
            for (int i = 0; i < commands2.Length; i++)
                woCreationButtons.Add(commands2[i], CreateCreationButton(commands2[i], i +1+ commands.Length));

            menuButton = new ToggleSwitch2 { IsOn = false, IsBorder = true, Width = 110, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Right, OnText = "Close Menu", OffText = "Open Menu", Margin = new Thickness(0, 0, 0, 0) ,TextColor=Color.Black};
            reloadButton = new Button { Width=150, BackgroundImage = WaveContent.Assets.uibackground_png, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Color.Black, Text = "Reload", Margin = new Thickness(0, 0, 0, 0),IsVisible=false };
            changeSceneButton = new Button {Width = 150, BackgroundImage = WaveContent.Assets.uibackground_png, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Color.Black, Text = "Change Scene", Margin = new Thickness(0, 0, 0, 50), IsVisible = false };
            exitButton = new Button {  Width = 150, BackgroundImage = WaveContent.Assets.uibackground_png, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Color.Black, Text = "Close Game", Margin = new Thickness(0, 50, 0, 0), IsVisible = false };
            keysButton = new Button { Width = 150, BackgroundImage = WaveContent.Assets.uibackground_png, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Color.Black, Text = "Keys", Margin = new Thickness(0, 100, 0, 0), IsVisible = false };

            debugSwitch = new ToggleSwitch2 {Width=120, IsBorder = true, VerticalAlignment = VerticalAlignment.Bottom, TextColor = Color.Black, HorizontalAlignment = HorizontalAlignment.Right, OnText = "Debugging", OffText = "Playing", IsOn = true, Margin = new Thickness(0, 0, 120, 0) };
            creationSwitch = new ToggleSwitch2 { Width = 120, IsBorder = true, HorizontalAlignment = HorizontalAlignment.Right, TextColor = Color.Black, OnText = "Hide create", OffText = "Create", IsOn = true, Margin = new Thickness(0, 0, 0, 0) };
            playerSwitch = new ToggleSwitch2 { Width = 120, IsBorder = true, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left, TextColor = Color.Black, OnText = "Hide players", OffText = "Change player", IsOn = true, Margin = new Thickness(0, 0, 0, 0) };

            SetButtons(changeSceneButton, ChangeScene );
            SetButtons(reloadButton, Reload);
            SetButtons(menuButton, HandleMenu);
            SetButtons(exitButton, Exit);
            SetButtons(keysButton, ChangeKeys);

            SetButtons(debugSwitch, ShowDebugCreationButtons);
            SetButtons(creationSwitch, ShowCreationButtons);
            SetButtons(playerSwitch, ShowPlayerSwitch);



            //Cottage info: This is the maximum number of units a cottage can shelter
            int maxCottageOccupation = Cottage.MAX_OCCUPATION;
            cottageList = new TextBlock[maxCottageOccupation];
            //we will make a grid of 2x3
            int width = 2;
            int height = maxCottageOccupation / width;
            for (int i = 0; i < width; i++)
                for (int j = 0; j <height; j++)
                {
                    int pos = i+(j*width);
                    cottageList[pos] = new TextBlock { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Color.Black, Text = "Unit", Margin = new Thickness(50*(width-i-1), 0, 150 * i, 30 * j) };
                    cottageList[pos].IsVisible = false;
                    Owner.Scene.EntityManager.Add(cottageList[pos]);
                }

            //Now, we add the generic info, coordinates, health, etc.
            woodText = CreateInfoBlock(0);
            coordinateText = CreateInfoBlock(1);
            woNameText = CreateInfoBlock(2);
            healthText = CreateInfoBlock(3);
            
            //Person info
            buildSkill = CreateInfoBlock(4);
            fightSkill = CreateInfoBlock(5);
            healSkill = CreateInfoBlock(6);
            chopSkill = CreateInfoBlock(7);

            //you know, this slider is only for integer. buff.
            //So we do a trick by using 1 unit as 0.1 speed
            //Being the minimum 0.1f and the maximum, 6f
            speedSlider = new Slider { Value = 2, Minimum = 2, Maximum = 60, Margin = new Thickness(10, 170, 0, 0) };
            speedSlider.RealTimeValueChanged += ChangeWOSpeed;
            speedSlider.IsVisible = false;
            Owner.Scene.EntityManager.Add(speedSlider);
            speedSlider.Entity.AddComponent(new MouseEnterBehavior());

            //The same with the scene speed
            gameSpeedSlider = new Slider { VerticalAlignment = VerticalAlignment.Top,HorizontalAlignment=HorizontalAlignment.Center,  Minimum = 1, Maximum = 20, Margin = new Thickness(0, 20, 0, 0) };
            gameSpeedSlider.RealTimeValueChanged += ChangeSceneSpeed;
            Owner.Scene.EntityManager.Add(gameSpeedSlider);
            gameSpeedSlider.Value = 10;
            gameSpeedSlider.Entity.AddComponent(new MouseEnterBehavior());

            //The stop button so we can stop the button
            stopButton = new Button {Width=100, BackgroundImage=WaveContent.Assets.uibackground_png,Foreground = Color.Black, Text = "Stop", Margin = new Thickness(10, 200, 0, 0) };
            stopButton.Click += StopWO;
            stopButton.IsVisible = false;
            Owner.Scene.EntityManager.Add(stopButton);
            stopButton.Entity.AddComponent(new MouseEnterBehavior());

            //The attack button so we can toggle the attack status
            attackButton = new Button {Width=150, BackgroundImage = WaveContent.Assets.uibackground_png, Foreground = Color.Black, Text = "Change Status", Margin = new Thickness(120, 200, 0, 0) };
            attackButton.Click += AttackWO;
            attackButton.IsVisible = false;
            Owner.Scene.EntityManager.Add(attackButton);
            attackButton.Entity.AddComponent(new MouseEnterBehavior());
        }

        /**
       * <summary>
       * Adds all stuff to GUI switches
       * </summary>
       */
        private void SetButtons(ToggleSwitch2 ts, EventHandler e)
        {
            EntityManager.Add(ts);
            ts.Toggled += e;
            //Behaviors for distinguish between GUI and play field
            ts.Entity.AddComponent(new MouseEnterBehavior());
        }

        /**
        * <summary>
        * Adds all stuff to GUI buttons
        * </summary>
        */
        private void SetButtons(Button button, EventHandler e )
        {
            EntityManager.Add(button);
            button.Click += e;
            //Behaviors for distinguish between GUI and play field
            button.Entity.AddComponent(new MouseEnterBehavior());
        }
        /**
         * <summary>
         * Moves to the key change scene
         * </summary>
         */
        private void ChangeKeys(object sender, EventArgs e)
        {   
            ScreenContext screenContext = new ScreenContext(new KeysScene(Owner.Scene.GetType()));
            WaveServices.ScreenContextManager.To(screenContext);
        }

        /**<summary>
* Creates an info textblock and places it accordingly
* </summary>
*/
        private TextBlock CreateInfoBlock(int i)
        {
            TextBlock t = new TextBlock { Foreground = Color.Black, Margin = new Thickness(0, 20*i, 0, 0), Width = 200 };
            Owner.Scene.EntityManager.Add(t);
            return t;
        }

        /**
        * <summary>
        * Changes the speed for the scene
        * </summary>
        */
        private void ChangeSceneSpeed(object sender, ChangedEventArgs e)
        {
            Owner.Scene.Speed = e.NewValue / 10f;
        }

        /**
        * <summary>
        * Creates a radio button for creating something and places it accordingly
        * </summary>
        */
        private RadioButton CreateCreationButton(CommandEnum woName, int position)
        {
            RadioButton b = new RadioButton { BorderColor=Color.Black,HorizontalAlignment = HorizontalAlignment.Right, Foreground = Color.Black, GroupName = "Creator", Text = woName.ToString(), Margin = new Thickness(0, 50 * position, 0, 0) };
            Owner.Scene.EntityManager.Add(b);
            b.Checked += AddWO;
            b.Entity.AddComponent(new MouseEnterBehavior());
            return b;
        }

        /**<summary>
        * This switcher changes the ui to show or hide all player change buttons
        * </summary>
        */
        private void ShowPlayerSwitch(object sender, EventArgs e)
        {
            bool on = (sender as ToggleSwitch2).IsOn;
            for (int i = 0; i < players.Length; i++)
            {

                playerButtons[i].IsVisible = on;
        
            }
            clearPlayerButton.IsVisible = on;

        }

        /**<summary>
       * This switcher changes the ui between debug interface and game interface. Only some objects can be created while playing
       * </summary>
       */
        private void ShowDebugCreationButtons(object sender, EventArgs e)
        {

            bool on = (sender as ToggleSwitch2).IsOn;
            //Show the player buttons
            if (clearPlayerButton != null)
                clearPlayerButton.IsVisible = on && playerSwitch.IsOn;
            foreach (RadioButton b in playerButtons)
                b.IsVisible = on && playerSwitch.IsOn;


            playerSwitch.IsVisible = on;

            //Make the slider also visible
            speedSlider.IsVisible = on && activePlayer != null && activePlayer.selectedWO != null;
            gameSpeedSlider.IsVisible = on;

            //Now we need a different kind of on
            on = on && creationSwitch.IsOn;

            foreach (CommandEnum rb in woCreationButtons.Keys)
            {
                woCreationButtons[rb].IsVisible = on;
            }

            //If any of these was checked, uncheck them
            if (!on)
            {
                foreach (CommandEnum rb in woCreationButtons.Keys)
                {
                    if (woCreationButtons[rb].IsChecked)
                    {
                        Uncheck();
                        break;
                    }

                }

            }
        }

        /**<summary>
         * This switcher changes the ui to show or hide all creation buttons
         * </summary>
         */
        private void ShowCreationButtons(object sender, EventArgs e)
        {
            bool on = (sender as ToggleSwitch2).IsOn;
            foreach (CommandEnum rb in woCreationButtons.Keys)
            {
                woCreationButtons[rb].IsVisible = on && debugSwitch.IsOn;
            }


            foreach (CommandEnum rb in buildingCreationButtons.Keys)
            {
                buildingCreationButtons[rb].IsVisible = on;
            }
   
            
            //If making invisible, uncheck all
            if (!on)
            {
                Uncheck();
            }
        }


        /**
         * <summary>
         * Button command for stopping a wo
         * </summary>
         */
        private void StopWO(object sender, EventArgs e)
        {
            if (activePlayer != null && activePlayer.selectedWO != null&& activePlayer.selectedWO.player == activePlayer)
            {
                activePlayer.selectedWO.Stop();
            }
        }

        private void AttackWO(object sender, EventArgs e)
        {
            
            if (activePlayer != null && activePlayer.selectedWO != null && activePlayer.selectedWO.player == activePlayer)
            {
                activePlayer.selectedWO.AttackToggle();
            }
        }
        /**
         * <summary>
         * Creates a new action button, cause there were not enough. Notice that buttons are only enabled or disabled, but not deleted
         * </summary>
         */
        public void CreateActionButton()
        {
            Button b = new Button {Width=110, BackgroundImage =WaveContent.Assets.uibackground_png,Foreground = Color.Black, Text = "Action", Margin = new Thickness(10, 250 + 50 * buttons.Count, 0, 0) };
            buttons.Add(b);
            b.Entity.AddComponent(new MouseEnterBehavior());
           
            Owner.Scene.EntityManager.Add(b);

        }
        /**
         * <summary>
         * Clears the action buttons and resets the list
         * </summary>
         */
        public void ClearActionButtons()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                buttons[i].Click -= actions[i].buttonAction;
                buttons[i].IsVisible = false;
            }
            optionsAlreadyShown = false;
            actions.Clear();
        }
        /**
         * <summary>
         * Adds an action to the list
         * </summary>
         */
        public void AddAction(string actionText, Action a)
        {
            ActionHandler ah = new ActionHandler { text = actionText, action = a, };
            actions.Add(ah);
        }

        /**
         * <summary>
         * Updates the action button with the corresponding action
         * </summary>
         */
        public void UpdateActionButton(int aux)
        {
            Button b = buttons[aux];
            //Add the button to the actionhandler, so we can remove it later when refreshing
            actions[aux].buttonAction = (object sender, EventArgs e) => { actions[aux].action();ClearActionButtons(); };

            //Add the action to the corresponding
            b.Click += actions[aux].buttonAction;
            b.Text = actions[aux].text;

            //Compute the approx width and height of the entity
            Vector2 size = b.Entity.FindChild("TextEntity").FindComponent<TextControl>().SpriteFont.MeasureString(b.Text);
            b.Width = Math.Max(100, size.X+5);
            b.IsVisible = true;
        }
        /**
         * <summary>
         * Executes inmediately the action at position i, and removes it from the list
         * </summary>
         */
        public void ExecuteAction(int i)
        {
            actions[i].action();
            actions.RemoveAt(i);
        }
        /**
         * <summary>
         * Event handler for manipulating a WO's speed
         * </summary>
         */
        private void ChangeWOSpeed(object sender, ChangedEventArgs e)
        {
            if (activePlayer != null && activePlayer.selectedWO != null)
            {
                activePlayer.selectedWO.genericSpeed=e.NewValue / 10f;           
            }
        }
        /**
         * <summary>
         * Updates the cottage interface, if any selected
         * </summary>
         */
        public void UpdateCottageUI(Cottage cottage)
        {
            if (cottage == null)
            {
                //Disable buttons
                for (int i = 0; i < cottageList.Length; i++)
                    cottageList[i].IsVisible = false;
            }else
            {
                
                List<WorldObject> people = cottage.GetPeople();
                //Enable buttons for each person
                for (int i = 0; i < people.Count; i++)
                {
                    WorldObject wo = people[i];
                    cottageList[i].Text =string.Format("{0}: {1} - {2}/{3}", i + 1 , wo.GetWoName(), wo.GetHealth() , wo.GetMaxHealth());
                    cottageList[i].IsVisible = true;
                }
                //Disable the rest of buttons
                for (int i = people.Count; i < cottageList.Length; i++)
                { 
                    cottageList[i].IsVisible = false;
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            //Reset the hover, otherwise it will stay forever if reloaded, because its static
            MouseEnterBehavior.numHover = 0;
            map = Map.map;
            fog = FogOfWar.fog;

        }
        protected override void Update(TimeSpan gameTime)
        {
           
            if (debugSwitch.IsOn)
            {
                //Only for debug purposes
                HandleDebugUI();
            }
            else
            {
                HandleUI();
            }

            if (cottageList[0].IsVisible && (activePlayer == null || activePlayer.selectedWO == null))
                //Disable cottage list if no longer active
                UpdateCottageUI(null);


            if (keysBehavior.IsCommandExecuted(CommandEnum.StopCreation))
            {
                Uncheck();
            }
            if (creating != null && keysBehavior.IsCommandExecuted(CommandEnum.Create) && !MouseOverGUI())
            {
                if (creating == "Castle")
                {
                    CreateCastle();
                }
                else
                {
                    Create();

                }

            }
            if (debugSwitch.IsOn)
            {
                foreach (CommandEnum cmd in woCreationButtons.Keys)
                    if (keysBehavior.IsCommandExecuted(cmd))
                    {
                        woCreationButtons[cmd].IsChecked = true;
                        break;
                    }
            }
            foreach (CommandEnum cmd in buildingCreationButtons.Keys)
                if (keysBehavior.IsCommandExecuted(cmd))
                {
                    buildingCreationButtons[cmd].IsChecked = true;
                    break;
                }

            if (keysBehavior.IsCommandExecuted(CommandEnum.ToggleFog))
            {
               // fog.ToggleFog(fogOn);
                fogOn = !fogOn;
            }
        }
        private void CreateCastle()
        {
            LayerTile tile = map.GetTileByWorldPosition(GetMousePosition());
            if (tile != null)
                CreateCastleToTile(activePlayer, tile.X, tile.Y, 3, 2);
        }

 
        public void CreateCastleToTile(Player p, int x, int y, int castleWidth, int castleHeight)
        {
            if (!map.IsTileOccupied(x, y))
            {
                Castle castle = new Castle();
                Entity entity = new Entity()
                    .AddComponent(new Transform2D())
                    .AddComponent(castle);

                bool createdCastle = castle.SetCastle(x, y, castleWidth, castleHeight, p);
                if (createdCastle)
                {
                    Owner.Scene.EntityManager.Add(entity);
                    if (p != null)
                        p.castle = castle;
                    castle.player = p;
                }
                    
                Uncheck();
            }
        }
    
        private Entity Create(LayerTile tile,Player p)
        {

            if (tile != null)
            {

                WorldObjectData data = WorldObjectData.GetData(creating);
                WorldObject wo = new WorldObject();
                Entity entity = new Entity()
                    .AddComponent(new Transform2D
                    {
                        Position = tile.LocalPosition,
                        DrawOrder = data.traversable ? 1f : 0.5f //traversable should be rendered behind non traversable
                    })
                    .AddComponent(wo)
                    .AddComponent(new FogRevealBehavior());
                data.AddTraits(entity);
                
                wo.SetWoName(creating);
                wo.SetTraversable(data.traversable);
                wo.SetMobile(data.mobile);
                wo.SetMaxHealth(data.maxHealth);
                wo.SetHealth(data.health);
                wo.player = p;
                bool canCreate = (!data.isBuilding || wo.player == null ||
                    ((fog==null||fog.IsVisible(tile)) && wo.player.HasEnoughWood(data.woodRequired)));
                if (creating=="FakeTree")
                {
                    canCreate = canCreate && map.GetMobile(tile.X, tile.Y) == null;
                }
                //Has it passed all this requirements?Let's check the tiles
                if (canCreate)
                {
                    //Reset the requirements
                    canCreate = false;
                    //¿can it move?
                    if (data.mobile)
                    {
                        if (map.GetMobile(tile.X, tile.Y) == null)
                            if (data.traversable)
                            {
                                //traversable, mobile object
                                //Nothing like this
                                map.SetMobile(tile.X, tile.Y, wo);
                                canCreate = true;
                            }
                            else
                            {
                                //non traversable, mobile object
                                //Like person
                                if (!map.IsTileOccupied(tile.X, tile.Y)&&(map.GetWorldObject(tile.X,tile.Y)==null||map.GetWorldObject(tile.X,tile.Y).IsTraversable(wo)))
                                {
                                    //must be free to create
                                    map.SetMobile(tile.X, tile.Y, wo);
                                    map.SetTileOccupied(tile.X, tile.Y, true);
                                    canCreate = true;
                                }
                            }
                        entity.AddComponent(new MovementBehavior())
                              .AddComponent(new DStarLite())
                              .AddComponent(new SwitchBehavior())
                              .AddComponent(new SpriteAtlas(WaveContent.Assets.persona_spritesheet))
                              .AddComponent(new SpriteAtlasRenderer())
                              .AddComponent(new Animation2D()
                              {
                                  PlayAutomatically = false,
                              })
                              .AddComponent(new AnimationBehavior());

                    }
                    else
                    {
                        if ((map.GetWorldObject(tile.X, tile.Y) == null&& creating != "Bridge") || (map.GetWorldObject(tile.X, tile.Y)!=null && map.GetWorldObject(tile.X, tile.Y).GetWoName()=="Water" && creating == "Bridge"))
                        {
                            
                            if (data.traversable)
                            {
                                //traversable, static object
                                //fake tree,traps
                                Trace.WriteLine(creating);
                                if (creating == "Bridge")
                                {
                                    WorldObject water = map.GetWorldObject(tile.X, tile.Y);
                                    entity.FindComponent<BridgeTraits>().water = water;
                                    water.Owner.IsVisible = false;
                                    map.SetTileOccupied(tile.X, tile.Y, false);
                                }
                                map.SetWorldObject(tile.X, tile.Y, wo);
                                canCreate = true;
                            }
                            else
                            {
                                //non-traversable, static object
                                //tree,rock
                                if (!map.IsTileOccupied(tile.X, tile.Y))
                                {
                                    //must be free to create
                                    map.SetWorldObject(tile.X, tile.Y, wo);
                                    map.SetTileOccupied(tile.X, tile.Y, true);
                                    canCreate = true;
                                }
                            }
                        }
                        entity.AddComponent(new SpriteRenderer())
                    .AddComponent(new Sprite(data.sprite));


                    }
                    //Has it passed all the requirements, including the tiles?Let's populate the entity with behaviors
                    if (canCreate)
                    {
                        if (data.isBuilding && wo.player != null)
                        {
                            wo.player.RemoveWood(data.woodRequired);
                        }
                        data.AddComponents(entity);
                        if (creating == "Person") { 
                            if (wo.player != null)
                                wo.player.GetPeople().Add(wo);
                        }
                      
                       
                        //From now on, we must set the visibility for those tiles visible, otherwise they must be not visible
                        //New fog of war handles this
                        entity.IsVisible = fog == null || (wo.IsVisible(activePlayer) && fog.IsVisible(tile.X, tile.Y));
                        Owner.Scene.EntityManager.Add(entity);

                        Uncheck();
                        //We update the tile visibility, in case it is revealed
                        if (fog != null) {
                            fog.AddUpdatedTile(tile);
                        }
                    }
                    return entity;
                }
            }
            return null;
        }
        private Entity Create()
        {
            return Create(map.GetTileByWorldPosition(GetMousePosition()), activePlayer);
        }
        public Entity CreateToTile(string creation, int x, int y, Player p)
        {
            creating = creation;
            return Create(map.GetTileByMapCoordinates(x, y),p);
        }
        public Entity CreateToTile(string creation, int x, int y)
        {
            creating = creation;
            return CreateToTile( creation,  x,  y, activePlayer);
        }
        private void AddWO(object sender, EventArgs e)
        {
            creating=((RadioButton)sender).Text;
        }
        public void SetCreating(RadioButton b)
        {
            b.IsChecked = true;
        }
        private void Uncheck()
        {
            foreach (CommandEnum cmd in buildingCreationButtons.Keys)
            {
                buildingCreationButtons[cmd].IsChecked = false;
            }
            foreach (CommandEnum cmd in woCreationButtons.Keys)
            {
                woCreationButtons[cmd].IsChecked = false;
            }

            creating = null;
        }
        /**
        * <summary>
        * Opens or closes the menu
        * </summary>
        */
        private void HandleMenu(object sender, EventArgs e)
        {
            reloadButton.IsVisible=menuButton.IsOn;
            changeSceneButton.IsVisible = menuButton.IsOn;
            exitButton.IsVisible = menuButton.IsOn;
            keysButton.IsVisible = menuButton.IsOn;
        }
        /**
         * <summary>
         * Finish the application
         * </summary>
         */
        private void Exit(object sender, EventArgs e)
        {
            Owner.Scene.Pause();
            Thread.Sleep(2000);
            WaveServices.Platform.Exit();

        }
        /**
         * <summary>
         * Restarts a new scene of the same class (myscene, playablescene, test, etc.) of the current scene
         * </summary>
         */
        private void Reload(object sender, EventArgs e)
        {
            Reload(Owner.Scene.GetType());
        }
        public static void Reload(Type type)
        {
            Scene newScene = Activator.CreateInstance(type) as Scene;
            ScreenContext screenContext = new ScreenContext(newScene);
            WaveServices.ScreenContextManager.To(screenContext);
        }
        /**
         * <summary>
         * Restarts a new scene of the different class (myscene/playablescene) of the current scene
         * </summary>
         */
        private void ChangeScene(object sender, EventArgs e)
        {
            Scene newScene = null;
            if (Owner.Scene is MyScene)
            {
                newScene = new PlayableScene();
            }
            else if(Owner.Scene is TestScene) {
                newScene = new TestScene((Owner.Scene as TestScene).number+1);
            }
                else 
            {
                newScene = new MyScene();
            }
            ScreenContext screenContext = new ScreenContext(newScene);
            WaveServices.ScreenContextManager.To(screenContext);
        }
        private Vector2 GetMousePosition()
        {
            return keysBehavior.currentMousePosition;
        }
        private void HandleUI()
        {
            if (activePlayer != null) {
                woodText.Text = string.Format("Wood: {0}", activePlayer.woodAmount);

                WorldObject wo = activePlayer.selectedWO;
                if (wo != null && (fog == null || fog.IsVisible(wo.GetX(), wo.GetY())))
                {
                    stopButton.IsVisible = wo.player == activePlayer;
                    if (wo.Owner.FindComponent<FightBehavior>() != null)
                    {
                        attackButton.IsVisible = wo.player == activePlayer;
                    }
                    else
                    {
                        attackButton.IsVisible = false;
                    }
                    int x = wo.GetX();
                    int y = wo.GetY();
                    coordinateText.Text = string.Format("({0},{1}) ", x,y);
                    woNameText.Text = string.Format("{0}, Status: {1}, {2}", wo.GetWoName(), wo.GetAction(), wo.attacking ? "Aggressive" : "Passive"); 
                    healthText.Text = string.Format("{0}/{1}: {2}", wo.GetHealth(), wo.GetMaxHealth(), wo.player != null ? wo.player.playerName : "No player");
                    UpdatePersonUI(wo);
                }
                else
                {
                    stopButton.IsVisible = false;
                    attackButton.IsVisible = false;
                    coordinateText.Text = string.Empty;
                    woNameText.Text = string.Empty;
                    healthText.Text = string.Empty;
                    ClearPersonUI();

                }
            }
            else
            {
                stopButton.IsVisible = false;
                attackButton.IsVisible = false;
                coordinateText.Text = string.Empty;
                woNameText.Text = string.Empty;
                healthText.Text = string.Empty;
                woodText.Text = string.Empty;
                ClearPersonUI();

            }

        }
        /**
         * <summary>
         * UI ignoring fog, shows everything and works with the tile the mouse is pointing at
         * ONLY for debug and developing purposes. like an editor
         * </summary>
         */
        private void HandleDebugUI()
        {
            if (activePlayer != null)
            {
                woodText.Text =string.Format( "Wood: {0}" , activePlayer.woodAmount.ToString());
                if (activePlayer.selectedWO != null)
                {
                    //Show the speed slider controller
                    speedSlider.Value = (int)(activePlayer.selectedWO.genericSpeed * 10);
                    speedSlider.IsVisible = true;
                    stopButton.IsVisible = activePlayer.selectedWO.player==activePlayer;

                    attackButton.IsVisible = activePlayer.selectedWO.player == activePlayer&& activePlayer.selectedWO.Owner.FindComponent<FightBehavior>() != null;

                }
                else
                {
                    speedSlider.IsVisible = false;
                    stopButton.IsVisible = false;
                    attackButton.IsVisible = false;
                }
            }
            else
            {
                speedSlider.IsVisible = false;
                stopButton.IsVisible = false;
                attackButton.IsVisible = false;
                woodText.Text = string.Empty;
            }
            LayerTile tile = map.GetTileByWorldPosition(GetMousePosition());
            if (tile != null)
            {
                coordinateText.Text = string.Format("({0},{1}) occupied:{2}", tile.X , tile.Y , map.IsTileOccupied(tile.X, tile.Y));
                WorldObject wo = map.GetMobile(tile.X, tile.Y);
                WorldObject wo2 = map.GetWorldObject(tile.X, tile.Y);
                if (wo == null || (wo2 != null && activePlayer!=null && activePlayer.selectedWO == wo2))
                    wo = wo2;
                if (wo != null)
                {
                    woNameText.Text = string.Format("{0}{1}, Status: {2}, {3}", 
                        wo.GetWoName(), 
                        activePlayer != null && wo == activePlayer.selectedWO ? ": selected" : string.Empty, 
                        wo.GetAction(), 
                        wo.attacking ? "Aggressive" : "Passive") ;
                    healthText.Text = string.Format("{0}/{1}: {2}", wo.GetHealth(), wo.GetMaxHealth(), wo.player != null ? wo.player.playerName : "No player");

                    UpdatePersonUI(wo);

                }
                else
                {
                    woNameText.Text = string.Empty;
                    healthText.Text = string.Empty;

                    ClearPersonUI();
                    
                }
            }
            else
            {
                coordinateText.Text = string.Empty;
                woNameText.Text = string.Empty;
                healthText.Text = string.Empty;
                ClearPersonUI();
            }
        }
        /**
         * <summary>
         * Shows the person skill points if it is a person, clears otherwise
         * </summary>
         */
        private void UpdatePersonUI(WorldObject wo)
        {
            if (wo.GetWoName() == "Person")
            { 
                Person person = wo.Owner.FindComponent<Person>();
                buildSkill.Text = string.Format("Building {0}", person.buildings);
                fightSkill.Text = string.Format("Fight {0}", person.fights);
                chopSkill.Text = string.Format("Chop {0}", person.trees);
                healSkill.Text = string.Format("Heal {0}", person.heals);
            }
            else
            {
                ClearPersonUI();
            }
        }
        private void UpdatePersonAttacking(WorldObject wo)
        {
            wo.attacking = !wo.attacking;
        }
        /**
         * <summary>
         * Clears the person skill points labels
         * </summary>
         */
        private void ClearPersonUI()
        {
            buildSkill.Text = string.Empty;
            fightSkill.Text = string.Empty;
            chopSkill.Text = string.Empty;
            healSkill.Text = string.Empty;
        }

        private WorldObject IsSomeoneAdjacent(LayerTile tile,Player p)
        {
 
            if (tile == null)
            {
                return null;
            }
            else
            {
                WorldObject adjacent = null;
                Trace.WriteLine(tile.X + ", " + tile.Y);
                if (map.InBounds(tile.X + 1, tile.Y))
                {
                    adjacent = map.GetMobile(tile.X + 1, tile.Y);
                    if (adjacent != null && adjacent.player == p)
                    {
                        return adjacent;
                    }
                }
                if (map.InBounds(tile.X - 1, tile.Y))
                {
                    adjacent = map.GetMobile(tile.X - 1, tile.Y);
                    if (adjacent != null && adjacent.player == p)
                    {
                        return adjacent;
                    }
                }
                if (map.InBounds(tile.X, tile.Y + 1))
                {
                    adjacent = map.GetMobile(tile.X, tile.Y + 1);
                    if (adjacent != null && adjacent.player == p)
                    {
                        return adjacent;
                    }
                }
                if (map.InBounds(tile.X, tile.Y - 1))
                {
                    adjacent = map.GetMobile(tile.X, tile.Y - 1);
                    if (adjacent != null && adjacent.player == p)
                    {
                        return adjacent;
                    }
                }

            }            

            return null;
        }
      public bool MouseOverGUI()
        {
            return MouseEnterBehavior.numHover > 0;
        }
    }
    }
