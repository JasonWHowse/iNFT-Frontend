using System;
using System.IO;

namespace iNFT.src.Logger {
    public class Log {

        private static bool reportingErrors = true;
        private static bool reportingWarnings = true;

        public static bool Toggle_Errors() {
            reportingErrors = !reportingErrors;
            return reportingErrors;
        }

        public static bool Toggle_Warnings() {
            reportingWarnings = !reportingWarnings;
            return reportingWarnings;
        }

        private static string _fileName;

        public static void StartLogger() {//Default path constructor
            _fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\log.log";
            BeginningOfLog();
        }

        public static void StartLogger(string path) {//sets path but not name
            _fileName = path + "log.log";
            BeginningOfLog();
        }

        public static void StartLogger(string path, string fileName) {//sets path and name
            _fileName = path + fileName;
            BeginningOfLog();
        }

        private static void BeginningOfLog() {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine();
                fs.WriteLine("==============================================");
                fs.WriteLine("New Log Started: " + DateTime.Now.ToString());
                fs.WriteLine("==============================================");
            }
        }

        public static string GetFileName() {
            return _fileName;
        }

        public static void InfoLog(string message) {
            LogMessage("INFO", message);
        }

        public static void InfoLog(object message) {
            LogMessage("INFO", message.ToString());
        }

        public static void ErrorLog(string message) {
            if (reportingErrors) {
                LogMessage("ERROR", message);
            }
        }

        public static void ErrorLog(object message) {
            if (reportingErrors) {
                LogMessage("ERROR", message.ToString());
            }
        }

        internal static void ErrorLog(Exception e) {
            if (reportingErrors) {
                ErrorLog(e.Message.ToString());
                ErrorLog(e.StackTrace.ToString());
            }
        }

        public static void WarningLog(string message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message);
            }
        }

        public static void WarningLog(object message) {
            if (reportingWarnings) {
                LogMessage("WARNING", message.ToString());
            }
        }

        private static void LogMessage(string type, string message) {
            using (StreamWriter fs = File.AppendText(_fileName)) {
                fs.WriteLine(type + " - " + DateTime.Now.ToString() + ": " + message);
            }
        }

        public static void ClearLog() {
            using (StreamWriter fs = File.CreateText(_fileName)) {
                ;
            }
        }

        public static string GetLog() {
            return File.ReadAllText(_fileName);
        }
    }
}
