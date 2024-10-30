﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB;
using Microsoft.EntityFrameworkCore;

namespace KlonsF.Classes
{
    public class UpgradeHelper
    {
        public static bool CanUseVeriom(string db_ver, string app_ver)
        {
            return string.Compare(db_ver, app_ver) <= 0 &&
                   string.Compare(db_ver, "012") >= 0;
        }

        private static string[] dbversions =
            new string[] { "301" };

        public static bool HasUpgrade(string db_ver, string app_ver)
        {
            if (!CanUseVeriom(db_ver, app_ver)) return false;
            if (db_ver == app_ver) return false;
            foreach (var s in dbversions)
            {
                if (string.Compare(s, db_ver) > 0) return true;
            }
            return false;
        }

        public static void ShowError(Exception e)
        {
            Form_Error.ShowException(Form_Main.MyInstance, e
                , "Datu bāzes aktualizācija neizdevās.");
        }


        public static string GetSQL(string sqlfilename)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;

            string resname = "KlonsF.SQL." + sqlfilename + ".txt";

            _assembly = Assembly.GetExecutingAssembly();
            _textStreamReader = new StreamReader(
                _assembly.GetManifestResourceStream(resname));

            string sql = _textStreamReader.ReadToEnd();
            return sql;
        }


        public static bool UpgradeThis(string from_ver, string to_ver)
        {
            try
            {
                foreach (var s in dbversions)
                {
                    if (string.Compare(s, from_ver) <= 0) continue;
                    if (string.Compare(s, to_ver) > 0) break;

                    if (!UpgradeThisA(s))
                    {
                        ShowError(null);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                ShowError(e);
                return false;
            }
        }

        public static bool UpgradeThisA(string to_ver)
        {
            return UpgradeThis("SQLV" + to_ver);
        }

        public static bool UpgradeThis(string sqlfilename)
        {
            string sql = GetSQL(sqlfilename);

            FbConnection conn = KlonsData.St.DbContextF.Database.GetDbConnection() as FbConnection;
            if (conn == null) return false;

            FbScript script = new FbScript(sql);
            script.Parse();

            FbBatchExecution fbe = new FbBatchExecution(conn);

            foreach (var cmd in script.Results) 
            {
                fbe.Statements.Add(cmd);
            }

            fbe.Execute();

            return true;
        }

    }
}
