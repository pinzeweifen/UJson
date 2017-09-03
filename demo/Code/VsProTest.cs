using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !UNITY_EDITOR
namespace ConsoleApp1
{
    class Program
    {
        public static string Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string data = sr.ReadToEnd();
            return data;
        }

        static void Main(string[] args)
        {
            var json = Read("F://UnityPro/Practice/demo/Assets/equip.json");
            JsonParseError error = new JsonParseError();
            JsonDocument document = JsonDocument.fromJson(json, ref error);

            if (error.error == ParseError.NoError)
            {
                if (document.isObject())
                {
                    IsObject(document.toObject());
                }
            }
            else
            {
                Console.WriteLine(error.error.ToString());
            }
        }

        private static void IsObject(JsonObject obj)
        {
            foreach (var value in obj.valueAll())
            {
                IsValue(value);
            }
        }

        private static void IsArray(JsonArray array)
        {
            foreach (var value in array.all())
            {
                IsValue(value);
            }
        }

        private static void IsValue(JsonValue value)
        {
            if (value.isArray())
            {
                IsArray(value.toArray());
                return;
            }
            else if (value.isObject())
            {
                IsObject(value.toObject());
                return;
            }

            Console.Write("key: [" + value.toKey() + "]  ");
            if (value.isString())
                Console.WriteLine("value: [\"" + value.toString() + "\"]");
            else
                Console.WriteLine("value: [" + value.toString() + "]");
        }
    }
}
#endif