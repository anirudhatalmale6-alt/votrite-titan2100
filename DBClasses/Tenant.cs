using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using VotRite;
using VotRite.Util;

namespace VotRiteBallotDataManager.AppCode
{
    public class Tenant
    {

        public const string ApartmentField = "apartment";
        public const string IdField = "tenant_id";
        public const string NameField = "tenant_name";
        public const string PercentField = "percent";
        public const string SharesField = "shares";
        public const string SharesProductField = "shares_product";
        public const string TableName = "tenants";
        public const string BallotIdField = "ballot_id";

        public int Id { get; set; }
        public string Name { get; set; }
        public float Percent { get; set; }
        public int Shares { get; set; }
        public int SharesProduct { get; set; }
        public string Apartment { get; set; }
        public int BallotId { get; set; }


        public static List<Tenant> GetAllTenants(int ballotId)
        {
            var tenants = new List<Tenant>();
            try
            {
                var dtTenants = DataManager.VotingContentDataInstance.GetData("SELECT * FROM tenants where ballot_id = @ballotid",
                                                             new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotId) });
                if (dtTenants != null)
                {
                    if (dtTenants.Rows.Count > 0)
                    {
                        tenants.AddRange(from DataRow dataRow in dtTenants.Rows
                                         select new Tenant
                                         {
                                             Id = Helper.Cast(dataRow[IdField], -1),
                                             Name = Helper.GetStringValue(dataRow[NameField]),
                                             Apartment =
                                                 Helper.GetStringValue(dataRow[ApartmentField]),
                                             Shares = Helper.Cast(dataRow[SharesField], 0),
                                             Percent =
                                                 Helper.Cast(dataRow[PercentField],
                                                             0f),
                                             SharesProduct =
                                                 Helper.Cast(dataRow[SharesProductField], 0),
                                             BallotId = Helper.Cast(dataRow[BallotIdField], -1)
                                         });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return tenants;
        }

    }
}
