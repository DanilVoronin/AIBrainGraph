using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tools.LoadSave
{
    /// <summary>
    /// Этот метод сохранения и загрузки сохраняет и загружает файлы как двоичные файлы.
    /// </summary>
    public class MMSaveLoadManagerMethodBinary : IMMSaveLoadManagerMethod
    {
        /// <summary>
        /// Saves the specified object to disk at the specified location after serializing it
        /// </summary>
        /// <param name="objectToSave"></param>
        /// <param name="saveFile"></param>
        public void Save(object objectToSave, FileStream saveFile)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(saveFile, objectToSave);
            saveFile.Close();
        }

        /// <summary>
        /// Loads the specified file from disk and deserializes it
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        public object Load(Type objectType, FileStream saveFile)
        {
            object savedObject;
            BinaryFormatter formatter = new BinaryFormatter();
            savedObject = formatter.Deserialize(saveFile);
            saveFile.Close();
            return savedObject;
        }
    }
}