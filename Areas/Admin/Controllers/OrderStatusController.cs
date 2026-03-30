using Boulevard.Models;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class OrderStatusController : Controller
    {
        private readonly OrderStatusDataAccess _orderStatusDataAccess;
        public OrderStatusController()
        {
            _orderStatusDataAccess = new OrderStatusDataAccess();
        }
        // GET: Admin/OrderStatus
        public async Task<ActionResult> Index()
        {
            try
            {
                var modelData = await _orderStatusDataAccess.GetAll();
                if (modelData.Count > 0)
                {
                    return View(modelData);
                }
                else
                {
                    return View(new List<OrderStatus>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                OrderStatus data = new OrderStatus();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _orderStatusDataAccess.GetOrderStatusByKey(key);
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdate(OrderStatus model)
        {
            try
            {
                if (model != null)
                {
                    if (model.OrderStatusId == 0)
                    {
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _orderStatusDataAccess.Create(model);
                    }
                    else
                    {
                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _orderStatusDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "OrderStatus");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                OrderStatus modelData = await _orderStatusDataAccess.GetOrderStatusById(id);

                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Delete";
                await _orderStatusDataAccess.Update(modelData);
                return RedirectToAction("Index", "OrderStatus");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}