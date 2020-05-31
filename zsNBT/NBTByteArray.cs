using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTByteArray : NBTTag, ICollection<byte>, ICollection
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.BYTEARRAY;
            }
        }

        List<byte> arr = new List<byte>();
        private List<byte> ArrayItems
        {
            get
            {
                return arr;
            }
            set
            {
                if (value == null) throw new Exception("Value cannot be null");
                arr = value;
            }
        }

        public byte[] ToArray()
        {
            return arr.ToArray();
        }
        public void Add(byte item)
        {
            if (ArrayItems.Contains(item)) return;
            ArrayItems.Add(item);
        }
        public NBTByteArray() { }
        public NBTByteArray(string TagName, byte[] initialItems)
        {
            Name = TagName;
            foreach(byte item in initialItems)
            {
                Add(item);
            }
        }

        public NBTByteArray(string TagName, List<byte> items) : this(TagName, items.ToArray())
        {
            
        }

        public NBTByteArray(NBTByteArray other) : this(other.Name, other.ToArray())
        {

        }
        public int Count
        {
            get
            {
                return ArrayItems.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return (ArrayItems as ICollection).SyncRoot;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override object Clone()
        {
            return new NBTByteArray(this);
        }
        void PrintIndent(StringBuilder builder, string indent, int level)
        {
            for(int i = 0; i < level; i++)
            {
                builder.Append(indent);
            }
        }
        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            PrintIndent(builder, indenter, indentLevel);

            builder.Append($"{GetCanonicalTagName(TagType)}({Name}): {Count} entries " + "{\n");

            foreach(byte item in arr)
            {
                PrintIndent(builder, indenter, indentLevel + 1);
                builder.Append($"[{arr.IndexOf(item)}]: {item}\n");
            }
            PrintIndent(builder, indenter, indentLevel);
            builder.Append("}");

        }

        internal override bool ReadTag(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length == 0) return true;
            else
            {
                for(int i = 0; i < length; i++)
                {
                    Add(reader.ReadByte());
                }

                return true;
            }
        }

        internal override void SkipTag(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length == 0) return;

            for(int i = 0; i < length; i++)
            {
                reader.ReadSingle();
            }

            return;
        }

        internal override void WriteData(BinaryWriter writer)
        {
            foreach (byte item in arr)
            {
                writer.Write(item);
            }
        }

        internal override void WriteTag(BinaryWriter writer)
        {
            writer.Write((int)TagType);
            writer.Write(Name);
            writer.Write(Count);
            WriteData(writer);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((byte[])array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }

        public void Clear()
        {
            ArrayItems.Clear();
        }

        public bool Contains(byte item)
        {
            if (ArrayItems.Contains(item)) return true;
            else return false;
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            arr.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove a item
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True on success or if the array did not contain this item</returns>
        public bool Remove(byte item)
        {
            if (!Contains(item)) return true;
            return ArrayItems.Remove(item);
        }

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return arr.GetEnumerator();
        }
    }
}
