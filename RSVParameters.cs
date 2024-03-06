using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ThrusterTest.UserControls
{
    public partial class RSVParameters : UserControl
    {
        private static string path = AppDomain.CurrentDomain.BaseDirectory;
        private static string filepath = Path.Combine(path, "profiles.xml");
        private bool button1WasClicked = false;

        public RSVParameters()
        {

            InitializeComponent();
            XmlDocument doc = new XmlDocument();
            doc.Load("profiles.xml");

            foreach (XmlNode node in doc.SelectNodes("//Profile/Name"))
            {
                cmbProfiles.Items.Add(node.InnerText);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                XmlDocument doc = new XmlDocument();

                // Check if the XML file exists
                if (File.Exists(filepath))
                {
                    doc.Load(filepath);
                }
                else
                {
                    XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(declaration);

                    XmlElement root = doc.CreateElement("Profiles");
                    doc.AppendChild(root);
                }
                string profileName = txtProfileName.Text;
                if (ProfileExists(doc, profileName))
                {
                    MessageBox.Show("Profile name already exists! Please enter another name.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the function if profile name already exists
                }

                XmlElement profile = doc.CreateElement("Profile");

                // Save data from each text box to the XML file
                SaveDataElement(doc, profile, "Name", txtProfileName.Text);
                SaveDataElement(doc, profile, "Draft", txtDraft.Text);
                SaveDataElement(doc, profile, "CBX", txtCBX.Text);
                SaveDataElement(doc, profile, "CGZ", txtCGZ.Text);
                SaveDataElement(doc, profile, "CGY", txtCGY.Text);
                SaveDataElement(doc, profile, "CGX", txtCGX.Text);
                SaveDataElement(doc, profile, "Mass", txtMass.Text);
                SaveDataElement(doc, profile, "Ballast1", txtBallast1.Text);
                SaveDataElement(doc, profile, "Ballast2", txtBallast2.Text);
                SaveDataElement(doc, profile, "FuelLevel", txtFuelLevel.Text);

                doc.DocumentElement.AppendChild(profile);
                doc.Save(filepath);

                cmbProfiles.Items.Add(txtProfileName.Text);

                button1WasClicked = true;
                if (button1WasClicked == true)
                {
                    cmbProfiles.Enabled = true;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }

        private bool ProfileExists(XmlDocument doc, string profileName)
        {
            // Use XPath to find profiles with the matching name
            var nodes = doc.SelectNodes("//Profile[Name='" + profileName + "']");

            // Return true if any nodes are found (meaning the profile exists)
            return nodes.Count > 0;
        }


        private void SaveDataElement(XmlDocument doc, XmlElement parentElement, string elementName, string elementValue)
        {
            XmlElement element = doc.CreateElement(elementName);
            element.InnerText = elementValue;
            parentElement.AppendChild(element);
        }

        private void cmbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProfiles.SelectedItem != null)
            {
                string selectedName = cmbProfiles.SelectedItem.value ?? cmbProfiles.SelectedItem.ToString();

                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                XmlNode profileNode = doc.SelectSingleNode($"//Profile[@Name='{selectedName}']");

                if (profileNode != null)
                {
                   
                    // ... (populate other text boxes)
                    txtProfileName.Text = GetValueFromProfileNode(profileNode, "Name");
                    txtDraft.Text = GetValueFromProfileNode(profileNode, "Draft");
                    txtCBZ.Text = GetValueFromProfileNode(profileNode, "CBZ");
                    txtCBY.Text = GetValueFromProfileNode(profileNode, "CBY");
                    txtCBX.Text = GetValueFromProfileNode(profileNode, "CBX");
                    txtCGZ.Text = GetValueFromProfileNode(profileNode, "CGZ");
                    txtCGY.Text = GetValueFromProfileNode(profileNode, "CGY");
                    txtCGX.Text = GetValueFromProfileNode(profileNode, "CGX");
                    txtMass.Text = GetValueFromProfileNode(profileNode, "Mass");
                    txtBallast1.Text = GetValueFromProfileNode(profileNode, "Ballast1");
                    txtBallast2.Text = GetValueFromProfileNode(profileNode, "Ballast2");
                    txtFuelLevel.Text = GetValueFromProfileNode(profileNode, "FuelLevel");
                }
                else
                {
                    // Handle case where profile not found:
                    MessageBox.Show("Profile '" + selectedName + "' not found in the XML file.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Optional: Clear text boxes to avoid displaying old data
                    txtProfileName.Clear();
                    txtDraft.Clear();
                    // ... clear other text boxes
                }


            }

            else
            {
                Console.WriteLine("Please select a profile first");
            }


        }

        private string GetValueFromProfileNode(XmlNode profileNode, string elementName)
        {
            if (profileNode != null)
            {
                XmlNode elementNode = profileNode.SelectSingleNode(elementName);
                if (elementNode != null)
                {
                    return elementNode.InnerText;
                }
            }
            return string.Empty;
        }

        private void FuelLevel(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && textBox == txtFuelLevel)
            {
                // Allow only digits and decimal point for txtFuelLevel
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // Allow only one decimal point
                if (e.KeyChar == '.' && textBox.Text.Contains("."))
                {
                    e.Handled = true;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check if a profile is selected
            if (cmbProfiles.SelectedItem != null)
            {
                // Load the XML document
                XmlDocument doc = new XmlDocument();
                doc.Load("profiles.xml");

                // Get the selected profile name
                string profileName = cmbProfiles.SelectedItem.ToString();

                // Find the profile node in the XML
                XmlNode profileNode = doc.SelectSingleNode($"//Profile[Name='{profileName}']");

                if (profileNode != null)
                {
                    XmlNode node = profileNode;

                    node.ParentNode.RemoveChild(node);

                    doc.Save("profiles.xml");

                    // Update the combobox
                    cmbProfiles.Items.RemoveAt(cmbProfiles.SelectedIndex);
                    cmbProfiles.SelectedIndex = -1;
                    cmbProfiles.Text = "";
                    cmbProfiles.Refresh();


                    txtProfileName.Clear();
                    txtMass.Clear();
                    txtBallast1.Clear();
                    txtBallast2.Clear();
                    txtCBX.Clear();
                    txtCBY.Clear();
                    txtCBZ.Clear();
                    txtCGX.Clear();
                    txtCGY.Clear();
                    txtCGZ.Clear();
                    txtFuelLevel.Clear();
                    txtFuelLevel.Clear();
                    txtDraft.Clear();


                    MessageBox.Show("Profile deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Handle case where profile is not found
                    MessageBox.Show("Profile not found!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Handle case where no profile is selected
                MessageBox.Show("Please select a profile to delete first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            cmbProfiles.SelectedIndex = -1;
            cmbProfiles.Text = "";
            cmbProfiles.Refresh();
            txtProfileName.Clear();
            txtMass.Clear();
            txtBallast1.Clear();
            txtBallast2.Clear();
            txtCBX.Clear();
            txtCBY.Clear();
            txtCBZ.Clear();
            txtCGX.Clear();
            txtCGY.Clear();
            txtCGZ.Clear();
            txtFuelLevel.Clear();
            txtFuelLevel.Clear();
            txtDraft.Clear();

        }

        private void RSVParameters_Load(object sender, EventArgs e)
        {
            if (groupBox1 != null)
            {
                groupBox1.Enabled = false;
            }

            cmbProfiles.Enabled = false;
        }
    }
}


