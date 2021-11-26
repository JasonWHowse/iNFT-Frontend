using iNFT.src.helper_functions;
using iNFT.src.Logger;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Numerics;
using System.Text;
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

        private readonly static string localNetAddress = "HTTP://127.0.0.1:8545";
        private readonly static string testNetAddress = "https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private readonly static string prodNetAddress = "https://mainnet.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";

        private readonly string testAccount = "0xd0fdc799d8125AAb9992e4c7470efB873d1d57dB";
        private readonly string localAccount = "0xe0d9F6E40f8c3fd3b121F54d09E069d51Ba64D96";

        private readonly string localContractAccount = "0xaD5b3231A4c02F5440a59db2284BbE2f89Fa34aA";
        private readonly string testContractAccount = "";

        private BigInteger localChainID = 5777;
        private BigInteger testChainID = 3;
        private BigInteger prodChainID = 1;

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
        public BigInteger envChainID;
        public Crypto chain;

        private Account account;


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
            this.chain = env;
            switch (env) {
                case Crypto.LOCAL:
                    this.envWeb3 = this.localWeb3;
                    this.envAbi = this.localAbi;
                    this.envContractByteCode = this.localByteCode;
                    this.EnvAccount = this.localAccount;
                    this.EnvContractAccount = this.localContractAccount;
                    this.envAddress = localNetAddress;
                    this.envChainID = this.localChainID;
                    break;
                case Crypto.ROPSTEN:
                    this.envWeb3 = this.testNet;
                    this.envAbi = this.testAbi;
                    this.envContractByteCode = this.testByteCode;
                    this.EnvAccount = this.testAccount;
                    this.EnvContractAccount = this.testContractAccount;
                    this.envAddress = testNetAddress;
                    this.envChainID = this.testChainID;
                    break;
                case Crypto.Ethereum_Mainnet:
                    this.envWeb3 = this.prodNet;
                    this.envAbi = this.prodAbi;
                    this.envContractByteCode = this.prodByteCode;
                    this.EnvAccount = this.prodAccount;
                    this.EnvContractAccount = this.prodContractAccount;
                    this.envAddress = prodNetAddress;
                    this.envChainID = this.prodChainID;
                    break;
            }
        }


        public async Task GetAccountBalance(string account) {
            this.AccBal = "Balance = " + (await this.envWeb3.Eth.GetBalance.SendRequestAsync(account)).Value.ToString();
        }

        public Contract GetContract(string account) {
            return this.envWeb3.Eth.GetContract(this.envAbi, account);
        }

        public async Task<decimal> CheckUserName(string privateKey) {
            this.account = new Account(privateKey, envChainID);
            this.envWeb3 = new Web3(account, this.envAddress);
            Log.InfoLog((await this.envWeb3.Eth.GetBalance.SendRequestAsync(this.account.Address)).Value.ToString());
            return Web3.Convert.FromWei(await this.envWeb3.Eth.GetBalance.SendRequestAsync(this.account.Address));
        }

        public void Logout() {
            this.account = null;
            this.SetEnvironment(this.chain);
        }

        public async Task GetHashFromContract() {
            Contract cont = this.GetContract(EnvContractAccount);
            Function getFunct = cont.GetFunction("get");
            string output = await getFunct.CallAsync<string>();
            this.MemeHash = output;
        }

        public async Task Mint(string hash) {
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
