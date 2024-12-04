using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TransportRental
{
    public partial class Registration : Form
    {
        private TransportRentalDataSetTableAdapters.QueriesTableAdapter db = new TransportRentalDataSetTableAdapters.QueriesTableAdapter();

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

        public Registration()
        {
            InitializeComponent();
            this.Text = string.Empty;
            this.ControlBox = false;
        }

        private void FormEntryLabel_Click(object sender, EventArgs e) => this.Close();

        private bool IsLoginExists(string login)
        {
            if ((bool)db.CheckLogin(login))
                MessageBox.Show("Ошибка! Пользователь с данным логином уже существует");
            return (bool)db.CheckLogin(login);
        }

        private bool IsCheckPhone()
        {
            if ((bool)db.CheckPhone(PhoneMaskedTextBox.Text))
            {
                MessageBox.Show("Ошибка! Пользователь с данным номером телефона уже существует");
                return false;
            }

            int countNum = 0;

            foreach (char a in PhoneMaskedTextBox.Text)
                if (Char.IsNumber(a))
                    countNum++;
            if (countNum == 11)
                return true;
            else
                MessageBox.Show("Ошибка! Не правильно набран номер телефона");

            return false;
        }

        private void RegistrationUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (object a in RegPanel.Controls)
                    if (a != PatronymicTextBox && a is TextBox && ((TextBox)a).Text == "")
                    {
                        MessageBox.Show("Ошибка! Заполните все поля");
                        return;
                    }

                if (IsLoginExists(LoginTextBox.Text))
                    return;

                if (!IsCheckPhone()) 
                    return;

                db.AddUser(LoginTextBox.Text,
                           PasswordTextBox.Text,
                           NameTextBox.Text,
                           SurnameTextBox.Text,
                           PatronymicTextBox.Text,
                           BirthdayDateTimePicker.Value.Date,
                           PhoneMaskedTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка! Что-то пошло не так");
            }

            Login.loginUser = LoginTextBox.Text;
            Login.passUser = PasswordTextBox.Text;

            this.Hide();
            User user = new User();
            user.ShowDialog();
            this.Close();
        }
    }
}
