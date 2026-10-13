namespace WindowsFormsApp1
{
    partial class FrmRecuperarContraseña
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRecuperarContraseña));
            this.BtnX = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PnlRecuperar = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnEnviarCodigo = new System.Windows.Forms.Button();
            this.TxtEmail = new System.Windows.Forms.TextBox();
            this.PnlRecuperar2 = new System.Windows.Forms.Panel();
            this.BtnVerficarCodigo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtCodigoVerificacion = new System.Windows.Forms.TextBox();
            this.TxtNuevaContraseña = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BtnCambiar = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtConfirmarContraseña = new System.Windows.Forms.TextBox();
            this.PnlRecuperar3 = new System.Windows.Forms.Panel();
            this.PnlRecuperar.SuspendLayout();
            this.PnlRecuperar2.SuspendLayout();
            this.PnlRecuperar3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnX
            // 
            this.BtnX.FlatAppearance.BorderSize = 0;
            this.BtnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnX.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnX.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnX.Location = new System.Drawing.Point(957, 0);
            this.BtnX.Name = "BtnX";
            this.BtnX.Size = new System.Drawing.Size(75, 69);
            this.BtnX.TabIndex = 6;
            this.BtnX.Text = "X";
            this.BtnX.UseVisualStyleBackColor = true;
            this.BtnX.Click += new System.EventHandler(this.BtnX_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(412, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(521, 49);
            this.label1.TabIndex = 7;
            this.label1.Text = "RECUPERAR CONTRASEÑA";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 452);
            this.panel1.TabIndex = 8;
            // 
            // PnlRecuperar
            // 
            this.PnlRecuperar.Controls.Add(this.label2);
            this.PnlRecuperar.Controls.Add(this.BtnEnviarCodigo);
            this.PnlRecuperar.Controls.Add(this.TxtEmail);
            this.PnlRecuperar.Location = new System.Drawing.Point(353, 124);
            this.PnlRecuperar.Name = "PnlRecuperar";
            this.PnlRecuperar.Size = new System.Drawing.Size(647, 296);
            this.PnlRecuperar.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(27, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(590, 18);
            this.label2.TabIndex = 18;
            this.label2.Text = "Ingresa tu correo electrónico registrado para enviarte un código de seguridad.";
            // 
            // BtnEnviarCodigo
            // 
            this.BtnEnviarCodigo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BtnEnviarCodigo.FlatAppearance.BorderSize = 0;
            this.BtnEnviarCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnEnviarCodigo.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnEnviarCodigo.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnEnviarCodigo.Location = new System.Drawing.Point(232, 167);
            this.BtnEnviarCodigo.Name = "BtnEnviarCodigo";
            this.BtnEnviarCodigo.Size = new System.Drawing.Size(163, 36);
            this.BtnEnviarCodigo.TabIndex = 17;
            this.BtnEnviarCodigo.Text = "ENVIAR";
            this.BtnEnviarCodigo.UseVisualStyleBackColor = false;
            this.BtnEnviarCodigo.Click += new System.EventHandler(this.BtnEnviarCodigo_Click);
            // 
            // TxtEmail
            // 
            this.TxtEmail.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmail.ForeColor = System.Drawing.Color.DimGray;
            this.TxtEmail.Location = new System.Drawing.Point(42, 107);
            this.TxtEmail.Name = "TxtEmail";
            this.TxtEmail.Size = new System.Drawing.Size(564, 36);
            this.TxtEmail.TabIndex = 16;
            this.TxtEmail.Text = "Correo";
            this.TxtEmail.Enter += new System.EventHandler(this.TxtEmail_Enter);
            this.TxtEmail.Leave += new System.EventHandler(this.TxtEmail_Leave);
            // 
            // PnlRecuperar2
            // 
            this.PnlRecuperar2.Controls.Add(this.BtnVerficarCodigo);
            this.PnlRecuperar2.Controls.Add(this.label3);
            this.PnlRecuperar2.Controls.Add(this.TxtCodigoVerificacion);
            this.PnlRecuperar2.Location = new System.Drawing.Point(350, 127);
            this.PnlRecuperar2.Name = "PnlRecuperar2";
            this.PnlRecuperar2.Size = new System.Drawing.Size(647, 296);
            this.PnlRecuperar2.TabIndex = 20;
            // 
            // BtnVerficarCodigo
            // 
            this.BtnVerficarCodigo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BtnVerficarCodigo.FlatAppearance.BorderSize = 0;
            this.BtnVerficarCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnVerficarCodigo.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnVerficarCodigo.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnVerficarCodigo.Location = new System.Drawing.Point(224, 174);
            this.BtnVerficarCodigo.Name = "BtnVerficarCodigo";
            this.BtnVerficarCodigo.Size = new System.Drawing.Size(163, 36);
            this.BtnVerficarCodigo.TabIndex = 19;
            this.BtnVerficarCodigo.Text = "VERIFICAR";
            this.BtnVerficarCodigo.UseVisualStyleBackColor = false;
            this.BtnVerficarCodigo.Click += new System.EventHandler(this.BtnVerficarCodigo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(39, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(556, 18);
            this.label3.TabIndex = 18;
            this.label3.Text = "Hemos enviado un código a tu correo. Por favor, ingrésalo a continuación.";
            // 
            // TxtCodigoVerificacion
            // 
            this.TxtCodigoVerificacion.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtCodigoVerificacion.ForeColor = System.Drawing.Color.DimGray;
            this.TxtCodigoVerificacion.Location = new System.Drawing.Point(42, 117);
            this.TxtCodigoVerificacion.Name = "TxtCodigoVerificacion";
            this.TxtCodigoVerificacion.Size = new System.Drawing.Size(564, 36);
            this.TxtCodigoVerificacion.TabIndex = 16;
            this.TxtCodigoVerificacion.Text = "Código";
            this.TxtCodigoVerificacion.Enter += new System.EventHandler(this.TxtCodigoVerificacion_Enter);
            this.TxtCodigoVerificacion.Leave += new System.EventHandler(this.TxtCodigoVerificacion_Leave);
            // 
            // TxtNuevaContraseña
            // 
            this.TxtNuevaContraseña.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtNuevaContraseña.ForeColor = System.Drawing.Color.DimGray;
            this.TxtNuevaContraseña.Location = new System.Drawing.Point(11, 57);
            this.TxtNuevaContraseña.Name = "TxtNuevaContraseña";
            this.TxtNuevaContraseña.Size = new System.Drawing.Size(564, 36);
            this.TxtNuevaContraseña.TabIndex = 16;
            this.TxtNuevaContraseña.Text = "Contraseña";
            this.TxtNuevaContraseña.Enter += new System.EventHandler(this.TxtNuevaContraseña_Enter);
            this.TxtNuevaContraseña.Leave += new System.EventHandler(this.TxtNuevaContraseña_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(6, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(334, 27);
            this.label4.TabIndex = 18;
            this.label4.Text = "Ingrese su nueva contraseña";
            // 
            // BtnCambiar
            // 
            this.BtnCambiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BtnCambiar.FlatAppearance.BorderSize = 0;
            this.BtnCambiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCambiar.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCambiar.ForeColor = System.Drawing.Color.Gainsboro;
            this.BtnCambiar.Location = new System.Drawing.Point(224, 228);
            this.BtnCambiar.Name = "BtnCambiar";
            this.BtnCambiar.Size = new System.Drawing.Size(163, 36);
            this.BtnCambiar.TabIndex = 19;
            this.BtnCambiar.Text = "CAMBIAR";
            this.BtnCambiar.UseVisualStyleBackColor = false;
            this.BtnCambiar.Click += new System.EventHandler(this.BtnCambiar_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(6, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(334, 27);
            this.label5.TabIndex = 20;
            this.label5.Text = "Ingrese su nueva contraseña";
            // 
            // TxtConfirmarContraseña
            // 
            this.TxtConfirmarContraseña.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtConfirmarContraseña.ForeColor = System.Drawing.Color.DimGray;
            this.TxtConfirmarContraseña.Location = new System.Drawing.Point(11, 149);
            this.TxtConfirmarContraseña.Name = "TxtConfirmarContraseña";
            this.TxtConfirmarContraseña.Size = new System.Drawing.Size(564, 36);
            this.TxtConfirmarContraseña.TabIndex = 21;
            this.TxtConfirmarContraseña.Text = "Contraseña";
            this.TxtConfirmarContraseña.Enter += new System.EventHandler(this.TxtConfirmarContraseña_Enter);
            this.TxtConfirmarContraseña.Leave += new System.EventHandler(this.TxtConfirmarContraseña_Leave);
            // 
            // PnlRecuperar3
            // 
            this.PnlRecuperar3.Controls.Add(this.TxtConfirmarContraseña);
            this.PnlRecuperar3.Controls.Add(this.label5);
            this.PnlRecuperar3.Controls.Add(this.BtnCambiar);
            this.PnlRecuperar3.Controls.Add(this.label4);
            this.PnlRecuperar3.Controls.Add(this.TxtNuevaContraseña);
            this.PnlRecuperar3.Location = new System.Drawing.Point(350, 124);
            this.PnlRecuperar3.Name = "PnlRecuperar3";
            this.PnlRecuperar3.Size = new System.Drawing.Size(647, 296);
            this.PnlRecuperar3.TabIndex = 21;
            // 
            // FrmRecuperarContraseña
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1025, 452);
            this.Controls.Add(this.PnlRecuperar3);
            this.Controls.Add(this.PnlRecuperar2);
            this.Controls.Add(this.PnlRecuperar);
            this.Controls.Add(this.BtnX);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmRecuperarContraseña";
            this.Text = "FrmRecuperarContraseña";
            this.Load += new System.EventHandler(this.FrmRecuperarContraseña_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmRecuperarContraseña_MouseDown);
            this.PnlRecuperar.ResumeLayout(false);
            this.PnlRecuperar.PerformLayout();
            this.PnlRecuperar2.ResumeLayout(false);
            this.PnlRecuperar2.PerformLayout();
            this.PnlRecuperar3.ResumeLayout(false);
            this.PnlRecuperar3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PnlRecuperar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtEmail;
        private System.Windows.Forms.Button BtnEnviarCodigo;
        private System.Windows.Forms.Panel PnlRecuperar2;
        private System.Windows.Forms.Button BtnVerficarCodigo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtCodigoVerificacion;
        private System.Windows.Forms.TextBox TxtNuevaContraseña;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnCambiar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtConfirmarContraseña;
        private System.Windows.Forms.Panel PnlRecuperar3;
    }
}