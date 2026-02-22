using System.Drawing;
using System.Windows.Forms;

namespace BoardingHouseSys.UI
{
    public static class UITheme
    {
        // Color Palette - Blue Theme
        public static Color PrimaryColor = Color.FromArgb(25, 118, 210);      // Royal Blue
        public static Color SecondaryColor = Color.FromArgb(96, 125, 139);    // Blue Grey
        public static Color SuccessColor = Color.FromArgb(56, 142, 60);       // Green
        public static Color DangerColor = Color.FromArgb(211, 47, 47);        // Red
        public static Color WarningColor = Color.FromArgb(255, 160, 0);       // Amber
        public static Color InfoColor = Color.FromArgb(2, 136, 209);          // Light Blue
        
        public static Color LightColor = Color.FromArgb(240, 242, 245);       // Very Light Blue/Gray (Background)
        public static Color DarkColor = Color.FromArgb(33, 33, 33);           // Almost Black (Text)
        public static Color HeaderColor = Color.FromArgb(21, 101, 192);       // Darker Blue (Header)
        
        // Modern Sidebar Palette
        public static Color SidebarColor = Color.FromArgb(13, 71, 161);       // Deep Blue
        public static Color SidebarTextColor = Color.FromArgb(225, 245, 254); // Very Light Blue Text
        public static Color SidebarActiveColor = Color.FromArgb(33, 150, 243);  // Brighter Blue for active
        public static Color SidebarHoverColor = Color.FromArgb(25, 118, 210);   // Hover Blue
        public static Color AccentColor = Color.FromArgb(68, 138, 255);       // Accent Blue

        // Fonts - Increased Sizes
        public static Font TitleFont = new Font("Segoe UI", 20, FontStyle.Bold);
        public static Font HeaderFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static Font NormalFont = new Font("Segoe UI", 12);
        public static Font SmallFont = new Font("Segoe UI", 10);
        
        // Button Styles
        public static void ApplyButtonStyle(Button button, int width = 140, int height = 40)
        {
            // Use the button's current background color
            ApplyButtonStyle(button, button.BackColor, width, height);
        }

        public static void ApplyButtonStyle(Button button, Color bgColor, int width = 140, int height = 40)
        {
            button.BackColor = bgColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold); // Bold for better visibility
            button.Size = new Size(width, height);
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            
            // Remove existing handlers to prevent stacking
            button.MouseEnter -= Button_MouseEnter;
            button.MouseLeave -= Button_MouseLeave;

            // Hover effects
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
        }

