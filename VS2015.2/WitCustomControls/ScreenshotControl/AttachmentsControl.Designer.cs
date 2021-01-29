namespace CodePlex.WitCustomControls.Screenshot
{
    partial class AttachmentsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.CaptureButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.AttachmentsListView = new System.Windows.Forms.ListView();
            this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SizeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.CommentsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.DefaultContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.DefaultContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DeleteButton);
            this.panel1.Controls.Add(this.CaptureButton);
            this.panel1.Controls.Add(this.AddButton);
            this.panel1.Controls.Add(this.OpenButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(319, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(71, 122);
            this.panel1.TabIndex = 0;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(5, 84);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(64, 22);
            this.DeleteButton.TabIndex = 3;
            this.DeleteButton.Text = "&Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // CaptureButton
            // 
            this.CaptureButton.Location = new System.Drawing.Point(5, 56);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(64, 22);
            this.CaptureButton.TabIndex = 2;
            this.CaptureButton.Text = "&Capture...";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(5, 28);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(64, 22);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "&Add...";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(5, 0);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(64, 22);
            this.OpenButton.TabIndex = 0;
            this.OpenButton.Text = "&Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.AttachmentsListView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(319, 122);
            this.panel2.TabIndex = 1;
            // 
            // AttachmentsListView
            // 
            this.AttachmentsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader,
            this.SizeColumnHeader,
            this.CommentsColumnHeader});
            this.AttachmentsListView.ContextMenuStrip = this.DefaultContextMenuStrip;
            this.AttachmentsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttachmentsListView.FullRowSelect = true;
            this.AttachmentsListView.HideSelection = false;
            this.AttachmentsListView.Location = new System.Drawing.Point(0, 0);
            this.AttachmentsListView.MultiSelect = false;
            this.AttachmentsListView.Name = "AttachmentsListView";
            this.AttachmentsListView.Size = new System.Drawing.Size(319, 122);
            this.AttachmentsListView.TabIndex = 0;
            this.AttachmentsListView.UseCompatibleStateImageBehavior = false;
            this.AttachmentsListView.View = System.Windows.Forms.View.Details;
            this.AttachmentsListView.DoubleClick += new System.EventHandler(this.AttachmentsListView_DoubleClick);
            this.AttachmentsListView.SelectedIndexChanged += new System.EventHandler(this.AttachmentsListView_SelectedIndexChanged);
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Name";
            this.NameColumnHeader.Width = 180;
            // 
            // SizeColumnHeader
            // 
            this.SizeColumnHeader.Text = "Size";
            this.SizeColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.SizeColumnHeader.Width = 40;
            // 
            // CommentsColumnHeader
            // 
            this.CommentsColumnHeader.Text = "Comments";
            this.CommentsColumnHeader.Width = 80;
            // 
            // DefaultContextMenuStrip
            // 
            this.DefaultContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.addToolStripMenuItem,
            this.captureToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.DefaultContextMenuStrip.Name = "contextMenuStrip1";
            this.DefaultContextMenuStrip.Size = new System.Drawing.Size(126, 92);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.addToolStripMenuItem.Text = "&Add...";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // captureToolStripMenuItem
            // 
            this.captureToolStripMenuItem.Name = "captureToolStripMenuItem";
            this.captureToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.captureToolStripMenuItem.Text = "&Capture...";
            this.captureToolStripMenuItem.Click += new System.EventHandler(this.captureToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // AttachmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "AttachmentControl";
            this.Size = new System.Drawing.Size(390, 122);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.DefaultContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button CaptureButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView AttachmentsListView;
        private System.Windows.Forms.ColumnHeader NameColumnHeader;
        private System.Windows.Forms.ColumnHeader SizeColumnHeader;
        private System.Windows.Forms.ColumnHeader CommentsColumnHeader;
        private System.Windows.Forms.ContextMenuStrip DefaultContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
