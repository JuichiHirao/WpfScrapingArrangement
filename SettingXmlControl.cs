﻿using System.IO;
using System.Xml.Linq;

namespace WpfScrapingArrangement
{
    public class SettingXmlControl
    {
        public string XmlFilename = "SETTING.xml";
        public Setting GetData()
        {
            Setting setting = new Setting();
            //string ListPathname = "AUTOSELECT_SETTING.xml";

            XElement root = null;

            try
            {
                root = XElement.Load(XmlFilename);
            }
            catch (FileNotFoundException)
            {
                root = new XElement("Setting");
            }
            string[] XmlInfo = new string[2];

            if (root.Element("BasePath") != null)
                setting.BasePath = root.Element("BasePath").Value;
            if (root.Element("LabelPath") != null)
                setting.LabelPath = root.Element("LabelPath").Value;
            if (root.Element("KoreanPornoPath") != null)
                setting.KoreanPornoPath = root.Element("KoreanPornoPath").Value;
            if (root.Element("KoreanPornoExportPath") != null)
                setting.KoreanPornoExportPath = root.Element("KoreanPornoExportPath").Value;

            return setting;
        }

        public void Save(string myBasePath, string myLabelPath, string myKoreanPornoPath, string myKoreanPornoExportPath)
        {
            XElement root = new XElement("Setting");

            root.Add(new XElement("BasePath", myBasePath));
            root.Add(new XElement("LabelPath", myLabelPath));
            root.Add(new XElement("KoreanPornoPath", myKoreanPornoPath));
            root.Add(new XElement("KoreanPornoExportPath", myKoreanPornoExportPath));

            root.Save(XmlFilename);
        }

    }
}