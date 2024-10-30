﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

namespace Cyotek.Windows.Forms
{
  public class TabListEventArgs : EventArgs
  {
    #region Constants

    private readonly TabListAction _action;

    private readonly TabListPage _tabPage;

    private readonly int _tabPageIndex;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TabListEventArgs"/> class.
    /// </summary>
    /// <param name="tabPage">The <see cref="TabListPage"/> the event is occurring for.</param>
    /// <param name="tabPageIndex">The zero-based index of <paramref name="tabPage"/> in the <see cref="TabList.TabListPages"/> collection..</param>
    /// <param name="action">One of the <see cref="TabListAction"/> values.</param>
    public TabListEventArgs(TabListPage tabPage, int tabPageIndex, TabListAction action)
    {
      _tabPage = tabPage;
      _tabPageIndex = tabPageIndex;
      _action = action;
    }

    #endregion

    #region Properties

    /// <summary>
    ///    Gets a value indicating which event is occurring.
    /// </summary>
    /// <value>
    /// One of the <see cref="TabListAction"/> values.
    /// </value>
    public TabListAction Action
    {
      get { return _action; }
    }

    /// <summary>
    /// Gets the <see cref="TabListPage"/> the event is occurring for.
    /// </summary>
    /// <value>
    /// The <see cref="TabListPage"/> the event is occurring for.
    /// </value>
    public TabListPage TabListPage
    {
      get { return _tabPage; }
    }

    /// <summary>
    /// Gets the zero-based index of the <see cref="TabListPage"/> in the <see cref="TabList.TabListPages"/> collection.
    /// </summary>
    /// <value>
    /// The zero-based index of the <see cref="TabListPage"/> in the <see cref="TabList.TabListPages"/> collection.
    /// </value>
    public int TabPageIndex
    {
      get { return _tabPageIndex; }
    }

    #endregion
  }
}

