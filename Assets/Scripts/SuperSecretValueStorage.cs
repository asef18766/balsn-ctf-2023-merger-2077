using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Random = System.Random;


internal static class RandomExtensions
{
    public static void Shuffle(this Random rng, Tuple<string, byte[]>[] array)
    {
        var n = array.Length;
        while (n > 1) 
        {
            var k = rng.Next(n--);
            if (array[n].Item1 != "")
                Debug.Log($"found {array[n].Item1}");
            if (array[k].Item1 != "")
                Debug.Log($"found {array[k].Item1}");

            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}
public class SuperSecretValueStorage : MonoBehaviour
{
    private const int TotalSz = 0x10000;
    /*
     * first 0x1000 bytes for header
     */
    private byte[] superSecretBytes = new byte[TotalSz];
    private ICryptoTransform _enc;
    private ICryptoTransform _dec;
    private Dictionary<string, Tuple<int, int>> DeserializeMapping()
    {
        var m = new MemoryStream();
        var b = new BinaryFormatter();
        var pt = _dec.TransformFinalBlock(superSecretBytes, 0, TotalSz + 16);
        var headerSz = BitConverter.ToInt32(pt[..4]);
        Debug.Log($"serialize header sz: {headerSz}");
        m.Write(pt, 4, headerSz);
        m.Position = 0;
        Array.Clear(pt, 0, TotalSz);
        return (Dictionary<string, Tuple<int, int>>)b.Deserialize(m);
    }

    private byte[] SerializeMapping(Dictionary<string, Tuple<int, int>> mapping)
    {
        var m = new MemoryStream();
        var b = new BinaryFormatter();
        b.Serialize(m, mapping);
        var arr = m.ToArray();
        Debug.Log($"serialize mapping sz: {arr.Length}");
        var list = new List<byte>();
        list.AddRange(BitConverter.GetBytes(arr.Length));
        list.AddRange(arr);
        return list.ToArray();
    }
    
    public object ReadSecretValue(string val)
    {
        var mapping = DeserializeMapping();
        var m = new MemoryStream();
        var b = new BinaryFormatter();
        Debug.Log($"get item {val} from offset {mapping[val].Item1} sz {mapping[val].Item2}");
        var pt = _dec.TransformFinalBlock(superSecretBytes, 0, TotalSz + 16);
        m.Write(pt, mapping[val].Item1, mapping[val].Item2);
        m.Position = 0;
        Array.Clear(pt, 0, TotalSz);
        return b.Deserialize(m);
    }
    // shuffle on write
    public void WriteSecretValue(string value, object o)
    {
        var mapping = DeserializeMapping();
        var rnd = new Random();
        var perm = new List<Tuple<string, byte[]>>();
        var remainSz = TotalSz - 0x1000;
        superSecretBytes = _dec.TransformFinalBlock(superSecretBytes, 0, TotalSz + 16);
        
        var m = new MemoryStream();
        var bf = new BinaryFormatter();
        bf.Serialize(m, o);
        var oArray = m.ToArray();
        if (!mapping.ContainsKey(value))
        {
            Debug.Log($"add perm :{value}");
            perm.Add(new Tuple<string, byte[]>(value, oArray));
            remainSz -= oArray.Length;
            
            foreach (var kv in mapping)
            {
                perm.Add(new Tuple<string, byte[]>(kv.Key,
                    superSecretBytes[kv.Value.Item1..(kv.Value.Item1 + kv.Value.Item2)]));
                remainSz -= kv.Value.Item2;
            }
        }
        else
        {
            foreach (var kv in mapping)
            {
                if (kv.Key == value)
                {
                    perm.Add(new Tuple<string, byte[]>(value, oArray));
                    remainSz -= oArray.Length;
                }
                else
                {
                    perm.Add(new Tuple<string, byte[]>(kv.Key,
                    superSecretBytes[kv.Value.Item1..(kv.Value.Item1 + kv.Value.Item2)]));
                    remainSz -= kv.Value.Item2;
                }
            }
        }

        for (var i = 0; i != remainSz; ++i)
        {
            var b = new byte[1];
            rnd.NextBytes(b);
            perm.Add(new Tuple<string, byte[]>("", b));
        }

        
        var dataArr = perm.ToArray();

        rnd.Shuffle(dataArr);

        var idx = 0x1000;
        for (var i = 0; i != dataArr.Length; ++i)
        {
            Array.Copy(dataArr[i].Item2, 0, superSecretBytes, idx, dataArr[i].Item2.Length);
            if (dataArr[i].Item1 != "")
            {
                if (!mapping.ContainsKey(dataArr[i].Item1))
                {
                    mapping.Add(dataArr[i].Item1, null);
                    Debug.Log($"add new mapping of {dataArr[i].Item1} offset {idx} len: {dataArr[i].Item2.Length}");
                }

                mapping[dataArr[i].Item1] = new Tuple<int, int>(idx, dataArr[i].Item2.Length);
            }
            idx += dataArr[i].Item2.Length;
        }
        Debug.Log($"final idx {idx}");

        var header = SerializeMapping(mapping);
        Array.Copy(header, superSecretBytes, header.Length);
        superSecretBytes = _enc.TransformFinalBlock(superSecretBytes, 0, TotalSz);
    }

    private void Start()
    {
        var aes = new AesCryptoServiceProvider();
        aes.Key = new byte[]
        {
            229, 133, 171, 229, 152, 142, 110, 111, 110, 111, 229, 133, 171, 232, 165, 191
        };
        aes.Mode = CipherMode.ECB;
        _enc = aes.CreateEncryptor();
        _dec = aes.CreateDecryptor();
        /*
        var rnd = new Random();
        var header = SerializeMapping(new Dictionary<string, Tuple<int, int>>());
        rnd.NextBytes(superSecretBytes);
        Debug.Log($"header len: {header.Length}");
        Array.Copy(header, superSecretBytes, header.Length);
        WriteSecretValue("score", 0);
        WriteSecretValue("flag", "BALSN{1 4M $O l@2Y 7O IMPl3men7 RWd .... 222}");
        
        var aes = new AesCryptoServiceProvider();
        aes.Key = new byte[]
        {
            229, 133, 171, 229, 152, 142, 110, 111, 110, 111, 229, 133, 171, 232, 165, 191
        };
        aes.Mode = CipherMode.ECB;
        var enc = aes.CreateEncryptor();
        using (var binWriter =
               new BinaryWriter(File.Open("C:/Users/asef18766/Desktop/stat.dmp", FileMode.Create)))
        {
            binWriter.Write(enc.TransformFinalBlock(superSecretBytes, 0, TotalSz));
        }
        */
        var asset = Resources.Load("奧利給") as TextAsset;
        if (asset != null) superSecretBytes = asset.bytes;
    }
}