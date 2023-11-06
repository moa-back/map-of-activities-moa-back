using MapOfActivitiesAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MapOfActivitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;

        public ImageController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }
     
        [HttpGet("api/image/{fileName}")]
        public async Task<ActionResult> Image(string fileName)
        {
            try
            {
                var fileStreamResult = await _fileStorage.GetImage(fileName);
                return fileStreamResult;
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                // Відобразити помилку або залогувати її
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
