#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8622
using System;
using System.Drawing;
using System.Windows.Forms;
using BoardingHouseSys.Data;
using BoardingHouseSys.Models;
using BoardingHouseSys.UI;

namespace BoardingHouseSys.Forms
{
    public class FormUsers : Form
    {
        private UserRepository _repository;
        private User _currentUser;

        // Controls
        private DataGridView dgvUsers = null!;
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private ComboBox cmbRole = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnRefresh = null!;
        private Button btnBack = null!;
        private GroupBox grpInput = null!;
        private Label lblUser = null!;
        private Label lblPass = null!;
        private Label lblRole = null!;
        private int _selectedUserId = 0;

        private Panel pnlTop = null!;
        private Button btnBackTop = null!;

        // Parameterless constructor for Designer support
        public FormUsers()
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            WireEvents();
            _currentUser = new User();
            _repository = new UserRepository();
        }

        public FormUsers(User user)
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            WireEvents();
            _currentUser = user;
            _repository = new UserRepository();

            // Populate Roles
            cmbRole.Items.Add("Admin");
            cmbRole.Items.Add("Boarder");

            // Only SuperAdmin can create other SuperAdmins
            if (_currentUser.Role == "SuperAdmin")
            {
                cmbRole.Items.Add("SuperAdmin");
            }
            cmbRole.SelectedIndex = 0;
            
