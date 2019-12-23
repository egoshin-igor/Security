using CommandLine;

namespace Cezar
{
    [Verb( "encode", HelpText = "Encode file contents with key" )]
    public class EncodeOptions
    {
        [Option( "in", Required = true, HelpText = "Input file path" )]
        public string InputFile { get; set; }
        [Option( "out", Required = true, HelpText = "Encoded file path" )]
        public string OutputFile { get; set; }
        [Option( "key", Required = true, HelpText = "Encoding key" )]
        public string Key { get; set; }
    }

    [Verb( "decode", HelpText = "Decode file contents with key" )]
    public class DecodeOptions
    {
        [Option( "in", Required = true, HelpText = "Encoded file path" )]
        public string InputFile { get; set; }
        [Option( "out", Required = true, HelpText = "Decoded file path" )]
        public string OutputFile { get; set; }
        [Option( "key", Required = true, HelpText = "Decoding key" )]
        public string Key { get; set; }
    }

    [Verb( "hack", HelpText = "Hack encoded file key" )]
    public class HackKeyOptions
    {
        [Option( "in", Required = true, HelpText = "Encoded file path" )]
        public string InputFile { get; set; }
        [Option( "out", Required = true, HelpText = "Possible keys" )]
        public string OutputFile { get; set; }
    }
}
