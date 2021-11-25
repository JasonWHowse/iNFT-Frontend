using iNFT.src;
using iNFT.src.Logger;
using iNFT.src.Toaster;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static iNFT.src.Ethereum_Interact;
using static iNFT.src.Toaster.Toaster;

namespace iNFT {
    public partial class MainWindow : Window {

        private readonly Toaster toast = new Toaster();
        private LogonCredentials creds = new LogonCredentials();
        private readonly Ethereum_Interact etherium = new Ethereum_Interact();
        private readonly IPFS_Interact IPFS = new IPFS_Interact();
        public MainWindow() {
            Log.StartLogger();
            this.InitializeComponent();
            _ = this.MainGrid.Children.Add(this.toast.GetToast());
            this.EnvironmentComboBox.ItemsSource = Enum.GetValues(typeof(Crypto));

            //todo: delete below

            this.EnvironmentComboBox.SelectedIndex = 0;

            //Task.Run(this.etherium.deletethis2contractDeploy);

            //todo: delete below

            this.InitializeLogonWindow();

            //TODOdelete this v
            this.NFTComboBox.ItemsSource = new string[] { "QmSgiPTvE9XZo6YvSs8Xw9HW311aAxLnz9qGqgZDNFj8xj", "QmZHd1fbAsE4j281P69a9gR8UdoK3G8DsJ2G7oxVQ8osQ3", "QmaNdRRK5rVxBiodg8fcSpiPoZHFJuqw5ackGFTacHbbKa", "QmWtJ2vPhy6eWSJ8MNk9Y7cLHE5gM3HWXSSUWCodNsqXZ2",
            "QmSyGCnVeNMQdxWmaV4eTNEfYo3i4xzE5f8mXm1te2hpfZ",
            "QmSyGCnVeNMQdxWmaV4eTNEfYo3i4xzE5f8mXm1te2pfZ"};
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
            Application.Current.MainWindow.Height = 200;
            Application.Current.MainWindow.MinHeight = 200;
            Application.Current.MainWindow.MaxHeight = 200;

            //this.UserNamePrivateKeyLabel.Visibility = Visibility.Visible;
            //this.UsernamePrivateKeyTextBox.Visibility = Visibility.Visible;
            this.UsernamePublicKeyTextBox.Visibility = Visibility.Visible;
            this.UserNamePublicKeyLabel.Visibility = Visibility.Visible;
            this.PasswordKeyTextBox.Visibility = Visibility.Visible;
            this.PasswordKeyLabel.Visibility = Visibility.Visible;
            this.LoginButton.Visibility = Visibility.Visible;
            this.EnvironmentComboBox.Visibility = Visibility.Visible;
            this.EnvironmentLabel.Visibility = Visibility.Visible;

            this.NFTComboBox.SelectedIndex = -1;
        }


        //test account = 0xE5Ef0ccc8A65b2F834341F5527B71Ec9CD3F23d7

        private void Login_Click(object sender, RoutedEventArgs e) {
            if (true) {//todo: Delete automatic password and environment assignment
                this.PasswordKeyTextBox.Password = "1234pass";
                //this.UsernamePrivateKeyTextBox.Password = "0xe0d9F6E40f8c3fd3b121F54d09E069d51Ba64D96";
                this.UsernamePublicKeyTextBox.Text = "0x2dF96C647E934C98EEeFbBEc79D7703B31e9aCE7";
                this.EnvironmentComboBox.SelectedIndex = 0;
            }
            this.authenticated = 100;
            if (this.EnvironmentComboBox.SelectedIndex == -1) {
                this.toast.PopToastie("Please Select An Environment", ToastColors.ERROR, 2);
                return;
            }
            if (this.UsernamePublicKeyTextBox.Text.Length == 0) {
                this.toast.PopToastie("Please Enter a Public Key", ToastColors.ERROR, 2);
                return;
            }
            /*            if (this.UsernamePrivateKeyTextBox.Password.Length == 0) {
                            this.toast.PopToastie("Please Enter a Private Key", ToastColors.ERROR, 2);
                            return;
                        }*/
            if (this.PasswordKeyTextBox.Password.Length == 0) {
                this.toast.PopToastie("Please Enter a Password", ToastColors.ERROR, 2);
                return;
            }
            try {
                //this.creds = new LogonCredentials(this.UsernamePublicKeyTextBox.Text, this.UsernamePrivateKeyTextBox.Password, this.PasswordKeyTextBox.Password); 
                this.creds = new LogonCredentials(this.UsernamePublicKeyTextBox.Text, this.PasswordKeyTextBox.Password);
                this.creds.OpenCredentials();
                Task.Run(this.CheckLogin).Wait();
                while (this.authenticated == 100) {
                    Thread.Sleep(500);
                }
                if (this.authenticated == 511) {
                    this.toast.PopToastie("Authentication Failed", ToastColors.ERROR, 2);
                    return;
                }
                if (this.authenticated == 400) {
                    //this.toast.PopToastie("Connections Failed", ToastColors.ERROR, 2);
                    this.toast.PopToastie(testToastMessage, ToastColors.WARNING, 2);
                    return;
                }
            } catch (Exception ex) {
                Log.ErrorLog(ex);
            } finally {
                this.creds.CloseCredentials();
            }
            this.PasswordKeyTextBox.Password = "";
            //this.UsernamePrivateKeyTextBox.Password = "";
            this.UsernamePublicKeyTextBox.Text = "";
            Clipboard.SetText(Log.GetFileName());
            this.InitializeMainWindow();
        }

