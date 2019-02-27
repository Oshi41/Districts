namespace Districts.Helper
{
    public interface IMessageHelper
    {
        void ShowDone();
        void ShowAwait();
        void ShowDoneBubble(string header = "Завершение работы", string text = "Готово", int seconds = 2);
    }
}