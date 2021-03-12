using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeTest.BaseaDeDatos
{
    class DispositivosDeAccesoConectados
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Puerto { get; set; }
        public string Pseudonimo { get; set; }
        public bool AllowAccess { get; set; }
        public int Accesos_Por_dia { get; set; }
        public string Ultima_averia { get; set; }
        public DateTime Ultimo_Mantenimiento { get; set; }
        public DateTime Ultimo_Acceso { get; set; }
    }
}
