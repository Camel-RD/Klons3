using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using KlonsLIB.Misc;
using KlonsLIB;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Data;
using KlonsF.Classes;
using System.Diagnostics;
using Klons3.ModelsF;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using KlonsF.Forms;

namespace KlonsF
{
    public partial class Form_SomeTools : MyFormBaseF
    {
        public Form_SomeTools()
        {
            InitializeComponent();
        }


        string db_filename = @"D:\A1-docs\c_net\Klons3\DB\klons_str.fdb";
        string reports_folder = @"D:\A1-docs\c_net\Klons3\Klons3\Reports";
        string xsd_filename = @"D:\A1-docs\c_net\Klons3\Klons3\ReportItems\ReportItemSchemasF.xsd";

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static string MakeConnectionString(string dbfilename)
        {
            var csb = new FbConnectionStringBuilder();
            csb.Charset = "UTF8";
            //csb.ClientLibrary = "fbclient.dll";
            csb.ServerType = FbServerType.Default;
            csb.Database = dbfilename;
            csb.DataSource = "localhost";
            csb.UserID = "KLONS";
            csb.Password = "parole";
            csb.Role = "RDB$ADMIN";
            csb.Port = 3051;
            var con_str = csb.ConnectionString;
            return con_str;
        }

        public static string[] GetDBTableNames(string dbfilename)
        {
            var con_str = MakeConnectionString(dbfilename);
            using (var con = new FbConnection(con_str))
            {
                con.Open();
                var tables_names = con.GetSchema("Tables", new string[] { null, null, null, "TABLE" });
                var ret = new string[tables_names.Rows.Count];
                for (int i = 0; i < tables_names.Rows.Count; i++)
                {
                    var row = tables_names.Rows[i];
                    ret[i] = (string)row["TABLE_NAME"];
                }
                con.Close();
                return ret;
            }
        }

        string[] ignore_table = ["F_TREP","F_TTEMP","F_ROPS","F_LOPS","F_TUDOC",
            "F_TABLE1","F_OPSD_YEAR_SEQ", "F_VW_OPS","PARAMS","USERS"];

        private void miGetDbTableNames_Click(object sender, EventArgs e)
        {
            //var constr = MakeConnectionString(db_filename);
            var table_name = GetDBTableNames(db_filename);
            var t = string.Join("\r\n", table_name);
            richTextBox1.Text = t;
        }

        void AddTsField(FbConnection con, string table_name)
        {
            var sql_ts = $"ALTER TABLE {table_name} ADD TS TIMESTAMP DEFAULT LOCALTIMESTAMP NOT NULL";
            var sql_bu =
               $""""
                CREATE TRIGGER TS_{table_name}_BU FOR {table_name}
                ACTIVE BEFORE UPDATE POSITION 77
                AS
                BEGIN
                    NEW.TS = LOACALTIMESTAM;
                END
                """";
            var cm = con.CreateCommand();
            cm.CommandText = sql_ts;
            cm.ExecuteNonQuery();
            cm.CommandText = sql_bu;
            cm.ExecuteNonQuery();
        }

        public void DoAddTsField(string dbfilename)
        {
            var table_names = GetDBTableNames(dbfilename);
            var con_str = MakeConnectionString(dbfilename);
            using var con = new FbConnection(con_str);
            con.Open();
            for (int i = 0; i < table_names.Length; i++)
            {
                var table_name = table_names[i];
                if (ignore_table.Where(x => table_name.StartsWith(x)).Any()) continue;
                try
                {
                    AddTsField(con, table_name);
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText($"{table_name}:\r\n{ex.ToString()}\r\n");
                }
            }
            con.Close();
        }

        private void miAddTsFields_Click(object sender, EventArgs e)
        {
            DoAddTsField(db_filename);
        }

        List<string> GetUsedTables(string reportfilename)
        {
            var tag1 = "<rd:TableName>";
            var tag2 = "</rd:TableName>";
            var ret = new List<string>();
            var text = File.ReadAllText(reportfilename, Encoding.UTF8);
            int k1 = 0;
            while (true)
            {
                k1 = text.IndexOf(tag1, k1);
                if (k1 == -1) break;
                k1 += tag1.Length;
                int k2 = text.IndexOf(tag2, k1);
                var table_name = text.Substring(k1, k2 - k1);
                ret.Add(table_name);
                k1 = k2 + tag2.Length;
                if (k1 >= text.Length) break;
            }
            return ret;
        }

