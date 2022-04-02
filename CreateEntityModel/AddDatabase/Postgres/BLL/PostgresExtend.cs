using CreateEntityModel.AddDatabase.Postgres.DAL;
using CreateEntityModel.CreateEntityModelBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateEntityModel.AddDatabase.Postgres.BLL
{
    public static class PostgresExtend
    {
        //通过对CreateDefaultBuilder扩展，连接数据库，获取数据信息
        public static IEntityBuild AddPostgreSql(this CreateDefaultBuilder builder,string connectionString, string tbschema = null)
        {
            //连接数据库
            var database = new DAL.Postgres(connectionString);
            //获取表信息
            var tableInfos = database.GetTableInfo(tbschema);
            //返回构建实体模型的类
            return new PostgresEntityBuild(tableInfos);
        }
    }
}
