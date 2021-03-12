using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ChromeTest.Demos
{
    public partial class BootStrapForm2 : Form
    {
        ChromiumWebBrowser m_chromeBrowser = null;

        SomeClass m_javascriptSvc = null;
        public static List<string> Puertos = new List<string>();
        public static SerialPort serial1;
        public static string lectura_ant { get; set; }
        private static System.Timers.Timer aTimer;

        public BootStrapForm2()
        {
       
            InitializeComponent();
            this.KeyPreview = true;
        }
       

        public static string GetAppLocation()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private void BootStrapForm2_Load(object sender, EventArgs e)
        {
            try
            {
                Coms();
                serial1 = new SerialPort(Puertos.FirstOrDefault(), 115200,       // baud rate
                Parity.None, // parity
                8,           // bits
                StopBits.One // stop bits
                );

                serial1.Handshake = Handshake.None;
                serial1.ReadTimeout = 1000; 
                serial1.WriteTimeout = 1000;
                serial1.Open();
                serial1.Write("b");
                serial1.DataReceived += (DataReceivedHandler);
            }
            catch (Exception) { }
            Thread t = new Thread(ClasesDeMetodos.Server.Service);
            t.Start();


            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

            var db = new SQLiteConnection(databasePath);
            db.CreateTable<BaseaDeDatos.UsuariosDB>();

            var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

            var db1 = new SQLiteConnection(databasePath1);
            db1.CreateTable<BaseaDeDatos.HorairosDeAcceso>();


            var databasePath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lista_de_torniquetes.db");

            var db2 = new SQLiteConnection(databasePath2);
            db2.CreateTable<BaseaDeDatos.DispositivosDeAccesoConectados>();

            string page = string.Format("{0}HTMLResources/html/BootstrapFormExample.html", GetAppLocation());
            //string page = string.Format("{0}HtmlAccesos/HTMLPage1.html", GetAppLocation());
            m_chromeBrowser = new ChromiumWebBrowser(page);
            m_javascriptSvc = new SomeClass(m_chromeBrowser);


            // Register the JavaScriptInteractionObj class with JS
            m_chromeBrowser.RegisterJsObject("winformObj", m_javascriptSvc);

            Controls.Add(m_chromeBrowser);

            ChromeDevToolsSystemMenu.CreateSysMenu(this);

            SetTimer();



        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(60000*0.1);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.Agarra_tabla();
            ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.Binari_de_excel();
            ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.borra_viejas_entradas();
            ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.CSV();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Test if the About item was selected from the system menu
            if ((m.Msg == ChromeDevToolsSystemMenu.WM_SYSCOMMAND) && ((int)m.WParam == ChromeDevToolsSystemMenu.SYSMENU_CHROME_DEV_TOOLS))
            {
                m_chromeBrowser.ShowDevTools();
            }
        }

        private void BootStrapForm2_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void BootStrapForm2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                m_chromeBrowser.ShowDevTools();
            }
        }

        private void BootStrapForm2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
        public static void Coms()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");
            Puertos.Clear();
            // Display each port name to the console.
            foreach (string port in ports)
            {
                Puertos.Add(port);
                Console.WriteLine(port);
            }

            Console.ReadLine();
            //iniciaPuertos(ports.FirstOrDefault());
        }
        public static void iniciaPuertos(string puerto)
        {
            try
            {

                Random rand = new Random();
                if (rand.NextDouble() > 0.5)
                {
                    serial1.Write("a");
                }
                else
                {
                    serial1.Write("b");
                }
                serial1.Write("b");

            }
            catch (Exception) { }
        }
        private static void DataReceivedHandler(
                      object sender,
                      SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            //  string indata = sp.ReadExisting();
            // string indata2 = sp.ReadLine();
            try
            {
                string indata3 = sp.ReadTo("\r\n");

                if (indata3.Length > 5)
                {
                    //run code on data received from serial port

                    try
                    {

                        //Es_padre
                        //DateTime dia_para = Convert.ToDateTime(fechas2[i]);

                        string lectura = indata3;//bob["lectura"].ToString();

                        if(lectura != lectura_ant){
                            lectura_ant = lectura;
                        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

                        var db = new SQLiteConnection(databasePath);
                        var query = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => v.Tarjeta == lectura).FirstOrDefault();
                        if (query == null)
                        {
                            ChromeTest.RegistroNuewvoUsuario.Form1.valor_tarjeta = lectura;
                            var form = new RegistroNuewvoUsuario.Form1();
                            form.ShowDialog();
                            serial1.Write("b");
                        }
                        else
                        {

                            if (query.Acceso_Autorizado)
                            {
                                var databasePath_horarios = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                                var db_horarios = new SQLiteConnection(databasePath_horarios);

                                // var query1 = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id > 0);
                                //  serial1.Write("a");
                                //sp.BaseStream.WriteByte((byte)('a'));
                                serial1.WriteLine("a");
                                if (query.Tipo_de_acceso == "" || query.Tipo_de_acceso == "Salida")
                                {
                                    query.Tipo_de_acceso = "Entrada";
                                    var aparato = Puertos.FirstOrDefault();
                                    query.UltimoAcesso = DateTime.Now;
                                    db.RunInTransaction(() =>
                                    {
                                        db.Update(query);
                                    });
                                    /////////////////////////////////////


                                    var s = db_horarios.Insert(new BaseaDeDatos.HorairosDeAcceso()
                                    {
                                        Dispositivo = aparato,
                                        Evento = "Entrada",
                                        Id_de_usuario = query.Id,
                                        Nombre = query.Nombre,
                                        Tarjeta = query.Tarjeta,
                                        Torniquete = aparato,
                                        Hora = DateTime.Now
                                    });

                                    ///////////////////////////
                                }
                                else if (query.Tipo_de_acceso == "Entrada")
                                {
                                    query.Tipo_de_acceso = "Salida";
                                    ///////////////////////////
                                    query.UltimoAcesso = DateTime.Now;
                                    var aparato = Puertos.FirstOrDefault();
                                    db.RunInTransaction(() =>
                                    {
                                        db.Update(query);
                                    });
                                    var s = db_horarios.Insert(new BaseaDeDatos.HorairosDeAcceso()
                                    {
                                        Dispositivo = aparato,
                                        Evento = "Salida",
                                        Id_de_usuario = query.Id,
                                        Nombre = query.Nombre,
                                        Tarjeta = query.Tarjeta,
                                        Torniquete = aparato,
                                        Hora = DateTime.Now
                                    });

                                    //////////////////////////
                                }

                            }
                            else
                            {
                                // serial1.Write("b");
                                // sp.BaseStream.WriteByte((byte)('b'));
                                serial1.WriteLine("b");
                            }

                        }
                    }
                        // sp.BaseStream.Flush();

                        Console.WriteLine(lectura);
                        //  sp.DiscardInBuffer();
                        //  sp.DiscardOutBuffer();

                        //  Console.Write(indata);
                    }

                    catch (Exception ex)
                    {


                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch(Exception)
            { }         

        }

        public class SomeClass
        {
            public Person m_theMan = null;

            [JavascriptIgnore]
            public ChromiumWebBrowser m_chromeBrowser { get; set; }

            public SomeClass(ChromiumWebBrowser webBrwsr)
            {
                m_chromeBrowser = webBrwsr;
            }

            public string SomeFunction()
            {
                return "yippieee";
            }

            public void ButtonPressed(string buttonName, string user_name)
            {
                Random rand = new Random();
                //MessageBox.Show(string.Format("Message box from C# winforms. Msg: {0}", buttonName));

                // var script = "document.body.style.backgroundColor = 'red';";
                //var script = "$('#inputEmail').val('a@a.com');";

                // var script = "var x = 1234";
                //   var script = "msgBoxFromJavaScript();";
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

                var db = new SQLiteConnection(databasePath);

                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                var db1 = new SQLiteConnection(databasePath1);

                switch (buttonName)
                {
                    case "login":
                        if (!String.IsNullOrEmpty(user_name))
                        {
                            var cadena_separacion_del_usuario_editar = user_name.Split(',');
                            RegistroNuewvoUsuario.Form1.Id_del_usuario =Convert.ToInt32(cadena_separacion_del_usuario_editar[0]);
                            ChromeTest.RegistroNuewvoUsuario.Form1.valor_tarjeta = cadena_separacion_del_usuario_editar[1];
                        }
                        var form = new RegistroNuewvoUsuario.Form1();
                        form.ShowDialog();
                        form.FormClosing += (s, e) =>
                        {

                            MessageBox.Show("OK");
                        };
                        form.FormClosed += (s, e) =>
                        {

                            MessageBox.Show("Ok");
                        };
                        break;
                    case "horario":
                        try
                        {
                           
                            var query1 = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id > 0);
                            List<string> ids_horarios = new List<string>();
                            List<string> hora_horarios = new List<string>();
                            List<string> nombre_horarios = new List<string>();
                            List<string> Dispositivos_horarios = new List<string>();
                            List<string> torniquete_horarios = new List<string>();
                            List<string> Evento_horarios = new List<string>();
                            List<string> Tarjeta_horarios = new List<string>();
                            List<string> Id_del_elemento = new List<string>();
                            foreach (var elemento in query1)
                            {
                                Id_del_elemento.Add(elemento.Id.ToString());
                                ids_horarios.Add(elemento.Id_de_usuario.ToString());
                                hora_horarios.Add(elemento.Hora.ToString());
                                nombre_horarios.Add(elemento.Nombre);
                                Dispositivos_horarios.Add(elemento.Tipo_de_usuario);
                                torniquete_horarios.Add(elemento.Torniquete);
                                Evento_horarios.Add(elemento.Evento);
                                Tarjeta_horarios.Add(elemento.Tarjeta);

                            }
                            string ids_del_elemento = String.Join(",", Id_del_elemento);
                            string ids_concatenado = String.Join(",", ids_horarios);
                            string horas = String.Join(",", hora_horarios);
                            string nombres = String.Join(",", nombre_horarios);
                            string dispositivos = String.Join(",", Dispositivos_horarios);
                            string tornis = String.Join(",", torniquete_horarios);
                            string Events = String.Join(",", Evento_horarios);
                            string tarkjetas_concat = String.Join(",", Tarjeta_horarios);
                            var script = "Prepara_pagina3('50','" + ids_concatenado + "','" + horas + "','" + nombres + "','" + dispositivos + "','" + tornis + "','" + Events + "','" + tarkjetas_concat + "','" + ids_del_elemento + "');";

                            //   var script = "Prepara_pagina3(" + "'50'," + "'hola'" + ");";
                            m_chromeBrowser.ExecuteScriptAsync(script);
                        }
                        catch (Exception) { }
                        break;
                    case "usuarios":
                        try
                        {
                            var query = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => v.Id > 0);
                            List<string> usuarios = new List<string>();
                            List<string> ids = new List<string>();
                            List<string> tarjeta = new List<string>();
                            List<string> hora = new List<string>();
                            List<string> tipo = new List<string>();
                            List<string> Id_elemento_lista = new List<string>();
                            foreach (var aca in query)
                            {

                                ids.Add(aca.Id.ToString());
                                usuarios.Add(aca.Nombre);
                                tarjeta.Add(aca.Tarjeta);
                                hora.Add(aca.UltimoAcesso.ToString());
                                tipo.Add(aca.Tipo_de_acceso);
                            }
                            string users = String.Join(",", usuarios);
                            string ids1 = String.Join(",", ids);
                            string tarjetas = String.Join(",", tarjeta);
                            string horario = String.Join(",", hora);
                            string t_acceso = String.Join(",", tipo);
                            var script2 = "Actualiza_base_de_usuarios_registrados('" + ids1 + "','" + users + "','" + tarjetas + "','" + horario + "','" + t_acceso + "');";
                            m_chromeBrowser.ExecuteScriptAsync(script2);
                        }
                        catch (Exception) {  }
                        break;
                    case "controladores":
                        string[] colores = new string[] { "#ff9800", "#f44336", "#2196F3", "#009688" };//  "#FF5733", "#A4321A", "#65453E", "#FF2E00", "#FFEB00", "#06DA0F", "#107614", "#4ED1AB", "#09ECF5", "#0965F5", "#2009F5", "#6309F5", "#9909F5", "#D209F5" };
                        var valor = 10;

                        List<string> colores_seleccionados = new List<string>();
                        var databasePath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lista_de_torniquetes.db");

                        var db2 = new SQLiteConnection(databasePath2);
                        var query2 = db2.Table<BaseaDeDatos.DispositivosDeAccesoConectados>().Where(v => v.Id > 0);
                        foreach(var elem in query2)
                        {
                            colores_seleccionados.Add(colores[(int)(Math.Round(rand.NextDouble() * 3))]);

                        }
                        
                        string arrat = String.Join(",", colores_seleccionados);
                        var script3 = "Anade_cuadros_de_controladores(" + "'5','" + arrat + "');";
                        m_chromeBrowser.ExecuteScriptAsync(script3);
                        break;
                    case "shot":

                        var form1 = new RegistroNuewvoUsuario.Form1();
                        form1.ShowDialog();
                        break;
                    case "Denegacion":

                        var query_denegacion = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => v.Nombre == user_name).FirstOrDefault();
                        query_denegacion.Acceso_Autorizado = false;

                        db.RunInTransaction(() =>
                        {
                            db.Update(query_denegacion);
                        });


                        var dyummy = 2 + 2;
                        break;
                    case "Aceptado":
                        var query_aceptacion = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => v.Nombre == user_name).FirstOrDefault();
                        query_aceptacion.Acceso_Autorizado = true;

                        db.RunInTransaction(() =>
                        {
                            db.Update(query_aceptacion);
                        });


                        break;
                    case "Elimina_elemento_de_la_lista_horario":
                        var databasePath_borra_del_horaio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                        var db1_borra_del_horaio = new SQLiteConnection(databasePath_borra_del_horaio);
                        int Indice_de_elemento_a_borrar_usuarios = Convert.ToInt32(user_name);
                       // var query1_borra_del_horaio = db1_borra_del_horaio.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id == Indice_de_elemento_a_borrar_usuarios);
                        var query1_borra_del_horaio = db1_borra_del_horaio.Table<BaseaDeDatos.HorairosDeAcceso>().Delete(v => v.Id == Indice_de_elemento_a_borrar_usuarios);

                     


                        break;
                    case "Borrar_Usuario_de_la_lista":
                        ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.Id_del_usuario = user_name;
                        ChromeTest.ClasesDeMetodos.ClaseMetodosUsuarios.Elimina_al_usuario();
                        break;
                    case "Descarga_excel":

                        var query_no_alumno = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => !v.Tipo_de_usuario.Contains("Alumno"));
                        List<String> usuarios_new_type = new List<String>();
                        List<String> usuarios_tipo_de_usuario = new List<String>();
                        foreach (var elem in query_no_alumno)
                        {
                            usuarios_new_type.Add(elem.Nombre);
                            usuarios_tipo_de_usuario.Add(elem.Tipo_de_usuario);
                        }
                        string arrelgo_nombre_administradores = String.Join(",", usuarios_new_type);
                        string arrelgo_administradores_tipo_de_usuario = String.Join(",", usuarios_tipo_de_usuario);
                        var script9 = "Descarga_super_excel('"  + arrelgo_nombre_administradores + "','"+ arrelgo_administradores_tipo_de_usuario+ "');";
                        m_chromeBrowser.ExecuteScriptAsync(script9);
                        break;
                    case "Download_Request":
                        //user_name
                        var query_accesador = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Nombre == user_name);
                        byte[] nona = new byte[] { 67, 89, 89, 87 };
                        ExcelPackage package = new ExcelPackage();
                        var worksheet = package.Workbook.Worksheets.Add("New Sheet");
                        worksheet.Cells["A1:E1"].AutoFilter = true;
                        worksheet.Cells[1, 1].Value = "Nombre";
                        worksheet.Cells[1, 2].Value = "Evento";

                        worksheet.Cells[1, 3].Value = "Fecha";

                        worksheet.Cells[1, 4].Value = "hora";
                        worksheet.Cells[1, 5].Value = "Tipo de Usuario";


                        int i = 2;
                        foreach (var elem in query_accesador)
                        {
                            worksheet.Cells[i, 1].Value = elem.Nombre;
                            worksheet.Cells[i, 2].Value = elem.Evento;
                            DateTime momento1 = (DateTime)elem.Hora;
                            DateTime momento = momento1;
                            worksheet.Cells[i, 3].Value = momento.Day.ToString() + "/" + momento.Month.ToString() + "/" + momento.Year.ToString() + "  " + momento.Hour.ToString() + ":" + momento.Minute.ToString() + ":" + momento.Second.ToString();
                            worksheet.Cells[i, 4].Value = elem.Tipo_de_usuario;



                            i++;
                        }
                        var mem = new MemoryStream();
                        package.SaveAs(mem);
                        string memString = "Memory test string !!";


                        
                        var thread = new Thread(() => {
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                            saveFileDialog1.Filter = "Excel|*.xlsx";

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {

                                byte[] buffer = mem.ToArray(); //Encoding.ASCII.GetBytes(memString);
                                MemoryStream ms = new MemoryStream(buffer);
                                //write to file
                                FileStream file = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
                                ms.WriteTo(file);
                                file.Close();
                                ms.Close();
                                /*
                                using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                                    using (StreamWriter se = new StreamWriter(s))
                                    {
                                        se.Write(mem.ToArray());
                                    }
                                    */
                            }


                        });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        break;
                    case "Login_process":
                        var arregament = user_name.Split('/');
                        if (arregament[0] == "SysAdmin" && arregament[1] == "pass123")
                        {
                            var script10 = "Change_perspective();";
                            m_chromeBrowser.ExecuteScriptAsync(script10);
                        }
                        break;
                    case "Cerrar_sesion":
                        m_chromeBrowser.ExecuteScriptAsync("Cerrar_sesion();");
                        break;
                    case "DownloadComplete_Request":
                        //user_name
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



                        var thread_Completo = new Thread(() => {
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                            saveFileDialog1.Filter = "Excel|*.xlsx";

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {

                                byte[] buffer = mem_Completo.ToArray(); //Encoding.ASCII.GetBytes(memString);
                                MemoryStream ms = new MemoryStream(buffer);
                                //write to file
                                FileStream file = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
                                ms.WriteTo(file);
                                file.Close();
                                ms.Close();
                                /*
                                using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                                    using (StreamWriter se = new StreamWriter(s))
                                    {
                                        se.Write(mem.ToArray());
                                    }
                                    */
                            }


                        });
                        thread_Completo.SetApartmentState(ApartmentState.STA);
                        thread_Completo.Start();
                        break;
                }





                // m_chromeBrowser.ExecuteScriptAsync(script);

            }

            public void Denega_usuario(string User)
            {
                var dummy = 2 + 2;
            }


        }
    }
    class Basic_user
    {
        public string Nombre;
        public string Tipo_User;
    }
}
