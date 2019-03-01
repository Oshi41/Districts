using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    interface IMessage
    { 
        void Unsubscribe<T>(EventHandler<T> callback) where T : EventArgs;

        void Subscribe<T>(EventHandler<T> callback) where T : EventArgs;
            
        void OnSendMessage(EventArgs args);
    }
}
