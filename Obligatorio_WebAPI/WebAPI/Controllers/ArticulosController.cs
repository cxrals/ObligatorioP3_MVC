﻿using DataTransferObjects;
using LogicaAplicacion.InterfacesCasosUso;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase {
        public ICUOrdenarArticulosAsc CUOrdenarArticulosAsc { get; set; }
        public ArticulosController(ICUOrdenarArticulosAsc cuOrdenarArticulosAsc) {
            CUOrdenarArticulosAsc = cuOrdenarArticulosAsc;
        }

        // GET: api/<ArticulosController>
        [HttpGet]
        public IActionResult Get() {
            try {
                List<ArticuloDTO> articuloDTOs = CUOrdenarArticulosAsc.OrdenarPorNombreAsc();
                if (articuloDTOs.Any()) {
                    // 200 ok
                    return Ok(articuloDTOs);
                } else {
                    // 404 - Not Found
                    return NotFound("No existen artículos.");
                }
            } catch (Exception ex) {
                // 500 - Internal Server Error
                return StatusCode(500, "Ocurrió un error en el servidor.");
            }
        }
    }
}
