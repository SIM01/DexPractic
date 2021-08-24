using System.IO;
using System.Reflection;
using BankSystem.Models;
namespace BankSystem.Service
{
    public class ExportData
    {
        public void ExportToFile<T>(string path, T person) 
        {
            PropertyInfo[] myProperty = person.GetType().GetProperties();
            var myFields = person.GetType().GetFields();
            string textToInsert = "";
            foreach (PropertyInfo f in myProperty)
            {
                textToInsert = textToInsert +"\n" +f.Name+" - " + f.GetValue(person) + ";";
            }
            foreach (var f in myFields)
            {
                textToInsert = textToInsert +"\n" +f.Name+" - " + f.GetValue(person) + ";";
            }

            
            using (FileStream fileStream = new FileStream(path, FileMode.Append))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(textToInsert);
                fileStream.Write(array, 0, array.Length);
            }
        }
    }
}