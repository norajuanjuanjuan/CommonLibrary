using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoTesterLib
{
   public class XmlHelper
    {
       public static void InitalCofigXMLFiles(string filePath)
       {
           XDocument doc = new XDocument(
               new XElement("Configuration",
                   new XElement("Server",
                       new XAttribute("Name","")),
                   new XElement("DB", ""),
                   new XElement("Simulator", ""),
                   new XElement("SoftVer","")
                   )
               );
           doc.Save(filePath);
       }

       //get value for level1
       public static string GetElementTextValue1(string filePath,string elementName)
       {
           string elementValue = "";
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement element = root.Element(elementName);
           if (element != null) elementValue = element.Value.ToString();
           return elementValue;
       }

       //get attr value for level1
       public static string GetElementAttrValue1(string filePath,string elementName,string attrName)
       {
           string elementAttrValue = "";
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement element = root.Element(elementName);
           XAttribute attr = element.Attribute(attrName);
           if (attr != null) elementAttrValue=attr.Value;
           return elementAttrValue;
       }

       //get values for level2
       public static List<string> GetElementValues2(string filePath, string parentElementName)
       {
           List<string> listAttrValues = new List<string>();
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement element = root.Element(parentElementName);
           var query = from item in element.Elements()
                       select new {
                       value=item.Value
                       };
           foreach(var item in query)
           {
               if (!listAttrValues.Contains(item.value))
                   listAttrValues.Add(item.value);
           }
           return listAttrValues;
       }
       //set value for level 1
       public static void SetElementTextValue1(string filePath,string elementName,string updatedValue)
       { 
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement element = root.Element(elementName);
           if (element != null) element.SetValue(updatedValue);
           doc.Save(filePath);
       }

       public static void SetElementAttributeValue(string filePath, string elementName,string attrName, string updatedValue)
       {
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement element = root.Element(elementName);
           XAttribute attr = element.Attribute(attrName);
           if (attr != null) attr.SetValue(updatedValue);
           doc.Save(filePath);
       }

       public static void AddElement(string filePath,string rootElement,string elementName,string elementValue)
       {
           XDocument doc = XDocument.Load(filePath);
           XElement root = doc.Element("Configuration");
           XElement secondRoot = root.Element(rootElement);
           bool isExists = false;
           var query = from item in secondRoot.Elements()
                       select new {
                           Name=item.Value
                       };
           foreach (var item in query)
           { if (item.Name == elementValue) isExists = true; }
           if (isExists == false)
           {
               XElement newElement = new XElement(elementName, elementValue);
               secondRoot.Add(newElement);
               doc.Save(filePath);
           }
       }
    }
}
