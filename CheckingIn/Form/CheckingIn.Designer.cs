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
            this.原始表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oa表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除所有数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除OA数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工作日设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.listView_log = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "表格文件2003|*.xls|表格文件2010|*.xlsx";
            this.openFileDialog1.Multiselect = true;
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
            this.menuStrip1.Size = new System.Drawing.Size(454, 25);
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
            this.增加ToolStripMenuItem});
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
            this.删除所有数据ToolStripMenuItem,
            this.删除OA数据ToolStripMenuItem});
            this.数据库ToolStripMenuItem.Name = "数据库ToolStripMenuItem";
            this.数据库ToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.数据库ToolStripMenuItem.Text = "数据库";
            // 
            // 删除所有数据ToolStripMenuItem
            // 
            this.删除所有数据ToolStripMenuItem.Name = "删除所有数据ToolStripMenuItem";
            this.删除所有数据ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.删除所有数据ToolStripMenuItem.Text = "删除原始数据";
            this.删除所有数据ToolStripMenuItem.Click += new System.EventHandler(this.删除考勤数据ToolStripMenuItem_Click);
            // 
            // 删除OA数据ToolStripMenuItem
            // 
            this.删除OA数据ToolStripMenuItem.Name = "删除OA数据ToolStripMenuItem";
            this.删除OA数据ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.删除OA数据ToolStripMenuItem.Text = "清空OA数据";
            this.删除OA数据ToolStripMenuItem.Click += new System.EventHandler(this.删除OA数据ToolStripMenuItem_Click_1);
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
            this.toolStrip1.Size = new System.Drawing.Size(454, 25);
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
            // listView_log
            // 
            this.listView_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader8});
            this.listView_log.FullRowSelect = true;
            this.listView_log.GridLines = true;
            this.listView_log.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_log.HideSelection = false;
            this.listView_log.Location = new System.Drawing.Point(12, 28);
            this.listView_log.MultiSelect = false;
            this.listView_log.Name = "listView_log";
            this.listView_log.Size = new System.Drawing.Size(436, 663);
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
            this.columnHeader8.Width = 297;
            // 
            // CheckingIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 719);
            this.Controls.Add(this.listView_log);
            this.Controls.Add(this.toolStrip1);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 输出文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripMenuItem oA数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 增加ToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 个人信息表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oa表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除所有数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除OA数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem 工作日设置ToolStripMenuItem;
    }
}

