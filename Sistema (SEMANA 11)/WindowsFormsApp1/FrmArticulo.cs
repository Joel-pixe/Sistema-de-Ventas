using BarcodeLib;
using Sistema.Entidad;
using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmArticulo : Form
    {
        public FrmArticulo()
        {
            InitializeComponent();
        }

        private int id;
        private string nombreAnt;
        private string RutaImagen;

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

        private void Limpiar()
        {
            TxtBuscar.Clear();
            TxtCodigo.Clear();
            TxtNombre.Clear();
            TxtPrecioVenta.Clear();
            TxtStock.Clear();
            TxtDescripcion.Clear();
            TxtImagen.Clear();
            PtbImagen.Image = null;
            PnlCodigo.BackgroundImage = null;

            BtnInsertar.Visible = true;
            BtnActualizar.Visible = false;
            BtnGenerarCodigo.Enabled = true;
            BtnGuardarCodigo.Enabled = true;

            DgvListado.Columns[0].Visible = false;

            BtnActivar.Visible = false;
            BtnDesactivar.Visible = false;
            BtnEliminar.Visible = false;

            ChkSeleccionar.Checked = false;
            LblTotal.Text = $"Total Registros: {DgvListado.Rows.Count}";
        }

        private async Task Listar()
        {
            try
            {
                DataTable tabla = await NArticulo.Listar();
                DgvListado.DataSource = tabla;
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
                this.Limpiar();
            }
            catch (Exception ex)
            {
                this.MensajeError(ex.Message);
            }
        }

        private async Task Insertar()
        {
            try
            {
                if (TxtNombre.Text == string.Empty || TxtCodigo.Text == string.Empty ||
                    TxtPrecioVenta.Text == string.Empty || TxtStock.Text == string.Empty ||
                    TxtImagen.Text == string.Empty || CmbCategoria.SelectedValue == null)
                {
                    MensajeError("Debe completar los campos obligatorios y seleccionar una categoría");
                    return;
                }

                string Respuesta = await NArticulo.Insertar(
                    Convert.ToInt32(CmbCategoria.SelectedValue),
                    TxtCodigo.Text.Trim(),
                    TxtNombre.Text.Trim(),
                    Convert.ToDecimal(TxtPrecioVenta.Text),
                    Convert.ToInt32(TxtStock.Text),
                    TxtDescripcion.Text.Trim(),
                    RutaImagen  // ruta completa — requiere VARCHAR(500) en BD
                );

                if (Respuesta == "OK")
                {
                    MensajeOK("El articulo se guardó correctamente!");
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

        private async Task Actualizar()
        {
            try
            {
                if (TxtNombre.Text == string.Empty || TxtCodigo.Text == string.Empty ||
                    TxtPrecioVenta.Text == string.Empty || CmbCategoria.SelectedValue == null)
                {
                    MensajeError("Debe completar los campos obligatorios");
                    return;
                }

                // Si no se seleccionó imagen nueva, usar la que ya estaba
                string imagenFinal = !string.IsNullOrEmpty(RutaImagen)
                                     ? RutaImagen
                                     : Convert.ToString(DgvListado.CurrentRow.Cells["Imagen"].Value);

                string Respuesta = await NArticulo.Actualizar(
                    this.id,
                    Convert.ToInt32(CmbCategoria.SelectedValue),
                    TxtCodigo.Text.Trim(),
                    this.nombreAnt,
                    TxtNombre.Text.Trim(),
                    Convert.ToDecimal(TxtPrecioVenta.Text),
                    Convert.ToInt32(TxtStock.Text),
                    TxtDescripcion.Text.Trim(),
                    imagenFinal  // ← ruta completa
                );

                if (Respuesta == "OK")
                    MensajeOK("El articulo se actualizó correctamente");
                else
                    MensajeError(Respuesta);

                await this.Listar();
                TabPrincipal.SelectedIndex = 0;
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
                DataTable tabla = await NArticulo.Buscar(valor);
                DgvListado.DataSource = tabla;
                LblTotal.Text = "Total Registros: " + tabla.Rows.Count;
                this.Formato();
            }
            catch (Exception ex)
            {
                this.MensajeError(ex.Message);
            }
        }

        private async Task Desactivar()
        {
            try
            {
                if (MessageBox.Show("Realmente desea desactivar lo(s) articulo(s) Seleccionado(s)?", "Desactivar Articulos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int codigoArticulo;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                        {
                            codigoArticulo = Convert.ToInt32(row.Cells["ID"].Value);
                            Respuesta = await NArticulo.Desactivar(codigoArticulo);

                            if (Respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                                MensajeOK($"El articulo {row.Cells["Nombre"].Value} se desactivó correctamente");
                            else
                                MensajeError(Respuesta);
                        }
                    }
                    await this.Listar();
                }
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }

        private async Task Activar()
        {
            try
            {
                if (MessageBox.Show("Realmente desea activar lo(s) articulo(s) Seleccionado(s)?", "Activar Articulos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int codigoArticulo;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                        {
                            codigoArticulo = Convert.ToInt32(row.Cells["ID"].Value);
                            Respuesta = await NArticulo.Activar(codigoArticulo);

                            if (Respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                                MensajeOK($"El articulo {row.Cells["Nombre"].Value} se activó correctamente");
                            else
                                MensajeError(Respuesta);
                        }
                    }
                    await this.Listar();
                }
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }


        private async Task Eliminar()
        {
            try
            {
                if (MessageBox.Show("Realmente desea eliminar lo(s) articulo(s) Seleccionado(s)?", "Eliminar Articulos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int codigoArticulo;
                    string Respuesta;

                    foreach (DataGridViewRow row in DgvListado.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                        {
                            codigoArticulo = Convert.ToInt32(row.Cells["ID"].Value);
                            Respuesta = await NArticulo.Eliminar(codigoArticulo);

                            if (Respuesta.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                                MensajeOK($"El articulo {row.Cells["Nombre"].Value} se eliminó correctamente");
                            else
                                MensajeError(Respuesta);
                        }
                    }
                    await this.Listar();
                }
            }
            catch (Exception ex) { MensajeError(ex.Message); }
        }

        private async Task Cancelar()
        {
            MessageBox.Show("Realmente desesa cancelar?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            this.Limpiar();
            TabPrincipal.SelectedIndex = 0;

        }

        private async Task CargarCategorias()
        {
            try
            {
                // Obtener las categorías
                DataTable datos = await NCategoria.Listar();

                // Filtrar solo las categorías activas
                if (datos.Columns.Contains("Estado"))
                {
                    DataView vista = datos.DefaultView;
                    vista.RowFilter = "Estado = true";
                    datos = vista.ToTable();
                }

                // Indicar qué se muestra y qué valor se obtiene
                CmbCategoria.DisplayMember = "Nombre";
                CmbCategoria.ValueMember = "ID";

                // Asignar el origen de datos
                CmbCategoria.DataSource = datos;

                // No seleccionar ninguna categoría al cargar
                CmbCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MensajeError("Error al cargar categorías: " + ex.Message);
            }
        }

        private async void FrmArticulo_Load(object sender, EventArgs e)
        {
            await this.Listar();
            await this.CargarCategorias();
        }

        private async void BtnInsertar_Click(object sender, EventArgs e)
        {
            await this.Insertar();
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            await this.Actualizar();
        }

        private async void BtnCancelar_Click(object sender, EventArgs e)
        {
            await this.Cancelar();
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
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)DgvListado.Rows[e.RowIndex].Cells["Seleccionar"];
                chk.Value = !Convert.ToBoolean(chk.Value);
            }
        }

        private async void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            await this.Buscar(TxtBuscar.Text);
        }

        private void DgvListado_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Limpiar();
                BtnInsertar.Visible = false;
                BtnActualizar.Visible = true;

                id = Convert.ToInt32(DgvListado.CurrentRow.Cells["ID"].Value);
                CmbCategoria.SelectedValue = Convert.ToString(DgvListado.CurrentRow.Cells["idcategoria"].Value);
                TxtCodigo.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Codigo"].Value);
                nombreAnt = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
                TxtNombre.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Nombre"].Value);
                TxtPrecioVenta.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Precio_Venta"].Value);
                TxtStock.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Stock"].Value);
                TxtDescripcion.Text = Convert.ToString(DgvListado.CurrentRow.Cells["Descripcion"].Value);

                // Cargar imagen usando la ruta completa guardada en BD
                string Imagen = Convert.ToString(DgvListado.CurrentRow.Cells["Imagen"].Value);
                if (!string.IsNullOrEmpty(Imagen) && System.IO.File.Exists(Imagen))
                {
                    PtbImagen.Image = Image.FromFile(Imagen);
                    RutaImagen = Imagen;  // ← guardar ruta completa para usar en Actualizar
                    TxtImagen.Text = Imagen.Substring(Imagen.LastIndexOf("\\") + 1);
                }
                else
                {
                    PtbImagen.Image = null;
                    TxtImagen.Clear();
                    RutaImagen = string.Empty;
                }

                // Cargar código de barras
                string codigoBarra = Convert.ToString(DgvListado.CurrentRow.Cells["Codigo"].Value);
                if (!string.IsNullOrEmpty(codigoBarra))
                {
                    var codigo = new Barcode();
                    codigo.IncludeLabel = true;
                    PnlCodigo.BackgroundImage = codigo.Encode(TYPE.CODE128, codigoBarra,
                                                 Color.Black, Color.White, 400, 80);
                    PnlCodigo.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    PnlCodigo.BackgroundImage = null;
                }

                TabPrincipal.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MensajeError(ex.Message);
            }
        }


        private void BtnCargarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivoImagen = new OpenFileDialog();
            archivoImagen.Filter = "Archivos de Imagen (jpeg, jpg, jpe, png)|*.jpeg; *.jpg; *.jpe; *.png";
            //mostrar cuadro de dialogo
            if (archivoImagen.ShowDialog() == DialogResult.OK)
            {
                PtbImagen.Image = Image.FromFile(archivoImagen.FileName);
                TxtImagen.Text = archivoImagen.FileName.Substring(archivoImagen.FileName.LastIndexOf("\\") + 1);
                RutaImagen = archivoImagen.FileName;
            }
        }

        private void BtnGenerarCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                var codigo = new Barcode();
                codigo.IncludeLabel = true;
                PnlCodigo.BackgroundImage = codigo.Encode(TYPE.CODE128, TxtCodigo.Text.Trim(), Color.Black, Color.White, 400, 80);
                BtnGenerarCodigo.Enabled = false;
            }
            catch (Exception)
            {
                this.MensajeError("Debe escribir el código antes de generarlo");
            }
            
        }

        private void BtnGuardarCodigo_Click_1(object sender, EventArgs e)
        {
            Image imagenCodigoBarras = (Image)PnlCodigo.BackgroundImage.Clone();
            //guardar la imagen
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Imagen png|*.png";
            saveFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                imagenCodigoBarras.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
            //limpie la imagen de la memoria
            imagenCodigoBarras.Dispose();
            BtnGuardarCodigo.Enabled = false;
        }
    }
}
