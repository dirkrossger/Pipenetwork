using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace ExportCoordinates
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Details> _ReadData;

        public void Filldata()
        {
            clsPipenetwork oNetwork = new clsPipenetwork();
            oNetwork.GetPipenetwork();
            _ReadData = oNetwork.Network;

            try
            {
                if (_ReadData.Count > 0)
                {
                    textBox1.Clear();
                    checkBox1_Listbox.CheckState = CheckState.Unchecked;

                    for (int i = 0; i < _ReadData.Count; i++)
                    {
                        checkedListBox1.Items.Add(_ReadData[i].Name);
                    }
                    label2_Found.Text = string.Format(checkedListBox1.Items.Count + " Pipenets found.");
                }

            }
            catch (System.Exception ex)
            { }
        }

        private void button1_Export_Click(object sender, EventArgs e)
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = acadDoc.Editor;
            //Layer m_Layer = new Layer();
            ObjectIdCollection _SelNetwork = new ObjectIdCollection();

            string name = "";

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                {
                    //name = checkedListBox1.GetItemText(i);
                    name = checkedListBox1.Items[i].ToString();

                    foreach (Details x in _ReadData)
                        if (name == x.Name)
                            _SelNetwork.Add(x.Id);

                }
            }

            Close();

            if (_SelNetwork.Count > 0)
                clsPipenetwork.ExportToExcel(_SelNetwork);
            else
                ed.WriteMessage("\nExport not successful!");


        }

        private void button1_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
