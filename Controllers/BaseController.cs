using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return await action();
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Error = vex.Message });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { Error = knf.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Error inesperado.", Details = ex.Message });
            }
        }
    }
}
