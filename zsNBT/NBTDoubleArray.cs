using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTDoubleArray : NBTTag, ICollection<double>, ICollection
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.DOUBLEARRAY;
            }
        }

        List<double> arr = new List<double>();
        private List<double> ArrayItems
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

        public double[] ToArray()
        {
            return arr.ToArray();
        }
        public void Add(double item)
        {
            if (ArrayItems.Contains(item)) return;
            ArrayItems.Add(item);
        }
        public NBTDoubleArray() { }
        public NBTDoubleArray(string TagName, double[] initialItems)
        {
            Name = TagName;
            foreach(double item in initialItems)
            {
                Add(item);
            }
        }

        public NBTDoubleArray(string TagName, List<double> items) : this(TagName, items.ToArray())
        {
            
        }

        public NBTDoubleArray(NBTDoubleArray other) : this(other.Name, other.ToArray())
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
            return new NBTDoubleArray(this);
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

            foreach(double item in arr)
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
                    Add(reader.ReadDouble());
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
                reader.ReadDouble();
            }

            return;
        }

        internal override void WriteData(BinaryWriter writer)
        {
            foreach (double item in arr)
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
            CopyTo((double[])array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }

        public void Clear()
        {
            ArrayItems.Clear();
        }

        public bool Contains(double item)
        {
            if (ArrayItems.Contains(item)) return true;
            else return false;
        }

        public void CopyTo(double[] array, int arrayIndex)
        {
            arr.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove a item
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True on success or if the array did not contain this item</returns>
        public bool Remove(double item)
        {
            if (!Contains(item)) return true;
            return ArrayItems.Remove(item);
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            return arr.GetEnumerator();
        }
    }
}
