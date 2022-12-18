using KotletkaShop.Models;
using System;
using System.Linq;
using static KotletkaShop.Models.CollectionCompareSubjects;
using static KotletkaShop.Models.CollectionConditions;
using static KotletkaShop.Models.DiscountApplicableObjectTypes;
using static KotletkaShop.Models.DiscountEligibleObjectTypes;
using static KotletkaShop.Models.DiscountMinimumRequirementTypes;
using static KotletkaShop.Models.DiscountTypes;
using static KotletkaShop.Models.SortingOrders;

namespace KotletkaShop.Data
{
    public class DbInitializer
    {
        public static void Initialize(StoreContext context)
        {
            _ = context.Database.EnsureCreated();

            // Поиск любого продукта
            if (context.Products.Any())
            {
                return;   // база данных уже существует и заполнена
            }

            // Инициализация тестовых изображений
            Image[] images =
            {
                new Image{Path="/Content/Images/placeholder.jpg",AltText="Заглушка зеленая"},
                new Image{Path="/Content/Images/placeholder-red.jpg",AltText="Заглушка красная"},
            };
            foreach (var i in images)
            {
                _ = context.Images.Add(i);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых коллекций
            Collection[] collections = {
                new Collection{Handle="Stickers_Collection",Title="Стикеры, наклейки, декали",Body="<p>Здесь все наклейки</p>",Published=true,CompareSubject1=ProductTag,Condition1=IsEqualTo,CompareTo1="sticker",ImageID=1,SortBy=A_Z},
                new Collection{Handle="CarParts_Collection",Title="Автозапчасти",Body="<p>Здесь все автозапчасти</p>",Published=false,CompareSubject1=ProductPrice,Condition1=IsLessThen,CompareTo1="260",ImageID=2,SortBy=Z_A}
            };
            foreach (var c in collections)
            {
                _ = context.Collections.Add(c);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых покупателей
            Customer[] customers = {
                new Customer{FirstName="Иван",MiddleName="Иванович",LastName="Иванов",Country="Россия",City="Иваново",Province="Ивановская область",District="Ивановский район",Building="1и",ZipCode=111000,PhoneNumber="+79990001122",Email="ivanov@mail.ru",Note="Первый клиент, юху!",AcceptsMarketing=false,RegisterDate=DateTime.Parse("2013-02-13"),ImageID=1,Tags="first, best"},
                new Customer{FirstName="Петр",MiddleName="Петрович",LastName="Петров",Country="Украина",City="Петрово",Province="Петровская область",District="Петровский район",Building="2п",ZipCode=222000,PhoneNumber="+79990002233",Email="petrov@mail.ru",Note="Второй клиент, meh",AcceptsMarketing=false,RegisterDate=DateTime.Parse("2013-02-14"),ImageID=2,Tags="second, meh"}
            };
            foreach (var c in customers)
            {
                _ = context.Customers.Add(c);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых типов товара
            ProductType[] productTypes = {
                new ProductType{Handle="Стикеры"},
                new ProductType{Handle="Запчасти"}
            };
            foreach (var p in productTypes)
            {
                _ = context.ProductTypes.Add(p);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых товаров
            Product[] products = {
                new Product{Handle="sticker_star",Title="Наклейка \"Звездочка\"",Body="<p>Лучшая наклейка в мире? Конечно! все любят звездочки!</p>",Vendor="Sticker-Shop",ProductTypeID=1,Tags="sticker, decal",Published=true,Option1Name="Размеры",Option1Value="2см, 3см, 4см",Weight=1,Quantity=-1,Price=250.0},
                new Product{Handle="bumper_carPart",Title="Запчасть \"Бампер\"",Body="<p>Лучшая Бампер в мире? Конечно! все любят звездбамперыочки!</p>",Vendor="Carpart-Shop",ProductTypeID=2,Tags="auto, engine, decal",Published=true,Option1Name="Цвета",Option1Value="Красный, Синий",Option2Name="Размеры",Option2Value="2см, 3см, 4см",Weight=15000,Quantity=18,Price=250.0},
                new Product{Handle="colenval",Title="Запчасть \"коленвал\"",Body="<p>Просто коленвал</p>",Vendor="Carpart-Shop",ProductTypeID=2,Tags="auto, bamper",Published=true,Weight=1,Quantity=-1,Price=250.0}
            };
            foreach (var p in products)
            {
                _ = context.Products.Add(p);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых изображений товара
            ProductImage[] productImages =  {
                new ProductImage {ProductID=1,ImageID=1,IsDefaultImage=true},
                new ProductImage {ProductID=2,ImageID=1,IsDefaultImage=true},
                new ProductImage {ProductID=2,ImageID=2},
                new ProductImage {ProductID=3,ImageID=1},
                new ProductImage {ProductID=3,ImageID=2,IsDefaultImage=true},
            };
            foreach (var p in productImages)
            {
                _ = context.ProductImages.Add(p);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых скидок
            Discount[] discounts = {
                new Discount{Handle="50_PERCENT_ALLPRODS_EVERYONE_MORETHAN1001",Type=Percentage,Value=0.50,AppliesTo=EntireOrder,MinimumRequirement=MinimumAmount,MinimumRequirementValue=1001,CustomerEligibility=Everyone,IsActive=true,TimesUsed=1,StartDate=DateTime.ParseExact("2019-02-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture),EndDate=DateTime.ParseExact("2019-05-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)},
                new Discount{Handle="50_RUBLE_1-3PRODS_2",Type=FixedAmount,Value=50,AppliesTo=SpecificProducts,ApplicableObjects="1, 3",MinimumRequirement=MinimumQuantity,MinimumRequirementValue=3,CustomerEligibility=SpecificCustomers,EligibleObjects="2",IsActive=true,TimesUsed=0,StartDate=DateTime.ParseExact("2019-04-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture),EndDate=DateTime.ParseExact("2100-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)},
                new Discount{Handle="FREESHIPPING__ALLPRODS_EVERYONE",Type=FreeShiping,AppliesTo=EntireOrder,MinimumRequirement=None,CustomerEligibility=Everyone,IsActive=true,TimesUsed=3,StartDate=DateTime.ParseExact("2019-02-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture),EndDate=DateTime.ParseExact("2019-05-01 00:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)}
            };
            foreach (var d in discounts)
            {
                _ = context.Discounts.Add(d);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых заказов
            Order[] orders = {
                new Order{CustomerID=1,DateCreated=DateTime.Parse("2019-01-01 00:01:01"),Paid=true,DatePaid=DateTime.Parse("2019-01-01 00:01:01"),Fulfilled=true,DateFulfilled=DateTime.Parse("2019-01-02"),Shipped=false,Canceled=false,ShippingCost=110,Note="Первый тестовый заказ",Draft=false},
                new Order{CustomerID=2,DateCreated=DateTime.Parse("2019-02-02 00:02:02"),Paid=true,DatePaid=DateTime.Parse("2019-01-02 00:02:12"),Fulfilled=true,DateFulfilled=DateTime.Parse("2019-01-03"),Shipped=false,Canceled=true,DateCanceled=DateTime.Parse("2019-02-03"),ShippingCost=220,Note="Второй тестовый заказ, отменен",Draft=false},
                new Order{CustomerID=1,DateCreated=DateTime.Parse("2019-03-03 00:03:03"),Paid=false,Fulfilled=false,Shipped=false,Canceled=false,ShippingCost=330,Note="Третий тестовый заказ, драфт",Draft=true}
            };
            foreach (var o in orders)
            {
                _ = context.Orders.Add(o);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых товаров заказа
            OrderProduct[] orderProducts = {
                new OrderProduct{OrderID=1,ProductID=1,Quantity=1},
                new OrderProduct{OrderID=1,ProductID=2,Quantity=3},
                new OrderProduct{OrderID=2,ProductID=1,Quantity=8},
                new OrderProduct{OrderID=3,ProductID=2,Quantity=12}
            };
            foreach (var p in orderProducts)
            {
                _ = context.OrderProducts.Add(p);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых скидок заказа
            OrderDiscount[] orderDiscounts = {
                new OrderDiscount{OrderID=1,DiscountID=1},
                new OrderDiscount{OrderID=2,DiscountID=2},
                new OrderDiscount{OrderID=2,DiscountID=3},
                new OrderDiscount{OrderID=3,DiscountID=1}
            };
            foreach (var d in orderDiscounts)
            {
                _ = context.OrderDiscounts.Add(d);
            }

            _ = context.SaveChanges();

            // Инициализация тестовых платежей
            Payment[] payments = {
                new Payment{CustomerID=1,OrderID=1,DatePaid=DateTime.Parse("2019-01-01 00:01:01"),Amount=500,Note="Платеж за первый заказ, первый платеж из 2"},
                new Payment{CustomerID=1,OrderID=1,DatePaid=DateTime.Parse("2019-01-01 00:01:01"),Amount=500,Note="Платеж за первый заказ, второй платеж из 2"},
                new Payment{CustomerID=2,OrderID=3,DatePaid=DateTime.Parse("2019-01-02 00:02:02"),Amount=2000,Note="Платеж за второй заказ"}
            };
            foreach (var p in payments)
            {
                _ = context.Payments.Add(p);
            }

            _ = context.SaveChanges();
        }
    }
}
