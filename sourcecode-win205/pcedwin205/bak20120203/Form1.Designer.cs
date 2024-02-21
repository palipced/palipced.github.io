namespace pced
{
    partial class Form1
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
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("词典导航");
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lookupsctword = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copy = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLookup = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toollbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toollbltimes = new System.Windows.Forms.ToolStripStatusLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.cboxInput = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cms3selectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cms3copy = new System.Windows.Forms.ToolStripMenuItem();
            this.cms3paste = new System.Windows.Forms.ToolStripMenuItem();
            this.cms3cut = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menupalimulu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLanguageSet = new System.Windows.Forms.ToolStripMenuItem();
            this.FAN = new System.Windows.Forms.ToolStripMenuItem();
            this.HAN = new System.Windows.Forms.ToolStripMenuItem();
            this.EN = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOtherset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuosSetWindowInFront = new System.Windows.Forms.ToolStripMenuItem();
            this.menuosCopyAutoCc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuosZwfcCx = new System.Windows.Forms.ToolStripMenuItem();
            this.menuosFontSize = new System.Windows.Forms.ToolStripMenuItem();
            this.fsoriginal = new System.Windows.Forms.ToolStripMenuItem();
            this.fsplus = new System.Windows.Forms.ToolStripMenuItem();
            this.fsplustwo = new System.Windows.Forms.ToolStripMenuItem();
            this.fsplusthree = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhFofa = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhJwzl = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhHowto = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhSly = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhReadme = new System.Windows.Forms.ToolStripMenuItem();
            this.menuhabout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stShowToolBar = new System.Windows.Forms.ToolStripMenuItem();
            this.stShow = new System.Windows.Forms.ToolStripMenuItem();
            this.stQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuosEnglishPali = new System.Windows.Forms.CheckBox();
            this.menuosBlurinputmode = new System.Windows.Forms.CheckBox();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.cboxABC = new System.Windows.Forms.CheckBox();
            this.btnTj = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.lblFhc = new System.Windows.Forms.Label();
            this.gboxFontSet = new System.Windows.Forms.GroupBox();
            this.rbtnVriRomanPali = new System.Windows.Forms.RadioButton();
            this.rbtnSangayana = new System.Windows.Forms.RadioButton();
            this.rbtnTahoma = new System.Windows.Forms.RadioButton();
            this.panelfhc = new System.Windows.Forms.Panel();
            this.pbtnfhcclose = new System.Windows.Forms.Button();
            this.btnshowfhc = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tsmiPaliWinPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.gboxFontSet.SuspendLayout();
            this.panelfhc.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lookupsctword,
            this.toolStripSeparator1,
            this.copy,
            this.selectAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(226, 76);
            // 
            // lookupsctword
            // 
            this.lookupsctword.Name = "lookupsctword";
            this.lookupsctword.Size = new System.Drawing.Size(225, 22);
            this.lookupsctword.Text = "look up this selected word";
            this.lookupsctword.Click += new System.EventHandler(this.lookupsctword_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // copy
            // 
            this.copy.Name = "copy";
            this.copy.Size = new System.Drawing.Size(225, 22);
            this.copy.Text = "copy";
            this.copy.Click += new System.EventHandler(this.copy_Click);
            // 
            // selectAll
            // 
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(225, 22);
            this.selectAll.Text = "select all";
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // btnLookup
            // 
            this.btnLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLookup.Image = ((System.Drawing.Image)(resources.GetObject("btnLookup.Image")));
            this.btnLookup.Location = new System.Drawing.Point(376, 63);
            this.btnLookup.Margin = new System.Windows.Forms.Padding(4);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(75, 23);
            this.btnLookup.TabIndex = 1;
            this.btnLookup.Text = "look    up";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            this.btnLookup.MouseHover += new System.EventHandler(this.btnLookup_MouseHover);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toollbl,
            this.toollbltimes});
            this.statusStrip1.Location = new System.Drawing.Point(0, 483);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 20, 0);
            this.statusStrip1.Size = new System.Drawing.Size(454, 30);
            this.statusStrip1.TabIndex = 35;
            // 
            // toollbl
            // 
            this.toollbl.AutoSize = false;
            this.toollbl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toollbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toollbl.Name = "toollbl";
            this.toollbl.Size = new System.Drawing.Size(337, 25);
            this.toollbl.Text = "觉悟之路网站 http://www.dhamma.org.cn/ 上座部佛教";
            this.toollbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toollbltimes
            // 
            this.toollbltimes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toollbltimes.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toollbltimes.Name = "toollbltimes";
            this.toollbltimes.Size = new System.Drawing.Size(82, 25);
            this.toollbltimes.Text = "0.1234567 s.";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.richTextBox1.Location = new System.Drawing.Point(34, 49);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(123, 66);
            this.richTextBox1.TabIndex = 39;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(7, 28);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3.Size = new System.Drawing.Size(426, 88);
            this.textBox3.TabIndex = 74;
            this.textBox3.WordWrap = false;
            this.textBox3.DoubleClick += new System.EventHandler(this.textBox3_DoubleClick);
            // 
            // cboxInput
            // 
            this.cboxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxInput.ContextMenuStrip = this.contextMenuStrip3;
            this.cboxInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxInput.FormattingEnabled = true;
            this.cboxInput.Location = new System.Drawing.Point(138, 63);
            this.cboxInput.Margin = new System.Windows.Forms.Padding(4);
            this.cboxInput.MaxDropDownItems = 26;
            this.cboxInput.Name = "cboxInput";
            this.cboxInput.Size = new System.Drawing.Size(235, 24);
            this.cboxInput.TabIndex = 77;
            this.cboxInput.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cboxInput_MouseUp);
            this.cboxInput.SelectionChangeCommitted += new System.EventHandler(this.cboxInput_SelectionChangeCommitted);
            this.cboxInput.Leave += new System.EventHandler(this.cboxInput_Leave);
            this.cboxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboxInput_KeyPress);
            this.cboxInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboxInput_KeyUp);
            this.cboxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboxInput_KeyDown);
            this.cboxInput.TextUpdate += new System.EventHandler(this.cboxInput_TextUpdate);
            this.cboxInput.DropDown += new System.EventHandler(this.cboxInput_DropDown);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cms3selectAll,
            this.cms3copy,
            this.cms3paste,
            this.cms3cut});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(125, 92);
            // 
            // cms3selectAll
            // 
            this.cms3selectAll.Name = "cms3selectAll";
            this.cms3selectAll.Size = new System.Drawing.Size(124, 22);
            this.cms3selectAll.Text = "selectAll";
            this.cms3selectAll.Click += new System.EventHandler(this.cms3selectAll_Click);
            // 
            // cms3copy
            // 
            this.cms3copy.Name = "cms3copy";
            this.cms3copy.Size = new System.Drawing.Size(124, 22);
            this.cms3copy.Text = "copy";
            this.cms3copy.Click += new System.EventHandler(this.cms3copy_Click);
            // 
            // cms3paste
            // 
            this.cms3paste.Name = "cms3paste";
            this.cms3paste.Size = new System.Drawing.Size(124, 22);
            this.cms3paste.Text = "paste";
            this.cms3paste.Click += new System.EventHandler(this.cms3paste_Click);
            // 
            // cms3cut
            // 
            this.cms3cut.Name = "cms3cut";
            this.cms3cut.Size = new System.Drawing.Size(124, 22);
            this.cms3cut.Text = "cut";
            this.cms3cut.Click += new System.EventHandler(this.cms3cut_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.ContextMenuStrip = this.contextMenuStrip1;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 4);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(4);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(27, 25);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(312, 234);
            this.webBrowser1.TabIndex = 82;
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser1_PreviewKeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menupalimulu,
            this.menuLanguageSet,
            this.menuOtherset,
            this.menuHelp,
            this.menuQuit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(454, 24);
            this.menuStrip1.TabIndex = 83;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menupalimulu
            // 
            this.menupalimulu.Name = "menupalimulu";
            this.menupalimulu.Size = new System.Drawing.Size(80, 20);
            this.menupalimulu.Text = "巴利三藏";
            this.menupalimulu.ToolTipText = "显示巴利三藏经典目录与工具条";
            this.menupalimulu.Click += new System.EventHandler(this.menupalimulu_Click);
            // 
            // menuLanguageSet
            // 
            this.menuLanguageSet.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FAN,
            this.HAN,
            this.EN});
            this.menuLanguageSet.Name = "menuLanguageSet";
            this.menuLanguageSet.Size = new System.Drawing.Size(96, 20);
            this.menuLanguageSet.Text = "Language set";
            // 
            // FAN
            // 
            this.FAN.Name = "FAN";
            this.FAN.Size = new System.Drawing.Size(116, 22);
            this.FAN.Text = "繁体";
            this.FAN.Click += new System.EventHandler(this.FAN_Click);
            // 
            // HAN
            // 
            this.HAN.Name = "HAN";
            this.HAN.Size = new System.Drawing.Size(116, 22);
            this.HAN.Text = "简体";
            this.HAN.Click += new System.EventHandler(this.HAN_Click);
            // 
            // EN
            // 
            this.EN.Name = "EN";
            this.EN.Size = new System.Drawing.Size(116, 22);
            this.EN.Text = "&English";
            this.EN.Click += new System.EventHandler(this.EN_Click);
            // 
            // menuOtherset
            // 
            this.menuOtherset.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuosSetWindowInFront,
            this.menuosCopyAutoCc,
            this.menuosZwfcCx,
            this.menuosFontSize,
            this.tsmiPaliWinPosition});
            this.menuOtherset.Name = "menuOtherset";
            this.menuOtherset.Size = new System.Drawing.Size(73, 20);
            this.menuOtherset.Text = "Other set";
            // 
            // menuosSetWindowInFront
            // 
            this.menuosSetWindowInFront.Name = "menuosSetWindowInFront";
            this.menuosSetWindowInFront.Size = new System.Drawing.Size(361, 22);
            this.menuosSetWindowInFront.Text = "Set window in front";
            this.menuosSetWindowInFront.Click += new System.EventHandler(this.menuosSetWindowInFront_Click);
            // 
            // menuosCopyAutoCc
            // 
            this.menuosCopyAutoCc.CheckOnClick = true;
            this.menuosCopyAutoCc.Name = "menuosCopyAutoCc";
            this.menuosCopyAutoCc.Size = new System.Drawing.Size(361, 22);
            this.menuosCopyAutoCc.Text = "打开在外部程序中复制自动粘贴查词功能";
            // 
            // menuosZwfcCx
            // 
            this.menuosZwfcCx.CheckOnClick = true;
            this.menuosZwfcCx.Name = "menuosZwfcCx";
            this.menuosZwfcCx.Size = new System.Drawing.Size(361, 22);
            this.menuosZwfcCx.Text = "输入中文反查巴利文时结果按模糊词序输出";
            // 
            // menuosFontSize
            // 
            this.menuosFontSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fsoriginal,
            this.fsplus,
            this.fsplustwo,
            this.fsplusthree});
            this.menuosFontSize.Name = "menuosFontSize";
            this.menuosFontSize.Size = new System.Drawing.Size(361, 22);
            this.menuosFontSize.Text = "Set PaliTxt Font Size";
            // 
            // fsoriginal
            // 
            this.fsoriginal.Name = "fsoriginal";
            this.fsoriginal.Size = new System.Drawing.Size(153, 22);
            this.fsoriginal.Text = "original";
            this.fsoriginal.Click += new System.EventHandler(this.fsoriginal_Click);
            // 
            // fsplus
            // 
            this.fsplus.Name = "fsplus";
            this.fsplus.Size = new System.Drawing.Size(153, 22);
            this.fsplus.Text = "plus";
            this.fsplus.Click += new System.EventHandler(this.fsplus_Click);
            // 
            // fsplustwo
            // 
            this.fsplustwo.Name = "fsplustwo";
            this.fsplustwo.Size = new System.Drawing.Size(153, 22);
            this.fsplustwo.Text = "plus plus";
            this.fsplustwo.Click += new System.EventHandler(this.fsplustwo_Click);
            // 
            // fsplusthree
            // 
            this.fsplusthree.Name = "fsplusthree";
            this.fsplusthree.Size = new System.Drawing.Size(153, 22);
            this.fsplusthree.Text = "plus plus plus";
            this.fsplusthree.Click += new System.EventHandler(this.fsplusthree_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuhFofa,
            this.menuhJwzl,
            this.menuhHowto,
            this.menuhSly,
            this.menuhReadme,
            this.menuhabout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(45, 20);
            this.menuHelp.Text = "Help";
            // 
            // menuhFofa
            // 
            this.menuhFofa.Name = "menuhFofa";
            this.menuhFofa.Size = new System.Drawing.Size(242, 22);
            this.menuhFofa.Text = "What The Buddha Taught";
            this.menuhFofa.Click += new System.EventHandler(this.menuhFofa_Click);
            // 
            // menuhJwzl
            // 
            this.menuhJwzl.Name = "menuhJwzl";
            this.menuhJwzl.Size = new System.Drawing.Size(242, 22);
            this.menuhJwzl.Text = "about \'www.dhamma.org.cn\'";
            this.menuhJwzl.Click += new System.EventHandler(this.menuhJwzl_Click);
            // 
            // menuhHowto
            // 
            this.menuhHowto.Name = "menuhHowto";
            this.menuhHowto.Size = new System.Drawing.Size(242, 22);
            this.menuhHowto.Text = "how to input pali letter";
            this.menuhHowto.Click += new System.EventHandler(this.menuhHowto_Click);
            // 
            // menuhSly
            // 
            this.menuhSly.Name = "menuhSly";
            this.menuhSly.Size = new System.Drawing.Size(242, 22);
            this.menuhSly.Text = "詞典縮略語表";
            this.menuhSly.Click += new System.EventHandler(this.menuhSly_Click);
            // 
            // menuhReadme
            // 
            this.menuhReadme.Name = "menuhReadme";
            this.menuhReadme.Size = new System.Drawing.Size(242, 22);
            this.menuhReadme.Text = "readme";
            this.menuhReadme.Click += new System.EventHandler(this.menuhReadme_Click);
            // 
            // menuhabout
            // 
            this.menuhabout.Name = "menuhabout";
            this.menuhabout.Size = new System.Drawing.Size(242, 22);
            this.menuhabout.Text = "about pced";
            this.menuhabout.Click += new System.EventHandler(this.menuhabout_Click);
            // 
            // menuQuit
            // 
            this.menuQuit.Name = "menuQuit";
            this.menuQuit.Size = new System.Drawing.Size(43, 20);
            this.menuQuit.Text = "Quit";
            this.menuQuit.ToolTipText = "退出程序";
            this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.HideSelection = false;
            this.treeView1.HotTracking = true;
            this.treeView1.Location = new System.Drawing.Point(0, 4);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            treeNode2.Name = "trRoot";
            treeNode2.Text = "词典导航";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(113, 236);
            this.treeView1.TabIndex = 85;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip2;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Pali Dictionary";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stShowToolBar,
            this.stShow,
            this.stQuit});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(167, 70);
            // 
            // stShowToolBar
            // 
            this.stShowToolBar.Name = "stShowToolBar";
            this.stShowToolBar.Size = new System.Drawing.Size(166, 22);
            this.stShowToolBar.Text = "Show the &ToolBar";
            this.stShowToolBar.Click += new System.EventHandler(this.stShowToolBar_Click);
            // 
            // stShow
            // 
            this.stShow.Name = "stShow";
            this.stShow.Size = new System.Drawing.Size(166, 22);
            this.stShow.Text = "&Show window";
            this.stShow.Click += new System.EventHandler(this.stShow_Click);
            // 
            // stQuit
            // 
            this.stQuit.Name = "stQuit";
            this.stQuit.Size = new System.Drawing.Size(166, 22);
            this.stQuit.Text = "&Quit";
            this.stQuit.Click += new System.EventHandler(this.stQuit_Click);
            // 
            // menuosEnglishPali
            // 
            this.menuosEnglishPali.AutoSize = true;
            this.menuosEnglishPali.Location = new System.Drawing.Point(376, 32);
            this.menuosEnglishPali.Name = "menuosEnglishPali";
            this.menuosEnglishPali.Size = new System.Drawing.Size(68, 20);
            this.menuosEnglishPali.TabIndex = 87;
            this.menuosEnglishPali.Text = "en-pali";
            this.menuosEnglishPali.UseVisualStyleBackColor = true;
            this.menuosEnglishPali.CheckedChanged += new System.EventHandler(this.menuosEnglishPali_CheckedChanged);
            this.menuosEnglishPali.MouseHover += new System.EventHandler(this.menuosEnglishPali_MouseHover);
            // 
            // menuosBlurinputmode
            // 
            this.menuosBlurinputmode.AutoSize = true;
            this.menuosBlurinputmode.Location = new System.Drawing.Point(6, 65);
            this.menuosBlurinputmode.Name = "menuosBlurinputmode";
            this.menuosBlurinputmode.Size = new System.Drawing.Size(81, 20);
            this.menuosBlurinputmode.TabIndex = 88;
            this.menuosBlurinputmode.Text = "blur-input";
            this.menuosBlurinputmode.UseVisualStyleBackColor = true;
            this.menuosBlurinputmode.CheckedChanged += new System.EventHandler(this.menuosBlurinputmode_CheckedChanged);
            this.menuosBlurinputmode.MouseHover += new System.EventHandler(this.menuosBlurinputmode_MouseHover);
            // 
            // btnForward
            // 
            this.btnForward.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnForward.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnForward.Image = ((System.Drawing.Image)(resources.GetObject("btnForward.Image")));
            this.btnForward.Location = new System.Drawing.Point(112, 63);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(25, 23);
            this.btnForward.TabIndex = 92;
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBack
            // 
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(86, 63);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(25, 23);
            this.btnBack.TabIndex = 93;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // cboxABC
            // 
            this.cboxABC.AutoSize = true;
            this.cboxABC.Checked = true;
            this.cboxABC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxABC.Location = new System.Drawing.Point(11, 5);
            this.cboxABC.Name = "cboxABC";
            this.cboxABC.Size = new System.Drawing.Size(50, 20);
            this.cboxABC.TabIndex = 95;
            this.cboxABC.Text = "abc";
            this.cboxABC.UseVisualStyleBackColor = true;
            // 
            // btnTj
            // 
            this.btnTj.Location = new System.Drawing.Point(164, 49);
            this.btnTj.Name = "btnTj";
            this.btnTj.Size = new System.Drawing.Size(75, 23);
            this.btnTj.TabIndex = 96;
            this.btnTj.Text = "btnTj";
            this.btnTj.UseVisualStyleBackColor = true;
            this.btnTj.Visible = false;
            this.btnTj.Click += new System.EventHandler(this.btnTj_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(67, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 97;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(95, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 98;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(123, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(28, 23);
            this.button3.TabIndex = 99;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.Control;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Location = new System.Drawing.Point(151, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(28, 23);
            this.button4.TabIndex = 100;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.Control;
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button5.Image = ((System.Drawing.Image)(resources.GetObject("button5.Image")));
            this.button5.Location = new System.Drawing.Point(179, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(28, 23);
            this.button5.TabIndex = 101;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.Control;
            this.button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Image = ((System.Drawing.Image)(resources.GetObject("button6.Image")));
            this.button6.Location = new System.Drawing.Point(207, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(28, 23);
            this.button6.TabIndex = 102;
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.SystemColors.Control;
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
            this.button7.Location = new System.Drawing.Point(235, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(28, 23);
            this.button7.TabIndex = 103;
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.SystemColors.Control;
            this.button8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button8.Image = ((System.Drawing.Image)(resources.GetObject("button8.Image")));
            this.button8.Location = new System.Drawing.Point(263, 3);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(28, 23);
            this.button8.TabIndex = 104;
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.SystemColors.Control;
            this.button9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button9.Image = ((System.Drawing.Image)(resources.GetObject("button9.Image")));
            this.button9.Location = new System.Drawing.Point(291, 3);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(28, 23);
            this.button9.TabIndex = 105;
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.SystemColors.Control;
            this.button10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button10.Image = ((System.Drawing.Image)(resources.GetObject("button10.Image")));
            this.button10.Location = new System.Drawing.Point(319, 3);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(28, 23);
            this.button10.TabIndex = 106;
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.SystemColors.Control;
            this.button11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button11.Enabled = false;
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button11.Image = ((System.Drawing.Image)(resources.GetObject("button11.Image")));
            this.button11.Location = new System.Drawing.Point(347, 3);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(28, 23);
            this.button11.TabIndex = 107;
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.SystemColors.Control;
            this.button12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button12.Enabled = false;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button12.Image = ((System.Drawing.Image)(resources.GetObject("button12.Image")));
            this.button12.Location = new System.Drawing.Point(375, 3);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(28, 23);
            this.button12.TabIndex = 108;
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // lblFhc
            // 
            this.lblFhc.AutoSize = true;
            this.lblFhc.Location = new System.Drawing.Point(6, 6);
            this.lblFhc.Name = "lblFhc";
            this.lblFhc.Size = new System.Drawing.Size(121, 16);
            this.lblFhc.TabIndex = 109;
            this.lblFhc.Text = "complex-word Box:";
            // 
            // gboxFontSet
            // 
            this.gboxFontSet.Controls.Add(this.rbtnVriRomanPali);
            this.gboxFontSet.Controls.Add(this.rbtnSangayana);
            this.gboxFontSet.Controls.Add(this.rbtnTahoma);
            this.gboxFontSet.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gboxFontSet.Location = new System.Drawing.Point(10, 29);
            this.gboxFontSet.Name = "gboxFontSet";
            this.gboxFontSet.Size = new System.Drawing.Size(360, 31);
            this.gboxFontSet.TabIndex = 113;
            this.gboxFontSet.TabStop = false;
            this.gboxFontSet.Text = "input && output font-code setting:";
            // 
            // rbtnVriRomanPali
            // 
            this.rbtnVriRomanPali.AutoSize = true;
            this.rbtnVriRomanPali.BackColor = System.Drawing.Color.Transparent;
            this.rbtnVriRomanPali.Location = new System.Drawing.Point(247, 11);
            this.rbtnVriRomanPali.Name = "rbtnVriRomanPali";
            this.rbtnVriRomanPali.Size = new System.Drawing.Size(109, 20);
            this.rbtnVriRomanPali.TabIndex = 2;
            this.rbtnVriRomanPali.TabStop = true;
            this.rbtnVriRomanPali.Text = "VriRomanPali";
            this.rbtnVriRomanPali.UseVisualStyleBackColor = false;
            this.rbtnVriRomanPali.CheckedChanged += new System.EventHandler(this.rbtnVriRomanPali_CheckedChanged);
            // 
            // rbtnSangayana
            // 
            this.rbtnSangayana.AutoSize = true;
            this.rbtnSangayana.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSangayana.Location = new System.Drawing.Point(145, 11);
            this.rbtnSangayana.Name = "rbtnSangayana";
            this.rbtnSangayana.Size = new System.Drawing.Size(96, 20);
            this.rbtnSangayana.TabIndex = 1;
            this.rbtnSangayana.TabStop = true;
            this.rbtnSangayana.Text = "Sangayana";
            this.rbtnSangayana.UseVisualStyleBackColor = false;
            this.rbtnSangayana.CheckedChanged += new System.EventHandler(this.rbtnSangayana_CheckedChanged);
            // 
            // rbtnTahoma
            // 
            this.rbtnTahoma.AutoSize = true;
            this.rbtnTahoma.BackColor = System.Drawing.Color.Transparent;
            this.rbtnTahoma.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbtnTahoma.Location = new System.Drawing.Point(6, 11);
            this.rbtnTahoma.Name = "rbtnTahoma";
            this.rbtnTahoma.Size = new System.Drawing.Size(133, 20);
            this.rbtnTahoma.TabIndex = 0;
            this.rbtnTahoma.TabStop = true;
            this.rbtnTahoma.Text = "Tahoma(unicode)";
            this.rbtnTahoma.UseVisualStyleBackColor = false;
            this.rbtnTahoma.CheckedChanged += new System.EventHandler(this.rbtnTahoma_CheckedChanged);
            // 
            // panelfhc
            // 
            this.panelfhc.BackColor = System.Drawing.SystemColors.Control;
            this.panelfhc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelfhc.Controls.Add(this.pbtnfhcclose);
            this.panelfhc.Controls.Add(this.textBox3);
            this.panelfhc.Controls.Add(this.lblFhc);
            this.panelfhc.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelfhc.Location = new System.Drawing.Point(0, 250);
            this.panelfhc.Name = "panelfhc";
            this.panelfhc.Size = new System.Drawing.Size(439, 120);
            this.panelfhc.TabIndex = 119;
            // 
            // pbtnfhcclose
            // 
            this.pbtnfhcclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbtnfhcclose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbtnfhcclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.pbtnfhcclose.Location = new System.Drawing.Point(414, 3);
            this.pbtnfhcclose.Name = "pbtnfhcclose";
            this.pbtnfhcclose.Size = new System.Drawing.Size(20, 23);
            this.pbtnfhcclose.TabIndex = 110;
            this.pbtnfhcclose.Text = "X";
            this.pbtnfhcclose.UseVisualStyleBackColor = true;
            this.pbtnfhcclose.Click += new System.EventHandler(this.pbtnfhcclose_Click);
            // 
            // btnshowfhc
            // 
            this.btnshowfhc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnshowfhc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnshowfhc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnshowfhc.Image = ((System.Drawing.Image)(resources.GetObject("btnshowfhc.Image")));
            this.btnshowfhc.Location = new System.Drawing.Point(0, 250);
            this.btnshowfhc.Margin = new System.Windows.Forms.Padding(0);
            this.btnshowfhc.Name = "btnshowfhc";
            this.btnshowfhc.Size = new System.Drawing.Size(11, 120);
            this.btnshowfhc.TabIndex = 120;
            this.btnshowfhc.UseVisualStyleBackColor = true;
            this.btnshowfhc.Click += new System.EventHandler(this.btnshowfhc_Click);
            this.btnshowfhc.MouseHover += new System.EventHandler(this.btnshowfhc_MouseHover);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboxInput);
            this.panel1.Controls.Add(this.menuosEnglishPali);
            this.panel1.Controls.Add(this.btnForward);
            this.panel1.Controls.Add(this.button8);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.gboxFontSet);
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button12);
            this.panel1.Controls.Add(this.btnBack);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.btnLookup);
            this.panel1.Controls.Add(this.button10);
            this.panel1.Controls.Add(this.button11);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.cboxABC);
            this.panel1.Controls.Add(this.menuosBlurinputmode);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 89);
            this.panel1.TabIndex = 121;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 113);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnshowfhc);
            this.splitContainer1.Panel1MinSize = 11;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.panelfhc);
            this.splitContainer1.Size = new System.Drawing.Size(454, 370);
            this.splitContainer1.SplitterDistance = 11;
            this.splitContainer1.TabIndex = 122;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer2.Panel2.Controls.Add(this.btnTj);
            this.splitContainer2.Panel2.Controls.Add(this.webBrowser1);
            this.splitContainer2.Size = new System.Drawing.Size(439, 250);
            this.splitContainer2.SplitterDistance = 117;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
            // 
            // tsmiPaliWinPosition
            // 
            this.tsmiPaliWinPosition.CheckOnClick = true;
            this.tsmiPaliWinPosition.Name = "tsmiPaliWinPosition";
            this.tsmiPaliWinPosition.Size = new System.Drawing.Size(361, 22);
            this.tsmiPaliWinPosition.Text = "读经窗口位置跟随工具条";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(454, 513);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PCED";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.gboxFontSet.ResumeLayout(false);
            this.gboxFontSet.PerformLayout();
            this.panelfhc.ResumeLayout(false);
            this.panelfhc.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toollbl;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAll;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripStatusLabel toollbltimes;
        public System.Windows.Forms.ComboBox cboxInput;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menupalimulu;
        private System.Windows.Forms.ToolStripMenuItem menuLanguageSet;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        public System.Windows.Forms.ToolStripMenuItem FAN;
        private System.Windows.Forms.ToolStripMenuItem HAN;
        private System.Windows.Forms.ToolStripMenuItem EN;
        private System.Windows.Forms.ToolStripMenuItem menuhFofa;
        private System.Windows.Forms.ToolStripMenuItem menuhJwzl;
        private System.Windows.Forms.ToolStripMenuItem menuhHowto;
        private System.Windows.Forms.ToolStripMenuItem menuhReadme;
        private System.Windows.Forms.ToolStripMenuItem menuOtherset;
        private System.Windows.Forms.ToolStripMenuItem menuosSetWindowInFront;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem menuhSly;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.CheckBox menuosEnglishPali;
        private System.Windows.Forms.CheckBox menuosBlurinputmode;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.CheckBox cboxABC;
        private System.Windows.Forms.Button btnTj;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Label lblFhc;
        private System.Windows.Forms.GroupBox gboxFontSet;
        public System.Windows.Forms.RadioButton rbtnVriRomanPali;
        public System.Windows.Forms.RadioButton rbtnSangayana;
        private System.Windows.Forms.RadioButton rbtnTahoma;
        private System.Windows.Forms.ToolStripMenuItem menuQuit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem stQuit;
        private System.Windows.Forms.ToolStripMenuItem stShow;
        private System.Windows.Forms.ToolStripMenuItem menuosCopyAutoCc;
        private System.Windows.Forms.Panel panelfhc;
        private System.Windows.Forms.Button pbtnfhcclose;
        private System.Windows.Forms.ToolStripMenuItem menuhabout;
        private System.Windows.Forms.Button btnshowfhc;
        private System.Windows.Forms.ToolStripMenuItem stShowToolBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem cms3selectAll;
        private System.Windows.Forms.ToolStripMenuItem cms3copy;
        private System.Windows.Forms.ToolStripMenuItem cms3paste;
        private System.Windows.Forms.ToolStripMenuItem cms3cut;
        private System.Windows.Forms.ToolStripMenuItem menuosZwfcCx;
        private System.Windows.Forms.ToolStripMenuItem menuosFontSize;
        public System.Windows.Forms.ToolStripMenuItem fsoriginal;
        private System.Windows.Forms.ToolStripMenuItem fsplus;
        private System.Windows.Forms.ToolStripMenuItem fsplustwo;
        private System.Windows.Forms.ToolStripMenuItem fsplusthree;
        private System.Windows.Forms.ToolStripMenuItem lookupsctword;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.ToolStripMenuItem tsmiPaliWinPosition;
    }
}

