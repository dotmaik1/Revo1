using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Stages_Report : Form
    {
        public Stages_Report()
        {
            InitializeComponent();
        }

        int well_id = 0;

        string site = "";
        string well = "";
        string user = "";
        string initDate = "";
        string endDate = "";

        string material1 = "";
        string material2 = "";
        string material3 = "";
        string material4 = "";
        string material5 = "";
        string material6 = "";

        string boxname1 = "";
        string boxname2 = "";
        string boxname3 = "";
        string boxname4 = "";
        string boxname5 = "";
        string boxname6 = "";

        decimal delivered1 = 0;
        decimal delivered2 = 0;
        decimal delivered3 = 0;
        decimal delivered4 = 0;
        decimal delivered5 = 0;
        decimal delivered6 = 0;

        decimal delivered = 0;

        decimal goal1 = 0;
        decimal goal2 = 0;
        decimal goal3 = 0;
        decimal goal4 = 0;
        decimal goal5 = 0;
        decimal goal6 = 0;

        decimal goal = 0;

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Stages_Report_Load(object sender, EventArgs e)
        {
            tbInitStage.Text = Properties.Settings.Default.fromRpt;
            tbEndStage.Text = Properties.Settings.Default.toRpt;

            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "select id, site, well, user, material1, material2, material3, material4, material5, material6, " +
                                      "boxname1, boxname2, boxname3, boxname4, boxname5, boxname6 from wells " +
                                      "where site = '" + Properties.Settings.Default.siteRpt + "' and well =  '" + Properties.Settings.Default.wellRpt + "'";

                    //MessageBox.Show(cmd.CommandText);

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            well_id = reader.GetInt32(0);
                            site = reader.GetString(1);
                            well = reader.GetString(2);
                            user = reader.GetString(3);
                            material1 = reader.GetString(4);
                            material2 = reader.GetString(5);
                            material3 = reader.GetString(6);
                            material4 = reader.GetString(7);
                            material5 = reader.GetString(8);
                            material6 = reader.GetString(9);
                            boxname1 = reader.GetString(10);
                            boxname2 = reader.GetString(11);
                            boxname3 = reader.GetString(12);
                            boxname4 = reader.GetString(13);
                            boxname5 = reader.GetString(14);
                            boxname6 = reader.GetString(15);
                        }
                    }


                    cmd.CommandText = "select sum(delivered1), sum(delivered2), sum(delivered3), sum(delivered4), sum(delivered5), sum(delivered6), " +
                    "min(initdate), max(enddate), sum(delivered1) + sum(delivered2) + sum(delivered3) + sum(delivered4) + sum(delivered5) + sum(delivered6)," +
                    "sum(goal1), sum(goal2), sum(goal3), sum(goal4), sum(goal5), sum(goal6), sum(goal1) + sum(goal2) + sum(goal3) + sum(goal4) + sum(goal5) + sum(goal6) " +
                    "from stages where well_id = " + well_id + " and stagenumber BETWEEN " + Properties.Settings.Default.fromRpt + " AND " + Properties.Settings.Default.toRpt;

                    //MessageBox.Show(cmd.CommandText);

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            delivered1 = reader.GetDecimal(0);
                            delivered2 = reader.GetDecimal(1);
                            delivered3 = reader.GetDecimal(2);
                            delivered4 = reader.GetDecimal(3);
                            delivered5 = reader.GetDecimal(4);
                            delivered6 = reader.GetDecimal(5);
                            initDate = reader.GetString(6);
                            endDate = reader.GetString(7);
                            delivered = reader.GetDecimal(8);
                            goal1 = reader.GetDecimal(9);
                            goal2 = reader.GetDecimal(10);
                            goal3 = reader.GetDecimal(11);
                            goal4 = reader.GetDecimal(12);
                            goal5 = reader.GetDecimal(13);
                            goal6 = reader.GetDecimal(14);
                            goal = reader.GetDecimal(15);
                        }
                    }




                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            } // END connections

            tbSite.Text = site;
            tbWell.Text = well;
            tbUser.Text = user;

            tbMaterial1.Text = material1;
            tbMaterial2.Text = material2;
            tbMaterial3.Text = material3;
            tbMaterial4.Text = material4;
            tbMaterial5.Text = material5;
            tbMaterial6.Text = material6;

            lbBOX1.Text = boxname1;
            lbBOX2.Text = boxname2;
            lbBOX3.Text = boxname3;
            lbBOX4.Text = boxname4;
            lbBOX5.Text = boxname5;
            lbBOX6.Text = boxname6;

            tbDelivered1.Text = delivered1.ToString();
            tbDelivered2.Text = delivered2.ToString();
            tbDelivered3.Text = delivered3.ToString();
            tbDelivered4.Text = delivered4.ToString();
            tbDelivered5.Text = delivered5.ToString();
            tbDelivered6.Text = delivered6.ToString();

            tbInitDate.Text = initDate;
            tbEndDate.Text = endDate;

            tbDelivered.Text = delivered.ToString();

            tbTarget1.Text = goal1.ToString();
            tbTarget2.Text = goal2.ToString();
            tbTarget3.Text = goal3.ToString();
            tbTarget4.Text = goal4.ToString();
            tbTarget5.Text = goal5.ToString();
            tbTarget6.Text = goal6.ToString();
            tbTarget.Text = goal.ToString();
        }
    }
}
