using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1.Helpers
{
    public static class ChartExportHelper
    {
        // Exportar a PNG - placeholder (el gráfico se exporta desde el botón del toolbar en UcVentasPeriodo)
        public static void ExportToPng(CartesianChart chart, string filePath)
        {
            if (chart == null) throw new ArgumentNullException(nameof(chart));

            // Create a simple bitmap with chart info
            var bitmap = new SKBitmap(800, 600, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                using (var font = new SKFont(SKTypeface.FromFamilyName("Arial"), 24))
                using (var paint = new SKPaint { Color = SKColors.DimGray, IsAntialias = true })
                {
                    canvas.DrawText("Gráfico de Ventas por Período", 50, 100, SKTextAlign.Left, font, paint);
                    using (var font2 = new SKFont(SKTypeface.FromFamilyName("Arial"), 14))
                    {
                        canvas.DrawText("Para exportar la imagen del gráfico, use el botón", 50, 150, SKTextAlign.Left, font2, paint);
                        canvas.DrawText("'Exportar PNG' en la barra de herramientas del gráfico.", 50, 180, SKTextAlign.Left, font2, paint);
                    }
                }
            }

            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (FileStream fs = File.OpenWrite(filePath))
                data.SaveTo(fs);
        }

        // Exportar a PDF (una página, título + filtros)
        public static void ExportToPdf(CartesianChart chart, string filePath, string title, Sistema.Entidad.FilterDto filtro)
        {
            if (chart == null) throw new ArgumentNullException(nameof(chart));

            const float pageWidth = 595f;  // A4 a 72 DPI
            const float pageHeight = 842f;
            const float margin = 40f;

            using (SKDocument doc = SKDocument.CreatePdf(filePath))
            using (SKCanvas canvas = doc.BeginPage(pageWidth, pageHeight))
            {
                canvas.Clear(SKColors.White);

                // Título
                using (var titleFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 18))
                using (var titlePaint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                {
                    canvas.DrawText(title, margin, margin + titleFont.Size, SKTextAlign.Left, titleFont, titlePaint);
                }

                // Subtítulo con fecha y filtros
                string subInfo = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}  |  " +
                                 $"Rango: {filtro.FechaInicio:dd/MM/yyyy} - {filtro.FechaFin:dd/MM/yyyy}  |  " +
                                 $"Granularidad: {filtro.Granularidad}  |  Métrica: {filtro.Metrica}";
                if (filtro.IdCategoria.HasValue) subInfo += $"  |  Categoría: {filtro.IdCategoria}";
                if (filtro.IdUsuario.HasValue) subInfo += $"  |  Vendedor: {filtro.IdUsuario}";

                using (var subFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 10))
                using (var subPaint = new SKPaint { Color = SKColors.Gray, IsAntialias = true })
                {
                    canvas.DrawText(subInfo, margin, margin + 40, SKTextAlign.Left, subFont, subPaint);
                }

                // Nota
                using (var noteFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 12))
                using (var notePaint = new SKPaint { Color = SKColors.DimGray, IsAntialias = true })
                {
                    canvas.DrawText("Gráfico disponible como PNG. Use 'Exportar PNG' en la barra del gráfico.", margin, margin + 80, SKTextAlign.Left, noteFont, notePaint);
                }

                // Pie de página
                using (var footFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 8))
                using (var footPaint = new SKPaint { Color = SKColors.LightGray, IsAntialias = true })
                {
                    canvas.DrawText("Sistema SEMANA 11 - Módulo Estadísticas", margin, pageHeight - 20, SKTextAlign.Left, footFont, footPaint);
                }

                doc.EndPage();
            }
        }

        // Helper para obtener SaveFileDialog configurado
        public static SaveFileDialog CreateSaveDialog(string tabName, string extension)
        {
            return new SaveFileDialog
            {
                Filter = extension == ".pdf" ? "PDF (*.pdf)|*.pdf" : "PNG (*.png)|*.png",
                FileName = $"Estadisticas_{tabName}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}",
                Title = $"Exportar gráfico a {extension.ToUpper()}"
            };
        }
    }
}