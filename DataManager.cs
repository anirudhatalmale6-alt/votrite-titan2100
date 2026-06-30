// Product:	VotRite
// Module:  DataManager.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using VotRite.Util;

namespace VotRite
{
	public enum Databases {
		VotingResultsData,
		VotingContentData
	}

    class DataManager
    {
		private static DataManager votingResultsDataInstance;
		private static DataManager votingContentDataInstance;

        private string connString;
        private string connPassword;
        private SQLiteConnection conn;

        public static DataManager VotingResultsDataInstance {
            get {
				return votingResultsDataInstance ?? (votingResultsDataInstance = new DataManager(Databases.VotingResultsData));
            }
		}

		public static DataManager VotingContentDataInstance {
			get {
				return votingContentDataInstance ?? (votingContentDataInstance = new DataManager(Databases.VotingContentData));
			}
		}

		protected DataManager(){}

        protected DataManager(Databases dataBase)
        {
            switch (dataBase) {
				case Databases.VotingResultsData:
					connString = AppManager.Configuration["System"]["ResultsDbConStr"];
                    connPassword = AppManager.Configuration["System"]["ResultsDbPassword"];
                    break;
				case Databases.VotingContentData:
					connString = AppManager.Configuration["System"]["ContentDbConStr"];
                    connPassword = AppManager.Configuration["System"]["ContentDbPassword"];
                    break;
				default:
					throw new ArgumentException(string.Format("Databases argument has wrong value={0}", dataBase.ToString()));
			}

            try
            {
                conn = new SQLiteConnection("Data Source=" + connString + ";Password=" + connPassword);
                conn.Open();

                if (conn.State != ConnectionState.Open)
                {
                    Logger.Instance.Write("Can't open database");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public void CloseConnection()
        {
            if (conn != null) conn.Close();
        }

        public DataTable GetData(string query)
        {
            DataSet ds;
            DataTable dt = null;
            SQLiteCommand cmd;
            SQLiteDataReader reader = null;

            try
            {
                cmd = new SQLiteCommand(conn);

                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                
                ds = new DataSet();
                dt = new DataTable();
                ds.EnforceConstraints = false;
                ds.Tables.Add(dt);

                dt.Load(reader);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return dt;
        }

        public bool ChangePass(string pass, bool setconn)
        {           

            try
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();

                conn.ConnectionString = @"Data Source=Ballot_Edit\data\votRiteBallotData_current.s3db";
                conn.Open();
                conn.ChangePassword(pass);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
            
        }
        public bool ChangePass(string pass)
        {

            try
            {
               
                conn.ChangePassword(pass);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }

        }

        public DataTable GetData(string query, IEnumerable<SQLiteParameter> parameters)
        {
            DataTable dt = null;
            SQLiteDataReader reader = null;

            try
            {
                var cmd = new SQLiteCommand(conn) {CommandText = query};
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                cmd.Prepare();
                reader = cmd.ExecuteReader();

                var ds = new DataSet();
                dt = new DataTable();
                ds.EnforceConstraints = false;
                ds.Tables.Add(dt);

                dt.Load(reader);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return dt;
        }

        public object GetScalarData(string query)
        {
            object data = null;
            SQLiteCommand cmd;

            try
            {
                cmd = new SQLiteCommand(conn);

                cmd.CommandText = query;
                data = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }

            return data;
        }

       
        public bool SetData(string query, IEnumerable<SQLiteParameter> parameters)
        {
            var rowsUpdated = 0;

            try
            {
                var cmd = new SQLiteCommand(conn) { CommandText = query };
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                cmd.Prepare();
                rowsUpdated = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(ex);
                return false;
            }

            return true;
        }


        public bool SetData(string query)
        {
            int rowsUpdated = 0;
            SQLiteCommand cmd;

            try
            {
                cmd = new SQLiteCommand(conn);
                cmd.CommandText = query;
                rowsUpdated = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(ex);
            }

            return rowsUpdated > 0;
        }
        
        public DataTable GetDataV2(string query, IEnumerable<SQLiteParameter> parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

                da.Fill(dt);
                //conn.Close();
                da.Dispose();
            } catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }

            return dt;
        }

        public DataTable GetDataV2(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

                da.Fill(dt);
                //conn.Close();
                da.Dispose();
            } catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }

            return dt;
        }

        public static DataTable executeResultDBQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection con = new SQLiteConnection("Data Source=" + Path.Combine( Directory.GetParent(System.Windows.Forms.Application.StartupPath).FullName, "Ballot_Edit", "Data", "votrite.s3db") + ";Password=VotRite2017");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, con);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public static int executeResultDBNonQuery(string query)
        {
            int cnt = 0;
            try
            {
                SQLiteConnection con = new SQLiteConnection("Data Source=" + Path.Combine(Directory.GetParent(System.Windows.Forms.Application.StartupPath).FullName, "Ballot_Edit", "Data", "votrite.s3db") + ";Password=VotRite2017");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            return cnt;
        }

        public static DataTable GetData_audit(string query)
        {
            DataSet ds;
            DataTable dt = null;
            SQLiteCommand cmd;
            SQLiteDataReader reader = null;

            try
            {
                SQLiteConnection con = new SQLiteConnection("Data Source=" + Path.Combine(Directory.GetParent(System.Windows.Forms.Application.StartupPath).FullName, "Ballot_Edit", "Data", "votRiteBallotData_current.s3db") + ";Password=VotRite2017");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd = new SQLiteCommand(con);

                cmd.CommandText = query;
                reader = cmd.ExecuteReader();

                ds = new DataSet();
                dt = new DataTable();
                ds.EnforceConstraints = false;
                ds.Tables.Add(dt);

                dt.Load(reader);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return dt;
        }

        public static object GetScalarData_n(string query)
        {
            object data = null;
            SQLiteCommand cmd;

            try
            {
                SQLiteConnection con = new SQLiteConnection("Data Source=" + Path.Combine(Directory.GetParent(System.Windows.Forms.Application.StartupPath).FullName, "Ballot_Edit", "Data", "votrite.s3db") + ";Password=VotRite2017");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd = new SQLiteCommand(con);

                cmd.CommandText = query;
                data = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                Logger.Instance.Write(query);
                Logger.Instance.Write(e);
            }

            return data;
        }

    }
}
