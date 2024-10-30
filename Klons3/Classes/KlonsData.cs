using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;
using KlonsLIB;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;
using ModelsA = Klons3.ModelsA;
using ModelsP = Klons3.ModelsP;
using ModelsM = Klons3.ModelsM;
using ModelsFRep = Klons3.ModelsFRep;
using Klons3.ModelsA;
using Klons3.ModelsF;
using Klons3.ModelsM;
using Klons3.ModelsP;
using Klons3.ModelsFRep;
using System.Diagnostics;
using Equin.ApplicationFramework;
using KlonsA.Classes;

namespace KlonsF.Classes;

public class KlonsData : KlonsDataModuleEF
{
    private static KlonsData _KlonsData = null;

    public readonly string Version = "301";
    public readonly string VersionStr = "2024.10.#1";

    public readonly string SettingsFileName = GetBasePath() + "\\Config\\Settings.xml";
    public readonly string MasterListFileName = GetBasePath() + "\\Config\\MasterList.xml";
    public readonly string FolderForXMLReports = GetBasePath() + "\\XMLReports";
    public readonly string FolderForDBBackUp = GetBasePath() + "\\DB-backup";
    public readonly string FolderForFbEmbed25 = GetBasePath() + "\\FbEmbed25" + (Environment.Is64BitProcess ? "x64" : "");
    public readonly string FolderForFbEmbed4 = GetBasePath() + "\\FbEmbed4" + (Environment.Is64BitProcess ? "x64" : "");

    public static int VersionRef(int ver) => ver;

    public readonly float SalaryCalcHistoryInterval = 1.0f;

    public KlonsSettings Settings = new KlonsSettings();
    public MasterList MasterList { get; private set; }
    public MasterEntry CurrentDBTag { get; private set; }
    public string CurrentUserName { get; private set; }
    public KlonsParams Params { get; private set; }
    public Form_Main MyMainForm => Form_Main.MyInstance as Form_Main;
    public ReportHelperF ReportHelperF { get; private set; }
    public ReportHelperA ReportHelperA { get; private set; }
    public ErrorList ErrorInfoList { get; } = new ErrorList();

    public DbContextF DbContextF { get; private set; } = null;
    public DbContextFRep DbContextFRep { get; private set; } = null;
    public DbContextA DbContextA { get; private set; } = null;
    public DbContextP DbContextP { get; private set; } = null;
    public DbContextM DbContextM { get; private set; } = null;

    public new static KlonsData St
    {
        get
        {
            if (_KlonsData != null) return _KlonsData;
            _KlonsData = new KlonsData();
            return _KlonsData;
        }
    }

    public void Init()
    {
        var st = St;

        LoadSettings();
        LoadMasterList();

        if (Settings.MasterEntry.Name != "")
        {
            var me = MasterList.GetMasterEntryByName(Settings.MasterEntry.Name);
            if (me != null)
            {
                Settings.MasterEntry.CopyFrom(me);
            }
        }
        Params = new KlonsParams();

        ReportHelperF = new ReportHelperF();
        ReportHelperA = new ReportHelperA();
    }
        
    private KlonsData()
    {
        _KlonsData = this;
        _KlonsDataModule = this;
        CurrentDBTag = null;
        MyData.Settings = Settings;
        MakeFakeDbList();
        //if (!IsInDesignMode)
        //    MakeDbContexts();
    }

    private void MakeFakeDbList()
    {
        FakeDbContexts["KlonsData"] = MyDbContext.GetFakeList(typeof(DbContextF));
        FakeDbContexts["KlonsAData"] = MyDbContext.GetFakeList(typeof(DbContextA));
        FakeDbContexts["KlonsPData"] = MyDbContext.GetFakeList(typeof(DbContextP));
        FakeDbContexts["KlonsMData"] = MyDbContext.GetFakeList(typeof(DbContextM));
        FakeDbContexts["KlonsRep"] = MyDbContext.GetFakeList(typeof(DbContextFRep));
    }



    private void MakeDbContexts(string connectonstring, string username)
    {
        DisposeDbContexts();

        DbContextF?.Dispose();
        DbContextA?.Dispose();
        DbContextP?.Dispose();
        DbContextM?.Dispose();
        DbContextFRep?.Dispose();

        DbContexts["KlonsData"] = DbContextF = new DbContextF(connectonstring, username);
        DbContexts["KlonsAData"] = DbContextA = new DbContextA(connectonstring, username);
        DbContexts["KlonsPData"] = DbContextP = new DbContextP(connectonstring, username);
        DbContexts["KlonsMData"] = DbContextM = new DbContextM(connectonstring, username);
        DbContexts["KlonsRep"] = DbContextFRep = new DbContextFRep(connectonstring, username);

        DataLoaderA.ResetState();

        foreach (var context in DbContexts.Values)
        {
            context.InitDbSetList();
        }
    }

