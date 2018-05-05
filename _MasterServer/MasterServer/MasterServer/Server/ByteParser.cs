﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using SNetwork;

namespace SNetwork
{
    public class ByteParser
    {
        public static string ConvertToASCII(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        public static byte[] ConvertASCIIToBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }

        public static MasterNetworkPlayer[] ConvertToNetworkPlayers(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<MasterNetworkPlayer[]>(jsonString);
        }

        public static byte[] ConvertNetworkPlayersToBytes(MasterNetworkPlayer[] list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static MasterNetworkPlayer ConvertToNetworkPlayer(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<MasterNetworkPlayer>(jsonString);
        }

        public static byte[] ConvertNetworkPlayerToBytes(MasterNetworkPlayer list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static byte[] ConvertKeyValuePairsToData(KeyValuePairs[] list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static byte[] ConvertKeyValuePairToData(KeyValuePairs data)
        {
            string jsonString = JsonConvert.SerializeObject(data);
            return ConvertASCIIToBytes(jsonString);
        }

        public static KeyValuePairs[] ConvertDataToKeyValuePairs(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<KeyValuePairs[]>(jsonString);
        }

        public static KeyValuePairs ConvertDataToKeyValuePair(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<KeyValuePairs>(jsonString);
        }

        public static object[] ConvertDataToObjects(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<object[]>(jsonString);
        }

        public static object ConvertDataToObject(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<object>(jsonString);
        }

        public static byte[] ConvertObjectToBytes(object Object)
        {
            string jsonString = JsonConvert.SerializeObject(Object);
            return ConvertASCIIToBytes(jsonString);
        }
    }
}
