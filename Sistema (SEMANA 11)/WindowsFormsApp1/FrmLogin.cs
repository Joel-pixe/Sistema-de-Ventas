using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern int ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);



        private void FrmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BtnX_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TxtEmail_Enter(object sender, EventArgs e)
        {
            if (TxtEmail.Text == "Usuario")
            {
                TxtEmail.Clear();
            }
        }

        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            if (TxtEmail.Text == "")
            {
                TxtEmail.Text = "Usuario";
            }
        }

        private void TxtClave_Enter(object sender, EventArgs e)
        {
            if (TxtClave.Text == "Clave")
            {
                TxtClave.Clear();
            }
        }

        private void TxtClave_Leave(object sender, EventArgs e)
        {
            if (TxtClave.Text == "")
            {
                TxtClave.Text = "Clave";
                TxtClave.PasswordChar = '\0';
            }
        }

        //funcion que valide que el correo tenga el formato correcto
        // y que la contrasena tenga 8 caracteres minimo, al menos 1 digito y un caracter
        //especial El metodo tiene que retornar true
        private async void BtnAcceder_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ValidarFormato())
                {
                    DataTable Tabla = new DataTable();
                    Tabla = await NUsuario.Login(TxtEmail.Text.Trim(), TxtClave.Text.Trim());
                    if (Tabla.Rows.Count <= 0)
                    {
                        MessageBox.Show("El Email o la Clave son Incorretas", "Acceso al Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (Convert.ToBoolean(Tabla.Rows[0][4]) == false)
                    {
                        MessageBox.Show("Usuario Inactivo, Contacte al adminstrador", "Acceso al Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //Instanciar el Formulario Principal
                        FrmDashBoard frm = new FrmDashBoard();
                        //Pasarle el Usuario
                        frm.Idusuario = Convert.ToInt32(Tabla.Rows[0][0]);
                        Variables.IdUsuario = Convert.ToInt32(Tabla.Rows[0][0]);
                        frm.Idrol = Convert.ToInt32(Tabla.Rows[0][1]);
                        frm.Rol = Convert.ToString(Tabla.Rows[0][2]);
                        Variables.Rol = Convert.ToString(Tabla.Rows[0][2]);
                        frm.Nombre = Convert.ToString((Tabla.Rows[0][3]));
                        frm.Estado = Convert.ToBoolean((Tabla.Rows[0][4]));
                        //Mostrar el Formulario
                        frm.Show();
                        //Esconder el Login
                        this.Hide(); 

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void TxtClave_TextChanged(object sender, EventArgs e)
        {
            TxtClave.PasswordChar = '*';
        }

        private void BtnShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            TxtClave.PasswordChar = '\0';
        }

        private void BtnShowPassword_MouseLeave(object sender, EventArgs e)
        {
            TxtClave.PasswordChar = '*';
        }

        private bool ValidarFormato()
        {
            string email = TxtEmail.Text.Trim();
            string clave = TxtClave.Text.Trim();

            try
            {
                MailAddress direccion = new MailAddress(email);

                if (!direccion.Host.Contains("."))
                {
                    MessageBox.Show("El formato del correo no es válido", "Acceso al Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("El formato del correo no es válido", "Acceso al Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string patronClave = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$";
            if (!Regex.IsMatch(clave, patronClave))
            {
                MessageBox.Show("La contraseña debe tener mínimo 8 caracteres, al menos un dígito y un carácter especial", "Acceso al Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void Olvidaste_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRecuperarContraseña frm = new FrmRecuperarContraseña();
            frm.ShowDialog();
        }
    }
}
