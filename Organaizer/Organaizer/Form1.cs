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
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Soap;

namespace Organaizer
{
    public partial class Form1 : Form
    {
        XmlDocument xmlDoc;
        string pathToXml = Application.StartupPath+ "/TreeSave.xml";
        public Form1()
        {
            InitializeComponent();
            //проверка на существование файла
            initXmlFile(pathToXml);
        }

        private void initXmlFile(string pathToXml)
        {
            if (File.Exists(pathToXml)){
                //чтение файла xml и запись в treeView
                TreeNode[] tempNodes;
                FileStream fs = new FileStream("TreeSave.xml", FileMode.Open);
                SoapFormatter sf = new SoapFormatter();
                tempNodes = (TreeNode[])sf.Deserialize(fs);
                treeView1.Nodes.AddRange(tempNodes);
                fs.Close();
            }
            else{
                //создание файла xml
                //массив временый который и будем сериализовать
                TreeNode[] tempNodes = new TreeNode[treeView1.Nodes.Count];
                // заполняем его
                for (int i = 0; i < treeView1.Nodes.Count; i++)
                    tempNodes[i] = treeView1.Nodes[i];
                // сама сериализация
                FileStream fs = new FileStream("TreeSave.xml", FileMode.Create);
                SoapFormatter sf = new SoapFormatter();
                sf.Serialize(fs, tempNodes);
                fs.Close();
                //treeView1.Nodes.Clear(); // очищаем для наглядности
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Close();
        }


        //Open the XML file, and start to populate the treeview
        private void populateTreeview()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open XML Document";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FileName = Application.StartupPath + "\\..\\..\\example.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Just a good practice -- change the cursor to a 
                    //wait cursor while the nodes populate
                    this.Cursor = Cursors.WaitCursor;
                    //First, we'll load the Xml document
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(dlg.FileName);
                    //Now, clear out the treeview, 
                    //and add the first (root) node
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(new
                      TreeNode(xDoc.DocumentElement.Name));
                    TreeNode tNode = new TreeNode();
                    tNode = (TreeNode)treeView1.Nodes[0];
                    //We make a call to addTreeNode, 
                    //where we'll add all of our nodes
                    addTreeNode(xDoc.DocumentElement, tNode);
                    //Expand the treeview to show all nodes
                    treeView1.ExpandAll();
                }
                catch (XmlException xExc)
                //Exception is thrown is there is an error in the Xml
                {
                    MessageBox.Show(xExc.Message);
                }
                catch (Exception ex) //General exception
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.Cursor = Cursors.Default; //Change the cursor back
                }
            }
        }
        //This function is called recursively until all nodes are loaded
        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes) //The current node has children
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                //Loop through the child nodes
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }
            else //No children, so add the outer xml (trimming off whitespace)
                treeNode.Text = xmlNode.OuterXml.Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode.Tag =richTextBox2.Rtf;
        }


        void Find(TreeNodeCollection Nodes, string str)
        {
            foreach (TreeNode tn in Nodes)
            {
                if (tn.Text == str)
                {
                    treeView1.Focus();
                    treeView1.SelectedNode = tn;//выделение нужного узла
                }
                Find(tn.Nodes, str);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //массив временый который и будем сериализовать
            TreeNode[] tempNodes = new TreeNode[treeView1.Nodes.Count];
            // заполняем его
            for (int i = 0; i < treeView1.Nodes.Count; i++)
                tempNodes[i] = treeView1.Nodes[i];
            // сама сериализация
            FileStream fs = new FileStream("TreeSave.xml", FileMode.Create);
            SoapFormatter sf = new SoapFormatter();
            sf.Serialize(fs, tempNodes);
            fs.Close();
            //treeView1.Nodes.Clear(); // очищаем для наглядности
        }


        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images |*.png;*.jpg;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image image = Image.FromFile(dialog.FileName);
                Clipboard.SetImage(image);
                richTextBox2.Paste();
            }
        }


        private void создатьКореньToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogForm dF = new DialogForm("Создать корень");
            dF.ShowDialog();
            if (dF.DialogResult == DialogResult.OK)
            {
                treeView1.Nodes.Add(dF.getText());
                Find(treeView1.Nodes, dF.getText());
                
            }
        }

        private void добавитьВетвьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogForm dF = new DialogForm("Добавить ветвь");
            dF.ShowDialog();
            string findText = "";
            if (dF.DialogResult == DialogResult.OK)
            {
                findText = dF.getText();
                treeView1.SelectedNode.Nodes.Add(findText);
            }
            //выделить узел
            Find(treeView1.Nodes, dF.getText());
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить "+ treeView1.SelectedNode.Text+" (+ все его ветки)?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                treeView1.SelectedNode.Remove();
            }
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogForm dF = new DialogForm("Редактирование", treeView1.SelectedNode.Text);
            dF.ShowDialog();
            if (dF.DialogResult == DialogResult.OK)
            {
                treeView1.SelectedNode.Text = dF.getText();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.Nodes.Count == 0)
            {
                удалитьToolStripMenuItem.Enabled = false;
                редактироватьToolStripMenuItem.Enabled = false;
                добавитьВетвьToolStripMenuItem.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem.Enabled = true;
                редактироватьToolStripMenuItem.Enabled = true;
                добавитьВетвьToolStripMenuItem.Enabled = true;
            }

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;    
            }
            if (e.Node.Tag != null)
            {
                richTextBox2.Rtf = e.Node.Tag.ToString();
            }
            else
            {
                richTextBox2.Rtf = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox2.SelectionFont= fontDialog1.Font;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox2.SelectionColor = colorDialog1.Color;
            }
        }
    }
}
