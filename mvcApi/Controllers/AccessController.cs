using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mvcApi.Models.WS;
using mvcApi.Models;

namespace mvcApi.Controllers
{
    public class AccessController : ApiController
    {   
        [HttpGet]
        public Reply index()
        {
            Reply oRetry = new Reply();
            oRetry.result = 1;
            oRetry.data =new { nombre="Raul"};
            oRetry.message = "Hello world";
            return oRetry;
        }


        [HttpPost]
        public Reply Login([FromBody]AccessViewModel model)
        {

            Reply oReply = new Reply();
            oReply.result = 0;

            try
            {
                using (mvcApiEntities db=new mvcApiEntities())
                {
                    var oExistUser = from d in db.users
                                where d.email == model.email && d.password == model.password && d.idStatus == 1
                                select d;

                    if (oExistUser.FirstOrDefault()!=null)
                    {
                        oReply.result = 1;
                        oReply.data = Guid.NewGuid().ToString();

                        user oUser = oExistUser.First();
                        oUser.token = oReply.data.ToString();

                        db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        oReply.message = "Credenciales incorrectas";
                    }
                }


            }
            catch (Exception s)
            {
                oReply.message = "Error en la autenticacion" +s.Message;
            }

            return oReply;
        }
    }
}
