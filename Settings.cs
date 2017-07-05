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
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // Carga el nombre de cada BOX desde los settings
            tbName1.Text = Properties.Settings.Default.BOXName1;
            tbName2.Text = Properties.Settings.Default.BOXName2;
            tbName3.Text = Properties.Settings.Default.BOXName3;
            tbName4.Text = Properties.Settings.Default.BOXName4;
            tbName5.Text = Properties.Settings.Default.BOXName5;
            tbName6.Text = Properties.Settings.Default.BOXName6;

            // Carga los pesos maximos desde los settings
            tbMax1.Text = Properties.Settings.Default.Max1.ToString();
            tbMAX2.Text = Properties.Settings.Default.Max2.ToString();
            tbMAX3.Text = Properties.Settings.Default.Max3.ToString();
            tbMAX4.Text = Properties.Settings.Default.Max4.ToString();
            tbMAX5.Text = Properties.Settings.Default.Max5.ToString();
            tbMAX6.Text = Properties.Settings.Default.Max6.ToString();

            tbMaterial.Text = Properties.Settings.Default.MaterialList;

            cbMaterial1.Text = Properties.Settings.Default.Material1;
            cbMaterial2.Text = Properties.Settings.Default.Material2;
            cbMaterial3.Text = Properties.Settings.Default.Material3;
            cbMaterial4.Text = Properties.Settings.Default.Material4;
            cbMaterial5.Text = Properties.Settings.Default.Material5;
            cbMaterial6.Text = Properties.Settings.Default.Material6;

            tbUser.Text = Properties.Settings.Default.User;

            // Carga la lista de materiales desde los settings
            //tbMaterial.Text = Helpers.ObjectFromString(Properties.Settings.Default.MaterialList) as string[];
            //tbMaterial.Text

            actulizarValoresDeTexboxMaterial();
        }

        private void actulizarValoresDeTexboxMaterial()
        {
            cbMaterial1.Items.Clear();
            cbMaterial2.Items.Clear();
            cbMaterial3.Items.Clear();
            cbMaterial4.Items.Clear();
            cbMaterial5.Items.Clear();
            cbMaterial6.Items.Clear();


            // Convierte la lista de materiales en lineas separadas para agregarlas al material combobox de cada BOX
            string linesWithoutSplit = Properties.Settings.Default.MaterialList;
            foreach (var material in linesWithoutSplit.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                cbMaterial1.Items.Add(material);
                cbMaterial2.Items.Add(material);
                cbMaterial3.Items.Add(material);
                cbMaterial4.Items.Add(material);
                cbMaterial5.Items.Add(material);
                cbMaterial6.Items.Add(material);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Guarda el nombre de cada BOX desde los settings
            Properties.Settings.Default.BOXName1 = tbName1.Text;
            Properties.Settings.Default.BOXName2 = tbName2.Text;
            Properties.Settings.Default.BOXName3 = tbName3.Text;
            Properties.Settings.Default.BOXName4 = tbName4.Text;
            Properties.Settings.Default.BOXName5 = tbName5.Text;
            Properties.Settings.Default.BOXName6 = tbName6.Text;

            // Guarda los pesos maximos desde los settings
            Properties.Settings.Default.Max1 = Int32.Parse(tbMax1.Text);
            Properties.Settings.Default.Max2 = Int32.Parse(tbMAX2.Text);
            Properties.Settings.Default.Max3 = Int32.Parse(tbMAX3.Text);
            Properties.Settings.Default.Max4 = Int32.Parse(tbMAX4.Text);
            Properties.Settings.Default.Max5 = Int32.Parse(tbMAX5.Text);
            Properties.Settings.Default.Max6 = Int32.Parse(tbMAX6.Text);

            Properties.Settings.Default.Material1 = cbMaterial1.Text;
            Properties.Settings.Default.Material2 = cbMaterial2.Text;
            Properties.Settings.Default.Material3 = cbMaterial3.Text;
            Properties.Settings.Default.Material4 = cbMaterial4.Text;
            Properties.Settings.Default.Material5 = cbMaterial5.Text;
            Properties.Settings.Default.Material6 = cbMaterial6.Text;

            Properties.Settings.Default.User = tbUser.Text;

            // Salva definitivamente los settings
            Properties.Settings.Default.Save();

            // Manda a llamar el metodo update que actualiza la information del Form1
            Form1 form = Application.OpenForms["Form1"] as Form1;
            form.settingsUpdated();

            this.Close();
        }

        private void tbMaterial_Leave(object sender, EventArgs e)
        {
            // Guarda la lista de materiales desde los settings
            Properties.Settings.Default.MaterialList = tbMaterial.Text;

            Properties.Settings.Default.Save();

            actulizarValoresDeTexboxMaterial();
        }
    }
}
