using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldsDivider
{
    public class StringFields
    {
        public static string GetRightField (List<string> lineFields, int pozition)
        {            
            string rField = "";
            if (pozition >= lineFields.Count || lineFields.Count == 0)
                rField = "null";
            else
                rField = lineFields[pozition];            
            lineFields.Clear();
            return rField;
        }
        public static List<string> GetAllFields (string line)
        {
            var field = new StringBuilder();
            var lineFields = new List<string>();
            char secondQuote = ' ';
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    field.Append(line[i + 1]);
                    i += 2;
                }
                if (line[i] != '\"' && line[i] != '\'')
                {
                    if (line[i] != ' ')
                        field.Append(line[i]);
                    else if (field.Length > 0)
                    {
                        lineFields.Add(field.ToString());
                        field.Clear();
                    }
                }
                else
                {
                    if (field.Length > 0)
                    {
                        lineFields.Add(field.ToString());
                        field.Clear();
                    }
                    secondQuote = line[i];
                    i++;
                    while (i != line.Length - 1 && line[i] != secondQuote)
                    {
                        if (line[i] == '\\')
                        {
                            field.Append(line[i + 1]);
                            i += 2;
                        }
                        field.Append(line[i]);
                        i++;
                    }
                    if (i == line.Length - 1 && line[i] != secondQuote)
                        field.Append(line[i]);
                    lineFields.Add(field.ToString());
                    field.Clear();
                }
            }
            if (field.Length > 0)
            {
                lineFields.Add(field.ToString());
                field.Clear();
            }
            return lineFields;
        }

            
        public static string[] GetRightFields(string[] text, int f)
        {
                    
            var rightFields = new string[text.Length];
            int j = 0;
            foreach (var str in text)
            {
                var allFields = GetAllFields(str);
                rightFields[j] = GetRightField(allFields, f);
                j++;
            }                          
            return rightFields;
        }
    }
}
