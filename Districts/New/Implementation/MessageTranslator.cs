using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class MessageTranslator : IMessage
    {
        private event EventHandler<EventArgs> _messageReceived;

        private Dictionary<object, EventHandler<EventArgs>> _invocationMap = 
            new Dictionary<object, EventHandler<EventArgs>>();

        public void Unsubscribe<T>(EventHandler<T> callback) where T : EventArgs
        {
            if (_invocationMap.ContainsKey(callback))
            {
                _messageReceived -= _invocationMap[callback];
                _invocationMap.Remove(callback);
            }
        }

        public void Subscribe<T>(EventHandler<T> callback) where T : EventArgs
        {
            if (_invocationMap.ContainsKey(callback))
            {
                throw new Exception("Was already subscribed!");
            }

            _invocationMap[callback] = (sender, args) =>
            {
                if (args is T exactArgs)
                {
                    callback(sender, exactArgs);
                }
            };

            _messageReceived += _invocationMap[callback];
        }

        public void OnSendMessage(EventArgs args)
        {
            _messageReceived?.Invoke(this, args);
        }
    }
}
