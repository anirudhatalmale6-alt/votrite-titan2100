using System;
using System.Data;
using System.Collections.Generic;

using VotRite;

namespace VotRiteBallotDataManager.AppCode
{
    class County
    {
        List<County> counties;

        public int Id { get; set; }
        public string State { get; set; }
        public string Name { get; set; }

        public List<County> GetCounties(int ballotId)
        {
            counties = new List<County>();

            string query = string.Format("select county_id, state, county from county " +
                "where county_id in (select county_id from ballot_county where ballot_id={0})",
                ballotId);

            try
            {
                DataTable dt = DataManager.VotingContentDataInstance.GetDataV2(query);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        County county;

                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            county = new County
                            {
                                Id = Helper.Cast(dt.Rows[i][0], -1),
                                State = (string)dt.Rows[i][1],
                                Name = (string)dt.Rows[i][2]
                            };

                            if (!counties.Contains(county))
                            {
                                counties.Add(county);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return counties;
        }

        public static List<County> GetCounties(int ballotId, int localeId)
        {
            List<County> counties = new List<County>();

            string query = string.Format("select county_id, state, county from county " +
                "where county_id in (select county_id from ballot_county where ballot_id={0})",
                ballotId);

            try
            {
                DataTable dt = DataManager.VotingContentDataInstance.GetDataV2(query);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        County county;

                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            county = new County
                            {
                                Id = Helper.Cast(dt.Rows[i][0], -1),
                                State = (string)dt.Rows[i][1],
                                Name = (string)dt.Rows[i][2]
                            };

                            if (!counties.Contains(county))
                            {
                                counties.Add(county);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return counties;
        }

        public static County GetCounty(int id)
        {
            County c = null;

            string query = string.Format(
                "select county_id, state, county from county " +
                "where county_id = {0}",
                id);

            try
            {
                DataTable dt = DataManager.VotingContentDataInstance.GetDataV2(query);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            c = new County
                            {
                                Id = Helper.Cast(dt.Rows[i][0], -1),
                                State = (string)dt.Rows[i][1],
                                Name = (string)dt.Rows[i][2]
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return c;
        }
    }
}
