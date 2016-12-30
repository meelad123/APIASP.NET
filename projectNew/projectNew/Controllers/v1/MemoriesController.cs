using Amazon.S3;
using Amazon.S3.Model;
using projectNew.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Claims;

namespace projectNew.Controllers.v1
{
    [Authorize]
    public class MemoriesController : ApiController
    {
        #region variables

        string bucketName = "mybuckpics";
        string awsAccessKeyId = "AKIAJBN56DYISWA3AS4Q";
        string awsSecretAccessKey = "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";
        #endregion

        [AcceptVerbs("GET")]
        public IHttpActionResult search(string id, string type, string query)
        {
            using (var _db = new DBContext())
            {
                List<Memory> resultMemories = new List<Memory>();
                switch (type.ToLower())
                {
                    case "user":
                        resultMemories = _db.MyMemories
                                    .Where(mem => mem.Vacation.User.UserName.ToLower().Equals(query.ToLower())).ToList();
                        break;
                    case "place":
                        resultMemories = _db.MyMemories
                            .Where(mem => mem.Place.ToLower().Equals(query.ToLower())).ToList();
                        break;
                    case "title":
                        resultMemories = _db.MyMemories
                                    .Where(mem => mem.Title.ToLower().Equals(query.ToLower())).ToList();
                        break;
                    default:
                        break;
                }

                if (resultMemories.Count > 0)
                    return Ok(resultMemories);

                return NotFound();
            }
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult getMemory(int id)
        {
            using (var _db = new DBContext())
            {
                Memory memory = _db.MyMemories.FirstOrDefault(mem => mem.MemoryId == id);
                if (memory == null)
                    return NotFound();
                return Ok(memory);
            }
        }
        [AcceptVerbs("DELETE")]
        public IHttpActionResult deleteMemory(int id)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                string userName = identity.Claims.ElementAt(0).Value;
                Memory memory = _db.MyMemories.Include("Media").FirstOrDefault(mem => mem.MemoryId == id);
                if (memory == null)
                    return NotFound();
                if (memory.Vacation.User.UserName == userName)
                {
                    _db.MyMemories.Remove(memory);
                    _db.SaveChanges();
                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
                    
            }
        }
        [AcceptVerbs("GET")]
        ///api/v1/memories/id/media-objects
        public IHttpActionResult getMediaObject(int id, string modifier)
        {
            List<Media> result = new List<Media>();
            using (var _db = new DBContext())
            {
                Memory memory = _db.MyMemories.Include("Media").FirstOrDefault(mem => mem.MemoryId == id);
                if (memory == null)
                    return NotFound();
                foreach (Media m in memory.Media)
                {
                    m.Memory = null;
                    if (m.Type == modifier)
                    {
                        result.Add(m);
                    }
                }
                
                return Ok(result);
            }
        }




        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> addMedia(int id, string modifier, [FromUri]SoundMedia smedia, [FromUri]PictureMedia pMedia, [FromUri]VideoMedia vMedia)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                string userName = identity.Claims.ElementAt(0).Value;
                Memory memory = _db.MyMemories.Include("Vacation").Include("Vacation.User").FirstOrDefault(mem => mem.MemoryId == id);
                if (memory.Vacation.User.UserName == userName)
                {
                    var randomName = Path.GetRandomFileName();
                    Media m = new Media();
                    switch (modifier)
                    {
                        case "sound":
                            m = smedia;
                            m.Type = "sound";
                            break;
                        case "video":
                            m = vMedia;
                            m.Type = "video";
                            break;
                        case "picture":
                            m = pMedia;
                            m.Type = "picture";
                            break;
                        default:
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                    }



                    InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider());


                    IList<HttpContent> files = provider.Files;

                    HttpContent file = files[0];
                    Stream fileStream = await file.ReadAsStreamAsync();

                    string fileExt = Path.GetExtension(file.Headers.ContentDisposition.FileName);

                    m.Url = randomName + fileExt;
                    m.MemoryId = id;
                    _db.MyMedia.Add(m);
                    _db.SaveChanges();
                    try
                    {
                        using (var client = new AmazonS3Client(
                            awsAccessKeyId,
                            awsSecretAccessKey,
                            Amazon.RegionEndpoint.EUCentral1))
                        {
                            client.PutObject(new PutObjectRequest()
                            {
                                InputStream = fileStream,
                                BucketName = bucketName,
                                Key = randomName + fileExt
                            });
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    catch (System.Exception e)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                    }
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

    }
}
