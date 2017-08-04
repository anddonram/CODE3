using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    class Invoker
    {
        private Command cmd;
        public void SetCommand(Command cmd)
        {
            this.cmd = cmd;
        }
        public void ExecuteCommand()
        {
            cmd.Execute();
        }
    }
}
