﻿using System.Runtime.CompilerServices;
using DistrictsLib.Interfaces;

namespace DistrictsLib.Implementation.ChangesNotifier
{
    public class SimpleNotifier : IChangeNotifier
    {
        private bool _isChanged;

        public void Notify<T>(T old, T val, [CallerMemberName] string member = null)
        {
            _isChanged = true;
        }

        public void SetChange(string member = null)
        {
            Notify<object>(null, null, member);
        }

        public bool IsChanged()
        {
            return _isChanged;
        }
    }
}
