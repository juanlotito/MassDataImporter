using importacionmasiva.api.net.Models;
using importacionmasiva.api.net.Services.Interface;
using importacionmasiva.api.net.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace importacionmasiva.api.net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportacionController : ControllerBase
    {
        private readonly IImportacionService _importacionService;

        public ImportacionController(IImportacionService importacionService)
        {
            _importacionService = importacionService;
        }

        [HttpPost("/api/xlsx/{tableName}/importacion/{registryName}/")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFromExcel([FromForm, Required] List<IFormFile> dataset, string registryName, string tableName) 
        {
            try 
            {
                await _importacionService.ImportFromExcel(dataset.FirstOrDefault(), registryName, tableName);

                return Ok(new Response(200, false, "Se realizó la importación correctamente."));
            }
            catch (CustomException ex)
            {
                var response = new Response(ex?.Code ?? 500, true, ex?.Message ?? "Hubo un error no controlado.");

                return StatusCode(response.status, response);
            }
        }

        [HttpPost("/api/txt/{tableName}/importacion/{registryName}/")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFromTxt([FromForm, Required] List<IFormFile> dataset, string registryName, string tableName)
        {
            try
            {
                await _importacionService.ImportFromTxt(dataset.FirstOrDefault(), registryName, tableName);

                return Ok(new Response(200, false, "Se realizó la importación correctamente."));
            }
            catch (CustomException ex)
            {
                var response = new Response(ex?.Code ?? 500, true, ex?.Message ?? "Hubo un error no controlado.");

                return StatusCode(response.status, response);
            }
        }

    }
}
