using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmRecuperarContraseña : Form
    {
        private string codigoGenerado;
        private string emailUsuario;
        private int idUsuarioRecuperacion;
        private DateTime horaGeneracionCodigo;

        private const string CorreoEmisor = "joel.arturo.jimenez.objio@gmail.com";
        private const string ContrasenaEmisor = "ipue byys fxjn wrsg";
        public FrmRecuperarContraseña()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern int ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);


        private void BtnX_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmRecuperarContraseña_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BtnCambiarPassword_Click(object sender, EventArgs e)
        {
           
        }

        private void FrmRecuperarContraseña_Load(object sender, EventArgs e)
        {
            PnlRecuperar.Visible = true;
            PnlRecuperar2.Visible = false;
            PnlRecuperar3.Visible = false;

        }

        private async void BtnEnviarCodigo_Click(object sender, EventArgs e)
        {
            string correoDestino = TxtEmail.Text.Trim();

            if (!ValidarCorreo(correoDestino))
            {
                MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable tabla = await NUsuario.BuscarPorEmail(correoDestino);
            if (tabla.Rows.Count == 0)
            {
                MessageBox.Show("No existe ningún usuario registrado con ese correo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            idUsuarioRecuperacion = Convert.ToInt32(tabla.Rows[0]["ID"]);

            Random rand = new Random();
            string codigoSeguridad = rand.Next(100000, 999999).ToString();

            bool enviado = EnviarCorreo(correoDestino, codigoSeguridad);
            if (enviado)
            {
                codigoGenerado = codigoSeguridad;
                emailUsuario = correoDestino;
                horaGeneracionCodigo = DateTime.Now;

                MessageBox.Show("El código se ha enviado correctamente.");
                PnlRecuperar.Visible = false;
                PnlRecuperar2.Visible = true;
            }
            else
            {
                MessageBox.Show("Hubo un error al enviar el correo. Inténtalo de nuevo más tarde.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

            private bool ValidarCorreo(string correo)
        {
            string expresion = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, expresion);
        }

        // Función interna para el envío de correos vía SMTP
        private bool EnviarCorreo(string correoDestino, string codigo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(CorreoEmisor, "Sistema de Ventas");
                mail.To.Add(correoDestino);
                mail.Subject = "Código de Recuperación de Contraseña";
                mail.Body = $@"
            <h3>Recuperación de Contraseña</h3>
            <p>Has solicitado restablecer tu contraseña en el Sistema de Ventas.</p>
            <p>Tu código de seguridad es: <b>{codigo}</b></p>
            <p>Si no solicitaste este cambio, ignora este correo.</p>";
                mail.IsBodyHtml = true;

                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(CorreoEmisor, ContrasenaEmisor);
                smtp.EnableSsl = true;

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        private void BtnVerficarCodigo_Click(object sender, EventArgs e)
        {
            string codigoIngresado = TxtCodigoVerificacion.Text.Trim();

            if (string.IsNullOrEmpty(codigoIngresado) || codigoIngresado == "Código")
            {
                MessageBox.Show("Por favor, ingrese el código de seguridad.", "Campo Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((DateTime.Now - horaGeneracionCodigo).TotalMinutes > 10)
            {
                MessageBox.Show("El código ha expirado, solicita uno nuevo.", "Código Expirado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PnlRecuperar2.Visible = false;
                PnlRecuperar.Visible = true;
                return;
            }

            if (codigoIngresado.Equals(codigoGenerado))
            {
                PnlRecuperar.Visible = false;
                PnlRecuperar2.Visible = false;
                PnlRecuperar3.Visible = true;
            }
            else
            {
                MessageBox.Show("El código es incorrecto. Por favor, verifíquelo e inténtelo de nuevo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCambiar_Click(object sender, EventArgs e)
        {
            string nuevaClave = TxtNuevaContraseña.Text.Trim();
            string confirmarClave = TxtConfirmarContraseña.Text.Trim();

            if (string.IsNullOrEmpty(nuevaClave) || nuevaClave == "Contraseña" ||
                string.IsNullOrEmpty(confirmarClave) || confirmarClave == "Contraseña")
            {
                MessageBox.Show("Debe completar ambos campos.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nuevaClave != confirmarClave)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string patronClave = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$";
            if (!Regex.IsMatch(nuevaClave, patronClave))
            {
                MessageBox.Show("La contraseña debe tener mínimo 8 caracteres, un dígito y un carácter especial.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resultado = await NUsuario.CambiarClave(idUsuarioRecuperacion, nuevaClave);

            if (resultado == "OK")
            {
                MessageBox.Show("Tu contraseña se cambió correctamente. Ya puedes iniciar sesión.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ocurrió un error: " + resultado, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            if (TxtEmail.Text == "")
            {
                TxtEmail.Text = "Correo";
            }
        }

        private void TxtEmail_Enter(object sender, EventArgs e)
        {
            if (TxtEmail.Text == "Correo")
            {
                TxtEmail.Clear();
            }
        }



        private void TxtCodigoVerificacion_Leave(object sender, EventArgs e)
        {
            if (TxtCodigoVerificacion.Text == "")
            {
                TxtCodigoVerificacion.Text = "Código";
            }
        }

        private void TxtCodigoVerificacion_Enter(object sender, EventArgs e)
        {
            if (TxtCodigoVerificacion.Text == "Código")
            {
                TxtCodigoVerificacion.Clear();


            }
        }
        private void TxtNuevaContraseña_Enter(object sender, EventArgs e)
        {
            if (TxtNuevaContraseña.Text == "Contraseña")
            {
                TxtNuevaContraseña.Clear();
            }
        }

        private void TxtNuevaContraseña_Leave(object sender, EventArgs e)
        {
            if (TxtNuevaContraseña.Text == "")
            {
                TxtNuevaContraseña.Text = "Contraseña";
            }
        }

        private void TxtConfirmarContraseña_Enter(object sender, EventArgs e)
        {
            if (TxtConfirmarContraseña.Text == "Contraseña")
            {
                TxtConfirmarContraseña.Clear();
            }
        }

        private void TxtConfirmarContraseña_Leave(object sender, EventArgs e)
        {
            if (TxtConfirmarContraseña.Text == "")
            {
                TxtConfirmarContraseña.Text = "Contraseña";
            }
        }
    }
}
