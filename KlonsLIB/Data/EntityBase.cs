using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.Data;

public class EntityBase : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;
    public EntityBase Me => this;

    public virtual void OnPropertyChanged(string propertyName)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public virtual void OnPropertyChanging(string propertyName)
    {
        var propertyChanging = PropertyChanging;
        if (propertyChanging != null)
        {
            propertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
    }

    public virtual string GetInfoView()
    {
        return ToString();
    }

}
