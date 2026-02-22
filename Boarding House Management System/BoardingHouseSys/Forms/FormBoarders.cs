#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8622
using System;
using System.Drawing;
using System.Windows.Forms;
using BoardingHouseSys.Data;
using BoardingHouseSys.Models;
using BoardingHouseSys.UI;
using System.Collections.Generic;
using System.Data;
using System.IO;
 
namespace BoardingHouseSys.Forms
{
    public class FormBoarders : Form
    {
        private BoarderRepository _boarderRepo;
        private RoomRepository _roomRepo;
        private UserRepository _userRepo;
        private BoardingHouseRepository _boardingHouseRepo;
        private PaymentRepository _paymentRepo;
        private User _currentUser;
        
        private int _selectedBoarderId = 0;
        private string? _selectedImagePath = null;
        private string? _currentDbImagePath = null;
        private bool _isUpdatingResults = false;
        private string _pendingSearchText = "";
        private int _selectedPaymentId = 0;
        private bool _isUpdatingPaymentResults = false;
        private string _pendingPaymentSearchText = "";

        // Designer Controls
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnBackTop;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.SplitContainer listSplitContainer;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnAddQuick;
        private System.Windows.Forms.DataGridView dgvBoarders;
        private System.Windows.Forms.Panel pnlPaymentsHeader;
        private System.Windows.Forms.Label lblPaymentsTitle;
        private System.Windows.Forms.TextBox txtPaymentSearch;
        private System.Windows.Forms.Button btnAddPaymentQuick;
        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.Panel inputScrollPanel;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.TableLayoutPanel inputLayout;
        private System.Windows.Forms.Label lblPhoto;
        private System.Windows.Forms.FlowLayoutPanel imagePanel;
        private System.Windows.Forms.PictureBox picProfile;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblBoardingHouse;
        private System.Windows.Forms.ComboBox cmbBoardingHouses;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.ComboBox cmbRooms;
        private System.Windows.Forms.CheckBox chkCreateAccount;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.FlowLayoutPanel btnPanel;
        private System.Windows.Forms.FlowLayoutPanel row1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.FlowLayoutPanel row2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.FlowLayoutPanel row3;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Timer searchTimer;
        private System.Windows.Forms.Timer paymentSearchTimer;
        private System.Windows.Forms.GroupBox grpPayments;
        private System.Windows.Forms.TableLayoutPanel paymentLayout;
        private System.Windows.Forms.Label lblPaymentAmount;
        private System.Windows.Forms.TextBox txtPaymentAmount;
        private System.Windows.Forms.Label lblPaymentMonth;
        private System.Windows.Forms.ComboBox cmbPaymentMonth;
        private System.Windows.Forms.Label lblPaymentYear;
        private System.Windows.Forms.NumericUpDown numPaymentYear;
        private System.Windows.Forms.Label lblPaymentStatus;
        private System.Windows.Forms.ComboBox cmbPaymentStatus;
        private System.Windows.Forms.Label lblPaymentNotes;
        private System.Windows.Forms.TextBox txtPaymentNotes;
        private System.Windows.Forms.FlowLayoutPanel paymentButtonPanel;
        private System.Windows.Forms.Button btnPaymentAdd;
        private System.Windows.Forms.Button btnPaymentUpdate;
        private System.Windows.Forms.Button btnPaymentDelete;
        private System.Windows.Forms.Button btnPaymentClear;

        public FormBoarders()
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            _boarderRepo = new BoarderRepository();
            _roomRepo = new RoomRepository();
            _userRepo = new UserRepository();
            _boardingHouseRepo = new BoardingHouseRepository();
            _paymentRepo = new PaymentRepository();
            _currentUser = new User();
            LoadPaymentDropdowns();
            WireEvents();
        }

        public FormBoarders(User user)
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            _boarderRepo = new BoarderRepository();
            _roomRepo = new RoomRepository();
            _userRepo = new UserRepository();
            _boardingHouseRepo = new BoardingHouseRepository();
            _paymentRepo = new PaymentRepository();
            _currentUser = user;
            LoadPaymentDropdowns();
            WireEvents();

