using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Thread = System.Threading.Thread;

using Newtonsoft.Json;

// For ambiguity between XML's and JSON's enums
using Formatting = Newtonsoft.Json.Formatting;

namespace BinaryXML
{
    public enum Format
    {
        XML, JSON, Binary
    }

    public static partial class Program
    {
        public static List<JsonConverter> JsonConverters = new List<JsonConverter>()
        {
            new Map.JsonParser()
        };

        public static bool Minify;
        public static bool NoIndent;
        public static bool NewLines;
        public static bool Timer;
        public static bool Watch;
        public static bool Test;

        public static void Main(string[] args)
        {
#if !DEBUG
            try
            {
                Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
#else
            //Run("-help".Split(' '));

            Run("1H-ForsakenCity.bin -test -timer -noindent".Split(' '));

            //Run("1H-ForsakenCity.bin -test -timer".Split(' '));

            /*
            Run("Test.bin Test.json -timer".Split(' '));
            Console.WriteLine();
            Run("Test.json Test.json.bin -timer".Split(' '));
            //*/
#endif

            Console.Write(_CONTINUE);
            Console.ReadKey();
        }

        public static void Run(string[] args)
        {
            #region Inputs Validation
            if (args.Length == 0)
            {
                Console.WriteLine(_SHORTHELP);
                return;
            }
            else if (args[0] == "-help")
            {
                Console.WriteLine(_LONGHELP);
                return;
            }

            var input = new FileInfo(args[0]);

            if (!input.Exists)
            {
                Console.WriteLine(_FILENOTFOUND, _INPUTFILE, input.FullName);
                return;
            }

            Console.WriteLine(_FILEFOUND, _INPUTFILE, input.FullName);
            #endregion

            #region Initial Parsing
            FileInfo output = null;
            Format? _inputFormat = null;
            Format? _outputFormat = null;

            var flags = new List<string>();
            int i = 1;
            for (; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("-")) break;

                // Read optional parameters that haven't been set yet in their specific order
                if (output == null && TryReadOutput(arg) ||
                    !_inputFormat.HasValue && TryReadInputFormat(arg) ||
                    !_outputFormat.HasValue && TryReadOutputFormat(arg))
                    continue;
            }
            #endregion

            #region Flags
            for (; i < args.Length; i++)
            {
                var arg = args[i];
                if (!arg.StartsWith("-")) Console.WriteLine(_INVALIDFLAG, arg);
                flags.Add(arg.Substring(1).ToLowerInvariant());
            }

            if (flags.Count > 0) Console.WriteLine();
            foreach (string flag in flags)
            {
                switch (flag)
                {
                    case "minify":
                        Minify = true;
                        Console.WriteLine(_FLAG, flag, _MINIFY);
                        break;
                    case "noindent":
                        NoIndent = true;
                        Console.WriteLine(_FLAG, flag, _NOINDENT);
                        break;
                    case "newlines":
                        NewLines = true;
                        Console.WriteLine(_FLAG, flag, _NEWLINES);
                        break;
                    case "timer":
                        Timer = true;
                        Console.WriteLine(_FLAG, flag, _TIMER);
                        break;
                    case "watch":
                        Watch = true;
                        Console.WriteLine(_FLAG, flag, _WATCH);
                        break;
                    case "test":
                        Console.WriteLine(_FLAG, flag, _TEST);
                        Test = true;
                        break;
                    default:
                        Console.WriteLine(_UNKNOWNFLAG, flag);
                        break;
                }
            }
            if (flags.Count > 0) Console.WriteLine();
            #endregion

            #region Input/Output
            // For some reason, VS can't see how outputFormat will always be assigned a value
            Format inputFormat, outputFormat = Format.XML;

            if (!_inputFormat.HasValue)
            {
                if (TryReadFormat(input.Extension.Substring(1), out Format format))
                {
                    Console.WriteLine(_INFERFORMAT, _INPUT, format.ToString());
                    inputFormat = format;
                }
                else
                {
                    Console.WriteLine(_DEFAUTFORMAT, _INPUT, _BIN);
                    inputFormat = Format.Binary;
                }
            }
            else
                inputFormat = _inputFormat.Value;

            // Test command only needs inputFormat, we need not decide other parameters and log them
            if (Test)
            {
                Console.WriteLine();

                // Watch bad
                Watch = false;

                var xml = input.ChangeExtension(".xml");
                var json = input.ChangeExtension(".json");

                // Input -> XML
                Start(input, xml, inputFormat, Format.XML);
                // Input -> JSON
                Start(input, json, inputFormat, Format.JSON);

                // XML -> Binary
                Start(xml, xml.AddExtension(".bin"), Format.XML, Format.Binary);
                // JSON -> Binary
                Start(json, json.AddExtension(".bin"), Format.JSON, Format.Binary);

                return;
            }

            if (!_outputFormat.HasValue && output == null)
            {
                AutoOutputFormat();
                AutoOutput();
            }
            else if (!_outputFormat.HasValue)
            {
                if (TryReadFormat(output.Extension.Substring(1), out Format format))
                {
                    Console.WriteLine(_INFERFORMAT, _OUTPUT, format.ToString());
                    outputFormat = format;
                }
                else
                {
                    AutoOutputFormat();
                }
            }
            else if (output == null)
            {
                outputFormat = _outputFormat.Value;
                AutoOutput();
            }
            #endregion

            #region Serialization
            Console.WriteLine();
            Start(input, output, inputFormat, outputFormat);
            #endregion

            #region Utils
            void AutoOutput()
            {
                output = input.ChangeExtension(FormatToExt(outputFormat));
                Console.WriteLine(_AUTOFILE, output.FullName);
            }

            void AutoOutputFormat()
            {
                switch (inputFormat)
                {
                    case Format.Binary:
                        Console.WriteLine(_DEFAUTFORMAT, _OUTPUT, _XML);
                        outputFormat = Format.XML;
                        break;
                    default:
                        Console.WriteLine(_DEFAUTFORMAT, _OUTPUT, _BIN);
                        outputFormat = Format.Binary;
                        break;
                }
            }

            string FormatToExt(Format format)
            {
                switch (format)
                {
                    case Format.Binary:
                        return ".bin";
                    case Format.JSON:
                        return ".json";
                    default:
                        return ".xml";
                }
            }
            #endregion

            #region Argument Parsers
            bool TryReadOutput(string arg)
            {
                var test = arg.ToLowerInvariant();

                // TODO: Figure out a better test
                if (test.Contains("."))
                {
                    output = new FileInfo(arg);
                    Console.WriteLine(_FILEFOUND, _OUTPUTFILE, output.FullName);
                    return true;
                }

                return false;
            }

            bool TryReadFormat(string arg, out Format format)
            {
                switch (arg.ToLowerInvariant())
                {
                    case "xml":
                        format = Format.XML;
                        break;
                    case "json":
                        format = Format.JSON;
                        break;
                    default:
                        if (arg.StartsWith("bin"))
                            format = Format.Binary;
                        else
                        {
                            format = Format.XML;
                            return false;
                        }
                        break;
                }

                return true;
            }

            bool TryReadInputFormat(string arg)
            {
                if (TryReadFormat(arg, out Format format))
                {
                    _inputFormat = format;
                    Console.WriteLine(_FORMATFOUND, _INPUTFILE, _inputFormat.ToString());
                }

                return false;
            }

            bool TryReadOutputFormat(string arg)
            {
                if (TryReadFormat(arg, out Format format))
                {
                    _inputFormat = format;
                    Console.WriteLine(_FORMATFOUND, _OUTPUTFILE, _inputFormat.ToString());
                }

                return false;
            }
            #endregion
        }

