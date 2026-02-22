#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8622
using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using BoardingHouseSys.Data;
using BoardingHouseSys.Models;
using BoardingHouseSys.UI;

namespace BoardingHouseSys.Forms
{
    public class FormRooms : Form
    {
        private RoomRepository _repository;
        private BoardingHouseRepository _boardingHouseRepo;
        private User _currentUser;

        // UI Controls
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnBackTop;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.Label lblBoardingHouse;
        private System.Windows.Forms.ComboBox cmbBoardingHouses;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnAddQuick;
        private System.Windows.Forms.DataGridView dgvRooms;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.TextBox txtRoomNumber;
        private System.Windows.Forms.Label lblCap;
        private System.Windows.Forms.TextBox txtCapacity;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Timer searchTimer;

        private int _selectedRoomId = 0;
        private bool _isUpdatingResults = false;
        private string _pendingSearchText = "";
        private int? _selectedBoardingHouseId = null;
        private bool _canManageRooms = false;

        // Parameterless constructor for Designer support
        public FormRooms()
        {
            InitializeComponent();
            WireEvents();
            _repository = new RoomRepository();
            _boardingHouseRepo = new BoardingHouseRepository();
            _currentUser = new User();
        }

        public FormRooms(User user)
        {
            InitializeComponent();
            WireEvents();
            _repository = new RoomRepository();
            _boardingHouseRepo = new BoardingHouseRepository();
            _currentUser = user;

            this.Load += (s, e) =>
            {
                ApplyAccessRules();
                LoadBoardingHouses();
                HideDetailsPanel();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlTop = new Panel();
            btnBackTop = new Button();
            grpInput = new GroupBox();
            btnRefresh = new Button();
            lblRoom = new Label();
            txtRoomNumber = new TextBox();
            lblCap = new Label();
            txtCapacity = new TextBox();
            lblRate = new Label();
            txtRate = new TextBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnClear = new Button();
            btnBack = new Button();
            dgvRooms = new DataGridView();
            pnlListHeader = new Panel();
            lblListTitle = new Label();
            lblBoardingHouse = new Label();
            cmbBoardingHouses = new ComboBox();
            btnAddQuick = new Button();
            txtSearch = new TextBox();
            searchTimer = new System.Windows.Forms.Timer(components);
            pnlTop.SuspendLayout();
            grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).BeginInit();
            pnlListHeader.SuspendLayout();
            SuspendLayout();
            
            // Apply Base Form Style
            UITheme.ApplyFormStyle(this);

            // 
            // pnlTop
            // 
            pnlTop.BackColor = UITheme.HeaderColor;
            pnlTop.Controls.Add(btnBackTop);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(10);
            pnlTop.Size = new Size(1284, 50);
            pnlTop.TabIndex = 0;
            // 
            // btnBackTop
            // 
            UITheme.ApplyNavButton(btnBackTop, 180, 30);
            btnBackTop.Dock = DockStyle.Left;
            btnBackTop.Location = new Point(10, 10);
            btnBackTop.Name = "btnBackTop";
            btnBackTop.TabIndex = 0;
            btnBackTop.Text = "← Back to Dashboard";
            btnBackTop.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpInput
            // 
            UITheme.ApplyGroupBoxStyle(grpInput);
            grpInput.Controls.Add(btnRefresh);
            grpInput.Controls.Add(lblRoom);
            grpInput.Controls.Add(txtRoomNumber);
            grpInput.Controls.Add(lblCap);
            grpInput.Controls.Add(txtCapacity);
            grpInput.Controls.Add(lblRate);
            grpInput.Controls.Add(txtRate);
            grpInput.Controls.Add(btnAdd);
            grpInput.Controls.Add(btnUpdate);
            grpInput.Controls.Add(btnDelete);
            grpInput.Controls.Add(btnClear);
            grpInput.Location = new Point(20, 70);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(380, 420);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "Room Details";
            // 
            // lblRoom
            // 
            UITheme.ApplyLabelStyle(lblRoom, true);
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(20, 40);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(65, 19);
            lblRoom.TabIndex = 0;
            lblRoom.Text = "Room #:";
            // 
            // txtRoomNumber
            // 
            UITheme.ApplyTextBoxStyle(txtRoomNumber);
            txtRoomNumber.Location = new Point(100, 35);
            txtRoomNumber.Name = "txtRoomNumber";
            txtRoomNumber.Size = new Size(250, 35);
            txtRoomNumber.TabIndex = 1;
            // 
            // lblCap
            // 
            UITheme.ApplyLabelStyle(lblCap, true);
            lblCap.AutoSize = true;
            lblCap.Location = new Point(20, 90);
            lblCap.Name = "lblCap";
            lblCap.Size = new Size(71, 19);
            lblCap.TabIndex = 2;
            lblCap.Text = "Capacity:";
            // 
            // txtCapacity
            // 
            UITheme.ApplyTextBoxStyle(txtCapacity);
            txtCapacity.Location = new Point(100, 85);
            txtCapacity.Name = "txtCapacity";
            txtCapacity.Size = new Size(250, 35);
            txtCapacity.TabIndex = 3;
            // 
            // lblRate
            // 
            UITheme.ApplyLabelStyle(lblRate, true);
            lblRate.AutoSize = true;
            lblRate.Location = new Point(20, 140);
            lblRate.Name = "lblRate";
            lblRate.Size = new Size(43, 19);
            lblRate.TabIndex = 4;
            lblRate.Text = "Rate:";
            // 
            // txtRate
            // 
            UITheme.ApplyTextBoxStyle(txtRate);
            txtRate.Location = new Point(100, 135);
            txtRate.Name = "txtRate";
            txtRate.Size = new Size(250, 35);
            txtRate.TabIndex = 5;
            // 
            // btnAdd
            // 
            UITheme.ApplySuccessButton(btnAdd, 80, 30);
            btnAdd.Location = new Point(100, 190);
            btnAdd.Name = "btnAdd";
            btnAdd.TabIndex = 6;
            btnAdd.Text = "Add";
            // 
            // btnUpdate
            // 
            UITheme.ApplyPrimaryButton(btnUpdate, 80, 30);
            btnUpdate.Location = new Point(185, 190);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Update";
            // 
            // btnDelete
            // 
            UITheme.ApplyDangerButton(btnDelete, 80, 30);
            btnDelete.Location = new Point(270, 190);
            btnDelete.Name = "btnDelete";
            btnDelete.TabIndex = 8;
            btnDelete.Text = "Delete";
            // 
            // btnClear
            // 
            UITheme.ApplySecondaryButton(btnClear, 250, 30);
            btnClear.Location = new Point(100, 230);
            btnClear.Name = "btnClear";
            btnClear.TabIndex = 9;
            btnClear.Text = "Clear";
            // 
            // btnRefresh
            // 
            UITheme.ApplySecondaryButton(btnRefresh, 330, 30);
            btnRefresh.Location = new Point(20, 280);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.TabIndex = 10;
            btnRefresh.Text = "Refresh List";
            // 
            // btnBack
            // 
            UITheme.ApplyNavButton(btnBack, 380, 40);
            btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBack.Location = new Point(20, 510);
            btnBack.Name = "btnBack";
            btnBack.TabIndex = 7;
            btnBack.Text = "Back to Dashboard";
            // 
            // dgvRooms
            // 
            UITheme.ApplyDataGridViewStyle(dgvRooms);
            dgvRooms.AllowUserToAddRows = false;
            dgvRooms.AllowUserToDeleteRows = false;
            dgvRooms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRooms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRooms.Location = new Point(420, 115);
            dgvRooms.MultiSelect = false;
            dgvRooms.Name = "dgvRooms";
            dgvRooms.ReadOnly = true;
            dgvRooms.RowHeadersVisible = false;
            dgvRooms.RowHeadersWidth = 62;
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.Size = new Size(985, 565);
            dgvRooms.TabIndex = 7;
            // 
            // pnlListHeader
            // 
            pnlListHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlListHeader.Controls.Add(lblListTitle);
            pnlListHeader.Controls.Add(lblBoardingHouse);
            pnlListHeader.Controls.Add(cmbBoardingHouses);
            pnlListHeader.Controls.Add(btnAddQuick);
            pnlListHeader.Controls.Add(txtSearch);
            pnlListHeader.Location = new Point(420, 70);
            pnlListHeader.Name = "pnlListHeader";
            pnlListHeader.Size = new Size(1185, 48);
            pnlListHeader.TabIndex = 6;
            // 
            // lblListTitle
            // 
            UITheme.ApplyHeaderLabelStyle(lblListTitle);
            lblListTitle.AutoSize = true;
            lblListTitle.Location = new Point(0, 12);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(85, 21);
            lblListTitle.TabIndex = 0;
            lblListTitle.Text = "Room List";
            // 
            // lblBoardingHouse
            // 
            UITheme.ApplyLabelStyle(lblBoardingHouse, true);
            lblBoardingHouse.AutoSize = true;
            lblBoardingHouse.Location = new Point(101, 12);
            lblBoardingHouse.Name = "lblBoardingHouse";
            lblBoardingHouse.Padding = new Padding(0, 0, 7, 0);
            lblBoardingHouse.Size = new Size(128, 19);
            lblBoardingHouse.TabIndex = 3;
            lblBoardingHouse.Text = "Boarding House:";
            lblBoardingHouse.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbBoardingHouses
            // 
            UITheme.ApplyComboBoxStyle(cmbBoardingHouses);
            cmbBoardingHouses.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoardingHouses.FormattingEnabled = true;
            cmbBoardingHouses.Location = new Point(236, 11);
            cmbBoardingHouses.Margin = new Padding(4, 0, 4, 0);
            cmbBoardingHouses.Name = "cmbBoardingHouses";
            cmbBoardingHouses.Size = new Size(183, 28);
            cmbBoardingHouses.TabIndex = 4;
            // 
            // btnAddQuick
            // 
            UITheme.ApplySuccessButton(btnAddQuick, 105, 48);
            btnAddQuick.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddQuick.Location = new Point(745, 10);
            btnAddQuick.Name = "btnAddQuick";
            btnAddQuick.TabIndex = 2;
            btnAddQuick.Text = "Add Room";
            // 
            // txtSearch
            // 
            UITheme.ApplyTextBoxStyle(txtSearch);
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.Location = new Point(560, 11);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search rooms...";
            txtSearch.Size = new Size(176, 27);
            txtSearch.TabIndex = 1;

            // 
            // searchTimer
            // 
            searchTimer.Interval = 350;
            // 
            // FormRooms
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 421);
            Controls.Add(pnlTop);
            Controls.Add(grpInput);
            Controls.Add(btnBack);
            Controls.Add(pnlListHeader);
            Controls.Add(dgvRooms);
            Name = "FormRooms";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Rooms";
            WindowState = FormWindowState.Maximized;
            Load += FormRooms_Load;
            pnlTop.ResumeLayout(false);
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).EndInit();
            pnlListHeader.ResumeLayout(false);
            pnlListHeader.PerformLayout();
            ResumeLayout(false);
        }

        private void WireEvents()
        {
            this.btnAdd.Click += (s, e) => AddRoom();
            this.btnUpdate.Click += (s, e) => UpdateRoom();
            this.btnDelete.Click += (s, e) => DeleteRoom();
            this.btnRefresh.Click += (s, e) => RefreshRooms();
            this.btnBack.Click += (s, e) => this.Close();
            this.btnBackTop.Click += (s, e) => this.Close();
            this.btnClear.Click += (s, e) => { ClearFields(); HideDetailsPanel(); };
            this.dgvRooms.CellClick += DgvRooms_CellClick;
            this.txtSearch.TextChanged += (s, e) => { _pendingSearchText = txtSearch.Text; searchTimer.Stop(); searchTimer.Start(); };
            this.searchTimer.Tick += (s, e) => { searchTimer.Stop(); PerformSearch(_pendingSearchText); };
            this.btnAddQuick.Click += (s, e) => { ClearFields(); ShowDetailsPanel(); };
            this.cmbBoardingHouses.SelectedIndexChanged += (s, e) => { OnBoardingHouseChanged(); };
        }

        private void LoadRooms()
        {
            try
            {
                if (_selectedBoardingHouseId == null)
                {
                    BindRooms(new DataTable());
                    return;
                }

                var rooms = _repository.GetRoomsByBoardingHouse(_selectedBoardingHouseId.Value);
                BindRooms(rooms);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms: " + ex.Message);
            }
        }

        private void BindRooms(DataTable rooms)
        {
            if (_isUpdatingResults) return;
            _isUpdatingResults = true;
            try
            {
                dgvRooms.DataSource = rooms;
                if (dgvRooms.Columns["Id"] != null) dgvRooms.Columns["Id"].Visible = false;
                if (dgvRooms.Columns["BoardingHouseId"] != null) dgvRooms.Columns["BoardingHouseId"].Visible = false;
                if (dgvRooms.Rows.Count > 0)
                {
                    dgvRooms.ClearSelection();
                    dgvRooms.CurrentCell = null;
                }
            }
            finally
            {
                _isUpdatingResults = false;
            }
        }

        private void PerformSearch(string keyword)
        {
            if (_isUpdatingResults) return;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadRooms();
                HideDetailsPanel();
                return;
            }

            try
            {
                if (_selectedBoardingHouseId == null)
                {
                    BindRooms(new DataTable());
                    HideDetailsPanel();
                    return;
                }

                var results = _repository.SearchRoomsByBoardingHouse(_selectedBoardingHouseId.Value, keyword);
                BindRooms(results);
                HideDetailsPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching rooms: " + ex.Message);
            }
        }

        private void RefreshRooms()
        {
            string keyword = txtSearch != null ? txtSearch.Text.Trim() : _pendingSearchText;
            PerformSearch(keyword);
        }

        private void ApplyAccessRules()
        {
            _canManageRooms = _currentUser != null && (_currentUser.Role == "SuperAdmin" || _currentUser.Role == "Admin");

            if (txtRoomNumber != null) txtRoomNumber.ReadOnly = !_canManageRooms;
            if (txtCapacity != null) txtCapacity.ReadOnly = !_canManageRooms;
            if (txtRate != null) txtRate.ReadOnly = !_canManageRooms;
            if (btnAdd != null) btnAdd.Enabled = _canManageRooms;
            if (btnUpdate != null) btnUpdate.Enabled = _canManageRooms;
            if (btnDelete != null) btnDelete.Enabled = _canManageRooms;
            if (btnClear != null) btnClear.Enabled = _canManageRooms;
            if (btnAddQuick != null) btnAddQuick.Enabled = _canManageRooms;
            UpdateManageState();
        }

        private void LoadBoardingHouses()
        {
            try
            {
                cmbBoardingHouses.Items.Clear();

                if (_currentUser == null) return;

                if (_currentUser.Role == "SuperAdmin")
                {
                    var houses = _boardingHouseRepo.GetAll();
                    foreach (var house in houses)
                    {
                        cmbBoardingHouses.Items.Add(new ComboBoxItem { Text = house.Name, Value = house.Id });
                    }
                }
                else if (_currentUser.Role == "Admin")
                {
                    var houses = _boardingHouseRepo.GetAllByOwner(_currentUser.Id);
                    foreach (var house in houses)
                    {
                        cmbBoardingHouses.Items.Add(new ComboBoxItem { Text = house.Name, Value = house.Id });
                    }
                }

                if (cmbBoardingHouses.Items.Count == 1)
                {
                    cmbBoardingHouses.SelectedIndex = 0;
                }
                else
                {
                    cmbBoardingHouses.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading boarding houses: " + ex.Message);
            }
        }

        private void OnBoardingHouseChanged()
        {
            _selectedBoardingHouseId = null;
            if (cmbBoardingHouses.SelectedItem is ComboBoxItem item)
            {
                _selectedBoardingHouseId = (int)item.Value;
            }

            LoadRooms();
            ClearFields();
            UpdateManageState();
        }

        private void UpdateManageState()
        {
            bool hasHouse = _selectedBoardingHouseId != null;
            if (btnAdd != null) btnAdd.Enabled = _canManageRooms && hasHouse;
            if (btnUpdate != null) btnUpdate.Enabled = _canManageRooms && hasHouse;
            if (btnDelete != null) btnDelete.Enabled = _canManageRooms && hasHouse;
            if (btnClear != null) btnClear.Enabled = _canManageRooms && hasHouse;
            if (btnAddQuick != null) btnAddQuick.Enabled = _canManageRooms && hasHouse;
        }

        private void ShowDetailsPanel()
        {
            if (grpInput != null) grpInput.Visible = true;
            UpdateManageState();
        }

        private void HideDetailsPanel()
        {
            if (grpInput != null) grpInput.Visible = false;
            if (btnUpdate != null) btnUpdate.Enabled = false;
            if (btnDelete != null) btnDelete.Enabled = false;
        }

        private void AddRoom()
        {
            if (_selectedBoardingHouseId == null)
            {
                MessageBox.Show("Please select a boarding house first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || string.IsNullOrWhiteSpace(txtCapacity.Text) || string.IsNullOrWhiteSpace(txtRate.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || !decimal.TryParse(txtRate.Text, out decimal rate))
            {
                MessageBox.Show("Invalid Capacity or Rate.");
                return;
            }

            var room = new Room
            {
                RoomNumber = txtRoomNumber.Text,
                Capacity = capacity,
                MonthlyRate = rate,
                BoardingHouseId = _selectedBoardingHouseId
            };

            try
            {
                _repository.AddRoom(room);
                MessageBox.Show("Room added successfully!");
                LoadRooms();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding room: " + ex.Message);
            }
        }

        private void UpdateRoom()
        {
            if (_selectedRoomId == 0)
            {
                MessageBox.Show("Please select a room to update.");
                return;
            }

            if (_selectedBoardingHouseId == null)
            {
                MessageBox.Show("Please select a boarding house first.");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || !decimal.TryParse(txtRate.Text, out decimal rate))
            {
                MessageBox.Show("Invalid Capacity or Rate.");
                return;
            }

            var room = new Room
            {
                Id = _selectedRoomId,
                RoomNumber = txtRoomNumber.Text,
                Capacity = capacity,
                MonthlyRate = rate,
                BoardingHouseId = _selectedBoardingHouseId
            };

            try
            {
                _repository.UpdateRoom(room);
                MessageBox.Show("Room updated successfully!");
                LoadRooms();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating room: " + ex.Message);
            }
        }

        private void DeleteRoom()
        {
            if (_selectedRoomId == 0)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure? This will delete the room.", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteRoom(_selectedRoomId);
                    MessageBox.Show("Room deleted.");
                    LoadRooms();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting room: " + ex.Message);
                }
            }
        }

        private void ClearFields()
        {
            _selectedRoomId = 0;
            txtRoomNumber.Clear();
            txtCapacity.Clear();
            txtRate.Clear();
            HideDetailsPanel();
        }

        private void DgvRooms_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingResults) return;
            if (e.RowIndex < 0 || dgvRooms.Rows.Count == 0) return;

            var row = dgvRooms.Rows[e.RowIndex];
            if (row.Cells["Id"].Value == null) return;

            _selectedRoomId = Convert.ToInt32(row.Cells["Id"].Value);
            txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString() ?? "";
            txtCapacity.Text = row.Cells["Capacity"].Value?.ToString() ?? "";
            txtRate.Text = row.Cells["MonthlyRate"].Value?.ToString() ?? "";
            ShowDetailsPanel();
        }

        private class ComboBoxItem
        {
            public string Text { get; set; } = "";
            public object Value { get; set; } = null!;
            public override string ToString() => Text;
        }

        private void FormRooms_Load(object sender, EventArgs e)
        {

        }
    }
}
