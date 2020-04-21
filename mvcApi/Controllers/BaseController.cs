using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mvcApi.Models;

namespace mvcApi.Controllers
{
    public class BaseController : ApiController
    {


        public bool verificarToken(string token)
        {

            using (var db=new mvcApiEntities())
            {
                var existUser = from d in db.users
                                where d.token == token && d.idStatus == 1
                                select d;

                if (existUser.FirstOrDefault()!=null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
    }
}
