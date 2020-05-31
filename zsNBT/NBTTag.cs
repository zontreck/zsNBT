using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zsNBT
{
    public abstract class NBTTag : ICloneable
    {

        /// <summary>
        /// Parent Compound tag. Either a NBTList or a NBTCompound. May be null via setting to "NULL"
        /// </summary>
        public NBTTag Parent { get; internal set; }

        /// <summary>
        /// Type of tag
        /// </summary>
        public abstract NBTTagType TagType { get; }

        /// <summary>
        /// Checks if the tag has a value
        /// </summary>
        public bool HasValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.COMPOUND:
                    case NBTTagType.END:
                    case NBTTagType.INVALID:
                        return false;
                    default:
                        return true;
                }
            }
        }

        internal string _Name;

        /// <summary>
        /// The tag name
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if(_Name == value)
                {
                    return;
                }

                NBTFolder comp = Parent as NBTFolder;
                if(comp != null)
                {
                    if(value == null)
                    {
                        throw new Exception("Name of tag cannot be null");
                    }else if (_Name != null)
                    {
                        comp.RenameTag(_Name, value);
                    }
                }

                _Name = value;
            }
        }

        /// <summary>
        /// The NBT Path
        /// </summary>
        public string Path
        {
            get
            {
                if(Parent == null)
                {
                    return Name;
                }
                NBTFolder parentAsList = Parent as NBTFolder;

                if(parentAsList != null)
                {
                    return parentAsList.Path + '[' + parentAsList.IndexOf(this) + ']';
                }
                else
                {
                    return Parent.Path + '/' + Name;
                }
            }
        }


        internal abstract bool ReadTag(BinaryReader reader);
        internal abstract void SkipTag(BinaryReader reader);
        internal abstract void WriteTag(BinaryWriter writer);
        internal abstract void WriteData(BinaryWriter writer);

        private string Error = "Indexer can only be used on a List, Compound, or an Array tag type";
        public virtual NBTTag this[string TagName]
        {
            get
            {
                throw new InvalidOperationException(Error);
            }
            set
            {
                throw new InvalidOperationException(Error);
            }
        }

        public virtual NBTTag this[int TagIndex]
        {
            get
            {
                throw new InvalidOperationException(Error);
            }
            set
            {
                throw new InvalidOperationException(Error);
            }
        }

        public static string GetCanonicalTagName(NBTTagType type)
        {
            switch (type)
            {
                case NBTTagType.COMPOUND:
                    return "Tag_Folder";
                case NBTTagType.STRING:
                    return "Tag_String";
                case NBTTagType.INT:
                    return "Tag_Int32";
                case NBTTagType.DOUBLE:
                    return "Tag_Double";
                case NBTTagType.STRINGARRAY:
                    return "Tag_StringArray";
                case NBTTagType.FLOAT:
                    return "Tag_Float";
                case NBTTagType.INTARRAY:
                    return "Tag_Int32Array";
                case NBTTagType.BYTE:
                    return "Tag_Byte";
                case NBTTagType.FLOATARRAY:
                    return "Tag_FloatArray";
                case NBTTagType.BYTEARRAY:
                    return "Tag_ByteArray";
                case NBTTagType.DOUBLEARRAY:
                    return "Tag_DoubleArray";
                default:
                    return null;
            }
        }

        public string StringValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.STRING:
                        return ((NBTString)this).Value;
                    default:
                        throw new Exception("Cannot get string value from unknown type: "+GetCanonicalTagName(TagType));
                }
            }
        }

        public float FloatValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.FLOAT:
                        return ((NBTFloat)this).Value;
                    default:
                        throw new Exception("Cannot get float value from type: " + GetCanonicalTagName(TagType));
                }
            }
        }

        public string[] StringArrayValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.STRINGARRAY:
                        return ((NBTStringArray)this).ToArray();
                    default:
                        throw new Exception("Cannot get array of strings from : " + GetCanonicalTagName(TagType));
                }
            }
        }

        public int[] IntArrayValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.INTARRAY:
                        return ((NBTIntArray)this).ToArray();
                    default:
                        throw new Exception("Cannot get array of ints from : " + GetCanonicalTagName(TagType));
                }
            }
        }

        public double DoubleValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.DOUBLE:
                        return ((NBTDouble)this).Value;
                    default:
                        throw new Exception("Cannot get double value from unknown type: " + GetCanonicalTagName(TagType));
                }
            }
        }

        public int IntValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.INT:
                        return ((NBTInt)this).Value;
                    default:
                        throw new Exception("Invalid type, expected: "+GetCanonicalTagName(TagType));
                }
            }
        }

        public byte ByteValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.BYTE:
                        return ((NBTByte)this).Value;
                    default:
                        throw new Exception("Unrecognized type for byte: " + GetCanonicalTagName(TagType));
                }
            }
        }

        public byte[] ByteArrayValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.BYTEARRAY:
                        return ((NBTByteArray)this).ToArray();
                    default:
                        throw new Exception("Unrecognized type for byte array: " + GetCanonicalTagName(TagType));
                }
            }
        }

        public float[] FloatArrayValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.FLOATARRAY:
                        return ((NBTFloatArray)this).ToArray();
                    default:
                        throw new Exception("Unrecognized type for float array: " + GetCanonicalTagName(TagType));
                }
            }
        }
        public double[] DoubleArrayValue
        {
            get
            {
                switch (TagType)
                {
                    case NBTTagType.DOUBLEARRAY:
                        return ((NBTDoubleArray)this).ToArray();
                    default:
                        throw new Exception("Unrecognized type for double array: " + GetCanonicalTagName(TagType));
                }
            }
        }
        public override string ToString()
        {
            return ToString("   ");
        }

        public abstract object Clone();

        public string ToString(string indentString)
        {
            if (indentString == null) throw new ArgumentNullException("Indent string cannot be null");
            var sb = new StringBuilder();
            PrettyPrint(sb, indentString, 0);
            return sb.ToString();
        }

        internal abstract void PrettyPrint(StringBuilder builder, string indenter, int indentLevel);
    }

    
    public enum NBTTagType
    {
        STRING=1,
        INT=5,
        FLOAT=8,
        BYTE=3,
        /*LONG,
        SHORT,*/
        DOUBLE=6,
        STRINGARRAY=7,
        INTARRAY=9,
        FLOATARRAY=10,
        BYTEARRAY=11,
        /*LONGARRAY,
        SHORTARRAY,*/
        DOUBLEARRAY=12,


        COMPOUND=2,

        END=4, // Used to show the end of a list or a compound


        INVALID=0
    }
}