        private async void CheckLogin() {
            try {
                if (await this.etherium.CheckUserName(this.creds)) {
                    this.authenticated = 200;
                } else {
                    this.authenticated = 511;
                }
            } catch (Exception e) {
                this.authenticated = 400;
                Log.ErrorLog(e);
                testToastMessage = e.Message;
            }
        }

        private string testToastMessage = ""; //todo: Delete testToastMessage and usages
        private int authenticated = 100;

        /*=============================Logon Block================================*/

        /*==========================Transfer Block================================*/

        private void InitializeTransferWindow() {//TODO: Implement if time permits
            throw new NotImplementedException();
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();//TODO: Implement if time permits
            this.FilePathTextBox.Text = "";
            this.FilePathTextBox.Visibility = Visibility.Hidden;
            this.CopytoClipboardButton.Visibility = Visibility.Hidden;
            //this.TransferButton.Visibility = Visibility.Hidden;
            if (false /*TODO: Delete false UPDATE: !IsInBlockchain(this.NFTComboBox.SelectedItem)*/ ) {
                this.toast.PopToastie("NFT does not exist", ToastColors.ERROR, 5);
            } else {
                //aTODO: Transfer stuff
                this.NFTComboBox.SelectedIndex = -1;
                this.toast.PopToastie("NFT Successfully Transfered", ToastColors.PRIMARY, 5);
            }
            //aTODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();
        }

        /*==========================Transfer Block================================*/

        /*==============================Main Block================================*/

        private void InitializeMainWindow() {
            //this.UserNamePrivateKeyLabel.Visibility = Visibility.Hidden;
            //this.UsernamePrivateKeyTextBox.Visibility = Visibility.Hidden;
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

            //TODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fileName = new OpenFileDialog();
            this.FileNameTextBox.Text = (fileName.ShowDialog() == true) ? fileName.FileName : "";
        }

        private void DisplayImage() {
            try {
                this.ImageNFTDisplay.Visibility = Visibility.Visible;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(this.filePath);
                bitmap.EndInit();
                this.ImageNFTDisplay.Source = bitmap;
            } catch (Exception exc) {
                Log.ErrorLog(exc);
            }
        }

