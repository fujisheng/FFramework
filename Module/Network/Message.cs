using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Network
{
    public class Message : IMessage
    {
        public int Id { get; set; }
        public byte[] Bytes { get; set; }
        public int Length { get; set; }

        public void Clear()
        {
            Array.Clear(Bytes, 0, Bytes.Length);
            Length = 0;
            Id = 0;
        }
    }
}