        private static void Button_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = ControlPaint.Light(btn.BackColor, 0.1f);
            }
        }

        private static void Button_MouseLeave(object? sender, EventArgs e)
        {
             // This simple logic might fail if we don't store original color.
             // For general buttons, we assume the current color is the base color (or close enough).
             // A better approach for general buttons is to store the base color in the Tag or similar, 
             // but strictly speaking, ControlPaint.Light creates a NEW color.
             // So when we leave, we need to revert.
             // Given the limitations, let's just darken it back or use the predefined colors if possible.
             // To be safe and simple: specific button types call Apply... again which sets the color.
             // But for hover, we need a generic revert.
             
             if (sender is Button btn)
             {
                 // Check against known palette to revert correctly
                 if (IsColorSimilar(btn.BackColor, PrimaryColor)) btn.BackColor = PrimaryColor;
                 else if (IsColorSimilar(btn.BackColor, SecondaryColor)) btn.BackColor = SecondaryColor;
                 else if (IsColorSimilar(btn.BackColor, SuccessColor)) btn.BackColor = SuccessColor;
                 else if (IsColorSimilar(btn.BackColor, DangerColor)) btn.BackColor = DangerColor;
                 else if (IsColorSimilar(btn.BackColor, WarningColor)) btn.BackColor = WarningColor;
                 else if (IsColorSimilar(btn.BackColor, InfoColor)) btn.BackColor = InfoColor;
                 else 
                 {
                     // Fallback: Darken it back
                     btn.BackColor = ControlPaint.Dark(btn.BackColor, 0.1f); // Rough approximation
                 }
             }
        }

        private static bool IsColorSimilar(Color c1, Color c2)
        {
             // Allow for slight floating point diffs from Light/Dark ops
             return Math.Abs(c1.R - c2.R) < 30 && Math.Abs(c1.G - c2.G) < 30 && Math.Abs(c1.B - c2.B) < 30;
        }
        
        public static void ApplyPrimaryButton(Button button, int width = 140, int height = 40)
        {
            ApplyButtonStyle(button, PrimaryColor, width, height);
            // Explicitly set Tag for easier color restoration if we wanted to get fancy, 
            // but the IsColorSimilar approach above is robust enough for simple hovers.
        }
        
        public static void ApplySecondaryButton(Button button, int width = 140, int height = 40)
        {
            ApplyButtonStyle(button, SecondaryColor, width, height);
        }

        public static void ApplyNavButton(Button button, int width = 100, int height = 35)
        {
            ApplyButtonStyle(button, SecondaryColor, width, height);
            button.Font = SmallFont;
        }
        
        public static void ApplySuccessButton(Button button, int width = 140, int height = 40)
        {
            ApplyButtonStyle(button, SuccessColor, width, height);
        }
        
        public static void ApplyDangerButton(Button button, int width = 140, int height = 40)
        {
            ApplyButtonStyle(button, DangerColor, width, height);
        }
        
        // TextBox Styles
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.BackColor = Color.White;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = NormalFont;
            textBox.ForeColor = DarkColor;
        }

        public static void ApplyNumericUpDownStyle(NumericUpDown nud)
        {
            nud.BackColor = Color.White;
            nud.BorderStyle = BorderStyle.FixedSingle;
            nud.Font = NormalFont;
            nud.ForeColor = DarkColor;
        }

        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.Font = NormalFont;
            comboBox.ForeColor = DarkColor;
            comboBox.FlatStyle = FlatStyle.Flat; 
        }

        public static void ApplyGroupBoxStyle(GroupBox groupBox)
        {
            groupBox.Font = new Font(NormalFont, FontStyle.Bold);
            groupBox.ForeColor = PrimaryColor;
            groupBox.BackColor = Color.White;
        }
        
        // Label Styles
        public static void ApplyLabelStyle(Label label, bool isBold = false)
        {
            label.Font = isBold ? new Font(NormalFont, FontStyle.Bold) : NormalFont;
            label.ForeColor = DarkColor;
        }
        
        public static void ApplyHeaderLabelStyle(Label label)
        {
            label.Font = HeaderFont;
            label.ForeColor = PrimaryColor;
        }

        public static void ApplyTitleLabelStyle(Label label)
        {
            label.Font = TitleFont;
            label.ForeColor = PrimaryColor;
        }

        public static void ApplySubHeaderLabelStyle(Label label)
        {
             label.Font = new Font("Segoe UI", 12, FontStyle.Bold);
             label.ForeColor = SecondaryColor;
        }
        
        // Form Styles
        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = LightColor; // Use the light blue/gray background
            form.Font = NormalFont;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ForeColor = DarkColor;
        }
        
        // Panel Styles
        public static void ApplyHeaderStyle(Panel panel)
        {
            panel.BackColor = HeaderColor;
            panel.ForeColor = Color.White;
            panel.Padding = new Padding(10);
        }

        public static void ApplyPanelStyle(Panel panel, Color? backgroundColor = null)
        {
            panel.BackColor = backgroundColor ?? LightColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(10);
        }
        
        public static void ApplySidebarButtonStyle(Button button, bool isActive = false)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = isActive ? SidebarActiveColor : SidebarColor;
            button.ForeColor = isActive ? Color.White : SidebarTextColor;
            button.Font = new Font("Segoe UI", 12F, isActive ? FontStyle.Bold : FontStyle.Regular); // Increased font size
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(20, 0, 0, 0); // Increased padding
            button.Size = new Size(240, 60); // Increased height to 60
            button.Cursor = Cursors.Hand;

            // Border for active state (Left accent bar)
            if (isActive)
            {
                button.FlatAppearance.BorderColor = SidebarColor; // Hide default border
            }

            // Remove existing handlers
            button.MouseEnter -= SidebarButton_MouseEnter;
            button.MouseLeave -= SidebarButton_MouseLeave;

            // Add handlers
            button.MouseEnter += SidebarButton_MouseEnter;
            button.MouseLeave += SidebarButton_MouseLeave;
        }

        private static void SidebarButton_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.BackColor != SidebarActiveColor) 
                    btn.BackColor = SidebarHoverColor;
            }
        }

        private static void SidebarButton_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.BackColor != SidebarHoverColor && btn.BackColor != SidebarActiveColor) 
                    btn.BackColor = SidebarColor;
                
                // If it was hovering (HoverColor), revert to normal (SidebarColor)
                // If it was active (ActiveColor), it stays ActiveColor.
                // But wait, the original logic said:
                // if (btn.BackColor != SidebarHoverColor && btn.BackColor != SidebarActiveColor) btn.BackColor = SidebarColor;
                // This logic is slightly flawed because if it IS HoverColor, we want to revert it.
                
                if (btn.BackColor == SidebarHoverColor)
                    btn.BackColor = SidebarColor;
            }
        }

        // DataGridView Styles
        public static void ApplyDataGridViewStyle(DataGridView gridView)
        {
            gridView.BackgroundColor = LightColor;
            gridView.BorderStyle = BorderStyle.None;
            gridView.EnableHeadersVisualStyles = false;
            gridView.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            gridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridView.ColumnHeadersDefaultCellStyle.Font = new Font(NormalFont, FontStyle.Bold);
            gridView.RowHeadersVisible = false;
            gridView.DefaultCellStyle.Font = NormalFont;
            gridView.DefaultCellStyle.BackColor = Color.White;
            gridView.DefaultCellStyle.ForeColor = DarkColor;
            gridView.DefaultCellStyle.SelectionBackColor = PrimaryColor;
            gridView.DefaultCellStyle.SelectionForeColor = Color.White;
            gridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            gridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            gridView.GridColor = PrimaryColor;
            gridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
