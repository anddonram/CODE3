using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo
{
    interface Command
    {

        void Execute();
        void Undo();
    }
}
