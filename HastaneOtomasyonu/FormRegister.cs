using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace HastaneOtomasyonu
{
    public partial class FormRegister : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        TextBox txtKullaniciAdi, txtSifre, txtSifreTekrar;
        TextBox txtAd, txtSoyad;
        ComboBox cmbYetki, cmbBrans;
        Button btnKayitOl, btnGeriDon;

        public FormRegister()
        {
            InitializeComponent();
            FormuHazirla();
        }
        private void FormuHazirla()
        {
            this.Text = "Yeni Üyelik Oluştur";
            this.Size = new Size(480, 650);
            this.MinimumSize = new Size(480, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            int labelX = 40;
            int inputX = 180;
            int width = 240;
            int height = 30;
            int spacingY = 50;
            int currentY = 30;

            // Kullanıcı Adı
            this.Controls.Add(new Label { Text = "Kullanıcı Adı:", Location = new Point(labelX, currentY), Font = genelFont });
            txtKullaniciAdi = new TextBox { Location = new Point(inputX, currentY - 5), Width = width, Font = genelFont };
            this.Controls.Add(txtKullaniciAdi);

            // Şifre
            currentY += spacingY;
            this.Controls.Add(new Label { Text = "Şifre:", Location = new Point(labelX, currentY), Font = genelFont });
            txtSifre = new TextBox { Location = new Point(inputX, currentY - 5), Width = width, Font = genelFont, PasswordChar = '●' };
            this.Controls.Add(txtSifre);

            // Şifre Tekrar
            currentY += spacingY;
            this.Controls.Add(new Label { Text = "Şifre Tekrar:", Location = new Point(labelX, currentY), Font = genelFont });
            txtSifreTekrar = new TextBox { Location = new Point(inputX, currentY - 5), Width = width, Font = genelFont, PasswordChar = '●' };
            this.Controls.Add(txtSifreTekrar);

            // Şifre Kuralları
            currentY += spacingY;
            Label lblKurallar = new Label
            {
                Text = "Şifre Kuralları: - En az 8 karakter - 1 büyük harf - 1 küçük harf - 1 rakam - 1 özel karakter (!@#$%^&*)",
                Location = new Point(labelX, currentY),
                Size = new Size(380, 80),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblKurallar);

            currentY += 90;
            // Yetki
            this.Controls.Add(new Label { Text = "Yetki:", Location = new Point(labelX, currentY), Font = genelFont });
            cmbYetki = new ComboBox
            {
                Location = new Point(inputX, currentY - 5),
                Width = width,
                Font = genelFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbYetki.Items.AddRange(new string[] { "Admin", "Doktor", "Hemşire", "Sekreter", "Danışma", "Muhasebe", "Laboratuvar", "Depo Sorumlusu" });
            cmbYetki.SelectedIndexChanged += CmbYetki_SelectedIndexChanged;
            this.Controls.Add(cmbYetki);

            // Doktor Bilgileri
            currentY += spacingY;
            this.Controls.Add(new Label { Text = "Ad:", Location = new Point(labelX, currentY), Font = genelFont });
            txtAd = new TextBox { Location = new Point(inputX, currentY - 5), Width = width, Font = genelFont, Visible = false };
            this.Controls.Add(txtAd);

            currentY += spacingY;
            this.Controls.Add(new Label { Text = "Soyad:", Location = new Point(labelX, currentY), Font = genelFont });
            txtSoyad = new TextBox { Location = new Point(inputX, currentY - 5), Width = width, Font = genelFont, Visible = false };
            this.Controls.Add(txtSoyad);

            currentY += spacingY;
            this.Controls.Add(new Label { Text = "Branş:", Location = new Point(labelX, currentY), Font = genelFont });
            cmbBrans = new ComboBox
            {
                Location = new Point(inputX, currentY - 5),
                Width = width,
                Font = genelFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            cmbBrans.Items.AddRange(new string[] {
        "Kardiyoloji", "Dahiliye", "Ortopedi", "Genel Cerrahi", "Nöroloji", "Psikiyatri",
        "Göz Hastalıkları", "KBB", "Üroloji", "Kadın Doğum", "Çocuk Sağlığı",
        "Anestezi", "Acil Servis"
    });
            this.Controls.Add(cmbBrans);

            // Kayıt Ol Butonu
            currentY += spacingY + 10;
            btnKayitOl = new Button
            {
                Text = "Kayıt Ol",
                Location = new Point(inputX, currentY),
                Width = width,
                Height = 40,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnKayitOl.Click += BtnKayitOl_Click;
            this.Controls.Add(btnKayitOl);

            // Geri Dön Butonu
            currentY += 55;
            btnGeriDon = new Button
            {
                Text = "Geri Dön",
                Location = new Point(inputX, currentY),
                Width = width,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnGeriDon.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnGeriDon);
        }


        private void CmbYetki_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool doktorMu = cmbYetki.Text == "Doktor";
            txtAd.Visible = doktorMu;
            txtSoyad.Visible = doktorMu;
            cmbBrans.Visible = doktorMu;
        }
        private bool SifreGuvenliMi(string sifre, out string hataMesaji)
        {
            hataMesaji = "";

            if (sifre.Length < 8)
                hataMesaji += "- Şifre en az 8 karakter olmalıdır.\n";
            if (!sifre.Any(char.IsUpper))
                hataMesaji += "- En az 1 büyük harf içermelidir.\n";
            if (!sifre.Any(char.IsLower))
                hataMesaji += "- En az 1 küçük harf içermelidir.\n";
            if (!sifre.Any(char.IsDigit))
                hataMesaji += "- En az 1 rakam içermelidir.\n";
            if (!sifre.Any(ch => "!@#$%^&*()_-+=<>?/{}[]|".Contains(ch)))
                hataMesaji += "- En az 1 özel karakter (!@#$%^&* vb.) içermelidir.\n";

            return string.IsNullOrEmpty(hataMesaji);
        }

        private void BtnKayitOl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text) || string.IsNullOrWhiteSpace(txtSifre.Text) ||
                string.IsNullOrWhiteSpace(txtSifreTekrar.Text) || cmbYetki.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!");
                return;
            }

            // Şifre güvenlik kontrolü
            if (!SifreGuvenliMi(txtSifre.Text, out string hata))
            {
                MessageBox.Show("Şifre güvenli değil:\n\n" + hata, "Geçersiz Şifre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtSifre.Text != txtSifreTekrar.Text)
            {
                MessageBox.Show("Şifreler uyuşmuyor!");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand kontrol = new SqlCommand("SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @KullaniciAdi", baglanti);
                kontrol.Parameters.AddWithValue("@KullaniciAdi", txtKullaniciAdi.Text);

                int kullaniciSayisi = (int)kontrol.ExecuteScalar();

                if (kullaniciSayisi > 0)
                {
                    MessageBox.Show("Bu kullanıcı adı zaten alınmış.");
                    return;
                }

                int doktorID = -1;

                if (cmbYetki.Text == "Doktor")
                {
                    SqlCommand doktorKomut = new SqlCommand(
                        "INSERT INTO Doktorlar (Ad, Soyad, Branş, Telefon) OUTPUT INSERTED.DoktorID VALUES (@Ad, @Soyad, @Brans, '')", baglanti);

                    doktorKomut.Parameters.AddWithValue("@Ad", SifrelemeHelper.Sifrele(txtAd.Text));
                    doktorKomut.Parameters.AddWithValue("@Soyad", SifrelemeHelper.Sifrele(txtSoyad.Text));
                    doktorKomut.Parameters.AddWithValue("@Brans", SifrelemeHelper.Sifrele(cmbBrans.Text)); 


                    doktorID = (int)doktorKomut.ExecuteScalar();
                }

                SqlCommand kullaniciKomut = new SqlCommand(
                    "INSERT INTO Kullanicilar (KullaniciAdi, Sifre, Yetki, DoktorID) VALUES (@KullaniciAdi, @Sifre, @Yetki, @DoktorID)", baglanti);

                kullaniciKomut.Parameters.AddWithValue("@KullaniciAdi", txtKullaniciAdi.Text);

                kullaniciKomut.Parameters.AddWithValue("@Sifre", SifrelemeHelper.Sifrele(txtSifre.Text));
                kullaniciKomut.Parameters.AddWithValue("@Yetki", cmbYetki.Text);
                if (doktorID != -1)
                    kullaniciKomut.Parameters.AddWithValue("@DoktorID", doktorID);
                else
                    kullaniciKomut.Parameters.AddWithValue("@DoktorID", DBNull.Value);

                kullaniciKomut.ExecuteNonQuery();

                MessageBox.Show("Kayıt başarılı! Şimdi giriş yapabilirsiniz.");
                this.Close();
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

    }
}
