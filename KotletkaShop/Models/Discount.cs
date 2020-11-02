using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KotletkaShop.Models
{
	// Типы скидки
	public enum DiscountTypes
	{
		Percentage = 1,
		FixedAmount,
		FreeShiping,
		BuyXGetY
	}

	// К чему может быть применена скидка: Весь заказ, конкретные товары,
	// конкретные коллекции
	public enum DiscountApplicableObjectTypes
	{
		EntireOrder = 1,
		SpecificProducts,
		SpecificCollections
	}

	// Кто может воспользоваться скидкой: Все, конкретные клиенты, конкретные
	// группы клиентов
	public enum DiscountEligibleObjectTypes
	{
		Everyone = 1,
		SpecificCustomers,
		SpecificGroupsOfCustomers
	}

	// Виды минимального уровня для активации скидки: Нет минимального значения,
	// необходимо минимальная сумма заказа, необходимо минимальное количество
	// товаров
	public enum DiscountMinimumRequirementTypes
	{
		None = 1,
		MinimumAmount,
		MinimumQuantity,
	}

	public class Discount
	{
		// Моедль EF
		public int DiscountID { get; set; }
		public string Handle { get; set; }
		public DiscountTypes Type { get; set; }
		public double Value { get; set; }
		public DiscountApplicableObjectTypes AppliesTo { get; set; }
		public string ApplicableObjects { get; set; }
		public DiscountMinimumRequirementTypes MinimumRequirement { get; set; }
		public double MinimumRequirementValue { get; set; }
		public DiscountEligibleObjectTypes CustomerEligibility { get; set; }
		public string EligibleObjects { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; } = DateTime.ParseExact("2100-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
		public bool IsActive { get; set; } = false;
		public int MaxTimesUsed { get; set; } = 0;
		public bool OneUsePerCustomer { get; set; } = false;
		public int TimesUsed { get; set; } = 0;

		[NotMapped]
		public List<Product> ApplicableProducts { get; set; }
		[NotMapped]
		public List<Collection> ApplicableCollections { get; set; }
		[NotMapped]
		public List<Customer> EligibleCustomers { get; set; }

		/// <summary>
		/// Вычленяет идентификаторы объектов, к которым применима скидка, из
		/// строки
		/// </summary>
		public List<int> ApplyableObjectsIDs
		{
			get
			{
				List<int> ids = new List<int>();

				if (ApplicableObjects != null)
				{
					ids = ApplicableObjects.Split(',').Select(int.Parse).ToList();
				}

				return ids;
			}
		}

		/// <summary>
		/// Вычленяет идентификаторы объектов, которым доступна скидка, из
		/// строки
		/// </summary>
		public List<int> EligibleObjectsIDs
		{
			get
			{
				List<int> ids = new List<int>();

				if (EligibleObjects != null)
				{
					ids = EligibleObjects.Split(',').Select(int.Parse).ToList();
				}

				return ids;
			}
		}

		/// <summary>
		/// Преобразует сумму скидки в проценты, в случае если тип скидки -
		/// процентный
		/// </summary>
		/// <returns></returns>
		public double ScreenValue()
		{
			if (Type == DiscountTypes.Percentage)
			{
				return Value * 100;
			}

			return Value;
		}

		/// <summary>
		/// Выводит краткую информацию о скидке в читаемом виде
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string text = Value + "";

			switch (Type)
			{
				case DiscountTypes.Percentage:
					text = (Value * 100) + "% ";
					break;
				case DiscountTypes.FixedAmount:
					text += " руб. ";  //LOCALIZE
					break;
				case DiscountTypes.FreeShiping:
					text = "Доставка бесплатно, "; //LOCALIZE
					break;
				case DiscountTypes.BuyXGetY:
					text = "Купи X получи Y, "; // Localize
					break;
				default:
					break;
			}

			switch (AppliesTo)
			{
				case DiscountApplicableObjectTypes.EntireOrder:
					text += "на весь заказ. "; //Localize
					break;
				case DiscountApplicableObjectTypes.SpecificProducts:
					text += "на " + ApplyableObjectsIDs.Count() + " товаров. "; //Localize
					break;
				case DiscountApplicableObjectTypes.SpecificCollections:
					text += "на " + ApplyableObjectsIDs.Count() + " коллекций. "; //Localize
					break;
				default:
					break;
			}

			switch (MinimumRequirement)
			{
				case DiscountMinimumRequirementTypes.None:
					break;
				case DiscountMinimumRequirementTypes.MinimumAmount:
					text += "Действует при заказе на сумму от " + MinimumRequirementValue + " руб. "; //Localize
					break;
				case DiscountMinimumRequirementTypes.MinimumQuantity:
					text += "Действует при количестве товаров в заказе от " + MinimumRequirementValue + " шт. "; //Localize
					break;
				default:
					break;
			}

			switch (CustomerEligibility)
			{
				case DiscountEligibleObjectTypes.Everyone:
					text += "Действует для всех. "; //Localize
					break;
				case DiscountEligibleObjectTypes.SpecificCustomers:
					text += "Действует для " + EligibleObjectsIDs.Count + " пользователей."; //Localize
					break;
				case DiscountEligibleObjectTypes.SpecificGroupsOfCustomers:
					text += "Действует для " + EligibleObjectsIDs.Count + " групп пользователей."; //Localize
					break;
				default:
					break;
			}

			return text;
		}

		/// <summary>
		/// Переводит типы скидки в читаемую форму
		/// </summary>
		/// <returns></returns>
		public string TypeToString()
		{
			switch (Type)
			{
				case DiscountTypes.Percentage:
					return "Процент"; //Localize
				case DiscountTypes.FixedAmount:
					return "Фиксированная сумма"; //Localize
				case DiscountTypes.FreeShiping:
					return "Бесплатная доставка"; //Localize
				case DiscountTypes.BuyXGetY:
					return "Купи X получи Y"; //Localize
				default:
					break;
			}

			return "";
		}

		/// <summary>
		/// Переводит виды объектов к которым применима скидка в читаемую
		/// форму
		/// </summary>
		/// <returns></returns>
		public string AppliesToToString()
		{
			switch (AppliesTo)
			{
				case DiscountApplicableObjectTypes.EntireOrder:
					return "Весь заказ"; //Localize
				case DiscountApplicableObjectTypes.SpecificProducts:
					return "Определенные товары"; //Localize
				case DiscountApplicableObjectTypes.SpecificCollections:
					return "Определенные коллекции"; //Localize
				default:
					break;
			}

			return "";
		}

		/// <summary>
		/// Переводит виды минимального количества для активации скидки в
		/// читаемую форму
		/// </summary>
		/// <returns></returns>
		public string MinimumRequirementToString()
		{
			switch (MinimumRequirement)
			{
				case DiscountMinimumRequirementTypes.None:
					return "нет"; //Localize
				case DiscountMinimumRequirementTypes.MinimumAmount:
					return "Сумма заказа"; //Localize
				case DiscountMinimumRequirementTypes.MinimumQuantity:
					return "Количество товаров в заказе"; //Localize
				default:
					break;
			}

			return "";
		}

		/// <summary>
		/// Переводит виды объектов, которым доступна скидка, в читаемую форму
		/// </summary>
		/// <returns></returns>
		public string CustomerEligibilityToString()
		{
			switch (CustomerEligibility)
			{
				case DiscountEligibleObjectTypes.Everyone:
					return "Для всех"; //Localize
				case DiscountEligibleObjectTypes.SpecificCustomers:
					return "Для определенных пользователей"; //Localize
				case DiscountEligibleObjectTypes.SpecificGroupsOfCustomers:
					return "Для определенных групп пользователей"; //Localize
				default:
					break;
			}

			return "";
		}
	}
}
