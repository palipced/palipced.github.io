namespace pced
{
    partial class frmtool
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmtool));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbmulu = new System.Windows.Forms.ToolStripButton();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.tsbdict = new System.Windows.Forms.ToolStripButton();
            this.tsddb = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbWindow = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiCascadeWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTileWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaxAllVisiblePaliWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHideAllPaliWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHideAllWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbHide = new System.Windows.Forms.ToolStripButton();
            this.tsbquit = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cboxHccc = new System.Windows.Forms.CheckBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.MintCream;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbmulu,
            this.toolStripSeparator1,
            this.tsbSearch,
            this.toolStripSeparator2,
            this.tsbdict,
            this.toolStripSeparator3,
            this.tsddb,
            this.toolStripSeparator4,
            this.tsddbWindow,
            this.toolStripSeparator5,
            this.tsbHide,
            this.toolStripSeparator6,
            this.tsbquit,
            this.toolStripSeparator7});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(637, 20);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsbmulu
            // 
            this.tsbmulu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbmulu.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsbmulu.ForeColor = System.Drawing.Color.DimGray;
            this.tsbmulu.Image = ((System.Drawing.Image)(resources.GetObject("tsbmulu.Image")));
            this.tsbmulu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbmulu.Name = "tsbmulu";
            this.tsbmulu.Size = new System.Drawing.Size(41, 17);
            this.tsbmulu.Text = "三藏";
            this.tsbmulu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbmulu.ToolTipText = "巴利三藏目录，可从中打开经典。";
            this.tsbmulu.Click += new System.EventHandler(this.tsbmulu_Click);
            this.tsbmulu.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSearch.ForeColor = System.Drawing.Color.DimGray;
            this.tsbSearch.Image = ((System.Drawing.Image)(resources.GetObject("tsbSearch.Image")));
            this.tsbSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Size = new System.Drawing.Size(41, 17);
            this.tsbSearch.Text = "搜索";
            this.tsbSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbSearch.ToolTipText = "Pali三藏全文搜索";
            this.tsbSearch.Click += new System.EventHandler(this.tsbSearch_Click);
            // 
            // tsbdict
            // 
            this.tsbdict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbdict.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsbdict.ForeColor = System.Drawing.Color.DimGray;
            this.tsbdict.Image = ((System.Drawing.Image)(resources.GetObject("tsbdict.Image")));
            this.tsbdict.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbdict.Name = "tsbdict";
            this.tsbdict.Size = new System.Drawing.Size(41, 17);
            this.tsbdict.Text = "辞典";
            this.tsbdict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbdict.ToolTipText = "显示巴利语辞典窗口。";
            this.tsbdict.Click += new System.EventHandler(this.tsbdict_Click);
            this.tsbdict.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsddb
            // 
            this.tsddb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddb.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsddb.ForeColor = System.Drawing.Color.DimGray;
            this.tsddb.Image = ((System.Drawing.Image)(resources.GetObject("tsddb.Image")));
            this.tsddb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddb.Name = "tsddb";
            this.tsddb.Size = new System.Drawing.Size(50, 17);
            this.tsddb.Text = "窗口";
            this.tsddb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddb.ToolTipText = "当前所有已经打开的读经窗口列表，若没有打开任何读经窗口，则无此列表；被最小化隐藏了的窗口，在列表中以深色表示。";
            this.tsddb.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsddbWindow
            // 
            this.tsddbWindow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCascadeWindows,
            this.tsmiTileWindows,
            this.tsmiMaxAllVisiblePaliWindows,
            this.tsmiRestore,
            this.tsmiHideAllPaliWindows,
            this.tsmiHideAllWindows});
            this.tsddbWindow.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsddbWindow.ForeColor = System.Drawing.Color.DimGray;
            this.tsddbWindow.Image = ((System.Drawing.Image)(resources.GetObject("tsddbWindow.Image")));
            this.tsddbWindow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbWindow.Name = "tsddbWindow";
            this.tsddbWindow.Size = new System.Drawing.Size(50, 17);
            this.tsddbWindow.Text = "管理";
            this.tsddbWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbWindow.ToolTipText = "管理当前已经打开的读经窗口等、可以进行层叠、平铺、最大化、及隐藏窗口。";
            this.tsddbWindow.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsmiCascadeWindows
            // 
            this.tsmiCascadeWindows.Name = "tsmiCascadeWindows";
            this.tsmiCascadeWindows.Size = new System.Drawing.Size(254, 22);
            this.tsmiCascadeWindows.Text = "层叠所有可见的读经窗口";
            this.tsmiCascadeWindows.Click += new System.EventHandler(this.tsmiCascadeWindows_Click);
            // 
            // tsmiTileWindows
            // 
            this.tsmiTileWindows.Name = "tsmiTileWindows";
            this.tsmiTileWindows.Size = new System.Drawing.Size(254, 22);
            this.tsmiTileWindows.Text = "平铺所有可见的读经窗口";
            this.tsmiTileWindows.Click += new System.EventHandler(this.tsmiTileWindows_Click);
            // 
            // tsmiMaxAllVisiblePaliWindows
            // 
            this.tsmiMaxAllVisiblePaliWindows.Name = "tsmiMaxAllVisiblePaliWindows";
            this.tsmiMaxAllVisiblePaliWindows.Size = new System.Drawing.Size(254, 22);
            this.tsmiMaxAllVisiblePaliWindows.Text = "最大化所有可见的读经窗口";
            this.tsmiMaxAllVisiblePaliWindows.Click += new System.EventHandler(this.tsmiMaxAllVisiblePaliWindows_Click);
            // 
            // tsmiRestore
            // 
            this.tsmiRestore.Name = "tsmiRestore";
            this.tsmiRestore.Size = new System.Drawing.Size(254, 22);
            this.tsmiRestore.Text = "撤销层叠/平铺/最大化";
            this.tsmiRestore.ToolTipText = "撤销最近一次的层叠/平铺/最大化操作，把所有可见的读经窗口复原为此前的位置与大小";
            this.tsmiRestore.Click += new System.EventHandler(this.tsmiRestore_Click);
            // 
            // tsmiHideAllPaliWindows
            // 
            this.tsmiHideAllPaliWindows.Name = "tsmiHideAllPaliWindows";
            this.tsmiHideAllPaliWindows.Size = new System.Drawing.Size(254, 22);
            this.tsmiHideAllPaliWindows.Text = "隐藏所有读经窗口";
            this.tsmiHideAllPaliWindows.Click += new System.EventHandler(this.tsmiHideAllPaliWindows_Click);
            // 
            // tsmiHideAllWindows
            // 
            this.tsmiHideAllWindows.Name = "tsmiHideAllWindows";
            this.tsmiHideAllWindows.Size = new System.Drawing.Size(254, 22);
            this.tsmiHideAllWindows.Text = "隐藏所有窗口";
            this.tsmiHideAllWindows.Click += new System.EventHandler(this.tsmiHideAllWindows_Click);
            // 
            // tsbHide
            // 
            this.tsbHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHide.ForeColor = System.Drawing.Color.DimGray;
            this.tsbHide.Image = ((System.Drawing.Image)(resources.GetObject("tsbHide.Image")));
            this.tsbHide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHide.Name = "tsbHide";
            this.tsbHide.Size = new System.Drawing.Size(41, 17);
            this.tsbHide.Text = "隐藏";
            this.tsbHide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbHide.ToolTipText = "隐藏此工具条。";
            this.tsbHide.Click += new System.EventHandler(this.tsbHide_Click);
            this.tsbHide.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // tsbquit
            // 
            this.tsbquit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbquit.ForeColor = System.Drawing.Color.DimGray;
            this.tsbquit.Image = ((System.Drawing.Image)(resources.GetObject("tsbquit.Image")));
            this.tsbquit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbquit.Name = "tsbquit";
            this.tsbquit.Size = new System.Drawing.Size(41, 17);
            this.tsbquit.Text = "退出";
            this.tsbquit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbquit.ToolTipText = "退出此程序。";
            this.tsbquit.Click += new System.EventHandler(this.tsbquit_Click);
            this.tsbquit.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel1.Location = new System.Drawing.Point(3, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 3);
            this.panel1.TabIndex = 1;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel2.Location = new System.Drawing.Point(3, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(634, 3);
            this.panel2.TabIndex = 2;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3.BackgroundImage")));
            this.panel3.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(3, 25);
            this.panel3.TabIndex = 3;
            this.panel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel4.Location = new System.Drawing.Point(634, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(3, 25);
            this.panel4.TabIndex = 4;
            this.panel4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // cboxHccc
            // 
            this.cboxHccc.AutoSize = true;
            this.cboxHccc.BackColor = System.Drawing.Color.MintCream;
            this.cboxHccc.Checked = true;
            this.cboxHccc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxHccc.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cboxHccc.ForeColor = System.Drawing.Color.DimGray;
            this.cboxHccc.Location = new System.Drawing.Point(520, 4);
            this.cboxHccc.Name = "cboxHccc";
            this.cboxHccc.Size = new System.Drawing.Size(86, 18);
            this.cboxHccc.TabIndex = 5;
            this.cboxHccc.Text = "划词查词";
            this.cboxHccc.UseVisualStyleBackColor = false;
            this.cboxHccc.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            this.cboxHccc.MouseHover += new System.EventHandler(this.cboxHccc_MouseHover);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 20);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 20);
            // 
            // frmtool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(640, 25);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cboxHccc);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.toolStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(298, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmtool";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "巴利三藏 义注 复注阅读 词典助译软件1.7版";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmtool_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.ToolStripDropDownButton tsddb;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideAllPaliWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideAllWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaxAllVisiblePaliWindows;
        public System.Windows.Forms.CheckBox cboxHccc;
        private System.Windows.Forms.ToolStripMenuItem tsmiCascadeWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiTileWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestore;
        public System.Windows.Forms.ToolStripButton tsbmulu;
        public System.Windows.Forms.ToolStripButton tsbdict;
        public System.Windows.Forms.ToolStripButton tsbquit;
        public System.Windows.Forms.ToolStripButton tsbHide;
        public System.Windows.Forms.ToolStripDropDownButton tsddbWindow;
        public System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    }
}