using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;


namespace AutoTesterLib
{
   public class sqlHelper
    {
       public static bool IsConnected(string serverName,string DBName)
       {
           bool isConnected = false;
          SqlConnection conn =ConnectDB(serverName,DBName);
           try {
               conn.Open(); 
               //check tables?
           }
           catch
           {
               //wrong connetion string
           }
           if (conn.State == ConnectionState.Open)
           { 
               isConnected = true;
               conn.Close();
           }
           if (isConnected == false) conn.Close();
           return isConnected;
       }

       public static SqlConnection ConnectDB()
       {
           string ServerName=XmlHelper.GetElementAttrValue1(Constant.ConfigurationXmlPath,"Server","Name");
           string DBName = XmlHelper.GetElementTextValue1(Constant.ConfigurationXmlPath,"DB");
           string connStr = "integrated security=SSPI;data source=" + ServerName + ";initial catalog=" + DBName;
           SqlConnection conn = new SqlConnection(connStr);
           conn.Open();
           return conn;
       }

       public static SqlConnection ConnectDB(string ServerName, string DBName)
       {
           SqlConnection conn =null;
           try
           {
               string connStr = "integrated security=SSPI;data source=" + ServerName + ";initial catalog=" + DBName;
               conn = new SqlConnection(connStr);
               conn.Open();
           }
           catch
           { }
           return conn;
       }

