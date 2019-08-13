using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace App1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetStatusBar(string _text)
        {
            Label_StatusBar.Content = _text;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_NPC.SelectedItem == null) return;
            TextBox_Name.Text = ((Mod)(ListBox_NPC.SelectedItem)).Name;
            TextBox_Author.Text = ((Mod)(ListBox_NPC.SelectedItem)).Author;
            TextBox_Version.Text = ((Mod)(ListBox_NPC.SelectedItem)).Version.ToString();
        }

        private void Button_AddNPC(object sender, RoutedEventArgs e)
        {
            Mod current = new Mod();
            ListBox_NPC.Items.Add(current);
            ListBox_NPC.SelectedItem = current;
        }

        private void Button_RemoveNPC(object sender, RoutedEventArgs e)
        {
            if (ListBox_NPC.SelectedItem == null) return;

            int index = ListBox_NPC.Items.IndexOf(ListBox_NPC.SelectedItem);
            ListBox_NPC.Items.Remove(ListBox_NPC.SelectedItem);
            if (index > 0)
                ListBox_NPC.SelectedItem = ListBox_NPC.Items.GetItemAt(index - 1);
            RefreshListBox();
        }

        private void TextBox_Name_Changed(object sender, TextChangedEventArgs e)
        {
            if (ListBox_NPC.SelectedItem == null) return;

            ((Mod)(ListBox_NPC.SelectedItem)).Name = ((TextBox)sender).Text;
            RefreshListBox();
        }

        private void TextBox_Author_Changed(object sender, TextChangedEventArgs e)
        {
            if (ListBox_NPC.SelectedItem == null) return;

            ((Mod)(ListBox_NPC.SelectedItem)).Author = ((TextBox)sender).Text;
            RefreshListBox();
        }

        private void TextBox_Version_Changed(object sender, TextChangedEventArgs e)
        {
            bool work = int.TryParse(((TextBox)sender).Text, out int result);
            if (!work) return;

            ((Mod)(ListBox_NPC.SelectedItem)).Version = result;
            RefreshListBox();
        }


        private void RefreshListBox() => ListBox_NPC.Items.Refresh();

        private void Menu_SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog()
            {
                Filter = Mod.SAVE
            };

            string path = "";

            if (ofd.ShowDialog() == true)
            {
                path = ofd.FileName;
            }
            else return;

            Mod[] toSave = new Mod[ListBox_NPC.Items.Count];
            ListBox_NPC.Items.CopyTo(toSave, 0);

            Save(path, toSave);
        }

        private void Menu_Load(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {                
                CheckFileExists = true,
                Filter = Mod.LOAD
            };
            string path = "";

            if (ofd.ShowDialog() == true)
            {
                path = ofd.FileName;
            }
            else return;

            Mod[] loadedContent = Load(path);
            RefreshList(loadedContent);
        }

        private void Save(string _path, Mod[] _save)
        {
            StreamWriter writer = new StreamWriter(_path);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(writer.BaseStream, _save);
            writer.Close();
        }

        private Mod[] Load(string _path)
        {
            StreamReader reader = new StreamReader(_path);
            BinaryFormatter formatter = new BinaryFormatter();

            Mod[] toReturn = (Mod[])formatter.Deserialize(reader.BaseStream);
            return toReturn;
        }

        private void RefreshList(Mod[] _in)
        {
            ListBox_NPC.Items.Clear();

            foreach (Mod item in _in)
            {
                ListBox_NPC.Items.Add(item);
            }

            RefreshListBox();
        }
    }

    [Serializable]
    public class Mod
    {
        public Mod()
        {
            Name = "Name";
            Author = "Author";
            Version = 0;
        }

        public string Name;
        public string Author;
        public int Version;

        public override string ToString() => $"{Author}: {Name}, Version {Version}";

        public const string SAVE = "Mod|*.mod|Xml|*.xml";
        public const string LOAD = "Mod|*.mod|Xml|*.xml|All supported Files|*.mod;*.xml";
    }

}