        public string ReplaceTagText(string text, int ka, int kb,
            string tag1, string tag2, Func<string, string> func_newvalue)
        {
            var sb = new StringBuilder();
            int k1 = ka;
            int head = 0;
            while (true)
            {
                k1 = text.IndexOf(tag1, k1);
                if (k1 == -1 || k1 > kb) break;
                k1 += tag1.Length;
                int k2 = text.IndexOf(tag2, k1);
                var value = text.Substring(k1, k2 - k1);

                var value_new = func_newvalue(value);

                if (value_new == null)
                {
                    k1 = k2 + tag2.Length;
                    if (k1 >= kb) break;
                    continue;
                }

                sb.Append(text.Substring(head, k1 - head));
                sb.Append(value_new);
                head = k2;
            }

            if (head < text.Length)
                sb.Append(text.Substring(head, text.Length - head));

            return sb.ToString();
        }

        public string ReplaceTagTextA(string text,
            string taga1, string taga2,
            string tagb1, string tagb2,
            Func<string, string> func_newvalue)
        {
            int k1 = 0;
            while (true)
            {
                k1 = text.IndexOf(taga1, k1);
                if (k1 == -1) break;
                k1 += taga1.Length;
                int k2 = text.IndexOf(taga2, k1);

                text = ReplaceTagText(text, k1, k2, tagb1, tagb2, func_newvalue);

                k1 = k2 + taga2.Length;
                if (k1 > text.Length) break;
            }

            return text;
        }

        public string AddTextLine(string text,
            string taga1, string taga2, string tagline)
        {
            var lines = text.Split("\r\n").ToList();
            int k1 = 0, k2 = 0;
            var poslist = new List<int>();
            while (true)
            {
                k1 = lines[k2..].FindIndex(x => x.Contains(taga1));
                if (k1 == -1) break;
                k1 = k1 + k2 + 1;
                k2 = lines[k1..].FindIndex(x => x.Contains(taga2));
                if (k2 == -1) break;
                k2 += k1;
                poslist.Add(k2);
                k2++;
                if (k2 >= lines.Count) break;
            }
            for (int i = poslist.Count - 1; i >= 0; i--)
            {
                lines.Insert(poslist[i], tagline);
            }
            var ret = string.Join("\r\n", lines);
            return ret;
        }

        public string KeepOnlyFirst(string text,
            string taga1, string taga2)
        {
            var lines = text.Split("\r\n").ToList();
            int k1 = 0, k2 = 0;
            var poslist = new List<(int, int)>();
            bool firstfound = false;
            while (true)
            {
                k1 = lines[k2..].FindIndex(x => x.Contains(taga1));
                if (k1 == -1) break;
                k1 = k1 + k2;
                k2 = lines[(k1 + 1)..].FindIndex(x => x.Contains(taga2));
                if (k2 == -1) break;
                k2 = k1 + k2 + 1;
                if (!firstfound)
                {
                    firstfound = true;
                    k2++;
                    continue;
                }
                poslist.Add((k1, k2));
                k2++;
                if (k2 >= lines.Count) break;
            }
            for (int i = poslist.Count - 1; i >= 0; i--)
            {
                (k1, k2) = poslist[i];
                lines.RemoveRange(k1, k2 - k1 + 1);
            }
            var ret = string.Join("\r\n", lines);
            return ret;
        }

        string MapTableName(string tableName)
        {
            string[] names = ["BalA2", "vw_OPS", "Persons", "ROPS1A", "ROPS2A",
                "ROPS2B", "ROPS3A", "TREPA1", "TREPAPAN", "TREPDARZ1", "TREPDARZ2",
                "TREPDOCS1", "TREPDOCS2", "TREPDOCSS", "TREPMT", "TREPOPS",
                "TREPOPSD", "TREPPVNZ1", "TREPUDOCS"];

            var ff = names.Where(x => x.ToLower() == tableName.ToLower()).FirstOrDefault();

            var ret = tableName;
            if (ff == null && tableName == "ROps2a1")
                ff = ret = "ROps2a";

            if (ff != null)
            {
                ret = "F_" + ret.ToUpper();
            }
            else
            {
                richTextBox1.AppendText($"Not found: {tableName}\r\n");
            }
            return ret;
        }

