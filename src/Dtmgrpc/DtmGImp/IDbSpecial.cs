using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Dtmgrpc.DtmGImp
{
    public interface IDbSpecial
    {
        string Name { get; }

        string GetPlaceHoldSQL(string sql);

        string GetInsertIgnoreTemplate(string tableAndValues, string pgConstraint);

        string GetXaSQL(string command, string xid);
    }

    public class MysqlDBSpecial : IDbSpecial
    {
        public string Name => "mysql";

        public string GetInsertIgnoreTemplate(string tableAndValues, string pgConstraint)
            => string.Format("insert ignore into {0}", tableAndValues);

        public string GetPlaceHoldSQL(string sql)
            => sql;

        public string GetXaSQL(string command, string xid)
            => string.Format("xa {0} '{1}'", command, xid);
    }

    public class PostgresDBSpecial : IDbSpecial
    {
        public string Name => "postgres";

        public string GetInsertIgnoreTemplate(string tableAndValues, string pgConstraint)
            => string.Format("insert into {0} on conflict ON CONSTRAINT {1} do nothing", tableAndValues, pgConstraint);

        public string GetPlaceHoldSQL(string sql)
            => sql;

        public string GetXaSQL(string command, string xid)
        {
            var dict = new Dictionary<string, string>
            {
                { "end", "" },
                { "start", "begin" },
                { "prepare", $"prepare transaction '{xid}'" },
                { "commit", $"commit prepared '{xid}'" },
                { "rollback", $"rollback prepared '{xid}'" },
            };

            return dict.TryGetValue(command, out var sql) ? sql : string.Empty;
        }
    }

    public class SqlServerDBSpecial : IDbSpecial
    {
        public string Name => "sqlserver";

        public string GetInsertIgnoreTemplate(string tableAndValues, string pgConstraint)
            => string.Format("insert into {0}", tableAndValues);

        public string GetPlaceHoldSQL(string sql)
            => sql;

        public string GetXaSQL(string command, string xid)
            => throw new DtmException("not support xa now!");
    }

    public class DbSpecialDelegate
    { 
        private readonly IDbSpecial _special;

        public DbSpecialDelegate(IEnumerable<IDbSpecial> specials, IOptions<DtmOptions> optionsAccs)
        {
            var dbSpecial = specials.FirstOrDefault(x => x.Name.Equals(optionsAccs.Value.DBType));

            if (dbSpecial == null) throw new DtmException($"unknown db type '{optionsAccs.Value.DBType}'");

            _special = dbSpecial;
        }

        public IDbSpecial GetDbSpecial() => _special;
    }
}