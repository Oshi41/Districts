﻿using System.Drawing;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Districts.Helper
{
    public static class MessageHelper
    {
        public static void ShowMessage(string header, string message)
        {
            MessageBox.Show(message, header);
        }
        public static void ShowDone()
        {
            MessageBox.Show("Готово");
        }

        public static void ShowAwait()
        {
            ShowDoneBubble("Ожидайте", "По окончанию действия программа уведомит вас об окончании");
        }

        public static void ShowDoneBubble(string header = "Завершение работы",
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