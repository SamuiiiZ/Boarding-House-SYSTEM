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
using System.IO;

namespace BoardingHouseSys.Forms
{
    public class FormOwnerProperties : Form
    {
        private User _currentUser;
        private BoardingHouseRepository _repository;
        private BoardingHouse? _selectedHouse;
        private bool _isUpdatingResults = false;
        private string _pendingSearchText = "";
        private bool _browseAll = false;

        // UI Controls - Exposed for Designer
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnBackTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.DataGridView dgvProperties;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Timer searchTimer;

        // Input Fields and Labels
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblRules;
        private System.Windows.Forms.TextBox txtRules;
        private System.Windows.Forms.Label lblAmenities;
        private System.Windows.Forms.TextBox txtAmenities;
        private System.Windows.Forms.Label lblPictures;
        private System.Windows.Forms.FlowLayoutPanel pnlImages;
        private System.Windows.Forms.PictureBox pbImage1;
        private System.Windows.Forms.PictureBox pbImage2;
        private System.Windows.Forms.PictureBox pbImage3;
        private System.Windows.Forms.Label lblPicturesHint;
        private System.Windows.Forms.FlowLayoutPanel pnlImageActions;
        private System.Windows.Forms.Button btnUploadImages;
        private System.Windows.Forms.Label lblDetailSummaryTitle;
        private System.Windows.Forms.Label lblDetailSummary;

        public event Action<BoardingHouse>? PropertySelected;

        public FormOwnerProperties()
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            WireEvents();
            _currentUser = new User();
            _repository = new BoardingHouseRepository();
        }

