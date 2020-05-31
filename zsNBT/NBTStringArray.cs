using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTStringArray : NBTTag, ICollection<string>, ICollection
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.STRINGARRAY;
            }
        }

        List<string> arr = new List<string>();
        private List<string> ArrayItems
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

        public string[] ToArray()
        {
            return ArrayItems.ToArray();
        }
        public void Add(string item)
        {
            if (ArrayItems.Contains(item)) return;
            ArrayItems.Add(item);
        }
        public NBTStringArray() { }
        public NBTStringArray(string TagName, string[] initialItems)
        {
            Name = TagName;
            foreach(string item in initialItems)
            {
                Add(item);
            }
        }

        public NBTStringArray(NBTStringArray other) : this(other.Name, other.ToArray())
        {

        }

        public NBTStringArray(string TagName, List<string> items) : this(TagName, items.ToArray()) { }


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
            return new NBTStringArray(this);
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

            foreach(string item in arr)
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
                    Add(reader.ReadString());
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
                reader.ReadString();
            }

            return;
        }

        internal override void WriteData(BinaryWriter writer)
        {
            foreach (string item in arr)
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
            CopyTo((string[])array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }

        public void Clear()
        {
            ArrayItems.Clear();
        }

        public bool Contains(string item)
        {
            if (ArrayItems.Contains(item)) return true;
            else return false;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            arr.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove a item
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True on success or if the array did not contain this item</returns>
        public bool Remove(string item)
        {
            if (!Contains(item)) return true;
            return ArrayItems.Remove(item);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return arr.GetEnumerator();
        }
    }
}
