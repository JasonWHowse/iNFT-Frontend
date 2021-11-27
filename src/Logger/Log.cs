using System;
using System.IO;

namespace iNFT.src.Logger {
    //Creates a log file that is usable from anywhere in the program. 
    //Only 1 logfile is useable at a time.  Use a StartLogger method
    //to use a change the log file used.
    public class Log {

        private static string _fileName;
        private static bool reportingErrors = true;
        private static bool reportingWarnings = true;

        //Sets the beginning of a log Set or can be used to mark a new log 
        //set area
        private static void BeginningOfLog() {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine();
                fs.WriteLine("==============================================");
                fs.WriteLine("New Log Started: " + DateTime.Now.ToString());
                fs.WriteLine("==============================================");
            }//using (StreamWriter fs = File.AppendText(_fileName)) {
        }//private static void BeginningOfLog() {

        public static void InfoLog(string message) {
            LogMessage("INFO", message);
        }

        public static void InfoLog(object message) {
            LogMessage("INFO", message.ToString());
        }//public static void InfoLog(object message) {

        public static void WarningLog(string message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message);
            }//if (reportingWarnings) {
        }//public static void WarningLog(string message) {

        public static void WarningLog(object message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message.ToString());
            }//if (reportingWarnings) {
        }//public static void WarningLog(object message) {

        public static void ErrorLog(string message) {
            if (reportingErrors) {
                LogMessage("ERROR", message);
            }//if (reportingErrors) {
        }//public static void ErrorLog(string message) {

        public static void ErrorLog(object message) {
            if (reportingErrors) {
                LogMessage("ERROR", message.ToString());
            }//if (reportingErrors) {
        }//public static void ErrorLog(object message) {

        internal static void ErrorLog(Exception e) {
            if (reportingErrors) {
                ErrorLog(e.Message.ToString());
                ErrorLog(e.StackTrace.ToString());
            }//if (reportingErrors) {
        }//internal static void ErrorLog(Exception e) {

        //Deletes all text from the current log file.
        public static void ClearLog() {
            using (StreamWriter fs = File.CreateText(_fileName)) {
                ;
            }//using (StreamWriter fs = File.CreateText(_fileName)) {
        }//public static void ClearLog() {

        //Gets the file path and name of the current log file
        public static string GetFileName() {
            return _fileName;
        }//public static string GetFileName() {

        //Gets all text as a string of the current log file.
        public static string GetLog() {
            return File.ReadAllText(_fileName);
        }//public static string GetLog() {

        //Writes to file from one of the log messages
        private static void LogMessage(string type, string message) {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine(type + " - " + DateTime.Now.ToString() + ": " + message);
            }//using (StreamWriter fs = File.AppendText(_fileName)) {
        }//private static void LogMessage(string type, string message) {

        //Default path/filename file starter
        public static void StartLogger() {
            _fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\log.log";
            BeginningOfLog();
        }//public static void StartLogger() {

        //sets custom path but default filename
        public static void StartLogger(string path) {
            _fileName = path + "log.log";
            BeginningOfLog();
        }//public static void StartLogger(string path) {

        //sets custom filename and path
        public static void StartLogger(string path, string fileName) {
            _fileName = path + fileName;
            BeginningOfLog();
        }//public static void StartLogger(string path, string fileName) {

        //Turns off error messages
        public static bool Toggle_Errors() {
            reportingErrors = !reportingErrors;
            return reportingErrors;
        }//public static bool Toggle_Errors() {

        //Turns off warning message
        public static bool Toggle_Warnings() {
            reportingWarnings = !reportingWarnings;
            return reportingWarnings;
        }//public static bool Toggle_Warnings() {
    }//public class Log {
}//namespace iNFT.src.Logger {