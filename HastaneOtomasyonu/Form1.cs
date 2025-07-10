using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class Form1 : Form
    {
        
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        
        TextBox txtAd, txtSoyad, txtTCNo, txtTelefon, txtAdres;
        ComboBox cmbCinsiyet;
        DateTimePicker dtpDogumTarihi;
        Button btnEkle;
        DataGridView dgvHastalar;

        public Form1()
        {
            InitializeComponent();
            FormuHazirla(); 
            HastalariListele(); 
        }
        private void BtnGeri_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void FormuHazirla()
        {
            // Label ve TextBox - Ad
            Label lblAd = new Label { Text = "Ad:", Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblAd);
            txtAd = new TextBox { Location = new Point(120, 20), Width = 200 };
            this.Controls.Add(txtAd);

            // Label ve TextBox - Soyad
            Label lblSoyad = new Label { Text = "Soyad:", Location = new Point(20, 60), AutoSize = true };
            this.Controls.Add(lblSoyad);
            txtSoyad = new TextBox { Location = new Point(120, 60), Width = 200 };
            this.Controls.Add(txtSoyad);

            // Label ve TextBox - TC No
            Label lblTCNo = new Label { Text = "TC No:", Location = new Point(20, 100), AutoSize = true };
            this.Controls.Add(lblTCNo);
            txtTCNo = new TextBox { Location = new Point(120, 100), Width = 200 };
            this.Controls.Add(txtTCNo);

            // Label ve TextBox - Telefon
            Label lblTelefon = new Label { Text = "Telefon:", Location = new Point(20, 140), AutoSize = true };
            this.Controls.Add(lblTelefon);
            txtTelefon = new TextBox { Location = new Point(120, 140), Width = 200 };
            this.Controls.Add(txtTelefon);

            // Label ve DateTimePicker - Doğum Tarihi
            Label lblDogumTarihi = new Label { Text = "Doğum Tarihi:", Location = new Point(20, 180), AutoSize = true };
            this.Controls.Add(lblDogumTarihi);
            dtpDogumTarihi = new DateTimePicker { Location = new Point(120, 180), Width = 200 };
            this.Controls.Add(dtpDogumTarihi);

            // Label ve ComboBox - Cinsiyet
            Label lblCinsiyet = new Label { Text = "Cinsiyet:", Location = new Point(20, 220), AutoSize = true };
            this.Controls.Add(lblCinsiyet);
            cmbCinsiyet = new ComboBox { Location = new Point(120, 220), Width = 200 };
            cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });
            this.Controls.Add(cmbCinsiyet);

            // Label ve TextBox - Adres
            Label lblAdres = new Label { Text = "Adres:", Location = new Point(20, 260), AutoSize = true };
            this.Controls.Add(lblAdres);
            txtAdres = new TextBox { Location = new Point(120, 260), Width = 200, Height = 60, Multiline = true };
            this.Controls.Add(txtAdres);

            // Ekle Butonu
            btnEkle = new Button { Text = "Hasta Ekle", Location = new Point(120, 340), Width = 200 };
            btnEkle.Click += BtnEkle_Click;
            this.Controls.Add(btnEkle);

            // DataGridView - Hasta Listesi
            dgvHastalar = new DataGridView
            {
                Location = new Point(350, 20),
                Width = 400,
                Height = 300,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            this.Controls.Add(dgvHastalar);

            Button btnGeri = new Button
            {
                Text = "Geri Dön",
                Location = new Point(20, 400),
                Width = 100
            };
            btnGeri.Click += BtnGeri_Click;
            this.Controls.Add(btnGeri);

        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO Hastalar (Ad, Soyad, TCNo, Telefon, DogumTarihi, Cinsiyet, Adres) " +
                    "VALUES (@Ad, @Soyad, @TCNo, @Telefon, @DogumTarihi, @Cinsiyet, @Adres)", baglanti);

                komut.Parameters.AddWithValue("@Ad", txtAd.Text);
                komut.Parameters.AddWithValue("@Soyad", txtSoyad.Text);
                komut.Parameters.AddWithValue("@TCNo", txtTCNo.Text);
                komut.Parameters.AddWithValue("@Telefon", txtTelefon.Text);
                komut.Parameters.AddWithValue("@DogumTarihi", dtpDogumTarihi.Value);
                komut.Parameters.AddWithValue("@Cinsiyet", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@Adres", txtAdres.Text);

                komut.ExecuteNonQuery();
                MessageBox.Show("Hasta başarıyla eklendi!");

                HastalariListele(); // Listeyi yenile
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void HastalariListele()
        {
            try
            {
                baglanti.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Hastalar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvHastalar.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
    }
}