        private void DisplayText() {
            try {
                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Visible;
                this.TextNFTDisplay.Text = File.ReadAllText(this.filePath);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        private string filePath = "";

        private async void SetFileName() {
            try {
                if (await this.IPFS.GetIPFSFile(this.filePath)) {
                    this.filePath = IPFS.FileName;
                } else {
                    this.filePath = "false";
                }
            } catch (Exception e) {
                this.filePath = "false";
                Log.ErrorLog(e);
            }
        }

        private void NFTComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.NFTComboBox.SelectedIndex == -1 /*TODO: UPDATE: && !IsInBlockchain(this.NFTComboBox.SelectedItem)*/) {
                this.filePath = this.FilePathTextBox.Text = "";
                this.FilePathTextBox.Visibility = Visibility.Hidden;
                this.CopytoClipboardButton.Visibility = Visibility.Hidden;
                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
                //this.TransferButton.Visibility = Visibility.Hidden;
            } else {
                //this.TransferButton.Visibility = Visibility.Visible;
                //todo: check if token still exists in users account.
                this.IPFS.DeleteFile(filePath);
                this.filePath = this.NFTComboBox.SelectedItem.ToString();
                Task.Run(this.SetFileName).Wait();
                while (this.filePath.Equals(this.NFTComboBox.SelectedValue)) {
                    Thread.Sleep(500);
                }

                if (this.filePath.Equals("false")) {
                    this.toast.PopToastie("No File Found", ToastColors.ERROR, 2);
                    //TODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();
                    return;
                }

                this.FilePathTextBox.Visibility = Visibility.Visible;
                this.CopytoClipboardButton.Visibility = Visibility.Visible;

                this.FilePathTextBox.Text = this.filePath.Split("\\")[^1];

                if (IPFS_Interact.Image_File_Types.Contains(IPFS_Interact.GetTypeByPathFromByteCode(this.filePath).ToLower())) {
                    this.DisplayImage();
                } else if (IPFS_Interact.Text_File_Types.Contains(IPFS_Interact.GetTypeByPathFromByteCode(this.filePath).ToLower())) {
                    this.DisplayText();
                }
            }
            //TODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();
        }

        private void FileNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            this.NFTComboBox.SelectedIndex = -1;
            this.MintButton.Visibility = this.FileNameTextBox.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;
            if (File.Exists(this.FileNameTextBox.Text)) {
                if (IPFS_Interact.Image_File_Types.Contains(this.FileNameTextBox.Text.Split(".")[^1].ToLower())) {
                    this.filePath = this.FileNameTextBox.Text;
                    this.DisplayImage();
                } else if (IPFS_Interact.Text_File_Types.Contains(this.FileNameTextBox.Text.Split(".")[^1].ToLower())) {
                    this.filePath = this.FileNameTextBox.Text;
                    this.DisplayText();
                } else {
                    this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                    this.TextNFTDisplay.Visibility = Visibility.Hidden;
                }
            } else {
                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
            }
        }

        private async void PostFileToIPFS() {
            try {
                this.IPFS_Hash = await IPFS.SetFileToIPFS(this.filePath);
            } catch (Exception e) {
                this.IPFS_Hash = "";
                Log.ErrorLog(e);
            }
        }

        private string IPFS_Hash = "";

        private void Mint_Button_Click(object sender, RoutedEventArgs e) {
            this.filePath = this.FileNameTextBox.Text;
            if (File.Exists(this.FileNameTextBox.Text)) {
                this.IPFS_Hash = "Waiting";
                Task.Run(this.PostFileToIPFS).Wait();
                while (this.IPFS_Hash.Equals("Waiting")) {
                    Thread.Sleep(500);
                }
                if (this.IPFS_Hash.Length != 0) {
                    this.toast.PopToastie("Success", ToastColors.PRIMARY, 2);
                    Log.InfoLog("https://gateway.ipfs.io/ipfs/" + IPFS_Hash);
                    //TODO: Mint to Etherium
                    //if(EthereumMint(this.FileNameTextBox.Text)){
                    //this.toast.PopToastie("Token Successfully Minted", ToastColors.PRIMARY, 5);
                    //}else{
                    //this.toast.PopToastie("Token Failed to Mint", ToastColors.ERROR, 5);
                    //}
                } else {
                    this.toast.PopToastie("Failed to Post to IPFS", ToastColors.ERROR, 2);
                }
                //if(EthereumMint(this.FileNameTextBox.Text)){
                //this.toast.PopToastie("Token Successfully Minted", ToastColors.PRIMARY, 5);
                //}else{
                //this.toast.PopToastie("Token Failed to Mint", ToastColors.ERROR, 5);
                //}

                //TODO: UPDATE: this.NFTComboBox.ItemsSource = GetBlockChainList();
                this.FileNameTextBox.Text = "";
            } else {
                this.toast.PopToastie("No Such File Exists", ToastColors.ERROR, 2);
            }
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e) {
            this.creds.DestroyToken();
            this.InitializeLogonWindow();
        }

        private void Copy_to_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(this.filePath);
        }

        private void EnvironmentChanged(object sender, SelectionChangedEventArgs e) {
            this.etherium.SetEnvironment((Crypto)this.EnvironmentComboBox.SelectedIndex);
        }

        /*==============================Main Block================================*/

        private void deployButton(object s, RoutedEventArgs e) {
            Task.Run(this.etherium.deletethis2contractDeploy);
        }
    }
}