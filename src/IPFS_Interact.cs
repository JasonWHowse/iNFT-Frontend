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

        private readonly IpfsClient defaultGateway = new IpfsClient("https://ipfs.infura.io:5001");

        private IpfsClient gateway;
        private string storePath;
        public string FileName { private set; get; }
        public string Ext { private set; get; }

        public IPFS_Interact() {
            this.Ext = this.FileName = "";
            this.gateway = this.defaultGateway;
            this.storePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }

        public IPFS_Interact(string gateway) : this() {
            this.gateway = new IpfsClient(gateway);
        }

        public IPFS_Interact(string gateway, string storePath) : this(gateway) {
            this.storePath = storePath;
        }

        public bool Display() {
            return new HashSet<string> { "webp", "jpeg", "mp4", "png", "gif"}.Contains(this.Ext);//todo get full extention list
        }

        public string GetTypeByPathFromByteCode(string path) {
            try {
                byte[] byteFile = File.ReadAllBytes(path);
                return MimeGuesser.GuessFileType(byteFile).Extension;
            } catch(Exception e) {
                return "";
            }
        }

        public async Task<bool> GetIPFSFile(string path) {
            using (Stream stream = await gateway.FileSystem.ReadFileAsync(path)) {
                try {
                    Stack<byte> bytes = new Stack<byte>();
                    int bits = stream.ReadByte();
                    while (bits != -1) {
                        bytes.Push((byte)bits);
                        bits = stream.ReadByte();
                    }
                    byte[] file = new byte[bytes.Count];
                    for (int i = 1; i <= file.Length; i++) {
                        file[^i] = bytes.Pop();
                    }
                    this.Ext = MimeGuesser.GuessFileType(file).Extension;
                    this.FileName = this.storePath + "StoredFile." + this.Ext;
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
                //todo: investigate file options and cancellation tokens
            } catch (Exception e) {
                Log.ErrorLog(e);
                return "";
            }
        }
    }
}
