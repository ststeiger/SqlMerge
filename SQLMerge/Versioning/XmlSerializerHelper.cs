
namespace SQLMerge
{


    public static class XmlSerializerHelper
    {


        public static string SerializeToXml<T>(System.Collections.Generic.List<T> objects, string rootElementName)
        {
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(
                typeof(System.Collections.Generic.List<T>), 
                new System.Xml.Serialization.XmlRootAttribute(rootElementName)
            );

            System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
            {
                serializer.Serialize(stringWriter, objects, namespaces);
                return stringWriter.ToString();
            } // End Using stringWriter 

        } // End Function SerializeToXml 


        public static System.Collections.Generic.List<T> DeserializeFromXml<T>(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(
                typeof(System.Collections.Generic.List<T>), 
                new System.Xml.Serialization.XmlRootAttribute("ArrayOf" + typeof(T).Name)
            ); //or specify your own root name

            using (System.IO.StringReader stringReader = new System.IO.StringReader(xml))
            {
                return (System.Collections.Generic.List<T>)serializer.Deserialize(stringReader);
            } // End Using stringReader 

        } // End Function DeserializeFromXml 


    } // End Class XmlSerializerHelper 


    public class Example
    {


        public static void Test()
        {
            System.Collections.Generic.List<sql_change_tracking> trackingList = new System.Collections.Generic.List<sql_change_tracking>
            {
                new sql_change_tracking
                {
                    ct_uid = System.Guid.NewGuid(),
                    ct_script_sequence_no = 1,
                    ct_script_sub_sequence_no = 1,
                    ct_script_name = "Script1.sql",
                    ct_folder_name = "Folder1",
                    ct_executed_by = "User1",
                    ct_executed_at =System. DateTime.Now,
                    ct_succeeded = true,
                    ct_error_message = null
                },
                new sql_change_tracking
                {
                    ct_uid = System.Guid.NewGuid(),
                    ct_script_sequence_no = 2,
                    ct_script_sub_sequence_no = 1,
                    ct_script_name = "Script2.sql",
                    ct_folder_name = "Folder1",
                    ct_executed_by = "User2",
                    ct_executed_at = System.DateTime.Now.AddDays(-1),
                    ct_succeeded = false,
                    ct_error_message = "Error occurred"
                }
            };

            string xml = XmlSerializerHelper.SerializeToXml(trackingList, "DeploymentTracking");
            System.Console.WriteLine(xml);

            // Optional: Deserialize back from XML
            System.Collections.Generic.List<sql_change_tracking> deserializedList = XmlSerializerHelper.DeserializeFromXml<sql_change_tracking>(xml);
            System.Console.WriteLine($"Deserialized List Count: {deserializedList.Count}");
        } // End Sub Test 


    } // End Class Example 


} // End Namespace 
