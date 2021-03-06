using HeyRed.Mime;
using iNFT.src.Utilities;
using Ipfs.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace iNFT.src.Controller {

    /// <summary>
    /// Class to post and get items from the ipfs network
    /// </summary>
    public class IPFS_Interact {
        private readonly IpfsClient defaultGateway = new IpfsClient("https://ipfs.infura.io:5001");

        private readonly IpfsClient gateway;
        private readonly string storeFileName;
        private readonly string storePath;

        /// <summary>
        /// Approved Image file types
        /// 
        /// Requirement D3.2.1
        /// </summary>
        public static readonly HashSet<string> Image_File_Types = new HashSet<string> { "webp", "jpeg", "png", "gif", "jpg" };

        /// <summary>
        /// Approved Raw Text file types
        /// 
        /// Requirement D3.3.1
        /// </summary>
        public static readonly HashSet<string> Text_File_Types = new HashSet<string> { "txt", "html", "xml", "css", "js", "htm", "json" };

        /// <summary>
        /// The files extentsion as determined by mime
        /// </summary>
        public string Ext { private set; get; }

        /// <summary>
        /// FileName used to store the IPFS file locally
        /// </summary>
        public string FileName { private set; get; }

        /// <summary>
        /// Basic Contructor
        /// </summary>
        public IPFS_Interact() {
            this.Ext = this.FileName = "";
            this.gateway = this.defaultGateway;
            this.storeFileName = "StoredFile";
            this.storePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }

        /// <summary>
        /// Constructor to set gateway
        /// </summary>
        /// <param name="gateway"></param>
        public IPFS_Interact(string gateway) : this() {
            this.gateway = new IpfsClient(gateway);
        }

        /// <summary>
        /// Constructor to set gateway and storepath
        /// </summary>
        /// <param name="gateway"></param>
        /// <param name="storePath"></param>
        public IPFS_Interact(string gateway, string storePath) : this(gateway) {
            this.storePath = storePath;
        }

        /// <summary>
        /// Constructor to set gateway, storepath, and storeFileName
        /// </summary>
        public IPFS_Interact(string gateway, string storePath, string storeFileName) : this(gateway, storePath) {
            this.storeFileName = storeFileName;
        }

        private byte[] StreamToByteArray(Stream input) {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Deletes all local storage files
        /// 
        /// Requirements D7.5.4
        /// </summary>
        public void DeleteFile() {
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

        /// <summary>
        /// Gets an IPFS file and stores it locally
        /// 
        /// Requirements D4.1.0, D4.1.1, D4.1.2, D7.5.1, D7.5.2
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> GetIPFSFile(string path) {
            using (Stream stream = await this.gateway.FileSystem.ReadFileAsync(path)) {
                try {
                    byte[] file = this.StreamToByteArray(stream);
                    this.Ext = MimeGuesser.GuessFileType(file).Extension;
                    this.FileName = this.storePath + this.storeFileName + "." + this.Ext;
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

        /// <summary>
        /// Posts a file to IPFS
        /// 
        /// Requirements D7.5.3
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> SetFileToIPFS(string path) {
            try {
                return (await this.gateway.FileSystem.AddFileAsync(path)).Id.ToString();
            } catch (Exception e) {
                Log.ErrorLog(e);
                return "";
            }
        }

        /// <summary>
        /// Gets the Extension determined by the bytecode of the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTypeByPathFromByteCode(string path) {
            try {
                byte[] byteFile = File.ReadAllBytes(path);
                return MimeGuesser.GuessFileType(byteFile).Extension;
            } catch (Exception) {
                return "";
            }
        }
    }
}