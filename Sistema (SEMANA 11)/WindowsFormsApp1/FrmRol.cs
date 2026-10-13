using Sistema.Negocios;
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
    public partial class FrmRol : Form
    {
        public FrmRol()
        {
            InitializeComponent();
        }
        private void MensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Formato()
        {
            DgvListado.Columns[0].Visible = false;  // idrol
            DgvListado.Columns[1].Visible = false;
            DgvListado.Columns[2].Width = 300;
            DgvListado.Columns[2].HeaderText = "Nombre";
        }

        private async Task Listar()
        {
            try
            {
                DataTable tabla = await NRol.Listar();
                DgvListado.DataSource = tabla;          // ← línea que faltaba
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async void Rol_Load(object sender, EventArgs e)
        {
            await this.Listar();
        }
    }
}
