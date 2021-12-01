using iNFT.src.Model;
using iNFT.src.Utilities;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace iNFT.src {
    /// <summary>
    /// Class to interact with the nft Contract
    /// </summary>
    public class Ethereum_Interact {

        private static readonly string localNetAddress = "HTTP://127.0.0.1:7545";
        private static readonly string prodNetAddress = "https://mainnet.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private static readonly string testNetAddress = "https://ropsten.infura.io/v3/c403a4afb4f5439588595f1f242e7c75";
        private readonly BigInteger localChainID = 5777;
        private readonly BigInteger prodChainID = 1;
        private readonly BigInteger testChainID = 3;
        private readonly string localContractAddress = "0x9801391dAc40C9DD4FBfFc55f5EC4c5b5fEeD51e";
        private readonly string prodContractAddress = "";
        private readonly string testContractAddress = "0x39DabC1a0E99B7A0b93A1894d1fa5c340bcd5a3A";
        private readonly Web3 localWeb3;
        private readonly Web3 prodNet;
        private readonly Web3 testNet;

        private Account account;
        private Web3 envWeb3;
        private string envAddress;

        /// <summary>
        /// Current Environment Network ID/Chain ID
        /// </summary>
        public BigInteger envChainID;

        /// <summary>
        /// Which Environment is being used
        /// </summary>
        public Crypto chain;

        /// <summary>
        /// Enum to choose which account is being used
        /// </summary>
        public enum Crypto {

            /// <summary>
            /// Local account 
            /// </summary>
            Local,

            /// <summary>
            /// Test account 
            /// </summary>
            Ropsten,

            /// <summary>
            /// Production account 
            /// </summary>
            Ethereum_Mainnet
        }

        /// <summary>
        /// Account address associated with the contract
        /// </summary>
        public string EnvContractAccount { get; private set; }

        /// <summary>
        /// Initializes the web3 contracts for each environment
        /// </summary>
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

        /// <summary>
        /// Constructor that sets the environment variables
        /// </summary>
        /// <param name="env"></param>
        public Ethereum_Interact(Crypto env) : this() {
            this.SetEnvironment(env);
        }
        /// <summary>
        /// Returns a boolean based on whether or not the Address has a Token at a particular index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountByIndex(int index) {
            try {
                return this.account.Address.Equals((await this.GetContract().GetFunction(
                    "ownerOf").CallAsync<object>(new object[] { index })).ToString());
            } catch (Exception e) {
                Log.ErrorLog(e);
                return false;
            }
        }
                /// <summary>
        /// Attempts to post a token to the block chain network and returns true if its successful
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<bool> Mint(string hash) {
            try {
                HexBigInteger gas = new HexBigInteger(new BigInteger(8000000));
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
        /// <summary>
        /// Sets the current environment and address to the appropriate private key
        /// return the account balance associated with the account
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public async Task<decimal> CheckUserName(string privateKey) {
            this.account = new Account(privateKey, this.envChainID);
            this.envWeb3 = new Web3(this.account, this.envAddress);
            return await this.GetAccountBalance(this.account.Address);
        }

        /// <summary>
        /// Returns address account
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<decimal> GetAccountBalance(string address) {
            return Web3.Convert.FromWei(await this.envWeb3.Eth.GetBalance.SendRequestAsync(address));
        }

        /// <summary>
        /// Returns all tokens associated with a particular account
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>[]> TokenList() {
            if (this.account == null) {
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

        /// <summary>
        /// return bool if the Account variable is null.
        /// </summary>
        /// <returns></returns>
        public bool AccountIsNull() {
            return this.account == null;
        }

        /// <summary>
        /// Gets the Contract detail
        /// </summary>
        /// <returns></returns>
        public Contract GetContract() {
            return this.envWeb3.Eth.GetContract(Contract_Details.NFT_ABI, this.EnvContractAccount);
        }

        /// <summary>
        /// logs user out by destroying environments
        /// </summary>
        public void Logout() {
            this.account = null;
            this.envWeb3 = null;
            this.SetEnvironment(this.chain);
        }

        /// <summary>
        /// Sets the environment variables based on locat, Ropsten, Main_Net
        /// </summary>
        /// <param name="env"></param>
        public void SetEnvironment(Crypto env) {
            this.chain = env;
            switch (env) {
                case Crypto.Local:
                    this.envWeb3 = this.localWeb3;
                    this.EnvContractAccount = this.localContractAddress;
                    this.envAddress = localNetAddress;
                    this.envChainID = this.localChainID;
                    break;
                case Crypto.Ropsten:
                    this.envWeb3 = this.testNet;
                    this.EnvContractAccount = this.testContractAddress;
                    this.envAddress = testNetAddress;
                    this.envChainID = this.testChainID;
                    break;
                case Crypto.Ethereum_Mainnet:
                    this.envWeb3 = this.prodNet;
                    this.EnvContractAccount = this.prodContractAddress;
                    this.envAddress = prodNetAddress;
                    this.envChainID = this.prodChainID;
                    break;
                default:
                    break;
            }
        }
    }
}