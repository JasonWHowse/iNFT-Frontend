using System;
using System.IO;

namespace iNFT.src.Logger {

    /// <summary>
    ///Creates a log file that is usable from anywhere in the program. 
    ///Only 1 logfile is useable at a time.  Use a StartLogger method
    ///to use a change the log file used.
    /// </summary>
    public class Log {

        private static string _fileName;
        private static bool reportingErrors = true;
        private static bool reportingWarnings = true;

        /// <summary>
        /// Sets the beginning of a log Set or can be used to mark a new log 
        /// set area
        /// </summary>
        private static void BeginningOfLog() {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine();
                fs.WriteLine("==============================================");
                fs.WriteLine("New Log Started: " + DateTime.Now.ToString());
                fs.WriteLine("==============================================");
            }//using (StreamWriter fs = File.AppendText(_fileName)) {
        }//private static void BeginningOfLog() {

        /// <summary>
        /// Generates Info log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void InfoLog(string message) {
            LogMessage("INFO", message);
        }

        /// <summary>
        /// Generates Info log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void InfoLog(object message) {
            LogMessage("INFO", message.ToString());
        }//public static void InfoLog(object message) {

        /// <summary>
        /// Generates Warning log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void WarningLog(string message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message);
            }//if (reportingWarnings) {
        }//public static void WarningLog(string message) {

        /// <summary>
        /// Generates Warning log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void WarningLog(object message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message.ToString());
            }//if (reportingWarnings) {
        }//public static void WarningLog(object message) {

        /// <summary>
        /// Generates Error log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void ErrorLog(string message) {
            if (reportingErrors) {
                LogMessage("ERROR", message);
            }//if (reportingErrors) {
        }//public static void ErrorLog(string message) {

        /// <summary>
        /// Generates Error log entry on the log file.
        /// </summary>
        /// <param name="message"></param>
        public static void ErrorLog(object message) {
            if (reportingErrors) {
                LogMessage("ERROR", message.ToString());
            }//if (reportingErrors) {
        }//public static void ErrorLog(object message) {

        /// <summary>
        /// Generates Error log entry on the log file.
        /// </summary>
        /// <param name="e"></param>
        internal static void ErrorLog(Exception e) {
            if (reportingErrors) {
                ErrorLog(e.Message.ToString());
                ErrorLog(e.StackTrace.ToString());
            }//if (reportingErrors) {
        }//internal static void ErrorLog(Exception e) {

        /// <summary>
        /// Deletes all text from the current log file.
        /// </summary>
        public static void ClearLog() {
            using (StreamWriter fs = File.CreateText(_fileName)) {
                ;
            }//using (StreamWriter fs = File.CreateText(_fileName)) {
        }//public static void ClearLog() {

        /// <summary>
        /// Gets the file path and name of the current log file
        /// </summary>
        /// <returns></returns>
        public static string GetFileName() {
            return _fileName;
        }//public static string GetFileName() {

        /// <summary>
        /// Gets all text as a string of the current log file.
        /// </summary>
        /// <returns></returns>
        public static string GetLog() {
            return File.ReadAllText(_fileName);
        }//public static string GetLog() {

        /// <summary>
        /// Writes to file from one of the log messages
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        private static void LogMessage(string type, string message) {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine(type + " - " + DateTime.Now.ToString() + ": " + message);
            }//using (StreamWriter fs = File.AppendText(_fileName)) {
        }//private static void LogMessage(string type, string message) {

        /// <summary>
        /// Default path/filename file starter
        /// </summary>
        public static void StartLogger() {
            _fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\log.log";
            BeginningOfLog();
        }//public static void StartLogger() {

        /// <summary>
        /// sets custom path but default filename
        /// </summary>
        /// <param name="path"></param>
        public static void StartLogger(string path) {
            _fileName = path + "log.log";
            BeginningOfLog();
        }//public static void StartLogger(string path) {

        /// <summary>
        /// sets custom filename and path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public static void StartLogger(string path, string fileName) {
            _fileName = path + fileName;
            BeginningOfLog();
        }//public static void StartLogger(string path, string fileName) {

        /// <summary>
        /// Turns off error messages
        /// </summary>
        /// <returns></returns>
        public static bool Toggle_Errors() {
            reportingErrors = !reportingErrors;
            return reportingErrors;
        }//public static bool Toggle_Errors() {

        /// <summary>
        /// Turns off warning message
        /// </summary>
        /// <returns></returns>
        public static bool Toggle_Warnings() {
            reportingWarnings = !reportingWarnings;
            return reportingWarnings;
        }//public static bool Toggle_Warnings() {
    }//public class Log {
}//namespace iNFT.src.Logger {