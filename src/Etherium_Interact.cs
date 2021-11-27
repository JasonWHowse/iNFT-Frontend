using iNFT.src.Logger;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace iNFT.src {
    class Ethereum_Interact {

        private readonly static string localNetAddress = "HTTP://127.0.0.1:8545";
        private readonly static string testNetAddress = "https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private readonly static string prodNetAddress = "https://mainnet.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";

        private readonly string localContractAccount = "0x9801391dAc40C9DD4FBfFc55f5EC4c5b5fEeD51e";
        private readonly string testContractAccount = "";
        private readonly string prodContractAccount = null;

        private readonly BigInteger localChainID = 5777;
        private readonly BigInteger testChainID = 3;
        private readonly BigInteger prodChainID = 1;

        private readonly Web3 localWeb3;
        private readonly Web3 testNet;
        private readonly Web3 prodNet;

        private Web3 envWeb3;
        private string envAddress;
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
                    this.EnvContractAccount = this.localContractAccount;
                    this.envAddress = localNetAddress;
                    this.envChainID = this.localChainID;
                    break;
                case Crypto.ROPSTEN:
                    this.envWeb3 = this.testNet;
                    this.EnvContractAccount = this.testContractAccount;
                    this.envAddress = testNetAddress;
                    this.envChainID = this.testChainID;
                    break;
                case Crypto.Ethereum_Mainnet:
                    this.envWeb3 = this.prodNet;
                    this.EnvContractAccount = this.prodContractAccount;
                    this.envAddress = prodNetAddress;
                    this.envChainID = this.prodChainID;
                    break;
                default:
                    break;
            }
        }


        public async Task GetAccountBalance(string account) {
            Log.InfoLog("Balance = " + (await this.envWeb3.Eth.GetBalance.SendRequestAsync(account)).Value.ToString());
        }

        public Contract GetContract() {
            return this.envWeb3.Eth.GetContract(Contract_Details.NFT_API, this.EnvContractAccount);
        }

        public async Task<decimal> CheckUserName(string privateKey) {
            this.account = new Account(privateKey, this.envChainID);
            this.envWeb3 = new Web3(this.account, this.envAddress);
            return Web3.Convert.FromWei(await this.envWeb3.Eth.GetBalance.SendRequestAsync(this.account.Address));
        }

        public void Logout() {
            this.account = null;
            this.SetEnvironment(this.chain);
        }

        public async Task GetHashFromContract() {
            Contract cont = this.GetContract();
            object[] parameters = new object[] { this.account.Address };
            Function getFunct = cont.GetFunction("balanceOf");
            string output = (await getFunct.CallAsync<int>(parameters)).ToString();
            Log.InfoLog(output);
            getFunct = cont.GetFunction("ownerOf");Log.InfoLog("ln123");
            parameters = new object[] { new BigInteger(new HexBigInteger("0x9dfa6381a7a4c9d157cb152ba58e07107cc387932249cb4c45a7869014fa4a20")) }; Log.InfoLog("ln124");
            try {
                output = (await getFunct.CallAsync<int>(parameters)).ToString();
                Log.InfoLog(output);
            }catch(Exception e) {
                Log.ErrorLog(e);
                Log.ErrorLog(e.GetType().ToString());
            }
        }

        public async Task<bool> Mint(string hash) {
            try {
                HexBigInteger gas = new HexBigInteger(new BigInteger(400000));
                HexBigInteger value = new HexBigInteger(new BigInteger(0));
                Contract cont = this.GetContract();
                Function setFunct = cont.GetFunction("mint");
                object[] parameters = { this.account.Address, hash };
                this.envWeb3.TransactionManager.UseLegacyAsDefault = true;
                string transaction = await setFunct.SendTransactionAsync(this.account.Address, gas, value, parameters);

                Log.InfoLog(transaction);
                return true;
            } catch (Exception e) {
                Log.ErrorLog(e);
                return false;
            }
        }
    }
}
