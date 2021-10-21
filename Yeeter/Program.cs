using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.Win32;
namespace Yeeter
{
    class Program
    {

        static string localfilename;
        static string onlinePassword;
        [STAThread]
        static void Main(string[] args)
        {

            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Yeeter was opened independently! Open Yeeter with another file to upload.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Adding Yeeter to the registry....");

                var key = Registry.ClassesRoot.CreateSubKey(@"*\shell\Yeet With Yeeter\");
                key = Registry.ClassesRoot.CreateSubKey(@"*\shell\Yeet With Yeeter\command");
                key.SetValue("", @"C:\Yeeter.exe /public %1");

                key = Registry.ClassesRoot.CreateSubKey(@"*\shell\Private Yeet With Yeeter\command");
                key.SetValue("", @"C:\Yeeter.exe /private %1");

                Console.WriteLine("File added to registry! you may close the program now and use Yeeter.");
            }
            else
            {


                Console.WriteLine("File uploading will begin!");


                string upload_type = arguments[1];

                string localpath = arguments[2];
                localfilename = Path.GetFileName(localpath);

                if (!ProtectFile(localfilename))
                    return;

                string username = "";

                start:

                if (upload_type == "/private")
                {

                    Console.WriteLine("****************************");
                    Console.WriteLine("Enter your Username:");
                    username = Console.ReadLine();

                    GetPassword(username);

                    Console.WriteLine("Enter your Password:");
                    string password = Console.ReadLine();
                    
                   

                    if (onlinePassword == password)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("AUTHENTICATION SUCCESSFULL.");
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("AUTHENTICATION FAILED.");
                        goto start;
                    }
                }

                try
                {
                    Uri finalpath1 = new Uri("ftp://srv176.main-hosting.eu/uploader/" + localfilename);
                    Uri finalpath2 = new Uri("ftp://srv176.main-hosting.eu/uploader/" + username + "/" + localfilename);


                    WebClient client = new WebClient();
                    client.Credentials = new NetworkCredential("u922155632", "Jefke123");
                    if (upload_type == "/public")
                    {
                        Console.WriteLine("UPLOADING PUBLICLY!");
                        client.UploadFileAsync(finalpath1, localpath);
                    } else if (upload_type == "/private")
                    {

                        Console.WriteLine("UPLOADING PRIVATELY!");
                        client.UploadFileAsync(finalpath2, localpath);
                    }
                        
                    client.UploadProgressChanged += WebClientUploadProgressChanged;
                    client.UploadFileCompleted += WebClientUploadFileCompleted;

                    Clipboard.SetText("http://sbpaintball.be/uploader/" + localfilename);
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Failed to upload file. " + ex);
                }
            }
            Console.ReadLine();

        }

        static void WebClientUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Upload {0}% complete. ", e.ProgressPercentage);
        }

        static void WebClientUploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your file has been uploaded and added to your clipboard. Press any key to close.");
            Console.Read();
        }

        static void GetPassword(string username)
        {
            
            string result;
            using (var client = new WebClient())
            {
                
                try
                {
                    result = client.DownloadString("http://sbpaintball.be/uploader/" + username + "/credentials.txt");
                    string[] parts = result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    onlinePassword = parts[2];

                    
                } catch (Exception ex)
                {
                    Console.WriteLine("Failed. " + ex.ToString());
                }
            }
        }

        static bool ProtectFile(string filename)
        {
            if (filename == "index.php" || filename == "credentials.txt")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You are not allowed to upload files named index.php");
                Console.ReadLine();
                return false;
            }

            return true;
        }
    }
}
