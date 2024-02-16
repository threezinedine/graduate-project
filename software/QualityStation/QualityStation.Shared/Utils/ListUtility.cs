using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Utils
{
	public static class ListUtility
	{
		public static T GetRandomValueFromAList<T>(List<T> list)
		{
			if (list == null || list.Count == 0) 
			{
				throw new ArgumentException("The list is either null or empty.");
			}

			Random random = new Random();
			int randomIndex = random.Next(0, list.Count);

			return list[randomIndex];
		}
	}
}
