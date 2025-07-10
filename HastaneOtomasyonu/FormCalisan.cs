using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormCalisan : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        TextBox txtAd, txtSoyad, txtTelefon;
        ComboBox cmbGorev;
        DataGridView dgvCalisanlar;
        Button btnEkle, btnGuncelle, btnSil;
        int secilenID = -1;

        public FormCalisan()
        {
            InitializeComponent();
            FormuHazirla();
            CalisanlariListele();
        }

        private void FormuHazirla()
        {
            this.Text = "Çalışan İşlemleri";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font font = new Font("Segoe UI", 10);

            Label lblAd = new Label { Text = "Ad:", Location = new Point(30, 30), Font = font };
            this.Controls.Add(lblAd);
            txtAd = new TextBox { Location = new Point(130, 25), Width = 200 };
            this.Controls.Add(txtAd);

            Label lblSoyad = new Label { Text = "Soyad:", Location = new Point(30, 70), Font = font };
            this.Controls.Add(lblSoyad);
            txtSoyad = new TextBox { Location = new Point(130, 65), Width = 200 };
            this.Controls.Add(txtSoyad);

            Label lblGorev = new Label { Text = "Görev:", Location = new Point(30, 110), Font = font };
            this.Controls.Add(lblGorev);
            cmbGorev = new ComboBox
            {
                Location = new Point(130, 105),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGorev.Items.AddRange(new string[] { "Hemşire", "Sekreter"," Danışma ","Temizlik Görevlisi", "Teknisyen" });
            this.Controls.Add(cmbGorev);

            Label lblTelefon = new Label { Text = "Telefon:", Location = new Point(30, 150), Font = font };
            this.Controls.Add(lblTelefon);
            txtTelefon = new TextBox { Location = new Point(130, 145), Width = 200 };
            this.Controls.Add(txtTelefon);

            btnEkle = new Button
            {
                Text = "Ekle",
                Location = new Point(130, 200),
                Width = 200,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnEkle.Click += BtnEkle_Click;
            this.Controls.Add(btnEkle);

            btnGuncelle = new Button
            {
                Text = "Güncelle",
                Location = new Point(130, 250),
                Width = 200,
                BackColor = Color.Orange,
                ForeColor = Color.White
            };
            btnGuncelle.Click += BtnGuncelle_Click;
            this.Controls.Add(btnGuncelle);

            btnSil = new Button
            {
                Text = "Sil",
                Location = new Point(130, 300),
                Width = 200,
                BackColor = Color.Firebrick,
                ForeColor = Color.White
            };
            btnSil.Click += BtnSil_Click;
            this.Controls.Add(btnSil);

            dgvCalisanlar = new DataGridView
            {
                Location = new Point(400, 30),
                Size = new Size(450, 500),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvCalisanlar.CellClick += DgvCalisanlar_CellClick;
            this.Controls.Add(dgvCalisanlar);

            Button btnGeri = new Button
            {
                Text = "Geri Dön",
                Location = new Point(30, 500), // Formun sol alt köşesine yakın
                Size = new Size(100, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            btnGeri.FlatAppearance.BorderSize = 0;
            btnGeri.Click += (s, e) => { this.Close(); }; // Formu kapatır
            this.Controls.Add(btnGeri);

        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (txtAd.Text == "" || txtSoyad.Text == "" || cmbGorev.SelectedIndex == -1 || txtTelefon.Text == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Calisanlar (Ad, Soyad, Gorev, Telefon) VALUES (@Ad, @Soyad, @Gorev, @Telefon)", baglanti);
                cmd.Parameters.AddWithValue("@Ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@Soyad", txtSoyad.Text);
                cmd.Parameters.AddWithValue("@Gorev", cmbGorev.Text);
                cmd.Parameters.AddWithValue("@Telefon", txtTelefon.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Çalışan eklendi!");
                CalisanlariListele();
                Temizle();
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

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenID == -1)
            {
                MessageBox.Show("Lütfen güncellenecek çalışanı seçin.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Calisanlar SET Ad=@Ad, Soyad=@Soyad, Gorev=@Gorev, Telefon=@Telefon WHERE CalisanID=@ID", baglanti);
                cmd.Parameters.AddWithValue("@Ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@Soyad", txtSoyad.Text);
                cmd.Parameters.AddWithValue("@Gorev", cmbGorev.Text);
                cmd.Parameters.AddWithValue("@Telefon", txtTelefon.Text);
                cmd.Parameters.AddWithValue("@ID", secilenID);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Çalışan güncellendi!");
                CalisanlariListele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (secilenID == -1)
            {
                MessageBox.Show("Lütfen silinecek çalışanı seçin.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Calisanlar WHERE CalisanID=@ID", baglanti);
                cmd.Parameters.AddWithValue("@ID", secilenID);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Çalışan silindi!");
                CalisanlariListele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void DgvCalisanlar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCalisanlar.Rows[e.RowIndex];
                secilenID = Convert.ToInt32(row.Cells["CalisanID"].Value);

                txtAd.Text = SifrelemeHelper.SifreCoz(row.Cells["Ad"].Value.ToString());
                txtSoyad.Text = SifrelemeHelper.SifreCoz(row.Cells["Soyad"].Value.ToString());
                cmbGorev.Text = row.Cells["Gorev"].Value.ToString(); // Şifrelenmediyse çözme
                txtTelefon.Text = SifrelemeHelper.SifreCoz(row.Cells["Telefon"].Value.ToString());
            }
        }


        private void CalisanlariListele()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Calisanlar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvCalisanlar.DataSource = dt;
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

        private void Temizle()
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtTelefon.Clear();
            cmbGorev.SelectedIndex = -1;
            secilenID = -1;
        }
    }
}
