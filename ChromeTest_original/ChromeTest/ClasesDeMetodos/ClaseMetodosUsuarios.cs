using OfficeOpenXml;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeTest.ClasesDeMetodos
{
   public class ClaseMetodosUsuarios
    {
        public static string Id_del_usuario { get; set; }

        public static String Tabla_Generada { get; set; }
        public static String HTML_Generada { get; set; }

        public static byte[] Tabla_Generada_arreglo { get; set; }
        public static void Elimina_al_usuario()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

            var db = new SQLiteConnection(databasePath);
           
            int Indice_de_elemento_a_borrar_usuarios = Convert.ToInt32(Id_del_usuario);
            var query_borradora = db.Table<BaseaDeDatos.UsuariosDB>().Delete(x => x.Id == Indice_de_elemento_a_borrar_usuarios);

        }

        public static void Binari_de_excel()
        {
            try
            {
                //user_name
                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                var db1 = new SQLiteConnection(databasePath1);
                var query_accesador_Completo = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id != 0);
                byte[] nona_Completo = new byte[] { 67, 89, 89, 87 };
                ExcelPackage package_Completo = new ExcelPackage();
                var worksheet_Completo = package_Completo.Workbook.Worksheets.Add("New Sheet");
                worksheet_Completo.Cells["A1:E1"].AutoFilter = true;
                worksheet_Completo.Cells[1, 1].Value = "Nombre";
                worksheet_Completo.Cells[1, 2].Value = "Evento";

                worksheet_Completo.Cells[1, 3].Value = "Fecha";

                worksheet_Completo.Cells[1, 4].Value = "hora";
                worksheet_Completo.Cells[1, 5].Value = "Tipo de Usuario";


                int i_Completo = 2;
                foreach (var elem in query_accesador_Completo)
                {
                    worksheet_Completo.Cells[i_Completo, 1].Value = elem.Nombre;
                    worksheet_Completo.Cells[i_Completo, 2].Value = elem.Evento;
                    DateTime momento1 = (DateTime)elem.Hora;
                    DateTime momento = momento1;
                    worksheet_Completo.Cells[i_Completo, 3].Value = momento.Day.ToString() + "/" + momento.Month.ToString() + "/" + momento.Year.ToString() + "  " + momento.Hour.ToString() + ":" + momento.Minute.ToString() + ":" + momento.Second.ToString();
                    worksheet_Completo.Cells[i_Completo, 4].Value = elem.Tipo_de_usuario;



                    i_Completo++;
                }
                var mem_Completo = new MemoryStream();
                package_Completo.SaveAs(mem_Completo);
                string memString_Completo = "Memory test string !!";
                Tabla_Generada_arreglo = mem_Completo.ToArray();
            }
            catch (Exception)
            {
                //Tabla_Generada_arreglo = ;
            }
            // return  mem_Completo.ToArray(); //Encoding.ASCII.GetBytes(memString);

        }


        public static void Agarra_tabla()
        {
            try
            {
                //var link_to_down = "<a href=\"https://" + direccioncompleta + "/Descarga_excel" + "\">Visit W3Schools.com!</a>";
                var link_to_down = "<table style='width:100 %'>";
                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                var db1 = new SQLiteConnection(databasePath1);
                var query1 = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id > 0);
                link_to_down += "<tr>";
                link_to_down += "<th> Id </th>";
                link_to_down += "<th> Nombre </th>";
                link_to_down += "<th> Evento </th>";
                link_to_down += "<th> Tipo de Usuario </th>";
                link_to_down += "<th> Hora </th>";
                link_to_down += "</tr>";
                foreach (var elem in query1)
                {
                    link_to_down += "<tr>";
                    link_to_down += "<td>" + elem.Id + "</td>";
                    link_to_down += "<td>" + elem.Nombre + "</td>";
                    link_to_down += "<td>" + elem.Evento + "</td>";
                    try
                    {
                        link_to_down += "<td>" + elem.Tipo_de_usuario + "</td>";
                    }
                    catch (Exception)
                    {
                        link_to_down += "<td>" + "-" + "</td>";
                    }

                    link_to_down += "<td>" + elem.Hora + "</td>";
                    link_to_down += "</tr>";
                }
                link_to_down += "</table>";
                ClaseMetodosUsuarios.HTML_Generada = link_to_down;
            }
            catch (Exception) { }
            //return link_to_down;
        }


        public static string CSV()
        {
         
                //var link_to_down = "<a href=\"https://" + direccioncompleta + "/Descarga_excel" + "\">Visit W3Schools.com!</a>";
                var link_to_down = "";
                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                var db1 = new SQLiteConnection(databasePath1);
                var query1 = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id > 0);
                link_to_down += " Id,Nombre,Evento,Tipo de Usuario,Hora";
                link_to_down += '\n';
                foreach (var elem in query1)
                {
                    link_to_down += elem.Id + ","+elem.Nombre + ","+ elem.Evento + ","+ elem.Tipo_de_usuario+ "," + elem.Hora;
                    link_to_down += '\n';


                }

            Tabla_Generada = link_to_down;
            return link_to_down;
        }

        public static void borra_viejas_entradas()
        {
            try {
                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                var db1 = new SQLiteConnection(databasePath1);
                var query_accesador_Completo = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id != 0);
                DateTime Ahora = DateTime.Now.AddDays(-7);

                List<DateTime> fechas = new List<DateTime>();
                List<DateTime> fechas_esta_semana = new List<DateTime>();
                foreach (var elemento in query_accesador_Completo)
                {
                    int result1 = DateTime.Compare(elemento.Hora, Ahora);
                    if (result1 < 0)
                    {
                        int entero = elemento.Id;
                        try
                        {
                            var query1_borra_del_horaio = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Delete(v => v.Id == entero);
                        }
                        catch (Exception)
                        {

                        }
                        //fechas.Add(elemento.Hora);
                        //relationship = "is earlier than";
                    }
                    else if (result1 == 0)
                    {
                        //relationship = "is the same time as";
                    }
                    else
                    {
                        // fechas_esta_semana.Add(elemento.Hora);
                        //relationship = "is later than";
                    }
                }


            }
            catch (Exception) { }
            
           
        }
    }
}
