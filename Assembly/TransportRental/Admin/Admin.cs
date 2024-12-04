using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace TransportRental
{
    public partial class Admin : Form
    {
        private TransportRentalDataSetTableAdapters.QueriesTableAdapter db = new TransportRentalDataSetTableAdapters.QueriesTableAdapter();

        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Panel activePanel;

        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.ColorList.Count);
            while (tempIndex == index)
                index = random.Next(ThemeColor.ColorList.Count);
            tempIndex = index;
            string color = ThemeColor.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }

        private void ActivateButton(object senderButton)
        {
            if (senderButton == null)
                return;

            if (currentButton != (Button)senderButton)
            {
                DisableButton(MenuPanel);
                Color color = SelectThemeColor();
                currentButton = (Button)senderButton;
                currentButton.BackColor = color;
                currentButton.ForeColor = Color.White;
                currentButton.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                TitleBarPanel.BackColor = color;
                LogoPanel.BackColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                ThemeColor.PrimaryColor = color;
                ThemeColor.SecondaryColor = ThemeColor.ChangeColorBrightness(color, -0.3);

            }
        }
        private void DisableButton(Panel senderPanel)
        {
            foreach (Control previousBtn in senderPanel.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }

        private void OpenPanel(Panel openPanel, object senderButton)
        {
            if (activePanel != null)
                activePanel.Visible = false;

            ActivateButton(senderButton);
            activePanel = openPanel;
            openPanel.Dock = DockStyle.Fill;
            openPanel.BringToFront();
            openPanel.Visible = true;
            TitleLabel.Text = ((Button)senderButton).Text.ToUpper();
            SetColorControlsPanel(openPanel, senderButton);
        }

        private void SetColorControlsPanel(Panel openPanel, object menuButton)
        {
            DisableButton(openPanel);
            foreach (object a in openPanel.Controls)
            {
                if (a is Button)
                {
                    ((Button)a).BackColor = ((Button)menuButton).BackColor;
                    ((Button)a).ForeColor = ((Button)menuButton).ForeColor;
                }
                else if (a is DataGridView)
                    ((DataGridView)a).BackgroundColor = ThemeColor.ChangeColorBrightness(((Button)menuButton).BackColor, 0.3);
                else if (a is Label && ((Label)a).Font.Bold)
                    ((Label)a).ForeColor = ThemeColor.ChangeColorBrightness(((Button)menuButton).BackColor, -0.3);
            }
        }

        private void AdminsButton_Click(object sender, EventArgs e)
        {
            OpenPanel(AdminsPanel, sender);
            SetColorControlsPanel(AddAdminPanel, sender);
            SetColorControlsPanel(EditAdminPanel, sender);
        }

        private void CarButton_Click(object sender, EventArgs e) => OpenPanel(CarPanel, sender);

        private void AccountButton_Click(object sender, EventArgs e) => OpenPanel(AccountPanel, sender);

        private void UsersButton_Click(object sender, EventArgs e) => OpenPanel(UsersPanel, sender);

        private void RentalButton_Click(object sender, EventArgs e) => OpenPanel(RentalPanel, sender);

        private void ArchiveRentalButton_Click(object sender, EventArgs e) => OpenPanel(ArсhiveRentalPanel, sender);

        private void ArchiveUsersButton_Click(object sender, EventArgs e) => OpenPanel(ArchiveUsersPanel, sender);

        private void LogoutAdminButton_Click(object sender, EventArgs e) => this.Close();

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

        public Admin()
        {
            InitializeComponent();
            random = new Random();
            this.Text = string.Empty;
            this.ControlBox = false;
        }

        private void GetConnectTable(string table)
        {
            switch (table)
            {
                case "Admins":
                case "1":
                    this.admins_not_id_levelTableAdapter.Fill(this.transportRentalDataSet.Admins_not_id_level);
                    AdminsDataGridView.DataSource = adminsnotidlevelBindingSource;
                    AdminsDataGridView.Columns[0].Visible = false;
                    AdminsDataGridView.Columns[1].HeaderText = "Действующий";
                    AdminsDataGridView.Columns[2].HeaderText = "Логин";
                    AdminsDataGridView.Columns[3].HeaderText = "Пароль";
                    AdminsDataGridView.Columns[4].HeaderText = "Имя";
                    AdminsDataGridView.Columns[5].HeaderText = "Фамилия";
                    AdminsDataGridView.Columns[6].HeaderText = "Отчество";
                    AdminsDataGridView.Columns[7].HeaderText = "Уровень доступа";
                    break;
                case "Users":
                case "2":
                    this.userTableAdapter.Fill(this.transportRentalDataSet.User);
                    UsersDataGridView.DataSource = userBindingSource;
                    UsersDataGridView.Columns[0].Visible = false;
                    UsersDataGridView.Columns[1].HeaderText = "Действующий";
                    UsersDataGridView.Columns[2].HeaderText = "Логин";
                    UsersDataGridView.Columns[3].Visible = false;
                    UsersDataGridView.Columns[4].HeaderText = "Имя";
                    UsersDataGridView.Columns[5].HeaderText = "Фамилия";
                    UsersDataGridView.Columns[6].HeaderText = "Отчество";
                    UsersDataGridView.Columns[7].HeaderText = "Дата рождения";
                    UsersDataGridView.Columns[8].HeaderText = "Номер телефона";
                    UsersDataGridView.Columns[9].HeaderText = "Дата регистрации";
                    break;
                case "Cars":
                case "3":
                    this.cars_for_adminsTableAdapter.Fill(this.transportRentalDataSet.Cars_for_admins);
                    CarsDataGridView.DataSource = carsforadminsBindingSource;
                    CarsDataGridView.Columns[0].Visible = false;
                    CarsDataGridView.Columns[1].Visible = false;
                    CarsDataGridView.Columns[2].HeaderText = "Логин пользователя";
                    CarsDataGridView.Columns[3].HeaderText = "Название автомобиля";
                    CarsDataGridView.Columns[4].HeaderText = "Информация об автомобиле";
                    CarsDataGridView.Columns[5].HeaderText = "Цена за аренду автомобиля";
                    CarsDataGridView.Columns[6].HeaderText = "Цена за аренду экипажа";
                    CarsDataGridView.Columns[7].HeaderText = "Наличие экипажа";
                    break;
                case "Rental":
                case "4":
                    this.rental_for_adminsTableAdapter.Fill(this.transportRentalDataSet.Rental_for_admins);
                    RentalDataGridView.DataSource = rentalforadminsBindingSource;
                    RentalDataGridView.Columns[0].Visible = false;
                    RentalDataGridView.Columns[1].HeaderText = "Арендатор";
                    RentalDataGridView.Columns[2].HeaderText = "Арендодатель";
                    RentalDataGridView.Columns[3].Visible = false;
                    RentalDataGridView.Columns[4].HeaderText = "Название автомобиля";
                    RentalDataGridView.Columns[5].HeaderText = "Информация об автомобиле";
                    RentalDataGridView.Columns[6].HeaderText = "Аренда экипажа";
                    RentalDataGridView.Columns[7].HeaderText = "Цена за аренду автомобиля";
                    RentalDataGridView.Columns[8].HeaderText = "Цена за аренду экипажа";
                    RentalDataGridView.Columns[9].HeaderText = "Цена аренды";
                    RentalDataGridView.Columns[10].HeaderText = "Дата начала аренды";
                    RentalDataGridView.Columns[11].HeaderText = "Дата окончания аренды";
                    break;
                case "StartedAndPastRental":
                case "5":
                    this.startedAndPastRentalTableAdapter.Fill(this.transportRentalDataSet.StartedAndPastRental, DateTime.Now.ToString());
                    RentalDataGridView.DataSource = startedAndPastRentalBindingSource;
                    RentalDataGridView.Columns[1].HeaderText = "Арендатор";
                    RentalDataGridView.Columns[2].HeaderText = "Арендодатель";
                    break;
                case "BlockedUsers":
                case "6":
                    this.blockedUsersTableAdapter.Fill(this.transportRentalDataSet.BlockedUsers);
                    UsersDataGridView.DataSource = blockedUsersBindingSource;
                    break;
                case "UnblockedUsers":
                case "7":
                    this.unblockedUsersTableAdapter.Fill(this.transportRentalDataSet.UnblockedUsers);
                    UsersDataGridView.DataSource = unblockedUsersBindingSource;
                    break;
                case "ArchiveRental":
                case "8":
                    this.archiveRentalTableAdapter.Fill(this.transportRentalDataSet.ArchiveRental);
                    ArchiveRentalDataGridView.DataSource = archiveRentalBindingSource;
                    ArchiveRentalDataGridView.Columns[0].Visible = false;
                    ArchiveRentalDataGridView.Columns[1].HeaderText = "Арендатор";
                    ArchiveRentalDataGridView.Columns[2].HeaderText = "Арендодатель";
                    ArchiveRentalDataGridView.Columns[3].HeaderText = "Название автомобиля";
                    ArchiveRentalDataGridView.Columns[4].HeaderText = "Информация об автомобиле";
                    ArchiveRentalDataGridView.Columns[5].HeaderText = "Цена за аренду автомобиля";
                    ArchiveRentalDataGridView.Columns[6].HeaderText = "Аренда экипажа";
                    ArchiveRentalDataGridView.Columns[7].HeaderText = "Цена за аренду экипажа";
                    ArchiveRentalDataGridView.Columns[8].HeaderText = "Цена аренды";
                    ArchiveRentalDataGridView.Columns[9].HeaderText = "Дата начала аренды";
                    ArchiveRentalDataGridView.Columns[10].HeaderText = "Дата окончания аренды";
                    break;
                case "ArchiveUsers":
                case "9":
                    this.archiveUsersTableAdapter.Fill(this.transportRentalDataSet.ArchiveUsers);
                    ArchiveUsersDataGridView.DataSource = archiveUsersBindingSource;
                    ArchiveUsersDataGridView.Columns[0].Visible = false;
                    ArchiveUsersDataGridView.Columns[1].HeaderText = "Логин пользователя";
                    ArchiveUsersDataGridView.Columns[2].HeaderText = "Имя";
                    ArchiveUsersDataGridView.Columns[3].HeaderText = "Фамилия";
                    ArchiveUsersDataGridView.Columns[4].HeaderText = "Отчество";
                    ArchiveUsersDataGridView.Columns[5].HeaderText = "Дата рождения";
                    ArchiveUsersDataGridView.Columns[6].HeaderText = "Номер телефона";
                    ArchiveUsersDataGridView.Columns[7].HeaderText = "Дата регистрации";
                    ArchiveUsersDataGridView.Columns[8].HeaderText = "Дата удаления аккаунта";
                    break;
                case "AccessLevel":
                case "10":
                    this.accessLevelTableAdapter.Fill(this.transportRentalDataSet.AccessLevel);
                    break;
                default:
                    MessageBox.Show("Ошибка! Данной таблицы не существует - ", table);
                    break;
            }
        }
        
        private void GetAccess(int accessLevel)
        {
            if (accessLevel < 1)
                return;

            AdminsButton.Enabled = true;
            UsersButton.Enabled = true;
            CarsButton.Enabled = true;
            RentalButton.Enabled = true;
            ArchiveRentalButton.Enabled = true;
            ArchiveUsersButton.Enabled = true;

            if (accessLevel < 2)
                return;

            UnlockUserButton.Enabled = true;
            BlockUserButton.Enabled = true;

            if (accessLevel < 3)
                return;

            if (accessLevel < 4)
                return;

            RemoveCarPublicationButton.Enabled = true;

            if (accessLevel < 5)
                return;

            DelUserButton.Enabled = true;

            if (accessLevel < 6)
                return;

            if (accessLevel < 7)
                return;

            if (accessLevel < 8)
                return;
        }

        private void GetRowsSum(Label sumLabel, DataGridView dataGrid, bool isSearch = false)
        {
            if (!isSearch)
            {
                sumLabel.Text = dataGrid.Rows.Count.ToString();                
                return;
            }

            int sum = 0;
            foreach (DataGridViewRow row in dataGrid.Rows)
                if (row.Visible)
                    sum++;
            sumLabel.Text = sum.ToString();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            GetConnectTable("Admins");
            GetConnectTable("Users");
            GetConnectTable("Cars");
            GetConnectTable("Rental");
            GetConnectTable("ArchiveRental");
            GetConnectTable("ArchiveUsers");
            GetConnectTable("AccessLevel");
            

            ConnectDB connectDB = new ConnectDB();

            connectDB.OpenConnection();

            SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM [Admins_not_id_level] WHERE login = '{Login.loginUser}' AND password = '{Login.passUser}'", connectDB.GetConnetcion());
            DataTable dataTable = new DataTable();

            adapter.Fill(dataTable);

            IdLabel.Text           = dataTable.Rows[0][0].ToString();
            LoginTextBox.Text      = dataTable.Rows[0][2].ToString();
            PasswordTextBox.Text   = dataTable.Rows[0][3].ToString();
            NameTextBox.Text       = dataTable.Rows[0][4].ToString();
            SurnameTextBox.Text    = dataTable.Rows[0][5].ToString();
            PatronymicTextBox.Text = dataTable.Rows[0][6].ToString();
            accessLevelLabel.Text  = dataTable.Rows[0][7].ToString();

            connectDB.CloseConnection();
            
            
            GetRowsSum(SumAdminsLabel, AdminsDataGridView);
            GetRowsSum(SumUsersLabel, UsersDataGridView);
            GetRowsSum(SumCarsLabel, CarsDataGridView);
            GetRowsSum(SumRentalLabel, RentalDataGridView);
            GetRowsSum(SumArchiveRentalLabel, ArchiveRentalDataGridView);
            GetRowsSum(SumArchiveUsersLabel, ArchiveUsersDataGridView);
            
            GetAccess(int.Parse(accessLevelLabel.Text));
            
            SetUpdateDataToEditAdminPanel();
            
        }

        private void GetSaveSelectedRow(string table, DataGridView dataGridView, Label sumLabel = null)
        {
            try
            {
                int rowIndex = dataGridView.CurrentCell.RowIndex;
                GetConnectTable(table);
                try
                {
                    dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[1];
                }
                catch
                {
                    dataGridView.CurrentCell = dataGridView.Rows[rowIndex - 1].Cells[1];
                }
            }
            catch
            {
                GetConnectTable(table);
            }

            if (sumLabel != null)
                GetRowsSum(sumLabel, dataGridView);
        }

        private bool IsLoginExists(string login)
        {
            if (LoginTextBox.Text.ToLower() == Login.loginUser.ToLower() && LoginNewAdminTextBox.Text != login)
                return false;
            if ((bool)db.CheckLogin(login))
            {
                MessageBox.Show("Ошибка! Пользователь с данным логином уже существует");
                return true;
            }
            return false;
        }

        private void AddAdminButton_Click(object sender, EventArgs e)
        {
            if (LoginNewAdminTextBox.Text == "" || PasswordNewAdminTextBox.Text == "")
            {
                MessageBox.Show("Ошибка! Для создания администратора, необходимо ввести логин и пароль");
                return;
            }

            if (PasswordNewAdminTextBox.Text != ConfirmPasswordNewAdminTextBox.Text)
            {
                MessageBox.Show("Ошибка! Пароль не подтвержден");
                return;
            }

            if (IsLoginExists(LoginNewAdminTextBox.Text))
                return;

            db.AddAdmin(LoginNewAdminTextBox.Text,
                        PasswordNewAdminTextBox.Text,
                        NameNewAdminTextBox.Text,
                        SurnameNewAdminTextBox.Text,
                        PatronymicNewAdminTextBox.Text,
                        (int)AccessLevelNewAdminComboBox.SelectedValue);

            GetSaveSelectedRow("1", AdminsDataGridView, SumAdminsLabel);
        }

        private void BlockAdminButton_Click(object sender, EventArgs e)
        {
            db.UpdateActiveAdmin((int)AdminsDataGridView.SelectedCells[0].Value, false);
            GetSaveSelectedRow("1", AdminsDataGridView);
        }

        private void GetShowAdminControlPanel(Panel openP, Panel closeP)
        {
            openP.Location = new Point(15, 150);
            openP.Size = new Size(300, 320);
            closeP.Visible = false;
            openP.Visible = !openP.Visible;
        }

        private void PanelAddAdminButton_Click(object sender, EventArgs e) => GetShowAdminControlPanel(AddAdminPanel, EditAdminPanel);

        private void PanelEditAdminButton_Click(object sender, EventArgs e)
        {
            GetShowAdminControlPanel(EditAdminPanel, AddAdminPanel);

            foreach (object a in AddAdminPanel.Controls)
                if (a is TextBox)
                    ((TextBox)a).Text = "";
        }

        private void EditAdminButton_Click(object sender, EventArgs e)
        {
            if (PasswordEditAdminTextBox.Text != ConfirmPasswordEditAdminTextBox.Text)
            {
                MessageBox.Show("Ошибка! Пароль не подтвержден");
                return;
            }

            if (IsLoginExists(LoginEditAdminTextBox.Text))
                return;

            db.UpdateAdmin((int)AdminsDataGridView.SelectedCells[0].Value,
                           LoginEditAdminTextBox.Text,
                           PasswordEditAdminTextBox.Text,
                           NameEditAdminTextBox.Text,
                           SurnameEditAdminTextBox.Text,
                           PatronymicEditAdminTextBox.Text);

            if ((int)AdminsDataGridView.SelectedCells[0].Value == int.Parse(IdLabel.Text))
            { 
                LoginTextBox.Text      = LoginEditAdminTextBox.Text;
                PasswordTextBox.Text   = PasswordEditAdminTextBox.Text;
                NameTextBox.Text       = NameEditAdminTextBox.Text;
                SurnameTextBox.Text    = SurnameEditAdminTextBox.Text;
                PatronymicTextBox.Text = PatronymicEditAdminTextBox.Text;
            }

            GetSaveSelectedRow("1", AdminsDataGridView, SumAdminsLabel);
            SetUpdateDataToEditAdminPanel();
        }

        private void SetUpdateDataToEditAdminPanel()
        {
            LoginEditAdminTextBox.Text      = AdminsDataGridView.SelectedCells[2].Value.ToString();
            PasswordEditAdminTextBox.Text   = AdminsDataGridView.SelectedCells[3].Value.ToString();
            NameEditAdminTextBox.Text       = AdminsDataGridView.SelectedCells[4].Value.ToString();
            SurnameEditAdminTextBox.Text    = AdminsDataGridView.SelectedCells[5].Value.ToString();
            PatronymicEditAdminTextBox.Text = AdminsDataGridView.SelectedCells[6].Value.ToString();
        }

        private void AdminsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SetUpdateDataToEditAdminPanel();

            PanelAddAdminButton.Enabled  = false;
            PanelEditAdminButton.Enabled = false;
            DelAdminButton.Enabled       = false;
            BlockAdminButton.Enabled     = false;
            UnlockAdminButton.Enabled    = false;
            LevelUpAdminButton.Enabled   = false;
            LevelDownAdminButton.Enabled = false;

            if ((int)AdminsDataGridView.SelectedCells[7].Value == 8 && AdminsDataGridView.SelectedCells[2].Value.ToString() == LoginTextBox.Text)
            {
                PanelAddAdminButton.Enabled  = true;
                PanelEditAdminButton.Enabled = true;
                return;
            }

            if (int.Parse(accessLevelLabel.Text) > 7)
            {
                PanelAddAdminButton.Enabled  = true;
                PanelEditAdminButton.Enabled = true;
                DelAdminButton.Enabled       = true;
                BlockAdminButton.Enabled     = true;
                UnlockAdminButton.Enabled    = true;
                LevelUpAdminButton.Enabled   = true;
                LevelDownAdminButton.Enabled = true;
                return;
            }

            if ((int)AdminsDataGridView.SelectedCells[7].Value < int.Parse(accessLevelLabel.Text))
            {
                if (int.Parse(accessLevelLabel.Text) > 5)
                {
                    BlockAdminButton.Enabled  = true;
                    UnlockAdminButton.Enabled = true;
                }
                if (int.Parse(accessLevelLabel.Text) > 6)
                {
                    LevelUpAdminButton.Enabled = (int)AdminsDataGridView.SelectedCells[7].Value >= (int.Parse(accessLevelLabel.Text) - 1) ? false : true;
                    LevelDownAdminButton.Enabled = true;
                }
            }
            
        }

        private void DelAdminButton_Click(object sender, EventArgs e)
        {
            db.DeleteAdmin((int)AdminsDataGridView.SelectedCells[0].Value);

            GetSaveSelectedRow("1", AdminsDataGridView, SumAdminsLabel);
            SetUpdateDataToEditAdminPanel();
        }

        private void UnlockAdminButton_Click(object sender, EventArgs e)
        {
            db.UpdateActiveAdmin((int)AdminsDataGridView.SelectedCells[0].Value, true);
            GetSaveSelectedRow("1", AdminsDataGridView);
        }

        private void LevelUpAdminButton_Click(object sender, EventArgs e)
        {
            try
            { 
                db.UpdateAccessLevelAdmin((int)AdminsDataGridView.SelectedCells[0].Value, 
                                          (int)AdminsDataGridView.SelectedCells[7].Value + 1);

                GetSaveSelectedRow("1", AdminsDataGridView);
            }
            catch
            {
                MessageBox.Show("Ошибка! Уровень доступа данного администратора максимальный");
            }

            if (int.Parse(accessLevelLabel.Text) == 8)
                return;

            LevelUpAdminButton.Enabled = (int)AdminsDataGridView.SelectedCells[7].Value >= (int.Parse(accessLevelLabel.Text) - 1) ? false : true;
           
        }

        private void LevelDownAdminButton_Click(object sender, EventArgs e)
        {
            try
            {
                db.UpdateAccessLevelAdmin((int)AdminsDataGridView.SelectedCells[0].Value,
                                          (int)AdminsDataGridView.SelectedCells[7].Value - 1);

                GetSaveSelectedRow("1", AdminsDataGridView);
            }
            catch
            {
                MessageBox.Show("Ошибка! Уровень доступа данного администратора минимальный");
            }
            LevelUpAdminButton.Enabled = true;
        }

        private void DelUserButton_Click(object sender, EventArgs e)
        {
            db.DeleteUser((int)UsersDataGridView.SelectedCells[0].Value);

            string table = SearchAllUsersRadioButton.Checked ? "2" : SearchActiveUsersRadioButton.Checked ? "7" : "6";
            GetSaveSelectedRow(table, UsersDataGridView, SumUsersLabel);
            GetRowsSum(SumArchiveUsersLabel, ArchiveUsersDataGridView);
        }

        private void UnlockUserButton_Click(object sender, EventArgs e)
        {
            db.UpdateActiveUser((int)UsersDataGridView.SelectedCells[0].Value, true);

            string table = SearchAllUsersRadioButton.Checked ? "2" : SearchActiveUsersRadioButton.Checked ? "7" : "6";
            GetSaveSelectedRow(table, UsersDataGridView);
        }

        private void BlockUserButton_Click(object sender, EventArgs e)
        {
            db.UpdateActiveUser((int)UsersDataGridView.SelectedCells[0].Value, false);

            string table = SearchAllUsersRadioButton.Checked ? "2" : SearchActiveUsersRadioButton.Checked ? "7" : "6";
            GetSaveSelectedRow(table, UsersDataGridView);
        }

        private void SearchRentalButton_Click(object sender, EventArgs e)
        {
            RentalDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in RentalDataGridView.Rows)
            {
                if (row.Cells[1].Value == null || row.Cells[2].Value == null)
                    break;
                if ((row.Cells[1].Value.ToString().StartsWith(LoginTenantOrLessorTextBox.Text)
                   || row.Cells[2].Value.ToString().StartsWith(LoginTenantOrLessorTextBox.Text))
                   && row.Cells[4].Value.ToString().StartsWith(NameRentalCarTextBox.Text))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumRentalLabel, RentalDataGridView, true);
        }

        private void RemoveCarPublicationButton_Click(object sender, EventArgs e)
        {
            RentalDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in RentalDataGridView.Rows)
            {
                if (row.Cells[4].Value == null)
                    break;
                if (((int)row.Cells[3].Value == (int)CarsDataGridView.SelectedCells[0].Value) && ((DateTime)row.Cells[10].Value > DateTime.Now))
                    db.DeleteRental((int)row.Cells[0].Value);
            }
            db.DeleteCar((int)CarsDataGridView.SelectedCells[0].Value);
            GetSaveSelectedRow("Cars", CarsDataGridView, SumCarsLabel);
        }

        private void SearchCarButton_Click(object sender, EventArgs e)
        {
            CarsDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in CarsDataGridView.Rows)
            {
                if (row.Cells[1].Value == null || row.Cells[2].Value == null)
                    break;
                if (row.Cells[2].Value.ToString().StartsWith(LoginLessorTextBox.Text)
                 && row.Cells[3].Value.ToString().StartsWith(NameCarTextBox.Text))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumCarsLabel, CarsDataGridView, true);
        }

        private void PastRentalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!StartedAndPastRentalRadioButton.Checked)
            {
                GetConnectTable("4");
                GetRowsSum(SumRentalLabel, RentalDataGridView);
                ComplitedRentalButton.Enabled = false;
                return;
            }

            GetSaveSelectedRow("5", RentalDataGridView);
            GetRowsSum(SumRentalLabel, RentalDataGridView);

            if (int.Parse(accessLevelLabel.Text) > 2)
                ComplitedRentalButton.Enabled = true;
        }

        private void ComplitedRentalButton_Click(object sender, EventArgs e)
        {
            try
            {
                db.DeleteRental((int)RentalDataGridView.SelectedCells[0].Value);
            }
            catch { }
            GetSaveSelectedRow("5", RentalDataGridView, SumRentalLabel);
            GetConnectTable("8");
            GetRowsSum(SumArchiveRentalLabel, ArchiveRentalDataGridView);
        }

        private void SearchBlockedUsersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!SearchBlockedUsersRadioButton.Checked)
                return;

            GetConnectTable("6");
            GetRowsSum(SumUsersLabel, UsersDataGridView);
        }

        private void SearchAllUsersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!SearchAllUsersRadioButton.Checked)
                return;

            GetConnectTable("2");
            GetRowsSum(SumUsersLabel, UsersDataGridView);
        }

        private void SearchActiveUsersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!SearchActiveUsersRadioButton.Checked)
                return;

            GetConnectTable("7");
            GetRowsSum(SumUsersLabel, UsersDataGridView);
        }

        private void SearchArchiveRentalbutton_Click(object sender, EventArgs e)
        {
            ArchiveRentalDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in ArchiveRentalDataGridView.Rows)
            {
                if (row.Cells[1].Value == null || row.Cells[2].Value == null)
                    break;
                if ((row.Cells[1].Value.ToString().StartsWith(ArchiveRentalFullNameUserTextBox.Text)
                   || row.Cells[2].Value.ToString().StartsWith(ArchiveRentalFullNameUserTextBox.Text))
                   && row.Cells[3].Value.ToString().StartsWith(ArchiveNameRentalCarTextBox.Text))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumArchiveRentalLabel, ArchiveRentalDataGridView, true);
        }

        private void SearchArchiveUserButton_Click(object sender, EventArgs e)
        {
            ArchiveUsersDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in ArchiveUsersDataGridView.Rows)
            {
                if (row.Cells[1].Value == null || row.Cells[2].Value == null)
                    break;
                if ((row.Cells[2].Value.ToString().StartsWith(ArchiveFullNameUserTextBox.Text)
                  || row.Cells[3].Value.ToString().StartsWith(ArchiveFullNameUserTextBox.Text)
                  || row.Cells[4].Value.ToString().StartsWith(ArchiveFullNameUserTextBox.Text))
                  && row.Cells[1].Value.ToString().StartsWith(ArchiveLoginUserTextBox.Text))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumArchiveUsersLabel, ArchiveUsersDataGridView, true);
        }

        private void SearchUserButton_Click(object sender, EventArgs e)
        {
            UsersDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in UsersDataGridView.Rows)
            {
                if (row.Cells[4].Value == null || row.Cells[5].Value == null)
                    break;
                if (row.Cells[4].Value.ToString().StartsWith(FullNameUserTextBox.Text)
                 || row.Cells[5].Value.ToString().StartsWith(FullNameUserTextBox.Text)
                 || row.Cells[6].Value.ToString().StartsWith(FullNameUserTextBox.Text))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumUsersLabel, UsersDataGridView, true);
        }
    }
}
