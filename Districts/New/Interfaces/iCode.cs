namespace Districts.New.Interfaces
{
    public enum CodeStatus
    {
        Good,
        NotWorking,
        Restricted,
    }

    public interface iCode
    {
        string Text { get; }
        CodeStatus Status { get; }
    }
}