        public static void Start(FileInfo input, FileInfo output, Format inputFormat, Format outputFormat)
        {
            if (Test)
            {
                Console.WriteLine("-----------------------------------------------------");
            }

            Console.WriteLine();
            Console.WriteLine(_OPERATION, input.FullName, output.FullName, inputFormat, outputFormat);

            Go();

            if (Watch)
            {
                var lastTime = File.GetLastWriteTime(input.FullName);

                while (true)
                {
                    if (File.GetLastWriteTime(input.FullName) > lastTime)
                    {
                        Console.WriteLine(_CHANGES, DateTime.Now.ToString("hh:mm:ss.fff"));
                        Go();
                        lastTime = File.GetLastWriteTime(input.FullName);
                        Console.WriteLine();
                    }

                    Thread.Sleep(500);
                }
            }

            void Go()
            {
                if (Timer)
                {
                    var timer = new Stopwatch();

                    timer.Restart();
                    FileStream inputStream = input.OpenReadWhenAvailable();
                    FileStream outputStream = output.OpenWriteWhenAvailable();
                    timer.Stop();
                    float filesTime;
                    Console.WriteLine(_STREAMSTIMED, filesTime = timer.FractionalMilliseconds());

                    timer.Restart();
                    var map = Load(inputStream, inputFormat);
                    timer.Stop();
                    float loadtime;
                    Console.WriteLine(_LOADEDTIMED, map.PackageName, loadtime = timer.FractionalMilliseconds());

                    timer.Restart();
                    Serialize(map, outputStream, outputFormat);
                    timer.Stop();
                    float writeTime;
                    Console.WriteLine(_DONETIMED, writeTime = timer.FractionalMilliseconds());

                    Console.WriteLine(_IOTIME, loadtime + writeTime);
                    Console.WriteLine(_TOTALTIME, loadtime + writeTime + filesTime);
                }
                else
                {
                    FileStream inputStream = input.OpenReadWhenAvailable();
                    FileStream outputStream = output.OpenWriteWhenAvailable();
                    Console.WriteLine(_STREAMS);

                    var map = Load(inputStream, inputFormat);
                    Console.WriteLine(_LOADED);

                    Serialize(map, outputStream, outputFormat);
                    Console.WriteLine(_DONE);
                }

                Console.WriteLine();
            }
        }

