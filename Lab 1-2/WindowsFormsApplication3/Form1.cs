using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using Mono.Security;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
using System.Collections;
using Lucene.Net.QueryParsers;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private string path;
        private string find_text;
        private string path_index;
        private void button1_Click(object sender, EventArgs e)
        {


            string whole_file = File.ReadAllText(path);
            // Разделение на строки.
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);
            lines[0] = lines[0] + ", Year";
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("(", ",");
                lines[i] = lines[i].Replace(")", "");

                if (lines[i].Split(',').Length < 3)
                {
                    lines[i] = lines[i] + " ,0000 ";
                }

            }

            // Посмотрим, сколько строк и столбцов есть.
            int num_rows = lines.Length;
            int num_cols = lines[0].Split(',').Length;

            // Выделите массив данных.
            string[,] values = new string[num_rows, num_cols];
            // Загрузите массив.
            for (int r = 0; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    values[r, c] = line_r[c];
                }
            }
       



            string connstring = "Server=db.mirvoda.com ; Port=5454; User Id = developer;Password = rtfP@ssw0rd;Database = Discordrtf;";
            int a = values.GetLength(0);
            int b = values.GetLength(1);
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            for (int i = 1; i < a; i++)
            {
                connection.Open();
                string id = values[i, 0];
                string year = values[i, 2].Substring(0, 4);
                string name = values[i, 1];
                //NpgsqlCommand command = new NpgsqlCommand("insert into movies (@id,@year,@name", connection);
                var cmd3 = new NpgsqlCommand("insert into movies values (@id,@year,@name)", connection);
                cmd3.Parameters.AddWithValue("@id", id);
                cmd3.Parameters.AddWithValue("@year", year);
                cmd3.Parameters.AddWithValue("@name", name);
                cmd3.ExecuteNonQuery();
                connection.Close();
                if (i == 11000)
                {
                    int j = 0;
                    j++;
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            string connstring = "Server=db.mirvoda.com ; Port=5454; User Id = developer;Password = rtfP@ssw0rd;Database = Discordrtf;";
            NpgsqlConnection con = new NpgsqlConnection(connstring);
            con.Open();
            string comand = "";
            bool flag_dig = false;
            bool flag_char = false;
            List<int> index_digit = new List<int>();
            // Ищем либо по тексту , либо по id и году фильма 
            for (int i = 0; i < find_text.Length; i++)
            {
                if (find_text[i] >= '0' && find_text[i] <= '9')
                {
                    flag_dig = true;
                    index_digit.Add(i);
                }
                else
                {
                    flag_char = true;
                }
            }

            if (flag_dig == true && flag_char == false)
            {
                comand = "SELECT * FROM  movies WHERE  id = " + find_text + " OR  year = " + find_text;
            }
            else if (flag_dig == false && flag_char == true)
            {
                comand = "SELECT * FROM  movies WHERE  name LIKE '%' || \'" + find_text + "\' || '%' LIMIT 10";
            }
            else
            {
                string digit;
                string char_;
                digit = find_text.Substring(index_digit[0], index_digit.Count);
                char_ = find_text.Remove(index_digit[0], index_digit.Count);
                if (index_digit[0] == 0)
                {
                    char_ = char_.Remove(0, 1);
                }
                else
                {
                    char_ = char_.Remove(char_.Length - 1, 1);
                }
                comand = "SELECT * FROM  movies WHERE  name LIKE '%' || \'" + char_ + "\' || '%' and year =" + digit + " LIMIT 10";
            }

            NpgsqlCommand cmd = new NpgsqlCommand(comand, con);
            //con.Parameters.Add("@text", NpgsqlTypes.NpgsqlDbType.Text);
            //con.Parameters["@text"].Value = find_text;

            NpgsqlDataReader dr = cmd.ExecuteReader();


            bool boolfound = false;


            if (dr.Read())
            {
                boolfound = true;
                MessageBox.Show("Connection established");
            }
            if (boolfound == false)
            {
                MessageBox.Show("Data does not exist");
            }
            dr.Close();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    var movieId = reader.GetInt32(0);
                    var movieyear = reader[1];
                    var movieName = reader.GetString(2);
                    textBox1.AppendText(movieId.ToString() + " " + movieName.ToString() + " " + movieyear.ToString() + Environment.NewLine);
                }
                catch
                { }
            }


            con.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            path = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            find_text = textBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string whole_file = File.ReadAllText(path);
            // Разделение на строки.
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);
            lines[0] = lines[0] + ", Year";
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("(", ",");
                lines[i] = lines[i].Replace(")", "");

                if (lines[i].Split(',').Length < 3)
                {
                    lines[i] = lines[i] + " ,0000 ";
                }

            }

            // Посмотрим, сколько строк и столбцов есть.
            int num_rows = lines.Length;
            int num_cols = lines[0].Split(',').Length;

            // Выделите массив данных.
            string[,] values = new string[num_rows, num_cols];
            // Загрузите массив.
            for (int r = 0; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    values[r, c] = line_r[c];
                }
            }

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            DirectoryInfo path_ = new DirectoryInfo(@"C:\Users\wwwle\Desktop\tmp");
            Lucene.Net.Store.Directory directory = new MMapDirectory(path_);
            IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            for (int i = 0; i < values.GetLength(0); i++)
            {

                Document doc = new Document();
                doc.Add(
                  new Field(
                    "id",
                     values[i, 0].ToString(),
                    Field.Store.YES,
                    Field.Index.ANALYZED));
                doc.Add(
                  new Field(
                    "name",
                    values[i, 1].ToString(),
                    Field.Store.YES,
                    Field.Index.ANALYZED));
                doc.Add(
                  new Field(
                    "year",
                    values[i, 2],
                    Field.Store.YES,
                    Field.Index.ANALYZED));
                writer.AddDocument(doc);
            }
            writer.Optimize();
            writer.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            path_index = textBox4.Text;
        }

        
       
        public static Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        public static DirectoryInfo path_ = new DirectoryInfo(@"C:\Users\wwwle\Desktop\tmp");
        public static Lucene.Net.Store.Directory directory = new MMapDirectory(path_);
        public static IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);



        private void button4_Click(object sender, EventArgs e)
        {
            int counter = 0;
           
            var query = find_text.ToLower();
            var array = query.Split(' ').ToList();
            List<string> res_list = new List<string>();
            var searcher = new IndexSearcher(writer.GetReader());

            var totalResults = new List<Document>();

            //поиск по одному слову из названия
            var phrase = new MultiPhraseQuery();
            foreach (var word in array)
            {
                phrase = new MultiPhraseQuery();
                if (!String.IsNullOrEmpty(word))
                {
                    phrase.Add(new Term("name", word));
                    var res = searcher.Search(phrase, 10).ScoreDocs;
                    foreach (var hit in res)
                    {
                        var foundDoc = searcher.Doc(hit.Doc);
                        if (!totalResults.Any(f =>
                            f.GetField("id").ToString() == foundDoc.GetField("id").ToString()))
                            totalResults.Add(foundDoc);
                    }
                }

            }

            //поиск по всем словам названия
            phrase = new MultiPhraseQuery();
            phrase.Add(new Term("name", query));
            var hits = searcher.Search(phrase, 10).ScoreDocs;
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                if (!totalResults.Any(f => f.GetField("id").ToString() == foundDoc.GetField("id").ToString()))
                    totalResults.Add(foundDoc);
            }

            //поиск по частичным словам названия
            foreach (var word in array)
            {
                if (!String.IsNullOrEmpty(word))
                {
                    var wild = new WildcardQuery(new Term("name", "*" + word + "*"));
                    var res = searcher.Search(wild, 10).ScoreDocs;
                    foreach (var hit in res)
                    {
                        var foundDoc = searcher.Doc(hit.Doc);
                        if (!totalResults.Any(f =>
                            f.GetField("id").ToString() == foundDoc.GetField("id").ToString()))
                            totalResults.Add(foundDoc);
                    }
                }
            }

            //поиск по году и названию (части названия)
            string year_to_find = "";
            int number = 0;
            foreach (var word in array)
            {
                bool result = Int32.TryParse(word, out number);
                if (result && number > 1800 && number <= 9999)
                {
                    year_to_find = word;
                    array.RemoveAt(array.IndexOf(word));
                    break;
                }
            }
            Console.WriteLine(number != 0);

            if (number != 0)
            {
                phrase = new MultiPhraseQuery();
                foreach (var word in array)
                {
                    if (!String.IsNullOrEmpty(word))
                    {
                        BooleanQuery booleanQuery = new BooleanQuery();

                        var wild = new WildcardQuery(new Term("name", "*" + word + "*"));
                        var num = NumericRangeQuery.NewIntRange("year", 1, number, number, true, true);

                        booleanQuery.Add(wild, Occur.SHOULD);
                        booleanQuery.Add(num, Occur.SHOULD);
                        var res = searcher.Search(booleanQuery, 10).ScoreDocs;
                        foreach (var hit in res)
                        {
                            var foundDoc = searcher.Doc(hit.Doc);
                            if (!totalResults.Any(f =>
                                f.GetField("id").ToString() == foundDoc.GetField("id").ToString()))
                                totalResults.Add(foundDoc);
                        }
                    }
                }
            }
            foreach (var doc in totalResults)
            {
                textBox1.AppendText(doc.ToString());
            }
        }



    }
}






