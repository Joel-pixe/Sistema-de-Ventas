namespace WindowsFormsApp1
{
    partial class FrmDashBoard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDashBoard));
            this.PnlMenuVertical = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.PnlBarraTitulo = new System.Windows.Forms.Panel();
            this.PnlContenedor = new System.Windows.Forms.Panel();
            this.BtnRestaurar = new System.Windows.Forms.PictureBox();
            this.BtnMaximizar = new System.Windows.Forms.PictureBox();
            this.BtnMinimizar = new System.Windows.Forms.PictureBox();
            this.BtnCerrar = new System.Windows.Forms.PictureBox();
            this.BtnProveedores = new System.Windows.Forms.Button();
            this.BtnClientes = new System.Windows.Forms.Button();
            this.BtnUsuarios = new System.Windows.Forms.Button();
            this.BtnVentas = new System.Windows.Forms.Button();
            this.BtnIngresos = new System.Windows.Forms.Button();
            this.BtnSalir = new System.Windows.Forms.Button();
            this.BtnArticulos = new System.Windows.Forms.Button();
            this.BtnCategoria = new System.Windows.Forms.Button();
            this.PicLogo = new System.Windows.Forms.PictureBox();
            this.PnlMenuVertical.SuspendLayout();
            this.panel1.SuspendLayout();
            this.PnlBarraTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRestaurar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMaximizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinimizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // PnlMenuVertical
            // 
            this.PnlMenuVertical.BackColor = System.Drawing.Color.DodgerBlue;
            this.PnlMenuVertical.Controls.Add(this.panel1);
            this.PnlMenuVertical.Controls.Add(this.PicLogo);
            this.PnlMenuVertical.Dock = System.Windows.Forms.DockStyle.Left;
            this.PnlMenuVertical.Location = new System.Drawing.Point(0, 0);
            this.PnlMenuVertical.Name = "PnlMenuVertical";
            this.PnlMenuVertical.Size = new System.Drawing.Size(250, 840);
            this.PnlMenuVertical.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.BtnProveedores);
            this.panel1.Controls.Add(this.BtnClientes);
            this.panel1.Controls.Add(this.BtnUsuarios);
            this.panel1.Controls.Add(this.BtnVentas);
            this.panel1.Controls.Add(this.BtnIngresos);
            this.panel1.Controls.Add(this.BtnSalir);
            this.panel1.Controls.Add(this.BtnArticulos);
            this.panel1.Controls.Add(this.BtnCategoria);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 207);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 633);
            this.panel1.TabIndex = 2;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Clientes.png");
            this.imageList1.Images.SetKeyName(1, "compras.png");
            this.imageList1.Images.SetKeyName(2, "empleados.png");
            this.imageList1.Images.SetKeyName(3, "icon cerrar2.png");
            this.imageList1.Images.SetKeyName(4, "icon maximizar.png");
            this.imageList1.Images.SetKeyName(5, "icon minimizar.png");
            this.imageList1.Images.SetKeyName(6, "icon restaurar.png");
            this.imageList1.Images.SetKeyName(7, "pagos.png");
            this.imageList1.Images.SetKeyName(8, "prodcutos.png");
            this.imageList1.Images.SetKeyName(9, "reloj.png");
            this.imageList1.Images.SetKeyName(10, "ventas.png");
            this.imageList1.Images.SetKeyName(11, "Screenshot_2026-07-17_091653-removebg-preview.png");
            this.imageList1.Images.SetKeyName(12, "categoria_icon_64x64.png");
            this.imageList1.Images.SetKeyName(13, "Screenshot_2026-07-17_094025-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(14, "Screenshot_2026-07-17_094616-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(15, "Screenshot_2026-07-17_095347-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(16, "Screenshot_2026-07-17_101348-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(17, "Screenshot_2026-07-17_101632-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(18, "Screenshot_2026-07-17_101924-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(19, "Screenshot_2026-07-17_102221-removebg-preview (1).png");
            this.imageList1.Images.SetKeyName(20, "estadisticas.png");
            // 
            // PnlBarraTitulo
            // 
            this.PnlBarraTitulo.Controls.Add(this.BtnRestaurar);
            this.PnlBarraTitulo.Controls.Add(this.BtnMaximizar);
            this.PnlBarraTitulo.Controls.Add(this.BtnMinimizar);
            this.PnlBarraTitulo.Controls.Add(this.BtnCerrar);
            this.PnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlBarraTitulo.Location = new System.Drawing.Point(250, 0);
            this.PnlBarraTitulo.Name = "PnlBarraTitulo";
            this.PnlBarraTitulo.Size = new System.Drawing.Size(1333, 100);
            this.PnlBarraTitulo.TabIndex = 1;
            this.PnlBarraTitulo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlBarraTitulo_MouseDown);
            // 
            // PnlContenedor
            // 
            this.PnlContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlContenedor.Location = new System.Drawing.Point(250, 100);
            this.PnlContenedor.Name = "PnlContenedor";
            this.PnlContenedor.Size = new System.Drawing.Size(1333, 740);
            this.PnlContenedor.TabIndex = 2;
            // 
            // BtnRestaurar
            // 
            this.BtnRestaurar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRestaurar.Image = global::WindowsFormsApp1.Properties.Resources.icon_restaurar;
            this.BtnRestaurar.Location = new System.Drawing.Point(1188, 21);
            this.BtnRestaurar.Name = "BtnRestaurar";
            this.BtnRestaurar.Size = new System.Drawing.Size(50, 50);
            this.BtnRestaurar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BtnRestaurar.TabIndex = 3;
            this.BtnRestaurar.TabStop = false;
            this.BtnRestaurar.Click += new System.EventHandler(this.BtnRestaurar_Click);
            // 
            // BtnMaximizar
            // 
            this.BtnMaximizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMaximizar.Image = global::WindowsFormsApp1.Properties.Resources.icon_maximizar;
            this.BtnMaximizar.Location = new System.Drawing.Point(1188, 21);
            this.BtnMaximizar.Name = "BtnMaximizar";
            this.BtnMaximizar.Size = new System.Drawing.Size(50, 50);
            this.BtnMaximizar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BtnMaximizar.TabIndex = 2;
            this.BtnMaximizar.TabStop = false;
            this.BtnMaximizar.Click += new System.EventHandler(this.BtnMaximizar_Click);
            // 
            // BtnMinimizar
            // 
            this.BtnMinimizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMinimizar.Image = global::WindowsFormsApp1.Properties.Resources.icon_minimizar;
            this.BtnMinimizar.Location = new System.Drawing.Point(1115, 21);
            this.BtnMinimizar.Name = "BtnMinimizar";
            this.BtnMinimizar.Size = new System.Drawing.Size(50, 50);
            this.BtnMinimizar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BtnMinimizar.TabIndex = 1;
            this.BtnMinimizar.TabStop = false;
            this.BtnMinimizar.Click += new System.EventHandler(this.BtnMinimizar_Click);
            // 
            // BtnCerrar
            // 
            this.BtnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCerrar.Image = global::WindowsFormsApp1.Properties.Resources.icon_cerrar2;
            this.BtnCerrar.Location = new System.Drawing.Point(1261, 21);
            this.BtnCerrar.Name = "BtnCerrar";
            this.BtnCerrar.Size = new System.Drawing.Size(50, 50);
            this.BtnCerrar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BtnCerrar.TabIndex = 0;
            this.BtnCerrar.TabStop = false;
            this.BtnCerrar.Click += new System.EventHandler(this.BtnCerrar_Click);
            // 
            // BtnProveedores
            // 
            this.BtnProveedores.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnProveedores.FlatAppearance.BorderSize = 0;
            this.BtnProveedores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnProveedores.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnProveedores.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnProveedores.ImageKey = "Screenshot_2026-07-17_102221-removebg-preview (1).png";
            this.BtnProveedores.ImageList = this.imageList1;
            this.BtnProveedores.Location = new System.Drawing.Point(0, 456);
            this.BtnProveedores.Name = "BtnProveedores";
            this.BtnProveedores.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnProveedores.Size = new System.Drawing.Size(260, 76);
            this.BtnProveedores.TabIndex = 11;
            this.BtnProveedores.Text = "PROVEEDORES";
            this.BtnProveedores.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnProveedores.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnProveedores.UseVisualStyleBackColor = true;
            this.BtnProveedores.Click += new System.EventHandler(this.BtnProveedores_Click);
            // 
            // BtnClientes
            // 
            this.BtnClientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnClientes.FlatAppearance.BorderSize = 0;
            this.BtnClientes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClientes.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClientes.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnClientes.ImageKey = "Screenshot_2026-07-17_101348-removebg-preview (1).png";
            this.BtnClientes.ImageList = this.imageList1;
            this.BtnClientes.Location = new System.Drawing.Point(0, 380);
            this.BtnClientes.Name = "BtnClientes";
            this.BtnClientes.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnClientes.Size = new System.Drawing.Size(260, 76);
            this.BtnClientes.TabIndex = 10;
            this.BtnClientes.Text = "CLIENTES";
            this.BtnClientes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnClientes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnClientes.UseVisualStyleBackColor = true;
            this.BtnClientes.Click += new System.EventHandler(this.BtnClientes_Click_1);
            // 
            // BtnUsuarios
            // 
            this.BtnUsuarios.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnUsuarios.FlatAppearance.BorderSize = 0;
            this.BtnUsuarios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnUsuarios.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnUsuarios.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnUsuarios.ImageKey = "Screenshot_2026-07-17_095347-removebg-preview (1).png";
            this.BtnUsuarios.ImageList = this.imageList1;
            this.BtnUsuarios.Location = new System.Drawing.Point(0, 304);
            this.BtnUsuarios.Name = "BtnUsuarios";
            this.BtnUsuarios.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnUsuarios.Size = new System.Drawing.Size(260, 76);
            this.BtnUsuarios.TabIndex = 9;
            this.BtnUsuarios.Text = "USUARIOS";
            this.BtnUsuarios.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnUsuarios.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnUsuarios.UseVisualStyleBackColor = true;
            this.BtnUsuarios.Click += new System.EventHandler(this.BtnUsuarios_Click);
            // 
            // BtnVentas
            // 
            this.BtnVentas.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnVentas.FlatAppearance.BorderSize = 0;
            this.BtnVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnVentas.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnVentas.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnVentas.ImageKey = "Screenshot_2026-07-17_094616-removebg-preview (1).png";
            this.BtnVentas.ImageList = this.imageList1;
            this.BtnVentas.Location = new System.Drawing.Point(0, 228);
            this.BtnVentas.Name = "BtnVentas";
            this.BtnVentas.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnVentas.Size = new System.Drawing.Size(260, 76);
            this.BtnVentas.TabIndex = 8;
            this.BtnVentas.Text = "VENTAS";
            this.BtnVentas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnVentas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnVentas.UseVisualStyleBackColor = true;
            this.BtnVentas.Click += new System.EventHandler(this.BtnVentas_Click_1);
            // 
            // BtnIngresos
            // 
            this.BtnIngresos.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnIngresos.FlatAppearance.BorderSize = 0;
            this.BtnIngresos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnIngresos.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnIngresos.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnIngresos.ImageKey = "Screenshot_2026-07-17_094025-removebg-preview (1).png";
            this.BtnIngresos.ImageList = this.imageList1;
            this.BtnIngresos.Location = new System.Drawing.Point(0, 152);
            this.BtnIngresos.Name = "BtnIngresos";
            this.BtnIngresos.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnIngresos.Size = new System.Drawing.Size(260, 76);
            this.BtnIngresos.TabIndex = 7;
            this.BtnIngresos.Text = "INGRESOS";
            this.BtnIngresos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnIngresos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnIngresos.UseVisualStyleBackColor = true;
            this.BtnIngresos.Click += new System.EventHandler(this.BtnIngresos_Click);
            // 
            // BtnSalir
            // 
            this.BtnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.Clock;
            this.BtnSalir.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BtnSalir.FlatAppearance.BorderSize = 0;
            this.BtnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSalir.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSalir.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnSalir.ImageKey = "Screenshot_2026-07-17_101632-removebg-preview (1).png";
            this.BtnSalir.ImageList = this.imageList1;
            this.BtnSalir.Location = new System.Drawing.Point(0, 557);
            this.BtnSalir.Name = "BtnSalir";
            this.BtnSalir.Padding = new System.Windows.Forms.Padding(0, 0, 13, 0);
            this.BtnSalir.Size = new System.Drawing.Size(260, 76);
            this.BtnSalir.TabIndex = 6;
            this.BtnSalir.Text = "SALIR";
            this.BtnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnSalir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnSalir.UseVisualStyleBackColor = true;
            this.BtnSalir.Click += new System.EventHandler(this.BtnSalir_Click);
            // 
            // BtnArticulos
            // 
            this.BtnArticulos.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnArticulos.FlatAppearance.BorderSize = 0;
            this.BtnArticulos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnArticulos.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnArticulos.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnArticulos.ImageKey = "Screenshot_2026-07-17_091653-removebg-preview.png";
            this.BtnArticulos.ImageList = this.imageList1;
            this.BtnArticulos.Location = new System.Drawing.Point(0, 76);
            this.BtnArticulos.Name = "BtnArticulos";
            this.BtnArticulos.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.BtnArticulos.Size = new System.Drawing.Size(260, 76);
            this.BtnArticulos.TabIndex = 3;
            this.BtnArticulos.Text = "ARTICULOS";
            this.BtnArticulos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnArticulos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnArticulos.UseVisualStyleBackColor = true;
            this.BtnArticulos.Click += new System.EventHandler(this.BtnArticulos_Click);
            // 
            // BtnCategoria
            // 
            this.BtnCategoria.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnCategoria.FlatAppearance.BorderSize = 0;
            this.BtnCategoria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCategoria.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCategoria.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnCategoria.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnCategoria.ImageKey = "categoria_icon_64x64.png";
            this.BtnCategoria.ImageList = this.imageList1;
            this.BtnCategoria.Location = new System.Drawing.Point(0, 0);
            this.BtnCategoria.Name = "BtnCategoria";
            this.BtnCategoria.Padding = new System.Windows.Forms.Padding(10, 0, 13, 0);
            this.BtnCategoria.Size = new System.Drawing.Size(260, 76);
            this.BtnCategoria.TabIndex = 2;
            this.BtnCategoria.Text = "CATEGORIAS";
            this.BtnCategoria.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnCategoria.UseVisualStyleBackColor = true;
            this.BtnCategoria.Click += new System.EventHandler(this.BtnCategoria_Click);
            // 
            // PicLogo
            // 
            this.PicLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.PicLogo.Image = global::WindowsFormsApp1.Properties.Resources.Screenshot_2026_07_17_192320_removebg_preview;
            this.PicLogo.Location = new System.Drawing.Point(0, 0);
            this.PicLogo.Name = "PicLogo";
            this.PicLogo.Size = new System.Drawing.Size(250, 207);
            this.PicLogo.TabIndex = 0;
            this.PicLogo.TabStop = false;
            // 
            // FrmDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1583, 840);
            this.Controls.Add(this.PnlContenedor);
            this.Controls.Add(this.PnlBarraTitulo);
            this.Controls.Add(this.PnlMenuVertical);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmDashBoard";
            this.Text = "FrmDashBoard";
            this.Load += new System.EventHandler(this.FrmDashBoard_Load);
            this.PnlMenuVertical.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.PnlBarraTitulo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BtnRestaurar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMaximizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinimizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PnlMenuVertical;
        private System.Windows.Forms.Panel PnlBarraTitulo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox PicLogo;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel PnlContenedor;
        private System.Windows.Forms.Button BtnSalir;
        private System.Windows.Forms.PictureBox BtnCerrar;
        private System.Windows.Forms.PictureBox BtnRestaurar;
        private System.Windows.Forms.PictureBox BtnMaximizar;
        private System.Windows.Forms.PictureBox BtnMinimizar;
        private System.Windows.Forms.Button BtnArticulos;
        private System.Windows.Forms.Button BtnCategoria;
        private System.Windows.Forms.Button BtnIngresos;
        private System.Windows.Forms.Button BtnVentas;
        private System.Windows.Forms.Button BtnUsuarios;
        private System.Windows.Forms.Button BtnClientes;
        private System.Windows.Forms.Button BtnProveedores;
    }
}