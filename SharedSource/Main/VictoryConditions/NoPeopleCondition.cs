using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Codigo.VictoryConditions
{
    public class NoPeopleCondition : VictoryCondition
    {
        public override string GetName()
        {
            return "Victory by enemy's wipeout";
        }
        public override bool PlayerMeetsCondition(Player p)
        {
            bool aux = true;
            if (p != null && p.GetPeople().Count > 0)
            {
                foreach (Player p2 in uibehavior.GetPlayers())
                {
                    if (p2 != p)
                    {
                        if (p2.GetPeople().Count > 0)
                        {
                            aux=false;
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
