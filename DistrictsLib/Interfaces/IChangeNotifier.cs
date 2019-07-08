using System.Runtime.CompilerServices;

namespace DistrictsLib.Interfaces
{
    public interface IChangeNotifier
    {
        void Notify<T>(T old, T val, [CallerMemberName] string member = null);
        void SetChange([CallerMemberName] string member = null);
        bool IsChanged();
    }
}
