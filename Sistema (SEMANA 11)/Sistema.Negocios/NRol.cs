using Sistema.Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Negocios
{
    public class NRol
    {
        public async static Task<DataTable> Listar()
        {
            //debemos instanciar la capa de acceso a datos
            DRol datos = new DRol();
            return await datos.Listar();
        }
    }
}
