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
    public partial class FrmMostrarArticulos : Form
    {
        public FrmMostrarArticulos()
        {
            InitializeComponent();
            this.Name = "FrmMostrarArticulos";
            this.Load += new System.EventHandler(this.FrmMostrarArticulos_Load);
        }

        private void Formato()
        {
            Seleccionar.Visible = false;

            DgvListado.Columns[1].Visible = false;  // ID (idarticulo)
            DgvListado.Columns[2].Visible = false;  // idcategoria

            DgvListado.Columns[3].Width = 120;
            DgvListado.Columns[3].HeaderText = "Categoría";

            DgvListado.Columns[4].Width = 100;
            DgvListado.Columns[4].HeaderText = "Código";

            DgvListado.Columns[5].Width = 150;
            DgvListado.Columns[5].HeaderText = "Nombre";

            DgvListado.Columns[6].Width = 100;
            DgvListado.Columns[6].HeaderText = "Precio Venta";

            DgvListado.Columns[7].Width = 80;
            DgvListado.Columns[7].HeaderText = "Stock";

            DgvListado.Columns[8].Width = 200;
            DgvListado.Columns[8].HeaderText = "Descripción";

            DgvListado.Columns[9].Visible = true;  // Imagen (ocultar)
            DgvListado.Columns[9].Width = 200;

            DgvListado.Columns[10].Width = 100;
            DgvListado.Columns[10].HeaderText = "Estado";
        }
        private async void Buscar(string valor)
        {
            try
            {
                DataTable tabla = await NArticulo.Buscar(valor);

                if (tabla.Columns.Contains("Estado"))
                {
                    DataView vista = tabla.DefaultView;
                    vista.RowFilter = "Estado = true";
                    tabla = vista.ToTable();
                }

                DgvListado.DataSource = tabla;
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            this.Buscar(TxtBuscar.Text);
        }

        private void DgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            Variables.IdArticulo = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
            Variables.Codigo = Convert.ToString(DgvListado.CurrentRow.Cells["Codigo"].Value);
            Variables.Nombre = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
            Variables.Precio = Convert.ToDecimal(DgvListado.CurrentRow.Cells["Precio_Venta"].Value);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmMostrarArticulos_Load(object sender, EventArgs e)
        {
            this.Buscar(string.Empty);  
        }
    }
}
