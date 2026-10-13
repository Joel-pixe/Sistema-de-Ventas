namespace WindowsFormsApp1
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtEmail = new System.Windows.Forms.TextBox();
            this.TxtClave = new System.Windows.Forms.TextBox();
            this.BtnAcceder = new System.Windows.Forms.Button();
            this.Olvidaste = new System.Windows.Forms.LinkLabel();
            this.BtnX = new System.Windows.Forms.Button();
            this.BtnShowPassword = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 478);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(609, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 49);
            this.label1.TabIndex = 0;
            this.label1.Text = "LOGIN";
            // 
            // TxtEmail
            // 
            this.TxtEmail.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmail.ForeColor = System.Drawing.Color.DimGray;
            this.TxtEmail.Location = new System.Drawing.Point(388, 109);
            this.TxtEmail.Name = "TxtEmail";
            this.TxtEmail.Size = new System.Drawing.Size(580, 36);
            this.TxtEmail.TabIndex = 1;
            this.TxtEmail.Text = "Usuario";
            this.TxtEmail.Enter += new System.EventHandler(this.TxtEmail_Enter);
            this.TxtEmail.Leave += new System.EventHandler(this.TxtEmail_Leave);
            // 
            // TxtClave
            // 
            this.TxtClave.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtClave.ForeColor = System.Drawing.Color.DimGray;
            this.TxtClave.Location = new System.Drawing.Point(388, 198);
            this.TxtClave.Name = "TxtClave";
            this.TxtClave.Size = new System.Drawing.Size(580, 36);
            this.TxtClave.TabIndex = 2;
            this.TxtClave.Text = "Clave";
            this.TxtClave.TextChanged += new System.EventHandler(this.TxtClave_TextChanged);
            this.TxtClave.Enter += new System.EventHandler(this.TxtClave_Enter);
            this.TxtClave.Leave += new System.EventHandler(this.TxtClave_Leave);
            // 
            // BtnAcceder
            // 
            this.BtnAcceder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BtnAcceder.FlatAppearance.BorderSize = 0;
            this.BtnAcceder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAcceder.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAcceder.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnAcceder.Location = new System.Drawing.Point(388, 285);
            this.BtnAcceder.Name = "BtnAcceder";
            this.BtnAcceder.Size = new System.Drawing.Size(580, 48);
            this.BtnAcceder.TabIndex = 3;
            this.BtnAcceder.Text = "ACCEDER";
            this.BtnAcceder.UseVisualStyleBackColor = false;
            this.BtnAcceder.Click += new System.EventHandler(this.BtnAcceder_Click);
            // 
            // Olvidaste
            // 
            this.Olvidaste.AutoSize = true;
            this.Olvidaste.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Olvidaste.LinkColor = System.Drawing.Color.FloralWhite;
            this.Olvidaste.Location = new System.Drawing.Point(599, 371);
            this.Olvidaste.Name = "Olvidaste";
            this.Olvidaste.Size = new System.Drawing.Size(203, 20);
            this.Olvidaste.TabIndex = 4;
            this.Olvidaste.TabStop = true;
            this.Olvidaste.Text = "¿Olvidaste tu contraseña?";
            this.Olvidaste.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Olvidaste_LinkClicked);
            // 
            // BtnX
            // 
            this.BtnX.FlatAppearance.BorderSize = 0;
            this.BtnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnX.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnX.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnX.Location = new System.Drawing.Point(967, 9);
            this.BtnX.Name = "BtnX";
            this.BtnX.Size = new System.Drawing.Size(75, 69);
            this.BtnX.TabIndex = 0;
            this.BtnX.Text = "X";
            this.BtnX.UseVisualStyleBackColor = true;
            this.BtnX.Click += new System.EventHandler(this.BtnX_Click);
            // 
            // BtnShowPassword
            // 
            this.BtnShowPassword.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnShowPassword.BackgroundImage")));
            this.BtnShowPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnShowPassword.Location = new System.Drawing.Point(974, 199);
            this.BtnShowPassword.Name = "BtnShowPassword";
            this.BtnShowPassword.Size = new System.Drawing.Size(44, 35);
            this.BtnShowPassword.TabIndex = 5;
            this.BtnShowPassword.UseVisualStyleBackColor = true;
            this.BtnShowPassword.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnShowPassword_MouseDown);
            this.BtnShowPassword.MouseLeave += new System.EventHandler(this.BtnShowPassword_MouseLeave);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(1043, 478);
            this.Controls.Add(this.BtnShowPassword);
            this.Controls.Add(this.BtnX);
            this.Controls.Add(this.Olvidaste);
            this.Controls.Add(this.BtnAcceder);
            this.Controls.Add(this.TxtClave);
            this.Controls.Add(this.TxtEmail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmLogin";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmLogin_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtEmail;
        private System.Windows.Forms.TextBox TxtClave;
        private System.Windows.Forms.Button BtnAcceder;
        private System.Windows.Forms.LinkLabel Olvidaste;
        private System.Windows.Forms.Button BtnX;
        private System.Windows.Forms.Button BtnShowPassword;
    }
}