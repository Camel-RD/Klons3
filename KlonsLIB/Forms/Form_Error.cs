using System;
using System.Data;
using System.Windows.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace KlonsLIB.Forms
{
    public partial class Form_Error : MyFormBase
    {
        public Form_Error()
        {
            InitializeComponent();
            CheckMyFontSize();
            CheckMyFontAndColors();
        }

        public static void ShowException(Form owner, Exception e)
        {
            if (e == null) return;
            Form_Error fe = new Form_Error();
            fe.tbMsg.Text = e.Message.Nz().Replace("\n", "\r\n").Replace("\r\r", "\r");
            fe.tbDescr.Text = e.ToString();
            if (e is MyException myex && !myex.ExtraData.IsNOE())
            {
                var data = myex.ExtraData.Replace("\n", "\r\n").Replace("\r\r", "\r");
                fe.tbDescr.AppendText("\r\n\r\n" + data);
            }
            try
            {
                fe.ShowDialog(owner);
            }
            catch(Exception) { }
        }
        public static void ShowException(Form owner, Exception e, DataGridView dgv)
        {
            MyException e1 = ExceptionHelper.TranslateException(e, dgv);
            ShowException(owner, e1);
        }
        public static void ShowException(Form owner, Exception e, DataTable table)
        {
            MyException e1 = ExceptionHelper.TranslateException(e, table);
            ShowException(owner, e1);
        }
        public static void ShowException(Form owner, Exception e, string newmsg)
        {
            MyException e1 = new MyException(newmsg, e);
            ShowException(owner, e1);
        }
        public static void ShowException(Form owner, Exception e, string newmsg, DataGridView dgv)
        {
            MyException e1 = ExceptionHelper.TranslateException(e, dgv);
            if (!string.IsNullOrEmpty(newmsg))
            {
                e1 = new MyException(newmsg, e1);
            }
            ShowException(owner, e1);
        }
        public static void ShowException(Form owner, Exception e, string newmsg, DataTable table)
        {
            MyException e1 = ExceptionHelper.TranslateException(e, table);
            if (!string.IsNullOrEmpty(newmsg))
            {
                e1 = new MyException(newmsg, e1);
            }
            ShowException(owner, e1);
        }

        public static void ShowException(Exception e)
        {
            ShowException(MyMainForm, e);
        }
        public static void ShowException(Exception e, DataGridView dgv)
        {
            ShowException(MyMainForm, e, dgv);
        }
        public static void ShowException(Exception e, DataTable table)
        {
            ShowException(MyMainForm, e, table);
        }
        public static void ShowException(Exception e, MyBindingSourceEf bs)
        {
            if (bs == null)
            {
                ShowException(MyMainForm, e);
            }
            else if (bs.UseDataGridView != null)
            {
                ShowException(MyMainForm, e, bs.UseDataGridView);
            }
            else
            {
                ShowException(MyMainForm, e);
            }
        }
        public static void ShowException(Exception e, MyBindingSourceEf2 bs)
        {
            if (bs == null)
            {
                ShowException(MyMainForm, e);
            }
            else if (bs.UseDataGridView != null)
            {
                ShowException(MyMainForm, e, bs.UseDataGridView);
            }
            else
            {
                ShowException(MyMainForm, e);
            }
        }
        public static void ShowException(Exception e, string newmsg)
        {
            ShowException(MyMainForm, e, newmsg);
        }
        public static void ShowException(Exception e, string newmsg, DataGridView dgv)
        {
            ShowException(MyMainForm, e, newmsg, dgv);
        }
        public static void ShowException(Exception e, string newmsg, DataTable table)
        {
            ShowException(MyMainForm, e, newmsg, table);
        }


        private void cmClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FormError_Load(object sender, EventArgs e)
        {
            tbDescr.Visible = false;
            Height = tbDescr.Top + (Height - ClientSize.Height);
        }

        private void cmDetails_Click(object sender, EventArgs e)
        {
            if (tbDescr.Visible)
            {
                tbDescr.Visible = false;
                Height = tbDescr.Top + (Height - ClientSize.Height);
            }
            else
            {
                Height = tbDescr.Bottom + 10 + (Height - ClientSize.Height); 
                tbDescr.Visible = true;
            }
        }
    }
    public class MyException : Exception
    {
        public string ExtraData { get; set; } = null;
        public MyException(string message)
            : base(message)
        {
        }
        public MyException(string message, Exception innerexception)
            : base(message.Zn() + Environment.NewLine, innerexception)
        {
        }
    }

}
