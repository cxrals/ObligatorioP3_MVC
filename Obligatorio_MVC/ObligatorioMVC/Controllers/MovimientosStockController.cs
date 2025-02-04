﻿using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Obligatorio.Filters;

namespace ObligatorioMVC.Controllers {
    public class MovimientosStockController : Controller {

        public string UrlApi;

        public MovimientosStockController(IConfiguration config) {
            UrlApi = config.GetValue<string>("ApiUrl");
        }

        // GET: MovimientosStockController
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult Index() {
            List<MovimientoStockIndexDTO> movimientosDeStock = new List<MovimientoStockIndexDTO>();

            try {
                HttpClient client = new HttpClient();
                string url = UrlApi + "movimientosStock";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = client.GetAsync(url);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string body = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    movimientosDeStock = JsonConvert.DeserializeObject<List<MovimientoStockIndexDTO>>(body);
                } else {
                    ViewBag.ErrorMsg = respuesta.Content.ReadAsStringAsync().Result;
                }
            } catch (Exception e) {
                ViewBag.ErrorMsg = e.Message;
            }

            return View(movimientosDeStock);
        }

        // GET: MovimientosStockController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        //--------------------------------------------------------------------------
        //----------------------------- CREATE -------------------------------------
        //--------------------------------------------------------------------------

        // GET: MovimientosStockController/Create
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult Create() {
            MovimientoStockDTO movimientoDeStock = new MovimientoStockDTO();
            try {
                movimientoDeStock.Articulos = ObtenerArticulos();
                movimientoDeStock.TiposMovimientos = ObtenerTiposDeMovimientos();
            } catch (Exception e) {
                ViewBag.ErrorMsg = e.Message;
            }
            return View(movimientoDeStock);
        }

