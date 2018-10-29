using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace GetAPILogsfromServers
{
    internal class GetFiles
    {
        private string Line;
        private string URL = null;
        private string[] dateTime = null;
        private string failedRequest = null;
        private string success = null;
        private string failedRequestTime = null;
        private DateTime? failedRequestDateTime = null;
        private string successTime = null;
        private DateTime? successDateTime = null;
        private string[] dates = null;
        private string method = null;
        private DateTime? requestStartDateTime = null;
        private string requestStartTime = null;
        private string pId = null;
        private string tId = null;
        private ArrayList result = null;
        private string[] str;
        private string[] ids = null;
        private TimeSpan timetaken = new TimeSpan(0, 0, 0);
        private DateTime? dateTimestamp = null;
        private string[] splitLines = null;
        private string timetaken1 = null;
        private string[] subscriber = null;
        private string subscriber1 = null;

        public void ParseLogData(StreamReader reader, string machineName)
        {
            try
            {
                GetFiles getDates = new GetFiles();
                while ((!string.IsNullOrEmpty(Line = reader.ReadLine())) && !string.IsNullOrEmpty(reader.ToString()) && Line != "\u0003##### END OF LOG #####")
                {
                    bool succeeded = false;
                    bool failed = false;
                    splitLines = null;

                    splitLines = Regex.Split(Line, @"\s{2,}");

                    if (splitLines[1].ToLower().Contains("authenticate request"))
                    {
                        subscriber = Regex.Split(splitLines[1].ToLower(), "authenticate request");
                        subscriber1 = subscriber[1];
                    }

                    //if (splitLines.Length >= 2 && splitLines[1].ToLower().Contains("url") && splitLines[1].ToLower().Contains("method") && splitLines[1].ToLower().Contains("request started") && !string.IsNullOrEmpty(splitLines.ToString()))
                    //{
                    //    str = Regex.Split(splitLines[1], @", ");

                    //    result = getDates.GetDates(splitLines);
                    //    if (result.Count != 0 && !string.IsNullOrEmpty(result.ToString()))
                    //    {
                    //        requestStartTime = result[3].ToString();
                    //        requestStartDateTime = DateTime.Parse(result[5].ToString());
                    //        //requestStartDateTime = requestStartDateTime.Value.AddMilliseconds(milliseconds);
                    //        pId = result[0].ToString();
                    //        tId = result[1].ToString();
                    //        if (result[4] != null)
                    //            method = result[4].ToString();
                    //        if (result[2] != null)
                    //            URL = str[2].Substring(4, str[2].Length - 4);
                    //    }
                    //}
                    //else if (splitLines.Length >= 2 && requestStartDateTime != null && (splitLines[1].ToLower().Contains("error") || splitLines[1].ToLower().Contains("failed") || splitLines[1].ToLower().Contains("exception")))
                    //{
                    //    result = getDates.GetDates(splitLines);
                    //    if (result.Count != 0 && !string.IsNullOrEmpty(result.ToString()))
                    //    {
                    //        failedRequestTime = result[3].ToString();
                    //        failedRequestDateTime = Convert.ToDateTime(result[4]);
                    //        timetaken = (DateTime.Parse(failedRequestTime.ToString())).Subtract(DateTime.Parse(requestStartTime.ToString()));

                    //        timetaken1 = timetaken.ToString(@"hh\:mm\:ss\:fff");
                    //        //int timetaken = DateTime.Compare(DateTime.Parse(failedRequestDateTime.ToString()), DateTime.Parse(failedRequestDateTime.ToString()));
                    //        failedRequest = "Failed";
                    //        failed = true;
                    //    }
                    //}
                    //else if (splitLines.Length >= 2 && requestStartDateTime != null && (splitLines[1].ToLower().Contains("response recived") || splitLines[1].ToLower().Contains("successfully")))
                    //{
                    //    result = getDates.GetDates(splitLines);
                    //    if (result.Count != 0 && !string.IsNullOrEmpty(result.ToString()))
                    //    {
                    //        successTime = result[3].ToString();
                    //        successDateTime = Convert.ToDateTime(result[4]);
                    //        timetaken = (DateTime.Parse(successTime.ToString())).Subtract(DateTime.Parse(requestStartTime.ToString()));
                    //        timetaken1 = timetaken.ToString(@"hh\:mm\:ss\:fff");
                    //        success = "Success";
                    //        succeeded = true;
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(URL) && (succeeded || failed))
                    //{
                    //
                    //    datalayer.Insert(ref machineName, ref tId, ref pId, ref requestStartDateTime, ref requestStartTime, ref URL, ref method, ref failedRequestDateTime, ref failedRequestTime, ref failedRequest, ref successDateTime, ref successTime, ref success, ref timetaken1);

                    //}
                    if (!string.IsNullOrWhiteSpace(subscriber1))
                    {
                        DataLayer datalayer = new DataLayer();
                        datalayer.Insert(ref machineName, ref subscriber1);
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        public ArrayList GetDates(string[] splitLines)
        {
            ArrayList arrayList = new ArrayList();
            string pid = ""; string tid = "", date = "", time = "", dateTime = "";

            string[] arr = splitLines[1].Split(' ');
            int index = arr[0].LastIndexOf('-');
            if (index != -1)
            {
                date = arr[0].Substring(0, index);
                time = arr[0].Substring(index + 1);
                dateTime = date + " " + time;
            }

            ids = splitLines[0].Split(' ');

            if (ids.Length > 1)
            {
                pid = ids[0];
                tid = ids[1];
            }
            arrayList.AddRange(new string[6]);
            arrayList[0] = pid.ToString(); arrayList[1] = tid.ToString(); arrayList[2] = date.ToString(); arrayList[3] = time.ToString();
            arrayList[4] = arr[8].Substring(0, arr[8].Length - 1).ToString(); arrayList[5] = dateTime;
            return arrayList;
        }
    }
}