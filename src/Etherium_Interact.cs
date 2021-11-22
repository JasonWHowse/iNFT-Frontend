using iNFT.src.Logger;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace iNFT.src {
    class Ethereum_Interact {

        public string MemeHash { get; private set; }

        public string AccBal { get; private set; }
        //public Contract _Contract { get; private set; }

        private readonly string localAbi = "[{\"constant\":false,\"inputs\":[{\"name\":\"_memeHash\",\"type\":\"string\"}],\"name\":\"set\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\",\"signature\":\"0x4ed3885e\"},{\"constant\":true,\"inputs\":[],\"name\":\"get\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0x6d4ce63c\"}]";
        private readonly string testAbi = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"},{\"indexed\":true,\"internalType\":\"bytes\",\"name\":\"result\",\"type\":\"bytes\"}],\"name\":\"ExecutionResult\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"foo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        private readonly string prodAbi = null;

        private readonly string localByteCode = "";
        private readonly string testByteCode = "";
        private readonly string prodByteCode = "";

        private readonly static string localAddress = "HTTP://127.0.0.1:8545";
        private readonly static string testNetAddress = "https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private readonly static string prodNetAddress = "https://mainnet.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";

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
        private string envAddress;
        private string envAbi;
        private string envContractByteCode;
        public string EnvAccount { get; private set; }
        public string EnvContractAccount { get; private set; }


        public enum Crypto { LOCAL, ROPSTEN, Ethereum_Mainnet }

        public Ethereum_Interact() {
            try {
                this.localWeb3 = new Web3(testNetAddress);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
            try {
                this.testNet = new Web3(testNetAddress);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
            try {
                this.prodNet = new Web3(prodNetAddress);
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        public Ethereum_Interact(Crypto env) : this() {
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
                    this.envAddress = localAddress;
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
            return true;// await this.envWeb3.Personal.UnlockAccount.SendRequestAsync(creds.GetPublicKey(), creds.GetPassword(), new HexBigInteger(10));
        }

        public async void deletethis2contractDeploy() { //todo: deletethis
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try {
                //envWeb3 = new Web3(new Account("272a956ed83288cd665ce55283a780ecbaf0eaa264529664355b0fdfe88e9abf"), envAddress);
                envWeb3 = new Web3(localAddress);
                await DeployContract(Contract_Details.API, Contract_Details.ByteCode);
            }catch(Exception e) {
                Log.ErrorLog(e);
            }
        }



        public async Task DeployContract(string ABI, string byteCode) {
            string senderAddress = "0x18b528231536146e4648f37A89005B51b26B505B";// EnvContractAccount;
            string password = "1234pass";
            Web3 web3 = envWeb3;
            bool unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, new HexBigInteger(120));
            Assert.True(unlockAccountResult);
            string transactionHash = await web3.Eth.DeployContract.SendRequestAsync(ABI, byteCode, senderAddress, new HexBigInteger(new BigInteger(6721975)));
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
    }
}
