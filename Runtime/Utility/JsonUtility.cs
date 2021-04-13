using System.IO;

namespace Framework.Utility
{
    public static class JsonUtility
    {
        public static void SaveJsonToFile(string json, string fileName, string savePath)
        {
            string finalPath = string.Format("{0}/{1}.json", savePath, fileName);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            FileStream file = new FileStream(finalPath, FileMode.Create);
            byte[] bts = System.Text.Encoding.UTF8.GetBytes(json);
            file.Write(bts, 0, bts.Length);
            if (file != null)
            {
                file.Close();
            }
        }
    }
}