        public static JsonSerializerSettings GetJsonSettings()
        {
            // TODO: JSON support for noindent flag
            return new JsonSerializerSettings()
            {
                Converters = JsonConverters,
                Formatting = Minify ? Formatting.None : Formatting.Indented
            };
        }

        public static Map Load(FileStream input, Format format)
        {
            var jsonSettings = GetJsonSettings();

            Map map;

            switch (format)
            {
                case Format.XML:
                    var document = new XmlDocument();
                    document.Load(XmlReader.Create(input, new XmlReaderSettings()
                    {
                        CheckCharacters = true,
                        IgnoreWhitespace = false,
                        MaxCharactersInDocument = 0
                    }));
                    map = new Map(document);
                    break;
                case Format.JSON:
                    var bytes = new byte[input.Length];
                    input.Read(bytes, 0, (int)input.Length);
                    map = JsonConvert.DeserializeObject<Map>(Encoding.UTF8.GetString(bytes), jsonSettings);
                    break;
                default:
                    map = new Map(new BinaryReader(input));
                    break;
            }

            input.Close();
            return map;
        }

        public static void Serialize(Map map, FileStream output, Format format)
        {
            var jsonSettings = GetJsonSettings();

            switch (format)
            {
                case Format.Binary:
                    map.Serialize(new BinaryWriter(output));
                    break;
                case Format.JSON:
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(map, jsonSettings));
                    output.Write(bytes, 0, bytes.Length);
                    break;
                default:
                    map.Serialize(XmlWriter.Create(output, new XmlWriterSettings()
                    {
                        IndentChars = NoIndent ? "" : "\t",
                        Indent = !Minify,
                        NewLineOnAttributes = NewLines,
                        WriteEndDocumentOnClose = true,
                        NewLineChars = "\n"
                    }
                    ));
                    break;
            }

            output.Close();
        }
    }
}
