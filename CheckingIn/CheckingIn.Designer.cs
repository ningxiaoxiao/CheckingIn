﻿namespace CheckingIn
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
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.listView_warn = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.输出文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取班次表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取邮箱表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oA数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.增加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清空OA数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取加班文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取外出文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取出差文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.读取补登文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.邮箱ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向全体发送ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向当前用户发送ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.班次ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加班ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出差ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.外出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.补登ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.邮箱ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.个人信息表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.结果表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.原始表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oa表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.警告表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "表格文件|*.xls";
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(21, 42);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(114, 26);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.BackColor = System.Drawing.Color.Salmon;
            this.monthCalendar1.ForeColor = System.Drawing.Color.Red;
            this.monthCalendar1.Location = new System.Drawing.Point(381, 48);
            this.monthCalendar1.Margin = new System.Windows.Forms.Padding(14);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.ShowTodayCircle = false;
            this.monthCalendar1.TabIndex = 4;
            this.monthCalendar1.TitleForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.monthCalendar1.TodayDate = new System.DateTime(2016, 7, 5, 0, 0, 0, 0);
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader7,
            this.columnHeader6});
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(18, 477);
            this.listView2.Margin = new System.Windows.Forms.Padding(4);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(691, 158);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.VirtualMode = true;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "name";
            this.columnHeader5.Width = 65;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "date";
            this.columnHeader7.Width = 70;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "time";
            this.columnHeader6.Width = 70;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 452);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 18);
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
            this.listView_warn.Location = new System.Drawing.Point(18, 219);
            this.listView_warn.Margin = new System.Windows.Forms.Padding(4);
            this.listView_warn.MultiSelect = false;
            this.listView_warn.Name = "listView_warn";
            this.listView_warn.Size = new System.Drawing.Size(356, 216);
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
            this.数据库ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(729, 34);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开文件ToolStripMenuItem,
            this.输出文件ToolStripMenuItem,
            this.读取班次表ToolStripMenuItem,
            this.读取邮箱表ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开文件ToolStripMenuItem
            // 
            this.打开文件ToolStripMenuItem.Name = "打开文件ToolStripMenuItem";
            this.打开文件ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.打开文件ToolStripMenuItem.Text = "打开文件";
            this.打开文件ToolStripMenuItem.Click += new System.EventHandler(this.打开文件ToolStripMenuItem_Click);
            // 
            // 输出文件ToolStripMenuItem
            // 
            this.输出文件ToolStripMenuItem.Name = "输出文件ToolStripMenuItem";
            this.输出文件ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.输出文件ToolStripMenuItem.Text = "输出文件";
            this.输出文件ToolStripMenuItem.Click += new System.EventHandler(this.输出文件ToolStripMenuItem_Click);
            // 
            // 读取班次表ToolStripMenuItem
            // 
            this.读取班次表ToolStripMenuItem.Name = "读取班次表ToolStripMenuItem";
            this.读取班次表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.读取班次表ToolStripMenuItem.Text = "读取班次表";
            this.读取班次表ToolStripMenuItem.Click += new System.EventHandler(this.读取班次表ToolStripMenuItem_Click);
            // 
            // 读取邮箱表ToolStripMenuItem
            // 
            this.读取邮箱表ToolStripMenuItem.Name = "读取邮箱表ToolStripMenuItem";
            this.读取邮箱表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.读取邮箱表ToolStripMenuItem.Text = "读取邮箱表";
            this.读取邮箱表ToolStripMenuItem.Click += new System.EventHandler(this.读取邮箱表ToolStripMenuItem_Click);
            // 
            // oA数据ToolStripMenuItem
            // 
            this.oA数据ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.增加ToolStripMenuItem,
            this.清空OA数据ToolStripMenuItem,
            this.读取加班文件ToolStripMenuItem,
            this.读取外出文件ToolStripMenuItem,
            this.读取出差文件ToolStripMenuItem,
            this.读取补登文件ToolStripMenuItem});
            this.oA数据ToolStripMenuItem.Name = "oA数据ToolStripMenuItem";
            this.oA数据ToolStripMenuItem.Size = new System.Drawing.Size(86, 28);
            this.oA数据ToolStripMenuItem.Text = "OA数据";
            // 
            // 增加ToolStripMenuItem
            // 
            this.增加ToolStripMenuItem.Name = "增加ToolStripMenuItem";
            this.增加ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.增加ToolStripMenuItem.Text = "增加";
            this.增加ToolStripMenuItem.Click += new System.EventHandler(this.增加ToolStripMenuItem_Click);
            // 
            // 清空OA数据ToolStripMenuItem
            // 
            this.清空OA数据ToolStripMenuItem.Name = "清空OA数据ToolStripMenuItem";
            this.清空OA数据ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.清空OA数据ToolStripMenuItem.Text = "清空OA数据";
            this.清空OA数据ToolStripMenuItem.Click += new System.EventHandler(this.清空OA数据ToolStripMenuItem_Click);
            // 
            // 读取加班文件ToolStripMenuItem
            // 
            this.读取加班文件ToolStripMenuItem.Name = "读取加班文件ToolStripMenuItem";
            this.读取加班文件ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.读取加班文件ToolStripMenuItem.Text = "读取加班文件";
            this.读取加班文件ToolStripMenuItem.Click += new System.EventHandler(this.读取加班文件ToolStripMenuItem_Click);
            // 
            // 读取外出文件ToolStripMenuItem
            // 
            this.读取外出文件ToolStripMenuItem.Name = "读取外出文件ToolStripMenuItem";
            this.读取外出文件ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.读取外出文件ToolStripMenuItem.Text = "读取外出文件";
            this.读取外出文件ToolStripMenuItem.Click += new System.EventHandler(this.读取外出文件ToolStripMenuItem_Click);
            // 
            // 读取出差文件ToolStripMenuItem
            // 
            this.读取出差文件ToolStripMenuItem.Name = "读取出差文件ToolStripMenuItem";
            this.读取出差文件ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.读取出差文件ToolStripMenuItem.Text = "读取出差文件";
            this.读取出差文件ToolStripMenuItem.Click += new System.EventHandler(this.读取出差文件ToolStripMenuItem_Click);
            // 
            // 读取补登文件ToolStripMenuItem
            // 
            this.读取补登文件ToolStripMenuItem.Name = "读取补登文件ToolStripMenuItem";
            this.读取补登文件ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.读取补登文件ToolStripMenuItem.Text = "读取补登文件";
            this.读取补登文件ToolStripMenuItem.Click += new System.EventHandler(this.读取补登文件ToolStripMenuItem_Click);
            // 
            // 邮箱ToolStripMenuItem
            // 
            this.邮箱ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.向全体发送ToolStripMenuItem,
            this.向当前用户发送ToolStripMenuItem});
            this.邮箱ToolStripMenuItem.Name = "邮箱ToolStripMenuItem";
            this.邮箱ToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.邮箱ToolStripMenuItem.Text = "邮箱";
            // 
            // 向全体发送ToolStripMenuItem
            // 
            this.向全体发送ToolStripMenuItem.Name = "向全体发送ToolStripMenuItem";
            this.向全体发送ToolStripMenuItem.Size = new System.Drawing.Size(219, 30);
            this.向全体发送ToolStripMenuItem.Text = "向全体发送";
            this.向全体发送ToolStripMenuItem.Click += new System.EventHandler(this.向全体发送ToolStripMenuItem_Click);
            // 
            // 向当前用户发送ToolStripMenuItem
            // 
            this.向当前用户发送ToolStripMenuItem.Name = "向当前用户发送ToolStripMenuItem";
            this.向当前用户发送ToolStripMenuItem.Size = new System.Drawing.Size(219, 30);
            this.向当前用户发送ToolStripMenuItem.Text = "向当前用户发送";
            this.向当前用户发送ToolStripMenuItem.Click += new System.EventHandler(this.向当前用户发送ToolStripMenuItem_Click);
            // 
            // 查看ToolStripMenuItem
            // 
            this.查看ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.班次ToolStripMenuItem,
            this.加班ToolStripMenuItem,
            this.出差ToolStripMenuItem,
            this.外出ToolStripMenuItem,
            this.补登ToolStripMenuItem,
            this.邮箱ToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.个人信息表ToolStripMenuItem,
            this.结果表ToolStripMenuItem,
            this.原始表ToolStripMenuItem,
            this.oa表ToolStripMenuItem,
            this.警告表ToolStripMenuItem});
            this.查看ToolStripMenuItem.Name = "查看ToolStripMenuItem";
            this.查看ToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.查看ToolStripMenuItem.Text = "查看";
            // 
            // 班次ToolStripMenuItem
            // 
            this.班次ToolStripMenuItem.Name = "班次ToolStripMenuItem";
            this.班次ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.班次ToolStripMenuItem.Text = "班次";
            this.班次ToolStripMenuItem.Click += new System.EventHandler(this.班次ToolStripMenuItem_Click);
            // 
            // 加班ToolStripMenuItem
            // 
            this.加班ToolStripMenuItem.Name = "加班ToolStripMenuItem";
            this.加班ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.加班ToolStripMenuItem.Text = "加班";
            this.加班ToolStripMenuItem.Click += new System.EventHandler(this.加班ToolStripMenuItem_Click);
            // 
            // 出差ToolStripMenuItem
            // 
            this.出差ToolStripMenuItem.Name = "出差ToolStripMenuItem";
            this.出差ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.出差ToolStripMenuItem.Text = "出差";
            this.出差ToolStripMenuItem.Click += new System.EventHandler(this.出差ToolStripMenuItem_Click);
            // 
            // 外出ToolStripMenuItem
            // 
            this.外出ToolStripMenuItem.Name = "外出ToolStripMenuItem";
            this.外出ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.外出ToolStripMenuItem.Text = "外出";
            this.外出ToolStripMenuItem.Click += new System.EventHandler(this.外出ToolStripMenuItem_Click);
            // 
            // 补登ToolStripMenuItem
            // 
            this.补登ToolStripMenuItem.Name = "补登ToolStripMenuItem";
            this.补登ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.补登ToolStripMenuItem.Text = "补登";
            this.补登ToolStripMenuItem.Click += new System.EventHandler(this.补登ToolStripMenuItem_Click);
            // 
            // 邮箱ToolStripMenuItem1
            // 
            this.邮箱ToolStripMenuItem1.Name = "邮箱ToolStripMenuItem1";
            this.邮箱ToolStripMenuItem1.Size = new System.Drawing.Size(183, 30);
            this.邮箱ToolStripMenuItem1.Text = "邮箱";
            this.邮箱ToolStripMenuItem1.Click += new System.EventHandler(this.邮箱ToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 6);
            // 
            // 个人信息表ToolStripMenuItem
            // 
            this.个人信息表ToolStripMenuItem.Name = "个人信息表ToolStripMenuItem";
            this.个人信息表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.个人信息表ToolStripMenuItem.Text = "个人信息表";
            this.个人信息表ToolStripMenuItem.Click += new System.EventHandler(this.个人信息表ToolStripMenuItem_Click);
            // 
            // 结果表ToolStripMenuItem
            // 
            this.结果表ToolStripMenuItem.Name = "结果表ToolStripMenuItem";
            this.结果表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.结果表ToolStripMenuItem.Text = "结果表";
            this.结果表ToolStripMenuItem.Click += new System.EventHandler(this.结果表ToolStripMenuItem_Click);
            // 
            // 原始表ToolStripMenuItem
            // 
            this.原始表ToolStripMenuItem.Name = "原始表ToolStripMenuItem";
            this.原始表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.原始表ToolStripMenuItem.Text = "原始表";
            this.原始表ToolStripMenuItem.Click += new System.EventHandler(this.原始表ToolStripMenuItem_Click);
            // 
            // oa表ToolStripMenuItem
            // 
            this.oa表ToolStripMenuItem.Name = "oa表ToolStripMenuItem";
            this.oa表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.oa表ToolStripMenuItem.Text = "oa表";
            this.oa表ToolStripMenuItem.Click += new System.EventHandler(this.oa表ToolStripMenuItem_Click);
            // 
            // 警告表ToolStripMenuItem
            // 
            this.警告表ToolStripMenuItem.Name = "警告表ToolStripMenuItem";
            this.警告表ToolStripMenuItem.Size = new System.Drawing.Size(183, 30);
            this.警告表ToolStripMenuItem.Text = "警告表";
            this.警告表ToolStripMenuItem.Click += new System.EventHandler(this.警告表ToolStripMenuItem_Click);
            // 
            // 数据库ToolStripMenuItem
            // 
            this.数据库ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除文件ToolStripMenuItem});
            this.数据库ToolStripMenuItem.Name = "数据库ToolStripMenuItem";
            this.数据库ToolStripMenuItem.Size = new System.Drawing.Size(76, 28);
            this.数据库ToolStripMenuItem.Text = "数据库";
            // 
            // 删除文件ToolStripMenuItem
            // 
            this.删除文件ToolStripMenuItem.Name = "删除文件ToolStripMenuItem";
            this.删除文件ToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.删除文件ToolStripMenuItem.Text = "删除文件";
            this.删除文件ToolStripMenuItem.Click += new System.EventHandler(this.删除文件ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripLabel1,
            this.toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 840);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(729, 36);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 33);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(50, 33);
            this.toolStripLabel1.Text = "消息:";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(142, 33);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(466, 327);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 90);
            this.label6.TabIndex = 20;
            this.label6.Text = "日期\r\n上班\r\n下班\r\n工作时间\r\n分析结果";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 90);
            this.label3.TabIndex = 21;
            this.label3.Text = "出勤/应出勤\r\n工作时间\r\n迟到\r\n出差(天)\r\n加班";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 196);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 22;
            this.label1.Text = "警告事件";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(552, 327);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 90);
            this.label4.TabIndex = 23;
            this.label4.Text = "日期\r\n上班\r\n下班\r\n工作时间\r\n分析结果";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(177, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 90);
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
            this.listView_log.Location = new System.Drawing.Point(18, 660);
            this.listView_log.Margin = new System.Windows.Forms.Padding(4);
            this.listView_log.MultiSelect = false;
            this.listView_log.Name = "listView_log";
            this.listView_log.Size = new System.Drawing.Size(691, 145);
            this.listView_log.TabIndex = 25;
            this.listView_log.UseCompatibleStateImageBehavior = false;
            this.listView_log.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "type";
            this.columnHeader1.Width = 44;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "text";
            this.columnHeader8.Width = 395;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(598, 435);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 34);
            this.button1.TabIndex = 26;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CheckingIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 876);
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
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "CheckingIn";
            this.Text = "考勤数据处理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CheckingIn_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
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
        private System.Windows.Forms.ToolStripMenuItem 读取加班文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读取外出文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读取出差文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读取补登文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 班次ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加班ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 出差ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 外出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 补登ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 邮箱ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 清空OA数据ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 个人信息表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 结果表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oa表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 警告表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除文件ToolStripMenuItem;
    }
}