       public static void exeTable(DataTable dt, string sql)
       {
           SqlConnection sqlcn_data = ConnectDB();
           SqlCommand cmd = sqlcn_data.CreateCommand();
           SqlDataAdapter da = new SqlDataAdapter(cmd);
           cmd.CommandTimeout = 0;
           cmd.CommandText = sql;
           try
           {
               da.Fill(dt);
               sqlcn_data.Close();
           }
           catch (SqlException ex)
           {
               throw ex;
           }

       }
       public static void exeNonQ( string sql)
       {
           SqlConnection sqlcn_data = ConnectDB();
           SqlCommand cmd = sqlcn_data.CreateCommand();
           cmd.CommandTimeout = 0;
           cmd.CommandText = sql;
           try
           {
               cmd.ExecuteNonQuery();
               sqlcn_data.Close();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //return one row according to unique id
       public static DataRow exeOneRowValue(string tableName,string IDColumnName, int ID)
       {
           DataTable dt_Row = new DataTable();
           string sql = "select * from " + tableName + " where " + IDColumnName + "=" + ID;
           exeTable(dt_Row, sql);
           if (dt_Row != null && dt_Row.Rows.Count != 0)
               return dt_Row.Rows[0];
           else return null;
       }
       //return column value 
       public static List<string> exeColumnValue(string tableName,string columnName,string filter)
       {
           List<string> listColumns = new List<string>();
           DataTable dt_Column = new DataTable();
           string sql="select "+columnName+" from "+tableName;
           if (filter != "") sql += " where " + filter;
           exeTable(dt_Column,sql);
           foreach (DataRow dr in dt_Column.Rows)
           {
               string str = dr[columnName].ToString().Trim();
               if (!listColumns.Contains(str)) listColumns.Add(str); 
           }
           return listColumns;
       }

       //add new row
       public static void AddNewRow(string tableName,object[] columnValues)
       {
           try
           {
               SqlConnection conn = ConnectDB();
               string sql = "select * from " + tableName;
               SqlDataAdapter da = new SqlDataAdapter(sql, conn);
               SqlCommandBuilder scb = new SqlCommandBuilder(da);//SqlCommandBuilder根据insertCommand构造updatecommand和deletecommand
               da.InsertCommand = scb.GetInsertCommand();

               DataTable dt = new DataTable();
               exeTable(dt, sql);
               DataRow dr = dt.NewRow();
               dr.ItemArray = columnValues;
               dt.Rows.Add(dr);

               da.Update(dt);
               conn.Close();
                
           }
           catch (Exception e) { throw e; }
       }

       //update field according to id
       public static void SetDeletedColumnForRemove(string tableName,string IdColumn,int Id)
       {
           string sql = "update "+tableName+" set Deleted=1 where "+IdColumn+"="+Id;
           exeNonQ(sql);
       }

           //delete a row with ID as table's primary key
       public static void DeleteRowWithID(string tableName, string IdColumn, int ID)
       {
           string sql = "delete from " + tableName + " where " + IdColumn + "=" + ID;
           exeNonQ(sql);
       }
       
       //with filter
       public static void DeleteRowWithID(string tableName, string IdColumn, int ID,string filter)
       {
           string sql = "delete from " + tableName + " where " + IdColumn + "=" + ID+" and "+filter;
           exeNonQ(sql);
       }

       public static void UpdateFieldValue(string tableName,int rowIndex,Dictionary<string,object> dic_column_value)
       {
           SqlConnection conn = ConnectDB();
           string sql = "select * from " + tableName;
           SqlDataAdapter da = new SqlDataAdapter(sql, conn);
           SqlCommandBuilder scb = new SqlCommandBuilder(da);
           da.UpdateCommand = scb.GetUpdateCommand();
           DataSet dataset = new DataSet();
           da.Fill(dataset,tableName);

           //更新字段值
           foreach(KeyValuePair<string,object> kvp in dic_column_value)
           {
               dataset.Tables[0].Rows[rowIndex][kvp.Key] = kvp.Value;
           }

           da.Update(dataset,tableName);
           conn.Close();
       }

       public static void UpdateFieldValueAcoordingToUniqueId(string tableName, string IdColumn,int IdValue, Dictionary<string, object> dic_column_value)
       {
           string sql = "select * from "+tableName;
           DataTable dt = new DataTable();
           exeTable(dt, sql);
           int rowIndex = -1;
           for (int i = 0; i < dt.Rows.Count; i++)
           {
               if (Convert.ToInt32(dt.Rows[i][IdColumn]) == IdValue)
                   rowIndex = i;
           }
           if (rowIndex != -1)
           {
               SqlConnection conn = ConnectDB();
               SqlDataAdapter da = new SqlDataAdapter(sql, conn);
               SqlCommandBuilder scb = new SqlCommandBuilder(da);
               da.UpdateCommand = scb.GetUpdateCommand();
               DataSet dataset = new DataSet();
               da.Fill(dataset, tableName);

               //更新字段值
               foreach (KeyValuePair<string, object> kvp in dic_column_value)
               {
                   dataset.Tables[0].Rows[rowIndex][kvp.Key] = kvp.Value;
               }

               da.Update(dataset, tableName);
               conn.Close();
           }
       }

       //check id exists or not 
       public static bool IsValueExists(string tableName,string IdColumnName,int Id)
       {
           bool IsExists = false;
           DataTable dt = new DataTable();
           string sql = "select "+IdColumnName+" from "+tableName+" where "+IdColumnName+"="+Id;
           exeTable(dt,sql);
           if (dt != null && dt.Rows.Count != 0) IsExists = true;
           return IsExists;
       }
       //check id exists or not 
       public static bool IsValueExists(string tableName, string IdColumnName, int Id,string extraFilter)
       {
           bool IsExists = false;
           DataTable dt = new DataTable();
           string sql = "select " + IdColumnName + " from " + tableName + " where " + IdColumnName + "=" + Id+ " and "+extraFilter;
           exeTable(dt, sql);
           if (dt != null && dt.Rows.Count != 0) IsExists = true;
           return IsExists;
       }
       //check string value
       public static bool IsValueExists(string tableName, string ColumnName, string value, string extraFilter)
       {
           bool IsExists = false;
           DataTable dt = new DataTable();
           string sql = "select " + ColumnName + " from " + tableName + " where " + ColumnName + "=N'" + value + "' and " + extraFilter;
           exeTable(dt, sql);
           if (dt != null && dt.Rows.Count != 0) IsExists = true;
           return IsExists;
       }
       //check string value
       public static bool IsValueExists(string tableName, string ColumnName, string value)
       {
           bool IsExists = false;
           DataTable dt = new DataTable();
           string sql = "select " + ColumnName + " from " + tableName + " where " + ColumnName + "=N'" + value+"'";
           exeTable(dt, sql);
           if (dt != null && dt.Rows.Count != 0) IsExists = true;
           return IsExists;
       }

       //get record count according to id
       public static int GetRecordCount(string tableName,string idColumnName,int id,string filter)
       {
           int count = 0;
           DataTable dt = new DataTable();
           string sql = "select count(*) from "+tableName+" where "+idColumnName+"="+id;
           if (filter != "") sql += " and " + filter;
           exeTable(dt,sql);
           if (dt != null && dt.Rows.Count != 0) count = Convert.ToInt32(dt.Rows[0][0]);
           return count;
       }

        public static int GetNewId(string tableName,string IdColumnName)
       {
            int id=0;
           string sql = "select max("+IdColumnName+") from "+tableName;
           DataTable dt = new DataTable();
           exeTable(dt,sql);
           if (dt.Rows[0][0].ToString()=="") id = 1;
           else 
               id = Convert.ToInt32(dt.Rows[0][0])+1;
           return id;
        }

       public static List<string> GetDBNames(string ServerName)
       {
           List<string> listDBName=new List<string>();
           string sql = "select [name] from master.dbo.sysdatabases where DBId>6 Order By [Name] ";
           DataTable dt = new DataTable();
           SqlConnection conn = sqlHelper.ConnectDB(ServerName, "master");
           sqlHelper.exeTable(dt, sql);
           foreach(DataRow dr in dt.Rows)
           {
               if (dr[0].ToString().Trim() == "AutoTester")
               {
                   if (!listDBName.Contains(dr[0].ToString().Trim()))
                       listDBName.Add(dr[0].ToString().Trim());
               }
           }
           return listDBName;
       }

       public static DataTable GetTableFromTable(DataTable sourceTable, string filter)
       {
           DataTable dt_New = sourceTable.Clone();
           DataRow[] drs = sourceTable.Select(filter);
           foreach (DataRow dr in drs)
           {
               dt_New.Rows.Add(dr.ItemArray);
           }
           return dt_New;
       }

       public static bool CheckServerName(string name)
       {
           try
           {
               bool isExists = false;
               PingReply pingRely;
               using (var ping = new Ping())
                   pingRely = ping.Send(name);
               if (pingRely.Status == IPStatus.Success) isExists = true;
               return isExists;
           }
           catch { return false; }
       }

    }
}
