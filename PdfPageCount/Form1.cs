using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing.Imaging;


namespace PageCounter
{
    public partial class Form1 : Form
    {

        FileStream filestream;
        
        public Form1()
        {
            InitializeComponent();
        }

      
        private void processFolder(string path, string newpath)
        {
            

            while (true)
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
                FileInfo[] currenFiles = new DirectoryInfo(path).GetFiles();
                for (int i = 0; i < currenFiles.Length; i++)
                {
                    int addition = 0;

                    if (currenFiles[i].Name.Contains(".pdf") || currenFiles[i].Name.Contains(".PDF") || currenFiles[i].Name.Contains(".Pdf"))
                    {
                        addition = GetNoOfPagesPDF(currenFiles[i].FullName);
                    }
                    else if (currenFiles[i].Name.Contains(".tif") || currenFiles[i].Name.Contains(".TIF") || currenFiles[i].Name.Contains(".Tif"))
                    {
                        addition = GetNoOfPagesTIFF(currenFiles[i].FullName);
                    }
                    else if (currenFiles[i].Name.Contains(".jpg") || currenFiles[i].Name.Contains(".JPG") || currenFiles[i].Name.Contains(".Jpg"))
                    {
                        addition = 1;
                    }

                    if (addition > 0)
                    {
                        filestream = new FileStream(this.label7.Text + "\\log.txt", FileMode.Append);
                        StreamWriter saveLog = new StreamWriter(filestream);
                        updateLabelText((Int32.Parse(label5.Text) + addition).ToString());
                        saveLog.WriteLine(DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond + " _ " + currenFiles[i].Name + " _ " + addition);
                        saveLog.Flush();
                        saveLog.Close();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        try
                        {
                            currenFiles[i].MoveTo(newpath + "\\" + currenFiles[i].Name);
                        }
                        catch(Exception de)
                        {
                        }
                    }
                }

                Thread.Sleep(500);

            }  
        }

        delegate void updateLabelTextDelegate(string newText);

        private void updateLabelText(string newText)
        {
            if (label5.InvokeRequired)
            {
                // this is worker thread
                updateLabelTextDelegate del = new updateLabelTextDelegate(updateLabelText);
                label5.Invoke(del, new object[] { newText });
            }
            else
            {
                // this is UI thread
                label5.Text = newText;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.label2.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThreadStart ts = delegate() { processFolder(this.folderBrowserDialog1.SelectedPath, this.folderBrowserDialog2.SelectedPath); };
            Thread workerThread = new Thread(ts);
            workerThread.Name = "uploader";

            if (button2.Text.Equals("Run"))
            {
                this.button1.Enabled = false;
                this.button3.Enabled = false;
                this.button4.Enabled = false;
                button2.Text = "Stop";
                workerThread.Start();
            }
            else
            {
                button2.Text = "Run";
                workerThread.Abort();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        public static int GetNoOfPagesPDF(string FileName)
        {
            int result = 0;
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                StreamReader r = new StreamReader(fs);
                string pdfText = r.ReadToEnd();

                System.Text.RegularExpressions.Regex regx = new Regex(@"/Type\s*/Page[^s]");
                System.Text.RegularExpressions.MatchCollection matches = regx.Matches(pdfText);
                result = matches.Count;
                return result;
            }
            // Check if Secure PDF
            catch (Exception ec)
            {
                return result;
            }

        }

        public static int GetNoOfPagesTIFF(string path)
        {
            int pgcount = 0;
            //Creating object for image class
            try
            {
                Image Tiff = Image.FromFile(path);
                pgcount = Tiff.GetFrameCount(FrameDimension.Page);
                return pgcount;
            }
            catch (Exception ex)
            {
                return pgcount;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                this.label4.Text = this.folderBrowserDialog2.SelectedPath;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog3.ShowDialog() == DialogResult.OK)
            {
                this.label7.Text = this.folderBrowserDialog3.SelectedPath;
            }

        }
    }
}