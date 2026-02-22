#nullable enable
#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BoardingHouseSys.Data;
using BoardingHouseSys.UI;

namespace BoardingHouseSys.Forms
{
    public class FormConnection : Form
    {
        private TextBox txtServer;
        private TextBox txtDatabase;
        private TextBox txtUser;
        private TextBox txtPassword;
        private Button btnSave;
        private Button btnTest;
        private Button btnCancel;
        private RadioButton rbLocal;
        private RadioButton rbRemote;

        public FormConnection()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            Size = new Size(400, 450);
            Text = "Database Connection Settings";
            StartPosition = FormStartPosition.CenterParent;
            UITheme.ApplyFormStyle(this);

            Label lblTitle = new Label();
            lblTitle.Text = "Connection Setup";
            UITheme.ApplyHeaderLabelStyle(lblTitle);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            Controls.Add(lblTitle);

            rbLocal = new RadioButton();
            rbLocal.Text = "Local (Client)";
            rbLocal.Location = new Point(40, 70);
            rbLocal.Checked = true;
            rbLocal.CheckedChanged += RbType_CheckedChanged;
            Controls.Add(rbLocal);

            rbRemote = new RadioButton();
            rbRemote.Text = "Remote (Server)";
            rbRemote.Location = new Point(200, 70);
            rbRemote.CheckedChanged += RbType_CheckedChanged;
            Controls.Add(rbRemote);

            Label lblServer = new Label();
            lblServer.Text = "Server IP:";
            UITheme.ApplyLabelStyle(lblServer);
            lblServer.Location = new Point(20, 110);
            Controls.Add(lblServer);

            txtServer = new TextBox();
            UITheme.ApplyTextBoxStyle(txtServer);
            txtServer.Location = new Point(130, 110);
            txtServer.Width = 200;
            txtServer.Text = "localhost";
            Controls.Add(txtServer);

            Label lblDb = new Label();
            lblDb.Text = "Database:";
            UITheme.ApplyLabelStyle(lblDb);
            lblDb.Location = new Point(20, 150);
            Controls.Add(lblDb);

            txtDatabase = new TextBox();
            UITheme.ApplyTextBoxStyle(txtDatabase);
            txtDatabase.Location = new Point(130, 150);
            txtDatabase.Width = 200;
            txtDatabase.Text = "BoardingHouseDB";
            Controls.Add(txtDatabase);

            Label lblUser = new Label();
            lblUser.Text = "Username:";
            UITheme.ApplyLabelStyle(lblUser);
            lblUser.Location = new Point(20, 190);
            Controls.Add(lblUser);

            txtUser = new TextBox();
            UITheme.ApplyTextBoxStyle(txtUser);
            txtUser.Location = new Point(130, 190);
            txtUser.Width = 200;
            txtUser.Text = "root";
            Controls.Add(txtUser);

            Label lblPass = new Label();
            lblPass.Text = "Password:";
            UITheme.ApplyLabelStyle(lblPass);
            lblPass.Location = new Point(20, 230);
            Controls.Add(lblPass);

            txtPassword = new TextBox();
            UITheme.ApplyTextBoxStyle(txtPassword);
            txtPassword.Location = new Point(130, 230);
            txtPassword.Width = 200;
            txtPassword.PasswordChar = '*';
            txtPassword.Text = "root";
            Controls.Add(txtPassword);

            btnTest = new Button();
            btnTest.Text = "Test Connection";
            UITheme.ApplySecondaryButton(btnTest, 150);
            btnTest.Location = new Point(20, 280);
            btnTest.Click += BtnTest_Click;
            Controls.Add(btnTest);

            btnSave = new Button();
            btnSave.Text = "Save & Apply";
            UITheme.ApplyPrimaryButton(btnSave, 150);
            btnSave.Location = new Point(200, 280);
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            UITheme.ApplyDangerButton(btnCancel, 100);
            btnCancel.Location = new Point(140, 330);
            btnCancel.Click += BtnCancel_Click;
            Controls.Add(btnCancel);
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void RbType_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbLocal.Checked)
            {
                txtServer.Text = "localhost";
                txtServer.Enabled = false;
            }
            else
            {
                txtServer.Enabled = true;
                if (txtServer.Text == "localhost") txtServer.Text = "";
            }
        }

        private void LoadSettings()
        {
            // Load from file if exists
            if (File.Exists("db_config.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines("db_config.txt");
                    if (lines.Length >= 4)
                    {
                        txtServer.Text = lines[0].Split('=')[1];
                        txtDatabase.Text = lines[1].Split('=')[1];
                        txtUser.Text = lines[2].Split('=')[1];
                        txtPassword.Text = lines[3].Split('=')[1];
                        
                        if (txtServer.Text.ToLower() == "localhost")
                            rbLocal.Checked = true;
                        else
                            rbRemote.Checked = true;
                    }
                }
                catch { }
            }
        }

        private string BuildConnectionString()
        {
            return $"Server={txtServer.Text};Database={txtDatabase.Text};Uid={txtUser.Text};Pwd={txtPassword.Text};";
        }

        private void BtnTest_Click(object? sender, EventArgs e)
        {
            string connStr = BuildConnectionString();
            try
            {
                DatabaseHelper tempHelper = new DatabaseHelper(connStr);
                // Try simple query
                tempHelper.ExecuteScalar("SELECT 1");
                MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Save to file
                string[] lines = new string[]
                {
                    $"Server={txtServer.Text}",
                    $"Database={txtDatabase.Text}",
                    $"Uid={txtUser.Text}",
                    $"Pwd={txtPassword.Text}"
                };
                File.WriteAllLines("db_config.txt", lines);

                // Update Runtime
                DatabaseHelper.SetConnectionString(BuildConnectionString());

                MessageBox.Show("Settings Saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message);
            }
        }
    }
}