        public string RemoveLines(string text, string contains)
        {
            var lines = text.Split("\r\n").ToList();
            lines = lines.Where(x => !x.ContainsCI(contains)).ToList();
            var ret = string.Join("\r\n", lines);
            return ret;
        }

        string ProcessReport1(string text)
        {
            text = ReplaceTagTextA(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                "<rd:DataSetName>", "</rd:DataSetName>",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<DataSources>", "</DataSources>",
                "<DataSource Name=\"", "\">",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<Query>", "</Query>",
                "<DataSourceName>", "</DataSourceName>",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                "<rd:SchemaPath>", "</rd:SchemaPath>",
                x => @"D:\A1-docs\c_net\Klons3\Klons3\ReportItems\ReportItemSchemasF.xsd");

            text = ReplaceTagTextA(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                "<rd:TableName>", "</rd:TableName>",
                MapTableName);

            text = ReplaceTagText(text,
                0, text.Length,
                "Fields!", ".Value",
                x => x.ToUpper());

            text = ReplaceTagTextA(text,
                "<Fields>", "</Fields>",
                "<Field Name=\"", "\">",
                x => x.ToUpper());

            text = ReplaceTagTextA(text,
                "<Fields>", "</Fields>",
                "<DataField>", "</DataField>",
                x => x.ToUpper());

            text = RemoveLines(text, "<rd:TableAdapterFillMethod>");
            text = RemoveLines(text, "<rd:TableAdapterGetDataMethod>");
            text = RemoveLines(text, "<rd:TableAdapterName>");
            return text;
        }

        string ProcessReport2(string text)
        {
            text = ReplaceTagTextA(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                "<rd:DataSetName>", "</rd:DataSetName>",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<DataSources>", "</DataSources>",
                "<DataSource Name=\"", "\">",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<Query>", "</Query>",
                "<DataSourceName>", "</DataSourceName>",
                x => "ReportItemSchemasF");

            text = ReplaceTagTextA(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                "<rd:SchemaPath>", "</rd:SchemaPath>",
                x => @"D:\A1-docs\c_net\Klons3\Klons3\ReportItems\ReportItemSchemasF.xsd");

            text = AddTextLine(text,
                "<rd:DataSetInfo>", "</rd:DataSetInfo>",
                @"        <rd:SchemaPath>D:\A1-docs\c_net\Klons3\Klons3\ReportItems\ReportItemSchemasF.xsd</rd:SchemaPath>");

            text = KeepOnlyFirst(text,
                "<DataSource Name=\"ReportItemSchemasF\">", "</DataSource>");

            text = RemoveLines(text, "<rd:TableAdapterFillMethod>");
            text = RemoveLines(text, "<rd:TableAdapterGetDataMethod>");
            text = RemoveLines(text, "<rd:TableAdapterName>");
            text = RemoveLines(text, "<rd:ObjectDataSourceType>");
            return text;
        }

        private void miListUseeTable_Click(object sender, EventArgs e)
        {
            var pt = Path.Combine(reports_folder, "In");
            var di = new DirectoryInfo(pt);
            var fis = di.GetFiles("*.rdlc");
            var namelist = new List<string>();
            foreach (var fi in fis)
            {
                var table_names = GetUsedTables(fi.FullName);
                namelist.AddRange(table_names);
                richTextBox1.AppendText("\r\n\r\n" + fi.Name + "\r\n");
                foreach (var table_name in table_names)
                {
                    richTextBox1.AppendText(table_name + "\r\n");
                }
            }
            namelist = namelist.Distinct().ToList();
            richTextBox1.AppendText("\r\n\r\nConsolidated:  \r\n");
            foreach (var table_name in namelist)
            {
                richTextBox1.AppendText(table_name + "\r\n");
            }
        }

        private void miReportsProcess1_Click(object sender, EventArgs e)
        {
            var pt = Path.Combine(reports_folder, "In");
            var di = new DirectoryInfo(pt);
            var reports_folder_out = Path.Combine(reports_folder, "Out");
            var fis = di.GetFiles("*.rdlc");
            foreach (var fi in fis)
            {
                var text = File.ReadAllText(fi.FullName);
                text = ProcessReport1(text);
                var fnm = Path.Combine(reports_folder_out, fi.Name);
                File.WriteAllText(fnm, text);
            }
            MessageBox.Show("Done");
        }

