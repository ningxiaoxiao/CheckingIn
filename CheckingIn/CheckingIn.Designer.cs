namespace CheckingIn
{
    partial class CheckingIn
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.label2 = new System.Windows.Forms.Label();
            this.listView_warn = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.读取班次表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取邮箱表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.输出文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oA数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.增加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readoafileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.邮箱ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向全体发送ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向当前用户发送ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加班ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出差ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.外出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.补登ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.个人信息表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.结果表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.原始表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oa表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除所有数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清空OA数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工作日设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.listView_log = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.oa_dataGridView2 = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oa_dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "表格文件2003|*.xls|表格文件2010|*.xlsx";
            this.openFileDialog1.Multiselect = true;
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(14, 28);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(77, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.BackColor = System.Drawing.Color.Salmon;
            this.monthCalendar1.ForeColor = System.Drawing.Color.Red;
            this.monthCalendar1.Location = new System.Drawing.Point(213, 28);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.ShowTodayCircle = false;
            this.monthCalendar1.TabIndex = 4;
            this.monthCalendar1.TitleForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.monthCalendar1.TodayDate = new System.DateTime(2016, 7, 5, 0, 0, 0, 0);
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(445, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "原始数据";
            // 
            // listView_warn
            // 
            this.listView_warn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10});
            this.listView_warn.FullRowSelect = true;
            this.listView_warn.GridLines = true;
            this.listView_warn.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_warn.HideSelection = false;
            this.listView_warn.Location = new System.Drawing.Point(520, 323);
            this.listView_warn.MultiSelect = false;
            this.listView_warn.Name = "listView_warn";
            this.listView_warn.Size = new System.Drawing.Size(521, 368);
            this.listView_warn.TabIndex = 12;
            this.listView_warn.UseCompatibleStateImageBehavior = false;
            this.listView_warn.View = System.Windows.Forms.View.Details;
            this.listView_warn.SelectedIndexChanged += new System.EventHandler(this.listView_warn_SelectedIndexChanged);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "date";
            this.columnHeader9.Width = 70;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "txt";
            this.columnHeader10.Width = 165;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.oA数据ToolStripMenuItem,
            this.邮箱ToolStripMenuItem,
            this.查看ToolStripMenuItem,
            this.数据库ToolStripMenuItem,
            this.工作日设置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1051, 25);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开文件ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.读取班次表ToolStripMenuItem,
            this.读取邮箱表ToolStripMenuItem,
            this.toolStripMenuItem3,
            this.输出文件ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开文件ToolStripMenuItem
            // 
            this.打开文件ToolStripMenuItem.Name = "打开文件ToolStripMenuItem";
            this.打开文件ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.打开文件ToolStripMenuItem.Text = "打开考勤器文件";
            this.打开文件ToolStripMenuItem.Click += new System.EventHandler(this.打开考勤器文件ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(157, 6);
            // 
            // 读取班次表ToolStripMenuItem
            // 
            this.读取班次表ToolStripMenuItem.Name = "读取班次表ToolStripMenuItem";
            this.读取班次表ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.读取班次表ToolStripMenuItem.Text = "读取班次表";
            this.读取班次表ToolStripMenuItem.Click += new System.EventHandler(this.读取班次表ToolStripMenuItem_Click);
            // 
            // 读取邮箱表ToolStripMenuItem
            // 
            this.读取邮箱表ToolStripMenuItem.Name = "读取邮箱表ToolStripMenuItem";
            this.读取邮箱表ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.读取邮箱表ToolStripMenuItem.Text = "读取邮箱表";
            this.读取邮箱表ToolStripMenuItem.Click += new System.EventHandler(this.读取邮箱表ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(157, 6);
            // 
            // 输出文件ToolStripMenuItem
            // 
            this.输出文件ToolStripMenuItem.Name = "输出文件ToolStripMenuItem";
            this.输出文件ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.输出文件ToolStripMenuItem.Text = "输出文件";
            this.输出文件ToolStripMenuItem.Click += new System.EventHandler(this.输出文件ToolStripMenuItem_Click);
            // 
            // oA数据ToolStripMenuItem
            // 
            this.oA数据ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.增加ToolStripMenuItem,
            this.readoafileToolStripMenuItem});
            this.oA数据ToolStripMenuItem.Name = "oA数据ToolStripMenuItem";
            this.oA数据ToolStripMenuItem.Size = new System.Drawing.Size(62, 21);
            this.oA数据ToolStripMenuItem.Text = "OA数据";
            // 
            // 增加ToolStripMenuItem
            // 
            this.增加ToolStripMenuItem.Name = "增加ToolStripMenuItem";
            this.增加ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.增加ToolStripMenuItem.Text = "增加";
            this.增加ToolStripMenuItem.Click += new System.EventHandler(this.增加ToolStripMenuItem_Click);
            // 
            // readoafileToolStripMenuItem
            // 
            this.readoafileToolStripMenuItem.Name = "readoafileToolStripMenuItem";
            this.readoafileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.readoafileToolStripMenuItem.Text = "读取OA文件";
            this.readoafileToolStripMenuItem.Click += new System.EventHandler(this.readoafileToolStripMenuItem_Click);
            // 
            // 邮箱ToolStripMenuItem
            // 
            this.邮箱ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.向全体发送ToolStripMenuItem,
            this.向当前用户发送ToolStripMenuItem});
            this.邮箱ToolStripMenuItem.Name = "邮箱ToolStripMenuItem";
            this.邮箱ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.邮箱ToolStripMenuItem.Text = "邮箱";
            // 
            // 向全体发送ToolStripMenuItem
            // 
            this.向全体发送ToolStripMenuItem.Name = "向全体发送ToolStripMenuItem";
            this.向全体发送ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.向全体发送ToolStripMenuItem.Text = "向全体发送";
            this.向全体发送ToolStripMenuItem.Click += new System.EventHandler(this.向全体发送ToolStripMenuItem_Click);
            // 
            // 向当前用户发送ToolStripMenuItem
            // 
            this.向当前用户发送ToolStripMenuItem.Name = "向当前用户发送ToolStripMenuItem";
            this.向当前用户发送ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.向当前用户发送ToolStripMenuItem.Text = "向当前用户发送";
            this.向当前用户发送ToolStripMenuItem.Click += new System.EventHandler(this.向当前用户发送ToolStripMenuItem_Click);
            // 
            // 查看ToolStripMenuItem
            // 
            this.查看ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.加班ToolStripMenuItem,
            this.出差ToolStripMenuItem,
            this.外出ToolStripMenuItem,
            this.补登ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.个人信息表ToolStripMenuItem,
            this.结果表ToolStripMenuItem,
            this.原始表ToolStripMenuItem,
            this.oa表ToolStripMenuItem});
            this.查看ToolStripMenuItem.Name = "查看ToolStripMenuItem";
            this.查看ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.查看ToolStripMenuItem.Text = "查看";
            // 
            // 加班ToolStripMenuItem
            // 
            this.加班ToolStripMenuItem.Name = "加班ToolStripMenuItem";
            this.加班ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.加班ToolStripMenuItem.Text = "加班";
            this.加班ToolStripMenuItem.Click += new System.EventHandler(this.加班ToolStripMenuItem_Click);
            // 
            // 出差ToolStripMenuItem
            // 
            this.出差ToolStripMenuItem.Name = "出差ToolStripMenuItem";
            this.出差ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.出差ToolStripMenuItem.Text = "出差";
            this.出差ToolStripMenuItem.Click += new System.EventHandler(this.出差ToolStripMenuItem_Click);
            // 
            // 外出ToolStripMenuItem
            // 
            this.外出ToolStripMenuItem.Name = "外出ToolStripMenuItem";
            this.外出ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.外出ToolStripMenuItem.Text = "外出";
            this.外出ToolStripMenuItem.Click += new System.EventHandler(this.外出ToolStripMenuItem_Click);
            // 
            // 补登ToolStripMenuItem
            // 
            this.补登ToolStripMenuItem.Name = "补登ToolStripMenuItem";
            this.补登ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.补登ToolStripMenuItem.Text = "补登";
            this.补登ToolStripMenuItem.Click += new System.EventHandler(this.补登ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            // 
            // 个人信息表ToolStripMenuItem
            // 
            this.个人信息表ToolStripMenuItem.Name = "个人信息表ToolStripMenuItem";
            this.个人信息表ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.个人信息表ToolStripMenuItem.Text = "个人信息表";
            this.个人信息表ToolStripMenuItem.Click += new System.EventHandler(this.个人信息表ToolStripMenuItem_Click);
            // 
            // 结果表ToolStripMenuItem
            // 
            this.结果表ToolStripMenuItem.Name = "结果表ToolStripMenuItem";
            this.结果表ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.结果表ToolStripMenuItem.Text = "oa结果表";
            this.结果表ToolStripMenuItem.Click += new System.EventHandler(this.结果表ToolStripMenuItem_Click);
            // 
            // 原始表ToolStripMenuItem
            // 
            this.原始表ToolStripMenuItem.Name = "原始表ToolStripMenuItem";
            this.原始表ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.原始表ToolStripMenuItem.Text = "考勤器原始表";
            this.原始表ToolStripMenuItem.Click += new System.EventHandler(this.原始表ToolStripMenuItem_Click);
            // 
            // oa表ToolStripMenuItem
            // 
            this.oa表ToolStripMenuItem.Name = "oa表ToolStripMenuItem";
            this.oa表ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.oa表ToolStripMenuItem.Text = "oa原始表";
            this.oa表ToolStripMenuItem.Click += new System.EventHandler(this.oa表ToolStripMenuItem_Click);
            // 
            // 数据库ToolStripMenuItem
            // 
            this.数据库ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除文件ToolStripMenuItem,
            this.删除所有数据ToolStripMenuItem,
            this.清空OA数据ToolStripMenuItem});
            this.数据库ToolStripMenuItem.Name = "数据库ToolStripMenuItem";
            this.数据库ToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.数据库ToolStripMenuItem.Text = "数据库";
            // 
            // 删除文件ToolStripMenuItem
            // 
            this.删除文件ToolStripMenuItem.Name = "删除文件ToolStripMenuItem";
            this.删除文件ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.删除文件ToolStripMenuItem.Text = "删除文件";
            this.删除文件ToolStripMenuItem.Click += new System.EventHandler(this.删除文件ToolStripMenuItem_Click);
            // 
            // 删除所有数据ToolStripMenuItem
            // 
            this.删除所有数据ToolStripMenuItem.Name = "删除所有数据ToolStripMenuItem";
            this.删除所有数据ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.删除所有数据ToolStripMenuItem.Text = "删除考勤原始数据";
            this.删除所有数据ToolStripMenuItem.Click += new System.EventHandler(this.删除考勤原始数据ToolStripMenuItem_Click);
            // 
            // 清空OA数据ToolStripMenuItem
            // 
            this.清空OA数据ToolStripMenuItem.Name = "清空OA数据ToolStripMenuItem";
            this.清空OA数据ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.清空OA数据ToolStripMenuItem.Text = "清空OA数据";
            this.清空OA数据ToolStripMenuItem.Click += new System.EventHandler(this.清空OA数据ToolStripMenuItem_Click_1);
            // 
            // 工作日设置ToolStripMenuItem
            // 
            this.工作日设置ToolStripMenuItem.Name = "工作日设置ToolStripMenuItem";
            this.工作日设置ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.工作日设置ToolStripMenuItem.Text = "工作日设置";
            this.工作日设置ToolStripMenuItem.Click += new System.EventHandler(this.工作日设置ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripLabel1,
            this.toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 694);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1051, 25);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabel1.Text = "消息:";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(96, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(214, 214);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 60);
            this.label6.TabIndex = 20;
            this.label6.Text = "日期\r\n上班\r\n下班\r\n工作时间\r\n分析结果";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 60);
            this.label3.TabIndex = 21;
            this.label3.Text = "出勤/应出勤\r\n工作时间\r\n迟到\r\n出差(天)\r\n加班";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(518, 308);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 22;
            this.label1.Text = "警告事件";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(271, 214);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 60);
            this.label4.TabIndex = 23;
            this.label4.Text = "日期\r\n上班\r\n下班\r\n工作时间\r\n分析结果";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(118, 63);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 60);
            this.label5.TabIndex = 24;
            this.label5.Text = "出勤/应出勤\r\n工作时间\r\n迟到\r\n出差(天)\r\n加班";
            // 
            // listView_log
            // 
            this.listView_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader8});
            this.listView_log.FullRowSelect = true;
            this.listView_log.GridLines = true;
            this.listView_log.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_log.HideSelection = false;
            this.listView_log.Location = new System.Drawing.Point(12, 308);
            this.listView_log.MultiSelect = false;
            this.listView_log.Name = "listView_log";
            this.listView_log.Size = new System.Drawing.Size(496, 383);
            this.listView_log.TabIndex = 25;
            this.listView_log.UseCompatibleStateImageBehavior = false;
            this.listView_log.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "type";
            this.columnHeader1.Width = 72;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "text";
            this.columnHeader8.Width = 848;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 136);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 26;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(445, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(596, 105);
            this.dataGridView1.TabIndex = 27;
            // 
            // oa_dataGridView2
            // 
            this.oa_dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.oa_dataGridView2.Location = new System.Drawing.Point(445, 180);
            this.oa_dataGridView2.Name = "oa_dataGridView2";
            this.oa_dataGridView2.RowTemplate.Height = 23;
            this.oa_dataGridView2.Size = new System.Drawing.Size(596, 118);
            this.oa_dataGridView2.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(445, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 29;
            this.label7.Text = "OA数据";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(97, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 30;
            this.button2.Text = "WEB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // CheckingIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 719);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.oa_dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView_log);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.listView_warn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "CheckingIn";
            this.Text = "考勤数据处理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CheckingIn_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oa_dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listView_warn;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 输出文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem oA数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 增加ToolStripMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem 读取班次表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读取邮箱表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 邮箱ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 向全体发送ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 向当前用户发送ToolStripMenuItem;
        private System.Windows.Forms.ListView listView_log;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ToolStripMenuItem 查看ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加班ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 出差ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 外出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 补登ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 个人信息表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 结果表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oa表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除文件ToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView oa_dataGridView2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem readoafileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除所有数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清空OA数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem 工作日设置ToolStripMenuItem;
    }
}

