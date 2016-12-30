using Amazon.S3;
using Amazon.S3.Model;
using projectNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace projectNew.Controllers.v1
{
    public class MediaController : ApiController
    {

        #region variables

        string bucketName = "mybuckpics";
        string awsAccessKeyId = "AKIAJBN56DYISWA3AS4Q";
        string awsSecretAccessKey = "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";
        #endregion

        [AcceptVerbs("DELETE")]
        public IHttpActionResult deleteMedia(int id)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                string userName = identity.Claims.ElementAt(0).Value;
                Media media = _db.MyMedia.Include("Memory.Vacation.User").FirstOrDefault(med => med.Id == id);
                if (media == null)
                    return NotFound();
                if (media.Memory.Vacation.User.UserName == userName)
                {
                    try
                    {
                        using (var client = new AmazonS3Client(
                            awsAccessKeyId,
                            awsSecretAccessKey,
                            Amazon.RegionEndpoint.EUCentral1))
                        {
                            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = bucketName,
                                Key = media.Url
                            };
                            try
                            {
                                client.DeleteObject(deleteObjectRequest);
                            }
                            catch (AmazonS3Exception s3Exception)
                            {
                                Console.WriteLine(s3Exception.Message,
                                                  s3Exception.InnerException);
                            }
                        }
                        _db.MyMedia.Remove(media);
                        _db.SaveChanges();
                        return Ok();
                    }
                    catch (System.Exception e)
                    {
                        return StatusCode(HttpStatusCode.InternalServerError);
                    }
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);

            }
        }
    }
}
