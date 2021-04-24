using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Thread = System.Threading.Thread;

using Newtonsoft.Json;

namespace BinaryXML
{
    public static class Extensions
    {
        public static void OptionalAdd(this Dictionary<string, short> strings, string str)
        {
            if (!strings.ContainsKey(str)) strings.Add(str, (short)strings.Count);
        }

        public static float FractionalMilliseconds(this Stopwatch watch)
            => watch.ElapsedTicks / 10000f;

        public static void Time(this Stopwatch watch, Action func, string message)
        {
            watch.Restart();
            func();
            watch.Start();
            Console.WriteLine(message, watch.FractionalMilliseconds());
        }

        // Funnily enough, calling those methods and initiating their stacks/loops takes enough time for most editors
        // to release the handle for the file
        // But we should still account for when that doesn't happen
        public static FileStream OpenReadWhenAvailable(this FileInfo info)
        {
            bool failed = false;

            while (true)
            {
                try
                {
                    if (failed) Console.WriteLine();
                    return info.OpenRead();
                }
                catch (IOException)
                {
                    failed = true;
                    Console.WriteLine("Cannot open file, retrying...");
                    Thread.Sleep(1000);
                }
            }
        }

        public static FileStream OpenWriteWhenAvailable(this FileInfo info)
        {
            bool failed = false;

            while (true)
            {
                try
                {
                    if (failed) Console.WriteLine();
                    return info.OpenWrite();
                }
                catch (IOException)
                {
                    failed = true;
                    Console.WriteLine("Cannot open file, retrying...");
                    Thread.Sleep(1000);
                }
            }
        }

        public static FileInfo ChangeExtension(this FileInfo info, string newExt)
        {
            return new FileInfo(Path.ChangeExtension(info.FullName, newExt));
        }

        public static FileInfo AddExtension(this FileInfo info, string newExt)
        {
            return new FileInfo(info.FullName + newExt);
        }

        public static void ReadUntil(this JsonReader reader, JsonToken token)
        {
            while (reader.TokenType != token) reader.Read();
        }

        public static void ReadUntilPast(this JsonReader reader, JsonToken token)
        {
            reader.ReadUntil(token);
            reader.Read();
        }

        public static string ReadStringProperty(this JsonReader reader)
        {
            reader.ReadUntil(JsonToken.PropertyName);
            return reader.ReadAsString();
        }

        public static int ReadIntProperty(this JsonReader reader)
        {
            reader.ReadUntil(JsonToken.PropertyName);
            return reader.ReadAsInt32() ?? 0;
        }

        public static List<T> ReadArrayProperty<T>(this JsonReader reader, Func<JsonReader, T> elementReader)
        {
            var list = new List<T>();

            reader.ReadUntil(JsonToken.PropertyName);
            reader.ReadUntilPast(JsonToken.StartArray);

            while (reader.TokenType != JsonToken.EndArray)
            {
                reader.Read();
                list.Add(elementReader(reader));
            }

            reader.Read();

            return list;
        }
    }
}
