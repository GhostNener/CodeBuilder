using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Helper
{
    public static class SqlHelper
    {
        public static readonly string connstr = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        public static SqlConnection OpenConnection()
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                return conn;
            }
        }
        /// <summary>
        /// 执行返回受影响的行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                return ExecuteNonQuery(conn, cmdText, parameters);
            }
        }
        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                return ExecuteScalar(conn, cmdText, parameters);
            }
        }
        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connstr))
            {
                con.Open();
                return ExecuteDataSet(con, cmdText, parameters);
            }
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connstr))
            {
                con.Open();
                return ExecuteDataTable(con, cmdText, parameters);
            }
        }
        /// <summary>
        /// 执行返回受影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlConnection conn, string cmdText,
           params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlConnection conn, string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(SqlConnection con, string cmdText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(con, cmdText, parameters).Tables[0];
        }
        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(SqlConnection con, string cmdText, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    adapter.FillSchema(ds, SchemaType.Source);
                    adapter.Fill(ds);
                    return ds;
                }
            }
        }
        /// <summary>
        /// null 转换为DBNull
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBValue(this object value)
        {
            return value == null ? DBNull.Value : value;
        }
        /// <summary>
        /// DBNull转换为null
        /// </summary>
        /// <param name="dbValue"></param>
        /// <returns></returns>
        public static object FromDBValue(this object dbValue)
        {
            return dbValue == DBNull.Value ? null : dbValue;
        }
    }
}