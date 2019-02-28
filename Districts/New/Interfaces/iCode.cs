namespace Districts.New.Interfaces
{
    public enum CodeStatus
    {
        Good,
        NotWorking,
        Restricted,
    }

    interface iCode
    {
        string Text { get; }
        CodeStatus Status { get; }
    }
}
