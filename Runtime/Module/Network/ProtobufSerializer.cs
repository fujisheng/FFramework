using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace Framework.Module.Network
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Deserialize(IMessage packet)
        {
            throw new System.NotImplementedException();
            //MemoryStream ms = new MemoryStream();
            //BinaryFormatter bm = new BinaryFormatter();
            //bm.Serialize(ms, p);
            //Serializer.Serialize<Person>(ms, p);
            //byte[] data = ms.ToArray();//length=27  709

            //反序列化操作
            //MemoryStream ms1 = new MemoryStream(data);
            //BinaryFormatter bm1 = new BinaryFormatter();
            //Person p1 = bm.Deserialize(ms1) as Person;
            //Person p1 = Serializer.Deserialize<Person>(ms1);
            //Console.ReadKey();
        }

        public IMessage Serialize(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
