using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBuilder
{
    public class CreateCodeHelper
    {
        public static int sqltype = 1;//数据库类型 1 sql server ；2 mysql
        public static string leftStr = "["; //左定界符
        public static string rightStr = "]";//右定界符

        #region GetDataTypeName 进行可空类型处理
        /// <summary>
        ///进行可空类型处理
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetDataTypeName(DataColumn column)
        {
            //如果列允许为NULL，并且在.NET中的类型不能为空（值类型）
            if (column.AllowDBNull && column.DataType.IsValueType)
            {
                return column.DataType + "?";
            }
            else
            {
                return column.DataType.ToString();
            }
        }
        #endregion
        #region GetDataTypeNameString 进行可空类型处理，String为空要特殊处理
        /// <summary>
        /// 进行可空类型处理，String为空要特殊处理
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetDataTypeNameString(DataColumn column)
        {
            //如果列允许为NULL，并且在.NET中的类型不能为空（值类型）
            if (column.AllowDBNull)
            {
                return column.DataType + "?";
            }
            else
            {
                return column.DataType.ToString();
            }
        }
        #endregion
        #region GetColumnNames 以数组形式返回列名
        /// <summary>
        /// 以数组形式返回列名
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string[] GetColumnNames(DataTable dt)
        {
            string[] colNames = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn dataCol = dt.Columns[i];
                colNames[i] = dataCol.ColumnName;
            }
            return colNames;
        }
        #endregion
        #region GetColumnNamesUpdate 以数组形式返回"列名=@列名"
        /// <summary>
        /// 以数组形式返回"列名=@列名"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string[] GetColumnNamesUpdate(DataTable dt)
        {
            string[] colNames = new string[dt.Columns.Count - 1];
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                DataColumn dataCol = dt.Columns[i];
                colNames[i - 1] = leftStr + dataCol.ColumnName + rightStr + "=@" + dataCol.ColumnName;
            }
            return colNames;
        }
        #endregion
        #region GetSqlParameter 得到 new SqlParameter
        /// <summary>
        /// 得到 new SqlParameter
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        private static void GetSqlParameter(DataTable dt, StringBuilder sb, string hasNamespace, bool isNullId)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            helper = "Helper." + helper;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i == 0 && isNullId == true) { continue; }
                if (GetDataTypeNameString(dt.Columns[i]).IndexOf("?") > 0)
                {
                    sb.AppendLine(hasNamespace + "                    ,new " + heP + "Parameter(\"@" + dt.Columns[i].ColumnName + "\", " + helper + "Helper.ToDBValue(model." + dt.Columns[i].ColumnName + "))");
                }
                else
                {
                    sb.AppendLine(hasNamespace + "                    ,new " + heP + "Parameter(\"@" + dt.Columns[i].ColumnName + "\", model." + dt.Columns[i].ColumnName + ")");
                }
            }
        }
        #endregion
        #region CreateModelCode 生成Model带命名空间
        /// <summary>
        /// 生成Model带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="strNamespace"></param>
        /// <returns></returns>
        public StringBuilder CreateModelCode(string tableName, DataTable dt, string strNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace " + strNamespace + " {");
            sb.AppendLine("    public  class  " + tableName + " {");
            foreach (DataColumn col in dt.Columns)
            {
                sb.AppendLine("        public " + GetDataTypeName(col) + " " + col.ColumnName + " {get; set;}");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateModelCode 生成Model不带命名空间
        /// <summary>
        /// 生成Model不带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public StringBuilder CreateModelCode(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public class  " + tableName + " {");

            foreach (DataColumn col in dt.Columns)
            {
                sb.AppendLine("    public " + GetDataTypeName(col) + " " + col.ColumnName + " {get; set;}");
            }
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateDALCode 生成DAL不带命名空间
        /// <summary>
        /// 生成DAL不带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public StringBuilder CreateDALCode(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public static class  " + tableName + "DAL {");
            sb.AppendLine("");
            GetAllDAL(tableName, dt, sb,"");
            sb.AppendLine("}");//tableNameDAL
            return sb;
        }

        private void GetAllDAL(string tableName, DataTable dt, StringBuilder sb,string otherStr="")
        {
            CreateToModel(tableName, dt, sb, otherStr);
            CreateInsert(tableName, dt, sb, otherStr);
            CreateDeleteById(tableName, dt, sb, otherStr);
            CreateUpdate(tableName, dt, sb, otherStr);
            CreateGetById(tableName, dt, sb, otherStr);
            CreateListAll(tableName, dt, sb, otherStr);
            CreateListByWhere(tableName, sb, otherStr);
            CreateListByPage(tableName, dt, sb, otherStr);
        }
        #endregion
        #region CreateDALCode 生成DAL带命名空间
        /// <summary>
        /// 生成DAL带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="strNamespace"></param>
        /// <returns></returns>
        public StringBuilder CreateDALCode(string tableName, DataTable dt, string strNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using Helper;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            string sqluse = sqltype == 2 ? "using MySql.Data.MySqlClient;" : "using System.Data.SqlClient;";
            sb.AppendLine(sqluse);
            sb.AppendLine("");
            sb.AppendLine("namespace " + strNamespace + " {");
            sb.AppendLine("    public static class  " + tableName + "DAL {");
            sb.AppendLine("");
            GetAllDAL(tableName, dt, sb, "    ");
            sb.AppendLine("    }");//tableNameDAL
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateToModel 生成ToModel
        /// <summary>
        /// 生成ToModel
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateToModel(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    public static " + tableName + " ToModel(DataRow row) {");
            sb.AppendLine(hasNamespace + "        " + tableName + " model = new " + tableName + "();");
            foreach (DataColumn col in dt.Columns)
            {
                if (GetDataTypeNameString(col).IndexOf("?") > 0)
                {
                    sb.AppendLine(hasNamespace + "        model." + col.ColumnName + " = (" + GetDataTypeName(col) + ")" + helper + "Helper.FromDBValue(row[\"" + col.ColumnName + "\"]);");
                }
                else
                {
                    sb.AppendLine(hasNamespace + "        model." + col.ColumnName + " = (" + GetDataTypeName(col) + ")row[\"" + col.ColumnName + "\"];");
                }
            }
            sb.AppendLine(hasNamespace + "        return model;");
            sb.AppendLine(hasNamespace + "    }");//ToModel
            sb.AppendLine("");
        }
        #endregion
        #region CreateListAll 生成ListAll

        /// <summary>
        /// 生成ListAll
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListAll(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 获得所有记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <returns>" + tableName + "类的对象的枚举</returns>");
            sb.AppendLine(hasNamespace + "    public static IEnumerable<" + tableName + "> ListAll() {");
            sb.AppendLine(hasNamespace + "        List<" + tableName + "> list = new List<" + tableName + ">();");
            string[] colNames = GetColumnNames(dt);
            for (int i = 0; i < colNames.Length; i++)
            {
                colNames[i] = leftStr + colNames[i] + rightStr;
            }
            sb.AppendLine(hasNamespace + "        DataTable dt = " + helper + "Helper.ExecuteDataTable(\"SELECT " + string.Join(", ", colNames) + " FROM " + leftStr + tableName + rightStr + "\");");
            sb.AppendLine(hasNamespace + "        foreach (DataRow row in dt.Rows)  {");
            sb.AppendLine(hasNamespace + "            list.Add(ToModel(row));");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        return list;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateInsert生成Insert
        /// <summary>
        /// 生成Insert
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateInsert(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            helper = "Helper." + helper;
            string[] colNames = GetColumnNames(dt);
            string[] colNamesTemp = new string[colNames.Length];
            string[] nullIdNames = new string[colNames.Length - 1];
            string[] nullIdNamesTemp = new string[colNames.Length - 1];
            for (int i = 0; i < colNamesTemp.Length; i++)
            {
                colNamesTemp[i] = leftStr + colNames[i] + rightStr;
                if (i >= 1)
                {
                    nullIdNames[i - 1] = colNames[i];
                    nullIdNamesTemp[i - 1] = leftStr + colNames[i] + rightStr;
                }
            }
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 插入一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>object 主键</returns>");
            sb.AppendLine(hasNamespace + "    public static object Insert(" + tableName + " model) {");
            sb.AppendLine(hasNamespace + "        object obj;");
            sb.AppendLine(hasNamespace + "        string isNullId = Convert.ToString(model." + colNames[0] + ");");
            sb.AppendLine(hasNamespace + "        if (isNullId.Equals(\"\") || isNullId.Equals(\"0\") || isNullId.Equals(new Guid().ToString()) || isNullId.Equals(null))");
            sb.AppendLine(hasNamespace + "        {");
            sb.AppendLine(hasNamespace + "           obj = " + helper + "Helper.ExecuteScalar(@\"INSERT INTO " + leftStr + tableName + rightStr + "(" + string.Join(", ", nullIdNamesTemp) + ") VALUES(@" + string.Join(", @", nullIdNames) + ") SELECT @@IDENTITY AS Id ;\"");
            GetSqlParameter(dt, sb, hasNamespace, true);
            sb.AppendLine(hasNamespace + "                );");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        else");
            sb.AppendLine(hasNamespace + "        {");
            sb.AppendLine(hasNamespace + "           obj = " + helper + "Helper.ExecuteScalar(@\"INSERT INTO " + leftStr + tableName + rightStr + "(" + string.Join(", ", colNamesTemp) + ") VALUES(@" + string.Join(", @", colNames) + ") SELECT @@IDENTITY AS Id ;\"");
            GetSqlParameter(dt, sb, hasNamespace, false);
            sb.AppendLine(hasNamespace + "                );");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "    return obj;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateUpdate 生成Update
        /// <summary>
        /// 生成Update
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateUpdate(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 更新一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>更新是否成功</returns>");
            sb.AppendLine(hasNamespace + "    public static bool Update(" + tableName + " model) {");
            sb.AppendLine(hasNamespace + "        int count = " + helper + "Helper.ExecuteNonQuery(\"UPDATE " + leftStr + tableName + rightStr + " SET " + string.Join(", ", GetColumnNamesUpdate(dt)) + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + "=@" + dt.Columns[0].ColumnName + "\"");
            GetSqlParameter(dt, sb, hasNamespace, false);
            sb.AppendLine(hasNamespace + "        );");
            sb.AppendLine(hasNamespace + "    return count > 0;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateGetById 生成GetById
        /// <summary>
        /// 生成GetById
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateGetById(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 获得一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"Id\">主键</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>" + tableName + "类的对象</returns>");
            sb.AppendLine(hasNamespace + "    public static " + tableName + " GetById(" + dt.Columns[0].DataType + " " + dt.Columns[0].ColumnName + ") {");
            string[] colNames = GetColumnNames(dt);
            for (int i = 0; i < colNames.Length; i++)
            {
                colNames[i] = leftStr + colNames[i] + rightStr;
            }
            sb.AppendLine(hasNamespace + "        DataTable dt = " + helper + "Helper.ExecuteDataTable(\"SELECT " + string.Join(", ", colNames) + " FROM " + leftStr + tableName + rightStr + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + "=@" + dt.Columns[0].ColumnName + "\", new " + heP + "Parameter(\"@" + dt.Columns[0].ColumnName + "\", " + dt.Columns[0].ColumnName + "));");
            sb.AppendLine(hasNamespace + "        if (dt.Rows.Count > 1) {");
            sb.AppendLine(hasNamespace + "            throw new Exception(\"more than 1 row was found\");");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        else if (dt.Rows.Count <= 0) {");
            sb.AppendLine(hasNamespace + "            return null;");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        DataRow row = dt.Rows[0];");
            sb.AppendLine(hasNamespace + "        " + tableName + " model = ToModel(row);");
            sb.AppendLine(hasNamespace + "        return model;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateDeleteById 生成DeleteById
        /// <summary>
        /// 生成DeleteById
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateDeleteById(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 删除一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"Id\">主键</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>删除是否成功</returns>");
            sb.AppendLine(hasNamespace + "    public static bool DeleteById(" + dt.Columns[0].DataType + " " + dt.Columns[0].ColumnName + ") {");
            sb.AppendLine(hasNamespace + "        int rows = " + helper + "Helper.ExecuteNonQuery(\"DELETE FROM " + leftStr + tableName + rightStr + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + " = @" + dt.Columns[0].ColumnName + "\", new " + heP + "Parameter(\"@" + dt.Columns[0].ColumnName + "\", " + dt.Columns[0].ColumnName + "));");
            sb.AppendLine(hasNamespace + "        return rows > 0;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion

        /// <summary>
        /// 生成ListByPage
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListByPage(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 分页查询");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"page\">页数（从1开始计数）</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"num\">每页个数（从1开始计数）</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"orderBy\">排序条件</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"isDesc\">是否降序</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"whereArr\">查询条件：例如ID=1,NAME='ADMIN'</param>");
            sb.AppendLine(hasNamespace + "    /// <returns></returns>");
            sb.AppendLine(hasNamespace + "    public static IEnumerable<" + tableName + "> ListByPage(int page = 1, int num = 10, string orderBy = \"" + dt.Columns[0].ColumnName + "\", bool isDesc = true, params string[] whereArr)");
            sb.AppendLine(hasNamespace + "    {");
            sb.AppendLine(hasNamespace + "        string whereStr = \"\";");
            sb.AppendLine(hasNamespace + "        List<string> ls = new List<string>();");
            sb.AppendLine(hasNamespace + "        foreach (var v in whereArr) { if (v != null && v != \"\") { ls.Add(v); } }");
            sb.AppendLine(hasNamespace + "        whereArr = ls.ToArray();");
            sb.AppendLine(hasNamespace + "        if (num < 1 || page < 1) { return null; }");
            sb.AppendLine(hasNamespace + "        List<" + tableName + "> list = new List<" + tableName + ">();");
            string pagetemp1 = sqltype == 2 ? "        if (whereArr != null && whereArr.Length > 0) { whereStr = \" and \" + string.Join(\" and \", whereArr); }" : "        if (where != null && where.Length > 0) { whereStr = \" and a.\" + string.Join(\" and a.\", where); }";
            sb.AppendLine(hasNamespace + pagetemp1);
            sb.AppendLine(hasNamespace + "        if (isDesc) { orderBy += \" desc\"; }");
            string pagetemp2 = sqltype == 2 ? "        DataTable dt = Helper.MySqlHelper.ExecuteDataTable(string.Format(@\"SELECT * FROM " + leftStr + tableName + rightStr + " WHERE (1=1) {0} ORDER BY {1} ASC LIMIT {2}, {3};\" , whereStr,orderBy,  ((page -1)* num), num));" : "        DataTable dt = Helper.SqlHelper.ExecuteDataTable(string.Format(@\"SELECT b.* FROM ( SELECT  a.*, ROW_NUMBER () OVER (ORDER BY a.{0} ) AS RowNumber FROM  " + leftStr + tableName + rightStr + " AS a WHERE (1 = 1) {1}) AS b WHERE  RowNumber BETWEEN {2} AND {3} ORDER BY b.{0}\" , orderBy, whereStr, ((page-1) * num + 1), page * num));";
            sb.AppendLine(hasNamespace + pagetemp2);
            sb.AppendLine(hasNamespace + "        foreach (DataRow row in dt.Rows) { list.Add(ToModel(row)); }");
            sb.AppendLine(hasNamespace + "        return list;");
            sb.AppendLine(hasNamespace + "    }");
        }
        #region CreateListByWhere 生成ListByWhere
        /// <summary>
        /// 生成ListByWhere
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListByWhere(string tableName, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 通过条件获得满足条件的记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"whereStr\">其他的sql 语句  </param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"fields\">需要的条件的字段名</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>满足条件的记录</returns>");
            sb.AppendLine(hasNamespace + "     public static IEnumerable<" + tableName + "> ListByWhere(" + tableName + " model,string whereStr, params string[] fields)");
            sb.AppendLine(hasNamespace + "     {");
            string sqlpar = sqltype == 2 ? "MySqlParameter" : "SqlParameter";
            sb.AppendLine(hasNamespace + "         List<" + sqlpar + "> lsParameter = new List<" + sqlpar + ">();");
            sb.AppendLine(hasNamespace + "         string str = Helper.GenericSQLGenerator.GetWhereStr<" + tableName + ">(model, \"" + tableName + "\", out lsParameter, fields);");
            sb.AppendLine(hasNamespace + "         if(whereStr!=null&&whereStr.Trim().Length>0){str=str+\" and \"+whereStr;}");
            sb.AppendLine(hasNamespace + "         List<" + tableName + "> list = new List<" + tableName + ">();");
            sb.AppendLine(hasNamespace + "         " + sqlpar + "[] sqlparm = lsParameter.ToArray();");
            string helper = sqltype == 2 ? "MySqlHelper" : "SqlHelper";
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "         DataTable dt = " + helper + ".ExecuteDataTable(str, sqlparm);");
            sb.AppendLine(hasNamespace + "         foreach (DataRow row in dt.Rows)");
            sb.AppendLine(hasNamespace + "         {");
            sb.AppendLine(hasNamespace + "             list.Add(ToModel(row));");
            sb.AppendLine(hasNamespace + "         }");
            sb.AppendLine(hasNamespace + "         return list;");
            sb.AppendLine(hasNamespace + "     }");
            sb.AppendLine("");
        }
        #endregion
    }
}
