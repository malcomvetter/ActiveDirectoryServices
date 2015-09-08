using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WorkingCode.CodeProject.PwdGen
{
    /// <summary>
    ///     Summary description for Form1.
    /// </summary>
    public class PwdGenUI : Form
    {
        private Button btnGenerate;
        private IContainer components;
        private TextBox genPwd;
        private Label label1;
        private Label label2;
        private ToolStripMenuItem mediumToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem policyToolStripMenuItem;
        private PasswordGenerator.PasswordGenerator pwdGen;
        private ToolStripMenuItem strongToolStripMenuItem;
        private ToolStripMenuItem veryStrongToolStripMenuItem;
        private ToolStripMenuItem weakToolStripMenuItem;

        public PwdGenUI()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            wipeClipboard();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void wipeClipboard()
        {
            for (var i = 0; i < 9; i++)
            {
                Clipboard.SetText(pwdGen.Generate(), TextDataFormat.Text);
            }
            Clipboard.Clear();
            label2.Text = "Clipboard has been wiped.";
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.Run(new PwdGenUI());
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            Opacity = 0.55;
            label1.Text = "Sleeping...";
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Opacity = 0.90;
            label1.Text = "And we're back ...";
        }

        private void PwdGenUI_Load(object sender, EventArgs e)
        {
            var RK = Registry.CurrentUser.OpenSubKey("Software\\PWGEN");
            if (RK != null)
            {
                menuStrip1.Visible = true;
            }
            pwdGen = new PasswordGenerator.PasswordGenerator();
            mediumToolStripMenuItem_Click();
            btnGenerate_Click();
            btnGenerate.Focus();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (null != pwdGen)
            {
                btnGenerate_Click();
            }
        }

        private void btnGenerate_Click()
        {
            if (null != pwdGen)
            {
                genPwd.Text = pwdGen.Generate();
                Clipboard.SetText(genPwd.Text, TextDataFormat.Text);
                label1.Text = "Automatically Copied.  Ready to Paste.";
                label2.Text = "Warning:  Clipboard Contents Wiped at Exit.";
            }
        }

        private void weakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediumToolStripMenuItem.Checked = false;
            strongToolStripMenuItem.Checked = false;
            veryStrongToolStripMenuItem.Checked = false;
            pwdGen.Exclusions = "ABCDEFGHIJKLMNOPQRSTUVWXYZo01lI";
            pwdGen.Minimum = 7;
            pwdGen.Maximum = pwdGen.Minimum + 1;
            pwdGen.ConsecutiveCharacters = true;
            pwdGen.RepeatCharacters = true;
            pwdGen.ExcludeSymbols = true;
            btnGenerate_Click();
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediumToolStripMenuItem_Click();
        }

        private void mediumToolStripMenuItem_Click()
        {
            weakToolStripMenuItem.Checked = false;
            strongToolStripMenuItem.Checked = false;
            veryStrongToolStripMenuItem.Checked = false;
            pwdGen.Exclusions = "`~$%^&*()=+[]{}\\|;:'\",<.>/?-_oO01lI";
            pwdGen.Minimum = 7;
            pwdGen.Maximum = pwdGen.Minimum + 1;
            pwdGen.ConsecutiveCharacters = false;
            pwdGen.RepeatCharacters = true;
            pwdGen.ExcludeSymbols = false;
            btnGenerate_Click();
        }

        private void strongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            weakToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            veryStrongToolStripMenuItem.Checked = false;
            pwdGen.Exclusions = "`~^()[]{}\\|;:'\",<.>/?";
            pwdGen.Minimum = 12;
            pwdGen.Maximum = pwdGen.Minimum + 1;
            pwdGen.ConsecutiveCharacters = false;
            pwdGen.RepeatCharacters = false;
            pwdGen.ExcludeSymbols = false;
            btnGenerate_Click();
        }

        private void veryStrongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            weakToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            strongToolStripMenuItem.Checked = false;
            pwdGen.Exclusions = null;
            pwdGen.Minimum = 18;
            pwdGen.Maximum = pwdGen.Minimum + 1;
            pwdGen.ConsecutiveCharacters = false;
            pwdGen.RepeatCharacters = false;
            pwdGen.ExcludeSymbols = false;
            btnGenerate_Click();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof (PwdGenUI));
            this.genPwd = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.policyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.weakToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.veryStrongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // genPwd
            // 
            this.genPwd.Location = new System.Drawing.Point(12, 41);
            this.genPwd.Name = "genPwd";
            this.genPwd.ShortcutsEnabled = false;
            this.genPwd.Size = new System.Drawing.Size(131, 20);
            this.genPwd.TabIndex = 4;
            this.genPwd.WordWrap = false;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnGenerate.Location = new System.Drawing.Point(155, 41);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 20);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Click to Generate a Random Password.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 7;
            // 
            // policyToolStripMenuItem
            // 
            this.policyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.weakToolStripMenuItem,
                this.mediumToolStripMenuItem,
                this.strongToolStripMenuItem,
                this.veryStrongToolStripMenuItem
            });
            this.policyToolStripMenuItem.Name = "policyToolStripMenuItem";
            this.policyToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.policyToolStripMenuItem.Text = "Policy";
            // 
            // weakToolStripMenuItem
            // 
            this.weakToolStripMenuItem.CheckOnClick = true;
            this.weakToolStripMenuItem.Name = "weakToolStripMenuItem";
            this.weakToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.weakToolStripMenuItem.Text = "Weak";
            this.weakToolStripMenuItem.Click += new System.EventHandler(this.weakToolStripMenuItem_Click);
            // 
            // mediumToolStripMenuItem
            // 
            this.mediumToolStripMenuItem.Checked = true;
            this.mediumToolStripMenuItem.CheckOnClick = true;
            this.mediumToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mediumToolStripMenuItem.Name = "mediumToolStripMenuItem";
            this.mediumToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mediumToolStripMenuItem.Text = "Medium";
            this.mediumToolStripMenuItem.Click += new System.EventHandler(this.mediumToolStripMenuItem_Click);
            // 
            // strongToolStripMenuItem
            // 
            this.strongToolStripMenuItem.CheckOnClick = true;
            this.strongToolStripMenuItem.Name = "strongToolStripMenuItem";
            this.strongToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.strongToolStripMenuItem.Text = "Strong";
            this.strongToolStripMenuItem.Click += new System.EventHandler(this.strongToolStripMenuItem_Click);
            // 
            // veryStrongToolStripMenuItem
            // 
            this.veryStrongToolStripMenuItem.CheckOnClick = true;
            this.veryStrongToolStripMenuItem.Name = "veryStrongToolStripMenuItem";
            this.veryStrongToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.veryStrongToolStripMenuItem.Text = "Very Strong";
            this.veryStrongToolStripMenuItem.Click += new System.EventHandler(this.veryStrongToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.policyToolStripMenuItem
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.MaximumSize = new System.Drawing.Size(0, 20);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(234, 20);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // PwdGenUI
            // 
            this.AllowDrop = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(234, 78);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.genPwd);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(250, 114);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 114);
            this.Name = "PwdGenUI";
            this.Opacity = 0.9;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HelpDesk Password Generator";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.PwdGenUI_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}