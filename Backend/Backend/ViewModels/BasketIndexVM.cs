﻿using Online_Shop___BackEnd.ViewModels.BasketViewModels;
namespace Backend.ViewModels
{
    public class BasketIndexVM
    {
        public BasketIndexVM()
        {
            BasketProducts = new List<BasketProductVM>();
        }

        public List<BasketProductVM> BasketProducts { get; set; }
    }
}