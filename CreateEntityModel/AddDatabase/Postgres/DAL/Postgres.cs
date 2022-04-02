using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Npgsql;
using CreateEntityModel.AddDatabase.Postgres.Model;

namespace CreateEntityModel.AddDatabase.Postgres.DAL
{
    public class Postgres
    {
        /// <summary>
        /// Npgsql 文档地址 https://www.npgsql.org/doc/index.html
        /// 可参考数据库连接及获取表和字段信息
        /// </summary>
        private NpgsqlConnection conn { get; set; }

        public Postgres(string ConnectionString)
        {
            conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
        }
        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetTableInfo(string tbschema = null)
        {
            List<TableInfo> tables = new List<TableInfo>();
            using (DataTable dt = conn.GetSchema("Tables"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    // 通过模式过滤表
                    if (tbschema != null && dr["table_schema"].ToString() != tbschema)
                        continue;
                    string tableName = dr["table_name"].ToString();
                    tables.Add(new TableInfo
                    {
                        TableName = tableName,
                        ColumnInfos = GetFileds(tableName)
                    });
                }
            }
            return tables;
        }
        /// <summary>
        /// 获取表中字段信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<ColumnInfo> GetFileds(string tableName)
        {
            if(tableName == "tb_xmxx")
            {
                string a = tableName;
            }
            List<ColumnInfo> _Fields = new List<ColumnInfo>();
            string[] restrictionValues = new string[4];
            restrictionValues[0] = null; // Catalog
            restrictionValues[1] = null; // Owner
            restrictionValues[2] = tableName; // Table
            restrictionValues[3] = null; // Column
            DataTable dt = conn.GetSchema("Columns", restrictionValues);
            foreach (DataRow dr in dt.Rows)
            {
                ColumnInfo field = new ColumnInfo();
                field.ColumnName = dr["column_name"].ToString();
                field.TypeName = dr["data_type"].ToString();
                //field.Description = dr["COLUMN_COMMENT"].ToString();
                _Fields.Add(field);
            }
            return _Fields;
        }
    }
}
