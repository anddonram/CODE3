using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    /**
     * <summary>
     * Interface for actions involving two WOs, commonly through implementing a behavior
     * </summary>
     */
   public interface ActionBehavior
    {
        /**
         * <summary>
         * returns whether the WOs can interact each other
         * </summary>
         */
        bool CanAct(WorldObject other);

        /**
         * <summary>
         * Starts the action between both actors through its behavior
         * </summary>
         */
        void Act(WorldObject other);
        /**
         * <summary>
         * Returns the command related with the action
         * </summary>
         */
        CommandEnum GetCommand();
        /**
        * <summary>
        * Returns whether a button can be shown for an action
        * </summary>
        */
        bool CanShowButton(WorldObject otherWO);

        string GetCommandName(WorldObject otherWO);
    }
}
