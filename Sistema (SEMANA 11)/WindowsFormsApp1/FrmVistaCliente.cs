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
    public partial class FrmVistaCliente : Form
    {
        public FrmVistaCliente()
        {
            InitializeComponent();
        }

        private async Task Listar()
        {
            try
            {
                DgvListado.DataSource = await NPersona.ListarClientes();
                this.Formato();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private async Task Buscar()
        {
            try
            {
                DgvListado.DataSource = await NPersona.BuscarClientes(TxtBuscar.Text);
                this.Formato();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void Formato()
        {
            DgvListado.Columns[0].Visible = false;
            DgvListado.Columns[1].Width = 50;
            DgvListado.Columns[2].Width = 100;
            DgvListado.Columns[2].HeaderText = "Tipo Persona";
            DgvListado.Columns[3].Width = 170;
            DgvListado.Columns[4].Width = 100;
            DgvListado.Columns[4].HeaderText = "Documento";
            DgvListado.Columns[5].Width = 100;
            DgvListado.Columns[5].HeaderText = "Número Doc.";
            DgvListado.Columns[6].Width = 120;
            DgvListado.Columns[6].HeaderText = "Dirección";
            DgvListado.Columns[7].Width = 100;
            DgvListado.Columns[7].HeaderText = "Télefono";
            DgvListado.Columns[8].Width = 120;
        }


        private void DgvListado_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Variables.IdCliente = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
                Variables.NombreCliente = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Seleccione desde la celda nombre. " + "| Error: " + ex.Message);
            }
        }

        private async void FrmVistaCliente_Load(object sender, EventArgs e)
        {
            await this.Listar();
        }

        private async void BtnBuscar_Click_1(object sender, EventArgs e)
        {
            await this.Buscar();
        }
    }
}
