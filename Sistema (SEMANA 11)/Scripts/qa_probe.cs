using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace QaProbe
{
    // Probe de QA: valida (1) la cadena de conexion real de la app, (2) el fix
    // TrustServerCertificate=True, (3) la clase Conexion de la capa Datos y
    // (4) la construccion de FrmLogin (recurso preserializado + binding redirect).
    internal static class Probe
    {
        [STAThread]
        private static int Main()
        {
            int fallos = 0;

            try
            {
                using (var cn = new SqlConnection("Server=localhost\\SQLEXPRESS; Database=mydatabase2;Integrated Security = SSPI"))
                {
                    cn.Open();
                    Console.WriteLine("[1] CADENA_APP: OK (versin servidor " + cn.ServerVersion + ")");
                }
            }
            catch (Exception ex)
            {
                fallos++;
                Console.WriteLine("[1] CADENA_APP: FAIL " + ex.GetType().Name + ": " + ex.Message
                    + " | inner: " + (ex.InnerException == null ? "-" : ex.InnerException.Message));
            }

            try
            {
                using (var cn = new SqlConnection("Server=localhost\\SQLEXPRESS; Database=mydatabase2;Integrated Security = SSPI; TrustServerCertificate=True"))
                {
                    cn.Open();
                    Console.WriteLine("[2] FIX_TRUSTCERT: OK (versin servidor " + cn.ServerVersion + ")");
                }
            }
            catch (Exception ex)
            {
                fallos++;
                Console.WriteLine("[2] FIX_TRUSTCERT: FAIL " + ex.GetType().Name + ": " + ex.Message);
            }

            try
            {
                using (SqlConnection cn = Sistema.Datos.Conexion.ObtenerInstancia().CrearConexion())
                {
                    cn.Open();
                    Console.WriteLine("[3] CLASE Conexion: OK (" + cn.Database + ")");
                }
            }
            catch (Exception ex)
            {
                fallos++;
                Console.WriteLine("[3] CLASE Conexion: FAIL " + ex.GetType().Name + ": " + ex.Message);
            }

            try
            {
                using (Form f = new WindowsFormsApp1.FrmLogin())
                    Console.WriteLine("[4] FrmLogin ctor: OK (recurso preserializado cargado con binding redirect)");
            }
            catch (Exception ex)
            {
                fallos++;
                Console.WriteLine("[4] FrmLogin ctor: FAIL " + ex.GetType().Name + ": " + ex.Message);
            }

            try
            {
                using (Form f = new WindowsFormsApp1.FrmDashBoard())
                {
                    FieldInfo fi = typeof(WindowsFormsApp1.FrmDashBoard).GetField("imageList1",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    ImageList il = (ImageList)fi.GetValue(f);
                    bool iconoOk = il.Images.Count == 21 && il.Images.ContainsKey("estadisticas.png");
                    Console.WriteLine("[5] FrmDashBoard ctor: OK | imagenes=" + il.Images.Count
                        + " (esperado 21), clave 'estadisticas.png'=" + il.Images.ContainsKey("estadisticas.png"));
                    if (!iconoOk) fallos++;
                }
            }
            catch (Exception ex)
            {
                fallos++;
                Console.WriteLine("[5] FrmDashBoard ctor: FAIL");
                Console.WriteLine(ex.ToString());
            }

            // [6] Diagnóstico: cómo leer la visibilidad propia (flag sin heredar)
            try
            {
                Button b1 = new Button();
                Button b2 = new Button();
                b2.Visible = false;
                Console.WriteLine("[6] b1.Visible=" + b1.Visible + ", b2.Visible=" + b2.Visible);
                foreach (FieldInfo fi in typeof(Control).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                    if (fi.Name.IndexOf("state", StringComparison.OrdinalIgnoreCase) >= 0
                     || fi.Name.IndexOf("vis", StringComparison.OrdinalIgnoreCase) >= 0)
                        Console.WriteLine("[6]   campo " + fi.Name + " : " + fi.FieldType.Name);
                foreach (MethodInfo mi in typeof(Control).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                    if (mi.Name.Contains("State") || mi.Name.Contains("Visible"))
                        Console.WriteLine("[6]   metodo " + mi.Name + " -> " + mi.ReturnType.Name);
                MethodInfo gs = typeof(Control).GetMethod("GetState", BindingFlags.NonPublic | BindingFlags.Instance);
                if (gs != null)
                    for (uint m = 1; m != 0; m <<= 1)
                    {
                        object v1 = gs.Invoke(b1, new object[] { unchecked((int)m) });
                        object v2 = gs.Invoke(b2, new object[] { unchecked((int)m) });
                        if (!Equals(v1, v2))
                            Console.WriteLine("[6]   mascara que distingue visibilidad: 0x" + m.ToString("X")
                                + " (b1=" + v1 + ", b2=" + v2 + ")");
                    }
            }
            catch (Exception ex) { Console.WriteLine("[6] FAIL " + ex); }

            Console.WriteLine("PROBE: " + (fallos == 0 ? "TODO OK" : fallos + " FALLOS"));
            return fallos;
        }
    }
}
