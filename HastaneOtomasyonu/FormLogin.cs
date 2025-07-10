using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class FormLogin : Form
    {
        SqlConnection baglanti = new SqlConnection(ConfigurationManager.ConnectionStrings["HastaneDB"].ConnectionString);

        TextBox txtKullaniciAdi, txtSifre;
        Button btnGirisYap, btnKayitOl;
        private int girisDenemeSayisi = 0;
        private DateTime sonDenemeZamani = DateTime.MinValue;
        private TimeSpan beklemeSuresi = TimeSpan.FromMinutes(1);
        private int maxDeneme = 5;

        public FormLogin()
        {
            InitializeComponent();
            FormuHazirla();
        }

        private void FormuHazirla()
        {
            this.Text = "Kullanıcı Girişi";
            this.Size = new Size(400, 350);
            this.MinimumSize = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Font genelFont = new Font("Segoe UI", 11, FontStyle.Regular);
            Color butonRenk = Color.SteelBlue;
            Color butonYaziRenk = Color.White;

            Label lblKullaniciAdi = new Label { Text = "Kullanıcı Adı:", Location = new Point(30, 50), AutoSize = true, Font = genelFont };
            this.Controls.Add(lblKullaniciAdi);
            txtKullaniciAdi = new TextBox { Location = new Point(150, 45), Width = 180, Font = genelFont };
            this.Controls.Add(txtKullaniciAdi);

            Label lblSifre = new Label { Text = "Parola:", Location = new Point(30, 100), AutoSize = true, Font = genelFont };
            this.Controls.Add(lblSifre);
            txtSifre = new TextBox { Location = new Point(150, 95), Width = 180, Font = genelFont, PasswordChar = '●' };
            this.Controls.Add(txtSifre);

            int formWidth = this.ClientSize.Width;
            int buttonWidth = 280;
            int centerX = (formWidth - buttonWidth) / 2;

            btnGirisYap = new Button
            {
                Text = "Giriş Yap",
                Location = new Point(centerX, 180),
                Width = buttonWidth,
                Height = 40,
                BackColor = butonRenk,
                ForeColor = butonYaziRenk,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnGirisYap.FlatAppearance.BorderSize = 0;
            btnGirisYap.Click += BtnGirisYap_Click;
            this.Controls.Add(btnGirisYap);

           /*btnKayitOl = new Button
            {
                Text = "Kayıt Ol",
                Location = new Point(centerX, 210), 
                Width = buttonWidth,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = genelFont,
                FlatStyle = FlatStyle.Flat
            };
            btnKayitOl.FlatAppearance.BorderSize = 0;
            btnKayitOl.Click += BtnKayitOl_Click;
            this.Controls.Add(btnKayitOl);*/
            
        }

        private void BtnGirisYap_Click(object sender, EventArgs e)
        {
            if (girisDenemeSayisi >= maxDeneme)
            {
                TimeSpan gecenSure = DateTime.Now - sonDenemeZamani;
                if (gecenSure < beklemeSuresi)
                {
                    int kalanSaniye = (int)(beklemeSuresi - gecenSure).TotalSeconds;
                    MessageBox.Show($"Çok fazla başarısız giriş denemesi yapıldı. Lütfen {kalanSaniye} saniye sonra tekrar deneyin.");
                    return;
                }
                else
                {
                    girisDenemeSayisi = 0;
                }
            }

            if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komut = new SqlCommand("SELECT * FROM Kullanicilar WHERE KullaniciAdi = @KullaniciAdi", baglanti);
                komut.Parameters.AddWithValue("@KullaniciAdi", txtKullaniciAdi.Text);

                SqlDataReader reader = komut.ExecuteReader();
                if (reader.Read())
                {
                    string sifreVeritabaninda = reader["Sifre"].ToString();
                    string cozulmusSifre = SifrelemeHelper.SifreCoz(sifreVeritabaninda);

                    if (cozulmusSifre == txtSifre.Text)
                    {
                        girisDenemeSayisi = 0;

                        

                        string yetki = reader["Yetki"].ToString();
                        int doktorID = -1;
                        try
                        {
                            var doktorValue = reader["DoktorID"];
                            if (doktorValue != DBNull.Value && int.TryParse(doktorValue.ToString(), out int parsedID))
                            {
                                doktorID = parsedID;
                            }
                        }
                        catch
                        {
                            doktorID = -1; 
                        }



                        MessageBox.Show("Giriş başarılı! Yetkiniz: " + yetki);
                        this.Hide();

                        if (yetki == "Admin")
                            new FormAdminPanel().Show();
                        else if (yetki == "Doktor")
                            new FormDoktorPanel(doktorID).Show();
                        else if (yetki == "Sekreter")
                            new FormSekreterPanel().Show();
                        else if (yetki == "Danışma")
                            new FormDanismaPanel().Show();
                        else if (yetki == "Depo Sorumlusu")
                            new FormStok().Show();
                        else
                        {
                            MessageBox.Show("Bu yetki için henüz bir panel tanımlanmadı. Lütfen sistem yöneticinizle görüşünüz.");
                            Application.Exit();
                        }
                    }
                    else
                    {
                        girisDenemeSayisi++;
                        sonDenemeZamani = DateTime.Now;
                        MessageBox.Show("Şifre yanlış!");
                    }
                }
                else
                {
                    girisDenemeSayisi++;
                    sonDenemeZamani = DateTime.Now;
                    MessageBox.Show("Kullanıcı adı bulunamadı!");
                }
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




        private void BtnKayitOl_Click(object sender, EventArgs e)
        {
            FormRegister formRegister = new FormRegister();
            formRegister.Show();
        }
    }
}
