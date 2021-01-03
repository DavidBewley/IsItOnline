using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IsTheServerUp.Repos
{
    public class LastStatusRepo
    {
        public bool WasPreviouslyOnline()
        {
            var reader = new StreamReader("PreviousStatus.json");
            var result = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            return JsonConvert.DeserializeObject<bool>(result);
        }

        public void SaveIsOnlineStatus(bool isOnline)
        {
            var writer = new StreamWriter("PreviousStatus.json");
            writer.Write(JsonConvert.SerializeObject(isOnline));
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
    }
}
