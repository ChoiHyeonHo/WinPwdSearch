using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace WinPwdSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            //유효성검사
            if (txtuid.Text.Trim().Length < 1 || txtpwd.Text.Trim().Length < 1)
            {
                MessageBox.Show("제대로 입력하여주세요");
            }
            MemberDB db = new MemberDB();
            Member member = db.Login(txtuid.Text.Trim(), txtpwd.Text.Trim());
            db.Dispose();

            if (member == null)
            {
                MessageBox.Show("회원 정보가 없습니다. 다시 확인하여 주십시오.");
            }
            else
            {
                if (member.IsAdmin == "Y")
                {
                    MessageBox.Show("관리자로 로그인하셨습니다. ");
                }
                else
                {
                    MessageBox.Show($"{member.Name}님 환영합니다.");
                }
                
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlSearch.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //유효성 검사
            if (txtIDS.Text.Trim().Length < 1 || txtNameS.Text.Trim().Length < 1 || txtEmailS.Text.Trim().Length < 1)
            {
                return;
            }

            MemberDB db = new MemberDB();
            //입력 정보가 적합한지 체크
            int result = db.SearchPwd(txtIDS.Text.Trim(), txtNameS.Text.Trim(), txtEmailS.Text.Trim());

            if (result < 1)
            {
                MessageBox.Show("회원정보가 없습니다.");
                return;
            }

            //비밀번호 생성 로직
            //새로운 비밀번호를 난수로 생성해서
            string newPwd = CreatePassWord();

            //새로운 비밀번호로 회원정보를 update하고,
            bool flag = db.UpdatePwd(newPwd, txtIDS.Text.Trim());
            
            if (flag)
            {
                //새로운 비밀번호를 메일로 발송해 주는것
                flag = SendMail(txtNameS.Text, txtEmailS.Text, txtIDS.Text, newPwd); //flag가 있으니 그냥 계속 쓰는것. 같은 변수에 다른 값 넣음
                if (flag)
                {
                    MessageBox.Show("초기화된 비밀번호를 Email로 발송하였습니다.");
                }
                else
                {
                    MessageBox.Show("메일 발송 중 오류가 발생했습니다.");
                }
            }
            else
            {
                MessageBox.Show("비밀번호 변경 중 오류가 발생했습니다.");
            }
        }

        private bool SendMail(string name, string email, string id, string newPwd)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.UseDefaultCredentials = false; //시스템에 설정된 인증 정보를 사용하지 않는다.
                client.EnableSsl = true; //SSL을 사용한다.
                client.DeliveryMethod = SmtpDeliveryMethod.Network; //네트워크로 전달받겠다.
                client.Credentials = new NetworkCredential("hyeonho5304@gmail.com", "chlgusgh2@");


                MailAddress mailTo = new MailAddress(email);
                MailAddress mailFrom = new MailAddress("hyeonho5304@gmail.com");
                MailMessage message = new MailMessage(mailFrom, mailTo);
                message.Subject = $"{name}님의 비밀번호 초기화 안내 메일입니다.";
                message.Body = $"{name}, 당신의 비밀번호. {newPwd}로 대체되었다.";
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;

                client.Send(message);

                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        private string CreatePassWord()
        {
            Random rnd = new Random();

            //신규 비밀번호 = 난수 8자리 (영문대문자 + 숫자)
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int val = rnd.Next(0, 36); //0~35

                if (val < 10) // 난수가 10보다 작으면 숫자
                    sb.Append(val);
                else //영문대문자 (65~90)
                    sb.Append((char)(val + 55));
            }
            return sb.ToString();
        } //비밀번호 난수 생성
    }
}
