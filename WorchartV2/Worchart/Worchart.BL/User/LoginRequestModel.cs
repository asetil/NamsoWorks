using Worchart.BL.Constants;
using Worchart.BL.Exceptions;

namespace Worchart.BL.User
{
    public class LoginRequestModel
    {
        public string CompanyCode { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public virtual bool IsValid()
        {
            if (!Email.ValidEmail())
            {
                throw new WorchartException(ErrorConstants.InvalidEmail);
            }
            return Password.Valid();
        }
    }
}
