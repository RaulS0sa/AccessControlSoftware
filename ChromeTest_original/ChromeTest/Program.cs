using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChromeTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new MainForm());
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolve_thi);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolve_thi2);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolve_thi3);
            Application.Run(new ChromeTest.Demos.BootStrapForm2());
       

       

        }
    static Assembly resolve_thi(object sender, ResolveEventArgs args)
    {
            String this_exe = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            System.Reflection.AssemblyName embedded_ass = new System.Reflection.AssemblyName(args.Name);
            String resource = this_exe + "." + embedded_ass.Name + ".dll";
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
        {
            byte[] assambly = new byte[stream.Length];
            stream.Read(assambly, 0, assambly.Length);
            return Assembly.Load(assambly);
        }
    }

        static Assembly resolve_thi2(object sender, ResolveEventArgs args)
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChromeTest.CefSharp.dll"))
                {
                    byte[] assambly = new byte[stream.Length];
                    stream.Read(assambly, 0, assambly.Length);
                    return Assembly.Load(assambly);
                }
            }
            catch (Exception)
            {

                String this_exe = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                System.Reflection.AssemblyName embedded_ass = new System.Reflection.AssemblyName(args.Name);
                String resource = this_exe + "." + embedded_ass.Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assambly = new byte[stream.Length];
                    stream.Read(assambly, 0, assambly.Length);
                    return Assembly.Load(assambly);
                }
            }
        }
        static Assembly resolve_thi3(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChromeTest.CefSharp.WinForms.dll"))
            {
                byte[] assambly = new byte[stream.Length];
                stream.Read(assambly, 0, assambly.Length);
                return Assembly.Load(assambly);
            }
        }
    }
}
