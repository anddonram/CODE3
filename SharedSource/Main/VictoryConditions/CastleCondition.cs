using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo.VictoryConditions
{
    class CastleCondition : VictoryCondition
    {
        public override string GetName()
        {
            return "Victory by castle destruction";
        }

        public override bool PlayerMeetsCondition(Player p)
        {
            bool aux = true;
            if (p != null && p.castle != null)
            {
                foreach (Player p2 in uibehavior.GetPlayers())
                {
                    if (p2 != p)
                    {
                        if (p2.castle != null)
                        {
                            aux = false;
                            break;
                        }
                    }
                }
                return aux;
            }
            else
                return false;
        }
    }
}
