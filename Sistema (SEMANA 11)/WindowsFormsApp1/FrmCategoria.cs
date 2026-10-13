using Sistema.Entidad;
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
    public partial class FrmCategoria : Form
    {
        public FrmCategoria()
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
            DgvListado.Columns[2].Width = 180;
            DgvListado.Columns[2].HeaderText = "Nombre Categoría";
            DgvListado.Columns[3].Width = 370;
            DgvListado.Columns[3].HeaderText = "Descripción";
            DgvListado.Columns[4].Width = 120;
        }

        //metodo para resetear la interfaz
        private void Limpiar()
        {
            TxtBuscar.Clear();
            TxtNombre.Clear();
            TxtDescripcion.Clear();

            BtnInsertar.Visible = true;
            BtnActualizar.Visible = false;

            DgvListado.Columns[0].Visible = false;
            BtnActivar.Visible = false;
            BtnEliminar.Visible = false;
            BtnDesactivar.Visible = false;

            ChkSeleccionar.Checked = false;
        }

        //Listar las categorias
        private async Task Listar()
        {
            try
            {
                //llenar el dataGridView con el DataTable
                DgvListado.DataSource = await NCategoria.Listar();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                this.Formato();
                this.Limpiar();

            }
            catch (Exception ex)
            {

                this.MensajeError(ex.Message);
            }
        }

        //Insertar una categoría
        private async Task Insertar()
        {
            try
            {
                string Respuesta = "";
                //validar que los campos obligatorios esten completos
                if (TxtNombre.Text == string.Empty)
                {
                    this.MensajeError("Debe completar los campos obligatorios");
                }
                else
                {
                    //tratamos de insertar la categoria
                    Respuesta = await NCategoria.Insertar(TxtNombre.Text.Trim(), TxtDescripcion.Text.Trim());
                    if (Respuesta == "OK")
                    {
                        MensajeOK("La categoría se guardó correctamente!");
                        await this.Listar(); //Actualiza la vista del DGV
                        TabPrincipal.SelectedIndex = 0;
                    }
                    else
                    {
                        MensajeOK(Respuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeOK(ex.Message);
            }
        }

        //actualizar una categoria
        private async Task Actualizar()
        {
            try
            {
                string Respuesta = "";
                if (TxtNombre.Text == "")
                {
                    MensajeError("Debe completar los campos obligatorios");
                }
                else
                {
                    Respuesta = await NCategoria.Actualizar(id, nombreAnt, TxtNombre.Text.Trim(), TxtDescripcion.Text.Trim());
                    if (Respuesta == "OK")
                    {
                        MensajeOK("La categoría se actualizó correctamente");
                    }
                    else
                    {
                        MensajeOK(Respuesta);
                    }
                    await this.Listar();
                    TabPrincipal.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }

        private int id;
        private string nombreAnt;

        //Filtrar categorias
        private async Task Buscar(string valor)
        {
            try
            {
                DgvListado.DataSource = await NCategoria.Buscar(valor);
                this.Formato();
                LblTotal.Text = "Total Registros: " + Convert.ToString(DgvListado.Rows.Count);
                //this.Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Desactivar Categorías
        
        private async Task Desactivar()
        {
            try
            {
                if (MessageBox.Show("Realmente desesa desactivar la(s) categoría(s) Seleccionada(s)?", "Desactivar Categorías", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
                {
                    int codigoCategoria;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells[0].Value))
                        {
                            codigoCategoria = Convert.ToInt32(row.Cells[1].Value);
                            Respuesta = await NCategoria.Desactivar(codigoCategoria);

                            if (Respuesta == "Ok")
                            {
                                MensajeOK($"La categoría {row.Cells[2].Value} se desactivó correctamente");
                            }
                            else
                            {
                                MensajeError(Respuesta);
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

        private async Task Activar()
        {
            try
            {
                if (MessageBox.Show("Realmente desesa activar la(s) categoría(s) Seleccionada(s)?", "Activar Categorías", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int codigoCategoria;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells[0].Value))
                        {
                            codigoCategoria = Convert.ToInt32(row.Cells[1].Value);
                            Respuesta = await NCategoria.Activar(codigoCategoria);

                            if (Respuesta == "Ok")
                            {
                                MensajeOK($"La categoría {row.Cells[2].Value} se activó correctamente");
                            }
                            else
                            {
                                MensajeError(Respuesta);
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


        private async Task Eliminar()
        {
            try
            {
                if (MessageBox.Show("Realmente desesa eliminar la(s) categoría(s) Seleccionada(s)?", "Eliminar Categorías", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int codigoCategoria;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells[0].Value))
                        {
                            codigoCategoria = Convert.ToInt32(row.Cells[1].Value);
                            Respuesta = await NCategoria.Eliminar(codigoCategoria);

                            if (Respuesta == "Ok")
                            {
                                MensajeOK($"La categoría {row.Cells[2].Value} se eliminó correctamente");
                            }
                            else
                            {
                                MensajeError(Respuesta);
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

        private void Cancelar()
        {
            MessageBox.Show("Realmente desesa cancelar?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            TabPrincipal.SelectedIndex = 0;

        }

        #endregion

        private async void FrmCategoria_Load(object sender, EventArgs e)
        {
            //listamos las categorias
            await this.Listar();
        }

        private async void BtnInsertar_Click(object sender, EventArgs e)
        {
            await this.Insertar();
        }

        private void DgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || DgvListado.CurrentRow == null)
            {
                return;
            }

            // Si el doble clic fue en la columna del checkbox, no abrir modo edicion
            if (e.ColumnIndex == DgvListado.Columns["Seleccionar"].Index)
            {
                return;
            }

            try
            {
                int idSeleccionado = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
                string nombreSeleccionado = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
                string descripcionSeleccionada = Convert.ToString(DgvListado.CurrentRow.Cells["Descripcion"].Value);

                this.Limpiar();

                id = idSeleccionado;
                nombreAnt = nombreSeleccionado;
                TxtNombre.Text = nombreSeleccionado;
                TxtDescripcion.Text = descripcionSeleccionada;

                BtnInsertar.Visible = false;
                BtnActualizar.Visible = true;

                TabPrincipal.SelectedIndex = 1;
            }
            catch (Exception)
            {
                MensajeError("Se debe seleccionar un nombre de Categoría");
            }
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            await this.Actualizar();
        }

        private async void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
           await this.Buscar(TxtBuscar.Text);
        }

        private void ChkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkSeleccionar.Checked)
            {
                DgvListado.Columns[0].Visible = true;
                BtnActivar.Visible = true;
                BtnDesactivar.Visible = true;
                BtnEliminar.Visible = true;
            }
            else
            {
                DgvListado.Columns[0].Visible = false;
                BtnActivar.Visible = false;
                BtnDesactivar.Visible = false;
                BtnEliminar.Visible = false;
            }
        }

        private void DgvListado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DgvListado.Columns["Seleccionar"].Index)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell) DgvListado.Rows[e.RowIndex].Cells["Seleccionar"];
                chk.Value = !Convert.ToBoolean(chk.Value);
            }
        }

        private async void BtnDesactivar_Click(object sender, EventArgs e)
        {
            await this.Desactivar();
        }

        private async void BtnActivar_Click(object sender, EventArgs e)
        {
            await this.Activar();
        }

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
            await this.Eliminar();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar(); 
        }

    }
}
