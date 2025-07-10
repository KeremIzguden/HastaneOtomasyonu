using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormDoktor : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        TextBox txtAd, txtSoyad, txtTelefon;
        ComboBox cmbBrans;
        DataGridView dgvDoktorlar;
        Button btnEkle, btnGuncelle, btnSil;
        int secilenDoktorID = -1;

        public FormDoktor()
        {
            InitializeComponent();
            FormuHazirla();
            DoktorlariListele();
        }

        private void FormuHazirla()
        {
            this.Text = "Doktor İşlemleri";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Font font = new Font("Segoe UI", 10);

            Label lblAd = new Label { Text = "Ad:", Location = new Point(30, 30), Font = font };
            this.Controls.Add(lblAd);
            txtAd = new TextBox { Location = new Point(130, 25), Width = 200 };
            this.Controls.Add(txtAd);

            Label lblSoyad = new Label { Text = "Soyad:", Location = new Point(30, 70), Font = font };
            this.Controls.Add(lblSoyad);
            txtSoyad = new TextBox { Location = new Point(130, 65), Width = 200 };
            this.Controls.Add(txtSoyad);

            Label lblBrans = new Label { Text = "Branş:", Location = new Point(30, 110), Font = font };
            this.Controls.Add(lblBrans);
            cmbBrans = new ComboBox { Location = new Point(130, 105), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBrans.Items.AddRange(new string[] {
                "Kardiyoloji", "Dahiliye", "Ortopedi", "Genel Cerrahi", "Nöroloji",
                "Psikiyatri", "Göz Hastalıkları", "KBB", "Üroloji",
                "Kadın Doğum", "Çocuk Sağlığı", "Anestezi", "Acil Servis"
            });
            this.Controls.Add(cmbBrans);

            Label lblTelefon = new Label { Text = "Telefon:", Location = new Point(30, 150), Font = font };
            this.Controls.Add(lblTelefon);
            txtTelefon = new TextBox { Location = new Point(130, 145), Width = 200 };
            this.Controls.Add(txtTelefon);

            btnEkle = new Button { Text = "Ekle", Location = new Point(130, 190), Width = 200, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnEkle.Click += BtnEkle_Click;
            this.Controls.Add(btnEkle);

            btnGuncelle = new Button { Text = "Güncelle", Location = new Point(130, 240), Width = 200, BackColor = Color.Orange, ForeColor = Color.White };
            btnGuncelle.Click += BtnGuncelle_Click;
            this.Controls.Add(btnGuncelle);

            btnSil = new Button { Text = "Sil", Location = new Point(130, 290), Width = 200, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnSil.Click += BtnSil_Click;
            this.Controls.Add(btnSil);

            dgvDoktorlar = new DataGridView { Location = new Point(370, 25), Size = new Size(580, 500), ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvDoktorlar.CellClick += DgvDoktorlar_CellClick;
            this.Controls.Add(dgvDoktorlar);

            Button btnGeri = new Button
            {
                Text = "Geri",
                Location = new Point(30, 500),
                Size = new Size(100, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGeri.FlatAppearance.BorderSize = 0;
            btnGeri.Click += (s, e) =>
            {
                this.Close();
            };
            this.Controls.Add(btnGeri);
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (txtAd.Text == "" || txtSoyad.Text == "" || txtTelefon.Text == "" || cmbBrans.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }
            try
            {
                baglanti.Open();

                // Doktoru ekle 
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Doktorlar (Ad, Soyad, Branş, Telefon) OUTPUT INSERTED.DoktorID VALUES (@Ad, @Soyad, @Brans, @Tel)", baglanti);
                cmd.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                cmd.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                cmd.Parameters.AddWithValue("@Brans", cmbBrans.Text);

                cmd.Parameters.AddWithValue("@Tel", SifrelemeHelper.Sifrele(txtTelefon.Text));

                int yeniDoktorID = (int)cmd.ExecuteScalar();

                // Kullanıcı adı 
                string kullaniciAdi = (txtAd.Text + "." + txtSoyad.Text).ToLower();
                string sifre = "Doktor123!";

                SqlCommand kullaniciCmd = new SqlCommand(
                    "INSERT INTO Kullanicilar (KullaniciAdi, Sifre, Yetki, DoktorID) VALUES (@KullaniciAdi, @Sifre, @Yetki, @DoktorID)", baglanti);
                kullaniciCmd.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                kullaniciCmd.Parameters.AddWithValue("@Sifre", SifrelemeHelper.Sifrele(sifre));
                kullaniciCmd.Parameters.AddWithValue("@Yetki", "Doktor");
                kullaniciCmd.Parameters.AddWithValue("@DoktorID", yeniDoktorID);
                kullaniciCmd.ExecuteNonQuery();

                MessageBox.Show($"Doktor ve kullanıcı hesabı başarıyla eklendi.\n\nKullanıcı Adı: {kullaniciAdi}\nŞifre: {sifre}");

                DoktorlariListele();
                Temizle();
            }
            finally
            {
                baglanti.Close();
            }
        }


        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenDoktorID == -1)
            {
                MessageBox.Show("Lütfen güncellenecek doktoru seçin.");
                return;
            }
            try
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Doktorlar SET Ad=@Ad, Soyad=@Soyad, Branş=@Brans, Telefon=@Tel WHERE DoktorID=@ID", baglanti);
                cmd.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                cmd.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                cmd.Parameters.AddWithValue("@Brans", cmbBrans.Text);
                cmd.Parameters.AddWithValue("@Tel", SifrelemeHelper.Sifrele(txtTelefon.Text));
                cmd.Parameters.AddWithValue("@ID", secilenDoktorID);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Doktor güncellendi.");
                DoktorlariListele();
                Temizle();
            }
            finally { baglanti.Close(); }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (secilenDoktorID == -1)
            {
                MessageBox.Show("Lütfen silinecek doktoru seçin.");
                return;
            }
            try
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Doktorlar WHERE DoktorID=@ID", baglanti);
                cmd.Parameters.AddWithValue("@ID", secilenDoktorID);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Doktor silindi.");
                DoktorlariListele();
                Temizle();
            }
            finally { baglanti.Close(); }
        }

        private void DgvDoktorlar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDoktorlar.Rows[e.RowIndex];
                secilenDoktorID = Convert.ToInt32(row.Cells["DoktorID"].Value);

                txtAd.Text = SifrelemeHelper.SifreCoz(row.Cells["Ad"].Value.ToString());
                txtSoyad.Text = SifrelemeHelper.SifreCoz(row.Cells["Soyad"].Value.ToString());
                cmbBrans.Text = row.Cells["Branş"].Value.ToString(); // şifreli değilse çözme
                txtTelefon.Text = SifrelemeHelper.SifreCoz(row.Cells["Telefon"].Value.ToString());
            }
        }


        private void DoktorlariListele()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Doktorlar", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                row["Ad"] = SifrelemeHelper.SifreCoz(row["Ad"].ToString());
                row["Soyad"] = SifrelemeHelper.SifreCoz(row["Soyad"].ToString());
                row["Telefon"] = SifrelemeHelper.SifreCoz(row["Telefon"].ToString());
                 
            }

            dgvDoktorlar.DataSource = dt;
        }


        private void Temizle()
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtTelefon.Clear();
            cmbBrans.SelectedIndex = -1;
            secilenDoktorID = -1;
        }
    }
}