            this.Load += (s, e) =>
            {
                LoadBoardingHouses();
                LoadRooms();
                LoadBoarders();
                LoadPaymentsForSelectedBoarder();
                HideDetailsPanel();
                ApplyTheme();
            };
        }

        private void ApplyTheme()
        {
             UITheme.ApplyHeaderStyle(pnlTop);
             UITheme.ApplyHeaderStyle(pnlListHeader);
             UITheme.ApplyHeaderStyle(pnlPaymentsHeader);
             
             UITheme.ApplyButtonStyle(btnBackTop);
             UITheme.ApplyButtonStyle(btnAddQuick);
             UITheme.ApplyButtonStyle(btnAddPaymentQuick);
             UITheme.ApplyButtonStyle(btnAdd);
             UITheme.ApplyButtonStyle(btnUpdate);
             UITheme.ApplyButtonStyle(btnDelete);
             UITheme.ApplyButtonStyle(btnClear);
             UITheme.ApplyButtonStyle(btnRefresh);
             UITheme.ApplyButtonStyle(btnBack);
             
             UITheme.ApplyButtonStyle(btnPaymentAdd);
             UITheme.ApplyButtonStyle(btnPaymentUpdate);
             UITheme.ApplyButtonStyle(btnPaymentDelete);
             UITheme.ApplyButtonStyle(btnPaymentClear);
             UITheme.ApplyButtonStyle(btnBrowseImage);
             
             UITheme.ApplyGroupBoxStyle(grpInput);
             UITheme.ApplyGroupBoxStyle(grpPayments);
             
             UITheme.ApplyTextBoxStyle(txtSearch);
             UITheme.ApplyTextBoxStyle(txtPaymentSearch);
             UITheme.ApplyTextBoxStyle(txtFullName);
             UITheme.ApplyTextBoxStyle(txtAddress);
             UITheme.ApplyTextBoxStyle(txtPhone);
             UITheme.ApplyTextBoxStyle(txtUsername);
             UITheme.ApplyTextBoxStyle(txtPassword);
             UITheme.ApplyTextBoxStyle(txtPaymentAmount);
             UITheme.ApplyTextBoxStyle(txtPaymentNotes);
             
             UITheme.ApplyComboBoxStyle(cmbBoardingHouses);
             UITheme.ApplyComboBoxStyle(cmbRooms);
             UITheme.ApplyComboBoxStyle(cmbPaymentMonth);
             UITheme.ApplyComboBoxStyle(cmbPaymentStatus);
             
             UITheme.ApplyNumericUpDownStyle(numPaymentYear);
        }

        private void WireEvents()
        {
            this.btnBackTop.Click += (s, e) => this.Close();
            this.chkCreateAccount.CheckedChanged += (s, e) => ToggleAccountFields(chkCreateAccount.Checked);
            this.btnRefresh.Click += (s, e) => RefreshBoarders();
            this.btnBack.Click += (s, e) => this.Close();
            this.btnClear.Click += (s, e) => { ClearFields(); HideDetailsPanel(); };
            this.btnAdd.Click += (s, e) => AddBoarder();
            this.btnUpdate.Click += (s, e) => UpdateBoarder();
            this.btnDelete.Click += (s, e) => DeleteBoarder();
            this.dgvBoarders.CellClick += DgvBoarders_CellClick;
            this.btnBrowseImage.Click += BtnBrowseImage_Click;
            this.txtSearch.TextChanged += (s, e) => { _pendingSearchText = txtSearch.Text; searchTimer.Stop(); searchTimer.Start(); };
            this.searchTimer.Tick += (s, e) => { searchTimer.Stop(); PerformSearch(_pendingSearchText); };
            this.btnAddQuick.Click += (s, e) => { ClearFields(); ShowDetailsPanel(); };
            this.cmbBoardingHouses.SelectedIndexChanged += (s, e) => LoadRoomsForSelectedHouse();
            this.dgvPayments.CellClick += DgvPayments_CellClick;
            this.btnPaymentAdd.Click += (s, e) => AddPayment();
            this.btnPaymentUpdate.Click += (s, e) => UpdatePayment();
            this.btnPaymentDelete.Click += (s, e) => DeletePayment();
            this.btnPaymentClear.Click += (s, e) => ClearPaymentFields();
            this.txtPaymentSearch.TextChanged += (s, e) => { _pendingPaymentSearchText = txtPaymentSearch.Text; paymentSearchTimer.Stop(); paymentSearchTimer.Start(); };
            this.paymentSearchTimer.Tick += (s, e) => { paymentSearchTimer.Stop(); PerformPaymentSearch(_pendingPaymentSearchText); };
            this.btnAddPaymentQuick.Click += (s, e) =>
            {
                if (_selectedBoarderId == 0)
                {
                    MessageBox.Show("Please select a boarder first.");
                    return;
                }
                ClearPaymentFields();
                ShowPaymentDetails(true);
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
            splitContainer1 = new SplitContainer();
            listPanel = new Panel();
            listSplitContainer = new SplitContainer();
            dgvBoarders = new DataGridView();
            pnlListHeader = new Panel();
            btnAddQuick = new Button();
            txtSearch = new TextBox();
            lblListTitle = new Label();
            dgvPayments = new DataGridView();
            pnlPaymentsHeader = new Panel();
            btnAddPaymentQuick = new Button();
            txtPaymentSearch = new TextBox();
            lblPaymentsTitle = new Label();
            inputScrollPanel = new Panel();
            grpPayments = new GroupBox();
            paymentLayout = new TableLayoutPanel();
            lblPaymentAmount = new Label();
            txtPaymentAmount = new TextBox();
            lblPaymentMonth = new Label();
            cmbPaymentMonth = new ComboBox();
            lblPaymentYear = new Label();
            numPaymentYear = new NumericUpDown();
            lblPaymentStatus = new Label();
            cmbPaymentStatus = new ComboBox();
            lblPaymentNotes = new Label();
            txtPaymentNotes = new TextBox();
            paymentButtonPanel = new FlowLayoutPanel();
            btnPaymentAdd = new Button();
            btnPaymentUpdate = new Button();
            btnPaymentDelete = new Button();
            btnPaymentClear = new Button();
            grpInput = new GroupBox();
            inputLayout = new TableLayoutPanel();
            lblPhoto = new Label();
            imagePanel = new FlowLayoutPanel();
            picProfile = new PictureBox();
            btnBrowseImage = new Button();
            lblFullName = new Label();
            txtFullName = new TextBox();
            lblAddress = new Label();
            txtAddress = new TextBox();
            lblPhone = new Label();
            txtPhone = new TextBox();
            lblRoom = new Label();
            cmbRooms = new ComboBox();
            lblBoardingHouse = new Label();
            cmbBoardingHouses = new ComboBox();
            chkCreateAccount = new CheckBox();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnPanel = new FlowLayoutPanel();
            row1 = new FlowLayoutPanel();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            row2 = new FlowLayoutPanel();
            btnClear = new Button();
            btnRefresh = new Button();
            row3 = new FlowLayoutPanel();
            btnBack = new Button();
            searchTimer = new System.Windows.Forms.Timer(components);
            paymentSearchTimer = new System.Windows.Forms.Timer(components);
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            listPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)listSplitContainer).BeginInit();
            listSplitContainer.Panel1.SuspendLayout();
            listSplitContainer.Panel2.SuspendLayout();
            listSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBoarders).BeginInit();
            pnlListHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPayments).BeginInit();
            pnlPaymentsHeader.SuspendLayout();
            inputScrollPanel.SuspendLayout();
            grpPayments.SuspendLayout();
            paymentLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPaymentYear).BeginInit();
            paymentButtonPanel.SuspendLayout();
            grpInput.SuspendLayout();
            inputLayout.SuspendLayout();
            imagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picProfile).BeginInit();
            btnPanel.SuspendLayout();
            row1.SuspendLayout();
            row2.SuspendLayout();
            row3.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(50, 50, 50);
            pnlTop.Controls.Add(btnBackTop);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(10);
            pnlTop.Size = new Size(899, 50);
            pnlTop.TabIndex = 1;
            // 
            // btnBackTop
            // 
            btnBackTop.BackColor = Color.White;
            btnBackTop.Dock = DockStyle.Left;
            btnBackTop.FlatStyle = FlatStyle.Flat;
            btnBackTop.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBackTop.Location = new Point(10, 10);
            btnBackTop.Name = "btnBackTop";
            btnBackTop.Size = new Size(180, 30);
            btnBackTop.TabIndex = 0;
            btnBackTop.Text = "← Back to Dashboard";
            btnBackTop.TextAlign = ContentAlignment.MiddleLeft;
            btnBackTop.UseVisualStyleBackColor = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 50);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(inputScrollPanel);
            splitContainer1.Panel2.Controls.Add(btnPanel);
            splitContainer1.Panel2Collapsed = true;
            splitContainer1.Size = new Size(899, 371);
            splitContainer1.SplitterDistance = 700;
            splitContainer1.TabIndex = 0;
            // 
            // listPanel
            // 
            listPanel.Controls.Add(listSplitContainer);
            listPanel.Dock = DockStyle.Fill;
            listPanel.Location = new Point(0, 0);
            listPanel.Name = "listPanel";
            listPanel.Padding = new Padding(10);
            listPanel.Size = new Size(899, 371);
            listPanel.TabIndex = 0;
            // 
            // listSplitContainer
            // 
            listSplitContainer.Dock = DockStyle.Fill;
            listSplitContainer.FixedPanel = FixedPanel.Panel2;
            listSplitContainer.Location = new Point(10, 10);
            listSplitContainer.Margin = new Padding(0);
            listSplitContainer.Name = "listSplitContainer";
            listSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // listSplitContainer.Panel1
            // 
            listSplitContainer.Panel1.Controls.Add(dgvBoarders);
            listSplitContainer.Panel1.Controls.Add(pnlListHeader);
            // 
            // listSplitContainer.Panel2
            // 
            listSplitContainer.Panel2.Controls.Add(dgvPayments);
            listSplitContainer.Panel2.Controls.Add(pnlPaymentsHeader);
            listSplitContainer.Size = new Size(879, 351);
            listSplitContainer.SplitterDistance = 232;
            listSplitContainer.SplitterWidth = 2;
            listSplitContainer.TabIndex = 0;
            // 
            // dgvBoarders
            // 
            dgvBoarders.AllowUserToAddRows = false;
            dgvBoarders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBoarders.BackgroundColor = Color.White;
            dgvBoarders.ColumnHeadersHeight = 35;
            dgvBoarders.Dock = DockStyle.Fill;
            dgvBoarders.Location = new Point(0, 42);
            dgvBoarders.MultiSelect = false;
            dgvBoarders.Name = "dgvBoarders";
            dgvBoarders.ReadOnly = true;
            dgvBoarders.RowHeadersVisible = false;
            dgvBoarders.RowHeadersWidth = 62;
            dgvBoarders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBoarders.Size = new Size(879, 190);
            dgvBoarders.TabIndex = 0;
            // 
            // pnlListHeader
            // 
            pnlListHeader.BackColor = Color.FromArgb(245, 245, 245);
            pnlListHeader.Controls.Add(btnAddQuick);
            pnlListHeader.Controls.Add(txtSearch);
            pnlListHeader.Controls.Add(lblListTitle);
            pnlListHeader.Dock = DockStyle.Top;
            pnlListHeader.Location = new Point(0, 0);
            pnlListHeader.Margin = new Padding(0);
            pnlListHeader.Name = "pnlListHeader";
            pnlListHeader.Padding = new Padding(7, 6, 7, 6);
            pnlListHeader.Size = new Size(879, 42);
            pnlListHeader.TabIndex = 0;
            // 
            // btnAddQuick
            // 
            btnAddQuick.BackColor = Color.FromArgb(40, 167, 69);
            btnAddQuick.Dock = DockStyle.Right;
            btnAddQuick.FlatStyle = FlatStyle.Flat;
            btnAddQuick.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddQuick.ForeColor = Color.White;
            btnAddQuick.Location = new Point(500, 6);
            btnAddQuick.Name = "btnAddQuick";
            btnAddQuick.Size = new Size(119, 30);
            btnAddQuick.TabIndex = 2;
            btnAddQuick.Text = "+ Add Boarder";
            btnAddQuick.UseVisualStyleBackColor = false;
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Right;
            txtSearch.Font = new Font("Segoe UI", 12F);
            txtSearch.Location = new Point(619, 6);
            txtSearch.Margin = new Padding(7, 3, 0, 0);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search boarders...";
            txtSearch.Size = new Size(253, 29);
            txtSearch.TabIndex = 1;
            // 
            // lblListTitle
            // 
            lblListTitle.AutoSize = true;
            lblListTitle.Dock = DockStyle.Left;
            lblListTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblListTitle.ForeColor = Color.FromArgb(0, 123, 255);
            lblListTitle.Location = new Point(7, 6);
            lblListTitle.Margin = new Padding(0, 3, 14, 0);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(138, 30);
            lblListTitle.TabIndex = 0;
            lblListTitle.Text = "Boarder List";
            // 
            // dgvPayments
            // 
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayments.BackgroundColor = Color.White;
            dgvPayments.ColumnHeadersHeight = 35;
            dgvPayments.Dock = DockStyle.Fill;
            dgvPayments.Location = new Point(0, 42);
            dgvPayments.MultiSelect = false;
            dgvPayments.Name = "dgvPayments";
            dgvPayments.ReadOnly = true;
            dgvPayments.RowHeadersVisible = false;
            dgvPayments.RowHeadersWidth = 62;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.Size = new Size(879, 75);
            dgvPayments.TabIndex = 1;
            // 
            // pnlPaymentsHeader
            // 
            pnlPaymentsHeader.BackColor = Color.FromArgb(245, 245, 245);
            pnlPaymentsHeader.Controls.Add(btnAddPaymentQuick);
            pnlPaymentsHeader.Controls.Add(txtPaymentSearch);
            pnlPaymentsHeader.Controls.Add(lblPaymentsTitle);
            pnlPaymentsHeader.Dock = DockStyle.Top;
            pnlPaymentsHeader.Location = new Point(0, 0);
            pnlPaymentsHeader.Margin = new Padding(0);
            pnlPaymentsHeader.Name = "pnlPaymentsHeader";
            pnlPaymentsHeader.Padding = new Padding(7, 6, 7, 6);
            pnlPaymentsHeader.Size = new Size(879, 42);
            pnlPaymentsHeader.TabIndex = 0;
            // 
            // btnAddPaymentQuick
            // 
            btnAddPaymentQuick.BackColor = Color.FromArgb(40, 167, 69);
            btnAddPaymentQuick.Dock = DockStyle.Right;
            btnAddPaymentQuick.FlatStyle = FlatStyle.Flat;
            btnAddPaymentQuick.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddPaymentQuick.ForeColor = Color.White;
            btnAddPaymentQuick.Location = new Point(500, 6);
            btnAddPaymentQuick.Name = "btnAddPaymentQuick";
            btnAddPaymentQuick.Size = new Size(119, 30);
            btnAddPaymentQuick.TabIndex = 2;
            btnAddPaymentQuick.Text = "+ Add Payment";
            btnAddPaymentQuick.UseVisualStyleBackColor = false;
            // 
            // txtPaymentSearch
            // 
            txtPaymentSearch.Dock = DockStyle.Right;
            txtPaymentSearch.Font = new Font("Segoe UI", 12F);
            txtPaymentSearch.Location = new Point(619, 6);
            txtPaymentSearch.Margin = new Padding(7, 3, 0, 0);
            txtPaymentSearch.Name = "txtPaymentSearch";
            txtPaymentSearch.PlaceholderText = "Search payments...";
            txtPaymentSearch.Size = new Size(253, 29);
            txtPaymentSearch.TabIndex = 1;
            // 
            // lblPaymentsTitle
            // 
            lblPaymentsTitle.AutoSize = true;
            lblPaymentsTitle.Dock = DockStyle.Left;
            lblPaymentsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblPaymentsTitle.ForeColor = Color.FromArgb(0, 123, 255);
            lblPaymentsTitle.Location = new Point(7, 6);
            lblPaymentsTitle.Margin = new Padding(0, 3, 14, 0);
            lblPaymentsTitle.Name = "lblPaymentsTitle";
            lblPaymentsTitle.Size = new Size(114, 30);
            lblPaymentsTitle.TabIndex = 0;
            lblPaymentsTitle.Text = "Payments";
            // 
            // inputScrollPanel
            // 
            inputScrollPanel.AutoScroll = true;
            inputScrollPanel.Controls.Add(grpPayments);
            inputScrollPanel.Controls.Add(grpInput);
            inputScrollPanel.Dock = DockStyle.Fill;
            inputScrollPanel.Location = new Point(0, 0);
            inputScrollPanel.Name = "inputScrollPanel";
            inputScrollPanel.Padding = new Padding(10);
            inputScrollPanel.Size = new Size(96, 0);
            inputScrollPanel.TabIndex = 0;
            // 
            // grpPayments
            // 
            grpPayments.AutoSize = true;
            grpPayments.Controls.Add(paymentLayout);
            grpPayments.Dock = DockStyle.Top;
            grpPayments.Font = new Font("Segoe UI", 10F);
            grpPayments.Location = new Point(10, 473);
            grpPayments.Margin = new Padding(3, 7, 3, 3);
            grpPayments.Name = "grpPayments";
            grpPayments.Padding = new Padding(10);
            grpPayments.Size = new Size(76, 354);
            grpPayments.TabIndex = 1;
            grpPayments.TabStop = false;
            grpPayments.Text = "Payments";
            // 
            // paymentLayout
            // 
            paymentLayout.AutoSize = true;
            paymentLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            paymentLayout.ColumnCount = 2;
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            paymentLayout.Controls.Add(lblPaymentAmount, 0, 0);
            paymentLayout.Controls.Add(txtPaymentAmount, 1, 0);
            paymentLayout.Controls.Add(lblPaymentMonth, 0, 1);
            paymentLayout.Controls.Add(cmbPaymentMonth, 1, 1);
            paymentLayout.Controls.Add(lblPaymentYear, 0, 2);
            paymentLayout.Controls.Add(numPaymentYear, 1, 2);
            paymentLayout.Controls.Add(lblPaymentStatus, 0, 3);
            paymentLayout.Controls.Add(cmbPaymentStatus, 1, 3);
            paymentLayout.Controls.Add(lblPaymentNotes, 0, 4);
            paymentLayout.Controls.Add(txtPaymentNotes, 1, 4);
            paymentLayout.Controls.Add(paymentButtonPanel, 0, 5);
            paymentLayout.Dock = DockStyle.Fill;
            paymentLayout.Location = new Point(10, 28);
            paymentLayout.Name = "paymentLayout";
            paymentLayout.Padding = new Padding(5);
            paymentLayout.RowCount = 6;
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.RowStyles.Add(new RowStyle());
            paymentLayout.Size = new Size(56, 316);
            paymentLayout.TabIndex = 0;
            // 
            // lblPaymentAmount
            // 
            lblPaymentAmount.Anchor = AnchorStyles.Left;
            lblPaymentAmount.AutoSize = true;
            lblPaymentAmount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentAmount.Location = new Point(8, 5);
            lblPaymentAmount.Name = "lblPaymentAmount";
            lblPaymentAmount.Size = new Size(10, 114);
            lblPaymentAmount.TabIndex = 0;
            lblPaymentAmount.Text = "Amount:";
            // 
            // txtPaymentAmount
            // 
            txtPaymentAmount.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPaymentAmount.Location = new Point(24, 49);
            txtPaymentAmount.Name = "txtPaymentAmount";
            txtPaymentAmount.Size = new Size(24, 25);
            txtPaymentAmount.TabIndex = 1;
            // 
            // lblPaymentMonth
            // 
            lblPaymentMonth.Anchor = AnchorStyles.Left;
            lblPaymentMonth.AutoSize = true;
            lblPaymentMonth.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentMonth.Location = new Point(8, 119);
            lblPaymentMonth.Name = "lblPaymentMonth";
            lblPaymentMonth.Size = new Size(10, 76);
            lblPaymentMonth.TabIndex = 2;
            lblPaymentMonth.Text = "Month:";
            // 
            // cmbPaymentMonth
            // 
            cmbPaymentMonth.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbPaymentMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMonth.Location = new Point(24, 144);
            cmbPaymentMonth.Name = "cmbPaymentMonth";
            cmbPaymentMonth.Size = new Size(24, 25);
            cmbPaymentMonth.TabIndex = 3;
            // 
            // lblPaymentYear
            // 
            lblPaymentYear.Anchor = AnchorStyles.Left;
            lblPaymentYear.AutoSize = true;
            lblPaymentYear.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentYear.Location = new Point(8, 195);
            lblPaymentYear.Name = "lblPaymentYear";
            lblPaymentYear.Size = new Size(10, 95);
            lblPaymentYear.TabIndex = 4;
            lblPaymentYear.Text = "Year:";
            // 
            // numPaymentYear
            // 
            numPaymentYear.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numPaymentYear.Location = new Point(24, 230);
            numPaymentYear.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            numPaymentYear.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            numPaymentYear.Name = "numPaymentYear";
            numPaymentYear.Size = new Size(24, 25);
            numPaymentYear.TabIndex = 5;
            numPaymentYear.Value = new decimal(new int[] { 2024, 0, 0, 0 });
            // 
            // lblPaymentStatus
            // 
            lblPaymentStatus.Anchor = AnchorStyles.Left;
            lblPaymentStatus.AutoSize = true;
            lblPaymentStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentStatus.Location = new Point(8, 290);
            lblPaymentStatus.Name = "lblPaymentStatus";
            lblPaymentStatus.Size = new Size(10, 133);
            lblPaymentStatus.TabIndex = 6;
            lblPaymentStatus.Text = "Status:";
            // 
            // cmbPaymentStatus
            // 
            cmbPaymentStatus.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentStatus.Location = new Point(24, 344);
            cmbPaymentStatus.Name = "cmbPaymentStatus";
            cmbPaymentStatus.Size = new Size(24, 25);
            cmbPaymentStatus.TabIndex = 7;
            // 
            // lblPaymentNotes
            // 
            lblPaymentNotes.Anchor = AnchorStyles.Left;
            lblPaymentNotes.AutoSize = true;
            lblPaymentNotes.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentNotes.Location = new Point(8, 423);
            lblPaymentNotes.Name = "lblPaymentNotes";
            lblPaymentNotes.Size = new Size(10, 95);
            lblPaymentNotes.TabIndex = 8;
            lblPaymentNotes.Text = "Notes:";
            // 
            // txtPaymentNotes
            // 
            txtPaymentNotes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPaymentNotes.Location = new Point(24, 445);
            txtPaymentNotes.Multiline = true;
            txtPaymentNotes.Name = "txtPaymentNotes";
            txtPaymentNotes.Size = new Size(24, 51);
            txtPaymentNotes.TabIndex = 9;
            // 
            // paymentButtonPanel
            // 
            paymentButtonPanel.AutoSize = true;
            paymentLayout.SetColumnSpan(paymentButtonPanel, 2);
            paymentButtonPanel.Controls.Add(btnPaymentAdd);
            paymentButtonPanel.Controls.Add(btnPaymentUpdate);
            paymentButtonPanel.Controls.Add(btnPaymentDelete);
            paymentButtonPanel.Controls.Add(btnPaymentClear);
            paymentButtonPanel.Dock = DockStyle.Fill;
            paymentButtonPanel.Location = new Point(5, 523);
            paymentButtonPanel.Margin = new Padding(0, 5, 0, 0);
            paymentButtonPanel.Name = "paymentButtonPanel";
            paymentButtonPanel.Size = new Size(46, 120);
            paymentButtonPanel.TabIndex = 10;
            // 
            // btnPaymentAdd
            // 
            btnPaymentAdd.BackColor = Color.FromArgb(40, 167, 69);
            btnPaymentAdd.FlatStyle = FlatStyle.Flat;
            btnPaymentAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPaymentAdd.ForeColor = Color.White;
            btnPaymentAdd.Location = new Point(3, 3);
            btnPaymentAdd.Name = "btnPaymentAdd";
            btnPaymentAdd.Size = new Size(77, 24);
            btnPaymentAdd.TabIndex = 0;
            btnPaymentAdd.Text = "Add";
            btnPaymentAdd.UseVisualStyleBackColor = false;
            // 
            // btnPaymentUpdate
            // 
            btnPaymentUpdate.BackColor = Color.FromArgb(0, 123, 255);
            btnPaymentUpdate.FlatStyle = FlatStyle.Flat;
            btnPaymentUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPaymentUpdate.ForeColor = Color.White;
            btnPaymentUpdate.Location = new Point(3, 33);
            btnPaymentUpdate.Name = "btnPaymentUpdate";
            btnPaymentUpdate.Size = new Size(77, 24);
            btnPaymentUpdate.TabIndex = 1;
            btnPaymentUpdate.Text = "Update";
            btnPaymentUpdate.UseVisualStyleBackColor = false;
            // 
            // btnPaymentDelete
            // 
            btnPaymentDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnPaymentDelete.FlatStyle = FlatStyle.Flat;
            btnPaymentDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPaymentDelete.ForeColor = Color.White;
            btnPaymentDelete.Location = new Point(3, 63);
            btnPaymentDelete.Name = "btnPaymentDelete";
            btnPaymentDelete.Size = new Size(77, 24);
            btnPaymentDelete.TabIndex = 2;
            btnPaymentDelete.Text = "Delete";
            btnPaymentDelete.UseVisualStyleBackColor = false;
            // 
            // btnPaymentClear
            // 
            btnPaymentClear.BackColor = Color.FromArgb(0, 123, 255);
            btnPaymentClear.FlatStyle = FlatStyle.Flat;
            btnPaymentClear.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPaymentClear.ForeColor = Color.White;
            btnPaymentClear.Location = new Point(3, 93);
            btnPaymentClear.Name = "btnPaymentClear";
            btnPaymentClear.Size = new Size(77, 24);
            btnPaymentClear.TabIndex = 3;
            btnPaymentClear.Text = "Clear";
            btnPaymentClear.UseVisualStyleBackColor = false;
            // 
            // grpInput
            // 
            grpInput.AutoSize = true;
            grpInput.Controls.Add(inputLayout);
            grpInput.Dock = DockStyle.Top;
            grpInput.Font = new Font("Segoe UI", 10F);
            grpInput.Location = new Point(10, 10);
            grpInput.Name = "grpInput";
            grpInput.Padding = new Padding(10);
            grpInput.Size = new Size(76, 463);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "Boarder Details";
            // 
            // inputLayout
            // 
            inputLayout.AutoSize = true;
            inputLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            inputLayout.ColumnCount = 2;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            inputLayout.Controls.Add(lblPhoto, 0, 0);
            inputLayout.Controls.Add(imagePanel, 1, 0);
            inputLayout.Controls.Add(lblFullName, 0, 1);
            inputLayout.Controls.Add(txtFullName, 1, 1);
            inputLayout.Controls.Add(lblAddress, 0, 2);
            inputLayout.Controls.Add(txtAddress, 1, 2);
            inputLayout.Controls.Add(lblPhone, 0, 3);
            inputLayout.Controls.Add(txtPhone, 1, 3);
            inputLayout.Controls.Add(lblRoom, 0, 4);
            inputLayout.Controls.Add(cmbRooms, 1, 4);
            inputLayout.Controls.Add(lblBoardingHouse, 0, 5);
            inputLayout.Controls.Add(cmbBoardingHouses, 1, 5);
            inputLayout.Controls.Add(chkCreateAccount, 1, 6);
            inputLayout.Controls.Add(lblUsername, 0, 7);
            inputLayout.Controls.Add(txtUsername, 1, 7);
            inputLayout.Controls.Add(lblPassword, 0, 8);
            inputLayout.Controls.Add(txtPassword, 1, 8);
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.Location = new Point(10, 28);
            inputLayout.Name = "inputLayout";
            inputLayout.Padding = new Padding(5);
            inputLayout.RowCount = 9;
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.RowStyles.Add(new RowStyle());
            inputLayout.Size = new Size(56, 425);
            inputLayout.TabIndex = 0;
            // 
            // lblPhoto
            // 
            lblPhoto.Anchor = AnchorStyles.Left;
            lblPhoto.AutoSize = true;
            lblPhoto.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPhoto.Location = new Point(8, 5);
            lblPhoto.Name = "lblPhoto";
            lblPhoto.Size = new Size(10, 228);
            lblPhoto.TabIndex = 0;
            lblPhoto.Text = "Profile Photo:";
            // 
            // imagePanel
            // 
            imagePanel.AutoSize = true;
            imagePanel.Controls.Add(picProfile);
            imagePanel.Controls.Add(btnBrowseImage);
            imagePanel.FlowDirection = FlowDirection.TopDown;
            imagePanel.Location = new Point(24, 8);
            imagePanel.Name = "imagePanel";
            imagePanel.Size = new Size(24, 159);
            imagePanel.TabIndex = 1;
            // 
            // picProfile
            // 
            picProfile.BorderStyle = BorderStyle.FixedSingle;
            picProfile.Location = new Point(3, 3);
            picProfile.Name = "picProfile";
            picProfile.Size = new Size(120, 120);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.TabIndex = 0;
            picProfile.TabStop = false;
            // 
            // btnBrowseImage
            // 
            btnBrowseImage.BackColor = Color.FromArgb(0, 123, 255);
            btnBrowseImage.FlatStyle = FlatStyle.Flat;
            btnBrowseImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBrowseImage.ForeColor = Color.White;
            btnBrowseImage.Location = new Point(3, 132);
            btnBrowseImage.Margin = new Padding(3, 6, 3, 3);
            btnBrowseImage.Name = "btnBrowseImage";
            btnBrowseImage.Size = new Size(96, 24);
            btnBrowseImage.TabIndex = 1;
            btnBrowseImage.Text = "Upload Photo";
            btnBrowseImage.UseVisualStyleBackColor = false;
            // 
            // lblFullName
            // 
            lblFullName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFullName.Location = new Point(8, 233);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(1, 17);
            lblFullName.TabIndex = 2;
            lblFullName.Text = "Full Name:";
            // 
            // txtFullName
            // 
            txtFullName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtFullName.Location = new Point(24, 236);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(24, 25);
            txtFullName.TabIndex = 3;
            // 
            // lblAddress
            // 
            lblAddress.Anchor = AnchorStyles.Left;
            lblAddress.AutoSize = true;
            lblAddress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAddress.Location = new Point(8, 264);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new Size(10, 133);
            lblAddress.TabIndex = 4;
            lblAddress.Text = "Address:";
            // 
            // txtAddress
            // 
            txtAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtAddress.Location = new Point(24, 318);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(24, 25);
            txtAddress.TabIndex = 5;
            // 
            // lblPhone
            // 
            lblPhone.Anchor = AnchorStyles.Left;
            lblPhone.AutoSize = true;
            lblPhone.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPhone.Location = new Point(8, 397);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(10, 114);
            lblPhone.TabIndex = 6;
            lblPhone.Text = "Phone:";
            // 
            // txtPhone
            // 
            txtPhone.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPhone.Location = new Point(24, 441);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(24, 25);
            txtPhone.TabIndex = 7;
            // 
            // lblRoom
            // 
            lblRoom.Anchor = AnchorStyles.Left;
            lblRoom.AutoSize = true;
            lblRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRoom.Location = new Point(8, 511);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(10, 95);
            lblRoom.TabIndex = 8;
            lblRoom.Text = "Room:";
            // 
            // cmbRooms
            // 
            cmbRooms.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbRooms.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRooms.Location = new Point(24, 545);
            cmbRooms.Name = "cmbRooms";
            cmbRooms.Size = new Size(24, 25);
            cmbRooms.TabIndex = 9;
            // 
            // lblBoardingHouse
            // 
            lblBoardingHouse.Anchor = AnchorStyles.Left;
            lblBoardingHouse.AutoSize = true;
            lblBoardingHouse.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBoardingHouse.Location = new Point(8, 606);
            lblBoardingHouse.Name = "lblBoardingHouse";
            lblBoardingHouse.Size = new Size(10, 266);
            lblBoardingHouse.TabIndex = 10;
            lblBoardingHouse.Text = "Boarding House:";
            // 
            // cmbBoardingHouses
            // 
            cmbBoardingHouses.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbBoardingHouses.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoardingHouses.Location = new Point(24, 725);
            cmbBoardingHouses.Name = "cmbBoardingHouses";
            cmbBoardingHouses.Size = new Size(24, 25);
            cmbBoardingHouses.TabIndex = 11;
            // 
            // chkCreateAccount
            // 
            chkCreateAccount.AutoSize = true;
            chkCreateAccount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkCreateAccount.Location = new Point(24, 875);
            chkCreateAccount.Name = "chkCreateAccount";
            chkCreateAccount.Size = new Size(24, 23);
            chkCreateAccount.TabIndex = 12;
            chkCreateAccount.Text = "Create Login Account";
            // 
            // lblUsername
            // 
            lblUsername.Anchor = AnchorStyles.Left;
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsername.Location = new Point(8, 901);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(10, 152);
            lblUsername.TabIndex = 13;
            lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            txtUsername.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtUsername.Enabled = false;
            txtUsername.Location = new Point(24, 964);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(24, 25);
            txtUsername.TabIndex = 14;
            // 
            // lblPassword
            // 
            lblPassword.Anchor = AnchorStyles.Left;
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.Location = new Point(8, 1053);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(10, 171);
            lblPassword.TabIndex = 15;
            lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Enabled = false;
            txtPassword.Location = new Point(24, 1126);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(24, 25);
            txtPassword.TabIndex = 16;
            // 
            // btnPanel
            // 
            btnPanel.Controls.Add(row1);
            btnPanel.Controls.Add(row2);
            btnPanel.Controls.Add(row3);
            btnPanel.Dock = DockStyle.Bottom;
            btnPanel.FlowDirection = FlowDirection.TopDown;
            btnPanel.Location = new Point(0, -38);
            btnPanel.Name = "btnPanel";
            btnPanel.Padding = new Padding(10, 5, 10, 5);
            btnPanel.Size = new Size(96, 138);
            btnPanel.TabIndex = 1;
            btnPanel.WrapContents = false;
            // 
            // row1
            // 
            row1.AutoSize = true;
            row1.Controls.Add(btnAdd);
            row1.Controls.Add(btnUpdate);
            row1.Controls.Add(btnDelete);
            row1.Location = new Point(13, 8);
            row1.Name = "row1";
            row1.Size = new Size(172, 82);
            row1.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(40, 167, 69);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(3, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(80, 35);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            btnUpdate.BackColor = Color.FromArgb(0, 123, 255);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Location = new Point(89, 3);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(80, 35);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(3, 44);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(80, 35);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            // 
            // row2
            // 
            row2.AutoSize = true;
            row2.Controls.Add(btnClear);
            row2.Controls.Add(btnRefresh);
            row2.Location = new Point(10, 98);
            row2.Margin = new Padding(0, 5, 0, 0);
            row2.Name = "row2";
            row2.Size = new Size(252, 36);
            row2.TabIndex = 1;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.FromArgb(0, 123, 255);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(3, 3);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(120, 30);
            btnClear.TabIndex = 0;
            btnClear.Text = "Clear Fields";
            btnClear.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(0, 123, 255);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(129, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 30);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh List";
            btnRefresh.UseVisualStyleBackColor = false;
            // 
            // row3
            // 
            row3.AutoSize = true;
            row3.Controls.Add(btnBack);
            row3.Location = new Point(10, 144);
            row3.Margin = new Padding(0, 10, 0, 0);
            row3.Name = "row3";
            row3.Size = new Size(256, 41);
            row3.TabIndex = 2;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.LightSlateGray;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(3, 3);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(250, 35);
            btnBack.TabIndex = 0;
            btnBack.Text = "Back to Dashboard";
            btnBack.UseVisualStyleBackColor = false;
            // 
            // searchTimer
            // 
            searchTimer.Interval = 350;
            // 
            // paymentSearchTimer
            // 
            paymentSearchTimer.Interval = 350;
            // 
            // FormBoarders
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(899, 421);
            Controls.Add(splitContainer1);
            Controls.Add(pnlTop);
            Name = "FormBoarders";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Boarders";
            WindowState = FormWindowState.Maximized;
            pnlTop.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            listPanel.ResumeLayout(false);
            listSplitContainer.Panel1.ResumeLayout(false);
            listSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)listSplitContainer).EndInit();
            listSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBoarders).EndInit();
            pnlListHeader.ResumeLayout(false);
            pnlListHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPayments).EndInit();
            pnlPaymentsHeader.ResumeLayout(false);
            pnlPaymentsHeader.PerformLayout();
            inputScrollPanel.ResumeLayout(false);
            inputScrollPanel.PerformLayout();
            grpPayments.ResumeLayout(false);
            grpPayments.PerformLayout();
            paymentLayout.ResumeLayout(false);
            paymentLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPaymentYear).EndInit();
            paymentButtonPanel.ResumeLayout(false);
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            inputLayout.ResumeLayout(false);
            inputLayout.PerformLayout();
            imagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picProfile).EndInit();
            btnPanel.ResumeLayout(false);
            btnPanel.PerformLayout();
            row1.ResumeLayout(false);
            row2.ResumeLayout(false);
            row3.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void LoadRooms()
        {
            LoadRoomsForSelectedHouse();
        }

        private void LoadRoomsForSelectedHouse()
        {
            try
            {
                cmbRooms.Items.Clear();
                int? houseId = GetSelectedBoardingHouseId();
                if (houseId == null)
                {
                    cmbRooms.SelectedIndex = -1;
                    return;
                }

                var rooms = _roomRepo.GetRoomsByBoardingHouse(houseId.Value);
                foreach (DataRow row in rooms.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string roomNum = row["RoomNumber"].ToString() ?? "";
                    string cap = row["Capacity"].ToString() ?? "";
                    
                    cmbRooms.Items.Add(new ComboBoxItem { Text = roomNum + " (Cap: " + cap + ")", Value = id });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms: " + ex.Message);
            }
        }

        private void LoadBoardingHouses()
        {
            try
            {
                List<BoardingHouse> houses;
                if (_currentUser.Role == "SuperAdmin")
                {
                    houses = _boardingHouseRepo.GetAll();
                }
                else
                {
                    houses = _boardingHouseRepo.GetAllByOwner(_currentUser.Id);
                }

                cmbBoardingHouses.Items.Clear();
                foreach (var house in houses)
                {
                    cmbBoardingHouses.Items.Add(new ComboBoxItem { Text = house.Name, Value = house.Id });
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

        private int? GetSelectedBoardingHouseId()
        {
            if (cmbBoardingHouses.SelectedItem is ComboBoxItem item)
            {
                return (int)item.Value;
            }
            return null;
        }

        private void LoadBoarders()
        {
            try
            {
                var boarders = _boarderRepo.GetAllBoarders();
                BindBoarders(boarders);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading boarders: " + ex.Message);
            }
        }

        private void BindBoarders(DataTable boarders)
        {
            if (_isUpdatingResults) return;
            _isUpdatingResults = true;
            try
            {
                dgvBoarders.DataSource = boarders;
                if (dgvBoarders.Columns["Id"] != null) dgvBoarders.Columns["Id"].Visible = false;
                if (dgvBoarders.Columns["UserId"] != null) dgvBoarders.Columns["UserId"].Visible = false;
                if (dgvBoarders.Columns["RoomId"] != null) dgvBoarders.Columns["RoomId"].Visible = false;
                if (dgvBoarders.Columns["BoardingHouseId"] != null) dgvBoarders.Columns["BoardingHouseId"].Visible = false;
                if (dgvBoarders.Columns["BoardingHouseName"] != null) dgvBoarders.Columns["BoardingHouseName"].Visible = false;
                if (dgvBoarders.Columns["ProfilePicturePath"] != null) dgvBoarders.Columns["ProfilePicturePath"].Visible = false;
                if (dgvBoarders.Rows.Count > 0)
                {
                    dgvBoarders.ClearSelection();
                    dgvBoarders.CurrentCell = null;
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
                LoadBoarders();
                HideDetailsPanel();
                return;
            }

            try
            {
                var results = _boarderRepo.SearchBoarders(keyword);
                BindBoarders(results);
                HideDetailsPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching boarders: " + ex.Message);
            }
        }

        private void RefreshBoarders()
        {
            string keyword = txtSearch != null ? txtSearch.Text.Trim() : _pendingSearchText;
            PerformSearch(keyword);
        }

        private void ToggleAccountFields(bool enable)
        {
            txtUsername.Enabled = enable;
            txtPassword.Enabled = enable;
        }

        private void BtnBrowseImage_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Profile Picture";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedImagePath = ofd.FileName;
                    ReplaceProfileImage(_selectedImagePath);
                }
            }
        }

        private string? SaveImage(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath)) return null;

            try
            {
                string folderPath = Path.Combine(Application.StartupPath, "ProfilePictures");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                string destPath = Path.Combine(folderPath, fileName);
                
                File.Copy(sourcePath, destPath, true);
                
                return "ProfilePictures/" + fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving image: " + ex.Message);
                return null;
            }
        }

        private void AddBoarder()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required.");
                return;
            }

            int? roomId = null;
            if (cmbRooms.SelectedItem is ComboBoxItem item)
            {
                roomId = (int)item.Value;
            }

            int? boardingHouseId = null;
            if (cmbBoardingHouses.SelectedItem is ComboBoxItem houseItem)
            {
                boardingHouseId = (int)houseItem.Value;
            }

            int? userId = null;
            if (chkCreateAccount.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Username and Password are required for account creation.");
                    return;
                }

                var newUser = new User
                {
                    Username = txtUsername.Text,
                    Role = "Boarder"
                };

                try
                {
                    userId = _userRepo.CreateUser(newUser, txtPassword.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating user account: " + ex.Message);
                    return;
                }
            }

            string? savedImagePath = null;
            if (!string.IsNullOrEmpty(_selectedImagePath))
            {
                savedImagePath = SaveImage(_selectedImagePath);
            }

            var boarder = new Boarder
            {
                FullName = txtFullName.Text,
                Address = txtAddress.Text,
                Phone = txtPhone.Text,
                RoomId = roomId,
                BoardingHouseId = boardingHouseId,
                UserId = userId,
                ProfilePicturePath = savedImagePath
            };

            try
            {
                _boarderRepo.AddBoarder(boarder);
                MessageBox.Show("Boarder added successfully!");
                LoadBoarders();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding boarder: " + ex.Message);
            }
        }

        private void UpdateBoarder()
        {
            if (_selectedBoarderId == 0)
            {
                MessageBox.Show("Please select a boarder to update.");
                return;
            }

            int? roomId = null;
            if (cmbRooms.SelectedItem is ComboBoxItem item)
            {
                roomId = (int)item.Value;
            }

            int? boardingHouseId = null;
            if (cmbBoardingHouses.SelectedItem is ComboBoxItem houseItem)
            {
                boardingHouseId = (int)houseItem.Value;
            }

            string? imagePath = _currentDbImagePath;
            if (!string.IsNullOrEmpty(_selectedImagePath))
            {
                imagePath = SaveImage(_selectedImagePath);
            }

            var boarder = new Boarder
            {
                Id = _selectedBoarderId,
                FullName = txtFullName.Text,
                Address = txtAddress.Text,
                Phone = txtPhone.Text,
                RoomId = roomId,
                BoardingHouseId = boardingHouseId,
                ProfilePicturePath = imagePath
            };

            try
            {
                _boarderRepo.UpdateBoarder(boarder);
                MessageBox.Show("Boarder updated successfully!");
                LoadBoarders();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating boarder: " + ex.Message);
            }
        }

        private void DeleteBoarder()
        {
            if (_selectedBoarderId == 0)
            {
                MessageBox.Show("Please select a boarder to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure? This will delete the boarder record.", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _boarderRepo.DeleteBoarder(_selectedBoarderId);
                    MessageBox.Show("Boarder deleted.");
                    LoadBoarders();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting boarder: " + ex.Message);
                }
            }
        }

        private void ShowBoarderDetails(bool isNew)
        {
            if (splitContainer1 != null) splitContainer1.Panel2Collapsed = false;
            if (grpInput != null) grpInput.Visible = true;
            if (grpPayments != null) grpPayments.Visible = false;

            if (btnAdd != null)
            {
                btnAdd.Visible = isNew;
                btnAdd.Enabled = isNew;
            }

            if (btnUpdate != null)
            {
                btnUpdate.Visible = !isNew;
                btnUpdate.Enabled = !isNew;
            }

            if (btnDelete != null)
            {
                btnDelete.Visible = !isNew;
                btnDelete.Enabled = !isNew;
            }
        }

        private void ShowPaymentDetails(bool isNew)
        {
            if (splitContainer1 != null) splitContainer1.Panel2Collapsed = false;
            if (grpInput != null) grpInput.Visible = false;
            if (grpPayments != null) grpPayments.Visible = true;

            if (btnPaymentAdd != null)
            {
                btnPaymentAdd.Visible = isNew;
                btnPaymentAdd.Enabled = isNew;
            }

            if (btnPaymentUpdate != null)
            {
                btnPaymentUpdate.Visible = !isNew;
                btnPaymentUpdate.Enabled = !isNew;
            }

            if (btnPaymentDelete != null)
            {
                btnPaymentDelete.Visible = !isNew;
                btnPaymentDelete.Enabled = !isNew;
            }

            if (btnPaymentClear != null)
            {
                btnPaymentClear.Visible = true;
                btnPaymentClear.Enabled = true;
            }
        }

        private void ShowDetailsPanel()
        {
            ShowBoarderDetails(_selectedBoarderId == 0);
        }

        private void HideDetailsPanel()
        {
            if (splitContainer1 != null) splitContainer1.Panel2Collapsed = true;

            if (grpInput != null) grpInput.Visible = false;
            if (grpPayments != null) grpPayments.Visible = false;

            if (btnAdd != null)
            {
                btnAdd.Visible = true;
                btnAdd.Enabled = true;
            }

            if (btnUpdate != null)
            {
                btnUpdate.Visible = false;
                btnUpdate.Enabled = false;
            }

            if (btnDelete != null)
            {
                btnDelete.Visible = false;
                btnDelete.Enabled = false;
            }

            if (btnPaymentAdd != null)
            {
                btnPaymentAdd.Visible = true;
                btnPaymentAdd.Enabled = true;
            }

            if (btnPaymentUpdate != null)
            {
                btnPaymentUpdate.Visible = false;
                btnPaymentUpdate.Enabled = false;
            }

            if (btnPaymentDelete != null)
            {
                btnPaymentDelete.Visible = false;
                btnPaymentDelete.Enabled = false;
            }
        }

        private void ReplaceProfileImage(string? relativePath)
        {
            if (picProfile == null) return;

            if (picProfile.Image != null)
            {
                var oldImage = picProfile.Image;
                picProfile.Image = null;
                oldImage.Dispose();
            }

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            string fullPath = Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.Combine(Application.StartupPath, relativePath);

            if (!File.Exists(fullPath))
            {
                return;
            }

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = Image.FromStream(stream))
                {
                    picProfile.Image = new Bitmap(temp);
                }
            }
            catch
            {
                if (picProfile.Image != null)
                {
                    var oldImage = picProfile.Image;
                    picProfile.Image = null;
                    oldImage.Dispose();
                }
            }
        }

        private void DgvBoarders_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingResults) return;
            if (e.RowIndex < 0 || dgvBoarders.Rows.Count == 0) return;

            var row = dgvBoarders.Rows[e.RowIndex];
            if (row.Cells["Id"].Value == null) return;

            _selectedBoarderId = Convert.ToInt32(row.Cells["Id"].Value);
            txtFullName.Text = row.Cells["FullName"].Value?.ToString() ?? "";
            txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
            txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";

            int boardingHouseId = 0;
            if (row.Cells["BoardingHouseId"].Value != DBNull.Value && row.Cells["BoardingHouseId"].Value != null)
                boardingHouseId = Convert.ToInt32(row.Cells["BoardingHouseId"].Value);

            cmbBoardingHouses.SelectedIndex = -1;
            foreach (ComboBoxItem item in cmbBoardingHouses.Items)
            {
                if ((int)item.Value == boardingHouseId)
                {
                    cmbBoardingHouses.SelectedItem = item;
                    break;
                }
            }

            int roomId = 0;
            if (row.Cells["RoomId"].Value != DBNull.Value && row.Cells["RoomId"].Value != null)
                roomId = Convert.ToInt32(row.Cells["RoomId"].Value);

            cmbRooms.SelectedIndex = -1;
            foreach (ComboBoxItem item in cmbRooms.Items)
            {
                if ((int)item.Value == roomId)
                {
                    cmbRooms.SelectedItem = item;
                    break;
                }
            }

            _currentDbImagePath = row.Cells["ProfilePicturePath"].Value?.ToString();
            _selectedImagePath = null;
            ReplaceProfileImage(_currentDbImagePath);
            ShowBoarderDetails(false);
            LoadPaymentsForSelectedBoarder();
        }

        private void ClearFields()
        {
            _selectedBoarderId = 0;
            txtFullName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            cmbRooms.SelectedIndex = -1;
            cmbBoardingHouses.SelectedIndex = -1;
            chkCreateAccount.Checked = false;
            txtUsername.Clear();
            txtPassword.Clear();
            ReplaceProfileImage(null);
            _selectedImagePath = null;
            _currentDbImagePath = null;
            HideDetailsPanel();
        }

        private void LoadPaymentDropdowns()
        {
            cmbPaymentMonth.Items.Clear();
            cmbPaymentMonth.Items.Add("January");
            cmbPaymentMonth.Items.Add("February");
            cmbPaymentMonth.Items.Add("March");
            cmbPaymentMonth.Items.Add("April");
            cmbPaymentMonth.Items.Add("May");
            cmbPaymentMonth.Items.Add("June");
            cmbPaymentMonth.Items.Add("July");
            cmbPaymentMonth.Items.Add("August");
            cmbPaymentMonth.Items.Add("September");
            cmbPaymentMonth.Items.Add("October");
            cmbPaymentMonth.Items.Add("November");
            cmbPaymentMonth.Items.Add("December");

            cmbPaymentStatus.Items.Clear();
            cmbPaymentStatus.Items.Add("Pending");
            cmbPaymentStatus.Items.Add("Paid");
            cmbPaymentStatus.Items.Add("Overdue");
        }

        private void LoadPaymentsForSelectedBoarder()
        {
            if (_selectedBoarderId == 0)
            {
                dgvPayments.DataSource = null;
                return;
            }

            try
            {
                var payments = _paymentRepo.GetPaymentsByBoarderId(_selectedBoarderId);
                dgvPayments.DataSource = payments;
                FormatPaymentsGrid();
                
                // Set total payments label if we had one, or just update the title
                lblPaymentsTitle.Text = $"Payment History - {_boarderRepo.GetBoarderById(_selectedBoarderId)?.FullName ?? "Unknown"}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payments: " + ex.Message);
            }
        }

        private void FormatPaymentsGrid()
        {
            if (dgvPayments.Columns.Count == 0) return;

            var colId = dgvPayments.Columns["Id"];
            if (colId != null) colId.Visible = false;

            var colBoarderId = dgvPayments.Columns["BoarderId"];
            if (colBoarderId != null) colBoarderId.Visible = false;

            var colAmount = dgvPayments.Columns["Amount"];
            if (colAmount != null)
            {
                colAmount.DefaultCellStyle.Format = "C2";
                colAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            var colPaymentDate = dgvPayments.Columns["PaymentDate"];
            if (colPaymentDate != null)
            {
                colPaymentDate.DefaultCellStyle.Format = "MM/dd/yyyy";
            }
        }

        private void PerformPaymentSearch(string keyword)
        {
            if (_selectedBoarderId == 0) return;

            try
            {
                // First get all payments for the boarder, then filter locally
                var allPayments = _paymentRepo.GetPaymentsByBoarderId(_selectedBoarderId);
                var filteredPayments = FilterPaymentsLocally(allPayments, keyword);
                dgvPayments.DataSource = filteredPayments;
                FormatPaymentsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching payments: " + ex.Message);
            }
        }

        private DataTable FilterPaymentsLocally(DataTable payments, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return payments;

            var filtered = payments.Clone();
            keyword = keyword.ToLower();

            foreach (DataRow row in payments.Rows)
            {
                if (row["MonthPaid"]?.ToString()?.ToLower().Contains(keyword) == true ||
                    row["Status"]?.ToString()?.ToLower().Contains(keyword) == true ||
                    row["Amount"]?.ToString()?.Contains(keyword) == true ||
                    row["Notes"]?.ToString()?.ToLower().Contains(keyword) == true)
                {
                    filtered.ImportRow(row);
                }
            }

            return filtered;
        }

        private void DgvPayments_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingPaymentResults) return;
            if (e.RowIndex < 0 || dgvPayments.Rows.Count == 0) return;

            var row = dgvPayments.Rows[e.RowIndex];
            if (row.Cells["Id"].Value == null) return;

            _selectedPaymentId = Convert.ToInt32(row.Cells["Id"].Value);
            txtPaymentAmount.Text = row.Cells["Amount"].Value?.ToString() ?? "";
            cmbPaymentMonth.Text = row.Cells["MonthPaid"].Value?.ToString() ?? "";
            numPaymentYear.Value = Convert.ToInt32(row.Cells["YearPaid"].Value ?? 0);
            cmbPaymentStatus.Text = row.Cells["Status"].Value?.ToString() ?? "";
            txtPaymentNotes.Text = row.Cells["Notes"].Value?.ToString() ?? "";
            ShowPaymentDetails(false);
        }

        private void AddPayment()
        {
            if (_selectedBoarderId == 0)
            {
                MessageBox.Show("Please select a boarder first.");
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.");
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbPaymentMonth.Text))
            {
                MessageBox.Show("Please select a payment month.");
                return;
            }

            var payment = new Payment
            {
                BoarderId = _selectedBoarderId,
                Amount = amount,
                MonthPaid = cmbPaymentMonth.Text,
                YearPaid = (int)numPaymentYear.Value,
                Status = cmbPaymentStatus.Text,
                Notes = txtPaymentNotes.Text,
                PaymentDate = DateTime.Now
            };

            try
            {
                _paymentRepo.AddPayment(payment);
                MessageBox.Show("Payment added successfully!");
                LoadPaymentsForSelectedBoarder();
                ClearPaymentFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding payment: " + ex.Message);
            }
        }

        private void UpdatePayment()
        {
            if (_selectedPaymentId == 0)
            {
                MessageBox.Show("Please select a payment to update.");
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.");
                return;
            }

            var payment = new Payment
            {
                Id = _selectedPaymentId,
                BoarderId = _selectedBoarderId,
                Amount = amount,
                MonthPaid = cmbPaymentMonth.Text,
                YearPaid = (int)numPaymentYear.Value,
                Status = cmbPaymentStatus.Text,
                Notes = txtPaymentNotes.Text,
                PaymentDate = DateTime.Now
            };

            try
            {
                _paymentRepo.UpdatePayment(payment);
                MessageBox.Show("Payment updated successfully!");
                LoadPaymentsForSelectedBoarder();
                ClearPaymentFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating payment: " + ex.Message);
            }
        }

        private void DeletePayment()
        {
            if (_selectedPaymentId == 0)
            {
                MessageBox.Show("Please select a payment to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure? This will delete the payment record.", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _paymentRepo.DeletePayment(_selectedPaymentId);
                    MessageBox.Show("Payment deleted.");
                    LoadPaymentsForSelectedBoarder();
                    ClearPaymentFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting payment: " + ex.Message);
                }
            }
        }

        private void ClearPaymentFields()
        {
            _selectedPaymentId = 0;
            txtPaymentAmount.Clear();
            cmbPaymentMonth.SelectedIndex = -1;
            numPaymentYear.Value = DateTime.Now.Year;
            cmbPaymentStatus.SelectedIndex = -1;
            txtPaymentNotes.Clear();
        }

        private class ComboBoxItem
        {
            public string Text { get; set; } = "";
            public object Value { get; set; } = null!;
            public override string ToString() => Text;
        }
    }
}
