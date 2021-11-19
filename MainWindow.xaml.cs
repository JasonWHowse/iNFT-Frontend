using iNFT.src;
using iNFT.src.Logger;
using iNFT.src.Toaster;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using static iNFT.src.Etherium_Interact;
using static iNFT.src.Toaster.Toaster;

namespace iNFT {
    public partial class MainWindow : Window {

        private readonly Toaster toast = new Toaster();
        private LogonCredentials creds = new LogonCredentials();
        private readonly Etherium_Interact etherium = new Etherium_Interact();
        public MainWindow() {
            Log.StartLogger();
            this.InitializeComponent();
            _ = this.MainGrid.Children.Add(this.toast.GetToast());
            this.EnvironmentComboBox.ItemsSource = Enum.GetValues(typeof (Crypto));

            this.InitializeLogonWindow();

            //TODOdelete this v
            this.NFTComboBox.ItemsSource = new string[] { "test", "test2", "test3" };
        }

        /*=============================Logon Block================================*/

        private void InitializeLogonWindow() {
            this.FilePathTextBox.Text = this.FileNameTextBox.Text = "";
            this.FileNameTextBox.Visibility = Visibility.Hidden;
            this.BrowseButton.Visibility = Visibility.Hidden;
            this.ImageNFTDisplay.Visibility = Visibility.Hidden;
            this.TextNFTDisplay.Visibility = Visibility.Hidden;
            this.NFTComboBox.Visibility = Visibility.Hidden;
            this.MintButton.Visibility = Visibility.Hidden;
            //this.TransferButton.Visibility = Visibility.Hidden;
            this.CopytoClipboardButton.Visibility = Visibility.Hidden;
            this.FilePathTextBox.Visibility = Visibility.Hidden;
            this.LogoutButton.Visibility = Visibility.Hidden;
            this.EnvironmentComboBox.Visibility = Visibility.Hidden;


            Application.Current.MainWindow.Width = 400;
            Application.Current.MainWindow.MinWidth = 400;
            Application.Current.MainWindow.MaxWidth = 400;
            Application.Current.MainWindow.Height = 280;
            Application.Current.MainWindow.MinHeight = 280;
            Application.Current.MainWindow.MaxHeight = 280;

            this.UserNamePrivateKeyLabel.Visibility = Visibility.Visible;
            this.UsernamePrivateKeyTextBox.Visibility = Visibility.Visible;
            this.UsernamePublicKeyTextBox.Visibility = Visibility.Visible;
            this.UserNamePublicKeyLabel.Visibility = Visibility.Visible;
            this.PasswordKeyTextBox.Visibility = Visibility.Visible;
            this.PasswordKeyLabel.Visibility = Visibility.Visible;
            this.LoginButton.Visibility = Visibility.Visible;
            this.EnvironmentComboBox.Visibility = Visibility.Visible;
            this.EnvironmentLabel.Visibility = Visibility.Visible;
        }

        private void Login_Click(object sender, RoutedEventArgs e) {
            this.authenticated = 100;
            if (this.EnvironmentComboBox.SelectedIndex == -1) {
               this.toast.PopToastie("Please Select An Environment" , ToastColors.ERROR, 2);
                return;
            }
            if (this.UsernamePublicKeyTextBox.Text.Length == 0) {
                this.toast.PopToastie("Please Enter a Public Key", ToastColors.ERROR, 2);
                return;
            }
            if (this.UsernamePrivateKeyTextBox.Password.Length == 0) {
                this.toast.PopToastie("Please Enter a Private Key", ToastColors.ERROR, 2);
                return;
            }
            if (this.PasswordKeyTextBox.Password.Length == 0) {
                this.toast.PopToastie("Please Enter a Password", ToastColors.ERROR, 2);
                return;
            }
            try {
                this.creds = new LogonCredentials(this.UsernamePublicKeyTextBox.Text, this.UsernamePrivateKeyTextBox.Password, this.PasswordKeyTextBox.Password);
                this.creds.OpenCredentials();
                _ = Task.Run(this.CheckLogin);
                while (this.authenticated == 100) {
                    Thread.Sleep(500);
                }
                if (this.authenticated == 511) {
                    this.toast.PopToastie("Authentication Failed", ToastColors.ERROR, 2);
                    return;
                }
            }catch(Exception ex) {
                Log.ErrorLog(ex);
            } finally {
                this.creds.CloseCredentials();
            }
            this.PasswordKeyTextBox.Password = "";
            this.UsernamePrivateKeyTextBox.Password = "";
            this.UsernamePublicKeyTextBox.Text = "";
            this.InitializeMainWindow();
        }

        private async void CheckLogin() {
            if (await this.etherium.CheckUserName(this.creds)) {
                this.authenticated = 200;
            } else {
                this.authenticated = 511;
            }
        }

        private int authenticated = 100;

        /*=============================Logon Block================================*/

        /*==========================Transfer Block================================*/

