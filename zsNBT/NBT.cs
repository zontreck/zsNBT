using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace zsNBT
{
    public static class NBTExtensions
    {
        private static void ProcessFields(FieldInfo[] fields, object src, NBTTag parent)
        {

            NBTTag currentParent = parent;
            foreach (FieldInfo fi in fields)
            {
                //Console.WriteLine($"Name: {fi.Name}\nType: {fi.FieldType.Name}\nValue: {fi.GetValue(src)}\n\n");
                NBTTag theTag = null;
                switch (fi.FieldType.Name)
                {
                    case "String":
                        theTag = new NBTString(fi.Name, (string)fi.GetValue(src));

                        break;
                    case "Int32":
                        theTag = new NBTInt(fi.Name, (int)fi.GetValue(src));
                        break;
                    case "Double":
                        theTag = new NBTDouble(fi.Name, (double)fi.GetValue(src));
                        break;
                    case "String[]":
                        theTag = new NBTStringArray(fi.Name, (string[])fi.GetValue(src));
                        break;
                    case "Int32[]":
                        theTag = new NBTIntArray(fi.Name, (int[])fi.GetValue(src));
                        break;
                    case "Single":
                        theTag = new NBTFloat(fi.Name, (float)fi.GetValue(src));
                        break;
                    case "Byte":
                        theTag = new NBTByte(fi.Name, (byte)fi.GetValue(src));
                        break;
                    case "List`1":
                        if (fi.GetValue(src).ToString().Contains("System.String"))
                        {
                            theTag = new NBTStringArray(fi.Name, (List<string>)fi.GetValue(src));
                        }else if (fi.GetValue(src).ToString().Contains("System.Int32"))
                        {
                            theTag = new NBTIntArray(fi.Name, (List<int>)fi.GetValue(src));
                        } else if (fi.GetValue(src).ToString().Contains("System.Byte"))
                        {
                            theTag = new NBTByteArray(fi.Name, (List<byte>)fi.GetValue(src));
                        }else if (fi.GetValue(src).ToString().Contains("System.Single"))
                        {
                            theTag = new NBTFloatArray(fi.Name, (List<float>)fi.GetValue(src));
                        } else if (fi.GetValue(src).ToString().Contains("System.Double"))
                        {
                            theTag = new NBTDoubleArray(fi.Name, (List<double>)fi.GetValue(src));
                        } else
                        {
                            // Treat as a compound tag
                            NBTFolder tags = new NBTFolder(fi.Name);
                            Type _typ = Type.GetType(fi.GetValue(src).ToString());
                            tags.Add(new NBTString("_TYPE", _typ.FullName));
                            
                        }


                        if (theTag == null) continue;
                        else
                        {
                            break;
                        }
                    case "Byte[]":
                        theTag = new NBTByteArray(fi.Name, (byte[])fi.GetValue(src));
                        break;
                    case "Single[]":
                        theTag = new NBTFloatArray(fi.Name, (float[])fi.GetValue(src));
                        break;
                    case "Double[]":
                        theTag = new NBTDoubleArray(fi.Name, (double[])fi.GetValue(src));
                        break;
                    default:
                        // Try to run as a folder
                        var valx = fi.GetValue(src);
                        NBTFolder thefolder = new NBTFolder(fi.Name);
                        ProcessFields(fi.FieldType.GetFields(), valx, thefolder);
                        theTag = thefolder;
                        break;
                }
                switch (currentParent.TagType)
                {
                    case NBTTagType.COMPOUND:
                        NBTFolder parFold = currentParent as NBTFolder;
                        parFold.Add(theTag);
                        break;
                    default:
                        continue;
                }
            }
        }
        public static NBTFolder ToNBT<T>(this T src, string TagName = "_ROOT_")
        {
            NBTFolder root = new NBTFolder(TagName);
            NBTString _type = new NBTString("_TYPE", typeof(T).FullName); // We only need to store the root type so we have a way to compare
            root.Add(_type);
            Type X = typeof(T);
            FieldInfo[] fields = X.GetFields();


            ProcessFields(fields, src, root);



            return root;
        }

        private static void ProcessFieldData(FieldInfo[] fields, NBTFolder src, object dest)
        {
            NBTTag currentParent = src;
            foreach(FieldInfo fi in fields)
            {
                //Console.WriteLine($"Name: {fi.Name}\nType: {fi.FieldType.Name}\nValue: {fi.GetValue(dest)}\n\n");
                try
                {

                    switch (fi.FieldType.Name)
                    {
                        case "String":

                            fi.SetValue(dest, currentParent[fi.Name].StringValue);
                            break;
                        case "Int32":
                            fi.SetValue(dest, currentParent[fi.Name].IntValue);
                            break;
                        case "Double":
                            fi.SetValue(dest, currentParent[fi.Name].DoubleValue);
                            break;
                        case "String[]":
                            fi.SetValue(dest, currentParent[fi.Name].StringArrayValue);
                            break;
                        case "Int32[]":
                            fi.SetValue(dest, currentParent[fi.Name].IntArrayValue);
                            break;
                        case "Single":
                            fi.SetValue(dest, currentParent[fi.Name].FloatValue);
                            break;
                        case "Byte":
                            fi.SetValue(dest, currentParent[fi.Name].ByteValue);
                            break;
                        case "List`1":
                            string TypeStr = fi.GetValue(dest).ToString();
                            if (TypeStr.Contains("System.String"))
                            {
                                fi.SetValue(dest, currentParent[fi.Name].StringArrayValue.ToList());
                            }
                            else if (TypeStr.Contains("System.Int32"))
                            {
                                fi.SetValue(dest, currentParent[fi.Name].IntArrayValue.ToList());
                            }
                            else if (TypeStr.Contains("System.Byte"))
                            {
                                fi.SetValue(dest, currentParent[fi.Name].ByteArrayValue.ToList());
                            }
                            else if (TypeStr.Contains("System.Single"))
                            {
                                fi.SetValue(dest, currentParent[fi.Name].FloatArrayValue.ToList());
                            }
                            else if (TypeStr.Contains("System.Double"))
                            {
                                fi.SetValue(dest, currentParent[fi.Name].DoubleArrayValue.ToList());
                            }
                            break;
                        case "Byte[]":
                            fi.SetValue(dest, currentParent[fi.Name].ByteArrayValue);
                            break;
                        case "Single[]":
                            fi.SetValue(dest, currentParent[fi.Name].FloatArrayValue);
                            break;
                        case "Double[]":
                            fi.SetValue(dest, currentParent[fi.Name].DoubleArrayValue);
                            break;
                        default:
                            var VALX = fi.GetValue(dest);
                            NBTFolder theFolder = currentParent[fi.Name] as NBTFolder;
                            ProcessFieldData(fi.FieldType.GetFields(), theFolder, VALX);
                            break;
                    }
                } catch(Exception e) { } // Silently catch.. If it does not 100% conform, do not crash the program.

            }
        }
        
        /// <summary>
        /// Convert a NBT Structure to a C# Class Object
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="src">The NBT containing the data</param>
        /// <returns>Converted NBT as a class</returns>
        public static T FromNBT<T>(this NBTFolder src)
        {
            T tx = default(T);
            Type X = typeof(T);
            if (X.FullName != src["_TYPE"].StringValue) throw new InvalidDataException("Invalid deserialize request");
            tx = (T)Activator.CreateInstance(X);
            FieldInfo[] fields = X.GetFields();

            ProcessFieldData(fields, src, tx);

            return tx;
        }


        public static void WriteFile(this NBTFolder folder, string FileName)
        {
            FileStream FS = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryWriter bw = new BinaryWriter(FS);
            folder.WriteTag(bw);
            bw.Write((int)NBTTagType.END);
            bw.Flush();
            bw.Close();
        }

        public static void ReadFile(this NBTFolder folder, string FileName)
        {
            FileStream FS = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(FS);
            folder.ReadTag(br);
        }

        public static MemoryStream WriteAsMemoryStream(this NBTFolder folder)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            folder.WriteTag(bw);
            return ms;
        }

        public static void ReadNBT(this MemoryStream ms, NBTFolder folder)
        {
            BinaryReader br = new BinaryReader(ms);
            folder.ReadTag(br);
        }
    }
}