    public void RecreateDbContextF()
    {
        DbContextF?.Dispose();
        DbContexts["KlonsData"] = DbContextF = new DbContextF(CurrentConnectionString, CurrentUserName);
        DbContextF.InitDbSetList();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void RecreateDbContextP()
    {
        DbContextP?.Dispose();
        DbContexts["KlonsPData"] = DbContextP = new DbContextP(CurrentConnectionString, CurrentUserName);
        DbContextP.InitDbSetList();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void RecreateDbContextA()
    {
        DbContextA?.Dispose();
        DbContexts["KlonsAData"] = DbContextA = new DbContextA(CurrentConnectionString, CurrentUserName);
        DbContextA.InitDbSetList();
        DataLoaderA.ResetState();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void RecreateDbContextM()
    {
        DbContextM?.Dispose();
        DbContexts["KlonsMData"] = DbContextM = new DbContextM(CurrentConnectionString, CurrentUserName);
        DbContextM.InitDbSetList();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private void PushUserName(string userName)
    {
        DbContextF.Database.ExecuteSql($"EXECUTE PROCEDURE SP_SET_USERNAME {userName}");
    }

    public bool ConnectTo(MasterEntry me, string username, string userpsw)
    {
        string filename = GetFileName(me);

        if (CurrentDBTag != null)
        {
            try
            {
                Params.Save();
            }
            catch (Exception) { }
        }

        string newconnstr = GetConnectionString(me, username, userpsw);
        if (string.IsNullOrEmpty(newconnstr))
            throw new InvalidOperationException("Connection string not configured");

        CurrentUserName = username;
        CurrentDBTag = new MasterEntry(me);
        CurrentConnectionString = newconnstr;

        MakeDbContexts(newconnstr, username);

        return true;
    }

    public string GetConnectionString(MasterEntry me, string username, string userpsw)
    {
        if (username.IsNOE())
            throw new Exception($"Nav norādīts lietotāja vārds.");
        string filename = GetFileName(me);

        string newconnstr = MasterList.GetTemplateByName(me.ConnStr);
        if (string.IsNullOrEmpty(newconnstr))
        {
            throw new Exception($"Nav atrasti pieslēguma parametri: [{me.ConnStr}]");
        }

        if (userpsw.IsNOE()) userpsw = "null";
        newconnstr = string.Format(newconnstr, filename, username, userpsw);
        var s1 = CheckConnectionString(newconnstr);
        if (s1 == null)
            throw new Exception($"Nekorekti pieslēguma dati:\n{newconnstr}");
        newconnstr = s1;

        var csb = new FbConnectionStringBuilder(newconnstr);
        if (csb.ServerType == FbServerType.Embedded && !File.Exists(filename))
        {
            throw new Exception($"Nav faila: [{filename}]");
        }

        return newconnstr;
    }

    public void GectSomeDbSysData(MasterEntry me, string username, string userpsw,
        out string usertp, out string dbver)
    {
        var constr = GetConnectionString(me, username, userpsw);
        var csb = new FbConnectionStringBuilder(constr);
        csb.Pooling = false;
        constr = csb.ToString();
        userpsw = userpsw.IsNOE() ? "null" : $"'{userpsw}'";
        using (var con = new FbConnection(constr))
        {
            con.Open();
            using (var cm = new FbCommand())
            {
                cm.Connection = con;
                cm.CommandText = $"execute procedure SP_SYS_CHECK_USER '{username}', {userpsw}";
                var ret = cm.ExecuteScalar();
                usertp = ret == DBNull.Value ? null : (string)ret;
                cm.CommandText = $"select pvalue from params where pname = 'version' and usr = 'SYSTEM'";
                ret = cm.ExecuteScalar();
                dbver = ret == DBNull.Value ? null : (string)ret;
            }
            con.Close();
        }
    }

    public string GetFileName(MasterEntry me)
    {
        string filename;
        if (string.IsNullOrEmpty(me.Path))
        {
            filename = GetBaseDBPath();
        }
        else
        {
            filename = me.Path;
            filename = filename.Replace("@", GetBaseDBPath());
        }
        filename = Path.Combine(filename, me.FileName);
        return filename;
    }

    public string CheckConnectionString(string constr)
    {
        try
        {
            var csb = new FbConnectionStringBuilder(constr);
            if (csb.ClientLibrary == "fbembed.dll")
            {
                csb.ClientLibrary = $"{FolderForFbEmbed25}\\fbembed.dll";
            }
            else if (csb.ClientLibrary == "fbclient.dll")
            {
                csb.ClientLibrary = $"{FolderForFbEmbed4}\\fbclient.dll";
            }
            return csb.ToString();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static string GetBasePath()
    {
        var s = Utils.GetMyFolderX();
        return s;
    }

    public string GetBaseDBPath()
    {
        return Path.Combine(GetBasePath(), Settings.BaseDBPath);
    }

    public string GetBackUpFolder()
    {
        string backupfolder = Settings.BackUpFolder;
        if (backupfolder.IsNOE() || backupfolder == "DB-backup")
            backupfolder = FolderForDBBackUp;
        return backupfolder;
    }

    public string GetManualsPath()
    {
        return Path.Combine(GetBasePath(), "Apraksts");
    }


    public KlonsSettings LoadedSettings = new KlonsSettings();
    public MasterList LoadedMasterList { get; private set; }


    public bool HasChangesMasterList() => MasterList != null && MasterList != LoadedMasterList;
    public bool HasChangesSettings() => Settings != null && Settings != LoadedSettings;

    public void LoadMasterList()
    {
        MasterList = MasterList.LoadList(St.MasterListFileName);
        LoadedMasterList = MasterList with { };
    }

    public bool SaveMasterList()
    {
        if (!HasChangesMasterList()) return true;
        bool rt = MasterList.SaveList(St.MasterListFileName);
        if (!rt) return false;
        LoadedMasterList = MasterList with { };
        return true;
    }

    public void LoadSettings()
    {
        Settings = KlonsSettings.LoadSettings(St.SettingsFileName);
        MyData.Settings = Settings;
        LoadedSettings = Settings with { };
    }

    public bool SaveSettings()
    {
        if (!HasChangesSettings()) return true;
        bool rt = Settings.SaveSettings(St.SettingsFileName);
        if (!rt) return false;
        LoadedSettings = Settings with { };
        return true;
    }

    public void CreateNewDB(string name, string descr)
    {
        MasterEntry me = new MasterEntry();
        me.Name = name;
        me.Descr = descr;
        me.ConnStr = Settings.BaseConnStr;
        string path = GetBaseDBPath();
        me.FileName = Utils.GetNextFile(path, "klons_", "fdb");
        me.FileName = Utils.GetFileNameFromURL(me.FileName);
        CreateNewDB(me);
    }
    public void CreateNewDB(MasterEntry me)
    {
        MasterList.ConnectionList.Add(me);
        string path = GetBaseDBPath();
        string fnmbase = path + "\\base\\Klons_00.fdb";
        string fnmnew = path + "\\" + me.FileName;
        if (!File.Exists(fnmbase))
        {
            throw new Exception("Fails [" + fnmbase + "] netika atrasts.");
        }
        File.Copy(fnmbase, fnmnew);
        return;
    }

    public void LoadUsersTable()
    {
        DbContextF.USERS.Load();
    }

    public void FillParamsForUser(string username)
    {
        Params.FillForUser(username);
    }

    public bool TestUserPassword(string username, string password)
    {
        var r = DbContextF.USERS.Find(username);
        if (r == null) return false;
        if (string.IsNullOrEmpty(r.PSW)) return true;
        if (password == null) return false;
        return r.PSW == password;
    }
    
    public string GetUserGroup(string username)
    {
        var r = DbContextF.USERS.Find(username);
        if (r == null) return null;
        if (string.IsNullOrEmpty(r.TP)) return null;
        return r.TP;
    }

    public void SetCurrentUserName(string username)
    {
        CurrentUserName = username;
    }
    public bool ChangeUserPassword(string username, string password)
    {
        var r = DbContextF.USERS.Find(username);
        if (r == null) return false;
        r.PSW = password;
        DbContextF.Save();
        return true;
    }

    public void FillBaseTables()
    {
        DataLoaderF.ReLoadData();
    }


    public static double RoundA(double d, int k)
    {
        return Math.Round(d, k, MidpointRounding.AwayFromZero);
    }

    public static decimal RoundA(decimal d, int k)
    {
        return Math.Round(d, k, MidpointRounding.AwayFromZero);
        /*
            if (KlonsData.St.Params.RoundUp)
                return Math.Round(d, k, MidpointRounding.AwayFromZero);
            else
                return Math.Round(d, k);
        */
    }


}
