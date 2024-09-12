namespace Windows_Forms_CORE_CHAT_UGH
{
    partial class Form2
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
            Username = new System.Windows.Forms.TextBox();
            Password = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            Register = new System.Windows.Forms.Button();
            BackToHome = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // Username
            // 
            Username.Location = new System.Drawing.Point(129, 98);
            Username.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Username.Name = "Username";
            Username.Size = new System.Drawing.Size(189, 27);
            Username.TabIndex = 0;
            // 
            // Password
            // 
            Password.Location = new System.Drawing.Point(129, 147);
            Password.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Password.Name = "Password";
            Password.Size = new System.Drawing.Size(189, 27);
            Password.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(180, 71);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(90, 18);
            label1.TabIndex = 2;
            label1.Text = "Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(180, 127);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(90, 18);
            label2.TabIndex = 3;
            label2.Text = "Password";
            // 
            // Register
            // 
            Register.Location = new System.Drawing.Point(158, 190);
            Register.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Register.Name = "Register";
            Register.Size = new System.Drawing.Size(112, 24);
            Register.TabIndex = 4;
            Register.Text = "Register";
            Register.UseVisualStyleBackColor = true;
            Register.Click += Register_Click;
            // 
            // BackToHome
            // 
            BackToHome.Location = new System.Drawing.Point(12, 302);
            BackToHome.Name = "BackToHome";
            BackToHome.Size = new System.Drawing.Size(112, 34);
            BackToHome.TabIndex = 5;
            BackToHome.Text = "Home";
            BackToHome.UseVisualStyleBackColor = true;
            BackToHome.Click += BackToHome_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(475, 348);
            Controls.Add(BackToHome);
            Controls.Add(Register);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Password);
            Controls.Add(Username);
            Font = new System.Drawing.Font("Alpharush", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.CornflowerBlue;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "Form2";
            RightToLeftLayout = true;
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Register;
        private System.Windows.Forms.Button BackToHome;
    }
}