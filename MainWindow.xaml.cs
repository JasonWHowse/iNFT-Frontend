using iNFT.src;
using iNFT.src.Logger;
using iNFT.src.Toaster;
using Microsoft.Win32;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static iNFT.src.Toaster.Toaster;

namespace iNFT {
    public partial class MainWindow : Window {

        private readonly Toaster toast = new Toaster();
        public MainWindow() {
            InitializeComponent();
            Log.StartLogger();
            this.MainGrid.Children.Add(toast.GetToast());

            this.ImageNFTDisplay.Visibility = Visibility.Hidden;
            this.TextNFTDisplay.Visibility = Visibility.Hidden;
            this.MintButton.Visibility = Visibility.Hidden;
            this.TransferButton.Visibility = Visibility.Hidden;
            this.FilePathTextBox.Visibility = Visibility.Hidden;
            this.CopytoClipboardButton.Visibility = Visibility.Hidden;

            this.NFTComboBox.ItemsSource = new string[] { "test", "test2", "test3" };

            //TODO: Populate Combo Box
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fileName = new OpenFileDialog();
            this.FileNameTextBox.Text = (fileName.ShowDialog() == true) ? fileName.FileName : "";
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            //this.lable.Content = this.FileNameTextBox.Text.Length > 0 ? new IPFS_Interact().GetTypeByPathFromByteCode(this.FileNameTextBox.Text) : "";
        }

        private void DisplayImage(string path) {
            try {
                this.ImageNFTDisplay.Visibility = Visibility.Visible;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                this.ImageNFTDisplay.Source = bitmap;
            } catch (Exception exc) {
                Log.ErrorLog(exc);
            }
        }

        private void NFTComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(this.NFTComboBox.SelectedIndex == -1 /*TODO: UPDATE: && IsInBlockchain(this.NFTComboBox.SelectedItem)*/) {
                this.FilePathTextBox.Text = "";
                this.FilePathTextBox.Visibility = Visibility.Hidden;
                this.CopytoClipboardButton.Visibility = Visibility.Hidden;
                this.TransferButton.Visibility = Visibility.Hidden;
            } else {
                //TODO: Determine if old downloaded file exists if so delete it
                this.FilePathTextBox.Visibility = Visibility.Visible;
                this.CopytoClipboardButton.Visibility = Visibility.Visible;
                this.TransferButton.Visibility = Visibility.Visible;

                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
                //TODO: Delete below
                this.FilePathTextBox.Text = this.NFTComboBox.SelectedItem.ToString();
                //TODO: UPDATE: this.FilePathTextBox.Text = Download(EthereumGetToken(this.NFTComboBox.SelectedItem));

                //TODO: UPDATE: if(file successfully downloaded){
                //toast.PopToastie("Successfully Downloaded File", ToastColors.Primary, 5);
                //}else{
                //toast.PopToastie("Failed to Download File", ToastColors.Error, 5);
                //}

                //TODO: UPDATE: if(FileCanBeDisplayed(this.FilePathTextBox.Text) && FileIsImage(this.FilePathTextBox.Text)){
                //DisplayImage(this.FilePathTextBox.Text)
                //}

                //TODO: UPDATE: else if(FileCanBeDisplayed(this.FilePathTextBox.Text) && FileIsText(This.FilePathTextBox.Text)){ 
                //this.TextNFTDisplay.Visibility = Visibility.Visible;
                //this.TextNFTDisplay.Text = GetTextFromFile(this.FilePathTextBox.Text);
                //}
            }
            //TODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();

        }

        private void FileNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (this.FileNameTextBox.Text.Length > 0) {
                this.MintButton.Visibility = Visibility.Visible;
            } else {
                this.MintButton.Visibility = Visibility.Hidden;
            }
        }

        private void Mint_Button_Click(object sender, RoutedEventArgs e) {
            //TODO: Mints NFT
            //TODO: Combo box updated
            //TODO: Mint button removed
            //TODO: Clear Text box
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e) {
            //TODO: Transfer stuff
            //TODO: update combo box
            //TODO: disable Transfer button
            //TODO: disable clipboard button
            //TODO: disable path textbox
            //TODO: deselect combobox
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e) {
            //TODO: Destroy user log info object 
            //TODO: Remove this form
        }

        private void Copy_to_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(this.FilePathTextBox.Text);
        }
    }
}
