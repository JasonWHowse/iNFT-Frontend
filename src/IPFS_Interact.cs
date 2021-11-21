using HeyRed.Mime;
using iNFT.src.Logger;
using Ipfs.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace iNFT.src {
    class IPFS_Interact {
        //gateway = https://gateway.ipfs.io/ipfs/QmWtJ2vPhy6eWSJ8MNk9Y7cLHE5gM3HWXSSUWCodNsqXZ2
        private readonly IpfsClient defaultGateway = new IpfsClient("https://ipfs.infura.io:5001");

        private IpfsClient gateway;
        private readonly string storePath;
        private readonly string storeFileName;
        public string FileName { private set; get; }
        public string Ext { private set; get; }

        public static readonly HashSet<string> Image_File_Types = new HashSet<string> { "webp", "jpeg", "png", "gif", "jpg", "pdf" };//todo: update image list

        public static readonly HashSet<string> Text_File_Types = new HashSet<string> { "txt", "html", "xml", "css", "js", "htm", "json" };//todo: text file list

        public IPFS_Interact() {
            this.Ext = this.FileName = "";
            this.gateway = this.defaultGateway;
            this.storeFileName = "StoredFile";
            this.storePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }

        public IPFS_Interact(string gateway) : this() {
            this.gateway = new IpfsClient(gateway);
        }

        public IPFS_Interact(string gateway, string storePath) : this(gateway) {
            this.storePath = storePath;
        }
        public IPFS_Interact(string gateway, string storePath, string storeFileName) : this(gateway, storePath) {
            this.storeFileName = storeFileName;
        }

        public bool Display() {
            return Image_File_Types.Contains(this.Ext.ToLower()) || Text_File_Types.Contains(this.Ext.ToLower());
        }

        public static string GetTypeByPathFromByteCode(string path) {
            try {
                byte[] byteFile = File.ReadAllBytes(path);
                return MimeGuesser.GuessFileType(byteFile).Extension;
            } catch (Exception) {
                return "";
            }
        }

        private byte[] StreamToByteArray(Stream input) {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        public async Task<bool> GetIPFSFile(string path) {
            using (Stream stream = await gateway.FileSystem.ReadFileAsync(path)) {
                try {
                    byte[] file = this.StreamToByteArray(stream);
                    this.Ext = MimeGuesser.GuessFileType(file).Extension;
                    this.FileName = this.storePath + storeFileName + "." + this.Ext;
                    using (Stream storeFile = File.Create(this.FileName)) {
                        try {
                            storeFile.Write(file);
                            return true;
                        } catch (Exception e) {
                            Log.ErrorLog(e);
                            return false;
                        }
                    }
                } catch (Exception e) {
                    Log.ErrorLog(e);
                }
            }
            return false;
        }

        public async Task<string> SetFileToIPFS(string path) {
            try {
                string output = (await gateway.FileSystem.AddFileAsync(path)).Id.ToString();
                return output;
            } catch (Exception e) {
                Log.ErrorLog(e);
                return "";
            }
        }

        public void DeleteFile(string filePath) {
            if (File.Exists(filePath)) {
                try {
                    File.Delete(filePath);
                } catch (Exception e) {
                    Log.ErrorLog(e);
                }
            } else {
                string[] files = Directory.GetFiles(this.storePath);
                for (int i = 0; i < files.Length; i++) {
                    if (files[i].Split(@"\")[^1].Contains(this.storeFileName)) {
                        try {
                            File.Delete(files[i]);
                        } catch (Exception e) {
                            Log.ErrorLog(e);
                        }
                    }
                }
            }
        }
    }
}