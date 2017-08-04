using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    /**
     * <summary>
     * Command for executing common actions
     * </summary>
     */
    class ActionCommand : Command
    {
   
        Receiver rec;
        public ActionCommand(Receiver rec)
        {
            this.rec = rec;
        }

        public void Execute()
        {
            rec.Execute();
        }

        public void Undo()
        {
            
        }
    }
}
