using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using KlonsLIB.Misc;

namespace KlonsF.Classes
{
    public record MasterEntry
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Descr { get; set; }
        public string ConnStr { get; set; }
        public string Path { get; set; }

        public MasterEntry()
        {
            Clear();
        }
        public MasterEntry(MasterEntry me)
        {
            CopyFrom(me);
        }

        public void Clear()
        {
            Name = "";
            FileName = "";
            Descr = "";
            ConnStr = "";
            Path = "";
        }
        public void CopyFrom(MasterEntry me)
        {
            Name = me.Name;
            FileName = me.FileName;
            Descr = me.Descr;
            ConnStr = me.ConnStr;
            Path = me.Path;
        }
    }

    public record ConnectionStringTemplate
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Template { get; set; }
    }

    public record MasterList
    {
        public BindingList<ConnectionStringTemplate> TemplateList = new BindingList<ConnectionStringTemplate>();
        public BindingList<MasterEntry> ConnectionList = new BindingList<MasterEntry>();

        public MasterEntry GetMasterEntryByName(string name)
        {
            foreach (var me in ConnectionList)
            {
                if (me.Name == name) return me;
            }
            return null;
        }
        public string GetTemplateByName(string name)
        {
            foreach (var te in TemplateList)
            {
                if (te.Name == name) return te.Template;
            }
            return null;
        }

        public string[] GetTemplateNames()
        {
            int k = TemplateList.Count;
            string[] connStrNames = new string[k];
            for (int i = 0; i < k; i++)
                connStrNames[i] = TemplateList[i].Name;
            Array.Sort(connStrNames);
            return connStrNames;
        }

        public static MasterList LoadList(string filename)
        {
            var rt = Utils.LoadDataFromXML<MasterList>(filename);
            if (rt != null) return rt;
            rt = new MasterList();
            return rt;
        }

        public bool SaveList(string filename)
        {
            return Utils.SaveDataToXML(this, filename);
        }

    }
}