        public FormOwnerProperties(User user)
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            WireEvents();
            _currentUser = user;
            _repository = new BoardingHouseRepository();
            LoadProperties();
            ApplyTheme();
        }

        public FormOwnerProperties(User user, bool browseAll)
        {
            InitializeComponent();
            UITheme.ApplyFormStyle(this);
            WireEvents();
            _currentUser = user;
            _repository = new BoardingHouseRepository();
            _browseAll = browseAll;
            LoadProperties();
            ApplyTheme();
            if (_browseAll) ApplyBrowseMode();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyHeaderStyle(pnlTop);
            UITheme.ApplyHeaderStyle(pnlListHeader);

            UITheme.ApplyPanelStyle(panelLeft, UITheme.LightColor);
            UITheme.ApplyPanelStyle(panelRight, Color.WhiteSmoke);

            UITheme.ApplyButtonStyle(btnBackTop);
            UITheme.ApplyButtonStyle(btnClose);
            UITheme.ApplyButtonStyle(btnAdd);
            UITheme.ApplyButtonStyle(btnEdit);
            UITheme.ApplyButtonStyle(btnDelete);
            UITheme.ApplyButtonStyle(btnSelect);

            UITheme.ApplyGroupBoxStyle(grpInput);

            UITheme.ApplyTextBoxStyle(txtSearch);
            UITheme.ApplyTextBoxStyle(txtName);
            UITheme.ApplyTextBoxStyle(txtAddress);
            UITheme.ApplyTextBoxStyle(txtDescription);
            UITheme.ApplyTextBoxStyle(txtRules);
            UITheme.ApplyTextBoxStyle(txtAmenities);

            if (dgvProperties != null) UITheme.ApplyDataGridViewStyle(dgvProperties);

            if (lblListTitle != null) UITheme.ApplyHeaderLabelStyle(lblListTitle);
            if (lblName != null) UITheme.ApplyLabelStyle(lblName, true);
            if (lblAddress != null) UITheme.ApplyLabelStyle(lblAddress, true);
            if (lblDescription != null) UITheme.ApplyLabelStyle(lblDescription, true);
            if (lblRules != null) UITheme.ApplyLabelStyle(lblRules, true);
            if (lblAmenities != null) UITheme.ApplyLabelStyle(lblAmenities, true);
            if (lblPictures != null) UITheme.ApplySubHeaderLabelStyle(lblPictures);
            if (lblDetailSummaryTitle != null) UITheme.ApplySubHeaderLabelStyle(lblDetailSummaryTitle);
            if (lblDetailSummary != null) lblDetailSummary.ForeColor = UITheme.DarkColor;
        }

        private void ApplyBrowseMode()
        {
            if (lblListTitle != null) lblListTitle.Text = "Available Boarding Houses";
            if (btnAdd != null) btnAdd.Visible = false;
            if (btnEdit != null) btnEdit.Visible = false;
            if (btnDelete != null) btnDelete.Visible = false;
            if (btnSelect != null) btnSelect.Visible = false;
            if (btnUploadImages != null) btnUploadImages.Visible = false;
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
            pnlBottom = new Panel();
            btnClose = new Button();
            splitContainer = new SplitContainer();
            panelLeft = new Panel();
            dgvProperties = new DataGridView();
            pnlListHeader = new Panel();
            txtSearch = new TextBox();
            lblListTitle = new Label();
            panelRight = new Panel();
            grpInput = new GroupBox();
            tableLayout = new TableLayoutPanel();
            txtName = new TextBox();
            lblAddress = new Label();
            txtAddress = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            lblRules = new Label();
            txtRules = new TextBox();
            lblAmenities = new Label();
            txtAmenities = new TextBox();
            lblName = new Label();
            lblPictures = new Label();
            pnlImages = new FlowLayoutPanel();
            lblPicturesHint = new Label();
            pnlImageActions = new FlowLayoutPanel();
            btnUploadImages = new Button();
            lblDetailSummaryTitle = new Label();
            lblDetailSummary = new Label();
            pnlButtons = new FlowLayoutPanel();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnSelect = new Button();
            searchTimer = new System.Windows.Forms.Timer(components);
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProperties).BeginInit();
            pnlListHeader.SuspendLayout();
            panelRight.SuspendLayout();
            grpInput.SuspendLayout();
            tableLayout.SuspendLayout();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(50, 50, 50);
            pnlTop.Controls.Add(btnBackTop);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(2);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(7, 6, 7, 6);
            pnlTop.Size = new Size(840, 30);
            pnlTop.TabIndex = 2;
            // 
            // btnBackTop
            // 
            btnBackTop.BackColor = Color.White;
            btnBackTop.Dock = DockStyle.Left;
            btnBackTop.FlatStyle = FlatStyle.Flat;
            btnBackTop.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBackTop.Location = new Point(7, 6);
            btnBackTop.Margin = new Padding(2);
            btnBackTop.Name = "btnBackTop";
            btnBackTop.Size = new Size(139, 18);
            btnBackTop.TabIndex = 0;
            btnBackTop.Text = "← Back to Dashboard";
            btnBackTop.TextAlign = ContentAlignment.MiddleLeft;
            btnBackTop.UseVisualStyleBackColor = false;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.WhiteSmoke;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 385);
            pnlBottom.Margin = new Padding(2);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(7, 6, 7, 6);
            pnlBottom.Size = new Size(840, 36);
            pnlBottom.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.LightSlateGray;
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(693, 6);
            btnClose.Margin = new Padding(2);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(140, 24);
            btnClose.TabIndex = 0;
            btnClose.Text = "Back to Dashboard";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.Location = new Point(0, 30);
            splitContainer.Margin = new Padding(2);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(panelLeft);
            splitContainer.Panel1MinSize = 320;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(panelRight);
            splitContainer.Panel2MinSize = 500;
            splitContainer.Size = new Size(840, 355);
            splitContainer.SplitterDistance = 320;
            splitContainer.SplitterWidth = 3;
            splitContainer.TabIndex = 0;
            // 
            // panelLeft
            // 
            panelLeft.Controls.Add(dgvProperties);
            panelLeft.Controls.Add(pnlListHeader);
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Margin = new Padding(2);
            panelLeft.Name = "panelLeft";
            panelLeft.Padding = new Padding(14, 12, 14, 12);
            panelLeft.Size = new Size(320, 355);
            panelLeft.TabIndex = 0;
            panelLeft.Paint += panelLeft_Paint;
            // 
            // dgvProperties
            // 
            dgvProperties.AllowUserToAddRows = false;
            dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProperties.BackgroundColor = Color.White;
            dgvProperties.ColumnHeadersHeight = 40;
            dgvProperties.Dock = DockStyle.Fill;
            dgvProperties.Location = new Point(14, 48);
            dgvProperties.Margin = new Padding(2);
            dgvProperties.MultiSelect = false;
            dgvProperties.Name = "dgvProperties";
            dgvProperties.ReadOnly = true;
            dgvProperties.RowHeadersVisible = false;
            dgvProperties.RowHeadersWidth = 62;
            dgvProperties.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProperties.Size = new Size(292, 295);
            dgvProperties.TabIndex = 0;
            // 
            // pnlListHeader
            // 
            pnlListHeader.Controls.Add(txtSearch);
            pnlListHeader.Controls.Add(lblListTitle);
            pnlListHeader.Dock = DockStyle.Top;
            pnlListHeader.Location = new Point(14, 12);
            pnlListHeader.Margin = new Padding(2);
            pnlListHeader.Name = "pnlListHeader";
            pnlListHeader.Size = new Size(292, 36);
            pnlListHeader.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(119, 9);
            txtSearch.Margin = new Padding(2);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search properties...";
            txtSearch.Size = new Size(148, 23);
            txtSearch.TabIndex = 1;
            // 
            // lblListTitle
            // 
            lblListTitle.AutoSize = true;
            lblListTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblListTitle.Location = new Point(0, 9);
            lblListTitle.Margin = new Padding(2, 0, 2, 0);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(106, 21);
            lblListTitle.TabIndex = 0;
            lblListTitle.Text = "Property List";
            lblListTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelRight
            // 
            panelRight.Controls.Add(grpInput);
            panelRight.Controls.Add(pnlButtons);
            panelRight.Dock = DockStyle.Fill;
            panelRight.Location = new Point(0, 0);
            panelRight.Margin = new Padding(2);
            panelRight.Name = "panelRight";
            panelRight.Padding = new Padding(14, 12, 14, 12);
            panelRight.Size = new Size(517, 355);
            panelRight.TabIndex = 0;
            // 
            // grpInput
            // 
            grpInput.Controls.Add(tableLayout);
            grpInput.Dock = DockStyle.Fill;
            grpInput.Location = new Point(14, 12);
            grpInput.Margin = new Padding(2);
            grpInput.Name = "grpInput";
            grpInput.Padding = new Padding(14, 12, 14, 12);
            grpInput.Size = new Size(489, 271);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "Property Details";
            // 
            // tableLayout
            // 
            tableLayout.ColumnCount = 1;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.Controls.Add(txtName, 0, 1);
            tableLayout.Controls.Add(lblAddress, 0, 2);
            tableLayout.Controls.Add(txtAddress, 0, 3);
            tableLayout.Controls.Add(lblDescription, 0, 4);
            tableLayout.Controls.Add(txtDescription, 0, 5);
            tableLayout.Controls.Add(lblRules, 0, 6);
            tableLayout.Controls.Add(txtRules, 0, 7);
            tableLayout.Controls.Add(lblAmenities, 0, 8);
            tableLayout.Controls.Add(txtAmenities, 0, 9);
            tableLayout.Controls.Add(lblName, 0, 0);
                tableLayout.Controls.Add(lblPictures, 0, 10);
            tableLayout.Controls.Add(pnlImages, 0, 11);
            tableLayout.Controls.Add(pnlImageActions, 0, 12);
            tableLayout.Controls.Add(lblDetailSummaryTitle, 0, 13);
            tableLayout.Controls.Add(lblDetailSummary, 0, 14);
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(14, 28);
            tableLayout.Margin = new Padding(2);
            tableLayout.Name = "tableLayout";
            tableLayout.RowCount = 15;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.AutoSize = true;
            tableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayout.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Fill;
            txtName.Font = new Font("Segoe UI", 11F);
            txtName.Location = new Point(2, 23);
            txtName.Margin = new Padding(2);
            txtName.Name = "txtName";
            txtName.Size = new Size(457, 27);
            txtName.TabIndex = 1;
            // 
            // lblAddress
            // 
            lblAddress.Dock = DockStyle.Fill;
            lblAddress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAddress.Location = new Point(0, 47);
            lblAddress.Margin = new Padding(0, 2, 0, 2);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new Size(461, 17);
            lblAddress.TabIndex = 2;
            lblAddress.Text = "Address:";
            lblAddress.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            txtAddress.Dock = DockStyle.Fill;
            txtAddress.Font = new Font("Segoe UI", 11F);
            txtAddress.Location = new Point(2, 68);
            txtAddress.Margin = new Padding(2);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(457, 27);
            txtAddress.TabIndex = 3;
            // 
            // lblDescription
            // 
            lblDescription.Dock = DockStyle.Fill;
            lblDescription.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDescription.Location = new Point(0, 92);
            lblDescription.Margin = new Padding(0, 2, 0, 2);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(461, 17);
            lblDescription.TabIndex = 4;
            lblDescription.Text = "Description:";
            lblDescription.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Font = new Font("Segoe UI", 11F);
            txtDescription.Location = new Point(2, 113);
            txtDescription.Margin = new Padding(2);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(457, 56);
            txtDescription.TabIndex = 5;
            // 
            // lblRules
            // 
            lblRules.Dock = DockStyle.Fill;
            lblRules.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRules.Location = new Point(0, 173);
            lblRules.Margin = new Padding(0, 2, 0, 2);
            lblRules.Name = "lblRules";
            lblRules.Size = new Size(461, 17);
            lblRules.TabIndex = 6;
            lblRules.Text = "Rules:";
            lblRules.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtRules
            // 
            txtRules.Dock = DockStyle.Fill;
            txtRules.Font = new Font("Segoe UI", 11F);
            txtRules.Location = new Point(2, 194);
            txtRules.Margin = new Padding(2);
            txtRules.Multiline = true;
            txtRules.Name = "txtRules";
            txtRules.Size = new Size(457, 56);
            txtRules.TabIndex = 7;
            // 
            // lblAmenities
            // 
            lblAmenities.Dock = DockStyle.Fill;
            lblAmenities.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAmenities.Location = new Point(0, 254);
            lblAmenities.Margin = new Padding(0, 2, 0, 2);
            lblAmenities.Name = "lblAmenities";
            lblAmenities.Size = new Size(461, 17);
            lblAmenities.TabIndex = 8;
            lblAmenities.Text = "Amenities:";
            lblAmenities.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtAmenities
            // 
            txtAmenities.Dock = DockStyle.Fill;
            txtAmenities.Font = new Font("Segoe UI", 11F);
            txtAmenities.Location = new Point(2, 275);
            txtAmenities.Margin = new Padding(2);
            txtAmenities.Multiline = true;
            txtAmenities.Name = "txtAmenities";
            txtAmenities.Size = new Size(457, 56);
            txtAmenities.TabIndex = 9;
            // 
            // lblPictures
            // 
            lblPictures.Dock = DockStyle.Fill;
            lblPictures.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPictures.Margin = new Padding(0, 8, 0, 2);
            lblPictures.Name = "lblPictures";
            lblPictures.Size = new Size(461, 17);
            lblPictures.TabIndex = 10;
            lblPictures.Text = "Pictures:";
            lblPictures.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlImages
            // 
            pnlImages.Dock = DockStyle.Fill;
            pnlImages.FlowDirection = FlowDirection.LeftToRight;
            pnlImages.Margin = new Padding(0, 2, 0, 2);
            pnlImages.Name = "pnlImages";
            pnlImages.Size = new Size(461, 90);
            pnlImages.TabIndex = 11;
            pnlImages.WrapContents = false;
            // 
            // lblPicturesHint
            // 
            lblPicturesHint.Dock = DockStyle.Fill;
            lblPicturesHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblPicturesHint.ForeColor = Color.DimGray;
            lblPicturesHint.Margin = new Padding(0, 4, 0, 0);
            lblPicturesHint.Name = "lblPicturesHint";
            lblPicturesHint.Size = new Size(270, 15);
            lblPicturesHint.TabIndex = 0;
            lblPicturesHint.Text = "No pictures available for this property.";
            lblPicturesHint.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlImageActions
            // 
            pnlImageActions.Dock = DockStyle.Fill;
            pnlImageActions.FlowDirection = FlowDirection.LeftToRight;
            pnlImageActions.Margin = new Padding(0, 2, 0, 2);
            pnlImageActions.Name = "pnlImageActions";
            pnlImageActions.Size = new Size(461, 30);
            pnlImageActions.TabIndex = 12;
            pnlImageActions.WrapContents = false;
            pnlImageActions.Controls.Add(btnUploadImages);
            pnlImageActions.Controls.Add(lblPicturesHint);
            // 
            // btnUploadImages
            // 
            btnUploadImages.BackColor = Color.FromArgb(40, 167, 69);
            btnUploadImages.FlatStyle = FlatStyle.Flat;
            btnUploadImages.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnUploadImages.ForeColor = Color.White;
            btnUploadImages.Margin = new Padding(0, 2, 10, 0);
            btnUploadImages.Name = "btnUploadImages";
            btnUploadImages.Size = new Size(130, 24);
            btnUploadImages.TabIndex = 0;
            btnUploadImages.Text = "Upload Pictures";
            btnUploadImages.UseVisualStyleBackColor = false;
            // 
            // lblName
            // 
            lblName.Dock = DockStyle.Fill;
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblName.Location = new Point(0, 2);
            lblName.Margin = new Padding(0, 2, 0, 2);
            lblName.Name = "lblName";
            lblName.Size = new Size(461, 17);
            lblName.TabIndex = 0;
            lblName.Text = "Name:";
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Click += lblName_Click;
            // 
            // lblDetailSummaryTitle
            // 
            lblDetailSummaryTitle.Dock = DockStyle.Fill;
            lblDetailSummaryTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailSummaryTitle.Margin = new Padding(0, 10, 0, 2);
            lblDetailSummaryTitle.Name = "lblDetailSummaryTitle";
            lblDetailSummaryTitle.Size = new Size(461, 17);
            lblDetailSummaryTitle.TabIndex = 13;
            lblDetailSummaryTitle.Text = "Property Summary:";
            lblDetailSummaryTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDetailSummary
            // 
            lblDetailSummary.Dock = DockStyle.Fill;
            lblDetailSummary.Font = new Font("Segoe UI", 9F);
            lblDetailSummary.Margin = new Padding(0, 0, 0, 0);
            lblDetailSummary.Name = "lblDetailSummary";
            lblDetailSummary.Size = new Size(461, 60);
            lblDetailSummary.TabIndex = 14;
            lblDetailSummary.Text = "Select a property to see summary.";
            lblDetailSummary.TextAlign = ContentAlignment.TopLeft;
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnSelect);
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Location = new Point(14, 283);
            pnlButtons.Margin = new Padding(2);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(7, 6, 7, 6);
            pnlButtons.Size = new Size(489, 60);
            pnlButtons.TabIndex = 1;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(40, 167, 69);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(9, 8);
            btnAdd.Margin = new Padding(2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(84, 27);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add New";
            btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnEdit
            // 
            btnEdit.BackColor = Color.FromArgb(0, 123, 255);
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEdit.ForeColor = Color.White;
            btnEdit.Location = new Point(97, 8);
            btnEdit.Margin = new Padding(2);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(84, 27);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Update";
            btnEdit.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(185, 8);
            btnDelete.Margin = new Padding(2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(84, 27);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            // 
            // pbImage1
            // 
            pbImage1 = new PictureBox();
            pbImage1.Size = new Size(140, 90);
            pbImage1.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage1.Margin = new Padding(0, 0, 10, 0);
            pbImage1.Cursor = Cursors.Hand;
            pbImage1.Click += (s, e) => ShowImageFull(pbImage1);
            // 
            // pbImage2
            // 
            pbImage2 = new PictureBox();
            pbImage2.Size = new Size(140, 90);
            pbImage2.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage2.Margin = new Padding(0, 0, 10, 0);
            pbImage2.Cursor = Cursors.Hand;
            pbImage2.Click += (s, e) => ShowImageFull(pbImage2);
            // 
            // pbImage3
            // 
            pbImage3 = new PictureBox();
            pbImage3.Size = new Size(140, 90);
            pbImage3.SizeMode = PictureBoxSizeMode.Zoom;
            pbImage3.Margin = new Padding(0, 0, 0, 0);
            pbImage3.Cursor = Cursors.Hand;
            pbImage3.Click += (s, e) => ShowImageFull(pbImage3);

            pnlImages.Controls.Add(pbImage1);
            pnlImages.Controls.Add(pbImage2);
            pnlImages.Controls.Add(pbImage3);
            // 
            // btnSelect
            // 
            btnSelect.BackColor = Color.FromArgb(255, 193, 7);
            btnSelect.FlatStyle = FlatStyle.Flat;
            btnSelect.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSelect.Location = new Point(9, 39);
            btnSelect.Margin = new Padding(2);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(224, 27);
            btnSelect.TabIndex = 3;
            btnSelect.Text = "MANAGE THIS PROPERTY";
            btnSelect.UseVisualStyleBackColor = false;
            // 
            // searchTimer
            // 
            searchTimer.Interval = 350;
            // 
            // FormOwnerProperties
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 421);
            Controls.Add(splitContainer);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Margin = new Padding(2);
            Name = "FormOwnerProperties";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "My Boarding Houses";
            WindowState = FormWindowState.Maximized;
            pnlTop.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProperties).EndInit();
            pnlListHeader.ResumeLayout(false);
            pnlListHeader.PerformLayout();
            panelRight.ResumeLayout(false);
            grpInput.ResumeLayout(false);
            tableLayout.ResumeLayout(false);
            tableLayout.PerformLayout();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void WireEvents()
        {
            this.btnBackTop.Click += (s, e) => this.Close();
            this.btnClose.Click += (s, e) => this.Close();
            this.btnAdd.Click += (s, e) => AddProperty();
            this.btnEdit.Click += (s, e) => UpdateProperty();
            this.btnDelete.Click += (s, e) => DeleteProperty();
            this.btnSelect.Click += (s, e) => SelectProperty();
            this.btnUploadImages.Click += (s, e) => UploadImagesForProperty();
            this.dgvProperties.SelectionChanged += (s, e) => LoadSelection();
            this.txtSearch.TextChanged += (s, e) => { _pendingSearchText = txtSearch.Text; searchTimer.Stop(); searchTimer.Start(); };
            this.searchTimer.Tick += (s, e) => { searchTimer.Stop(); PerformSearch(_pendingSearchText); };
        }

        private void LoadProperties()
        {
            try
            {
                var list = _browseAll ? _repository.GetAll() : _repository.GetAllByOwner(_currentUser.Id);
                BindProperties(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading properties: " + ex.Message);
            }
        }

        private void BindProperties(List<BoardingHouse> list)
        {
            if (_isUpdatingResults) return;
            _isUpdatingResults = true;
            try
            {
                dgvProperties.DataSource = list;

                if (dgvProperties.Columns["OwnerId"] != null) dgvProperties.Columns["OwnerId"].Visible = false;
                if (dgvProperties.Columns["IsActive"] != null) dgvProperties.Columns["IsActive"].Visible = false;
                if (dgvProperties.Columns["CreatedAt"] != null) dgvProperties.Columns["CreatedAt"].Visible = false;
                if (dgvProperties.Columns["OwnerName"] != null) dgvProperties.Columns["OwnerName"].Visible = false;
                if (dgvProperties.Columns["ImagePath1"] != null) dgvProperties.Columns["ImagePath1"].Visible = false;
                if (dgvProperties.Columns["ImagePath2"] != null) dgvProperties.Columns["ImagePath2"].Visible = false;
                if (dgvProperties.Columns["ImagePath3"] != null) dgvProperties.Columns["ImagePath3"].Visible = false;
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
                LoadProperties();
                return;
            }

            try
            {
                var list = _browseAll ? _repository.Search(keyword) : _repository.SearchByOwner(_currentUser.Id, keyword);
                BindProperties(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching properties: " + ex.Message);
            }
        }

        private void UpdateImageBoxes()
        {
            if (_selectedHouse == null)
            {
                ClearImageBoxes();
                if (lblPicturesHint != null) lblPicturesHint.Visible = true;
                return;
            }

            bool anyImage = false;

            if (SetImageBox(pbImage1, _selectedHouse.ImagePath1)) anyImage = true;
            if (SetImageBox(pbImage2, _selectedHouse.ImagePath2)) anyImage = true;
            if (SetImageBox(pbImage3, _selectedHouse.ImagePath3)) anyImage = true;

            if (lblPicturesHint != null) lblPicturesHint.Visible = !anyImage;
        }

        private void ClearImageBoxes()
        {
            if (pbImage1 != null) SetImageBox(pbImage1, null);
            if (pbImage2 != null) SetImageBox(pbImage2, null);
            if (pbImage3 != null) SetImageBox(pbImage3, null);
            if (lblPicturesHint != null) lblPicturesHint.Visible = true;
        }

        private bool SetImageBox(PictureBox box, string? relativePath)
        {
            if (box == null) return false;

            if (box.Image != null)
            {
                box.Image.Dispose();
                box.Image = null;
            }

            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            string imagePath = Path.Combine(Application.StartupPath, relativePath);
            if (!File.Exists(imagePath)) return false;

            try
            {
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = Image.FromStream(stream))
                {
                    box.Image = new Bitmap(temp);
                    return true;
                }
            }
            catch
            {
                if (box.Image != null)
                {
                    box.Image.Dispose();
                    box.Image = null;
                }
            }
            return false;
        }

        private void UploadImagesForProperty()
        {
            if (_selectedHouse == null)
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
                    string? img1 = _selectedHouse.ImagePath1;
                    string? img2 = _selectedHouse.ImagePath2;
                    string? img3 = _selectedHouse.ImagePath3;

                    int fileIndex = 0;
                    for (int slot = 1; slot <= 3 && fileIndex < files.Length; slot++)
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

                    _selectedHouse.ImagePath1 = img1;
                    _selectedHouse.ImagePath2 = img2;
                    _selectedHouse.ImagePath3 = img3;

                    _repository.UpdateImages(
                        _selectedHouse.Id,
                        _selectedHouse.ImagePath1,
                        _selectedHouse.ImagePath2,
                        _selectedHouse.ImagePath3);

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

        private void ShowImageFull(PictureBox source)
        {
            if (source == null || source.Image == null) return;

            using (var form = new Form())
            using (var viewer = new PictureBox())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.WindowState = FormWindowState.Maximized;
                form.BackColor = Color.Black;
                form.Text = "Picture";

                viewer.Dock = DockStyle.Fill;
                viewer.SizeMode = PictureBoxSizeMode.Zoom;
                viewer.Image = source.Image;

                form.Controls.Add(viewer);
                form.ShowDialog(this);
            }
        }

        private void LoadSelection()
        {
            if (dgvProperties.SelectedRows.Count > 0)
            {
                _selectedHouse = (BoardingHouse)dgvProperties.SelectedRows[0].DataBoundItem;
                txtName.Text = _selectedHouse.Name;
                txtAddress.Text = _selectedHouse.Address;
                txtDescription.Text = _selectedHouse.Description;
                txtRules.Text = _selectedHouse.Rules;
                txtAmenities.Text = _selectedHouse.Amenities;

                UpdateImageBoxes();

                if (lblDetailSummary != null && _selectedHouse != null)
                {
                    string owner = string.IsNullOrWhiteSpace(_selectedHouse.OwnerName) ? "N/A" : _selectedHouse.OwnerName;
                    string amenities = string.IsNullOrWhiteSpace(_selectedHouse.Amenities) ? "None" : _selectedHouse.Amenities;
                    lblDetailSummary.Text =
                        $"Name: {_selectedHouse.Name}\n" +
                        $"Owner: {owner}\n" +
                        $"Address: {_selectedHouse.Address}\n" +
                        $"Amenities: {amenities}";
                }

                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnSelect.Enabled = true;
            }
            else
            {
                ClearInputs();
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnSelect.Enabled = false;
            }
        }

        private void ClearInputs()
        {
            _selectedHouse = null;
            txtName.Clear();
            txtAddress.Clear();
            txtDescription.Clear();
            txtRules.Clear();
            txtAmenities.Clear();
            ClearImageBoxes();
            if (lblDetailSummary != null) lblDetailSummary.Text = "Select a property to see summary.";
        }

        private void AddProperty()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Property Name is required.");
                return;
            }

            var bh = new BoardingHouse
            {
                OwnerId = _currentUser.Id,
                Name = txtName.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Rules = txtRules.Text.Trim(),
                Amenities = txtAmenities.Text.Trim()
            };

            try
            {
                _repository.Add(bh);
                MessageBox.Show("Property added successfully!");
                LoadProperties();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding property: " + ex.Message);
            }
        }

        private void UpdateProperty()
        {
            if (_selectedHouse == null) return;

            _selectedHouse.Name = txtName.Text.Trim();
            _selectedHouse.Address = txtAddress.Text.Trim();
            _selectedHouse.Description = txtDescription.Text.Trim();
            _selectedHouse.Rules = txtRules.Text.Trim();
            _selectedHouse.Amenities = txtAmenities.Text.Trim();

            try
            {
                _repository.Update(_selectedHouse);
                MessageBox.Show("Property updated successfully!");
                LoadProperties();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating property: " + ex.Message);
            }
        }

        private void DeleteProperty()
        {
            if (_selectedHouse == null) return;

            if (MessageBox.Show("Are you sure you want to delete this property?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _repository.Delete(_selectedHouse.Id);
                    LoadProperties();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting property: " + ex.Message);
                }
            }
        }

        private void SelectProperty()
        {
            if (_selectedHouse != null)
            {
                PropertySelected?.Invoke(_selectedHouse);
                MessageBox.Show($"Selected property: {_selectedHouse.Name}. \nThe dashboard will now update to show data for this property.", "Context Switched");
                this.Close();
            }
        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void panelLeft_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
