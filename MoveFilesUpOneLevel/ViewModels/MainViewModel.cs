using MoveFilesUpOneLevel.ServiceClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace MoveFilesUpOneLevel.ViewModels
{
    public class MainViewModel : ViewModel
    {
        List<string> fileNamesInDestinationFolder = new List<string>();
        List<string> willResultInDuplicateFileNames = new List<string>();
        List<FileInfo> movingTheseFiles = new List<FileInfo>();

        public MainViewModel()
        {
            this.SourceDirectory = "choose source directory";
            this.DestinationDirectory = "choose destination directory";
            this.DeleteEmptyFoldersOnceDone = true;
            _previewCommand = new DelegateCommand(this.PreviewCommandAction, this.CanPreview); //add this line to wherever you initialize your commands
            _consolidateCommand = new DelegateCommand(this.ConsolidateCommandAction, this.CanConsolidate); //add this line to wherever you initialize your commands
            _testCommand = new DelegateCommand(this.TestCommandAction, this.CanTest); //add this line to wherever you initialize your commands

            this.IsConsolidationAllowed = false;
        }

        public void RefreshCanExecuteChanged()
        {
            _previewCommand.RaiseCanExecuteChanged();
            _consolidateCommand.RaiseCanExecuteChanged();
        }

        DelegateCommand _previewCommand;
        public ICommand PreviewCommand { get { return _previewCommand; } }
        private void PreviewCommandAction(object obj)
        {
            this.PreviewResults = string.Empty;
            fileNamesInDestinationFolder.Clear();
            willResultInDuplicateFileNames.Clear();
            movingTheseFiles.Clear();

            GetAllFileNames();
            int canBeMoved = PublishResults(true, fileNamesInDestinationFolder);
            PublishResults(false, willResultInDuplicateFileNames);

            if (canBeMoved > 0)
            {
                this.IsConsolidationAllowed = true;
                this.PreviewResults = String.Format("{0} files will be moved,", canBeMoved);
                this.PreviewResults += String.Format(" {0} folders will be deleted", this.RemoveEmptyFolders_Preview());
            }
        }

        private bool CanPreview(object obj)
        {
            bool src = Directory.Exists(this.SourceDirectory);
            bool dest = Directory.Exists(this.DestinationDirectory);

            bool result = src && dest;
            return result;
        }

        DelegateCommand _consolidateCommand;
        public ICommand ConsolidateCommand { get { return _consolidateCommand; } }
        private void ConsolidateCommandAction(object obj)
        {
            MoveNonDuplicates();
            if (this.DeleteEmptyFoldersOnceDone)
                RemoveEmptyFolders();
        }

        private bool CanConsolidate(object obj)
        {
            return this.IsConsolidationAllowed;
        }

        private void MoveNonDuplicates()
        {
            foreach (FileInfo fi in movingTheseFiles)
            {
                File.Move(fi.FullName, System.IO.Path.Combine(DestinationDirectory, fi.Name));
            }
        }

        private void RemoveEmptyFolders()
        {
            DirectoryInfo diUpper = new DirectoryInfo(this.SourceDirectory);
            DirectoryInfo[] dis = diUpper.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                FileInfo[] files = dis[i].GetFiles();
                if (files.Length == 0)
                    Directory.Delete(dis[i].FullName);
            }
        }

        private int RemoveEmptyFolders_Preview()
        {
            int count = 0;
            DirectoryInfo diUpper = new DirectoryInfo(this.SourceDirectory);
            DirectoryInfo[] dis = diUpper.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                DirectoryInfo d = dis[i];
                bool contains = false;
                FileInfo[] files = dis[i].GetFiles();
                for (int j = 0; j < files.Length; j++)
                {
                    for (int k = 0; k < willResultInDuplicateFileNames.Count; k++)
                    {
                        if (files[j].Name == willResultInDuplicateFileNames[k])
                        {
                            contains = true;
                            break;
                        }
                    }
                }
                if (contains == false)
                    count++;
            }
            return count;
        }

        private void GetAllFileNames()
        {
            DirectoryInfo diUpper = new DirectoryInfo(this.SourceDirectory);
            DirectoryInfo[] allTargetDirectories = diUpper.GetDirectories();
            for (int i = 0; i < allTargetDirectories.Length; i++)
            {
                DirectoryInfo diCurrent = allTargetDirectories[i];
                FileInfo[] filesInCurrent = diCurrent.GetFiles();
                for (int j = 0; j < filesInCurrent.Length; j++)
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

        private int PublishResults(bool willBeChanged, List<string> files)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("These files will ");
            if (!willBeChanged) sb.Append("NOT ");
            sb.Append("be moved:");
            sb.Append(Environment.NewLine);

            foreach (string f in files)
            {
                sb.Append(f);
                sb.Append(Environment.NewLine);
            }

            if (willBeChanged)
                this.WillBeMoved = sb.ToString();
            else
                this.WillNotBeMoved = sb.ToString();

            return files.Count;
        }


        private bool _isConsolidationAllowed;
        public bool IsConsolidationAllowed
        {
            get { return _isConsolidationAllowed; }
            set
            {
                if (_isConsolidationAllowed == value) return;
                _isConsolidationAllowed = value;
                OnPropertyChanged("IsConsolidationAllowed");
                _consolidateCommand.RaiseCanExecuteChanged();
            }
        }


        private string _sourceDirectory;
        public string SourceDirectory
        {
            get { return _sourceDirectory; }
            set
            {
                if (_sourceDirectory == value) return;
                _sourceDirectory = value;
                OnPropertyChanged("SourceDirectory");
            }
        }

        private string _destinationDirectory;
        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                if (_destinationDirectory == value) return;
                _destinationDirectory = value;
                OnPropertyChanged("DestinationDirectory");
            }
        }

        private bool _deleteEmptyFoldersOnceDone;
        public bool DeleteEmptyFoldersOnceDone
        {
            get { return _deleteEmptyFoldersOnceDone; }
            set
            {
                if (_deleteEmptyFoldersOnceDone == value) return;
                _deleteEmptyFoldersOnceDone = value;
                OnPropertyChanged("DeleteEmptyFoldersOnceDone");
            }
        }

        private string _willBeMoved;
        public string WillBeMoved
        {
            get { return _willBeMoved; }
            set
            {
                if (_willBeMoved == value) return;
                _willBeMoved = value;
                OnPropertyChanged("WillBeMoved");
            }
        }

        private string _willNotBeMoved;
        public string WillNotBeMoved
        {
            get { return _willNotBeMoved; }
            set
            {
                if (_willNotBeMoved == value) return;
                _willNotBeMoved = value;
                OnPropertyChanged("WillNotBeMoved");
            }
        }

        private string _previewResults;
        public string PreviewResults
        {
            get { return _previewResults; }
            set
            {
                if (_previewResults == value) return;
                _previewResults = value;
                OnPropertyChanged("PreviewResults");
            }
        }



        DelegateCommand _testCommand;
        public ICommand TestCommand { get { return _testCommand; } }
        private void TestCommandAction(object obj)
        {
            PrepareEnvironment();
        }

        private bool CanTest(object obj)
        {
            return true;
        }


        private void PrepareEnvironment()
        {
            string sourceFolderMain = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "test");
            string destinationFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo diTarget = new DirectoryInfo(destinationFolder);
            FileInfo[] fiTarget = diTarget.GetFiles();
            for (int i = 1; i <= 50; i++)
            {
                FileInfo a = new FileInfo(System.IO.Path.Combine(destinationFolder, i.ToString() + ".txt"));
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
            if (!di1.Exists)
                Directory.CreateDirectory(sourceFolder1);

            FileInfo[] fi1 = di1.GetFiles();
            if (fi1.Length == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder1, i.ToString() + ".txt")))
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
            if (fi2.Length == 0)
            {
                for (int i = 5; i < 10; i++)
                {
                    using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder2, i.ToString() + ".txt")))
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
            if (fi3.Length == 0)
            {
                for (int i = 15; i < 20; i++)
                {
                    using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(sourceFolder3, i.ToString() + ".txt")))
                    {
                        sw.Write(i.ToString());
                    }
                }
            }

            this.SourceDirectory = @"C:\Users\YEVGENIY\Documents\svn\MoveFilesUpOneLevel\MoveFilesUpOneLevel\bin\Debug\test";
            this.DestinationDirectory = @"C:\Users\YEVGENIY\Documents\svn\MoveFilesUpOneLevel\MoveFilesUpOneLevel\bin\Debug";
            RefreshCanExecuteChanged();
        }

    }
}
