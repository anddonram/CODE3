using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    public class PopulateInitialPosition
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool castleReversed { get; private set; }

        public static PopulateInitialPosition GetPlayerPosition(int playerNum, int maxPlayers,int width,int height, int castleWidth, int castleHeight)
        {
            PopulateInitialPosition initial = null;
            switch (maxPlayers)
            {
                case 2:
                    initial = GetPositionFor2(playerNum,width,height,  castleWidth,  castleHeight);
                    break;
                case 3:
                case 4:
                    initial = GetPositionFor3Or4(playerNum, width, height, castleWidth, castleHeight);
                    break;
                default:
                    break;
            }
            return initial;
        }
        /**
         * <summary>
         * 2 players, one in front of the other
         * </summary>
         */
        private static PopulateInitialPosition GetPositionFor2(int playerNum, int width, int height, 
            int castleWidth, int castleHeight)
        {
            PopulateInitialPosition res = new PopulateInitialPosition();
            res. x = (width-castleWidth)/2;
            res. y = 0;
            if (playerNum == 1)
            {
               res.y = height -castleHeight;
            }
            res.castleReversed = false;
            return res;
        }

        /**
         * <summary>
         * 3 or 4 players, in corners
         * </summary>
         */

        private static PopulateInitialPosition GetPositionFor3Or4(int playerNum, int width, int height, int castleWidth, int castleHeight)
        {
            PopulateInitialPosition res = new PopulateInitialPosition();
            if (playerNum == 0 || playerNum == 2) 
                res.x = 0;
            else
                res.x = width-castleWidth;

            if (playerNum == 0 || playerNum == 3)
                res.y = 0;
            else     
                res.y = height - castleHeight;
            res.castleReversed = false;
 
            return res;
        }

       

    }
}
