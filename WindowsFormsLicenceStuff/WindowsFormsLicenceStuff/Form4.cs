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
using System.Security.Cryptography;

namespace WindowsFormsLicenceStuff
{
    public partial class Form4 : Form
    {
        String conString = "Data Source=EN614211;Initial Catalog=LicenceDB;Integrated Security=True";
        public Form4()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int maxUserIdString = 0;
            int nextUserId = 0;
            SqlConnection con = new SqlConnection(conString);
            if (verifyPassword(textBox2.Text) && verifyUsername(textBox1.Text))
            {
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    String maxQuery = "SELECT max(UserID) FROM [LicenceDB].[dbo].[Users]";
                    SqlCommand addUser = new SqlCommand(maxQuery, con);
                    SqlDataReader objReader = addUser.ExecuteReader();
                    if (objReader.HasRows)
                    {
                        while (objReader.Read())
                        {
                            maxUserIdString = objReader.GetInt32(0);
                        }
                    }
                    nextUserId = maxUserIdString + 1;
                    con.Close();
                }
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    String insertUserQuery = "INSERT INTO [LicenceDB].[dbo].[Users] (UserID, UserName, Password) " +
                            "VALUES (" + nextUserId + ", '" + textBox1.Text + "', '" + Encrypt(textBox2.Text) + "')";
                    SqlCommand insertUserCommand = new SqlCommand(insertUserQuery, con);

                    insertUserCommand.ExecuteNonQuery();
                    con.Close();
                }

                MessageBox.Show("New Account Created", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        private Boolean verifyUsername(String user)
        {
            List<String> usernames = new List<String>();
            SqlConnection con = new SqlConnection(conString);

            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                String userQuery = "SELECT [UserName] FROM [LicenceDB].[dbo].[Users]";
                SqlCommand usernameCmd = new SqlCommand(userQuery, con);
                using (SqlDataReader objReader = usernameCmd.ExecuteReader())
                {
                    if (objReader.HasRows)
                    {
                        while (objReader.Read())
                        {
                            String username = objReader.GetString(objReader.GetOrdinal("UserName")).Trim();
                            usernames.Add(username);
                        }
                    }
                }
                con.Close();

                if (!usernames.Contains(textBox1.Text)) {
                    return true;
                }
            }

            MessageBox.Show("User already exists", "Username error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private Boolean verifyPassword(String pass)
        {
            if (textBox2.Text.Equals(textBox3.Text))
                return true;

            MessageBox.Show("Passwords do not match", "Password Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        public String Encrypt(String plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //return as base64 string
            return Convert.ToBase64String(HashPassword(plainText));
        }

        public static byte[] HashPassword(String password)
        {
            var provider = new SHA1CryptoServiceProvider();
            var encoding = new UnicodeEncoding();
            return provider.ComputeHash(encoding.GetBytes(password));
        }
    }
}