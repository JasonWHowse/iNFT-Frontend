using iNFT.src.helper_functions;
using iNFT.src.Logger;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace iNFT.src {

    /// <summary>
    /// Class to deploy contracts from abis and bytecode
    /// </summary>
    public class Deploy_Contract {

        private static string TransactionHash = "";
        private static string ContAddress = "";
        private static readonly string pathLocation = GetProjectPath() + @"\build\contracts\";
        private static readonly string pathName = GetProjectPath() + @"\build\contracts\";
        private static readonly string DeploymentPrivateKey = "3108730f93f6852632793acfe849b846bf7e2ee129a0ad84c932b89ef1f00ef2";//this is a local key not available on prod or test accounts.  Update as needed.  Do not leave Main_net private keys here
        private static readonly string localAddress = "HTTP://127.0.0.1:8545";
        private static readonly BigInteger ChainID = new BigInteger(5777);

        /// <summary>
        /// Gets the primary project path
        /// </summary>
        /// <returns></returns>
        private static string GetProjectPath() {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Split("bin")[0];
        }//private static string GetProjectPath() {

        /// <summary>
        /// deploys a smart contract, a lot of variables need to be configured manually before using
        /// </summary>
        public static async void Contract_Preparation() {
            try {
                string[] files = Directory.GetFiles(pathName);
                JObject[] jsonArray = new JObject[files.Length];
                Log.InfoLog("filelength = " + files.Length + " jsonarraylength = " + jsonArray.Length);
                int index = 0;
                foreach (string file in files) {
                    jsonArray[index++] = Helpers.GetJsonObject(file);
                }//foreach (string file in files) {
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
                for (int i = 0; i < jsonArray.Length; i++) {
                    /*if (!((string)jsonArray[i]["contractName"]).ToLower().Contains("nft")) {
                        continue;
                    }*/
                    try {
                        TransactionHash = "";
                        ContAddress = "";
                        JObject json = jsonArray[i];
                        bool? test = null;
                        try {
                            test = await DeployContract(json["abi"].ToString(), (string)json["bytecode"]);
                        } catch (Exception e) {//try {
                            Log.WarningLog("Failed to deploy" + (string)json["contractName"]);
                            Log.ErrorLog(e);
                            continue;
                        }//catch (Exception e) {
                        while (test == null) {
                            Thread.Sleep(5000);
                        }//while (test == null) {
                        if (TransactionHash.Length != 0) {
                            JObject networks = (JObject)json["networks"];
                            networks.AddFirst(new JProperty(ChainID.ToString(), JObject.Parse("{}")));
                            JObject networks5777 = (JObject)networks[ChainID.ToString()];
                            networks5777.AddFirst(new JProperty("events", JObject.Parse("{}")));
                            networks5777.Property("events").AddAfterSelf(new JProperty("links", JObject.Parse("{}")));
                            networks5777.Property("links").AddAfterSelf(new JProperty("address", ContAddress));
                            networks5777.Property("address").AddAfterSelf(new JProperty("transactionHash", TransactionHash));
                            using (StreamWriter fs = File.CreateText(pathLocation + (string)json["contractName"] + ".json")) {
                                fs.WriteLine(json.ToString());
                            }//using (StreamWriter fs = File.CreateText(pathLocation + (string)json["contractName"] + ".json")) {
                            Log.InfoLog("Successfully deployed " + (string)json["contractName"]);
                        } else {//if (TransactionHash.Length != 0) {
                            Log.WarningLog("Failed to deploy" + (string)json["contractName"]);
                        }//else {
                    } catch (Exception e) {//try {
                        Log.ErrorLog(e);
                        Log.WarningLog("Failed to deploy" + (string)jsonArray[i]["contractName"]);
                    }//catch (Exception e) {
                }
            } catch (Exception e) {//try {
                Log.ErrorLog(e);
            }//catch (Exception e) {
        }//public static async void Contract_Preparation() {

        private static async Task<bool?> DeployContract(string ABI, string byteCode) {
            Account account = new Account(DeploymentPrivateKey, ChainID);
            Web3 web3 = new Web3(account, localAddress);
            web3.TransactionManager.UseLegacyAsDefault = true;
            string fromAddress = web3.TransactionManager?.Account?.Address;

            Log.WarningLog("Estimated Gas: " + (await web3.Eth.DeployContract.EstimateGasAsync(ABI, byteCode, fromAddress)).ToString());

            string transactionHash = await web3.Eth.DeployContract.SendRequestAsync(ABI, byteCode, fromAddress, new HexBigInteger(new BigInteger(20000000000)));
            Nethereum.RPC.Eth.DTOs.TransactionReceipt receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null) {
                Thread.Sleep(5000);
            }

            ContAddress = receipt.ContractAddress;
            TransactionHash = receipt.TransactionHash;
            return true;
        }//private static async Task<bool?> DeployContract(string ABI, string byteCode) {
    }//public class Deploy_Contract {
}//namespace iNFT.src {