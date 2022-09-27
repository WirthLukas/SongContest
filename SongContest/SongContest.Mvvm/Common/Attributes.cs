using System;

namespace SongContest.Mvvm.Common
{
    /// <summary>
    /// Ein Property mit dem NoNotify Attribut wird von der <see cref="BaseViewModel.NotifyAllProperties"/>
    /// Methode der <see cref="BaseViewModel"/> Klasse ignoriert
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NoNotifyAttribute : Attribute { }

    /// <summary>
    /// Gibt dem Framework an, dass das markierte Property eines Viewmodels
    /// validiert werden soll <see cref="ValidateEntityViewModel{TEntity}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateAttribute : Attribute { }
}
