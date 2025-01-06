﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsF.Forms;
using KlonsF.FormsReportParams;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using KlonsA.Forms;
using KlonsM.FormsM;
using System.Diagnostics;
using KlonsA.Classes;
using KlonsP.Classes;
using KlonsM.Classes;
using System.IO;

namespace KlonsF;

public partial class Form_Main : MyMainFormBase
{
    public Form_Main()
    {
        MyMainForm = this;
        KlonsData.St.Init();
        InitializeComponent();
        SetupMenuRenderer();
        CheckMyFontAndColors();
        Application.ThreadException += Application_ThreadException;
        KlonsLIB.MyData.InWine = MyData.Settings.InWine == "YES";
    }

    private void Form_Main_Load(object sender, EventArgs e)
    {
        MyData.LoadSettings();
        if (MyData.Settings.WindowPos == "max")
        {
            this.WindowState = FormWindowState.Maximized;
        }
        tsWindowList.Visible = MyData.Settings.ShowWindowListToolStrip;

    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ChangeDB();
        DoVersionCheck();
    }


    private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        if (e.Exception is ArgumentException &&
            e.Exception.Message == "Parameter is not valid." &&
            e.Exception.Source == "System.Drawing" &&
            e.Exception.StackTrace.Contains("Microsoft.Reporting.WinForms.AsyncWaitMessage.OnLoad"))
            return;

