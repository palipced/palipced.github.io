namespace pced
{
    partial class frmLucn
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tboxSs = new System.Windows.Forms.TextBox();
            this.cmsInput = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.button2 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.cmsWeb = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmwSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmwCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPageNext = new System.Windows.Forms.Button();
            this.btnPagePrior = new System.Windows.Forms.Button();
            this.btnPageFirst = new System.Windows.Forms.Button();
            this.btnPageLast = new System.Windows.Forms.Button();
            this.btnDcinx = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.cmsInput.SuspendLayout();
            this.cmsWeb.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.AutoSize = true;
            this.button1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(622, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "make index";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(508, 158);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(190, 232);
            this.textBox1.TabIndex = 5;
            this.textBox1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(620, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // tboxSs
            // 
            this.tboxSs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxSs.ContextMenuStrip = this.cmsInput;
            this.tboxSs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboxSs.Location = new System.Drawing.Point(12, 10);
            this.tboxSs.Name = "tboxSs";
            this.tboxSs.Size = new System.Drawing.Size(490, 22);
            this.tboxSs.TabIndex = 1;
            this.tboxSs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tboxSs_KeyDown);
            this.tboxSs.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // cmsInput
            // 
            this.cmsInput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiSelectAll,
            this.cmiCopy,
            this.cmiPaste,
            this.cmiCut});
            this.cmsInput.Name = "cmsInput";
            this.cmsInput.Size = new System.Drawing.Size(125, 92);
            // 
            // cmiSelectAll
            // 
            this.cmiSelectAll.Name = "cmiSelectAll";
            this.cmiSelectAll.Size = new System.Drawing.Size(124, 22);
            this.cmiSelectAll.Text = "selectAll";
            this.cmiSelectAll.Click += new System.EventHandler(this.cmiSelectAll_Click);
            // 
            // cmiCopy
            // 
            this.cmiCopy.Name = "cmiCopy";
            this.cmiCopy.Size = new System.Drawing.Size(124, 22);
            this.cmiCopy.Text = "copy";
            this.cmiCopy.Click += new System.EventHandler(this.cmiCopy_Click);
            // 
            // cmiPaste
            // 
            this.cmiPaste.Name = "cmiPaste";
            this.cmiPaste.Size = new System.Drawing.Size(124, 22);
            this.cmiPaste.Text = "paste";
            this.cmiPaste.Click += new System.EventHandler(this.cmiPaste_Click);
            // 
            // cmiCut
            // 
            this.cmiCut.Name = "cmiCut";
            this.cmiCut.Size = new System.Drawing.Size(124, 22);
            this.cmiCut.Text = "cut";
            this.cmiCut.Click += new System.EventHandler(this.cmiCut_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(508, 9);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Search";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.ContextMenuStrip = this.cmsWeb;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(12, 66);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(718, 395);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser1_PreviewKeyDown);
            // 
            // cmsWeb
            // 
            this.cmsWeb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmwSelectAll,
            this.cmwCopy});
            this.cmsWeb.Name = "cmsWeb";
            this.cmsWeb.Size = new System.Drawing.Size(125, 48);
            // 
            // cmwSelectAll
            // 
            this.cmwSelectAll.Name = "cmwSelectAll";
            this.cmwSelectAll.Size = new System.Drawing.Size(124, 22);
            this.cmwSelectAll.Text = "selectAll";
            this.cmwSelectAll.Click += new System.EventHandler(this.cmwSelectAll_Click);
            // 
            // cmwCopy
            // 
            this.cmwCopy.Name = "cmwCopy";
            this.cmwCopy.Size = new System.Drawing.Size(124, 22);
            this.cmwCopy.Text = "copy";
            this.cmwCopy.Click += new System.EventHandler(this.cmwCopy_Click);
            // 
            // btnPageNext
            // 
            this.btnPageNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPageNext.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPageNext.Location = new System.Drawing.Point(622, 38);
            this.btnPageNext.Name = "btnPageNext";
            this.btnPageNext.Size = new System.Drawing.Size(51, 23);
            this.btnPageNext.TabIndex = 3;
            this.btnPageNext.Text = "Down>";
            this.btnPageNext.UseVisualStyleBackColor = true;
            this.btnPageNext.Click += new System.EventHandler(this.btnPageNext_Click);
            // 
            // btnPagePrior
            // 
            this.btnPagePrior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPagePrior.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPagePrior.Location = new System.Drawing.Point(565, 38);
            this.btnPagePrior.Name = "btnPagePrior";
            this.btnPagePrior.Size = new System.Drawing.Size(51, 23);
            this.btnPagePrior.TabIndex = 4;
            this.btnPagePrior.Text = "<Up";
            this.btnPagePrior.UseVisualStyleBackColor = true;
            this.btnPagePrior.Click += new System.EventHandler(this.btnPagePrior_Click);
            // 
            // btnPageFirst
            // 
            this.btnPageFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPageFirst.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPageFirst.Location = new System.Drawing.Point(508, 38);
            this.btnPageFirst.Name = "btnPageFirst";
            this.btnPageFirst.Size = new System.Drawing.Size(57, 23);
            this.btnPageFirst.TabIndex = 5;
            this.btnPageFirst.Text = "|<First";
            this.btnPageFirst.UseVisualStyleBackColor = true;
            this.btnPageFirst.Click += new System.EventHandler(this.btnPageFirst_Click);
            // 
            // btnPageLast
            // 
            this.btnPageLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPageLast.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPageLast.Location = new System.Drawing.Point(679, 38);
            this.btnPageLast.Name = "btnPageLast";
            this.btnPageLast.Size = new System.Drawing.Size(51, 23);
            this.btnPageLast.TabIndex = 6;
            this.btnPageLast.Text = "Last>|";
            this.btnPageLast.UseVisualStyleBackColor = true;
            this.btnPageLast.Click += new System.EventHandler(this.btnPageLast_Click);
            // 
            // btnDcinx
            // 
            this.btnDcinx.ForeColor = System.Drawing.Color.Blue;
            this.btnDcinx.Location = new System.Drawing.Point(508, 71);
            this.btnDcinx.Name = "btnDcinx";
            this.btnDcinx.Size = new System.Drawing.Size(75, 23);
            this.btnDcinx.TabIndex = 10;
            this.btnDcinx.Text = "建立词索引";
            this.btnDcinx.UseVisualStyleBackColor = true;
            this.btnDcinx.Visible = false;
            this.btnDcinx.Click += new System.EventHandler(this.btnDcinx_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(12, 38);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(490, 22);
            this.textBox2.TabIndex = 0;
            this.textBox2.Text = "here can input ahead 3+letter use english mode, then press \'Enter\' to list pali w" +
                "ord.";
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress_1);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(12, 66);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(490, 324);
            this.listBox1.TabIndex = 7;
            this.listBox1.Visible = false;
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            this.listBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listBox1_KeyPress);
            // 
            // frmLucn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 473);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDcinx);
            this.Controls.Add(this.tboxSs);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnPageLast);
            this.Controls.Add(this.btnPageFirst);
            this.Controls.Add(this.btnPageNext);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnPagePrior);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.button1);
            this.Name = "frmLucn";
            this.Text = "pali Tipiṭaka full text search";
            this.Load += new System.EventHandler(this.frmLucn_Load);
            this.Shown += new System.EventHandler(this.frmLucn_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLucn_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLucn_FormClosing);
            this.cmsInput.ResumeLayout(false);
            this.cmsWeb.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tboxSs;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btnPageNext;
        private System.Windows.Forms.Button btnPagePrior;
        private System.Windows.Forms.Button btnPageFirst;
        private System.Windows.Forms.Button btnPageLast;
        private System.Windows.Forms.ContextMenuStrip cmsInput;
        private System.Windows.Forms.ToolStripMenuItem cmiPaste;
        private System.Windows.Forms.ToolStripMenuItem cmiSelectAll;
        private System.Windows.Forms.ToolStripMenuItem cmiCopy;
        private System.Windows.Forms.ToolStripMenuItem cmiCut;
        private System.Windows.Forms.ContextMenuStrip cmsWeb;
        private System.Windows.Forms.ToolStripMenuItem cmwSelectAll;
        private System.Windows.Forms.ToolStripMenuItem cmwCopy;
        private System.Windows.Forms.Button btnDcinx;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ListBox listBox1;
    }
}