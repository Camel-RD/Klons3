﻿using System.Xml.Serialization;

namespace KlonsA.Classes
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class nm_e_gramatinas
    {

        private nm_e_gramatinasPamatdati pamatdatiField;

        private nm_e_gramatinasGigv[] gigvField;

        /// <remarks/>
        public nm_e_gramatinasPamatdati pamatdati
        {
            get
            {
                return this.pamatdatiField;
            }
            set
            {
                this.pamatdatiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("gigv")]
        public nm_e_gramatinasGigv[] gigv
        {
            get
            {
                return this.gigvField;
            }
            set
            {
                this.gigvField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasPamatdati
    {

        private string isais_nosaukField;

        private long nmr_kodsField;

        private sbyte gramatinasField;

        private string periods_noField;

        private string periods_lidzField;

        private string gramatinas_iesniedzejsField;

        private string parametriField;

        private System.DateTime izpildes_laiksField;

        /// <remarks/>
        public string isais_nosauk
        {
            get
            {
                return this.isais_nosaukField;
            }
            set
            {
                this.isais_nosaukField = value;
            }
        }

        /// <remarks/>
        public long nmr_kods
        {
            get
            {
                return this.nmr_kodsField;
            }
            set
            {
                this.nmr_kodsField = value;
            }
        }

        /// <remarks/>
        public sbyte gramatinas
        {
            get
            {
                return this.gramatinasField;
            }
            set
            {
                this.gramatinasField = value;
            }
        }

        /// <remarks/>
        public string periods_no
        {
            get
            {
                return this.periods_noField;
            }
            set
            {
                this.periods_noField = value;
            }
        }

        /// <remarks/>
        public string periods_lidz
        {
            get
            {
                return this.periods_lidzField;
            }
            set
            {
                this.periods_lidzField = value;
            }
        }

        /// <remarks/>
        public string gramatinas_iesniedzejs
        {
            get
            {
                return this.gramatinas_iesniedzejsField;
            }
            set
            {
                this.gramatinas_iesniedzejsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Parametri
        {
            get
            {
                return this.parametriField;
            }
            set
            {
                this.parametriField = value;
            }
        }

        /// <remarks/>
        public System.DateTime izpildes_laiks
        {
            get
            {
                return this.izpildes_laiksField;
            }
            set
            {
                this.izpildes_laiksField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigv
    {

        private long pers_kodsField;

        private string vards_uzvardsField;

        private string numurs_regField;

        private System.DateTime datums_izdField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.DateTime datums_ierField;

        private nm_e_gramatinasGigvApgadajamais[] apgadajamieField;

        private nm_e_gramatinasGigvPensijas pensijasField;

        private nm_e_gramatinasGigvPapildu_atvieglojums[] papildu_atvieglojumiField;

        private nm_e_gramatinasGigvPazime_progr_iin_likmes pazime_progr_iin_likmesField;

        private nm_e_gramatinasGigvPrognozetais_mnm[] prognozetie_mnmField;

        /// <remarks/>
        public long pers_kods
        {
            get
            {
                return this.pers_kodsField;
            }
            set
            {
                this.pers_kodsField = value;
            }
        }

        /// <remarks/>
        public string vards_uzvards
        {
            get
            {
                return this.vards_uzvardsField;
            }
            set
            {
                this.vards_uzvardsField = value;
            }
        }

        /// <remarks/>
        public string numurs_reg
        {
            get
            {
                return this.numurs_regField;
            }
            set
            {
                this.numurs_regField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_izd
        {
            get
            {
                return this.datums_izdField;
            }
            set
            {
                this.datums_izdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("apgadajamais", IsNullable = false)]
        public nm_e_gramatinasGigvApgadajamais[] apgadajamie
        {
            get
            {
                return this.apgadajamieField;
            }
            set
            {
                this.apgadajamieField = value;
            }
        }

        /// <remarks/>
        public nm_e_gramatinasGigvPensijas pensijas
        {
            get
            {
                return this.pensijasField;
            }
            set
            {
                this.pensijasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("papildu_atvieglojums", IsNullable = false)]
        public nm_e_gramatinasGigvPapildu_atvieglojums[] papildu_atvieglojumi
        {
            get
            {
                return this.papildu_atvieglojumiField;
            }
            set
            {
                this.papildu_atvieglojumiField = value;
            }
        }

        /// <remarks/>
        public nm_e_gramatinasGigvPazime_progr_iin_likmes pazime_progr_iin_likmes
        {
            get
            {
                return this.pazime_progr_iin_likmesField;
            }
            set
            {
                this.pazime_progr_iin_likmesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("prognozetais_mnm", IsNullable = false)]
        public nm_e_gramatinasGigvPrognozetais_mnm[] prognozetie_mnm
        {
            get
            {
                return this.prognozetie_mnmField;
            }
            set
            {
                this.prognozetie_mnmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvApgadajamais
    {

        private string vards_uzvardsField;

        private string veidsField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.DateTime datums_ierField;

        /// <remarks/>
        public string vards_uzvards
        {
            get
            {
                return this.vards_uzvardsField;
            }
            set
            {
                this.vards_uzvardsField = value;
            }
        }

        /// <remarks/>
        public string veids
        {
            get
            {
                return this.veidsField;
            }
            set
            {
                this.veidsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPensijas
    {

        private nm_e_gramatinasGigvPensijasPensija pensijaField;

        /// <remarks/>
        public nm_e_gramatinasGigvPensijasPensija pensija
        {
            get
            {
                return this.pensijaField;
            }
            set
            {
                this.pensijaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPensijasPensija
    {

        private string veidsField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.DateTime datums_ierField;

        /// <remarks/>
        public string veids
        {
            get
            {
                return this.veidsField;
            }
            set
            {
                this.veidsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPapildu_atvieglojums
    {

        private string veidsField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.DateTime datums_ierField;

        /// <remarks/>
        public string veids
        {
            get
            {
                return this.veidsField;
            }
            set
            {
                this.veidsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPazime_progr_iin_likmes
    {

        private nm_e_gramatinasGigvPazime_progr_iin_likmesPazime_progr_iin_likme pazime_progr_iin_likmeField;

        /// <remarks/>
        public nm_e_gramatinasGigvPazime_progr_iin_likmesPazime_progr_iin_likme pazime_progr_iin_likme
        {
            get
            {
                return this.pazime_progr_iin_likmeField;
            }
            set
            {
                this.pazime_progr_iin_likmeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPazime_progr_iin_likmesPazime_progr_iin_likme
    {

        private string veidsField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.DateTime datums_ierField;

        /// <remarks/>
        public string veids
        {
            get
            {
                return this.veidsField;
            }
            set
            {
                this.veidsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nm_e_gramatinasGigvPrognozetais_mnm
    {

        private string veidsField;

        private System.DateTime datums_noField;

        private System.Nullable<System.DateTime> datums_lidzField;

        private bool datums_lidzFieldSpecified;

        private System.Nullable<System.DateTime> datums_ierField;

        private bool datums_ierFieldSpecified;

        private float summaField;

        /// <remarks/>
        public string veids
        {
            get
            {
                return this.veidsField;
            }
            set
            {
                this.veidsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime datums_no
        {
            get
            {
                return this.datums_noField;
            }
            set
            {
                this.datums_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_lidz
        {
            get
            {
                return this.datums_lidzField;
            }
            set
            {
                this.datums_lidzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_lidzSpecified
        {
            get
            {
                return this.datums_lidzFieldSpecified;
            }
            set
            {
                this.datums_lidzFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> datums_ier
        {
            get
            {
                return this.datums_ierField;
            }
            set
            {
                this.datums_ierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool datums_ierSpecified
        {
            get
            {
                return this.datums_ierFieldSpecified;
            }
            set
            {
                this.datums_ierFieldSpecified = value;
            }
        }

        /// <remarks/>
        public float summa
        {
            get
            {
                return this.summaField;
            }
            set
            {
                this.summaField = value;
            }
        }
    }

}