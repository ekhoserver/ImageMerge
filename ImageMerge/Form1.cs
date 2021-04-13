using Microsoft.WindowsAPICodePack.Dialogs;
using OpenCvSharp;
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

namespace ImageMerge
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int imgWidth = Convert.ToInt32(textBox1.Text);
            int blankPx = Convert.ToInt32(textBox2.Text);

            Mat srcImage1 = new Mat();
            Mat blankImg = new Mat(new OpenCvSharp.Size(imgWidth, blankPx), MatType.CV_8UC3,
                new Scalar(textBox3.BackColor.B, textBox3.BackColor.G, textBox3.BackColor.R));
            Mat board;

            if (!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(dialog.FileName);
                    int Count = 0;
                    foreach(var XX in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(XX);
                        board = new Mat();

                        toolStripProgressBar1.Maximum = dirs.Length;

                        int total = dirs.Length;
                        var items = di.GetFiles("*.*").Where(s => s.Name.EndsWith(".jpg") || s.Name.EndsWith(".png") || s.Name.EndsWith(".bmp"));
             
                        foreach (var item in items)
                        {
                            Application.DoEvents();
                            Invoke((MethodInvoker)(() => toolStripStatusLabel1.Text = $"작업목록: {di.Name}"));
                            //Console.WriteLine(item.Name);
                            srcImage1 = Cv2.ImRead(item.FullName);

                            double ratio = imgWidth / (double)srcImage1.Width;

                            Cv2.Resize(srcImage1, srcImage1, new OpenCvSharp.Size(0, 0), ratio, ratio, InterpolationFlags.Area);

                            if (board.Width != imgWidth)
                            {
                                Cv2.VConcat(srcImage1, blankImg, board);
                            }
                            else
                            {
                                Cv2.VConcat(srcImage1, blankImg, srcImage1);
                                Cv2.VConcat(board, srcImage1, board);
                            }
                            srcImage1.Dispose();
                    
                            Cv2.ImWrite($"Output\\{di.Name}.png", board);
                        }

                        toolStripProgressBar1.Value = ++Count;
                        board.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = textBox1.ForeColor;

            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                textBox3.BackColor = MyDialog.Color;
            }

        }

    }
}