        private void miReportsProcess2_Click(object sender, EventArgs e)
        {
            var pt = Path.Combine(reports_folder, "In");
            var di = new DirectoryInfo(pt);
            var reports_folder_out = Path.Combine(reports_folder, "Out");
            var fis = di.GetFiles("*.rdlc");
            foreach (var fi in fis)
            {
                var text = File.ReadAllText(fi.FullName);
                text = ProcessReport2(text);
                var fnm = Path.Combine(reports_folder_out, fi.Name);
                File.WriteAllText(fnm, text);
            }
            MessageBox.Show("Done");
        }


        private void GenerateXSD()
        {
            var types = new[]
            {
                typeof(Klons3.ModelsFRep.F_ROPS1A),
                typeof(Klons3.ModelsFRep.F_ROPS2A),
                typeof(Klons3.ModelsFRep.F_ROPS2B),
                typeof(Klons3.ModelsFRep.F_ROPS3A),
                typeof(Klons3.ModelsFRep.F_TREPA1),

                typeof(Klons3.ModelsFRep.F_TREPAPAN),
                typeof(Klons3.ModelsFRep.F_TREPDARZ1),
                typeof(Klons3.ModelsFRep.F_TREPDARZ2),
                typeof(Klons3.ModelsFRep.F_TREPDOCS1),
                typeof(Klons3.ModelsFRep.F_TREPDOCS2),

                typeof(Klons3.ModelsFRep.F_TREPDOCSS),
                typeof(Klons3.ModelsFRep.F_TREPMT),
                typeof(Klons3.ModelsFRep.F_TREPOPS),

                typeof(Klons3.ModelsFRep.F_TREPOPSD),

                typeof(Klons3.ModelsFRep.F_TREPPVNZ1),
                typeof(Klons3.ModelsFRep.F_TREPUDOCS),

                typeof(KlonsP.Classes.EventInfo0),
                typeof(KlonsP.Classes.ItemInfo),
                typeof(KlonsP.Classes.ItemRepInfo),
                typeof(KlonsP.Classes.EventRepInfo),
                typeof(Klons3.ModelsP.P_TAXDEPRECYEAR),

                typeof(Forms.RepRow_BalMT),

                typeof(KlonsA.Classes.SickDayCalcRow),
                typeof(KlonsA.Classes.AvPayCalcRow),
                typeof(KlonsA.Classes.WorkPayCalcRow1),
                typeof(KlonsA.Classes.VacationCalcRow),
                typeof(KlonsA.Classes.CalcRRow),
                typeof(KlonsA.Classes.BonusCalcRow),
                typeof(KlonsA.Classes.SalaryCalcRow),
                typeof(KlonsA.Classes.TimeReportRow1),
                typeof(KlonsA.Classes.TimeReportRow2a),
                typeof(KlonsA.Classes.IINReportRow),
                typeof(KlonsA.Classes.RepRowPerson),
                typeof(KlonsA.Classes.RepRowPosition),
                typeof(KlonsA.Classes.RepRowEvent),
                typeof(KlonsA.Classes.RepRowPersonStatusChange),
                typeof(KlonsA.Classes.RepRowPersonRatesChange),
                typeof(KlonsA.Classes.RepRowPositionChange),
                typeof(KlonsA.Classes.SalaryRepRow2),
                typeof(KlonsA.Classes.RepRowPayList),
                typeof(KlonsA.Classes.RepRowPayListRow),
                typeof(KlonsA.Classes.VSAOIReportRow1),
                typeof(KlonsA.Classes.EventsReportRow),
                typeof(KlonsM.Classes.InvoiceMainData),
                typeof(KlonsM.Classes.InvoiceRowData),
                typeof(KlonsM.Classes.InvoiceTotalsRowData),


            };
            var xri = new System.Xml.Serialization.XmlReflectionImporter();
            var xss = new System.Xml.Serialization.XmlSchemas();
            var xse = new System.Xml.Serialization.XmlSchemaExporter(xss);
            foreach (var type in types)
            {
                var xtm = xri.ImportTypeMapping(type);
                xse.ExportTypeMapping(xtm);
            }
            using var sw = new StreamWriter(xsd_filename, false, Encoding.UTF8);
            for (int i = 0; i < xss.Count; i++)
            {
                var xs = xss[i];
                xs.Id = "ReportItemSchemasF";
                xs.Write(sw);
            }
            sw.Close();

            var text = File.ReadAllText(xsd_filename);
            text = RemoveLines(text, "type=\"ArrayOf");
            File.WriteAllText(xsd_filename, text);
        }

