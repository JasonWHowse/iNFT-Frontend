using iNFT.src.Logger;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace iNFT.src {
    public class Ethereum_Interact {

        private static readonly string localNetAddress = "HTTP://127.0.0.1:8545";
        private static readonly string testNetAddress = "https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private static readonly string prodNetAddress = "https://mainnet.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";

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

        public bool AccountIsNull() {
            return this.account == null;
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
            this.envWeb3 = null;
            this.SetEnvironment(this.chain);
        }

        public async Task<List<string>[]> TokenList() {
            if(this.account == null) {
                return null;
            }
            List<string>[] list = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
            for (int index = 1; index > 0; index++) {
                try {
                    string tempAddress = (await this.GetContract().GetFunction(
                    "ownerOf").CallAsync<object>(new object[] { index })).ToString();
                    if (this.account.Address.Equals(tempAddress)) {
                        list[0].Add(index.ToString());
                        string url = (await this.GetContract().GetFunction(
                    "tokenURI").CallAsync<object>(new object[] { index })).ToString();
                        list[1].Add(url);
                        list[2].Add(url.Split("/")[^1]);
                    }
                } catch (Exception) {
                    index = -100;
                }
            }
            return list;
        }

        public async Task<bool> GetHashFromContract(int index) {
            try {
                return this.account.Address.Equals((await this.GetContract().GetFunction(
                    "ownerOf").CallAsync<object>(new object[] { index })).ToString());
            } catch (Exception e) {
                Log.ErrorLog(e);
                return false;
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
