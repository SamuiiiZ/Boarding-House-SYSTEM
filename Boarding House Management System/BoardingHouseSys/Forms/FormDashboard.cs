#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8622
using System;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using BoardingHouseSys.Data;
using BoardingHouseSys.Models;
using BoardingHouseSys.UI;
using MySql.Data.MySqlClient;
using System.Data;

namespace BoardingHouseSys.Forms
{
    public class FormDashboard : Form
    {
        private User _currentUser;
        private BoardingHouse? _currentBoardingHouse;
        private BoardingHouse? _selectedHouseDetails;
        private BoarderRepository _boarderRepo;
        private BoardingHouseRepository _boardingHouseRepo;
        private DatabaseHelper _dbHelper;
        private System.Windows.Forms.Timer _statsTimer;
        
        // UI Controls - Exposed for Designer
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.FlowLayoutPanel pnlSidebar;
        private System.Windows.Forms.Button btnMyProperties;
        private System.Windows.Forms.Button btnManageBoarders;
        private System.Windows.Forms.Button btnManageRooms;
        private System.Windows.Forms.Button btnManageUsers;
        // btnReports removed
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnMyDetails;
        private System.Windows.Forms.Button btnBoardingHouses;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblStats; // Keep for fallback or re-purpose
        private System.Windows.Forms.DataGridView dgvSearchResults;
        private System.Windows.Forms.SplitContainer splitResults;
        private System.Windows.Forms.Panel panelList;
        private System.Windows.Forms.Label lblListTitle;
        
        // Dashboard Stats Controls
        private System.Windows.Forms.FlowLayoutPanel pnlDashboardStats;
        private System.Windows.Forms.Label lblTotalBoarders;
        private System.Windows.Forms.Label lblTotalIncome;
        private System.Windows.Forms.Label lblOccupancy;
        private System.Windows.Forms.Label lblPendingPayments;
        private BoardingHouseSys.UI.PieChartControl pieChartOccupancy;
        private BoardingHouseSys.UI.LineChartControl lineChartIncome;
        private System.Windows.Forms.DataGridView dgvRevenueBreakdown;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Label lblDetailsTitle;
        private System.Windows.Forms.Label lblDetailsHint;
        private System.Windows.Forms.TableLayoutPanel tblDetails;
        private System.Windows.Forms.Label lblDetailName;
        private System.Windows.Forms.Label lblDetailAddress;
        private System.Windows.Forms.Label lblDetailDescription;
        private System.Windows.Forms.Label lblDetailRules;
        private System.Windows.Forms.Label lblDetailAmenities;
        private System.Windows.Forms.Label lblDetailOwner;
        private System.Windows.Forms.TextBox txtDetailName;
        private System.Windows.Forms.TextBox txtDetailAddress;
        private System.Windows.Forms.TextBox txtDetailDescription;
        private System.Windows.Forms.TextBox txtDetailRules;
        private System.Windows.Forms.TextBox txtDetailAmenities;
        private System.Windows.Forms.TextBox txtDetailOwner;
        private System.Windows.Forms.GroupBox grpImages;
        private System.Windows.Forms.TableLayoutPanel imageTable;
        private System.Windows.Forms.PictureBox pbImage1;
        private System.Windows.Forms.PictureBox pbImage2;
        private System.Windows.Forms.PictureBox pbImage3;
        private System.Windows.Forms.FlowLayoutPanel pnlImageActions;
        private System.Windows.Forms.Button btnUploadImages;
        private System.Windows.Forms.Label lblImageHint;
        private int _selectedImageSlot = 1;
        private System.Windows.Forms.PictureBox pbRoomPreview;
        private Label lblWelcome;
        private FlowLayoutPanel pnlHeaderRight;
        private Button btnLogout;
        private PictureBox pbProfile;
        private TextBox txtSearch;
        private TextBox txtBoardingHouseSearch;
        private Panel pnlHeader;
        private System.Windows.Forms.Label lblRoomPreview;
        private Panel pnlDetailActions;
        private System.Windows.Forms.Label lblDetailSummaryTitle;
        private System.Windows.Forms.Label lblDetailSummary;
        private System.Windows.Forms.Timer? _searchDebounce;
        private string _pendingSearchText = string.Empty;
        private bool _isUpdatingResults;

        public FormDashboard()
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            InitializeSearchDebounce();
            WireEvents();
        }

        public FormDashboard(User user)
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            InitializeSearchDebounce();
            WireEvents();
            
            _currentUser = user;
            _boarderRepo = new BoarderRepository();
            _boardingHouseRepo = new BoardingHouseRepository();
            _dbHelper = new DatabaseHelper();
            
            if (lblWelcome != null)
                lblWelcome.Text = $"Welcome, {user.Username} ({user.Role})";

            LoadProfilePicture();
            SetupRoleBasedAccess();
            if (txtSearch != null)
            {
                if (_currentUser.Role == "SuperAdmin" || _currentUser.Role == "Admin")
                    txtSearch.PlaceholderText = "Search boarders...";
                else
                    txtSearch.PlaceholderText = "Search boarding houses...";
                UITheme.ApplyTextBoxStyle(txtSearch);
            }
            ClearBoardingHouseDetails();
            
            ApplyModernSidebar();
            InitializeDashboardControls();
            if (pnlDashboardStats != null)
            {
                pnlDashboardStats.Resize += (s, e) => UpdateDashboardLayoutSizes();
                UpdateDashboardLayoutSizes();
            }
            StartStatsTimer();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyHeaderStyle(pnlHeader);
            UITheme.ApplyPanelStyle(panelList, UITheme.LightColor);
            UITheme.ApplyPanelStyle(panelDetails, Color.WhiteSmoke);

            if (lblListTitle != null) UITheme.ApplyHeaderLabelStyle(lblListTitle);
            if (lblDetailsTitle != null) UITheme.ApplyHeaderLabelStyle(lblDetailsTitle);
            if (lblDetailsHint != null) UITheme.ApplyLabelStyle(lblDetailsHint);
            if (lblDetailSummaryTitle != null) UITheme.ApplySubHeaderLabelStyle(lblDetailSummaryTitle);
            if (lblDetailSummary != null) lblDetailSummary.ForeColor = UITheme.DarkColor;

            if (btnUploadImages != null) UITheme.ApplyButtonStyle(btnUploadImages);

            if (btnLogout != null)
            {
                btnLogout.BackColor = UITheme.DangerColor;
                btnLogout.ForeColor = Color.White;
                btnLogout.FlatStyle = FlatStyle.Flat;
            }

