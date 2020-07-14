namespace EmojiPad
{
    partial class EmojiPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmojiPicker));
            this.sideBar = new System.Windows.Forms.PictureBox();
            this.emojiBox = new System.Windows.Forms.FlowLayoutPanel();
            this.searchBar = new System.Windows.Forms.TextBox();
            this.emojiCaption = new System.Windows.Forms.Label();
            this.emojiPreview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.sideBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emojiPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // sideBar
            // 
            this.sideBar.Image = ((System.Drawing.Image)(resources.GetObject("sideBar.Image")));
            this.sideBar.Location = new System.Drawing.Point(-6, -80);
            this.sideBar.Name = "sideBar";
            this.sideBar.Size = new System.Drawing.Size(50, 252);
            this.sideBar.TabIndex = 4;
            this.sideBar.TabStop = false;
            // 
            // emojiBox
            // 
            this.emojiBox.AutoScroll = true;
            this.emojiBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(53)))));
            this.emojiBox.Location = new System.Drawing.Point(52, 52);
            this.emojiBox.Name = "emojiBox";
            this.emojiBox.Size = new System.Drawing.Size(284, 242);
            this.emojiBox.TabIndex = 5;
            // 
            // searchBar
            // 
            this.searchBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.searchBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.searchBar.Location = new System.Drawing.Point(53, 12);
            this.searchBar.Name = "searchBar";
            this.searchBar.Size = new System.Drawing.Size(279, 27);
            this.searchBar.TabIndex = 6;
            this.searchBar.TextChanged += new System.EventHandler(this.searchBar_TextChanged);
            // 
            // emojiCaption
            // 
            this.emojiCaption.AutoSize = true;
            this.emojiCaption.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.emojiCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.emojiCaption.Location = new System.Drawing.Point(53, 306);
            this.emojiCaption.Name = "emojiCaption";
            this.emojiCaption.Size = new System.Drawing.Size(176, 20);
            this.emojiCaption.TabIndex = 7;
            this.emojiCaption.Text = "Press the emoji to copy it";
            // 
            // emojiPreview
            // 
            this.emojiPreview.Location = new System.Drawing.Point(292, 306);
            this.emojiPreview.Name = "emojiPreview";
            this.emojiPreview.Size = new System.Drawing.Size(40, 40);
            this.emojiPreview.TabIndex = 8;
            this.emojiPreview.TabStop = false;
            // 
            // EmojiPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(349, 359);
            this.ControlBox = false;
            this.Controls.Add(this.emojiPreview);
            this.Controls.Add(this.emojiCaption);
            this.Controls.Add(this.searchBar);
            this.Controls.Add(this.emojiBox);
            this.Controls.Add(this.sideBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EmojiPicker";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Pick an Emoji";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.EmojiPicker_Deactivate);
            this.Load += new System.EventHandler(this.EmojiPicker_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EmojiPicker_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EmojiPicker_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EmojiPicker_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.sideBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emojiPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox sideBar;
        private System.Windows.Forms.FlowLayoutPanel emojiBox;
        private System.Windows.Forms.Label emojiCaption;
        private System.Windows.Forms.PictureBox emojiPreview;
        public System.Windows.Forms.TextBox searchBar;
    }
}