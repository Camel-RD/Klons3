using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using FirebirdSql.Data.FirebirdClient;

namespace KlonsF.Forms
{
    public partial class Form_Import : MyFormBaseF
    {
        public Form_Import()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            var kv = new[] {
                new { key = 0, val = "neko" },
                new { key = 1, val = "jaunos" },
                new { key = 2, val = "visus" }
            };
            dgcTask.DataSource = kv;
            dgcTask.ValueMember = "key";
            dgcTask.DisplayMember = "val";
        }

        public List<CellError> ErrorList = null;
        public WorkBookConfig WorkBookConfig = null;
        public string FileName = null;

        private void Form_Import_Load(object sender, EventArgs e)
        {
            dgvCount.AutoGenerateColumns = false;
        }

        public void GetCounts()
        {
            WorkBookConfig.GetCount();
            bsCount.DataSource = WorkBookConfig.SheetsConfig;
        }

        public void DoTestRun()
        {

            FileStream fs = null;
            try
            {
                fs = new FileStream(FileName, FileMode.Open);
            }
            catch (Exception)
            {
                MyMainForm.ShowWarning("Neizdevās atvērt failu:\n" + FileName);
                return;
            }


            if (WorkBookConfig != null)
            {
                bsCount.DataSource = null;
                WorkBookConfig.WB.Close();
                WorkBookConfig.Stream.Close();
            }

            WorkBookConfig = new WorkBookConfig(fs);
            int rowscount = WorkBookConfig.GetFirstCount();
            ErrorList = null;

            var fmpr = new Form_Progress();
            fmpr.Message = "Pārbaudam XLS failu ...";
            fmpr.MaxProgress = rowscount;


            WorkBookConfig.ProgressChanged += (sender, arg) =>
            {
                fmpr.Progress = WorkBookConfig.RowsDone;
            };

            fmpr.OnCancel = () =>
            {
                WorkBookConfig.Cancel = true;
            };

            fmpr.RunThis = () =>
            {
                Task.Factory.StartNew(() =>
                {
                    ErrorList = WorkBookConfig.TestXLS();

                }).ContinueWith((t) => {

                    fmpr.Close();

                    GetCounts();
                    SetSkipDocs(false);

                    if (WorkBookConfig.Cancel)
                    {
                        MyMainForm.ShowInfo("XLS faila pārbaude pātraukts.");
                    }
                    else if (t.IsFaulted)
                    {
                        var me = new MyException("XLS faila pārbaude neizdevās.", t.Exception);
                        Form_Error.ShowException(me);
                    }
                    else
                    {
                        MyMainForm.ShowInfo("XLS faila pārbaude pabeigta.");
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            };

            fmpr.ShowDialog(MyMainForm);
        }

        public void DoImport()
        {
            var fmpr = new Form_Progress();
            fmpr.Message = "Importējam datus ...";
            fmpr.MaxProgress = WorkBookConfig.RowCount;

            WorkBookConfig.ProgressChanged += (sender, arg) =>
            {
                fmpr.Progress = WorkBookConfig.RowsDone;
            };

            fmpr.OnCancel = () =>
            {
                WorkBookConfig.Cancel = true;
            };

            string er = null;

            fmpr.RunThis = () =>
            {
                Task.Factory.StartNew(() =>
                {
                    er = WorkBookConfig.ImportXLS();
                    if (er != "OK") return;

                    fmpr.Message = "Saglabājam izmaiņas ...";
                    WorkBookConfig.RowsDone = 0;
                    fmpr.Progress = 0;
                    fmpr.MaxProgress = WorkBookConfig.GetUpdateCount();
                    SaveAllData();

                }).ContinueWith((t) => {
                    fmpr.Close();
                    if (WorkBookConfig.Cancel)
                    {
                        MyMainForm.ShowInfo("Datu imports pātraukts.");
                        RejectChanges();
                    }
                    else if (t.IsFaulted)
                    {
                        var me = new MyException("Datu imports neizdevās.", t.Exception);
                        Form_Error.ShowException(me);
                        RejectChanges();
                    }
                    else if(er != "OK")
                    {
                        var me = new MyException("Datu imports neizdevās.", new Exception(er));
                        Form_Error.ShowException(me);
                        RejectChanges();
                    }
                    else
                    {
                        MyMainForm.ShowInfo("Datu imports pabeigts.");
                        WorkBookConfig.ImportDone = true;
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            };

            fmpr.ShowDialog(MyMainForm);
        }

        public void RejectChanges()
        {
            MyData.DbContextF.RejectChanges();
        }

        public bool SaveAllData()
        {
            try
            {
                MyData.DbContextF.Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        private void cmImport_Click(object sender, EventArgs e)
        {
            if(WorkBookConfig == null)
            {
                MyMainForm.ShowInfo("Nav norādīts importējamais XLS fails.");
                return;
            }
            if (WorkBookConfig.ImportDone)
            {
                MyMainForm.ShowInfo("Dati tika jau importēti.");
                return;
            }
            WorkBookConfig.GetCount();
            if (WorkBookConfig.RowCountBad > 0)
            {
                MyMainForm.ShowInfo("Norādītajā XLS failā tika atrastas kļūdas.");
                return;
            }
            DoImport();
        }

        public void SetSkipDocs(bool skip)
        {
            for (int i = 1; i < WorkBookConfig.SheetsConfig.Count; i++)
            {
                WorkBookConfig.SheetsConfig[i].Task = ESheetTask.AddNew;
            }
            if (skip)
            {
                WorkBookConfig.SheetsConfig[0].Task = ESheetTask.Ignore;
                var kv = new[]{
                    new { key = ESheetTask.Ignore, val = "neko" },
                    new { key = ESheetTask.AddNew, val = "jaunos" },
                    new { key = ESheetTask.DoAll, val = "visus" }
                };
                dgcTask.DataSource = kv;
                dgcTask.ValueMember = "key";
                dgcTask.DisplayMember = "val";

            }
            else
            {
                WorkBookConfig.SheetsConfig[0].Task = ESheetTask.AddNew;
                var kv = new[] {
                    new { key = ESheetTask.AddNew, val = "jaunos" },
                    new { key = ESheetTask.DoAll, val = "visus" }
                };
                dgcTask.DataSource = kv;
                dgcTask.ValueMember = "key";
                dgcTask.DisplayMember = "val";
            }

            IgnoreSkipDocsEvent = true;
            if(chSkipDocs.Checked != skip) chSkipDocs.Checked = skip;
            IgnoreSkipDocsEvent = false;
        }

        private bool IgnoreSkipDocsEvent = false;
        private void chSkipDocs_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoading || IgnoreSkipDocsEvent || WorkBookConfig == null) return;
            SetSkipDocs(chSkipDocs.Checked);
        }

        private void btGetFile_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Title = "Norādi importējamo XLS failu";
            fd.CheckFileExists = false;
            fd.Filter = "Excel files (*.xls)|*.xls";
            var rt = fd.ShowDialog(MyMainForm);
            if (rt != DialogResult.OK) return;
            FileName = fd.FileName;
            tbFileName.Text = fd.FileName;
            if (!File.Exists(FileName))
            {
                MyMainForm.ShowWarning("Fails nav atrasts.");
                return;
            }
            DoTestRun();
        }

        private void cmShowErrors_Click(object sender, EventArgs e)
        {
            if(ErrorList == null || ErrorList.Count == 0)
            {
                MyMainForm.ShowInfo("Kļūdu saraksts ir tukšs.");
                return;
            }
            var fm = MyMainForm.ShowFormDialog<Form_ImportErrors>();
            if (fm == null) return;
            fm.SetErrorList(ErrorList);
        }

        private void dgvCount_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.RowIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private void Form_Import_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WorkBookConfig != null)
            {
                bsCount.DataSource = null;
                WorkBookConfig.WB.Close();
                WorkBookConfig.Stream.Close();
            }
        }
    }


}