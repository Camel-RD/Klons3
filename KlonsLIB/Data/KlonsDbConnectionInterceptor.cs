using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.Data;

public class KlonsDbConnectionInterceptor : DbConnectionInterceptor
{
    protected readonly string _userName;
    public KlonsDbConnectionInterceptor(string username)
    {
        _userName = username;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (eventData.Context is MyDbContext mctx)
        {
            mctx.SetDbUserName((FbConnection)connection, _userName);
        }
    }

}
