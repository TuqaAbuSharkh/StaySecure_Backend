using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.DTOs.Response.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IReportService
    {
        Task<List<TopicStatisticsResponse>>GetTopicStatisticsAsync();


        Task<AwarenessReportResponse> GetAwarenessReportAsync();

        Task<DailyTipResponse?>GetDailyTipAsync(string userId);


    }


}