            LoadUsers();
            ApplyTheme();
        }
        
        private void ApplyTheme()
        {
             UITheme.ApplyHeaderStyle(pnlTop);
             
             UITheme.ApplyNavButton(btnBackTop, 180, 30);
             UITheme.ApplySuccessButton(btnAdd, 100, 45);
             UITheme.ApplyPrimaryButton(btnUpdate, 100, 45);
             UITheme.ApplyDangerButton(btnDelete, 100, 45);
             UITheme.ApplySecondaryButton(btnRefresh, 320, 45);
             UITheme.ApplyNavButton(btnBack, 380, 50);
             
             UITheme.ApplyGroupBoxStyle(grpInput);
             
             UITheme.ApplyTextBoxStyle(txtUsername);
             UITheme.ApplyTextBoxStyle(txtPassword);
             
             UITheme.ApplyComboBoxStyle(cmbRole);
        }

        private void WireEvents()
        {
            this.btnAdd.Click += (s, e) => AddUser();
            this.btnUpdate.Click += (s, e) => UpdateUser();
            this.btnDelete.Click += (s, e) => DeleteUser();
            this.btnRefresh.Click += (s, e) => LoadUsers();
            this.btnBack.Click += (s, e) => this.Close();
            this.btnBackTop.Click += (s, e) => this.Close();
            this.dgvUsers.CellClick += DgvUsers_CellClick;
            this.grpInput.Enter += grpInput_Enter;
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnBackTop = new System.Windows.Forms.Button();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.grpInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlTop.Controls.Add(this.btnBackTop);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 50;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(1050, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // btnBackTop
            // 
            this.btnBackTop.BackColor = System.Drawing.Color.White;
            this.btnBackTop.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnBackTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackTop.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBackTop.Location = new System.Drawing.Point(10, 10);
            this.btnBackTop.Name = "btnBackTop";
            this.btnBackTop.Size = new System.Drawing.Size(180, 30);
            this.btnBackTop.TabIndex = 0;
            this.btnBackTop.Text = "← Back to Dashboard";
            this.btnBackTop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackTop.UseVisualStyleBackColor = false;
            // 
            // dgvUsers
            // 
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.BackgroundColor = System.Drawing.Color.White;
            this.dgvUsers.ColumnHeadersHeight = 34;
            this.dgvUsers.Location = new System.Drawing.Point(420, 70);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.RowHeadersVisible = false;
            this.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.Size = new System.Drawing.Size(600, 610);
            this.dgvUsers.TabIndex = 1;
            this.dgvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // grpInput
            // 
            this.grpInput.Controls.Add(this.lblUser);
            this.grpInput.Controls.Add(this.txtUsername);
            this.grpInput.Controls.Add(this.lblPass);
            this.grpInput.Controls.Add(this.txtPassword);
            this.grpInput.Controls.Add(this.lblRole);
            this.grpInput.Controls.Add(this.cmbRole);
            this.grpInput.Controls.Add(this.btnAdd);
            this.grpInput.Controls.Add(this.btnUpdate);
            this.grpInput.Controls.Add(this.btnDelete);
            this.grpInput.Controls.Add(this.btnRefresh);
            this.grpInput.Location = new System.Drawing.Point(20, 70);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(380, 420);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "User Details";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUser.Location = new System.Drawing.Point(20, 40);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(120, 25);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsername.Location = new System.Drawing.Point(140, 35);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(210, 35);
            this.txtUsername.TabIndex = 1;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPass.Location = new System.Drawing.Point(20, 90);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(120, 25);
            this.lblPass.TabIndex = 2;
            this.lblPass.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassword.Location = new System.Drawing.Point(140, 85);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(210, 35);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.PlaceholderText = "Leave blank if unchanged";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(20, 140);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(120, 25);
            this.lblRole.TabIndex = 4;
            this.lblRole.Text = "Role:";
            // 
            // cmbRole
            // 
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbRole.Location = new System.Drawing.Point(140, 135);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(210, 38);
            this.cmbRole.TabIndex = 5;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = UITheme.SuccessColor;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(20, 190);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 45);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Create";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = UITheme.PrimaryColor;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Location = new System.Drawing.Point(130, 190);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 45);
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = UITheme.DangerColor;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(240, 190);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 45);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = UITheme.PrimaryColor;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(20, 250);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(320, 45);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh List";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(20, 510);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(380, 50);
            this.btnBack.TabIndex = 10;
            this.btnBack.Text = "Back to Dashboard";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // 
            // FormUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 650);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.grpInput);
            this.Controls.Add(this.dgvUsers);
            this.Name = "FormUsers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Users";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.ResumeLayout(false);
        }

        private void DgvUsers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                _selectedUserId = Convert.ToInt32(row.Cells["Id"].Value);
                txtUsername.Text = row.Cells["Username"].Value.ToString();

                string role = row.Cells["Role"].Value.ToString()!;
                if (cmbRole.Items.Contains(role))
                {
                    cmbRole.SelectedItem = role;
                }
                txtPassword.Clear(); // Don't show hash
            }
        }

        private void UpdateUser()
        {
            if (_selectedUserId == 0)
            {
                MessageBox.Show("Please select a user to update.");
                return;
            }

            try
            {
                var user = new User
                {
                    Id = _selectedUserId,
                    Username = txtUsername.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "Boarder"
                };

                // Pass password only if user typed something
                string? newPass = string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text.Trim();

                _repository.UpdateUser(user, newPass);
                MessageBox.Show("User updated successfully!");
                LoadUsers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user: " + ex.Message);
            }
        }

        private void DeleteUser()
        {
            if (_selectedUserId == 0)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteUser(_selectedUserId);
                    MessageBox.Show("User deleted successfully!");
                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting user: " + ex.Message);
                }
            }
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 0;
            _selectedUserId = 0;
        }

        private void LoadUsers()
        {
            try
            {
                var users = _repository.GetAllUsers();
                dgvUsers.DataSource = users;

                // Hide sensitive or internal columns
                if (dgvUsers.Columns["PasswordHash"] != null)
                    dgvUsers.Columns["PasswordHash"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            try
            {
                var user = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "Boarder"
                };

                _repository.CreateUser(user, txtPassword.Text.Trim());

                MessageBox.Show("User created successfully!");
                LoadUsers();
                txtUsername.Clear();
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating user: " + ex.Message);
            }
        }

        private void grpInput_Enter(object? sender, EventArgs e)
        {

        }
    }
}
