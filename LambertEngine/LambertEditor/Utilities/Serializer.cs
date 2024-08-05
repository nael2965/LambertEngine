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
        public static void ToFile<T>(T instances, string path)
        {
            Debug.WriteLine($"파일에 데이터 저장 시도: {path} (Attempting to save data to file: {path})");
            try
            {
                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instances);
                Debug.WriteLine("데이터 저장 성공 (Data saved successfully)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터 저장 오류: {ex.Message} (Error saving data: {ex.Message})");
            }
        }

        public static T FromFile<T>(string path)
        {
            Debug.WriteLine($"파일에서 데이터 로드 시도: {path} (Attempting to load data from file: {path})");
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs);
                Debug.WriteLine("데이터 로드 성공 (Data loaded successfully)");
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터 로드 오류: {ex.Message} (Error loading data: {ex.Message})");
                return default(T);
            }
        }
    }
}
