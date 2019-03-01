using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.New.Implementation;
using Districts.New.Interfaces;

namespace Districts.Parser.v2
{
    class Parser
    {
        private readonly IAppSettings _settings;

        public Parser(IAppSettings settings)
        {
            _settings = settings;
        }
    }
}
