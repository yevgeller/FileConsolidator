using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoveFilesUpOneLevel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sourceFolderMain = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "test");
        string destinationFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        List<string> fileNamesInDestinationFolder = new List<string>();
        List<string> willResultInDuplicateFileNames = new List<string>();
        List<FileInfo> movingTheseFiles = new List<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void doIt_Click(object sender, RoutedEventArgs e)
        {
            fileNamesInDestinationFolder.Clear();
            willResultInDuplicateFileNames.Clear();
            movingTheseFiles.Clear();

            GetAllFileNames();
            PublishResults(true, fileNamesInDestinationFolder, willBeChanged);
            PublishResults(false, willResultInDuplicateFileNames, willNotBeChanged);

            MoveNonDuplicates();
            RemoveEmptyFolders();
        }

        private void GetAllFileNames()
        {
            DirectoryInfo diUpper = new DirectoryInfo(sourceFolderMain);
            DirectoryInfo[] allTargetDirectories = diUpper.GetDirectories();
            for(int i = 0; i<allTargetDirectories.Length; i++)
            {
                DirectoryInfo diCurrent = allTargetDirectories[i];
                FileInfo[] filesInCurrent = diCurrent.GetFiles();
                for(int j=0;j<filesInCurrent.Length;j++)
                {
                    string candidate = filesInCurrent[j].Name;
                    if (willResultInDuplicateFileNames.Contains(candidate) || fileNamesInDestinationFolder.Contains(candidate))
                    {
                        if (!willResultInDuplicateFileNames.Contains(candidate))
                            willResultInDuplicateFileNames.Add(candidate);

                        if (fileNamesInDestinationFolder.Contains(candidate))
                        {
                            fileNamesInDestinationFolder.Remove(candidate);
                            movingTheseFiles.RemoveAll(x => x.Name == candidate);
                        }
                    }
                    else
                    {
                        fileNamesInDestinationFolder.Add(candidate);
                        movingTheseFiles.Add(filesInCurrent[j]);
                    }
                }
            }
        }

        private void PublishResults (bool willBeChanged, List<string> files, TextBlock control)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("These files will ");
            if (!willBeChanged) sb.Append("NOT ");
            sb.Append("be moved.");
            sb.Append(Environment.NewLine);

            foreach(string f in files)
            {
                sb.Append(f);
                sb.Append(Environment.NewLine);
            }

            control.Text = sb.ToString();

        }

        private void MoveNonDuplicates()
        {
            foreach(FileInfo fi in movingTheseFiles)
            {
                File.Move(fi.FullName, System.IO.Path.Combine(destinationFolder, fi.Name));
            }
        }

        private void RemoveEmptyFolders()
        {
            DirectoryInfo diUpper = new DirectoryInfo(sourceFolderMain);
            DirectoryInfo[] dis = diUpper.GetDirectories();
            for(int i=0; i<dis.Length; i++)
            {
                FileInfo[] files = dis[i].GetFiles();
                if (files.Length == 0)
                    Directory.Delete(dis[i].FullName);
            }
        }

        private void PrepareEnvironment()
        {
            DirectoryInfo diTarget = new DirectoryInfo(destinationFolder);
            FileInfo[] fiTarget = diTarget.GetFiles();
            for(int i=1;i<=50;i++)
            {
                FileInfo a = new FileInfo(System.IO.Path.Combine(destinationFolder, i.ToString()+".txt"));
                for (int j = 0; j < fiTarget.Length; j++)
                {
                    if (fiTarget[j].FullName == a.FullName)
                    {
                        File.Delete(a.FullName);
                        break;
                    }
                }                
            }

            string sourceFolder1 = System.IO.Path.Combine(sourceFolderMain, "test1");
            DirectoryInfo di1 = new DirectoryInfo(sourceFolder1);
            if(!di1.Exists)
                Directory.CreateDirectory(sourceFolder1);

            FileInfo[] fi1 = di1.GetFiles();
            if (fi1.Length == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    using(StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder1, i.ToString()+".txt")))
                    {
                        sw.Write(i.ToString());
                    }
                }
            } 
            
            string sourceFolder2 = System.IO.Path.Combine(sourceFolderMain, "test2");
            DirectoryInfo di2 = new DirectoryInfo(sourceFolder2);
            if (!di2.Exists)
                Directory.CreateDirectory(sourceFolder2);

            FileInfo[] fi2 = di2.GetFiles();
            if(fi2.Length == 0)
            {
                for(int i=5;i<10;i++)
                {
                    using(StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder2, i.ToString()+".txt")))
                    {
                        sw.Write(i.ToString());
                    }
                }
            }            
            
            string sourceFolder3 = System.IO.Path.Combine(sourceFolderMain, "test3");
            DirectoryInfo di3 = new DirectoryInfo(sourceFolder3);
            if (!di3.Exists)
                Directory.CreateDirectory(sourceFolder3);

            FileInfo[] fi3 = di3.GetFiles();
            if(fi3.Length == 0)
            {
                for(int i=15;i<20;i++)
                {
                    using(StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder3, i.ToString()+".txt")))
                    {
                        sw.Write(i.ToString());
                    }
                }
            }
        }

        private void prep_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            PrepareEnvironment();
#endif
        }

        private void getSrc_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult res = fbd.ShowDialog();
            if(res == System.Windows.Forms.DialogResult.OK)
            {
                sourceFolderMain = fbd.SelectedPath;
                gotSrc.Text = sourceFolderMain;
            }
        }

        private void getDest_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult res = fbd.ShowDialog();
            if(res == System.Windows.Forms.DialogResult.OK)
            {
                destinationFolder = fbd.SelectedPath;
                gotDest.Text = destinationFolder;
            }
        }
    }
}
