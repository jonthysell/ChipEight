namespace ChipEight.WinFormsApp
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.keyPadTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.keyFButton = new System.Windows.Forms.Button();
            this.keyBButton = new System.Windows.Forms.Button();
            this.key0Button = new System.Windows.Forms.Button();
            this.keyAButton = new System.Windows.Forms.Button();
            this.keyEButton = new System.Windows.Forms.Button();
            this.key9Button = new System.Windows.Forms.Button();
            this.key8Button = new System.Windows.Forms.Button();
            this.key7Button = new System.Windows.Forms.Button();
            this.keyDButton = new System.Windows.Forms.Button();
            this.key6Button = new System.Windows.Forms.Button();
            this.key5Button = new System.Windows.Forms.Button();
            this.key4Button = new System.Windows.Forms.Button();
            this.keyCButton = new System.Windows.Forms.Button();
            this.key3Button = new System.Windows.Forms.Button();
            this.key2Button = new System.Windows.Forms.Button();
            this.key1Button = new System.Windows.Forms.Button();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            this.keyPadTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(624, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 419);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(624, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTableLayoutPanel.Controls.Add(this.mainPictureBox, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.keyPadTableLayoutPanel, 1, 0);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 1;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(624, 395);
            this.mainTableLayoutPanel.TabIndex = 2;
            // 
            // mainPictureBox
            // 
            this.mainPictureBox.BackColor = System.Drawing.Color.Black;
            this.mainPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPictureBox.Location = new System.Drawing.Point(3, 3);
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.Size = new System.Drawing.Size(306, 389);
            this.mainPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.mainPictureBox.TabIndex = 0;
            this.mainPictureBox.TabStop = false;
            // 
            // keyPadTableLayoutPanel
            // 
            this.keyPadTableLayoutPanel.ColumnCount = 4;
            this.keyPadTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.Controls.Add(this.keyFButton, 3, 3);
            this.keyPadTableLayoutPanel.Controls.Add(this.keyBButton, 2, 3);
            this.keyPadTableLayoutPanel.Controls.Add(this.key0Button, 1, 3);
            this.keyPadTableLayoutPanel.Controls.Add(this.keyAButton, 0, 3);
            this.keyPadTableLayoutPanel.Controls.Add(this.keyEButton, 3, 2);
            this.keyPadTableLayoutPanel.Controls.Add(this.key9Button, 2, 2);
            this.keyPadTableLayoutPanel.Controls.Add(this.key8Button, 1, 2);
            this.keyPadTableLayoutPanel.Controls.Add(this.key7Button, 0, 2);
            this.keyPadTableLayoutPanel.Controls.Add(this.keyDButton, 3, 1);
            this.keyPadTableLayoutPanel.Controls.Add(this.key6Button, 2, 1);
            this.keyPadTableLayoutPanel.Controls.Add(this.key5Button, 1, 1);
            this.keyPadTableLayoutPanel.Controls.Add(this.key4Button, 0, 1);
            this.keyPadTableLayoutPanel.Controls.Add(this.keyCButton, 3, 0);
            this.keyPadTableLayoutPanel.Controls.Add(this.key3Button, 2, 0);
            this.keyPadTableLayoutPanel.Controls.Add(this.key2Button, 1, 0);
            this.keyPadTableLayoutPanel.Controls.Add(this.key1Button, 0, 0);
            this.keyPadTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyPadTableLayoutPanel.Location = new System.Drawing.Point(315, 3);
            this.keyPadTableLayoutPanel.Name = "keyPadTableLayoutPanel";
            this.keyPadTableLayoutPanel.RowCount = 4;
            this.keyPadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.keyPadTableLayoutPanel.Size = new System.Drawing.Size(306, 389);
            this.keyPadTableLayoutPanel.TabIndex = 1;
            // 
            // keyFButton
            // 
            this.keyFButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyFButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyFButton.Location = new System.Drawing.Point(231, 294);
            this.keyFButton.Name = "keyFButton";
            this.keyFButton.Size = new System.Drawing.Size(72, 92);
            this.keyFButton.TabIndex = 15;
            this.keyFButton.Text = "F";
            this.keyFButton.UseVisualStyleBackColor = true;
            this.keyFButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyFButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyFButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyFButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // keyBButton
            // 
            this.keyBButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyBButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyBButton.Location = new System.Drawing.Point(155, 294);
            this.keyBButton.Name = "keyBButton";
            this.keyBButton.Size = new System.Drawing.Size(70, 92);
            this.keyBButton.TabIndex = 14;
            this.keyBButton.Text = "B";
            this.keyBButton.UseVisualStyleBackColor = true;
            this.keyBButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyBButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyBButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyBButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key0Button
            // 
            this.key0Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key0Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key0Button.Location = new System.Drawing.Point(79, 294);
            this.key0Button.Name = "key0Button";
            this.key0Button.Size = new System.Drawing.Size(70, 92);
            this.key0Button.TabIndex = 13;
            this.key0Button.Text = "0";
            this.key0Button.UseVisualStyleBackColor = true;
            this.key0Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key0Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key0Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key0Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // keyAButton
            // 
            this.keyAButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyAButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyAButton.Location = new System.Drawing.Point(3, 294);
            this.keyAButton.Name = "keyAButton";
            this.keyAButton.Size = new System.Drawing.Size(70, 92);
            this.keyAButton.TabIndex = 12;
            this.keyAButton.Text = "A";
            this.keyAButton.UseVisualStyleBackColor = true;
            this.keyAButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyAButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyAButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyAButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // keyEButton
            // 
            this.keyEButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyEButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyEButton.Location = new System.Drawing.Point(231, 197);
            this.keyEButton.Name = "keyEButton";
            this.keyEButton.Size = new System.Drawing.Size(72, 91);
            this.keyEButton.TabIndex = 11;
            this.keyEButton.Text = "E";
            this.keyEButton.UseVisualStyleBackColor = true;
            this.keyEButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyEButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyEButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyEButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key9Button
            // 
            this.key9Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key9Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key9Button.Location = new System.Drawing.Point(155, 197);
            this.key9Button.Name = "key9Button";
            this.key9Button.Size = new System.Drawing.Size(70, 91);
            this.key9Button.TabIndex = 10;
            this.key9Button.Text = "9";
            this.key9Button.UseVisualStyleBackColor = true;
            this.key9Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key9Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key9Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key9Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key8Button
            // 
            this.key8Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key8Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key8Button.Location = new System.Drawing.Point(79, 197);
            this.key8Button.Name = "key8Button";
            this.key8Button.Size = new System.Drawing.Size(70, 91);
            this.key8Button.TabIndex = 9;
            this.key8Button.Text = "8";
            this.key8Button.UseVisualStyleBackColor = true;
            this.key8Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key8Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key8Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key8Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key7Button
            // 
            this.key7Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key7Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key7Button.Location = new System.Drawing.Point(3, 197);
            this.key7Button.Name = "key7Button";
            this.key7Button.Size = new System.Drawing.Size(70, 91);
            this.key7Button.TabIndex = 8;
            this.key7Button.Text = "7";
            this.key7Button.UseVisualStyleBackColor = true;
            this.key7Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key7Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key7Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key7Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // keyDButton
            // 
            this.keyDButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyDButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyDButton.Location = new System.Drawing.Point(231, 100);
            this.keyDButton.Name = "keyDButton";
            this.keyDButton.Size = new System.Drawing.Size(72, 91);
            this.keyDButton.TabIndex = 7;
            this.keyDButton.Text = "D";
            this.keyDButton.UseVisualStyleBackColor = true;
            this.keyDButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyDButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyDButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyDButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key6Button
            // 
            this.key6Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key6Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key6Button.Location = new System.Drawing.Point(155, 100);
            this.key6Button.Name = "key6Button";
            this.key6Button.Size = new System.Drawing.Size(70, 91);
            this.key6Button.TabIndex = 6;
            this.key6Button.Text = "6";
            this.key6Button.UseVisualStyleBackColor = true;
            this.key6Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key6Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key6Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key6Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key5Button
            // 
            this.key5Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key5Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key5Button.Location = new System.Drawing.Point(79, 100);
            this.key5Button.Name = "key5Button";
            this.key5Button.Size = new System.Drawing.Size(70, 91);
            this.key5Button.TabIndex = 5;
            this.key5Button.Text = "5";
            this.key5Button.UseVisualStyleBackColor = true;
            this.key5Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key5Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key5Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key5Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key4Button
            // 
            this.key4Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key4Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key4Button.Location = new System.Drawing.Point(3, 100);
            this.key4Button.Name = "key4Button";
            this.key4Button.Size = new System.Drawing.Size(70, 91);
            this.key4Button.TabIndex = 4;
            this.key4Button.Text = "4";
            this.key4Button.UseVisualStyleBackColor = true;
            this.key4Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key4Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key4Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key4Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // keyCButton
            // 
            this.keyCButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyCButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyCButton.Location = new System.Drawing.Point(231, 3);
            this.keyCButton.Name = "keyCButton";
            this.keyCButton.Size = new System.Drawing.Size(72, 91);
            this.keyCButton.TabIndex = 3;
            this.keyCButton.Text = "C";
            this.keyCButton.UseVisualStyleBackColor = true;
            this.keyCButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.keyCButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.keyCButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.keyCButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key3Button
            // 
            this.key3Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key3Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key3Button.Location = new System.Drawing.Point(155, 3);
            this.key3Button.Name = "key3Button";
            this.key3Button.Size = new System.Drawing.Size(70, 91);
            this.key3Button.TabIndex = 2;
            this.key3Button.Text = "3";
            this.key3Button.UseVisualStyleBackColor = true;
            this.key3Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key3Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key3Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key3Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key2Button
            // 
            this.key2Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key2Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key2Button.Location = new System.Drawing.Point(79, 3);
            this.key2Button.Name = "key2Button";
            this.key2Button.Size = new System.Drawing.Size(70, 91);
            this.key2Button.TabIndex = 1;
            this.key2Button.Text = "2";
            this.key2Button.UseVisualStyleBackColor = true;
            this.key2Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key2Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key2Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key2Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // key1Button
            // 
            this.key1Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.key1Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key1Button.Location = new System.Drawing.Point(3, 3);
            this.key1Button.Name = "key1Button";
            this.key1Button.Size = new System.Drawing.Size(70, 91);
            this.key1Button.TabIndex = 0;
            this.key1Button.Text = "1";
            this.key1Button.UseVisualStyleBackColor = true;
            this.key1Button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyDown);
            this.key1Button.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyPadKeyUp);
            this.key1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseDown);
            this.key1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyPadMouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "ChipEight.WinFormsApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            this.keyPadTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.TableLayoutPanel keyPadTableLayoutPanel;
        private System.Windows.Forms.Button key1Button;
        private System.Windows.Forms.Button keyFButton;
        private System.Windows.Forms.Button keyBButton;
        private System.Windows.Forms.Button key0Button;
        private System.Windows.Forms.Button keyAButton;
        private System.Windows.Forms.Button keyEButton;
        private System.Windows.Forms.Button key9Button;
        private System.Windows.Forms.Button key8Button;
        private System.Windows.Forms.Button key7Button;
        private System.Windows.Forms.Button keyDButton;
        private System.Windows.Forms.Button key6Button;
        private System.Windows.Forms.Button key5Button;
        private System.Windows.Forms.Button key4Button;
        private System.Windows.Forms.Button keyCButton;
        private System.Windows.Forms.Button key3Button;
        private System.Windows.Forms.Button key2Button;
    }
}

