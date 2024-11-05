using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Supermarket superMarket = new Supermarket();
            superMarket.Work();
        }
    }

    class Supermarket
    {
        const string CommandExit = "1";

        private int _money = 0;
        private int _initialAmountClients = 5;
        private Queue<Buyer> _buyerQueue;
        private List<Product> _assortment;

        public Supermarket()
        {
            FillInListProducts();
            _buyerQueue = new Queue<Buyer>(_initialAmountClients);
            FillQueue();
        }

        public void Work()
        {
            string userInput;
            bool isDoesService = true;

            while (isDoesService)
            {
                Console.WriteLine($"Приключенцев в очереди {_buyerQueue.Count}\nВведите любую клавишу " +
                                    $"для обслуживания следующего приключенца.\nДля выхода введите {CommandExit}.");

                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandExit:
                        isDoesService = false;
                        Console.WriteLine("Завершение работы. Для продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                        break;

                    default:
                        ServeCustomers();
                        break;
                }
            }
        }

        private void FillQueue()
        {
            for (int i = 0; i < _initialAmountClients; i++)
            {
                List<Product> products = new List<Product>();
                products.AddRange(_assortment);
                Buyer buyer = new Buyer();
                buyer.FillCart(products);
                _buyerQueue.Enqueue(buyer);
            }
        }

        private void FillInListProducts()
        {
            _assortment = new List<Product>()
            {
                new Product("Зелье здоровья", 30),
                new Product("Зелье маны", 35),
                new Product("Ядовитая стрела", 13),
                new Product("Эльфийский лук", 750),
                new Product("Дворфийский ржавый меч", 350),
                new Product("Мантия", 250),
                new Product("Карта сокровищ", 120),
                new Product("Кусок пирога", 70),
                new Product("Ключ от подземелья", 450)
            };
        }

        private void ServeCustomers()
        {
            if (_buyerQueue.Count > 0)
            {
                Buyer buyer = _buyerQueue.Dequeue();

                Console.Clear();
                Console.WriteLine($"Приключенец хочет взять:");

                buyer.ShowContentsCart();
                buyer.VerifyIsEnoughMoney();

                MakeDeal(buyer);
                buyer.MakeDeal();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Приключенцев в лавке не осталось!\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private void TakeMoneyForGoods(int money)
        {
            _money += money;
        }

        private void MakeDeal(Buyer buyer)
        {
            int totalPriceProducts = buyer.GetTotalPriceProductsInCart();
            TakeMoneyForGoods(totalPriceProducts);

            Console.WriteLine($"\nПриключенец купил товар на сумму {totalPriceProducts} золотых.\n");
        }
    }

    class Buyer
    {
        private List<Product> _cart;
        private List<Product> _bag = new List<Product>();

        private int _money;
        private int _minimumMoney = 500;
        private int _maximumMoney = 1500;

        public Buyer()
        {
            _money = UserUtils.GenerateRandomNumber(_minimumMoney, _maximumMoney);
            _cart = new List<Product>();
        }

        public void FillCart(List<Product> products)
        {
            int amountProducts = 5;

            for (int i = 0; i < amountProducts; i++)
            {
                _cart.Add(products[UserUtils.GenerateRandomNumber(products.Count)]);
            }
        }

        public void MakeDeal()
        {
            PayForProduct(GetTotalPriceProductsInCart());
            MoveGoods();
        }

        public void ShowContentsCart()
        {
            for (int i = 0; i < _cart.Count; i++)
            {
                Console.WriteLine($"{_cart[i].Name}");
            }

            Console.WriteLine($"\nна сумму {GetTotalPriceProductsInCart()} золотых.");
        }

        public int GetTotalPriceProductsInCart()
        {
            int totalPrice = 0;

            for (int i = 0; i < _cart.Count; i++)
            {
                totalPrice += _cart[i].Price;
            }

            return totalPrice;
        }

        public void VerifyIsEnoughMoney()
        {
            while (_money < GetTotalPriceProductsInCart())
            {
                int indexProduct = UserUtils.GenerateRandomNumber(_cart.Count);
                Console.WriteLine($"\nУ приключенца не хватило золота и он отказывается от товара {_cart[indexProduct].Name}");
                RemoveProduct(indexProduct);
            }
        }

        private void PayForProduct(int money)
        {
            _money -= money;
        }

        private void RemoveProduct(int indexProduct)
        {
            _cart.RemoveAt(indexProduct);
        }

        private void MoveGoods()
        {
            _bag.AddRange(_cart);
            _cart.Clear();
        }
    }

    class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }

        public static int GenerateRandomNumber(int max)
        {
            return s_random.Next(max);
        }
    }
}
