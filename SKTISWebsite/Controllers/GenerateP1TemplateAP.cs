using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeAPClose;
using HMS.SKTIS.Utils;
using HMS.SKTIS.Core;

namespace SKTISWebsite.Controllers
{
    public class GenerateP1TemplateAP
    {
        public string getP1GL(DateTime Date, int Week, int Year, string Location)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["SKTISEntitiesADO"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "GenerateP1TemplateGL1";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.Parameters.Add(new SqlParameter("@ParamdateTo", Date));
            cmd.Parameters.Add(new SqlParameter("@ParamWeek", Week));
            cmd.Parameters.Add(new SqlParameter("@paramYear", Year));
            cmd.Parameters.Add(new SqlParameter("@ParamLocation", Location));

            conn.Open();

            reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    sb.Append(dr[i].ToString());
                    if (i < dt.Columns.Count - 1)
                        sb.Append("\t");
                }
                sb.AppendLine();
            }
            conn.Close();
            return sb.ToString();
        }
        public string getP1(string Location, int Week, int Year){
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["SKTISEntitiesADO"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "GenerateP1TemplateAP1";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.Parameters.Add(new SqlParameter("@ParamLocation", Location));
            cmd.Parameters.Add(new SqlParameter("@ParamWeek", Week));
            cmd.Parameters.Add(new SqlParameter("@ParamYear", Year));
            cmd.Parameters.Add(new SqlParameter("@ParamPage", "APOPEN"));

            conn.Open();

            reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            var counter = 1;
            var counter2 = 1;
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                // insert enter after header
                if (counter == 2)
                {
                    //sb.AppendLine();
                    counter2 = 1;
                }

                // insert enter after every location
                if (counter2 == 6)
                {
                    //sb.AppendLine();
                    counter2 = 1;
                }
                counter2++;

                for (int i = 5; i < dt.Columns.Count; i++)
                {
                        
                    sb.Append(dr[i].ToString());

                    if (i < dt.Columns.Count - 1)
                        sb.Append("\t");
                }
                    
                sb.AppendLine();
                counter++;
            }
            conn.Close();
            return sb.ToString();
        }

        public string getP1Close(P1Template<TPOFeeAPCloseViewModel> bulkData, string Location, int Week, int Year)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["SKTISEntitiesADO"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "GenerateP1TemplateAP1";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.Parameters.Add(new SqlParameter("@ParamLocation", Location));
            cmd.Parameters.Add(new SqlParameter("@ParamWeek", Week));
            cmd.Parameters.Add(new SqlParameter("@ParamYear", Year));
            cmd.Parameters.Add(new SqlParameter("@ParamPage", "APCLOSE"));

            conn.Open();

            reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            var counter = 0;
            int countColumn = 0;
            string loc = "";
            string findLoc = "";
            
            StringBuilder sb = new StringBuilder();
            
                for (int j = 0; j < bulkData.Data.Count; j++)
                {
                    findLoc = bulkData.Data[j].LocationCode;
                    if (bulkData.Data[j].Check == true)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //=======================================================================================================================
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                loc = dr[0].ToString();
                                if (counter == 0)
                                {
                                    //write header
                                    if (i >= 4)
                                    {
                                        sb.Append(dr[i].ToString());

                                        if (i < dt.Columns.Count - 1)
                                            sb.Append("\t");
                                    }
                                    //write header}
                                }
                                else
                                {

                                    if (countColumn == 0 && dr[i].ToString() != findLoc)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        //write value
                                        if (i >= 4)
                                        {
                                            sb.Append(dr[i].ToString());

                                            if (i < dt.Columns.Count - 1)
                                            {
                                                sb.Append("\t");
                                            }
                                        }
                                        //write value
                                    }
                                    countColumn = 1;
                                }
                            }
                            if (counter == 0 || loc == findLoc)
                            {
                                sb.AppendLine();

                            }
                            counter++;
                            //=======================================================================================================================
                            countColumn = 0;
                        }
                    }
                    

                }
            conn.Close();
            return sb.ToString();
        }
    }
}