using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryXML
{
    public static partial class Program
    {
        public const string _SHORTHELP = "Usage: BinaryXML <input> [output] [input format] [output format] [flags]\n" +
            "Available formats: XML, JSON, BIN.\n" +
            "Available flags: -minify, -newlines, -timer, -watch, -test.\n" +
            "Use (BinaryXML -help) to get further details.";

        public const string _LONGHELP = "Usage: BinaryXML <input> [output] [input format] [output format] [flags]\n\n" +
            "The input file is the only required parameter, the tool will figure out the rest based on what it knows:\n" +
            " - Input and output formats will be inferred from file extensions.\n" +
            " - Output file path will be decided using the output format\n" +
            " - Output file format defaults to XML if the input is binary, and to binary otherwise.\n\n" +
            "Parameters will be parsed in their specific order, as in, the first format to be found will always be assumed to be the input format. Parsing parameters will stop when the first flag is found.\n\n" +
            "Available flags:\n" +
            " - minify: " + _MINIFY + "\n" +
            " - newlines: " + _NEWLINES + "\n" +
            " - timer: " + _TIMER + "\n" +
            " - watch: " + _WATCH + "\n" +
            " - test: " + _TEST + "\n";

        public const string _CONTINUE = "Press any key to continue...";

        #region Arguments Parsing
        public const string _FILEFOUND = "{0} file: {1}.";
        public const string _FILENOTFOUND = "{0} file doesn't exist: {1}.";
        public const string _FORMATFOUND = "{0} format: {1}.";

        public const string _AUTOFILE = "Automatically decided output file: {0}";
        public const string _DEFAUTFORMAT = "Defaulted {0} file format: {1}";
        public const string _INFERFORMAT = "Inferred {0} file format from extension: {1}";

        public const string _INPUTFILE = "Input";
        public const string _OUTPUTFILE = "Output";
        public const string _INPUT = "input";
        public const string _OUTPUT = "output";

        public const string _XML = "XML";
        public const string _JSON = "JSON";
        public const string _BIN = "Binary";
        #endregion

        #region Serialization
        public const string _CHANGES = "{0} | Detected changes, serializing...";

        public const string _STREAMS = "Successfully opened files!";
        public const string _LOADED = "Successfully loaded map ({0})!";
        public const string _DONE = "Successfully serialized!";

        public const string _STREAMSTIMED = "Successfully opened files in {0}ms!";
        public const string _LOADEDTIMED = "Successfully loaded map ({0}) in {1}ms!";
        public const string _DONETIMED = "Successfully serialized in {0}ms!";
        public const string _IOTIME = "Total read/write time: {0}ms";
        public const string _TOTALTIME = "Total time: {0}ms";

        public const string _OPERATION = "Starting operation:\n - Input ({2}): {0}\n - Output ({3}): {1}\n";
        #endregion

        #region Flags
        public const string _FLAG = "Found {0} flag. {1}";
        public const string _UNKNOWNFLAG = "Unrecognized flag: {0}.";
        public const string _INVALIDFLAG = "Invalid flag: {0}.";

        public const string _MINIFY = "XML/JSON output will be minified.";
        public const string _NOINDENT = "XML output will have no indentions.";
        public const string _NEWLINES = "XML output will have attributes on separate lines.";
        public const string _TIMER = "Operations will be timed.";
        public const string _WATCH = "Input file will be watched for changes.";
        public const string _TEST = "Input file will be converted into JSON and XML then both output files will be converted into binary.";
        #endregion
    }
}
