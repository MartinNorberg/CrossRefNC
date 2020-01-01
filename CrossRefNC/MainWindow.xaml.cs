namespace CrossRefNC
{
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.WindowsAPICodePack.Dialogs;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private ViewModel ViewModel => (ViewModel)this.DataContext;

        private void BtnBrowseNcPath_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.NcPath = this.GetPath();
        }

        private void BtnBrowseCsPath_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.CsharpPath = this.GetPath();
        }

        private string GetPath()
        {
            var path = string.Empty;
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = dialog.FileName;
                }

                return path;
            }
        }

        private void BtnGetList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ViewModel.ReadFiles();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgCrossRef_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            object item = this.dgCrossRef.SelectedItem;
            var filePath = (this.dgCrossRef.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
            var variable = (this.dgCrossRef.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
            var lineNumber = this.ViewModel.ShowSelectedFile(filePath, variable);
            this.FocusSelectedVariable(lineNumber);
        }

        private void FocusSelectedVariable(int lineNumber)
        {
            this.txtFileView.Focus();
            this.txtFileView.Select(this.txtFileView.GetCharacterIndexFromLineIndex(lineNumber), this.txtFileView.GetLineLength(lineNumber));
            Keyboard.Focus(this.dgCrossRef);
            this.dgCrossRef.Focus();
        }

        private void DgCrossRef_KeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.Focus(this.dgCrossRef);
        }
    }
}
