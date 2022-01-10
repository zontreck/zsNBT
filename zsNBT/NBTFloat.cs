using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTFloat : NBTTag
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.FLOAT;
            }
        }

        float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("value");
                }
                _value = value;
            }
        }
        public NBTFloat() { }
        public NBTFloat(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public NBTFloat(NBTFloat other)
        {
            if (other == null) throw new ArgumentNullException("other", "cannot be null");
            Name = other.Name;
            Value = other.Value;
        }

        public override object Clone()
        {
            return new NBTFloat(this);
        }

        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                builder.Append(indenter);
            }
            builder.Append(GetCanonicalTagName(TagType));
            builder.Append($"({Name}): \"{Value}\"");
        }

        internal override bool ReadTag(BinaryReader reader)
        {
            Value = reader.ReadSingle();
            return true;
        }

        internal override void SkipTag(BinaryReader reader)
        {
            reader.ReadSingle();
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
