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
    public class FormLogin : Form
    {
        private UserRepository _userRepository;
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Button btnExit = null!;
        private Label lblUsername = null!;
        private Label lblPassword = null!;
        private Label label1 = null!;
        private Label lblTitle = null!;
        private Panel panel1 = null!;
        private PictureBox pictureBox1 = null!;

        public FormLogin()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Event Handlers
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            btnExit = new Button();
            lblUsername = new Label();
            lblPassword = new Label();
            lblTitle = new Label();
            label1 = new Label();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            
            // Apply theme styles
            UITheme.ApplyFormStyle(this);
            ApplyTheme();

            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(450, 230);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(320, 35);
            txtUsername.TabIndex = 1;
            txtUsername.Text = "admin";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.WhiteSmoke;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(450, 315);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(320, 35);
            txtPassword.TabIndex = 2;
            txtPassword.Text = "admin123";
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(25, 118, 210);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(450, 380);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(150, 45);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.FromArgb(108, 117, 125);
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Segoe UI", 11F);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(620, 380);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(150, 45);
            btnExit.TabIndex = 4;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(64, 64, 64);
            lblUsername.Location = new Point(450, 200);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(116, 30);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Username:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblPassword.Location = new Point(450, 285);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(109, 30);
            lblPassword.TabIndex = 0;
            lblPassword.Text = "Password:";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(25, 118, 210);
            lblTitle.Location = new Point(450, 120);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(664, 48);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Boarding House Management System";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(108, 117, 125);
            label1.Location = new Point(450, 80);
            label1.Name = "label1";
            label1.Size = new Size(66, 38);
            label1.TabIndex = 7;
            label1.Text = "RJK";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(25, 118, 210);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 600);
            panel1.TabIndex = 8;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = resources.GetObject("pictureBox1.Image") as Image;
            pictureBox1.Location = new Point(50, 150);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(300, 300);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // FormLogin
            // 
            BackColor = Color.White;
            ClientSize = new Size(1150, 600);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnExit);
            FormBorderStyle = FormBorderStyle.None;
            Icon = resources.GetObject("$this.Icon") as Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login - Boarding House Management System";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyButtonStyle(btnLogin);
            UITheme.ApplyButtonStyle(btnExit);
            UITheme.ApplyTextBoxStyle(txtUsername);
            UITheme.ApplyTextBoxStyle(txtPassword);
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User? user = _userRepository.Authenticate(username, password);
                if (user != null)
                {
                    // Hide login, show dashboard
                    this.Hide();
                    FormDashboard dashboard = new FormDashboard(user);
                    dashboard.FormClosed += (s, args) =>
                    {
                        // Ensure app exits if login is also closed or handle logout logic
                        // If logout clicked, show login. If X clicked, show login.
                        this.Show();
                        this.txtPassword.Clear();
                    };
                    dashboard.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message + "\n\nMake sure you have run the 'database_schema.sql' script and updated the connection string in Data/DatabaseHelper.cs.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblTitle_Click(object? sender, EventArgs e)
        {

        }
    }
}
