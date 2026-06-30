using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using VotRite;

namespace VotRiteBallotDataManager.AppCode
{
    public class MachineConfig : IDisposable
    {
        public int SN { get; set; }
        public string MachineNo { get; set; }
        public string Location { get; set; }
        public string MainLocation { get; set; }
        public  string BackupDrive { get; set; }
        public int TotalBallots { get; set; }

        public void Dispose()
        {            
            GC.SuppressFinalize(this);
        }

        public static List<MachineConfig> GetMachineConfigs()
        {
            List<MachineConfig> list = new List<MachineConfig>();
            try
            {
                string query = "select *,(Select count(distinct ballot_id) from MachinBallot where Machine_SN = MachinConfig.SN) as TotalBallots from MachinConfig";
                var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            var item = new MachineConfig
                            {
                                SN = Convert.ToInt32(dataRow["SN"].ToString()),
                                MachineNo = dataRow["MachineNo"].ToString(),
                                Location = dataRow["Location"].ToString(),
                                MainLocation = dataRow["MainLocation"].ToString(),
                                BackupDrive = dataRow["BackupDrive"].ToString(),
                                TotalBallots = Convert.ToInt32(dataRow["TotalBallots"].ToString())
                            };
                            list.Add(item);
                        }
                    }
                }
                        }
            catch (Exception)
            {
            }
            return list;
        }

        public static List<MachineConfig> GetMachineConfig(string MachineNo)
        {
            List<MachineConfig> list = new List<MachineConfig>();
            try
            {
                string query = "select *,(Select count(distinct ballot_id) from MachinBallot where Machine_SN = MachinConfig.SN) as TotalBallots from MachinConfig where MachineNo=@MachineNo";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@MachineNo", Value = MachineNo });
                var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query, sqlParameters);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            var item = new MachineConfig
                            {
                                SN = Convert.ToInt32(dataRow["SN"].ToString()),
                                MachineNo = dataRow["MachineNo"].ToString(),
                                Location = dataRow["Location"].ToString(),
                                MainLocation = dataRow["MainLocation"].ToString(),
                                BackupDrive = dataRow["BackupDrive"].ToString(),
                                TotalBallots = Convert.ToInt32(dataRow["TotalBallots"].ToString())
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static List<MachineConfig> GetMachineConfig(int SN)
        {
            List<MachineConfig> list = new List<MachineConfig>();
            try
            {
                string query = "select *,(Select count(distinct ballot_id) from MachinBallot where Machine_SN = MachinConfig.SN) as TotalBallots from MachinConfig where SN=@SN";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@SN", Value = SN });
                var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query, sqlParameters);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            var item = new MachineConfig
                            {
                                SN = Convert.ToInt32(dataRow["SN"].ToString()),
                                MachineNo = dataRow["MachineNo"].ToString(),
                                Location = dataRow["Location"].ToString(),
                                MainLocation = dataRow["MainLocation"].ToString(),
                                BackupDrive = dataRow["BackupDrive"].ToString(),
                                TotalBallots = Convert.ToInt32(dataRow["TotalBallots"].ToString())
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static bool InsertMachineConfig(MachineConfig mc)
        {
            try
            {
                string query = "Insert into MachinConfig(MachineNo,Location,BackupDrive,MainLocation) values (@MachineNo,@Location,@BackupDrive,@MainLocation)";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@MachineNo", Value = mc.MachineNo });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@Location", Value = mc.Location });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@BackupDrive", Value = mc.BackupDrive });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@MainLocation", Value = mc.MainLocation });
                return DataManager.VotingContentDataInstance.SetData(query, sqlParameters);                
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public static bool UpdateMachineConfig(MachineConfig mc)
        {
            try
            {
                string query = "Update MachinConfig Set MachineNo=@MachineNo,MainLocation=@MainLocation,Location=@Location,BackupDrive=@BackupDrive where SN=@SN";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@MachineNo", Value = mc.MachineNo });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@Location", Value = mc.Location });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@MainLocation", Value = mc.MainLocation });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@BackupDrive", Value = mc.BackupDrive });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@SN", Value = mc.SN });
                return DataManager.VotingContentDataInstance.SetData(query, sqlParameters);
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool RemoveMachineConfig(int SN)
        {            
            try
            {
                string query = "Delete from MachinConfig where SN=@SN";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@SN", Value = SN });
                bool done = DataManager.VotingContentDataInstance.SetData(query, sqlParameters);

                MachineBallot.RemoveMachineBallots(SN);

                return done;

            }
            catch (Exception)
            {
                return false;
            }
            
        }

    }
    public class MachineBallot
    {
        public int Machine_SN { get; set; }
        public int Ballot_ID { get; set; }

        public static List<MachineBallot> GetMachineBallots(int SN)
        {
            List<MachineBallot> list = new List<MachineBallot>();
            try
            {
                string query = "select * from MachinBallot where Machine_SN=@SN";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@SN", Value = SN });
                var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query, sqlParameters);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            var item = new MachineBallot
                            {
                                Machine_SN = Convert.ToInt32(dataRow["Machine_SN"].ToString()),
                                Ballot_ID = Convert.ToInt32(dataRow["Ballot_ID"].ToString())
                               
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static List<MachineBallot> GetAllMachineBallots()
        {
            List<MachineBallot> list = new List<MachineBallot>();
            try
            {
                string query = "select * from MachinBallot";
               var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            var item = new MachineBallot
                            {
                                Machine_SN = Convert.ToInt32(dataRow["Machine_SN"].ToString()),
                                Ballot_ID = Convert.ToInt32(dataRow["Ballot_ID"].ToString())

                            };
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static bool InsertMachineBallots( MachineBallot mb)
        {
            try
            {
                string query = "Insert into MachinBallot(Machine_SN,Ballot_ID) values (@Machine_SN,@Ballot_ID)";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@Machine_SN", Value = mb.Machine_SN });
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@Ballot_ID", Value = mb.Ballot_ID });               
                return DataManager.VotingContentDataInstance.SetData(query, sqlParameters);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RemoveMachineBallots(int SN)
        {
            try
            {
                string query = "Delete from MachinBallot where Machine_SN=@Machine_SN";
                var sqlParameters = new List<SQLiteParameter>();
                sqlParameters.Add(new SQLiteParameter() { ParameterName = "@Machine_SN", Value = SN });
                return DataManager.VotingContentDataInstance.SetData(query, sqlParameters);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
