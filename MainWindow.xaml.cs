﻿using iNFT.src;
using iNFT.src.Logger;
using iNFT.src.Toaster;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
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
        private readonly Ethereum_Interact etherium = new Ethereum_Interact();
        private readonly IPFS_Interact IPFS = new IPFS_Interact();
        public MainWindow() {
            Log.StartLogger();
            this.InitializeComponent();
            _ = this.MainGrid.Children.Add(this.toast.GetToast());
            this.EnvironmentComboBox.ItemsSource = Enum.GetValues(typeof(Crypto));
            this.UsernamePrivateKeyTextBox.Password = "270afe316a844a84ff12c3c8cd6206edf812751b807eeba251ad84bb33f3e78a";
            this.EnvironmentComboBox.SelectedIndex = 0;
            this.InitializeLogonWindow();
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
                this.toast.PopToastie((this.userBalance == 0M ? "Warning!\r\n" : "") + "Current Balance: " + this.userBalance, this.userBalance == 0M ? ToastColors.WARNING : ToastColors.PRIMARY, 2);
            } catch (Exception ex) {
                this.toast.PopToastie("Failed To Connect to Environment", ToastColors.ERROR, 2);
                Log.ErrorLog(ex);
                return;
            }
            this.UsernamePrivateKeyTextBox.Password = "";
            this.SetNFTComboBox();
            this.InitializeMainWindow();
        }

        private async void CheckLogin() {
            try {
                this.userBalance = await this.etherium.CheckUserName(this.UsernamePrivateKeyTextBox.Password);
            } catch (Exception e) {
                this.userBalance = -2M;
                Log.ErrorLog(e);
            }
        }

        private decimal userBalance = -1M;

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
            if (false /*aTODO: Delete false UPDATE: !IsInBlockchain(this.NFTComboBox.SelectedItem)*/ ) {
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

        private async void GetTokens() {
            this.nftList = await this.etherium.TokenList();
        }

        private List<string>[] nftList;

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
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
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
                this.filePath = await this.IPFS.GetIPFSFile(this.filePath) ? this.IPFS.FileName : "false";
            } catch (Exception e) {
                this.filePath = "false";
                Log.ErrorLog(e);
            }
        }
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
                this.IPFS.DeleteFile(this.filePath.Split("\\")[^1]);
                this.filePath = this.NFTComboBox.SelectedItem.ToString();Log.InfoLog(filePath);
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
                    Log.InfoLog("change");
                    this.DisplayImage();
                } else if (IPFS_Interact.Text_File_Types.Contains(IPFS_Interact.GetTypeByPathFromByteCode(this.filePath).ToLower())) {
                    this.DisplayText();
                }
            }
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
                this.IPFS_Hash = await this.IPFS.SetFileToIPFS(this.filePath);
            } catch (Exception e) {
                this.IPFS_Hash = "";
                Log.ErrorLog(e);
            }
        }

        private async void Mint() {
            try {
                this.hasMinted = await this.etherium.Mint(this.IPFS_Hash);
            } catch (Exception e) {
                Log.ErrorLog(e);
                this.hasMinted = false;
            }
        }

        private bool? hasMinted = null;
        private string IPFS_Hash = "";

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

        private void Logout_Button_Click(object sender, RoutedEventArgs e) {
            this.etherium.Logout();
            this.InitializeLogonWindow();
        }

        private void Copy_to_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(this.filePath);
        }

        private void EnvironmentChanged(object sender, SelectionChangedEventArgs e) {
            this.etherium.SetEnvironment((Crypto)this.EnvironmentComboBox.SelectedIndex);
        }

        /*==============================Main Block================================*/

        //uncomment to deploy contracts this is a dev level tool
        //private void deployButton(object s, RoutedEventArgs e) {
        //    Task.Run(Deploy_Contract.Contract_Preparation);
        //}
    }
}