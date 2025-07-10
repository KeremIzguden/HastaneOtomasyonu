using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using HastaneOtomasyonu;
using System.Text.RegularExpressions;



namespace HastaneOtomasyonu
{
    public partial class FormHasta : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        TextBox txtAd, txtSoyad, txtTCNo, txtTelefon, txtAdres;
        ComboBox cmbCinsiyet;
        DateTimePicker dtpDogumTarihi;
        Button btnEkle, btnGuncelle, btnSil, btnGeri;
        DataGridView dgvHastalar;

        int seciliHastaID = -1;

        public FormHasta()
        {
            InitializeComponent();
            FormuHazirla();
            HastalariListele();
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtTCNo.Text) || string.IsNullOrWhiteSpace(txtTelefon.Text) ||
                string.IsNullOrWhiteSpace(txtAdres.Text) || cmbCinsiyet.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm bilgileri eksiksiz giriniz.");
                return;
            }

            string tcNo = txtTCNo.Text.Trim();

            // TC Kimlik No doğrulaması
            if (!TcKimlikNoValidator.GecerliMi(tcNo))
            {
                MessageBox.Show("Geçersiz TC Kimlik Numarası! Lütfen 11 haneli ve kurallara uygun bir numara giriniz.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("INSERT INTO Hastalar (Ad, Soyad, TCNo, Telefon, DogumTarihi, Cinsiyet, Adres) VALUES (@Ad, @Soyad, @TCNo, @Telefon, @DogumTarihi, @Cinsiyet, @Adres)", baglanti);
                komut.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                komut.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                komut.Parameters.AddWithValue("@TCNo", SifrelemeHelper.Sifrele(tcNo));
                komut.Parameters.AddWithValue("@Telefon", SifrelemeHelper.Sifrele(txtTelefon.Text));
                komut.Parameters.AddWithValue("@DogumTarihi", dtpDogumTarihi.Value);
                komut.Parameters.AddWithValue("@Cinsiyet", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@Adres", SifrelemeHelper.Sifrele(txtAdres.Text));
                komut.ExecuteNonQuery();

                MessageBox.Show("Hasta başarıyla eklendi.");
                HastalariListele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }


        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliHastaID == -1)
            {
                MessageBox.Show("Güncellenecek bir hasta seçmelisiniz.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("UPDATE Hastalar SET Ad=@Ad, Soyad=@Soyad, TCNo=@TCNo, Telefon=@Telefon, DogumTarihi=@DogumTarihi, Cinsiyet=@Cinsiyet, Adres=@Adres WHERE HastaID=@HastaID", baglanti);
                komut.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                komut.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                komut.Parameters.AddWithValue("@TCNo", SifrelemeHelper.Sifrele(txtTCNo.Text));
                komut.Parameters.AddWithValue("@Telefon", SifrelemeHelper.Sifrele(txtTelefon.Text));
                komut.Parameters.AddWithValue("@DogumTarihi", dtpDogumTarihi.Value);
                komut.Parameters.AddWithValue("@Cinsiyet", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@Adres", SifrelemeHelper.Sifrele(txtAdres.Text));
                komut.Parameters.AddWithValue("@HastaID", seciliHastaID);
                komut.ExecuteNonQuery();

                MessageBox.Show("Hasta başarıyla güncellendi.");
                HastalariListele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void DgvHastalar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHastalar.Rows[e.RowIndex];
                seciliHastaID = Convert.ToInt32(row.Cells["HastaID"].Value);
                txtAd.Text = SifrelemeHelper.SifreCoz(row.Cells["Ad"].ToString());
                txtSoyad.Text = SifrelemeHelper.SifreCoz(row.Cells["Soyad"].ToString());
                txtTCNo.Text = SifrelemeHelper.SifreCoz(row.Cells["TCNo"].ToString());
                txtTelefon.Text = SifrelemeHelper.SifreCoz(row.Cells["Telefon"].ToString());
                dtpDogumTarihi.Value = Convert.ToDateTime(row.Cells["DogumTarihi"].Value);
                cmbCinsiyet.Text = row.Cells["Cinsiyet"].ToString();
                txtAdres.Text = SifrelemeHelper.SifreCoz(row.Cells["Adres"].ToString());
            }
        }

        private void HastalariListele()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Hastalar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    row["Ad"] = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                    row["Soyad"] = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                    row["TCNo"] = SifrelemeHelper.SifreCoz(row["TCNo"].ToString());
                    row["Telefon"] = SifrelemeHelper.SifreCoz(row["Telefon"].ToString());
                    row["Adres"] = SifrelemeHelper.SifreCoz(row["Adres"].ToString());
                }

                dgvHastalar.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (seciliHastaID == -1)
            {
                MessageBox.Show("Silinecek bir hasta seçmelisiniz.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("DELETE FROM Hastalar WHERE HastaID=@HastaID", baglanti);
                komut.Parameters.AddWithValue("@HastaID", seciliHastaID);
                komut.ExecuteNonQuery();

                MessageBox.Show("Hasta başarıyla silindi.");
                HastalariListele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void BtnGeri_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Temizle()
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtTCNo.Clear();
            txtTelefon.Clear();
            txtAdres.Clear();
            cmbCinsiyet.SelectedIndex = -1;
            dtpDogumTarihi.Value = DateTime.Today;
            seciliHastaID = -1;
        }

        private void FormuHazirla()
        {
            this.Text = "Hasta İşlemleri";
            this.Size = new Size(1000, 600);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 11, FontStyle.Regular);
            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Color butonRenk = Color.SteelBlue;
            Color butonYaziRenk = Color.White;

            int labelX = 30;
            int inputX = 160;
            int startY = 30;
            int dikeyBosluk = 45;

            // Ad
            Label lblAd = new Label { Text = "Ad:", Location = new Point(labelX, startY), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblAd);
            txtAd = new TextBox { Location = new Point(inputX, startY - 5), Width = 250, Font = genelFont };
            this.Controls.Add(txtAd);

            // Soyad
            Label lblSoyad = new Label { Text = "Soyad:", Location = new Point(labelX, startY + dikeyBosluk), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblSoyad);
            txtSoyad = new TextBox { Location = new Point(inputX, startY + dikeyBosluk - 5), Width = 250, Font = genelFont };
            this.Controls.Add(txtSoyad);

            // TC No
            Label lblTCNo = new Label { Text = "TC No:", Location = new Point(labelX, startY + dikeyBosluk * 2), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblTCNo);
            txtTCNo = new TextBox { Location = new Point(inputX, startY + dikeyBosluk * 2 - 5), Width = 250, Font = genelFont };
            txtTCNo.MaxLength = 11;
            txtTCNo.KeyPress += SadeceRakamGirisi_KeyPress;
            this.Controls.Add(txtTCNo);

            // Telefon
            Label lblTelefon = new Label { Text = "Telefon:", Location = new Point(labelX, startY + dikeyBosluk * 3), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblTelefon);
            txtTelefon = new TextBox { Location = new Point(inputX, startY + dikeyBosluk * 3 - 5), Width = 250, Font = genelFont };
            txtTelefon.MaxLength = 11;
            txtTelefon.KeyPress += SadeceRakamGirisi_KeyPress;
            this.Controls.Add(txtTelefon);


            // Doğum Tarihi
            Label lblDogumTarihi = new Label { Text = "Doğum Tarihi:", Location = new Point(labelX, startY + dikeyBosluk * 4), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblDogumTarihi);
            dtpDogumTarihi = new DateTimePicker { Location = new Point(inputX, startY + dikeyBosluk * 4 - 5), Width = 250, Font = genelFont };
            this.Controls.Add(dtpDogumTarihi);

            // Cinsiyet
            Label lblCinsiyet = new Label { Text = "Cinsiyet:", Location = new Point(labelX, startY + dikeyBosluk * 5), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblCinsiyet);
            cmbCinsiyet = new ComboBox { Location = new Point(inputX, startY + dikeyBosluk * 5 - 5), Width = 250, Font = genelFont };
            cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });
            this.Controls.Add(cmbCinsiyet);

            // Adres
            Label lblAdres = new Label { Text = "Adres:", Location = new Point(labelX, startY + dikeyBosluk * 6), AutoSize = true, Font = labelFont };
            this.Controls.Add(lblAdres);
            txtAdres = new TextBox { Location = new Point(inputX, startY + dikeyBosluk * 6 - 5), Width = 250, Height = 60, Multiline = true, Font = genelFont };
            this.Controls.Add(txtAdres);

            // Ekle Butonu
            btnEkle = new Button { Text = "Ekle", Location = new Point(inputX, startY + dikeyBosluk * 8), Width = 80, Height = 40, BackColor = butonRenk, ForeColor = butonYaziRenk, Font = genelFont, FlatStyle = FlatStyle.Flat };
            btnEkle.FlatAppearance.BorderSize = 0;
            btnEkle.Click += BtnEkle_Click;
            this.Controls.Add(btnEkle);

            // Güncelle Butonu
            btnGuncelle = new Button { Text = "Güncelle", Location = new Point(inputX + 90, startY + dikeyBosluk * 8), Width = 80, Height = 40, BackColor = Color.Orange, ForeColor = Color.White, Font = genelFont, FlatStyle = FlatStyle.Flat };
            btnGuncelle.FlatAppearance.BorderSize = 0;
            btnGuncelle.Click += BtnGuncelle_Click;
            this.Controls.Add(btnGuncelle);

            // Sil Butonu
            btnSil = new Button { Text = "Sil", Location = new Point(inputX + 180, startY + dikeyBosluk * 8), Width = 80, Height = 40, BackColor = Color.Firebrick, ForeColor = Color.White, Font = genelFont, FlatStyle = FlatStyle.Flat };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += BtnSil_Click;
            this.Controls.Add(btnSil);

            // DataGridView
            dgvHastalar = new DataGridView
            {
                Location = new Point(450, 30),
                Width = 500,
                Height = 450,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Gainsboro }
            };
            dgvHastalar.CellClick += DgvHastalar_CellClick;
            this.Controls.Add(dgvHastalar);

            // Geri Butonu
            btnGeri = new Button { Text = "Geri Dön", Width = 100, Height = 40, Location = new Point(850, 500), BackColor = Color.Gray, ForeColor = Color.White, Font = genelFont, FlatStyle = FlatStyle.Flat };
            btnGeri.FlatAppearance.BorderSize = 0;
            btnGeri.Click += BtnGeri_Click;
            this.Controls.Add(btnGeri);
        }
        private void SadeceRakamGirisi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Harf, boşluk veya sembol engellenir
            }
        }

    }
}