        Form_Error.ShowException(e.Exception,
            "Programmas kļūda!\r\n" +
            "Ieteicams programmu aizvērt un palaist no jauna.\r\n" +
            "Ja kļūda bieži atkārtojas, ieteicams par to informēt programmas izstrādātāju.");
    }


    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KlonsData MyData { get { return KlonsData.St; } }

    public override MyColorTheme MyColorTheme
    {
        get { return MyData.Settings.ColorTheme; }
    }

    public void ChangeDB()
    {
        if (!CloseAllForms()) return;
        Form_StartUp fs = new Form_StartUp();
        if (fs.ShowDialog(this) != DialogResult.OK)
        {
            Application.Exit();
            return;
        }
    }

    private int klonsverURLNr = -1;
    private void DoVersionCheck()
    {
        if (MyData.Settings.DoVersionCheck != "YES") return;
        string ld = MyData.Settings.LastVersionCheckDate;
        DateTime dt;
        if (Utils.StringToDate(ld, out dt))
        {
            if (dt.AddDays(2) > DateTime.Today) return;
        }
        StartNextKlonsVerURL();
    }

    private void StartNextKlonsVerURL()
    {
        var urls = KlonsF.Properties.Settings.Default.KlonsVerURLS;
        klonsverURLNr++;
        if (urls == null || urls.Count <= klonsverURLNr) return;
        aDownloader1.URL = urls[klonsverURLNr];
        if (aDownloader1.StartDownload()) return;
        StartNextKlonsVerURL();
    }

    private void CheckVersionNr(string ver)
    {
        MyData.Settings.LastVersionCheckDate = Utils.DateToString(DateTime.Today);
        string v = MyData.Version;
        if (string.Compare(v, ver) < 0)
        {
            ShowInfo("Ir pieejama jauna programmas versija.\n" +
                     "To var lejuplādēt interneta lapā www.klons.id.lv.");
        }
    }

    private void aDownloader1_DataReceived(object sender, EventArgs e)
    {
        string ver = aDownloader1.GetData();
        if (ver == null || ver.Length != 3) return;
        Invoke(new Action<string>(CheckVersionNr), ver);
    }

    private void aDownloader1_DownloadFailed(object sender, EventArgs e)
    {
        StartNextKlonsVerURL();
    }

    private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!CloseAllForms())
        {
            e.Cancel = true;
            return;
        }
        try
        {
            MyData.DbContextF?.Save();
        }
        catch (Exception) { }

        try
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                MyData.Settings.WindowPos = "max";
            }
            else
            {
                MyData.Settings.WindowPos = "normal";
            }
            MyData.SaveSettings();
            FirebirdSql.Data.FirebirdClient.FbConnection.ClearAllPools();
        }
        catch (Exception) { }
    }

    protected override void OnMdiChildActivate(EventArgs e)
    {
        base.OnMdiChildActivate(e);
        miCloseMDIForm.Visible = ActiveMdiChild != null;
    }

    private void miCloseMDIForm_Click(object sender, EventArgs e)
    {
        if (ActiveMdiChild == null) return;
        ActiveMdiChild.Close();
    }

    public void LoadDataA()
    {
        var fs = new FormA_DataLoad();
        fs.ShowDialog(this);
    }

    public bool CheckDataA()
    {
        if (DataLoaderA.HasDataA() && DataLoaderA.DataLoaded) return true;
        DataLoaderA.LoadSomeData();
        if (DataLoaderA.HasDataA() && DataLoaderA.DataLoaded) return true;
        LoadDataA();
        return DataLoaderA.HasDataA() && DataLoaderA.DataLoaded;
    }

    public bool CheckDataP()
    {
        DataLoaderP.CheckData();
        return DataLoaderP.HasData();
    }

    private bool AskBeforeDataReload()
    {
        if (!CloseAllForms()) return false;
        var rt = MyMessageBox.Show("Esošie dati tiks izmesti un ielādēti no jauna.\n"
            + "Vai turpināt?", "Klons", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, this);
        return rt == DialogResult.Yes;
    }


    #region ============== ShowForm___ ==============

    public void ShowFormSettings()
    {
        var fs = new Form_Settings();
        fs.ShowDialog(this);
    }
    public void ShowFormUsersDialog()
    {
        var f = new Form_Users();
        f.ShowDialog(this);
    }
    public Form_OpsFilter ShowFormOPSFilter()
    {
        return ShowForm<Form_OpsFilter>();
    }
    public Form_Docs ShowFormDocs()
    {
        return ShowForm<Form_Docs>();
    }
    public Form_Docs FindFormDocs()
    {
        return FindForm(typeof(Form_Docs)) as Form_Docs;
    }
    public void ShowFormPersons()
    {
        var f = ShowForm<Form_Persons>();
    }
    public void ShowFormPersonsDialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_Persons>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormAcplan()
    {
        var f = ShowForm<Form_AcPlan>();
    }
    public void ShowFormAcplanDialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_AcPlan>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormAcp3()
    {
        var f = ShowForm<Form_AcP3>();
    }
    public void ShowFormAcp3Dialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_AcP3>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormAcp4()
    {
        var f = ShowForm<Form_AcP4>();
    }
    public void ShowFormAcp4Dialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_AcP4>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormAcp5()
    {
        var f = ShowForm<Form_AcPVN>();
    }
    public void ShowFormAcp5Dialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_AcPVN>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormAcPVN()
    {
        var f = ShowForm<Form_AcPVN>();
    }
    public void ShowFormBal0()
    {
        var f = ShowForm<Form_Bal0>();
    }
    public void ShowFormDocTyp()
    {
        var f = ShowForm<Form_DocTyp>();
    }
    public void ShowFormDocTypDialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_DocTyp>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }
    public void ShowFormDocs0()
    {
        var f = ShowForm<Form_Docs0>();
    }
    public void ShowFormOpsRules()
    {
        var f = ShowForm<Form_OpsRules>();
    }
    public void ShowFormOpsRulesDialog(Action<string> onselectedvalue)
    {
        var f = ShowFormDialog<Form_OpsRules>();
        if (f == null) return;
        f.OnSelectedValueStr = onselectedvalue;
    }


    private bool BetaMWarningShow = false;

    private void WarnBetaM()
    {
        if (!MyData.Settings.DontShowBetaWarning && !BetaMWarningShow)
        {
            ShowWarning("Darbs pie programmas noliktavas moduļa turpinās. " +
                "Tas tiek piedāvāts testēšanas nolūkiem. " +
                "Lietotāji tiek aicināti izmēģināt noliktavas moduļa iespējas " +
                "un sūtīt savus ieteikumus tā uzlabojumiem un papildinājumiem.");
            BetaMWarningShow = true;
        }
    }
    public T ShowFormM<T>() where T : MyFormBaseF, new()
    {
        bool loaded = DataLoaderM.CheckLoad();
        if (!loaded) return null;
        WarnBetaM();
        var f = ShowForm<T>() as MyFormBaseF;
        return (T)f;
    }
    public T ShowFormM<T>(Func<T> func_createform) where T : MyFormBaseF
    {
        bool loaded = DataLoaderM.CheckLoad();
        if (!loaded) return null;
        WarnBetaM();
        var f = ShowForm(func_createform);
        return f;
    }
    public T ShowFormMDialog<T>() where T : MyFormBaseF, new()
    {
        bool loaded = DataLoaderM.CheckLoad();
        if (!loaded) return null;
        var f = ShowFormDialog<T>() as MyFormBaseF;
        if (f == null) return null;
        return (T)f;
    }
    public T ShowFormMDialogO<T>(Action<object> onselectedobject) where T : MyFormBaseF, new()
    {
        bool loaded = DataLoaderM.CheckLoad();
        if (!loaded) return null;
        var f = ShowFormDialog<T>() as MyFormBaseF;
        if (f == null) return null;
        f.OnSelectedObject = onselectedobject;
        return (T)f;
    }


    public T ShowFormA<T>() where T : MyFormBaseF, new()
    {
        if (!CheckDataA()) return null;
        var f = ShowForm<T>();
        return f;
    }

    public T ShowFormA<T>(Func<T> func_createform) where T : MyFormBaseF, new()
    {
        if (!CheckDataA()) return null;
        var f = ShowForm(func_createform);
        return f;
    }

    public T ShowFormP<T>() where T : MyFormBaseF, new()
    {
        if (!CheckDataP()) return null;
        var f = ShowForm<T>();
        return f;
    }

    #endregion


    #region ============ Menu Clicks System ==============

    private void miNomainītSaimniecību_Click(object sender, EventArgs e)
    {
        ChangeDB();

    }
    private void miKāStrādāsim_Click(object sender, EventArgs e)
    {
        if (!CloseAllForms()) return;
        ShowFormSettings();
    }
    private void miShowWindowList_Click(object sender, EventArgs e)
    {
        bool b = !miShowWindowList.Checked;
        miShowWindowList.Checked = b;
        MyData.Settings.ShowWindowListToolStrip = b;
        tsWindowList.Visible = b;
    }
    private void miFDokumentuLabojumi_Click(object sender, EventArgs e)
    {
        ShowForm<Form_LOPSd>();
    }
    private void miFKontējumuLabojumi_Click(object sender, EventArgs e)
    {
        ShowForm<Form_LOPS>();
    }
    private void miFMeklētIzmaiņas_Click(object sender, EventArgs e)
    {
        ShowForm<Form_LogDiff>();
    }
    private void miFDatuEksports_Click(object sender, EventArgs e)
    {
        if (!CloseAllForms()) return;
        ShowFormDialog<Form_Export>();
    }
    private void miFDatuImports_Click(object sender, EventArgs e)
    {
        if (!CloseAllForms()) return;
        ShowFormDialog<Form_Import>();
    }
    private void miAtvērtProgrammasMapi_Click(object sender, EventArgs e)
    {
        var myfolder = KlonsData.GetBasePath();
        try { Process.Start("explorer.exe", myfolder); } catch (Exception) { }
    }
    private void miAtvērtRezervesKopijuMapi_Click(object sender, EventArgs e)
    {
        var myfolder = MyData.GetBackUpFolder();
        if (myfolder.IsNOE()) return;
        try { Process.Start("explorer.exe", myfolder); } catch (Exception) { }
    }
    private void miReloadDataF_Click(object sender, EventArgs e)
    {
        if (!AskBeforeDataReload()) return;
        DataLoaderF.ReLoadData();
    }
    private void miReloadDataA_Click(object sender, EventArgs e)
    {
        if (!AskBeforeDataReload()) return;
        DataLoaderA.ReLoadData();
    }
    private void miReloadDataP_Click(object sender, EventArgs e)
    {
        if (!AskBeforeDataReload()) return;
        DataLoaderP.ReLoadData();
    }
    private void miReloadDataM_Click(object sender, EventArgs e)
    {
        if (!AskBeforeDataReload()) return;
        MyData.RecreateDbContextM();
        DataLoaderM.FillAll();
    }
    private void miAizvērt_Click(object sender, EventArgs e)
    {
        if (!CloseAllForms()) return;
        Application.Exit();
    }
    private void miZiņasParUzņemumu_Click(object sender, EventArgs e)
    {
        var f = ShowForm<Form_CompanyData>();
    }


    #endregion


    #region ============ Menu Clicks F ==============

    private void miFDokumenti_Click(object sender, EventArgs e)
    {
        ShowFormDocs();
    }
    private void miFIeraksti_Click(object sender, EventArgs e)
    {
        ShowFormOPSFilter();
    }
    private void miFSākumaAtlikumi_Click(object sender, EventArgs e)
    {
        ShowFormBal0();
    }
    private void miFNeapmaksātieRēķini_Click(object sender, EventArgs e)
    {
        ShowFormDocs0();
    }
    private void miFKontuPlāns_Click(object sender, EventArgs e)
    {
        ShowFormAcplan();
    }

    private void miFPVNPazīmes_Click(object sender, EventArgs e)
    {
        ShowFormAcPVN();
    }

    private void miFKontējumaPazīmes_Click(object sender, EventArgs e)
    {
        ShowFormAcp4();
    }
    private void miFDokumentuVeidi_Click(object sender, EventArgs e)
    {
        ShowFormDocTyp();
    }
    private void miFPersonas_Click(object sender, EventArgs e)
    {
        ShowFormPersons();
    }
    private void miFBankas_Click(object sender, EventArgs e)
    {
        ShowForm<Form_Bankas>();
    }
    private void miFValūtuKursi_Click(object sender, EventArgs e)
    {
        ShowForm<Form_Currency>();
    }
    private void miFKontējumuKontrole_Click(object sender, EventArgs e)
    {
        ShowFormOpsRulesDialog(null);
    }

    #endregion


    #region ============ Menu Clicks F Reports ==============

    private void form1ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var f = new Form_SomeTools();
        f.Owner = this;
        f.Show();
    }

    private void miFPVNKopsavilkums_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_PVNKops>();
    }
    private void miFPVNDeklarācija_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_PVNDekl>();
    }
    private void miFŽurnāls_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_PVNZ1>();
    }
    private void miFPVNSummuKontrole_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_PVNCheck>();
    }
    private void miFAvansaNorēķins_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_AvNor>();
    }
    private void miFBilance_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Apgr1>();
    }
    private void miFPaMēnešiem_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_ApgrMT>();
    }
    private void miFNaudasPlūsma_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_ApgrNP>();
    }
    private void miFDarijumuŽurnāls_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_ApgrDZ>();
    }
    private void miFPilnais_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_ApgrFull>();
    }
    private void miFKontuKorespondence_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Koresp>();
    }
    private void miFValūtasKontuAtskaites_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Currency>();
    }
    private void miFDarijumuŽurnāls1_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Darz1>();
    }
    private void miFNaudasPlūsma1_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_NPMT>();
    }
    private void miFKasesGrāmata_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_KasesGr>();
    }
    private void miFPersonuPārskats_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Persons>();
    }
    private void miFSkaidrasNaudasDarijumi_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_SkaidraNauda>();
    }
    private void miFBilanceFormulas_Click(object sender, EventArgs e)
    {
        ShowForm<Form_BilancesFormula>();
    }
    private void miFBilaneAtskaite_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_Bilance>();
    }
    private void miFEdsTp_Click(object sender, EventArgs e)
    {
        ShowForm<FormRep_EdsTp>();
    }

    #endregion


    #region ============ Menu Clicks P ==============

    private void miPPamatlīdzekļi_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_Items>();
    }

    private void miPNotikumi_Click(object sender, System.EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_Events>();
    }

    private void miPTaxDeprecYR_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_TaxDeprecYR>();
    }

    private void miPPamatlīdzekļuKategorijas_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_Cat1>();
    }

    private void miPNolietojumaKategorijas_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_CatD>();
    }

    private void miPNolietojumaKategorijasNodokļuVajadzībām_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_CatT>();
    }

    private void miPStruktūrvienības_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_Departments>();
    }

    private void miPAtrašanāsVietas_Click(object sender, EventArgs e)
    {
        ShowFormP<KlonsP.Forms.FormP_Places>();
    }

    private void miPKustībasPārskats_Click(object sender, EventArgs e)
    {
        if (!CheckDataP()) return;
        ShowFormDialog<KlonsP.Forms.FormPRep_Movement>();
    }

    private void miPNnolietojumsNodokļuVajadzībāmPaKategorijām_Click(object sender, EventArgs e)
    {
        if (!CheckDataP()) return;
        ShowFormDialog<KlonsP.Forms.FormP_TaxDeprecCat>();
    }

    #endregion


    #region ============ Menu Clicks A ==============
    private void miAAtlasītDatus_Click(object sender, EventArgs e)
    {
        if (!CloseAllForms()) return;
        LoadDataA();
    }
    private void miRepStats_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Stats>();
    }
    private void miADarbiniekuDati_Click(object sender, EventArgs e)
    {
        if (!CheckDataA()) return;
        if (MyData.DbContextA.BL_A_PERSONS.Count == 0)
        {
            ShowWarning("Darbinieku saraksts ir tukšs.");
            return;
        }
        ShowForm(() => new FormA_PersonsR());
    }
    private void miADarbinieki1_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Persons>();
    }
    private void miNeapliekamaisMinimums_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_UntaxedMinimum>();
    }
    private void miAIzmaiņasNodokļuMaksātājaGrāmatiņāsnoEDS_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Persons_Egr>();
    }

    private void miADarbaNespējasLapas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Persons_DN_lapas>();
    }
    private void miANotikumuIzklāsts_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Events>();
    }
    private void miAstrādājošoPārskats_Click(object sender, EventArgs e)
    {
        ShowFormA<FormARep_ActivePersons>();
    }
    private void miANeizmantotāsAtvaļinājumaDienas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormARep_VacDays>();
    }
    private void miAStruktūrvienības_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Departments>();
    }
    private void miADatiParPerioduPirmsUzskaitesSākšanas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PastData>();
    }
    private void miADarbiniekaDarbaLaikaUzskaitesDati_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_TimeSheet_Person>();
    }
    private void miADarbaLaikaPlānuSaraksts_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PlanList>();
    }
    private void miADarbaLaikaPlāns_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Plan>();
    }
    private void miADarbaLaikaUzskaitesLapuŠabloni_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_TimeSheetTempl>();
    }
    private void miADarbaLaikaUzskaitesLapas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_TimeSheets>();
    }
    private void miADarbaLaikaUzskaitesLapa_Click(object sender, EventArgs e)
    {
        ShowFormA(() => new FormA_TimeSheet());
    }
    private void miADarbaUzskaite_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PieceWork>();
    }
    private void miAKkatalogs_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PieceWorkCatalog>();
    }
    private void miAKatalogaStruktūra_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PieceWorkCatStruct>();
    }
    private void miAAlgasAprēķinaLapuŠabloni_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_SalarySheetTempl>();
    }
    private void miAAlgasAprēķinaLapuSaraksts_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_SalarySheets>();
    }
    private void miAAlgasAprēķinaLapas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_SalarySheet>();
    }
    private void miAPārskatsPaPersonām_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PaymentsByPerson>();
    }
    private void miASarakstuŠabloni_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PayListsTempl>();
    }
    private void miAMakasājumuSaraksti1_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_PayLists>();
    }
    private void miAPersonuSaraksts_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_FizPersons>();
    }
    private void miAMaksājumi_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_FpPayLists>();
    }
    private void miAPapildusNotikumuKodi_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_EventTypes2>();
    }
    private void miALikmes_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Rates>();
    }
    private void miATeritoriālieKodi_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_TeritorialCodes>();
    }
    private void miAProfesijuKlasifikators_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Professions>();
    }
    private void miAZiņuKodi_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_ReportCodes>();
    }
    private void miAIenākumuVeidi_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_IncomeCodes>();
    }
    private void miASvētkuDienas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Holidays>();
    }
    private void miAKalendārs_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Calendar>();
    }
    private void miABankas_Click(object sender, EventArgs e)
    {
        ShowFormA<FormA_Banks>();
    }

    private void miAKopsavilkums_Click(object sender, EventArgs e)
    {
        MyData.ReportHelperA.WarnIfHasChanges();
        ShowFormA<FormARep_Aggregate>();
    }

    private void miAZiņasParDarbaŅēmējiem_Click(object sender, EventArgs e)
    {
        MyData.ReportHelperA.WarnIfHasChanges();
        ShowFormA<FormARep_Zinas>();
    }

    private void miAZiņojumsParVSAOI_Click(object sender, EventArgs e)
    {
        MyData.ReportHelperA.WarnIfHasChanges();
        ShowFormA<FormARep_VSAOI>();
    }

    private void miAPaziņojumsParFiziskajāmPersonāmIzmaksātajāmSummāmkopsavilkums_Click(object sender, EventArgs e)
    {
        MyData.ReportHelperA.WarnIfHasChanges();
        ShowFormA<FormARep_IINK>();
    }

    #endregion


    #region ============ Menu Clicks M ==============
    private void miMDokumenti_Click(object sender, EventArgs e)
    {
        ShowFormM(() => new FormM_DocList());
    }
    private void miMNoliktavasPartneri_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_Stores>();
    }
    private void miMArtikuli_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_Items>();
    }
    private void miMArtikuluKategorijas_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_ItemsCat>();
    }
    private void miCenuLapuSaraksts_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_PriceLists>();
    }
    private void miCenuLapa_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_PriceList>();
    }
    private void miAtlaižuLapuSaraksts_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_DiscountLists>();
    }
    private void miAtlaižuLapa_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_DiscountList>();
    }
    private void miMInventarizācijasDokumenti_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_InvDocList>();
    }
    private void miMArtikulaKustībasPārskats_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_ItemMovement>();
    }
    private void miMIzlietojumaPārskats_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepItemLinks>();
    }
    private void miMKustībaPaArtikuliem_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepItemLinks>();
    }
    private void miMKustībaPaArtikuluKategorijām_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepMovementPerItemsCat>();
    }
    private void miMKustībaPaPiegādātājiem_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepMovementPerSupplier>();
    }
    private void miMRealizācijasPašizmaksa_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepAccCosts>();
    }
    private void miMRealizācijasPašizmaksaPaDokumentiem_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_RepAccCostsPerDoc>();
    }
    private void miMPilnsPārrēķins_Click(object sender, EventArgs e)
    {
        KlonsM.Classes.DataLoaderM.CheckLoad();
        KlonsM.Classes.DataTasksM.FullRecalc();
    }
    private void miMAtlikumuPārrēķins_Click(object sender, EventArgs e)
    {
        KlonsM.Classes.DataLoaderM.CheckLoad();
        KlonsM.Classes.DataTasksM.RecalcAmounts();
    }
    private void miMIsGonePārrēķins_Click(object sender, EventArgs e)
    {
        KlonsM.Classes.DataLoaderM.CheckLoad();
        KlonsM.Classes.DataTasksM.RecalcIsGone(true);
    }
    private void miMKontuPlāns_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_Accounts>();
    }
    private void miMDarijumuVeidi_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_TransactionType>();
    }
    private void miMNorēķinuVeidi_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_PaymentTypes>();
    }
    private void miPartneruKategorijas_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_StoresCat>();
    }
    private void miMValstis_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_Countries>();
    }
    private void miMBankas_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_Banks>();
    }
    private void miMKontēšanasShēma_Click(object sender, EventArgs e)
    {
        ShowFormM<FormM_PvnRates2>();
    }
    private void miMPVNAprēķinaAtsauces_Click(object sender, EventArgs e)
    {
        KlonsM.Classes.DataLoaderM.CheckLoad();
        var fm = new FormM_PVNTexts();
        fm.ShowDialog();
    }
    private void miMParams_Click(object sender, EventArgs e)
    {
        KlonsM.Classes.DataLoaderM.CheckLoad();
        var fm = new FormM_Params();
        fm.ShowDialog();
    }

    #endregion


    #region ============ Menu Clicks Info ==============
    
    private void miFApraksts_Click(object sender, EventArgs e)
    {
        var myfolder = MyData.GetManualsPath();
        try { Process.Start("explorer.exe", myfolder); } catch (Exception) { }
    }
    private void miParProgrammu_Click(object sender, EventArgs e)
    {
        new Form_About().ShowDialog(MyMainForm);
    }


    #endregion

}

