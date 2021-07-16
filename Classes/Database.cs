using System;
using System.Collections.Generic;
using System.IO;

namespace DataBaseNS {
    class Database {
        string dataBasePath;
        static List<string> dbList = new List<string>();
        public Database(string dataBasePath) {
            this.dataBasePath = dataBasePath;
            
        }
        public void AddChannel(string userName, string channelName) {
            using (StreamReader sr = new StreamReader(dataBasePath)) {
                if (!sr.ReadToEnd().Contains(channelName)) {
                    var userIndex = sr.ReadToEnd().IndexOf(userName);
                    sr.Close();
                    using (StreamWriter sw = new StreamWriter(dataBasePath, append: true)) {
                        sw.Write($"User: {userName}");
                        sw.Write($" channelName: {channelName}");
                    }
                }
            }
        }
    }
}
