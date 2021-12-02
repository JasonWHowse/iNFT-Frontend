using iNFT.src.Controller;
using iNFT.src.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static iNFT.src.Ethereum_Interact;
using static iNFT.src.Utilities.Toaster;

namespace iNFT.src.View {
    public partial class MainWindow : Window {

        private readonly Ethereum_Interact etherium = new Ethereum_Interact();
        private readonly Toaster toast = new Toaster();

        /// <summary>
        /// MainWindow
        /// 
        /// Requirements D7.0.0
        /// </summary>
        public MainWindow() {
            Log.StartLogger();
            this.InitializeComponent();
            _ = this.MainGrid.Children.Add(this.toast.GetToast());
            this.EnvironmentComboBox.ItemsSource = Enum.GetValues(typeof(Crypto));
            this.InitializeLogonWindow();
        }

        /// <summary>
        /// Requirement D1.0.0
        /// </summary>
        /*=============================Logon Block================================*/

        private decimal userBalance = -1M;

        /// <summary>
        /// Requirements D7.2.2
        /// </summary>
        private async void CheckLogin() {
            try {
                this.userBalance = await this.etherium.CheckUserName(this.UsernamePrivateKeyTextBox.Password);
            } catch (Exception e) {
                this.userBalance = -2M;
                Log.ErrorLog(e);
            }
        }

        /// <summary>
        /// Requirement D1.2.0, D1.2.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnvironmentChanged(object sender, SelectionChangedEventArgs e) {
            if (this.EnvironmentComboBox.SelectedIndex == -1) {
            } else if (this.EnvironmentComboBox.SelectedIndex == 2) {
                this.toast.PopToastie("The Main Network contract has not been deployed", ToastColors.WARNING, 2);
                this.EnvironmentComboBox.SelectedIndex = -1;
            } else {
                this.etherium.SetEnvironment((Crypto)this.EnvironmentComboBox.SelectedIndex);
            }
        }

        /// <summary>
        /// Requirement D1.1.0, D1.1.1, D1.1.2, D6.4.0
        /// </summary>
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
            Application.Current.MainWindow.Height = 160;
            Application.Current.MainWindow.MinHeight = 160;
            Application.Current.MainWindow.MaxHeight = 160;

            this.UserNamePrivateKeyLabel.Visibility = Visibility.Visible;
            this.UsernamePrivateKeyTextBox.Visibility = Visibility.Visible;
            this.LoginButton.Visibility = Visibility.Visible;
            this.EnvironmentComboBox.Visibility = Visibility.Visible;
            this.EnvironmentLabel.Visibility = Visibility.Visible;

