using System;
using System.Runtime.CompilerServices;

namespace Districts.New.Implementation
{
    public interface ITrace
    {
        void Write(string message);
        void Write(Exception e,
            string message = null,
            [CallerMemberName] string file = null,
            [CallerLineNumber] int lineNumber = -1);
    }
}
