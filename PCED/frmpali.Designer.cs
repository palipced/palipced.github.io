namespace pced
{
    partial class frmpali
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmpali));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.cMenuWeb2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmWeb2selectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmWeb2copy = new System.Windows.Forms.ToolStripMenuItem();
            this.cwWeb2highLight = new System.Windows.Forms.ToolStripMenuItem();
            this.cwWeb2cancelHighLight = new System.Windows.Forms.ToolStripMenuItem();
            this.cwWeb2cancelAllBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.cwWeb2Lookup = new System.Windows.Forms.ToolStripMenuItem();
            this.cwWeb2Search = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnmula = new System.Windows.Forms.ToolStripButton();
            this.tsbtnattha = new System.Windows.Forms.ToolStripButton();
            this.tsbtntika = new System.Windows.Forms.ToolStripButton();
            this.tsbShowToolbar = new System.Windows.Forms.ToolStripButton();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.statusWeb = new System.Windows.Forms.StatusStrip();
            this.tsslPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslVRIPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslMyanmarPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslPTSPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslThaiPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.cMenuTWord = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmtselectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmtcopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmtpaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmtcut = new System.Windows.Forms.ToolStripMenuItem();
            this.tstboxWord = new System.Windows.Forms.TextBox();
            this.btnCz = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrior = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.tslHitsNum = new System.Windows.Forms.Label();
            this.btntoPage = new System.Windows.Forms.Button();
            this.cboxPage = new System.Windows.Forms.ComboBox();
            this.tboxPage = new System.Windows.Forms.TextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tsDDbtnNikaya = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsDDbtnBook = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsDDbtnChapter = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsDDbtnTitle = new System.Windows.Forms.ToolStripDropDownButton();
            this.copywithFormatonlyUnicodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenuWeb2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusWeb.SuspendLayout();
            this.cMenuTWord.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.ContextMenuStrip = this.cMenuWeb2;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 50);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(23, 25);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(742, 400);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.SizeChanged += new System.EventHandler(this.webBrowser1_SizeChanged);
            this.webBrowser1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser1_PreviewKeyDown);
            // 
            // cMenuWeb2
            // 
            this.cMenuWeb2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmWeb2selectAll,
            this.cmWeb2copy,
            this.cwWeb2highLight,
            this.cwWeb2cancelHighLight,
            this.cwWeb2cancelAllBlue,
            this.cwWeb2Lookup,
            this.cwWeb2Search,
            this.copywithFormatonlyUnicodeToolStripMenuItem});
            this.cMenuWeb2.Name = "cMenuWeb2";
            this.cMenuWeb2.Size = new System.Drawing.Size(272, 202);
            // 
            // cmWeb2selectAll
            // 
            this.cmWeb2selectAll.Name = "cmWeb2selectAll";
            this.cmWeb2selectAll.Size = new System.Drawing.Size(271, 22);
            this.cmWeb2selectAll.Text = "selectAll";
            this.cmWeb2selectAll.Click += new System.EventHandler(this.cmWeb2selectAll_Click);
            // 
            // cmWeb2copy
            // 
            this.cmWeb2copy.Name = "cmWeb2copy";
            this.cmWeb2copy.Size = new System.Drawing.Size(271, 22);
            this.cmWeb2copy.Text = "copy";
            this.cmWeb2copy.Click += new System.EventHandler(this.cmWeb2copy_Click);
            // 
            // cwWeb2highLight
            // 
            this.cwWeb2highLight.Name = "cwWeb2highLight";
            this.cwWeb2highLight.Size = new System.Drawing.Size(271, 22);
            this.cwWeb2highLight.Text = "HighLight Selected-text";
            this.cwWeb2highLight.Click += new System.EventHandler(this.cwWeb2highLight_Click);
            // 
            // cwWeb2cancelHighLight
            // 
            this.cwWeb2cancelHighLight.Name = "cwWeb2cancelHighLight";
            this.cwWeb2cancelHighLight.Size = new System.Drawing.Size(271, 22);
            this.cwWeb2cancelHighLight.Text = "Cancel HighLight Selected-text";
            this.cwWeb2cancelHighLight.Click += new System.EventHandler(this.cwWeb2cancelHighLight_Click);
            // 
            // cwWeb2cancelAllBlue
            // 
            this.cwWeb2cancelAllBlue.Name = "cwWeb2cancelAllBlue";
            this.cwWeb2cancelAllBlue.Size = new System.Drawing.Size(271, 22);
            this.cwWeb2cancelAllBlue.Text = "Cancel All blue-HighLight";
            this.cwWeb2cancelAllBlue.Click += new System.EventHandler(this.cwWeb2cancelAllBlue_Click);
            // 
            // cwWeb2Lookup
            // 
            this.cwWeb2Lookup.Name = "cwWeb2Lookup";
            this.cwWeb2Lookup.Size = new System.Drawing.Size(271, 22);
            this.cwWeb2Lookup.Text = "Look up Selected-text";
            this.cwWeb2Lookup.Click += new System.EventHandler(this.cwWeb2Lookup_Click);
            // 
            // cwWeb2Search
            // 
            this.cwWeb2Search.Name = "cwWeb2Search";
            this.cwWeb2Search.Size = new System.Drawing.Size(271, 22);
            this.cwWeb2Search.Text = "Search Selected-text in Pali canon";
            this.cwWeb2Search.Click += new System.EventHandler(this.cwWeb2Search_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnmula,
            this.tsbtnattha,
            this.tsbtntika,
            this.tsbShowToolbar,
            this.tsbCopy});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(199, 25);
            this.toolStrip1.TabIndex = 116;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnmula
            // 
            this.tsbtnmula.AutoToolTip = false;
            this.tsbtnmula.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnmula.Enabled = false;
            this.tsbtnmula.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnmula.Name = "tsbtnmula";
            this.tsbtnmula.Size = new System.Drawing.Size(41, 22);
            this.tsbtnmula.Text = "根本";
            this.tsbtnmula.Click += new System.EventHandler(this.tsbtnmula_Click);
            this.tsbtnmula.MouseEnter += new System.EventHandler(this.tsbtnmula_MouseEnter);
            // 
            // tsbtnattha
            // 
            this.tsbtnattha.AutoToolTip = false;
            this.tsbtnattha.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnattha.Enabled = false;
            this.tsbtnattha.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnattha.Name = "tsbtnattha";
            this.tsbtnattha.Size = new System.Drawing.Size(41, 22);
            this.tsbtnattha.Text = "义注";
            this.tsbtnattha.Click += new System.EventHandler(this.tsbtnattha_Click);
            this.tsbtnattha.MouseEnter += new System.EventHandler(this.tsbtnattha_MouseEnter);
            // 
            // tsbtntika
            // 
            this.tsbtntika.AutoToolTip = false;
            this.tsbtntika.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtntika.Enabled = false;
            this.tsbtntika.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtntika.Name = "tsbtntika";
            this.tsbtntika.Size = new System.Drawing.Size(41, 22);
            this.tsbtntika.Text = "复注";
            this.tsbtntika.Click += new System.EventHandler(this.tsbtntika_Click);
            this.tsbtntika.MouseEnter += new System.EventHandler(this.tsbtntika_MouseEnter);
            // 
            // tsbShowToolbar
            // 
            this.tsbShowToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbShowToolbar.Image = ((System.Drawing.Image)(resources.GetObject("tsbShowToolbar.Image")));
            this.tsbShowToolbar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowToolbar.Name = "tsbShowToolbar";
            this.tsbShowToolbar.Size = new System.Drawing.Size(23, 22);
            this.tsbShowToolbar.Text = "T";
            this.tsbShowToolbar.ToolTipText = "如果‘工具条’被隐藏了，点击此按钮可以显示它";
            this.tsbShowToolbar.Click += new System.EventHandler(this.tsbShowToolbar_Click);
            // 
            // tsbCopy
            // 
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopy.Image")));
            this.tsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Size = new System.Drawing.Size(41, 22);
            this.tsbCopy.Text = "帮助";
            this.tsbCopy.ToolTipText = "关于复制";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(552, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 117;
            this.button1.Text = "编程测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusWeb
            // 
            this.statusWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusWeb.AutoSize = false;
            this.statusWeb.Dock = System.Windows.Forms.DockStyle.None;
            this.statusWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusWeb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslPage,
            this.tsslVRIPage,
            this.tsslMyanmarPage,
            this.tsslPTSPage,
            this.tsslThaiPage});
            this.statusWeb.Location = new System.Drawing.Point(0, 451);
            this.statusWeb.Name = "statusWeb";
            this.statusWeb.Size = new System.Drawing.Size(742, 22);
            this.statusWeb.TabIndex = 118;
            this.statusWeb.Text = "statusStrip1";
            // 
            // tsslPage
            // 
            this.tsslPage.AutoSize = false;
            this.tsslPage.Name = "tsslPage";
            this.tsslPage.Size = new System.Drawing.Size(50, 17);
            this.tsslPage.Text = "PAGE:";
            // 
            // tsslVRIPage
            // 
            this.tsslVRIPage.AutoSize = false;
            this.tsslVRIPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsslVRIPage.Name = "tsslVRIPage";
            this.tsslVRIPage.Size = new System.Drawing.Size(75, 17);
            this.tsslVRIPage.Text = "VRI 1.0001";
            this.tsslVRIPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslMyanmarPage
            // 
            this.tsslMyanmarPage.AutoSize = false;
            this.tsslMyanmarPage.Name = "tsslMyanmarPage";
            this.tsslMyanmarPage.Size = new System.Drawing.Size(110, 17);
            this.tsslMyanmarPage.Text = "Myanmar 1.0001";
            this.tsslMyanmarPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslPTSPage
            // 
            this.tsslPTSPage.AutoSize = false;
            this.tsslPTSPage.Name = "tsslPTSPage";
            this.tsslPTSPage.Size = new System.Drawing.Size(80, 17);
            this.tsslPTSPage.Text = "PTS 1.0001";
            this.tsslPTSPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslThaiPage
            // 
            this.tsslThaiPage.AutoSize = false;
            this.tsslThaiPage.Name = "tsslThaiPage";
            this.tsslThaiPage.Size = new System.Drawing.Size(80, 17);
            this.tsslThaiPage.Text = "Thai 1.0001";
            this.tsslThaiPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cMenuTWord
            // 
            this.cMenuTWord.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmtselectAll,
            this.cmtcopy,
            this.cmtpaste,
            this.cmtcut});
            this.cMenuTWord.Name = "cMenuTWord";
            this.cMenuTWord.Size = new System.Drawing.Size(124, 92);
            // 
            // cmtselectAll
            // 
            this.cmtselectAll.Name = "cmtselectAll";
            this.cmtselectAll.Size = new System.Drawing.Size(123, 22);
            this.cmtselectAll.Text = "selectAll";
            this.cmtselectAll.Click += new System.EventHandler(this.cmtselectAll_Click);
            // 
            // cmtcopy
            // 
            this.cmtcopy.Name = "cmtcopy";
            this.cmtcopy.Size = new System.Drawing.Size(123, 22);
            this.cmtcopy.Text = "copy";
            this.cmtcopy.Click += new System.EventHandler(this.cmtcopy_Click);
            // 
            // cmtpaste
            // 
            this.cmtpaste.Name = "cmtpaste";
            this.cmtpaste.Size = new System.Drawing.Size(123, 22);
            this.cmtpaste.Text = "paste";
            this.cmtpaste.Click += new System.EventHandler(this.cmtpaste_Click);
            // 
            // cmtcut
            // 
            this.cmtcut.Name = "cmtcut";
            this.cmtcut.Size = new System.Drawing.Size(123, 22);
            this.cmtcut.Text = "cut";
            this.cmtcut.Click += new System.EventHandler(this.cmtcut_Click);
            // 
            // tstboxWord
            // 
            this.tstboxWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tstboxWord.ContextMenuStrip = this.cMenuTWord;
            this.tstboxWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tstboxWord.Location = new System.Drawing.Point(366, 25);
            this.tstboxWord.Name = "tstboxWord";
            this.tstboxWord.Size = new System.Drawing.Size(163, 24);
            this.tstboxWord.TabIndex = 119;
            this.tstboxWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstboxWord_KeyDown);
            this.tstboxWord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tstboxWord_KeyPress);
            // 
            // btnCz
            // 
            this.btnCz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCz.Location = new System.Drawing.Point(530, 26);
            this.btnCz.Name = "btnCz";
            this.btnCz.Size = new System.Drawing.Size(50, 23);
            this.btnCz.TabIndex = 120;
            this.btnCz.Text = "查找";
            this.btnCz.UseVisualStyleBackColor = true;
            this.btnCz.Click += new System.EventHandler(this.btnCz_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirst.Location = new System.Drawing.Point(580, 26);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(27, 23);
            this.btnFirst.TabIndex = 121;
            this.btnFirst.Text = "|<";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrior
            // 
            this.btnPrior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrior.Location = new System.Drawing.Point(607, 26);
            this.btnPrior.Name = "btnPrior";
            this.btnPrior.Size = new System.Drawing.Size(27, 23);
            this.btnPrior.TabIndex = 122;
            this.btnPrior.Text = "<";
            this.btnPrior.UseVisualStyleBackColor = true;
            this.btnPrior.Click += new System.EventHandler(this.btnPrior_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(634, 26);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(27, 23);
            this.btnNext.TabIndex = 123;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLast.Location = new System.Drawing.Point(661, 26);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(27, 23);
            this.btnLast.TabIndex = 124;
            this.btnLast.Text = ">|";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // tslHitsNum
            // 
            this.tslHitsNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tslHitsNum.AutoSize = true;
            this.tslHitsNum.Location = new System.Drawing.Point(692, 30);
            this.tslHitsNum.Name = "tslHitsNum";
            this.tslHitsNum.Size = new System.Drawing.Size(14, 15);
            this.tslHitsNum.TabIndex = 125;
            this.tslHitsNum.Text = "0";
            // 
            // btntoPage
            // 
            this.btntoPage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btntoPage.Location = new System.Drawing.Point(334, 26);
            this.btntoPage.Name = "btntoPage";
            this.btntoPage.Size = new System.Drawing.Size(30, 23);
            this.btntoPage.TabIndex = 126;
            this.btntoPage.Text = "=>";
            this.btntoPage.UseVisualStyleBackColor = true;
            this.btntoPage.Click += new System.EventHandler(this.btntoPage_Click);
            // 
            // cboxPage
            // 
            this.cboxPage.FormattingEnabled = true;
            this.cboxPage.Items.AddRange(new object[] {
            "VRI PAGE:",
            "Myanmar :",
            "PTS PAGE:",
            "Thai    :",
            "Paragraph:"});
            this.cboxPage.Location = new System.Drawing.Point(206, 26);
            this.cboxPage.Name = "cboxPage";
            this.cboxPage.Size = new System.Drawing.Size(85, 23);
            this.cboxPage.TabIndex = 127;
            this.cboxPage.Text = "PTS PAGE:";
            this.cboxPage.SelectionChangeCommitted += new System.EventHandler(this.cboxPage_SelectionChangeCommitted);
            // 
            // tboxPage
            // 
            this.tboxPage.Location = new System.Drawing.Point(293, 27);
            this.tboxPage.Name = "tboxPage";
            this.tboxPage.Size = new System.Drawing.Size(40, 21);
            this.tboxPage.TabIndex = 128;
            this.tboxPage.Text = "1.18";
            this.tboxPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tboxPage_KeyPress);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsDDbtnNikaya,
            this.tsDDbtnBook,
            this.tsDDbtnChapter,
            this.tsDDbtnTitle});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(742, 25);
            this.toolStrip2.TabIndex = 129;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tsDDbtnNikaya
            // 
            this.tsDDbtnNikaya.AutoSize = false;
            this.tsDDbtnNikaya.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDDbtnNikaya.Image = ((System.Drawing.Image)(resources.GetObject("tsDDbtnNikaya.Image")));
            this.tsDDbtnNikaya.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDbtnNikaya.Name = "tsDDbtnNikaya";
            this.tsDDbtnNikaya.Size = new System.Drawing.Size(135, 22);
            this.tsDDbtnNikaya.Text = "Nikaya";
            this.tsDDbtnNikaya.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsDDbtnNikaya_DropDownItemClicked);
            // 
            // tsDDbtnBook
            // 
            this.tsDDbtnBook.AutoSize = false;
            this.tsDDbtnBook.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDDbtnBook.Image = ((System.Drawing.Image)(resources.GetObject("tsDDbtnBook.Image")));
            this.tsDDbtnBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDbtnBook.Name = "tsDDbtnBook";
            this.tsDDbtnBook.Size = new System.Drawing.Size(180, 22);
            this.tsDDbtnBook.Text = "Book";
            this.tsDDbtnBook.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsDDbtnBook_DropDownItemClicked);
            // 
            // tsDDbtnChapter
            // 
            this.tsDDbtnChapter.AutoSize = false;
            this.tsDDbtnChapter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDDbtnChapter.Image = ((System.Drawing.Image)(resources.GetObject("tsDDbtnChapter.Image")));
            this.tsDDbtnChapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDbtnChapter.Name = "tsDDbtnChapter";
            this.tsDDbtnChapter.Size = new System.Drawing.Size(220, 22);
            this.tsDDbtnChapter.Text = "Chapter";
            this.tsDDbtnChapter.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsDDbtnChapter_DropDownItemClicked);
            // 
            // tsDDbtnTitle
            // 
            this.tsDDbtnTitle.AutoSize = false;
            this.tsDDbtnTitle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDDbtnTitle.Image = ((System.Drawing.Image)(resources.GetObject("tsDDbtnTitle.Image")));
            this.tsDDbtnTitle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDbtnTitle.Name = "tsDDbtnTitle";
            this.tsDDbtnTitle.Size = new System.Drawing.Size(175, 22);
            this.tsDDbtnTitle.Text = "Title";
            this.tsDDbtnTitle.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsDDbtnTitle_DropDownItemClicked);
            // 
            // copywithFormatonlyUnicodeToolStripMenuItem
            // 
            this.copywithFormatonlyUnicodeToolStripMenuItem.Name = "copywithFormatonlyUnicodeToolStripMenuItem";
            this.copywithFormatonlyUnicodeToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.copywithFormatonlyUnicodeToolStripMenuItem.Text = "copy (with format: only unicode)";
            this.copywithFormatonlyUnicodeToolStripMenuItem.Click += new System.EventHandler(this.copywithFormatonlyUnicodeToolStripMenuItem_Click);
            // 
            // frmpali
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 473);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.tstboxWord);
            this.Controls.Add(this.btntoPage);
            this.Controls.Add(this.tboxPage);
            this.Controls.Add(this.cboxPage);
            this.Controls.Add(this.statusWeb);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.btnCz);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.btnPrior);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.tslHitsNum);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(50, 70);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmpali";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmpali";
            this.Activated += new System.EventHandler(this.frmpali_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmpali_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmpali_FormClosed);
            this.Load += new System.EventHandler(this.frmpali_Load);
            this.ResizeBegin += new System.EventHandler(this.frmpali_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.frmpali_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.frmpali_SizeChanged);
            this.cMenuWeb2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusWeb.ResumeLayout(false);
            this.statusWeb.PerformLayout();
            this.cMenuTWord.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnmula;
        private System.Windows.Forms.ToolStripButton tsbtnattha;
        private System.Windows.Forms.ToolStripButton tsbtntika;
        private System.Windows.Forms.ContextMenuStrip cMenuWeb2;
        private System.Windows.Forms.ToolStripMenuItem cmWeb2copy;
        private System.Windows.Forms.ToolStripMenuItem cmWeb2selectAll;
        private System.Windows.Forms.ToolStripButton tsbShowToolbar;
        private System.Windows.Forms.ToolStripButton tsbCopy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusStrip statusWeb;
        private System.Windows.Forms.ToolStripStatusLabel tsslVRIPage;
        private System.Windows.Forms.ToolStripStatusLabel tsslMyanmarPage;
        private System.Windows.Forms.ToolStripStatusLabel tsslPTSPage;
        private System.Windows.Forms.ToolStripStatusLabel tsslThaiPage;
        private System.Windows.Forms.ToolStripStatusLabel tsslPage;
        private System.Windows.Forms.ToolStripMenuItem cwWeb2highLight;
        private System.Windows.Forms.ToolStripMenuItem cwWeb2cancelHighLight;
        private System.Windows.Forms.ToolStripMenuItem cwWeb2cancelAllBlue;
        private System.Windows.Forms.ContextMenuStrip cMenuTWord;
        private System.Windows.Forms.ToolStripMenuItem cmtselectAll;
        private System.Windows.Forms.ToolStripMenuItem cmtcopy;
        private System.Windows.Forms.ToolStripMenuItem cmtpaste;
        private System.Windows.Forms.ToolStripMenuItem cmtcut;
        private System.Windows.Forms.Button btnCz;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrior;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Label tslHitsNum;
        private System.Windows.Forms.Button btntoPage;
        private System.Windows.Forms.ComboBox cboxPage;
        private System.Windows.Forms.TextBox tboxPage;
        public System.Windows.Forms.TextBox tstboxWord;
        private System.Windows.Forms.ToolStripMenuItem cwWeb2Lookup;
        private System.Windows.Forms.ToolStripMenuItem cwWeb2Search;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripDropDownButton tsDDbtnNikaya;
        private System.Windows.Forms.ToolStripDropDownButton tsDDbtnBook;
        private System.Windows.Forms.ToolStripDropDownButton tsDDbtnChapter;
        private System.Windows.Forms.ToolStripDropDownButton tsDDbtnTitle;
        private System.Windows.Forms.ToolStripMenuItem copywithFormatonlyUnicodeToolStripMenuItem;
    }
}