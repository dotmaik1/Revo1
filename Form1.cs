using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;
using System.Globalization;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        // Variable para guardar el alert log
        StreamWriter logFile;


        SerialPortManager _spManager;

        // Variables para almacenar lo que llegue del puerto COM y saber si estoy trabajando
        string nuevaTrama = "";

        int checkpointCounter = 1;

        decimal initWeight1 = 0;
        decimal initWeight2 = 0;
        decimal initWeight3 = 0;
        decimal initWeight4 = 0;
        decimal initWeight5 = 0;
        decimal initWeight6 = 0;

        decimal weight1 = 0;
        decimal weight2 = 0;
        decimal weight3 = 0;
        decimal weight4 = 0;
        decimal weight5 = 0;
        decimal weight6 = 0;

        decimal fooCkpt1 = 0;
        decimal fooCkpt2 = 0;
        decimal fooCkpt3 = 0;
        decimal fooCkpt4 = 0;
        decimal fooCkpt5 = 0;
        decimal fooCkpt6 = 0;

        decimal barCkpt1 = 0;
        decimal barCkpt2 = 0;
        decimal barCkpt3 = 0;
        decimal barCkpt4 = 0;
        decimal barCkpt5 = 0;
        decimal barCkpt6 = 0;

        decimal subDelivered1 = 0;
        decimal subDelivered2 = 0;
        decimal subDelivered3 = 0;
        decimal subDelivered4 = 0;
        decimal subDelivered5 = 0;
        decimal subDelivered6 = 0;

        public Form1()
        {
            InitializeComponent();
            UserInitialization();
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            comboBoxCOM.DataSource = mySerialSettings.PortNameCollection;
            //baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            //dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            //parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            //stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            Captura_informacion form = new Captura_informacion();
            form.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            settingsUpdated();
        }

        public void settingsUpdated()
        {
            lbBOXName1.Text = Properties.Settings.Default.BOXName1;
            lbBOXName2.Text = Properties.Settings.Default.BOXName2;
            lbBOXName3.Text = Properties.Settings.Default.BOXName3;
            lbBOXName4.Text = Properties.Settings.Default.BOXName4;
            lbBOXName5.Text = Properties.Settings.Default.BOXName5;
            lbBOXName6.Text = Properties.Settings.Default.BOXName6;

            vpbBOX1.Maximum = Properties.Settings.Default.Max1;
            vpbBOX2.Maximum = Properties.Settings.Default.Max2;
            vpbBOX3.Maximum = Properties.Settings.Default.Max3;
            vpbBOX4.Maximum = Properties.Settings.Default.Max4;
            vpbBOX5.Maximum = Properties.Settings.Default.Max5;
            vpbBOX6.Maximum = Properties.Settings.Default.Max6;
        }

        public void stageConfigured()
        {
            tbSite.Text = Properties.Settings.Default.Site;
            tbWell.Text = Properties.Settings.Default.Well;
            tbUser.Text = Properties.Settings.Default.User;
            tbStage.Text = Properties.Settings.Default.Stage;
            tbTarget.Text = Properties.Settings.Default.Target.ToString();

            ///// TODO: cambair esto no puede estar definido aqui, tiene que ser capturado en cuanto se le da start stage  den4fr   341x
            tbInitDate.Text = Properties.Settings.Default.InitDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            tbMaterial1.Text = Properties.Settings.Default.Material1;
            tbMaterial2.Text = Properties.Settings.Default.Material2;
            tbMaterial3.Text = Properties.Settings.Default.Material3;
            tbMaterial4.Text = Properties.Settings.Default.Material4;
            tbMaterial5.Text = Properties.Settings.Default.Material5;
            tbMaterial6.Text = Properties.Settings.Default.Material6;

            tbSubTarget1.Text = Properties.Settings.Default.Subtarget1.ToString();
            tbSubTarget2.Text = Properties.Settings.Default.Subtarget2.ToString();
            tbSubTarget3.Text = Properties.Settings.Default.Subtarget3.ToString();
            tbSubTarget4.Text = Properties.Settings.Default.Subtarget4.ToString();
            tbSubTarget5.Text = Properties.Settings.Default.Subtarget5.ToString();
            tbSubTarget6.Text = Properties.Settings.Default.Subtarget6.ToString();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //tbTime.Text = DateTime.Now.ToUniversalTime().ToString();
            tbTime.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // TODO: hacer algo para cuando se desconecta de repente el puerto serial .... el try no funciona porque esto ocurre en otra clase
            if (comboBoxCOM.Text != "")
            {
                btnConfigure.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                lbStatus.Text = "Work in progress";

                _spManager.StartListening();

                // Limpia el datagrid de status y agrega las nuevas columnas
                dgStatus.Columns.Clear();
                dgStatus.Rows.Clear();
                dgStatus.Refresh();

                dgStatus.Columns.Add("checkpoint", "Checkpoint");
                dgStatus.Columns.Add("time", "Time");

                // Vuelve el color de las progress bar a gris

                vpbBOX1.Color = Color.Gray;
                vpbBOX2.Color = Color.Gray;
                vpbBOX3.Color = Color.Gray;
                vpbBOX4.Color = Color.Gray;
                vpbBOX5.Color = Color.Gray;
                vpbBOX6.Color = Color.Gray;
            }
            else
            {
                MessageBox.Show("Please select a COM port");
            }
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click_1(object sender, EventArgs e)
        {
            _spManager.StopListening();
            btnConfigure.Enabled = true;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            lbStatus.Text = "Work Done";

            // Se actualiza la tabla de stages con la fecha en que termino y true en finished
            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "update stages set enddate = :enddate, " +
                                                        "termino = 'true', " +
                                                        "delivered1 = :delivered1, " +
                                                        "delivered2 = :delivered2, " +
                                                        "delivered3 = :delivered3, " +
                                                        "delivered4 = :delivered4, " +
                                                        "delivered5 = :delivered5, " +
                                                        "delivered6 = :delivered6 " +
                                                        "where id = :id";

                    cmd.Parameters.Add("enddate", DbType.String).Value = DateTime.Now;
                    cmd.Parameters.Add("delivered1", DbType.String).Value = subDelivered1;
                    cmd.Parameters.Add("delivered2", DbType.String).Value = subDelivered2;
                    cmd.Parameters.Add("delivered3", DbType.String).Value = subDelivered3;
                    cmd.Parameters.Add("delivered4", DbType.String).Value = subDelivered4;
                    cmd.Parameters.Add("delivered5", DbType.String).Value = subDelivered5;
                    cmd.Parameters.Add("delivered6", DbType.String).Value = subDelivered6;
                    cmd.Parameters.Add("id", DbType.String).Value = Properties.Settings.Default.currentStage;

                    cmd.ExecuteNonQuery();
                }
                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }

            // Se reinician todos las variables para la nueva corrida
            initWeight1 = 0;
            initWeight2 = 0;
            initWeight3 = 0;
            initWeight4 = 0;
            initWeight5 = 0;
            initWeight6 = 0;
            weight1 = 0;
            weight2 = 0;
            weight3 = 0;
            weight4 = 0;
            weight5 = 0;
            weight6 = 0;
            fooCkpt1 = 0;
            fooCkpt2 = 0;
            fooCkpt3 = 0;
            fooCkpt4 = 0;
            fooCkpt5 = 0;
            fooCkpt6 = 0;
            barCkpt1 = 0;
            barCkpt2 = 0;
            barCkpt3 = 0;
            barCkpt4 = 0;
            barCkpt5 = 0;
            barCkpt6 = 0;
            subDelivered1 = 0;
            subDelivered2 = 0;
            subDelivered3 = 0;
            subDelivered4 = 0;
            subDelivered5 = 0;
            subDelivered6 = 0;
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }
            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is receibing ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);


            nuevaTrama = nuevaTrama + str;

            tbData.AppendText(str); // Solamente sirve para el debugging
            //tbData.ScrollToCaret();

            // Ejemplo de trama que corrige #@B00.0!#@B01-323000.00%B02-400.36$B03+30.0!#@B01-324000.00%B02-400.36$B03+30.0!#@B01-32   ----> ""
            // TODO: Ver si se puede eliminar porque el siguiente depurador de tramas me empieza siempre las tramas con # que es lo que necesito
            int countGato = nuevaTrama.Split('#').Length - 1;
            int countExclamacion = nuevaTrama.Split('!').Length - 1;
            if (countExclamacion > 1 || countGato > 1)
                nuevaTrama = "";

            // Ejemplo de trama que corrige ($B03+30.0! #@B01+490000.00)      -----> #@B01+490000.00
            int indexOfGato = nuevaTrama.IndexOf('#');
            int indexOfExclamacion = nuevaTrama.IndexOf('!');
            //if (indexOfExclamacion < indexOfGato)
            //    nuevaTrama = nuevaTrama.Substring(indexOfGato); 

            // TODO: mejorar las condiciones para que la trama sea valide
            if (countGato == 1 && countExclamacion == 1)
            {
                if (indexOfExclamacion < indexOfGato)
                {
                    nuevaTrama = nuevaTrama.Substring(indexOfGato); 
                }
                else // (indexOfExclamacion > indexOfGato)
                {
                    // TODO: Sigue habiendo un erro quien sabe porque se arreglo cuando arreglo cuando le agregue el pedo del log
                    // TODO: Hay que agregar un catch para cuando fallan los indexOf y ver como solucionar esa bronca (por ejemplo limpiar la variable linea.... solo un ejemplo)

                    // Se eliminan los espacios en blanco de la trama,,,, me estaba mandando un error cuando por alguna razon entraban espacios en blanco
                    nuevaTrama = nuevaTrama.Replace(" ", "");

                    // Se guarda el log de como van llegando las tramas, de momento no se hace nada....
                    if (Properties.Settings.Default.LogTrama)
                        loguearTrama(nuevaTrama);


                    // TODO: Aqui hay un error pendejo hay que ponerle un try catch
                    int gato = nuevaTrama.IndexOf("#");
                    int arroba = nuevaTrama.IndexOf("@");
                    int porciento = nuevaTrama.IndexOf("%");
                    int pesos = nuevaTrama.IndexOf("$");
                    int exclamacion = nuevaTrama.IndexOf("!");

                    string fisrtBox = nuevaTrama.Substring(arroba + 1, 3);
                    string secondBox = nuevaTrama.Substring(porciento + 1, 3);
                    string thirdBox = nuevaTrama.Substring(pesos + 1, 3);

                    string firstWeight = nuevaTrama.Substring(arroba + 4, porciento - arroba - 4);
                    string secondWeight = nuevaTrama.Substring(porciento + 4, pesos - porciento - 4);
                    string thirdWeight = nuevaTrama.Substring(pesos + 4, exclamacion - pesos - 4);

                    // Log
                    tbData2.AppendText(nuevaTrama + "\r\n");
                    //tbData2.ScrollToCaret();


                    ///// TODO: Este es un workarround en realidad esto se debe de hacer de una mejor manera
                    // Validacion para cuando la bascula esta desconectada ????????? 
                    decimal number;

                    bool result1 = Decimal.TryParse(firstWeight, out number);
                    if (!result1)
                        fisrtBox = fisrtBox + "-NotConnected";
                    bool result2 = Decimal.TryParse(secondWeight, out number);
                    if (!result2)
                        secondBox = secondBox + "-NotConnected";
                    bool result3 = Decimal.TryParse(thirdWeight, out number);
                    if (!result3)
                        thirdBox = thirdBox + "-NotConnected";

                    // Result para los el workarround, el usuario tiene que ver un mensaje de error cuando una de las basculas no esta conectada
                    if (fisrtBox == "B01-NotConnected")
                        tbInitWeight1.Text = "Not connected";
                    if (fisrtBox == "B04-NotConnected")
                        tbInitWeight4.Text = "Not connected";

                    if (secondBox == "B02-NotConnected")
                        tbInitWeight2.Text = "Not connected";
                    if (secondBox == "B05-NotConnected")
                        tbInitWeight5.Text = "Not connected";

                    if (thirdBox == "B03-NotConnected")
                        tbInitWeight3.Text = "Not connected";
                    if (thirdBox == "B06-NotConnected")
                        tbInitWeight6.Text = "Not connected";


                    /*
                     *
                     * fisrtBox BOX=1
                     *
                     */
                    if (fisrtBox == "B01")
                    {
                        if (initWeight1 == 0) // Guarda el peso inicial del box y hace otros calculos
                        {
                            initWeight1 = Convert.ToDecimal(firstWeight) * 1000; // Se obtiene el peso inicial

                            tbInitWeight1.Text = initWeight1.ToString(); // Se muestra el peso inicial al usuario
                            vpbBOX1.Color = Color.Lime; // Colorea el progress bar de lima

                            // Se guarda la informacion para calcular los checkpoints
                            fooCkpt1 = initWeight1;
                            barCkpt1 = 0;

                            // Se agrega las columnas al datagrid
                            dgStatus.Columns.Add("Actual-B01", "Actual Weight " + Properties.Settings.Default.BOXName1);
                            dgStatus.Columns.Add("ToTarget-B01", "Weight Target " + Properties.Settings.Default.BOXName1);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight1", initWeight1);
                        }

                        // Calculos internos 
                        weight1 = Convert.ToDecimal(firstWeight) * 1000; // Se recibe el peso
                        subDelivered1 = initWeight1 - weight1; // Se calcula lo entregado (initial 50,000 - Weight 49,000 = subDelivered 1,000) (Siempre aumenta)
                        barCkpt1 = fooCkpt1 - weight1; // Se guarda el valor del checkpoint que va creciendo ( Ejemplo initial Weight 1 - Weight1 ... (50,000 - 40,000 = 1,000 Hay que hacer un checkpoint))
                        calcularTotalDelivered(); // Calcula el total delivered y se lo muestra al usuario

                        // Despliega informacion al usuario
                        tbActualWeight1.Text = weight1.ToString(); // Se calcula y se muestra el total weight al usuario 
                        tbDelivered1.Text = subDelivered1.ToString(); // Se muestra al usuario lo entregado
                        vpbBOX1.Value = Convert.ToInt32(weight1); // Se muestra el valor actual en el progress bar


                        // Logica de los checkpoints menores a 10,000 lbs
                        if ((Properties.Settings.Default.Subtarget1 - subDelivered1) <= 5000) // Checkpoints menores a 5,000 lbs
                        {
                            if (barCkpt1 >= 1000) // Los checkpoints menores a 5,000 lbs deben ser cada 1,000 lbs
                                checkpointBOX1();
                        }
                        else if ((Properties.Settings.Default.Subtarget1 - subDelivered1) < 10000) // Checkpoints menores a 10,000 lbs (solo tiene que haver uno a los 5, posible erro y no se muestre)
                        {
                            if (barCkpt1 >= 5000)
                                checkpointBOX1();
                        }

                        // checkpoints normales cada 10,000 lbs
                        if (barCkpt1 >= 10000)
                            checkpointBOX1();
                    }

                    /*
                     *
                     * fisrtBox BOX=4
                     *
                     */
                    else if (fisrtBox == "B04")
                    {
                        if (initWeight4 == 0)
                        {
                            initWeight4 = Convert.ToDecimal(firstWeight) * 1000;
                            tbInitWeight4.Text = initWeight1.ToString();
                            vpbBOX4.Color = Color.Lime;
                            fooCkpt4 = initWeight4;
                            barCkpt4 = 0;
                            dgStatus.Columns.Add("Actual-B04", "Actual Weight " + Properties.Settings.Default.BOXName4);
                            dgStatus.Columns.Add("ToTarget-B04", "Weight Target " + Properties.Settings.Default.BOXName4);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight4", initWeight4);
                        }
                        tbActualWeight4.Text = weight4.ToString();
                        weight4 = Convert.ToDecimal(firstWeight) * 1000;
                        subDelivered4 = initWeight4 - weight4;
                        calcularTotalDelivered();
                        tbDelivered4.Text = subDelivered4.ToString();
                        vpbBOX4.Value = Convert.ToInt32(weight4);
                        barCkpt4 = fooCkpt4 - weight4;
                        if ((Properties.Settings.Default.Subtarget4 - subDelivered4) <= 5000)
                        {
                            if (barCkpt4 >= 1000)
                                checkpointBOX4();
                        }
                        else if ((Properties.Settings.Default.Subtarget4 - subDelivered4) < 10000) // Checkpoints menores a 10,000 lbs (solo tiene que haver uno a los 5, posible erro y no se muestre)
                        {
                            if (barCkpt4 >= 5000)
                                checkpointBOX4();
                        }
                        if (barCkpt4 >= 10000)
                            checkpointBOX4();
                    }

                    /*
                     *
                     * secondBox BOX=2
                     *
                     */
                    if (secondBox == "B02")
                    {
                        if (initWeight2 == 0)
                        {
                            initWeight2 = Convert.ToDecimal(secondWeight) * 1000;
                            tbInitWeight2.Text = initWeight2.ToString();
                            fooCkpt2 = initWeight2;
                            barCkpt2 = 0;
                            vpbBOX2.Color = Color.Lime;
                            dgStatus.Columns.Add("Actual-B02", "Actual Weight " + Properties.Settings.Default.BOXName2);
                            dgStatus.Columns.Add("ToTarget-B02", "Weight Target " + Properties.Settings.Default.BOXName2);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight2", initWeight2);
                        }
                        tbActualWeight2.Text = weight2.ToString();
                        weight2 = Convert.ToDecimal(secondWeight) * 1000;
                        subDelivered2 = initWeight2 - weight2;
                        tbDelivered2.Text = subDelivered2.ToString();
                        vpbBOX2.Value = Convert.ToInt32(weight2);
                        calcularTotalDelivered();
                        barCkpt2 = fooCkpt2 - weight2;
                        if ((Properties.Settings.Default.Subtarget2 - subDelivered2) <= 5000)
                        {
                            if (barCkpt2 >= 1000)
                                checkpointBOX2();
                        }
                        else if ((Properties.Settings.Default.Subtarget2 - subDelivered2) < 10000)
                        {
                            if (barCkpt2 >= 5000)
                                checkpointBOX2();
                        }
                        if (barCkpt2 >= 10000)
                            checkpointBOX2();
                    }

                    /*
                     *
                     * secondBox BOX=5
                     *
                     */
                    if (secondBox == "B05")
                    {
                        if (initWeight5 == 0)
                        {
                            initWeight5 = Convert.ToDecimal(secondWeight) * 1000;
                            tbInitWeight5.Text = initWeight5.ToString();
                            vpbBOX5.Color = Color.Lime;
                            fooCkpt5 = initWeight5;
                            barCkpt5 = 0;
                            dgStatus.Columns.Add("Actual-B05", "Actual Weight " + Properties.Settings.Default.BOXName5);
                            dgStatus.Columns.Add("ToTarget-B05", "Weight Target " + Properties.Settings.Default.BOXName5);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight5", initWeight5);
                        }
                        tbActualWeight5.Text = weight5.ToString();
                        weight5 = Convert.ToDecimal(secondWeight) * 1000;
                        subDelivered5 = initWeight5 - weight5;
                        tbDelivered5.Text = subDelivered5.ToString();
                        vpbBOX5.Value = Convert.ToInt32(weight5);
                        calcularTotalDelivered();
                        barCkpt5 = fooCkpt5 - weight5;
                        if ((Properties.Settings.Default.Subtarget5 - subDelivered5) <= 5000)
                        {
                            if (barCkpt5 >= 1000)
                                checkpointBOX5();
                        }
                        else if ((Properties.Settings.Default.Subtarget5 - subDelivered5) < 10000)
                        {
                            if (barCkpt5 >= 5000)
                                checkpointBOX5();
                        }
                        if (barCkpt5 >= 10000)
                            checkpointBOX5();
                    }

                    /*
                     *
                     * thirdBox BOX=3
                     *
                     */
                    if (thirdBox == "B03")
                    {
                        if (initWeight3 == 0)
                        {
                            initWeight3 = Convert.ToDecimal(thirdWeight) * 1000;
                            tbInitWeight3.Text = initWeight3.ToString();
                            vpbBOX3.Color = Color.Lime;
                            fooCkpt3 = initWeight3;
                            barCkpt3 = 0;
                            dgStatus.Columns.Add("Actual-B03", "Actual Weight " + Properties.Settings.Default.BOXName3);
                            dgStatus.Columns.Add("ToTarget-B03", "Weight Target " + Properties.Settings.Default.BOXName3);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight3", initWeight3);
                        }
                        tbActualWeight3.Text = weight3.ToString();
                        weight3 = Convert.ToDecimal(thirdWeight) * 1000;
                        subDelivered3 = initWeight3 - weight3;
                        tbDelivered3.Text = subDelivered3.ToString();
                        vpbBOX3.Value = Convert.ToInt32(weight3);
                        calcularTotalDelivered();
                        barCkpt3 = fooCkpt3 - weight3;
                        if ((Properties.Settings.Default.Subtarget3 - subDelivered3) <= 5000)
                        {
                            if (barCkpt3 >= 1000)
                                checkpointBOX3();
                        }
                        else if ((Properties.Settings.Default.Subtarget3 - subDelivered3) < 10000)
                        {
                            if (barCkpt3 >= 5000)
                                checkpointBOX3();
                        }
                        if (barCkpt3 >= 10000)
                            checkpointBOX3();
                    }

                    /*
                     *
                     * thirdBox BOX=6
                     *
                     */
                    if (thirdBox == "B06")
                    {
                        if (initWeight6 == 0)
                        {
                            initWeight6 = Convert.ToDecimal(thirdWeight) * 1000;
                            tbInitWeight6.Text = initWeight6.ToString();
                            vpbBOX6.Color = Color.Lime;
                            fooCkpt6 = initWeight6;
                            barCkpt6 = 0;
                            dgStatus.Columns.Add("Actual-B06", "Actual Weight " + Properties.Settings.Default.BOXName6);
                            dgStatus.Columns.Add("ToTarget-B06", "Weight Target " + Properties.Settings.Default.BOXName6);

                            // Actualizar el init weight en la tabla de stages
                            updateInitWeight("initweight6", initWeight6);
                        }
                        tbActualWeight6.Text = weight6.ToString();
                        weight6 = Convert.ToDecimal(thirdWeight) * 1000;
                        subDelivered6 = initWeight6 - weight6;
                        tbDelivered6.Text = subDelivered6.ToString();
                        vpbBOX6.Value = Convert.ToInt32(weight6);
                        calcularTotalDelivered();
                        barCkpt6 = fooCkpt6 - weight6;
                        if ((Properties.Settings.Default.Subtarget6 - subDelivered6) <= 5000)
                        {
                            if (barCkpt6 >= 1000)
                                checkpointBOX6();
                        }
                        else if ((Properties.Settings.Default.Subtarget6 - subDelivered6) < 10000)
                        {
                            if (barCkpt6 >= 5000)
                                checkpointBOX6();
                        }
                        if (barCkpt6 >= 10000)
                            checkpointBOX6();
                    }


                    // Se recetea el contenido de la nueva trama para empezar el proceso de nuevo
                    nuevaTrama = "";

                } // End Else (indexOfExclamacion > indexOfGato)
                
            }// End if (countGato == 1 && countExclamacion == 1)

        }

        private void updateInitWeight(string weightName, decimal initWeight1)
        {
            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "update stages set " + weightName + " = :weight where id = :id";

                    cmd.Parameters.Add("weight", DbType.String).Value = initWeight1;
                    cmd.Parameters.Add("id", DbType.String).Value = Properties.Settings.Default.currentStage;

                    cmd.ExecuteNonQuery();
                }
                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
        }

        private void loguearTrama(string nuevaTrama)
        {
            if (!File.Exists("Log.txt"))
            {
                logFile = new StreamWriter("Log.txt");
            }
            else
            {
                logFile = File.AppendText("Log.txt");
            }
            logFile.WriteLine(nuevaTrama);
            logFile.Close();
        }

        private void saveChackpoint1(string weightBOXno, string deliveredBOXno, decimal weight, decimal toTarget)
        {
            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    //parameterized insert - more flexibility on parameter creation
                    cmd.CommandText = "insert into checkpoints (id_stage, " +
                                                                "checkpoint_number, " +
                                                                "timestamp, " +
                                                                weightBOXno + "," +
                                                                deliveredBOXno + ") " +
                                                                "VALUES (@id_stage, " +
                                                                        "@checkpoint_number, " +
                                                                        "@timestamp, " +
                                                                        "@weight, " +
                                                                        "@delivered)";

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@id_stage", Value = Properties.Settings.Default.currentStage });
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@checkpoint_number", Value = checkpointCounter });
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@timestamp", Value = DateTime.Now.ToString("HH:mm:ss") });
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@weight", Value = weight });
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter { ParameterName = "@delivered", Value = toTarget });
                    cmd.ExecuteNonQuery();
                }
                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
        }

        private void checkpointBOX1()
        {
            saveChackpoint1("weight1", "totarget1", weight1, Properties.Settings.Default.Subtarget1 - subDelivered1);

            // Agrega la nueva fila al Stage Status
            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter; // TODO: Revisar la logica porque los checkpoints aqui son dinamicos
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss"); // hora en formato 24 hrs
            row.Cells["Actual-B01"].Value = weight1; // Solamente el peso actual
            row.Cells["ToTarget-B01"].Value = Properties.Settings.Default.Subtarget1 - subDelivered1; // Target = 50,000 subdelivered = 10,000 ; Faltan 40,00

            // Scroll Down cuando se agrega una nueva linea
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;

            // Se modifica el checkpoint
            fooCkpt1 = weight1;
            barCkpt1 = 0;

            // Se incrementa el valor de checkpoint (hay que ver si se puede quitar para hacer mas simple el codigo -> no se alimenta de la DB porque es una metrica que ni usan nosotros la inventamos)
            checkpointCounter++;
        }

        private void checkpointBOX4()
        {
            saveChackpoint1("weight4", "totarget4", weight4, Properties.Settings.Default.Subtarget4 - subDelivered4);

            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter;
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Actual-B04"].Value = weight4;
            row.Cells["ToTarget-B04"].Value = Properties.Settings.Default.Subtarget4 - subDelivered4;
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;
            fooCkpt4 = weight4;
            barCkpt4 = 0;
            checkpointCounter++;
        }

        private void checkpointBOX2()
        {
            saveChackpoint1("weight2", "totarget2", weight2, Properties.Settings.Default.Subtarget2 - subDelivered2);

            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter;
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Actual-B02"].Value = weight2;
            row.Cells["ToTarget-B02"].Value = Properties.Settings.Default.Subtarget2 - subDelivered2;
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;
            fooCkpt2 = weight2;
            barCkpt2 = 0;
            checkpointCounter++;
        }

        private void checkpointBOX6()
        {
            saveChackpoint1("weight6", "totarget6", weight6, Properties.Settings.Default.Subtarget6 - subDelivered6);

            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter;
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Actual-B06"].Value = weight6;
            row.Cells["ToTarget-B06"].Value = Properties.Settings.Default.Subtarget6 - subDelivered6;
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;
            fooCkpt6 = weight6;
            barCkpt6 = 0;
            checkpointCounter++;
        }

        private void checkpointBOX3()
        {
            saveChackpoint1("weight3", "totarget3", weight3, Properties.Settings.Default.Subtarget3 - subDelivered3);

            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter;
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Actual-B03"].Value = weight3;
            row.Cells["ToTarget-B03"].Value = Properties.Settings.Default.Subtarget3 - subDelivered3;
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;
            fooCkpt3 = weight3;
            barCkpt3 = 0;
            checkpointCounter++;
        }

        private void checkpointBOX5()
        {
            saveChackpoint1("weight5", "totarget5", weight5, Properties.Settings.Default.Subtarget5 - subDelivered5);

            int index = dgStatus.Rows.Add();
            DataGridViewRow row = dgStatus.Rows[index];
            row.Cells["checkpoint"].Value = checkpointCounter;
            row.Cells["time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Actual-B05"].Value = weight5;
            row.Cells["ToTarget-B05"].Value = Properties.Settings.Default.Subtarget5 - subDelivered5;
            dgStatus.FirstDisplayedScrollingRowIndex = dgStatus.RowCount - 1;
            fooCkpt5 = weight5;
            barCkpt5 = 0;
            checkpointCounter++;
        }

        private void calcularTotalDelivered()
        {
            // Rutina para sumar todos los valores recibidos en sub delivered y sumarselos al delivered
            decimal val1, val2, val3, val4, val5, val6;

            decimal.TryParse(tbDelivered1.Text, out val1);
            decimal.TryParse(tbDelivered2.Text, out val2);
            decimal.TryParse(tbDelivered3.Text, out val3);
            decimal.TryParse(tbDelivered4.Text, out val4);
            decimal.TryParse(tbDelivered5.Text, out val5);
            decimal.TryParse(tbDelivered6.Text, out val6);

            tbDelivered.Text = (val1 + val2 + val3 + val4 + val5).ToString();
        }


        #region Reportes Tab

        // Variables for this region
        string siteUno = "";
        string siteDos = "";
        string wellUno = "";
        string wellDos = "";


        /// <summary>
        /// Boton activar reportes
        /// </summary>
        private void btnActiveReports_Click(object sender, EventArgs e)
        {
            // Habilita los combobox para reportear
            cbSiteStageRpt.Enabled = true;
            cbSiteWellRpt.Enabled = true;

            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT site from wells";

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbSiteWellRpt.Items.Add(reader.GetString(0));
                            cbSiteStageRpt.Items.Add(reader.GetString(0));
                        }
                    }
                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }


        }

        // Cuando se cambia el contenido de primer combobox (Well report -> Site) se actualiza el contenido del segundo combobox (Well)
        private void cbSiteWellRpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox) sender;
            siteUno = (string) cbSiteWellRpt.SelectedItem;

            // Se habilita el contenido del well rpt combobox
            cbWellWellRpt.Enabled = true; 

            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT well from wells WHERE site = '" + siteUno + "'";

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbWellWellRpt.Items.Add(reader.GetString(0));
                        }
                    }
                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }
        }

        // Index Change Well selected from from report.
        private void cbWellWellRpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            wellUno = (string)cbWellWellRpt.SelectedItem;

            btnWellRpt.Enabled = true;
        }

        private void cbSiteStageRpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            

            ComboBox comboBox = (ComboBox)sender;
            siteDos = (string)cbSiteStageRpt.SelectedItem;

            // Se habilita el contenido del well rpt combobox
            cbWellStageRpt.Enabled = true;

            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT well from wells WHERE site = '" + siteDos + "'";

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbWellStageRpt.Items.Add(reader.GetString(0));
                        }
                    }
                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }
        }

        private void cbWellStageRpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbInitStageRpt.Enabled = true;
            cbEndStageRpt.Enabled = true;

            ComboBox comboBox = (ComboBox)sender;
            wellDos = (string)cbWellStageRpt.SelectedItem;

            int wellId = 0;

            var connString = string.Format(@"Data Source=development.db; Pooling=false; FailIfMissing=false;");

            using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id from wells WHERE site = '" + siteDos + "' and well = '" + wellDos + "'";

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            wellId = reader.GetInt32(0);
                        }
                    }

                    cmd.CommandText = "select stagenumber from stages where well_id = " + wellId;

                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbInitStageRpt.Items.Add(reader.GetString(0));
                            cbEndStageRpt.Items.Add(reader.GetString(0));
                        }
                    }

                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }
        }

        // Boton de reportes por well
        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(siteUno + " " + wellUno);

            // Guardamos la informacion en los settings para utilizarla en la forma de reportes por WELL
            Properties.Settings.Default.wellRpt = wellUno;
            Properties.Settings.Default.siteRpt = siteUno;

            Locations_Report form = new Locations_Report();
            form.Show();
        }

        // Boton de reportes por stage
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.siteRpt = siteDos;
            Properties.Settings.Default.wellRpt = wellDos;
            Properties.Settings.Default.fromRpt = cbInitStageRpt.Text;
            Properties.Settings.Default.toRpt = cbEndStageRpt.Text;

            //MessageBox.Show(Properties.Settings.Default.wellRpt + " " + Properties.Settings.Default.siteRpt + " " +
            //    Properties.Settings.Default.fromRpt + " " + Properties.Settings.Default.toRpt);

            Stages_Report form = new Stages_Report();
            form.Show();
        }


        #endregion

        private void comboBoxCOM_Click(object sender, EventArgs e)
        {
            UserInitialization();
        }

        private void cbEndStageRpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnStageRpt.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form = new About();
            form.Show();
        }

        


    }
}
