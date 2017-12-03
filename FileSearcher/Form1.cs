using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileSearcher
{
    public partial class Form1 : Form
    {
        private List<FileInfo> files = new List<FileInfo>();
        private Dictionary<string, int> wordsList = new Dictionary<string, int>();
        private string extension = ".txt";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                DirectoryInfo directoryForSearch = new DirectoryInfo(path);
                backgroundWorker1.RunWorkerAsync(directoryForSearch);
            }
        }

        private void CheckFilesNames(DirectoryInfo parentDirectory)
        {
            FileInfo[] filesInDirectory = parentDirectory.GetFiles();
            foreach(FileInfo file in filesInDirectory)
            {
                if (file.Extension==extension)
                {
                    files.Add(file);
                }
            }
            DirectoryInfo[] subDirectories = parentDirectory.GetDirectories();
            foreach(DirectoryInfo subDirectory in subDirectories)
            {
                CheckFilesNames(subDirectory);
            }     
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo subDirectory = (DirectoryInfo) e.Argument;
            CheckFilesNames(subDirectory);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (FileInfo file in files)
            {
                string content = File.ReadAllText(file.FullName);
                string[] words = content.Split(null);
                
                foreach (string word in words)
                {
                    if (wordsList.ContainsKey(word))
                    {
                        wordsList[word]++;
                    }else
                    {
                        wordsList.Add(word,1);
                    }
                }
            }

            foreach (KeyValuePair<string,int> word in wordsList.OrderBy(key => key.Value).Reverse())
            {
                richTextBox1.AppendText(word.Key + "\t" + word.Value+"\n");
            }
        }
    }
}