        // POST: MovimientosStockController/Create
        [HttpPost]
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult Create(MovimientoStockDTO movimientoDeStock) {
            movimientoDeStock.EmailUsuario = HttpContext.Session.GetString("Email");
            try {
                HttpClient cliente = new HttpClient();
                string url = UrlApi + "movimientosStock";
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = cliente.PostAsJsonAsync(url, movimientoDeStock);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string body = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    return RedirectToAction(nameof(Index));
                } else {
                    ViewBag.ErrorMsg = respuesta.Content.ReadAsStringAsync().Result;
                }
            } catch { }
            movimientoDeStock.Articulos = ObtenerArticulos();
            movimientoDeStock.TiposMovimientos = ObtenerTiposDeMovimientos();
            return View(movimientoDeStock);
        }

        //--------------------------------------------------------------------------
        //----------------------------- BUSCAR -------------------------------------
        //--------------------------------------------------------------------------
        [HttpGet]
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult BuscarPorFecha(string desde, string hasta, int? page) {
            if (page == null) page = 1;
            List<ArticuloDTO> articulosConMovimientosDeStock = new List<ArticuloDTO>();

            try {
                HttpClient client = new HttpClient();
                string url = UrlApi + $"MovimientosStock/MovimientosPorFecha/{desde}/{hasta}/{page}";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = client.GetAsync(url);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string body = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    articulosConMovimientosDeStock = JsonConvert.DeserializeObject<List<ArticuloDTO>>(body);
                    double cantidadPaginas = ObtenerCantidadPaginas($"MovimientosStock/CantidadDePaginasFechas/{desde}/{hasta}");
                    ViewBag.Paginas = Math.Ceiling(cantidadPaginas);
                    ViewBag.Desde = desde;
                    ViewBag.Hasta = hasta;
                } else {
                    ViewBag.ErrorMsg = respuesta.Content.ReadAsStringAsync().Result;
                }
            } catch (Exception e) {
                ViewBag.ErrorMsg = e.Message;
            }

            return View(articulosConMovimientosDeStock);
        }

        [HttpPost]
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult BuscarPorFecha(string desde, string hasta) {
            return RedirectToAction("BuscarPorFecha", new { desde, hasta, page = 1 });
        }

        [HttpGet]
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult BuscarPorArticuloYTipo(int idArticulo, string tipoMovimiento, int? page) {
            if (page == null) page = 1;

            List<MovimientoStockIndexDTO> movimientosDeStock = new List<MovimientoStockIndexDTO>();

            try {
                HttpClient client = new HttpClient();
                string url = UrlApi + $"MovimientosStock/MovimientosPorArticuloYTipo/{idArticulo}/{tipoMovimiento}/{page}";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = client.GetAsync(url);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string body = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    movimientosDeStock = JsonConvert.DeserializeObject<List<MovimientoStockIndexDTO>>(body);
                    double cantidadPaginas = ObtenerCantidadPaginas($"MovimientosStock/CantidadDePaginas/{idArticulo}/{tipoMovimiento}");
                    ViewBag.Paginas = Math.Ceiling(cantidadPaginas);
                    ViewBag.ArticuloId = idArticulo;
                    ViewBag.TipoMov = tipoMovimiento;
                } else {
                    ViewBag.ErrorMsg = respuesta.Content.ReadAsStringAsync().Result;
                }
            } catch (Exception e) {
                ViewBag.ErrorMsg = e.Message;
            }

            return View(movimientosDeStock);
        }

        [HttpPost]
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult BuscarPorArticuloYTipo(int idArticulo, string tipoMovimiento) {
            return RedirectToAction("BuscarPorArticuloYTipo", new { idArticulo, tipoMovimiento, page = 1 });
        }

        //--------------------------------------------------------------------------
        //----------------------------- RESUMEN ------------------------------------
        //--------------------------------------------------------------------------
        [Privado(TipoUsuarios = "Encargado")]
        public ActionResult ObtenerResumen() {
            List<MovimientoCantidadPorAnioYTipoDTO> movimientosDeStock = new List<MovimientoCantidadPorAnioYTipoDTO>();
            List<MovimientoStockResumenDTO> resumenMS = new List<MovimientoStockResumenDTO>();

            try {
                HttpClient client = new HttpClient();
                string url = UrlApi + "MovimientosStock/ResumenMovimientos";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = client.GetAsync(url);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string body = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    movimientosDeStock = JsonConvert.DeserializeObject<List<MovimientoCantidadPorAnioYTipoDTO>>(body);

                    resumenMS = movimientosDeStock
                    .GroupBy(mse => mse.Anio)
                    .Select(mse => new MovimientoStockResumenDTO {
                        Anio = mse.Key,
                        TipoCantidad = mse.Select(item => $"Tipo: {item.TipoMovimiento} - Cantidad: {item.Cantidad}").ToList(),
                        CantidadTotalAnio = mse.Sum(item => item.Cantidad)
                    })
                    .ToList();
                } else {
                    ViewBag.ErrorMsg = respuesta.Content.ReadAsStringAsync().Result;
                }
            } catch (Exception e) {
                ViewBag.ErrorMsg = e.Message;
            }

            return View(resumenMS);
        }

        //--------------------------------------------------------------------------
        //----------------------------- UTILS --------------------------------------
        //--------------------------------------------------------------------------
        public List<ArticuloDTO> ObtenerArticulos() {
            List<ArticuloDTO> articulos = new List<ArticuloDTO>();

            HttpClient cliente = new HttpClient();
            string url = UrlApi + "articulos";
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var tarea = cliente.GetAsync(url);
            tarea.Wait();
            var respuesta = tarea.Result;

            HttpContent content = respuesta.Content;
            var tarea2 = content.ReadAsStringAsync();
            tarea2.Wait();
            string body = tarea2.Result;

            if (respuesta.IsSuccessStatusCode) {
                articulos = JsonConvert.DeserializeObject<List<ArticuloDTO>>(body);
            }

            return articulos;
        }

        public List<TipoMovimientoDTO> ObtenerTiposDeMovimientos() {
            List<TipoMovimientoDTO> tiposDeMovimientos = new List<TipoMovimientoDTO>();

            HttpClient cliente = new HttpClient();
            string url = UrlApi + "tiposMovimientos";
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var tarea = cliente.GetAsync(url);
            tarea.Wait();
            var respuesta = tarea.Result;

            HttpContent content = respuesta.Content;
            var tarea2 = content.ReadAsStringAsync();
            tarea2.Wait();
            string body = tarea2.Result;

            if (respuesta.IsSuccessStatusCode) {
                tiposDeMovimientos = JsonConvert.DeserializeObject<List<TipoMovimientoDTO>>(body);
            }

            return tiposDeMovimientos;
        }

        public double ObtenerCantidadPaginas(string url) {
            double cantidadPaginas = 0;
            try {
                HttpClient cliente = new HttpClient();
                string requestUrl = UrlApi + url;
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var tarea = cliente.GetAsync(requestUrl);
                tarea.Wait();
                var respuesta = tarea.Result;

                HttpContent content = respuesta.Content;
                var tarea2 = content.ReadAsStringAsync();
                tarea2.Wait();
                string contenido = tarea2.Result;

                if (respuesta.IsSuccessStatusCode) {
                    contenido = contenido.Replace(".", ",");
                    double.TryParse(contenido, out cantidadPaginas);
                } else if ((int)respuesta.StatusCode == StatusCodes.Status400BadRequest || (int)respuesta.StatusCode == StatusCodes.Status500InternalServerError) {
                    cantidadPaginas = -1;
                }
            } catch (Exception ex) {
                throw;
            }
            return cantidadPaginas;
        }

    }
}
