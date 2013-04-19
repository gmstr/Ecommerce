﻿using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Payments;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IShippingStatusService _shippingStatusService;
        private readonly IPaymentStatusService _paymentStatusService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public OrderController(IOrderService orderService, IShippingStatusService shippingStatusService, IPaymentStatusService paymentStatusService, IShippingMethodManager shippingMethodManager)
        {
            _orderService = orderService;
            _shippingStatusService = shippingStatusService;
            _paymentStatusService = paymentStatusService;
            _shippingMethodManager = shippingMethodManager;
        }

        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            return View(_orderService.GetPaged(page));
        }

        [HttpGet]
        public ActionResult Edit(Order order)
        {
            ViewData["ShippingStatuses"] = _shippingStatusService.GetOptions();
            ViewData["PaymentStatuses"] = _paymentStatusService.GetOptions();
            ViewData["ShippingMethods"] = _shippingMethodManager.GetOptions();
            if (order.ShippingMethod != null)
                ViewData["ShippingMethodId"] = order.ShippingMethod.Id;
            return order != null
                       ? (ActionResult) View(order)
                       : RedirectToAction("Index");
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Order order, int shippingMethodId = 0)
        {
            if (shippingMethodId != 0)
                order.ShippingMethod = _shippingMethodManager.Get(shippingMethodId);
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            return RedirectToAction("Index");
        }
    }
}