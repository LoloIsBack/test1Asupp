///https://vincentlaine.developpez.com/tuto/dotnet/comdoc/
///https://openclassrooms.com/fr/courses/2818931-programmez-en-oriente-objet-avec-c/2819146-creez-un-projet-bibliotheque-de-classes
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace S7SiemensManager
{
    public partial class Form1 : Form
    {
        public static void LoloUseParams(params int[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Console.Write(list[i] + " ");
            }
            Console.WriteLine();
        }

        public Form1()
        {
            InitializeComponent();
    }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            var item = CbActions.SelectedItem.ToString();
            if (item.LoloRegex(@"(?i)comment") && item.LoloRegex(@"(?i)cleaner")){
                S7.Db.Macros.CommentsCleaner(); }
            else if (item.LoloRegex(@"(?i)motor") && item.LoloRegex(@"(?i)cleaner")){
                S7.Db.Macros.MotorsCleaner(); }
            else if (item.LoloRegex(@"(?i)motor") && item.LoloRegex(@"(?i)list")){
                Gentool.Macros.InitProject(); }
            else if (item.LoloRegex(@"(?i)Symbol") && item.LoloRegex(@"(?i)import")){
                S7.SymbolicsTable.Macros.Importation(); }
            else if (item.LoloRegex(@"(?i)material") && item.LoloRegex(@"(?i)checker")){
                S7.SymbolicsTable.Macros.MaterialsSchemaChecker(); }
            else if (item.LoloRegex(@"(?i)lolo") && item.LoloRegex(@"(?i)use")){
                LoloUseParams(1, 3, 5, 6); }
            else {
                MessageBox.Show(string.Format("Aucune action pour la commande '{0}'",item)); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CbActions.SelectedIndex = 0;          
        }
    }
}