            if (txtDetailName != null) UITheme.ApplyTextBoxStyle(txtDetailName);
            if (txtDetailAddress != null) UITheme.ApplyTextBoxStyle(txtDetailAddress);
            if (txtDetailDescription != null) UITheme.ApplyTextBoxStyle(txtDetailDescription);
            if (txtDetailRules != null) UITheme.ApplyTextBoxStyle(txtDetailRules);
            if (txtDetailAmenities != null) UITheme.ApplyTextBoxStyle(txtDetailAmenities);
            if (txtDetailOwner != null) UITheme.ApplyTextBoxStyle(txtDetailOwner);
            if (txtBoardingHouseSearch != null) UITheme.ApplyTextBoxStyle(txtBoardingHouseSearch);
            if (dgvSearchResults != null) UITheme.ApplyDataGridViewStyle(dgvSearchResults);
        }

        private void ApplyModernSidebar()
        {
            pnlSidebar.BackColor = UITheme.SidebarColor;
            foreach (Control ctrl in pnlSidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    UITheme.ApplySidebarButtonStyle(btn, false);
                    btn.Margin = new Padding(0, 5, 0, 5);
                }
            }
            // Set 'My Properties' as active by default for now (or whichever is logical)
            // But we don't have a specific "Home" button active logic here yet.
            // Let's just style them.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                _searchDebounce?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            mainContainer = new SplitContainer();
            pnlSidebar = new FlowLayoutPanel();
            btnMyProperties = new Button();
            btnManageBoarders = new Button();
            btnManageRooms = new Button();
            btnManageUsers = new Button();
            btnSettings = new Button();
            btnMyDetails = new Button();
            btnBoardingHouses = new Button();
            pnlContent = new Panel();
            splitResults = new SplitContainer();
            panelList = new Panel();
            dgvSearchResults = new DataGridView();
            pnlDashboardStats = new FlowLayoutPanel();
            txtBoardingHouseSearch = new TextBox();
            lblListTitle = new Label();
            panelDetails = new Panel();
            pnlDetailActions = new Panel();
            lblDetailSummaryTitle = new Label();
            lblDetailSummary = new Label();
            grpImages = new GroupBox();
            imageTable = new TableLayoutPanel();
            pbImage1 = new PictureBox();
            pbImage2 = new PictureBox();
            pbImage3 = new PictureBox();
            pnlImageActions = new FlowLayoutPanel();
            btnUploadImages = new Button();
            lblImageHint = new Label();
            pbRoomPreview = new PictureBox();
            lblRoomPreview = new Label();
            tblDetails = new TableLayoutPanel();
            lblDetailName = new Label();
            txtDetailName = new TextBox();
            lblDetailAddress = new Label();
            txtDetailAddress = new TextBox();
            lblDetailDescription = new Label();
            txtDetailDescription = new TextBox();
            lblDetailRules = new Label();
            txtDetailRules = new TextBox();
            lblDetailAmenities = new Label();
            txtDetailAmenities = new TextBox();
            lblDetailOwner = new Label();
            txtDetailOwner = new TextBox();
            lblDetailsHint = new Label();
            lblDetailsTitle = new Label();
            lblStats = new Label();
            lblWelcome = new Label();
            pnlHeaderRight = new FlowLayoutPanel();
            btnLogout = new Button();
            pbProfile = new PictureBox();
            txtSearch = new TextBox();
            pnlHeader = new Panel();
            ((ISupportInitialize)mainContainer).BeginInit();
            mainContainer.Panel1.SuspendLayout();
            mainContainer.Panel2.SuspendLayout();
            mainContainer.SuspendLayout();
            pnlSidebar.SuspendLayout();
            pnlContent.SuspendLayout();
            ((ISupportInitialize)splitResults).BeginInit();
            splitResults.Panel1.SuspendLayout();
            splitResults.Panel2.SuspendLayout();
            splitResults.SuspendLayout();
            panelList.SuspendLayout();
            ((ISupportInitialize)dgvSearchResults).BeginInit();
            panelDetails.SuspendLayout();
            pnlDetailActions.SuspendLayout();
            grpImages.SuspendLayout();
            imageTable.SuspendLayout();
            ((ISupportInitialize)pbImage1).BeginInit();
            ((ISupportInitialize)pbImage2).BeginInit();
            ((ISupportInitialize)pbImage3).BeginInit();
            pnlImageActions.SuspendLayout();
            ((ISupportInitialize)pbRoomPreview).BeginInit();
            tblDetails.SuspendLayout();
            pnlHeaderRight.SuspendLayout();
            ((ISupportInitialize)pbProfile).BeginInit();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // mainContainer
            // 
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.FixedPanel = FixedPanel.Panel1;
            mainContainer.IsSplitterFixed = true;
            mainContainer.Location = new Point(0, 80);
            mainContainer.Name = "mainContainer";
            // 
            // mainContainer.Panel1
            // 
            mainContainer.Panel1.Controls.Add(pnlSidebar);
            // 
            // mainContainer.Panel2
            // 
            mainContainer.Panel2.Controls.Add(pnlContent);
            mainContainer.Size = new Size(1284, 621);
            mainContainer.SplitterDistance = 240;
            mainContainer.SplitterWidth = 1;
            mainContainer.TabIndex = 0;
            // 
            // pnlSidebar
            // 
            pnlSidebar.AutoScroll = true;
            pnlSidebar.BackColor = Color.FromArgb(240, 240, 240);
            pnlSidebar.Controls.Add(btnMyProperties);
            pnlSidebar.Controls.Add(btnManageBoarders);
            pnlSidebar.Controls.Add(btnManageRooms);
            pnlSidebar.Controls.Add(btnManageUsers);
            pnlSidebar.Controls.Add(btnSettings);
            pnlSidebar.Controls.Add(btnMyDetails);
            pnlSidebar.Controls.Add(btnBoardingHouses);
            pnlSidebar.Dock = DockStyle.Fill;
            pnlSidebar.FlowDirection = FlowDirection.TopDown;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(10);
            pnlSidebar.Size = new Size(240, 621);
            pnlSidebar.TabIndex = 0;
            // 
            // btnMyProperties
            // 
            btnMyProperties.BackColor = Color.White;
            btnMyProperties.Cursor = Cursors.Hand;
            btnMyProperties.FlatStyle = FlatStyle.Flat;
            btnMyProperties.Font = new Font("Segoe UI", 11F);
            btnMyProperties.Location = new Point(10, 10);
            btnMyProperties.Margin = new Padding(0, 0, 0, 10);
            btnMyProperties.Name = "btnMyProperties";
            btnMyProperties.Size = new Size(210, 45);
            btnMyProperties.TabIndex = 0;
            btnMyProperties.Text = "My Properties";
            btnMyProperties.TextAlign = ContentAlignment.MiddleLeft;
            btnMyProperties.UseVisualStyleBackColor = false;
            // 
            // btnManageBoarders
            // 
            btnManageBoarders.BackColor = Color.White;
            btnManageBoarders.Cursor = Cursors.Hand;
            btnManageBoarders.FlatStyle = FlatStyle.Flat;
            btnManageBoarders.Font = new Font("Segoe UI", 11F);
            btnManageBoarders.Location = new Point(10, 65);
            btnManageBoarders.Margin = new Padding(0, 0, 0, 10);
            btnManageBoarders.Name = "btnManageBoarders";
            btnManageBoarders.Size = new Size(210, 45);
            btnManageBoarders.TabIndex = 1;
            btnManageBoarders.Text = "Manage Boarders";
            btnManageBoarders.TextAlign = ContentAlignment.MiddleLeft;
            btnManageBoarders.UseVisualStyleBackColor = false;
            // 
            // btnManageRooms
            // 
            btnManageRooms.BackColor = Color.White;
            btnManageRooms.Cursor = Cursors.Hand;
            btnManageRooms.FlatStyle = FlatStyle.Flat;
            btnManageRooms.Font = new Font("Segoe UI", 11F);
            btnManageRooms.Location = new Point(10, 120);
            btnManageRooms.Margin = new Padding(0, 0, 0, 10);
            btnManageRooms.Name = "btnManageRooms";
            btnManageRooms.Size = new Size(210, 45);
            btnManageRooms.TabIndex = 2;
            btnManageRooms.Text = "Manage Rooms";
            btnManageRooms.TextAlign = ContentAlignment.MiddleLeft;
            btnManageRooms.UseVisualStyleBackColor = false;
            // 
            // btnManageUsers
            // 
            btnManageUsers.BackColor = Color.White;
            btnManageUsers.Cursor = Cursors.Hand;
            btnManageUsers.FlatStyle = FlatStyle.Flat;
            btnManageUsers.Font = new Font("Segoe UI", 11F);
            btnManageUsers.Location = new Point(10, 175);
            btnManageUsers.Margin = new Padding(0, 0, 0, 10);
            btnManageUsers.Name = "btnManageUsers";
            btnManageUsers.Size = new Size(210, 45);
            btnManageUsers.TabIndex = 3;
            btnManageUsers.Text = "Manage Users";
            btnManageUsers.TextAlign = ContentAlignment.MiddleLeft;
            btnManageUsers.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            btnSettings.BackColor = Color.White;
            btnSettings.Cursor = Cursors.Hand;
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.Font = new Font("Segoe UI", 11F);
            btnSettings.Location = new Point(10, 230);
            btnSettings.Margin = new Padding(0, 0, 0, 10);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(210, 45);
            btnSettings.TabIndex = 5;
            btnSettings.Text = "Settings";
            btnSettings.TextAlign = ContentAlignment.MiddleLeft;
            btnSettings.UseVisualStyleBackColor = false;
            // 
            // btnMyDetails
            // 
            btnMyDetails.BackColor = Color.White;
            btnMyDetails.Cursor = Cursors.Hand;
            btnMyDetails.FlatStyle = FlatStyle.Flat;
            btnMyDetails.Font = new Font("Segoe UI", 11F);
            btnMyDetails.Location = new Point(10, 285);
            btnMyDetails.Margin = new Padding(0, 0, 0, 10);
            btnMyDetails.Name = "btnMyDetails";
            btnMyDetails.Size = new Size(210, 45);
            btnMyDetails.TabIndex = 6;
            btnMyDetails.Text = "My Details";
            btnMyDetails.TextAlign = ContentAlignment.MiddleLeft;
            btnMyDetails.UseVisualStyleBackColor = false;
            // 
            // btnBoardingHouses
            // 
            btnBoardingHouses.BackColor = Color.White;
            btnBoardingHouses.Cursor = Cursors.Hand;
            btnBoardingHouses.FlatStyle = FlatStyle.Flat;
            btnBoardingHouses.Font = new Font("Segoe UI", 11F);
            btnBoardingHouses.Location = new Point(10, 340);
            btnBoardingHouses.Margin = new Padding(0, 0, 0, 10);
            btnBoardingHouses.Name = "btnBoardingHouses";
            btnBoardingHouses.Size = new Size(210, 45);
            btnBoardingHouses.TabIndex = 7;
            btnBoardingHouses.Text = "Boarding Houses";
            btnBoardingHouses.TextAlign = ContentAlignment.MiddleLeft;
            btnBoardingHouses.UseVisualStyleBackColor = false;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(splitResults);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(20);
            pnlContent.Size = new Size(1043, 621);
            pnlContent.TabIndex = 0;
            // 
            // splitResults
            // 
            splitResults.Dock = DockStyle.Fill;
            splitResults.FixedPanel = FixedPanel.Panel2;
            splitResults.IsSplitterFixed = true;
            splitResults.Location = new Point(20, 20);
            splitResults.Name = "splitResults";
            // 
            // splitResults.Panel1
            // 
            splitResults.Panel1.Controls.Add(panelList);
            splitResults.Panel1MinSize = 520;
            // 
            // splitResults.Panel2
            // 
            splitResults.Panel2.Controls.Add(panelDetails);
            splitResults.Panel2Collapsed = true;
            splitResults.Panel2MinSize = 420;
            splitResults.Size = new Size(1003, 581);
            splitResults.SplitterDistance = 520;
            splitResults.SplitterWidth = 1;
            splitResults.TabIndex = 0;
            // 
            // panelList
            // 
            panelList.BackColor = Color.White;
            panelList.Controls.Add(dgvSearchResults);
            panelList.Controls.Add(pnlDashboardStats);
            panelList.Controls.Add(txtBoardingHouseSearch);
            panelList.Controls.Add(lblListTitle);
            panelList.Dock = DockStyle.Fill;
            panelList.Location = new Point(0, 0);
            panelList.Name = "panelList";
            panelList.Size = new Size(1003, 581);
            panelList.TabIndex = 0;
            // 
            // dgvSearchResults
            // 
            dgvSearchResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSearchResults.BackgroundColor = Color.White;
            dgvSearchResults.ColumnHeadersHeight = 34;
            dgvSearchResults.Dock = DockStyle.Fill;
            dgvSearchResults.Location = new Point(0, 55);
            dgvSearchResults.Name = "dgvSearchResults";
            dgvSearchResults.ReadOnly = true;
            dgvSearchResults.RowHeadersVisible = false;
            dgvSearchResults.RowHeadersWidth = 62;
            dgvSearchResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSearchResults.Size = new Size(1003, 526);
            dgvSearchResults.TabIndex = 0;
            dgvSearchResults.Visible = false;
            // 
            // pnlDashboardStats
            // 
            pnlDashboardStats.AutoScroll = true;
            pnlDashboardStats.Dock = DockStyle.Fill;
            pnlDashboardStats.Location = new Point(0, 55);
            pnlDashboardStats.Name = "pnlDashboardStats";
            pnlDashboardStats.Padding = new Padding(20);
            pnlDashboardStats.Size = new Size(1003, 526);
            pnlDashboardStats.TabIndex = 3;
            // 
            // txtBoardingHouseSearch
            // 
            txtBoardingHouseSearch.Dock = DockStyle.Top;
            txtBoardingHouseSearch.Font = new Font("Segoe UI", 10F);
            txtBoardingHouseSearch.Location = new Point(0, 30);
            txtBoardingHouseSearch.Margin = new Padding(0, 0, 0, 8);
            txtBoardingHouseSearch.Name = "txtBoardingHouseSearch";
            txtBoardingHouseSearch.PlaceholderText = "Search boarding houses...";
            txtBoardingHouseSearch.Size = new Size(1003, 25);
            txtBoardingHouseSearch.TabIndex = 3;
            txtBoardingHouseSearch.Visible = false;
            // 
            // lblListTitle
            // 
            lblListTitle.Dock = DockStyle.Top;
            lblListTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblListTitle.Location = new Point(0, 0);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Padding = new Padding(0, 0, 0, 4);
            lblListTitle.Size = new Size(1003, 30);
            lblListTitle.TabIndex = 2;
            lblListTitle.Text = "Search Results";
            // 
            // panelDetails
            // 
            panelDetails.AutoScroll = true;
            panelDetails.BackColor = Color.WhiteSmoke;
            panelDetails.Controls.Add(pnlDetailActions);
            panelDetails.Controls.Add(grpImages);
            panelDetails.Controls.Add(pbRoomPreview);
            panelDetails.Controls.Add(lblRoomPreview);
            panelDetails.Controls.Add(tblDetails);
            panelDetails.Controls.Add(lblDetailsHint);
            panelDetails.Controls.Add(lblDetailsTitle);
            panelDetails.Dock = DockStyle.Fill;
            panelDetails.Location = new Point(0, 0);
            panelDetails.Name = "panelDetails";
            panelDetails.Padding = new Padding(16);
            panelDetails.Size = new Size(96, 100);
            panelDetails.TabIndex = 1;
            // 
            // pnlDetailActions
            // 
            pnlDetailActions.AutoScroll = true;
            pnlDetailActions.Controls.Add(lblDetailSummaryTitle);
            pnlDetailActions.Controls.Add(lblDetailSummary);
            pnlDetailActions.Dock = DockStyle.Bottom;
            pnlDetailActions.Location = new Point(16, 790);
            pnlDetailActions.Name = "pnlDetailActions";
            pnlDetailActions.Padding = new Padding(10);
            pnlDetailActions.Size = new Size(91, 150);
            pnlDetailActions.TabIndex = 6;
            // 
            // lblDetailSummaryTitle
            // 
            lblDetailSummaryTitle.AutoSize = true;
            lblDetailSummaryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDetailSummaryTitle.ForeColor = Color.FromArgb(0, 123, 255);
            lblDetailSummaryTitle.Location = new Point(10, 10);
            lblDetailSummaryTitle.Margin = new Padding(0, 0, 0, 10);
            lblDetailSummaryTitle.Name = "lblDetailSummaryTitle";
            lblDetailSummaryTitle.Size = new Size(154, 21);
            lblDetailSummaryTitle.TabIndex = 0;
            lblDetailSummaryTitle.Text = "Property Summary";
            // 
            // lblDetailSummary
            // 
            lblDetailSummary.AutoSize = true;
            lblDetailSummary.Font = new Font("Segoe UI", 10F);
            lblDetailSummary.ForeColor = Color.FromArgb(52, 58, 64);
            lblDetailSummary.Location = new Point(13, 104);
            lblDetailSummary.MaximumSize = new Size(380, 0);
            lblDetailSummary.Name = "lblDetailSummary";
            lblDetailSummary.Size = new Size(199, 19);
            lblDetailSummary.TabIndex = 1;
            lblDetailSummary.Text = "Select a property to see details.";
            // 
            // grpImages
            // 
            grpImages.Controls.Add(imageTable);
            grpImages.Controls.Add(lblImageHint);
            grpImages.Dock = DockStyle.Top;
            grpImages.Location = new Point(16, 556);
            grpImages.Name = "grpImages";
            grpImages.Padding = new Padding(8);
            grpImages.Size = new Size(91, 250);
            grpImages.TabIndex = 4;
            grpImages.TabStop = false;
            grpImages.Text = "Pictures";
            // 
            // imageTable
            // 
            imageTable.ColumnCount = 3;
            imageTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            imageTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            imageTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            imageTable.Controls.Add(pbImage1, 0, 0);
            imageTable.Controls.Add(pbImage2, 1, 0);
            imageTable.Controls.Add(pbImage3, 2, 0);
            imageTable.Controls.Add(pnlImageActions, 0, 1);
            imageTable.Dock = DockStyle.Fill;
            imageTable.Location = new Point(8, 24);
            imageTable.Name = "imageTable";
            imageTable.RowCount = 2;
            imageTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            imageTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            imageTable.Size = new Size(75, 218);
            imageTable.TabIndex = 0;
            // 
            // pbImage1
            // 
            pbImage1.BackColor = Color.White;
            pbImage1.BorderStyle = BorderStyle.FixedSingle;
            pbImage1.Dock = DockStyle.Fill;
            pbImage1.Location = new Point(3, 3);
            pbImage1.Name = "pbImage1";
            pbImage1.Size = new Size(18, 134);
            pbImage1.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage1.TabIndex = 0;
            pbImage1.TabStop = false;
            // 
            // pbImage2
            // 
            pbImage2.BackColor = Color.White;
            pbImage2.BorderStyle = BorderStyle.FixedSingle;
            pbImage2.Dock = DockStyle.Fill;
            pbImage2.Location = new Point(27, 3);
            pbImage2.Name = "pbImage2";
            pbImage2.Size = new Size(18, 134);
            pbImage2.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage2.TabIndex = 1;
            pbImage2.TabStop = false;
            // 
            // pbImage3
            // 
            pbImage3.BackColor = Color.White;
            pbImage3.BorderStyle = BorderStyle.FixedSingle;
            pbImage3.Dock = DockStyle.Fill;
            pbImage3.Location = new Point(51, 3);
            pbImage3.Name = "pbImage3";
            pbImage3.Size = new Size(21, 134);
            pbImage3.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage3.TabIndex = 2;
            pbImage3.TabStop = false;
            // 
            // pnlImageActions
            // 
            pnlImageActions.AutoSize = true;
            pnlImageActions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            imageTable.SetColumnSpan(pnlImageActions, 3);
            pnlImageActions.Controls.Add(btnUploadImages);
            pnlImageActions.Dock = DockStyle.Fill;
            pnlImageActions.Location = new Point(3, 143);
            pnlImageActions.Name = "pnlImageActions";
            pnlImageActions.Padding = new Padding(6, 4, 6, 4);
            pnlImageActions.Size = new Size(69, 72);
            pnlImageActions.TabIndex = 3;
            pnlImageActions.WrapContents = false;
            // 
            // btnUploadImages
            // 
            btnUploadImages.BackColor = Color.FromArgb(40, 167, 69);
            btnUploadImages.FlatStyle = FlatStyle.Flat;
            btnUploadImages.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnUploadImages.ForeColor = Color.White;
            btnUploadImages.Location = new Point(9, 7);
            btnUploadImages.Name = "btnUploadImages";
            btnUploadImages.Size = new Size(120, 26);
            btnUploadImages.TabIndex = 0;
            btnUploadImages.Text = "Upload Pictures";
            btnUploadImages.UseVisualStyleBackColor = false;
            // 
            // lblImageHint
            // 
            lblImageHint.AutoSize = true;
            lblImageHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblImageHint.ForeColor = Color.DimGray;
            lblImageHint.Location = new Point(12, 24);
            lblImageHint.Name = "lblImageHint";
            lblImageHint.Size = new Size(226, 13);
            lblImageHint.TabIndex = 1;
            lblImageHint.Text = "Click a picture to choose which one to replace.";
            // 
            // pbRoomPreview
            // 
            pbRoomPreview.BorderStyle = BorderStyle.FixedSingle;
            pbRoomPreview.Dock = DockStyle.Top;
            pbRoomPreview.Location = new Point(16, 356);
            pbRoomPreview.Name = "pbRoomPreview";
            pbRoomPreview.Size = new Size(91, 200);
            pbRoomPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pbRoomPreview.TabIndex = 2;
            pbRoomPreview.TabStop = false;
            pbRoomPreview.Visible = false;
            // 
            // lblRoomPreview
            // 
            lblRoomPreview.AutoSize = true;
            lblRoomPreview.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRoomPreview.Location = new Point(16, 262);
            lblRoomPreview.Name = "lblRoomPreview";
            lblRoomPreview.Size = new Size(107, 19);
            lblRoomPreview.TabIndex = 3;
            lblRoomPreview.Text = "Room Preview";
            lblRoomPreview.Visible = false;
            // 
            // tblDetails
            // 
            tblDetails.AutoSize = true;
            tblDetails.ColumnCount = 2;
            tblDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tblDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblDetails.Controls.Add(lblDetailName, 0, 0);
            tblDetails.Controls.Add(txtDetailName, 1, 0);
            tblDetails.Controls.Add(lblDetailAddress, 0, 1);
            tblDetails.Controls.Add(txtDetailAddress, 1, 1);
            tblDetails.Controls.Add(lblDetailDescription, 0, 2);
            tblDetails.Controls.Add(txtDetailDescription, 1, 2);
            tblDetails.Controls.Add(lblDetailRules, 0, 3);
            tblDetails.Controls.Add(txtDetailRules, 1, 3);
            tblDetails.Controls.Add(lblDetailAmenities, 0, 4);
            tblDetails.Controls.Add(txtDetailAmenities, 1, 4);
            tblDetails.Controls.Add(lblDetailOwner, 0, 5);
            tblDetails.Controls.Add(txtDetailOwner, 1, 5);
            tblDetails.Dock = DockStyle.Top;
            tblDetails.Location = new Point(16, 51);
            tblDetails.Name = "tblDetails";
            tblDetails.Padding = new Padding(0, 8, 0, 8);
            tblDetails.RowCount = 6;
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.RowStyles.Add(new RowStyle());
            tblDetails.Size = new Size(91, 305);
            tblDetails.TabIndex = 2;
            // 
            // lblDetailName
            // 
            lblDetailName.AutoSize = true;
            lblDetailName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailName.ForeColor = Color.DimGray;
            lblDetailName.Location = new Point(3, 8);
            lblDetailName.Name = "lblDetailName";
            lblDetailName.Size = new Size(49, 19);
            lblDetailName.TabIndex = 0;
            lblDetailName.Text = "Name";
            // 
            // txtDetailName
            // 
            txtDetailName.Dock = DockStyle.Fill;
            txtDetailName.Location = new Point(133, 8);
            txtDetailName.Margin = new Padding(3, 0, 3, 8);
            txtDetailName.Name = "txtDetailName";
            txtDetailName.Size = new Size(1, 23);
            txtDetailName.TabIndex = 1;
            // 
            // lblDetailAddress
            // 
            lblDetailAddress.AutoSize = true;
            lblDetailAddress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailAddress.ForeColor = Color.DimGray;
            lblDetailAddress.Location = new Point(3, 39);
            lblDetailAddress.Name = "lblDetailAddress";
            lblDetailAddress.Size = new Size(63, 19);
            lblDetailAddress.TabIndex = 2;
            lblDetailAddress.Text = "Address";
            // 
            // txtDetailAddress
            // 
            txtDetailAddress.Dock = DockStyle.Fill;
            txtDetailAddress.Location = new Point(133, 39);
            txtDetailAddress.Margin = new Padding(3, 0, 3, 8);
            txtDetailAddress.Name = "txtDetailAddress";
            txtDetailAddress.Size = new Size(1, 23);
            txtDetailAddress.TabIndex = 3;
            // 
            // lblDetailDescription
            // 
            lblDetailDescription.AutoSize = true;
            lblDetailDescription.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailDescription.ForeColor = Color.DimGray;
            lblDetailDescription.Location = new Point(3, 70);
            lblDetailDescription.Name = "lblDetailDescription";
            lblDetailDescription.Size = new Size(85, 19);
            lblDetailDescription.TabIndex = 4;
            lblDetailDescription.Text = "Description";
            // 
            // txtDetailDescription
            // 
            txtDetailDescription.Dock = DockStyle.Fill;
            txtDetailDescription.Location = new Point(133, 70);
            txtDetailDescription.Margin = new Padding(3, 0, 3, 8);
            txtDetailDescription.MinimumSize = new Size(0, 60);
            txtDetailDescription.Multiline = true;
            txtDetailDescription.Name = "txtDetailDescription";
            txtDetailDescription.ScrollBars = ScrollBars.Vertical;
            txtDetailDescription.Size = new Size(1, 60);
            txtDetailDescription.TabIndex = 5;
            // 
            // lblDetailRules
            // 
            lblDetailRules.AutoSize = true;
            lblDetailRules.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailRules.ForeColor = Color.DimGray;
            lblDetailRules.Location = new Point(3, 138);
            lblDetailRules.Name = "lblDetailRules";
            lblDetailRules.Size = new Size(44, 19);
            lblDetailRules.TabIndex = 6;
            lblDetailRules.Text = "Rules";
            // 
            // txtDetailRules
            // 
            txtDetailRules.Dock = DockStyle.Fill;
            txtDetailRules.Location = new Point(133, 138);
            txtDetailRules.Margin = new Padding(3, 0, 3, 8);
            txtDetailRules.MinimumSize = new Size(0, 60);
            txtDetailRules.Multiline = true;
            txtDetailRules.Name = "txtDetailRules";
            txtDetailRules.ScrollBars = ScrollBars.Vertical;
            txtDetailRules.Size = new Size(1, 60);
            txtDetailRules.TabIndex = 7;
            // 
            // lblDetailAmenities
            // 
            lblDetailAmenities.AutoSize = true;
            lblDetailAmenities.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailAmenities.ForeColor = Color.DimGray;
            lblDetailAmenities.Location = new Point(3, 206);
            lblDetailAmenities.Name = "lblDetailAmenities";
            lblDetailAmenities.Size = new Size(75, 19);
            lblDetailAmenities.TabIndex = 8;
            lblDetailAmenities.Text = "Amenities";
            // 
            // txtDetailAmenities
            // 
            txtDetailAmenities.Dock = DockStyle.Fill;
            txtDetailAmenities.Location = new Point(133, 206);
            txtDetailAmenities.Margin = new Padding(3, 0, 3, 8);
            txtDetailAmenities.MinimumSize = new Size(0, 60);
            txtDetailAmenities.Multiline = true;
            txtDetailAmenities.Name = "txtDetailAmenities";
            txtDetailAmenities.ScrollBars = ScrollBars.Vertical;
            txtDetailAmenities.Size = new Size(1, 60);
            txtDetailAmenities.TabIndex = 9;
            // 
            // lblDetailOwner
            // 
            lblDetailOwner.AutoSize = true;
            lblDetailOwner.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailOwner.ForeColor = Color.DimGray;
            lblDetailOwner.Location = new Point(3, 274);
            lblDetailOwner.Name = "lblDetailOwner";
            lblDetailOwner.Size = new Size(53, 19);
            lblDetailOwner.TabIndex = 10;
            lblDetailOwner.Text = "Owner";
            // 
            // txtDetailOwner
            // 
            txtDetailOwner.Dock = DockStyle.Fill;
            txtDetailOwner.Location = new Point(133, 274);
            txtDetailOwner.Margin = new Padding(3, 0, 3, 0);
            txtDetailOwner.Name = "txtDetailOwner";
            txtDetailOwner.ReadOnly = true;
            txtDetailOwner.Size = new Size(1, 23);
            txtDetailOwner.TabIndex = 11;
            // 
            // lblDetailsHint
            // 
            lblDetailsHint.AutoSize = true;
            lblDetailsHint.Dock = DockStyle.Top;
            lblDetailsHint.Font = new Font("Segoe UI", 9F);
            lblDetailsHint.ForeColor = Color.Gray;
            lblDetailsHint.Location = new Point(16, 36);
            lblDetailsHint.Margin = new Padding(0, 6, 0, 12);
            lblDetailsHint.Name = "lblDetailsHint";
            lblDetailsHint.Size = new Size(214, 15);
            lblDetailsHint.TabIndex = 1;
            lblDetailsHint.Text = "Select a boarding house to view details.";
            // 
            // lblDetailsTitle
            // 
            lblDetailsTitle.AutoSize = true;
            lblDetailsTitle.Dock = DockStyle.Top;
            lblDetailsTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDetailsTitle.Location = new Point(16, 16);
            lblDetailsTitle.Name = "lblDetailsTitle";
            lblDetailsTitle.Size = new Size(173, 20);
            lblDetailsTitle.TabIndex = 0;
            lblDetailsTitle.Text = "Boarding House Details";
            // 
            // lblStats
            // 
            lblStats.Dock = DockStyle.Fill;
            lblStats.Font = new Font("Segoe UI", 14F);
            lblStats.Location = new Point(0, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(1003, 581);
            lblStats.TabIndex = 1;
            lblStats.Text = "Dashboard Overview";
            lblStats.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Dock = DockStyle.Left;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(108, 30);
            lblWelcome.TabIndex = 1;
            lblWelcome.Text = "Welcome";
            lblWelcome.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlHeaderRight
            // 
            pnlHeaderRight.BackColor = Color.Transparent;
            pnlHeaderRight.Controls.Add(btnLogout);
            pnlHeaderRight.Controls.Add(pbProfile);
            pnlHeaderRight.Controls.Add(txtSearch);
            pnlHeaderRight.Dock = DockStyle.Right;
            pnlHeaderRight.FlowDirection = FlowDirection.RightToLeft;
            pnlHeaderRight.Location = new Point(664, 20);
            pnlHeaderRight.Name = "pnlHeaderRight";
            pnlHeaderRight.Padding = new Padding(0, 10, 0, 0);
            pnlHeaderRight.Size = new Size(600, 40);
            pnlHeaderRight.TabIndex = 0;
            pnlHeaderRight.WrapContents = false;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.IndianRed;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(510, 10);
            btnLogout.Margin = new Padding(10, 0, 0, 0);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(90, 36);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = false;
            // 
            // pbProfile
            // 
            pbProfile.BackColor = Color.White;
            pbProfile.BorderStyle = BorderStyle.FixedSingle;
            pbProfile.Location = new Point(450, 10);
            pbProfile.Margin = new Padding(10, 0, 10, 0);
            pbProfile.Name = "pbProfile";
            pbProfile.Size = new Size(40, 40);
            pbProfile.SizeMode = PictureBoxSizeMode.Zoom;
            pbProfile.TabIndex = 1;
            pbProfile.TabStop = false;
            // 
            // txtSearch
            // 
            txtSearch.Font = new Font("Segoe UI", 12F);
            txtSearch.Location = new Point(58, 10);
            txtSearch.Margin = new Padding(0, 0, 10, 0);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search boarding houses...";
            txtSearch.Size = new Size(372, 29);
            txtSearch.TabIndex = 2;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(50, 50, 50);
            pnlHeader.Controls.Add(pnlHeaderRight);
            pnlHeader.Controls.Add(lblWelcome);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20);
            pnlHeader.Size = new Size(1284, 80);
            pnlHeader.TabIndex = 1;
            // 
            // FormDashboard
            // 
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1284, 701);
            Controls.Add(mainContainer);
            Controls.Add(pnlHeader);
            Name = "FormDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Boarding House Management System - Dashboard";
            WindowState = FormWindowState.Maximized;
            mainContainer.Panel1.ResumeLayout(false);
            mainContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)mainContainer).EndInit();
            mainContainer.ResumeLayout(false);
            pnlSidebar.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            splitResults.Panel1.ResumeLayout(false);
            splitResults.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitResults).EndInit();
            splitResults.ResumeLayout(false);
            panelList.ResumeLayout(false);
            panelList.PerformLayout();
            ((ISupportInitialize)dgvSearchResults).EndInit();
            panelDetails.ResumeLayout(false);
            panelDetails.PerformLayout();
            pnlDetailActions.ResumeLayout(false);
            pnlDetailActions.PerformLayout();
            grpImages.ResumeLayout(false);
            grpImages.PerformLayout();
            imageTable.ResumeLayout(false);
            imageTable.PerformLayout();
            ((ISupportInitialize)pbImage1).EndInit();
            ((ISupportInitialize)pbImage2).EndInit();
            ((ISupportInitialize)pbImage3).EndInit();
            pnlImageActions.ResumeLayout(false);
            ((ISupportInitialize)pbRoomPreview).EndInit();
            tblDetails.ResumeLayout(false);
            tblDetails.PerformLayout();
            pnlHeaderRight.ResumeLayout(false);
            pnlHeaderRight.PerformLayout();
            ((ISupportInitialize)pbProfile).EndInit();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
        }

        // Helper to configure buttons in InitComponent (called for each button)
        // Note: In true Designer code, these properties are set inline. 
        // I will use a helper here for brevity but call it from InitializeComponent.
        // Ideally, I should inline them to be 100% designer standard, but this works too.
        private void ConfigureSidebarButton(Button btn, string text)
        {
            btn.Text = text;
            btn.Size = new System.Drawing.Size(300, 50);
            btn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.BackColor = System.Drawing.Color.White;
            btn.Font = new System.Drawing.Font("Segoe UI", 11F);
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void WireEvents()
        {
            this.btnLogout.Click += (s, e) => this.Close();
            this.btnManageBoarders.Click += (s, e) => OpenForm(new FormBoarders(_currentUser));
            this.btnManageRooms.Click += (s, e) => OpenForm(new FormRooms(_currentUser));
            this.btnManageUsers.Click += (s, e) => OpenForm(new FormUsers(_currentUser));
            // this.btnReports.Click += (s, e) => OpenForm(new FormReports());

            this.btnSettings.Click += (s, e) => OpenForm(new FormConnection());
            this.btnMyDetails.Click += (s, e) => OpenForm(new FormBoarderDetails(_currentUser));
            this.btnMyProperties.Click += (s, e) => OpenMyProperties();
            this.btnBoardingHouses.Click += (s, e) => FocusBoardingHouseSearch();
            this.txtSearch.TextChanged += (s, e) => OnSearchTextChanged();
            if (this.txtBoardingHouseSearch != null)
                this.txtBoardingHouseSearch.TextChanged += (s, e) => OnBoardingHouseSearchChanged();
            this.dgvSearchResults.SelectionChanged += (s, e) => OnSearchSelectionChanged();
            this.dgvSearchResults.CellDoubleClick += OnSearchCellDoubleClick;
            this.btnUploadImages.Click += (s, e) => UploadImagesForProperty();
            this.pbImage1.Click += (s, e) => SelectImageSlot(1);
            this.pbImage2.Click += (s, e) => SelectImageSlot(2);
            this.pbImage3.Click += (s, e) => SelectImageSlot(3);
        }

        private void InitializeSearchDebounce()
        {
            _searchDebounce?.Dispose();
            _searchDebounce = new System.Windows.Forms.Timer();
            _searchDebounce.Interval = 250;
            _searchDebounce.Tick += (s, e) =>
            {
                if (_searchDebounce != null) _searchDebounce.Stop();
                PerformSearch(_pendingSearchText);
            };
        }

        private void OnBoardingHouseSearchChanged()
        {
            if (txtBoardingHouseSearch == null || _boardingHouseRepo == null) return;

            string keyword = txtBoardingHouseSearch.Text.Trim();
            try
            {
                var houses = string.IsNullOrEmpty(keyword)
                    ? _boardingHouseRepo.GetAll()
                    : _boardingHouseRepo.Search(keyword);

                ShowBoardingHouseResults(houses);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching boarding houses: " + ex.Message);
            }
        }

        private void OnSearchTextChanged()
        {
            if (txtSearch == null) return;
            _pendingSearchText = txtSearch.Text.Trim();
            if (_searchDebounce == null)
            {
                PerformSearch(_pendingSearchText);
                return;
            }
            _searchDebounce.Stop();
            _searchDebounce.Start();
        }

        private void LoadProfilePicture()
        {
            if (_currentUser == null) return;

            try
            {
                // Default fallback
                if (pbProfile != null) pbProfile.BackColor = Color.LightGray;

                // Specific logic for 'keith'
                if (string.Equals(_currentUser.Username, "keith", StringComparison.OrdinalIgnoreCase))
                {
                    string keithImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keith.jpg");
                    if (System.IO.File.Exists(keithImage) && pbProfile != null)
                    {
                        pbProfile.ImageLocation = keithImage;
                    }
                }
                // Specific logic for 'rodz'
                else if (string.Equals(_currentUser.Username, "rodz", StringComparison.OrdinalIgnoreCase))
                {
                    string rodzImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rodz.jpg");
                    if (System.IO.File.Exists(rodzImage) && pbProfile != null)
                    {
                        pbProfile.ImageLocation = rodzImage;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fail silently for profile picture
                Console.WriteLine("Error loading profile picture: " + ex.Message);
            }
        }

        private void SetupRoleBasedAccess()
        {
            if (_currentUser == null) return;

            // Default: Hide all
            if (this.btnMyProperties != null) this.btnMyProperties.Visible = false;
            if (this.btnManageBoarders != null) this.btnManageBoarders.Visible = false;
            if (this.btnManageRooms != null) this.btnManageRooms.Visible = false;
            if (this.btnManageUsers != null) this.btnManageUsers.Visible = false;
            if (this.btnSettings != null) this.btnSettings.Visible = false;
            if (this.btnMyDetails != null) this.btnMyDetails.Visible = false;
            if (this.btnBoardingHouses != null) this.btnBoardingHouses.Visible = false;

            if (_currentUser.Role == "SuperAdmin")
            {
                if (this.btnMyProperties != null) this.btnMyProperties.Visible = true;
                if (this.btnManageUsers != null) this.btnManageUsers.Visible = true;
                if (this.btnManageBoarders != null) this.btnManageBoarders.Visible = true;
                if (this.btnManageRooms != null) this.btnManageRooms.Visible = true;
                // if (this.btnReports != null) this.btnReports.Visible = true;

                if (this.btnSettings != null) this.btnSettings.Visible = true;
                if (this.btnBoardingHouses != null) this.btnBoardingHouses.Visible = true;
                if (this.lblStats != null) this.lblStats.Text = "Super Admin Dashboard\n\nOverview:\n- Manage Users\n- Full System Access";
            }
            else if (_currentUser.Role == "Admin")
            {
                if (this.btnMyProperties != null) this.btnMyProperties.Visible = true;
                if (this.btnManageBoarders != null) this.btnManageBoarders.Visible = true;
                if (this.btnManageRooms != null) this.btnManageRooms.Visible = true;
                if (this.btnSettings != null) this.btnSettings.Visible = true;
                if (this.btnBoardingHouses != null) this.btnBoardingHouses.Visible = true;
                if (this.lblStats != null) this.lblStats.Text = "Admin Dashboard\n\nOverview:\n- Manage Boarders\n- Manage Rooms\n- Record Payments";
            }
            else if (_currentUser.Role == "Boarder")
            {
                if (this.btnMyDetails != null) this.btnMyDetails.Visible = true;
                if (this.btnBoardingHouses != null) this.btnBoardingHouses.Visible = true;
                if (this.lblStats != null) this.lblStats.Text = "Boarder Dashboard\n\n- View Rent\n- View Payments\n- Browse Boarding Houses";
            }
            UpdateImageActionVisibility();
        }

        private void OpenForm(Form form)
        {
            form.ShowDialog(); // Show as modal
        }

        private void OpenMyProperties()
        {
            var form = new FormOwnerProperties(_currentUser);
            form.PropertySelected += (bh) =>
            {
                _currentBoardingHouse = bh;
                if (lblStats != null)
                    lblStats.Text = $"Managed Property:\n{bh.Name}\n\nOverview:\n- Manage Boarders\n- Manage Rooms\n- Record Payments";
                // In future: Refresh repositories or grids to filter by bh.Id
            };
            form.ShowDialog();
        }

        private void UpdateRoomPreview()
        {
            try
            {
                if (dgvSearchResults != null && dgvSearchResults.SelectedRows.Count > 0)
                {
                    var row = dgvSearchResults.SelectedRows[0];
                    if (dgvSearchResults.Columns.Contains("RoomNumber") && row.Cells["RoomNumber"].Value != null)
                    {
                        string roomNum = row.Cells["RoomNumber"].Value.ToString() ?? "";
                        string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"room_{roomNum}.jpg");

                        if (System.IO.File.Exists(imagePath) && pbRoomPreview != null)
                        {
                            pbRoomPreview.ImageLocation = imagePath;
                        }
                        else
                        {
                            // Fallback to generic placeholder
                            string defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "room_placeholder.jpg");
                            if (System.IO.File.Exists(defaultPath) && pbRoomPreview != null)
                                pbRoomPreview.ImageLocation = defaultPath;
                            else if (pbRoomPreview != null)
                                pbRoomPreview.BackColor = Color.LightSkyBlue;
                        }

                        if (lblRoomPreview != null) lblRoomPreview.Visible = true;
                        if (pbRoomPreview != null) pbRoomPreview.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating room preview: " + ex.Message);
            }
        }

        private void FocusBoardingHouseSearch()
        {
            if (_boardingHouseRepo == null) return;

            if (txtSearch != null)
            {
                txtSearch.Text = "";
            }

            if (txtBoardingHouseSearch != null)
            {
                txtBoardingHouseSearch.Visible = true;
                txtBoardingHouseSearch.Text = "";
                txtBoardingHouseSearch.Focus();
            }

            if (lblListTitle != null)
            {
                lblListTitle.Text = "Boarding Houses";
            }

            if (lblDetailsTitle != null)
            {
                lblDetailsTitle.Text = "Boarding House Details";
            }

            if (lblDetailsHint != null)
            {
                lblDetailsHint.Text = "Select a boarding house to view details.";
                lblDetailsHint.Visible = true;
            }

            ClearBoardingHouseDetails();

            try
            {
                var houses = _boardingHouseRepo.GetAll();
                if (houses.Count > 0)
                {
                    ShowBoardingHouseResults(houses);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading boarding houses: " + ex.Message);
            }
        }

        private void OnSearchSelectionChanged()
        {
            if (_isUpdatingResults) return;
            if (dgvSearchResults == null || dgvSearchResults.SelectedRows.Count == 0)
            {
                ClearBoardingHouseDetails();
                return;
            }

            var row = dgvSearchResults.SelectedRows[0];
            if (TryPopulateBoardingHouseDetailsFromRow(row))
            {
                if (pbRoomPreview != null) pbRoomPreview.Visible = false;
                if (lblRoomPreview != null) lblRoomPreview.Visible = false;
                return;
            }

            ClearBoardingHouseDetails();
            UpdateRoomPreview();
        }

        private void OnSearchCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearchResults == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgvSearchResults.Rows.Count) return;

            var row = dgvSearchResults.Rows[e.RowIndex];
            if (dgvSearchResults.DataSource is System.Data.DataTable)
            {
                OpenBoarderDetailsFromRow(row);
            }
            else
            {
                if (TryPopulateBoardingHouseDetailsFromRow(row))
                {
                    if (pbRoomPreview != null) pbRoomPreview.Visible = false;
                    if (lblRoomPreview != null) lblRoomPreview.Visible = false;
                }
            }
        }

        private bool TryPopulateBoardingHouseDetailsFromRow(DataGridViewRow row)
        {
            if (row.DataBoundItem is BoardingHouse boundHouse)
            {
                PopulateBoardingHouseDetails(boundHouse);
                return true;
            }

            if (row.DataGridView == null) return false;
            var columns = row.DataGridView.Columns;
            if (columns == null) return false;
            if (!columns.Contains("Id") || !columns.Contains("Name")) return false;

            var bh = new BoardingHouse
            {
                Id = row.Cells["Id"].Value != null ? Convert.ToInt32(row.Cells["Id"].Value) : 0,
                Name = row.Cells["Name"].Value?.ToString() ?? string.Empty,
                Address = columns.Contains("Address") ? row.Cells["Address"].Value?.ToString() ?? string.Empty : string.Empty,
                Description = columns.Contains("Description") ? row.Cells["Description"].Value?.ToString() : null,
                Rules = columns.Contains("Rules") ? row.Cells["Rules"].Value?.ToString() : null,
                Amenities = columns.Contains("Amenities") ? row.Cells["Amenities"].Value?.ToString() : null,
                OwnerName = columns.Contains("OwnerName") ? row.Cells["OwnerName"].Value?.ToString() ?? string.Empty : string.Empty,
                ImagePath1 = columns.Contains("ImagePath1") ? row.Cells["ImagePath1"].Value?.ToString() : null,
                ImagePath2 = columns.Contains("ImagePath2") ? row.Cells["ImagePath2"].Value?.ToString() : null,
                ImagePath3 = columns.Contains("ImagePath3") ? row.Cells["ImagePath3"].Value?.ToString() : null
            };

            PopulateBoardingHouseDetails(bh);
            return true;
        }

        private void OpenBoarderDetailsFromRow(DataGridViewRow row)
        {
            try
            {
                if (row.DataGridView == null) return;
                var columns = row.DataGridView.Columns;
                if (columns == null) return;
                if (!columns.Contains("UserId")) return;

                object? userIdObj = row.Cells["UserId"].Value;
                if (userIdObj == null || userIdObj == DBNull.Value) return;

                int userId = Convert.ToInt32(userIdObj);
                string username = columns.Contains("Username") ? row.Cells["Username"].Value?.ToString() ?? string.Empty : string.Empty;

                var user = new User
                {
                    Id = userId,
                    Username = username,
                    Role = "Boarder"
                };

                var form = new FormBoarderDetails(user);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening boarder details: " + ex.Message);
            }
        }

        private void PopulateBoardingHouseDetails(BoardingHouse bh)
        {
            _selectedHouseDetails = bh;
            if (splitResults != null) splitResults.Panel2Collapsed = false;
            if (panelDetails != null) panelDetails.Visible = true;
            if (txtDetailName != null) txtDetailName.Text = string.IsNullOrWhiteSpace(bh.Name) ? string.Empty : bh.Name;
            if (txtDetailAddress != null) txtDetailAddress.Text = string.IsNullOrWhiteSpace(bh.Address) ? string.Empty : bh.Address;
            if (txtDetailDescription != null) txtDetailDescription.Text = string.IsNullOrWhiteSpace(bh.Description) ? string.Empty : bh.Description;
            if (txtDetailRules != null) txtDetailRules.Text = string.IsNullOrWhiteSpace(bh.Rules) ? string.Empty : bh.Rules;
            if (txtDetailAmenities != null) txtDetailAmenities.Text = string.IsNullOrWhiteSpace(bh.Amenities) ? string.Empty : bh.Amenities;
            if (txtDetailOwner != null) txtDetailOwner.Text = string.IsNullOrWhiteSpace(bh.OwnerName) ? string.Empty : bh.OwnerName;
            if (lblDetailsHint != null) lblDetailsHint.Visible = false;
            
            if (lblDetailSummary != null)
            {
                lblDetailSummary.Text = $"Name: {bh.Name}\n" +
                                      $"Owner: {bh.OwnerName ?? "N/A"}\n" +
                                      $"Address: {bh.Address}\n" +
                                      $"Amenities: {(string.IsNullOrEmpty(bh.Amenities) ? "None" : bh.Amenities)}";
            }

            UpdateImageBoxes();
            UpdateImageActionVisibility();
        }

        private void ClearBoardingHouseDetails()
        {
            _selectedHouseDetails = null;
            if (splitResults != null) splitResults.Panel2Collapsed = true;
            if (panelDetails != null) panelDetails.Visible = false;
            if (txtDetailName != null) txtDetailName.Text = string.Empty;
            if (txtDetailAddress != null) txtDetailAddress.Text = string.Empty;
            if (txtDetailDescription != null) txtDetailDescription.Text = string.Empty;
            if (txtDetailRules != null) txtDetailRules.Text = string.Empty;
            if (txtDetailAmenities != null) txtDetailAmenities.Text = string.Empty;
            if (txtDetailOwner != null) txtDetailOwner.Text = string.Empty;
            if (lblDetailsHint != null) lblDetailsHint.Visible = true;
            if (lblDetailSummary != null) lblDetailSummary.Text = "Select a property to see details.";
            ClearImageBoxes();
            UpdateImageActionVisibility();
        }

        private void UpdateImageActionVisibility()
        {
            bool canManage = _currentUser != null && (_currentUser.Role == "Admin" || _currentUser.Role == "SuperAdmin");
            bool hasSelection = _selectedHouseDetails != null;

            if (pnlImageActions != null) pnlImageActions.Visible = canManage;
            if (pnlDetailActions != null) pnlDetailActions.Visible = true; // Always visible as it shows info
            if (btnUploadImages != null) btnUploadImages.Enabled = canManage && hasSelection;
            
            if (txtDetailName != null) txtDetailName.ReadOnly = !canManage;
            if (txtDetailAddress != null) txtDetailAddress.ReadOnly = !canManage;
            if (txtDetailDescription != null) txtDetailDescription.ReadOnly = !canManage;
            if (txtDetailRules != null) txtDetailRules.ReadOnly = !canManage;
            if (txtDetailAmenities != null) txtDetailAmenities.ReadOnly = !canManage;
        }

        private void RefreshSearchResults()
        {
            string keyword = txtSearch != null ? txtSearch.Text.Trim() : _pendingSearchText;
            PerformSearch(keyword);
        }

        private void UpdateImageBoxes()
        {
            if (_selectedHouseDetails == null)
            {
                ClearImageBoxes();
                return;
            }

            SetImageBox(pbImage1, _selectedHouseDetails.ImagePath1);
            SetImageBox(pbImage2, _selectedHouseDetails.ImagePath2);
            SetImageBox(pbImage3, _selectedHouseDetails.ImagePath3);
            HighlightSelectedImageSlot();
        }

        private void ClearImageBoxes()
        {
            if (pbImage1 != null) ReplaceImage(pbImage1, null);
            if (pbImage2 != null) ReplaceImage(pbImage2, null);
            if (pbImage3 != null) ReplaceImage(pbImage3, null);
        }

        private void SetImageBox(PictureBox box, string? relativePath)
        {
            if (box == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                ReplaceImage(box, null);
                return;
            }

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            if (File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var temp = Image.FromStream(stream))
                    {
                        ReplaceImage(box, new Bitmap(temp));
                    }
                }
                catch
                {
                    ReplaceImage(box, null);
                }
            }
            else
            {
                ReplaceImage(box, null);
            }
        }

        private void ReplaceImage(PictureBox box, Image? nextImage)
        {
            if (box == null) return;
            var oldImage = box.Image;
            box.Image = nextImage;
            if (oldImage != null)
            {
                oldImage.Dispose();
            }
        }

        private void AddOrUpdateImage(int slot)
        {
            if (_selectedHouseDetails == null) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Boarding House Picture";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string? savedPath = SaveBoardingHouseImage(ofd.FileName);
                    if (string.IsNullOrEmpty(savedPath)) return;

                    switch (slot)
                    {
                        case 1:
                            _selectedHouseDetails.ImagePath1 = savedPath;
                            break;
                        case 2:
                            _selectedHouseDetails.ImagePath2 = savedPath;
                            break;
                        case 3:
                            _selectedHouseDetails.ImagePath3 = savedPath;
                            break;
                    }

                    _boardingHouseRepo.UpdateImages(_selectedHouseDetails.Id, _selectedHouseDetails.ImagePath1, _selectedHouseDetails.ImagePath2, _selectedHouseDetails.ImagePath3);
                    UpdateImageBoxes();
                }
            }
        }

        private string? SaveBoardingHouseImage(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath)) return null;

            try
            {
                string folderPath = Path.Combine(Application.StartupPath, "BoardingHousePictures");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                string destPath = Path.Combine(folderPath, fileName);

                File.Copy(sourcePath, destPath, true);

                return "BoardingHousePictures/" + fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving image: " + ex.Message);
                return null;
            }
        }

        private void SelectImageSlot(int slot)
        {
            if (slot < 1 || slot > 3) return;
            _selectedImageSlot = slot;
            HighlightSelectedImageSlot();
        }

        private void HighlightSelectedImageSlot()
        {
            if (pbImage1 != null) pbImage1.BorderStyle = _selectedImageSlot == 1 ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
            if (pbImage2 != null) pbImage2.BorderStyle = _selectedImageSlot == 2 ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
            if (pbImage3 != null) pbImage3.BorderStyle = _selectedImageSlot == 3 ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
        }

        private void UploadImagesForProperty()
        {
            if (_selectedHouseDetails == null)
            {
                MessageBox.Show("Please select a property first.");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Pictures";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var files = ofd.FileNames;
                    string? img1 = _selectedHouseDetails.ImagePath1;
                    string? img2 = _selectedHouseDetails.ImagePath2;
                    string? img3 = _selectedHouseDetails.ImagePath3;

                    int fileIndex = 0;
                    for (int slot = _selectedImageSlot; slot <= 3 && fileIndex < files.Length; slot++)
                    {
                        string? savedPath = SaveBoardingHouseImage(files[fileIndex]);
                        if (string.IsNullOrEmpty(savedPath))
                        {
                            fileIndex++;
                            continue;
                        }

                        switch (slot)
                        {
                            case 1:
                                img1 = savedPath;
                                break;
                            case 2:
                                img2 = savedPath;
                                break;
                            case 3:
                                img3 = savedPath;
                                break;
                        }

                        fileIndex++;
                    }

                    _selectedHouseDetails.ImagePath1 = img1;
                    _selectedHouseDetails.ImagePath2 = img2;
                    _selectedHouseDetails.ImagePath3 = img3;

                    _boardingHouseRepo.UpdateImages(
                        _selectedHouseDetails.Id,
                        _selectedHouseDetails.ImagePath1,
                        _selectedHouseDetails.ImagePath2,
                        _selectedHouseDetails.ImagePath3);

                    UpdateImageBoxes();
                }
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch != null ? txtSearch.Text.Trim() : "";
            PerformSearch(keyword);
        }

        private void PerformSearch(string keyword)
        {
            if (_isUpdatingResults) return;

            if (string.IsNullOrEmpty(keyword))
            {
                if (dgvSearchResults != null) dgvSearchResults.Visible = false;
                if (pbRoomPreview != null) pbRoomPreview.Visible = false;
                if (lblRoomPreview != null) lblRoomPreview.Visible = false;
                if (lblStats != null) lblStats.Visible = false;
                if (pnlDashboardStats != null) pnlDashboardStats.Visible = true;
                ClearBoardingHouseDetails();
                return;
            }

            try
            {
                bool searched = false;

                if (_currentUser != null && (_currentUser.Role == "SuperAdmin" || _currentUser.Role == "Admin"))
                {
                    var boarderResults = _boarderRepo.SearchBoarders(keyword);
                    if (boarderResults != null && boarderResults.Rows.Count > 0)
                    {
                        ShowBoarderResults(boarderResults);
                        ClearBoardingHouseDetails();
                        searched = true;
                    }
                }
                else
                {
                    var houseResults = _boardingHouseRepo.Search(keyword);
                    if (houseResults.Count > 0)
                    {
                        ShowBoardingHouseResults(houseResults);
                        ClearBoardingHouseDetails();
                        searched = true;
                    }
                }

                if (!searched)
                {
                    if (dgvSearchResults != null) dgvSearchResults.Visible = false;
                    if (pbRoomPreview != null) pbRoomPreview.Visible = false;
                    if (lblRoomPreview != null) lblRoomPreview.Visible = false;
                    if (lblStats != null) lblStats.Visible = false;
                    if (pnlDashboardStats != null) pnlDashboardStats.Visible = true;
                    ClearBoardingHouseDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message);
            }
        }

        private void ShowBoarderResults(System.Data.DataTable results)
        {
            if (dgvSearchResults != null)
            {
                _isUpdatingResults = true;
                try
                {
                    dgvSearchResults.DataSource = results;

                    if (dgvSearchResults.Columns["Id"] != null) dgvSearchResults.Columns["Id"].Visible = false;
                    if (dgvSearchResults.Columns["RoomId"] != null) dgvSearchResults.Columns["RoomId"].Visible = false;
                    if (dgvSearchResults.Columns["UserId"] != null) dgvSearchResults.Columns["UserId"].Visible = false;

                    if (dgvSearchResults.Columns["BoardingHouseName"] != null)
                        dgvSearchResults.Columns["BoardingHouseName"].HeaderText = "Boarding House";

                    dgvSearchResults.Visible = true;
                    if (dgvSearchResults.Rows.Count > 0)
                    {
                        dgvSearchResults.ClearSelection();
                        dgvSearchResults.CurrentCell = null;
                    }
                }
                finally
                {
                    _isUpdatingResults = false;
                }
            }
            if (pnlDashboardStats != null) pnlDashboardStats.Visible = false;

        }

        private void ShowBoardingHouseResults(System.Collections.Generic.List<BoardingHouse> results)
        {
            if (dgvSearchResults != null)
            {
                _isUpdatingResults = true;
                try
                {
                    dgvSearchResults.DataSource = results;

                    if (dgvSearchResults.Columns["OwnerId"] != null) dgvSearchResults.Columns["OwnerId"].Visible = false;
                    if (dgvSearchResults.Columns["IsActive"] != null) dgvSearchResults.Columns["IsActive"].Visible = false;
                    if (dgvSearchResults.Columns["CreatedAt"] != null) dgvSearchResults.Columns["CreatedAt"].Visible = false;

                    dgvSearchResults.Visible = true;
                    if (dgvSearchResults.Rows.Count > 0)
                    {
                        dgvSearchResults.ClearSelection();
                        dgvSearchResults.CurrentCell = null;
                    }
                }
                finally
                {
                    _isUpdatingResults = false;
                }
            }
            if (pnlDashboardStats != null) pnlDashboardStats.Visible = false;

            if (pbRoomPreview != null) pbRoomPreview.Visible = false;
            if (lblRoomPreview != null) lblRoomPreview.Visible = false;
        }
        private void InitializeDashboardControls()
        {
            if (pnlDashboardStats == null) return;
            pnlDashboardStats.Controls.Clear();
            pnlDashboardStats.Padding = new Padding(20);
            pnlDashboardStats.AutoScroll = true;

            // 1. Title
            Label lblTitle = new Label();
            lblTitle.Text = "Dashboard Overview";
            lblTitle.Font = UITheme.TitleFont;
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            lblTitle.Margin = new Padding(0, 0, 0, 20);
            pnlDashboardStats.Controls.Add(lblTitle);
            pnlDashboardStats.SetFlowBreak(lblTitle, true);

            // 2. Metrics Cards Row
            FlowLayoutPanel pnlCards = new FlowLayoutPanel();
            pnlCards.AutoSize = true;
            pnlCards.FlowDirection = FlowDirection.LeftToRight;
            pnlCards.WrapContents = true;
            pnlCards.Margin = new Padding(0, 0, 0, 20);
            
            AddDashboardCard(pnlCards, "Total Boarders", "0", UITheme.InfoColor, ref lblTotalBoarders);
            AddDashboardCard(pnlCards, "Monthly Income", "$0.00", UITheme.SuccessColor, ref lblTotalIncome);
            AddDashboardCard(pnlCards, "Occupancy Rate", "0%", UITheme.WarningColor, ref lblOccupancy);
            AddDashboardCard(pnlCards, "Pending Payments", "0", UITheme.DangerColor, ref lblPendingPayments);

            pnlDashboardStats.Controls.Add(pnlCards);
            pnlDashboardStats.SetFlowBreak(pnlCards, true);

            // 3. Charts Row
            FlowLayoutPanel pnlCharts = new FlowLayoutPanel();
            pnlCharts.AutoSize = true;
            pnlCharts.FlowDirection = FlowDirection.LeftToRight;
            pnlCharts.WrapContents = true;
            pnlCharts.Margin = new Padding(0, 0, 0, 20);

            // Income Trend Chart
            lineChartIncome = new BoardingHouseSys.UI.LineChartControl();
            lineChartIncome.Title = "Income Trend (Last 6 Months)";
            lineChartIncome.Size = new Size(500, 300);
            lineChartIncome.BackColor = Color.White;
            lineChartIncome.Margin = new Padding(0, 0, 20, 20);
            pnlCharts.Controls.Add(lineChartIncome);

            // Occupancy Pie Chart
            pieChartOccupancy = new BoardingHouseSys.UI.PieChartControl();
            pieChartOccupancy.Size = new Size(360, 260);
            pieChartOccupancy.BackColor = Color.White;
            pieChartOccupancy.Margin = new Padding(0, 0, 0, 20);
            pnlCharts.Controls.Add(pieChartOccupancy);

            pnlDashboardStats.Controls.Add(pnlCharts);
            pnlDashboardStats.SetFlowBreak(pnlCharts, true);

            // 4. Revenue Breakdown Grid
            Label lblGridTitle = new Label();
            lblGridTitle.Text = "Revenue by Boarding House";
            lblGridTitle.Font = UITheme.HeaderFont;
            lblGridTitle.AutoSize = true;
            lblGridTitle.Margin = new Padding(0, 0, 0, 10);
            pnlDashboardStats.Controls.Add(lblGridTitle);
            pnlDashboardStats.SetFlowBreak(lblGridTitle, true);

            dgvRevenueBreakdown = new DataGridView();
            UITheme.ApplyDataGridViewStyle(dgvRevenueBreakdown);
            dgvRevenueBreakdown.Size = new Size(850, 250);
            pnlDashboardStats.Controls.Add(dgvRevenueBreakdown);
        }

        private void UpdateDashboardLayoutSizes()
        {
            if (pnlDashboardStats == null) return;

            int availableWidth = pnlDashboardStats.ClientSize.Width - pnlDashboardStats.Padding.Horizontal;
            if (availableWidth <= 0) return;

            if (lineChartIncome != null)
            {
                int chartWidth = Math.Max(300, (availableWidth - 20) / 2);
                int chartHeight = Math.Max(220, pnlDashboardStats.ClientSize.Height / 3);
                lineChartIncome.Size = new Size(chartWidth, chartHeight);
            }

            if (pieChartOccupancy != null)
            {
                int chartWidth = Math.Max(260, (availableWidth - 20) / 2);
                int chartHeight = lineChartIncome != null ? lineChartIncome.Height : Math.Max(220, pnlDashboardStats.ClientSize.Height / 3);
                pieChartOccupancy.Size = new Size(chartWidth, chartHeight);
            }

            if (dgvRevenueBreakdown != null)
            {
                dgvRevenueBreakdown.Width = availableWidth;
                dgvRevenueBreakdown.Height = Math.Max(200, pnlDashboardStats.ClientSize.Height / 3);
            }
        }

        private void AddDashboardCard(Control parent, string title, string value, Color color, ref Label lblValue)
        {
            Panel card = new Panel();
            card.Size = new Size(200, 100);
            card.BackColor = Color.White;
            card.Margin = new Padding(0, 0, 20, 20);
            // Add a colored left border
            Panel border = new Panel();
            border.Dock = DockStyle.Left;
            border.Width = 5;
            border.BackColor = color;
            card.Controls.Add(border);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.Location = new Point(15, 10);
            lblTitle.AutoSize = true;
            card.Controls.Add(lblTitle);

            lblValue = new Label();
            lblValue.Text = value;
            lblValue.ForeColor = Color.Black; // Dark text on white card
            lblValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblValue.Location = new Point(15, 35);
            lblValue.AutoSize = true;
            card.Controls.Add(lblValue);
            
            // Subtle border for card
            card.BorderStyle = BorderStyle.FixedSingle;

            parent.Controls.Add(card);
        }

        private void StartStatsTimer()
        {
            _statsTimer = new System.Windows.Forms.Timer();
            _statsTimer.Interval = 30000; // 30 seconds
            _statsTimer.Tick += (s, e) => LoadDashboardStats();
            _statsTimer.Start();
            
            // Initial load
            LoadDashboardStats();
        }

        private void LoadDashboardStats()
        {
            try
            {
                if (_dbHelper == null) return;
                var now = DateTime.Now;
                string currentMonth = now.ToString("MMMM");
                int currentYear = now.Year;
                bool isSuperAdmin = _currentUser != null && _currentUser.Role == "SuperAdmin";
                bool isAdmin = _currentUser != null && _currentUser.Role == "Admin";
                int ownerId = _currentUser != null ? _currentUser.Id : 0;

                long totalBoarders;
                decimal income;
                long totalCapacity;
                long pendingCount;

                if (isAdmin && ownerId > 0)
                {
                    string sqlBoardersOwner = $@"
                        SELECT COUNT(*) 
                        FROM Boarders b
                        INNER JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                        WHERE b.IsActive = 1 AND bh.OwnerId = {ownerId}";
                    totalBoarders = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlBoardersOwner));

                    string sqlIncomeOwner = $@"
                        SELECT COALESCE(SUM(p.Amount), 0) 
                        FROM Payments p
                        INNER JOIN Boarders b ON p.BoarderId = b.Id
                        INNER JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                        WHERE p.MonthPaid = '{currentMonth}' 
                          AND p.YearPaid = {currentYear}
                          AND bh.OwnerId = {ownerId}";
                    income = Convert.ToDecimal(_dbHelper.ExecuteScalar(sqlIncomeOwner));

                    string sqlCapacityOwner = $@"
                        SELECT COALESCE(SUM(r.Capacity), 0) 
                        FROM Rooms r
                        INNER JOIN BoardingHouses bh ON r.BoardingHouseId = bh.Id
                        WHERE r.IsActive = 1 AND bh.OwnerId = {ownerId}";
                    totalCapacity = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlCapacityOwner));

                    string sqlPendingOwner = $@"
                        SELECT COUNT(*) 
                        FROM Payments p
                        INNER JOIN Boarders b ON p.BoarderId = b.Id
                        INNER JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                        WHERE p.Status = 'Pending' AND bh.OwnerId = {ownerId}";
                    pendingCount = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlPendingOwner));
                }
                else
                {
                    string sqlBoarders = "SELECT COUNT(*) FROM Boarders WHERE IsActive = 1";
                    totalBoarders = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlBoarders));

                    string sqlIncome = $"SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE MonthPaid = '{currentMonth}' AND YearPaid = {currentYear}";
                    income = Convert.ToDecimal(_dbHelper.ExecuteScalar(sqlIncome));

                    string sqlCapacity = "SELECT COALESCE(SUM(Capacity), 0) FROM Rooms WHERE IsActive = 1";
                    totalCapacity = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlCapacity));

                    string sqlPending = "SELECT COUNT(*) FROM Payments WHERE Status = 'Pending'";
                    pendingCount = Convert.ToInt64(_dbHelper.ExecuteScalar(sqlPending));
                }

                if (lblTotalBoarders != null) lblTotalBoarders.Text = totalBoarders.ToString();

                if (lblTotalIncome != null) lblTotalIncome.Text = income.ToString("C");

                if (lblOccupancy != null)
                {
                    if (totalCapacity > 0)
                    {
                        double occ = (double)totalBoarders / totalCapacity * 100;
                        lblOccupancy.Text = $"{occ:F1}%";
                    }
                    else
                    {
                        lblOccupancy.Text = "0%";
                    }
                }

                if (lblPendingPayments != null) lblPendingPayments.Text = pendingCount.ToString();

                if (pieChartOccupancy != null)
                {
                    pieChartOccupancy.Data.Clear();
                    pieChartOccupancy.Data["Occupied"] = totalBoarders;
                    pieChartOccupancy.Data["Vacant"] = (totalCapacity > totalBoarders) ? (totalCapacity - totalBoarders) : 0;
                    pieChartOccupancy.Invalidate();
                }

                // 6. Update Line Chart (Income Trend - Last 6 Months)
                if (lineChartIncome != null)
                {
                    lineChartIncome.Values.Clear();
                    lineChartIncome.Labels.Clear();
                    
                    for (int i = 5; i >= 0; i--)
                    {
                        DateTime d = now.AddMonths(-i);
                        string mName = d.ToString("MMMM");
                        int y = d.Year;

                        decimal val;
                        if (isAdmin && ownerId > 0)
                        {
                            string sqlTrendOwner = $@"
                                SELECT COALESCE(SUM(p.Amount), 0) 
                                FROM Payments p
                                INNER JOIN Boarders b ON p.BoarderId = b.Id
                                INNER JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                                WHERE p.MonthPaid = '{mName}' 
                                  AND p.YearPaid = {y}
                                  AND bh.OwnerId = {ownerId}";
                            val = Convert.ToDecimal(_dbHelper.ExecuteScalar(sqlTrendOwner));
                        }
                        else
                        {
                            string sqlTrend = $"SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE MonthPaid = '{mName}' AND YearPaid = {y}";
                            val = Convert.ToDecimal(_dbHelper.ExecuteScalar(sqlTrend));
                        }
                        
                        lineChartIncome.Values.Add((double)val);
                        lineChartIncome.Labels.Add(d.ToString("MMM"));
                    }
                    lineChartIncome.Invalidate();
                }

                string sqlBreakdown;
                if (isAdmin && ownerId > 0)
                {
                    sqlBreakdown = $@"
                        SELECT 
                            bh.Name as 'Boarding House',
                            COUNT(b.Id) as 'Total Boarders',
                            COALESCE(SUM(p.Amount), 0) as 'Revenue (This Month)'
                        FROM BoardingHouses bh
                        LEFT JOIN Boarders b ON b.BoardingHouseId = bh.Id AND b.IsActive = 1
                        LEFT JOIN Payments p ON p.BoarderId = b.Id 
                            AND p.MonthPaid = '{currentMonth}' 
                            AND p.YearPaid = {currentYear}
                        WHERE bh.IsActive = 1
                          AND bh.OwnerId = {ownerId}
                        GROUP BY bh.Id, bh.Name";
                }
                else
                {
                    sqlBreakdown = $@"
                        SELECT 
                            bh.Name as 'Boarding House',
                            COUNT(b.Id) as 'Total Boarders',
                            COALESCE(SUM(p.Amount), 0) as 'Revenue (This Month)'
                        FROM BoardingHouses bh
                        LEFT JOIN Boarders b ON b.BoardingHouseId = bh.Id AND b.IsActive = 1
                        LEFT JOIN Payments p ON p.BoarderId = b.Id 
                            AND p.MonthPaid = '{currentMonth}' 
                            AND p.YearPaid = {currentYear}
                        WHERE bh.IsActive = 1
                        GROUP BY bh.Id, bh.Name";
                }
                
                DataTable dt = _dbHelper.ExecuteQuery(sqlBreakdown);
                if (dgvRevenueBreakdown != null) dgvRevenueBreakdown.DataSource = dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading dashboard stats: " + ex.Message);
            }
        }
    }
}

