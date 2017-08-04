using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Codigo.VictoryConditions
{
    public class NoPeople : VictoryCondition
    {
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
                            Trace.WriteLine(p2.Name + ": " + p2.GetPeople().Count);
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
