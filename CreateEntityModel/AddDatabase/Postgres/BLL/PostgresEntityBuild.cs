using CreateEntityModel.AddDatabase.Postgres.Model;
using CreateEntityModel.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateEntityModel.AddDatabase.Postgres.BLL
{
    //构建实体模型的类
    public class PostgresEntityBuild : IEntityBuild
    {
        private List<TableInfo> TableInfo { get; set; }
        public PostgresEntityBuild(List<TableInfo> tableInfo)
        {
            TableInfo = tableInfo;
        }
        /// <summary>
        /// 构建实体模型方法
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ICreateEntityFile EntityBuild(Action<EntityBuildModel> options)
        {
            //构建实体模型需要的相关参数
            EntityBuildModel model = new EntityBuildModel();
            //委托赋值
            options(model);
            Dictionary<string, string> fileContent = new Dictionary<string, string>();
            //历遍数据库中表信息
            foreach (var item in TableInfo)
            {
                //使用委托自定义类名规则
                if (model.CustomClassName != null)
                {
                    item.TableName = model.CustomClassName(item.TableName);
                }
                //根据数据库表名，字段信息，构建实体模型
                string content = CreatModel(item.TableName, item.ColumnInfos, model.NamespaceName, model.Using);
                //将表名，内容加入字典
                fileContent.Add(item.TableName, content);
            }
            //返回创建模型文件的类
            return new CreateEntityFile(fileContent);
        }
        /// <summary>
        /// 构建实体模型,把相关信息按模型格式拼接成字符串
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnInfos"></param>
        /// <param name="namespaceName"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        private string CreatModel(string tableName, List<ColumnInfo> columnInfos, string namespaceName, List<string> reference = null)
        {
            StringBuilder attribute = new StringBuilder();
            foreach (ColumnInfo item in columnInfos)
            {
                attribute.AppendSpaceLine("/// <summary>", 8);
                attribute.AppendSpaceLine($"/// {item.Description}", 8);
                attribute.AppendSpaceLine("/// </summary>", 8);
                attribute.AppendSpaceLine($"public {TypeCast(item.TypeName)} {item.ColumnName}", 8);
                attribute.AppendLine(" { get; set; }");
            }

            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine("using System;", 0, 0);
            strclass.AppendSpaceLine("using System.Collections.Generic;");
            strclass.AppendSpaceLine("using System.Text;");
            strclass.AppendSpaceLine("using Npgsql;");

            if (reference != null)
            {
                reference.ForEach(o =>
                {
                    strclass.AppendSpaceLine($"using {o};");
                });
            }
            strclass.AppendSpaceLine($"namespace {namespaceName}", 0, 2);
            strclass.AppendSpaceLine("{");
            strclass.AppendSpaceLine($"public class {tableName}", 4);
            strclass.AppendSpaceLine("{", 4);
            //封装属性
            strclass.AppendSpaceLine(attribute.ToString());
            strclass.AppendSpaceLine("}", 4);
            strclass.AppendSpaceLine("}");
            return strclass.ToString();
        }
        /// <summary>
        /// 数据库字段类型转换为C#属性类型
        /// 字段映射参考地址 https://www.npgsql.org/doc/types/basic.html
        /// </summary>
        /// <param name="TypeName">数据库字段类型</param>
        /// <returns></returns>
        private string TypeCast(string TypeName)
        {
            switch (TypeName.ToLower())
            {
                case "boolean":
                    TypeName = "bool";
                    break;
                case "smallint":
                    TypeName = "short";
                    break;
                case "integer":
                    TypeName = "int";
                    break;
                case "bigint":
                    TypeName = "long";
                    break;
                case "real":
                    TypeName = "float";
                    break;
                case "double precision":
                    TypeName = "double";
                    break;
                case "numeric":
                    TypeName = "decimal";
                    break;
                case "money":
                    TypeName = "decimal";
                    break;
                case "text":
                    TypeName = "string";
                    break;
                case "character varying":
                    TypeName = "string";
                    break;
                case "character":
                    TypeName = "string";
                    break;
                case "citext":
                    TypeName = "string";
                    break;
                case "json":
                    TypeName = "string";
                    break;
                case "jsonb":
                    TypeName = "string";
                    break;
                case "xml":
                    TypeName = "string";
                    break;
                case "point":
                    TypeName = "NpgsqlPoint";
                    break;
                case "lseg":
                    TypeName = "NpgsqlLSeg";
                    break;
                case "path":
                    TypeName = "NpgsqlPath";
                    break;
                case "polygon":
                    TypeName = "NpgsqlPolygon";
                    break;
                case "line":
                    TypeName = "NpgsqlLine";
                    break;
                case "circle":
                    TypeName = "NpgsqlCircle";
                    break;
                case "box":
                    TypeName = "NpgsqlBox";
                    break;
                case "uuid":
                    TypeName = "Guid";
                    break;
                case "date":
                    TypeName = "DateTime";
                    break;
                case "interval":
                    TypeName = "TimeSpan";
                    break;
                case "timestamp without time zone":
                    TypeName = "DateTime";
                    break;
                case "timestamp with time zone":
                    TypeName = "DateTime";
                    break;
                case "time without time zone":
                    TypeName = "TimeSpan";
                    break;
                case "time with time zone":
                    TypeName = "DateTimeOffset";
                    break;
                case "bit(1)":
                    TypeName = "bool";
                    break;
                case "bytea":
                    TypeName = "byte[]";
                    break;
                case "oid":
                    TypeName = "unit";
                    break;
                case "xid":
                    TypeName = "unit";
                    break;
                case "cid":
                    TypeName = "unit";
                    break;
                case "bit(n)":
                    TypeName = "BitArray";
                    break;
                case "bit varying":
                    TypeName = "BitArray";
                    break;
                case "hstore":
                    TypeName = "Dictionary<string,string>";
                    break;
                case "name":
                    TypeName = "string";
                    break;
                case "(internal)char":
                    TypeName = "char";
                    break;
                case "geometry":
                    TypeName = "PostgisGeometry";
                    break;
                case "record":
                    TypeName = "object[]";
                    break;
                case "enum types":
                    TypeName = "TEnum";
                    break;
                default:
                    TypeName = TypeName.ToLower();
                    break;
            }
            return TypeName;            
        }
    }
}
