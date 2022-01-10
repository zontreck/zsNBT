using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zsNBT
{
    public sealed class NBTInt : NBTTag
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.INT;
            }
        }

        public NBTInt() { }
        int _val = 0;
        public int Value
        {
            get
            {
                return _val;
            }
            set
            {
                if (value == null) throw new ArgumentException("Value cannot be null");
                _val = value;
            }
        }
        public NBTInt(string TagName, int val)
        {
            Name = TagName;
            Value = val;
        }

        public NBTInt(NBTInt other)
        {
            if (other == null) throw new ArgumentNullException("other", "Other cannot be null");
            Name = other.Name;
            Value = other.Value;
        }

        public override object Clone()
        {
            return new NBTInt(this);
        }

        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            for(int i = 0; i < indentLevel; i++)
            {
                builder.Append(indenter);
            }
            builder.Append(GetCanonicalTagName(TagType));
            builder.Append($"({Name}): {Value}");
        }

        internal override bool ReadTag(BinaryReader reader)
        {
            Value = reader.ReadInt32();

            return true;
        }

        internal override void SkipTag(BinaryReader reader)
        {
            reader.ReadInt32();
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
