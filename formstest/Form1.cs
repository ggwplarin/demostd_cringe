using System.Text;
using System.Text.Json;

namespace formstest
{
    public partial class Form1 : Form
    {



        public async void FetchData()
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://localhost:5147/cringes");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Cringe>>(content);
                dataGridView1.DataSource = data;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        public Form1()
        {
            InitializeComponent();
            FetchData();
        }

        class Cringe
        {
            public int id { get; set; }
            public string? name { get; set; }
            public int? lvl { get; set; }
            public int gg { get; set; }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //add
            if (!(!string.IsNullOrWhiteSpace(textBox1.Text) && Int32.TryParse(textBox2.Text, out _) && Int32.TryParse(textBox3.Text, out _))) return;
            try
            {
                var cringe = new Cringe() { id = 0, name = textBox1.Text, lvl = Int32.Parse(textBox2.Text), gg = Int32.Parse(textBox3.Text) };
                var content = new StringContent(JsonSerializer.Serialize(cringe), Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                var response = await client.PostAsync("http://localhost:5147/cringes", content);
                response.EnsureSuccessStatusCode();

                FetchData();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var selected = (Cringe)dataGridView1.SelectedRows[0].DataBoundItem;
                textBox4.Text = selected.id.ToString();
                textBox7.Text = selected.name.ToString();
                textBox6.Text = selected.lvl.ToString();
                textBox5.Text = selected.gg.ToString();
            }
            catch (Exception)
            {
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //edit
            if (!(!string.IsNullOrWhiteSpace(textBox7.Text) && Int32.TryParse(textBox6.Text, out _) && Int32.TryParse(textBox5.Text, out _))) return;
            try
            {
                var cringe = new Cringe() { id = Int32.Parse(textBox4.Text), name = textBox7.Text, lvl = Int32.Parse(textBox6.Text), gg = Int32.Parse(textBox5.Text) };
                var content = new StringContent(JsonSerializer.Serialize(cringe), Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                var response = await client.PutAsync($"http://localhost:5147/cringes/{cringe.id}", content);
                response.EnsureSuccessStatusCode();

                FetchData();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //del
            if (textBox4.Text == string.Empty) return;
            try
            {
                var cringe = new Cringe() { id = Int32.Parse(textBox4.Text), name = textBox7.Text, lvl = Int32.Parse(textBox6.Text), gg = Int32.Parse(textBox5.Text) };
                HttpClient client = new HttpClient();
                var response = await client.DeleteAsync($"http://localhost:5147/cringes/{cringe.id}");
                response.EnsureSuccessStatusCode();

                FetchData();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}