        private void InitializeTransferWindow() {

        }

        /*==========================Transfer Block================================*/

        /*==============================Main Block================================*/

        private void InitializeMainWindow() {
            this.UserNamePrivateKeyLabel.Visibility = Visibility.Hidden;
            this.UsernamePrivateKeyTextBox.Visibility = Visibility.Hidden;
            this.UsernamePublicKeyTextBox.Visibility = Visibility.Hidden;
            this.UserNamePublicKeyLabel.Visibility = Visibility.Hidden;
            this.PasswordKeyTextBox.Visibility = Visibility.Hidden;
            this.PasswordKeyLabel.Visibility = Visibility.Hidden;
            this.LoginButton.Visibility = Visibility.Hidden;
            this.EnvironmentComboBox.Visibility = Visibility.Hidden;
            this.EnvironmentLabel.Visibility = Visibility.Hidden;

            this.ImageNFTDisplay.Visibility = Visibility.Hidden;
            this.TextNFTDisplay.Visibility = Visibility.Hidden;
            this.MintButton.Visibility = Visibility.Hidden;
            //this.TransferButton.Visibility = Visibility.Hidden;
            this.FilePathTextBox.Visibility = Visibility.Hidden;
            this.CopytoClipboardButton.Visibility = Visibility.Hidden;

            this.FileNameTextBox.Visibility = Visibility.Visible;
            this.BrowseButton.Visibility = Visibility.Visible;
            this.NFTComboBox.Visibility = Visibility.Visible;
            this.LogoutButton.Visibility = Visibility.Visible;

            Application.Current.MainWindow.Width = 800;
            Application.Current.MainWindow.MinWidth = 800;
            Application.Current.MainWindow.MaxWidth = 800;
            Application.Current.MainWindow.Height = 450;
            Application.Current.MainWindow.MinHeight = 450;
            Application.Current.MainWindow.MaxHeight = 450;

            //TODO: UPDATE: this.NFTComboBox.ItemsSource = new string[] { "test", "test2", "test3" };
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fileName = new OpenFileDialog();
            this.FileNameTextBox.Text = (fileName.ShowDialog() == true) ? fileName.FileName : "";
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
            if(this.NFTComboBox.SelectedIndex == -1 /*TODO: UPDATE: && !IsInBlockchain(this.NFTComboBox.SelectedItem)*/) {
                this.FilePathTextBox.Text = "";
                this.FilePathTextBox.Visibility = Visibility.Hidden;
                this.CopytoClipboardButton.Visibility = Visibility.Hidden;
                //this.TransferButton.Visibility = Visibility.Hidden;
            } else {
                //TODO: Determine if old downloaded file exists if so delete it
                this.FilePathTextBox.Visibility = Visibility.Visible;
                this.CopytoClipboardButton.Visibility = Visibility.Visible;
                //this.TransferButton.Visibility = Visibility.Visible;

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
            this.MintButton.Visibility = this.FileNameTextBox.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Mint_Button_Click(object sender, RoutedEventArgs e) {
            //TODO: UPDATE: if(FileIsValid(this.FileNameTextBox.Text){
            //if(EthereumMint(this.FileNameTextBox.Text)){
            //this.toast.PopToastie("Token Successfully Minted", ToastColors.PRIMARY, 5);
            //}else{
            //this.toast.PopToastie("Token Failed to Mint", ToastColors.ERROR, 5);
            //}
            //TODO: Combo box updated
            this.FileNameTextBox.Text = "";
        }
        /*
        private void Transfer_Button_Click(object sender, RoutedEventArgs e) {
            this.FilePathTextBox.Text = "";
            this.FilePathTextBox.Visibility = Visibility.Hidden;
            this.CopytoClipboardButton.Visibility = Visibility.Hidden;
            //this.TransferButton.Visibility = Visibility.Hidden;
            if (false /*TODO: Delete false UPDATE: !IsInBlockchain(this.NFTComboBox.SelectedItem) ) {
                this.toast.PopToastie("NFT does not exist", ToastColors.ERROR, 5);
            } else {
                //TODO: Transfer stuff
                this.NFTComboBox.SelectedIndex = -1;
                this.toast.PopToastie("NFT Successfully Transfered", ToastColors.PRIMARY, 5);
            }
            //TODO: update combo box
        }*/

        private void Logout_Button_Click(object sender, RoutedEventArgs e) {
            //TODO: Destroy user log info object 
            this.InitializeLogonWindow();
        }

        private void Copy_to_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(this.FilePathTextBox.Text);
        }

        private void EnvironmentChanged(object sender, SelectionChangedEventArgs e) {
 //           this.etherium.SetEnvironment((Crypto)Enum.GetValues(typeof(Crypto)).GetValue(this.EnvironmentComboBox.SelectedIndex));
            this.etherium.SetEnvironment((Crypto)this.EnvironmentComboBox.SelectedIndex);
        }

        /*==============================Main Block================================*/
    }
}
