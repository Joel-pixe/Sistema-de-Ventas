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
using static Sistema.Negocios.NIngresos;

namespace WindowsFormsApp1
{
    public partial class FrmIngresos : Form
    {
        // Crear DtDetalle para llenar el detalle de un ingreso
        private DataTable DtDetalle = new DataTable();
        public FrmIngresos()
        {
            InitializeComponent();
        }

        #region "Mis Metodos"

        //metodo para mostrar un mensaje cuando algo salga bien
        private void MensajeOK(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //metodo para mostrar un mensaje cuando algo salga mal

        private void MensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //funcion para darle formato al DataGridView
        private void Formato()
        {
            DgvListado.Columns[0].Visible = false;
            DgvListado.Columns[1].Visible = false;
            DgvListado.Columns[2].Visible = false;
            DgvListado.Columns[0].Width = 100;
            DgvListado.Columns[3].Width = 150;
            DgvListado.Columns[4].Width = 150;
            DgvListado.Columns[5].Width = 100;
            DgvListado.Columns[5].HeaderText = "Documento";
            DgvListado.Columns[6].Width = 70;
            DgvListado.Columns[6].HeaderText = "Serie";
            DgvListado.Columns[7].Width = 70;
            DgvListado.Columns[7].HeaderText = "Número";
            DgvListado.Columns[8].Width = 60;
            DgvListado.Columns[9].Width = 100;
            DgvListado.Columns[10].Width = 100;
            DgvListado.Columns[11].Width = 100;
        }

        //metodo para resetear la interfaz
        private void Limpiar()
        {
            TxtBuscar.Clear();
            TxtCodigo.Clear();
            TxtIdProveedor.Clear();
            TxtNombreProveedor.Clear();
            TxtSerieComprobante.Clear();
            TxtNumComprobante.Clear();
            DtDetalle.Clear();
            TxtSubtotal.Text = "0.00";
            TxtTotalImpuesto.Text = "0.00";
            TxtTotal.Text = "0.00";
            ChkSeleccionar.Checked = false;
        }

        private void CrearTabla()
        {
            this.DtDetalle.Columns.Add("idarticulo", System.Type.GetType("System.Int32"));
            this.DtDetalle.Columns.Add("codigo", System.Type.GetType("System.String"));
            this.DtDetalle.Columns.Add("articulo", System.Type.GetType("System.String"));
            this.DtDetalle.Columns.Add("cantidad", System.Type.GetType("System.Int32"));
            this.DtDetalle.Columns.Add("precio", System.Type.GetType("System.Decimal"));
            this.DtDetalle.Columns.Add("importe", System.Type.GetType("System.Decimal"));

            //Añadirlo al DGV
            DgvDetalle.DataSource = this.DtDetalle;

            DgvDetalle.Columns[0].Visible = false;
            DgvDetalle.Columns[1].HeaderText = "CODIGO";
            DgvDetalle.Columns[1].Width = 100;
            DgvDetalle.Columns[2].HeaderText = "ARTICULO";
            DgvDetalle.Columns[2].Width = 200;
            DgvDetalle.Columns[3].HeaderText = "CANTIDAD";
            DgvDetalle.Columns[3].Width = 70;
            DgvDetalle.Columns[4].HeaderText = "PRECIO";
            DgvDetalle.Columns[4].Width = 70;
            DgvDetalle.Columns[5].HeaderText = "IMPORTE";
            DgvDetalle.Columns[5].Width = 80;

            DgvDetalle.Columns[1].ReadOnly = false;
            DgvDetalle.Columns[2].ReadOnly = false;
            DgvDetalle.Columns[5].ReadOnly = false;
        }

        private async Task Listar()
        {
            try
            {
                DgvListado.DataSource = await NIngresos.Listar();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                this.Formato();
                this.Limpiar();

            }
            catch (Exception ex)
            {

                this.MensajeError(ex.Message);
            }
        }

        private int id;
        private string nombreAnt;

        //Filtrar categorias
        private async Task Buscar(string valor)
        {
            try
            {
                DgvListado.DataSource = await NIngresos.Buscar(valor);
                this.Formato();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                //this.Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Cancelar()
        {
            MessageBox.Show("Realmente desesa cancelar?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            TabPrincipal.SelectedIndex = 0;

        }

        #endregion

        private async void FrmIngresos_Load(object sender, EventArgs e)
        {
            await this.Listar();
            this.CrearTabla();
        }

        private async void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DataTable Tabla = new DataTable();
                    Tabla = await NArticulo.BuscarCodigo(TxtCodigo.Text.Trim());

                    if (Tabla.Rows.Count <= 0)
                    {
                        this.MensajeError("No existe un artículo con ese código de barras");
                    }
                    else
                    {
                        this.AgregarDetalle
                            (
                            Convert.ToInt32(Tabla.Rows[0][0]),
                            Convert.ToString(Tabla.Rows[0][1]),
                            Convert.ToString(Tabla.Rows[0][2]),
                            Convert.ToDecimal(Tabla.Rows[0][3])
                            );
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        //Metodo agregar detalle de un articulo para ingreso
        private void AgregarDetalle(int IdArticulo, string Codigo, string Nombre, decimal Precio)
        {
            bool Agregar = true;
            foreach (DataRow FilaTemp in DtDetalle.Rows)
            {
                if (Convert.ToInt32(FilaTemp["idarticulo"]) == IdArticulo)
                {
                    Agregar = false;
                    MensajeError("El articulo ya ha sido agregado");
                }
            }

            if (Agregar)
            {
                DataRow Fila = DtDetalle.NewRow();
                Fila["idarticulo"] = IdArticulo;
                Fila["codigo"] = Codigo;
                Fila["articulo"] = Nombre;
                Fila["cantidad"] = 1;
                Fila["precio"] = Precio;
                Fila["importe"] = Precio;
                this.DtDetalle.Rows.Add(Fila);

                //Debes calcular los totales
                this.CalcularTotales();
            }
        }

        private void CalcularTotales()
        {
            decimal Total = 0;
            decimal Subtotal = 0;

            if (DgvDetalle.Rows.Count == 0)
            {
                Total = 0;
            }
            else
            {
                foreach (DataRow item in DtDetalle.Rows)
                {
                    Total = Total + Convert.ToDecimal(item["importe"]);
                }
            }

            Subtotal = Total / (1 + Convert.ToDecimal(TxtImpuesto.Text));
            TxtTotal.Text = Total.ToString("#0.00#");
            TxtSubtotal.Text = Subtotal.ToString("#0.00#");
            TxtTotalImpuesto.Text = (Total - Subtotal).ToString("#0.00#");
        }

        private void BtnVerArticulos_Click(object sender, EventArgs e)
        {
            FrmMostrarArticulos frm = new FrmMostrarArticulos();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.AgregarDetalle(Variables.IdArticulo, Variables.Codigo, Variables.Nombre, Variables.Precio);
            }
        }

        private void DgvDetalle_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataRow Fila = (DataRow)DtDetalle.Rows[e.RowIndex];
            decimal Precio = Convert.ToDecimal(Fila["precio"]);
            int Cantidad = Convert.ToInt32(Fila["cantidad"]);
            Fila["importe"] = Precio * Cantidad;
            this.CalcularTotales();
        }

        private void DgvDetalle_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.CalcularTotales();
        }

        private async void BtnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                if (TxtIdProveedor.Text == string.Empty || TxtImpuesto.Text == string.Empty || TxtNumComprobante.Text == string.Empty || DtDetalle.Rows.Count == 0)
                {
                    this.MensajeError("Falta ingresar algunos datos, serán remarcados");
                    return;
                }

                string Respuesta = await NIngresos.Insertar(
                    Convert.ToInt32(TxtIdProveedor.Text),
                    Variables.IdUsuario,
                    CboComprobante.Text,
                    TxtSerieComprobante.Text.Trim(),
                    TxtNumComprobante.Text.Trim(),
                    Convert.ToDecimal(TxtImpuesto.Text),
                    Convert.ToDecimal(TxtTotal.Text),
                    DtDetalle
                );

                if (Respuesta == "OK")
                {
                    MensajeOK("El Ingreso se guardó correctamente!");
                    await this.Listar();
                    TabPrincipal.SelectedIndex = 0;
                }
                else
                {
                    MensajeError(Respuesta);
                }
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private void BtnBuscarProveedor_Click(object sender, EventArgs e)
        {
            FrmVistaProveedores vista = new FrmVistaProveedores();
            vista.ShowDialog();
            TxtIdProveedor.Text = Variables.IdProveedor.ToString();
            TxtNombreProveedor.Text = Variables.NombreProveedor;
        }



        private void ChkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkSeleccionar.Checked)
            {
                DgvListado.Columns[0].Visible = true;
                BtnAnular.Visible = true;
            }
            else
            {
                DgvListado.Columns[0].Visible = false;
                BtnAnular.Visible = false;
            }
        }

