using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChromeTest.ClasesDeMetodos
{
    class Server
    {
        public static TcpListener server;
        public static Socket ClientSocket;
        public static NetworkStream stream;
        public static void Service()
        {
            try
            {


                //IPAddress ipAddress = Dns.GetHostEntry("192.168.100.100").AddressList[0];

                Int32 port = 16000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                var direccioncompleta = GetLocalIPAddress() +":"+ port.ToString();
             
                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);
                
                server.Start();
                ClientSocket = server.AcceptSocket();
                if (ClientSocket.Connected)
                {

                    int i;
                    using (stream = new NetworkStream(ClientSocket))
                    {
                        String Respuesta = "0";
                        // Buffer for reading data
                        Byte[] bytes = new Byte[256];
                        String data = null;
                        String aca = "";
                        i = stream.Read(bytes, 0, bytes.Length);
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        var arr = data.Split('\n');
                        if (data.Contains("ESP8266"))
                        {

                            String St = arr[0];

                            int pFrom = St.IndexOf("GET /") + "GET /".Length;
                            int pTo = St.LastIndexOf(" HTTP/1.1\r");

                            String result = St.Substring(pFrom, pTo - pFrom);
                            if (result.Contains("registro"))
                            {
                                var lista = result.Split('/');
                                Agrega_torniquete(lista[1]);
                            }
                            else if (result.Contains("acceso"))
                            {
                                var lista = result.Split('/');
                                if (Busca_Usuario(lista[1]) == 1)
                                {
                                    switch (lista[2])
                                    {
                                        case "2":
                                            Respuesta = "2";
                                            break;
                                        case "1":
                                            Respuesta = "1";
                                            break;
                                    }
                                }


                            }
                            aca += "HTTP/1.0 200 OK";
                            aca += System.Environment.NewLine;
                            aca += "Content-Type: text/plain; charset=UTF-8";
                            aca += System.Environment.NewLine;
                            aca += "Content-Length: " + "4";
                            aca += System.Environment.NewLine;
                            aca += System.Environment.NewLine;
                            aca += Respuesta;

                        }
                        else
                        {
                            if (data.Contains("Descarga_excel") == false) {
                                var link_to_down = "<a href=\"https://"+ direccioncompleta + "/Descarga_excel" + "\">Visit W3Schools.com!</a>";
                                link_to_down = ClaseMetodosUsuarios.HTML_Generada;
                                /*
                                link_to_down = "<table style='width:100 %'>";
                                var databasePath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HorariosDeAccesosDB.db");

                                var db1 = new SQLiteConnection(databasePath1);
                                var query1 = db1.Table<BaseaDeDatos.HorairosDeAcceso>().Where(v => v.Id > 0);
                                link_to_down += "<tr>";
                                link_to_down += "<th> Id </th>";
                                link_to_down += "<th> Nombre </th>";
                                link_to_down +="<th> Evento </th>";
                                link_to_down += "<th> Tipo de Usuario </th>";
                                link_to_down += "<th> Hora </th>";
                                link_to_down +="</tr>";
                                foreach (var elem in query1)
                                {
                                    link_to_down += "<tr>";
                                    link_to_down += "<td>" + elem.Id+ "</td>";
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
                                */
                                /*
                                 *
                                 *
                                 *     // Dispositivo = aparato,
                            Evento = "Entrada",
                            Id_de_usuario = query.Id,
                            Nombre = query.Nombre,
                            Tarjeta = query.Tarjeta,
                           // Torniquete = aparato,
                            Hora = DateTime.Now,
                            Tipo_de_usuario = query.Tipo_de_usuario


                                 *<table style="width:100%">
   <tr>
     <th>Firstname</th>
     <th>Lastname</th> 
     <th>Age</th>
   </tr>
   <tr>
     <td>Jill</td>
     <td>Smith</td> 
     <td>50</td>
   </tr>
   <tr>
     <td>Eve</td>
     <td>Jackson</td> 
     <td>94</td>
   </tr>
 </table> 
                                 */
                                var gran_mensaje = "<html><body>"+ link_to_down + "</body></html>";
                            byte[] bytes3 = Encoding.UTF8.GetBytes(gran_mensaje);
                            aca += "HTTP/1.0 200 OK";
                            aca += System.Environment.NewLine;
                            aca += "Content-Type: text/html; charset=UTF-8";
                            aca += System.Environment.NewLine;
                            aca += "Content-Length: " + bytes3.Length.ToString();
                            aca += System.Environment.NewLine;
                            aca += System.Environment.NewLine;

                            aca += gran_mensaje;
                        }
                            else
                            {
                                var gran_mensaje = ClaseMetodosUsuarios.Tabla_Generada;
                                byte[] bytes3 = ASCIIEncoding.ASCII.GetBytes(gran_mensaje);// Encoding.UTF8.GetBytes(gran_mensaje);//ClaseMetodosUsuarios.Tabla_Generada_arreglo;//Encoding.UTF8.GetBytes(gran_mensaje);
                                //aca += "HTTP/1.0 200 OK";
                                aca += "HTTP/1.0 200 OK";
                                aca += System.Environment.NewLine;
                                // aca += "Content-Type: application/octet-stream; charset=UTF-8";
                                aca += "Content-Type: application/octet-stream; charset=UTF-8";// name=\"some_filename.csv\"; charset=UTF-8";
                                aca += System.Environment.NewLine;
                                aca += "Content-Disposition: attachment; filename=\"some_filename.csv\"";
                                aca += System.Environment.NewLine;
                                aca += "Content-Length: " + ASCIIEncoding.ASCII.GetByteCount(gran_mensaje);//(bytes3.Length).ToString();
                                aca += System.Environment.NewLine;
                                //aca += gran_mensaje;
                                aca += System.Environment.NewLine;
                                aca += gran_mensaje;

                            }

                            //  Toast.MakeText(activida, "hola", ToastLength.Short).Show();
                        }
                        Console.WriteLine("Received: {0}", data);
                        // byte[] msg = System.Text.Encoding.ASCII.GetBytes("hola");
                        //  byte[] bytes2 = Encoding.UTF8.GetBytes("hola");

                        // Send back a response.
                        // String aca = "";

                        //writer.Write(content);
                        byte[] bytes2 = ASCIIEncoding.ASCII.GetBytes(aca);// Encoding.UTF8.GetBytes(aca);
                        //stream.Write(bytes2, 0, bytes2.Length );
                        //stream.WriteAsync(bytes2, 0, bytes2.Length);
                        // Console.WriteLine("Sent: {0}", msg);
                        stream.BeginWrite(bytes2, 0, bytes2.Length, myWriteCallBack, null);
                        //Thread.Sleep(10);
                        //ClientSocket.Close();
                        //server.Stop();
                        
                    }


                }
                //Service();

            }
            catch (Exception) {
                Service();
            }


            


        }

        public static void myWriteCallBack(IAsyncResult ar)
        {
           
            try
            {
               // stream.EndWrite(ar);
            }
            catch (Exception ex) {
                Console.WriteLine("Sent: {0}", ex.ToString());
            }
            server.Stop();
            ClientSocket.Close();
            Service();
            //NetworkStream myNetworkStream = (NetworkStream)ar.AsyncState;
            //myNetworkStream.EndWrite(ar);
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static void Agrega_torniquete(String Mac)
        {
            try
            {
                var databasePath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lista_de_torniquetes.db");

                var db2 = new SQLiteConnection(databasePath2);
                var query = db2.Table<BaseaDeDatos.DispositivosDeAccesoConectados>().Where(v => v.Puerto == Mac).FirstOrDefault();
               // var query2 = db2.Table<BaseaDeDatos.DispositivosDeAccesoConectados>().Where(v => v.Id > 0).Last();

                if (query == null)
                {

                    var valor = Mac.Split(':')[0];
                    var s = db2.Insert(new BaseaDeDatos.DispositivosDeAccesoConectados()
                    {
                        Puerto = Mac,
                        Pseudonimo = "torniquete" + valor,
                        AllowAccess = true,
                        Accesos_Por_dia = 0,
                        Ultima_averia = "",
                        Ultimo_Mantenimiento = DateTime.Now,
                        Ultimo_Acceso = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static int Busca_Usuario(String lectura)
        {
            int retorno = 0;
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

            var db = new SQLiteConnection(databasePath);
            var query = db.Table<BaseaDeDatos.UsuariosDB>().Where(v => v.Tarjeta == lectura).FirstOrDefault();
            if (query == null)
            {
                ChromeTest.RegistroNuewvoUsuario.Form1.valor_tarjeta = lectura;
                var form = new RegistroNuewvoUsuario.Form1();
                form.ShowDialog();
                // serial1.Write("b");
                retorno = 0;
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
                    //serial1.WriteLine("a");
                    retorno = 1;
                    if (query.Tipo_de_acceso == "" || query.Tipo_de_acceso == "Salida")
                    {
                        query.Tipo_de_acceso = "Entrada";
                       // var aparato = Puertos.FirstOrDefault();
                        query.UltimoAcesso = DateTime.Now;
                        db.RunInTransaction(() =>
                        {
                            db.Update(query);
                        });
                        /////////////////////////////////////


                        var s = db_horarios.Insert(new BaseaDeDatos.HorairosDeAcceso()
                        {
                           // Dispositivo = aparato,
                            Evento = "Entrada",
                            Id_de_usuario = query.Id,
                            Nombre = query.Nombre,
                            Tarjeta = query.Tarjeta,
                           // Torniquete = aparato,
                            Hora = DateTime.Now,
                            Tipo_de_usuario = query.Tipo_de_usuario
                        });

                        ///////////////////////////
                    }
                    else if (query.Tipo_de_acceso == "Entrada")
                    {
                        query.Tipo_de_acceso = "Salida";
                        ///////////////////////////
                        query.UltimoAcesso = DateTime.Now;
                       // var aparato = Puertos.FirstOrDefault();
                        db.RunInTransaction(() =>
                        {
                            db.Update(query);
                        });
                        var s = db_horarios.Insert(new BaseaDeDatos.HorairosDeAcceso()
                        {
                          //  Dispositivo = aparato,
                            Evento = "Salida",
                            Id_de_usuario = query.Id,
                            Nombre = query.Nombre,
                            Tarjeta = query.Tarjeta,
                          //  Torniquete = aparato,
                            Hora = DateTime.Now,
                            Tipo_de_usuario = query.Tipo_de_usuario
                        });

                        //////////////////////////
                    }

                }
                else
                {
                    // serial1.Write("b");
                    // sp.BaseStream.WriteByte((byte)('b'));
                   // serial1.WriteLine("b");
                    retorno = 0;
                }
                //return retorno;

            }
            return retorno;
        }

    }
}
