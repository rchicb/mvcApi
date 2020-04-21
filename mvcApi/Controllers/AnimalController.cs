using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mvcApi.Models.WS;
using mvcApi.Models;
using mvcApi.Models.WS.TableViewModels;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text;

namespace mvcApi.Controllers
{
    public class AnimalController : BaseController
    {
        [HttpPost]
        public Reply listaAnimales([FromBody]SecurityViewModel model)
        {
            Reply oReply = new Reply();
            oReply.result = 0;

            if (!verificarToken(model.token))
            {
                oReply.message = "usuario no autorizado";
                return oReply;
            }

            using (var db=new mvcApiEntities())
            {
                oReply.data = obtenerLista(db);
                oReply.result = 1;
            }


                return oReply;

        }
    
        [HttpPost]
        public Reply crearAnimal([FromBody]AnimalViewModel model)
        {
            Reply oReply = new Reply();
            oReply.result = 0;

            if (!ModelState.IsValid)
            {
                oReply.message = "No se enviaron los datos correctamente";
                return oReply;
            }

            if (!verificarToken(model.Token))
            {
                oReply.message = "Usuario no autorizado";
                return oReply;
            }

            using (var db =new mvcApiEntities())
            {
                animal oAnimal = new animal();
                oAnimal.nombre = model.Nombre;
                oAnimal.patas = model.Patas;
                oAnimal.idState = 1;
                //db.animal.Add(oAnimal);
                
                db.Entry(oAnimal).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();

                

                oReply.data = obtenerLista(db);
                oReply.message = "El Animal " + model.Nombre + " fue creado con exito";
                oReply.result = 1;
            }



                return oReply;
        }
        
        [HttpPut]
        public Reply modificarAnimal([FromBody]EditAnimalViewModel model)
        {
            Reply oReply = new Reply();
            oReply.result = 0;

            if (!ModelState.IsValid)
            {
                oReply.message = "No se estan enviando bien los datos";
                return oReply;
            }

            if (!verificarToken(model.Token))
            {
                oReply.message = "Usuario no autorizado";
                return oReply;
            }

            using (var db=new mvcApiEntities()) {
                var oAnimal = db.animal.Find(model.Id);

                oAnimal.nombre = model.Nombre;
                oAnimal.patas = model.Patas;

                db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                oReply.data = obtenerLista(db);
                oReply.message = "El Animal "+model.Nombre+" fue modificado con exito";
                oReply.result = 1;
            }

            return oReply;

        }

        [HttpDelete]
        public Reply eliminarAnimal([FromBody]DeleteAnimalViewModel model)
        {
            Reply oReply = new Reply();
            oReply.result = 0;

            if (!ModelState.IsValid)
            {
                oReply.message = "No se estan enviando bien los datos";
                return oReply;
            }

            if (!verificarToken(model.Token))
            {
                oReply.message = "Usuario no autorizado";
                return oReply;
            }

            using (var db = new mvcApiEntities())
            {
                var oAnimal = db.animal.Find(model.Id);
                oAnimal.idState = 2;
                db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                oReply.data = obtenerLista(db);
                oReply.message = "El Animal " + model.Id + " fue eliminado con exito";
                oReply.result = 1;
            }

            return oReply;

        }

        [HttpPost]
        public async Task<Reply> Photo([FromUri]AnimalPictureViewModel model)
        {
            Reply oReply = new Reply();
            oReply.result = 0;
            
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            if (!verificarToken(model.token))
            {
                oReply.message = "Usuario no Autorizado";
                return oReply;
            }

            //viene multipart
            if (!Request.Content.IsMimeMultipartContent())
            {
                oReply.message = "no viene imagen en el contenido";
                return oReply;
            }

            await Request.Content.ReadAsMultipartAsync(provider);

            FileInfo fileInfoPicture = null;

            foreach (MultipartFileData filePhoto in provider.FileData)
            {
                if (filePhoto.Headers.ContentDisposition.Name.Replace("\"","").Replace("\\","").Equals("picture"))
                {
                    fileInfoPicture = new FileInfo(filePhoto.LocalFileName);
                }
            }

            if (fileInfoPicture!=null)
            {
                using (FileStream fs=fileInfoPicture.Open(FileMode.Open,FileAccess.Read))
                {
                    byte[] b = new byte[fileInfoPicture.Length];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b,0,b.Length)>0) ;


                    try
                    {

                        using (var db=new mvcApiEntities())
                        {
                            var oAnimal = db.animal.Find(model.Id);
                            oAnimal.picture = b;
                            db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            oReply.result = 1;
                            oReply.message = "Se actualizo la imagen";
                        }

                    }
                    catch (Exception ex)
                    {
                        oReply.message = "Error al leer y cargar a la matriz de bytes";

                    }

                }

            }

            return oReply;
        }
        #region Helper
        public List<AnimalTableViewModel> obtenerLista(mvcApiEntities db)
        {
            List<AnimalTableViewModel> lst = (from d in db.animal
                                              where d.idState == 1
                                              select new AnimalTableViewModel
                                              {
                                                  Nombre = d.nombre,
                                                  Patas = d.patas
                                              }).ToList();

            return lst;
        }

        #endregion
    }
}
