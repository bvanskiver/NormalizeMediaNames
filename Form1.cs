using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NormalizeMediaNames
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.textBox1.Text = Environment.CurrentDirectory;
            this.textBox2.Text = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours.ToString();

            this.GetFiles();
        }

        private void GetFiles()
        {
            this.listView1.Items.Clear();

            if (!string.IsNullOrEmpty(this.textBox1.Text))
            {
                DirectoryInfo di = new DirectoryInfo(this.textBox1.Text);
                if (di.Exists)
                {
                    var files = new List<MediaFile>();

                    foreach (var file in di.GetFiles())
                    {
                        if (Regex.IsMatch(file.Name, @"VID_\d{8}_\d{6}.mp4")) // VID_20150711_101346.mp4  IMG_20150704_090600.jpg
                        {
                            files.Add(new MediaFile(file.Name,
                                new DateTime(int.Parse(file.Name.Substring(4, 4)), // Year
                                    int.Parse(file.Name.Substring(8, 2)), // Month
                                    int.Parse(file.Name.Substring(10, 2)), // Day
                                    int.Parse(file.Name.Substring(13, 2)), // Hour
                                    int.Parse(file.Name.Substring(15, 2)), // Minute
                                    int.Parse(file.Name.Substring(17, 2))), // Second
                                 ".mp4"));
                        }
                        else if (Regex.IsMatch(file.Name, @"IMG_\d{8}_\d{6}.jpg")) // IMG_20150704_090600.jpg
                        {
                            files.Add(new MediaFile(file.Name,
                                new DateTime(int.Parse(file.Name.Substring(4, 4)), // Year
                                    int.Parse(file.Name.Substring(8, 2)), // Month
                                    int.Parse(file.Name.Substring(10, 2)), // Day
                                    int.Parse(file.Name.Substring(13, 2)), // Hour
                                    int.Parse(file.Name.Substring(15, 2)), // Minute
                                    int.Parse(file.Name.Substring(17, 2))), // Second
                                 ".jpg"));
                        }
                        else if (Regex.IsMatch(file.Name, @"IMG_\d{4}.JPG")) // IMG_2326.JPG
                        {
                            files.Add(new MediaFile(file.Name, file.LastWriteTime, ".jpg"));
                        }
                        else if (Regex.IsMatch(file.Name, @"MVI_\d{4}.MOV")) // MVI_2334.MOV
                        {
                            files.Add(new MediaFile(file.Name, file.LastWriteTime, ".mov"));
                        }
                    }

                    foreach (var file in files)
                    {
                        this.listView1.Items.Add(new ListViewItem(new[] { file.OriginalName, file.Name }));
                    }
                }
            }

            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.listView1.Columns[0].Width += 10;
            this.listView1.Columns[1].Width += 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = this.textBox1.Text;

            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.folderBrowserDialog1.SelectedPath;
                this.GetFiles();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetFiles();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(this.textBox1.Text);

            foreach (ListViewItem item in this.listView1.Items)
            {
                FileInfo file = new FileInfo(Path.Combine(di.FullName, item.SubItems[0].Text));
                if (file.Exists)
                {
                    file.MoveTo(Path.Combine(di.FullName, item.SubItems[1].Text));

                    var date = DateTime.Parse(Path.GetFileNameWithoutExtension(item.SubItems[1].Text).Replace(".", ":"));
                    if (file.LastWriteTime != date)
                        File.SetLastWriteTime(file.FullName, date);
                }
            }

            MessageBox.Show("Done");
        }
    }
}
