using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTString : NBTTag
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.STRING;
            }
        }

        string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentException("value");
                }
                _value = value;
            }
        }
        public NBTString() { }
        public NBTString(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public NBTString(NBTString other)
        {
            if (other == null) throw new ArgumentNullException("other", "cannot be null");
            Name = other.Name;
            Value = other.Value;
        }

        public override object Clone()
        {
            return new NBTString(this);
        }

        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            for(int i = 0; i < indentLevel; i++)
            {
                builder.Append(indenter);
            }
            builder.Append(GetCanonicalTagName(TagType));
            builder.Append($"({Name}): \"{Value}\"");
        }

        internal override bool ReadTag(BinaryReader reader)
        {
            Value = reader.ReadString();
            return true;
        }

        internal override void SkipTag(BinaryReader reader)
        {
            reader.ReadString();
        }

        internal override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        internal override void WriteTag(BinaryWriter writer)
        {
            writer.Write((int)TagType);
            if (Name == null) throw new Exception("Name cannot be null");
            writer.Write(Name);
            WriteData(writer);
        }
    }
}
