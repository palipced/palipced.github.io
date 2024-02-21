namespace pced
{
    partial class frmCk
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
            this.lvCdxx = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.button1 = new System.Windows.Forms.Button();
            this.tboxCdlb = new System.Windows.Forms.TextBox();
            this.tboxCdbz = new System.Windows.Forms.TextBox();
            this.tboxCdName = new System.Windows.Forms.TextBox();
            this.tboxCdshm = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvCdxx
            // 
            this.lvCdxx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvCdxx.FullRowSelect = true;
            this.lvCdxx.HideSelection = false;
            this.lvCdxx.Location = new System.Drawing.Point(12, 12);
            this.lvCdxx.Name = "lvCdxx";
            this.lvCdxx.Size = new System.Drawing.Size(638, 266);
            this.lvCdxx.TabIndex = 0;
            this.lvCdxx.UseCompatibleStateImageBehavior = false;
            this.lvCdxx.View = System.Windows.Forms.View.Details;
            this.lvCdxx.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCdxx_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "词典类别";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "词典标识";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "词典名称";
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "词典说明";
            this.columnHeader4.Width = 300;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "词汇数量";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(385, 286);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "修改所选词典信息";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tboxCdlb
            // 
            this.tboxCdlb.Location = new System.Drawing.Point(12, 286);
            this.tboxCdlb.Name = "tboxCdlb";
            this.tboxCdlb.Size = new System.Drawing.Size(250, 21);
            this.tboxCdlb.TabIndex = 2;
            // 
            // tboxCdbz
            // 
            this.tboxCdbz.Location = new System.Drawing.Point(12, 313);
            this.tboxCdbz.Name = "tboxCdbz";
            this.tboxCdbz.Size = new System.Drawing.Size(250, 21);
            this.tboxCdbz.TabIndex = 3;
            // 
            // tboxCdName
            // 
            this.tboxCdName.Location = new System.Drawing.Point(12, 340);
            this.tboxCdName.Name = "tboxCdName";
            this.tboxCdName.Size = new System.Drawing.Size(250, 21);
            this.tboxCdName.TabIndex = 4;
            // 
            // tboxCdshm
            // 
            this.tboxCdshm.Location = new System.Drawing.Point(12, 367);
            this.tboxCdshm.Name = "tboxCdshm";
            this.tboxCdshm.Size = new System.Drawing.Size(250, 21);
            this.tboxCdshm.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(385, 315);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "从词库中导出所选词典";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(335, 344);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(260, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "导出（从中删除所选词典）为管理用词库";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmCk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 453);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tboxCdshm);
            this.Controls.Add(this.tboxCdName);
            this.Controls.Add(this.tboxCdbz);
            this.Controls.Add(this.tboxCdlb);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lvCdxx);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmCk";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "词库管理";
            this.Shown += new System.EventHandler(this.frmCk_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvCdxx;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TextBox tboxCdlb;
        private System.Windows.Forms.TextBox tboxCdbz;
        private System.Windows.Forms.TextBox tboxCdName;
        private System.Windows.Forms.TextBox tboxCdshm;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}