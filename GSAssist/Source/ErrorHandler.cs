using System.Windows.Forms;

namespace GSAssist
{
    class ErrorHandler
    {
        public static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "GS Assist", MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
    }
}
