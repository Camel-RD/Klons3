using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsF.Classes
{
    [Serializable()] 
    public record KlonsSettings: IKlonsSettings
    {
        private MasterEntry masterEntry = new MasterEntry();
        private MyColorTheme colorTheme = null;
        private string colorThemeId = "system";
        private int formFontSize = 10;
        private string formFontName = "";
        private int formFontStyle = 0;
        private Font formFont = null;

        public MasterEntry MasterEntry
        {
            get { return masterEntry; }
            set
            {
                if (value == null)
                {
                    masterEntry = new MasterEntry();
                    return;
                }
                if (masterEntry.Equals(value)) return;
                masterEntry = value;
            }
        }

        public string LastUserName { get; set; } = "";
        public string BaseDBPath { get; set; } = "DB";
        public string BaseConnStr { get;set; } = "FBServer";
        public string DoVersionCheck { get; set; } = "NO";
        public string InWine { get; set; } = "NO";
        public string LastVersionCheckDate { get; set; } = "";

        [XmlIgnore]
        public MyColorTheme ColorTheme
        {
            get
            {
                if (colorTheme == null)
                {
                    colorTheme = ColorThemeHelper.ColorTheme_System;
                }
                return colorTheme;
            }
        }

        public string ColorThemeId
        {
            get { return colorThemeId; }
            set
            {
                if (colorThemeId == value) return;
                colorThemeId = value;
                colorTheme = null;
                switch (value)
                {
                    case "system":
                        colorTheme = ColorThemeHelper.ColorTheme_System;
                        break;
                    case "dark1":
                        colorTheme = ColorThemeHelper.ColorTheme_Dark1;
                        break;
                    case "green":
                        colorTheme = ColorThemeHelper.ColorTheme_Green;
                        break;
                    case "blackonwhite":
                        colorTheme = ColorThemeHelper.ColorTheme_BlackOnWhite;
                        break;

                }
                if (colorTheme == null)
                {
                    colorTheme = ColorThemeHelper.ColorTheme_System;
                    colorThemeId = "system";

                }
            }
        }

        public int FormFontSize
        {
            get { return formFontSize; }
            set
            {
                if (formFontSize == value) return;
                formFontSize = value;
                CheckFont();
            }
        }

        public string FormFontName
        {
            get { return formFontName; }
            set
            {
                if (formFontName == value) return;
                formFontName = value;
                CheckFont();
            }
        }
        public int FormFontStyle
        {
            get { return formFontStyle; }
            set
            {
                if (formFontStyle == value) return;
                FormFontStyle = value;
                CheckFont();
            }
        }

        private void CheckFont()
        {
            if (formFont != null)
            {
                if (formFont.Name == formFontName &&
                    formFont.FontSizeX() == formFontSize &&
                    formFont.Style == (FontStyle) formFontStyle)
                    return;
            }
            string fnm = formFontName;
            if (string.IsNullOrEmpty(fnm))
            {
                fnm = SystemFonts.DefaultFont.Name;
            }
            FontStyle fs = (FontStyle) formFontStyle;
            formFont = new Font(fnm, formFontSize, fs);
        }

        [XmlIgnore]
        public Font FormFont
        {
            get
            {
                CheckFont();
                return formFont;
            }
            set
            {
                if(value == null) return;
                if (formFont != null)
                {
                    if (formFont.Name == value.Name &&
                        formFont.FontSizeX() == value.SizeInPoints &&
                        formFont.Style == value.Style)
                        return;
                }
                formFontName = value.Name;
                formFontSize = value.FontSizeX();
                formFontStyle = (int)value.Style;
                formFont = new Font(formFontName, formFontSize, (FontStyle)formFontStyle);
            }
        }

        public int OpsTableHeight { get; set; } = 0;
        public string ColumnWidths_Docs2 { get; set; } = "";
        public string ColumnWidths_Ops2 { get; set; } = "";
        public string ColumnWidths_Dops2 { get; set; } = "";
        public string ColumnWidths_SalarySheet { get; set; } = "";
        public string ColumnWidths_SalaryBonus { get; set; } = "";
        public string ColumnWidths_AEvents { get; set; } = "";
        public string ColumnWidths_PItems { get; set; } = "";
        public string ColumnWidths_PEvents { get; set; } = "";
        public string ColumnWidths_MDocs { get; set; } = "";
        public string ColumnWidths_MDocsFilter { get; set; } = "";
        public string ColumnWidths_MDoc { get; set; } = "";
        public string ColumnWidths_MStores { get; set; } = "";
        public string ColumnWidths_MItems { get; set; } = "";
        public int ReportZoom { get; set; } = 100;
        public string WindowPos { get; set; } = "normal";
        public int BackUpPlan { get; set; } = 1;

        [XmlIgnore]
        public EBackUpPlan BackUpPlanX
        {
            get
            {
                if (BackUpPlan < 0 || BackUpPlan > 3) BackUpPlan = 1;
                return (EBackUpPlan)BackUpPlan;
            }
            set
            {
                BackUpPlan = (int)value;
            }
        }

        public string BackUpFolder { get; set; } = "";
        public bool OpsShowFullPersonName { get; set; } = false;
        public bool OpsShowPersonsRegNum { get; set; } = false;
        public bool OpsShowPersonsPVNRegNum { get; set; } = false;
        public bool ShowPersonList { get; set; } = false;
        public bool ShowPayDataPanel { get; set; } = false;
        public bool ShowPayLists { get; set; } = false;
        public bool ShowPayListFilterPanel { get; set; } = false;
        public bool ShowFpPayDataPanel { get; set; } = false;
        public bool ShowFpPayLists { get; set; } = false;
        public bool DontShowBetaWarning { get; set; } = false;
        public bool ShowWindowListToolStrip { get; set; } = true;
        public bool SpecialFeatures { get; set; } = false;

        public static KlonsSettings LoadSettings(string filename)
        {
            var rt = Utils.LoadDataFromXML<KlonsSettings>(filename);
            if (rt != null) return rt;
            rt = new KlonsSettings();
            return rt;
        }

        public bool SaveSettings(string filename)
        {
            return Utils.SaveDataToXML(this, filename);
        }

    }

    public enum EBackUpPlan
    {
        Never, WhenUpgrading, Daily, WhenConnecting
    }

}
