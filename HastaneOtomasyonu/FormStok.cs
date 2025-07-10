using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormStok : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        DataGridView dgvStoklar;
        TextBox txtUrunAdi, txtMiktar, txtBirimFiyat, txtKategori, txtTedarikci;
        DateTimePicker dtpSonKullanma;
        Button btnEkle, btnSil;

        public FormStok()
        {
            InitializeComponent();
            FormuHazirla();
            StoklariYukle();
        }

        private void FormuHazirla()
        {
            this.Text = "Stok Yönetimi";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font font = new Font("Segoe UI", 10);
            int x = 30, y = 30, h = 40;

            this.Controls.Add(new Label { Text = "Ürün Adı:", Location = new Point(x, y), Font = font });
            txtUrunAdi = new TextBox { Location = new Point(x + 120, y), Width = 200 }; this.Controls.Add(txtUrunAdi);

            this.Controls.Add(new Label { Text = "Miktar:", Location = new Point(x, y + h), Font = font });
            txtMiktar = new TextBox { Location = new Point(x + 120, y + h), Width = 200 }; this.Controls.Add(txtMiktar);

            this.Controls.Add(new Label { Text = "Birim Fiyat:", Location = new Point(x, y + h * 2), Font = font });
            txtBirimFiyat = new TextBox { Location = new Point(x + 120, y + h * 2), Width = 200 }; this.Controls.Add(txtBirimFiyat);

            this.Controls.Add(new Label { Text = "Kategori:", Location = new Point(x, y + h * 3), Font = font });
            txtKategori = new TextBox { Location = new Point(x + 120, y + h * 3), Width = 200 }; this.Controls.Add(txtKategori);

            this.Controls.Add(new Label { Text = "Tedarikçi:", Location = new Point(x, y + h * 4), Font = font });
            txtTedarikci = new TextBox { Location = new Point(x + 120, y + h * 4), Width = 200 }; this.Controls.Add(txtTedarikci);

            this.Controls.Add(new Label { Text = "Son Kullanma Tarihi:", Location = new Point(x, y + h * 5), Font = font });
            dtpSonKullanma = new DateTimePicker { Location = new Point(x + 180, y + h * 5), Width = 140 }; this.Controls.Add(dtpSonKullanma);

            PictureBox pbSearch = new PictureBox
            {
                Image = Image.FromFile("search.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(24, 24),
                Location = new Point(400, 500)
            };
            this.Controls.Add(pbSearch);

            TextBox txtAra = new TextBox
            {
                Location = new Point(430, 500),
                Width = 200,
                Font = font,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtAra);

            txtAra.TextChanged += (s, e) =>
            {
                if (dgvStoklar.DataSource is DataTable dt)
                    dt.DefaultView.RowFilter = $"UrunAdi LIKE '%{txtAra.Text}%'";
            };

            Button btnGuncelle = new Button
            {
                Text = "Stok Güncelle",
                Location = new Point(30, 430),
                Width = 250,
                Height = 40,
                BackColor = Color.Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = font
            };
            btnGuncelle.FlatAppearance.BorderSize = 0;
            btnGuncelle.Click += BtnGuncelle_Click;
            this.Controls.Add(btnGuncelle);

            btnEkle = new Button
            {
                Text = "Stok Ekle",
                Location = new Point(x + 120, y + h * 6),
                Width = 200,
                Height = 40,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                Font = font
            };
            btnEkle.Click += BtnEkle_Click;
            this.Controls.Add(btnEkle);

            dgvStoklar = new DataGridView
            {
                Location = new Point(400, 30),
                Size = new Size(560, 450),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvStoklar);

            dgvStoklar.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvStoklar.Rows[e.RowIndex];
                    txtUrunAdi.Text = row.Cells["UrunAdi"].Value.ToString();
                    txtMiktar.Text = row.Cells["Miktar"].Value.ToString();
                    txtBirimFiyat.Text = row.Cells["BirimFiyat"].Value.ToString();
                    txtKategori.Text = row.Cells["Kategori"].Value.ToString();
                    txtTedarikci.Text = row.Cells["Tedarikci"].Value.ToString();
                    dtpSonKullanma.Value = Convert.ToDateTime(row.Cells["SonKullanmaTarihi"].Value);
                }
            };

            btnSil = new Button
            {
                Text = "Seçili Stok Kaydını Sil",
                Location = new Point(30, 370),
                Width = 250,
                Height = 40,
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += BtnSil_Click;
            this.Controls.Add(btnSil);

            dgvStoklar.RowPrePaint += DgvStoklar_RowPrePaint;

        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Stoklar (UrunAdi, Miktar, BirimFiyat, SonKullanmaTarihi, Kategori, Tedarikci) VALUES (@Urun, @Miktar, @Fiyat, @SKT, @Kategori, @Tedarikci)", baglanti);
                cmd.Parameters.AddWithValue("@Urun", SifrelemeHelper.Sifrele(txtUrunAdi.Text));
                cmd.Parameters.AddWithValue("@Miktar", int.Parse(txtMiktar.Text));
                cmd.Parameters.AddWithValue("@Fiyat", decimal.Parse(txtBirimFiyat.Text));
                cmd.Parameters.AddWithValue("@SKT", dtpSonKullanma.Value);
                cmd.Parameters.AddWithValue("@Kategori", SifrelemeHelper.Sifrele(txtKategori.Text));
                cmd.Parameters.AddWithValue("@Tedarikci", SifrelemeHelper.Sifrele(txtTedarikci.Text));
                cmd.ExecuteNonQuery();

                MessageBox.Show("Stok başarıyla eklendi.");
                StoklariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { baglanti.Close(); }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgvStoklar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellenecek bir stok seçin.");
                return;
            }

            int id = Convert.ToInt32(dgvStoklar.SelectedRows[0].Cells["StokID"].Value);

            try
            {
                using (SqlConnection conn = new SqlConnection(baglanti.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"
                    UPDATE Stoklar 
                    SET UrunAdi = @Urun, Miktar = @Miktar, BirimFiyat = @Fiyat,
                        SonKullanmaTarihi = @SKT, Kategori = @Kategori, Tedarikci = @Tedarikci
                    WHERE StokID = @id", conn);

                    cmd.Parameters.AddWithValue("@Urun", SifrelemeHelper.Sifrele(txtUrunAdi.Text));
                    cmd.Parameters.AddWithValue("@Miktar", int.Parse(txtMiktar.Text));
                    cmd.Parameters.AddWithValue("@Fiyat", decimal.Parse(txtBirimFiyat.Text));
                    cmd.Parameters.AddWithValue("@SKT", dtpSonKullanma.Value);
                    cmd.Parameters.AddWithValue("@Kategori", SifrelemeHelper.Sifrele(txtKategori.Text));
                    cmd.Parameters.AddWithValue("@Tedarikci", SifrelemeHelper.Sifrele(txtTedarikci.Text));
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Stok güncellendi.");
                StoklariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
        }

        private void StoklariYukle()
        {
            try
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();

                baglanti.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Stoklar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        row["UrunAdi"] = SifrelemeHelper.SifreCoz(row["UrunAdi"].ToString());
                        row["Kategori"] = SifrelemeHelper.SifreCoz(row["Kategori"].ToString());
                        row["Tedarikci"] = SifrelemeHelper.SifreCoz(row["Tedarikci"].ToString());
                    }
                    catch
                    {
                        row["UrunAdi"] = "";
                        row["Kategori"] = "";
                        row["Tedarikci"] = "";
                    }
                }

                dgvStoklar.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yükleme hatası: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (dgvStoklar.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvStoklar.SelectedRows[0].Cells["StokID"].Value);

                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                        baglanti.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM Stoklar WHERE StokID = @id", baglanti);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Seçili stok kaydı silindi.");
                    StoklariYukle();
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
            else
            {
                MessageBox.Show("Lütfen silmek için bir stok seçin.");
            }
        }
        private void DgvStoklar_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                var row = dgvStoklar.Rows[e.RowIndex];
                if (row.Cells["SonKullanmaTarihi"].Value != null)
                {
                    DateTime skt = Convert.ToDateTime(row.Cells["SonKullanmaTarihi"].Value);
                    DateTime bugun = DateTime.Today;
                    TimeSpan fark = skt - bugun;

                    if (fark.TotalDays <= 7)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral; // kırmızı
                    }
                    else if (fark.TotalDays <= 15)
                    {
                        row.DefaultCellStyle.BackColor = Color.Khaki; // sarı
                    }
                }
            }
            catch
            {
                // hatalı tarih formatı varsa atla
            }
        }

    }
}
