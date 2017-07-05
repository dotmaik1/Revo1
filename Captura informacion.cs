using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Captura_informacion : Form
    {
        public Captura_informacion()
        {
            InitializeComponent();
        }

        private void Captura_informacion_Load(object sender, EventArgs e)
        {
            tbSite.Text = Properties.Settings.Default.Site;
            tbWell.Text = Properties.Settings.Default.Well;
            tbUser.Text = Properties.Settings.Default.User;
            tbStage.Text = Properties.Settings.Default.Stage;
            tbStage.Text = Properties.Settings.Default.Stage;

            tbMaterial1.Text = Properties.Settings.Default.Material1;
            tbMaterial2.Text = Properties.Settings.Default.Material2;
            tbMaterial3.Text = Properties.Settings.Default.Material3;
            tbMaterial4.Text = Properties.Settings.Default.Material4;
            tbMaterial5.Text = Properties.Settings.Default.Material5;
            tbMaterial6.Text = Properties.Settings.Default.Material6;

            // Convierte la lista de materiales en lineas separadas para agregarlas al material combobox de cada BOX
            /*string linesWithoutSplit = Properties.Settings.Default.MaterialList;
            foreach(var material in linesWithoutSplit.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                cbMaterial1.Items.Add(material);
                cbMaterial2.Items.Add(material);
                cbMaterial3.Items.Add(material);
                cbMaterial4.Items.Add(material);
                cbMaterial5.Items.Add(material);
                cbMaterial6.Items.Add(material);
            }*/
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Site = tbSite.Text;
            Properties.Settings.Default.Well = tbWell.Text;
            Properties.Settings.Default.Stage = tbStage.Text;

            Properties.Settings.Default.Subtarget1 = nupSubTarget1.Value;
            Properties.Settings.Default.Subtarget2 = nupSubTarget2.Value;
            Properties.Settings.Default.Subtarget3 = nupSubTarget3.Value;
            Properties.Settings.Default.Subtarget4 = nupSubTarget4.Value;
            Properties.Settings.Default.Subtarget5 = nupSubTarget5.Value;
            Properties.Settings.Default.Subtarget6 = nupSubTarget6.Value;

            Properties.Settings.Default.Target = Convert.ToDecimal(tbTarget.Text);
            Properties.Settings.Default.InitDate = DateTime.Now;

            // Salva definitivamente los settings
            Properties.Settings.Default.Save();

            //TODO: Como le voy a hacer con la sequencia ????? La secuancia solo sirve para pintar las grafiquitas, asi que no mameis
            //cbMaterial1;
            //tbSubTarget1;
            //Sequence1;

            // Guarda el stage en la base de datos
            saveStage();
            //this.Close();
        }

        private void saveStage()
        {
            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "select id from wells where site = '" + tbSite.Text + "' and well =  '" + tbWell.Text + "'";
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    if (id == 0)
                    {
                        // Si no existe hay que agregarlo a la DB
                        cmd.CommandText = "insert into wells (site, " +
                                                            "well, " +
                                                            "user, " +
                                                            "material1, " +
                                                            "material2, " +
                                                            "material3, " +
                                                            "material4, " +
                                                            "material5, " +
                                                            "material6, " +
                                                            "boxname1, " +
                                                            "boxname2, " +
                                                            "boxname3, " +
                                                            "boxname4, " +
                                                            "boxname5, " +
                                                            "boxname6)" +
                                                            "VALUES (@site, " +
                                                                    "@well, " +
                                                                    "@user, " +
                                                                    "@material1, " +
                                                                    "@material2, " +
                                                                    "@material3, " +
                                                                    "@material4, " +
                                                                    "@material5, " +
                                                                    "@material6, " +
                                                                    "@boxname1, " +
                                                                    "@boxname2, " +
                                                                    "@boxname3, " +
                                                                    "@boxname4, " +
                                                                    "@boxname5, " +
                                                                    "@boxname6)";

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@site", Value = tbSite.Text });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@well", Value = tbWell.Text });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@user", Value = Properties.Settings.Default.User });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material1", Value = Properties.Settings.Default.Material1 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material2", Value = Properties.Settings.Default.Material2 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material3", Value = Properties.Settings.Default.Material3 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material4", Value = Properties.Settings.Default.Material4 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material5", Value = Properties.Settings.Default.Material5 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@material6", Value = Properties.Settings.Default.Material6 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname1", Value = Properties.Settings.Default.BOXName1 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname2", Value = Properties.Settings.Default.BOXName2 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname3", Value = Properties.Settings.Default.BOXName3 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname4", Value = Properties.Settings.Default.BOXName4 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname5", Value = Properties.Settings.Default.BOXName5 });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@boxname6", Value = Properties.Settings.Default.BOXName6 });

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"SELECT last_insert_rowid()";

                        // Guardamos el numero de fila en "currentWell".
                        Properties.Settings.Default.currentWell = Convert.ToInt32(cmd.ExecuteScalar());
                        //MessageBox.Show(Properties.Settings.Default.currentWell.ToString());


                    }
                    else
                    {
                        // Si existe se agarra ese id para insertar datos en el nuevo stage.
                        Properties.Settings.Default.currentWell = Convert.ToInt32(id);
                        //MessageBox.Show("la fila es:" + Properties.Settings.Default.currentWell.ToString());

                        // Actualizamos el nombre de los BOX y del material en la tabla wells.
                        cmd.CommandText = "update wells set user = :user, " +
                                                            "material1 = :material1, " +
                                                            "material2 = :material2, " +
                                                            "material3 = :material3, " +
                                                            "material4 = :material4, " +
                                                            "material5 = :material5, " +
                                                            "material6 = :material6, " +
                                                            "boxname1 = :boxname1, " +
                                                            "boxname2 = :boxname2, " +
                                                            "boxname3 = :boxname3, " +
                                                            "boxname4 = :boxname4, " +
                                                            "boxname5 = :boxname5, " +
                                                            "boxname6 = :boxname6 " +
                                                            "where id = :id";

                        cmd.Parameters.Add("user", DbType.String).Value = Properties.Settings.Default.User;
                        cmd.Parameters.Add("material1", DbType.String).Value = Properties.Settings.Default.Material1;
                        cmd.Parameters.Add("material2", DbType.String).Value = Properties.Settings.Default.Material2;
                        cmd.Parameters.Add("material3", DbType.String).Value = Properties.Settings.Default.Material3;
                        cmd.Parameters.Add("material4", DbType.String).Value = Properties.Settings.Default.Material4;
                        cmd.Parameters.Add("material5", DbType.String).Value = Properties.Settings.Default.Material5;
                        cmd.Parameters.Add("material6", DbType.String).Value = Properties.Settings.Default.Material6;
                        cmd.Parameters.Add("boxname1", DbType.String).Value = Properties.Settings.Default.BOXName1;
                        cmd.Parameters.Add("boxname2", DbType.String).Value = Properties.Settings.Default.BOXName2;
                        cmd.Parameters.Add("boxname3", DbType.String).Value = Properties.Settings.Default.BOXName3;
                        cmd.Parameters.Add("boxname4", DbType.String).Value = Properties.Settings.Default.BOXName4;
                        cmd.Parameters.Add("boxname5", DbType.String).Value = Properties.Settings.Default.BOXName5;
                        cmd.Parameters.Add("boxname6", DbType.String).Value = Properties.Settings.Default.BOXName6;
                        cmd.Parameters.Add("id", DbType.String).Value = Properties.Settings.Default.currentWell;

                        cmd.ExecuteNonQuery();

                        //MessageBox.Show("haber que pedo");

                    }
                    // Hay que checar que el stage number no exista si existe hay que desplegarle un mensaje al usuario y que meta otro stage... porque porque si
                    cmd.CommandText = "select id from stages where well_id = '" + Properties.Settings.Default.currentWell + "' and stagenumber =  '" + tbStage.Text + "'";
                    int id_stage = Convert.ToInt32(cmd.ExecuteScalar());


                    if (id_stage == 0) // Si no existe hay que agregarlo
                    {
                        // Se guarda la informacion del stage
                        cmd.CommandText = "insert into stages (well_id, " +
                                                                "stagenumber, " +
                                                                "goal1, " +
                                                                "goal2, " +
                                                                "goal3, " +
                                                                "goal4, " +
                                                                "goal5, " +
                                                                "goal6, " +
                                                                "initdate)" +
                                                                "VALUES (@well_id, " +
                                                                        "@stagenumber, " +
                                                                        "@goal1, " +
                                                                        "@goal2, " +
                                                                        "@goal3, " +
                                                                        "@goal4, " +
                                                                        "@goal5, " +
                                                                        "@goal6, " +
                                                                        "@initdate)";

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@well_id", Value = Properties.Settings.Default.currentWell });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@stagenumber", Value = tbStage.Text });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal1", Value = nupSubTarget1.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal2", Value = nupSubTarget2.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal3", Value = nupSubTarget3.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal4", Value = nupSubTarget4.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal5", Value = nupSubTarget5.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@goal6", Value = nupSubTarget6.Value });
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@initdate", Value = DateTime.Now });

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"SELECT last_insert_rowid()";
                        cmd.ExecuteNonQuery();
                        Properties.Settings.Default.currentStage = Convert.ToInt32(cmd.ExecuteScalar());

                        // Manda a llamar el metodo update que actualiza la information del Form1
                        Form1 form = Application.OpenForms["Form1"] as Form1;
                        form.stageConfigured();

                        // Se cierra la forma para poder trabajar en el nuevo stage
                        this.Close();
                    }
                    else // Si si existe le desplegamos un error al usuario
                    {
                        // TODO: validar que el stage ste cerrado, si no lo esta seleccionar ese stage como current stage o mandar al usuario a unifinished stages
                        MessageBox.Show("The stage was previously used, please insert a diferent stage");
                    }
                }
                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Se ejecuta cuando el valor de cualquier sub target cambia
        private void nupSubTarget6_ValueChanged(object sender, EventArgs e)
        {
            empyToZero(); // Pone un 0 si el numeric up down es null

            // Calcula y asigna el target
            tbTarget.Text = Convert.ToString(nupSubTarget1.Value + nupSubTarget2.Value + nupSubTarget2.Value +
                nupSubTarget3.Value + nupSubTarget4.Value + nupSubTarget5.Value + nupSubTarget6.Value);
        }

        private void nupSubTarget1_KeyUp(object sender, KeyEventArgs e)
        {
            empyToZero(); // Pone un 0 si el numeric up down es null

            // Calcula y asigna el target
            tbTarget.Text = Convert.ToString(nupSubTarget1.Value + nupSubTarget2.Value + nupSubTarget2.Value + 
                nupSubTarget3.Value + nupSubTarget4.Value + nupSubTarget5.Value + nupSubTarget6.Value);
        }

        /// <summary>
        /// Se encarga de que los numeric up downs (subtargets) nunca sean nullos // Pone un 0 si el numeric up down es null
        /// </summary>
        private void empyToZero()
        {
            if (nupSubTarget1.Text == "")
                nupSubTarget1.Text = "0";
            if (nupSubTarget2.Text == "")
                nupSubTarget2.Text = "0";
            if (nupSubTarget3.Text == "")
                nupSubTarget3.Text = "0";
            if (nupSubTarget4.Text == "")
                nupSubTarget4.Text = "0";
            if (nupSubTarget5.Text == "")
                nupSubTarget5.Text = "0";
            if (nupSubTarget6.Text == "")
                nupSubTarget6.Text = "0";
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tbSite_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbWell_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }
}
