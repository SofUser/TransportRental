using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace TransportRental
{
    public partial class User : Form
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
            {
                index = random.Next(ThemeColor.ColorList.Count);
            }
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
                currentButton.ForeColor = Color.WhiteSmoke;
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
                if (previousBtn is Button)
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.WhiteSmoke;
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
                    ((Label)a).ForeColor = ThemeColor.ChangeColorBrightness(((Button)menuButton).BackColor, -0.2);
            }
        }

        private void AccountButton_Click(object sender, EventArgs e) => OpenPanel(AccountPanel, sender);

        private void MyCarsButton_Click(object sender, EventArgs e)
        {
            OpenPanel(MyCarsPanel, sender);
            SetColorControlsPanel(AddCarPanel, sender);
            SetColorControlsPanel(EditCarPanel, sender);
        }

        private void RentalCarsButton_Click(object sender, EventArgs e) => OpenPanel(RentalCarsPanel, sender);

        private void CarsButton_Click(object sender, EventArgs e) => OpenPanel(CarsPanel, sender);

        private void ReportingButton_Click(object sender, EventArgs e) => OpenPanel(ReportingPanel, sender);
    
        private void LogoutUserButton_Click(object sender, EventArgs e) => this.Close();

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

        public User()
        {
            InitializeComponent();
            random = new Random();
            this.Text = string.Empty;
            this.ControlBox = false;
        }

        private bool IsLoginExists(string login)
        {
            if (LoginTextBox.Text.ToLower() == Login.loginUser.ToLower())
                return false;
            if ((bool)db.CheckLogin(login))
            {
                MessageBox.Show("Ошибка! Пользователь с данным логином уже существует");
                return true;
            }
            return false;
        }

        private bool IsCheckPhone()
        {
            if ((bool)db.CheckPhone(PhoneMaskedTextBox.Text))
            {
                MessageBox.Show("Ошибка! Пользователь с данным номером телефона уже существует");
                return false;
            }

            int count = 0;

            foreach (char a in PhoneMaskedTextBox.Text)
                if (Char.IsNumber(a))
                    count++;
            if (count == 11)
                return true;
            else
                MessageBox.Show("Ошибка! Не правильно набран номер телефона");

            return false;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (ConfirmPasswordTextBox.Text != PasswordTextBox.Text)
            {
                MessageBox.Show("Ошибка! Неправильно набран пароль");
                return;
            }

            if (IsLoginExists(LoginTextBox.Text))
                return;

            if (!IsCheckPhone())
                return;
            
            db.UpdateUser(int.Parse(IdLabel.Text),
                               LoginTextBox.Text,
                            PasswordTextBox.Text,
                                NameTextBox.Text,
                             SurnameTextBox.Text,
                          PatronymicTextBox.Text,
                          DateTime.Parse(BirthdayDateTimePicker.Text),
                          int.Parse(PhoneMaskedTextBox.Text));
            MessageBox.Show("Данные успешно изменены");
            
        }

        private void GetConnectTable(string table)
        {
            switch (table)
            {
                case "Cars":
                case "1":
                    this.carsTableAdapter.Fill(this.transportRentalDataSet.Cars, int.Parse(IdLabel.Text));
                    CarsDataGridView.DataSource = carsBindingSource;
                    CarsDataGridView.Columns[0].Visible = false;
                    CarsDataGridView.Columns[1].HeaderText = "Арендодатель";
                    CarsDataGridView.Columns[2].HeaderText = "Название автомобиля";
                    CarsDataGridView.Columns[3].HeaderText = "Информация об автомобиле";
                    CarsDataGridView.Columns[4].HeaderText = "Цена за автомобиля";
                    CarsDataGridView.Columns[5].HeaderText = "Цена за экипаж";
                    CarsDataGridView.Columns[6].HeaderText = "Наличие экипажа";
                    break;
                case "MyCars":
                case "2":
                    this.myCarsTableAdapter.Fill(this.transportRentalDataSet.MyCars, int.Parse(IdLabel.Text));
                    MyCarsDataGridView.DataSource = myCarsBindingSource;
                    MyCarsDataGridView.Columns[0].Visible = false;
                    MyCarsDataGridView.Columns[1].HeaderText = "Название автомобиля";
                    MyCarsDataGridView.Columns[2].HeaderText = "Информация об автомобиле";
                    MyCarsDataGridView.Columns[3].HeaderText = "Цена за автомобиль";
                    MyCarsDataGridView.Columns[4].HeaderText = "Цена за экипажа";
                    MyCarsDataGridView.Columns[5].HeaderText = "Наличие экипажа";
                    break;
                case "MyRentalCars":
                case "3":
                    this.myRentalCarsTableAdapter.Fill(this.transportRentalDataSet.MyRentalCars, int.Parse(IdLabel.Text));
                    MyCarsDataGridView.DataSource = myRentalCarsBindingSource;
                    MyCarsDataGridView.Columns[0].Visible = false;
                    MyCarsDataGridView.Columns[1].HeaderText = "Арендатор";
                    MyCarsDataGridView.Columns[2].HeaderText = "Название автомобиля";
                    MyCarsDataGridView.Columns[3].HeaderText = "Информация об автомобиле";
                    MyCarsDataGridView.Columns[4].HeaderText = "Цена аренды";
                    MyCarsDataGridView.Columns[5].HeaderText = "Аренда экипажа";
                    MyCarsDataGridView.Columns[6].HeaderText = "Дата начала аренды";
                    MyCarsDataGridView.Columns[7].HeaderText = "Дата окончания аренды";
                    break;
                case "RentalCars":
                case "4":
                    this.rentalCarsTableAdapter.Fill(this.transportRentalDataSet.RentalCars, int.Parse(IdLabel.Text));
                    RentalCarsDataGridView.DataSource = rentalCarsBindingSource;
                    RentalCarsDataGridView.Columns[0].Visible = false;
                    RentalCarsDataGridView.Columns[1].HeaderText = "Арендодатель";
                    RentalCarsDataGridView.Columns[2].HeaderText = "Название автомобиля";
                    RentalCarsDataGridView.Columns[3].HeaderText = "Информация об автомобиле";
                    RentalCarsDataGridView.Columns[4].HeaderText = "Цена за автомобиля";
                    RentalCarsDataGridView.Columns[5].HeaderText = "Цена за экипаж";
                    RentalCarsDataGridView.Columns[6].HeaderText = "Цена аренды";
                    RentalCarsDataGridView.Columns[7].HeaderText = "Дата начала аренды";
                    RentalCarsDataGridView.Columns[8].HeaderText = "Дата окончания аренды";
                    break;
                case "CarReservation":
                case "5":
                    try
                    {
                        this.carReservationTableAdapter.Fill(this.transportRentalDataSet.CarReservation, (int)CarsDataGridView.SelectedCells[0].Value);
                    }
                    catch
                    {
                        this.carReservationTableAdapter.Fill(this.transportRentalDataSet.CarReservation, 0);
                    }
                    CarReservationDataGridView.DataSource = carReservationBindingSource;
                    CarReservationDataGridView.Columns[0].Visible = false;
                    CarReservationDataGridView.Columns[1].Visible = false;
                    CarReservationDataGridView.Columns[2].HeaderText = "Дата начала аренды автомобия";
                    CarReservationDataGridView.Columns[3].HeaderText = "Дата окончания аренды автомобиля";
                    break;
                case "ArchiveMyRental":
                case "6":
                    this.archiveMyRentalTableAdapter.Fill(this.transportRentalDataSet.ArchiveMyRental, int.Parse(IdLabel.Text), ArchiveStartRentalDateTimePicker.Value, ArchiveEndRentalDateTimePicker.Value);
                    ArchiveMyRentalDataGridView.DataSource = archiveMyRentalBindingSource;
                    var totalProfit = db.TotalProfit(int.Parse(IdLabel.Text), ArchiveStartRentalDateTimePicker.Value, ArchiveEndRentalDateTimePicker.Value);
                    TotalProfitLabel.Text = "Общая прибыль: " + string.Format(totalProfit % 1 == 0 ? "{0:0}" : "{0:0.00}", totalProfit == null ? 0 : totalProfit);
                    ArchiveMyRentalDataGridView.Columns[0].Visible = false;
                    ArchiveMyRentalDataGridView.Columns[1].HeaderText = "Арендатор";
                    ArchiveMyRentalDataGridView.Columns[2].HeaderText = "Арендодатель";
                    ArchiveMyRentalDataGridView.Columns[3].HeaderText = "Название автомобиля";
                    ArchiveMyRentalDataGridView.Columns[4].HeaderText = "Информация об автомобиле";
                    ArchiveMyRentalDataGridView.Columns[5].HeaderText = "Цена за автомобиля";
                    ArchiveMyRentalDataGridView.Columns[6].HeaderText = "Аренда экипажа";
                    ArchiveMyRentalDataGridView.Columns[7].HeaderText = "Цена за экипаж";
                    ArchiveMyRentalDataGridView.Columns[8].HeaderText = "Цена аренды";
                    ArchiveMyRentalDataGridView.Columns[9].HeaderText = "Дата начала аренды";
                    ArchiveMyRentalDataGridView.Columns[10].HeaderText = "Дата окончания аренды";
                    break;
                default:
                    MessageBox.Show("Ошибка! Данной таблицы не существует - ", table);
                    break;
            }
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

        private void User_Load(object sender, EventArgs e)
        {
            {
                ConnectDB connectDB = new ConnectDB();

                connectDB.OpenConnection();

                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM [User] WHERE login = '{Login.loginUser}' AND password = '{Login.passUser}'", connectDB.GetConnetcion());
                DataTable dataTable = new DataTable();
                
                adapter.Fill(dataTable);

                IdLabel.Text                 = dataTable.Rows[0][0].ToString();
                LoginTextBox.Text            = dataTable.Rows[0][2].ToString();
                PasswordTextBox.Text         = dataTable.Rows[0][3].ToString();
                NameTextBox.Text             = dataTable.Rows[0][4].ToString();
                SurnameTextBox.Text          = dataTable.Rows[0][5].ToString();
                PatronymicTextBox.Text       = dataTable.Rows[0][6].ToString();
                BirthdayDateTimePicker.Value = (DateTime)dataTable.Rows[0][7];
                PhoneMaskedTextBox.Text      = dataTable.Rows[0][8].ToString();
                DateRegistrationLabel.Text   = ((DateTime)dataTable.Rows[0][9]).ToString("dd.MM.yyyy");

                connectDB.CloseConnection();
            }
            {
                GetConnectTable("MyCars");
                GetConnectTable("RentalCars");
                GetConnectTable("Cars");
                GetConnectTable("CarReservation");
                GetConnectTable("ArchiveMyRental");
            }
            {
                GetRowsSum(SumMyCarsLabel, MyCarsDataGridView);
                GetRowsSum(SumRentalCarsLabel, RentalCarsDataGridView);
                GetRowsSum(SumCarsLabel, CarsDataGridView);
                GetRowsSum(SumArchiveMyRentalCarsLabel, ArchiveMyRentalDataGridView);
            }
            {
                StartRentalDateTimePicker.Value        = DateTime.Now;
                EndRentalDateTimePicker.Value          = DateTime.Now;
                ArchiveStartRentalDateTimePicker.Value = DateTime.Parse(DateRegistrationLabel.Text);
                ArchiveEndRentalDateTimePicker.Value   = DateTime.Now;
            }
        }

        private void MyCarsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!MyCarsRadioButton.Checked)
                return;

            GetConnectTable("MyCars");
            GetRowsSum(SumMyCarsLabel, MyCarsDataGridView);
            SetUpdateDataToEditCarPanel();
            PanelEditCarButton.Enabled    = true;
            DelCarButton.Enabled          = true;
            ComplitedRentalButton.Enabled = false;
            TextSumMyCarsLabel.Text = "Автомобилей: ";
            TextSumMyCarsLabel.Location = new Point(MyCarsDataGridView.Location.X, TextSumMyCarsLabel.Location.Y);
            SumMyCarsLabel.Location = new Point(390, SumMyCarsLabel.Location.Y);
        }

        private void RentalCarsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!MyRentalCarsRadioButton.Checked)
                return;

            GetConnectTable("MyRentalCars");
            GetRowsSum(SumMyCarsLabel, MyCarsDataGridView);
            EditCarPanel.Visible          = false;
            PanelEditCarButton.Enabled    = false;
            DelCarButton.Enabled          = false;
            ComplitedRentalButton.Enabled = true;
            TextSumMyCarsLabel.Text = "Сделок по аренде: ";
            TextSumMyCarsLabel.Location = new Point(MyCarsDataGridView.Location.X, TextSumMyCarsLabel.Location.Y);
            SumMyCarsLabel.Location = new Point(420, SumMyCarsLabel.Location.Y);
        }

        private void ShowCarControlPanel(Panel openP, Panel closeP)
        {
            openP.Location = new Point(10, 10);
            openP.Size     = new Size(270, 372);
            closeP.Visible = false;
            openP.Visible  = !openP.Visible;
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

        private void PanelAddCarButton_Click(object sender, EventArgs e)
        {
            ShowCarControlPanel(AddCarPanel, EditCarPanel);
        }

        private void PanelEditCarButton_Click(object sender, EventArgs e)
        {
            ShowCarControlPanel(EditCarPanel, AddCarPanel);
            SetUpdateDataToEditCarPanel();
        }

        private void SetUpdateDataToEditCarPanel()
        {
            try
            {
                EditNameCarTextBox.Text     = MyCarsDataGridView.SelectedCells[1].Value.ToString();
                EditRentalInfoTextBox.Text  = MyCarsDataGridView.SelectedCells[2].Value.ToString();
                EditPriceAutoTextBox.Text   = MyCarsDataGridView.SelectedCells[3].Value.ToString();
                EditUseTeamCheckBox.Checked = (bool)MyCarsDataGridView.SelectedCells[5].Value;
                EditPriceTeamTextBox.Text   = MyCarsDataGridView.SelectedCells[4].Value.ToString();
            }
            catch
            { }
        }

        private void MyCarsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SetUpdateDataToEditCarPanel();
        }

        private void DelCarButton_Click(object sender, EventArgs e)
        {
            try
            {
                db.DeleteCar((int)MyCarsDataGridView.SelectedCells[0].Value);
            }
            catch { }
            GetSaveSelectedRow("2", MyCarsDataGridView, SumMyCarsLabel);
        }

        private void AddCarButton_Click(object sender, EventArgs e)
        {
            if (NameNewCarTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите название автомобиля");
                return;
            }
            if (PriceNewCarTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите цену за аренду автомобиля");
                return;
            }
            if (PriceNewTeamTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите зарплату команде");
                return;
            }
            if (!int.TryParse(PriceNewCarTextBox.Text, out int a) || !int.TryParse(PriceNewTeamTextBox.Text, out int b))
            {
                MessageBox.Show("Ошибка! В цене введены не цифры");
                return;
            }

            db.AddCar(int.Parse(IdLabel.Text),
                      NameNewCarTextBox.Text,
                      NewRentalInfoTextBox.Text,
                      decimal.Parse(PriceNewCarTextBox.Text),
                      decimal.Parse(PriceNewTeamTextBox.Text),
                      UseNewTeamCheckBox.Checked);

            GetSaveSelectedRow("2", MyCarsDataGridView, SumMyCarsLabel);
            MyCarsRadioButton.Checked = true;
            MyRentalCarsRadioButton.Checked = false;
        }

        private void EditCarButton_Click(object sender, EventArgs e)
        {
            if (EditNameCarTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите название автомобиля");
                return;
            }
            if (EditPriceAutoTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите цену за аренду автомобиля");
                return;
            }
            if (EditPriceTeamTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Ошибка! Введите зарплату команде");
                return;
            }

            db.UpdateCar((int)MyCarsDataGridView.SelectedCells[0].Value,
                         EditNameCarTextBox.Text,
                         EditRentalInfoTextBox.Text,
                         decimal.Parse(EditPriceAutoTextBox.Text),
                         decimal.Parse(EditPriceTeamTextBox.Text),
                         EditUseTeamCheckBox.Checked);
            GetSaveSelectedRow("2", MyCarsDataGridView);
        }

        private void UseTeamCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EditPriceTeamTextBox.Enabled = !EditPriceTeamTextBox.Enabled;
            EditPriceTeamTextBox.Text = "0";
        }

        private void NewUseTeamCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PriceNewTeamTextBox.Enabled = !PriceNewTeamTextBox.Enabled;
            PriceNewTeamTextBox.Text = "0";
        }

        private void ComplitedRentalButton_Click(object sender, EventArgs e)
        {
            try
            {
                db.DeleteRental((int)MyCarsDataGridView.SelectedCells[0].Value);
                GetSaveSelectedRow("3", MyCarsDataGridView, SumMyCarsLabel);
            }
            catch
            {}
        }

        private void SetIsDateTimeFailed()
        {
            RentalCarButton.Enabled      = false;
            CarReservationPanel.Visible  = false;
            MinTimeRentalPanel.Visible   = false;
            MaxTimeRentalPanel.Visible   = false;
            ComplitedRentalPanel.Visible = false;
            PassedDateTimePanel.Visible  = false;

            TimeSpan zeroTS = new TimeSpan(0, 0, 0, 0);
            if (EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value < zeroTS)
            {
                ErrorDateTimeRentalPanel.Visible = true;
                return;
            }
            else
                ErrorDateTimeRentalPanel.Visible = false;

            TimeSpan minRentalTS = new TimeSpan(0, 0, 15, 0);
            if (EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value < minRentalTS)
            {
                MinTimeRentalPanel.Visible = true;
                return;
            }

            TimeSpan maxRentalTS = new TimeSpan(365, 0, 0, 0);
            if (EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value > maxRentalTS)
            {
                MaxTimeRentalPanel.Visible = true;
                return;
            }

            if (StartRentalDateTimePicker.Value < DateTime.Now)
            {
                PassedDateTimePanel.Visible = true;
                return;
            }

            if (CarReservationDataGridView.Rows.Count > 0) {
                CarReservationDataGridView.CurrentCell = null;
                foreach (DataGridViewRow row in CarReservationDataGridView.Rows)
                {
                    if (row.Cells[2].Value == null || row.Cells[3].Value == null)
                        break;
                    if (!(((DateTime)row.Cells[2].Value < StartRentalDateTimePicker.Value
                       &&  (DateTime)row.Cells[3].Value < StartRentalDateTimePicker.Value)
                       || ((DateTime)row.Cells[2].Value > EndRentalDateTimePicker.Value
                       &&  (DateTime)row.Cells[3].Value > EndRentalDateTimePicker.Value)))
                    {
                        CarReservationPanel.Visible = true;
                        return;
                    }
                }    
            }

            RentalCarButton.Enabled = true;
        }

        private void SetPriceRental()
        {
            PriceAutoLabel.Text   = "0";
            PriceTeamLabel.Text   = "0";
            PriceRentalLabel.Text = "0";

            SetIsDateTimeFailed();

            if (!RentalCarButton.Enabled) return;

            PriceAutoLabel.Text   = ((decimal)CarsDataGridView.SelectedCells[4].Value).ToString("N2");
            PriceTeamLabel.Text   = RentalTeamCheckBox.Checked ? ((decimal)CarsDataGridView.SelectedCells[5].Value).ToString("N2") : "0";
            PriceRentalLabel.Text = (((EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value).Days * 24
                                    + (EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value).Hours
                            + (double)(EndRentalDateTimePicker.Value - StartRentalDateTimePicker.Value).Minutes / 60) 
                                * (double.Parse(PriceAutoLabel.Text) + double.Parse(PriceTeamLabel.Text))).ToString("N2");
        }

        private void CarsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            RentalTeamCheckBox.Checked = RentalTeamCheckBox.Enabled = (bool)CarsDataGridView.SelectedCells[6].Value;

            GetConnectTable("CarReservation");

            SetPriceRental();
        }

        private void RentalTeamCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetPriceRental();
        }

        private void SearchCarButton_Click(object sender, EventArgs e)
        { 
            if (SearchCarNameTextBox.Text == "" && SearchLessorNameTextBox.Text == "")
            {
                GetConnectTable("Cars");
                GetRowsSum(SumCarsLabel, CarsDataGridView);
                return;
            }

            CarsDataGridView.CurrentCell = null;
            foreach (DataGridViewRow row in CarsDataGridView.Rows)
            {
                if ((row.Cells[1].Value != null || row.Cells[2].Value != null)
                 && (row.Cells[1].Value.ToString().StartsWith(SearchLessorNameTextBox.Text) 
                 &&  row.Cells[2].Value.ToString().StartsWith(SearchCarNameTextBox.Text)))
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            GetRowsSum(SumCarsLabel, CarsDataGridView, true);
        }

        private void RentalCarButton_Click(object sender, EventArgs e)
        {
            db.AddRental(int.Parse(IdLabel.Text),
                        (int)CarsDataGridView.SelectedCells[0].Value,
                        RentalTeamCheckBox.Checked,
                        decimal.Parse(PriceRentalLabel.Text),
                        StartRentalDateTimePicker.Value,
                        EndRentalDateTimePicker.Value);

            RentalCarButton.Enabled      = false;
            ComplitedRentalPanel.Visible = true;
            
            GetConnectTable("4");
            GetRowsSum(SumCarsLabel, CarsDataGridView);
            GetConnectTable("5");
        }

        private void StartRentalDateTimePicker_ValueChanged(object sender, EventArgs e) => SetPriceRental();

        private void EndRentalDateTimePicker_ValueChanged  (object sender, EventArgs e) => SetPriceRental();

        private void CancelRentalButton_Click(object sender, EventArgs e)
        {
            if (DateTime.Now > (DateTime)RentalCarsDataGridView.SelectedCells[7].Value)
            {
                CancelRentalPanel.Visible        = true;
                CommentCancelRentalPanel.Visible = true;
                return;
            }

            CancelRentalPanel.Visible        = false;
            CommentCancelRentalPanel.Visible = false;
            
            db.DeleteRental((int)RentalCarsDataGridView.SelectedCells[0].Value);
            GetSaveSelectedRow("4", RentalCarsDataGridView, label40);
        }

        private void ArchiveStartRentalDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            GetConnectTable("6");
            GetRowsSum(SumArchiveMyRentalCarsLabel, ArchiveMyRentalDataGridView);
        }

        private void ArchiveEndRentalDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            GetConnectTable("6");
            GetRowsSum(SumArchiveMyRentalCarsLabel, ArchiveMyRentalDataGridView);
        }

        private void AllArchiveMyRentalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllArchiveMyRentalCheckBox.Checked)
            {
                ArchiveStartRentalDateTimePicker.Enabled = true;
                ArchiveEndRentalDateTimePicker.Enabled   = true;
                return;
            }

            ArchiveStartRentalDateTimePicker.Enabled = false;
            ArchiveEndRentalDateTimePicker.Enabled   = false;

            ArchiveStartRentalDateTimePicker.Value = DateTime.Parse(DateRegistrationLabel.Text);
            ArchiveEndRentalDateTimePicker.Value   = DateTime.Now;
        }
    }
}
