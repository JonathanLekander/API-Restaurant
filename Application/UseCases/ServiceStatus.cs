using Application.DTOs.Response;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ServiceStatus : IServiceStatus
    {
        private readonly IStatusQuery _statusQuery;

        public ServiceStatus(IStatusQuery statusQuery)
        {
            _statusQuery = statusQuery;
        }
        public async Task<List<GenericResponse>> GetStatuses()
        {
            var statuses = await _statusQuery.GetListStatusAsync();

            var responseStatuses = new List<GenericResponse>();

            foreach (var status in statuses)
            {
                var responseStatus = new GenericResponse
                {
                    id = status.Id,
                    name = status.Name,
                };

                responseStatuses.Add(responseStatus);
            }
            return responseStatuses;

        }
    }
}
