using System;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Models.Enums;

namespace BeerViewer.Models.Raw
{
	internal static class Extensions
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static int? Get(this int[] array, int index)
			=> (array == null || index < 0 || index >= array.Length)
				? (int?)null
				: array[index];

		public static bool IsHeavilyDamage(this LimitedValue value)
			=> (double)(value.Current - value.Minimum) / (value.Maximum - value.Minimum) <= 0.25;

		public static ConditionType ToConditionType(this int condition)
		{
			if (condition >= 50) return ConditionType.Brilliant;
			if (condition >= 40) return ConditionType.Normal;
			if (condition >= 30) return ConditionType.Tired;
			if (condition >= 20) return ConditionType.OrangeTired;
			return ConditionType.RedTired;
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> iterator)
		{
			foreach (var item in enumerable)
				iterator?.Invoke(item);
		}

		public static bool IsNumerable(this SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.CarrierBased_ReconPlane:
				case SlotItemType.CarrierBased_ReconPlane_II:
				case SlotItemType.CarrierBased_Fighter:
				case SlotItemType.CarrierBased_TorpedoBomber:
				case SlotItemType.CarrierBased_DiveBomber:
				case SlotItemType.ReconSeaplane:
				case SlotItemType.SeaplaneBomber:
				case SlotItemType.SeaplaneFighter:
				case SlotItemType.Autogyro:
				case SlotItemType.AntiSubmarinePatrolAircraft:
				case SlotItemType.LargeFlyingBoat:
				case SlotItemType.LandBased_Attacker:
				case SlotItemType.Interceptor_Fighter:
				case SlotItemType.JetPowered_Fighter:
				case SlotItemType.JetPowered_FighterBomber:
				case SlotItemType.JetPowered_Bomber:
				case SlotItemType.JetPowered_ReconPlane:
					return true;

				default:
					return false;
			}
		}

		public static IEnumerable<TResult> Return<TResult>(TResult value)
		{
			yield return value;
		}

		public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var set = new HashSet<TKey>(EqualityComparer<TKey>.Default);

			foreach (var item in source)
			{
				var key = keySelector(item);
				if (set.Add(key)) yield return item;
			}
		}

		public static bool HasItems<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}
	}
}