        private void DgvListado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DgvListado.Columns.Count > 0 && DgvListado.Columns["Seleccionar"] != null
                && e.RowIndex >= 0 && e.ColumnIndex == DgvListado.Columns["Seleccionar"].Index)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)DgvListado.Rows[e.RowIndex].Cells["Seleccionar"];
                chk.Value = !Convert.ToBoolean(chk.Value);
            }
        }

        private async void BtnAnular_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult Opcion;
                Opcion = MessageBox.Show("Realmente deseas anular el(los) registro(s)?", "Sistema de ventas", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (Opcion == DialogResult.OK)
                {
                    int Codigo;
                    string Rpta = "";

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells[0].Value))
                        {
                            Codigo = Convert.ToInt32(row.Cells[1].Value);
                            Rpta = await NIngresos.Anular(Codigo);

                            if (Rpta.Equals("Ok"))
                            {
                                this.MensajeOK("Se anuló el registro: " + Convert.ToString(Codigo));
                            }
                            else
                            {
                                this.MensajeError(Rpta);
                            }
                        }
                    }
                   await this.Listar();
                }
                
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            panelDetalleIngreso.Visible = false;
        }

        private async void DgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DgvMostrarDetalles.DataSource = await NIngresos.ListarDetalle(Convert.ToInt32(DgvListado.CurrentRow.Cells["Id"].Value));
                decimal Total, Subtotal;
                decimal Impuesto = Convert.ToDecimal(DgvListado.CurrentRow.Cells["Impuesto"].Value);
                Total = Convert.ToDecimal(DgvListado.CurrentRow.Cells["Total"].Value);
                Subtotal = Total / (1 + Impuesto);
                TxtSubTotalD.Text = Subtotal.ToString("#0.00#");
                TxtTotalImpuestoD.Text = (Total - Subtotal).ToString("#0.00#");
                TxtTotalD.Text = Total.ToString("#0.00#");
                panelDetalleIngreso.Visible = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private void DgvDetalle_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (DgvDetalle.Columns[e.ColumnIndex].Name == "cantidad")
            {
                MessageBox.Show("No se pueden ingresar cantidades decimales. Ingrese un numero entero.", "Cantidad invalida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("El valor ingresado no es valido.", "Dato invalido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            e.ThrowException = false;
            e.Cancel = true;
        }
    }
}
