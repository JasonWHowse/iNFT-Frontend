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
            //Log.Toggle_Errors();
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (false) {
                BigInteger ChainID = new BigInteger(5777);
                Account account = new Account(DeploymentPrivateKey, ChainID);
                Web3 web3 = new Web3(account, localAddress);
                Log.InfoLog("Balance = " + (await web3.Eth.GetBalance.SendRequestAsync(account.Address)).Value.ToString());
                return;
            }
            try {
                string[] files = Directory.GetFiles(pathName);
                JObject[] jsonArray = new JObject[files.Length];
                Log.InfoLog("filelength = " + files.Length + " jsonarraylength = " + jsonArray.Length);
                int index = 0;
                foreach(string file in files) {
                    jsonArray[index++] = Helpers.GetJsonObject(file);
                }
                Log.InfoLog("Start Sleep");
                Thread.Sleep(1000);
                Log.InfoLog("1");
                Thread.Sleep(1000);
                Log.InfoLog("2");
                Thread.Sleep(1000);
                Log.InfoLog("3");
                Thread.Sleep(1000);
                Log.InfoLog("4");
                Thread.Sleep(1000);
                Log.InfoLog("5");
                Log.InfoLog("Stop Sleep");
                //foreach (string abiFile in files) {
                for (int i = 0; i < jsonArray.Length; i++) { 
                    if (!((string)jsonArray[i]["contractName"]).ToLower().Contains("nft")) {
                        continue;
                    }
                    try {
                        TransactionHash = "";
                        ContAddress = "";
                        JObject json = jsonArray[i];
                        bool? test = null;
                        try {
                            test = await DeployContract(json["abi"].ToString(), (string)json["bytecode"]);
                        }catch(Exception e) {
                            Log.WarningLog("Failed to deploy" + (string)json["contractName"]);
                            Log.ErrorLog(e);
                            continue;
                        }
                        while (test == null) {
                            Thread.Sleep(5000);
                        }
                        if (TransactionHash.Length != 0) {
                            JObject networks = (JObject)json["networks"];
                            networks.AddFirst(new JProperty("5777", JObject.Parse("{}")));
                            JObject networks5777 = (JObject)networks["5777"];
                            networks5777.AddFirst(new JProperty("events", JObject.Parse("{}")));
                            networks5777.Property("events").AddAfterSelf(new JProperty("links", JObject.Parse("{}")));
                            networks5777.Property("links").AddAfterSelf(new JProperty("address", ContAddress));
                            networks5777.Property("address").AddAfterSelf(new JProperty("transactionHash", TransactionHash));
                            using (StreamWriter fs = File.CreateText(pathLocation + (string)json["contractName"] + ".json")) {
                                fs.WriteLine(json.ToString());
                            }
                            Log.InfoLog("Successfully deployed " + (string)json["contractName"]);
                        } else {
                            Log.WarningLog("Failed to deploy" + (string)json["contractName"]);
                        }

                    } catch (Exception e) {
                        Log.ErrorLog(e);
                        Log.WarningLog("Failed to deploy" + (string)jsonArray[i]["contractName"]);
                    }
                }
            } catch (Exception e) {
                Log.ErrorLog(e);
            }
        }

        private static string TransactionHash = "";
        private static string ContAddress = "";

        private static string pathLocation = @"D:\Jason Howse\Documents\College\Masters Classes\CSC478 - Software Engineering Capstone\Group Project Information and Assignments\iNFT\Front End\build\contracts\";
        private static string pathName = @"D:\Jason Howse\Documents\College\Masters Classes\CSC478 - Software Engineering Capstone\Group Project Information and Assignments\iNFT\Front End\build\contracts\";

        private static string DeploymentPrivateKey = "2e1df6a9175677f877a5d7b88b409c5cb77d6511a245bb0bb6de92846206eb1f";

        public async Task<bool?> DeployContract(string ABI, string byteCode) {
            BigInteger ChainID = new BigInteger(5777);
            Account account = new Account(DeploymentPrivateKey, ChainID);
            Web3 web3 = new Web3(account, localAddress);
            web3.TransactionManager.UseLegacyAsDefault = true;
            string fromAddress = web3.TransactionManager?.Account?.Address;

            Log.WarningLog("Estimated Gas: " + (await web3.Eth.DeployContract.EstimateGasAsync(ABI, byteCode, fromAddress)).ToString());

            string transactionHash = await web3.Eth.DeployContract.SendRequestAsync(ABI, byteCode, fromAddress, new HexBigInteger(new BigInteger(20000000000)));
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            while (receipt == null) {
                Thread.Sleep(5000);
            }

            //Log.InfoLog("BlockHash: " + receipt.BlockHash);
            //Log.InfoLog("BlockNumber: " + receipt.BlockNumber);
            //Log.InfoLog("ContractAddress: " + receipt.ContractAddress);
            ContAddress = receipt.ContractAddress;
            //Log.InfoLog("CumulativeGasUsed: " + receipt.CumulativeGasUsed);
            //Log.InfoLog("EffectiveGasPrice: " + receipt.EffectiveGasPrice);
            //Log.InfoLog("GasUsed: " + receipt.GasUsed);
            //Log.InfoLog("Status: " + receipt.Status);
            //Log.InfoLog("TransactionHash: " + receipt.TransactionHash);
            TransactionHash = receipt.TransactionHash;
            //Log.InfoLog("trancsactionHash: " + transactionHash);
            //Log.InfoLog("TransactionIndex: " + receipt.TransactionIndex);
            //Log.InfoLog("Type: " + receipt.Type);
            return true;
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
