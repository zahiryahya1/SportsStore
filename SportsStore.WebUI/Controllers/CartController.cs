using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;


/* use sessions to store and retrieve cart object because I want the cart to be assiciated with the users brower */
namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductsRepository repository;
        private IOrderProcessor orderProcessor;

        public CartController(IProductsRepository repo, IOrderProcessor proc)
        {
            repository = repo;
            orderProcessor = proc;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                ReturnUrl = returnUrl,
                Cart = cart
                }
            );
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productid, string returnUrl)
        {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productid);
            
            if (product != null)
            {
                cart.AddItem(product, 1);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                cart.RemoveLine(product);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            // check if thequantity is less then the actual amount in the db
            for (int i = 0; i < cart.Lines.Count(); i++)
            {
                Product tempInCart;
                tempInCart = cart.Lines.ElementAt(i).Product;

                var prod = repository.Products.FirstOrDefault(item => item.ProductID == tempInCart.ProductID);
                // should never happen. sanity check
                if (prod == null)
                {
                    ModelState.AddModelError("", "Product not found in Inventory!");
                    break;
                }

                if (cart.Lines.ElementAt(i).Quantity > prod.Amount)
                {
                    ModelState.AddModelError("", "Quantity selected exceeds inventory! Please remove Items from Cart.");
                }
            }

            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                // for each product, remove the amount
                for (int i = 0; i < cart.Lines.Count(); i++)
                {
                    var cartItem = cart.Lines.ElementAt(i);

                    repository.UpdateAmount(cartItem.Product, cartItem.Quantity);
                }
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }

        /* 
         * removed method because used model binding to bind the cart to http session,
         * thus removing the need to get it in the controller
        private Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }

            return cart;
        }
        */
    }
}