using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // Formulario wrapper para alojar el UserControl del Dashboard
    // dentro de PnlContenedor de FrmDashBoard (patron AbrirFormulario).
    public class FrmDashboardHost : Form
    {
        public FrmDashboardHost()
        {
            Text = "Dashboard";
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.WhiteSmoke;
            ShowInTaskbar = false;

            UcDashboard Dashboard = new UcDashboard
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(Dashboard);
        }
    }
}
