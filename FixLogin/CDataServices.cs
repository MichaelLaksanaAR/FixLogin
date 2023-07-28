using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Windows.Forms;


//Version   : 1.0.0.1 
//Date      : 20161109
//
//2018-03-02    Added Date utility


namespace ARDataServices
{
    public class cDataServices
    {
        private SqlConnection m_cn = new SqlConnection();

        //// PURPOSE: This class is used to abstract data services.  
        //internal cDataServices(string sConnectionString)
        public void Open(string sConnectionString)
        {
            try
            {
                m_cn.ConnectionString = sConnectionString;
                m_cn.Open();
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
            }
        }

        public void Close()
        {
            if (m_cn.State == ConnectionState.Open)
            {
                m_cn.Close();
                //  m_cn = null;
            }
        }

        #region "SqlClient"
        public SqlDataReader GetReader(string sSql)
        {
            //// PURPOSE: This function will return a DataReader object for a given
            //// SQL expression.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.CommandTimeout = 0;
                SqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
                return null;
            }
        }
        public SqlDataReader GetReader(string sSql, int TimeOut = 0)
        {
            //// PURPOSE: This function will return a DataReader object for a given
            //// SQL expression.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.CommandTimeout = TimeOut;
                SqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                ex.ToString();
                //MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
                return null;
            }
        }

        public int GetScalarSql(string sSql)
        {
            //// PURPOSE: This function will return a Scalar for a given SQL expression.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                int iNewRecordID = 0;
                iNewRecordID = Convert.ToInt32(cmd.ExecuteScalar());
                return iNewRecordID;
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
                return -1;
            }
        }

        public int GetScalarSql(string sSql, int TimeOut = 0)
        {
            //// PURPOSE: This function will return a Scalar for a given SQL expression.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.CommandTimeout = TimeOut;
                int iNewRecordID = 0;
                iNewRecordID = Convert.ToInt32(cmd.ExecuteScalar());
                return iNewRecordID;
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
                return -1;
            }
        }

        public void ExecuteNonQuerySql(string sSql)
        {
            //// PURPOSE: This function will execute a non-query SQL statement such
            //// as an INSERT or UPDATE or DELETE statement.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                //cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
            }
        }
        public void ExecuteNonQuerySql(string sSql, int TimeOut = 0)
        {
            //// PURPOSE: This function will execute a non-query SQL statement such
            //// as an INSERT or UPDATE or DELETE statement.
            try
            {
                SqlCommand cmd = m_cn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.CommandTimeout = TimeOut;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
            }
        }
        #endregion
    }

    public class cSafeDataServices
    {
        SqlConnection m_cn = new SqlConnection();
        SqlCommand command;
        SqlTransaction transaction;
        //// PURPOSE: This class is used to abstract data services.  
        public void New(string sConnectionString)
        {
            try
            {
                m_cn.ConnectionString = sConnectionString;
                m_cn.Open();
                command = m_cn.CreateCommand();
                transaction = m_cn.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Connection = m_cn;
                command.Transaction = transaction;
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
                //ErrLog(m_sLogPath, ex.Message)
            }
        }

        internal void ExecuteNonQuery(string sSql)
        {
            try
            {
                command.CommandText = sSql;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                try
                {
                    e.ToString();
                    transaction.Rollback();
                }
                catch (SqlException ex)
                {
                    ex.ToString();
                    if ((transaction.Connection != null))
                    {
                        //Console.WriteLine("An exception of type " + ex.GetType().ToString() + " was encountered while attempting to roll back the transaction.");
                    }
                }
            }
        }
        internal void Commit()
        {
            try
            {
                transaction.Commit();
                if (m_cn.State == ConnectionState.Open)
                {
                    m_cn.Close();
                    m_cn = null;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Interaction.MsgBox(ex.Message);
            }
        }
    }

    public class cSqlUtilities
    {
        public string Date2SqlDate(DateTime datevalue)
        { string temp = "{d '" + datevalue.ToString("yyyy-MM-dd") + "'}"; return temp; }
        public string DateTime2SqlDateTime(DateTime datevalue)
        { string temp = "{ts '" + datevalue.ToString("yyyy-MM-dd HH:mm:ss") + "'}"; return temp; }
        public string DateTime2SqlTime(DateTime datevalue)
        { string temp = "{t '" + datevalue.ToString("HH:mm:ss") + "'}"; return temp; }
        public string SqlSingleQuote(string value)
        { string temp = value.Replace("'", "''"); return temp; }
    }
}

