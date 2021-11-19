using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Web3;

namespace iNFT.src {
    class LogonCredentials {
        public bool Active { private set; get; }
        private string publicKey;
        private string privateKey;
        private string password;
        public bool allowTransfer = false;
        public LogonCredentials() {
            this.Active = false;
            this.publicKey = "";
            this.privateKey = "";
            this.password = "";
            this.allowTransfer = false;
        }

        public LogonCredentials(string PublicKey, string PrivateKey, string Password) {
            this.Active = true;
            this.publicKey = PublicKey;
            this.privateKey = PrivateKey;
            this.password = Password;
            this.allowTransfer = false;
        }

        public LogonCredentials(string PublicKey, string Password) : this(PublicKey, "", Password) { }

        public void DestroyToken() {
            this.Active = false;
            this.publicKey = "";
            this.privateKey = "";
            this.password = "";
            this.allowTransfer = false;
        }

        public void OpenCredentials() {
            this.allowTransfer = true;
        }

        public void CloseCredentials() {
            this.allowTransfer = false;
        }

        public string GetPublicKey() {
            return this.allowTransfer ? this.publicKey : "";
        }

        public string GetPassword() {
            return this.allowTransfer ? this.password : "";
        }

        public string GetPrivateKey() {
            return this.allowTransfer ? this.privateKey : "";
        }
    }
}
