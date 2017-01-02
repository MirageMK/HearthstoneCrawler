using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;

namespace HSCore.Extensions
{
    public static class IsolatedStorageExtensions
    {
        public static void SaveObject(this IsolatedStorage isoStorage, object obj, string fileName)
        {
            IsolatedStorageFileStream writeStream = new IsolatedStorageFileStream(fileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writeStream, obj);
            writeStream.Flush();
            writeStream.Close();
        }

        public static T LoadObject<T>(this IsolatedStorage isoStorage, string fileName)
        {
            try
            {
                IsolatedStorageFileStream readStream = new IsolatedStorageFileStream(fileName, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                T readData = (T) formatter.Deserialize(readStream);
                readStream.Flush();
                readStream.Close();

                return readData;
            }
            catch(Exception)
            {
                return default(T);
            }
        }
    }
}
