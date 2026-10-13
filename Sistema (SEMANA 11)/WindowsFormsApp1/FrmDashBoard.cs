using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmDashBoard : Form
    {
        private Button BtnEstadisticas;

        public FrmDashBoard()
        {
            InitializeComponent();
            BtnRestaurar.Visible = false;
            BtnMaximizar.Visible = true;
            CrearBotonEstadisticas();
        }

        private void CrearBotonEstadisticas()
        {
            BtnEstadisticas = new Button
            {
                Dock = DockStyle.Top,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Century Gothic", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.White,
                ImageList = imageList1,
                ImageKey = "estadisticas.png",
                Location = new Point(0, 304),
                Name = "BtnEstadisticas",
                Padding = new Padding(0, 0, 30, 0),
                Size = new Size(260, 76),
                TabIndex = 10,
                Text = "ESTADÍSTICAS",
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                UseVisualStyleBackColor = true,
                Visible = false // Se muestra solo para Administrador
            };
            BtnEstadisticas.Click += BtnEstadisticas_Click;

            // Insertar en índice 3 (tras BtnUsuarios): primero se agrega al panel
            // y luego se reordena; SetChildIndex exige que el control ya sea hijo.
            // panel1.Controls: [Proveedores, Clientes, Usuarios, Ventas, Ingresos, Salir, Articulos, Categoria]
            panel1.Controls.Add(BtnEstadisticas);
            if (panel1.Controls.Count > 3)
            {
                panel1.Controls.SetChildIndex(BtnEstadisticas, 3);
            }
        }

        public int Idusuario { get; set; }
        public int Idrol { get; set; }
        public string Rol { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern int ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        //Vamos a crear dos variables para que almacenen las coordenadas del formulario
        int LX, LY;


        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            LX = Location.X;
            LY = Location.Y;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
            BtnRestaurar.Visible = true;
            BtnMaximizar.Visible = false;
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void PnlBarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        //Método para que muestre un formulario en el PnlContenedor
        private void AbrirFormulario(object frmHijo)
        {
            if (this.PnlContenedor.Controls.Count > 0)
            {
                this.PnlContenedor.Controls.RemoveAt(0);
            }

            Form fh = frmHijo as Form;
            fh.TopLevel = false; // Para que no sea la capa principal sino dentro de el DASHBOARD
            fh.Dock = DockStyle.Fill;
            this.PnlContenedor.Controls.Add(fh);
            this.PnlContenedor.Tag = fh;
            fh.Show();
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PnlContenedor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FrmDashBoard_Load(object sender, EventArgs e)
        {
            MessageBox.Show($"Bienvenido(a) al Sistema, {Nombre}", "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AplicarPermisos();

            // Cargar vista inicial según el rol
            if (Variables.Rol == "Administrador")
            {
                AbrirFormulario(new FrmEstadisticas());
            }
            else
            {
                // El dashboard analitico es la pantalla inicial para otros roles
                AbrirFormulario(new FrmDashboardHost());
            }
        }

        private void BtnVentas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVenta());
        }

        private void BtnClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmCliente());
        }

        private void BtnArticulos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmArticulo());
        }

        private void BtnCategoria_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmCategoria());
        }

        private void BtnIngresos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmIngresos());
        }

        private void BtnVentas_Click_1(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVenta());
        }

        private void BtnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmUsuario());
        }

        private void BtnEstadisticas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmEstadisticas());
        }

        private void BtnClientes_Click_1(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmCliente());
        }

        private void BtnProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedores());
        }

        private void BtnRestaurar_Click(object sender, EventArgs e)
        {
            this.Size = new Size(1300, 650);
            Location = new Point(LX, LY);
            BtnRestaurar.Visible = false;
            BtnMaximizar.Visible = true;
        }

        private void AplicarPermisos()
        {
            string Rol = Variables.Rol;

            if (Rol == "Administrador")
            {
                BtnCategoria.Visible = true;
                BtnArticulos.Visible = true;
                BtnIngresos.Visible = true;
                BtnProveedores.Visible = true;
                BtnClientes.Visible = true;
                BtnUsuarios.Visible = true;
                BtnEstadisticas.Visible = true;
            }
            else if (Rol == "Vendedor")
            {
                BtnCategoria.Visible = false;
                BtnArticulos.Visible = false;
                BtnIngresos.Visible = true;
                BtnProveedores.Visible = false;
                BtnClientes.Visible = true;
                BtnUsuarios.Visible = false;
                BtnEstadisticas.Visible = false;
            }
            else if (Rol == "Almacen")
            {
                BtnCategoria.Visible = true;
                BtnArticulos.Visible = true;
                BtnIngresos.Visible = true;
                BtnProveedores.Visible = true;
                BtnClientes.Visible = false;
                BtnUsuarios.Visible = false;
                BtnEstadisticas.Visible = false;
            }
        }
    }
}