        private void miGenerateXSD_Click(object sender, EventArgs e)
        {
            GenerateXSD();
            MessageBox.Show("Done");
        }

        void TestReload()
        {
            var sw = new Stopwatch();
            sw.Start();
            myBindingSourceEf1.MyDataSource = null;
            //myBindingSourceEf1.KillLists();
            //Debug.Assert(myBindingSourceEf1.Count == 0, "KillLists failed");
            var m1 = GC.GetTotalMemory(true);
            var ctx = Classes.KlonsData.St.DbContextF;
            var ops = ctx.F_OPS.Local.ToList();
            long t1 = sw.ElapsedMilliseconds;
            for (int i = 0; i < ops.Count; i++)
            {
                var op = ops[i];
                ctx.Entry(op).State = EntityState.Detached;
                ops[i] = null;
            }
            ops.Clear();
            var docs = ctx.F_OPSD.Local.ToList();
            for (int i = 0; i < docs.Count; i++)
            {
                var doc = docs[i];
                ctx.Entry(doc).State = EntityState.Detached;
                docs[i] = null;
            }
            docs.Clear();
            t1 = sw.ElapsedMilliseconds - t1;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ctx.F_OPSD.Load();
            ctx.F_OPS.Load();
            myBindingSourceEf1.RecreateLists();
            var m2 = GC.GetTotalMemory(false);
            sw.Stop();
            var msg = $"Done: {m2 / 1000000} - {m1 / 1000000} = {(m2 - m1) / 1000000} " +
                $"t:{sw.ElapsedMilliseconds} t1:{t1}\r\n";
            richTextBox1.AppendText(msg);
        }

        void TestReload2()
        {
            var sw = new Stopwatch();
            sw.Start();
            myBindingSourceEf1.MyDataSource = null;
            var m1 = GC.GetTotalMemory(true);
            var ctx = Classes.KlonsData.St.DbContextF;
            long t1 = sw.ElapsedMilliseconds;
            ctx.ClearLocalDbSet<F_OPS>();
            ctx.ClearLocalDbSet<F_OPSD>();

            /*
            var docs = ctx.F_OPSD.Local.ToList();
            for (int i = 0; i < docs.Count; i++)
            {
                var doc = docs[i];
                ctx.Entry(doc).State = EntityState.Detached;
                docs[i] = null;
            }
            docs.Clear();
            */

            t1 = sw.ElapsedMilliseconds - t1;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ctx.F_OPSD.Load();
            ctx.F_OPS.Load();
            var m2 = GC.GetTotalMemory(false);
            sw.Stop();
            var msg = $"Done: {m2 / 1000000} - {m1 / 1000000} = {(m2 - m1) / 1000000} " +
                $"t:{sw.ElapsedMilliseconds} t1:{t1}\r\n";
            richTextBox1.AppendText(msg);
        }

        private void TestReloadDocs()
        {
            for (int i = 0; i < 10; i++)
            {
                richTextBox1.AppendText($"{i} ");
                TestReload2();
            }
        }


        void TestSetState()
        {
            var ctx = KlonsData.St.DbContextF;
            /*
            var ac = new F_ACP21()
            {
                AC = "TEST"
            };
            ctx.F_ACP21.Add(ac);
            ctx.SaveChanges();*/
            
            var ac = ctx.F_ACP21.Find("TEST");
            ac.NAME = "test2";
            var entry = ctx.Entry(ac);
            //var internal_entry = entry.GetInfrastructure();
            //internal_entry.SetEntityState(EntityState.Unchanged, true);
            entry.State = EntityState.Unchanged;
        }

        private void miTestXYZ_Click(object sender, EventArgs e)
        {
            TestSetState();
        }

    }

}
