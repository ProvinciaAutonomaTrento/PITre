// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaUtils.Interfaces.DbManagement;
using System.Data;

namespace DocsPaDB;

/// <summary>
///
/// </summary>
public class TransactionContext : ITransactionContext
{
    /// <summary>
    ///
    /// </summary>
    protected ITransactionContext _instance = null;

    /// <summary>
    ///
    /// </summary>
    public TransactionContext() : this(IsolationLevel.Unspecified)
    {
    }

    public TransactionContext(bool openConnection)
    {
        string dbType = DocsPaVO.Settings.AppSettings.Instance.DBType.ToUpper();
        this._instance = this.CreateConcreteObject(dbType, string.Empty, IsolationLevel.Unspecified, openConnection);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="iso"></param>
    public TransactionContext(IsolationLevel iso)
    {
        string dbType = DocsPaVO.Settings.AppSettings.Instance.DBType.ToUpper();
        this._instance = this.CreateConcreteObject(dbType, string.Empty, iso);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbKey"></param>
    /// <param name="iso"></param>
    public TransactionContext(string dbKey, IsolationLevel iso)
    {
        string dbType = DocsPaVO.Settings.AppSettings.Instance.DBType.ToUpper();
        this._instance = this.CreateConcreteObject(dbType, dbKey, iso);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbKey"></param>
    public TransactionContext(string dbKey) : this(dbKey, IsolationLevel.Unspecified)
    {
    }

    #region Public methods

    /// <summary>
    /// Chiave del database
    /// </summary>
    public string DbKey
    {
        get
        {
            return this._instance.DbKey;
        }
    }

    /// <summary>
    /// Indica se l'istanza � proprietaria della transazione
    /// </summary>
    public bool IsTransactionOwner
    {
        get
        {
            return this._instance.IsTransactionOwner;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public TransactionContextStateEnum State
    {
        get
        {
            return this._instance.State;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void Complete()
    {
        this._instance.Complete();
    }

    /// <summary>
    ///
    /// </summary>
    public void Dispose()
    {
        this._instance.Dispose();
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Creazione istanza concreta contesto transazionale
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="dbKey"></param>
    /// <param name="iso"></param>
    /// <returns></returns>
    protected ITransactionContext CreateConcreteObject(string dbType, string dbKey, IsolationLevel iso)
    {
        ITransactionContext instance = null;

        switch (dbType)
        {
#if false   // sql server non gestito
            case "SQL":
                instance = new DocsPaDbManagement.Database.SqlServer.SqlTransactionContext(dbKey, iso);
                break;
#endif
            case "ORACLE":
                instance = new DocsPaDbManagement.Database.Oracle.OracleTransactionContext(dbKey, iso);
                break;
        }

        return instance;
    }

    protected ITransactionContext CreateConcreteObject(string dbType, string dbKey, IsolationLevel iso, bool openConnection)
    {
        ITransactionContext instance = null;

        switch (dbType)
        {
#if false   // sql server non gestito
            case "SQL":
                instance = new DocsPaDbManagement.Database.SqlServer.SqlTransactionContext(dbKey, iso);
                break;
#endif
            case "ORACLE":
                instance = new DocsPaDbManagement.Database.Oracle.OracleTransactionContext(dbKey, iso, openConnection);
                break;
        }

        return instance;
    }

        #endregion
    }

