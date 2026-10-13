using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmPrincipal : Form
    {
        private int childFormNumber = 0;

        public int Idusuario { get; set; }
        public int Idrol { get; set; }

        public string Rol { get; set; }

        public string Nombre { get; set; }

        public bool Estado { get; set; }

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
   
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void categoríasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Crear una instancia del formulario a mostrar
            FrmCategoria formulario = new FrmCategoria();
            //debemos asignarle un padre o contenedor
            formulario.MdiParent = this;
            //mostramos el formulario
            formulario.Show();
        }

        private void articulosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmArticulo formulario = new FrmArticulo();
            //debemos asignarle un padre o contenedor
            formulario.MdiParent = this;
            //mostramos el formulario
            formulario.Show();
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRol formulario = new FrmRol();
            //debemos asignarle un padre o contenedor
            formulario.MdiParent = this;
            //mostramos el formulario
            formulario.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro que desea salir del Sistema?", "Confirmar Salida", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUsuario formulario = new FrmUsuario();
            //debemos asignarle un padre o contenedor
            formulario.MdiParent = this;
            //mostramos el formulario
            formulario.Show();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            MessageBox.Show($"Bienvenido(a) al Sistema, {Nombre}", "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (Rol == "Administrador")
            {
                almacenToolStripMenuItem.Enabled = true;
                ingresosToolStripMenuItem.Enabled = true;
                ventasToolStripMenuItem.Enabled = true;
                accesoToolStripMenuItem.Enabled = true;
                consultasToolStripMenuItem.Enabled = true;
            }
            else if(Rol == "Vendedor")
            {
                almacenToolStripMenuItem.Enabled = false;
                ingresosToolStripMenuItem.Enabled = false;
                ventasToolStripMenuItem.Enabled = true;
                clientesToolStripMenuItem.Enabled = false;
                accesoToolStripMenuItem.Enabled = false;
                consultasToolStripMenuItem.Enabled = true;
            }
            else if (Rol == "Almacen")
            {
                almacenToolStripMenuItem.Enabled = true;
                ingresosToolStripMenuItem.Enabled = true;
                ventasToolStripMenuItem.Enabled = false;
                clientesToolStripMenuItem.Enabled = false;
                accesoToolStripMenuItem.Enabled = false;
                consultasToolStripMenuItem.Enabled = false;
            }
            else
            {
                almacenToolStripMenuItem.Enabled = false;
                ingresosToolStripMenuItem.Enabled = false;
                ventasToolStripMenuItem.Enabled = false;
                accesoToolStripMenuItem.Enabled = false;
                consultasToolStripMenuItem.Enabled = false;
            }
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void comprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmIngresos frm = new FrmIngresos();
            frm.MdiParent = this;
            frm.Show();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProveedores frm = new FrmProveedores();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ingresosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmIngresos frm = new FrmIngresos();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
