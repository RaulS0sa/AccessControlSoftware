using AForge.Video;
using AForge.Video.DirectShow;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChromeTest.RegistroNuewvoUsuario
{
    public partial class Form1 : Form
    {
        public static string valor_tarjeta { get; set; }
        public static string Usuario { get; set; }
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        Bitmap Foto { get; set; }
        public static int Id_del_usuario { get; set; }
        public static string nombre_usuario = "";
        public List<string> camaras = new List<string>();
        public Form1()
        {
           
            Form1.Usuario = "";
            InitializeComponent();
            this.KeyPreview = true;
            getCamList();

            if (button1.Text == "start")
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    CloseVideoSource();
                    videoSource.DesiredFrameSize = new Size(320, 240);
                    videoSource.Start();
                    button1.Text = "stop";
                }
                else
                {
                    MessageBox.Show(" Error: No Device selected");
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                    CloseVideoSource();
                    button1.Text = "start";
                }
            }

            var tarjeta_anterior = !String.IsNullOrEmpty(valor_tarjeta) ? valor_tarjeta : "";
            label2.Text = tarjeta_anterior;
            nombre_usuario = tarjeta_anterior;
            if (Id_del_usuario != 0)
            {
                Label lab = new Label();
                lab.Location = new Point(377, 175);
                lab.Visible = true;
                lab.AutoSize = true;
                lab.Text = "Nombre:  " + nombre_usuario;
                
                this.Controls.Add(lab);
                
              //  MessageBox.Show(Id_del_usuario.ToString());
            }
            // Random rand = new Random();
            // label2.Text = ChromeTest.RegistroNuewvoUsuario.Form1.valor_tarjeta;

            //  this.FormClosed += Form1_Close();
        }
        public void Form1_Close(object s, object e)
        {
            MessageBox.Show("aoc");
        }

       
        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                comboBox1.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();
                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    comboBox1.Items.Add(device.Name);
                    camaras.Add(device.Name);
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                MessageBox.Show("No capture device on your system");
            }
        }
     

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = null;

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;
                }
            }

            if (fileName != null)
            {
                //Do something with the file, for example read text from it
                string text = File.ReadAllText(fileName);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

     

       // private void button2_Click(object sender, EventArgs e)
       // {
           /* Bitmap bImage = Foto;  //Your Bitmap Image
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            ClasesContendedoras.ClaseContenedora1.Foto = bImage;
            var SigBase64 = Convert.ToBase64String(byteImage); //Get Base64
            var form = new PopUpForms.Form1();
            form.Show(this);
            //  MessageBox.Show(SigBase64);
            */
       // }
        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }

        }
        //eventhandler if new frame is ready
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = img;
            Foto = img;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseVideoSource();
        }
        private void capture_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("Capture-" + DateTime.Now.ToString("HH-mm-ss tt") + ".jpg");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (button1.Text == "start")
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    CloseVideoSource();
                    videoSource.DesiredFrameSize = new Size(320, 240);
                    videoSource.Start();
                    button1.Text = "stop";
                }
                else
                {
                    MessageBox.Show(" Error: No Device selected");
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                    CloseVideoSource();
                    button1.Text = "start";
                }
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pictureBox2.Image = Foto;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Usuario != "") { 
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UsuariosDB.db");

            var db = new SQLiteConnection(databasePath);
            //db.CreateTable<BaseaDeDatos.UsuariosDB>();
            if (!String.IsNullOrEmpty(textBox1.Text) || !String.IsNullOrWhiteSpace(textBox1.Text))
            {
                //db.CreateTable<BaseaDeDatos.UsuariosDB>();
                Bitmap bImage = Foto;//ClasesContendedoras.ClaseContenedora1.Foto;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                bImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();

                var SigBase64 = Convert.ToBase64String(byteImage); //Get Base
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                //  pictureBox1.Image = ClasesContendedoras.ClaseContenedora1.Foto;

                var s = db.Insert(new BaseaDeDatos.UsuariosDB()
                {
                    Fecha_de_ingreso = DateTime.Now,
                    Foto = SigBase64,
                    Nombre = textBox1.Text,
                    Tarjeta = label2.Text,
                    Tipo_de_acceso = "",
                    UltimoAcesso = DateTime.Now,
                    Acceso_Autorizado = true,
                    Tipo_de_usuario = Form1.Usuario
                });
                    CloseVideoSource();
                    MessageBox.Show("Usuario Grabado");
                    this.Close();

            }
                else
                {
                    MessageBox.Show("Debes incluir un nommbre para el usuario");
                }
        }
            else
            {
                MessageBox.Show("Selecciona un Tipo de Usuario");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Form1.Usuario = "Alumno";
               // MessageBox.Show(Form1.Usuario);
            }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Form1.Usuario = "Profesor";
              //  MessageBox.Show(Form1.Usuario);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                Form1.Usuario = "Administrador";
                //  MessageBox.Show(Form1.Usuario);
            }

        }
    }
}
