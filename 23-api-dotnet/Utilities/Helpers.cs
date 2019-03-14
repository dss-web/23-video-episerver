using System;
using System.Xml.XPath;

namespace Visual
{
    public static class Helpers
    {
        public static string GetNodeChildValue(XPathNavigator node, string childName)
        {
            XPathNodeIterator contentNode = node.SelectChildren(childName, "");
            if ((contentNode.MoveNext()) && (contentNode.Current != null)) return contentNode.Current.Value;
            else return null;
        }

        public static int ConvertStringToInteger(string value)
        {
            int result;

            try
            {
                result = Convert.ToInt32(value);
            }
            catch
            {
                return -1;
            }

            return result;
        }

        public static double ConvertStringToDouble(string value)
        {
            double result;

            try
            {
                result = Convert.ToDouble(value);
            }
            catch
            {
                return -1;
            }

            return result;
        }
    }
}
