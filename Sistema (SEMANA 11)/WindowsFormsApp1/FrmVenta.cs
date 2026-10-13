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
    public partial class FrmVenta : Form
    {
        private DataTable DtDetalle = new DataTable();
        public FrmVenta()
        {
            InitializeComponent();
        }
        private void MensajeOK(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

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


        private void Limpiar()
        {
            TxtBuscar.Clear();
            TxtCodigo.Clear();
            TxtIdProveedor.Clear();
            TxtNombreProveedor.Clear();
            TxtSerieComprobante.Clear();
            TxtNumComprobante.Clear();
            DtDetalle.Clear();
            TxtSubtotal.Text = ImpresorFactura.Monto(0m);
            TxtTotalImpuesto.Text = ImpresorFactura.Monto(0m);
            TxtTotal.Text = ImpresorFactura.Monto(0m);
            ChkSeleccionar.Checked = false; 
        }

        private void CrearTabla()
        {
            this.DtDetalle.Columns.Add("idarticulo", System.Type.GetType("System.Int32"));
            this.DtDetalle.Columns.Add("codigo", System.Type.GetType("System.String"));
            this.DtDetalle.Columns.Add("articulo", System.Type.GetType("System.String"));
            this.DtDetalle.Columns.Add("cantidad", System.Type.GetType("System.Int32"));
            this.DtDetalle.Columns.Add("precio", System.Type.GetType("System.Decimal"));
            this.DtDetalle.Columns.Add("descuento", System.Type.GetType("System.Decimal"));
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
            DgvDetalle.Columns[5].HeaderText = "DESCUENTO";   
            DgvDetalle.Columns[5].Width = 80;
            DgvDetalle.Columns[6].HeaderText = "IMPORTE";     
            DgvDetalle.Columns[6].Width = 80;

            DgvDetalle.Columns[1].ReadOnly = false;
            DgvDetalle.Columns[2].ReadOnly = false;
            DgvDetalle.Columns[5].ReadOnly = false;   
            DgvDetalle.Columns[6].ReadOnly = true;    
        }

        private async Task Listar()
        {
            try
            {
                DgvListado.DataSource = await NVenta.Listar();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                this.Formato();
                this.Limpiar();
                this.ActualizarBotonesImpresion();

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
                DgvListado.DataSource = await NVenta.Buscar(valor);
                this.Formato();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                this.ActualizarBotonesImpresion();
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

        private async void FrmVenta_Load(object sender, EventArgs e)
        {
            DgvListado.SelectionChanged += DgvListado_SelectionChanged;
            await this.Listar();
            this.CrearTabla();
            DgvListado.CellFormatting += DgvListado_CellFormatting;
        }

        private void CalcularTotales()  
        {
            decimal Subtotal = 0;
            decimal Impuesto = Convert.ToDecimal(TxtImpuesto.Text);

            foreach (DataRow item in DtDetalle.Rows)
            {
                Subtotal = Subtotal + Convert.ToDecimal(item["importe"]);
            }

            decimal ITBIS = Math.Round(Subtotal * Impuesto, 2);
            TxtSubtotal.Text = ImpresorFactura.Monto(Subtotal);
            TxtTotalImpuesto.Text = ImpresorFactura.Monto(ITBIS);
            TxtTotal.Text = ImpresorFactura.Monto(Subtotal + ITBIS);
        }

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
                Fila["descuento"] = 0;
                Fila["importe"] = Precio;
                this.DtDetalle.Rows.Add(Fila);

                //Debes calcular los totales
                this.CalcularTotales();
            }
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
            decimal Descuento = Fila["descuento"] == DBNull.Value ? 0 : Convert.ToDecimal(Fila["descuento"]);
            decimal ImporteSinDescuento = Precio * Cantidad;

            if (Descuento < 0)
            {
                Fila["descuento"] = 0;
                throw new Exception("El descuento no puede ser un número negativo");
            }

            if (Descuento > ImporteSinDescuento)
            {
                Fila["descuento"] = 0;
                throw new Exception("El descuento no puede ser mayor al importe del artículo");
            }

            Fila["importe"] = ImporteSinDescuento - Descuento;
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

                string Respuesta = await NVenta.Insertar(
                     Convert.ToInt32(TxtIdProveedor.Text),
                     Variables.IdUsuario,
                     CboComprobante.Text,
                     TxtSerieComprobante.Text.Trim(),
                     Convert.ToInt32(TxtNumComprobante.Text.Trim()),
                     Convert.ToDecimal(TxtImpuesto.Text),
                     ImpresorFactura.MontoValor(TxtTotal.Text),
                     DtDetalle
                );

                if (Respuesta == "OK")
                {
                    MensajeOK("La venta se guardó correctamente!");
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
            FrmVistaCliente vista = new FrmVistaCliente();
            vista.ShowDialog();
            TxtIdProveedor.Text = Variables.IdCliente.ToString();
            TxtNombreProveedor.Text = Variables.NombreCliente;
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
                            Rpta = await NVenta.Anular(Codigo);

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

        private async void DgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow Fila = this.FilaVentaActual();
                if (Fila == null)
                {
                    return;
                }

                DataTable Detalle = await NVenta.ListarDetalle(Convert.ToInt32(Fila.Cells["Id"].Value));
                DgvMostrarDetalles.DataSource = Detalle;

                decimal Subtotal = 0;
                foreach (DataRow item in Detalle.Rows)
                {
                    decimal Precio = Convert.ToDecimal(item["PRECIO"]);
                    int Cantidad = Convert.ToInt32(item["CANTIDAD"]);
                    decimal Descuento = Convert.ToDecimal(item["DESCUENTO"]);
                    Subtotal = Subtotal + (Precio * Cantidad - Descuento);
                }

                decimal Impuesto = Convert.ToDecimal(Fila.Cells["Impuesto"].Value);
                decimal ITBIS = Math.Round(Subtotal * Impuesto, 2);
                TxtSubTotalD.Text = ImpresorFactura.Monto(Subtotal);
                TxtTotalImpuestoD.Text = ImpresorFactura.Monto(ITBIS);
                TxtTotalD.Text = ImpresorFactura.Monto(Subtotal + ITBIS);
                panelDetalleIngreso.Visible = true;
            }
            catch (Exception ex)
            {
                this.MensajeError(ex.Message);
            }
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            panelDetalleIngreso.Visible = false;
        }

        private void DgvListado_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (DgvListado.Columns[e.ColumnIndex].Name == "Estado" && e.Value != null && e.Value != DBNull.Value)
            {
                if (int.TryParse(e.Value.ToString(), out int estado))
                {
                    e.Value = estado == 1 ? "Aceptado" : "Anulado";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvListado_SelectionChanged(object sender, EventArgs e)
        {
            this.ActualizarBotonesImpresion();
        }

        private void ActualizarBotonesImpresion()
        {
            DataGridViewRow Fila = this.FilaVentaActual();
            bool Imprimible = Fila != null
                && DgvListado.Columns.Contains("Tipo_Comprobante")
                && this.EsComprobanteImprimible(Convert.ToString(Fila.Cells["Tipo_Comprobante"].Value));
            BtnImprimir.Enabled = Imprimible;
            BtnReimprimir.Enabled = Imprimible;
        }

        private DataGridViewRow FilaVentaActual()
        {
            if (DgvListado.Rows.Count == 0)
            {
                return null;
            }
            return DgvListado.CurrentRow ?? DgvListado.Rows[0];
        }

        private bool EsComprobanteImprimible(string TipoComprobante)
        {
            string Tipo = (TipoComprobante ?? string.Empty).Trim().ToUpperInvariant();
            return Tipo == "FACTURA" || Tipo == "TICKET" || Tipo == "BOLETA";
        }

        private async void BtnImprimir_Click(object sender, EventArgs e)
        {
            await this.ImprimirVenta(false);
        }

        private async void BtnReimprimir_Click(object sender, EventArgs e)
        {
            await this.ImprimirVenta(true);
        }

        private async Task ImprimirVenta(bool Reimpresion)
        {
            try
            {
                DataGridViewRow Fila = this.FilaVentaActual();
                if (Fila == null)
                {
                    this.MensajeError("No hay ventas en el listado para imprimir");
                    return;
                }

                string TipoComprobante = Convert.ToString(Fila.Cells["Tipo_Comprobante"].Value);
                if (!this.EsComprobanteImprimible(TipoComprobante))
                {
                    this.MensajeError("Las gu\u00edas no generan factura, por lo que no se pueden imprimir");
                    return;
                }

                int IdVenta = Convert.ToInt32(Fila.Cells["Id"].Value);
                DataTable Comprobante = await NVenta.MostrarComprobante(IdVenta);

                if (Comprobante.Rows.Count == 0)
                {
                    this.MensajeError("No se encontr\u00f3 la venta seleccionada");
                    return;
                }

                new ImpresorFactura(Comprobante, Reimpresion).VistaPrevia(this);
            }
            catch (Exception ex)
            {
                this.MensajeError(ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private void DgvDetalle_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                this.MensajeError("Por favor, ingrese una cantidad entera válida (sin puntos ni comas).");
            }
            else
            {
                this.MensajeError("Error en el formato del dato ingresado.");
            }

            // Evita que se despliegue el cuadro de diálogo de error por defecto del sistema
            e.ThrowException = false;
        }
    }
}
