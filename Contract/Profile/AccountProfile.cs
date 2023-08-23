using Backend.Contract.Entity;
using Backend.Contract.Entity.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Contract.Profile
{
    public class AccountProfile : AutoMapper.Profile
    {
        public AccountProfile()
        {
            CreateMap<EmailRegisterVO, Account>();
        }
    }
}
