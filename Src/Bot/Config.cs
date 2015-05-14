using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey
{
    public class Config
    {
        public string UserId;
        public string UserToken;
        public DateTime UserTokenExpiresAt;
        public string AppId;
        public string AppSecret;
        
        public string GetAppToken()
        {
            return AppId + "|" + AppSecret;
        }

        public static Config Load()
        {
            if (!File.Exists(FilePath))
                return new Config();
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(FilePath));
            }
            catch (Exception e)
            {
                File.WriteAllText(DirPath + "config_error_" + DateTime.UtcNow.Ticks +".txt", e.ToString());
                throw e;
            }
        }

        public void Save()
        {
            if (!Directory.Exists(DirPath))
                Directory.CreateDirectory(DirPath);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            File.WriteAllText(FilePath, json);
        }

        static string DirPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/JimCarrey/"; } }
        static string FilePath { get { return DirPath + "config.js"; } }
    }
}
