using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace QaImgList
{
    // Herramienta QA para ImageLists de recursos:
    //  scan -> compara max(SetKeyName) de cada Designer.cs con las imagenes del ImageStream del .resx
    //  fix  -> regenera imageList1.ImageStream agregando el icono "estadisticas.png" generado
    internal static class Tool
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("uso: qa_imglist_tool.exe scan <dirProyecto>");
                Console.WriteLine("     qa_imglist_tool.exe fix  <FrmDashBoard.resx>");
                return 2;
            }

            return args[0] == "scan" ? Scan(args[1]) : Fix(args[1]);
        }

        private static int Scan(string dir)
        {
            int fallos = 0;
            foreach (string archivo in Directory.GetFiles(dir))
            {
                if (!archivo.EndsWith(".Designer.cs")) continue;
                string designer = archivo;

                string resx = Path.Combine(Path.GetDirectoryName(designer),
                    Path.GetFileName(designer).Replace(".Designer.cs", ".resx"));
                if (!File.Exists(resx)) continue;

                Dictionary<string, int> maxPorCampo = new Dictionary<string, int>();
                foreach (Match m in Regex.Matches(File.ReadAllText(designer),
                    @"(\w+)\.Images\.SetKeyName\((\d+)\s*,"))
                {
                    string campo = m.Groups[1].Value;
                    int idx = int.Parse(m.Groups[2].Value);
                    int actual;
                    maxPorCampo[campo] = maxPorCampo.TryGetValue(campo, out actual) ? Math.Max(actual, idx) : idx;
                }
                if (maxPorCampo.Count == 0) continue;

                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(resx);

                foreach (KeyValuePair<string, int> kv in maxPorCampo)
                {
                    XmlNode nodo = xml.SelectSingleNode("//data[@name='" + kv.Key + ".ImageStream']/value");
                    int imagenes = -1;
                    if (nodo != null && !string.IsNullOrWhiteSpace(nodo.InnerText))
                    {
                        try
                        {
                            byte[] bytes = Convert.FromBase64String(Normalizar(nodo.InnerText));
                            ImageListStreamer st = (ImageListStreamer)new BinaryFormatter().Deserialize(new MemoryStream(bytes));
                            ImageList il = new ImageList();
                            il.ImageStream = st;
                            imagenes = il.Images.Count;
                            il.Dispose();
                        }
                        catch (Exception ex) { imagenes = -2; Console.WriteLine("  error leyendo stream: " + ex.Message); }
                    }

                    bool ok = imagenes > kv.Value;
                    if (!ok) fallos++;
                    Console.WriteLine(string.Format("{0} [{1}] SetKeyName max={2}, imagenes={3} -> {4}",
                        Path.GetFileName(resx), kv.Key, kv.Value, imagenes, ok ? "OK" : "FALLA (crash en runtime)"));
                }
            }
            Console.WriteLine("SCAN: " + (fallos == 0 ? "sin desajustes" : fallos + " desajuste(s)"));
            return fallos;
        }

        private static string Normalizar(string base64)
        {
            return Regex.Replace(base64, @"\s", "");
        }

        private static int Fix(string resxPath)
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.Load(resxPath);

            XmlNode nodo = xml.SelectSingleNode("//data[@name='imageList1.ImageStream']/value");
            if (nodo == null) { Console.WriteLine("no se encontro imageList1.ImageStream"); return 2; }

            byte[] bytes = Convert.FromBase64String(Normalizar(nodo.InnerText));
            ImageListStreamer streamer = (ImageListStreamer)new BinaryFormatter().Deserialize(new MemoryStream(bytes));
            ImageList ilViejo = new ImageList();
            ilViejo.ImageStream = streamer;

            Size tam = ilViejo.ImageSize;
            ColorDepth prof = ilViejo.ColorDepth;
            Console.WriteLine("stream actual: " + ilViejo.Images.Count + " imagenes, tam=" + tam + ", depth=" + prof);

            List<Bitmap> imagenes = new List<Bitmap>();
            for (int i = 0; i < ilViejo.Images.Count; i++)
                imagenes.Add(Clonar(ilViejo.Images[i]));

            Bitmap icono = CrearIconoEstadisticas(tam.Width, tam.Height);
            imagenes.Add(icono);
            Console.WriteLine("nuevo stream: " + imagenes.Count + " imagenes (20 originales + estadisticas.png)");

            ImageList ilNuevo = new ImageList();
            ilNuevo.ImageSize = tam;
            ilNuevo.ColorDepth = prof;
            foreach (Bitmap bmp in imagenes) ilNuevo.Images.Add(bmp);

            MemoryStream ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, ilNuevo.ImageStream);
            nodo.InnerText = Convert.ToBase64String(ms.ToArray());

            XmlTextWriter writer = new XmlTextWriter(resxPath, new System.Text.UTF8Encoding(false));
            writer.Formatting = Formatting.Indented;
            xml.Save(writer);
            writer.Close();

            ilViejo.Dispose();
            ilNuevo.Dispose();
            foreach (Bitmap bmp in imagenes) bmp.Dispose();
            Console.WriteLine("resx actualizado: " + resxPath);
            return 0;
        }

        private static Bitmap Clonar(Image img)
        {
            Bitmap copia = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(copia))
                g.DrawImageUnscaled(img, 0, 0);
            return copia;
        }

        private static Bitmap CrearIconoEstadisticas(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float margenIzq = w * 0.14f;
                float baseY = h * 0.86f;
                float altoMax = h * 0.62f;
                float anchoBarra = Math.Max(2f, w * 0.16f);
                float separacion = (w - margenIzq * 2f - anchoBarra * 3f) / 2f;

                using (Pen eje = new Pen(Color.FromArgb(120, 90, 100, 110), Math.Max(1f, h * 0.05f)))
                    g.DrawLine(eje, margenIzq * 0.5f, baseY, w - margenIzq * 0.4f, baseY);

                Color[] colores = { Color.FromArgb(0, 120, 215), Color.FromArgb(46, 204, 113), Color.FromArgb(243, 156, 18) };
                float[] alturas = { 0.55f, 1.0f, 0.75f };

                for (int i = 0; i < 3; i++)
                {
                    float x = margenIzq + i * (anchoBarra + separacion);
                    float alto = altoMax * alturas[i];
                    RectangleF rect = new RectangleF(x, baseY - alto, anchoBarra, alto);
                    using (SolidBrush b = new SolidBrush(colores[i]))
                        g.FillRectangle(b, rect);
                }
            }
            return bmp;
        }
    }
}
