using iNFT.src.Logger;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace iNFT.src {
    class Etherium_Interact {

        public string MemeHash { get; private set; }

        public string AccBal { get; private set; }
        //public Contract _Contract { get; private set; }

        private readonly string localAbi = "[{\"constant\":false,\"inputs\":[{\"name\":\"_memeHash\",\"type\":\"string\"}],\"name\":\"set\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\",\"signature\":\"0x4ed3885e\"},{\"constant\":true,\"inputs\":[],\"name\":\"get\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0x6d4ce63c\"}]";
        private readonly string testAbi = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"},{\"indexed\":true,\"internalType\":\"bytes\",\"name\":\"result\",\"type\":\"bytes\"}],\"name\":\"ExecutionResult\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"foo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        private readonly string prodAbi = null;

        private readonly string localByteCode = "";
        private readonly string testByteCode = "";
        private readonly string prodByteCode = "";

        private readonly string testAccount = "0xd0fdc799d8125AAb9992e4c7470efB873d1d57dB";
        private readonly string testContractAccount = "0x4a1171207E34b11d002B64332D0c7F7D4ED86AEa";

        private readonly string localAccount = "0xe0d9F6E40f8c3fd3b121F54d09E069d51Ba64D96";
        private readonly string localContractAccount = "0xaD5b3231A4c02F5440a59db2284BbE2f89Fa34aA";

        private readonly string prodAccount = null;
        private readonly string prodContractAccount = null;

        private readonly Web3 localWeb3;
        private readonly Web3 testNet;
        private readonly Web3 prodNet;

        private Web3 envWeb3;
        private string envAbi;
        private string envContractByteCode;
        public string EnvAccount { get; private set; }
        public string EnvContractAccount { get; private set; }


        public enum Crypto { LOCAL, ROPSTEN, Ethereum_Mainnet }

        public Etherium_Interact() {
            try {
                this.localWeb3 = new Web3("HTTP://127.0.0.1:8545");
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
            try {
                this.testNet = new Web3("https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75");
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
            try {
                this.prodNet = null;
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        public Etherium_Interact(Crypto env) : this() {
            this.SetEnvironment(env);
        }

        public void SetEnvironment(Crypto env) {
            switch (env) {
                case Crypto.LOCAL:
                    this.envWeb3 = this.localWeb3;
                    this.envAbi = this.localAbi;
                    this.envContractByteCode = this.localByteCode;
                    this.EnvAccount = this.localAccount;
                    this.EnvContractAccount = this.localContractAccount;
                    break;
                case Crypto.ROPSTEN:
                    this.envWeb3 = this.testNet;
                    this.envAbi = this.testAbi;
                    this.envContractByteCode = this.testByteCode;
                    this.EnvAccount = this.testAccount;
                    this.EnvContractAccount = this.testContractAccount;
                    break;
                case Crypto.Ethereum_Mainnet:
                    this.envWeb3 = this.prodNet;
                    this.envAbi = this.prodAbi;
                    this.envContractByteCode = this.prodByteCode;
                    this.EnvAccount = this.prodAccount;
                    this.EnvContractAccount = this.prodContractAccount;
                    break;
            }
        }


        public async Task GetAccountBalance(string account) {
            this.AccBal = "Balance = " + (await this.envWeb3.Eth.GetBalance.SendRequestAsync(account)).Value.ToString();
        }

        public Contract GetContract(string account) {
            return this.envWeb3.Eth.GetContract(this.envAbi, account);
        }

        public async Task<bool> CheckUserName(LogonCredentials creds) {
            return await this.envWeb3.Personal.UnlockAccount.SendRequestAsync(creds.GetPublicKey(), creds.GetPassword(), new HexBigInteger(10));
        }

        public async Task DeployContract(string ABI, string byteCode) {
            string senderAddress = EnvContractAccount;
            string password = "1234pass";
            Web3 web3 = envWeb3;
            bool unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, new HexBigInteger(120));
            Assert.True(unlockAccountResult);
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(ABI, byteCode, senderAddress);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            while (receipt == null) {
                Thread.Sleep(5000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            Log.InfoLog(receipt.ContractAddress);
        }

        public async Task GetHashFromContract() {
            Contract cont = this.GetContract(EnvContractAccount);
            Function getFunct = cont.GetFunction("get");
            string output = await getFunct.CallAsync<string>();
            this.MemeHash = output;
        }

        public async Task SetHashForContract(string hash) {
            try {
                HexBigInteger gas = new HexBigInteger(new BigInteger(400000));
                HexBigInteger value = new HexBigInteger(new BigInteger(0));
                Contract cont = this.GetContract(EnvContractAccount);
                Function setFunct = cont.GetFunction("set");

                string transaction = await setFunct.SendTransactionAsync(localAccount, gas, value, hash);

                Log.InfoLog(transaction);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        private string localPrivateKey = "138526a71c4caff6b2243e2bc0cbe620e317163abb16f5c585a3ca923cfeaf42";
        private string DefaultmemeHash = "QmaNdRRK5rVxBiodg8fcSpiPoZHFJuqw5ackGFTacHbbKa";
        private string primaryDefaultHash = "QmZHd1fbAsE4j281P69a9gR8UdoK3G8DsJ2G7oxVQ8osQ3";
        private string atfmeme = "QmSgiPTvE9XZo6YvSs8Xw9HW311aAxLnz9qGqgZDNFj8xj";
    }
}
