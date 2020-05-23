using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IconChanger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            foreach (var Drive in DriveInfo.GetDrives())
            {
                CBDrive.Items.Add(Drive.Name);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OFDIcon.ShowDialog() == DialogResult.OK)
            {
                txtIcon.Text = OFDIcon.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(CBDrive.Text))
            {
                Console.WriteLine("Exist");
                string INFFile = CBDrive.Text + "AUTORUN.INF";
                Console.WriteLine(INFFile);
                string IconFile = CBDrive.Text + "_icon.ico";
                Console.WriteLine(IconFile);
                INIReader NR = new INIReader();
                if (File.Exists(INFFile))
                {
                    Console.WriteLine("Loaded existing INF");
                    NR.LoadFile(INFFile);
                }
                if (!NR.HasAutorunsHeader())
                {
                    Console.WriteLine("Wrote Autoruns Header");
                    NR.WriteAutorunsHeader();
                }
                else
                {
                    Console.WriteLine("Autoruns header found");
                }
                if (txtTitle.Text != "")
                {
                    Console.WriteLine("Set label key");
                    NR.SetKey("label", txtTitle.Text);
                }
                if (txtIcon.Text != "" && File.Exists(txtIcon.Text.Replace(@"""", "")))
                {
                    NR.SetKey("ICON", "_icon.ico");
                    Console.WriteLine("Writing icon...");
                    FileInfo FInfo = new FileInfo(txtIcon.Text);
                    if (FInfo.Extension.ToLower().EndsWith("ico"))
                    {
                        Console.WriteLine("Source Copy");
                        File.Copy(FInfo.FullName, IconFile, true);
                    }
                    else
                    {
                        Console.WriteLine("Source Render");
                        ImageModifier Mod = new ImageModifier();
                        Image Source = Image.FromFile(FInfo.FullName);
                        Mod.SaveIcon(Source, IconFile);
                    }
                    Console.WriteLine("Settings attributes for icon");
                    File.SetAttributes(IconFile, FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.NotContentIndexed);
                }
                Console.WriteLine("Saving INF");
                NR.SaveFile(INFFile);
                Console.WriteLine("Settings attributes for icon");
                File.SetAttributes(INFFile, FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.NotContentIndexed);
                Console.WriteLine("Done.");
                MessageBox.Show(this, "You may need to eject, and re-instert the drive to see the changes", "Completed");
            }
        }

        private void CBDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(CBDrive.Text))
            {
                foreach (var Drive in DriveInfo.GetDrives())
                {
                    if (Drive.Name.ToLower() == CBDrive.Text.ToLower())
                    {
                        txtTitle.Text = Drive.VolumeLabel;
                        return;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(CBDrive.Text))
            {
                string INFFile = CBDrive.Text + "AUTORUN.INF";
                string IconFile = CBDrive.Text + "_icon.ico";
                if (File.Exists(INFFile))
                {
                    INIReader NR = new INIReader();
                    Console.WriteLine("Loaded existing INF");
                    NR.LoadFile(INFFile);
                    NR.RemoveKey("ICON");
                    Console.WriteLine("Saving INF");
                    File.SetAttributes(INFFile, FileAttributes.Normal);
                    NR.SaveFile(INFFile);
                    File.SetAttributes(INFFile, FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.NotContentIndexed);
                    Console.WriteLine("Done.");
                    MessageBox.Show(this, "You may need to eject, and re-instert the drive to see the changes", "Completed");
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm HF = new HelpForm();
            HF.ShowDialog();
        }
    }
}