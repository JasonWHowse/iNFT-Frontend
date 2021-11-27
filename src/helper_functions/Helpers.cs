using iNFT.src.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace iNFT.src.helper_functions {
    class Helpers {
        //Attemps to get a manipulatable JObject from a json file, full path accepted
        public static JObject GetJsonObject(string fileNamepath) {
            JObject output = null;
            try {
                output = (JObject)JsonConvert.DeserializeObject(new StreamReader(fileNamepath).ReadToEnd());
            } catch (Exception e) { //try {
                Log.ErrorLog(e);
            }//} catch (Exception) { 
            return output;
        }//public static JObject GetJsonObject(string fileNamepath) {

        //API for filename and path 
        public static JObject GetJsonObject(string fileName, string path) {
            return GetJsonObject(path + fileName);
        }//public static JObject GetJsonObject(string fileName, string path) {
    }//class Helpers {
}//namespace iNFT.src.helper_functions {