using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmProveedores : Form
    {
        public FrmProveedores()
        {
            InitializeComponent();
        }

        private int id;
        private string nombreAnt;
        private void MensajeOK(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task Listar()
        {
            try
            {
                DgvListado.DataSource = await NPersona.ListarProveedores();
                this.Formato();
                this.Limpiar();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private bool ValidarFormato()
        {
            string tipoDoc = CboTipoDocumento.Text.Trim();
            string numDoc = TxtNumDocumento.Text.Trim();

            if (tipoDoc == "CEDULA")
            {
                string regexCedulaDNI = @"^\d{3}-?\d{7}-?\d{1}$";
                if (!Regex.IsMatch(numDoc, regexCedulaDNI))
                {
                    MensajeError("La cédula/DNI no es válida. Debe tener 11 dígitos en formato dominicano (Ej: 001-1234567-8 o sin guiones).");
                    TxtNumDocumento.Focus();
                    return false;
                }
            }
            else if (tipoDoc == "PASAPORTE")
            {
                string regexPasaporte = @"^[a-zA-Z]\d{7}$";
                if (!Regex.IsMatch(numDoc, regexPasaporte))
                {
                    MensajeError("El pasaporte no es válido. Debe comenzar con una letra seguida de 7 números (Ej: P1234567).");
                    TxtNumDocumento.Focus();
                    return false;
                }
            }
            else
            {
                MensajeError("Por favor, seleccione un Tipo de Documento válido (DNI, Cédula o Pasaporte).");
                CboTipoDocumento.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TxtDireccion.Text))
            {
                string regexDireccion = @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s,.\-#°]{5,150}$";
                if (!Regex.IsMatch(TxtDireccion.Text.Trim(), regexDireccion))
                {
                    MensajeError("La dirección contiene caracteres no permitidos.");
                    TxtDireccion.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(TxtTelefono.Text))
            {
                string regexTelefono = @"^(809|829|849)-?\d{3}-?\d{4}$";
                if (!Regex.IsMatch(TxtTelefono.Text.Trim(), regexTelefono))
                {
                    MensajeError("El teléfono debe ser un número válido de República Dominicana (Ej: 809-555-1234).");
                    TxtTelefono.Focus();
                    return false;
                }
            }

            string email = TxtEmail.Text.Trim();

            try
            {
                MailAddress direccion = new MailAddress(email);

                if (!direccion.Host.Contains("."))
                {
                    MensajeError("El formato del correo no es válido");
                    TxtEmail.Focus();
                    return false;
                }
            }
            catch (FormatException)
            {
                MensajeError("El formato del correo no es válido");
                TxtEmail.Focus();
                return false;
            }

            return true;
        }
        private async Task Buscar()
        {
            try
            {
                DgvListado.DataSource = await NPersona.BuscarProveedores(TxtBuscar.Text);
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
            DgvListado.Columns[1].Visible = false;
            DgvListado.Columns[2].Width = 120;
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

        private void Limpiar()
        {
            TxtBuscar.Clear();
            TxtNombre.Clear();
            id = 0;
            nombreAnt = "";
            TxtNumDocumento.Clear();
            TxtDireccion.Clear();
            TxtTelefono.Clear();
            TxtEmail.Clear();
            BtnInsertar.Visible = true;
            BtnActualizar.Visible = false;

            DgvListado.Columns[0].Visible = false;
            BtnEliminar.Visible = false;
            ChkSeleccionar.Checked = false;
        }

        private async Task Insertar()
        {
            try
            {
                if (TxtNombre.Text == string.Empty || CboTipoDocumento.Text == string.Empty ||
                    TxtNumDocumento.Text == string.Empty)
                {
                    MensajeError("Debe completar los campos obligatorios");
                    return;
                }

                if (!this.ValidarFormato())
                {
                    return;
                }

                string Respuesta = await NPersona.Insertar(
                    "Proveedor",
                    TxtNombre.Text.Trim(),
                    CboTipoDocumento.Text,
                    TxtNumDocumento.Text.Trim(),
                    TxtDireccion.Text.Trim(),
                    TxtTelefono.Text.Trim(),
                    TxtEmail.Text.Trim()
                );

                if (Respuesta == "OK")
                {
                    MensajeOK("El proveedor se guardó correctamente!");
                    await this.Listar();
                    TabGeneral.SelectedIndex = 0;
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

        private async Task Actualizar()
        {
            try
            {
                if (TxtNombre.Text == string.Empty || CboTipoDocumento.Text == string.Empty ||
                    TxtNumDocumento.Text == string.Empty)
                {
                    MensajeError("Debe completar los campos obligatorios");
                    return;
                }

                if (!this.ValidarFormato())
                {
                    return;
                }

                string Respuesta = await NPersona.Actualizar(
                    this.id,
                    "Proveedor",
                    this.nombreAnt,
                    TxtNombre.Text.Trim(),
                    CboTipoDocumento.Text,
                    TxtNumDocumento.Text.Trim(),
                    TxtDireccion.Text.Trim(),
                    TxtTelefono.Text.Trim(),
                    TxtEmail.Text.Trim()
                );

                if (Respuesta == "OK")
                    MensajeOK("El proveedor se actualizó correctamente");
                else
                    MensajeError(Respuesta);

                await this.Listar();
                TabGeneral.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async Task Eliminar()
        {
            try
            {
                if (MessageBox.Show("Realmente desea eliminar lo(s) proveedor(es) Seleccionado(s)?", "Eliminar Proveedores", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int idProveedor;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                        {
                            idProveedor = Convert.ToInt32(row.Cells["ID"].Value);
                            Respuesta = await NPersona.Eliminar(idProveedor);

                            if (Respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                                MensajeOK($"El proveedor {row.Cells["Nombre"].Value} se eliminó correctamente");
                            else
                                MensajeError(Respuesta);
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

        private void Cancelar()
        {
            this.Limpiar();
            TabGeneral.SelectedIndex = 0;
        }

        private async void BtnInsertar_Click(object sender, EventArgs e)
        {
            await this.Insertar();
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            await this.Actualizar();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
           await this.Eliminar();
        }

        private void ChkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkSeleccionar.Checked)
            {
                DgvListado.Columns[0].Visible = true;
                BtnEliminar.Visible = true;
            }
            else
            {
                DgvListado.Columns[0].Visible = false;
                BtnEliminar.Visible = false;
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

        private async void FrmProveedores_Load(object sender, EventArgs e)
        {
            await this.Listar();
        }

        private void DgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Limpiar();
                id = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
                nombreAnt = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);

                BtnActualizar.Visible = true;
                BtnInsertar.Visible = true;
                TxtNombre.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
                CboTipoDocumento.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Tipo_Documento"].Value);
                TxtNumDocumento.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Num_Documento"].Value);
                TxtDireccion.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Direccion"].Value);
                TxtTelefono.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Telefono"].Value);
                TxtEmail.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Email"].Value);
                TabGeneral.SelectedIndex = 1;
                BtnInsertar.Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Seleccione desde la celda nombre. " + "| Error: " + ex.Message);
            }
        }

        private async void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            await this.Buscar();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
