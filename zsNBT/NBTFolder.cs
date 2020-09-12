using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace zsNBT
{
    public sealed class NBTFolder : NBTTag, ICollection<NBTTag>, ICollection
    {
        public override NBTTagType TagType
        {
            get
            {
                return NBTTagType.COMPOUND;
            }
        }

        readonly Dictionary<string, NBTTag> tags = new Dictionary<string, NBTTag>();
        public NBTFolder() { }
        public NBTFolder(string TagName)
        {
            Name = TagName;
        }

        public NBTFolder(IEnumerable<NBTTag> tags) : this(null, tags) { }

        public NBTFolder(string tagName, IEnumerable<NBTTag> tags) {
            if (tags == null) throw new ArgumentNullException("Tags", "Tags cannot be null");
            if (tagName == null) tagName = "NULL";
            Name = tagName;
            foreach(NBTTag tag in tags)
            {
                Add(tag);
            }
        }


        public int Count
        {
            get
            {
                return tags.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
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
                return (tags as ICollection).SyncRoot;
            }
        }

        public void Add(NBTTag item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("newTag");
            }
            else if (item == this) throw new ArgumentException("Cannot add self to self");
            else if (item.Name == null) throw new Exception("Only named tags are allowed");
            else if (item.Parent != null) throw new Exception("A tag may only be added to one compound/list at a time");

            tags.Add(item.Name, item);
            item.Parent = this;
        }

        public void Clear()
        {
            foreach(NBTTag tag in tags.Values)
            {
                tag.Parent = null;
            }
            tags.Clear();
        }

        public bool Contains(NBTTag item)
        {
            if (item == null) throw new Exception("Tag must not be null");
            return tags.ContainsValue(item);
        }

        public bool Contains(string itemName)
        {
            return tags.ContainsKey(itemName);
        }

        public void CopyTo(NBTTag[] array, int arrayIndex)
        {
            tags.Values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((NBTTag[])array, index);
        }


        public bool Remove(NBTTag item)
        {
            // Delete the tag
            string tagName = "";
            
            foreach(KeyValuePair<string,NBTTag> tag in tags)
            {
                if (tag.Value == item)
                {
                    tagName = tag.Key;
                    break;
                }
            }

            return tags.Remove(tagName);

        }

        internal void RenameTag(string oldName, string newName)
        {
            NBTTag tag;
            if(tags.TryGetValue(newName, out tag))
            {
                throw new ArgumentException("Cannot rename: a tag with that name already exists");
            }
            if(!tags.TryGetValue(oldName, out tag))
            {
                throw new ArgumentException("Cannot rename: That tag does not exist");
            }

            tags.Remove(oldName);
            tags.Add(newName, tag);
        }

        internal override bool ReadTag(BinaryReader reader)
        {
            if (Parent != null)
            {
                SkipTag(reader);
                return false;
            }

            while (true)
            {
                NBTTagType nextTag = (NBTTagType)reader.ReadInt32();
                NBTTag newTag=null;
                switch (nextTag)
                {
                    case NBTTagType.COMPOUND:
                        newTag = new NBTFolder();
                        break;
                    case NBTTagType.STRING:
                        newTag = new NBTString();
                        break;
                    case NBTTagType.END:
                        return true;
                    case NBTTagType.INT:
                        newTag = new NBTInt();
                        break;
                    case NBTTagType.DOUBLE:
                        newTag = new NBTDouble();
                        break;
                    case NBTTagType.STRINGARRAY:
                        newTag = new NBTStringArray();
                        break;
                    case NBTTagType.FLOAT:
                        newTag = new NBTFloat();
                        break;
                    case NBTTagType.INTARRAY:
                        newTag = new NBTIntArray();
                        break;
                    case NBTTagType.BYTE:
                        newTag = new NBTByte();
                        break;
                    case NBTTagType.BYTEARRAY:
                        newTag = new NBTByteArray();
                        break;
                    case NBTTagType.FLOATARRAY:
                        newTag = new NBTFloatArray();
                        break;
                    case NBTTagType.DOUBLEARRAY:
                        newTag = new NBTDoubleArray();
                        break;
                }

                newTag.Name = reader.ReadString();
                if (newTag.ReadTag(reader))
                {
                    tags.Add(newTag.Name, newTag);
                }
                newTag.Parent = this;
            }
        }

        internal override void SkipTag(BinaryReader reader)
        {
            while (true)
            {
                NBTTagType nextTag = (NBTTagType)reader.ReadInt32();
                NBTTag newTag=null;
                switch (nextTag)
                {
                    case NBTTagType.END:
                        return;
                    case NBTTagType.COMPOUND:
                        newTag = new NBTFolder();
                        break;
                    case NBTTagType.STRING:
                        newTag = new NBTString();
                        break;
                    case NBTTagType.INT:
                        newTag = new NBTInt();
                        break;
                    case NBTTagType.DOUBLE:
                        newTag = new NBTDouble();
                        break;
                    case NBTTagType.STRINGARRAY:
                        newTag = new NBTStringArray();
                        break;
                    case NBTTagType.FLOAT:
                        newTag = new NBTFloat();
                        break;
                    case NBTTagType.INTARRAY:
                        newTag = new NBTIntArray();
                        break;
                    case NBTTagType.BYTE:
                        newTag = new NBTByte();
                        break;
                    case NBTTagType.BYTEARRAY:
                        newTag = new NBTByteArray();
                        break;
                    case NBTTagType.FLOATARRAY:
                        newTag = new NBTFloatArray();
                        break;
                    case NBTTagType.DOUBLEARRAY:
                        newTag = new NBTDoubleArray();
                        break;
                }
                reader.ReadString();
                newTag.SkipTag(reader);
            }
        }
        public int IndexOf(NBTTag item)
        {
            int index = 0;
            foreach(NBTTag tag in tags.Values)
            {
                if (tag == item) return index;
                index++;
            }
            return -1;
        }
        internal override void WriteData(BinaryWriter writer)
        {
            foreach(NBTTag tag in tags.Values)
            {
                tag.WriteTag(writer);
            }
            writer.Write((int)NBTTagType.END);
        }

        internal override void WriteTag(BinaryWriter writer)
        {
            writer.Write((int)NBTTagType.COMPOUND);
            if (Name == null) throw new Exception("Name is null");
            writer.Write(Name);
            WriteData(writer);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tags.Values.GetEnumerator();
        }

        public override object Clone()
        {
            return new NBTFolder(this);
        }

        internal override void PrettyPrint(StringBuilder builder, string indenter, int indentLevel)
        {
            for(int i = 0; i < indentLevel; i++)
            {
                builder.Append(indenter);
            }
            builder.Append(GetCanonicalTagName(TagType));

            builder.Append($"({Name}): {tags.Count} entries "+"{");
            if (Count > 0)
            {
                builder.Append("\n");
                foreach(NBTTag tag in tags.Values)
                {
                    tag.PrettyPrint(builder, indenter, indentLevel + 1);
                    builder.Append("\n");
                }

                for(int i = 0; i < indentLevel; i++)
                {
                    builder.Append(indenter);
                }
            }

            builder.Append("}");
            
        }

        public IEnumerator<NBTTag> GetEnumerator()
        {
            return GetEnumerator();
        }

        public override NBTTag this[string tagName]
        {
            get
            {
                return tags[tagName];
            }
            set
            {
                if (tags.ContainsKey(tagName)) tags[tagName] = value;
                else
                {
                    tags.Add(tagName, value);
                }
            }
        }

        public override NBTTag this[int TagIndex] {
            get
            {
                return tags.ElementAt(TagIndex).Value;
            }
            set
            {
                try
                {

                    string TagName = tags.ElementAt(TagIndex).Key;
                    tags[TagName] = value;
                }catch(Exception e)
                {
                    tags.Add(value.Name, value);
                }
            }
        }
    }
}
