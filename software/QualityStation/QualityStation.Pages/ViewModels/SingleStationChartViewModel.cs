using QualityStation.Shared.Pages.Services.RecordLoadingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.ViewModels
{
    public class SingleStationChartViewModel
    {
        private readonly RecordLoadingService m_recordLoadingService;

        public class DataItem<T>
        {
            public string Date { get; set; }
            public T Value { get; set; }
        }

        public SingleStationChartViewModel(RecordLoadingService recordLoadingService)
        {
            m_recordLoadingService = recordLoadingService; 
        }

        public async Task<string?> LoadRecordsById(string stationId)
        {
            var res = await m_recordLoadingService.LoadRecordsById(stationId);
            m_recordLoadingService.Records.Reverse();
            return res;
        }

        public List<DataItem<double>> GetDataFloat(string attributeName)
        {
            List<DataItem<double>> res = new List<DataItem<double>>();

            foreach (var record in m_recordLoadingService.Records)
            {
				double value = 0;
				try
				{
					value = Convert.ToDouble(record[attributeName].ToString());
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					value = 0;
				}

                res.Add(new DataItem<double>
                {
                    Date = (record["Created"].ToString())!,
					Value = value,
                });
            }

            return res;
        }

        public List<DataItem<int>> GetDataInteger(string attributeName)
        {
            List<DataItem<int>> res = new List<DataItem<int>>();

            foreach (var record in m_recordLoadingService.Records)
            {
                int value = 0;
                try
                {
                    value = Convert.ToInt32(record[attributeName].ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    value = 0;
                }

                res.Add(new DataItem<int>
                {
                    Date = (record["Created"].ToString())!,
                    Value = value,
                });
            }

            return res;
        }
    }
}
