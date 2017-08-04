using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.TiledMap;

namespace Codigo.Components
{
    /**
     * <summary>
     * When selecting a mobile WO,
     * prints, for each tile, the values and scores for computed in the d* path
     * </summary>
     */
    public class DStarDrawable : SceneBehavior
    {
        /**
         * <summary>
         * The selected WO's dstar, if its a mobile
         * </summary>
         */
        private DStarLite dstar;
        /**
         * <summary>
         * A grid of boxes, one for each tile
         * </summary>
         */
        private TextBlock[,] boxes;
        /**
         * <summary>
         * The map width
         * </summary>
         */
        private int width;
        /**
         * <summary>
         * The map height
         * </summary>
         */
        private int height;
        /**
         * <summary>
         * Initializes all stuff: the grid, etc.
         * </summary>
         */
        public void Set()
        {
            width = Map.map.width;
            height = Map.map.height;
            boxes = new TextBlock[width, height];
            int dist = 100;
            Grid grid = new Grid()
            {
                Margin = new Thickness(dist, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = WaveServices.Platform.ScreenHeight,
                Width = WaveServices.Platform.ScreenWidth-dist
            };
            for (int j = 0; j < height; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100/width, GridUnitType.Proportional) });
            }
            for (int i = 0; i < width; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

                for (int j = 0; j < height; j++)
                {
                    boxes[i, j] = new TextBlock(string.Format("buttonic{0},{1}",i,j))
                    {
                        TextWrapping = true,
                        Foreground = Color.Black
                    };
                    boxes[i, j].SetValue(GridControl.RowProperty, i);
                    boxes[i, j].SetValue(GridControl.ColumnProperty, j);
                    grid.Add(boxes[i, j]);
                  
                }
            }
            
            Scene.EntityManager.Add(grid);
            WaveServices.Layout.PerformLayout(Scene);
        }
        /**
         * <summary>
         * Shows the scores for a computing path if selecting a mobile
         * </summary>
         */
        protected override void Update(TimeSpan gameTime)
        {
            if (UIBehavior.ui.activePlayer != null && UIBehavior.ui.activePlayer.selectedWO != null && UIBehavior.ui.activePlayer.selectedWO.IsMobile())
            {
                if (dstar == null)
                {
                    dstar = UIBehavior.ui.activePlayer.selectedWO.Owner.FindComponent<DStarLite>();
                }
                for (int i = 0; i < width; i++)
                {

                    for (int j = 0; j < height; j++)
                    {
                        LayerTile t = Map.map.GetTileByMapCoordinates(i,j);

                        Vector2 fscore = Vector2.Zero;
                        dstar.fScore.TryGetValue(t, out fscore);

                        float rhs = float.PositiveInfinity;
                        dstar.rhs.TryGetValue(t, out rhs);

                        float gscore = float.PositiveInfinity;
                        dstar.gScore.TryGetValue(t, out gscore);
                        boxes[t.Y, t.X].Text = string.Format("({3},{4}), fs:({0}), rhs:{1}, gs:{2}", fscore, rhs, gscore,i,j);
                    }
                }
            }
            else
            {
                dstar = null;
            }
        }

        protected override void ResolveDependencies()
        {

        }
    }
}
