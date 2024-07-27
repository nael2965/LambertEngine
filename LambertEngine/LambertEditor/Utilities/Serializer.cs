using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LambertEditor.Utilities
{
    public static class Serializer
    {
        public static void ToFile<T>(T instances, string path){
            try
            {
                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instances);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                //TODO: error log 

            }
        }
        
        public static T FromFile<T>(string path){
            try{
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs);
                Debug.Write("a");
                return instance;
            }catch (Exception ex){
                Debug.Write(ex.Message);
                Debug.Write("B");
                //TODO: error log
                return default(T);
            }
        }





    }
}
