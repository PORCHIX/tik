using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseNS {
    class DataBase {
        public static async Task<List<string>> GetUserChannelList(long userId) {
            using (StreamReader sr = new($"../../../Users/{userId}.txt")) {
                var fileContent = await sr.ReadToEndAsync();
                var channelList = fileContent.Split('@', StringSplitOptions.RemoveEmptyEntries).ToList();
                for(int i = 0; i < channelList.Count; i++) {
                    channelList[i] = channelList[i].Trim(new[] { '\n', '\r' }); 
                }
                return channelList;
            }
        }
        public static async Task AddToUserFile_AddingChanel_Status (long userId) {
            using (StreamWriter sw = new($"../../../Users/{userId}.txt",true)) {
                await sw.WriteLineAsync("Status: adding channel");
            }
        }
        public static async Task RemoveFromUserFile_AddingChannel_Status (long userId) {
            string filePath = $"../../../Users/{userId}.txt";
            var buffer = await File.ReadAllLinesAsync(filePath);
            using (StreamWriter sw = new(filePath)) {
                foreach(var item in buffer) {
                    if(item!= "Status: adding channel")
                        await sw.WriteLineAsync(item);
                }
            }
        }
        public static async Task RemoveFromFile_ChannelName(long userId, string channel) {
            string filePath = $"../../../Users/{userId}.txt";
            var buffer = await File.ReadAllLinesAsync(filePath);
            using (StreamWriter sw = new(filePath)) {
                foreach (var item in buffer) {
                    if (item != $"@{channel}")
                        await sw.WriteLineAsync(item);
                }
            }
        }
        public static async Task<bool> IsUserAddingChannel (long userId) {
            string filePath = $"../../../Users/{userId}.txt";
            string[] buffer = await File.ReadAllLinesAsync(filePath);
            return Array.Exists(buffer, el => el.Contains("Status: adding channel"));
        }
    }
    
}
