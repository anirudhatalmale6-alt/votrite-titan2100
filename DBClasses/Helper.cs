using VotRite;

namespace VotRiteBallotDataManager.AppCode
{
    internal class Helper
    {
        public static string GetStringValue(object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        public static bool Cast(object objToConvert, bool defaultValue)
        {
            bool value;

            return bool.TryParse(GetStringValue(objToConvert), out value) ? value : defaultValue;
        }

        public static int GetMaxId(string tableName, string idField)
        {
            object maxIdObj = DataManager.VotingContentDataInstance.GetScalarData(string.Format("SELECT MAX({0}) FROM {1}",
                                                                        idField,
                                                                        tableName));

            //System.Data.DataTable tbTmp = DataManager.Instance.GetData("select * from ballot");


            return maxIdObj == null || string.IsNullOrEmpty(maxIdObj.ToString()) ? -1 : int.Parse(maxIdObj.ToString());
        }

        public static int Cast(object objToConvert, int defaultValue)
        {
            int value;

            return int.TryParse(GetStringValue(objToConvert), out value) ? value : defaultValue;
        }

        public static float Cast(object objToConvert, float defaultValue)
        {
            float result;
            return (float.TryParse(GetStringValue(objToConvert), out result) ? result : defaultValue);
        }


        /*public static int GetMaxId(string tableName, string idField)
        {
            var maxIdObj =
                DataManager.VotingContentDataInstance.GetScalarData(string.Format("SELECT MAX({0}) FROM {1}",
                                                                                  idField,
                                                                                  tableName));

            return maxIdObj == null || string.IsNullOrEmpty(maxIdObj.ToString()) ? -1 : int.Parse(maxIdObj.ToString());
        }*/


        public static string EscapeStringData(string inputString)
        {
            return string.IsNullOrEmpty(inputString) ? string.Empty : inputString.Replace("'", "''");
        }
    }
}
