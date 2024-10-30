using System;
using System.Data;
using System.Windows.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static KlonsLIB.Forms.Form_Error2;

namespace KlonsLIB.Forms
{
    public partial class Form_Error2 : MyFormBase
    {
        public Form_Error2()
        {
            InitializeComponent();
            CheckMyFontSize();
            CheckMyFontAndColors();
        }

        public enum EFormErrorResult { None, DoRollBack}
        public enum EPromptState { None, CanRollBack, WillRollBack}
        public EFormErrorResult FormErrorResult { get; private set; } = EFormErrorResult.None;
        public EPromptState PromptState { get; private set; } = EPromptState.None;

        public static EFormErrorResult ShowException(Form owner, Exception e, EPromptState promptstate)
        {
            if (e == null) return EFormErrorResult.None;
            Form_Error2 fe = new Form_Error2();
            fe.PromptState = promptstate;
            if (promptstate == EPromptState.CanRollBack)
                fe.cmRollBack.Visible = true;
            fe.tbMsg.Text = e.Message;
            fe.tbDescr.Text = e.ToString();
            if (e is MyException myex && !myex.ExtraData.IsNOE())
            {
                var data = myex.ExtraData.Replace("\n", "\r\n").Replace("\r\r", "\r");
                fe.tbDescr.AppendText("\r\n\r\n" + data);
            }
            if (promptstate == EPromptState.WillRollBack)
                fe.tbMsg.AppendText("\r\n\r\n* Nesaglabātās izmaiņas tiks atceltas.");
            try
            {
                fe.ShowDialog(owner);
                return fe.FormErrorResult;
            }
            catch (Exception) { }
            return EFormErrorResult.None;
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

        private void cmRollBack_Click(object sender, EventArgs e)
        {
            FormErrorResult = EFormErrorResult.DoRollBack;
            DialogResult = DialogResult.OK;
        }
    }

}
