using System.Drawing;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Districts.Helper
{
    public interface IMessageHelper
    {
        void ShowDone();
        void ShowAwait();
        void ShowDoneBubble(string header = "Завершение работы", string text = "Готово", int seconds = 2);
    }

    public class MessageHelper : IMessageHelper
    {
        private static IMessageHelper _instanse;
        public static IMessageHelper Instance => _instanse ?? (_instanse = new MessageHelper());


        public void ShowDone()
        {
            MessageBox.Show("Готово");
        }

        public void ShowAwait()
        {
            ShowDoneBubble("Ожидайте", "По окончанию действия программа уведомит вас об окончании");
        }

        public void ShowDoneBubble(string header = "Завершение работы",
            string text = "Готово",
            int seconds = 2)
        {
            var icon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = text,
                BalloonTipTitle = header,
                Visible = true
            };
            icon.ShowBalloonTip(1000 * seconds);
        }
    }
}