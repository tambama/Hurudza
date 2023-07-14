using System.Xml.Serialization;
using Serilog;

namespace Hurudza.Common.Utils.Helpers;

public class Helper
{
    public static string ToXML<T>(T @object)
    {
        using (var stringwriter = new System.IO.StringWriter())
        {
            var serializer = new XmlSerializer(@object.GetType());
            serializer.Serialize(stringwriter, @object);
            return stringwriter.ToString();
        }
    }

    public static T FromXML<T>(string xml)
    {
        T returnObject = default(T);
        if (string.IsNullOrEmpty(xml)) return default(T);

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                returnObject = (T)serializer.Deserialize(reader);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }

        return returnObject;
    }
}