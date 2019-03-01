using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Implementation.Args
{
    class MessageArgs : EventArgs
    {
        public List<string> Streets { get; }

        public MessageArgs(List<string> streets)
        {
            Streets = streets;
        }
    }
}
