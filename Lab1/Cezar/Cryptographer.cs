using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cezar
{
    public static class Cryptographer
    {
        private const string MessageAlphabet = "\n\r\"\',.:;!-()0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЖЗИЙКЛМНОПРСТУФЧЦЧШЩЪЫЬЭЮЯ";
        private const string KeyAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static readonly Encoding Encoding = Encoding.UTF8;

        private const int MaxKeyLength = 20;

        public static byte[] Encode( string message, string key )
        {
            bool isValidMessage = ValidateMessage( message );
            if ( !isValidMessage )
                throw new Exception( $"Not valid message. Valid lang: {MessageAlphabet}" );

            bool isValidKey = ValidateKey( key );
            if ( !isValidKey )
                throw new Exception( $"Not valid key. Valid key lang: {KeyAlphabet}" );

            byte[] filteredBytes = Encoding.GetBytes( message );
            byte[] keyBytes = Encoding.GetBytes( key );

            return Encode( filteredBytes, keyBytes );
        }

        public static byte[] Decode( byte[] message, string key )
        {
            bool isValidKey = ValidateKey( key );
            if ( !isValidKey )
                throw new Exception( $"Not valid key. Max length:{MaxKeyLength}, valid key lang: {KeyAlphabet}" );

            byte[] keyBytes = Encoding.GetBytes( key );
            return Decode( message, keyBytes );
        }

        public static List<string> FindKeys( byte[] encryptedBytes )
        {
            List<byte[]> keys = FindByteKeys( encryptedBytes );

            return keys
                .ConvertAll( k => Encoding.GetString( k ) )
                .Where( k => k.All( ch => KeyAlphabet.Contains( ch ) ) )
                .ToList();
        }

        private static List<byte[]> FindByteKeys( byte[] encryptedBytes )
        {
            var result = new List<byte[]>();
            for ( int keyLength = 1; keyLength <= MaxKeyLength; ++keyLength )
            {
                List<HashSet<byte>> keySet = FindKeySet( encryptedBytes, keyLength );

                if ( keySet.Count == 0 )
                    continue;

                List<byte[]> keys = FindKeys( keySet, keyLength );

                result.AddRange( keys );
            }

            return result;
        }

        private static List<byte[]> FindKeys( List<HashSet<byte>> keySet, int keyLength )
        {
            var keys = new List<byte[]>();
            var keysStack = new Stack<ByteContainer>();
            keysStack.Push( new ByteContainer { Bytes = new byte[ keyLength ], Length = 0 } );

            while ( keysStack.Count != 0 )
            {
                ByteContainer data = keysStack.Pop();
                if ( data.Length == keyLength )
                {
                    keys.Add( data.Bytes );
                }
                else
                {
                    foreach ( byte possibleShift in keySet[ data.Length ] )
                    {
                        byte[] key = new byte[ keyLength ];
                        Array.Copy( data.Bytes, key, keyLength );
                        key[ data.Length ] = possibleShift;
                        keysStack.Push( new ByteContainer { Bytes = key, Length = data.Length + 1 } );
                    }
                }
            }

            return keys;
        }

        private static List<HashSet<byte>> FindKeySet( byte[] encryptedBytes, int keyLength )
        {
            var keysSet = new List<HashSet<byte>>();
            bool isKeyFound = true;

            for ( int keyIndex = 0; keyIndex < keyLength; ++keyIndex )
            {
                var possibleShifts = new HashSet<byte>();
                for ( int i = keyIndex; i < encryptedBytes.Length; i += keyLength )
                {
                    HashSet<byte> possibleShiftsForByte = GetPossibleShifts( encryptedBytes[ i ] );
                    if ( i == keyIndex )
                    {
                        possibleShifts.UnionWith( possibleShiftsForByte );
                    }
                    else
                    {
                        possibleShifts.IntersectWith( possibleShiftsForByte );
                    }
                }

                if ( possibleShifts.Count == 0 )
                {
                    isKeyFound = false;
                    break;
                }

                keysSet.Add( possibleShifts );
            }

            return isKeyFound
                ? keysSet
                : new List<HashSet<byte>>();
        }

        private static HashSet<byte> GetPossibleShifts( byte encryptedByte )
        {
            var set = new HashSet<byte>();
            var alphabetBytes = Encoding.GetBytes( MessageAlphabet );
            foreach ( char letter in alphabetBytes )
            {
                int key = encryptedByte - letter;
                if ( key < 0 )
                {
                    key = 256 + key;
                }
                set.Add( ( byte )key );
            }

            return set;
        }

        private static byte[] Decode( byte[] message, byte[] key )
        {
            return message.Zip( GetShifts( key ), ( byte encryptedByte, byte shift ) =>
            {
                int difference = encryptedByte - shift;
                if ( difference < 0 )
                {
                    difference = 256 + difference;
                }

                return ( byte )difference;
            } ).ToArray();
        }

        private static byte[] Encode( byte[] message, byte[] key )
        {
            return message.Zip( GetShifts( key ), ( byte originalByte, byte shift ) =>
            {
                return ( byte )( ( originalByte + shift ) % 256 );
            } ).ToArray();
        }

        private static bool ValidateMessage( string input )
        {
            foreach ( char ch in input )
            {
                if ( MessageAlphabet.IndexOf( ch ) == -1 )
                    return false;
            }

            return true;
        }

        private static bool ValidateKey( string input )
        {
            if ( input.Length > MaxKeyLength )
                return false;

            foreach ( char ch in input )
            {
                if ( KeyAlphabet.IndexOf( ch ) == -1 )
                    return false;
            }

            return true;
        }

        private static IEnumerable<byte> GetShifts( byte[] bytes )
        {
            while ( true )
            {
                foreach ( byte b in bytes )
                {
                    yield return b;
                }
            }
        }

        private struct ByteContainer
        {
            public byte[] Bytes;
            public int Length;
        }
    }
}
