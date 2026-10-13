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
    public partial class FrmUsuario : Form
    {
        public FrmUsuario()
        {
            InitializeComponent();
        }

        private int id;
        private string emailAnt;

         //Método para mostrar un mensaje cuando algo salga bien
        private void MensajeOK(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Método para mostrar un mensaje cuando algo salga mal

        private void MensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //Función para darle formato al DataGridView
        private void Formato()
        {

            DgvListado.Columns[0].Visible = false;

            DgvListado.Columns[1].Visible = false;

            DgvListado.Columns[2].Visible = false;

            DgvListado.Columns[3].Width = 120;
            DgvListado.Columns[3].HeaderText = "Rol";

            DgvListado.Columns[4].Width = 160;
            DgvListado.Columns[4].HeaderText = "Nombre";

            DgvListado.Columns[5].Width = 120;
            DgvListado.Columns[5].HeaderText = "Tipo Documento";

            DgvListado.Columns[6].Width = 120;
            DgvListado.Columns[6].HeaderText = "Núm. Documento";

            DgvListado.Columns[7].Width = 160;
            DgvListado.Columns[7].HeaderText = "Dirección";

            DgvListado.Columns[8].Width = 110;
            DgvListado.Columns[8].HeaderText = "Teléfono";

            DgvListado.Columns[9].Width = 180;
            DgvListado.Columns[9].HeaderText = "Email";

            DgvListado.Columns[10].Width = 80;
            DgvListado.Columns[10].HeaderText = "Estado";
        }


        private void Limpiar()
        {
            TxtBuscar.Clear();

            // CORRECCIÓN: solo resetear si ya tiene datos cargados
            if (CmbRol.DataSource != null)
                CmbRol.SelectedValue = DBNull.Value;

            TxtNombre.Clear();

            if (CmbTipoDocumento.DataSource != null)
                CmbTipoDocumento.SelectedValue = DBNull.Value;
            else
                CmbTipoDocumento.SelectedIndex = -1;

            TxtNumeroDocumento.Clear();
            TxtDireccion.Clear();
            TxtTelefono.Clear();
            TxtEmail.Clear();
            TxtClave.Clear();

            BtnInsertar.Visible = true;
            BtnActualizar.Visible = false;
            DgvListado.Columns[0].Visible = false;
            BtnActivar.Visible = false;
            BtnDesactivar.Visible = false;
            BtnEliminar.Visible = false;

            ChkSeleccionar.Checked = false;
            LblTotal.Text = $"Total Registros: {DgvListado.Rows.Count}";
        }

   

        private async Task CargarRoles()
        {
            try
            {
                DataTable datos = await NRol.Listar();
                CmbRol.DisplayMember = "nombre";
                CmbRol.ValueMember = "idrol";
                CmbRol.DataSource = datos;
                CmbRol.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MensajeError("Error al cargar roles: " + ex.Message);
            }
        }
        private async Task Listar()
        {
            try
            {
                DataTable tabla = await NUsuario.Listar();
                DgvListado.DataSource = tabla;
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
                this.Limpiar();
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async Task Buscar(string valor)
        {
            try
            {
                DataTable tabla = await NUsuario.Buscar(valor);
                DgvListado.DataSource = tabla;
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async Task Insertar()
        {
            try
            {
                // Validar campos obligatorios
                if (CmbRol.SelectedValue == null
                    || TxtNombre.Text == string.Empty
                    || CmbTipoDocumento.SelectedItem == null
                    || TxtNumeroDocumento.Text == string.Empty
                    || TxtEmail.Text == string.Empty
                    || TxtClave.Text == string.Empty)
                {
                    MensajeError("Debe completar los campos obligatorios (*).");
                    return;
                }

                if (!this.ValidarFormato())
                {
                    return;
                }

                string respuesta = await NUsuario.Insertar(
                    Convert.ToInt32(CmbRol.SelectedValue),
                    TxtNombre.Text.Trim(),
                    CmbTipoDocumento.SelectedItem.ToString(),
                    TxtNumeroDocumento.Text.Trim(),
                    TxtDireccion.Text.Trim(),
                    TxtTelefono.Text.Trim(),
                    TxtEmail.Text.Trim(),
                    TxtClave.Text.Trim()
                );

                if (respuesta == "OK")
                {
                    MensajeOK("El usuario se guardó correctamente.");
                    await this.Listar();
                    TabPrincipal.SelectedIndex = 0;
                }
                else
                {
                    MensajeError(respuesta);
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
                if (CmbRol.SelectedValue == null
                    || TxtNombre.Text == string.Empty
                    || CmbTipoDocumento.SelectedItem == null
                    || TxtNumeroDocumento.Text == string.Empty
                    || TxtEmail.Text == string.Empty)
                {
                    MensajeError("Debe completar los campos obligatorios (*).");
                    return;
                }

                if (!this.ValidarFormato())
                {
                    return;
                }

                string respuesta =  await NUsuario.Actualizar(
                    this.id,
                    Convert.ToInt32(CmbRol.SelectedValue),
                    this.emailAnt,
                    TxtNombre.Text.Trim(),
                    CmbTipoDocumento.SelectedItem.ToString(),
                    TxtNumeroDocumento.Text.Trim(),
                    TxtDireccion.Text.Trim(),
                    TxtTelefono.Text.Trim(),
                    TxtEmail.Text.Trim(),
                    TxtClave.Text.Trim()   // vacío → el SP conserva la clave actual
                );

                if (respuesta == "OK")
                {
                    MensajeOK("El usuario se actualizó correctamente.");
                }
                else
                {
                    MensajeError(respuesta);
                }

                await this.Listar();
                TabPrincipal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async Task Activar()
        {
            try
            {
                if (MessageBox.Show(
                        "¿Desea activar el/los usuario(s) seleccionado(s)?",
                        "Activar Usuarios",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes) return;

                foreach (DataGridViewRow row in DgvListado.Rows)
                {
                    if (!Convert.ToBoolean(row.Cells["Seleccionar"].Value)) continue;

                    int codigoUsuario = Convert.ToInt32(row.Cells["ID"].Value);
                    string respuesta = await NUsuario.Activar(codigoUsuario);

                    if (respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                        MensajeOK($"El usuario {row.Cells["Nombre"].Value} se activó correctamente.");
                    else
                        MensajeError(respuesta);
                }
                await this.Listar();
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }

        private async Task Desactivar()
        {
            try
            {
                if (MessageBox.Show(
                        "¿Desea desactivar el/los usuario(s) seleccionado(s)?",
                        "Desactivar Usuarios",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes) return;

                foreach (DataGridViewRow row in DgvListado.Rows)
                {
                    if (!Convert.ToBoolean(row.Cells["Seleccionar"].Value)) continue;

                    int codigoUsuario = Convert.ToInt32(row.Cells["ID"].Value);
                    string respuesta = await NUsuario.Desactivar(codigoUsuario);

                    if (respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                        MensajeOK($"El usuario {row.Cells["Nombre"].Value} se desactivó correctamente.");
                    else
                        MensajeError(respuesta);
                }
                await this.Listar();
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }

        private async Task Eliminar()
        {
            try
            {
                if (MessageBox.Show(
                        "¿Desea eliminar el/los usuario(s) seleccionado(s)?",
                        "Eliminar Usuarios",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes) return;

                foreach (DataGridViewRow row in DgvListado.Rows)
                {
                    if (!Convert.ToBoolean(row.Cells["Seleccionar"].Value)) continue;

                    int codigoUsuario = Convert.ToInt32(row.Cells["ID"].Value);
                    string respuesta = await NUsuario.Eliminar(codigoUsuario);

                    if (respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                        MensajeOK($"El usuario {row.Cells["Nombre"].Value} se eliminó correctamente.");
                    else
                        MensajeError(respuesta);
                }
                await this.Listar();
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }

        private void Cancelar()
        {
            if (MessageBox.Show("¿Desea cancelar la operación?", "Cancelar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Limpiar();
                TabPrincipal.SelectedIndex = 0;
            }
        }

        private async void FrmUsuario_Load(object sender, EventArgs e)
        {
            await this.Listar();
            await this.CargarRoles();
        }

        private async void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            await this.Buscar(TxtBuscar.Text);
        }

        private async void BtnInsertar_Click(object sender, EventArgs e)
        {
            await this.Insertar();
        }

        private async void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private async void BtnActivar_Click(object sender, EventArgs e)
        {
            await this.Activar();
        }

        private async void BtnDesactivar_Click(object sender, EventArgs e)
        {
            await this.Desactivar();  
        }

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
            await this.Eliminar();
        }

        private void ChkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            bool activo = ChkSeleccionar.Checked;
            DgvListado.Columns[0].Visible = activo;
            BtnActivar.Visible = activo;
            BtnDesactivar.Visible = activo;
            BtnEliminar.Visible = activo;
        }

        private void DgvListado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DgvListado.Columns["Seleccionar"].Index)
            {
                DataGridViewCheckBoxCell chk =
                    (DataGridViewCheckBoxCell)DgvListado.Rows[e.RowIndex].Cells["Seleccionar"];
                chk.Value = !Convert.ToBoolean(chk.Value);
            }
        }

        private void DgvListado_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Limpiar();
                BtnInsertar.Visible = false;
                BtnActualizar.Visible = true;

                id = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
                emailAnt = Convert.ToString(DgvListado.CurrentRow.Cells["Email"].Value);

                CmbRol.SelectedValue = Convert.ToString(DgvListado.CurrentRow.Cells["idrol"].Value);

                TxtNombre.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);

                // Seleccionar tipo de documento en el ComboBox
                string tipoDoc = Convert.ToString(DgvListado.CurrentRow.Cells["Tipo_Documento"].Value);
                CmbTipoDocumento.SelectedItem = tipoDoc;

                TxtNumeroDocumento.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Num_Documento"].Value);
                TxtDireccion.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Direccion"].Value);
                TxtTelefono.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Telefono"].Value);
                TxtEmail.Text = emailAnt;
                TxtClave.Clear(); // no se muestra la clave; dejar vacío para no modificarla

                TabPrincipal.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            await this.Actualizar();
        }

        private bool ValidarFormato()
        {
            string tipoDoc = CmbTipoDocumento.Text.Trim();
            string numDoc = TxtNumeroDocumento.Text.Trim();

            if (tipoDoc == "Cédula" || tipoDoc == "DNI")
            {
                string regexCedulaDNI = @"^\d{3}-?\d{7}-?\d{1}$";
                if (!Regex.IsMatch(numDoc, regexCedulaDNI))
                {
                    MensajeError("La cédula/DNI no es válida. Debe tener 11 dígitos en formato dominicano (Ej: 001-1234567-8 o sin guiones).");
                    TxtNumeroDocumento.Focus();
                    return false;
                }
            }
            else if (tipoDoc == "Pasaporte")
            {
                string regexPasaporte = @"^[a-zA-Z]\d{7}$";
                if (!Regex.IsMatch(numDoc, regexPasaporte))
                {
                    MensajeError("El pasaporte no es válido. Debe comenzar con una letra seguida de 7 números (Ej: P1234567).");
                    TxtNumeroDocumento.Focus();
                    return false;
                }
            }
            else
            {
                MensajeError("Por favor, seleccione un Tipo de Documento válido (DNI, Cédula o Pasaporte).");
                CmbTipoDocumento.Focus();
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
            string clave = TxtClave.Text.Trim();

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

            if (clave != string.Empty)
            {
                string patronClave = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$";
                if (!Regex.IsMatch(clave, patronClave))
                {
                    MensajeError("La contraseña debe tener mínimo 8 caracteres, al menos un dígito y un carácter especial");
                    TxtClave.Focus();
                    return false;
                }
            }

            return true;
        }
    }
 }

