using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChromeTest.BaseaDeDatos
{
    class HorairosDeAcceso
    {
        // Hora ID  Nombre Tarjeta Dispositivo Torniquete  Evento
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Id_de_usuario { get; set; }
        public string Tarjeta { get; set; }
        public string Dispositivo { get; set; }
        public string Torniquete { get; set; }
        public string Evento { get; set; }
        public DateTime Hora{ get; set; }
        public string Tipo_de_usuario { get; set; }
    }
}
