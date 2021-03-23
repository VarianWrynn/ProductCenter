using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PermaisuriCMS.Common
{
    public class XmlObjectHelper
    {
        /// <summary>
        /// XML序列化对象
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ObjectToXml(Object instance)
        {
            MemoryStream stream = null;
            TextWriter writer = null;
            var objectXml = string.Empty;
            try
            {
                stream = new MemoryStream(); // read xml in memory
                writer = new StreamWriter(stream, new UTF8Encoding());

                // get serialise object
                var t = instance.GetType();
                var serializer = new XmlSerializer(t);

                var xsn = new XmlSerializerNamespaces();
                xsn.Add(string.Empty, string.Empty);

                serializer.Serialize(writer, instance, xsn); // read object
                var count = (int) stream.Length; // saves object in memory stream
                var arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                // copy stream contents in byte array
                stream.Read(arr, 0, count);
                //UnicodeEncoding utf = new UnicodeEncoding(); // convert byte array to string
                var utf = new UTF8Encoding();
                objectXml = utf.GetString(arr).Trim();
            }
            catch (Exception ex)
            {
                var ss = ex.Message;
            }
            finally
            {
                if (stream != null && stream.Length > 0)
                {
                    stream.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return FormatXml(objectXml);
        }

        /// <summary>
        /// 格式化XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static string FormatXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return "";
            }

            const string startXml = "<?";
            const string endXml = "?>";
            var startPos = xml.IndexOf(startXml, StringComparison.Ordinal);
            var endPos = xml.IndexOf(endXml, StringComparison.Ordinal);
            if (!(startPos == -1 || endPos == -1))
            {
                return xml.Remove(startPos, endPos - startPos + endXml.Length);
            }
            return xml;
        }

        /// <summary>
        /// 反序列化XML字符串
        /// </summary>
        /// <param name="xml">xml data of employee</param>
        /// <returns></returns>
        public static object XmlToObject(string xml, Type t)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            Object o = null;
            try
            {
                // serialise to object
                var serializer = new XmlSerializer(t);
                stream = new StringReader(xml); // read xml data
                reader = new XmlTextReader(stream); // create reader

                //XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                //xsn.Add("xmlns", "http://tempuri.org/XMLSchema.xsd");
                // covert reader to object
                o = serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                var ss = ex.Message;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return o;
        }

        /// <summary>
        /// 将对象序列化为xml
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="t">对象类型</param>
        /// <returns></returns>
        public static string SerializableObjectToXml(object o, Type t)
        {
            var xs = new XmlSerializer(t);
            var mem = new MemoryStream();
            var writer = new XmlTextWriter(mem, Encoding.GetEncoding("UTF-16"));

            xs.Serialize(writer, o);
            writer.Close();
            return Encoding.GetEncoding("UTF-16").GetString(mem.ToArray());
        }

        /// <summary>
        /// 将xml反序列化为对象
        /// </summary>
        /// <param name="xmlString">原始XML字符串</param>
        /// <param name="t">反序列化的对象类型</param>
        /// <returns></returns>
        public static object DeserializeXmlToObject(string xmlString, Type t)
        {
            var xs = new XmlSerializer(t);
            var mem = new StreamReader(new MemoryStream(Encoding.GetEncoding("UTF-16").GetBytes(xmlString)),
                Encoding.GetEncoding("UTF-16"));
            var xr = XmlReader.Create(mem);
            var o = xs.Deserialize(xr);
            return o;
        }


        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static string GetConfigValue(string target, string xmlPath)
        {
            var xdoc = new XmlDocument();
            xdoc.Load(xmlPath);
            var root = xdoc.DocumentElement;
            if (root == null) return "";
            var elemList = root.GetElementsByTagName(target);
            return elemList[0].InnerXml;
        }
    }
}
