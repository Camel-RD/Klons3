using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public class PaylistImportRow : INotifyPropertyChanged
{
    public int SBR { get; set; }
    public DateTime? Date { get; set; }
    public A_PERSONS Person { get; set; }
    public A_POSITIONS Position { get; set; }
    public string Name { get; set; } = null;
    public string RegNr { get; set; } = null;
    public decimal Amount { get; set; } = 0M;

    public event PropertyChangedEventHandler PropertyChanged;
}

