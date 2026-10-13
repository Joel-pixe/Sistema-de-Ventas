    using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ImpresorFactura
    {
        public const string Moneda = "RD$";

        public static readonly CultureInfo CulturaDO = new CultureInfo("es-DO");

        private const float AltoCabecera = 22f;
        private const float AltoFila = 20f;
        private const float ReservaCierre = 250f;

        private static readonly float[] AnchosColumna = { 80f, 250f, 60f, 110f, 120f, 150f };

        private readonly DataTable detalle;
        private readonly bool reimprimir;
        private readonly string tipoComprobante;
        private readonly string serieComprobante;
        private readonly string numComprobante;
        private readonly DateTime fecha;
        private readonly string cliente;
        private readonly string tipoDocumento;
        private readonly string numDocumento;
        private readonly string vendedor;
        private readonly decimal impuesto;
        private readonly decimal totalGuardado;
        private readonly decimal subtotal;
        private readonly decimal itbis;
        private readonly decimal totalRecalculado;

        private int filaActual;
        private int pagina;

        public ImpresorFactura(DataTable comprobante, bool reimprimir)
        {
            if (comprobante == null || comprobante.Rows.Count == 0)
            {
                throw new ArgumentException("La venta no tiene datos para imprimir");
            }

            this.detalle = comprobante;
            this.reimprimir = reimprimir;

            DataRow cabecera = comprobante.Rows[0];
            this.tipoComprobante = Convert.ToString(cabecera["TipoComprobante"]).Trim().ToUpperInvariant();
            this.serieComprobante = Convert.ToString(cabecera["SerieComprobante"]);
            this.numComprobante = Convert.ToString(cabecera["NumComprobante"]);
            this.fecha = Convert.ToDateTime(cabecera["Fecha"]);
            this.cliente = Convert.ToString(cabecera["Cliente"]);
            this.tipoDocumento = Convert.ToString(cabecera["TipoDocumento"]);
            this.numDocumento = Convert.ToString(cabecera["NumDocumento"]);
            this.vendedor = Convert.ToString(cabecera["Vendedor"]);
            this.impuesto = Convert.ToDecimal(cabecera["Impuesto"]);
            this.totalGuardado = Convert.ToDecimal(cabecera["Total"]);

            decimal baseImponible = 0;
            foreach (DataRow fila in comprobante.Rows)
            {
                decimal precio = Convert.ToDecimal(fila["Precio"]);
                int cantidad = Convert.ToInt32(fila["Cantidad"]);
                decimal descuento = Convert.ToDecimal(fila["Descuento"]);
                baseImponible += precio * cantidad - descuento;
            }

            this.subtotal = Math.Round(baseImponible, 2);
            this.itbis = Math.Round(this.subtotal * this.impuesto, 2);
            this.totalRecalculado = this.subtotal + this.itbis;
        }

        public static string Monto(decimal valor)
        {
            return Moneda + valor.ToString("N2", CulturaDO);
        }

        public static decimal MontoValor(string texto)
        {
            return decimal.Parse((texto ?? string.Empty).Replace(Moneda, string.Empty).Trim(),
                NumberStyles.Number, CulturaDO);
        }

        public void VistaPrevia(IWin32Window propietario)
        {
            if (Math.Abs(this.totalRecalculado - this.totalGuardado) > 0.01m)
            {
                MessageBox.Show(
                    "El total recalculado " + Monto(this.totalRecalculado) +
                    " difiere del total registrado " + Monto(this.totalGuardado) +
                    ". La factura se imprimir\u00e1 con el total recalculado.",
                    "Sistema de Ventas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using (PrintDocument documento = new PrintDocument())
            using (PrintPreviewDialog vista = new PrintPreviewDialog())
            {
                documento.DefaultPageSettings.PaperSize = new PaperSize("Carta", 850, 1100);
                documento.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
                documento.BeginPrint += Documento_BeginPrint;
                documento.PrintPage += Documento_PrintPage;
                vista.Document = documento;
                vista.ShowInTaskbar = false;
                vista.Width = 900;
                vista.Height = 1000;
                vista.ShowDialog(propietario);
            }
        }

        private void Documento_BeginPrint(object sender, PrintEventArgs e)
        {
            this.filaActual = 0;
            this.pagina = 0;
        }

        private void Documento_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle area = e.MarginBounds;
            float y = area.Top;
            this.pagina++;

            using (Recursos f = new Recursos())
            {
                y = DibujarEncabezado(e.Graphics, area, y, f);

                if (this.filaActual < this.detalle.Rows.Count)
                {
                    if (y + AltoCabecera + AltoFila > area.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    y = DibujarCabeceraTabla(e.Graphics, area, y, f);

                    while (this.filaActual < this.detalle.Rows.Count)
                    {
                        if (y + AltoFila > area.Bottom)
                        {
                            e.HasMorePages = true;
                            return;
                        }

                        DibujarFilaDetalle(e.Graphics, area.Left, y, this.detalle.Rows[this.filaActual], f);
                        y += AltoFila;
                        this.filaActual++;
                    }
                }

                if (y + ReservaCierre > area.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                DibujarCierre(e.Graphics, area, y, f);
            }
        }

        private float DibujarEncabezado(Graphics g, Rectangle area, float y, Recursos f)
        {
            float x = area.Left;
            float ancho = area.Width;

            g.DrawString("P\u00e1gina " + this.pagina, f.FuenteDatos, Brushes.Gray,
                new RectangleF(area.Right - 120f, y, 120f, 15f), f.Derecha);
            y += 20f;

            if (this.pagina > 1)
            {
                g.DrawString(Empresa.Nombre, f.FuenteNegrita, Brushes.Black,
                    new RectangleF(x, y, ancho, 16f), f.Centro);
                y += 18f;

                string lineaContinuacion = TituloComprobante() + "  -  NCF: " + this.serieComprobante + this.numComprobante;
                if (this.reimprimir)
                {
                    lineaContinuacion += "  -  REIMPRESI\u00d3N";
                }
                g.DrawString(lineaContinuacion, f.FuenteDatos, Brushes.Black,
                    new RectangleF(x, y, ancho, 15f), f.Centro);
                y += 16f;

                g.DrawLine(f.Acento, x, y + 2f, area.Right, y + 2f);
                return y + 12f;
            }

            if (!string.IsNullOrWhiteSpace(Empresa.Nombre))
            {
                g.DrawString(Empresa.Nombre, f.FuenteEmpresa, Brushes.Black,
                    new RectangleF(x, y, ancho, 28f), f.Centro);
                y += 30f;
            }
            if (!string.IsNullOrWhiteSpace(Empresa.Rnc))
            {
                g.DrawString("RNC: " + Empresa.Rnc, f.FuenteDatos, Brushes.Black,
                    new RectangleF(x, y, ancho, 15f), f.Centro);
                y += 16f;
            }
            if (!string.IsNullOrWhiteSpace(Empresa.Direccion))
            {
                g.DrawString(Empresa.Direccion, f.FuenteDatos, Brushes.Black,
                    new RectangleF(x, y, ancho, 15f), f.Centro);
                y += 16f;
            }

            g.DrawLine(f.Acento, x, y + 2f, area.Right, y + 2f);
            y += 12f;

            if (this.reimprimir)
            {
                g.DrawString("REIMPRESI\u00d3N", f.FuenteReimpresion, Brushes.Firebrick,
                    new RectangleF(x, y, ancho, 24f), f.Centro);
                y += 26f;
            }

            g.DrawString(TituloComprobante(), f.FuenteTitulo, Brushes.Black,
                new RectangleF(x, y, ancho, 24f), f.Centro);
            y += 30f;

            RectangleF cajaNcf = new RectangleF(area.Right - 300f, y, 300f, 52f);
            g.DrawRectangle(f.Linea, cajaNcf.X, cajaNcf.Y, cajaNcf.Width, cajaNcf.Height);
            g.DrawString("NCF", f.FuenteNegrita, Brushes.Black,
                new RectangleF(cajaNcf.X + 8f, cajaNcf.Y + 5f, cajaNcf.Width - 16f, 15f), f.Izquierdo);
            g.DrawString(this.serieComprobante + this.numComprobante, f.FuenteNcf, Brushes.Black,
                new RectangleF(cajaNcf.X + 8f, cajaNcf.Y + 21f, cajaNcf.Width - 16f, 26f), f.Izquierdo);

            float yIzquierda = y + 6f;
            g.DrawString("Fecha: " + this.fecha.ToString("dd/MM/yyyy"), f.FuenteDatos, Brushes.Black,
                new RectangleF(x, yIzquierda, 440f, 15f), f.Izquierdo);
            yIzquierda += 18f;
            g.DrawString("Vendedor: " + this.vendedor, f.FuenteDatos, Brushes.Black,
                new RectangleF(x, yIzquierda, 440f, 15f), f.Izquierdo);
            y += 64f;

            RectangleF cajaCliente = new RectangleF(x, y, ancho, 48f);
            g.FillRectangle(f.Fondo, cajaCliente);
            g.DrawRectangle(f.Linea, cajaCliente.X, cajaCliente.Y, cajaCliente.Width, cajaCliente.Height);
            g.DrawString("Cliente: " + this.cliente, f.FuenteNegrita, Brushes.Black,
                new RectangleF(x + 8f, y + 5f, ancho - 16f, 17f), f.Izquierdo);
            g.DrawString(EtiquetaDocumento() + ": " + this.numDocumento, f.FuenteDatos, Brushes.Black,
                new RectangleF(x + 8f, y + 25f, ancho - 16f, 17f), f.Izquierdo);

            return y + 62f;
        }

        private float DibujarCabeceraTabla(Graphics g, Rectangle area, float y, Recursos f)
        {
            string[] encabezados = { "C\u00d3DIGO", "ART\u00cdCULO", "CANT.", "PRECIO", "DESCUENTO", "IMPORTE" };
            float x = area.Left;

            g.FillRectangle(f.FondoCabecera, new RectangleF(area.Left, y, area.Width, AltoCabecera));

            for (int i = 0; i < AnchosColumna.Length; i++)
            {
                RectangleF celda = new RectangleF(x, y, AnchosColumna[i], AltoCabecera);
                g.DrawString(encabezados[i], f.FuenteNegrita, Brushes.Black, celda, f.Centro);
                g.DrawRectangle(f.Linea, celda.X, celda.Y, celda.Width, celda.Height);
                x += AnchosColumna[i];
            }

            return y + AltoCabecera;
        }

        private void DibujarFilaDetalle(Graphics g, float x, float y, DataRow fila, Recursos f)
        {
            decimal precio = Convert.ToDecimal(fila["Precio"]);
            int cantidad = Convert.ToInt32(fila["Cantidad"]);
            decimal descuento = Convert.ToDecimal(fila["Descuento"]);

            string[] valores =
            {
                Convert.ToString(fila["Codigo"]),
                Convert.ToString(fila["Articulo"]),
                cantidad.ToString(CulturaDO),
                Monto(precio),
                Monto(descuento),
                Monto(precio * cantidad - descuento)
            };

            for (int i = 0; i < AnchosColumna.Length; i++)
            {
                bool derecha = i >= 2;
                RectangleF celda = derecha
                    ? new RectangleF(x + 4f, y, AnchosColumna[i] - 10f, AltoFila)
                    : new RectangleF(x + 6f, y, AnchosColumna[i] - 10f, AltoFila);

                g.DrawString(valores[i], f.FuenteDatos, Brushes.Black, celda,
                    derecha ? f.Derecha : f.IzquierdoRecorte);
                g.DrawRectangle(f.Linea, x, y, AnchosColumna[i], AltoFila);
                x += AnchosColumna[i];
            }
        }

        private void DibujarCierre(Graphics g, Rectangle area, float y, Recursos f)
        {
            float x = area.Left;
            float ancho = area.Width;
            float anchoCaja = 340f;
            float xCaja = area.Right - anchoCaja;

            g.DrawRectangle(f.Linea, xCaja, y, anchoCaja, 72f);
            DibujarLineaTotal(g, xCaja, y, anchoCaja, "Subtotal", Monto(this.subtotal), f, false);
            g.DrawLine(f.Linea, xCaja, y + 24f, area.Right, y + 24f);
            DibujarLineaTotal(g, xCaja, y + 24f, anchoCaja, "ITBIS " + Porcentaje(), Monto(this.itbis), f, false);
            g.DrawLine(f.Linea, xCaja, y + 48f, area.Right, y + 48f);
            DibujarLineaTotal(g, xCaja, y + 48f, anchoCaja, "Total", Monto(this.totalRecalculado), f, true);
            y += 86f;

            g.DrawString(Leyenda(), f.FuenteDatos, Brushes.Black,
                new RectangleF(x, y, ancho, 34f), f.CentroAjuste);
            y += 50f;

            float anchoFirma = 300f;
            float xFirma = x + (ancho - anchoFirma) / 2f;
            g.DrawLine(f.Linea, xFirma, y, xFirma + anchoFirma, y);
            y += 8f;
            g.DrawString("Firma del vendedor", f.FuenteDatos, Brushes.Black,
                new RectangleF(x, y, ancho, 16f), f.Centro);
            y += 18f;
            g.DrawString("Vendedor: " + this.vendedor, f.FuenteDatos, Brushes.Black,
                new RectangleF(x, y, ancho, 16f), f.Centro);
            y += 26f;
            g.DrawString("Gracias por su compra", f.FuenteGracias, Brushes.Black,
                new RectangleF(x, y, ancho, 22f), f.Centro);
        }

        private void DibujarLineaTotal(Graphics g, float x, float y, float ancho,
            string etiqueta, string valor, Recursos f, bool resaltar)
        {
            Font fuente = resaltar ? f.FuenteTotal : f.FuenteDatos;
            g.DrawString(etiqueta, fuente, Brushes.Black,
                new RectangleF(x + 8f, y, ancho * 0.5f, 24f), f.IzquierdoCentro);
            g.DrawString(valor, fuente, Brushes.Black,
                new RectangleF(x + ancho * 0.4f, y, ancho * 0.6f - 8f, 24f), f.Derecha);
        }

        private string TituloComprobante()
        {
            switch (this.tipoComprobante)
            {
                case "FACTURA":
                    return "Comprobante de Cr\u00e9dito Fiscal";
                case "TICKET":
                case "BOLETA":
                    return "Comprobante de Consumo";
                default:
                    return string.IsNullOrEmpty(this.tipoComprobante) ? "Comprobante" : this.tipoComprobante;
            }
        }

        private string Leyenda()
        {
            switch (this.tipoComprobante)
            {
                case "FACTURA":
                    return "Comprobante de Cr\u00e9dito Fiscal - conserve este documento para efectos de cr\u00e9dito fiscal.";
                case "TICKET":
                case "BOLETA":
                    return "Comprobante de Consumo - conserve este comprobante.";
                default:
                    return "Documento comercial - conserve una copia.";
            }
        }

        private string EtiquetaDocumento()
        {
            string digitos = new string(this.numDocumento.Where(char.IsDigit).ToArray());
            if (digitos.Length == 11)
            {
                return "C\u00e9dula";
            }
            if (digitos.Length == 9)
            {
                return "RNC";
            }
            return string.IsNullOrWhiteSpace(this.tipoDocumento) ? "Documento" : this.tipoDocumento;
        }

        private string Porcentaje()
        {
            return (this.impuesto * 100m).ToString("0.##", CulturaDO) + "%";
        }

        private sealed class Recursos : IDisposable
        {
            public readonly Font FuenteEmpresa = new Font("Arial", 14f, FontStyle.Bold);
            public readonly Font FuenteTitulo = new Font("Arial", 12f, FontStyle.Bold);
            public readonly Font FuenteReimpresion = new Font("Arial", 12f, FontStyle.Bold);
            public readonly Font FuenteNcf = new Font("Arial", 11f, FontStyle.Bold);
            public readonly Font FuenteTotal = new Font("Arial", 11f, FontStyle.Bold);
            public readonly Font FuenteGracias = new Font("Arial", 11f, FontStyle.Bold);
            public readonly Font FuenteNegrita = new Font("Arial", 9f, FontStyle.Bold);
            public readonly Font FuenteDatos = new Font("Arial", 9f);

            public readonly Pen Linea = new Pen(Color.Black, 0.7f);
            public readonly Pen Acento = new Pen(Color.SteelBlue, 2f);

            public readonly Brush Fondo = new SolidBrush(Color.FromArgb(245, 245, 245));
            public readonly Brush FondoCabecera = new SolidBrush(Color.FromArgb(225, 225, 225));

            public readonly StringFormat Centro = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            public readonly StringFormat CentroAjuste = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            public readonly StringFormat Izquierdo = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            public readonly StringFormat IzquierdoCentro = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            public readonly StringFormat IzquierdoRecorte = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            public readonly StringFormat Derecha = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            public void Dispose()
            {
                this.FuenteEmpresa.Dispose();
                this.FuenteTitulo.Dispose();
                this.FuenteReimpresion.Dispose();
                this.FuenteNcf.Dispose();
                this.FuenteTotal.Dispose();
                this.FuenteGracias.Dispose();
                this.FuenteNegrita.Dispose();
                this.FuenteDatos.Dispose();
                this.Linea.Dispose();
                this.Acento.Dispose();
                this.Fondo.Dispose();
                this.FondoCabecera.Dispose();
                this.Centro.Dispose();
                this.CentroAjuste.Dispose();
                this.Izquierdo.Dispose();
                this.IzquierdoCentro.Dispose();
                this.IzquierdoRecorte.Dispose();
                this.Derecha.Dispose();
            }
        }
    }
}
