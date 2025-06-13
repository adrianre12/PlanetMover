using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WorldMover
{
    public static class Xml
    {
        #region Fields

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
        private static readonly XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });  //new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        #endregion

        #region Methods

        public static string Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return DoSerialize(obj);
        }

        private static string DoSerialize(this object obj)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, WriterSettings))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj, Namespaces);
                return sb.ToString();
            }
        }

        public static T Deserialize<T>(this string data)
            where T : class
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return DoDeserialize<T>(data);
        }

        private static T DoDeserialize<T>(this string data) where T : class
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        #endregion
    }
}
