using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    /**
     * <summary>
     * Holds information to execute the action
     * </summary>
     */
    interface Receiver
    {
        /**
         * <summary>
         * Executes an action
         * </summary>
         */
       void Execute();
    }
}
