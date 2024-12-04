using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TransportRental
{
    public partial class Login : Form
    {
        public static string loginUser,
                             passUser;

        private void CloseButton_Click(object sender, EventArgs e) => Application.Exit();

        private void MaximizeButton_Click(object sender, EventArgs e) => 
            this.WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;

        private void MinimizeButton_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void TitleBarPanel_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        public Login()
        {
            InitializeComponent();
            this.Text = string.Empty;
            this.ControlBox = false;
        }

        private void EntryButton_Click(object sender, EventArgs e)
        {
            TransportRentalDataSetTableAdapters.QueriesTableAdapter connectDB = new TransportRentalDataSetTableAdapters.QueriesTableAdapter();
            string checkResult = connectDB.UserVerification(LoginTextBox.Text, PasswordTextBox.Text);

            loginUser = LoginTextBox.Text;
            passUser = PasswordTextBox.Text;
            
            this.Hide();

            if (checkResult == "Block")
                MessageBox.Show("Ошибка! Данный пользователь заблокирован");
            else if (checkResult == "User")
            {
                User user = new User();
                user.ShowDialog();
            }
            else if (checkResult == "Admin")
            {
                Admin developer = new Admin();
                developer.ShowDialog();
            }
            else 
                MessageBox.Show("Ошибка! Данного пользователя не существует");
            try
            {
                this.Show();
            }
            catch { }
        }

        private void FormRegistrationLabel_Click(object sender, EventArgs e)
        {
            this.Hide();
            Registration registration= new Registration();
            registration.ShowDialog();
            try
            {
                this.Show();
            }
            catch { }
        }
    }
}
