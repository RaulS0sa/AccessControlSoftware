using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeTest.BaseaDeDatos
{
    class UsuariosDB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tarjeta { get; set; }

        public string Matricula { get; set; }
        public string Foto { get; set; }
        public DateTime Fecha_de_ingreso { get; set; }
        public DateTime UltimoAcesso { get; set; }
        public string Tipo_de_acceso { get; set; }
        public bool Acceso_Autorizado { get; set; }
        public string Tipo_de_usuario { get; set; }


    }
}
