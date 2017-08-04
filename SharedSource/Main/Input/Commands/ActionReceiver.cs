using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    /**
     * <summary>
     * Generic class for almost all implemented actions, except movement actions
     * </summary>
     */
    class ActionReceiver : Receiver
    {
        ActionBehavior sender;
        WorldObject receiver;

        public ActionReceiver(ActionBehavior orig, WorldObject dest)
        {
            sender = orig;
            receiver = dest;
        }
        public void Execute()
        {
            sender.Act(receiver);
        }
    }
}
