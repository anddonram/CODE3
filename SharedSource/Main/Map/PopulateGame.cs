using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework.Managers;

namespace Codigo
{
    public class PopulateGame
    {
        private int width = 12;
        private int height = 20;

        private int woodStart = 4;
        private int woodEnd = 16;
        /**
  * <summary>
  * The extension of the wood relative to the total map
  * </summary>
  */
        private float woodPercentage = 0.6f;

        private float obstacleFrequency = 0.05f;
        private float emptyFrequency = 0.05f;
        private Random rnd = new Random();

        public const int castleWidth = 4;
        public const int castleHeight = 2;

        private int riverSize = 2;

        public PopulateGame()
        {
            width = Map.map.width;
            height = Map.map.height;
            woodStart = (int)(height * (1 - woodPercentage) / 2);
            woodEnd = (int)(height * (1 + woodPercentage) / 2);
        }
        /**
         * The number of persons we will have available at the start of the game
         */
        private int personNum = 4;
       
        public void SetPlayer(Player p, int playerNum, int maxPlayers)
        {
            
            PopulateInitialPosition initial = PopulateInitialPosition.GetPlayerPosition(playerNum, maxPlayers,
                width, height, castleWidth, castleHeight);
            int x = initial.x;
            int y = initial.y;

            UIBehavior.ui.CreateCastleToTile(p, x, y, castleWidth, castleHeight);
  
            //create person    
            for (int i = 0; i < personNum; i++)
            {
                UIBehavior.ui.CreateToTile("Person", x + i, y, p);    
            }
        }
        public void SetWoods()
        {
            for (int i=woodStart;i<woodEnd;i++)
                for(int j = 0; j < width; j++)
                {
                    if (rnd.NextDouble() <= obstacleFrequency)
                    {
                        UIBehavior.ui.CreateToTile("Rock", j, i, null);
                    }
                    if (rnd.NextDouble() <= emptyFrequency)
                    {    
                    }
                    else {
                        UIBehavior.ui.CreateToTile("Tree", j, i, null);
                    }
                }
        }

      
        public void SetRiver()
        {
            for (int i = (height - riverSize) / 2; i < (height + riverSize) / 2; i++)
                for (int j = 0; j < width; j++)
                {
                    UIBehavior.ui.CreateToTile("Water",j,i,null);
                }
        }

    }
}

