using System;

namespace Districts.New.Interfaces
{
    public enum ActionType
    {
        Taken,
        Dropped,
        Deleted
    }

    public interface iRecord
    {
        ActionType Action { get; }
        string Subject { get; }
        DateTime Date { get;  }
    }
}
