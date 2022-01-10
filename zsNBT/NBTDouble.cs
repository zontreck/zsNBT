using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTDouble : NBTTag
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.DOUBLE;
            }
        }
        double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null) throw new Exception("Cannot set to null");
                _value = value;
            }
        }
        public NBTDouble() { }
        public NBTDouble(string TagName, double Val)
        {
            Name = TagName;
            Value = Val;
        }

        public NBTDouble(NBTDouble other)
        {
            if (other == null) throw new Exception("Cannot be null");
            Name = other.Name;
            Value = other.Value;
        }

        public override object Clone()
        {
            return new NBTDouble(this);
        }

        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            for(int i = 0; i < indentLevel; i++)
            {
                builder.Append(indenter);
            }
            builder.Append($"{GetCanonicalTagName(TagType)}({Name}): {Value}");
        }

        internal override bool ReadTag(BinaryReader reader)
        {
            Value = reader.ReadDouble();
            return true;
        }

        internal override void SkipTag(BinaryReader reader)
        {
            reader.ReadDouble();
        }

        internal override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        internal override void WriteTag(BinaryWriter writer)
        {
            writer.Write((int)TagType);
            writer.Write(Name);
            WriteData(writer);
        }
    }
}
