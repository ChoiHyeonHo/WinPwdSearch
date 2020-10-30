using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace WinPwdSearch
{
    public class Member // 로그인한 멤버의 정보를 담아두고 계속 달고다닌다.
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IsAdmin { get; set; }
    }


    public class MemberDB : IDisposable
    {
        MySqlConnection conn;
        public MemberDB()//생성자
        {

            conn = new MySqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["gudi"].ConnectionString;
            conn.Open();
        }


        public Member Login(string uid, string pwd)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT ID, Name, Email, Isadmin FROM membertbl where ID = @ID and Pwd = @Pwd;";
            cmd.Connection = conn;

            cmd.Parameters.Add("@ID", MySqlDbType.VarChar);
            cmd.Parameters["@ID"].Value = uid;

            cmd.Parameters.Add("@Pwd", MySqlDbType.VarChar);
            cmd.Parameters["@Pwd"].Value = pwd;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) //로그인 성공시 정보전달
            {
                Member member = new Member();
                member.ID = reader["ID"].ToString();
                member.Name = reader["Name"].ToString();
                member.Email = reader["Email"].ToString();
                member.IsAdmin = reader["Isadmin"].ToString();

                return member;
            }
            else //로그인 실패시 null
            {
                return null; //리턴 타입이 멤버이기 때문에 없으면 그냥 Null
            }
            
        } //로그인

        public int SearchPwd(string uid, string name, string Email)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = @"select count(*) from membertbl 
                                where ID = @ID and Name = @Name and Email = @Email;";
            cmd.Connection = conn;

            cmd.Parameters.Add("@ID", MySqlDbType.VarChar);
            cmd.Parameters["@ID"].Value = uid;

            cmd.Parameters.Add("@Name", MySqlDbType.VarChar);
            cmd.Parameters["@Name"].Value = name;

            cmd.Parameters.Add("@Email", MySqlDbType.VarChar);
            cmd.Parameters["@Email"].Value = Email;

            return Convert.ToInt32(cmd.ExecuteScalar());
        } //PWD 검증

        public bool UpdatePwd(string newPwd, string uid) // Pwd변경후 메일을 보내야함. 변경성공 여부를 bool타입으로 반환
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "update membertbl set Pwd = @Pwd where ID = @ID;";
                cmd.Connection = conn;

                cmd.Parameters.Add("@Pwd", MySqlDbType.VarChar);
                cmd.Parameters["@Pwd"].Value = newPwd;

                cmd.Parameters.Add("@ID", MySqlDbType.VarChar);
                cmd.Parameters["@ID"].Value = uid;

                //ExecuteNonQuery는 적용된 Row 수를 반환
                //정상적인 실행을 했을 때에도 적용된 행의 수에 따라서 다른 결과를 처리할 수 있다.
                int iRows = cmd.ExecuteNonQuery();

                if (iRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception err)
            {
                return false;
            }      
        }



        public void Dispose()
        {
            conn.Close();
        }
    }
}
