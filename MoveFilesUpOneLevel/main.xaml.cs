using MoveFilesUpOneLevel.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MoveFilesUpOneLevel
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : Window
    {
        MainViewModel mv;

        public main()
        {
            InitializeComponent();
            mv = new MainViewModel();
            this.DataContext = mv;
        }

        private void getSrc_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult res = fbd.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                mv.SourceDirectory = fbd.SelectedPath;
                mv.IsConsolidationAllowed = false;
                mv.RefreshCanExecuteChanged();
            }
        }

        private void getDest_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult res = fbd.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                mv.DestinationDirectory = fbd.SelectedPath;
                mv.IsConsolidationAllowed = false;
                mv.RefreshCanExecuteChanged();
            }
        }
    }
}
