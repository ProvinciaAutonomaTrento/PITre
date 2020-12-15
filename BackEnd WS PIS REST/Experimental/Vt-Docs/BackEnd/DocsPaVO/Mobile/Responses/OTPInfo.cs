namespace DocsPaVO.Mobile.Responses
{
    public class OTPInfo
    {
        private string idUtente;
        private string email;

        public OTPInfo(string idUtente, string email)
        {
            this.idUtente = idUtente;
            this.email = email;
        }

        public string Email { get => email;  }
        public string UserId { get => idUtente;  }
    }
}