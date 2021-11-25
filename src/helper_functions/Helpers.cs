using iNFT.src.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace iNFT.src.helper_functions {
    class Helpers {
        public static JObject GetJsonObject(string fileName, string path) {
            JObject output = null;
            try {
                output = (JObject)JsonConvert.DeserializeObject(new StreamReader(path + fileName).ReadToEnd());
            } catch (Exception e) { //try {
                Log.ErrorLog(e);
            }//} catch (Exception) { 
            return output;
        }//public static JObject GetJsonObject(string fileName, string path) {
        public static JObject GetJsonObject(string fileNamepath) {
            JObject output = null;
            try {
                output = (JObject)JsonConvert.DeserializeObject(new StreamReader(fileNamepath).ReadToEnd());
            } catch (Exception e) { //try {
                Log.ErrorLog(e);
            }//} catch (Exception) { 
            return output;
        }//public static JObject GetJsonObject(string fileName, string path) {
    }//class Helpers {
}//namespace iNFT.src.helper_functions {