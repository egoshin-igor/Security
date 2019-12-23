
using System;
using System.IO;
using System.Linq;
using CommandLine;

namespace Cezar
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                var i = Parser.Default.ParseArguments<EncodeOptions, DecodeOptions, HackKeyOptions>( args )
                .MapResult(
                    ( EncodeOptions options ) => RunEncode( options ),
                    ( DecodeOptions options ) => RunDecode( options ),
                    ( HackKeyOptions options ) => RunHack( options ),
                    errors => throw new Exception( "Parsing Error!" ) );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }

        private static int RunEncode( EncodeOptions options )
        {
            Console.WriteLine( "Encoding..." );
            string input = File.ReadAllText( options.InputFile, Cryptographer.Encoding );
            byte[] encoded = Cryptographer.Encode( input, options.Key );
            File.WriteAllBytes( options.OutputFile, encoded );
            Console.WriteLine( "Encoding: SUCCESS" );

            return 0;
        }

        private static int RunDecode( DecodeOptions options )
        {
            Console.WriteLine( "Decoding..." );
            byte[] input = File.ReadAllBytes( options.InputFile );
            byte[] decoded = Cryptographer.Decode( input, options.Key );
            File.WriteAllBytes( options.OutputFile, decoded );
            Console.WriteLine( "Decoding: SUCCESS" );

            return 0;
        }

        private static int RunHack( HackKeyOptions options )
        {
            Console.WriteLine( "Searching keys began" );
            byte[] input = File.ReadAllBytes( options.InputFile );
            var keys = Cryptographer.FindKeys( input );
            if ( keys.Any() )
            {
                Console.WriteLine( "Keys found" );
                using ( var fileStream = new FileStream( options.OutputFile, FileMode.Create ) )
                using ( var streamWriter = new StreamWriter( fileStream, Cryptographer.Encoding ) )
                {
                    foreach ( string key in keys )
                    {
                        streamWriter.WriteLine( key );
                    }
                }
            }
            else
            {
                Console.WriteLine( "Keys not found" );
            }

            return 0;
        }
    }
}