            this.NFTComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Requirement D1.2.5, D1.3.0, D1.3.1, D1.3.2, D1.3.3, D1.4.0, D1.4.1, D1.4.2, D1.4.3, D1.4.4, D7.2.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e) {
            if (this.UsernamePrivateKeyTextBox.Password.Length > 1 && this.UsernamePrivateKeyTextBox.Password.ToLower()[1] == 'x') {
                this.UsernamePrivateKeyTextBox.Password = this.UsernamePrivateKeyTextBox.Password.ToLower().Split("x")[1];
            }
            this.userBalance = -1M;
            if (this.EnvironmentComboBox.SelectedIndex == -1) {
                this.toast.PopToastie("Please Select An Environment", ToastColors.ERROR, 2);
                return;
            }
            if (this.UsernamePrivateKeyTextBox.Password.Length == 0) {
                this.toast.PopToastie("Please Enter a Private Key", ToastColors.ERROR, 2);
                return;
            }
            if (!Regex.IsMatch(this.UsernamePrivateKeyTextBox.Password, @"^[0-9a-fA-F]+$")) {
                this.toast.PopToastie("Invalid Private Key", ToastColors.ERROR, 2);
                return;
            }
            try {
                Task.Run(this.CheckLogin).Wait();
                while (this.userBalance == -1M) {
                    Thread.Sleep(500);
                }
                if (this.userBalance == -2M) {
                    this.toast.PopToastie("Failed To Connect to Environment", ToastColors.ERROR, 2);
                    this.etherium.Logout();
                    return;
                }
                this.SetNFTComboBox();
                this.toast.PopToastie((this.userBalance == 0M ? "Warning!\r\n" : "") + "Current Balance: " + this.userBalance, this.userBalance == 0M ? ToastColors.WARNING : ToastColors.PRIMARY, 4);
            } catch (Exception ex) {
                this.toast.PopToastie("Failed To Connect to Environment", ToastColors.ERROR, 2);
                Log.ErrorLog(ex);
                return;
            }
            this.UsernamePrivateKeyTextBox.Password = "";
            this.InitializeMainWindow();
        }

        /*=============================Logon Block================================*/

        /// <summary>
        /// D5.0.0
        /// </summary>
        /*==========================Transfer Block================================*/

        private void InitializeTransferWindow() {//TODO: Implement if time permits
            throw new NotImplementedException();
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        /*==========================Transfer Block================================*/

        /// <summary>
        /// Requirements D2.0.0, D3.0.0, D4.0.0, D6.0.0
        /// </summary>
        /*==============================Main Block================================*/

        private readonly IPFS_Interact IPFS = new IPFS_Interact();
        private bool? hasMinted = null;
        private List<string>[] nftList;
        private string filePath = "";
        private string IPFS_Hash = "";

        private async void GetTokens() {
            this.nftList = await this.etherium.TokenList();
        }

        /// <summary>
        /// Requirements D7.2.7
        /// </summary>
        private async void Mint() {
            try {
                this.hasMinted = await this.etherium.Mint(this.IPFS_Hash);
            } catch (Exception e) {
                Log.ErrorLog(e);
                this.hasMinted = false;
            }
        }

        /// <summary>
        /// Requirements D3.4.2, D7.2.6
        /// </summary>
        private async void PostFileToIPFS() {
            try {
                this.IPFS_Hash = await this.IPFS.SetFileToIPFS(this.filePath);
            } catch (Exception e) {
                this.IPFS_Hash = "";
                Log.ErrorLog(e);
            }
        }

        /// <summary>
        /// Requirements D7.2.5
        /// </summary>
        private async void SetFileName() {
            try {
                this.filePath = await this.IPFS.GetIPFSFile(this.filePath) ? this.IPFS.FileName : "false";
            } catch (Exception e) {
                this.filePath = "false";
                Log.ErrorLog(e);
            }
        }

        /// <summary>
        /// Requirements D3.1.0, D3.1.2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fileName = new OpenFileDialog();
            this.FileNameTextBox.Text = (fileName.ShowDialog() == true) ? fileName.FileName : "";
        }

        /// <summary>
        /// Requirements D3.1.2, D3.2.0, D3.2.1, D4.2.0, D4.2.1, D7.2.3
        /// </summary>
        private void DisplayImage() {
            try {
                this.ImageNFTDisplay.Visibility = Visibility.Visible;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(this.filePath);
                bitmap.EndInit();
                this.ImageNFTDisplay.Source = bitmap;
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        /// <summary>
        /// Requirements D3.1.2, D3.3.0, D3.3.1, D4.3.0, D4.3.1, D7.2.4
        /// </summary>
        private void DisplayText() {
            try {
                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Visible;
                this.TextNFTDisplay.Text = File.ReadAllText(this.filePath);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        /// <summary>
        /// Requirements D3.1.0, D3.1.1, D3.1.2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            this.NFTComboBox.SelectedIndex = -1;
            this.MintButton.Visibility = this.FileNameTextBox.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;
            if (File.Exists(this.FileNameTextBox.Text)) {
                string ext = IPFS_Interact.GetTypeByPathFromByteCode(this.FileNameTextBox.Text).ToLower();
                if (IPFS_Interact.Image_File_Types.Contains(ext)) {
                    this.filePath = this.FileNameTextBox.Text;
                    this.DisplayImage();
                } else if (IPFS_Interact.Text_File_Types.Contains(ext)) {
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

        /// <summary>
        /// Requirements D1.5.0, D1.5.1, D1.5.2, D1.6.0
        /// </summary>
        private void InitializeMainWindow() {
            this.UserNamePrivateKeyLabel.Visibility = Visibility.Hidden;
            this.UsernamePrivateKeyTextBox.Visibility = Visibility.Hidden;
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
        }

        /// <summary>
        /// Requirements D6.0.0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Button_Click(object sender, RoutedEventArgs e) {
            this.etherium.Logout();
            this.InitializeLogonWindow();
        }

        /// <summary>
        /// Requirements D3.4.0, D3.4.1, D3.4.2, D3.4.3, D3.4.4, D3.4.5, D3.4.6, D7.1.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mint_Button_Click(object sender, RoutedEventArgs e) {
            this.hasMinted = null;
            this.filePath = this.FileNameTextBox.Text;
            if (File.Exists(this.FileNameTextBox.Text)) {
                this.IPFS_Hash = "Waiting";
                Task.Run(this.PostFileToIPFS).Wait();
                while (this.IPFS_Hash.Equals("Waiting")) {
                    Thread.Sleep(500);
                }
                if (this.IPFS_Hash.Length != 0) {
                    Log.InfoLog("https://gateway.ipfs.io/ipfs/" + this.IPFS_Hash);

                    Task.Run(this.Mint).Wait();
                    while (this.hasMinted == null) {
                        Thread.Sleep(500);
                    }

                    if (this.hasMinted == true) {
                        this.toast.PopToastie("Token Successfully Minted", ToastColors.PRIMARY, 5);
                    } else {
                        this.toast.PopToastie("Token Failed to Mint", ToastColors.ERROR, 5);
                    }
                } else {
                    this.toast.PopToastie("Failed to Post to IPFS", ToastColors.ERROR, 2);
                }

                this.SetNFTComboBox();
                this.FileNameTextBox.Text = "";
            } else {
                this.toast.PopToastie("No Such File Exists", ToastColors.ERROR, 2);
            }
        }


        /// <summary>
        /// Requirements D4.0.0, D4.1.0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NFTComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.NFTComboBox.SelectedIndex == -1) {
                this.filePath = this.FilePathTextBox.Text = "";
                this.FilePathTextBox.Visibility = Visibility.Hidden;
                this.CopytoClipboardButton.Visibility = Visibility.Hidden;
                this.ImageNFTDisplay.Visibility = Visibility.Hidden;
                this.TextNFTDisplay.Visibility = Visibility.Hidden;
                //this.TransferButton.Visibility = Visibility.Hidden;
            } else {
                //this.TransferButton.Visibility = Visibility.Visible;
                this.IPFS.DeleteFile();
                this.filePath = this.NFTComboBox.SelectedItem.ToString();
                Task.Run(this.SetFileName).Wait();
                while (this.filePath.Equals(this.NFTComboBox.SelectedValue)) {
                    Thread.Sleep(500);
                }
                if (this.filePath.Equals("false")) {
                    this.toast.PopToastie("No File Found", ToastColors.ERROR, 2);
                    this.SetNFTComboBox();
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
        }

        /// <summary>
        /// Requirements D2.2.0, D3.4.7
        /// </summary>
        private void SetNFTComboBox() {
            this.nftList = null;
            if (etherium.AccountIsNull()) {
                this.NFTComboBox.ItemsSource = null;
                return;
            }
            _ = Task.Run(this.GetTokens);
            while (this.nftList == null) {
                Thread.Sleep(500);
            }
            this.NFTComboBox.ItemsSource = this.nftList[2];
            this.NFTComboBox.IsEnabled = true;
        }

        private void Copy_to_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(this.filePath);
        }

        /*==============================Main Block================================*/

        //uncomment to deploy contracts this is a dev level tool
        //private void deployButton(object s, RoutedEventArgs e) {
        //    Task.Run(Deploy_Contract.Contract_Preparation);
        //}
    }
}