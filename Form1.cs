using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NormalizeMediaNames
{
    public partial class Form1 : Form
    {
        private DirectoryInfo directory;

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = Environment.CurrentDirectory;
            textBox2.Text = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours.ToString();

            GetFiles();
        }

        private void GetFiles()
        {
            listView1.Items.Clear();

            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                directory = new DirectoryInfo(textBox1.Text);
                if (directory.Exists)
                {
                    var files = new List<MediaFile>();

                    foreach (var file in directory.GetFiles())
                    {
                        MediaFile mediaFile = null;
                        if (Regex.IsMatch(file.Name, @"VID_\d{8}_\d{6}.mp4")) // VID_20150711_101346.mp4  IMG_20150704_090600.jpg
                        {
                            mediaFile = new MediaFile(file.Name,
                                new DateTime(int.Parse(file.Name.Substring(4, 4)), // Year
                                    int.Parse(file.Name.Substring(8, 2)), // Month
                                    int.Parse(file.Name.Substring(10, 2)), // Day
                                    int.Parse(file.Name.Substring(13, 2)), // Hour
                                    int.Parse(file.Name.Substring(15, 2)), // Minute
                                    int.Parse(file.Name.Substring(17, 2))), // Second
                                 ".mp4");
                        }
                        else if (Regex.IsMatch(file.Name, @"IMG_\d{8}_\d{6}.jpg")) // IMG_20150704_090600.jpg
                        {
                            mediaFile = new MediaFile(file.Name,
                                new DateTime(int.Parse(file.Name.Substring(4, 4)), // Year
                                    int.Parse(file.Name.Substring(8, 2)), // Month
                                    int.Parse(file.Name.Substring(10, 2)), // Day
                                    int.Parse(file.Name.Substring(13, 2)), // Hour
                                    int.Parse(file.Name.Substring(15, 2)), // Minute
                                    int.Parse(file.Name.Substring(17, 2))), // Second
                                 ".jpg");
                        }
                        else if (Regex.IsMatch(file.Name, @"IMG_\d{4}.JPG")) // IMG_2326.JPG
                        {
                            mediaFile = new MediaFile(file.Name, file.LastWriteTime, ".jpg");
                        }
                        else if (Regex.IsMatch(file.Name, @"MVI_\d{4}.MOV")) // MVI_2334.MOV
                        {
                            mediaFile = new MediaFile(file.Name, file.LastWriteTime, ".mov");
                        }

                        if (mediaFile != null)
                        {
                            // Duplicate in list
                            while (files.Any(x => x.Name == mediaFile.Name))
                            {
                                ++mediaFile.NameIncrement;
                            }

                            // Duplicate on file system
                            while (File.Exists(Path.Combine(directory.FullName, mediaFile.Name)))
                            {
                                ++mediaFile.NameIncrement;
                            }

                            files.Add(mediaFile);
                        }
                    }

                    foreach (var file in files)
                    {
                        listView1.Items.Add(new ListViewItem(new[] { file.OriginalName, file.Name }));
                    }
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.Columns[0].Width += 10;
            listView1.Columns[1].Width += 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                GetFiles();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetFiles();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                var file = new FileInfo(Path.Combine(directory.FullName, item.SubItems[0].Text));
                if (file.Exists)
                {
                    file.MoveTo(Path.Combine(directory.FullName, item.SubItems[1].Text));

                    var date = DateTime.Parse(Path.GetFileNameWithoutExtension(item.SubItems[1].Text).Replace(".", ":"));
                    if (file.LastWriteTime != date)
                        File.SetLastWriteTime(file.FullName, date);
                }
            }

            Application.Exit();
        }
    }
}
